using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace ShutdownTimer.Helpers
{
    public static class SettingsProvider
    {
        public static SettingsData settings; // working copy of settings
        private static string settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Shutdown Timer Classic";
        private static string settingsPath = settingsDirectory + "\\settings.json";

        public static void Load()
        {
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
            settings = new SettingsData();
            try { settings = JsonConvert.DeserializeObject<SettingsData>(settingsJson); } catch (Exception) { }
            CheckSettings();
        }

        private static void CheckSettings()
        {
            settings.AppVersion = Application.ProductVersion;
            if (settings.DefaultTimer is null)
            {
                settings.DefaultTimer = new TimerData
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
            if (settings.TrayIconTheme is null) { settings.TrayIconTheme = "Automatic"; }
        }

        public static void ClearSettings()
        {
            settings = new SettingsData();
            CheckSettings();
            Save();
        }

        public static void Save()
        {
            string settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(settingsPath, settingsJson);
        }
    }

    public class SettingsData
    {
        public string AppVersion { get; set; }

        public bool RememberLastState { get; set; }

        public TimerData DefaultTimer { get; set; }

        public string TrayIconTheme { get; set; }
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
