﻿using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;

namespace GGO
{
    public class Configuration
    {
        /// <summary>
        /// If the Debug window should be shown.
        /// </summary>
        public bool Debug => (bool)Raw["debug"] || Environment.GetEnvironmentVariable("LemonDev", EnvironmentVariableTarget.User) == "true";
        /// <summary>
        /// The name for the current player.
        /// </summary>
        public string Name => (string)Raw["name"];
        /// <summary>
        /// If the parts of the vanilla HUD that collide with GGO should be disabled.
        /// </summary>
        public bool DisableHud => (bool)Raw["disable_hud"];
        /// <summary>
        /// If the GTA Radar should be disabled/hidden
        /// </summary>
        public bool DisableRadar => (bool)Raw["disable_radar"];


        /// <summary>
        /// If the dead markers should be shown.
        /// </summary>
        public bool DeadMarkers => (bool)Raw["dead_markers"];
        /// <summary>
        /// If the vehicle information should be shown.
        /// </summary>
        public bool VehicleInfo => (bool)Raw["vehicle_info"];
        /// <summary>
        /// If the squad members should be shown.
        /// </summary>
        public bool SquadMembers => (bool)Raw["squad_members"];


        /// <summary>
        /// Separation between the UI elements.
        /// </summary>
        public Size CommonSpacing => CreateSize("common_spacing");
        /// <summary>
        /// Size for the squared backgrounds.
        /// </summary>
        public Size SquaredBackground => CreateSize("squared_background");


        /// <summary>
        /// Size for the icons.
        /// </summary>
        public Size IconSize => CreateSize("icon_size");
        /// <summary>
        /// Position of the image relative to the background.
        /// </summary>
        public Size IconPosition => CreateSize("icon_position");


        /// <summary>
        /// Position of the squad information.
        /// </summary>
        public Point SquadPosition => new Point(CreateSize("squad_position"));
        /// <summary>
        /// Position of the name relative to the background.
        /// </summary>
        public Size NamePosition => CreateSize("name_position");
        /// <summary>
        /// Size for the squad information.
        /// </summary>
        public Size SquadSize => CreateSize("squad_size");

        /// <summary>
        /// The position of the inventory background
        /// </summary>
        public Point InventoryBackgroundPosition => new Point(CreateSize("inv_background_position"));
        /// <summary>
        /// The position of the coloured tag
        /// </summary>
        public Point InventoryColourPosition => new Point(CreateSize("inv_colour_position"));
        /// <summary>
        /// The position of the player name on the inventory
        /// </summary>
        public Point InventoryPlayerName => new Point(CreateSize("inv_playername_position"));
        /// <summary>
        /// The size of the inventory background
        /// </summary>
        public Size InventoryBackgroundSize => CreateSize("inv_background_size");
        /// <summary>
        /// The size for the information space
        /// </summary>
        public Size InventoryInfoSize => CreateSize("inv_info_size");
        /// <summary>
        /// The size for the colour space
        /// </summary>
        public Size InventoryColourSize => CreateSize("inv_coloured_size");
        /// <summary>
        /// The size of the inventory weapons.
        /// </summary>
        public Size InventoryWeaponSize => CreateSize("inv_weapon_size");
        /// <summary>
        /// The inventory position of the weapons.
        /// </summary>
        public Point InventoryWeaponPosition => new Point(CreateSize("inv_weapon_position"));
        /// <summary>
        /// The separation between inventory icons.
        /// </summary>
        public Size InventoryWeaponSeparation => CreateSize("inv_weapon_separation");
        /// <summary>
        /// The inventory position of the gender icon.
        /// </summary>
        public Point InventoryGender => new Point(CreateSize("inv_gender_position"));
        /// <summary>
        /// The offset of the inventory rectangles.
        /// </summary>
        public Size InventoryRectangleOffset => CreateSize("inv_rectangle_offset");
        /// <summary>
        /// The size of the inventory rectangles.
        /// </summary>
        public Size InventoryRectangleSize => CreateSize("inv_rectangle_size");
        /// <summary>
        /// The position of the "Life" text.
        /// </summary>
        public Point InventoryLifePosition => new Point(CreateSize("inv_life_position"));
        /// <summary>
        /// The position of the health bar on the inventory.
        /// </summary>
        public Point InventoryHealthPosition => new Point(CreateSize("inv_health_position"));
        /// <summary>
        /// The size of the health bar on the inventory.
        /// </summary>
        public Size InventoryHealthSize => CreateSize("inv_health_size");


        /// <summary>
        /// The position of the player information.
        /// </summary>
        public Point PlayerPosition => new Point(CreateSize("player_position"));
        /// <summary>
        /// Size of the player information.
        /// </summary>
        public Size PlayerSize => CreateSize("player_size");
        /// <summary>
        /// Offset of the ammo.
        /// </summary>
        public Point AmmoOffset => new Point(CreateSize("ammo_offset"));
        /// <summary>
        /// Position of the weapon images relative to the background.
        /// </summary>
        public Size WeaponPosition => CreateSize("weapon_position");
        /// <summary>
        /// Size for the weapon images.
        /// </summary>
        public Size WeaponSize => CreateSize("weapon_size");
        /// <summary>
        /// The size of the weapon background
        /// </summary>
        public Size WeaponBackground => new Size(PlayerSize.Width - CommonSpacing.Width - SquaredBackground.Width, PlayerSize.Height);
        /// <summary>
        /// The position of the player information.
        /// </summary>
        public Point PlayerInformation => new Point(PlayerPosition.X + SquaredBackground.Width + CommonSpacing.Width, PlayerPosition.Y);
        /// <summary>
        /// The position of the icon for the primary weapon.
        /// </summary>
        public Point PrimaryIcon => new Point(PlayerPosition.X, PlayerPosition.Y + CommonSpacing.Height + SquaredBackground.Height);
        /// <summary>
        /// The position of the icon for the primary weapon.
        /// </summary>
        public Point SecondaryIcon => new Point(PlayerPosition.X, PlayerPosition.Y + (CommonSpacing.Height * 2) + (SquaredBackground.Height * 2));
        /// <summary>
        /// The position of the ammo for the primary weapon.
        /// </summary>
        public Point PrimaryBackground => new Point(PrimaryIcon.X + SquaredBackground.Width + CommonSpacing.Width, PrimaryIcon.Y);
        /// <summary>
        /// The position of the ammo for the secondary weapon.
        /// </summary>
        public Point SecondaryBackground => new Point(SecondaryIcon.X + SquaredBackground.Width + CommonSpacing.Width, SecondaryIcon.Y);
        /// <summary>
        /// The position of the primary ammo counter.
        /// </summary>
        public Point PrimaryAmmo => new Point(PrimaryBackground.X + AmmoOffset.X, PrimaryBackground.Y + AmmoOffset.Y);
        /// <summary>
        /// The position of the secondary ammo counter.
        /// </summary>
        public Point SecondaryAmmo => new Point(SecondaryBackground.X + AmmoOffset.X, SecondaryBackground.Y + AmmoOffset.Y);
        /// <summary>
        /// The position of the primary weapon background.
        /// </summary>
        public Point PrimaryWeapon => new Point(PrimaryBackground.X + CommonSpacing.Width + SquaredBackground.Width, PrimaryBackground.Y);
        /// <summary>
        /// The position of the secondary weapon background.
        /// </summary>
        public Point SecondaryWeapon => new Point(SecondaryBackground.X + CommonSpacing.Width + SquaredBackground.Width, SecondaryBackground.Y);
        /// <summary>
        /// The position of the vehicle information.
        /// </summary>
        public Point VehicleIcon => new Point(PlayerPosition.X, PlayerPosition.Y - SquaredBackground.Width - CommonSpacing.Width);
        /// <summary>
        /// The position of the vehicle information.
        /// </summary>
        public Point VehicleInformation => new Point(VehicleIcon.X + SquaredBackground.Width + CommonSpacing.Width, VehicleIcon.Y);

        /// <summary>
        /// Size for the dividers on the health bars.
        /// </summary>
        public Size DividerSize => CreateSize("divider_size");
        /// <summary>
        /// Position of the health dividers.
        /// </summary>
        public Size DividerPosition => CreateSize("divider_position");
        /// <summary>
        /// Size for the squad health bars.
        /// </summary>
        public Size SquadHealthSize => CreateSize("squad_health_size");
        /// <summary>
        /// Position of the squad health bar.
        /// </summary>
        public Size SquadHealthPos => CreateSize("squad_health_position");
        /// <summary>
        /// Size of the player health bar.
        /// </summary>
        public Size PlayerHealthSize => CreateSize("player_health_size");
        /// <summary>
        /// Position of the player health bar.
        /// </summary>
        public Size PlayerHealthPos => CreateSize("player_health_position");
        
        /// <summary>
        /// Size for the health markers.
        /// </summary>
        public Size DeadMarker => CreateSize("dead_marker");

        /// <summary>
        /// The current screen resolution.
        /// </summary>
        private Size Resolution { get; set; }
        /// <summary>
        /// The RAW Configuration.
        /// </summary>
        public JObject Raw { get; set; }
        /// <summary>
        /// The inventory configuration.
        /// </summary>
        public JObject Inventory { get; set; }

        /// <summary>
        /// Loads up the configuration from "GGO.Shared.json"
        /// </summary>
        public Configuration(string Location, Size CurrentResolution)
        {
            // Read all of the text that is on the files
            string HudConfig = File.ReadAllText(Location + "\\GGO.Hud.json");
            string InvConfig = File.ReadAllText(Location + "\\GGO.Inventory.json");
            // Load it on the parsers
            Raw = JObject.Parse(HudConfig);
            Inventory = JObject.Parse(InvConfig);
            // And store our current resolution
            Resolution = CurrentResolution;
        }

        /// <summary>
        /// Creates a Size from a JSON array.
        /// </summary>
        /// <returns>The working Size.</returns>
        private Size CreateSize(string ConfigOption)
        {
            return new Size((int)(Resolution.Width * (float)Raw[ConfigOption][0]), (int)(Resolution.Height * (float)Raw[ConfigOption][1]));
        }
    }
}
