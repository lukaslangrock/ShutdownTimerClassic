using System;
using System.Drawing;
using ShutdownTimer.Helpers;
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

            // Prevent font-fallback and subsequent layout issues. This application is currently only in English and doesn't require display of non-Latin characters.
            this.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            DpiScaling.CorrectInitialDpi(this);
            EnsureMessageFits();
        }

        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);
            // Child fonts finish scaling after the form's DPI-change event.
            if (IsHandleCreated && !Disposing && !IsDisposed)
            {
                BeginInvoke((Action)(() => { if (!IsDisposed) { EnsureMessageFits(); } }));
            }
        }

        private void EnsureMessageFits()
        {
            int requiredHeight = messageLabel.GetPreferredSize(new Size(messageLabel.Width, 0)).Height
                + (int)Math.Ceiling(2f * DeviceDpi / 96f);
            if (requiredHeight > messageLabel.Height)
            {
                // The label and the input/buttons retain their designer anchor distances.
                ClientSize = new Size(ClientSize.Width, ClientSize.Height + requiredHeight - messageLabel.Height);
            }
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
