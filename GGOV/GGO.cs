﻿using GGO.Converters;
using GGO.HUD;
using GGO.Inventory;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using LemonUI;
using LemonUI.Elements;
using LemonUI.Extensions;
using LemonUI.Menus;
using LemonUI.Scaleform;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace GGO
{
    /// <summary>
    /// Main script for the entire GGO HUD System.
    /// </summary>
    public class GGO : Script
    {
        #region Fields

        /// <summary>
        /// The location of this script.
        /// </summary>
        private readonly string location = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
        /// <summary>
        /// The Pool that holds all of the Processable items.
        /// </summary>
        internal static readonly ObjectPool pool = new ObjectPool();

        /// <summary>
        /// The main configuration menu.
        /// </summary>
        internal static readonly SettingsMenu menu = new SettingsMenu();
        /// <summary>
        /// The currently active HUD Preset.
        /// </summary>
        internal static Preset selectedPreset = null;

        /// <summary>
        /// The inventory of the user.
        /// </summary>
        internal static readonly PlayerInventory inventory = new PlayerInventory();

        /// <summary>
        /// The currently active Death Markers.
        /// </summary>
        private readonly Dictionary<Ped, ScaledTexture> markers = new Dictionary<Ped, ScaledTexture>();
        /// <summary>
        /// The next time for the Marker update.
        /// </summary>
        private int nextMarkerUpdate = 0;

        /// <summary>
        /// The next time for updating the Squad Members.
        /// </summary>
        private int nextSquadUpdate = 0;

        #endregion

        #region Properties

        /// <summary>
        /// The names for the Ped Models.
        /// </summary>
        public static Dictionary<Model, string> Names { get; private set; } = new Dictionary<Model, string>();
        /// <summary>
        /// The Squad members panel.
        /// </summary>
        public static SquadMembers Squad { get; } = new SquadMembers();
        /// <summary>
        /// The fields with the information of the player.
        /// </summary>
        public static PlayerFields Player { get; } = new PlayerFields();

        #endregion

        #region Constructor

        public GGO()
        {
            // Look for the names in GGOV/Names and load them
            foreach (string file in Directory.EnumerateFiles(Path.Combine(location, "GGOV", "Names")))
            {
                // If the file is not JSON, warn about it and skip it
                if (Path.GetExtension(file) != ".json")
                {
                    Notification.Show($"~o~Warning~s~: Non JSON file found in Names Directory! ({Path.GetFileName(file)})");
                    continue;
                }

                // Otherwise, try to load it
                string contents = File.ReadAllText(file);
                // And then to parse it
                Dictionary<string, string> loaded;
                // If we failed, notify the user and continue
                try
                {
                    loaded = JsonConvert.DeserializeObject<Dictionary<string, string>>(contents);
                }
                catch (JsonSerializationException e)
                {
                    Notification.Show($"~r~Error~s~:Unable to load {Path.GetFileName(file)}: {e.Message}");
                    continue;
                }

                // Otherwise, add the names onto the list
                foreach (KeyValuePair<string, string> pair in loaded)
                {
                    // Convert the model from a number or a string
                    Model model;
                    if (int.TryParse(pair.Key, out int number))
                    {
                        model = new Model(number);
                    }
                    else
                    {
                        model = new Model(pair.Key);
                    }

                    // If the key is already on the dictionary, warn the user
                    if (Names.ContainsKey(model))
                    {
                        Notification.Show($"~o~Warning~s~: Model {pair.Key} ({model.Hash}) has more than one name!");
                    }
                    // Then, just add it to the list
                    Names[model] = pair.Value;
                }
            }

            // Build the menus
            menu.Presets.Buttons.Add(new InstructionalButton("Create New", Control.FrontendX));
            menu.Presets.Buttons.Add(new InstructionalButton("Save Presets", Control.FrontendY));
            // Add the UI elements into the pool
            pool.Add(menu);
            pool.Add(menu.Presets);
            pool.Add(inventory);
            pool.Add(Squad);
            pool.Add(Player);
            // And add the tick event
            Tick += HUD_Tick;

            // Once everything is loaded, load the presets if they are present
            if (File.Exists("scripts\\GGOV\\HUDPresets.json"))
            {
                string contents = File.ReadAllText("scripts\\GGOV\\HUDPresets.json");
                List<Preset> foundPresets = JsonConvert.DeserializeObject<List<Preset>>(contents, new PresetConverter());
                foreach (Preset preset in foundPresets)
                {
                    pool.Add(preset);
                    menu.Presets.AddSubMenu(preset);
                }
            }
        }

        #endregion

        #region Events

        private void HUD_Tick(object sender, EventArgs e)
        {
            // Make the inventory visible based on the button pressed
            Game.DisableControlThisFrame(Control.SelectWeapon);
            if (Game.IsControlJustPressed(Control.SelectWeapon))
            {
                inventory.Visible = !inventory.Visible;
            }

            // If a Ped update is required and we are not in a cutscene
            if ((nextSquadUpdate <= Game.GameTime || nextSquadUpdate == 0) && !Game.IsCutsceneActive)
            {
                // Iterate over the peds in the whole game world
                foreach (Ped ped in World.GetAllPeds())
                {
                    bool isFriend = Game.Player.Character.GetRelationshipWithPed(ped) <= Relationship.Like && ped.GetRelationshipWithPed(Game.Player.Character) <= Relationship.Like;
                    bool sameGroup = Game.Player.Character.PedGroup == ped.PedGroup;
                    bool groupLeader = ped.PedGroup?.Leader == Game.Player.Character;

                    // If the ped is a friend or is part of the player's group, is not the player, and is not part of the squad, add it
                    if ((isFriend || sameGroup || groupLeader) && ped != Game.Player.Character && !Squad.Contains(ped))
                    {
                        Squad.Add(new PedHealth(ped));
                    }
                }

                // Finally, set the new update time
                nextSquadUpdate = Game.GameTime + 1000;
            }

            // If the user entered ggohudconfig in the cheat input, open the menu
            if (Game.WasCheatStringJustEntered("ggo"))
            {
                menu.Visible = true;
            }

            // If the presets menu is visible, disable the controls that collide with X/Space and Y/Square
            if (menu.Presets.Visible)
            {
                DisableControlCollisions();
            }

            // Just process the HUD Elements
            pool.Process();

            // If the presets menu is still visible
            if (menu.Presets.Visible)
            {
                // Disable the colliding controls again
                DisableControlCollisions();
                // If the user pressed X/Square/Space
                if (Game.IsControlJustPressed(Control.FrontendX))
                {
                    // Ask the user for the name
                    menu.Presets.Visible = false;
                    string input = Game.GetUserInput(WindowTitle.EnterMessage60, "", 60);
                    menu.Presets.Visible = true;
                    // If the user didn't entered anything, return
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Notification.Show("~r~Error~s~: The Preset name is empty, is only whitespaces or it was cancelled.");
                        return;
                    }
                    // Otherwise, create a new preset
                    Preset newMenu = new Preset(input);
                    menu.Presets.AddSubMenu(newMenu);
                    pool.Add(newMenu);
                }
                // If the user pressed Y/Triangle/Tab
                else if (Game.IsControlJustPressed(Control.FrontendY))
                {
                    // Get all of the presets in a list
                    List<Preset> fields = new List<Preset>();
                    pool.ForEach<Preset>(x => fields.Add(x));
                    // Convert them to JSON
                    string json = JsonConvert.SerializeObject(fields, new PresetConverter());
                    // And write them to a file
                    Directory.CreateDirectory("scripts\\GGOV");
                    File.WriteAllText("scripts\\GGOV\\HUDPresets.json", json);

                    Notification.Show("The Presets have been ~g~Saved~s~!");
                }
            }

            // Finally, update the markers
            UpdateMarkers();
        }

        private void UpdateMarkers()
        {
            // If is time for the next update or is the first update
            if (nextMarkerUpdate <= Game.GameTime || nextMarkerUpdate == 0)
            {
                // Iterate the the peds on the game world
                foreach (Ped ped in World.GetAllPeds())
                {
                    // If the ped is dead and is not part of the markers, add it
                    if (ped.IsDead && !markers.ContainsKey(ped))
                    {
                        markers.Add(ped, new ScaledTexture(PointF.Empty, new SizeF(220 * 0.75f, 124 * 0.75f), "ggo", "marker_dead"));
                    }
                }

                // Finally, set the new update time
                nextMarkerUpdate = Game.GameTime + 500;
            }

            // Iterate over the existing items
            // (creating the new dictionary is required to prevent the "collection was edited" exception)
            foreach (KeyValuePair<Ped, ScaledTexture> marker in new Dictionary<Ped, ScaledTexture>(markers))
            {
                Ped ped = marker.Key;

                // If the ped is no longer present in the game world, remove it and continue
                if (!ped.Exists())
                {
                    markers.Remove(ped);
                    continue;
                }

                // If the ped is not on the screen, skip it
                if (!ped.IsOnScreen)
                {
                    continue;
                }

                // Get the position of the ped head
                Vector3 headPos = ped.Bones[Bone.SkelHead].Position;

                // And then conver it to screen coordinates
                OutputArgument originalX = new OutputArgument();
                OutputArgument originalY = new OutputArgument();
                bool ok = Function.Call<bool>(Hash.GET_SCREEN_COORD_FROM_WORLD_COORD, headPos.X, headPos.Y, headPos.Z, originalX, originalY);

                // If it was unable to get the position, continue
                if (!ok)
                {
                    continue;
                }

                // Otherwise, convert the position from relative to absolute
                PointF screenPos = new PointF(originalX.GetResult<float>(), originalY.GetResult<float>()).ToAbsolute();
                // And set it for the correct
                marker.Value.Position = new PointF(screenPos.X - (marker.Value.Size.Width * 0.5f), screenPos.Y - marker.Value.Size.Height);

                // Finally, draw it
                marker.Value.Draw();
            }
        }

        private void DisableControlCollisions()
        {
            Game.DisableControlThisFrame(Control.VehicleExit);
            Game.DisableControlThisFrame(Control.Jump);
        }

        #endregion
    }
}
