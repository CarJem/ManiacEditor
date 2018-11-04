namespace ManiacEditor
{
    partial class DeviceLostBox
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
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.showDetailsCheckbox = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.errorLabel = new System.Windows.Forms.Label();
            this.imageViewer1 = new ManiacEditor.ImageViewer();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(126, 12);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(447, 48);
            this.label1.TabIndex = 0;
            this.label1.Text = "The DirectX Device was just lost! This may or will lead to a crash! To prevent lo" +
    "ss of data, would you like to make an emergency backup?\r\n";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.button1.Location = new System.Drawing.Point(129, 96);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "&Yes and Exit\r\n";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.No;
            this.button2.Location = new System.Drawing.Point(220, 96);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(128, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "&No and Try to Resume";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.button3.Location = new System.Drawing.Point(354, 96);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(128, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "&Yes and Try to Resume";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.button4.Location = new System.Drawing.Point(488, 96);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(85, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "&No and Exit\r\n";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // showDetailsCheckbox
            // 
            this.showDetailsCheckbox.AutoSize = true;
            this.showDetailsCheckbox.Location = new System.Drawing.Point(485, 142);
            this.showDetailsCheckbox.Name = "showDetailsCheckbox";
            this.showDetailsCheckbox.Size = new System.Drawing.Size(88, 17);
            this.showDetailsCheckbox.TabIndex = 9;
            this.showDetailsCheckbox.Text = "Show Details";
            this.showDetailsCheckbox.UseVisualStyleBackColor = true;
            this.showDetailsCheckbox.CheckedChanged += new System.EventHandler(this.showDetailsCheckbox_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Location = new System.Drawing.Point(0, 165);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(592, 17);
            this.panel1.TabIndex = 10;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(586, 11);
            this.textBox1.TabIndex = 11;
            this.textBox1.Visible = false;
            // 
            // errorLabel
            // 
            this.errorLabel.Location = new System.Drawing.Point(126, 60);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.errorLabel.Size = new System.Drawing.Size(447, 33);
            this.errorLabel.TabIndex = 11;
            this.errorLabel.Text = "Error Code: \r\nSecondary Error Code (Mostly Unimportant): \r\n";
            // 
            // imageViewer1
            // 
            this.imageViewer1.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.imageViewer1.BackColor = System.Drawing.Color.Transparent;
            this.imageViewer1.BackgroundImage = global::ManiacEditor.Properties.Resources.ErrorMonitor;
            this.imageViewer1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.imageViewer1.Image = null;
            this.imageViewer1.ImageLocation = null;
            this.imageViewer1.IsActive = false;
            this.imageViewer1.IsThumbnail = false;
            this.imageViewer1.Location = new System.Drawing.Point(12, 12);
            this.imageViewer1.Name = "imageViewer1";
            this.imageViewer1.Size = new System.Drawing.Size(108, 107);
            this.imageViewer1.TabIndex = 4;
            // 
            // DeviceLostBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 182);
            this.ControlBox = false;
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.showDetailsCheckbox);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.imageViewer1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeviceLostBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Maniac Editor (Generations Edition) - Error";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private ImageViewer imageViewer1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox showDetailsCheckbox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label errorLabel;
    }
}