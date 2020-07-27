namespace ShutdownTimerWin32
{
    partial class Countdown
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Countdown));
            this.timeLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.counterTimer = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.timerStopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerRestartMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appRestartMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.timeMenuItem = new System.Windows.Forms.ToolStripTextBox();
            this.titlebarPictureBox = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.titlebarPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // timeLabel
            // 
            this.timeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.timeLabel.BackColor = System.Drawing.Color.Transparent;
            this.timeLabel.Font = new System.Drawing.Font("Verdana", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLabel.ForeColor = System.Drawing.Color.White;
            this.timeLabel.Location = new System.Drawing.Point(12, 51);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(335, 86);
            this.timeLabel.TabIndex = 14;
            this.timeLabel.Text = "00:00:00";
            this.timeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.BackColor = System.Drawing.Color.Transparent;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(12, 9);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(215, 31);
            this.titleLabel.TabIndex = 15;
            this.titleLabel.Text = "Shutdown Timer";
            // 
            // counterTimer
            // 
            this.counterTimer.Enabled = true;
            this.counterTimer.Interval = 1000;
            this.counterTimer.Tick += new System.EventHandler(this.Counter_Tick);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipTitle = "Shutdown Timer";
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Shutdown Timer";
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timerStopMenuItem,
            this.timerRestartMenuItem,
            this.appRestartMenuItem,
            this.toolStripSeparator,
            this.timeMenuItem});
            this.contextMenuStrip.Name = "contextMenu";
            this.contextMenuStrip.Size = new System.Drawing.Size(181, 123);
            // 
            // timerStopMenuItem
            // 
            this.timerStopMenuItem.Name = "timerStopMenuItem";
            this.timerStopMenuItem.Size = new System.Drawing.Size(180, 22);
            this.timerStopMenuItem.Text = "Stop and exit";
            this.timerStopMenuItem.Click += new System.EventHandler(this.TimerStopMenuItem_Click);
            // 
            // timerRestartMenuItem
            // 
            this.timerRestartMenuItem.Name = "timerRestartMenuItem";
            this.timerRestartMenuItem.Size = new System.Drawing.Size(180, 22);
            this.timerRestartMenuItem.Text = "Restart the timer";
            this.timerRestartMenuItem.Click += new System.EventHandler(this.TimerRestartMenuItem_Click);
            // 
            // appRestartMenuItem
            // 
            this.appRestartMenuItem.Name = "appRestartMenuItem";
            this.appRestartMenuItem.Size = new System.Drawing.Size(180, 22);
            this.appRestartMenuItem.Text = "Restart the app";
            this.appRestartMenuItem.Click += new System.EventHandler(this.AppRestartMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(177, 6);
            // 
            // timeMenuItem
            // 
            this.timeMenuItem.Enabled = false;
            this.timeMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.timeMenuItem.Name = "timeMenuItem";
            this.timeMenuItem.ReadOnly = true;
            this.timeMenuItem.Size = new System.Drawing.Size(100, 23);
            // 
            // titlebarPictureBox
            // 
            this.titlebarPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.titlebarPictureBox.BackColor = System.Drawing.Color.White;
            this.titlebarPictureBox.Location = new System.Drawing.Point(12, 43);
            this.titlebarPictureBox.Name = "titlebarPictureBox";
            this.titlebarPictureBox.Size = new System.Drawing.Size(335, 5);
            this.titlebarPictureBox.TabIndex = 16;
            this.titlebarPictureBox.TabStop = false;
            // 
            // Countdown
            // 
            this.AccessibleDescription = "Shows the time left until power action gets executed.";
            this.AccessibleName = "Shutdown Timer Countdown";
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Alert;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.ClientSize = new System.Drawing.Size(359, 146);
            this.ContextMenuStrip = this.contextMenuStrip;
            this.Controls.Add(this.titlebarPictureBox);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.timeLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Countdown";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Countdown";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Countdown_FormClosing);
            this.Load += new System.EventHandler(this.Countdown_Load);
            this.Resize += new System.EventHandler(this.Countdown_Resize);
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.titlebarPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.PictureBox titlebarPictureBox;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Timer counterTimer;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem timerStopMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timerRestartMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripTextBox timeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appRestartMenuItem;
    }
}