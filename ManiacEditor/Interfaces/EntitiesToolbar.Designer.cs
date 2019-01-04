namespace ManiacEditor
{
    partial class EntitiesToolbar
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
        public void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.entitiesList = new System.Windows.Forms.ComboBox();
            this.entityProperties = new System.Windows.Forms.PropertyGrid();
            this.gbSpawn = new System.Windows.Forms.GroupBox();
            this.defaultFilter = new System.Windows.Forms.ComboBox();
            this.btnSpawn = new System.Windows.Forms.Button();
            this.cbSpawn = new System.Windows.Forms.ComboBox();
            this.gbEditor = new System.Windows.Forms.GroupBox();
            this.MoreButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.otherFilterCheck = new System.Windows.Forms.CheckBox();
            this.bothFilterCheck = new System.Windows.Forms.CheckBox();
            this.encoreFilterCheck = new System.Windows.Forms.CheckBox();
            this.maniaFilterCheck = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.goToThisEntityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.entityPropertiesView1 = new ManiacEditor.Interfaces.EntityPropertiesView();
            this.gbSpawn.SuspendLayout();
            this.gbEditor.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // entitiesList
            // 
            this.entitiesList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.entitiesList.BackColor = System.Drawing.Color.White;
            this.entitiesList.ForeColor = System.Drawing.Color.Black;
            this.entitiesList.FormattingEnabled = true;
            this.entitiesList.Location = new System.Drawing.Point(7, 19);
            this.entitiesList.Name = "entitiesList";
            this.entitiesList.Size = new System.Drawing.Size(204, 21);
            this.entitiesList.TabIndex = 0;
            this.entitiesList.DropDown += new System.EventHandler(this.entitiesList_DropDown);
            this.entitiesList.SelectedIndexChanged += new System.EventHandler(this.EntitiesList_SelectedIndexChanged);
            this.entitiesList.DropDownClosed += new System.EventHandler(this.entitiesList_DropDownClosed);
            this.entitiesList.TextChanged += new System.EventHandler(this.entitiesList_TextChanged);
            // 
            // entityProperties
            // 
            this.entityProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.entityProperties.BackColor = System.Drawing.Color.White;
            this.entityProperties.CategoryForeColor = System.Drawing.Color.Black;
            this.entityProperties.CausesValidation = false;
            this.entityProperties.CommandsBorderColor = System.Drawing.Color.DarkGray;
            this.entityProperties.CommandsForeColor = System.Drawing.Color.Black;
            this.entityProperties.HelpBackColor = System.Drawing.Color.White;
            this.entityProperties.HelpBorderColor = System.Drawing.Color.DarkGray;
            this.entityProperties.HelpForeColor = System.Drawing.Color.Black;
            this.entityProperties.HelpVisible = false;
            this.entityProperties.LineColor = System.Drawing.Color.Silver;
            this.entityProperties.Location = new System.Drawing.Point(7, 46);
            this.entityProperties.Name = "entityProperties";
            this.entityProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.entityProperties.SelectedItemWithFocusBackColor = System.Drawing.Color.DodgerBlue;
            this.entityProperties.SelectedItemWithFocusForeColor = System.Drawing.Color.White;
            this.entityProperties.Size = new System.Drawing.Size(234, 236);
            this.entityProperties.TabIndex = 1;
            this.entityProperties.ToolbarVisible = false;
            this.entityProperties.ViewBackColor = System.Drawing.Color.White;
            this.entityProperties.ViewBorderColor = System.Drawing.Color.DarkGray;
            this.entityProperties.ViewForeColor = System.Drawing.Color.Black;
            this.entityProperties.MouseMove += new System.Windows.Forms.MouseEventHandler(this.entityProperties_MouseHover);
            this.entityProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.entityProperties_PropertyValueChanged);
            this.entityProperties.PropertyTabChanged += new System.Windows.Forms.PropertyTabChangedEventHandler(this.entityProperties_PropertyTabChanged);
            this.entityProperties.Click += new System.EventHandler(this.entityProperties_Click);
            // 
            // gbSpawn
            // 
            this.gbSpawn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSpawn.Controls.Add(this.defaultFilter);
            this.gbSpawn.Controls.Add(this.btnSpawn);
            this.gbSpawn.Controls.Add(this.cbSpawn);
            this.gbSpawn.ForeColor = System.Drawing.SystemColors.WindowText;
            this.gbSpawn.Location = new System.Drawing.Point(6, 6);
            this.gbSpawn.Name = "gbSpawn";
            this.gbSpawn.Size = new System.Drawing.Size(247, 49);
            this.gbSpawn.TabIndex = 2;
            this.gbSpawn.TabStop = false;
            this.gbSpawn.Text = "Entity Spawner";
            this.gbSpawn.Enter += new System.EventHandler(this.gbSpawn_Enter);
            // 
            // defaultFilter
            // 
            this.defaultFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.defaultFilter.BackColor = System.Drawing.Color.White;
            this.defaultFilter.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ManiacEditor.Properties.Settings.Default, "DefaultFilter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.defaultFilter.DisplayMember = "Name";
            this.defaultFilter.ForeColor = System.Drawing.Color.Black;
            this.defaultFilter.FormattingEnabled = true;
            this.defaultFilter.Location = new System.Drawing.Point(168, 20);
            this.defaultFilter.MaxDropDownItems = 3;
            this.defaultFilter.Name = "defaultFilter";
            this.defaultFilter.Size = new System.Drawing.Size(73, 21);
            this.defaultFilter.TabIndex = 2;
            this.defaultFilter.Text = global::ManiacEditor.Properties.Settings.Default.DefaultFilter;
            this.defaultFilter.SelectedIndexChanged += new System.EventHandler(this.defaultFilter_SelectedIndexChanged);
            // 
            // btnSpawn
            // 
            this.btnSpawn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSpawn.ForeColor = System.Drawing.Color.Black;
            this.btnSpawn.Location = new System.Drawing.Point(112, 19);
            this.btnSpawn.Name = "btnSpawn";
            this.btnSpawn.Size = new System.Drawing.Size(50, 23);
            this.btnSpawn.TabIndex = 1;
            this.btnSpawn.Text = "Spawn";
            this.btnSpawn.UseVisualStyleBackColor = true;
            this.btnSpawn.Click += new System.EventHandler(this.btnSpawn_Click);
            // 
            // cbSpawn
            // 
            this.cbSpawn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSpawn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbSpawn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSpawn.BackColor = System.Drawing.Color.White;
            this.cbSpawn.DisplayMember = "Name";
            this.cbSpawn.ForeColor = System.Drawing.Color.Black;
            this.cbSpawn.FormattingEnabled = true;
            this.cbSpawn.Location = new System.Drawing.Point(7, 20);
            this.cbSpawn.Name = "cbSpawn";
            this.cbSpawn.Size = new System.Drawing.Size(99, 21);
            this.cbSpawn.Sorted = true;
            this.cbSpawn.TabIndex = 0;
            this.cbSpawn.SelectedIndexChanged += new System.EventHandler(this.cbSpawn_SelectedIndexChanged);
            this.cbSpawn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbSpawn_KeyDown);
            // 
            // gbEditor
            // 
            this.gbEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbEditor.Controls.Add(this.elementHost1);
            this.gbEditor.Controls.Add(this.MoreButton);
            this.gbEditor.Controls.Add(this.groupBox2);
            this.gbEditor.Controls.Add(this.entitiesList);
            this.gbEditor.Controls.Add(this.entityProperties);
            this.gbEditor.ForeColor = System.Drawing.SystemColors.WindowText;
            this.gbEditor.Location = new System.Drawing.Point(6, 61);
            this.gbEditor.Name = "gbEditor";
            this.gbEditor.Size = new System.Drawing.Size(247, 343);
            this.gbEditor.TabIndex = 3;
            this.gbEditor.TabStop = false;
            this.gbEditor.Text = "Entity Editor";
            this.gbEditor.Enter += new System.EventHandler(this.gbEditor_Enter);
            // 
            // MoreButton
            // 
            this.MoreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MoreButton.Enabled = false;
            this.MoreButton.ForeColor = System.Drawing.Color.Black;
            this.MoreButton.Location = new System.Drawing.Point(217, 19);
            this.MoreButton.Name = "MoreButton";
            this.MoreButton.Size = new System.Drawing.Size(24, 21);
            this.MoreButton.TabIndex = 3;
            this.MoreButton.Text = "...";
            this.MoreButton.UseVisualStyleBackColor = true;
            this.MoreButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.searchBox);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.Location = new System.Drawing.Point(7, 288);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(234, 49);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Search Box";
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.BackColor = System.Drawing.Color.White;
            this.searchBox.ForeColor = System.Drawing.Color.Black;
            this.searchBox.Location = new System.Drawing.Point(6, 23);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(222, 20);
            this.searchBox.TabIndex = 2;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.otherFilterCheck);
            this.groupBox1.Controls.Add(this.bothFilterCheck);
            this.groupBox1.Controls.Add(this.encoreFilterCheck);
            this.groupBox1.Controls.Add(this.maniaFilterCheck);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.groupBox1.Location = new System.Drawing.Point(6, 410);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox1.Size = new System.Drawing.Size(247, 71);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter Entity View";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // otherFilterCheck
            // 
            this.otherFilterCheck.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.otherFilterCheck.Checked = global::ManiacEditor.Properties.Settings.Default.showOtherEntities;
            this.otherFilterCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.otherFilterCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ManiacEditor.Properties.Settings.Default, "showOtherEntities", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.otherFilterCheck.Location = new System.Drawing.Point(131, 45);
            this.otherFilterCheck.Margin = new System.Windows.Forms.Padding(5);
            this.otherFilterCheck.Name = "otherFilterCheck";
            this.otherFilterCheck.Size = new System.Drawing.Size(108, 18);
            this.otherFilterCheck.TabIndex = 33;
            this.otherFilterCheck.Text = "Others ( 0b1000 )";
            this.otherFilterCheck.UseVisualStyleBackColor = true;
            this.otherFilterCheck.CheckedChanged += new System.EventHandler(this.otherFilterCheck_CheckedChanged);
            // 
            // bothFilterCheck
            // 
            this.bothFilterCheck.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.bothFilterCheck.Checked = global::ManiacEditor.Properties.Settings.Default.showBothEntities;
            this.bothFilterCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bothFilterCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ManiacEditor.Properties.Settings.Default, "showBothEntities", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.bothFilterCheck.Location = new System.Drawing.Point(8, 45);
            this.bothFilterCheck.Margin = new System.Windows.Forms.Padding(5);
            this.bothFilterCheck.Name = "bothFilterCheck";
            this.bothFilterCheck.Size = new System.Drawing.Size(120, 18);
            this.bothFilterCheck.TabIndex = 32;
            this.bothFilterCheck.Text = "Common ( 0b0001 )";
            this.bothFilterCheck.UseVisualStyleBackColor = true;
            this.bothFilterCheck.CheckedChanged += new System.EventHandler(this.bothFilterCheck_CheckedChanged);
            // 
            // encoreFilterCheck
            // 
            this.encoreFilterCheck.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.encoreFilterCheck.Checked = global::ManiacEditor.Properties.Settings.Default.showEncoreEntities;
            this.encoreFilterCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.encoreFilterCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ManiacEditor.Properties.Settings.Default, "showEncoreEntities", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.encoreFilterCheck.Location = new System.Drawing.Point(131, 21);
            this.encoreFilterCheck.Margin = new System.Windows.Forms.Padding(5);
            this.encoreFilterCheck.Name = "encoreFilterCheck";
            this.encoreFilterCheck.Size = new System.Drawing.Size(114, 18);
            this.encoreFilterCheck.TabIndex = 31;
            this.encoreFilterCheck.Text = "Encore ( 0b0100 )";
            this.encoreFilterCheck.UseVisualStyleBackColor = true;
            this.encoreFilterCheck.CheckedChanged += new System.EventHandler(this.encoreFilterCheck_CheckedChanged);
            // 
            // maniaFilterCheck
            // 
            this.maniaFilterCheck.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.maniaFilterCheck.Checked = global::ManiacEditor.Properties.Settings.Default.showManiaEntities;
            this.maniaFilterCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.maniaFilterCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ManiacEditor.Properties.Settings.Default, "showManiaEntities", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.maniaFilterCheck.Location = new System.Drawing.Point(8, 21);
            this.maniaFilterCheck.Margin = new System.Windows.Forms.Padding(5);
            this.maniaFilterCheck.Name = "maniaFilterCheck";
            this.maniaFilterCheck.Size = new System.Drawing.Size(108, 18);
            this.maniaFilterCheck.TabIndex = 30;
            this.maniaFilterCheck.Text = "Mania ( 0b0010 )";
            this.maniaFilterCheck.UseVisualStyleBackColor = true;
            this.maniaFilterCheck.CheckedChanged += new System.EventHandler(this.maniaFilterCheck_CheckedChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goToThisEntityToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(159, 26);
            // 
            // goToThisEntityToolStripMenuItem
            // 
            this.goToThisEntityToolStripMenuItem.Name = "goToThisEntityToolStripMenuItem";
            this.goToThisEntityToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.goToThisEntityToolStripMenuItem.Text = "Go to this Entity";
            this.goToThisEntityToolStripMenuItem.Click += new System.EventHandler(this.goToThisEntityToolStripMenuItem_Click);
            // 
            // elementHost1
            // 
            this.elementHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.elementHost1.Location = new System.Drawing.Point(7, 46);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(234, 236);
            this.elementHost1.TabIndex = 5;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.entityPropertiesView1;
            // 
            // EntitiesToolbar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbEditor);
            this.Controls.Add(this.gbSpawn);
            this.Name = "EntitiesToolbar";
            this.Size = new System.Drawing.Size(256, 487);
            this.gbSpawn.ResumeLayout(false);
            this.gbEditor.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ComboBox entitiesList;
        private System.Windows.Forms.PropertyGrid entityProperties;
        private System.Windows.Forms.GroupBox gbSpawn;
        private System.Windows.Forms.Button btnSpawn;
        private System.Windows.Forms.ComboBox cbSpawn;
        private System.Windows.Forms.GroupBox gbEditor;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox otherFilterCheck;
        private System.Windows.Forms.CheckBox bothFilterCheck;
        private System.Windows.Forms.CheckBox encoreFilterCheck;
        private System.Windows.Forms.CheckBox maniaFilterCheck;
        private System.Windows.Forms.ComboBox defaultFilter;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem goToThisEntityToolStripMenuItem;
        public System.Windows.Forms.Button MoreButton;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private Interfaces.EntityPropertiesView entityPropertiesView1;
        //private CSharp.Winform.UI.Loading.WaitLoading waitLoading1;
    }
}
