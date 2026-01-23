using System;

namespace ShutdownTimer.Helpers
{
    class Numerics
    {
        /// <summary>
        /// Converts a TimeSpan into hours, minutes and seconds.
        /// </summary>
        /// <returns>Integers for hours, minutes and seconds</returns>
        public static (int, int, int) ConvertTimeSpan(TimeSpan ts)
        {
            int hours = ts.Hours;
            int minutes = ts.Minutes;
            int seconds = ts.Seconds;

            hours += ts.Days * 24;

            return (hours, minutes, seconds);
        }

        /// <summary>
        /// Converts a TimeSpan into a string of hours, minutes and seconds.
        /// </summary>
        /// <returns>Combined string with hours, minutes and seconds</returns>
        public static string ConvertTimeSpanToString(TimeSpan ts)
        {
            int hours, minutes, seconds;

            (hours, minutes, seconds) = ConvertTimeSpan(ts);

            return AddZeros(hours) + ":" + AddZeros(minutes) + ":" + AddZeros(seconds);
        }

        /// <summary>
        /// Calculates a TimeSpan to count down towards based on given input.
        /// </summary>
        /// <param name="pHours">target hour value</param>
        /// <param name="pMinutes">target minute value</param>
        /// <param name="pSeconds">target second value</param>
        /// <param name="interpretAsTimeOfDay">Treats the time values as a target time of day and calculates a timespan from now to the target time instead.</param>
        /// <returns></returns>
        public static TimeSpan CalculateCountdownTimeSpan (Object pHours, Object pMinutes, Object pSeconds, bool interpretAsTimeOfDay)
        {
            int hours = Convert.ToInt32(pHours);
            int minutes = Convert.ToInt32(pMinutes);
            int seconds = Convert.ToInt32(pSeconds);
            TimeSpan ts;

            if (interpretAsTimeOfDay)
            {
                // calculate timespan till a time of day target
                bool today = Numerics.TodayOrTomorrow(hours, minutes, seconds);
                DateTime target = DateTime.Parse(hours + ":" + minutes + ":" + seconds);
                if (!today) { target = target.AddDays(1); }
                ts = target.Subtract(DateTime.Now);
            } else
            {
                // normal timespan parsing
                ts = new TimeSpan(hours, minutes, seconds);
            }

            return ts;
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

        /// <summary>
        /// determine if a given set of hours, minutes, and seconds regards a time of the current day or the next day
        /// </summary>
        /// <returns>true if today, false if tomorrow</returns>
        public static bool TodayOrTomorrow(int hours, int minutes, int seconds)
        {
            DateTime now = DateTime.Now;
            DateTime target = new DateTime(now.Year, now.Month, now.Day, hours, minutes, seconds);

            if (target < now) { return false; } // specified time is in the past (for the current day) -> for tomorrow
            else { return true; } // specified time is in the future -> for today
        }
    }
}
