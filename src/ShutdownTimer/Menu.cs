using ShutdownTimer.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ShutdownTimer
{
    public partial class Menu : Form
    {
        public bool ApplyArgumentValues { get; set; }
        public int ArgTimeH { get; set; }
        public int ArgTimeM { get; set; }
        public int ArgTimeS { get; set; }
        public string ArgAction { get; set; }
        public string ArgMode { get; set; }
        public string ArgPassword { get; set; }
        public string ArgCommand { get; set; }
        public bool ArgGraceful { get; set; }
        public bool ArgPreventSleep { get; set; }
        public bool ArgBackground { get; set; }
        public bool ArgUseTimeOfDay { get; set; }

        private string password; // used for password protection
        private string command; // used for custom command
        private bool isConcluded = false; // true after form has concluded its function

        public Menu()
        {
            InitializeComponent();
        }

        #region "form events"

        private void Menu_Load(object sender, EventArgs e)
        {
            ExceptionHandler.Log("Check multiple instances setting");

            if (!SettingsProvider.Settings.EnableMultipleInstances)
            {
                ExceptionHandler.Log("Multiple instances disabled");

                ExceptionHandler.Log("Checking for running instance");
                if (!ApplicationInstanceManager.IsSingleInstance())
                {
                    MessageBox.Show("Another instance of this application is already running. To allow multiple instances, please check the \"Allow multiple instances\" option in the application settings.\n\nExiting...", "Application already running!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ExceptionHandler.Log("Another instance detected; exiting");
                    Application.Exit();
                }
            }
            else
            {
                ExceptionHandler.Log("Multiple instances allowed");
            }

            ExceptionHandler.Log("Initializing form");

            versionLabel.Text = "v" + Application.ProductVersion.Remove(Application.ProductVersion.LastIndexOf(".")); // Display current version
#if DEBUG
            versionLabel.Text += "_debug";
#endif

            infoToolTip.SetToolTip(gracefulCheckBox, "Applications that do not exit when prompted automatically get terminated by default to ensure a successful shutdown." +
                "\n\nA graceful shutdown, on the other hand, will wait for all applications to exit before continuing with the shutdown." +
                "\nThis might result in an unsuccessful shutdown if one or more applications are unresponsive or require a user interaction to exit.");
            infoToolTip.SetToolTip(preventSleepCheckBox, "Depending on the power settings of your system, it might go to sleep after a certain amount of time due to inactivity." +
                "\nThis option will keep the system awake to ensure the timer can properly run and execute a shutdown.");
            infoToolTip.SetToolTip(backgroundCheckBox, "This will launch the countdown without a visible window but will show a tray icon in your taskbar.");
            infoToolTip.SetToolTip(countdownModeRadioButton, "Will count down from the hours, minutes and seconds selected below,\nlike a countdown timer, and execute the power action when it reaches zero.");
            infoToolTip.SetToolTip(timeOfDayModeRadioButton, "In this mode you can select the target time of day (24h clock) for the power action.\nIf the time has already passed, it will roll over to tomorrow.\n\nWhen you press start, the appropriate countdown will be calculated.\n");

            // Prevent font-fallback and subsequent layout issues. This application is currently only in English and doesn't require display of non-Latin characters.
            this.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);

            ExceptionHandler.Log("Form initialized");
        }

        private void Menu_Shown(object sender, EventArgs e)
        {
            ExceptionHandler.Log("Form shown");
            // Check for startup arguments
            if (ApplyArgumentValues)
            {
                // Apply given setting
                ExceptionHandler.Log("Applying CLI arguments");
                LoadArgs();
            }
            else
            {
                // Load settings
                ExceptionHandler.Log("Loading settings");
                Application.DoEvents();
                LoadSettings();
            }
        }

        private void Menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExceptionHandler.Log("Form closing");

            // Only save settings when form hasn't concluded it's function.
            if (!isConcluded)
            {
                SaveSettings();
            }
        }

        private void ActionComboBox_TextChanged(object sender, EventArgs e)
        {
            // disables graceful checkbox for all modes which can not be executed gracefully / which always execute gracefully
            if (actionComboBox.Text == "Shutdown" || actionComboBox.Text == "Restart" || actionComboBox.Text == "Logout") { gracefulCheckBox.Enabled = true; }
            else { gracefulCheckBox.Enabled = false; }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            ExceptionHandler.Log("Showing settings form");
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            ExceptionHandler.Log("Start requested");

            (bool allChecksPassed, string listOfErrorsFound, string listOfWarningsFound) = RunChecks();

            if (!allChecksPassed)
            {
                ExceptionHandler.Log("Start aborted: failing checks");
                MessageBox.Show("The following error(s) occurred:\n\n" + listOfErrorsFound + "Please resolve the problem(s) and try again.", "There seems to be a problem!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!listOfWarningsFound.Equals(""))
            {
                if (MessageBox.Show(listOfWarningsFound + "Do you wish to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                {
                    ExceptionHandler.Log("User didn't accept the warnings; aborting");
                    return;
                }
            }

            if (!String.IsNullOrEmpty(ArgCommand))
            {
                command = ArgCommand;
            }
            else if (actionComboBox.Text.Equals("Custom Command"))
            {
                ExceptionHandler.Log("Custom command requested");
                using (var form = new InputBox())
                {
                    form.Title = "Custom Command";
                    form.Message = "Please enter the command you want to have executed. If you wish to launch a file, just enter the full file path.\n\n" +
                        "Note: Execution will use this user's permissions.";

                    ExceptionHandler.Log("Prompting for custom command");
                    var result = form.ShowDialog();

                    if (result != DialogResult.OK)
                    {
                        ExceptionHandler.Log("User cancelled custom command input; aborting");
                        return;
                    }

                    if (String.IsNullOrWhiteSpace(form.ReturnValue))
                    {
                        ExceptionHandler.Log("Invalid custom command input; aborting");
                        MessageBox.Show("Custom command field was empty. Please enter a valid command or use a different action!", "Invalid command!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ExceptionHandler.Log("Accepted command: \"" + form.ReturnValue + "\".");
                    command = form.ReturnValue;
                }
            }

            if (!String.IsNullOrEmpty(ArgPassword))
            {
                password = ArgPassword;
            }
            else if (SettingsProvider.Settings.PasswordProtection)
            {
                ExceptionHandler.Log("Password protection enabled");
                using (var form = new InputBox())
                {
                    form.Title = "Password Protection";
                    form.Message = "Please set a password to enable password protection.\n\n" +
                        "You can disable this dialog in the settings under Advanced > Password Protection.";
                    form.PasswordMode = true;
                    ExceptionHandler.Log("Prompting for password");
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        ExceptionHandler.Log("Password set");
                        password = form.ReturnValue;
                    } else
                    {
                        ExceptionHandler.Log("User declines password prompt; aborting");
                        return;
                    }
                }
            }

            StartCountdown();
        }

        #endregion

        /// <summary>
        /// Checks user input before further processing
        /// </summary>
        /// <returns>Report of all failed checks</returns>
        private (bool allChecksPassed, string listOfErrorsFound, string ListOfWarningsFound) RunChecks()
        {
            ExceptionHandler.Log("Running checks...");

            string errMessages = ""; // error messages will append to this
            string warnMessages = ""; // warning messages will append to this

            // Check if chosen action is a valid option
            if (!actionComboBox.Items.Contains(actionComboBox.Text))
            {
                errMessages += "Please select a valid action from the dropdown menu!\n\n";
            }

            // Check if all time values are zero when in countdown mode
            if (hoursNumericUpDown.Value == 0 && minutesNumericUpDown.Value == 0 && secondsNumericUpDown.Value == 0 && countdownModeRadioButton.Checked)
            {
                errMessages += "The timer cannot start at 0 when in countdown mode!\n\n";
            }

            // Respective check for either countdown or timeOfDay mode
            try
            {
                DateTimeOffset nowReference = DateTimeOffset.Now;
                TimeSpan ts = Time.CalculateCountdownTimeSpan(hoursNumericUpDown.Value, minutesNumericUpDown.Value, secondsNumericUpDown.Value, timeOfDayModeRadioButton.Checked, nowReference);

                // Sanity check
                if (ts.TotalDays > 100)
                {
                    warnMessages += $"Your chosen time equates to {Math.Round(ts.TotalDays)} days ({Math.Round(ts.TotalDays / 365, 2)} years)!\n" +
                        "This is a bit much, and the application was never designed for this kind of runtime, so good luck!\n\n";
                }

                // Check for time discontinuities when using timeOfDay mode
                if (timeOfDayModeRadioButton.Checked)
                {
                    
                    DateTimeOffset actualEnd = Time.CalculateLocalEndTime(ts, nowReference, TimeZoneInfo.Local);
                    TimeSpan diff = actualEnd.Offset - nowReference.Offset;
                    ExceptionHandler.Log($"Time discontinuity check: {diff.TotalSeconds} seconds diff detected");

                    if (diff.TotalSeconds != 0)
                    {
                        warnMessages += $"Your local clock will shift during this countdown! The actual time the countdown will end on diverges from your target by {Math.Abs(diff.TotalSeconds)} seconds, that's {Math.Abs(diff.TotalHours)} hour(s). This can be caused by daylight savings transition or other scheduled events in your local timezone.\n\n" +
                            $"The countdown will use the difference between now and your target of {nowReference.Add(ts):HH:mm:ss}. " +
                            $"However, your clock will display {actualEnd:HH:mm:ss} when the timer ends.\n\n" +
                            $"The countdown will always use simple arithmetic instead of relying on timezone-aware calculation. Please be aware of this discrepancy!\n\n";
                    }
                }
            }
            catch
            {
                errMessages += "TimeSpan conversion failed! Please check if your time values are within a reasonable range or represent a valid time of day, if you used the time of day mode.\n\n";
            }

            // Check if we can get permissions forExitWindowsEx() (used for shutdown/reboot/logoff)
            try
            {
                if (actionComboBox.Text.Contains("Shutdown") || actionComboBox.Text.Contains("Restart") || actionComboBox.Text.Contains("Logout"))
                {
                    ExitWindows.GetPrivileges(); // throws on error
                }
            }
            catch (Win32Exception ex)
            {
                errMessages += "Permission failure. Couldn't enable the required privliges for Shutdown/Restart/Logout from Windows. Error: " + ex.Message + "\n\n";
                errMessages += "To learn more about this issue, please go the application's settings and in the \"About\"-tab, click on \"Create Logfile\".";
            }

            if (errMessages.Equals(""))
            {
                ExceptionHandler.Log("Ran all checks, no errors found.");
                return (true, errMessages, warnMessages);
            }
            else
            {
                ExceptionHandler.Log("Ran all checks, the following errors have been found:\n" + errMessages);
                ExceptionHandler.Log("as well as the following warnings:\n" + warnMessages);
                return (false, errMessages, warnMessages);
            }
        }

        /// <summary>
        /// Load UI element data from args
        /// </summary>
        private void LoadArgs()
        {
            actionComboBox.Text = ArgAction;
            gracefulCheckBox.Checked = ArgGraceful;
            preventSleepCheckBox.Checked = ArgPreventSleep;
            backgroundCheckBox.Checked = ArgBackground;
            hoursNumericUpDown.Value = ArgTimeH;
            minutesNumericUpDown.Value = ArgTimeM;
            secondsNumericUpDown.Value = ArgTimeS;
            countdownModeRadioButton.Checked = !ArgUseTimeOfDay;
            timeOfDayModeRadioButton.Checked = ArgUseTimeOfDay;

            if (ArgMode.Equals("Lock"))
            {
                ExceptionHandler.Log("Setting 'Lock' mode");
                startButton.Text = "Start (with recommended settings)";
                settingsButton.Enabled = false;
                actionGroupBox.Enabled = false;
                timeGroupBox.Enabled = false;
            }
        }

        /// <summary>
        /// Load UI element data from settings
        /// </summary>
        private void LoadSettings()
        {
            actionComboBox.Text = SettingsProvider.Settings.DefaultTimer.Action;
            gracefulCheckBox.Checked = SettingsProvider.Settings.DefaultTimer.Graceful;
            preventSleepCheckBox.Checked = SettingsProvider.Settings.DefaultTimer.PreventSleep;
            backgroundCheckBox.Checked = SettingsProvider.Settings.DefaultTimer.Background;
            countdownModeRadioButton.Checked = SettingsProvider.Settings.DefaultTimer.CountdownMode;
            timeOfDayModeRadioButton.Checked = !SettingsProvider.Settings.DefaultTimer.CountdownMode;
            hoursNumericUpDown.Value = SettingsProvider.Settings.DefaultTimer.Hours;
            minutesNumericUpDown.Value = SettingsProvider.Settings.DefaultTimer.Minutes;
            secondsNumericUpDown.Value = SettingsProvider.Settings.DefaultTimer.Seconds;

            // Check if the user has opted to remember the last UI screen position,
            // and if LastScreenPosition is available (not null), apply its X and Y coordinates to position the form.
            if (SettingsProvider.Settings.RememberLastScreenPositionUI && SettingsProvider.Settings.LastScreenPositionUI != null)
            {
                this.Location = new Point(SettingsProvider.Settings.LastScreenPositionUI.X, SettingsProvider.Settings.LastScreenPositionUI.Y);
            }
        }

        /// <summary>
        /// Saves current timer settings as default settings if activated in settings
        /// and the last UI position
        /// </summary>
        private void SaveSettings()
        {
            if (SettingsProvider.SettingsLoaded)
            {
                ExceptionHandler.Log("Saving settings...");

                if (SettingsProvider.Settings.RememberLastState)
                {
                    SettingsProvider.Settings.DefaultTimer.Action = actionComboBox.Text;
                    SettingsProvider.Settings.DefaultTimer.Graceful = gracefulCheckBox.Checked;
                    SettingsProvider.Settings.DefaultTimer.PreventSleep = preventSleepCheckBox.Checked;
                    SettingsProvider.Settings.DefaultTimer.Background = backgroundCheckBox.Checked;
                    SettingsProvider.Settings.DefaultTimer.CountdownMode = countdownModeRadioButton.Checked;
                    SettingsProvider.Settings.DefaultTimer.Hours = Convert.ToInt32(hoursNumericUpDown.Value);
                    SettingsProvider.Settings.DefaultTimer.Minutes = Convert.ToInt32(minutesNumericUpDown.Value);
                    SettingsProvider.Settings.DefaultTimer.Seconds = Convert.ToInt32(secondsNumericUpDown.Value);
                }

                if (SettingsProvider.Settings.RememberLastScreenPositionUI)
                {
                    SettingsProvider.Settings.LastScreenPositionUI = new LastScreenPosition
                    {
                        X = this.Location.X,
                        Y = this.Location.Y
                    };
                }

                SettingsProvider.Save();
                ExceptionHandler.Log("Settings saved");
            }
            else
            {
                ExceptionHandler.Log("Ignoring SaveSettings() call as no settings are loaded");
            }
        }

        /// <summary>
        /// Starts the countdown with values from UI
        /// </summary>
        private void StartCountdown()
        {
            ExceptionHandler.Log("Preparing countdown");
            startButton.Enabled = false;
            actionGroupBox.Enabled = false;
            timeGroupBox.Enabled = false;
            isConcluded = true; // mark form as concluded to prevent further logic on form closing events
            SaveSettings();

            ExceptionHandler.Log("Countdown initiated");
            this.Hide();

            ExceptionHandler.Log("Starting countdown...");

            // Calculate TimeSpan
            ExceptionHandler.Log("Calculating timespan");
            TimeSpan timeSpan = Time.CalculateCountdownTimeSpan(hoursNumericUpDown.Value, minutesNumericUpDown.Value, secondsNumericUpDown.Value, timeOfDayModeRadioButton.Checked, DateTime.Now);

            Timer.CountdownTimeSpan = timeSpan;
            Timer.Action = actionComboBox.Text;
            Timer.Graceful = gracefulCheckBox.Checked;
            Timer.PreventSystemSleep = preventSleepCheckBox.Checked;
            Timer.Command = command;

            // Show countdown window
            ExceptionHandler.Log("Starting timer");
            Timer.Start(password, !backgroundCheckBox.Checked);
        }
    }
}
