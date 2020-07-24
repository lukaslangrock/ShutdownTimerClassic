using System;
using System.Drawing;
using System.Windows.Forms;
using ShutdownTimerWin32.Helpers;

namespace ShutdownTimerWin32
{
    public partial class Countdown : Form
    {
        public int hours = 0;
        public int minutes = 0;
        public int seconds = 0;
        private int restoreHours;
        private int restoreMinutes;
        private int restoreSeconds;
        public string action = "Shutdown"; // defines what power action to execute (fallback to shutdown if not changed)
        public bool graceful; // uses a graceful shutdown which allows apps to save their work or interrupt the shutdown
        public bool preventSystemSleep; // tells Windows that the system should stay awake during countdown
        public bool UI = true; // disables UI updates when set to false (used for running in background)
        private bool allow_close = false; // if false displays a 'are you sure' message box when closing.
        private bool animation_switch = false; // used to switch background colors

        public Countdown()
        {
            InitializeComponent();
        }

        private void Countdown_Load(object sender, EventArgs e)
        {
            // store original values for timer restart
            restoreHours = hours;
            restoreMinutes = minutes;
            restoreSeconds = seconds;

            if (UI == true) { UpdateUI(); title_label.Text = action + " Timer"; } // initial label update
            else // prepares window for running in background
            {
                time_label.Text = "Interface disabled";
                UpdateBackgroundUI();
                this.TopMost = false;
                this.ShowInTaskbar = false;
                this.MinimizeBox = true;
                this.WindowState = FormWindowState.Minimized;
                notifyIcon.BalloonTipText = "Timer started. The power action will be executed in " + hours + " hours, " + minutes + " minutes and " + seconds + " seconds.";
                notifyIcon.ShowBalloonTip(10000);
            }

            if (preventSystemSleep) { ExecutionState.SetThreadExecutionState(ExecutionState.EXECUTION_STATE.ES_CONTINUOUS | ExecutionState.EXECUTION_STATE.ES_SYSTEM_REQUIRED); } // give the system some coffee so it stays awake when tired using some fancy EXECUTION_STATE flags
        }

        /// <summary>
        /// Updates the time label with current time left and applies the corresponding background color.
        /// </summary>
        private void UpdateUI()
        {
            // Temporary variables
            string temp_hours;
            string temp_minutes;
            string temp_seconds;

            // Add 0 before single digit numbers
            temp_hours = Numerics.AddZeros(hours);
            temp_minutes = Numerics.AddZeros(minutes);
            temp_seconds = Numerics.AddZeros(seconds);

            // Update time labels
            string seperator = ":";
            time_label.Text = temp_hours + seperator + temp_minutes + seperator + temp_seconds;
            timeMenuItem.Text = temp_hours + seperator + temp_minutes + seperator + temp_seconds;

            // Decide what color/animation to use
            if (hours > 0 || minutes >= 30) { BackColor = Color.ForestGreen; }
            else if (minutes >= 10) { BackColor = Color.DarkOrange; }
            else if (minutes >= 1) { BackColor = Color.OrangeRed; }
            else { WarningAnimation(); }

            // Update UI
            Application.DoEvents();
        }

        /// <summary>
        /// Updates the tray icon and the application name text so the user can see the time left in the Task-Manager when the application runs in the background.
        /// </summary>
        private void UpdateBackgroundUI()
        {
            // Temporary variables
            string temp_hours;
            string temp_minutes;
            string temp_seconds;

            // Add 0 before single digit numbers
            temp_hours = Numerics.AddZeros(hours);
            temp_minutes = Numerics.AddZeros(minutes);
            temp_seconds = Numerics.AddZeros(seconds);

            // Update time labels
            string seperator = ":";
            this.Text = "Countdown - " + temp_hours + seperator + temp_minutes + seperator + temp_seconds;
            timeMenuItem.Text = temp_hours + seperator + temp_minutes + seperator + temp_seconds;

            // Decide what tray message to show
            if (hours == 2 && minutes == 0 && seconds == 00) { notifyIcon.BalloonTipText = "2 hours remaining until the power action will be executed."; notifyIcon.ShowBalloonTip(5000); }
            else if (hours == 1 && minutes == 0 && seconds == 00) { notifyIcon.BalloonTipText = "1 hour remaining until the power action will be executed."; notifyIcon.ShowBalloonTip(5000); }
            else if (hours == 0 && minutes == 30 && seconds == 00) { notifyIcon.BalloonTipText = "30 minutes remaining until the power action will be executed."; notifyIcon.ShowBalloonTip(5000); }
            else if (hours == 0 && minutes == 5 && seconds == 00) { notifyIcon.BalloonTipText = "5 minutes remaining until the power action will be executed."; notifyIcon.ShowBalloonTip(5000); }
            else if (hours == 0 && minutes == 0 && seconds == 30) { notifyIcon.BalloonTipText = "30 seconds remaining until the power action will be executed."; notifyIcon.ShowBalloonTip(5000); }
        }

        /// <summary>
        /// Switches from background color from red to black (and vice versa) when called.
        /// </summary>
        private void WarningAnimation()
        {
            if (animation_switch == true) { BackColor = Color.Red; animation_switch = false; }
            else if (animation_switch == false) { BackColor = Color.Black; animation_switch = true; }
        }

        /// <summary>
        /// Stops the countdown, displays an exit message and closes exits the application.
        /// </summary>
        private void ExitApplication()
        {
            allow_close = true;
            counterTimer.Stop();
            ExecutionState.SetThreadExecutionState(ExecutionState.EXECUTION_STATE.ES_CONTINUOUS); // clear EXECUTION_STATE flags to allow the system to go to sleep if it's tired
            string caption1 = "Timer canceled";
            string message1 = "Your timer was canceled successfully!\nThe application will now close.";
            MessageBox.Show(message1, caption1, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Exit();
        }

        /// <summary>
        /// Restarts the application.
        /// </summary>
        private void RestartApplication()
        {
            allow_close = true;
            counterTimer.Stop();
            ExecutionState.SetThreadExecutionState(ExecutionState.EXECUTION_STATE.ES_CONTINUOUS); // clear EXECUTION_STATE flags to allow the system to go to sleep if it's tired
            Application.DoEvents();
            Application.Restart();
        }

        /// <summary>
        /// Restarts the countdown with initial values.
        /// </summary>
        private void RestartTimer()
        {
            counterTimer.Stop();
            hours = restoreHours;
            minutes = restoreMinutes;
            seconds = restoreSeconds;
            counterTimer.Start();
            if (UI == true) { UpdateUI(); } else { UpdateBackgroundUI(); }
            if (this.WindowState == FormWindowState.Minimized) { notifyIcon.BalloonTipText = "Timer restarted. The power action will be executed in " + hours + " hours, " + minutes + " minutes and " + seconds + " seconds."; notifyIcon.ShowBalloonTip(10000); }
        }

        private void Countdown_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UI == false && allow_close == false)
            {
                e.Cancel = true; // ignore closing attempts while in background to prevent message box
            }
            else if (allow_close == false)
            {
                e.Cancel = true;
                string caption = "Are you sure?";
                string message = "Do you really want to cancel the timer?";
                DialogResult question = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (question == DialogResult.Yes)
                {
                    ExitApplication();
                }
            }
        }

        /// <summary>
        /// Minimizes application to system tray
        /// </summary>
        private void Countdown_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon.Visible = true;
            }
        }

        #region "tray menu events"

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

        #endregion

        private void Counter_Tick(object sender, EventArgs e)
        {
            if (seconds == 1 && minutes == 0 && hours == 0)
            {
                // Target reached

                counterTimer.Stop();
                if (UI == true)
                {
                    seconds = 0;
                    minutes = 0;
                    hours = 0;
                }
                UpdateUI();
                ExecutePowerAction(action);
                Application.DoEvents();
                Application.Exit();
            }
            else // count down if target not reached
            {
                if (seconds == 0)
                {
                    if (minutes == 0)
                    {
                        if (hours == 0)
                        {
                            // This point should never be reached as it would mean that the counter
                            // already was on 00:00:00 and then counted another second so we would have
                            // counted one second over the desired target time.
                            // Although this should not be possible I integrated an error message anyways
                            counterTimer.Stop();
                            MessageBox.Show("You should never see this. How can you see this?", "WTF?", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        }
                        else
                        {
                            hours -= 1;
                            minutes = 59;
                            seconds = 59;
                        }
                    }
                    else
                    {
                        minutes -= 1;
                        seconds = 59;
                    }
                }
                else
                {
                    seconds -= 1;
                }
                if (UI == true) { UpdateUI(); } // only update UI if the application is actually shown
                else { UpdateBackgroundUI(); } // else update only the application name
            }
        }

        public void ExecutePowerAction(string ChoosenAction)
        {
            allow_close = true; // disable close question

            switch (ChoosenAction)
            {
                case "Shutdown":
                    ExitWindows.Shutdown(!graceful);
                    break;

                case "Restart":
                    ExitWindows.Reboot(!graceful);
                    break;

                case "Hibernate":
                    Application.SetSuspendState(PowerState.Hibernate, false, false);
                    break;

                case "Sleep":
                    Application.SetSuspendState(PowerState.Suspend, false, false);
                    break;

                case "Logout":
                    ExitWindows.LogOff(!graceful);
                    break;

                case "Lock":
                    ExitWindows.Lock();
                    break;
            }

            ExecutionState.SetThreadExecutionState(ExecutionState.EXECUTION_STATE.ES_CONTINUOUS); // clear EXECUTION_STATE flags to allow the system to go to sleep if it's tired.
        }
    }
}
