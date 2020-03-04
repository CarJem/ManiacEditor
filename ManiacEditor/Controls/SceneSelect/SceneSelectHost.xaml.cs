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
        #region Definitions

        #region Collections
        public List<Tuple<string, List<Tuple<string, string>>>> Categories { get; set; } = new List<Tuple<string, List<Tuple<string, string>>>>();
        public Dictionary<string, List<Tuple<string, Tuple<Gameconfig.SceneInfo, string>>>> Directories { get; set; } = new Dictionary<string, List<Tuple<string, Tuple<Gameconfig.SceneInfo, string>>>>();
        #endregion

        #region Structures
        public bool isFileViewMode { get => isFilesView.IsChecked.Value; }
        public string PreviousDataFolder { get; set; }
        public bool WithinAParentForm { get; set; } = false;
        public int SelectedCategoryIndex
        {
            get
            {
                if (ScenesTree.SelectedNode != null)
                {
                    return ScenesTree.SelectedNode.Index;
                }
                else
                {
                    return -1;
                }
            }
        }
        public bool IsFunctionalGameConfig { get; set; } = true;
        public string DataDirectory 
        { 
            get
            {
                if (SceneState.ExtraDataDirectories != null && SceneState.ExtraDataDirectories.Count >= 1) return SceneState.ExtraDataDirectories[0];
                else return string.Empty;
            } 
            set
            {
                if (SceneState.ExtraDataDirectories == null) SceneState.ExtraDataDirectories = new List<string>();
                else SceneState.ExtraDataDirectories.Clear();
                SceneState.ExtraDataDirectories.Add(value);
            }
        }

        public string GetComboBoxItemString(object item)
        {
            return (item as ComboBoxItem).Content.ToString();
        }
        #endregion

        #region Classes
        public Gameconfig _GameConfig { get; set; }
        public SceneSelectWindow Window { get; set; }
        private Controls.Editor.MainEditor Instance { get; set; }
        public Classes.General.SceneState SceneState { get; set; } = new Classes.General.SceneState();
        #endregion

        #region Legacy WinForms Stuff
        private System.ComponentModel.IContainer Components = null;
        private System.Windows.Forms.ContextMenuStrip SceneInfoContextMenu;
        private System.Windows.Forms.ContextMenuStrip FolderEditContext;

        private System.Windows.Forms.ContextMenuStrip RecentDataDirEditContext;
        private System.Windows.Forms.ContextMenuStrip SavedDataDirEditContext;

        private System.Windows.Forms.ContextMenuStrip ScenesTreeCategoryContextMenu;

        private System.Windows.Forms.ToolStripSeparator Seperator1;
        private System.Windows.Forms.ToolStripSeparator Seperator2;
        private System.Windows.Forms.ToolStripSeparator Seperator3;

        private System.Windows.Forms.ToolStripMenuItem RemoveSavedFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveRecentDataDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveSavedDataDirectoryToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem MoveCategoryUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveCategoryDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveSceneInfoUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveSceneInfoDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditSceneInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeleteSceneInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeleteSelectedCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditSelectedCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddSceneToolStripMenuItem;



        private System.Windows.Forms.TreeView ScenesTree { get; set; }
        private System.Windows.Forms.TreeView RecentsTree { get; set; }
        #endregion

        #endregion

        #region Init
        public SceneSelectHost(Gameconfig _Config = null, Controls.Editor.MainEditor _Instance = null, SceneSelectWindow _Window = null)
        {
            InitializeComponent();
            Instance = _Instance;
            InitilizeBase();
            InitilizeHost(_Config, _Window);
        }
        public void SetupWinFormControls()
        {
            this.Components = new System.ComponentModel.Container();
            this.ScenesTree = new System.Windows.Forms.TreeView();
            this.RecentsTree = new System.Windows.Forms.TreeView();

            this.ScenesTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ScenesTree.BackColor = System.Drawing.Color.White;
            this.ScenesTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ScenesTree.ForeColor = System.Drawing.Color.Black;
            this.ScenesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScenesTree.TabIndex = 0;
            this.ScenesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ScenesTreeAfterSelectEvent);
            this.ScenesTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ScenesTreeNodeMouseDoubleClickEvent);
            this.ScenesTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ScenesTreeNodeDoubleClickEvent);
            this.ScenesTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScenesTreeNodeMouseUpEvent);
            this.ScenesTree.HideSelection = false;

            this.RecentsTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RecentsTree.BackColor = System.Drawing.Color.White;
            this.RecentsTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RecentsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RecentsTree.TabIndex = 13;
            this.RecentsTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.RecentsTreeMouseDownEvent);
            this.RecentsTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.RecentsTreeNodeDoubleClickEvent);
            this.RecentsTree.HideSelection = false;

            ScenesTree.Show();
            RecentsTree.Show();
        }
        public void SetupWinFormContextMenuItems()
        {
            this.Seperator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Seperator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Seperator3 = new System.Windows.Forms.ToolStripSeparator();

            this.AddSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditSelectedCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteSelectedCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveCategoryUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveCategoryDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteSceneInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditSceneInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveSceneInfoUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveSceneInfoDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveSavedFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveRecentDataDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveSavedDataDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.Seperator1.Size = new System.Drawing.Size(226, 6);
            this.Seperator2.Size = new System.Drawing.Size(226, 6);
            this.Seperator3.Size = new System.Drawing.Size(193, 6);

            this.AddSceneToolStripMenuItem.Text = "Add Scene to Category";
            this.DeleteSceneInfoToolStripMenuItem.Text = "Delete Scene Info";
            this.MoveSceneInfoUpToolStripMenuItem.Text = "Move Scene Info Up";
            this.MoveSceneInfoDownToolStripMenuItem.Text = "Move Scene Info Down";
            this.AddCategoryToolStripMenuItem.Text = "Add Category to Gameconfig";
            this.EditSelectedCategoryToolStripMenuItem.Text = "Edit Selected Category";
            this.DeleteSelectedCategoryToolStripMenuItem.Text = "Delete Selected Category";
            this.MoveCategoryUpToolStripMenuItem.Text = "Move Category Up";
            this.MoveCategoryDownToolStripMenuItem.Text = "Move Category Down";
            this.EditSceneInfoToolStripMenuItem.Text = "Edit Scene Info";

            this.RemoveRecentDataDirectoryToolStripMenuItem.Text = "Remove Recent Data Folder";
            this.RemoveSavedDataDirectoryToolStripMenuItem.Text = "Remove Saved Data Folder";

            this.AddSceneToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.DeleteSceneInfoToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.MoveSceneInfoUpToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.MoveSceneInfoDownToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.AddCategoryToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.EditSelectedCategoryToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.MoveCategoryDownToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.EditSceneInfoToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.MoveCategoryUpToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.DeleteSelectedCategoryToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.RemoveSavedFolderToolStripMenuItem.Size = new System.Drawing.Size(187, 22);

            this.AddSceneToolStripMenuItem.Click += new System.EventHandler(this.GameConfigAddSceneEvent);
            this.DeleteSceneInfoToolStripMenuItem.Click += new System.EventHandler(this.GameConfigDeleteSceneEvent);
            this.MoveSceneInfoUpToolStripMenuItem.Click += new System.EventHandler(this.GameConfigMoveSceneUpEvent);
            this.MoveSceneInfoDownToolStripMenuItem.Click += new System.EventHandler(this.GameConfigMoveSceneDownEvent);
            this.AddCategoryToolStripMenuItem.Click += new System.EventHandler(this.GameConfigAddCategoryEvent);
            this.EditSelectedCategoryToolStripMenuItem.Click += new System.EventHandler(this.GameConfigEditCategoryEvent);
            this.DeleteSelectedCategoryToolStripMenuItem.Click += new System.EventHandler(this.GameConfigDeleteCategoryEvent);
            this.MoveCategoryUpToolStripMenuItem.Click += new System.EventHandler(this.GameConfigMoveCategoryUpEvent);
            this.MoveCategoryDownToolStripMenuItem.Click += new System.EventHandler(this.GameConfigMoveCategoryDownEvent);
            this.EditSceneInfoToolStripMenuItem.Click += new System.EventHandler(this.GameConfigEditSceneEvent);
            this.RemoveSavedFolderToolStripMenuItem.Click += new System.EventHandler(this.RemoveSelectedSavedPlaceEvent);
            this.RemoveRecentDataDirectoryToolStripMenuItem.Click += RemoveRecentDataDirectoryToolStripMenuItem_Click;
            this.RemoveSavedDataDirectoryToolStripMenuItem.Click += RemoveSavedDataDirectoryToolStripMenuItem_Click;
        }
        public void SetupWinFormContextMenu()
        {
            this.FolderEditContext = new System.Windows.Forms.ContextMenuStrip(this.Components);
            this.RecentDataDirEditContext = new System.Windows.Forms.ContextMenuStrip(this.Components);
            this.SavedDataDirEditContext = new System.Windows.Forms.ContextMenuStrip(this.Components);

            this.SceneInfoContextMenu = new System.Windows.Forms.ContextMenuStrip(this.Components);
            this.ScenesTreeCategoryContextMenu = new System.Windows.Forms.ContextMenuStrip(this.Components);

            this.SceneInfoContextMenu.Items.Add(this.AddSceneToolStripMenuItem);
            this.SceneInfoContextMenu.Items.Add(this.Seperator1);
            this.SceneInfoContextMenu.Items.Add(this.AddCategoryToolStripMenuItem);
            this.SceneInfoContextMenu.Items.Add(this.EditSelectedCategoryToolStripMenuItem);
            this.SceneInfoContextMenu.Items.Add(this.DeleteSelectedCategoryToolStripMenuItem);
            this.SceneInfoContextMenu.Items.Add(this.Seperator2);
            this.SceneInfoContextMenu.Items.Add(this.MoveCategoryUpToolStripMenuItem);
            this.SceneInfoContextMenu.Items.Add(this.MoveCategoryDownToolStripMenuItem);
            this.SceneInfoContextMenu.Size = new System.Drawing.Size(230, 148);
            this.SceneInfoContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ScenesTreeSceneContextMenuOpeningEvent);

            this.ScenesTreeCategoryContextMenu.Items.Add(this.DeleteSceneInfoToolStripMenuItem);
            this.ScenesTreeCategoryContextMenu.Items.Add(this.EditSceneInfoToolStripMenuItem);
            this.ScenesTreeCategoryContextMenu.Items.Add(this.Seperator3);
            this.ScenesTreeCategoryContextMenu.Items.Add(this.MoveSceneInfoUpToolStripMenuItem);
            this.ScenesTreeCategoryContextMenu.Items.Add(this.MoveSceneInfoDownToolStripMenuItem);
            this.ScenesTreeCategoryContextMenu.Size = new System.Drawing.Size(197, 98);
            this.ScenesTreeCategoryContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ScenesTreeCategoryContextMenuOpeningEvent);

            this.RecentDataDirEditContext.Items.Add(this.RemoveRecentDataDirectoryToolStripMenuItem);
            this.RecentDataDirEditContext.Size = new System.Drawing.Size(196, 26);
            this.RecentDataDirEditContext.Opening += RecentDataDirEditContext_Opening;

            this.SavedDataDirEditContext.Items.Add(this.RemoveSavedDataDirectoryToolStripMenuItem);
            this.SavedDataDirEditContext.Size = new System.Drawing.Size(196, 26);
            this.SavedDataDirEditContext.Opening += SavedDataDirEditContext_Opening;

            this.FolderEditContext.Items.Add(this.RemoveSavedFolderToolStripMenuItem);
            this.FolderEditContext.Size = new System.Drawing.Size(188, 26);

        }

        private void InitilizeBase()
        {
            RemoveNullEntries();
            SetupWinFormControls();
            SetupWinFormContextMenuItems();
            SetupWinFormContextMenu();
        }

        private void InitilizeHost(Gameconfig _Config, SceneSelectWindow _Window = null)
        {
            WithinAParentForm = (Window != null);
            Window = _Window;

            UpdateRecentsTree();
            RefreshTheme();
            ScenesTree.Visible = true;

            UpdateMasterDataDirectoryComboBox();
            SetupGameConfig(_Config);
            InitilizeEvents();

            if (Properties.Settings.MySettings.SavedDataDirectories.Contains(Properties.Settings.MySettings.DefaultMasterDataDirectory))
            {
                MasterDataDirectorySelectorComboBox.SelectedItem = Properties.Settings.MySettings.DefaultMasterDataDirectory;
            }

        }

        private void InitilizeEvents()
        {
            MasterDataDirectorySelectorComboBox.SelectionChanged += MasterDataDirectorySelectorComboBox_SelectionChanged;
        }

        #endregion

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
            UpdateScenesTree();
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
                    SceneInfoContextMenu.Show(ScenesTree, e.Location);
                else if (e.Node.ImageKey == "File")
                    ScenesTreeCategoryContextMenu.Show(ScenesTree, e.Location);
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

        #region GameConfig Editing Events
        public void GameConfigThrowError()
        {
            System.Windows.MessageBox.Show("RSDK-Reverse's GameConfig Library for RSDKv5 currently is Broken, and to prvent unwanted corruption to you gameconfigs, I have disabled this feature for this build.", "Feature Disabled");
        }
        private void GameConfigAddSceneEvent(object sender, EventArgs e)
        {
            if (!IsFunctionalGameConfig)
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
                form.Owner = System.Windows.Window.GetWindow(Instance.StartScreen);
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
            if (!IsFunctionalGameConfig)
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
                form.Owner = System.Windows.Window.GetWindow(Instance.StartScreen);
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
            if (!IsFunctionalGameConfig)
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
                    form.Owner = System.Windows.Window.GetWindow(Instance.StartScreen);
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
            if (!IsFunctionalGameConfig)
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
                form.Owner = System.Windows.Window.GetWindow(Instance.StartScreen);
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
            if (!IsFunctionalGameConfig)
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
            if (!IsFunctionalGameConfig)
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
            if (!IsFunctionalGameConfig)
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
            if (!IsFunctionalGameConfig)
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
            if (!IsFunctionalGameConfig)
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
            if (!IsFunctionalGameConfig)
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

        #region UI Related Methods
        public void RefreshTheme()
        {
            if (App.Skin == Enums.Skin.Dark)
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
        public void RecentsTreeOpenContextMenu(TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RecentsTree.SelectedNode = e.Node;
                if (e.Node.ImageKey == "SavedDataFolder")
                    SavedDataDirEditContext.Show(RecentsTree, e.Location);
                else if (e.Node.ImageKey == "RecentDataFolder")
                    RecentDataDirEditContext.Show(RecentsTree, e.Location);
                else if (e.Node.ImageKey == "SavedPlace")
                    FolderEditContext.Show(RecentsTree, e.Location);
            }
        }
        public void UpdateSceneCategoryContextMenu()
        {
            if (isFileViewMode)
            {
                ScenesTreeCategoryContextMenu.Enabled = false;
            }
            else
            {
                ScenesTreeCategoryContextMenu.Enabled = true;
                MoveSceneInfoDownToolStripMenuItem.Enabled = false;
                MoveSceneInfoUpToolStripMenuItem.Enabled = false;
            }
        }
        public void UpdateSceneInfoContextMenu()
        {
            if (isFileViewMode)
            {
                SceneInfoContextMenu.Enabled = false;
            }
            else
            {
                SceneInfoContextMenu.Enabled = true;
                if (SelectedCategoryIndex != -1)
                {
                    if (ScenesTree.SelectedNode.Index == 0)
                    {
                        MoveCategoryUpToolStripMenuItem.Enabled = false;
                    }
                    else
                    {
                        MoveCategoryUpToolStripMenuItem.Enabled = true;
                    }

                    if (ScenesTree.SelectedNode.Index == ScenesTree.Nodes.Count - 1)
                    {
                        MoveCategoryDownToolStripMenuItem.Enabled = false;
                    }
                    else
                    {
                        MoveCategoryDownToolStripMenuItem.Enabled = true;
                    }
                }
                else
                {
                    MoveCategoryUpToolStripMenuItem.Enabled = false;
                    MoveCategoryDownToolStripMenuItem.Enabled = false;
                }
            }

        }
        public void UpdateRecentDataDirInfoContextMenu()
        {

        }
        public void UpdateSavedDataDirInfoContextMenu()
        {

        }
        public void UpdateDataDirectoryLabel()
        {
            if (dataLabelToolStripItem != null)
            {
                if (DataDirectory != null && DataDirectory != string.Empty) dataLabelToolStripItem.Content = "Data Directory: " + DataDirectory;
                else dataLabelToolStripItem.Content = "Data Directory: N/A";
            }
        }

        private bool AllowMasterDataDirectoryUpdate = true;
        public void UpdateMasterDataDirectoryComboBox()
        {
            if (MasterDataDirectorySelectorComboBox != null && AllowMasterDataDirectoryUpdate)
            {
                AllowMasterDataDirectoryUpdate = false;
                var lastItem = MasterDataDirectorySelectorComboBox.SelectedItem;
                MasterDataDirectorySelectorComboBox.SelectionChanged -= MasterDataDirectorySelectorComboBox_SelectionChanged;

                MasterDataDirectorySelectorComboBox.Items.Clear();
                if (Properties.Settings.MySettings.SavedDataDirectories == null) Properties.Settings.MySettings.SavedDataDirectories = new StringCollection();
                foreach (string savedDirectories in Properties.Settings.MySettings.SavedDataDirectories)
                {
                    MasterDataDirectorySelectorComboBox.Items.Add(savedDirectories);
                }
                
                MasterDataDirectorySelectorComboBox.SelectedItem = lastItem;

                MasterDataDirectorySelectorComboBox.SelectionChanged += MasterDataDirectorySelectorComboBox_SelectionChanged;
                AllowMasterDataDirectoryUpdate = true;
            }
        }
        public void UpdateRecentsTree()
        {
            UpdateDataDirectoryLabel();
            RecentsTree.Nodes.Clear();
            RecentsTree.Nodes.Add("Saved Data Directories");
            RecentsTree.Nodes.Add("Recent Data Directories");
            RecentsTree.Nodes.Add("Saved Places");
            RecentsTree.ImageList = new ImageList();
            RecentsTree.ImageList.Images.Add("Folder", Properties.Resources.folder);
            RecentsTree.ImageList.Images.Add("File", Properties.Resources.file);
            RecentsTree.ImageList.Images.Add("SubFolder", Properties.Resources.folder);

            if (Properties.Settings.MySettings.SavedDataDirectories != null && Properties.Settings.MySettings.SavedDataDirectories?.Count > 0)
            {
                foreach (string dataDir in Properties.Settings.MySettings.SavedDataDirectories)
                {
                    var node = RecentsTree.Nodes[0].Nodes.Add(dataDir);
                    node.Tag = dataDir;
                    node.ToolTipText = dataDir;
                    node.ImageKey = "SavedDataFolder";
                }
                RecentsTree.Nodes[0].ExpandAll();
            }

            if (Properties.Settings.MySettings.RecentDataDirectories != null && Properties.Settings.MySettings.RecentDataDirectories?.Count > 0)
            {
                foreach (string dataDir in Properties.Settings.MySettings.RecentDataDirectories)
                {
                    var node = RecentsTree.Nodes[1].Nodes.Add(dataDir);
                    node.Tag = dataDir;
                    node.ToolTipText = dataDir;
                    node.ImageKey = "RecentDataFolder";
                }
                RecentsTree.Nodes[1].ExpandAll();
            }

            if (Properties.Settings.MySettings.SavedPlaces != null && Properties.Settings.MySettings.SavedPlaces?.Count > 0)
            {
                foreach (string folder in Properties.Settings.MySettings.SavedPlaces)
                {
                    var node = RecentsTree.Nodes[1].Nodes.Add(folder, folder, this.RecentsTree.ImageList.Images.IndexOfKey("SubFolder"), this.RecentsTree.ImageList.Images.IndexOfKey("SubFolder"));
                    node.Tag = folder;
                    node.ToolTipText = folder;
                    node.ImageKey = "SavedPlace";
                }
                RecentsTree.Nodes[1].ExpandAll();
            }
            UpdateMasterDataDirectoryComboBox();
        }
        public void UpdateScenesTree(Gameconfig config)
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

            UpdateScenesTree();
        }
        private void UpdateScenesTree(string filter = "")
        {
            if (filter == "") filter = FilterText.Text;
            ScenesTree.Nodes.Clear();
            if ((bool)isFilesView.IsChecked)
            {
                foreach (KeyValuePair<string, List<Tuple<string, Tuple<Gameconfig.SceneInfo, string>>>> directory in Directories)
                {
                    TreeNode dir_node = new TreeNode(directory.Key);
                    dir_node.ImageKey = "Folder";
                    dir_node.SelectedImageKey = "Folder";
                    dir_node.ContextMenuStrip = SceneInfoContextMenu;
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
                                SceneState.SceneID = scene.SceneID;
                                SceneState.Zone = scene.Zone;
                                SceneState.Name = scene.Name;
                                SceneState.LevelID = scene.LevelID;
                                SceneState.IsEncoreMode = (scene.ModeFilter == 5 ? true : false);
                                CurrenInfoLabel.Visibility = Visibility.Visible;
                                CurrenInfoLabel.Content = string.Format("Level ID: {0} || Scene ID: {1} || Name: {2} || Zone: {3} || Encore: {4}", SceneState.LevelID, SceneState.SceneID, SceneState.Name, SceneState.Zone, (SceneState.IsEncoreMode ? "Yes" : "No"));
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
                                SceneState.SceneID = scene.SceneID;
                                SceneState.Zone = scene.Zone;
                                SceneState.Name = scene.Name;
                                SceneState.LevelID = scene.LevelID;
                                SceneState.IsEncoreMode = (scene.ModeFilter == 5 ? true : false);
                                CurrenInfoLabel.Visibility = Visibility.Visible;
                                CurrenInfoLabel.Content = string.Format("Level ID: {0} || Scene ID: {1} || Name: {2} || Zone: {3} || Encore: {4}", SceneState.LevelID, SceneState.SceneID, SceneState.Name, SceneState.Zone, (SceneState.IsEncoreMode ? "Yes" : "No"));
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
                SceneState.SceneID = "";
                SceneState.Zone = "";
                SceneState.Name = "";
                SceneState.IsEncoreMode = false;
                SceneState.LevelID = -1;
                CurrenInfoLabel.Visibility = Visibility.Hidden;
            }

        }
        #endregion

        #region General Events
        private void RemoveSavedDataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedSavedDataFolder();
        }
        private void RemoveRecentDataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedRecentDataFolder();
        }
        private void MasterDataDirectorySelectorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllowMasterDataDirectoryUpdate && MasterDataDirectorySelectorComboBox.SelectedIndex != -1) LoadMasterDataDirectory(MasterDataDirectorySelectorComboBox.SelectedItem.ToString());
        }
        private void RecentsTreeMouseDownEvent(object sender, TreeNodeMouseClickEventArgs e)
        {
            RecentsTreeOpenContextMenu(e);
        }
        private void BrowseButtonEvent(object sender, RoutedEventArgs e)
        {
            BrowseForSceneFile();
        }
        private void RemoveSelectedSavedPlaceEvent(object sender, EventArgs e)
        {
            RemoveSelectedSavedPlace();
        }
        private void RecentDataDirEditContext_Opening(object sender, CancelEventArgs e)
        {
            UpdateRecentDataDirInfoContextMenu();
        }

        private void SavedDataDirEditContext_Opening(object sender, CancelEventArgs e)
        {
            UpdateSavedDataDirInfoContextMenu();
        }
        private void ScenesTreeSceneContextMenuOpeningEvent(object sender, CancelEventArgs e)
        {
            UpdateSceneInfoContextMenu();
        }
        private void ScenesTreeCategoryContextMenuOpeningEvent(object sender, CancelEventArgs e)
        {
            UpdateSceneCategoryContextMenu();
        }
        private void LoadEvent(object sender, RoutedEventArgs e)
        {
            UniversalLoadMethod();
        }
        private void OnLoadedEvent(object sender, RoutedEventArgs e)
        {
            var host = new System.Windows.Forms.Integration.WindowsFormsHost();
            host.Child = this.ScenesTree;
            this.scenesTreeHost.Children.Add(host);
            var host2 = new System.Windows.Forms.Integration.WindowsFormsHost();
            host2.Child = this.RecentsTree;
            this.recentDataDirListHost.Children.Add(host2);
        }
        private void RefreshButtonEvent(object sender, RoutedEventArgs e)
        {
            UpdateRecentsTree();
        }
        private void CloseHostEvent()
        {
            if (!Methods.Editor.SolutionState.isImportingObjects)
            {
                ManiacEditor.Methods.Editor.SolutionLoader.OpenSceneUsingSceneSelect(this);
            }
            if (Window != null) Window.Close();

        }
        private void CancelButtonEvent(object sender, RoutedEventArgs e)
        {
            if (Window != null) Window.DialogResult = false;
        }
        private void SelectButtonEvent(object sender, RoutedEventArgs e)
        {
            Methods.Editor.SolutionState.LevelID = SceneState.LevelID;
            if (!isFilesView.IsChecked.Value)
            {
                SceneState.FilePath = ScenesTree.SelectedNode.Tag as string;
            }
            else
            {
                Tuple<Gameconfig.SceneInfo, string> tag = ScenesTree.SelectedNode.Tag as Tuple<Gameconfig.SceneInfo, string>;
                if (tag != null)
                {
                    SceneState.FilePath = tag.Item2;
                }
                else return;
            }

            CloseHostEvent();
        }
        private void RecentsTreeNodeDoubleClickEvent(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                RecentsTree.SelectedNode = e.Node;
                UniversalLoadMethod();
            }
        }
        private void AddDataDirectoryEvent(object sender, RoutedEventArgs e)
        {
            AddANewDataDirectory();
        }
        public void AddSavedPlaceEvent(object sender, EventArgs e)
        {
            AddANewSavedPlaceDialog();
        }
        private void RemoveAllDataFoldersEvent(object sender, RoutedEventArgs e)
        {
            RemoveAllDataFolders();
        }
        private void RemoveAllSavedPlacesEvent(object sender, RoutedEventArgs e)
        {
            RemoveAllSavedPlaces();
        }
        private void FilterTextTextChangedEvent(object sender, TextChangedEventArgs e)
        {
            UpdateScenesTree();
        }
        #endregion

        #region Data Managemenet
        public void RemoveNullEntries()
        {
            if (Properties.Settings.MySettings.RecentDataDirectories != null) Properties.Settings.MySettings.RecentDataDirectories.Remove(null);
            if (Properties.Settings.MySettings.SavedPlaces != null) Properties.Settings.MySettings.SavedPlaces.Remove(null);
        }
        public void RemoveSelectedSavedPlace()
        {
            String toRemove = RecentsTree.SelectedNode.Tag.ToString();
            if (Properties.Settings.MySettings.SavedPlaces.Contains(toRemove))
            {
                Properties.Settings.MySettings.SavedPlaces.Remove(toRemove);
            }
            UpdateRecentsTree();
        }
        public void RemoveSelectedRecentDataFolder()
        {
            String toRemove = RecentsTree.SelectedNode.Tag.ToString();
            if (Properties.Settings.MySettings.RecentDataDirectories.Contains(toRemove))
            {
                Properties.Settings.MySettings.RecentDataDirectories.Remove(toRemove);
            }
            UpdateRecentsTree();
        }
        public void RemoveSelectedSavedDataFolder()
        {
            String toRemove = RecentsTree.SelectedNode.Tag.ToString();
            if (Properties.Settings.MySettings.SavedDataDirectories.Contains(toRemove))
            {
                Properties.Settings.MySettings.SavedDataDirectories.Remove(toRemove);
            }
            UpdateRecentsTree();
        }
        public void UniversalLoadMethod()
        {
            if (Methods.Editor.SolutionState.isImportingObjects == true)
            {
                MessageBox.Show("You can't do that while importing objects!");
            }
            else
            {
                if (RecentsTree.SelectedNode.ImageKey == "SavedDataFolder")
                {
                    LoadDataDirectory(RecentsTree.SelectedNode.Tag.ToString());
                }
                else if (RecentsTree.SelectedNode.ImageKey == "RecentDataFolder")
                {
                    LoadDataDirectory(RecentsTree.SelectedNode.Tag.ToString());
                }
                else if (RecentsTree.SelectedNode.ImageKey == "SavedPlace")
                {
                    if (_GameConfig != null) BrowseForSceneFileFromSavedPlace(RecentsTree.SelectedNode.Tag.ToString());
                    else MessageBox.Show("Please Select/Open a Data Directory First", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void RemoveAllSavedPlaces()
        {
            if (MessageBox.Show("Are you sure you want to do this? No undos here!", "Delete All Saved Places", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (Properties.Settings.MySettings.SavedPlaces != null)
                {
                    Properties.Settings.MySettings.SavedPlaces.Clear();
                    UpdateRecentsTree();
                }
            }
        }
        public void RemoveAllDataFolders()
        {
            if (MessageBox.Show("Are you sure you want to do this? No undos here!", "Delete All Data Directories", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (Properties.Settings.MySettings.RecentDataDirectories != null)
                {
                    Properties.Settings.MySettings.RecentDataDirectories.Clear();

                    UpdateRecentsTree();
                }
            }
        }
        public void AddANewDataDirectory()
        {
            if (Methods.Editor.SolutionState.isImportingObjects == false)
            {
                string newDataDirectory = ManiacEditor.Methods.Editor.SolutionPaths.SelectDataDirectory();
                string returnDataDirectory;

                if (string.IsNullOrWhiteSpace(newDataDirectory)) return;
                if (ManiacEditor.Methods.Editor.SolutionPaths.DoesDataDirHaveGameConfig(newDataDirectory))
                {
                    DataDirectory = newDataDirectory;
                    returnDataDirectory = newDataDirectory;
                    bool goodDataDir = ManiacEditor.Methods.Editor.SolutionPaths.SetGameConfig(returnDataDirectory);
                    if (goodDataDir == true)
                    {
                        Classes.Prefrences.DataDirectoriesStorage.AddRecentDataFolder(DataDirectory);
                        Classes.Prefrences.DataDirectoriesStorage.AddSavedDataFolder(DataDirectory);
                        if (Instance != null) Instance.UpdateDataFolderLabel(DataDirectory);
                        UpdateRecentsTree();
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
        public void AddANewSavedPlaceDialog()
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
                var mySettings = Properties.Settings.MySettings;
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

                Classes.Options.GeneralSettings.Save();

                UpdateRecentsTree();
            }
            catch (Exception ex)
            {
                Debug.Print("Failed to add Saved Place to list: " + ex);
            }
        }
        public void SetupGameConfig(Gameconfig config)
        {
            if (config != null)
            {
                if (browse != null) browse.IsEnabled = true;
                LoadFromGameConfig(config);
                _GameConfig = config;
            }
            else
            {
                if (browse != null) browse.IsEnabled = false;
            }
        }
        public void BrowseForSceneFileFromSavedPlace(string initialDir)
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.InitialDirectory = initialDir;
            open.Filter = "Scene File|*.bin";
            if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                SceneState.FilePath = open.FileName;
                SceneState.SceneDirectory = System.IO.Path.GetDirectoryName(open.FileName);
                SceneState.LoadType = Classes.General.SceneState.LoadMethod.FullPath;
                CloseHostEvent();
            }
        }
        public void BrowseForSceneFile()
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.Filter = "Scene File|*.bin";
            if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                SceneState.FilePath = open.FileName;
                SceneState.SceneDirectory = System.IO.Path.GetDirectoryName(open.FileName);
                SceneState.LoadType = Classes.General.SceneState.LoadMethod.FullPath;
                CloseHostEvent();
            }
        }
        public void LoadMasterDataDirectory(string SelectedDataDirectory)
        {
            SceneState.MasterDataDirectory = SelectedDataDirectory;
            Gameconfig GameConfig = ManiacEditor.Methods.Editor.SolutionPaths.GetGameConfig(SelectedDataDirectory);
            if (GameConfig != null)
            {
                _GameConfig = GameConfig;
                LoadFromGameConfig(_GameConfig);
                Classes.Prefrences.DataDirectoriesStorage.AddRecentDataFolder(DataDirectory);
                Properties.Settings.MySettings.DefaultMasterDataDirectory = SceneState.MasterDataDirectory;
            }
            else
            {
                UnloadMasterDataDirectory();
            }

            UpdateDataDirectoryLabel();
            UpdateMasterDataDirectoryComboBox();
        }
        public void LoadDataDirectory(string SelectedDataDirectory)
        {
            DataDirectory = SelectedDataDirectory;
            Gameconfig GameConfig = ManiacEditor.Methods.Editor.SolutionPaths.GetGameConfig(SelectedDataDirectory);
            if (GameConfig != null)
            {
                _GameConfig = GameConfig;
                LoadFromGameConfig(_GameConfig);
                Classes.Prefrences.DataDirectoriesStorage.AddRecentDataFolder(DataDirectory);
            }
            else
            {
                UnloadDataDirectory();
            }

            UpdateDataDirectoryLabel();
            UpdateMasterDataDirectoryComboBox();

        }
        public void UnloadDataDirectory()
        {
            _GameConfig = null;
            DataDirectory = string.Empty;
            UpdateScenesTree(_GameConfig);
            UpdateScenesTree();
            UpdateDataDirectoryLabel();
            UpdateMasterDataDirectoryComboBox();
        }
        public void UnloadMasterDataDirectory()
        {
            _GameConfig = null;
            SceneState.MasterDataDirectory = string.Empty;
            UpdateScenesTree(_GameConfig);
            UpdateScenesTree();
            UpdateDataDirectoryLabel();
            UpdateMasterDataDirectoryComboBox();
        }
        public void LoadSaveState(ManiacEditor.Classes.Prefrences.DataStateHistoryStorage.DataStateHistoryCollection.SaveState SaveState)
        {
            bool AllowedToProceed = true;
            LoadMasterDataDirectory(SaveState.MasterDataDirectory);
            LoadDataDirectory((SaveState.ExtraDataDirectories != null && SaveState.ExtraDataDirectories.Count >= 1 ? SaveState.ExtraDataDirectories[0] : string.Empty));

            UpdateDataDirectoryLabel();
            UpdateMasterDataDirectoryComboBox();
        }
        public void LoadFromGameConfig(Gameconfig config)
        {
            UpdateScenesTree(config);
            if (Properties.Settings.MyDefaults.SceneSelectFilesViewDefault) this.isFilesView.IsChecked = true;
            else this.isFilesView.IsChecked = false;
        }
        #endregion
    }
}
