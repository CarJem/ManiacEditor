namespace ManiacEditor
{
    partial class TilesToolbarOld
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.option1CheckBox = new System.Windows.Forms.CheckBox();
			this.option2CheckBox = new System.Windows.Forms.CheckBox();
			this.tileOption4 = new System.Windows.Forms.CheckBox();
			this.tileOption3 = new System.Windows.Forms.CheckBox();
			this.tileOption2 = new System.Windows.Forms.CheckBox();
			this.tileOption1 = new System.Windows.Forms.CheckBox();
			this.option6CheckBox = new System.Windows.Forms.CheckBox();
			this.option5CheckBox = new System.Windows.Forms.CheckBox();
			this.option4CheckBox = new System.Windows.Forms.CheckBox();
			this.option3CheckBox = new System.Windows.Forms.CheckBox();
			this.selectedTileLabel = new System.Windows.Forms.Label();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.editTileInTileManiacToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
			this.contextMenuStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// option1CheckBox
			// 
			this.option1CheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.option1CheckBox.AutoSize = true;
			this.option1CheckBox.Location = new System.Drawing.Point(11, 335);
			this.option1CheckBox.Name = "option1CheckBox";
			this.option1CheckBox.Size = new System.Drawing.Size(92, 30);
			this.option1CheckBox.TabIndex = 2;
			this.option1CheckBox.Text = "Flip Horizontal\r\n (Ctrl)";
			this.option1CheckBox.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.option1CheckBox.UseVisualStyleBackColor = true;
			this.option1CheckBox.CheckedChanged += new System.EventHandler(this.option1CheckBox_CheckedChanged);
			// 
			// option2CheckBox
			// 
			this.option2CheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.option2CheckBox.AutoSize = true;
			this.option2CheckBox.Location = new System.Drawing.Point(133, 335);
			this.option2CheckBox.Name = "option2CheckBox";
			this.option2CheckBox.Size = new System.Drawing.Size(80, 30);
			this.option2CheckBox.TabIndex = 3;
			this.option2CheckBox.Text = "Flip Vertical\r\n (Shift)";
			this.option2CheckBox.UseVisualStyleBackColor = true;
			this.option2CheckBox.CheckedChanged += new System.EventHandler(this.option2CheckBox_CheckedChanged);
			// 
			// tileOption4
			// 
			this.tileOption4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.tileOption4.AutoSize = true;
			this.tileOption4.Location = new System.Drawing.Point(129, 45);
			this.tileOption4.Name = "tileOption4";
			this.tileOption4.Size = new System.Drawing.Size(115, 30);
			this.tileOption4.TabIndex = 13;
			this.tileOption4.Text = "Solid (All excl. top) \r\n(Plane 2)";
			this.tileOption4.UseVisualStyleBackColor = false;
			this.tileOption4.CheckedChanged += new System.EventHandler(this.tileOption4_CheckedChanged);
			// 
			// tileOption3
			// 
			this.tileOption3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.tileOption3.AutoSize = true;
			this.tileOption3.Location = new System.Drawing.Point(7, 45);
			this.tileOption3.Name = "tileOption3";
			this.tileOption3.Size = new System.Drawing.Size(80, 30);
			this.tileOption3.TabIndex = 12;
			this.tileOption3.Text = "Solid (Top) \r\n(Plane 2)";
			this.tileOption3.UseVisualStyleBackColor = false;
			this.tileOption3.CheckedChanged += new System.EventHandler(this.tileOption3_CheckedChanged);
			// 
			// tileOption2
			// 
			this.tileOption2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.tileOption2.AutoSize = true;
			this.tileOption2.Location = new System.Drawing.Point(129, 23);
			this.tileOption2.Name = "tileOption2";
			this.tileOption2.Size = new System.Drawing.Size(112, 17);
			this.tileOption2.TabIndex = 11;
			this.tileOption2.Text = "Solid (All excl. top)";
			this.tileOption2.UseVisualStyleBackColor = false;
			this.tileOption2.CheckedChanged += new System.EventHandler(this.tileOption2_CheckedChanged);
			// 
			// tileOption1
			// 
			this.tileOption1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.tileOption1.AutoSize = true;
			this.tileOption1.Location = new System.Drawing.Point(7, 21);
			this.tileOption1.Name = "tileOption1";
			this.tileOption1.Size = new System.Drawing.Size(77, 17);
			this.tileOption1.TabIndex = 10;
			this.tileOption1.Text = "Solid (Top)";
			this.tileOption1.UseVisualStyleBackColor = false;
			this.tileOption1.CheckedChanged += new System.EventHandler(this.tileOption1_CheckedChanged);
			// 
			// option6CheckBox
			// 
			this.option6CheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.option6CheckBox.Checked = global::ManiacEditor.Properties.Settings.Default.Unkown2Default;
			this.option6CheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ManiacEditor.Properties.Settings.Default, "Unkown2Default", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.option6CheckBox.Location = new System.Drawing.Point(133, 392);
			this.option6CheckBox.Name = "option6CheckBox";
			this.option6CheckBox.Size = new System.Drawing.Size(112, 31);
			this.option6CheckBox.TabIndex = 7;
			this.option6CheckBox.Text = "Solid (All excl. top) (Plane 2)";
			this.option6CheckBox.UseVisualStyleBackColor = true;
			// 
			// option5CheckBox
			// 
			this.option5CheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.option5CheckBox.Checked = global::ManiacEditor.Properties.Settings.Default.Unkown1Default;
			this.option5CheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ManiacEditor.Properties.Settings.Default, "Unkown1Default", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.option5CheckBox.Location = new System.Drawing.Point(11, 394);
			this.option5CheckBox.Name = "option5CheckBox";
			this.option5CheckBox.Size = new System.Drawing.Size(83, 31);
			this.option5CheckBox.TabIndex = 6;
			this.option5CheckBox.Text = "Solid (Top) (Plane 2)";
			this.option5CheckBox.UseVisualStyleBackColor = true;
			// 
			// option4CheckBox
			// 
			this.option4CheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.option4CheckBox.AutoSize = true;
			this.option4CheckBox.Checked = global::ManiacEditor.Properties.Settings.Default.SolidAllButTopDefault;
			this.option4CheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ManiacEditor.Properties.Settings.Default, "SolidAllButTopDefault", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.option4CheckBox.Location = new System.Drawing.Point(133, 369);
			this.option4CheckBox.Name = "option4CheckBox";
			this.option4CheckBox.Size = new System.Drawing.Size(112, 17);
			this.option4CheckBox.TabIndex = 5;
			this.option4CheckBox.Text = "Solid (All excl. top)";
			this.option4CheckBox.UseVisualStyleBackColor = true;
			// 
			// option3CheckBox
			// 
			this.option3CheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.option3CheckBox.AutoSize = true;
			this.option3CheckBox.Checked = global::ManiacEditor.Properties.Settings.Default.SolidTopDefault;
			this.option3CheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ManiacEditor.Properties.Settings.Default, "SolidTopDefault", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.option3CheckBox.Location = new System.Drawing.Point(11, 371);
			this.option3CheckBox.Name = "option3CheckBox";
			this.option3CheckBox.Size = new System.Drawing.Size(77, 17);
			this.option3CheckBox.TabIndex = 4;
			this.option3CheckBox.Text = "Solid (Top)";
			this.option3CheckBox.UseVisualStyleBackColor = true;
			// 
			// selectedTileLabel
			// 
			this.selectedTileLabel.AutoSize = true;
			this.selectedTileLabel.BackColor = System.Drawing.Color.Transparent;
			this.selectedTileLabel.Location = new System.Drawing.Point(-1, 0);
			this.selectedTileLabel.Name = "selectedTileLabel";
			this.selectedTileLabel.Padding = new System.Windows.Forms.Padding(3);
			this.selectedTileLabel.Size = new System.Drawing.Size(101, 19);
			this.selectedTileLabel.TabIndex = 14;
			this.selectedTileLabel.Text = "Selected Tile: N/A";
			this.selectedTileLabel.Click += new System.EventHandler(this.selectedTileLabel_Click);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editTileInTileManiacToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(218, 26);
			// 
			// editTileInTileManiacToolStripMenuItem
			// 
			this.editTileInTileManiacToolStripMenuItem.Name = "editTileInTileManiacToolStripMenuItem";
			this.editTileInTileManiacToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.editTileInTileManiacToolStripMenuItem.Text = "Edit Tile #{0} in Tile Maniac";
			this.editTileInTileManiacToolStripMenuItem.Click += new System.EventHandler(this.editTileInTileManiacToolStripMenuItem_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.tileOption1);
			this.groupBox1.Controls.Add(this.tileOption4);
			this.groupBox1.Controls.Add(this.tileOption2);
			this.groupBox1.Controls.Add(this.tileOption3);
			this.groupBox1.Location = new System.Drawing.Point(4, 429);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(242, 93);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Selected Tiles";
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.Transparent;
			this.panel1.Controls.Add(this.selectedTileLabel);
			this.panel1.Location = new System.Drawing.Point(146, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(96, 19);
			this.panel1.TabIndex = 16;
			// 
			// elementHost1
			// 
			this.elementHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.elementHost1.Location = new System.Drawing.Point(0, 0);
			this.elementHost1.Name = "elementHost1";
			this.elementHost1.Size = new System.Drawing.Size(250, 329);
			this.elementHost1.TabIndex = 17;
			this.elementHost1.Text = "elementHost1";
			this.elementHost1.Child = null;
			// 
			// TilesToolbarOld
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.option6CheckBox);
			this.Controls.Add(this.option5CheckBox);
			this.Controls.Add(this.option4CheckBox);
			this.Controls.Add(this.option3CheckBox);
			this.Controls.Add(this.option2CheckBox);
			this.Controls.Add(this.option1CheckBox);
			this.Controls.Add(this.elementHost1);
			this.MinimumSize = new System.Drawing.Size(250, 500);
			this.Name = "TilesToolbarOld";
			this.Size = new System.Drawing.Size(250, 525);
			this.Load += new System.EventHandler(this.TilesToolbar_Load);
			this.Resize += new System.EventHandler(this.TilesToolbar_Resize);
			this.contextMenuStrip1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
		private System.Windows.Forms.CheckBox option1CheckBox;
        private System.Windows.Forms.CheckBox option2CheckBox;
        private System.Windows.Forms.CheckBox option3CheckBox;
        private System.Windows.Forms.CheckBox option4CheckBox;
        private System.Windows.Forms.CheckBox option5CheckBox;
        private System.Windows.Forms.CheckBox option6CheckBox;
        private System.Windows.Forms.CheckBox tileOption4;
        private System.Windows.Forms.CheckBox tileOption3;
        private System.Windows.Forms.CheckBox tileOption2;
        private System.Windows.Forms.CheckBox tileOption1;
        public System.Windows.Forms.Label selectedTileLabel;
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        public System.Windows.Forms.ToolStripMenuItem editTileInTileManiacToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Integration.ElementHost elementHost1;
	}
}
