using System;

namespace ShutdownTimer.Helpers
{
    class Numerics
    {
        /// <summary>
        /// Converts a TimeSpan into a string of hours, minutes and seconds.
        /// </summary>
        /// <returns>Combined string with hours, minutes and seconds</returns>
        public static string ConvertTimeSpanToString(TimeSpan ts)
        {
            int hours = ts.Hours;
            int minutes = ts.Minutes;
            int seconds = ts.Seconds;

            hours += ts.Days * 24;

            return AddZeros(hours) + ":" + AddZeros(minutes) + ":" + AddZeros(seconds);
        }

        /// <summary>
        /// Adds a zero before values lower than ten.
        /// </summary>
        /// <param name="input">Input number</param>
        /// <returns>Formatted output string</returns>
        public static string AddZeros(int input)
        {
            string output;

            if (input < 10) { output = "0" + input.ToString(); }
            else { output = input.ToString(); }

            return output;
        }
    }
}
