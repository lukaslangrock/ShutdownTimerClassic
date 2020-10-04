using Microsoft.Win32;
using System;
using Windows.UI.ViewManagement;

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
            ExceptionHandler.LogEvent("[WindowsAPIs] Get windows theme");

            bool lighttheme = false; // default if all checks fail (may happen when not on Windows 10)

            try // Get app theme as fallback
            {
                Windows.UI.Color winTheme = new UISettings().GetColorValue(UIColorType.Background);
                if (winTheme.ToString() == "#FFFFFFFF") { lighttheme = true; }
                //else if (winTheme.ToString() == "#FF000000") { lighttheme = false; }
            }
            catch (Exception) { ExceptionHandler.LogEvent("[WindowsAPIs] Failed to get winTheme"); }

            try // Get actual default Windows theme which (the same as the taskbar)
            {
                int key = (int)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "SystemUsesLightTheme", null);
                if (key == 0) { lighttheme = false; }
                else if (key == 1) { lighttheme = true; }
            }
            catch (Exception) { ExceptionHandler.LogEvent("[WindowsAPIs] Failed to read registry theme value"); }

            return lighttheme;
        }
    }
}
