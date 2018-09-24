﻿using GTA;
using System;
using System.Drawing;

namespace GGO.Common
{
    public class Draw
    {
        /// <summary>
        /// Color for the backgrounds of the items.
        /// </summary>
        public static Color CBackground = Color.FromArgb(175, 0, 0, 0);
        /// <summary>
        /// Color for the dividers of the health bar.
        /// </summary>
        public static Color CDivider = Color.FromArgb(125, 230, 230, 230);

        /// <summary>
        /// Draws an icon with it's respective background.
        /// </summary>
        public static void Icon(Configuration Config, string ImageFile, Point Position)
        {
            // Draw the rectangle on the background
            UIRectangle Rect = new UIRectangle(Position, Config.IconBackgroundSize, CBackground);
            Rect.Draw();

            // Calculate the position of the image
            Point ImagePos = Position + Config.IconPosition;
            // And finally, add the image on top
            UI.DrawTexture(ImageFile, 0, 0, 200, ImagePos, Config.IconImageSize);
        }

        /// <summary>
        /// Draws the complete information of a ped. That includes name and health.
        /// </summary>
        /// <param name="Character">The ped to get the information.</param>
        /// <param name="Position">The position on the screen.</param>
        /// <param name="TotalSize">The full size of the information field.</param>
        public static void PedInfo(Configuration Config, Ped Character, Point Position)
        {
            // First, draw the black background
            UIRectangle Background = new UIRectangle(Position, Config.SquadInfoSize, CBackground);
            Background.Draw();

            // Then, calculate the health bar: (Percentage / 100) * DefaultWidth
            float Width = (Character.HealthPercentage() / 100) * Config.SquadHealthSize.Width;
            // Create a Size with the required size
            Size NewHealthSize = new Size(Convert.ToInt32(Width), Config.SquadHealthSize.Height);

            // For the dividers, get the distance between each one of them
            int HealthSep = Config.SquadHealthSize.Width / 4;

            // Prior to drawing the health bar we need the separators
            for (int Count = 0; Count < 5; Count++)
            {
                // Calculate the position of the separator
                Point Pos = (Position + Config.SquadHealthPos) + new Size(HealthSep * Count, 0) + Config.DividerPosition;
                // And draw it on screen
                UIRectangle Divider = new UIRectangle(Pos, Config.DividerSize, CDivider);
                Divider.Draw();
            }

            // After the separators are there, draw the health bar on the top
            UIRectangle HealthBar = new UIRectangle(Position + Config.SquadHealthPos, NewHealthSize, Character.HealthColor());
            HealthBar.Draw();

            // And finally, draw the ped name
            UIText Name = new UIText(Character.Name(Config.Name), Position + Config.NamePosition, 0.3f);
            Name.Draw();
        }
    }
}
