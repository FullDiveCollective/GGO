﻿namespace GGO.API
{
    /// <summary>
    /// Type of data field that is going to be shown.
    /// </summary>
    public enum FieldType
    {
        Health = 0,
        Weapon = 1
    }

    /// <summary>
    /// Abstract class used for showing all of the player data.
    /// </summary>
    public abstract class PlayerField
    {
        #region Generic

        /// <summary>
        /// Gets the type of field to be shown on the player section.
        /// </summary>
        public abstract FieldType GetFieldType();
        /// <summary>
        /// If the information field should be shown during the next game tick.
        /// </summary>
        public abstract bool ShouldBeShown();
        /// <summary>
        /// Gets the name for the icon.
        /// </summary>
        public abstract string GetIconName();
        /// <summary>
        /// Gets the current value for the health bar or ammo count.
        /// </summary>
        public abstract float GetCurrentValue();

        #endregion

        #region Health Only

        /// <summary>
        /// Gets the name of the information field.
        /// It only needs to be implemented on Health.
        /// </summary>
        public abstract string GetName();
        /// <summary>
        /// Gets the maximum value for health bar.
        /// It only needs to be implemented on Health.
        /// </summary>
        public abstract float GetMaxValue();

        #endregion

        #region Weapon Only

        /// <summary>
        /// Gets the name for the image of the weapon. This (obviously) only needs to be implemented on Weapon.
        /// </summary>
        public abstract string GetWeaponImage();

        #endregion
    }
}
