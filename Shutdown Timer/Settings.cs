using ShutdownTimer.Helpers;
using System;
using System.Windows.Forms;

namespace ShutdownTimer
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            appLabel.Text = Application.ProductName + "@v" + Application.ProductVersion.Remove(Application.ProductVersion.LastIndexOf("."));
            LoadSettings();
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void RememberStateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (rememberStateCheckBox.Checked)
            {
                SettingsProvider.settings.RememberLastState = true;
                customDefaultsGroupBox.Enabled = false;
            }
            else
            {
                SettingsProvider.settings.RememberLastState = false;
                customDefaultsGroupBox.Enabled = true;
            }
        }

        private void ClearSettingsButton_Click(object sender, EventArgs e)
        {
            SettingsProvider.ClearSettings();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // general controls
            rememberStateCheckBox.Checked = SettingsProvider.settings.RememberLastState;
            trayiconThemeComboBox.Text = SettingsProvider.settings.TrayIconTheme;

            // default timer
            actionComboBox.Text = SettingsProvider.settings.DefaultTimer.Action;
            gracefulCheckBox.Checked = SettingsProvider.settings.DefaultTimer.Graceful;
            preventSleepCheckBox.Checked = SettingsProvider.settings.DefaultTimer.PreventSleep;
            backgroundCheckBox.Checked = SettingsProvider.settings.DefaultTimer.Background;
            hoursNumericUpDown.Value = SettingsProvider.settings.DefaultTimer.Hours;
            minutesNumericUpDown.Value = SettingsProvider.settings.DefaultTimer.Minutes;
            secondsNumericUpDown.Value = SettingsProvider.settings.DefaultTimer.Seconds;
        }

        private void SaveSettings()
        {
            // general controls
            SettingsProvider.settings.RememberLastState = rememberStateCheckBox.Checked;
            SettingsProvider.settings.TrayIconTheme = trayiconThemeComboBox.Text;

            // default timer
            if (!SettingsProvider.settings.RememberLastState)
            {
                SettingsProvider.settings.DefaultTimer.Action = actionComboBox.Text;
                SettingsProvider.settings.DefaultTimer.Graceful = gracefulCheckBox.Checked;
                SettingsProvider.settings.DefaultTimer.PreventSleep = preventSleepCheckBox.Checked;
                SettingsProvider.settings.DefaultTimer.Background = backgroundCheckBox.Checked;
                SettingsProvider.settings.DefaultTimer.Hours = Convert.ToInt32(hoursNumericUpDown.Value);
                SettingsProvider.settings.DefaultTimer.Minutes = Convert.ToInt32(minutesNumericUpDown.Value);
                SettingsProvider.settings.DefaultTimer.Seconds = Convert.ToInt32(secondsNumericUpDown.Value);
            }

            SettingsProvider.Save();
        }

        private void GithubLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/lukaslangrock/ShutdownTimerClassic");
        }

        private void AppLicenseLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/lukaslangrock/ShutdownTimerClassic/blob/master/LICENSE");
        }

        private void AppSourceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/lukaslangrock/ShutdownTimerClassic");
        }

        private void FALicenseLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://fontawesome.com/license/free");
        }

        private void FASourceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/FortAwesome/Font-Awesome");
        }

        private void GithubButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/lukaslangrock/ShutdownTimerClassic/issues");
        }

        private void Emailbutton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:lukas.langrock@outlook.de");
        }
    }
}
