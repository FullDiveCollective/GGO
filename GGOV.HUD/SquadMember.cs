﻿using GTA;
using LemonUI.Elements;
using System.Collections.Generic;
using System.Drawing;

namespace GGO
{
    /// <summary>
    /// UI Element that shows the information of a ped.
    /// </summary>
    public class SquadMember : Field
    {
        #region Fields

        private readonly ScaledText name = new ScaledText(PointF.Empty, "", 0.295f);
        private readonly ScaledRectangle health = new ScaledRectangle(PointF.Empty, SizeF.Empty);
        private readonly List<ScaledRectangle> separators = new List<ScaledRectangle>();

        #endregion

        #region Properties

        /// <summary>
        /// The ped that is getting the information fetched from.
        /// </summary>
        public Ped Ped { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new Squad Member HUD information for the specified ped.
        /// </summary>
        /// <param name="ped">The Ped to use as a base.</param>
        public SquadMember(Ped ped)
        {
            Ped = ped;

            if (ped.IsPlayer)
            {
                name.Text = Game.Player.Name;
            }

            for (int i = 0; i < 5; i++)
            {
                separators.Add(new ScaledRectangle(PointF.Empty, SizeF.Empty)
                {
                    Color = Color.FromArgb(250, 255, 255, 255)
                });
            }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Recalculates the position of the squad member's information.
        /// </summary>
        /// <param name="position">The new position of the squad member.</param>
        public override void Recalculate(PointF position)
        {
            base.Recalculate(position);

            infoBackground.Size = new SizeF(108, 50);

            name.Position = new PointF(infoBackground.Position.X + 4, infoBackground.Position.Y + 5);

            health.Position = new PointF(infoBackground.Position.X + 8, infoBackground.Position.Y + 34);
            health.Size = new SizeF(89, 4);

            for (int i = 0; i < separators.Count; i++)
            {
                const float width = 2;
                const float height = 8;
                ScaledRectangle separator = separators[i];
                separator.Size = new SizeF(width, height);
                separator.Position = new PointF(health.Position.X + (health.Size.Width / (separators.Count - 1) * i), health.Position.Y + (health.Size.Height * 0.5f) - (height * 0.5f));
            }
        }
        /// <summary>
        /// Processes the information of the squad member.
        /// </summary>
        public override void Process()
        {
            // Everyone knows that I'm against

            // Get the health percentage
            float percentage = (Ped.HealthFloat - 100) / (Ped.MaxHealthFloat - 100);
            // Make sure that is not under 0, over 1 or NaN
            if (percentage < 0)
            {
                percentage = 0;
            }
            else if (percentage > 1)
            {
                percentage = 1;
            }
            else if (float.IsNaN(percentage))
            {
                percentage = 0;
            }
            // And set the size of the health bar
            health.Size = new SizeF(89 * percentage, 4);

            // Then, just draw everything else
            base.Process();
            infoBackground.Draw();
            name.Draw();
            foreach (ScaledRectangle separator in separators)
            {
                separator.Draw();
            }
            health.Draw();
        }
        #endregion
    }
}
