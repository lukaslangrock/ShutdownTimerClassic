using System;

namespace ShutdownTimer.Helpers
{
    class Time
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

            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        /// <summary>
        /// Calculates a TimeSpan to count down towards based on given input.
        /// </summary>
        /// <param name="pHours">target hour value</param>
        /// <param name="pMinutes">target minute value</param>
        /// <param name="pSeconds">target second value</param>
        /// <param name="interpretAsTimeOfDay">Treats the time values as a target time of day and calculates a timespan from now to the target time instead.</param>
        /// <param name="nowReference">reference to DateTime.Now</param>
        /// <returns></returns>
        public static TimeSpan CalculateCountdownTimeSpan(Object pHours, Object pMinutes, Object pSeconds, bool interpretAsTimeOfDay, DateTimeOffset nowReference)
        {
            int hours = Convert.ToInt32(pHours);
            int minutes = Convert.ToInt32(pMinutes);
            int seconds = Convert.ToInt32(pSeconds);

            if (interpretAsTimeOfDay)
            {
                TimeSpan targetTime = new TimeSpan(hours, minutes, seconds);
                TimeSpan currentTime = nowReference.TimeOfDay;

                TimeSpan diff = targetTime - currentTime;

                // if the diff evaluates negative, the target must be for tomorrow instead of today, so add another day
                if (diff < TimeSpan.Zero)
                {
                    diff = diff.Add(TimeSpan.FromDays(1));
                }

                return diff;
            }
            else
            {
                // normal timespan parsing
                return new TimeSpan(hours, minutes, seconds);
            }
        }

        /// <summary>
        /// The TimeSpan calculated by CalculateCountdownTimeSpan is very naive, however real world time includes jumps and discontinuities, such as daylight savings time.
        /// As the result, the countdown will terminate at a time different than what the user put in (regarding only timeOfDay mode).
        /// This function calculates the real local end time a user can expect, in order to inform them of what's going to happen and why.
        /// </summary>
        /// <param name="target">calculated timespan from now until countdown end</param>
        /// <param name="nowReference">reference time to DateTime.Now</param>
        /// <returns>the point in time when the countdown will terminate</returns>
        public static DateTimeOffset CalculateLocalEndTime(TimeSpan target, DateTimeOffset nowReference, TimeZoneInfo tzReference)
        {
            DateTimeOffset countdownEndTime = nowReference.Add(target);
            DateTimeOffset actualEndTime = TimeZoneInfo.ConvertTime(countdownEndTime, tzReference);

            return actualEndTime;
        }
    }
}
