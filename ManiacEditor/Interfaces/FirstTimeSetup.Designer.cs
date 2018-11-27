namespace ManiacEditor
{
    partial class FirstTimeSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstTimeSetup));
            this.button3 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.minimalOption = new System.Windows.Forms.RadioButton();
            this.basicOption = new System.Windows.Forms.RadioButton();
            this.superOption = new System.Windows.Forms.RadioButton();
            this.hyperOption = new System.Windows.Forms.RadioButton();
            this.modeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.Location = new System.Drawing.Point(424, 188);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Lets Go!";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(487, 59);
            this.label1.TabIndex = 3;
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // minimalOption
            // 
            this.minimalOption.AutoSize = true;
            this.minimalOption.Location = new System.Drawing.Point(15, 71);
            this.minimalOption.Name = "minimalOption";
            this.minimalOption.Size = new System.Drawing.Size(60, 17);
            this.minimalOption.TabIndex = 4;
            this.minimalOption.TabStop = true;
            this.minimalOption.Text = "Minimal";
            this.minimalOption.UseVisualStyleBackColor = true;
            this.minimalOption.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            this.minimalOption.MouseHover += new System.EventHandler(this.radioButton1_MouseHover);
            // 
            // basicOption
            // 
            this.basicOption.AutoSize = true;
            this.basicOption.Location = new System.Drawing.Point(15, 94);
            this.basicOption.Name = "basicOption";
            this.basicOption.Size = new System.Drawing.Size(51, 17);
            this.basicOption.TabIndex = 5;
            this.basicOption.TabStop = true;
            this.basicOption.Text = "Basic";
            this.basicOption.UseVisualStyleBackColor = true;
            this.basicOption.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            this.basicOption.MouseHover += new System.EventHandler(this.radioButton2_MouseHover);
            // 
            // superOption
            // 
            this.superOption.AutoSize = true;
            this.superOption.Location = new System.Drawing.Point(15, 117);
            this.superOption.Name = "superOption";
            this.superOption.Size = new System.Drawing.Size(53, 17);
            this.superOption.TabIndex = 6;
            this.superOption.TabStop = true;
            this.superOption.Text = "Super";
            this.superOption.UseVisualStyleBackColor = true;
            this.superOption.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            this.superOption.MouseHover += new System.EventHandler(this.radioButton3_MouseHover);
            // 
            // hyperOption
            // 
            this.hyperOption.AutoSize = true;
            this.hyperOption.Location = new System.Drawing.Point(15, 140);
            this.hyperOption.Name = "hyperOption";
            this.hyperOption.Size = new System.Drawing.Size(53, 17);
            this.hyperOption.TabIndex = 7;
            this.hyperOption.TabStop = true;
            this.hyperOption.Text = "Hyper";
            this.hyperOption.UseVisualStyleBackColor = true;
            this.hyperOption.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            this.hyperOption.MouseHover += new System.EventHandler(this.radioButton4_MouseHover);
            // 
            // modeLabel
            // 
            this.modeLabel.Location = new System.Drawing.Point(12, 188);
            this.modeLabel.Name = "modeLabel";
            this.modeLabel.Size = new System.Drawing.Size(406, 26);
            this.modeLabel.TabIndex = 8;
            this.modeLabel.Text = "Hover over an option to see what it offers...";
            // 
            // FirstTimeSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 223);
            this.Controls.Add(this.modeLabel);
            this.Controls.Add(this.hyperOption);
            this.Controls.Add(this.superOption);
            this.Controls.Add(this.basicOption);
            this.Controls.Add(this.minimalOption);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirstTimeSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome to Maniac Editor!";
            this.Load += new System.EventHandler(this.DevWarningBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton minimalOption;
        private System.Windows.Forms.RadioButton basicOption;
        private System.Windows.Forms.RadioButton superOption;
        private System.Windows.Forms.RadioButton hyperOption;
        private System.Windows.Forms.Label modeLabel;
    }
}