using Microsoft.Xna.Framework;

namespace ManiacEditor
{
    partial class Sanic2Maniac
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.editSelectedCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.moveCategoryUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveCategoryDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteSceneInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.moveSceneInfoUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveSceneInfoDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.recentDataDirList = new System.Windows.Forms.TreeView();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dataDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savedPlaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderEditContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeSavedFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataDirEditContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeDataDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGameconfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadGameconfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveGameconfigAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveGameconfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataDirectoriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDataDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllDataDirectoriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comingSoonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comingSoonToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.comingSoonToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.filterLabel = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.folderEditContext.SuspendLayout();
            this.dataDirEditContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // scenesTree
            // 
            this.scenesTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.scenesTree.BackColor = System.Drawing.Color.White;
            this.scenesTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scenesTree.ForeColor = System.Drawing.Color.Black;
            this.scenesTree.Location = new System.Drawing.Point(12, 60);
            this.scenesTree.Name = "scenesTree";
            this.scenesTree.Size = new System.Drawing.Size(612, 349);
            this.scenesTree.TabIndex = 0;
            this.scenesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.scenesTree_AfterSelect);
            this.scenesTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.scenesTree_NodeMouseClick);
            this.scenesTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.scenesTree_NodeMouseDoubleClick);
            this.scenesTree.Click += new System.EventHandler(this.scenesTree_Click);
            this.scenesTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.scenesTree_MouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItem2,
            this.editSelectedCategoryToolStripMenuItem,
            this.deleteSelectedCategoryToolStripMenuItem,
            this.toolStripSeparator2,
            this.moveCategoryUpToolStripMenuItem,
            this.moveCategoryDownToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(230, 148);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.addToolStripMenuItem.Text = "Add Scene to Category";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(226, 6);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(229, 22);
            this.toolStripMenuItem2.Text = "Add Category to Gameconfig";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.addCategoryToolStripMenuItem_Click);
            // 
            // editSelectedCategoryToolStripMenuItem
            // 
            this.editSelectedCategoryToolStripMenuItem.Name = "editSelectedCategoryToolStripMenuItem";
            this.editSelectedCategoryToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.editSelectedCategoryToolStripMenuItem.Text = "Edit Selected Category";
            this.editSelectedCategoryToolStripMenuItem.Click += new System.EventHandler(this.editCategoryMenuItem_Click);
            // 
            // deleteSelectedCategoryToolStripMenuItem
            // 
            this.deleteSelectedCategoryToolStripMenuItem.Name = "deleteSelectedCategoryToolStripMenuItem";
            this.deleteSelectedCategoryToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.deleteSelectedCategoryToolStripMenuItem.Text = "Delete Selected Category";
            this.deleteSelectedCategoryToolStripMenuItem.Click += new System.EventHandler(this.deleteCategoryToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(226, 6);
            // 
            // moveCategoryUpToolStripMenuItem
            // 
            this.moveCategoryUpToolStripMenuItem.Name = "moveCategoryUpToolStripMenuItem";
            this.moveCategoryUpToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.moveCategoryUpToolStripMenuItem.Text = "Move Category Up";
            this.moveCategoryUpToolStripMenuItem.Click += new System.EventHandler(this.moveCategoryUpToolStripMenuItem_Click);
            // 
            // moveCategoryDownToolStripMenuItem
            // 
            this.moveCategoryDownToolStripMenuItem.Name = "moveCategoryDownToolStripMenuItem";
            this.moveCategoryDownToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.moveCategoryDownToolStripMenuItem.Text = "Move Category Down";
            this.moveCategoryDownToolStripMenuItem.Click += new System.EventHandler(this.moveCategoryDownToolStripMenuItem_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteSceneInfoToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripSeparator3,
            this.moveSceneInfoUpToolStripMenuItem,
            this.moveSceneInfoDownToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(197, 98);
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
            // 
            // deleteSceneInfoToolStripMenuItem
            // 
            this.deleteSceneInfoToolStripMenuItem.Name = "deleteSceneInfoToolStripMenuItem";
            this.deleteSceneInfoToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.deleteSceneInfoToolStripMenuItem.Text = "Delete Scene Info";
            this.deleteSceneInfoToolStripMenuItem.Click += new System.EventHandler(this.deleteSceneInfoToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(196, 22);
            this.toolStripMenuItem1.Text = "Edit Scene Info";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(193, 6);
            // 
            // moveSceneInfoUpToolStripMenuItem
            // 
            this.moveSceneInfoUpToolStripMenuItem.Name = "moveSceneInfoUpToolStripMenuItem";
            this.moveSceneInfoUpToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.moveSceneInfoUpToolStripMenuItem.Text = "Move Scene Info Up";
            this.moveSceneInfoUpToolStripMenuItem.Click += new System.EventHandler(this.moveSceneInfoUpToolStripMenuItem_Click);
            // 
            // moveSceneInfoDownToolStripMenuItem
            // 
            this.moveSceneInfoDownToolStripMenuItem.Name = "moveSceneInfoDownToolStripMenuItem";
            this.moveSceneInfoDownToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.moveSceneInfoDownToolStripMenuItem.Text = "Move Scene Info Down";
            this.moveSceneInfoDownToolStripMenuItem.Click += new System.EventHandler(this.moveSceneInfoDownToolStripMenuItem_Click);
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
            this.recentDataDirList.BackColor = System.Drawing.Color.White;
            this.recentDataDirList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.recentDataDirList.Location = new System.Drawing.Point(630, 60);
            this.recentDataDirList.Name = "recentDataDirList";
            this.recentDataDirList.Size = new System.Drawing.Size(442, 349);
            this.recentDataDirList.TabIndex = 13;
            this.recentDataDirList.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.recentDataDirList_Click);
            this.recentDataDirList.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.recentDataDirList_NodeMouseDoubleClick);
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
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1084, 24);
            this.menuStrip1.TabIndex = 22;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGameconfigToolStripMenuItem,
            this.loadGameconfigToolStripMenuItem,
            this.saveGameconfigAsToolStripMenuItem,
            this.saveGameconfigToolStripMenuItem,
            this.toolStripSeparator4,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comingSoonToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataDirectoriesToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comingSoonToolStripMenuItem2});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comingSoonToolStripMenuItem1});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // newGameconfigToolStripMenuItem
            // 
            this.newGameconfigToolStripMenuItem.Enabled = false;
            this.newGameconfigToolStripMenuItem.Name = "newGameconfigToolStripMenuItem";
            this.newGameconfigToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.newGameconfigToolStripMenuItem.Text = "New Gameconfig";
            // 
            // loadGameconfigToolStripMenuItem
            // 
            this.loadGameconfigToolStripMenuItem.Enabled = false;
            this.loadGameconfigToolStripMenuItem.Name = "loadGameconfigToolStripMenuItem";
            this.loadGameconfigToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.loadGameconfigToolStripMenuItem.Text = "Load Gameconfig";
            // 
            // saveGameconfigAsToolStripMenuItem
            // 
            this.saveGameconfigAsToolStripMenuItem.Enabled = false;
            this.saveGameconfigAsToolStripMenuItem.Name = "saveGameconfigAsToolStripMenuItem";
            this.saveGameconfigAsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.saveGameconfigAsToolStripMenuItem.Text = "Save Gameconfig As...";
            // 
            // saveGameconfigToolStripMenuItem
            // 
            this.saveGameconfigToolStripMenuItem.Enabled = false;
            this.saveGameconfigToolStripMenuItem.Name = "saveGameconfigToolStripMenuItem";
            this.saveGameconfigToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.saveGameconfigToolStripMenuItem.Text = "Save Gameconfig";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(188, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Enabled = false;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // dataDirectoriesToolStripMenuItem
            // 
            this.dataDirectoriesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDataDirectoryToolStripMenuItem,
            this.removeAllDataDirectoriesToolStripMenuItem});
            this.dataDirectoriesToolStripMenuItem.Name = "dataDirectoriesToolStripMenuItem";
            this.dataDirectoriesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.dataDirectoriesToolStripMenuItem.Text = "Data Directories";
            // 
            // addDataDirectoryToolStripMenuItem
            // 
            this.addDataDirectoryToolStripMenuItem.Name = "addDataDirectoryToolStripMenuItem";
            this.addDataDirectoryToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.addDataDirectoryToolStripMenuItem.Text = "Add Data Directory";
            this.addDataDirectoryToolStripMenuItem.Click += new System.EventHandler(this.dataDirectoryToolStripMenuItem_Click);
            // 
            // removeAllDataDirectoriesToolStripMenuItem
            // 
            this.removeAllDataDirectoriesToolStripMenuItem.Name = "removeAllDataDirectoriesToolStripMenuItem";
            this.removeAllDataDirectoriesToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.removeAllDataDirectoriesToolStripMenuItem.Text = "Remove All Data Directories";
            // 
            // comingSoonToolStripMenuItem
            // 
            this.comingSoonToolStripMenuItem.Enabled = false;
            this.comingSoonToolStripMenuItem.Name = "comingSoonToolStripMenuItem";
            this.comingSoonToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.comingSoonToolStripMenuItem.Text = "Coming Soon!";
            // 
            // comingSoonToolStripMenuItem1
            // 
            this.comingSoonToolStripMenuItem1.Enabled = false;
            this.comingSoonToolStripMenuItem1.Name = "comingSoonToolStripMenuItem1";
            this.comingSoonToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.comingSoonToolStripMenuItem1.Text = "Coming Soon!";
            // 
            // comingSoonToolStripMenuItem2
            // 
            this.comingSoonToolStripMenuItem2.Enabled = false;
            this.comingSoonToolStripMenuItem2.Name = "comingSoonToolStripMenuItem2";
            this.comingSoonToolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.comingSoonToolStripMenuItem2.Text = "Coming Soon!";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.ForeColor = System.Drawing.Color.Black;
            this.textBox1.Location = new System.Drawing.Point(53, 34);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(1019, 20);
            this.textBox1.TabIndex = 23;
            this.textBox1.TextChanged += new System.EventHandler(this.FilterText_TextChanged);
            // 
            // filterLabel
            // 
            this.filterLabel.AutoSize = true;
            this.filterLabel.Location = new System.Drawing.Point(12, 37);
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Size = new System.Drawing.Size(32, 13);
            this.filterLabel.TabIndex = 24;
            this.filterLabel.Text = "Filter:";
            // 
            // Sanic2Maniac
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 419);
            this.Controls.Add(this.filterLabel);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.recentDataDirList);
            this.Controls.Add(this.scenesTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 200);
            this.Name = "Sanic2Maniac";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InsanicManiac Editor";
            this.Load += new System.EventHandler(this.Sanic2Maniac_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip3.ResumeLayout(false);
            this.folderEditContext.ResumeLayout(false);
            this.dataDirEditContext.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView scenesTree;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteSceneInfoToolStripMenuItem;
        private System.Windows.Forms.TreeView recentDataDirList;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem dataDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savedPlaceToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip folderEditContext;
        private System.Windows.Forms.ToolStripMenuItem removeSavedFolderToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip dataDirEditContext;
        private System.Windows.Forms.ToolStripMenuItem removeDataDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem editSelectedCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem moveCategoryUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveCategoryDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem moveSceneInfoUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveSceneInfoDownToolStripMenuItem;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newGameconfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadGameconfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveGameconfigAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveGameconfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataDirectoriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addDataDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem comingSoonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem comingSoonToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem removeAllDataDirectoriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem comingSoonToolStripMenuItem2;
        private System.Windows.Forms.Label filterLabel;
        private System.Windows.Forms.TextBox textBox1;
    }
}