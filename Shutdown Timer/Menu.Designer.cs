namespace ShutdownTimerWin32
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
            this.hours_updown = new System.Windows.Forms.NumericUpDown();
            this.minutes_updown = new System.Windows.Forms.NumericUpDown();
            this.seconds_updown = new System.Windows.Forms.NumericUpDown();
            this.title_label = new System.Windows.Forms.Label();
            this.titlebar_picture = new System.Windows.Forms.PictureBox();
            this.hours_label = new System.Windows.Forms.Label();
            this.minutes_label = new System.Windows.Forms.Label();
            this.seconds_label = new System.Windows.Forms.Label();
            this.time_group = new System.Windows.Forms.GroupBox();
            this.action_group = new System.Windows.Forms.GroupBox();
            this.preventSleep_check = new System.Windows.Forms.CheckBox();
            this.graceful_check = new System.Windows.Forms.CheckBox();
            this.background_check = new System.Windows.Forms.CheckBox();
            this.action_label = new System.Windows.Forms.Label();
            this.action_combo = new System.Windows.Forms.ComboBox();
            this.start_button = new System.Windows.Forms.Button();
            this.version_label = new System.Windows.Forms.Label();
            this.github_pb = new System.Windows.Forms.PictureBox();
            this.info_tooltip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.hours_updown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minutes_updown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seconds_updown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.titlebar_picture)).BeginInit();
            this.time_group.SuspendLayout();
            this.action_group.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.github_pb)).BeginInit();
            this.SuspendLayout();
            // 
            // hours_updown
            // 
            this.hours_updown.Location = new System.Drawing.Point(9, 37);
            this.hours_updown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.hours_updown.Name = "hours_updown";
            this.hours_updown.Size = new System.Drawing.Size(50, 20);
            this.hours_updown.TabIndex = 0;
            // 
            // minutes_updown
            // 
            this.minutes_updown.Location = new System.Drawing.Point(65, 37);
            this.minutes_updown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minutes_updown.Name = "minutes_updown";
            this.minutes_updown.Size = new System.Drawing.Size(50, 20);
            this.minutes_updown.TabIndex = 1;
            // 
            // seconds_updown
            // 
            this.seconds_updown.Location = new System.Drawing.Point(121, 37);
            this.seconds_updown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.seconds_updown.Name = "seconds_updown";
            this.seconds_updown.Size = new System.Drawing.Size(50, 20);
            this.seconds_updown.TabIndex = 2;
            // 
            // title_label
            // 
            this.title_label.AutoSize = true;
            this.title_label.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title_label.Location = new System.Drawing.Point(12, 9);
            this.title_label.Name = "title_label";
            this.title_label.Size = new System.Drawing.Size(215, 31);
            this.title_label.TabIndex = 3;
            this.title_label.Text = "Shutdown Timer";
            this.title_label.Click += new System.EventHandler(this.Title_label_Click);
            // 
            // titlebar_picture
            // 
            this.titlebar_picture.BackColor = System.Drawing.Color.Black;
            this.titlebar_picture.Location = new System.Drawing.Point(18, 43);
            this.titlebar_picture.Name = "titlebar_picture";
            this.titlebar_picture.Size = new System.Drawing.Size(252, 5);
            this.titlebar_picture.TabIndex = 4;
            this.titlebar_picture.TabStop = false;
            // 
            // hours_label
            // 
            this.hours_label.AutoSize = true;
            this.hours_label.Location = new System.Drawing.Point(6, 21);
            this.hours_label.Name = "hours_label";
            this.hours_label.Size = new System.Drawing.Size(35, 13);
            this.hours_label.TabIndex = 5;
            this.hours_label.Text = "Hours";
            // 
            // minutes_label
            // 
            this.minutes_label.AutoSize = true;
            this.minutes_label.Location = new System.Drawing.Point(62, 21);
            this.minutes_label.Name = "minutes_label";
            this.minutes_label.Size = new System.Drawing.Size(44, 13);
            this.minutes_label.TabIndex = 6;
            this.minutes_label.Text = "Minutes";
            // 
            // seconds_label
            // 
            this.seconds_label.AutoSize = true;
            this.seconds_label.Location = new System.Drawing.Point(118, 21);
            this.seconds_label.Name = "seconds_label";
            this.seconds_label.Size = new System.Drawing.Size(49, 13);
            this.seconds_label.TabIndex = 7;
            this.seconds_label.Text = "Seconds";
            // 
            // time_group
            // 
            this.time_group.Controls.Add(this.hours_updown);
            this.time_group.Controls.Add(this.seconds_label);
            this.time_group.Controls.Add(this.hours_label);
            this.time_group.Controls.Add(this.minutes_label);
            this.time_group.Controls.Add(this.minutes_updown);
            this.time_group.Controls.Add(this.seconds_updown);
            this.time_group.Location = new System.Drawing.Point(18, 189);
            this.time_group.Name = "time_group";
            this.time_group.Size = new System.Drawing.Size(252, 70);
            this.time_group.TabIndex = 8;
            this.time_group.TabStop = false;
            this.time_group.Text = "When to do it?";
            // 
            // action_group
            // 
            this.action_group.Controls.Add(this.preventSleep_check);
            this.action_group.Controls.Add(this.graceful_check);
            this.action_group.Controls.Add(this.background_check);
            this.action_group.Controls.Add(this.action_label);
            this.action_group.Controls.Add(this.action_combo);
            this.action_group.Location = new System.Drawing.Point(18, 64);
            this.action_group.Name = "action_group";
            this.action_group.Size = new System.Drawing.Size(252, 119);
            this.action_group.TabIndex = 9;
            this.action_group.TabStop = false;
            this.action_group.Text = "What to do?";
            // 
            // preventSleep_check
            // 
            this.preventSleep_check.AutoSize = true;
            this.preventSleep_check.Checked = true;
            this.preventSleep_check.CheckState = System.Windows.Forms.CheckState.Checked;
            this.preventSleep_check.Location = new System.Drawing.Point(9, 69);
            this.preventSleep_check.Name = "preventSleep_check";
            this.preventSleep_check.Size = new System.Drawing.Size(190, 17);
            this.preventSleep_check.TabIndex = 13;
            this.preventSleep_check.Text = "Prevent system from going to sleep";
            this.preventSleep_check.UseVisualStyleBackColor = true;
            // 
            // graceful_check
            // 
            this.graceful_check.AutoSize = true;
            this.graceful_check.Location = new System.Drawing.Point(9, 46);
            this.graceful_check.Name = "graceful_check";
            this.graceful_check.Size = new System.Drawing.Size(186, 17);
            this.graceful_check.TabIndex = 12;
            this.graceful_check.Text = "Graceful (do not force close apps)";
            this.graceful_check.UseVisualStyleBackColor = true;
            // 
            // background_check
            // 
            this.background_check.AutoSize = true;
            this.background_check.Location = new System.Drawing.Point(9, 92);
            this.background_check.Name = "background_check";
            this.background_check.Size = new System.Drawing.Size(117, 17);
            this.background_check.TabIndex = 11;
            this.background_check.Text = "Run in background";
            this.background_check.UseVisualStyleBackColor = true;
            // 
            // action_label
            // 
            this.action_label.AutoSize = true;
            this.action_label.Location = new System.Drawing.Point(6, 22);
            this.action_label.Name = "action_label";
            this.action_label.Size = new System.Drawing.Size(87, 13);
            this.action_label.TabIndex = 1;
            this.action_label.Text = "Select an action:";
            // 
            // action_combo
            // 
            this.action_combo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.action_combo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.action_combo.FormattingEnabled = true;
            this.action_combo.Items.AddRange(new object[] {
            "Shutdown",
            "Restart",
            "Hibernate",
            "Sleep",
            "Logout",
            "Lock"});
            this.action_combo.Location = new System.Drawing.Point(99, 19);
            this.action_combo.Name = "action_combo";
            this.action_combo.Size = new System.Drawing.Size(147, 21);
            this.action_combo.TabIndex = 0;
            this.action_combo.Text = "Shutdown";
            this.action_combo.TextChanged += new System.EventHandler(this.action_combo_TextChanged);
            // 
            // start_button
            // 
            this.start_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.start_button.Location = new System.Drawing.Point(18, 265);
            this.start_button.Name = "start_button";
            this.start_button.Size = new System.Drawing.Size(252, 30);
            this.start_button.TabIndex = 10;
            this.start_button.Text = "Start";
            this.start_button.UseVisualStyleBackColor = true;
            this.start_button.Click += new System.EventHandler(this.Start_button_Click);
            // 
            // version_label
            // 
            this.version_label.AutoSize = true;
            this.version_label.Location = new System.Drawing.Point(233, 51);
            this.version_label.Name = "version_label";
            this.version_label.Size = new System.Drawing.Size(37, 13);
            this.version_label.TabIndex = 11;
            this.version_label.Text = "v0.0.0";
            // 
            // github_pb
            // 
            this.github_pb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.github_pb.BackColor = System.Drawing.Color.Transparent;
            this.github_pb.Image = global::ShutdownTimerWin32.Properties.Resources.github;
            this.github_pb.Location = new System.Drawing.Point(246, 12);
            this.github_pb.Name = "github_pb";
            this.github_pb.Size = new System.Drawing.Size(24, 24);
            this.github_pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.github_pb.TabIndex = 12;
            this.github_pb.TabStop = false;
            this.github_pb.Click += new System.EventHandler(this.Github_pb_Click);
            // 
            // info_tooltip
            // 
            this.info_tooltip.AutoPopDelay = 30000;
            this.info_tooltip.InitialDelay = 500;
            this.info_tooltip.ReshowDelay = 100;
            this.info_tooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.info_tooltip.ToolTipTitle = "Help";
            // 
            // Menu
            // 
            this.AcceptButton = this.start_button;
            this.AccessibleDescription = "The main window where you can choose the time and power action.";
            this.AccessibleName = "Shutdown Timer";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 307);
            this.Controls.Add(this.github_pb);
            this.Controls.Add(this.version_label);
            this.Controls.Add(this.start_button);
            this.Controls.Add(this.action_group);
            this.Controls.Add(this.time_group);
            this.Controls.Add(this.titlebar_picture);
            this.Controls.Add(this.title_label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Menu";
            this.ShowIcon = false;
            this.Text = "Shutdown Timer";
            this.Load += new System.EventHandler(this.Menu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.hours_updown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minutes_updown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seconds_updown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.titlebar_picture)).EndInit();
            this.time_group.ResumeLayout(false);
            this.time_group.PerformLayout();
            this.action_group.ResumeLayout(false);
            this.action_group.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.github_pb)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown hours_updown;
        private System.Windows.Forms.NumericUpDown minutes_updown;
        private System.Windows.Forms.NumericUpDown seconds_updown;
        private System.Windows.Forms.Label title_label;
        private System.Windows.Forms.PictureBox titlebar_picture;
        private System.Windows.Forms.Label hours_label;
        private System.Windows.Forms.Label minutes_label;
        private System.Windows.Forms.Label seconds_label;
        private System.Windows.Forms.GroupBox time_group;
        private System.Windows.Forms.GroupBox action_group;
        private System.Windows.Forms.Button start_button;
        private System.Windows.Forms.Label action_label;
        private System.Windows.Forms.ComboBox action_combo;
        private System.Windows.Forms.CheckBox background_check;
        private System.Windows.Forms.Label version_label;
        private System.Windows.Forms.PictureBox github_pb;
        private System.Windows.Forms.CheckBox graceful_check;
        private System.Windows.Forms.CheckBox preventSleep_check;
        private System.Windows.Forms.ToolTip info_tooltip;
    }
}

