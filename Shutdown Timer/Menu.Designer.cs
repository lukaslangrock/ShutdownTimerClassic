namespace ShutdownTimer
{
    partial class Menu
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            this.hoursNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.minutesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.secondsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.titleLabel = new System.Windows.Forms.Label();
            this.titlebarPictureBox = new System.Windows.Forms.PictureBox();
            this.hoursLabel = new System.Windows.Forms.Label();
            this.minutesLabel = new System.Windows.Forms.Label();
            this.secondsLabel = new System.Windows.Forms.Label();
            this.timeGroupBox = new System.Windows.Forms.GroupBox();
            this.actionGroupBox = new System.Windows.Forms.GroupBox();
            this.preventSleepCheckBox = new System.Windows.Forms.CheckBox();
            this.gracefulCheckBox = new System.Windows.Forms.CheckBox();
            this.backgroundCheckBox = new System.Windows.Forms.CheckBox();
            this.actionLabel = new System.Windows.Forms.Label();
            this.actionComboBox = new System.Windows.Forms.ComboBox();
            this.startButton = new System.Windows.Forms.Button();
            this.versionLabel = new System.Windows.Forms.Label();
            this.infoToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusLabel = new System.Windows.Forms.Label();
            this.settingsButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.hoursNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minutesNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.secondsNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.titlebarPictureBox)).BeginInit();
            this.timeGroupBox.SuspendLayout();
            this.actionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // hoursNumericUpDown
            // 
            this.hoursNumericUpDown.Location = new System.Drawing.Point(12, 46);
            this.hoursNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.hoursNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.hoursNumericUpDown.Name = "hoursNumericUpDown";
            this.hoursNumericUpDown.Size = new System.Drawing.Size(67, 22);
            this.hoursNumericUpDown.TabIndex = 0;
            // 
            // minutesNumericUpDown
            // 
            this.minutesNumericUpDown.Location = new System.Drawing.Point(87, 46);
            this.minutesNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.minutesNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minutesNumericUpDown.Name = "minutesNumericUpDown";
            this.minutesNumericUpDown.Size = new System.Drawing.Size(67, 22);
            this.minutesNumericUpDown.TabIndex = 1;
            // 
            // secondsNumericUpDown
            // 
            this.secondsNumericUpDown.Location = new System.Drawing.Point(161, 46);
            this.secondsNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.secondsNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.secondsNumericUpDown.Name = "secondsNumericUpDown";
            this.secondsNumericUpDown.Size = new System.Drawing.Size(67, 22);
            this.secondsNumericUpDown.TabIndex = 2;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(16, 11);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(266, 40);
            this.titleLabel.TabIndex = 3;
            this.titleLabel.Text = "Shutdown Timer";
            this.titleLabel.Click += new System.EventHandler(this.TitleLabel_Click);
            // 
            // titlebarPictureBox
            // 
            this.titlebarPictureBox.BackColor = System.Drawing.Color.Black;
            this.titlebarPictureBox.Location = new System.Drawing.Point(24, 53);
            this.titlebarPictureBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.titlebarPictureBox.Name = "titlebarPictureBox";
            this.titlebarPictureBox.Size = new System.Drawing.Size(336, 6);
            this.titlebarPictureBox.TabIndex = 4;
            this.titlebarPictureBox.TabStop = false;
            // 
            // hoursLabel
            // 
            this.hoursLabel.AutoSize = true;
            this.hoursLabel.Location = new System.Drawing.Point(8, 26);
            this.hoursLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.hoursLabel.Name = "hoursLabel";
            this.hoursLabel.Size = new System.Drawing.Size(46, 17);
            this.hoursLabel.TabIndex = 5;
            this.hoursLabel.Text = "Hours";
            // 
            // minutesLabel
            // 
            this.minutesLabel.AutoSize = true;
            this.minutesLabel.Location = new System.Drawing.Point(83, 26);
            this.minutesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.minutesLabel.Name = "minutesLabel";
            this.minutesLabel.Size = new System.Drawing.Size(57, 17);
            this.minutesLabel.TabIndex = 6;
            this.minutesLabel.Text = "Minutes";
            // 
            // secondsLabel
            // 
            this.secondsLabel.AutoSize = true;
            this.secondsLabel.Location = new System.Drawing.Point(157, 26);
            this.secondsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.secondsLabel.Name = "secondsLabel";
            this.secondsLabel.Size = new System.Drawing.Size(63, 17);
            this.secondsLabel.TabIndex = 7;
            this.secondsLabel.Text = "Seconds";
            // 
            // timeGroupBox
            // 
            this.timeGroupBox.Controls.Add(this.hoursNumericUpDown);
            this.timeGroupBox.Controls.Add(this.secondsLabel);
            this.timeGroupBox.Controls.Add(this.hoursLabel);
            this.timeGroupBox.Controls.Add(this.minutesLabel);
            this.timeGroupBox.Controls.Add(this.minutesNumericUpDown);
            this.timeGroupBox.Controls.Add(this.secondsNumericUpDown);
            this.timeGroupBox.Location = new System.Drawing.Point(24, 236);
            this.timeGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.timeGroupBox.Name = "timeGroupBox";
            this.timeGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.timeGroupBox.Size = new System.Drawing.Size(336, 86);
            this.timeGroupBox.TabIndex = 8;
            this.timeGroupBox.TabStop = false;
            this.timeGroupBox.Text = "When to do it?";
            // 
            // actionGroupBox
            // 
            this.actionGroupBox.Controls.Add(this.preventSleepCheckBox);
            this.actionGroupBox.Controls.Add(this.gracefulCheckBox);
            this.actionGroupBox.Controls.Add(this.backgroundCheckBox);
            this.actionGroupBox.Controls.Add(this.actionLabel);
            this.actionGroupBox.Controls.Add(this.actionComboBox);
            this.actionGroupBox.Location = new System.Drawing.Point(24, 82);
            this.actionGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.actionGroupBox.Name = "actionGroupBox";
            this.actionGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.actionGroupBox.Size = new System.Drawing.Size(336, 146);
            this.actionGroupBox.TabIndex = 9;
            this.actionGroupBox.TabStop = false;
            this.actionGroupBox.Text = "What to do?";
            // 
            // preventSleepCheckBox
            // 
            this.preventSleepCheckBox.AutoSize = true;
            this.preventSleepCheckBox.Checked = true;
            this.preventSleepCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.preventSleepCheckBox.Location = new System.Drawing.Point(12, 85);
            this.preventSleepCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.preventSleepCheckBox.Name = "preventSleepCheckBox";
            this.preventSleepCheckBox.Size = new System.Drawing.Size(252, 21);
            this.preventSleepCheckBox.TabIndex = 13;
            this.preventSleepCheckBox.Text = "Prevent system from going to sleep";
            this.preventSleepCheckBox.UseVisualStyleBackColor = true;
            // 
            // gracefulCheckBox
            // 
            this.gracefulCheckBox.AutoSize = true;
            this.gracefulCheckBox.Location = new System.Drawing.Point(12, 57);
            this.gracefulCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gracefulCheckBox.Name = "gracefulCheckBox";
            this.gracefulCheckBox.Size = new System.Drawing.Size(246, 21);
            this.gracefulCheckBox.TabIndex = 12;
            this.gracefulCheckBox.Text = "Graceful (do not force close apps)";
            this.gracefulCheckBox.UseVisualStyleBackColor = true;
            // 
            // backgroundCheckBox
            // 
            this.backgroundCheckBox.AutoSize = true;
            this.backgroundCheckBox.Location = new System.Drawing.Point(12, 113);
            this.backgroundCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.backgroundCheckBox.Name = "backgroundCheckBox";
            this.backgroundCheckBox.Size = new System.Drawing.Size(150, 21);
            this.backgroundCheckBox.TabIndex = 11;
            this.backgroundCheckBox.Text = "Run in background";
            this.backgroundCheckBox.UseVisualStyleBackColor = true;
            // 
            // actionLabel
            // 
            this.actionLabel.AutoSize = true;
            this.actionLabel.Location = new System.Drawing.Point(8, 27);
            this.actionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.actionLabel.Name = "actionLabel";
            this.actionLabel.Size = new System.Drawing.Size(113, 17);
            this.actionLabel.TabIndex = 1;
            this.actionLabel.Text = "Select an action:";
            // 
            // actionComboBox
            // 
            this.actionComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.actionComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.actionComboBox.FormattingEnabled = true;
            this.actionComboBox.Items.AddRange(new object[] {
            "Shutdown",
            "Restart",
            "Hibernate",
            "Sleep",
            "Logout",
            "Lock"});
            this.actionComboBox.Location = new System.Drawing.Point(132, 23);
            this.actionComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.actionComboBox.Name = "actionComboBox";
            this.actionComboBox.Size = new System.Drawing.Size(195, 24);
            this.actionComboBox.TabIndex = 0;
            this.actionComboBox.Text = "Shutdown";
            this.actionComboBox.TextChanged += new System.EventHandler(this.ActionComboBox_TextChanged);
            // 
            // startButton
            // 
            this.startButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startButton.Location = new System.Drawing.Point(24, 330);
            this.startButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(336, 38);
            this.startButton.TabIndex = 10;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(311, 63);
            this.versionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(47, 17);
            this.versionLabel.TabIndex = 11;
            this.versionLabel.Text = "v0.0.0";
            // 
            // infoToolTip
            // 
            this.infoToolTip.AutoPopDelay = 30000;
            this.infoToolTip.InitialDelay = 500;
            this.infoToolTip.ReshowDelay = 100;
            this.infoToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.infoToolTip.ToolTipTitle = "Help";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.Color.MidnightBlue;
            this.statusLabel.Location = new System.Drawing.Point(20, 63);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(220, 17);
            this.statusLabel.TabIndex = 13;
            this.statusLabel.Text = "Status messages will appear here";
            this.statusLabel.Visible = false;
            // 
            // settingsButton
            // 
            this.settingsButton.BackgroundImage = global::ShutdownTimer.Properties.Resources.fa_cog_solid;
            this.settingsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.settingsButton.Location = new System.Drawing.Point(319, 11);
            this.settingsButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(41, 38);
            this.settingsButton.TabIndex = 14;
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
            // 
            // Menu
            // 
            this.AcceptButton = this.startButton;
            this.AccessibleDescription = "The main window where you can choose the time and power action.";
            this.AccessibleName = "Shutdown Timer";
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 383);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.actionGroupBox);
            this.Controls.Add(this.timeGroupBox);
            this.Controls.Add(this.titlebarPictureBox);
            this.Controls.Add(this.titleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Menu";
            this.ShowIcon = false;
            this.Text = "Shutdown Timer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Menu_FormClosing);
            this.Load += new System.EventHandler(this.Menu_Load);
            this.Shown += new System.EventHandler(this.Menu_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.hoursNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minutesNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.secondsNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.titlebarPictureBox)).EndInit();
            this.timeGroupBox.ResumeLayout(false);
            this.timeGroupBox.PerformLayout();
            this.actionGroupBox.ResumeLayout(false);
            this.actionGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown hoursNumericUpDown;
        private System.Windows.Forms.NumericUpDown minutesNumericUpDown;
        private System.Windows.Forms.NumericUpDown secondsNumericUpDown;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.PictureBox titlebarPictureBox;
        private System.Windows.Forms.Label hoursLabel;
        private System.Windows.Forms.Label minutesLabel;
        private System.Windows.Forms.Label secondsLabel;
        private System.Windows.Forms.GroupBox timeGroupBox;
        private System.Windows.Forms.GroupBox actionGroupBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label actionLabel;
        private System.Windows.Forms.ComboBox actionComboBox;
        private System.Windows.Forms.CheckBox backgroundCheckBox;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.CheckBox gracefulCheckBox;
        private System.Windows.Forms.CheckBox preventSleepCheckBox;
        private System.Windows.Forms.ToolTip infoToolTip;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button settingsButton;
    }
}

