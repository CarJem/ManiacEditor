namespace ManiacEditor.Interfaces
{
    partial class UpdateStatusBox
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateStatusBox));
			this.updateInfoLabel = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.linkLabel2 = new System.Windows.Forms.LinkLabel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// updateInfoLabel
			// 
			this.updateInfoLabel.AutoSize = true;
			this.updateInfoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.updateInfoLabel.Location = new System.Drawing.Point(0, 0);
			this.updateInfoLabel.Name = "updateInfoLabel";
			this.updateInfoLabel.Size = new System.Drawing.Size(102, 78);
			this.updateInfoLabel.TabIndex = 0;
			this.updateInfoLabel.Text = "No Updates Found!\r\n\r\nLocal Version: {0}\r\nCurrent Version: {1} \r\n\r\nDetails: {2}\r\n";
			// 
			// linkLabel1
			// 
			this.linkLabel1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(12, 131);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(83, 13);
			this.linkLabel1.TabIndex = 32;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Get Releases....";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// linkLabel2
			// 
			this.linkLabel2.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.linkLabel2.AutoSize = true;
			this.linkLabel2.Location = new System.Drawing.Point(400, 131);
			this.linkLabel2.Name = "linkLabel2";
			this.linkLabel2.Size = new System.Drawing.Size(87, 13);
			this.linkLabel2.TabIndex = 31;
			this.linkLabel2.TabStop = true;
			this.linkLabel2.Text = "Get Dev Builds...";
			this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.updateInfoLabel);
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(475, 116);
			this.panel1.TabIndex = 33;
			// 
			// UpdateStatusBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(499, 153);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.linkLabel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(307, 170);
			this.Name = "UpdateStatusBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Updates";
			this.Load += new System.EventHandler(this.UpdateStatusBox_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label updateInfoLabel;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Panel panel1;
    }
}