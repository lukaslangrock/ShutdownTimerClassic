using ShutdownTimer.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShutdownTimer
{
    public partial class Menu : Form
    {
        public bool OverrideSettings { get; set; }
        public int ArgTimeH { get; set; }
        public int ArgTimeM { get; set; }
        public int ArgTimeS { get; set; }
        public string ArgAction { get; set; }
        public string ArgMode { get; set; }
        public bool ArgGraceful { get; set; }
        public bool ArgPreventSleep { get; set; }
        public bool ArgBackground { get; set; }

        private string checkResult;
        private string password; // used for password protection
        private string command; // used for custom command

        public Menu()
        {
            InitializeComponent();
        }

        #region "form events"

        private void Menu_Load(object sender, EventArgs e)
        {
            ExceptionHandler.Log("Checking multiple instances configuration");

            if (!SettingsProvider.Settings.EnableMultipleInstances)
            {
                ExceptionHandler.Log("Multiple instances are not allowed");

                ExceptionHandler.Log("Checking for another instance already running");
                if (!ApplicationInstanceManager.IsSingleInstance())
                {
                    MessageBox.Show("Another instance of this application is already running. To allow multiple instances, please check the \"Allow multiple instances\" option in the application settings.\n\nExiting...", "Application already running!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ExceptionHandler.Log("Detected another instance already running. Exiting...");
                    Application.Exit();
                }
            }
            else
            {
                ExceptionHandler.Log("Multiple instances are allowed");
            }

            ExceptionHandler.Log("Setting up form...");

            versionLabel.Text = "v" + Application.ProductVersion.Remove(Application.ProductVersion.LastIndexOf(".")); // Display current version
            infoToolTip.SetToolTip(gracefulCheckBox, "Applications that do not exit when prompted automatically get terminated by default to ensure a successful shutdown." +
                "\n\nA graceful shutdown on the other hand will wait for all applications to exit before continuing with the shutdown." +
                "\nThis might result in an unsuccessful shutdown if one or more applications are unresponsive or require a user interaction to exit!");
            infoToolTip.SetToolTip(preventSleepCheckBox, "Depending on the power settings of your system, it might go to sleep after certain amount of time due to inactivity." +
                "\nThis option will keep the system awake to ensure the timer can properly run and execute a shutdown.");
            infoToolTip.SetToolTip(backgroundCheckBox, "This will launch the countdown without a visible window but will show a tray icon in your taskbar.");

            ExceptionHandler.Log("Setup finished");
        }

        private void Menu_Shown(object sender, EventArgs e)
        {
            ExceptionHandler.Log("Showing form");
            // Check for startup arguments
            if (OverrideSettings)
            {
                // Apply given setting
                ExceptionHandler.Log("Loading args");
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
            ExceptionHandler.Log("Form closing...");
            SaveSettings();
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
            ExceptionHandler.Log("Initiaing preparation for countdown start");

            if (RunChecks())
            {
                // Disable controls
                ExceptionHandler.Log("Preparing for countdown start");
                startButton.Enabled = false;
                actionGroupBox.Enabled = false;
                timeGroupBox.Enabled = false;

                SaveSettings();

                if (actionComboBox.Text.Equals("Custom Command"))
                {
                    ExceptionHandler.Log("Entering custom command setup");
                    using (var form = new InputBox())
                    {
                        form.Title = "Custom Command";
                        form.Message = "Please enter the command you want to have executed. If you wish to launch a file, just enter the full file path.\n\n" +
                            "Note: Execution will use this user's permissions.";
                        ExceptionHandler.Log("Requesting custom command from user...");
                        var result = form.ShowDialog();
                        ExceptionHandler.Log("Saving command");
                        command = form.ReturnValue;
                    }
                }

                if (SettingsProvider.Settings.PasswordProtection)
                {
                    ExceptionHandler.Log("Enabeling password protection");
                    using (var form = new InputBox())
                    {
                        form.Title = "Password Protection";
                        form.Message = "Please set a password to enable password protection.\n\n" +
                            "You can disable this dialog in the settings under Advanced > Password Protection";
                        form.PasswordMode = true;
                        ExceptionHandler.Log("Requesting password setup from user...");
                        var result = form.ShowDialog();
                        ExceptionHandler.Log("Saving password");
                        password = form.ReturnValue;
                    }
                }

                ExceptionHandler.Log("Initiaing countdown start");
                this.Hide();
                StartCountdown();
            }
            else
            {
                ExceptionHandler.Log("Invalid countdown");
                ExceptionHandler.Log(checkResult);
                MessageBox.Show("The following error(s) occurred:\n\n" + checkResult + "Please try to resolve the(se) problem(s) and try again.", "There seems to be a problem!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        /// <summary>
        /// Checks user input before further processing
        /// </summary>
        /// <returns>Result of checks</returns>
        private bool RunChecks()
        {
            ExceptionHandler.Log("Running checks...");

            bool errTracker = true; // if anything goes wrong the tracker will be set to false
            string errMessage = null; // error messages will append to this

            // Check if chosen action is a valid option
            if (!actionComboBox.Items.Contains(actionComboBox.Text))
            {
                errTracker = false;
                errMessage += "Please select a valid action from the dropdown menu!\n\n";
            }

            // Check if all time values are zero
            if (hoursNumericUpDown.Value == 0 && minutesNumericUpDown.Value == 0 && secondsNumericUpDown.Value == 0)
            {
                errTracker = false;
                errMessage += "The timer cannot start at 0!\n\n";
            }

            // Respective check for either countdown or timeOfDay mode
            if (countdownModeRadioButton.Checked)
            {
                // Try to build and convert a the values to a TimeSpan and export it as a string.
                try
                {
                    Numerics.ConvertTimeSpanToString(new TimeSpan(Convert.ToInt32(hoursNumericUpDown.Value), Convert.ToInt32(minutesNumericUpDown.Value), Convert.ToInt32(secondsNumericUpDown.Value)));
                }
                catch
                {
                    errTracker = false;
                    errMessage += "TimeSpan conversion failed! Please check if your time values are within a reasonable range.\n\n";
                }
            }
            else
            {
                // Try to build a valid DateTime
                try
                {
                    DateTime now = DateTime.Now;
                    DateTime target = new DateTime(now.Year, now.Month, now.Day, Convert.ToInt32(hoursNumericUpDown.Value), Convert.ToInt32(minutesNumericUpDown.Value), Convert.ToInt32(secondsNumericUpDown.Value));
                }
                catch
                {
                    errTracker = false;
                    errMessage += "DateTime conversion failed! Please check if your time values can represent a valid time of day.\n\n";
                }
            }


            // Sanity check
            try
            {
                TimeSpan ts = new TimeSpan(Convert.ToInt32(hoursNumericUpDown.Value), Convert.ToInt32(minutesNumericUpDown.Value), Convert.ToInt32(secondsNumericUpDown.Value));
                if (ts.TotalDays > 100)
                {
                    MessageBox.Show($"Your chosen time equates to {Math.Round(ts.TotalDays)} days ({Math.Round(ts.TotalDays / 365, 2)} years)!\n" +
                        "It is highly discouraged to choose such an insane amount of time as either your hardware, operating system, or this is app will fail *way* before you even come close to reaching the target!" +
                        "\n\nBut if you are actually going to do this, please tell me how long this app survived.",
                        "This isn't Stargate and your PC won't stand the test of time!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch { }

            ExceptionHandler.Log("Ran checks: " + errTracker.ToString());
            checkResult = errMessage;
            return errTracker;
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
            ExceptionHandler.Log("Saving settings...");

            if (SettingsProvider.SettingsLoaded)
            {
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
            ExceptionHandler.Log("Starting countdown...");

            // Calculate TimeSpan
            ExceptionHandler.Log("Calculating timespan");
            TimeSpan timeSpan;
            if (countdownModeRadioButton.Checked)
            {
                // Calculate TimeSpan for Countdown as it was given in the form
                timeSpan = new TimeSpan(Convert.ToInt32(hoursNumericUpDown.Value), Convert.ToInt32(minutesNumericUpDown.Value), Convert.ToInt32(secondsNumericUpDown.Value));
            }
            else
            {
                // Use form data as a point in time and calculate TimeSpan from now to this point
                bool today = TodayOrTomorrow(Convert.ToInt32(hoursNumericUpDown.Value), Convert.ToInt32(minutesNumericUpDown.Value), Convert.ToInt32(secondsNumericUpDown.Value));
                DateTime target = DateTime.Parse(Convert.ToInt32(hoursNumericUpDown.Value) + ":" + Convert.ToInt32(minutesNumericUpDown.Value) + ":" + Convert.ToInt32(secondsNumericUpDown.Value));
                if (!today) { target = target.AddDays(1); }
                timeSpan = target.Subtract(DateTime.Now);
            }

            Timer.CountdownTimeSpan = timeSpan;
            Timer.Action = actionComboBox.Text;
            Timer.Graceful = gracefulCheckBox.Checked;
            Timer.PreventSystemSleep = preventSleepCheckBox.Checked;
            Timer.Command = command;

            // Show countdown window
            ExceptionHandler.Log("Starting timer...");
            Timer.Start(password, !backgroundCheckBox.Checked);
        }

        /// <summary>
        /// determine if a given set of hours, minutes, and seconds regards a time of the current day or the next day
        /// </summary>
        /// <returns>true if today, false if tomorrow</returns>
        private bool TodayOrTomorrow(int hours, int minutes, int seconds)
        {
            DateTime now = DateTime.Now;
            DateTime target = new DateTime(now.Year, now.Month, now.Day, hours, minutes, seconds);

            if (target < now) { return false; } // specified time is in the past (for the current day) -> for tomorrow
            else { return true; } // specified time is in the future -> for today
        }
    }
}
