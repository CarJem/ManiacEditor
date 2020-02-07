using RSDKv5;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ImageList = System.Windows.Forms.ImageList;
using MouseButtons = System.Windows.Forms.MouseButtons;
using TreeNode = System.Windows.Forms.TreeNode;
using TreeNodeMouseClickEventArgs = System.Windows.Forms.TreeNodeMouseClickEventArgs;
using TreeViewEventArgs = System.Windows.Forms.TreeViewEventArgs;

namespace ManiacEditor.Controls.SceneSelect
{
    public partial class SceneSelectHost : UserControl
	{
		public List<Tuple<string, List<Tuple<string, string>>>> Categories = new List<Tuple<string, List<Tuple<string, string>>>>();
		public Dictionary<string, List<Tuple<string, Tuple<Gameconfig.SceneInfo, string>>>> Directories = new Dictionary<string, List<Tuple<string, Tuple<Gameconfig.SceneInfo, string>>>>();

		public bool isFileViewMode { get => isFilesView.IsChecked.Value; }

		public Gameconfig _GameConfig;
		public SceneSelectWindow Window;

		
		public string PreviousDataFolder; //In the case we have a bad gameconfig
        public bool WithinAParentForm = false;
		public bool UsingDataPack = false;
		public int SelectedCategoryIndex = -1;
		public Controls.Editor.MainEditor EditorInstance;


        //Information For the File Handler
        public string SelectedSceneResult = null;
        public int LevelID = -1;
		public string CurrentZone = "";
		public string CurrentName = "";
		public string CurrentSceneID = "";
        public bool isEncore = false;
        public bool Browsed = false;

        #region Winform Stuff
        private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem deleteSceneInfoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteSelectedCategoryToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem editSelectedCategoryToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem moveCategoryUpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem moveCategoryDownToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem moveSceneInfoUpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem moveSceneInfoDownToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ContextMenuStrip folderEditContext;
		private System.Windows.Forms.ToolStripMenuItem removeSavedFolderToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip dataDirEditContext;
		private System.Windows.Forms.ToolStripMenuItem removeDataDirectoryToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip DataPackContextMenu;
		private System.Windows.Forms.ToolStripMenuItem EditDataPacksToolStripMenuItems;
		private System.Windows.Forms.TreeView ScenesTree;
		private System.Windows.Forms.TreeView RecentsTree;
        #endregion

        public SceneSelectHost(Gameconfig config = null, Controls.Editor.MainEditor _Instance = null, SceneSelectWindow _Window = null)
		{
			if (Core.Settings.MySettings.DataDirectories != null)  Core.Settings.MySettings.DataDirectories.Remove(null);
			if (Core.Settings.MySettings.SavedPlaces != null)  Core.Settings.MySettings.SavedPlaces.Remove(null);

			EditorInstance = _Instance;
			InitializeComponent();
			SetupWinFormTreeStuff();
			WithinAParentForm = (Window != null);
			Window = _Window;
			ReloadRecentsTree();
            SetupGameConfig(config);
            UpdateSceneSelectTheme();
            ScenesTree.Visible = true;
		}

        #region Setup Region
        public void SetupWinFormTreeStuff()
        {


            this.components = new System.ComponentModel.Container();
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
            this.DataPackContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.EditDataPacksToolStripMenuItems = new System.Windows.Forms.ToolStripMenuItem();
            this.ScenesTree = new System.Windows.Forms.TreeView();
            this.RecentsTree = new System.Windows.Forms.TreeView();

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
            this.moveCategoryDownToolStripMenuItem
            });
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(230, 148);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ScenesTreeSceneContextMenuOpeningEvent);
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
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.ScenesTreeCategoryContextMenuOpeningEvent);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.addToolStripMenuItem.Text = "Add Scene to Category";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.GameConfigAddSceneEvent);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(226, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(226, 6);

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
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.ScenesTreeCategoryContextMenuOpeningEvent);
            this.folderEditContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeSavedFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataDirEditContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeDataDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            // 
            // deleteSceneInfoToolStripMenuItem
            // 
            this.deleteSceneInfoToolStripMenuItem.Name = "deleteSceneInfoToolStripMenuItem";
            this.deleteSceneInfoToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.deleteSceneInfoToolStripMenuItem.Text = "Delete Scene Info";
            this.deleteSceneInfoToolStripMenuItem.Click += new System.EventHandler(this.GameConfigDeleteSceneEvent);
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
            this.moveSceneInfoUpToolStripMenuItem.Click += new System.EventHandler(this.GameConfigMoveSceneUpEvent);
            // 
            // moveSceneInfoDownToolStripMenuItem
            // 
            this.moveSceneInfoDownToolStripMenuItem.Name = "moveSceneInfoDownToolStripMenuItem";
            this.moveSceneInfoDownToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.moveSceneInfoDownToolStripMenuItem.Text = "Move Scene Info Down";
            this.moveSceneInfoDownToolStripMenuItem.Click += new System.EventHandler(this.GameConfigMoveSceneDownEvent);
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
            this.toolStripMenuItem2.Click += new System.EventHandler(this.GameConfigAddCategoryEvent);
            // 
            // editSelectedCategoryToolStripMenuItem
            // 
            this.editSelectedCategoryToolStripMenuItem.Name = "editSelectedCategoryToolStripMenuItem";
            this.editSelectedCategoryToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.editSelectedCategoryToolStripMenuItem.Text = "Edit Selected Category";
            this.editSelectedCategoryToolStripMenuItem.Click += new System.EventHandler(this.GameConfigEditCategoryEvent);
            // 
            // deleteSelectedCategoryToolStripMenuItem
            // 
            this.deleteSelectedCategoryToolStripMenuItem.Name = "deleteSelectedCategoryToolStripMenuItem";
            this.deleteSelectedCategoryToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.deleteSelectedCategoryToolStripMenuItem.Text = "Delete Selected Category";
            this.deleteSelectedCategoryToolStripMenuItem.Click += new System.EventHandler(this.GameConfigDeleteCategoryEvent);
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
            this.moveCategoryUpToolStripMenuItem.Click += new System.EventHandler(this.GameConfigMoveCategoryUpEvent);
            // 
            // moveCategoryDownToolStripMenuItem
            // 
            this.moveCategoryDownToolStripMenuItem.Name = "moveCategoryDownToolStripMenuItem";
            this.moveCategoryDownToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.moveCategoryDownToolStripMenuItem.Text = "Move Category Down";
            this.moveCategoryDownToolStripMenuItem.Click += new System.EventHandler(this.GameConfigMoveCategoryDownEvent);
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
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.ScenesTreeCategoryContextMenuOpeningEvent);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(196, 22);
            this.toolStripMenuItem1.Text = "Edit Scene Info";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.GameConfigEditSceneEvent);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(193, 6);
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
            this.removeDataDirectoryToolStripMenuItem.Click += new System.EventHandler(this.RemoveDataFolderEvent);
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
            this.removeSavedFolderToolStripMenuItem.Click += new System.EventHandler(this.RemoveSavedPlaceEvent);
            // 
            // modEditContext
            // 
            this.DataPackContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EditDataPacksToolStripMenuItems});
            this.DataPackContextMenu.Name = "folderEditContext";
            this.DataPackContextMenu.Size = new System.Drawing.Size(194, 70);
            // 
            // EditDataPacksToolStripMenuItems
            // 
            this.EditDataPacksToolStripMenuItems.Name = "EditDataPacksToolStripMenuItems";
            this.EditDataPacksToolStripMenuItems.Size = new System.Drawing.Size(193, 22);
            this.EditDataPacksToolStripMenuItems.Text = "Edit Data Packs...";
            this.EditDataPacksToolStripMenuItems.Click += new System.EventHandler(this.EditModPacksEvent);
            // 
            // scenesTree
            // 
            this.ScenesTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ScenesTree.BackColor = System.Drawing.Color.White;
            this.ScenesTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ScenesTree.ForeColor = System.Drawing.Color.Black;
            this.ScenesTree.Name = "scenesTree";
            this.ScenesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScenesTree.TabIndex = 0;
            this.ScenesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ScenesTreeAfterSelectEvent);
            this.ScenesTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ScenesTreeNodeMouseDoubleClickEvent);
            this.ScenesTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ScenesTreeNodeDoubleClickEvent);
            this.ScenesTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScenesTreeNodeMouseUpEvent);
            this.ScenesTree.HideSelection = false;
            // 
            // recentDataDirList
            // 
            this.RecentsTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RecentsTree.BackColor = System.Drawing.Color.White;
            this.RecentsTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RecentsTree.Name = "recentDataDirList";
            this.RecentsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RecentsTree.TabIndex = 13;
            this.RecentsTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.RecentsTreeMouseDownEvent);
            this.RecentsTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.RecentsTreeNodeDoubleClick);
            this.RecentsTree.HideSelection = false;


            ScenesTree.Show();
            RecentsTree.Show();
        }

        public void UpdateSceneSelectTheme()
        {
            if (App.Skin == Skin.Dark)
            {
                ScenesTree.BackColor = Methods.Internal.Theming.darkTheme1;
                ScenesTree.ForeColor = Methods.Internal.Theming.darkTheme3;

                RecentsTree.BackColor = Methods.Internal.Theming.darkTheme1;
                RecentsTree.ForeColor = Methods.Internal.Theming.darkTheme3;
            }



            else
            {
                ScenesTree.BackColor = System.Drawing.Color.White;
                ScenesTree.ForeColor = System.Drawing.Color.Black;

                RecentsTree.BackColor = System.Drawing.Color.White;
                RecentsTree.ForeColor = System.Drawing.Color.Black;
            }
        }

        public void SetupGameConfig(Gameconfig config)
        {
            if (config != null)
            {
                browse.IsEnabled = true;
                LoadFromGameConfig(config);
                _GameConfig = config;
            }
            else
            {
                browse.IsEnabled = false;
            }
        }
        #endregion

        public void UpdateToolstrip(object sender, EventArgs e)
		{
			if (ScenesTree.SelectedNode != null)
			{
				SelectedCategoryIndex = ScenesTree.SelectedNode.Index;
			}
			else
			{
				SelectedCategoryIndex = -1;
			}
		}

        #region Scenes Tree Events
        private void ScenesTreeNodeDoubleClickEvent(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (selectButton.IsEnabled)
            {
                UpdateSelectedSceneInfo();
                SelectButtonEvent(null, null);
            }
        }

        private void SceneTreeUpdateRequiredEvent(object sender, RoutedEventArgs e)
        {
            UpdateTree();
        }

        private void ScenesTreeAfterSelectEvent(object sender, TreeViewEventArgs e)
        {
            selectButton.IsEnabled = ScenesTree.SelectedNode.Tag != null;
            UpdateSelectedSceneInfo();
        }

        private void ScenesTreeNodeMouseDoubleClickEvent(object sender, TreeNodeMouseClickEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                ScenesTree.SelectedNode = e.Node;
                if (e.Node.ImageKey == "Folder")
                    contextMenuStrip1.Show(ScenesTree, e.Location);
                else if (e.Node.ImageKey == "File")
                    contextMenuStrip2.Show(ScenesTree, e.Location);
            }
            UpdateSelectedSceneInfo();
        }

        private void ScenesTreeNodeMouseUpEvent(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (ScenesTree.SelectedNode == null)
            {
                selectButton.IsEnabled = false;
            }
            if (_GameConfig != null) browse.IsEnabled = true;
            else browse.IsEnabled = true;
            UpdateSelectedSceneInfo();
        }
        #endregion

        #region Browse Events
        private void BrowseEvent(object sender, RoutedEventArgs e)
		{
			System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
			open.Filter = "Scene File|*.bin";
			if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
			{
				SelectedSceneResult = open.FileName;
				Browsed = true;
				Close();
			}
		}
		private void SavedPlacesBrowseEvent(string initialDir)
		{
			System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
			open.InitialDirectory = initialDir;
			open.Filter = "Scene File|*.bin";
			if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
			{
				SelectedSceneResult = open.FileName;             
				Browsed = true;
				Close();
			}
		}
        #endregion

        #region Scenes Tree Context Menu Events

        private void ScenesTreeSceneContextMenuOpeningEvent(object sender, CancelEventArgs e)
        {
            if (isFileViewMode)
            {
                contextMenuStrip1.Enabled = false;
            }
            else
            {
                contextMenuStrip1.Enabled = true;
                if (SelectedCategoryIndex != -1)
                {
                    if (ScenesTree.SelectedNode.Index == 0)
                    {
                        moveCategoryUpToolStripMenuItem.Enabled = false;
                    }
                    else
                    {
                        moveCategoryUpToolStripMenuItem.Enabled = true;
                    }

                    if (ScenesTree.SelectedNode.Index == ScenesTree.Nodes.Count - 1)
                    {
                        moveCategoryDownToolStripMenuItem.Enabled = false;
                    }
                    else
                    {
                        moveCategoryDownToolStripMenuItem.Enabled = true;
                    }
                }
                else
                {
                    moveCategoryUpToolStripMenuItem.Enabled = false;
                    moveCategoryDownToolStripMenuItem.Enabled = false;
                }
            }


        }
        private void ScenesTreeCategoryContextMenuOpeningEvent(object sender, CancelEventArgs e)
        {
            if (isFileViewMode)
            {
                contextMenuStrip2.Enabled = false;
            }
            else
            {
                contextMenuStrip2.Enabled = true;
                moveSceneInfoDownToolStripMenuItem.Enabled = false;
                moveSceneInfoUpToolStripMenuItem.Enabled = false;
            }
        }

        #region GameConfig Events

        bool isFunctional = true;

        public void GameConfigThrowError()
        {
            System.Windows.MessageBox.Show("RSDK-Reverse's GameConfig Library for RSDKv5 currently is Broken, and to prvent unwanted corruption to you gameconfigs, I have disabled this feature for this build.", "Feature Disabled");
        }

        private void GameConfigAddSceneEvent(object sender, EventArgs e)
        {
            if (!isFunctional)
            {
                GameConfigThrowError();
                return;
            }
            var form = new EditSceneSelectInfoWindow();
            if (Window != null)
            {
                form.Owner = System.Windows.Window.GetWindow(Window);
            }
            else if (WithinAParentForm)
            {
                form.Owner = System.Windows.Window.GetWindow(EditorInstance.StartScreen);
            }
            form.ShowDialog();
            if (form.DialogResult == true)
            {
                var cat = _GameConfig.Categories.Where(t => t.Name == ScenesTree.SelectedNode.Text).FirstOrDefault();
                if (cat != null)
                {
                    cat.Scenes.Add(form.Scene);
                    LoadFromGameConfig(_GameConfig);
                    WriteGameConfigChangesToFile();
                }
            }
        }
        private void GameConfigAddCategoryEvent(object sender, EventArgs e)
        {
            if (!isFunctional)
            {
                GameConfigThrowError();
                return;
            }
            var form = new EditSceneSelectInfoWindow();
            if (Window != null)
            {
                form.Owner = System.Windows.Window.GetWindow(Window);
            }
            else if (WithinAParentForm)
            {
                form.Owner = System.Windows.Window.GetWindow(EditorInstance.StartScreen);
            }
            form.ShowDialog();
            if (form.DialogResult == true)
            {
                var scenes = new List<RSDKv5.Gameconfig.SceneInfo>();
                scenes.Add(form.Scene);

                var form2 = new SceneSelect.SceneSelectEditCategoryLabelWindow();
                form.Owner = System.Windows.Window.GetWindow(Window);
                form2.Scenes = scenes;
                form2.ShowDialog();

                if (form2.DialogResult == true)
                {
                    if (form2.Category != null)
                    {
                        _GameConfig.Categories.Insert(ScenesTree.SelectedNode.Index, form2.Category);
                        LoadFromGameConfig(_GameConfig);
                        WriteGameConfigChangesToFile();
                    }


                }
            }
        }
        private void GameConfigEditSceneEvent(object sender, EventArgs e)
        {
            if (!isFunctional)
            {
                GameConfigThrowError();
                return;
            }
            var cat = _GameConfig.Categories.Where(t => t.Name == ScenesTree.SelectedNode.Parent.Text).FirstOrDefault();
            if (cat != null)
            {
                var scene = cat.Scenes.Where(t => t.Index == ScenesTree.SelectedNode.Index).FirstOrDefault();
                var form = new EditSceneSelectInfoWindow(scene);
                if (Window != null)
                {
                    form.Owner = System.Windows.Window.GetWindow(Window);
                }
                else if (WithinAParentForm)
                {
                    form.Owner = System.Windows.Window.GetWindow(EditorInstance.StartScreen);
                }
                form.ShowDialog();
                if (form.DialogResult == true)
                {
                    LoadFromGameConfig(_GameConfig);
                    WriteGameConfigChangesToFile();
                }
            }
        }
        private void GameConfigEditCategoryEvent(object sender, EventArgs e)
        {
            if (!isFunctional)
            {
                GameConfigThrowError();
                return;
            }
            var Category = _GameConfig.Categories[ScenesTree.SelectedNode.Index];
            var form = new SceneSelectEditCategoryLabelWindow(Category, Category.Scenes);
            if (Window != null)
            {
                form.Owner = System.Windows.Window.GetWindow(Window);
            }
            else if (WithinAParentForm)
            {
                form.Owner = System.Windows.Window.GetWindow(EditorInstance.StartScreen);
            }
            form.ShowDialog();
            if (form.DialogResult == true)
            {
                LoadFromGameConfig(_GameConfig);
                WriteGameConfigChangesToFile();
            }

        }
        private void GameConfigDeleteSceneEvent(object sender, EventArgs e)
        {
            if (!isFunctional)
            {
                GameConfigThrowError();
                return;
            }
            var cat = _GameConfig.Categories.Where(t => t.Name == ScenesTree.SelectedNode.Parent.Text).FirstOrDefault();
            if (cat != null)
            {
                var scene = cat.Scenes.FindIndex(t => t.Index == ScenesTree.SelectedNode.Index);
                if (scene + 1 < cat.Scenes.Count && !char.IsDigit(cat.Scenes[scene].Name[0]) && char.IsDigit(cat.Scenes[scene + 1].Name[0]))
                {
                    if (MessageBox.Show("This Scene as other acts attached,\n" +
                        "Deleting this scene will set the next scene as the main scene of the stage, \n" +
                        "Are you sure you want to continue?",
                        "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        return;
                    cat.Scenes[scene + 1].Name = cat.Scenes[scene].Name.
                        Replace(" " + cat.Scenes[scene].SceneID, " " + cat.Scenes[scene + 1].SceneID);
                }
                cat.Scenes.RemoveAt(scene);
                LoadFromGameConfig(_GameConfig);
                WriteGameConfigChangesToFile();

            }
        }
        private void GameConfigDeleteCategoryEvent(object sender, EventArgs e)
        {
            if (!isFunctional)
            {
                GameConfigThrowError();
                return;
            }
            _GameConfig.Categories.RemoveAt(ScenesTree.SelectedNode.Index);
            LoadFromGameConfig(_GameConfig);
            WriteGameConfigChangesToFile();
        }
        private void GameConfigMoveCategoryUpEvent(object sender, EventArgs e)
        {
            if (!isFunctional)
            {
                GameConfigThrowError();
                return;
            }
            var item = _GameConfig.Categories[ScenesTree.SelectedNode.Index];
            int OldIndex = _GameConfig.Categories.IndexOf(item);
            var itemAbove = _GameConfig.Categories[ScenesTree.SelectedNode.Index - 1];
            _GameConfig.Categories.RemoveAt(ScenesTree.SelectedNode.Index);
            int NewIndex = _GameConfig.Categories.IndexOf(itemAbove);

            if (NewIndex == OldIndex) NewIndex--;

            _GameConfig.Categories.Insert(NewIndex, item);

            LoadFromGameConfig(_GameConfig);
            WriteGameConfigChangesToFile();

        }
        private void GameConfigMoveCategoryDownEvent(object sender, EventArgs e)
        {
            if (!isFunctional)
            {
                GameConfigThrowError();
                return;
            }
            var item = _GameConfig.Categories[ScenesTree.SelectedNode.Index];
            int OldIndex = _GameConfig.Categories.IndexOf(item);
            var itemAbove = _GameConfig.Categories[ScenesTree.SelectedNode.Index + 1];
            _GameConfig.Categories.RemoveAt(ScenesTree.SelectedNode.Index);
            int NewIndex = _GameConfig.Categories.IndexOf(itemAbove);

            if (NewIndex == OldIndex) NewIndex++;

            _GameConfig.Categories.Insert(NewIndex, item);

            LoadFromGameConfig(_GameConfig);
            WriteGameConfigChangesToFile();

        }
        private void GameConfigMoveSceneUpEvent(object sender, EventArgs e)
        {
            if (!isFunctional)
            {
                GameConfigThrowError();
                return;
            }
            var cat = _GameConfig.Categories.Where(t => t.Name == ScenesTree.SelectedNode.Parent.Text).FirstOrDefault();
            if (cat != null)
            {
                var scene = cat.Scenes.Where(t => t.Index == ScenesTree.SelectedNode.Index).FirstOrDefault();
                int OldIndex = cat.Scenes.IndexOf(scene);
                var sceneAbove = cat.Scenes.Where(t => t.Index == ScenesTree.SelectedNode.Index - 1).FirstOrDefault();
                cat.Scenes.Remove(scene);
                int NewIndex = cat.Scenes.IndexOf(sceneAbove);

                if (NewIndex == OldIndex) NewIndex--;
                cat.Scenes.Insert(NewIndex, scene);

                LoadFromGameConfig(_GameConfig);
                WriteGameConfigChangesToFile();

            }
        }
        private void GameConfigMoveSceneDownEvent(object sender, EventArgs e)
        {
            if (!isFunctional)
            {
                GameConfigThrowError();
                return;
            }
            var cat = _GameConfig.Categories.Where(t => t.Name == ScenesTree.SelectedNode.Parent.Text).FirstOrDefault();
            if (cat != null)
            {

                var scene = cat.Scenes.Where(t => t.Index == ScenesTree.SelectedNode.Index).FirstOrDefault();
                int OldIndex = cat.Scenes.IndexOf(scene);
                var sceneBelow = cat.Scenes.Where(t => t.Index == ScenesTree.SelectedNode.Index + 1).FirstOrDefault();
                cat.Scenes.Remove(scene);
                int NewIndex = cat.Scenes.IndexOf(sceneBelow);

                if (NewIndex == OldIndex) NewIndex++;
                cat.Scenes.Insert(NewIndex, scene);


                LoadFromGameConfig(_GameConfig);
                WriteGameConfigChangesToFile();
            }
        }
        private void WriteGameConfigChangesToFile()
        {
            if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                _GameConfig.Write(_GameConfig.FilePath);
        }

        #endregion

        #endregion

        #region Recents Tree Context Menu Item Events
        private void RecentsTreeMouseDownEvent(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RecentsTree.SelectedNode = e.Node;
                if (e.Node.ImageKey == "DataFolder")
                    dataDirEditContext.Show(RecentsTree, e.Location);
                else if (e.Node.ImageKey == "SavedPlace")
                    folderEditContext.Show(RecentsTree, e.Location);
                else if (e.Node.ImageKey == "DataPack")
                    DataPackContextMenu.Show(RecentsTree, e.Location);
            }


        }
        private void RemoveSavedPlaceEvent(object sender, EventArgs e)
		{
			String toRemove = RecentsTree.SelectedNode.Tag.ToString();
			if (Core.Settings.MySettings.SavedPlaces.Contains(toRemove))
			{
				Core.Settings.MySettings.SavedPlaces.Remove(toRemove);
			}
			ReloadRecentsTree();
		}
		private void RemoveDataFolderEvent(object sender, EventArgs e)
		{
			String toRemove = RecentsTree.SelectedNode.Tag.ToString();
			if (Core.Settings.MySettings.DataDirectories.Contains(toRemove))
			{
				Core.Settings.MySettings.DataDirectories.Remove(toRemove);
			}
			
			ReloadRecentsTree();
		}
        #endregion

        #region Loading / Refreshing / Unloading / Updating
        private void UnloadDataPackEvent(object sender, RoutedEventArgs e)
        {
            if (RecentsTree.SelectedNode != null) LoadDataDirectory(RecentsTree.SelectedNode.Tag.ToString());
        }
        public void ReloadRecentsTree()
        {
            if (EditorInstance == null) return;

            if (EditorInstance.DataDirectory != null) dataLabelToolStripItem.Content = "Data Directory: " + EditorInstance.DataDirectory;
            else dataLabelToolStripItem.Content = "Data Directory: NULL";

            RecentsTree.Nodes.Clear();
            RecentsTree.Nodes.Add("Recent Data Directories");
            RecentsTree.Nodes.Add("Saved Places");
            RecentsTree.Nodes.Add("Data Packs");
            RecentsTree.ImageList = new ImageList();
            RecentsTree.ImageList.Images.Add("Folder", Properties.Resources.folder);
            RecentsTree.ImageList.Images.Add("File", Properties.Resources.file);

            if (Core.Settings.MySettings.DataDirectories != null && Core.Settings.MySettings.DataDirectories?.Count > 0)
            {
                foreach (string dataDir in Core.Settings.MySettings.DataDirectories)
                {
                    var node = RecentsTree.Nodes[0].Nodes.Add(dataDir);
                    node.Tag = dataDir;
                    node.ToolTipText = dataDir;
                    node.ImageKey = "DataFolder";
                }
                RecentsTree.Nodes[0].ExpandAll();
            }

            if (Core.Settings.MySettings.SavedPlaces != null && Core.Settings.MySettings.SavedPlaces?.Count > 0)
            {
                StringCollection recentFolders = new StringCollection();
                this.RecentsTree.ImageList.Images.Add("SubFolder", Properties.Resources.folder);
                int index = this.RecentsTree.ImageList.Images.IndexOfKey("SubFolder");
                recentFolders = Core.Settings.MySettings.SavedPlaces;
                foreach (string folder in recentFolders)
                {
                    var node = RecentsTree.Nodes[1].Nodes.Add(folder, folder, index, index);
                    node.Tag = folder;
                    node.ToolTipText = folder;
                    node.ImageKey = "SavedPlace";
                }
                RecentsTree.Nodes[1].ExpandAll();
            }

            if (ManiacEditor.Methods.Prefrences.DataPackStorage.ModListInformation?.Count > 0)
            {
                List<string> modPacks = new List<string>();
                this.RecentsTree.ImageList.Images.Add("SubFolder", Properties.Resources.folder);
                int index = this.RecentsTree.ImageList.Images.IndexOfKey("SubFolder");
                modPacks = ManiacEditor.Methods.Prefrences.DataPackStorage.DataPackNamesToList();
                foreach (string packs in modPacks)
                {
                    var node = RecentsTree.Nodes[2].Nodes.Add(packs, packs, index, index);
                    node.Tag = modPacks.IndexOf(packs);
                    node.ToolTipText = packs;
                    node.ImageKey = "DataPack";
                }
                RecentsTree.Nodes[2].ExpandAll();
            }

        }
        private void LoadEvent(object sender, RoutedEventArgs e)
        {
            if (Classes.Editor.SolutionState.isImportingObjects == true)
            {
                MessageBox.Show("You can't do that while importing objects!");
            }
            else
            {
                if (RecentsTree.SelectedNode.ImageKey == "DataFolder")
                {
                    UnloadDataPack();
                    LoadDataDirectory(RecentsTree.SelectedNode.Tag.ToString());
                }
                else if (RecentsTree.SelectedNode.ImageKey == "DataPack")
                {
                    UnloadDataPack();
                    LoadDataPackFromTag(RecentsTree.SelectedNode.Tag.ToString());
                }
                else if (RecentsTree.SelectedNode.ImageKey == "SavedPlace")
                {
                    if (_GameConfig != null) SavedPlacesBrowseEvent(RecentsTree.SelectedNode.Tag.ToString());
                    else MessageBox.Show("Please Select/Open a Data Directory First", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        public void LoadDataDirectory(string dataDirectory)
        {
            EditorInstance.DataDirectory = dataDirectory;

            bool AllowedToProceed = true;

            if (EditorInstance.DataDirectory != null) dataLabelToolStripItem.Content = "Data Directory: " + EditorInstance.DataDirectory;
            else
            {
                UnloadDataDirectory();
                AllowedToProceed = false;
            }

            if (AllowedToProceed)
            {
                String SelectedDataDirectory = RecentsTree.SelectedNode.Tag.ToString();
                Gameconfig GameConfig = EditorInstance.Paths.SetandReturnGameConfig(SelectedDataDirectory);
                if (GameConfig != null)
                {
                    _GameConfig = GameConfig;
                    LoadFromGameConfig(_GameConfig);
                }
                else
                {
                    UnloadDataDirectory();
                }
            }
            else
            {
                EditorInstance.DataDirectory = "";
            }

        }
        private void UnloadDataDirectory()
        {
            dataLabelToolStripItem.Content = "Data Directory: NULL";
            _GameConfig = null;
            ReloadScenesTree(_GameConfig);
            UpdateTree();
        }


        public void UnloadDataPack()
        {
            EditorInstance.ResourcePackList.Clear();
            Classes.Editor.SolutionState.DataDirectoryReadOnlyMode = false;
            EditorInstance.DataDirectory = null;
            dataPackStatusLabel.Content = "";
            UnloadDataDirectory();
        }
        public void LoadDataPackFromTag(string tag)
        {
            PreviousDataFolder = EditorInstance.DataDirectory;
            if (!Int32.TryParse(tag, out int Index)) return;
            if (ManiacEditor.Methods.Prefrences.DataPackStorage.ModListInformation == null) return;

            var pack = ManiacEditor.Methods.Prefrences.DataPackStorage.ModListInformation[Index];

            LoadDataPack(pack);
        }
        public void LoadDataPackFromName(string packName)
        {
            PreviousDataFolder = EditorInstance.DataDirectory;
            if (ManiacEditor.Methods.Prefrences.DataPackStorage.ModListInformation == null) return;

            var pack = ManiacEditor.Methods.Prefrences.DataPackStorage.ModListInformation.Where(x => x.Item1 == packName).FirstOrDefault();

            LoadDataPack(pack);
        }
        public void LoadDataPack(Tuple<string,List<Tuple<string,string>>> pack)
        {
            bool AllowedToProceed = true;
            EditorInstance.LoadedDataPack = pack.Item1;

            foreach (var item in pack.Item2)
            {
                if (item.Item1 == "DataDir") EditorInstance.DataDirectory = item.Item2;
                else if (item.Item1 == "Mod") EditorInstance.ResourcePackList.Add(item.Item2);
                else if (item.Item1 == "ReadOnlyDataFolder" && item.Item2 == "TRUE") Classes.Editor.SolutionState.DataDirectoryReadOnlyMode = true;
            }
            Gameconfig GameConfig = EditorInstance.Paths.SetandReturnGameConfig();

            if (GameConfig == null) AllowedToProceed = false;
            if (EditorInstance.DataDirectory == null) AllowedToProceed = false;

            if (AllowedToProceed)
            {
                dataLabelToolStripItem.Content = "Data Directory: " + EditorInstance.DataDirectory;
                dataPackStatusLabel.Content = "Loaded Data Pack: " + EditorInstance.LoadedDataPack;
                LoadFromGameConfig(GameConfig);
                _GameConfig = GameConfig;
            }
            else
            {
                _GameConfig = null;
                UnloadDataDirectory();
                dataPackStatusLabel.Content = "";
                ReloadScenesTree(_GameConfig);
            }
        }
        public void LoadFromGameConfig(Gameconfig config)
        {
            ReloadScenesTree(config);
            if (Core.Settings.MyDefaults.SceneSelectFilesViewDefault) this.isFilesView.IsChecked = true;
            else this.isFilesView.IsChecked = false;
        }
        public void ReloadScenesTree(Gameconfig config)
        {
            Categories.Clear();
            Directories.Clear();

            if (config == null) return; 
            foreach (Gameconfig.Category category in config.Categories)
            {
                List<Tuple<string, string>> scenes = new List<Tuple<string, string>>();
                foreach (Gameconfig.SceneInfo scene in category.Scenes)
                {
                    scenes.Add(new Tuple<string, string>(scene.Name, scene.Zone + "\\Scene" + scene.SceneID + ".bin"));

                    List<Tuple<string, Tuple<Gameconfig.SceneInfo, string>>> files;
                    if (!Directories.TryGetValue(scene.Zone, out files))
                    {
                        files = new List<Tuple<string, Tuple<Gameconfig.SceneInfo, string>>>();
                        Directories[scene.Zone] = files;
                    }
                    files.Add(new Tuple<string, Tuple<Gameconfig.SceneInfo, string>>("Scene" + scene.SceneID + ".bin" + (scene.ModeFilter == 5 ? " (Encore)" : ""), new Tuple<Gameconfig.SceneInfo, string>(scene, scene.Zone + "\\Scene" + scene.SceneID + ".bin")));
                }
                Categories.Add(new Tuple<string, List<Tuple<string, string>>>(category.Name, scenes));
            }

            // Sort
            Directories = Directories.OrderBy(key => key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

            this.ScenesTree.ImageList = new ImageList();
            this.ScenesTree.ImageList.Images.Add("Folder", Properties.Resources.folder);
            this.ScenesTree.ImageList.Images.Add("File", Properties.Resources.file);

            UpdateTree();
        }
        private void UpdateTree()
        {
            UpdateScenesTreeFilter(FilterText.Text);
        }
        private void UpdateScenesTreeFilter(string filter)
        {
            ScenesTree.Nodes.Clear();
            if ((bool)isFilesView.IsChecked)
            {
                foreach (KeyValuePair<string, List<Tuple<string, Tuple<Gameconfig.SceneInfo, string>>>> directory in Directories)
                {
                    TreeNode dir_node = new TreeNode(directory.Key);
                    dir_node.ImageKey = "Folder";
                    dir_node.SelectedImageKey = "Folder";
                    dir_node.ContextMenuStrip = contextMenuStrip1;
                    foreach (Tuple<string, Tuple<Gameconfig.SceneInfo, string>> file in directory.Value)
                    {
                        TreeNode file_node = new TreeNode(file.Item1);
                        file_node.Tag = file.Item2;
                        file_node.ImageKey = "File";
                        file_node.ImageKey = "File";
                        file_node.SelectedImageKey = "File";
                        if (filter == "" || (directory.Key + "/" + file.Item1).ToLower().Contains(filter.ToLower()))
                            dir_node.Nodes.Add(file_node);
                    }
                    if (dir_node.Nodes.Count > 0)
                        ScenesTree.Nodes.Add(dir_node);
                }
            }
            else
            {
                foreach (Tuple<string, List<Tuple<string, string>>> category in Categories)
                {
                    TreeNode dir_node = new TreeNode(category.Item1);
                    dir_node.ImageKey = "Folder";
                    dir_node.SelectedImageKey = "Folder";
                    string last = "";
                    foreach (Tuple<string, string> scene in category.Item2)
                    {
                        string scene_name = scene.Item1;
                        if (char.IsDigit(scene.Item1[0]))
                            scene_name = last + scene.Item1;

                        TreeNode file_node = new TreeNode(scene_name + " (" + scene.Item2 + ")");
                        file_node.Tag = scene.Item2;
                        file_node.ImageKey = "File";
                        file_node.SelectedImageKey = "File";
                        if (filter == "" || scene.Item2.ToLower().Contains(filter.ToLower()) || scene_name.ToLower().Contains(filter.ToLower()))
                            dir_node.Nodes.Add(file_node);

                        // Only the first act specify the full name, so lets save it
                        int i = scene_name.Length;
                        while (char.IsDigit(scene_name[i - 1]) || (i >= 2 && char.IsDigit(scene_name[i - 2])))
                            --i;
                        last = scene_name.Substring(0, i);
                    }
                    if (dir_node.Nodes.Count > 0)
                        ScenesTree.Nodes.Add(dir_node);
                }
            }
            browse.IsEnabled = (_GameConfig != null);
            if (filter != "")
            {
                ScenesTree.ExpandAll();
            }
        }
        public void UpdateSelectedSceneInfo()
        {
            if (ScenesTree.SelectedNode != null)
            {
                if (ScenesTree.SelectedNode.Parent != null)
                {
                    if (isFilesView.IsChecked.Value)
                    {
                        Tuple<Gameconfig.SceneInfo, string> tag = ScenesTree.SelectedNode.Tag as Tuple<Gameconfig.SceneInfo, string>;
                        if (tag != null)
                        {
                            Gameconfig.SceneInfo scene = tag.Item1;
                            if (scene != null)
                            {
                                CurrentSceneID = scene.SceneID;
                                CurrentZone = scene.Zone;
                                CurrentName = scene.Name;
                                LevelID = scene.LevelID;
                                isEncore = (scene.ModeFilter == 5 ? true : false);
                                CurrenInfoLabel.Visibility = Visibility.Visible;
                                CurrenInfoLabel.Content = string.Format("Level ID: {0} || Scene ID: {1} || Name: {2} || Zone: {3} || Encore: {4}", LevelID, CurrentSceneID, CurrentName, CurrentZone, (isEncore ? "Yes" : "No"));
                            }
                            else Reset();
                        }
                        else Reset();

                    }
                    else
                    {
                        var cat = _GameConfig.Categories.Where(t => t.Name == ScenesTree.SelectedNode.Parent.Text).FirstOrDefault();
                        if (cat != null)
                        {
                            var scene = cat.Scenes.Where(t => t.Index == ScenesTree.SelectedNode.Index).FirstOrDefault();
                            if (scene != null)
                            {
                                CurrentSceneID = scene.SceneID;
                                CurrentZone = scene.Zone;
                                CurrentName = scene.Name;
                                LevelID = scene.LevelID;
                                isEncore = (scene.ModeFilter == 5 ? true : false);
                                CurrenInfoLabel.Visibility = Visibility.Visible;
                                CurrenInfoLabel.Content = string.Format("Level ID: {0} || Scene ID: {1} || Name: {2} || Zone: {3} || Encore: {4}", LevelID, CurrentSceneID, CurrentName, CurrentZone, (isEncore ? "Yes" : "No"));
                            }
                        }
                        else Reset();
                    }

                }
                else Reset();

            }
            else Reset();

            void Reset()
            {
                CurrentSceneID = "";
                CurrentZone = "";
                CurrentName = "";
                isEncore = false;
                LevelID = -1;
                CurrenInfoLabel.Visibility = Visibility.Hidden;
            }

        }
        private void FilterTextTextChangedEvent(object sender, TextChangedEventArgs e)
        {
            UpdateTree();
        }
        #endregion

        #region Recents Tree Events
        private void RecentsTreeNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                RecentsTree.SelectedNode = e.Node;
                LoadEvent(sender, null);
            }
        }


        #endregion

        #region Options Button Events
        private void RemoveAllDataFoldersEvent(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to do this? No undos here!", "Delete All Data Directories", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				if (Core.Settings.MySettings.DataDirectories != null)
				{
					Core.Settings.MySettings.DataDirectories.Clear();
					
					ReloadRecentsTree();
				}


			}

		}

        private void RemoveAllSavedPlacesEvent(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to do this? No undos here!", "Delete All Saved Places", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				if (Core.Settings.MySettings.SavedPlaces != null)
				{
					Core.Settings.MySettings.SavedPlaces.Clear();
					ReloadRecentsTree();
				}
			}
		}

        #endregion

        #region Mod Pack Editor Events

        private void EditModPacksEvent(object sender, EventArgs e)
		{
            OpenModPackEditorEvent(null, null);
		}

        private void OpenModPackEditorEvent(object sender, RoutedEventArgs e)
        {
            Controls.SceneSelect.DataPackEditor editor = new Controls.SceneSelect.DataPackEditor(EditorInstance);
            if (Window != null) editor.Owner = Window.Owner;
            else editor.Owner = Application.Current.MainWindow;
            editor.ShowDialog();
            ReloadRecentsTree();
        }

        #endregion

        #region Window Events
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var host = new System.Windows.Forms.Integration.WindowsFormsHost();
            host.Child = this.ScenesTree;
            this.scenesTreeHost.Children.Add(host);
            var host2 = new System.Windows.Forms.Integration.WindowsFormsHost();
            host2.Child = this.RecentsTree;
            this.recentDataDirListHost.Children.Add(host2);
        }
        private void SceneSelectGotFocusEvent(object sender, RoutedEventArgs e)
        {
            ReloadRecentsTree();
        }
        private void Close()
        {
            if (!Classes.Editor.SolutionState.isImportingObjects)
            {
                ManiacEditor.Classes.Editor.SolutionLoader.OpenSceneUsingExistingSceneSelect(this);
            }
            if (Window != null) Window.Close();

        }
        #endregion

        #region General Button Events
        private void CancelButtonEvent(object sender, RoutedEventArgs e)
		{
			if (Window != null) Window.DialogResult = false;
		}
        private void SelectButtonEvent(object sender, RoutedEventArgs e)
        {
            Classes.Editor.SolutionState.LevelID = LevelID;
            if (!isFilesView.IsChecked.Value)
            {
                SelectedSceneResult = ScenesTree.SelectedNode.Tag as string;
            }
            else
            {
                Tuple<Gameconfig.SceneInfo, string> tag = ScenesTree.SelectedNode.Tag as Tuple<Gameconfig.SceneInfo, string>;
                if (tag != null)
                {
                    SelectedSceneResult = tag.Item2;
                }
                else return;
            }

            Close();

        }

        #region Add Button Events
        private void AddDataDirectoryEvent(object sender, RoutedEventArgs e)
        {
            if (Classes.Editor.SolutionState.isImportingObjects == false)
            {
                string newDataDirectory = EditorInstance.GetDataDirectory();
                string returnDataDirectory;

                if (string.IsNullOrWhiteSpace(newDataDirectory)) return;
                if (EditorInstance.IsDataDirectoryValid(newDataDirectory))
                {
                    EditorInstance.DataDirectory = newDataDirectory;
                    returnDataDirectory = newDataDirectory;
                    bool goodDataDir = EditorInstance.SetGameConfig();
                    if (goodDataDir == true)
                    {
                        EditorInstance.AddRecentDataFolder(EditorInstance.DataDirectory);
                        ReloadRecentsTree();
                    }

                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("You can't do that while importing objects!");
            }


        }
        public void AddSavedPlaceEvent(object sender, EventArgs e)
        {
            String newSavedPlace;
            using (var folderBrowserDialog = new GenerationsLib.Core.FolderSelectDialog())
            {
                folderBrowserDialog.Title = "Select A Folder";

                if (!folderBrowserDialog.ShowDialog())
                {
                    newSavedPlace = null;
                }
                else
                {
                    newSavedPlace = folderBrowserDialog.FileName.ToString();
                }

            }
            MessageBox.Show(newSavedPlace);
            AddANewSavedPlace(newSavedPlace);


        }
        public void AddANewSavedPlace(string savedFolder)
        {
            try
            {
                var mySettings = Core.Settings.MySettings;
                var savedPlaces = mySettings.SavedPlaces;

                if (savedPlaces == null)
                {
                    savedPlaces = new StringCollection();
                    mySettings.SavedPlaces = savedPlaces;
                }

                if (savedPlaces.Contains(savedFolder))
                {
                    savedPlaces.Remove(savedFolder);
                }

                savedPlaces.Insert(0, savedFolder);

                Core.Options.GeneralSettings.Save();

                ReloadRecentsTree();
            }
            catch (Exception ex)
            {
                Debug.Print("Failed to add Saved Place to list: " + ex);
            }
        }

        #endregion
        #endregion
    }
}
