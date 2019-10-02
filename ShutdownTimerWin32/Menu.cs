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
            version_label.Text = "v" + Application.ProductVersion.Remove(Application.ProductVersion.LastIndexOf("."));
        }

        private void Github_pb_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Lukas34/ShutdownTimerWin32");
        }

        private void Start_button_Click(object sender, EventArgs e)
        {
            if (RunChecks() == true)
            {
                // Disable controls
                start_button.Enabled = false;
                method_group.Enabled = false;
                time_group.Enabled = false;

                // Hide
                this.Hide();

                // Show countdown
                using (Countdown countdown = new Countdown
                {
                    hours = Convert.ToInt32(hours_updown.Value),
                    minutes = Convert.ToInt32(minutes_updown.Value),
                    seconds = Convert.ToInt32(seconds_updown.Value),
                    method = method_combo.Text
                })
                {
                    if (background_check.Checked == true) { countdown.TopMost = false; countdown.WindowState = FormWindowState.Minimized; }
                    countdown.ShowDialog();
                }

                // Exit application after countdown is closed
                Application.Exit();
            }
            else
            {
                MessageBox.Show("The following error(s) occurred:\n\n" + CheckResult + "Please try to resolve the(se) problem(s) and try again.", "There seems to be a problem!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CheckResult;
        private bool RunChecks()
        {
            bool err_tracker = true; // if anything goes wrong the tracker will be set to false
            string err_message = ""; // error messages will append to this

            if (!method_combo.Items.Contains(method_combo.Text))
            {
                err_tracker = false;
                err_message += "Please select a valid method from the dropdown menu!\n\n";
            }

            if (hours_updown.Value == 0 && minutes_updown.Value == 0 && seconds_updown.Value == 0)
            {
                err_tracker = false;
                err_message += "The timer cannot start at 0!\n\n";
            }

            CheckResult = err_message;
            return err_tracker;
        }
    }
}
