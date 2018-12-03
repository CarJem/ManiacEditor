namespace ManiacEditor.Interfaces
{
    partial class KeybindTool
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.resultLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.shiftBox = new System.Windows.Forms.CheckBox();
            this.tabBox = new System.Windows.Forms.CheckBox();
            this.altBox = new System.Windows.Forms.CheckBox();
            this.ctrlBox = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(0, 0);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(257, 157);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Window;
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.resultLabel);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.shiftBox);
            this.tabPage1.Controls.Add(this.tabBox);
            this.tabPage1.Controls.Add(this.altBox);
            this.tabPage1.Controls.Add(this.ctrlBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(249, 131);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Keyboard";
            // 
            // resultLabel
            // 
            this.resultLabel.AutoSize = true;
            this.resultLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.resultLabel.Location = new System.Drawing.Point(5, 113);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(57, 13);
            this.resultLabel.TabIndex = 7;
            this.resultLabel.Text = "Result: {0}";
            this.resultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Enabled = false;
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(166, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Set Keybind";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(123, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Input Key Here:";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.ForeColor = System.Drawing.Color.Black;
            this.textBox1.Location = new System.Drawing.Point(126, 28);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(115, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // shiftBox
            // 
            this.shiftBox.AutoSize = true;
            this.shiftBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.shiftBox.Location = new System.Drawing.Point(8, 77);
            this.shiftBox.Name = "shiftBox";
            this.shiftBox.Size = new System.Drawing.Size(57, 17);
            this.shiftBox.TabIndex = 3;
            this.shiftBox.Text = "SHIFT";
            this.shiftBox.UseVisualStyleBackColor = true;
            this.shiftBox.CheckedChanged += new System.EventHandler(this.shiftBox_CheckedChanged);
            // 
            // tabBox
            // 
            this.tabBox.AutoSize = true;
            this.tabBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabBox.Location = new System.Drawing.Point(8, 54);
            this.tabBox.Name = "tabBox";
            this.tabBox.Size = new System.Drawing.Size(47, 17);
            this.tabBox.TabIndex = 2;
            this.tabBox.Text = "TAB";
            this.tabBox.UseVisualStyleBackColor = true;
            this.tabBox.CheckedChanged += new System.EventHandler(this.tabBOX_CheckedChanged);
            // 
            // altBox
            // 
            this.altBox.AutoSize = true;
            this.altBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.altBox.Location = new System.Drawing.Point(8, 31);
            this.altBox.Name = "altBox";
            this.altBox.Size = new System.Drawing.Size(46, 17);
            this.altBox.TabIndex = 1;
            this.altBox.Text = "ALT";
            this.altBox.UseVisualStyleBackColor = true;
            this.altBox.CheckedChanged += new System.EventHandler(this.altBox_CheckedChanged);
            // 
            // ctrlBox
            // 
            this.ctrlBox.AutoSize = true;
            this.ctrlBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ctrlBox.Location = new System.Drawing.Point(8, 8);
            this.ctrlBox.Name = "ctrlBox";
            this.ctrlBox.Size = new System.Drawing.Size(54, 17);
            this.ctrlBox.TabIndex = 0;
            this.ctrlBox.Text = "CTRL";
            this.ctrlBox.UseVisualStyleBackColor = true;
            this.ctrlBox.CheckedChanged += new System.EventHandler(this.ctrlBox_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(249, 131);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Mouse";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.Window;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(243, 125);
            this.label3.TabIndex = 0;
            this.label3.Text = "Not Implemented Yet";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(132, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Not Implemented Yet!";
            // 
            // KeybindTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 157);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "KeybindTool";
            this.Text = "Set Key Binding...";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label resultLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox shiftBox;
        private System.Windows.Forms.CheckBox tabBox;
        private System.Windows.Forms.CheckBox altBox;
        private System.Windows.Forms.CheckBox ctrlBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}