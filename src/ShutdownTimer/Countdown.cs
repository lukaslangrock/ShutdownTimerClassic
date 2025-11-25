using ShutdownTimer.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ShutdownTimer
{
    public partial class Countdown : Form
    {
        public string Password { get; set; } // if value is not empty then a password will be required to change or disable the countdown
        public bool IsUserLaunched { get; set; } // false if launched from CLI
        public bool IsForegroundUI { get; set; } // disables form UI updates when set to false (used for running in background)
        public bool IsReadOnly { get; set; } // disables all UI controls and exit dialogs when true

        private TimeSpan lastStateUITimeSpan; // used to limit UI events that should only be executed once per second instead of once per update
        private bool ignoreClose; // true: cancel close events without asking | false: default behaviour (if ignoreClose == true, allowClose will be ignored)
        private bool allowClose; // true: accept close without asking | false: Ask user to confirm closing
        private int lockState = 0; // used for Password Protection free/locked/unlocked
        private int logTimerCounter = 0; // used to log events every 10,000 timer ticks

        public Countdown()
        {
            InitializeComponent();
        }

        public void UpdateExternal(TimeSpan ts)
        {
            this.Invoke(new Action(() => UpdateUI(ts)));
        }

        public void ExitExternal()
        {
            this.Invoke(new Action(() =>
            {
                ignoreClose = false;
                allowClose = true;
                this.Close();
            }));
        }

        #region "form events"

        // entrypoint
        private void Countdown_Load(object sender, EventArgs e)
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

            ExceptionHandler.Log("Starting stopwatch");
            Timer.Start();

            ExceptionHandler.Log("Preparing UI...");

            // Set custom background color / transparency

            if (SettingsProvider.Settings.DisableAnimations)
            {
                if (SettingsProvider.Settings.BackgroundColor == Color.Transparent)
                {
                    BackColor = Color.Black;
                    TransparencyKey = Color.Black;
                    FormBorderStyle = FormBorderStyle.None;
                }
                else
                {
                    BackColor = SettingsProvider.Settings.BackgroundColor;
                }
            }

            // Set trayIcon icon to the opposite of the selected theme
            bool lighttheme;

            if (SettingsProvider.Settings.TrayIconTheme == "Light") { lighttheme = true; }
            else if (SettingsProvider.Settings.TrayIconTheme == "Dark") { lighttheme = false; }
            else { lighttheme = WindowsAPIs.SystemUsesLightTheme(); }

            // When the dark theme is selected we are using the light icon to generate contrast (and vise versa), you wouldn't want a white icon on a white background.
            notifyIcon.Icon = lighttheme ? Properties.Resources.icon_dark : Properties.Resources.icon_light;

            // Load password and set the lock state
            if (!String.IsNullOrEmpty(Password))
            {
                ExceptionHandler.Log("A password has been detected");
                ChangeLockState("locked");
            }

            /// Setup UI

            titleLabel.Text = Timer.Action + " Timer";

            TopMost = !SettingsProvider.Settings.DisableAlwaysOnTop;

            // Prevent font-fallback and subsequent layout issues. This application is currently only in english and doesn't require display of non-latin chacracters.
            this.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);

            if (!IsForegroundUI)
            {
                ignoreClose = true; // Disable close dialogs and ignore closing from form
                TopMost = false;
                ShowInTaskbar = false;
                WindowState = FormWindowState.Minimized;
                timerUIHideMenuItem.Enabled = false;
                timerUIShowMenuItem.Enabled = true;
                SendNotification("Timer started. The power action will be executed in " + Timer.CountdownTimeSpan.Hours + " hours, " + Timer.CountdownTimeSpan.Minutes + " minutes and " + Timer.CountdownTimeSpan.Seconds + " seconds.");
                Hide();
            }

            if (IsReadOnly)
            {
                ignoreClose = true;
                contextMenuStrip.Enabled = false;
            }

            if (!IsUserLaunched) // disable restart app menu button because it would also keep the CLI args so the countdown would restart so it's basically useless
            {
                appRestartMenuItem.Enabled = false;
            }

            // Check if the user has opted to remember the last Countdown screen position,
            // and if LastScreenPosition is available (not null), apply its X and Y coordinates to position the form.

            if (SettingsProvider.Settings.RememberLastScreenPositionCountdown && SettingsProvider.Settings.LastScreenPositionCountdown != null)
            {
                this.Location = new Point(SettingsProvider.Settings.LastScreenPositionCountdown.X, SettingsProvider.Settings.LastScreenPositionCountdown.Y);
            }

            ExceptionHandler.Log("Prepared UI");

            ExceptionHandler.Log("Updating UI");
            UpdateUI(Timer.CountdownTimeSpan);

            ExceptionHandler.Log("Entering countdown sequence...");
        }

        /// <summary>
        /// Resize countdown text if enabled in settings
        /// </summary>
        private void Countdown_SizeChanged(object sender, EventArgs e)
        {
            if (SettingsProvider.Settings.AdaptiveCountdownTextSize)
            {
                int defaultWidth = 375;
                int defaultHeight = 185;
                int defaultFontSize = 24;

                float scaleX = Size.Width / defaultWidth;
                float scaleY = Size.Height / defaultHeight;
                float scaledSize = defaultFontSize * Math.Min(scaleX, scaleY);

                timeLabel.Font = new Font(timeLabel.Font.FontFamily, Math.Max(defaultFontSize, scaledSize), timeLabel.Font.Style);
            }
        }

        /// <summary>
        /// Clicking on the lock-icon will allow the user to change the lock state
        /// </summary>
        private void LockStatePictureBox_Click(object sender, EventArgs e)
        {
            ExceptionHandler.Log("User clicked on the lock icon");

            switch (lockState)
            {
                case 0: // lock state 'free': nothing needs to be done
                    ExceptionHandler.Log("Error: lockState = free, the user should not be able to click the icon as it should be invisible and deactivated");
                    break;

                case 1: // lock state 'locked': ask user for password to unlock
                    ExceptionHandler.Log("lockState = locked, requesting password for unlock from user");
                    UnlockUIByPassword();
                    break;

                case 2: // lock state 'unlocked': change lockstate to 'locked'
                    ExceptionHandler.Log("lockState = unlocked, asking user for confirmation lock the UI");
                    DialogResult result = MessageBox.Show("Would you like to re-lock the countdown?", "Password Protection", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ExceptionHandler.Log("User wanted to lock the UI");
                        ChangeLockState("locked");
                    }
                    else { ExceptionHandler.Log("User wanted to keep the UI unlocked"); }
                    break;
            }
        }

        /// <summary>
        /// Saves the last Countdown position
        /// </summary>
        private void SaveSettings()
        {
            ExceptionHandler.Log("Saving settings...");

            // Only store last screen position if setting is enabled and countdown is not in background
            if (SettingsProvider.Settings.RememberLastScreenPositionCountdown && IsForegroundUI)
            {
                SettingsProvider.Settings.LastScreenPositionCountdown = new LastScreenPosition
                {
                    X = this.Location.X,
                    Y = this.Location.Y
                };
            }

            SettingsProvider.Save();

            ExceptionHandler.Log("Settings saved");
        }

        /// <summary>
        /// Logic to prevent or ignore unwanted close events and notify the user.
        /// </summary>
        private void Countdown_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExceptionHandler.Log("FormClosing event triggered");
            if (ignoreClose) // Ignore closing event
            {
                e.Cancel = true;
                ExceptionHandler.Log("Ignoring the close event due to ignoreClose");
                return;
            }
            else if (!allowClose && !SettingsProvider.Settings.DisableNotifications) // Ask user for confirmation
            {
                e.Cancel = true;
                ExceptionHandler.Log("Asking user for confirmation to exit and cancel the timer");
                string caption = "Are you sure?";
                string message = "Do you really want to cancel the timer?";
                DialogResult question = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (question == DialogResult.Yes) { ExceptionHandler.Log("User wanted to exit"); ExitApplication(); return; }
            }
            else if (!allowClose) // Just close without asking first, as user has disabled confirmations in settings
            {
                e.Cancel = true;
                ExceptionHandler.Log("Exiting without asking user as defined per DisableNotifications-setting");
                ExitApplication();
                return;
            }
            // allowClose == true is not handled and if ignoreClose == false, the application will exit
            ExceptionHandler.Log("Form now closing");
            //ExceptionHandler.CreateAutoLogIfApplicable();
        }

        #endregion

        #region "form actions / state changes"

        /// <summary>
        /// Stops the countdown, displays an exit message and closes exits the application.
        /// Notice: This logic allows for a graceful exit and is therefore initially called by Countdown_FormClosing before redirecting there again through Application.Exit();
        /// </summary>
        private void ExitApplication()
        {
            ExceptionHandler.Log("Trying to exit the application");

            if (lockState == 1)
            {
                ExceptionHandler.Log("Attempt halted due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.Log("Password protection passed, restarting exit sequence..."); ExitApplication(); }
            }
            else
            {
                ExceptionHandler.Log("Exiting application...");

                ignoreClose = false;
                allowClose = true;
                ExceptionHandler.Log("Stopping timer");
                Timer.Pause();
                ExceptionHandler.Log("Confirming application halt to user");
                SendNotification("Your timer was canceled successfully!\nThe application will now close"); // Show windows toast notification as confirmation
                ExceptionHandler.Log("Saving all settings");
                SaveSettings();
                ExceptionHandler.Log("Exiting...");
                Application.Exit();
            }
        }

        /// <summary>
        /// Restarts the application.
        /// </summary>
        private void RestartApplication()
        {
            ExceptionHandler.Log("Trying to restart the application");
            if (lockState == 1)
            {
                ExceptionHandler.Log("Attempt canceled due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.Log("Password protection passed, restarting restart sequence..."); RestartApplication(); }
            }
            else
            {
                ExceptionHandler.Log("Restarting application...");

                ignoreClose = false;
                allowClose = true;
                ExceptionHandler.Log("Stopping timer");
                Timer.Pause();
                ExceptionHandler.Log("Clearing EXECUTION_STATE flags");
                ExecutionState.SetThreadExecutionState(ExecutionState.EXECUTION_STATE.ES_CONTINUOUS); // Clear EXECUTION_STATE flags to allow the system to go to sleep if it's tired
                ExceptionHandler.Log("Saving all settings");
                SaveSettings();
                ExceptionHandler.Log("Restarting...");
                Application.Restart();
            }
        }

        /// <summary>
        /// Resets the timer to initial values.
        /// </summary>
        private void ResetTimer()
        {

            ExceptionHandler.Log("Trying to reset the timer");
            if (lockState == 1)
            {
                ExceptionHandler.Log("Attempt canceled due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.Log("Password protection disabled, restarting timer reset sequence..."); ResetTimer(); }
            }
            else
            {
                ExceptionHandler.Log("Resetting clock");
                Timer.Reset();
                ExceptionHandler.Log("Updating UI");
                UpdateUI(Timer.CountdownTimeSpan);
                if (this.WindowState == FormWindowState.Minimized) { SendNotification("Timer has been reset. Remaining time until power action will be executed is " + Timer.CountdownTimeSpan.Hours + " hours, " + Timer.CountdownTimeSpan.Minutes + " minutes and " + Timer.CountdownTimeSpan.Seconds + " seconds."); }
            }
        }

        /// <summary>
        /// Pauses/Resumes the countdown timer
        /// </summary>
        private void PauseResumeTimer()
        {
            if (Timer.IsRunning())
            {
                Timer.Pause();
                contextMenuStrip.Items[0].Text = "Resume";
                titleLabel.Text = Timer.Action + " Timer (paused)";
            }
            else
            {
                Timer.Resume();
                contextMenuStrip.Items[0].Text = "Pause";
                titleLabel.Text = Timer.Action + " Timer";
            }
        }

        /// <summary>
        /// Shows a Dialog for setting a new countdown time
        /// </summary>
        private void ShowSetNewCountdownDialog()
        {
            ExceptionHandler.Log("User requested to set a new countdown");
            using (var form = new InputBox())
            {
                form.Title = "Set a new countdown";
                form.Message = "Enter new time for the countdown in the format of HH:mm:ss or HH:mm.\n\nThis will replace the current timer in place.";
                TopMost = false;
                form.ShowDialog();
                TopMost = !SettingsProvider.Settings.DisableAlwaysOnTop;
                if (form.DialogResult == DialogResult.None || form.DialogResult == DialogResult.Cancel)
                {
                    return;
                }
                else if (form.ReturnValue == "" || form.ReturnValue == null)
                {
                    ExceptionHandler.Log("Countdown update aborted due to no input");
                    MessageBox.Show("Operation aborted: You have not supplied a new time value!", "Countdown Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ExceptionHandler.Log("Parsing supplied input");
                    try
                    {
                        String[] values = form.ReturnValue.Split(':');
                        if (values.Length == 2) // HH:mm
                        {
                            int newHours = Convert.ToInt32(values[0]);
                            int newMinutes = Convert.ToInt32(values[1]);
                            Timer.CountdownTimeSpan = new TimeSpan(newHours, newMinutes, 0);
                            Timer.Reset();

                            ExceptionHandler.Log("Countdown updated using HH:mm");
                        }
                        else if (values.Length == 3) // HH:mm:ss
                        {
                            int newHours = Convert.ToInt32(values[0]);
                            int newMinutes = Convert.ToInt32(values[1]);
                            int newSeconds = Convert.ToInt32(values[2]);
                            Timer.CountdownTimeSpan = new TimeSpan(newHours, newMinutes, newSeconds);
                            Timer.Reset();

                            ExceptionHandler.Log("Countdown updated using HH:mm:ss");
                        }
                        else
                        {
                            ExceptionHandler.Log("Countdown update aborted due to malformed input");
                            MessageBox.Show("Operation aborted: You have not supplied a valid time value!", "Countdown Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception)
                    {
                        ExceptionHandler.Log("Countdown update aborted due to error in input processing");
                        MessageBox.Show("Operation aborted: You have either not supplied a valid time value or there was an internal error outside the scope of your input while processing it.", "Countdown Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Hides countdown window
        /// </summary>
        private void HideUI()
        {
            ExceptionHandler.Log("Hiding UI");

            timerUIHideMenuItem.Enabled = false;
            timerUIShowMenuItem.Enabled = true;
            TopMost = false;
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            IsForegroundUI = false;
            ignoreClose = true; // Prevent closing (and closing dialog) after ShowInTaskbar changed
            UpdateUI(Timer.GetTimeRemaining());
            SendNotification("Timer has been moved to the background. Right-click the tray icon for more info.");
        }

        /// <summary>
        /// Shows countdown window
        /// </summary>
        private void ShowUI()
        {
            ExceptionHandler.Log("Showing UI");

            timerUIHideMenuItem.Enabled = true;
            timerUIShowMenuItem.Enabled = false;
            if (!SettingsProvider.Settings.DisableAlwaysOnTop) { TopMost = true; }
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            IsForegroundUI = true;
            ignoreClose = true; // Prevent closing (and closing dialog) after ShowInTaskbar changed

            // Re-Enable close question after the main thread has moved on and the close event raised from the this. ShowInTaskbar has been ignored
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.Sleep(100); // Waiting to make sure the main thread has moved on and successfully
                ignoreClose = false; // Re-Enable close question
            }).Start();

            UpdateUI(Timer.GetTimeRemaining());
        }

        /// <summary>
        /// Lock / Unlock the UI and update the picturebox
        /// </summary>
        private void ChangeLockState(string newLockState)
        {
            ExceptionHandler.Log("Switching to lockState: " + newLockState);

            switch (newLockState)
            {
                case "free":
                    lockState = 0;
                    lockStatePictureBox.Image = null;
                    lockStatePictureBox.Visible = false;
                    lockStatePictureBox.Enabled = false;
                    break;

                case "locked":
                    lockState = 1;
                    lockStatePictureBox.Image = Properties.Resources.fa_lock_white;
                    lockStatePictureBox.Visible = true;
                    lockStatePictureBox.Enabled = true;
                    break;

                case "unlocked":
                    lockState = 2;
                    lockStatePictureBox.Image = Properties.Resources.fa_unlock_white;
                    lockStatePictureBox.Visible = true;
                    lockStatePictureBox.Enabled = true;
                    break;
            }
        }

        private bool UnlockUIByPassword(bool reasonBecauseOfAction = false)
        {
            ExceptionHandler.Log("Unlock has been requested");

            string message;
            if (reasonBecauseOfAction)
            {
                message = "This countdown has been protected with a password. Enter your password to release the lock.\n" +
                    "You can re-lock the countdown by clicking on the lock icon afterwards.";
            }
            else
            {
                message = "Enter your password to unlock this countdown.\n\n" +
                    "You can re-lock the countdown by clicking on the lock icon afterwards.";
            }

            using (var form = new InputBox())
            {
                form.Title = "Password Protection";
                form.Message = message;
                form.PasswordMode = true;
                TopMost = false;
                var result = form.ShowDialog();
                TopMost = !SettingsProvider.Settings.DisableAlwaysOnTop;
                if (form.ReturnValue == Password)
                {
                    ExceptionHandler.Log("Successfully unlocked");
                    ChangeLockState("unlocked");
                    return true;
                }
                else
                {
                    ExceptionHandler.Log("Unlock has failed");
                    MessageBox.Show("Incorrect password!\n\nThis countdown will stay locked until the correct password has been entered.", "Password Protection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        /// <summary>
        /// Sends a toast notification (or trymenu balloontip on legacy systems) to the user, unless it was disabled in settings
        /// </summary>
        /// <param name="message">Message content to be displayed in notification</param>
        private void SendNotification(string message)
        {
            if (!SettingsProvider.Settings.DisableNotifications)
            {
                ExceptionHandler.Log("Sending notification: " + message);
                notifyIcon.BalloonTipText = message;
                notifyIcon.ShowBalloonTip(5000);
            }
        }

        #endregion

        #region "tray menu events"

        /// <summary>
        /// Pause/Resume timer option in the tray menu
        /// 
        /// </summary>
        private void TimerPauseMenuItem_Click(object sender, EventArgs e)
        {
            ExceptionHandler.Log("Trying to pause/resume countdown");

            if (lockState == 1)
            {
                ExceptionHandler.Log("Attempt halted due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.Log("Password protection passed"); PauseResumeTimer(); }
            }
            else
            {
                PauseResumeTimer();
            }
        }

        /// <summary>
        /// Update time option in the tray menu
        /// </summary>
        private void UpdateTimeMenuItem_Click(object sender, EventArgs e)
        {
            ExceptionHandler.Log("Trying to set a new countdown");

            if (lockState == 1)
            {
                ExceptionHandler.Log("Attempt halted due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.Log("Password protection passed"); ShowSetNewCountdownDialog(); }
            }
            else
            {
                ShowSetNewCountdownDialog();
            }
        }

        /// <summary>
        /// Stop timer option in the tray menu
        /// </summary>
        private void TimerStopMenuItem_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }

        /// <summary>
        /// Restart timer option in the tray menu
        /// </summary>
        private void TimerResetMenuItem_Click(object sender, EventArgs e)
        {
            ResetTimer();
        }

        /// <summary>
        /// Restart application option in the tray menu
        /// </summary>
        private void AppRestartMenuItem_Click(object sender, EventArgs e)
        {
            RestartApplication();
        }

        /// <summary>
        /// Hide countdown window option in the tray menu
        /// </summary>
        private void TimerUIHideMenuItem_Click(object sender, EventArgs e)
        {
            ExceptionHandler.Log("Trying to hide the UI");

            if (lockState == 1)
            {
                ExceptionHandler.Log("Attempt halted due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.Log("Password protection passed"); HideUI(); }
            }
            else
            {
                HideUI();
            }
        }

        /// <summary>
        /// Show countdown window option in the tray menu
        /// </summary>
        private void TimerUIShowMenuItem_Click(object sender, EventArgs e)
        {
            ExceptionHandler.Log("Trying to show the UI");

            if (lockState == 1)
            {
                ExceptionHandler.Log("Attempt halted due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.Log("Password protection passed"); ShowUI(); }
            }
            else
            {
                ShowUI();
            }
        }

        #endregion

        #region "ui updates"

        /// <summary>
        /// Updates the time label with current time left and applies the corresponding background color.
        /// </summary>
        private void UpdateUI(TimeSpan ts)
        {
            if (logTimerCounter <= 0)
            {
                ExceptionHandler.Log("Heartbeat: The countdown reads " + Numerics.ConvertTimeSpanToString(ts) + " with memory usage at " + Format.BytesToString(Process.GetCurrentProcess().PrivateMemorySize64) + ". Next heartbeat in 100,000 ticks.");
                logTimerCounter = 100000;
            }
            else
            {
                logTimerCounter--;
            }

            if (Math.Truncate(lastStateUITimeSpan.TotalSeconds) != Math.Truncate(ts.TotalSeconds)) // only if a full second passed between updates
            {
                // Save current ts to last state memory
                lastStateUITimeSpan = ts;

                // display every started second for one second instead of switching directly to the next lower second. makes for smoother display of time.
                if (ts.Milliseconds > 0)
                {
                    ts = ts.Add(TimeSpan.FromSeconds(1));
                }

                // Update time labels
                string elapsedTime = Numerics.ConvertTimeSpanToString(ts);
                timeLabel.Text = elapsedTime;
                timeMenuItem.Text = elapsedTime;

                if (IsForegroundUI) // UI for countdown window
                {
                    this.Text = "Countdown";

                    // Decide which color/animation to use
                    if (!SettingsProvider.Settings.DisableAnimations)
                    {
                        if (ts.Days > 0 || ts.Hours > 0 || ts.Minutes >= 30) { BackColor = Color.ForestGreen; }
                        else if (ts.Minutes >= 10) { BackColor = Color.DarkOrange; }
                        else if (ts.Minutes >= 1) { BackColor = Color.OrangeRed; }
                        else
                        {
                            if (ts.Seconds % 2 == 0) { BackColor = Color.Red; }
                            else { BackColor = Color.Black; }
                        }
                    }
                }
                else // UI for tray menu
                {
                    this.Text = "Countdown - " + elapsedTime;

                    // Decide which tray message to show          
                    if (ts.Days == 0 && ts.Hours == 2 && ts.Minutes == 0 && ts.Seconds == 00) { SendNotification("2 hours remaining until the power action will be executed"); }
                    else if (ts.Days == 0 && ts.Hours == 1 && ts.Minutes == 0 && ts.Seconds == 00) { SendNotification("1 hour remaining until the power action will be executed."); }
                    else if (ts.Days == 0 && ts.Hours == 0 && ts.Minutes == 30 && ts.Seconds == 00) { SendNotification("30 minutes remaining until the power action will be executed."); }
                    else if (ts.Days == 0 && ts.Hours == 0 && ts.Minutes == 5 && ts.Seconds == 00) { SendNotification("5 minutes remaining until the power action will be executed."); }
                    else if (ts.Days == 0 && ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 30) { SendNotification("30 seconds remaining until the power action will be executed."); }
                }
            }

            // Correct window states if unexpected changes occur
            if (!IsForegroundUI && WindowState != FormWindowState.Minimized)
            {
                ExceptionHandler.Log("Application was set for background operation but form was visible, switching to foreground operation");
                ShowUI();
            }
            else if (IsForegroundUI && WindowState == FormWindowState.Minimized)
            {
                ExceptionHandler.Log("Application was set for foreground operation but form was minimized, switching to background operation");
                HideUI();
            }
        }
        #endregion
    }
}
