using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
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
                try { Settings = JsonConvert.DeserializeObject<SettingsData>(settingsJson); } catch (Exception ex) { ExceptionHandler.LogEvent("[Settings] Error deserializing object: " + ex.ToString()); }
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

            if (Settings.AppVersion != Application.ProductVersion && Settings.AppVersion != null) { MigrateSettings(); }
            ExceptionHandler.LogEvent("[Settings] Setting current product version");
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
                    TimeMode = "Countdown",
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

        public static void MigrateSettings()
        {
            ExceptionHandler.LogEvent("[Settings] Trying to migrate settings from older version: " + Settings.AppVersion);
            try
            {
                JObject oldSettings = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(settingsPath));
                // Example of how to get data that was not imported into the current settings object: string test = oldSettings["DefaultTimer"]["Action"].Value<string>();
                ExceptionHandler.LogEvent("[Settings] Object deserialized: " + oldSettings.ToString());
                switch (Settings.AppVersion)
                {
                    case "1.2.3.0":
                        Settings.DefaultTimer.TimeMode = "Countdown"; // this was the only option in <=v1.2.3, so it will be set to this by default
                        ExceptionHandler.LogEvent("[Settings] Migrated settings from v1.2.3");
                        goto case "1.3.0.0"; // going to the next version in case we make multi-version jumps and there are multiple migration steps.

                    case "1.3.0.0":
                        ExceptionHandler.LogEvent("[Settings] Migration procedure has imported all supported older settings");
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogEvent("[Settings] Error deserializing object: " + ex.ToString());
            }
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

        // advanced settings
        public bool ForceIfHungFlag { get; set; }
        public bool DisableAlwaysOnTop { get; set; }
        public bool DisableAnimations { get; set; }
        public bool DisableNotifications { get; set; }
        public bool PasswordProtection { get; set; }
        public Color BackgroundColor { get; set; }
        public bool AdaptiveCountdownTextSize { get; set; }
    }

    public class TimerData
    {
        public string Action { get; set; }
        public bool Graceful { get; set; }
        public bool PreventSleep { get; set; }
        public bool Background { get; set; }
        public string TimeMode { get; set; }
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
