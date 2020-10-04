using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace ShutdownTimer.Helpers
{
    public static class SettingsProvider
    {
        public static SettingsData Settings { get; set; } // current settings
        public static bool SettingsLoaded = false;
        private static readonly string settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Shutdown Timer Classic";
        private static readonly string settingsPath = settingsDirectory + "\\settings.json";

        public static void Load()
        {
            ExceptionHandler.LogEvent("[Settings] Loading settings.json");

            // make sure respective appdata dir exists
            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }

            // make sure settings.json exists
            if (!File.Exists(settingsPath))
            {
                SettingsData emptySettingsData = new SettingsData();
                string emptySettingsDataJson = JsonConvert.SerializeObject(emptySettingsData, Formatting.Indented);
                File.WriteAllText(settingsPath, emptySettingsDataJson);
            }
            string settingsJson = File.ReadAllText(settingsPath);
            Settings = new SettingsData();
            try { Settings = JsonConvert.DeserializeObject<SettingsData>(settingsJson); } catch (Exception) { }
            CheckSettings();
            SettingsLoaded = true;
        }

        private static void CheckSettings()
        {
            ExceptionHandler.LogEvent("[Settings] Checking settings object");

            Settings.AppVersion = Application.ProductVersion;

            if (Settings.TrayIconTheme is null) { Settings.TrayIconTheme = "Automatic"; }

            if (Settings.DefaultTimer is null)
            {
                Settings.DefaultTimer = new TimerData
                {
                    Action = "Shutdown",
                    Graceful = false,
                    PreventSleep = true,
                    Background = false,
                    Hours = 0,
                    Minutes = 0,
                    Seconds = 0
                };
            }
        }

        public static void ClearSettings()
        {
            ExceptionHandler.LogEvent("[Settings] Clearing settings");

            Settings = new SettingsData();
            CheckSettings();
            Save();
        }

        public static void Save()
        {
            ExceptionHandler.LogEvent("[Settings] Saving settings.json");

            if (SettingsLoaded)
            {
                string settingsJson = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                File.WriteAllText(settingsPath, settingsJson);
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

        // advanced settings
        public bool ForceIfHungFlag { get; set; }
    }

    public class TimerData
    {
        public string Action { get; set; }
        public bool Graceful { get; set; }
        public bool PreventSleep { get; set; }
        public bool Background { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }
}
