using Microsoft.Win32;
using System;

namespace ShutdownTimer.Helpers
{
    public static class WindowsAPIs
    {
        /// <summary>
        /// Checks if the system currently uses the light theme or not
        /// </summary>
        public static bool SystemUsesLightTheme()
        {
            ExceptionHandler.Log("Getting windows theme");

            bool lighttheme = false; // default if all checks fail (may happen when not on Windows 10)

            try // Get actual default Windows theme which (the same as the taskbar)
            {
                int key = (int)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "SystemUsesLightTheme", null);
                if (key == 0) { lighttheme = false; }
                else if (key == 1) { lighttheme = true; }
            }
            catch (Exception) { ExceptionHandler.Log("Failed to read registry theme value"); }

            return lighttheme;
        }
    }
}
