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
using ManiacEditor.Classes.General;
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
        public SceneSelectCategoriesCollection Categories { get; set; } = new SceneSelectCategoriesCollection();
        public Dictionary<string, List<SceneSelectDirectory>> Directories { get; set; } = new Dictionary<string, List<SceneSelectDirectory>>();
        #endregion

        #region Structures
        public bool isFileViewMode { get => isFilesView.IsChecked.Value; }
        public string PreviousDataFolder { get; set; }
        public bool WithinAParentForm { get; set; } = false;
        private bool GameConfigAutoSave
        {
            get
            {
                if (AutoSaveCheckbox.IsChecked.HasValue) return AutoSaveCheckbox.IsChecked.Value;
                else return false;
            }
            set
            {
                AutoSaveCheckbox.IsChecked = value;
            }
        }
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
            this.RecentsTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.RecentsTreeAfterSelectEvent);
            this.RecentsTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.RecentsTreeMouseDownEvent);
            this.RecentsTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.RecentsTreeNodeDoubleClickEvent);
            this.RecentsTree.HideSelection = false;

            ScenesTree.Show();
            RecentsTree.Show();
        }

        private void InitilizeBase()
        {
            RemoveNullEntries();
            SetupWinFormControls();
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

            if (!WithinAParentForm)
            {
                CancelButton.Visibility = Visibility.Collapsed;
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
        private void RecentsTreeAfterSelectEvent(object sender, TreeViewEventArgs e)
        {
            loadSelectedButton.IsEnabled = RecentsTree.SelectedNode != null && RecentsTree.SelectedNode.Tag != null;
            UpdateSelectedSceneInfo();
        }
        private void ScenesTreeNodeMouseDoubleClickEvent(object sender, TreeNodeMouseClickEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                ScenesTree.SelectedNode = e.Node;
                if (e.Node.ImageKey == "Folder")
                    SceneInfoContextMenu.IsOpen = true;
                else if (e.Node.ImageKey == "File")
                    ScenesTreeCategoryContextMenu.IsOpen = true;
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
        private void GameConfigAddSceneEvent(object sender, RoutedEventArgs e)
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
                var cat = _GameConfig.Categories.Where(t => t.Name == ScenesTree.SelectedNode.Parent.Text).FirstOrDefault();
                if (cat != null)
                {
                    cat.Scenes.Add(form.Scene);
                    WriteGameConfigChangesToFile();
                }
            }
        }
        private void GameConfigAddCategoryEvent(object sender, RoutedEventArgs e)
        {
            if (!IsFunctionalGameConfig)
            {
                GameConfigThrowError();
                return;
            }
            var form = new EditSceneSelectInfoWindow();

            if (Window != null) form.Owner = System.Windows.Window.GetWindow(Window);
            else if (WithinAParentForm) form.Owner = System.Windows.Window.GetWindow(Instance);

            form.ShowDialog();
            if (form.DialogResult == true)
            {
                var scenes = new List<RSDKv5.Gameconfig.SceneInfo>();
                scenes.Add(form.Scene);

                var form2 = new SceneSelect.SceneSelectEditCategoryLabelWindow();

                if (Window != null) form2.Owner = System.Windows.Window.GetWindow(Window);
                else if (WithinAParentForm) form2.Owner = System.Windows.Window.GetWindow(Instance);

                form2.Scenes = scenes;
                form2.ShowDialog();

                if (form2.DialogResult == true)
                {
                    if (form2.Category != null)
                    {
                        _GameConfig.Categories.Insert(ScenesTree.SelectedNode.Index, form2.Category);
                        WriteGameConfigChangesToFile();
                    }


                }
            }
        }
        private void GameConfigEditSceneEvent(object sender, RoutedEventArgs e)
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
                    WriteGameConfigChangesToFile();
                }
            }
        }
        private void GameConfigEditCategoryEvent(object sender, RoutedEventArgs e)
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
                WriteGameConfigChangesToFile();
            }

        }
        private void GameConfigDeleteSceneEvent(object sender, RoutedEventArgs e)
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
                WriteGameConfigChangesToFile();

            }
        }
        private void GameConfigDeleteCategoryEvent(object sender, RoutedEventArgs e)
        {
            if (!IsFunctionalGameConfig)
            {
                GameConfigThrowError();
                return;
            }
            _GameConfig.Categories.RemoveAt(ScenesTree.SelectedNode.Index);
            WriteGameConfigChangesToFile();
        }
        private void GameConfigMoveCategoryUpEvent(object sender, RoutedEventArgs e)
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

            WriteGameConfigChangesToFile();

        }
        private void GameConfigMoveCategoryDownEvent(object sender, RoutedEventArgs e)
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

            WriteGameConfigChangesToFile();

        }
        private void GameConfigMoveSceneUpEvent(object sender, RoutedEventArgs e)
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

                WriteGameConfigChangesToFile();

            }
        }
        private void GameConfigMoveSceneDownEvent(object sender, RoutedEventArgs e)
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

                WriteGameConfigChangesToFile();
            }
        }
        private void WriteGameConfigChangesToFile()
        {
            if (!GameConfigAutoSave)
            {
                bool result = MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                if (result == false) return;
            }
            _GameConfig.Write(_GameConfig.FilePath);
            LoadFromGameConfig(new Gameconfig(_GameConfig.FilePath));
        }
        private void UpdateGameConfigSceneIndexes(Gameconfig config)
        {
            int currentIndex = 0;
            for (int c = 0; c < config.Categories.Count; c++)
            {
                for (int i = 0; i < config.Categories[c].Scenes.Count; i++)
                {
                    config.Categories[c].Scenes[i].Index = currentIndex;
                    config.Categories[c].Scenes[i].LevelID = currentIndex;
                    currentIndex = currentIndex + 1;
                }
            }

        }

        #endregion

        #region UI Related Methods
        public void RefreshTheme()
        {
            ScenesTree.BackColor = Methods.Internal.Theming.ThemeBrush2;
            ScenesTree.ForeColor = Methods.Internal.Theming.ThemeBrush4;

            RecentsTree.BackColor = Methods.Internal.Theming.ThemeBrush2;
            RecentsTree.ForeColor = Methods.Internal.Theming.ThemeBrush4;
        }
        public void RecentsTreeOpenContextMenu(TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RecentsTree.SelectedNode = e.Node;
                if (e.Node.ImageKey == "SavedDataFolder")
                    SavedDataDirEditContext.IsOpen = true;
                else if (e.Node.ImageKey == "RecentDataFolder")
                    RecentDataDirEditContext.IsOpen = true;
                else if (e.Node.ImageKey == "SavedPlace")
                    FolderEditContext.IsOpen = true;
            }
        }
        public void UpdateSceneCategoryContextMenu()
        {
            if (isFileViewMode)
            {
                ScenesTreeCategoryContextMenu.IsEnabled = false;
            }
            else
            {
                ScenesTreeCategoryContextMenu.IsEnabled = true;
                MoveSceneInfoDownToolStripMenuItem.IsEnabled = false;
                MoveSceneInfoUpToolStripMenuItem.IsEnabled = false;
            }
        }
        public void UpdateSceneInfoContextMenu()
        {
            if (isFileViewMode)
            {
                SceneInfoContextMenu.IsEnabled = false;
            }
            else
            {
                SceneInfoContextMenu.IsEnabled = true;
                if (SelectedCategoryIndex != -1)
                {
                    if (ScenesTree.SelectedNode.Index == 0)
                    {
                        MoveCategoryUpToolStripMenuItem.IsEnabled = false;
                    }
                    else
                    {
                        MoveCategoryUpToolStripMenuItem.IsEnabled = true;
                    }

                    if (ScenesTree.SelectedNode.Index == ScenesTree.Nodes.Count - 1)
                    {
                        MoveCategoryDownToolStripMenuItem.IsEnabled = false;
                    }
                    else
                    {
                        MoveCategoryDownToolStripMenuItem.IsEnabled = true;
                    }
                }
                else
                {
                    MoveCategoryUpToolStripMenuItem.IsEnabled = false;
                    MoveCategoryDownToolStripMenuItem.IsEnabled = false;
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
                if (DataDirectory != null && DataDirectory != string.Empty) dataLabelToolStripItem.Text = "Data Directory: " + DataDirectory;
                else dataLabelToolStripItem.Text = "Data Directory: N/A";
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
                MasterDataDirectorySelectorComboBox.Items.Add("(Unset)");
                if (Properties.Settings.MySettings.SavedDataDirectories == null) Properties.Settings.MySettings.SavedDataDirectories = new StringCollection();
                foreach (string savedDirectories in Properties.Settings.MySettings.SavedDataDirectories)
                {
                    MasterDataDirectorySelectorComboBox.Items.Add(savedDirectories);
                }

                if (lastItem == null) MasterDataDirectorySelectorComboBox.SelectedIndex = 0;
                else MasterDataDirectorySelectorComboBox.SelectedItem = lastItem;

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
                List<SceneSelectScene> scenes = new List<SceneSelectScene>();
                foreach (Gameconfig.SceneInfo scene in category.Scenes)
                {
                    scenes.Add(new SceneSelectScene(scene.Name, scene.Zone + "\\Scene" + scene.SceneID + ".bin"));

                    List<SceneSelectDirectory> files;
                    if (!Directories.TryGetValue(scene.Zone, out files))
                    {
                        files = new List<SceneSelectDirectory>();
                        Directories[scene.Zone] = files;
                    }
                    Directories[scene.Zone].Add(new SceneSelectDirectory("Scene" + scene.SceneID + ".bin" + (scene.ModeFilter == 5 ? " (Encore)" : ""), scene, scene.Zone + "\\Scene" + scene.SceneID + ".bin"));
                }
                Categories.Add(category.Name, scenes);
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
                foreach (KeyValuePair<string, List<SceneSelectDirectory>> directory in Directories)
                {
                    TreeNode dir_node = new TreeNode(directory.Key);
                    dir_node.ImageKey = "Folder";
                    dir_node.SelectedImageKey = "Folder";
                    foreach (SceneSelectDirectory file in directory.Value)
                    {
                        TreeNode file_node = new TreeNode(file.Name);
                        file_node.Tag = file.SceneInfo;
                        file_node.ImageKey = "File";
                        file_node.ImageKey = "File";
                        file_node.SelectedImageKey = "File";
                        if (filter == "" || (directory.Key + "/" + file.Name).ToLower().Contains(filter.ToLower()))
                            dir_node.Nodes.Add(file_node);
                    }
                    if (dir_node.Nodes.Count > 0)
                        ScenesTree.Nodes.Add(dir_node);
                }
            }
            else
            {
                foreach (SceneSelectCategory category in Categories.Items)
                {
                    TreeNode dir_node = new TreeNode(category.Name);
                    dir_node.ImageKey = "Folder";
                    dir_node.SelectedImageKey = "Folder";
                    string last = "";
                    foreach (SceneSelectScene scene in category.Entries)
                    {
                        string scene_name = scene.Name;
                        if (char.IsDigit(scene.Name[0]))
                            scene_name = last + scene.Name;

                        TreeNode file_node = new TreeNode(scene_name + " (" + scene.Path + ")");
                        file_node.Tag = scene.Path;
                        file_node.ImageKey = "File";
                        file_node.SelectedImageKey = "File";
                        if (filter == "" || scene.Path.ToLower().Contains(filter.ToLower()) || scene_name.ToLower().Contains(filter.ToLower()))
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
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            addButton.IsOpen = true;
        }
        private void RemoveSavedDataDirectoryToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedSavedDataFolder();
        }
        private void RemoveRecentDataDirectoryToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedRecentDataFolder();
        }
        private void MasterDataDirectorySelectorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllowMasterDataDirectoryUpdate && MasterDataDirectorySelectorComboBox.SelectedIndex != -1)
            {
                if (MasterDataDirectorySelectorComboBox.SelectedIndex == 0) LoadMasterDataDirectory(null);
                else LoadMasterDataDirectory(MasterDataDirectorySelectorComboBox.SelectedItem.ToString());
            }
        }
        private void RecentsTreeMouseDownEvent(object sender, TreeNodeMouseClickEventArgs e)
        {
            RecentsTreeOpenContextMenu(e);
        }
        private void BrowseButtonEvent(object sender, RoutedEventArgs e)
        {
            BrowseForSceneFile();
        }
        private void RemoveSelectedSavedPlaceEvent(object sender, RoutedEventArgs e)
        {
            RemoveSelectedSavedPlace();
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
            else if (RecentsTree.SelectedNode != null)
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
                        if (Instance != null) Methods.Internal.UserInterface.Status.UpdateDataFolderLabel(DataDirectory);
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
                SceneState.DataDirectory = DataDirectory;
                SceneState.FilePath = open.FileName;
                SceneState.LevelID = -1;
                SceneState.IsEncoreMode = false;
                SceneState.SceneID = "N/A";
                SceneState.Zone = "N/A";
                SceneState.LoadType = Classes.General.SceneState.LoadMethod.FullPath;
                SceneState.Name = "N/A";
                SceneState.MasterDataDirectory = Methods.Editor.SolutionPaths.DefaultMasterDataDirectory;
                CloseHostEvent();
            }
        }
        public void LoadMasterDataDirectory(string SelectedDataDirectory)
        {
            UnloadDataDirectory();
            if (SelectedDataDirectory != null)
            {
                SceneState.MasterDataDirectory = SelectedDataDirectory;
                Gameconfig GameConfig = ManiacEditor.Methods.Editor.SolutionPaths.GetGameConfig(SelectedDataDirectory);
                if (GameConfig != null)
                {
                    LoadFromGameConfig(GameConfig);
                    Classes.Prefrences.DataDirectoriesStorage.AddRecentDataFolder(DataDirectory);
                    Properties.Settings.MySettings.DefaultMasterDataDirectory = SceneState.MasterDataDirectory;
                }
                else UnloadMasterDataDirectory();
            }
            else
            {
                Properties.Settings.MySettings.DefaultMasterDataDirectory = null;
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
                LoadFromGameConfig(GameConfig);
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

            if (SaveState.DataDirectory != string.Empty && SaveState.DataDirectory != "")
            {
                LoadDataDirectory((SaveState.ExtraDataDirectories != null && SaveState.ExtraDataDirectories.Count >= 1 ? SaveState.ExtraDataDirectories[0] : string.Empty));
            }

            UpdateDataDirectoryLabel();
            UpdateMasterDataDirectoryComboBox();
        }
        public void LoadFromGameConfig(Gameconfig config)
        {
            _GameConfig = null;
            _GameConfig = config;
            UpdateScenesTree(config);
            if (Properties.Settings.MyDefaults.SceneSelectFilesViewDefault) this.isFilesView.IsChecked = true;
            else this.isFilesView.IsChecked = false;
        }
        #endregion

        private void UnloadDataPackButton_Click(object sender, RoutedEventArgs e)
        {
            UnloadDataDirectory();
            MasterDataDirectorySelectorComboBox_SelectionChanged(null, null);
        }
        private void ScenesTreeCategoryContextMenuOpeningEvent(object sender, RoutedEventArgs e)
        {
            UpdateSceneInfoContextMenu();
        }
        private void ScenesTreeSceneContextMenuOpeningEvent(object sender, RoutedEventArgs e)
        {
            UpdateSceneCategoryContextMenu();
        }
        private void SavedDataDirEditContext_Opening(object sender, RoutedEventArgs e)
        {
            UpdateSavedDataDirInfoContextMenu();
        }
        private void RecentDataDirEditContext_Opening(object sender, RoutedEventArgs e)
        {
            UpdateSavedDataDirInfoContextMenu();
        }


    }
}
