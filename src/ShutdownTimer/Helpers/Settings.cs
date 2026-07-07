using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShutdownTimer.Helpers
{
    public static class SettingsProvider
    {
        public static SettingsData Settings { get; set; } // current settings
        public static bool SettingsLoaded = false;
        public static bool TemporaryMode = false;
        private static readonly string settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Shutdown Timer Classic";
        private static readonly string settingsPath = settingsDirectory + "\\settings.json";

        public static void Load()
        {
            ExceptionHandler.Log("Loading settings");

            if (!TemporaryMode)
            {
                // make sure respective appdata dir exists
                if (!Directory.Exists(settingsDirectory))
                {
                    ExceptionHandler.Log("Creating settings directory");
                    Directory.CreateDirectory(settingsDirectory);
                }

                // make sure settings.json exists
                if (!File.Exists(settingsPath))
                {
                    ExceptionHandler.Log("Settings file not found; creating new settings object");
                    SettingsData emptySettingsData = new SettingsData();
                    ExceptionHandler.Log("Serializing settings");
                    string emptySettingsDataJson = JsonConvert.SerializeObject(emptySettingsData, Formatting.Indented);
                    ExceptionHandler.Log("Writing settings.json");
                    File.WriteAllText(settingsPath, emptySettingsDataJson);
                }

                ExceptionHandler.Log("Loading settings.json");
                string settingsJson = File.ReadAllText(settingsPath);
                Settings = new SettingsData();
                try { Settings = JsonConvert.DeserializeObject<SettingsData>(settingsJson); } catch (Exception ex) { ExceptionHandler.Log("Error deserializing object: " + ex.Message); }
                CheckSettings();
                SettingsLoaded = true;
                ExceptionHandler.Log("Successfully loaded settings to an object in application memory");
            }
            else
            {
                ExceptionHandler.Log("Initializing temporary settings (TemporaryMode)");
                Settings = new SettingsData();
                CheckSettings();
                SettingsLoaded = true;
                ExceptionHandler.Log("Temporary settings initialized");
            }
        }

        private static void CheckSettings()
        {
            ExceptionHandler.Log("Validating settings object");
            if (Settings == null)
            {
                ExceptionHandler.Log("Settings null or corrupted; initializing defaults");
                ClearSettings();
            }

            // If settings were created by an earlier version some migration steps might have to occur before using them
            ExceptionHandler.Log("Check settings version");
            if (Settings.AppVersion != null && Settings.AppVersion != "")
            {
                // 
                String[] importVerString = Settings.AppVersion.Split('.');
                int[] importVer = new int[] { 0, 0, 0, 0 }; // defaults in case string is corrupted and does not contain 4 numbers
                for (int i = 0; i < 4; i++) // read version number into usable integer array
                {
                    try { importVer[i] = Convert.ToInt32(importVerString[i]); }
                    catch { ExceptionHandler.Log("Part " + (i + 1).ToString() + " of settings version number unreadable '" + importVerString[i] + "' filling with 0"); }
                }

                if (importVer[0] == 1) // everything v1.X
                {
                    if (importVer[1] < 3) // for < v1.3
                    {
                        // Issue #49: When reading from earlier settings version DefaultTimer.CountdownMode does not exist and is initialized as false but default should be true
                        Settings.DefaultTimer.CountdownMode = true;
                        ExceptionHandler.Log("Set DefaultTimer.CountdownMode to true as migration step from <v1.3");
                    }

                    if (importVer[1] == 3 && importVer[2] == 0) // for = v1.3.0
                    {
                        // Issue #49: Same as above, except we force change this to lessen the general impact of the issue.
                        // It's not very nice, especially for people who were not impact and changed this to false manually, but the group impacted is way larger than the one not impacted
                        Settings.DefaultTimer.CountdownMode = true;
                        ExceptionHandler.Log("Forced DefaultTimer.CountdownMode to true as mitigation for users impacted by Issue #49");
                    }
                }
            }

            ExceptionHandler.Log("Set product version in settings");
            Settings.AppVersion = Application.ProductVersion;

            ExceptionHandler.Log("Check TrayIconTheme field");
            if (Settings.TrayIconTheme is null)
            {
                ExceptionHandler.Log("Restore TrayIconTheme default");
                Settings.TrayIconTheme = "Automatic";
            }

            ExceptionHandler.Log("Check DefaultTimer field");
            if (Settings.DefaultTimer is null)
            {
                ExceptionHandler.Log("Restore DefaultTimer defaults");
                Settings.DefaultTimer = new TimerData
                {
                    Action = "Shutdown",
                    Graceful = false,
                    PreventSleep = true,
                    Background = false,
                    CountdownMode = true,
                    Hours = 0,
                    Minutes = 0,
                    Seconds = 0
                };
            }

            ExceptionHandler.Log("Check BackgroundColor field");
            if (Settings.BackgroundColor == Color.Empty)
            {
                ExceptionHandler.Log("Restore BackgroundColor defaults");
                Settings.BackgroundColor = Color.RoyalBlue;
            }

            ExceptionHandler.Log("Settings validated");
        }

        public static void ClearSettings()
        {
            ExceptionHandler.Log("Clearing settings");

            Settings = new SettingsData();
            ExceptionHandler.Log("Created default settings object");
            CheckSettings();
            Save();

            ExceptionHandler.Log("Settings cleared");
        }

        public static void Save()
        {
            if (!TemporaryMode)
            {
                ExceptionHandler.Log("Saving settings");

                if (SettingsLoaded)
                {
                    ExceptionHandler.Log("Serializing settings");
                    string settingsJson = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                    ExceptionHandler.Log("Writing settings.json");
                    File.WriteAllText(settingsPath, settingsJson);
                    ExceptionHandler.Log("Settings saved");
                }
                else
                {
                    ExceptionHandler.Log("Ignore save: settings not loaded");
                }
            }
            else
            {
                ExceptionHandler.Log("Ignore save: TemporaryMode enabled");
            }
        }
    }

    public class SettingsData
    {
        // meta
        public string AppVersion { get; set; }

        // general settings
        public bool RememberLastState { get; set; }
        public string TrayIconTheme { get; set; }
        public TimerData DefaultTimer { get; set; }
        public bool RememberLastScreenPositionUI { get; set; }
        public LastScreenPosition LastScreenPositionUI { get; set; }
        public bool RememberLastScreenPositionCountdown { get; set; }
        public LastScreenPosition LastScreenPositionCountdown { get; set; }
        public bool EnableMultipleInstances { get; set; }
        // advanced settings
        public bool ForceIfHungFlag { get; set; }
        public bool DisableAlwaysOnTop { get; set; }
        public bool DisableAnimations { get; set; }
        public bool DisableNotifications { get; set; }
        public bool PasswordProtection { get; set; }
        public Color BackgroundColor { get; set; }
        public bool AdaptiveCountdownTextSize { get; set; }
        public bool HideTrayIcon { get; set; }
        public bool SaveEventLogOnExit { get; set; }
    }

    public class TimerData
    {
        public string Action { get; set; }
        public bool Graceful { get; set; }
        public bool PreventSleep { get; set; }
        public bool Background { get; set; }
        public bool CountdownMode { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }

    public class LastScreenPosition
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
