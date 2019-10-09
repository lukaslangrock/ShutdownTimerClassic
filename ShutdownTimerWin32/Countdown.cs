using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace ShutdownTimerWin32
{
    public partial class Countdown : Form
    {
        public int hours = 0;
        public int minutes = 0;
        public int seconds = 0;
        public string method = "Shutdown";
        public bool UI = true;
        private bool allow_close = false;
        private bool animation_switch = false;

        public Countdown()
        {
            InitializeComponent();
        }

        private void Countdown_Load(object sender, EventArgs e)
        {
            if (UI == true) { UpdateUI(); } // initial time label update
            else
            {
                time_label.Text = "Interface disabled";
                UpdateNameText();
                this.TopMost = false;
                this.ShowInTaskbar = false;
                this.MinimizeBox = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void UpdateUI()
        {
            // Temporary variables
            string temp_hours;
            string temp_minutes;
            string temp_seconds;

            // Add 0 before single digit numbers
            if (hours < 10) { temp_hours = "0" + hours.ToString(); }
            else { temp_hours = hours.ToString(); }
            if (minutes < 10) { temp_minutes = "0" + minutes.ToString(); }
            else { temp_minutes = minutes.ToString(); }
            if (seconds < 10) { temp_seconds = "0" + seconds.ToString(); }
            else { temp_seconds = seconds.ToString(); }

            // Update time label
            string seperator = ":";
            time_label.Text = temp_hours + seperator + temp_minutes + seperator + temp_seconds;

            // Decide what color/animation to use
            if (hours > 0 || minutes >= 30) { BackColor = Color.ForestGreen; }
            else if (minutes >= 10) { BackColor = Color.DarkOrange; }
            else if (minutes >= 1) { BackColor = Color.OrangeRed; }
            else { Warning_Animation(); }

            // Update UI
            Application.DoEvents();
        }

        private void UpdateNameText()
        {
            // Temporary variables
            string temp_hours;
            string temp_minutes;
            string temp_seconds;

            // Add 0 before single digit numbers
            if (hours < 10) { temp_hours = "0" + hours.ToString(); }
            else { temp_hours = hours.ToString(); }
            if (minutes < 10) { temp_minutes = "0" + minutes.ToString(); }
            else { temp_minutes = minutes.ToString(); }
            if (seconds < 10) { temp_seconds = "0" + seconds.ToString(); }
            else { temp_seconds = seconds.ToString(); }

            // Update time label
            string seperator = ":";
            this.Text = "Countdown - " + temp_hours + seperator + temp_minutes + seperator + temp_seconds;
        }

        private void Warning_Animation()
        {
            if (animation_switch == true) { BackColor = Color.Red; animation_switch = false; }
            else if (animation_switch == false) { BackColor = Color.Black; animation_switch = true; }
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
                string message = "Do you really want to cancel the shutdown timer?";
                DialogResult question = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (question == DialogResult.Yes)
                {
                    allow_close = true;
                    counter.Stop();
                    string caption2 = "Shutdown canceled";
                    string message2 = "Your shutdown timer was canceled successfully!\nThe application will now close.";
                    MessageBox.Show(message2, caption2, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
            }
        }

        private void Counter_Tick(object sender, EventArgs e)
        {
            if (seconds == 1 && minutes == 0 && hours == 0)
            {
                // Target reached

                counter.Stop();
                seconds = 0;
                minutes = 0;
                hours = 0;
                UpdateUI();
                ShutdownWindows(method);
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
                            counter.Stop();
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
                else { UpdateNameText(); } // else update only the application name
            }
        }

        public void ShutdownWindows(string ChoosenMethod)
        {
            allow_close = true; // Disable close question
            switch (ChoosenMethod)
            {
                case "Shutdown":
                    Process.Start("shutdown", "/s /f");
                    break;

                case "Restart":
                    Process.Start("shutdown", "/r /f");
                    break;

                case "Hibernate":
                    Application.SetSuspendState(PowerState.Hibernate, true, true);
                    break;

                case "Sleep":
                    Application.SetSuspendState(PowerState.Suspend, true, true);
                    break;

                case "Logout":
                    Process.Start("shutdown", "/l /f");
                    break;

                case "Lock":
                    Process.Start("rundll32", "user32.dll LockWorkStation");
                    break;
            }
        }
    }
}
