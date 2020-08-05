using ShutdownTimer.Helpers;
using System;
using System.Windows.Forms;

namespace ShutdownTimer
{
    public partial class Menu : Form
    {
        private readonly string[] startupArgs;
        private string CheckResult;

        public Menu(string[] args)
        {
            InitializeComponent();
            startupArgs = args;
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            versionLabel.Text = "v" + Application.ProductVersion.Remove(Application.ProductVersion.LastIndexOf(".")); // Display current version
            infoToolTip.SetToolTip(gracefulCheckBox, "Applications that do not exit when prompted automatically get terminated by default to ensure a successful shutdown." +
                "\n\nA graceful shutdown on the other hand will wait for all applications to exit before continuing with the shutdown." +
                "\nThis might result in an unsuccessful shutdown if one or more applications are unresponsive or require a user interaction to exit!");
            infoToolTip.SetToolTip(preventSleepCheckBox, "Depending on the power settings of your system, it might go to sleep after certain amount of time due to inactivity." +
                "\nThis option will keep the system awake to ensure the timer can properly run and execute a shutdown.");
            infoToolTip.SetToolTip(backgroundCheckBox, "This will launch the countdown without a visible window but will show a tray icon in your taskbar.");
            infoToolTip.SetToolTip(hoursNumericUpDown, "This defines the hours to count down from. Use can use any positive whole number.");
            infoToolTip.SetToolTip(minutesNumericUpDown, "This defines the minutes to count down from. Use can use any positive whole number.\nValues above 59 will get converted into their corresponding seconds, minutes and hours.");
            infoToolTip.SetToolTip(secondsNumericUpDown, "This defines the seconds to count down from. Use can use any positive whole number.\nValues above 59 will get converted into their corresponding seconds, minutes and hours.");

            // Check for startup arguments
            if (startupArgs.Length > 0) { ProcessArgs(); }
        }

        private void titleLabel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE." +
                    "\n\nBy using this software you agree to the above mentioned terms as this software is licensed under the MIT License. For more information visit: https://opensource.org/licenses/MIT.", "MIT License", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void githubPictureBox_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Lukas34/ShutdownTimerClassic"); // Show GitHub page
        }

        private void actionComboBox_TextChanged(object sender, EventArgs e)
        {
            // disables graceful checkbox for all modes which can not be executed gracefully / which always execute gracefully
            if (actionComboBox.Text == "Shutdown" || actionComboBox.Text == "Restart" || actionComboBox.Text == "Logout") { gracefulCheckBox.Enabled = true; }
            else { gracefulCheckBox.Enabled = false; }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (RunChecks() == true)
            {
                // Disable controls
                startButton.Enabled = false;
                actionGroupBox.Enabled = false;
                timeGroupBox.Enabled = false;

                // Hide
                this.Hide();

                // Start countdown
                StartCountdown();
            }
            else
            {
                MessageBox.Show("The following error(s) occurred:\n\n" + CheckResult + "Please try to resolve the(se) problem(s) and try again.", "There seems to be a problem!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Checks user input before further processing
        /// </summary>
        /// <returns>Result of checks</returns>
        private bool RunChecks()
        {
            bool err_tracker = true; // if anything goes wrong the tracker will be set to false
            string err_message = ""; // error messages will append to this

            if (!actionComboBox.Items.Contains(actionComboBox.Text))
            {
                err_tracker = false;
                err_message += "Please select a valid action from the dropdown menu!\n\n";
            }

            if (hoursNumericUpDown.Value == 0 && minutesNumericUpDown.Value == 0 && secondsNumericUpDown.Value == 0)
            {
                err_tracker = false;
                err_message += "The timer cannot start at 0!\n\n";
            }

            try
            {
                Numerics.ConvertTime(Convert.ToInt32(hoursNumericUpDown.Value), Convert.ToInt32(minutesNumericUpDown.Value), Convert.ToInt32(secondsNumericUpDown.Value));
            }
            catch
            {
                err_tracker = false;
                err_message += "Time conversion failed! Please check if your time values are within a reasonable range.\n\n";
            }

            CheckResult = err_message;
            return err_tracker;
        }

        /// <summary>
        /// Starts the countdown with values from UI
        /// </summary>
        /// <param name="pStatus"></param>
        private void StartCountdown(string pStatus = null)
        {
            // Calculate TimeSpan
            TimeSpan timeSpan = new TimeSpan(Convert.ToInt32(hoursNumericUpDown.Value), Convert.ToInt32(minutesNumericUpDown.Value), Convert.ToInt32(secondsNumericUpDown.Value));

            // Show countdown window
            using (Countdown countdown = new Countdown
            {
                countdownTimeSpan = timeSpan,
                action = actionComboBox.Text,
                graceful = gracefulCheckBox.Checked,
                preventSystemSleep = preventSleepCheckBox.Checked,
                UI = !backgroundCheckBox.Checked,
                status = pStatus
            })
            {
                countdown.Owner = this;
                countdown.ShowDialog();
                Application.Exit(); // Exit application after countdown is closed
            }
        }

        /// <summary>
        /// Read application's startup arguments and process events
        /// </summary>
        private void ProcessArgs()
        {
            string timeArg = null;
            string controlMode = "Recommend"; // Use recommend control mode by default
            //Control Modes:
            //Takeover:     Overrides settings and starts the timer
            //Lock:         Overrides settings and locks UI controls but lets user control wether or not to start the timer
            //Recommend:    Prepopulates settings but leaves the UI unlocked

            // Read args and do some processing
            for (var i = 0; i < startupArgs.Length; i++)
            {
                switch (startupArgs[i])
                {
                    case "/SetTime":
                        timeArg = startupArgs[i + 1];
                        break;

                    case "/SetAction":
                        if (!string.IsNullOrWhiteSpace(startupArgs[i + 1])) { actionComboBox.Text = startupArgs[i + 1]; }
                        break;

                    case "/Graceful":
                        gracefulCheckBox.Checked = true;
                        break;

                    case "/AllowSleep":
                        preventSleepCheckBox.Checked = false;
                        break;

                    case "/Background":
                        backgroundCheckBox.Checked = true;
                        break;

                    case "/Takeover":
                        controlMode = "Takeover";
                        break;

                    case "/Lock":
                        controlMode = "Lock";
                        break;

                    case "/Recommend":
                        controlMode = "Recommend";
                        break;
                }
            }

            // Process time arg
            if (!string.IsNullOrWhiteSpace(timeArg))
            {
                if (!timeArg.Contains(":"))
                {
                    // time in seconds
                    secondsNumericUpDown.Value = Convert.ToDecimal(timeArg);
                }
                else
                {
                    string[] splittedTimeArg = timeArg.Split(':');
                    int count = splittedTimeArg.Length - 1; // Count number of colons
                    switch (count)
                    {
                        case 0:
                            MessageBox.Show("StartupArgs Error: Please provide a valid argument after /SetTime", "Invalid argument", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;

                        case 1:
                            // Assuming HH:mm
                            hoursNumericUpDown.Value = Convert.ToDecimal(splittedTimeArg[0]);
                            minutesNumericUpDown.Value = Convert.ToDecimal(splittedTimeArg[1]);
                            break;

                        case 2:
                            // Assuming HH:mm:ss
                            hoursNumericUpDown.Value = Convert.ToDecimal(splittedTimeArg[0]);
                            minutesNumericUpDown.Value = Convert.ToDecimal(splittedTimeArg[1]);
                            secondsNumericUpDown.Value = Convert.ToDecimal(splittedTimeArg[2]);
                            break;
                    }
                }
            }

            // Process control mode
            switch (controlMode)
            {
                case "Takeover":
                    StartCountdown("Externally initiated countdown!");
                    break;

                case "Lock":
                    actionGroupBox.Enabled = false;
                    timeGroupBox.Enabled = false;
                    statusLabel.Text = "Interface has been prefilled and locked";
                    statusLabel.Visible = true;
                    break;

                case "Recommend":
                    actionGroupBox.Enabled = true;
                    timeGroupBox.Enabled = true;
                    statusLabel.Text = "Recommended settings have been prefilled";
                    statusLabel.Visible = true;
                    break;
            }
        }
    }
}
