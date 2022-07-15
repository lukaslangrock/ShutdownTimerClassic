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
        //properties
        public TimeSpan CountdownTimeSpan { get; set; } // timespan after which the power action gets executed
        public string Action { get; set; } // defines what power action to execute (fallback to shutdown if not changed)
        public bool Graceful { get; set; } // uses a graceful shutdown which allows apps to save their work or interrupt the shutdown
        public bool PreventSystemSleep { get; set; } // tells Windows that the system should stay awake during countdown
        public bool UI { get; set; } // disables UI updates when set to false (used for running in background)
        public bool Forced { get; set; } // disables all UI controls and exit dialogs
        public string Password { get; set; } // if value is not empty then a password will be required to change or disable the countdown
        public bool UserLaunch { get; set; } // false if launched from CLI
        public string Command { get; set; } // for executing a custom command instead of a power action

        //private
        private FormWindowState lastStateUIFormWindowState; // used to update UI immediately after WindowState change
        private TimeSpan lastStateUITimeSpan; // used to limit UI events that should only be executed once per second instead of once per update
        private Stopwatch stopwatch; // measures exact timespan
        private bool ignoreClose; // true: cancel close events without asking | false: default behaviour (if ignoreClose == true, allowClose will be ignored)
        private bool allowClose; // true: accept close without asking | false: Ask user to confirm closing
        private bool animationSwitch = true; // used to switch warning animation colors
        private bool paused = false; // used to pause/resume the timer
        private int lockState = 0; // used for Password Protection free/locked/unlocked
        private int logTimerCounter = 0; // used to log events every 10,000 timer ticks

        public Countdown()
        {
            InitializeComponent();
        }

        #region "form events"

        // entrypoint
        private void Countdown_Load(object sender, EventArgs e)
        {
            ExceptionHandler.LogEvent("[Countdown] Starting stopwatch");

            // Setup clock
            stopwatch = new Stopwatch();
            stopwatch.Start();

            ExceptionHandler.LogEvent("[Countdown] Preparing UI...");

            // Set custom background color / transparency

            if (SettingsProvider.Settings.DisableAnimations)
            {
                if (SettingsProvider.Settings.BackgroundColor == Color.Transparent)
                {
                    BackColor = Color.Black;
                    TransparencyKey = Color.Black;
                    FormBorderStyle = FormBorderStyle.None;
                } else
                {
                    BackColor = SettingsProvider.Settings.BackgroundColor;
                }
            }

            // Set trayIcon icon to the opposite of the selected theme
            bool lighttheme;

            if (SettingsProvider.Settings.TrayIconTheme == "Light") { lighttheme = true; }
            else if (SettingsProvider.Settings.TrayIconTheme == "Dark") { lighttheme = false; }
            else { lighttheme = WindowsAPIs.GetWindowsLightTheme(); }

            // When the dark theme is selected we are using the light icon to generate contrast (and vise versa), you wouldn't want a white icon on a white background.
            notifyIcon.Icon = lighttheme ? Properties.Resources.icon_dark : Properties.Resources.icon_light;

            // Load password and set the lock state
            if (!String.IsNullOrEmpty(Password))
            {
                ExceptionHandler.LogEvent("[Countdown] A password has been detected");
                ChangeLockState("locked");
            }

            // Setup UI
            titleLabel.Text = Action + " Timer";

            TopMost = !SettingsProvider.Settings.DisableAlwaysOnTop;

            if (!UI)
            {
                ignoreClose = true; // Disable close dialogs and ignore closing from form
                TopMost = false;
                ShowInTaskbar = false;
                WindowState = FormWindowState.Minimized;
                timerUIHideMenuItem.Enabled = false;
                timerUIShowMenuItem.Enabled = true;
                notifyIcon.BalloonTipText = "Timer started. The power action will be executed in " + CountdownTimeSpan.Hours + " hours, " + CountdownTimeSpan.Minutes + " minutes and " + CountdownTimeSpan.Seconds + " seconds.";
                notifyIcon.ShowBalloonTip(10000);
                Hide();
            }

            if (Forced)
            {
                ignoreClose = true;
                contextMenuStrip.Enabled = false;
            }

            if (!UserLaunch) // disable restart app menu button because it would also keep the CLI args so the countdown would restart so it's basically useless
            {
                appRestartMenuItem.Enabled = false;
            }

            ExceptionHandler.LogEvent("[Countdown] Prepared UI");

            ExceptionHandler.LogEvent("[Countdown] Updating UI");
            UpdateUI(CountdownTimeSpan);

            if (PreventSystemSleep)
            {
                ExceptionHandler.LogEvent("[Countdown] Executing sleep prevention by setting the EXECUTION_STATE flags");
                ExecutionState.SetThreadExecutionState(ExecutionState.EXECUTION_STATE.ES_CONTINUOUS | ExecutionState.EXECUTION_STATE.ES_SYSTEM_REQUIRED);
            } // give the system some coffee so it stays awake when tired using some fancy EXECUTION_STATE flags

            ExceptionHandler.LogEvent("[Countdown] Entering countdown sequence...");
        }

        /// <summary>
        /// Resize countdown text if enabled in settings
        /// </summary>
        private void Countdown_SizeChanged(object sender, EventArgs e)
        {
            // default window size: 375x185
            // default label font: 24pt

            if (SettingsProvider.Settings.AdaptiveCountdownTextSize)
            {
                if (Size.Width > 375 && Size.Height > 185)
                {
                    float autosize = ((Size.Width / 375) + (Size.Height / 185)) * 16;
                    timeLabel.Font = new Font(timeLabel.Font.FontFamily, autosize, FontStyle.Bold);
                }
                else
                {
                    timeLabel.Font = new Font(timeLabel.Font.FontFamily, 24, FontStyle.Bold);
                }
            }
        }

        /// <summary>
        /// Checks the stopwatch and updates UI every 100ms
        /// </summary>
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan interval = CountdownTimeSpan - stopwatch.Elapsed + new TimeSpan(0, 0, 1); // Calculate countdown (add 1 second for smooth start)
            UpdateUI(interval);

            if (interval.TotalSeconds <= 0) //check if interval is negative
            {
                ExceptionHandler.LogEvent("[Countdown] Countdown reached 0");
                stopwatch.Stop();
                refreshTimer.Stop();
                ExecutePowerAction(Action);
                Application.DoEvents();
                Application.Exit();
            }
        }

        /// <summary>
        /// Clicking on the lock-icon will allow the user to change the lock state
        /// </summary>
        private void LockStatePictureBox_Click(object sender, EventArgs e)
        {
            ExceptionHandler.LogEvent("[Countdown] User clicked on the lock icon");

            switch (lockState)
            {
                case 0: // lock state 'free': nothing needs to be done
                    ExceptionHandler.LogEvent("[Countdown] Error: lockState = free, the user should not be able to click the icon as it should be invisible and deactivated");
                    break;

                case 1: // lock state 'locked': ask user for password to unlock
                    ExceptionHandler.LogEvent("[Countdown] lockState = locked, requesting password for unlock from user");
                    UnlockUIByPassword();
                    break;

                case 2: // lock state 'unlocked': change lockstate to 'locked'
                    ExceptionHandler.LogEvent("[Countdown] lockState = unlocked, asking user for confirmation lock the UI");
                    DialogResult result = MessageBox.Show("Would you like to re-lock the countdown?", "Password Protection", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ExceptionHandler.LogEvent("[Countdown] User wanted to lock the UI");
                        ChangeLockState("locked");
                    }
                    else { ExceptionHandler.LogEvent("[Countdown] User wanted to keep the UI unlocked"); }
                    break;
            }
        }

        /// <summary>
        /// Logic to prevent or ignore unwanted close events and notify the user.
        /// </summary>
        private void Countdown_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExceptionHandler.LogEvent("[Countdown] FormClosing event triggered");
            if (ignoreClose)
            {
                e.Cancel = true; // Ignore closing event
                ExceptionHandler.LogEvent("[Countdown] Ignoring the close event due to ignoreClose");
            }
            else if (!allowClose)
            {
                e.Cancel = true;
                ExceptionHandler.LogEvent("[Countdown] Asking user for confirmation to exit and cancel the timer");
                string caption = "Are you sure?";
                string message = "Do you really want to cancel the timer?";
                DialogResult question = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (question == DialogResult.Yes) { ExceptionHandler.LogEvent("[Countdown] User wanted to exit"); ExitApplication(); }
            }
            // allowClose == true is not handled and if ignoreClose == false, the application will exit
        }

        #endregion

        #region "form actions / state changes"

        /// <summary>
        /// Stops the countdown, displays an exit message and closes exits the application.
        /// </summary>
        private void ExitApplication()
        {
            ExceptionHandler.LogEvent("[Countdown] Trying to exit the application");

            if (lockState == 1)
            {
                ExceptionHandler.LogEvent("[Countdown] Attempt canceled due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.LogEvent("[Countdown] Password protection passed, restarting exit sequence..."); ExitApplication(); }
            }
            else
            {
                ExceptionHandler.LogEvent("[Countdown] Exiting application...");

                ignoreClose = false;
                allowClose = true;
                ExceptionHandler.LogEvent("[Countdown] Stopping clocks");
                stopwatch.Stop();
                refreshTimer.Stop();
                ExceptionHandler.LogEvent("[Countdown] Clearing EXECUTION_STATE flags");
                ExecutionState.SetThreadExecutionState(ExecutionState.EXECUTION_STATE.ES_CONTINUOUS); // Clear EXECUTION_STATE flags to allow the system to go to sleep if it's tired
                ExceptionHandler.LogEvent("[Countdown] Confirming application halt to user");
                string caption1 = "Timer canceled";
                string message1 = "Your timer was canceled successfully!\nThe application will now close.";
                MessageBox.Show(message1, caption1, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ExceptionHandler.LogEvent("[Countdown] Exiting...");
                Application.Exit();
            }
        }

        /// <summary>
        /// Restarts the application.
        /// </summary>
        private void RestartApplication()
        {
            ExceptionHandler.LogEvent("[Countdown] Trying to restart the application");
            if (lockState == 1)
            {
                ExceptionHandler.LogEvent("[Countdown] Attempt canceled due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.LogEvent("[Countdown] Password protection passed, restarting restart sequence..."); RestartApplication(); }
            }
            else
            {
                ExceptionHandler.LogEvent("[Countdown] Restarting application...");

                ignoreClose = false;
                allowClose = true;
                ExceptionHandler.LogEvent("[Countdown] Stopping clocks");
                stopwatch.Stop();
                refreshTimer.Stop();
                ExceptionHandler.LogEvent("[Countdown] Clearing EXECUTION_STATE flags");
                ExecutionState.SetThreadExecutionState(ExecutionState.EXECUTION_STATE.ES_CONTINUOUS); // Clear EXECUTION_STATE flags to allow the system to go to sleep if it's tired
                ExceptionHandler.LogEvent("[Countdown] Restarting...");
                Application.Restart();
            }
        }

        /// <summary>
        /// Restarts the countdown with initial values.
        /// </summary>
        private void RestartTimer()
        {
            ExceptionHandler.LogEvent("[Countdown] Trying to restart the timer");
            if (lockState == 1)
            {
                ExceptionHandler.LogEvent("[Countdown] Attempt canceled due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.LogEvent("[Countdown] Password protection disabled, restarting timer restart sequence..."); RestartTimer(); }
            }
            else
            {
                ExceptionHandler.LogEvent("[Countdown] Restarting timer...");

                ExceptionHandler.LogEvent("[Countdown] Restarting clocks");
                stopwatch.Stop();
                stopwatch = new Stopwatch();
                stopwatch.Start();
                ExceptionHandler.LogEvent("[Countdown] Updating UI");
                UpdateUI(CountdownTimeSpan);
                if (this.WindowState == FormWindowState.Minimized) { notifyIcon.BalloonTipText = "Timer restarted. The power action will be executed in " + CountdownTimeSpan.Hours + " hours, " + CountdownTimeSpan.Minutes + " minutes and " + CountdownTimeSpan.Seconds + " seconds."; notifyIcon.ShowBalloonTip(10000); }
                ExceptionHandler.LogEvent("[Countdown] Timer restarted");
            }
        }

        /// <summary>
        /// Pauses/Resumes the countdown timer
        /// </summary>
        private void PauseResumeTimer()
        {
            if (!paused)
            {
                stopwatch.Stop();
                paused = true;
                refreshTimer.Enabled = false;
                contextMenuStrip.Items[0].Text = "Resume";
                titleLabel.Text = Action + " Timer (paused)";
            }
            else
            {
                stopwatch.Start();
                paused = false;
                refreshTimer.Enabled = true;
                contextMenuStrip.Items[0].Text = "Pause";
                titleLabel.Text = Action + " Timer";
            }
        }

        /// <summary>
        /// Updates the countdown with a new timespan
        /// </summary>
        private void UpdateTimer()
        {
            ExceptionHandler.LogEvent("[Countdown] Countdown update requested");
            using (var form = new InputBox())
            {
                form.Title = "Countdown Update";
                form.Message = "Enter new time for the countdown in the format of HH:mm:ss or HH:mm";
                TopMost = false;
                var result = form.ShowDialog();
                TopMost = !SettingsProvider.Settings.DisableAlwaysOnTop;
                if (form.ReturnValue == "")
                {
                    ExceptionHandler.LogEvent("[Countdown] Countdown update aborted due to no input");
                    MessageBox.Show("Operation aborted: You have not supplied a new time value!", "Countdown Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ExceptionHandler.LogEvent("[Countdown] Parsing supplied input");
                    try
                    {
                        
                        String[] values = form.ReturnValue.Split(':');
                        if (values.Length == 2) // HH:mm
                        {
                            int newHours = Convert.ToInt32(values[0]);
                            int newMinutes = Convert.ToInt32(values[1]);
                            CountdownTimeSpan = new TimeSpan(newHours, newMinutes, 0);
                            stopwatch.Restart();
                            ExceptionHandler.LogEvent("[Countdown] Countdown updated using HH:mm");
                        }
                        else if (values.Length == 3) // HH:mm:ss
                        {
                            int newHours = Convert.ToInt32(values[0]);
                            int newMinutes = Convert.ToInt32(values[1]);
                            int newSeconds = Convert.ToInt32(values[2]);
                            CountdownTimeSpan = new TimeSpan(newHours, newMinutes, newSeconds);
                            stopwatch.Restart();
                            ExceptionHandler.LogEvent("[Countdown] Countdown updated using HH:mm:ss");
                        }
                        else
                        {
                            ExceptionHandler.LogEvent("[Countdown] Countdown update aborted due to malformed input");
                            MessageBox.Show("Operation aborted: You have not supplied a valid time value!", "Countdown Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception)
                    {
                        ExceptionHandler.LogEvent("[Countdown] Countdown update aborted due to error in input processing");
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
            ExceptionHandler.LogEvent("[Countdown] Hiding UI");

            timerUIHideMenuItem.Enabled = false;
            timerUIShowMenuItem.Enabled = true;
            TopMost = false;
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            UI = false;
            ignoreClose = true; // Prevent closing (and closing dialog) after ShowInTaskbar changed
            TimeSpan currentTimeSpan = CountdownTimeSpan - stopwatch.Elapsed + new TimeSpan(0, 0, 1); // Calculate countdown (add 1 second for smooth start)
            UpdateUI(currentTimeSpan);

            notifyIcon.BalloonTipText = "Timer has been moved to the background. Right-click the tray icon for more info.";
            notifyIcon.ShowBalloonTip(10000);
        }

        /// <summary>
        /// Shows countdown window
        /// </summary>
        private void ShowUI()
        {
            ExceptionHandler.LogEvent("[Countdown] Showing UI");

            timerUIHideMenuItem.Enabled = true;
            timerUIShowMenuItem.Enabled = false;
            if (!SettingsProvider.Settings.DisableAlwaysOnTop) { TopMost = true; }
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            UI = true;
            ignoreClose = true; // Prevent closing (and closing dialog) after ShowInTaskbar changed

            // Re-Enable close question after the main thread has moved on and the close event raised from the this. ShowInTaskbar has been ignored
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.Sleep(100); // Waiting to make sure the main thread has moved on and successfully
                ignoreClose = false; // Re-Enable close question
            }).Start();

            TimeSpan currentTimeSpan = CountdownTimeSpan - stopwatch.Elapsed + new TimeSpan(0, 0, 1); // Calculate countdown (add 1 second for smooth start)
            UpdateUI(currentTimeSpan);
        }

        /// <summary>
        /// Lock / Unlock the UI and update the picturebox
        /// </summary>
        private void ChangeLockState(string newLockState)
        {
            ExceptionHandler.LogEvent("[Countdown] Switching to lockState: " + newLockState);

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
            ExceptionHandler.LogEvent("[Countdown] Unlock has been requested");

            string message;
            if (reasonBecauseOfAction)
            {
                message = "A password is required to unlock this countdown. Please enter your password to execute this action.\n" +
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
                    ExceptionHandler.LogEvent("[Countdown] Successfully unlocked");
                    ChangeLockState("unlocked");
                    return true;
                }
                else
                {
                    ExceptionHandler.LogEvent("[Countdown] Unlock has failed");
                    MessageBox.Show("Incorrect password!\n\nThis countdown will stay locked until the correct password has been entered.", "Password Protection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
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
            ExceptionHandler.LogEvent("[Countdown] Trying to pause/resume countdown");

            if (lockState == 1)
            {
                ExceptionHandler.LogEvent("[Countdown] Attempt halted due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.LogEvent("[Countdown] Password protection passed"); PauseResumeTimer(); }
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
            ExceptionHandler.LogEvent("[Countdown] Trying to update remaining time");

            if (lockState == 1)
            {
                ExceptionHandler.LogEvent("[Countdown] Attempt halted due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.LogEvent("[Countdown] Password protection passed"); UpdateTimer(); }
            }
            else
            {
                UpdateTimer();
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
        private void TimerRestartMenuItem_Click(object sender, EventArgs e)
        {
            RestartTimer();
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
            ExceptionHandler.LogEvent("[Countdown] Trying to hide the UI");

            if (lockState == 1)
            {
                ExceptionHandler.LogEvent("[Countdown] Attempt halted due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.LogEvent("[Countdown] Password protection passed"); HideUI(); }
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
            ExceptionHandler.LogEvent("[Countdown] Trying to show the UI");

            if (lockState == 1)
            {
                ExceptionHandler.LogEvent("[Countdown] Attempt halted due to password protection");
                if (UnlockUIByPassword(true)) { ExceptionHandler.LogEvent("[Countdown] Password protection passed"); ShowUI(); }
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
            // log current state every 10,000 ticks (~16 min.)
            if (logTimerCounter <= 0)
            {
                ExceptionHandler.LogEvent("[Countdown] Application still alive and counting down: " + Numerics.ConvertTimeSpanToString(ts));
                logTimerCounter = 10000;
            }
            else
            {
                logTimerCounter--;
            }

            // update UI
            if (Math.Round(lastStateUITimeSpan.TotalSeconds) != Math.Round(ts.TotalSeconds) || lastStateUITimeSpan.TotalSeconds <= 0 || lastStateUIFormWindowState != WindowState) // Only update if the seconds from the TimeSpan actually changed and when it first started
            {
                // Save current data to last state memory
                lastStateUITimeSpan = ts;
                lastStateUIFormWindowState = WindowState;

                // Update time labels
                string elapsedTime = Numerics.ConvertTimeSpanToString(ts);
                timeLabel.Text = elapsedTime;
                timeMenuItem.Text = elapsedTime;

                if (UI) // UI for countdown window
                {
                    this.Text = "Countdown";

                    // Decide which color/animation to use
                    if (!SettingsProvider.Settings.DisableAnimations)
                    {
                        if (ts.Days > 0 || ts.Hours > 0 || ts.Minutes >= 30) { BackColor = Color.ForestGreen; }
                        else if (ts.Minutes >= 10) { BackColor = Color.DarkOrange; }
                        else if (ts.Minutes >= 1) { BackColor = Color.OrangeRed; }
                        else { WarningAnimation(); }
                    }
                }
                else // UI for tray menu
                {
                    this.Text = "Countdown - " + elapsedTime;

                    // Decide which tray message to show
                    if (!SettingsProvider.Settings.DisableNotifications)
                    {
                        if (ts.Days == 0 && ts.Hours == 2 && ts.Minutes == 0 && ts.Seconds == 00) { notifyIcon.BalloonTipText = "2 hours remaining until the power action will be executed."; notifyIcon.ShowBalloonTip(5000); }
                        else if (ts.Days == 0 && ts.Hours == 1 && ts.Minutes == 0 && ts.Seconds == 00) { notifyIcon.BalloonTipText = "1 hour remaining until the power action will be executed."; notifyIcon.ShowBalloonTip(5000); }
                        else if (ts.Days == 0 && ts.Hours == 0 && ts.Minutes == 30 && ts.Seconds == 00) { notifyIcon.BalloonTipText = "30 minutes remaining until the power action will be executed."; notifyIcon.ShowBalloonTip(5000); }
                        else if (ts.Days == 0 && ts.Hours == 0 && ts.Minutes == 5 && ts.Seconds == 00) { notifyIcon.BalloonTipText = "5 minutes remaining until the power action will be executed."; notifyIcon.ShowBalloonTip(5000); }
                        else if (ts.Days == 0 && ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 30) { notifyIcon.BalloonTipText = "30 seconds remaining until the power action will be executed."; notifyIcon.ShowBalloonTip(5000); }
                    }
                }
            }

            // Correct window states if unexpected changes occur
            if (!UI && WindowState != FormWindowState.Minimized)
            {
                // Window is visible when UI is set to background operation
                ShowUI();
            }
            else if (UI && WindowState == FormWindowState.Minimized)
            {
                // Window is hidden when UI is set to foreground operation
                HideUI();
            }

            // Update UI
            Application.DoEvents();
        }

        /// <summary>
        /// Switches from background color from red to black (and vice versa) when called.
        /// </summary>
        private void WarningAnimation()
        {
            animationSwitch = !animationSwitch; // Switch animation color

            if (animationSwitch) { BackColor = Color.Red; }
            else { BackColor = Color.Black; }
        }

        #endregion

        private void ExecutePowerAction(string choosenAction)
        {
            ExceptionHandler.LogEvent("[Countdown] Executing power action");

            ignoreClose = false; // do not ignore close event
            allowClose = true; // disable close question

            switch (choosenAction)
            {
                case "Shutdown":
                    ExitWindows.Shutdown(!Graceful);
                    break;

                case "Restart":
                    ExitWindows.Reboot(!Graceful);
                    break;

                case "Hibernate":
                    Application.SetSuspendState(PowerState.Hibernate, false, false);
                    break;

                case "Sleep":
                    Application.SetSuspendState(PowerState.Suspend, false, false);
                    break;

                case "Logout":
                    ExitWindows.LogOff(!Graceful);
                    break;

                case "Lock":
                    ExitWindows.Lock();
                    break;

                case "Custom Command":
                    Process.Start(Command);
                    break;
            }

            if (PreventSystemSleep)
            {
                ExceptionHandler.LogEvent("[Countdown] Clearing EXECUTION_STATE flags");
                ExecutionState.SetThreadExecutionState(ExecutionState.EXECUTION_STATE.ES_CONTINUOUS); // Clear EXECUTION_STATE flags to allow the system to go to sleep if it's tired.
            }
        }
    }
}
