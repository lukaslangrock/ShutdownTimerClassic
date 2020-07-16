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
            this.time_label = new System.Windows.Forms.Label();
            this.title_label = new System.Windows.Forms.Label();
            this.counterTimer = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.timerStopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerRestartMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appRestartMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.timeMenuItem = new System.Windows.Forms.ToolStripTextBox();
            this.titlebar_picture = new System.Windows.Forms.PictureBox();
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.titlebar_picture)).BeginInit();
            this.SuspendLayout();
            // 
            // time_label
            // 
            this.time_label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.time_label.BackColor = System.Drawing.Color.Transparent;
            this.time_label.Font = new System.Drawing.Font("Verdana", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.time_label.ForeColor = System.Drawing.Color.White;
            this.time_label.Location = new System.Drawing.Point(12, 51);
            this.time_label.Name = "time_label";
            this.time_label.Size = new System.Drawing.Size(335, 86);
            this.time_label.TabIndex = 14;
            this.time_label.Text = "00:00:00";
            this.time_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // title_label
            // 
            this.title_label.AutoSize = true;
            this.title_label.BackColor = System.Drawing.Color.Transparent;
            this.title_label.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title_label.ForeColor = System.Drawing.Color.White;
            this.title_label.Location = new System.Drawing.Point(12, 9);
            this.title_label.Name = "title_label";
            this.title_label.Size = new System.Drawing.Size(215, 31);
            this.title_label.TabIndex = 15;
            this.title_label.Text = "Shutdown Timer";
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
            this.notifyIcon.ContextMenuStrip = this.contextMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Shutdown Timer";
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timerStopMenuItem,
            this.timerRestartMenuItem,
            this.appRestartMenuItem,
            this.toolStripSeparator1,
            this.timeMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(162, 101);
            // 
            // timerStopMenuItem
            // 
            this.timerStopMenuItem.Name = "timerStopMenuItem";
            this.timerStopMenuItem.Size = new System.Drawing.Size(161, 22);
            this.timerStopMenuItem.Text = "Stop and exit";
            this.timerStopMenuItem.Click += new System.EventHandler(this.TimerStopMenuItem_Click);
            // 
            // timerRestartMenuItem
            // 
            this.timerRestartMenuItem.Name = "timerRestartMenuItem";
            this.timerRestartMenuItem.Size = new System.Drawing.Size(161, 22);
            this.timerRestartMenuItem.Text = "Restart the timer";
            this.timerRestartMenuItem.Click += new System.EventHandler(this.TimerRestartMenuItem_Click);
            // 
            // appRestartMenuItem
            // 
            this.appRestartMenuItem.Name = "appRestartMenuItem";
            this.appRestartMenuItem.Size = new System.Drawing.Size(161, 22);
            this.appRestartMenuItem.Text = "Restart the app";
            this.appRestartMenuItem.Click += new System.EventHandler(this.AppRestartMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(158, 6);
            // 
            // timeMenuItem
            // 
            this.timeMenuItem.Enabled = false;
            this.timeMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.timeMenuItem.Name = "timeMenuItem";
            this.timeMenuItem.ReadOnly = true;
            this.timeMenuItem.Size = new System.Drawing.Size(100, 23);
            // 
            // titlebar_picture
            // 
            this.titlebar_picture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.titlebar_picture.BackColor = System.Drawing.Color.White;
            this.titlebar_picture.Location = new System.Drawing.Point(12, 43);
            this.titlebar_picture.Name = "titlebar_picture";
            this.titlebar_picture.Size = new System.Drawing.Size(335, 5);
            this.titlebar_picture.TabIndex = 16;
            this.titlebar_picture.TabStop = false;
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
            this.ContextMenuStrip = this.contextMenu;
            this.Controls.Add(this.titlebar_picture);
            this.Controls.Add(this.title_label);
            this.Controls.Add(this.time_label);
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
            this.contextMenu.ResumeLayout(false);
            this.contextMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.titlebar_picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label time_label;
        private System.Windows.Forms.PictureBox titlebar_picture;
        private System.Windows.Forms.Label title_label;
        private System.Windows.Forms.Timer counterTimer;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem timerStopMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timerRestartMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox timeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appRestartMenuItem;
    }
}