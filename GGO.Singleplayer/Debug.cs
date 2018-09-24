﻿using GGO.Common;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace GGO.Singleplayer
{
    public partial class Debug : Form
    {
        public Debug(Configuration Config)
        {
            InitializeComponent();

            // Get the current GGOHud version
            Version Current = Assembly.GetExecutingAssembly().GetName().Version;

            // Set the icon as the GTA V executable
            Icon GameIcon = Icon.ExtractAssociatedIcon("GTA5.exe");
            Icon = GameIcon;

            // And then set the corresponding texts on the fields
            Version.Text = string.Format("Running GGOHud v{0}", Current);
            PlayerName.Text = string.Format("Custom name: {0}", Config.Name);
            HudDisabled.Text = string.Format("HUD Disabled: {0}", Config.DisableHud);

            ElementsRelative.Text = string.Format("elements_relative: {0}", Config.ElementsRelative);
            DividerSize.Text = string.Format("divider_size: {0}", Config.DividerSize);
            DividerPosition.Text = string.Format("divider_pos: {0}", Config.DividerPosition);
            NamePosition.Text = string.Format("name_pos: {0}", Config.NamePosition);

            IconImageSize.Text = string.Format("icon_image_size: {0}", Config.IconImageSize);
            IconBackgroundSize.Text = string.Format("icon_background_size: {0}", Config.IconBackgroundSize);
            IconPosition.Text = string.Format("icon_relative_pos: {0}", Config.IconPosition);

            SquadPosition.Text = string.Format("squad_general_pos: {0}", Config.SquadPosition);
            SquadInfoSize.Text = string.Format("squad_info_size: {0}", Config.SquadInfoSize);
            SquadHealthSize.Text = string.Format("squad_health_size: {0}", Config.SquadHealthSize);
            SquadHealthPos.Text = string.Format("squad_health_pos: {0}", Config.SquadHealthPos);
        }
    }
}
