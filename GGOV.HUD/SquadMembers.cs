﻿using GTA;
using LemonUI;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GGO
{
    /// <summary>
    /// The Squad Members (aka friendly peds on the top right).
    /// </summary>
    public sealed class SquadMembers : IContainer<PedHealth>
    {
        #region Fields

        private readonly List<PedHealth> members = new List<PedHealth>();

        #endregion

        #region Properties

        /// <summary>
        /// If the squad members are visible or not.
        /// </summary>
        public bool Visible { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new information of squad members.
        /// </summary>
        internal SquadMembers()
        {
        }

        #endregion

        #region Functions

        /// <summary>
        /// Adds a squad member information into the HUD.
        /// </summary>
        /// <param name="member">The member's object to add.</param>
        public void Add(PedHealth member)
        {
            // If is already part of the squad members or the ped has another object attached to it, ignore it
            if (members.Contains(member) || Contains(member.Ped))
            {
                return;
            }
            // Otherwise, add it to the list
            members.Add(member);
            // And recalculate the items
            Recalculate();
        }
        /// <summary>
        /// Removes the information of a Squad Member from the HUD.
        /// </summary>
        /// <param name="member">The member's information to remove.</param>
        public void Remove(PedHealth member) => members.Remove(member);
        /// <summary>
        /// Removes the information of specific squad members from the HUD.
        /// </summary>
        /// <param name="func">The predicate to match.</param>
        public void Remove(Func<PedHealth, bool> func)
        {
            List<PedHealth> copy = new List<PedHealth>(members);
            foreach (PedHealth member in copy)
            {
                if (func(member))
                {
                    members.Remove(member);
                }
            }
        }
        /// <summary>
        /// Checks if the information of a specific squad member is present.
        /// </summary>
        /// <param name="member">The member's information to check.</param>
        public bool Contains(PedHealth member) => members.Contains(member);
        /// <summary>
        /// Checks if the ped has a Member object attached to it.
        /// </summary>
        /// <param name="ped">The ped to check.</param>
        public bool Contains(Ped ped)
        {
            // Iterate over the member objects
            foreach (PedHealth member in members)
            {
                // If the peds match, return true
                if (member.Ped == ped)
                {
                    return true;
                }
            }
            // If we got here, none were found so just return false
            return false;
        }
        /// <summary>
        /// Removes the information of all of the Squad Member's.
        /// </summary>
        public void Clear() => members.Clear();
        /// <summary>
        /// Processes the Squad Member's info.
        /// </summary>
        public void Process()
        {
            // If the player is not on the list of squad members, add it
            if (!Contains(Game.Player.Character))
            {
                Add(new PedHealth(Game.Player.Character));
            }

            // Check that the peds are present in the game world
            // If not, force a recalculation
            foreach (PedHealth member in members)
            {
                if (!member.Ped.Exists())
                {
                    Remove(member);
                }
            }

            // Finally, draw the squad members
            foreach (PedHealth member in members)
            {
                member.Process();
            }
        }
        /// <summary>
        /// Recalculates the positions of the Squad Member's information.
        /// </summary>
        public void Recalculate()
        {
            for (int i = 0; i < members.Count; i++)
            {
                // 50 is the height of the spaces
                // 5 is the separation between them
                members[i].Recalculate(new PointF(103, 66 + (50 * i) + (5 * i)));
            }
        }

        #endregion
    }
}
