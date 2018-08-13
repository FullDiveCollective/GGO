﻿using GGOHud.Tools;
using GTA;
using GTA.Native;
using System;
using System.Drawing;
using System.IO;

namespace GGOHud
{
    class Draw
    {
        /// <summary>
        /// Draws a text on the screen.
        /// </summary>
        /// <param name="Message">The message to draw.</param>
        /// <param name="Position">The position on the screen.</param>
        /// <param name="FontSize">The size of the font.</param>
        public static void Text(string Message, Point Position, float FontSize = 0.475f, bool Centered = true)
        {
            UIText ToDraw = new UIText(Message, Position, FontSize, Color.White, GTA.Font.ChaletLondon, Centered);
            ToDraw.Draw();
        }

        /// <summary>
        /// Draw a dummy text on the screen.
        /// A dummy text is a string that contains "-".
        /// </summary>
        /// <param name="Position">The position on the screen.</param>
        public static void Dummy(Point Position)
        {
            UIText ToDraw = new UIText("-", Position, 0.475f);
            ToDraw.Draw();
        }

        /// <summary>
        /// Draws a texture from a file on the disk.
        /// </summary>
        /// <param name="Filename">The image file to draw.</param>
        /// <param name="Position">The position on the screen.</param>
        /// <param name="DrawSize">The size of the image being drawn.</param>
        /// <param name="FailSafe">If the Draw should be ignored if the file does not exist.</param>
        public static void Image(string Filename, Point Position, Size DrawSize, bool FailSafe = false)
        {
            if (!File.Exists(Filename) && FailSafe)
                return;

            UI.DrawTexture(Filename, 0, 0, 200, Position, DrawSize);
        }

        /// <summary>
        /// Draws a rectangle on the screen.
        /// </summary>
        /// <param name="Position">The position on the screen.</param>
        /// <param name="DrawSize">The size of the rectangle being created.</param>
        /// <param name="ShapeColor">The color of the rectangle.</param>
        public static void Rectangle(Point Position, Size DrawSize, Color ShapeColor)
        {
            UIRectangle ToDraw = new UIRectangle(Position, DrawSize, ShapeColor);
            ToDraw.Draw();
        }

        public static void HealthBar(Point Position, Size DrawSize, Entity DestinationEntity)
        {
            // Calculate the bar size
            int Health = Function.Call<int>(Hash.GET_ENTITY_HEALTH, DestinationEntity) - 100;
            int MaxHealth = Function.Call<int>(Hash.GET_ENTITY_MAX_HEALTH, DestinationEntity) - 100;
            int HealthPercentage = Convert.ToInt32(((float)Health / MaxHealth) * 100f);
            float Size = (DrawSize.Width / 100f) * HealthPercentage;

            // Store the size of the bar in a new object
            DrawSize.Width = Convert.ToInt32(Size);

            // Then, draw the bar by itself
            Rectangle(Position, DrawSize, Colors.FromHealth(MaxHealth, HealthPercentage));
        }
    }
}
