namespace ShutdownTimer
{
    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.titlebarPictureBox = new System.Windows.Forms.PictureBox();
            this.titleLabel = new System.Windows.Forms.Label();
            this.appLabel = new System.Windows.Forms.Label();
            this.footerLabel = new System.Windows.Forms.Label();
            this.githubLinkLabel = new System.Windows.Forms.LinkLabel();
            this.licenseLinkLabel = new System.Windows.Forms.LinkLabel();
            this.falicenseLinkLabel = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.titlebarPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // titlebarPictureBox
            // 
            this.titlebarPictureBox.BackColor = System.Drawing.Color.Black;
            this.titlebarPictureBox.Location = new System.Drawing.Point(18, 43);
            this.titlebarPictureBox.Name = "titlebarPictureBox";
            this.titlebarPictureBox.Size = new System.Drawing.Size(113, 5);
            this.titlebarPictureBox.TabIndex = 6;
            this.titlebarPictureBox.TabStop = false;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(12, 9);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(113, 31);
            this.titleLabel.TabIndex = 5;
            this.titleLabel.Text = "Settings";
            // 
            // appLabel
            // 
            this.appLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appLabel.Location = new System.Drawing.Point(372, 9);
            this.appLabel.Name = "appLabel";
            this.appLabel.Size = new System.Drawing.Size(200, 13);
            this.appLabel.TabIndex = 7;
            this.appLabel.Text = "ShutdownTimerClassic@v0.0.0";
            this.appLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // footerLabel
            // 
            this.footerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.footerLabel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.footerLabel.Location = new System.Drawing.Point(12, 332);
            this.footerLabel.Name = "footerLabel";
            this.footerLabel.Size = new System.Drawing.Size(560, 20);
            this.footerLabel.TabIndex = 9;
            this.footerLabel.Text = "Made with love in Germany";
            this.footerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // githubLinkLabel
            // 
            this.githubLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.githubLinkLabel.Location = new System.Drawing.Point(372, 22);
            this.githubLinkLabel.Name = "githubLinkLabel";
            this.githubLinkLabel.Size = new System.Drawing.Size(200, 13);
            this.githubLinkLabel.TabIndex = 10;
            this.githubLinkLabel.TabStop = true;
            this.githubLinkLabel.Text = "View sourcecode on GitHub";
            this.githubLinkLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.githubLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GithubLinkLabel_LinkClicked);
            // 
            // licenseLinkLabel
            // 
            this.licenseLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.licenseLinkLabel.Location = new System.Drawing.Point(372, 35);
            this.licenseLinkLabel.Name = "licenseLinkLabel";
            this.licenseLinkLabel.Size = new System.Drawing.Size(200, 13);
            this.licenseLinkLabel.TabIndex = 11;
            this.licenseLinkLabel.TabStop = true;
            this.licenseLinkLabel.Text = "View license for this app";
            this.licenseLinkLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.licenseLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LicenseLinkLabel_LinkClicked);
            // 
            // falicenseLinkLabel
            // 
            this.falicenseLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.falicenseLinkLabel.Location = new System.Drawing.Point(372, 48);
            this.falicenseLinkLabel.Name = "falicenseLinkLabel";
            this.falicenseLinkLabel.Size = new System.Drawing.Size(200, 13);
            this.falicenseLinkLabel.TabIndex = 12;
            this.falicenseLinkLabel.TabStop = true;
            this.falicenseLinkLabel.Text = "View license for Font Awesome";
            this.falicenseLinkLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.falicenseLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.FALicenseLinkLabel_LinkClicked);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.falicenseLinkLabel);
            this.Controls.Add(this.licenseLinkLabel);
            this.Controls.Add(this.githubLinkLabel);
            this.Controls.Add(this.footerLabel);
            this.Controls.Add(this.appLabel);
            this.Controls.Add(this.titlebarPictureBox);
            this.Controls.Add(this.titleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.titlebarPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox titlebarPictureBox;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label appLabel;
        private System.Windows.Forms.Label footerLabel;
        private System.Windows.Forms.LinkLabel githubLinkLabel;
        private System.Windows.Forms.LinkLabel licenseLinkLabel;
        private System.Windows.Forms.LinkLabel falicenseLinkLabel;
    }
}