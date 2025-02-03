namespace ShutdownTimer.Helpers
{
    public static class WindowsAPIs
    {
        /// <summary>
        /// Checks Windows APIs to get the current Windows theme (not the app theme, the actual system theme for the taskbar, start menu, etc).
        /// </summary>
        /// <returns>true: Light theme; false: Dark theme</returns>
        public static bool GetWindowsLightTheme()
        {
            ExceptionHandler.LogEvent("[WindowsAPIs] Dummy Mode: This legacy version does not support auto theme detection");

            return false;
        }
    }
}
