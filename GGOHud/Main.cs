using GTA;
using System;
using System.Globalization;
using System.Threading;

namespace GGOHud
{
    public class GGOHud : Script
    {
        /// <summary>
        /// Class to get our configuration values.
        /// </summary>
        private Configuration Config = new Configuration("scripts\\GGOHud.ini", "GGOHud");

        public GGOHud()
        {
            // Patch our locale so we don't have the "coma vs dot" problem
            CultureInfo CultureCopy = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            CultureCopy.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = CultureCopy;

            // Add our OnTick event
            Tick += OnTick;

            if (Config.Debug)
            {
                UI.Notify("GGOHud has been enabled.");
            }
        }

        private void OnTick(object Sender, EventArgs Args)
        {

        }
    }
}
