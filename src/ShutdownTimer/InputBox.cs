using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShutdownTimer
{
    public partial class InputBox : Form
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool PasswordMode { get; set; }

        public string ReturnValue { get; set; }

        public InputBox()
        {
            InitializeComponent();
        }

        private void InputBox_Load(object sender, EventArgs e)
        {
            Text = "Shutdown Timer - " + Title;
            titleLabel.Text = Title;
            messageLabel.Text = Message;
            if (PasswordMode) { inputTextBox.PasswordChar = Convert.ToChar("*"); }

            // Prevent font-fallback and subsequent layout issues. This application is currently only in english and doesn't require display of non-latin chacracters.
            this.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.ReturnValue = inputTextBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = !inputTextBox.Text.Equals("");
        }
    }
}
