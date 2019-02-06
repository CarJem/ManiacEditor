namespace TileManiac
{
    partial class OptionsBox
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsBox));
			this.button8 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.panel15 = new System.Windows.Forms.Panel();
			this.checkBox5 = new System.Windows.Forms.CheckBox();
			this.label85 = new System.Windows.Forms.Label();
			this.checkBox37 = new System.Windows.Forms.CheckBox();
			this.panel9 = new System.Windows.Forms.Panel();
			this.label11 = new System.Windows.Forms.Label();
			this.label27 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.panel10 = new System.Windows.Forms.Panel();
			this.label12 = new System.Windows.Forms.Label();
			this.enableWindowsClipboard = new System.Windows.Forms.CheckBox();
			this.importOptionsButton = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.panel11 = new System.Windows.Forms.Panel();
			this.checkBox33 = new System.Windows.Forms.CheckBox();
			this.label22 = new System.Windows.Forms.Label();
			this.RPCCheckBox = new System.Windows.Forms.CheckBox();
			this.button4 = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.overlayEditorViewRadioButton = new System.Windows.Forms.RadioButton();
			this.collisionEditorViewRadioButton = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.overlayRenderViewRadioButton = new System.Windows.Forms.RadioButton();
			this.tileRenderViewRadioButton = new System.Windows.Forms.RadioButton();
			this.collisionRenderViewRadioButton = new System.Windows.Forms.RadioButton();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.collisionListRadioButton = new System.Windows.Forms.RadioButton();
			this.tileListRadioButton = new System.Windows.Forms.RadioButton();
			this.checkBox4 = new System.Windows.Forms.CheckBox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.tabPage2.SuspendLayout();
			this.panel15.SuspendLayout();
			this.panel9.SuspendLayout();
			this.panel10.SuspendLayout();
			this.panel11.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// button8
			// 
			this.button8.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button8.ForeColor = System.Drawing.Color.Black;
			this.button8.Location = new System.Drawing.Point(219, 591);
			this.button8.Name = "button8";
			this.button8.Size = new System.Drawing.Size(75, 22);
			this.button8.TabIndex = 111;
			this.button8.Text = "Save";
			this.button8.UseVisualStyleBackColor = true;
			// 
			// button6
			// 
			this.button6.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button6.ForeColor = System.Drawing.Color.Black;
			this.button6.Location = new System.Drawing.Point(300, 591);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(75, 22);
			this.button6.TabIndex = 110;
			this.button6.Text = "&OK";
			this.button6.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.SystemColors.Window;
			this.tabPage2.Controls.Add(this.panel15);
			this.tabPage2.Controls.Add(this.panel9);
			this.tabPage2.Controls.Add(this.panel10);
			this.tabPage2.Controls.Add(this.importOptionsButton);
			this.tabPage2.Controls.Add(this.button5);
			this.tabPage2.Controls.Add(this.panel11);
			this.tabPage2.Controls.Add(this.button4);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(359, 547);
			this.tabPage2.TabIndex = 3;
			this.tabPage2.Text = "Other";
			// 
			// panel15
			// 
			this.panel15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel15.Controls.Add(this.checkBox5);
			this.panel15.Controls.Add(this.label85);
			this.panel15.Controls.Add(this.checkBox37);
			this.panel15.Location = new System.Drawing.Point(8, 6);
			this.panel15.Name = "panel15";
			this.panel15.Size = new System.Drawing.Size(183, 142);
			this.panel15.TabIndex = 109;
			// 
			// checkBox5
			// 
			this.checkBox5.BackColor = System.Drawing.Color.Transparent;
			this.checkBox5.Checked = global::TileManiac.Properties.Settings.Default.PromptForChoiceOnImageWrite;
			this.checkBox5.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox5.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TileManiac.Properties.Settings.Default, "PromptForChoiceOnImageWrite", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBox5.Location = new System.Drawing.Point(11, 82);
			this.checkBox5.Margin = new System.Windows.Forms.Padding(5);
			this.checkBox5.Name = "checkBox5";
			this.checkBox5.Size = new System.Drawing.Size(157, 31);
			this.checkBox5.TabIndex = 58;
			this.checkBox5.Text = "Prompt for Choice (if not checked above)";
			this.checkBox5.UseVisualStyleBackColor = false;
			// 
			// label85
			// 
			this.label85.BackColor = System.Drawing.Color.Transparent;
			this.label85.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label85.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label85.Location = new System.Drawing.Point(13, 8);
			this.label85.Name = "label85";
			this.label85.Size = new System.Drawing.Size(155, 13);
			this.label85.TabIndex = 56;
			this.label85.Text = "General Options:";
			this.label85.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// checkBox37
			// 
			this.checkBox37.BackColor = System.Drawing.Color.Transparent;
			this.checkBox37.Checked = global::TileManiac.Properties.Settings.Default.AllowDirect16x16TilesGIFEditing;
			this.checkBox37.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TileManiac.Properties.Settings.Default, "AllowDirect16x16TilesGIFEditing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBox37.Location = new System.Drawing.Point(11, 26);
			this.checkBox37.Margin = new System.Windows.Forms.Padding(5);
			this.checkBox37.Name = "checkBox37";
			this.checkBox37.Size = new System.Drawing.Size(157, 46);
			this.checkBox37.TabIndex = 57;
			this.checkBox37.Text = "Allow Tile Image Editing to Directly Edit the Original File*";
			this.checkBox37.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox37.UseVisualStyleBackColor = false;
			// 
			// panel9
			// 
			this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel9.Controls.Add(this.label11);
			this.panel9.Controls.Add(this.label27);
			this.panel9.Controls.Add(this.label25);
			this.panel9.Location = new System.Drawing.Point(211, 274);
			this.panel9.Name = "panel9";
			this.panel9.Size = new System.Drawing.Size(136, 184);
			this.panel9.TabIndex = 109;
			// 
			// label11
			// 
			this.label11.BackColor = System.Drawing.Color.Transparent;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label11.Location = new System.Drawing.Point(6, 8);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(125, 29);
			this.label11.TabIndex = 74;
			this.label11.Text = "Other Notes for Certain Settings:";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label27
			// 
			this.label27.BackColor = System.Drawing.Color.Transparent;
			this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label27.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label27.Location = new System.Drawing.Point(3, 52);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(128, 36);
			this.label27.TabIndex = 77;
			this.label27.Text = "* Use at your own risk";
			this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label25
			// 
			this.label25.BackColor = System.Drawing.Color.Transparent;
			this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label25.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label25.Location = new System.Drawing.Point(6, 96);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(125, 80);
			this.label25.TabIndex = 80;
			this.label25.Text = "** Extremely Dangerous to touch if you have no Idea what you \r\nare doing, please " +
    "make a backup";
			this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel10
			// 
			this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel10.Controls.Add(this.label12);
			this.panel10.Controls.Add(this.enableWindowsClipboard);
			this.panel10.Location = new System.Drawing.Point(8, 154);
			this.panel10.Name = "panel10";
			this.panel10.Size = new System.Drawing.Size(183, 179);
			this.panel10.TabIndex = 108;
			// 
			// label12
			// 
			this.label12.BackColor = System.Drawing.Color.Transparent;
			this.label12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(16, 8);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(160, 13);
			this.label12.TabIndex = 56;
			this.label12.Text = "Experimental Options:";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// enableWindowsClipboard
			// 
			this.enableWindowsClipboard.BackColor = System.Drawing.Color.Transparent;
			this.enableWindowsClipboard.Enabled = false;
			this.enableWindowsClipboard.Location = new System.Drawing.Point(16, 30);
			this.enableWindowsClipboard.Margin = new System.Windows.Forms.Padding(5);
			this.enableWindowsClipboard.Name = "enableWindowsClipboard";
			this.enableWindowsClipboard.Size = new System.Drawing.Size(152, 32);
			this.enableWindowsClipboard.TabIndex = 57;
			this.enableWindowsClipboard.Text = "Enable Windows Clipboard*";
			this.enableWindowsClipboard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.enableWindowsClipboard.UseVisualStyleBackColor = false;
			// 
			// importOptionsButton
			// 
			this.importOptionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.importOptionsButton.BackColor = System.Drawing.Color.Gainsboro;
			this.importOptionsButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.importOptionsButton.ForeColor = System.Drawing.Color.Black;
			this.importOptionsButton.Location = new System.Drawing.Point(211, 460);
			this.importOptionsButton.Name = "importOptionsButton";
			this.importOptionsButton.Size = new System.Drawing.Size(136, 22);
			this.importOptionsButton.TabIndex = 106;
			this.importOptionsButton.Text = "Import Options";
			this.importOptionsButton.UseVisualStyleBackColor = false;
			// 
			// button5
			// 
			this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button5.BackColor = System.Drawing.Color.Gainsboro;
			this.button5.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button5.ForeColor = System.Drawing.Color.Black;
			this.button5.Location = new System.Drawing.Point(211, 488);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(136, 22);
			this.button5.TabIndex = 105;
			this.button5.Text = "Export Options";
			this.button5.UseVisualStyleBackColor = false;
			// 
			// panel11
			// 
			this.panel11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel11.Controls.Add(this.checkBox33);
			this.panel11.Controls.Add(this.label22);
			this.panel11.Controls.Add(this.RPCCheckBox);
			this.panel11.Location = new System.Drawing.Point(7, 339);
			this.panel11.Name = "panel11";
			this.panel11.Size = new System.Drawing.Size(184, 199);
			this.panel11.TabIndex = 104;
			// 
			// checkBox33
			// 
			this.checkBox33.BackColor = System.Drawing.Color.Transparent;
			this.checkBox33.Enabled = false;
			this.checkBox33.Location = new System.Drawing.Point(16, 78);
			this.checkBox33.Name = "checkBox33";
			this.checkBox33.Size = new System.Drawing.Size(160, 40);
			this.checkBox33.TabIndex = 56;
			this.checkBox33.Text = "Check for Updates on Startup";
			this.checkBox33.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox33.UseVisualStyleBackColor = false;
			// 
			// label22
			// 
			this.label22.BackColor = System.Drawing.Color.Transparent;
			this.label22.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label22.Location = new System.Drawing.Point(16, 8);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(160, 13);
			this.label22.TabIndex = 55;
			this.label22.Text = "Misc Settings:";
			this.label22.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// RPCCheckBox
			// 
			this.RPCCheckBox.BackColor = System.Drawing.Color.Transparent;
			this.RPCCheckBox.Enabled = false;
			this.RPCCheckBox.Location = new System.Drawing.Point(16, 32);
			this.RPCCheckBox.Name = "RPCCheckBox";
			this.RPCCheckBox.Size = new System.Drawing.Size(160, 40);
			this.RPCCheckBox.TabIndex = 29;
			this.RPCCheckBox.Text = "Enable Discord Rich Presence";
			this.RPCCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RPCCheckBox.UseVisualStyleBackColor = false;
			// 
			// button4
			// 
			this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button4.BackColor = System.Drawing.Color.Gainsboro;
			this.button4.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button4.ForeColor = System.Drawing.Color.Black;
			this.button4.Location = new System.Drawing.Point(211, 516);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(136, 22);
			this.button4.TabIndex = 103;
			this.button4.Text = "Reset Options to Default";
			this.button4.UseVisualStyleBackColor = false;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(367, 573);
			this.tabControl1.TabIndex = 109;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.panel1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(359, 547);
			this.tabPage1.TabIndex = 4;
			this.tabPage1.Text = "Defaults";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.groupBox3);
			this.panel1.Controls.Add(this.groupBox2);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Controls.Add(this.checkBox4);
			this.panel1.Controls.Add(this.checkBox3);
			this.panel1.Controls.Add(this.checkBox2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.checkBox1);
			this.panel1.Location = new System.Drawing.Point(6, 6);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(347, 535);
			this.panel1.TabIndex = 110;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.overlayEditorViewRadioButton);
			this.groupBox2.Controls.Add(this.collisionEditorViewRadioButton);
			this.groupBox2.ForeColor = System.Drawing.SystemColors.MenuText;
			this.groupBox2.Location = new System.Drawing.Point(11, 346);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(176, 67);
			this.groupBox2.TabIndex = 123;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Editor View Setting (Non Classic)";
			// 
			// overlayEditorViewRadioButton
			// 
			this.overlayEditorViewRadioButton.AutoSize = true;
			this.overlayEditorViewRadioButton.Location = new System.Drawing.Point(6, 42);
			this.overlayEditorViewRadioButton.Name = "overlayEditorViewRadioButton";
			this.overlayEditorViewRadioButton.Size = new System.Drawing.Size(94, 17);
			this.overlayEditorViewRadioButton.TabIndex = 103;
			this.overlayEditorViewRadioButton.Text = "Hybrid Overlay";
			this.overlayEditorViewRadioButton.UseVisualStyleBackColor = true;
			this.overlayEditorViewRadioButton.Click += new System.EventHandler(this.EditorViewModeChanged);
			// 
			// collisionEditorViewRadioButton
			// 
			this.collisionEditorViewRadioButton.AutoSize = true;
			this.collisionEditorViewRadioButton.Checked = true;
			this.collisionEditorViewRadioButton.Location = new System.Drawing.Point(6, 19);
			this.collisionEditorViewRadioButton.Name = "collisionEditorViewRadioButton";
			this.collisionEditorViewRadioButton.Size = new System.Drawing.Size(63, 17);
			this.collisionEditorViewRadioButton.TabIndex = 100;
			this.collisionEditorViewRadioButton.TabStop = true;
			this.collisionEditorViewRadioButton.Text = "Collision";
			this.collisionEditorViewRadioButton.UseVisualStyleBackColor = true;
			this.collisionEditorViewRadioButton.Click += new System.EventHandler(this.EditorViewModeChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.overlayRenderViewRadioButton);
			this.groupBox1.Controls.Add(this.tileRenderViewRadioButton);
			this.groupBox1.Controls.Add(this.collisionRenderViewRadioButton);
			this.groupBox1.ForeColor = System.Drawing.SystemColors.MenuText;
			this.groupBox1.Location = new System.Drawing.Point(11, 248);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(118, 92);
			this.groupBox1.TabIndex = 122;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tile Viewer Setting";
			// 
			// overlayRenderViewRadioButton
			// 
			this.overlayRenderViewRadioButton.AutoSize = true;
			this.overlayRenderViewRadioButton.Location = new System.Drawing.Point(6, 65);
			this.overlayRenderViewRadioButton.Name = "overlayRenderViewRadioButton";
			this.overlayRenderViewRadioButton.Size = new System.Drawing.Size(94, 17);
			this.overlayRenderViewRadioButton.TabIndex = 103;
			this.overlayRenderViewRadioButton.Text = "Hybrid Overlay";
			this.overlayRenderViewRadioButton.UseVisualStyleBackColor = true;
			this.overlayRenderViewRadioButton.Click += new System.EventHandler(this.RenderViewModeChanged);
			// 
			// tileRenderViewRadioButton
			// 
			this.tileRenderViewRadioButton.AutoSize = true;
			this.tileRenderViewRadioButton.Location = new System.Drawing.Point(6, 42);
			this.tileRenderViewRadioButton.Name = "tileRenderViewRadioButton";
			this.tileRenderViewRadioButton.Size = new System.Drawing.Size(42, 17);
			this.tileRenderViewRadioButton.TabIndex = 101;
			this.tileRenderViewRadioButton.Text = "Tile";
			this.tileRenderViewRadioButton.UseVisualStyleBackColor = true;
			this.tileRenderViewRadioButton.Click += new System.EventHandler(this.RenderViewModeChanged);
			// 
			// collisionRenderViewRadioButton
			// 
			this.collisionRenderViewRadioButton.AutoSize = true;
			this.collisionRenderViewRadioButton.Checked = true;
			this.collisionRenderViewRadioButton.Location = new System.Drawing.Point(6, 19);
			this.collisionRenderViewRadioButton.Name = "collisionRenderViewRadioButton";
			this.collisionRenderViewRadioButton.Size = new System.Drawing.Size(63, 17);
			this.collisionRenderViewRadioButton.TabIndex = 100;
			this.collisionRenderViewRadioButton.TabStop = true;
			this.collisionRenderViewRadioButton.Text = "Collision";
			this.collisionRenderViewRadioButton.UseVisualStyleBackColor = true;
			this.collisionRenderViewRadioButton.Click += new System.EventHandler(this.RenderViewModeChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.collisionListRadioButton);
			this.groupBox3.Controls.Add(this.tileListRadioButton);
			this.groupBox3.ForeColor = System.Drawing.SystemColors.MenuText;
			this.groupBox3.Location = new System.Drawing.Point(11, 176);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(100, 66);
			this.groupBox3.TabIndex = 122;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "List Setting";
			// 
			// collisionListRadioButton
			// 
			this.collisionListRadioButton.AutoSize = true;
			this.collisionListRadioButton.Checked = true;
			this.collisionListRadioButton.Location = new System.Drawing.Point(6, 42);
			this.collisionListRadioButton.Name = "collisionListRadioButton";
			this.collisionListRadioButton.Size = new System.Drawing.Size(63, 17);
			this.collisionListRadioButton.TabIndex = 101;
			this.collisionListRadioButton.TabStop = true;
			this.collisionListRadioButton.Text = "Collision";
			this.collisionListRadioButton.UseVisualStyleBackColor = true;
			this.collisionListRadioButton.Click += new System.EventHandler(this.ListViewModeChanged);
			// 
			// tileListRadioButton
			// 
			this.tileListRadioButton.AutoSize = true;
			this.tileListRadioButton.Location = new System.Drawing.Point(6, 19);
			this.tileListRadioButton.Name = "tileListRadioButton";
			this.tileListRadioButton.Size = new System.Drawing.Size(47, 17);
			this.tileListRadioButton.TabIndex = 100;
			this.tileListRadioButton.Text = "Tiles";
			this.tileListRadioButton.UseVisualStyleBackColor = true;
			this.tileListRadioButton.Click += new System.EventHandler(this.ListViewModeChanged);
			// 
			// checkBox4
			// 
			this.checkBox4.BackColor = System.Drawing.Color.Transparent;
			this.checkBox4.Checked = global::TileManiac.Properties.Settings.Default.MirrorMode;
			this.checkBox4.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TileManiac.Properties.Settings.Default, "MirrorMode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBox4.Location = new System.Drawing.Point(11, 142);
			this.checkBox4.Name = "checkBox4";
			this.checkBox4.Size = new System.Drawing.Size(157, 28);
			this.checkBox4.TabIndex = 60;
			this.checkBox4.Text = "Enable Mirror Mode";
			this.checkBox4.UseVisualStyleBackColor = false;
			// 
			// checkBox3
			// 
			this.checkBox3.BackColor = System.Drawing.Color.Transparent;
			this.checkBox3.Checked = global::TileManiac.Properties.Settings.Default.ClassicMode;
			this.checkBox3.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TileManiac.Properties.Settings.Default, "ClassicMode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBox3.Location = new System.Drawing.Point(11, 108);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(157, 28);
			this.checkBox3.TabIndex = 59;
			this.checkBox3.Text = "Show Classic Mode";
			this.checkBox3.UseVisualStyleBackColor = false;
			// 
			// checkBox2
			// 
			this.checkBox2.BackColor = System.Drawing.Color.Transparent;
			this.checkBox2.Checked = global::TileManiac.Properties.Settings.Default.ShowGrid;
			this.checkBox2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TileManiac.Properties.Settings.Default, "ShowGrid", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBox2.Location = new System.Drawing.Point(11, 76);
			this.checkBox2.Margin = new System.Windows.Forms.Padding(1);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(157, 28);
			this.checkBox2.TabIndex = 58;
			this.checkBox2.Text = "Show Grid";
			this.checkBox2.UseVisualStyleBackColor = false;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(13, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(329, 13);
			this.label1.TabIndex = 56;
			this.label1.Text = "Default Options:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// checkBox1
			// 
			this.checkBox1.BackColor = System.Drawing.Color.Transparent;
			this.checkBox1.Checked = global::TileManiac.Properties.Settings.Default.ScrollAllowed;
			this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TileManiac.Properties.Settings.Default, "ScrollAllowed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBox1.Location = new System.Drawing.Point(11, 26);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(157, 46);
			this.checkBox1.TabIndex = 57;
			this.checkBox1.Text = "Scrolling in Classic Mode Allowed";
			this.checkBox1.UseVisualStyleBackColor = false;
			// 
			// OptionsBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(382, 619);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.button8);
			this.Controls.Add(this.button6);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "OptionsBox";
			this.Text = "Tile Maniac Options";
			this.tabPage2.ResumeLayout(false);
			this.panel15.ResumeLayout(false);
			this.panel9.ResumeLayout(false);
			this.panel10.ResumeLayout(false);
			this.panel11.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.Label label85;
        private System.Windows.Forms.CheckBox checkBox37;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox enableWindowsClipboard;
        private System.Windows.Forms.Button importOptionsButton;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.CheckBox checkBox33;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.CheckBox RPCCheckBox;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton overlayEditorViewRadioButton;
        private System.Windows.Forms.RadioButton collisionEditorViewRadioButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton overlayRenderViewRadioButton;
        private System.Windows.Forms.RadioButton tileRenderViewRadioButton;
        private System.Windows.Forms.RadioButton collisionRenderViewRadioButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton collisionListRadioButton;
        private System.Windows.Forms.RadioButton tileListRadioButton;
        private System.Windows.Forms.CheckBox checkBox5;
    }
}