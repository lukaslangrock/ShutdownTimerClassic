namespace ShutdownTimerWin32.Helpers
{
    class Numerics
    {
        /// <summary>
        /// Checks if seconds or minutes are over 59 and calculates new correct values.
        /// Example: 72 minutes will get converted to 1 hour and 12 minutes.
        /// </summary>
        /// <returns>New data for hours, minutes and seconds</returns>
        public static (int, int, int) ConvertTime(int hours, int minutes, int seconds)
        {
            while (seconds >= 60)
            {
                minutes += 1;
                seconds -= 60;
            }

            while (minutes >= 60)
            {
                hours += 1;
                minutes -= 60;
            }

            return (hours, minutes, seconds);
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
