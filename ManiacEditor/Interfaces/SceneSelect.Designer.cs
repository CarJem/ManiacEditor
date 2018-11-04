using Microsoft.Xna.Framework;

namespace ManiacEditor
{
    partial class SceneSelect
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
            this.scenesTree = new System.Windows.Forms.TreeView();
            this.selectButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.searchLabel = new System.Windows.Forms.Label();
            this.FilterText = new System.Windows.Forms.TextBox();
            this.browse = new System.Windows.Forms.Button();
            this.isFilesView = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSceneInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.recentDataDirList = new System.Windows.Forms.TreeView();
            this.button3 = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dataDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savedPlaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearDataDirectoriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllSavedPlacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataLabelToolStripItem = new System.Windows.Forms.ToolStripTextBox();
            this.folderEditContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeSavedFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataDirEditContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeDataDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preRenderCheckbox = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.folderEditContext.SuspendLayout();
            this.dataDirEditContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // scenesTree
            // 
            this.scenesTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.scenesTree.Location = new System.Drawing.Point(14, 63);
            this.scenesTree.Name = "scenesTree";
            this.scenesTree.Size = new System.Drawing.Size(472, 349);
            this.scenesTree.TabIndex = 0;
            this.scenesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.scenesTree_AfterSelect);
            this.scenesTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.scenesTree_NodeMouseClick);
            this.scenesTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.scenesTree_NodeMouseDoubleClick);
            this.scenesTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.scenesTree_MouseUp);
            // 
            // selectButton
            // 
            this.selectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectButton.Enabled = false;
            this.selectButton.Location = new System.Drawing.Point(381, 418);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(105, 28);
            this.selectButton.TabIndex = 4;
            this.selectButton.Text = "Select";
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(893, 418);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(105, 28);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // searchLabel
            // 
            this.searchLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchLabel.AutoSize = true;
            this.searchLabel.Location = new System.Drawing.Point(11, 37);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(32, 13);
            this.searchLabel.TabIndex = 8;
            this.searchLabel.Text = "Filter:";
            // 
            // FilterText
            // 
            this.FilterText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterText.Location = new System.Drawing.Point(54, 37);
            this.FilterText.Name = "FilterText";
            this.FilterText.Size = new System.Drawing.Size(432, 20);
            this.FilterText.TabIndex = 7;
            this.FilterText.TextChanged += new System.EventHandler(this.FilterText_TextChanged);
            // 
            // browse
            // 
            this.browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browse.Location = new System.Drawing.Point(270, 418);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(105, 28);
            this.browse.TabIndex = 9;
            this.browse.Text = "Browse...";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // isFilesView
            // 
            this.isFilesView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.isFilesView.AutoSize = true;
            this.isFilesView.Location = new System.Drawing.Point(14, 429);
            this.isFilesView.Name = "isFilesView";
            this.isFilesView.Size = new System.Drawing.Size(73, 17);
            this.isFilesView.TabIndex = 6;
            this.isFilesView.Text = "Files View";
            this.isFilesView.UseVisualStyleBackColor = true;
            this.isFilesView.CheckedChanged += new System.EventHandler(this.isFilesView_CheckedChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(196, 26);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.addToolStripMenuItem.Text = "Add Scene to Category";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.deleteSceneInfoToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(166, 48);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItem1.Text = "Edit Scene Info";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // deleteSceneInfoToolStripMenuItem
            // 
            this.deleteSceneInfoToolStripMenuItem.Name = "deleteSceneInfoToolStripMenuItem";
            this.deleteSceneInfoToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.deleteSceneInfoToolStripMenuItem.Text = "Delete Scene Info";
            this.deleteSceneInfoToolStripMenuItem.Click += new System.EventHandler(this.deleteSceneInfoToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(492, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Quick Panel:";
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(320, 324);
            // 
            // recentDataDirList
            // 
            this.recentDataDirList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.recentDataDirList.Location = new System.Drawing.Point(492, 63);
            this.recentDataDirList.Name = "recentDataDirList";
            this.recentDataDirList.Size = new System.Drawing.Size(506, 349);
            this.recentDataDirList.TabIndex = 13;
            this.recentDataDirList.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.recentDataDirList_Click);
            this.recentDataDirList.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.recentDataDirList_NodeMouseDoubleClick);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(718, 418);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 28);
            this.button3.TabIndex = 16;
            this.button3.Text = "Load Selected";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.load_Click);
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Location = new System.Drawing.Point(812, 418);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 28);
            this.addButton.TabIndex = 19;
            this.addButton.Text = "Add...";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataDirectoryToolStripMenuItem,
            this.savedPlaceToolStripMenuItem});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(150, 48);
            // 
            // dataDirectoryToolStripMenuItem
            // 
            this.dataDirectoryToolStripMenuItem.Name = "dataDirectoryToolStripMenuItem";
            this.dataDirectoryToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.dataDirectoryToolStripMenuItem.Text = "Data Directory";
            this.dataDirectoryToolStripMenuItem.Click += new System.EventHandler(this.dataDirectoryToolStripMenuItem_Click);
            // 
            // savedPlaceToolStripMenuItem
            // 
            this.savedPlaceToolStripMenuItem.Name = "savedPlaceToolStripMenuItem";
            this.savedPlaceToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.savedPlaceToolStripMenuItem.Text = "Saved Place";
            this.savedPlaceToolStripMenuItem.Click += new System.EventHandler(this.savedPlaceToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.dataLabelToolStripItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(1010, 24);
            this.menuStrip1.TabIndex = 20;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearDataDirectoriesToolStripMenuItem,
            this.removeAllSavedPlacesToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // clearDataDirectoriesToolStripMenuItem
            // 
            this.clearDataDirectoriesToolStripMenuItem.Name = "clearDataDirectoriesToolStripMenuItem";
            this.clearDataDirectoriesToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.clearDataDirectoriesToolStripMenuItem.Text = "Remove All Data Directories";
            // 
            // removeAllSavedPlacesToolStripMenuItem
            // 
            this.removeAllSavedPlacesToolStripMenuItem.Name = "removeAllSavedPlacesToolStripMenuItem";
            this.removeAllSavedPlacesToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.removeAllSavedPlacesToolStripMenuItem.Text = "Remove All Saved Places";
            // 
            // dataLabelToolStripItem
            // 
            this.dataLabelToolStripItem.AutoSize = false;
            this.dataLabelToolStripItem.BackColor = System.Drawing.SystemColors.Control;
            this.dataLabelToolStripItem.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataLabelToolStripItem.Name = "dataLabelToolStripItem";
            this.dataLabelToolStripItem.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.dataLabelToolStripItem.ReadOnly = true;
            this.dataLabelToolStripItem.Size = new System.Drawing.Size(940, 20);
            this.dataLabelToolStripItem.Text = "Data Directory: NULL";
            // 
            // folderEditContext
            // 
            this.folderEditContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeSavedFolderToolStripMenuItem});
            this.folderEditContext.Name = "folderEditContext";
            this.folderEditContext.Size = new System.Drawing.Size(188, 26);
            // 
            // removeSavedFolderToolStripMenuItem
            // 
            this.removeSavedFolderToolStripMenuItem.Name = "removeSavedFolderToolStripMenuItem";
            this.removeSavedFolderToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.removeSavedFolderToolStripMenuItem.Text = "Remove Saved Folder";
            this.removeSavedFolderToolStripMenuItem.Click += new System.EventHandler(this.removeSavedFolderToolStripMenuItem_Click);
            // 
            // dataDirEditContext
            // 
            this.dataDirEditContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeDataDirectoryToolStripMenuItem});
            this.dataDirEditContext.Name = "dataDirEditContext";
            this.dataDirEditContext.Size = new System.Drawing.Size(196, 26);
            // 
            // removeDataDirectoryToolStripMenuItem
            // 
            this.removeDataDirectoryToolStripMenuItem.Name = "removeDataDirectoryToolStripMenuItem";
            this.removeDataDirectoryToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.removeDataDirectoryToolStripMenuItem.Text = "Remove Data Directory";
            this.removeDataDirectoryToolStripMenuItem.Click += new System.EventHandler(this.removeDataDirectoryToolStripMenuItem_Click);
            // 
            // preRenderCheckbox
            // 
            this.preRenderCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.preRenderCheckbox.AutoSize = true;
            this.preRenderCheckbox.Enabled = false;
            this.preRenderCheckbox.Location = new System.Drawing.Point(93, 429);
            this.preRenderCheckbox.Name = "preRenderCheckbox";
            this.preRenderCheckbox.Size = new System.Drawing.Size(122, 17);
            this.preRenderCheckbox.TabIndex = 21;
            this.preRenderCheckbox.Text = "Pre-Render on Load";
            this.preRenderCheckbox.UseVisualStyleBackColor = true;
            this.preRenderCheckbox.CheckedChanged += new System.EventHandler(this.preRenderCheckbox_CheckedChanged);
            // 
            // SceneSelect
            // 
            this.AcceptButton = this.selectButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(1010, 456);
            this.Controls.Add(this.preRenderCheckbox);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.recentDataDirList);
            this.Controls.Add(this.browse);
            this.Controls.Add(this.searchLabel);
            this.Controls.Add(this.FilterText);
            this.Controls.Add(this.isFilesView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.selectButton);
            this.Controls.Add(this.scenesTree);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 200);
            this.Name = "SceneSelect";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Scene...";
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.folderEditContext.ResumeLayout(false);
            this.dataDirEditContext.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView scenesTree;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox isFilesView;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.TextBox FilterText;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteSceneInfoToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TreeView recentDataDirList;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem dataDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savedPlaceToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearDataDirectoriesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip folderEditContext;
        private System.Windows.Forms.ToolStripMenuItem removeSavedFolderToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip dataDirEditContext;
        private System.Windows.Forms.ToolStripMenuItem removeDataDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllSavedPlacesToolStripMenuItem;
        private System.Windows.Forms.CheckBox preRenderCheckbox;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStripTextBox dataLabelToolStripItem;
    }
}