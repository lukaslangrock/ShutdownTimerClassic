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
            ExceptionHandler.LogEvent("[Settings] Loading settings...");

            if (!TemporaryMode)
            {
                // make sure respective appdata dir exists
                if (!Directory.Exists(settingsDirectory))
                {
                    ExceptionHandler.LogEvent("[Settings] Creating settings directory");
                    Directory.CreateDirectory(settingsDirectory);
                }

                // make sure settings.json exists
                if (!File.Exists(settingsPath))
                {
                    ExceptionHandler.LogEvent("[Settings] Settings file does not exist.");
                    ExceptionHandler.LogEvent("[Settings] Creating new empty settings object");
                    SettingsData emptySettingsData = new SettingsData();
                    ExceptionHandler.LogEvent("[Settings] Serializing settings");
                    string emptySettingsDataJson = JsonConvert.SerializeObject(emptySettingsData, Formatting.Indented);
                    ExceptionHandler.LogEvent("[Settings] Writing settings.json");
                    File.WriteAllText(settingsPath, emptySettingsDataJson);
                }

                ExceptionHandler.LogEvent("[Settings] Loading settings.json");
                string settingsJson = File.ReadAllText(settingsPath);
                Settings = new SettingsData();
                try { Settings = JsonConvert.DeserializeObject<SettingsData>(settingsJson); } catch (Exception ex) { ExceptionHandler.LogEvent("[Settings] Error deserializing object: " + ex.Message); }
                CheckSettings();
                SettingsLoaded = true;
                ExceptionHandler.LogEvent("[Settings] Successfully loaded settings to an object in application memory");
            }
            else
            {
                ExceptionHandler.LogEvent("[Settings] Creating new ephemeral settings due to TemporaryMode");
                Settings = new SettingsData();
                CheckSettings();
                SettingsLoaded = true;
                ExceptionHandler.LogEvent("[Settings] Successfully created new tempoarary settings object in application memory");
            }
        }

        private static void CheckSettings()
        {
            ExceptionHandler.LogEvent("[Settings] Checking settings object");
            if (Settings == null)
            {
                ExceptionHandler.LogEvent("[Settings] Settings could not be loaded, the file might be corrupted or empty. Initializing new settings object.");
                ClearSettings();
            }

            // If settings were created by an earlier version some migration steps might have to occur before using them
            ExceptionHandler.LogEvent("[Settings] Checking product version");
            if (Settings.AppVersion != null && Settings.AppVersion != "")
            {
                // 
                String[] importVerString = Settings.AppVersion.Split('.');
                int[] importVer = new int[] { 0, 0, 0, 0 }; // defaults in case string is corrupted and does not contain 4 numbers
                for (int i = 0; i < 4; i++) // read version number into useable integer array
                {
                    try { importVer[i] = Convert.ToInt32(importVerString[i]); }
                    catch { ExceptionHandler.LogEvent("[Settings] Part " + (i + 1).ToString() + " of settings version number unreadable '" + importVerString[i] + "' filling with 0"); }
                }

                if (importVer[0] == 1) // everything v1.X
                {
                    if (importVer[1] < 3) // for < v1.3
                    {
                        // Issue #49: When reading from earlier settings version DefaultTimer.CountdownMode does not exist and is initialized as false but default should be true
                        Settings.DefaultTimer.CountdownMode = true;
                        ExceptionHandler.LogEvent("[Settings] Set DefaultTimer.CountdownMode to true as migration step from <v1.3");
                    }

                    if (importVer[1] == 3 && importVer[2] == 0) // for = v1.3.0
                    {
                        // Issue #49: Same as above, except we force change this to lessen the general impact of the issue.
                        // It's not very nice, especially for people who were not impact and changed this to false manually, but the group impacted is way larger than the one not impacted
                        Settings.DefaultTimer.CountdownMode = true;
                        ExceptionHandler.LogEvent("[Settings] Forced DefaultTimer.CountdownMode to true as mitigation for users impacted by Issue #49");
                    }
                }
            }

            ExceptionHandler.LogEvent("[Settings] Setting product version");
            Settings.AppVersion = Application.ProductVersion;

            ExceptionHandler.LogEvent("[Settings] Checking field: TrayIconTheme");
            if (Settings.TrayIconTheme is null)
            {
                ExceptionHandler.LogEvent("[Settings] Restoring TrayIconTheme to defaults");
                Settings.TrayIconTheme = "Automatic";
            }

            ExceptionHandler.LogEvent("[Settings] Checking field: DefaultTimer");
            if (Settings.DefaultTimer is null)
            {
                ExceptionHandler.LogEvent("[Settings] Restoring DefaultTimer to defaults");
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

            ExceptionHandler.LogEvent("[Settings] Checking field: BackgroundColor");
            if (Settings.BackgroundColor == Color.Empty)
            {
                ExceptionHandler.LogEvent("[Settings] Restoring BackgroundColor to defaults");
                Settings.BackgroundColor = Color.RoyalBlue;
            }

            ExceptionHandler.LogEvent("[Settings] Checked settings object");
        }

        public static void ClearSettings()
        {
            ExceptionHandler.LogEvent("[Settings] Clearing settings...");

            Settings = new SettingsData();
            ExceptionHandler.LogEvent("[Settings] Created new empty settings object");
            CheckSettings();
            Save();

            ExceptionHandler.LogEvent("[Settings] Cleared settings");
        }

        public static void Save()
        {
            if (!TemporaryMode)
            {
                ExceptionHandler.LogEvent("[Settings] Saving settings...");

                if (SettingsLoaded)
                {
                    ExceptionHandler.LogEvent("[Settings] Serializing settings");
                    string settingsJson = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                    ExceptionHandler.LogEvent("[Settings] Writing settings.json");
                    File.WriteAllText(settingsPath, settingsJson);
                    ExceptionHandler.LogEvent("[Settings] Saved settings");
                }
                else
                {
                    ExceptionHandler.LogEvent("[Settings] Ignoring Settings.Save() call as no settings are loaded");
                }
            }
            else
            {
                ExceptionHandler.LogEvent("[Settings] Ignoring Settings.Save() call due to TemporaryMode");
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
