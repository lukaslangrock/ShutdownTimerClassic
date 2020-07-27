using ShutdownTimerWin32.Helpers;
using System;
using System.Windows.Forms;

namespace ShutdownTimerWin32
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
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
        }

        private void titleLabel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE." +
                    "\n\nBy using this software you agree to the above mentioned terms as this software is licensed under the MIT License. For more information visit: https://opensource.org/licenses/MIT.", "MIT License", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void githubPictureBox_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Lukas34/ShutdownTimerWin32"); // Show GitHub page
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

                // Temporary variables
                int tempHours, tempMinutes, tempSeconds;

                // Convert time values if necessarry
                (tempHours, tempMinutes, tempSeconds) = Numerics.ConvertTime(Convert.ToInt32(hoursNumericUpDown.Value), Convert.ToInt32(minutesNumericUpDown.Value), Convert.ToInt32(secondsNumericUpDown.Value));

                // Show countdown
                using (Countdown countdown = new Countdown
                {
                    hours = tempHours,
                    minutes = tempMinutes,
                    seconds = tempSeconds,
                    action = actionComboBox.Text,
                    graceful = gracefulCheckBox.Checked,
                    preventSystemSleep = preventSleepCheckBox.Checked,
                    UI = !backgroundCheckBox.Checked
                })
                {
                    countdown.Owner = this;
                    countdown.ShowDialog();
                    Application.Exit(); // Exit application after countdown is closed
                }
            }
            else
            {
                MessageBox.Show("The following error(s) occurred:\n\n" + CheckResult + "Please try to resolve the(se) problem(s) and try again.", "There seems to be a problem!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CheckResult;

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
    }
}
