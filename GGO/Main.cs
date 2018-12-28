using GGO.Properties;
using GTA;
using GTA.Native;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace GGO
{
    public class GGO : Script
    {
        /// <summary>
        /// Our configuration parameters.
        /// </summary>
        public static Configuration Config = new Configuration("scripts", new Size(UI.WIDTH, UI.HEIGHT));

        public GGO()
        {
            // Notify that we are starting the script
            Logging.Info("===== GGOV for SHVDN is booting up... =====");

            // Add our Tick and Aborted events
            Tick += OnTick;
            Aborted += OnAbort;

            // If the debug mode is enabled
            if (Config.Debug)
            {
                // Set the logging level to debug
                Logging.CurrentLevel = Logging.Level.Debug;
                // And print the configuration values
                Logging.Debug("Configuration Values:");

                foreach (PropertyDescriptor Descriptor in TypeDescriptor.GetProperties(Config))
                {
                    Logging.Debug(Descriptor.Name + ": " + Descriptor.GetValue(Config).ToString());
                }
            }

            // Notify that the script has started
            Logging.Info("GGOV for SHVDN is up and running");
        }

        private void OnTick(object Sender, EventArgs Args)
        {
            // Don't draw the UI if the game is loading, paused, player is dead or it cannot be controlled
            if (Game.IsLoading || Game.IsPaused || !Game.Player.Character.IsAlive || !Game.Player.CanControlCharacter)
            {
                return;
            }

            // Disable the weapons menu
            Game.DisableControlThisFrame(0, Control.SelectWeapon);
            // If the user just pressed TAB/L1/LB, center the cursor
            if (Game.IsDisabledControlJustPressed(0, Control.SelectWeapon))
            {
                bool OK = Function.Call<bool>(Hash._0xFC695459D4D0E219, 0.5f, 0.5f); // _SET_CURSOR_POSTION
                if (!OK)
                {
                    Logging.Error("Unable to set the cursor on the center of the screen.");
                }
            }
            // Draw the inventory if the player tried to open the weapon selector
            if (Game.IsDisabledControlPressed(0, Control.SelectWeapon))
            {
                Inventory.Draw();
                Function.Call(Hash._SHOW_CURSOR_THIS_FRAME);
            }

            // Reset the index of the images
            Toolkit.ResetIndex();

            // Disable the colliding HUD elements by default
            if (Config.DisableHud)
            {
                UI.HideHudComponentThisFrame(HudComponent.WeaponIcon);
                UI.HideHudComponentThisFrame(HudComponent.AreaName);
                UI.HideHudComponentThisFrame(HudComponent.StreetName);
                UI.HideHudComponentThisFrame(HudComponent.VehicleName);
                UI.HideHudComponentThisFrame(HudComponent.HelpText);
            }

            // If the user wants to disable the Radar and is not hidden, do it now
            if (Config.DisableRadar && !Function.Call<bool>(Hash.IS_RADAR_HIDDEN))
            {
                Function.Call(Hash.DISPLAY_RADAR, false);
            }

            // Get all of the peds and store them during this tick
            Ped[] NearbyPeds = World.GetAllPeds().OrderBy(P => P.GetHashCode()).ToArray();
            
            // Draw the squad information on the top left if the user wants to
            if (Config.SquadMembers)
            {
                // Store the peds that are friend of us
                Ped[] FriendlyPeds = NearbyPeds.Where(P => P.IsFriendly() && P.IsMissionEntity()).ToArray();

                // And iterate over them
                foreach (Ped SquadMember in FriendlyPeds)
                {
                    // Get the number of the ped
                    int Number = Array.IndexOf(FriendlyPeds, SquadMember);

                    // Select the correct image and name for the file
                    Bitmap ImageType = SquadMember.IsAlive ? Resources.IconAlive : Resources.IconDead;
                    string ImageName = SquadMember.IsAlive ? nameof(Resources.IconAlive) : nameof(Resources.IconDead);

                    // Draw the icon and the ped info
                    Toolkit.Icon(ImageType, ImageName, Calculations.GetSquadPosition(Config, Number));
                    Toolkit.EntityInfo(SquadMember, true, Number);
                }
            }

            // Draw the dead ped markers over their heads, if the user wants to
            if (Config.DeadMarkers)
            {
                // Iterate over the dead peds
                foreach (Ped DeadPed in NearbyPeds.Where(P => P.IsDead && P.IsOnScreen).ToArray())
                {
                    // And draw the dead marker
                    Toolkit.DeadMarker(DeadPed);
                }
            }

            // Then, start by drawing the player info
            Toolkit.Icon(Resources.IconAlive, nameof(Resources.IconAlive), Config.PlayerPosition);
            Toolkit.EntityInfo(Game.Player.Character);

            // If the player is on a vehicle, also draw that information
            if (Game.Player.Character.CurrentVehicle != null && Config.VehicleInfo)
            {
                Toolkit.Icon(Resources.IconVehicle, nameof(Resources.IconVehicle), Config.VehicleIcon);
                Toolkit.EntityInfo(Game.Player.Character.CurrentVehicle);
            }

            // Get the current weapon style
            WeaponStyle CurrentStyle = Game.Player.Character.Weapons.GetStyle();

            // And draw the weapon information for both the primary and secondary
            // If they are not available, draw dummies instead
            if (CurrentStyle == WeaponStyle.Main || CurrentStyle == WeaponStyle.Double)
            {
                Toolkit.Icon(Resources.IconWeapon, nameof(Resources.IconWeapon), Config.PrimaryIcon);
                Toolkit.WeaponInfo(Game.Player.Character.Weapons.Current, CurrentStyle);
            }
            else
            {
                Toolkit.Icon(Resources.NoWeapon, nameof(Resources.NoWeapon), Config.PrimaryIcon);
                Toolkit.Icon(Resources.NoWeapon, nameof(Resources.NoWeapon), Config.PrimaryBackground);
            }
            if (CurrentStyle == WeaponStyle.Sidearm || CurrentStyle == WeaponStyle.Double)
            {
                Toolkit.Icon(Resources.IconWeapon, nameof(Resources.IconWeapon), Config.SecondaryIcon);
                Toolkit.WeaponInfo(Game.Player.Character.Weapons.Current, CurrentStyle);
            }
            else
            {
                Toolkit.Icon(Resources.NoWeapon, nameof(Resources.NoWeapon), Config.SecondaryIcon);
                Toolkit.Icon(Resources.NoWeapon, nameof(Resources.NoWeapon), Config.SecondaryBackground);
            }
        }

        public static void OnAbort(object Sender, EventArgs Args)
        {
            // Reset the Radar state to enabled (just if the script is aborted but not started again)
            Function.Call(Hash.DISPLAY_RADAR, true);
        }
    }
}
