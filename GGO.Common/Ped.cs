using GTA;
using GTA.Native;
using System.Drawing;

namespace GGO.Common
{
    public static class PedExtension
    {
        /// <summary>
        /// Returns a color based on the player health.
        /// </summary>
        /// <param name="ThePed">The ped to check.</param>
        /// <returns>A color that match the current ped health.</returns>
        public static Color HealthColor(this Ped ThePed)
        {
            // Get our health
            float CurrentHealth = Function.Call<int>(Hash.GET_ENTITY_HEALTH, ThePed) - 100;
            float MaxHealth = Function.Call<int>(Hash.GET_PED_MAX_HEALTH, ThePed) - 100;
            float Percentage = (CurrentHealth / MaxHealth) * 100;

            // If the player is on normal levels
            // Return White
            if (Percentage >= 50 && Percentage <= 100)
            {
                return Colors.HealthNormal;
            }
            // If the player is under risky levels
            // Return Yellow
            else if (Percentage <= 50 && Percentage >= 25)
            {
                return Colors.HealthDanger;
            }
            // If the player is about to die
            // Return Red
            else if (Percentage <= 25)
            {
                return Colors.HealthDying;
            }
            // If the player is under 0 or over 100
            // Return blue
            else
            {
                return Colors.HealthOverflow;
            }
        }

        /// <summary>
        /// Returns the name of a ped based on the model.
        /// </summary>
        /// <param name="ThePed">The ped to check.</param>
        /// <param name="Custom">The custom name for the player ped.</param>
        /// <returns>The name that corresponds for the ped.</returns>
        public static string Name(this Ped ThePed, Configuration Config)
        {
            // If the ped is the player and the custom name has not been changed
            // Return the Social Club username
            if (Config.Name == "default" && ThePed == Game.Player.Character)
            {
                return Game.Player.Name;
            }
            // If the ped is the player and a custom name has been added
            // Return that custom name
            else if (ThePed == Game.Player.Character)
            {
                return Config.Name;
            }
            // If is not the player but there is a custom name available
            // Return that ped name
            else if (Config.PedNames.IsNameDefined(ThePed.Model.Hash))
            {
                return Config.PedNames.GetName(ThePed.Model.Hash);
            }
            // If none of the previous ones work
            // Return the hash as a string
            else
            {
                return ThePed.Model.Hash.ToString();
            }
        }
    }
}
