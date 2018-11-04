using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManiacEditor.Properties;
using System.Configuration;
using RSDKv5;
using System.Diagnostics;

namespace ManiacEditor
{
    public partial class SceneSelect : Form
    {
        public List<Tuple<string, List<Tuple<string, string>>>> Categories = new List<Tuple<string, List<Tuple<string, string>>>>();
        public Dictionary<string, List<string>> Directories = new Dictionary<string, List<string>>();
        public GameConfig _GameConfig;

        //In the case we have a bad gameconfig
        public string prevDataDir;

        public string Result = null;
        public int LevelID = -1;
        public bool isEncore = false;


        public SceneSelect(GameConfig config = null)
        {
            InitializeComponent();
            if (Properties.EditorState.Default.preRenderSceneSelectCheckbox) preRenderCheckbox.Checked = true;
            if (Properties.Settings.Default.preRenderSceneOption == 1) preRenderCheckbox.Enabled = true;
            ReloadQuickPanel();
            if (config != null)
            {
                LoadFromGameConfig(config);
                _GameConfig = config;
            }
        }

        public void LoadFromGameConfig(GameConfig config)
        {
            Categories.Clear();
            Directories.Clear();
            RSDKv5.GameConfig.CurrentLevelID = 0;
            foreach (GameConfig.Category category in config.Categories)
            {
                List<Tuple<string, string>> scenes = new List<Tuple<string, string>>();
                foreach (GameConfig.SceneInfo scene in category.Scenes)
                {
                    scenes.Add(new Tuple<string, string>(scene.Name, scene.Zone + "\\Scene" + scene.SceneID + ".bin"));

                    List<string> files;
                    if (!Directories.TryGetValue(scene.Zone, out files))
                    {
                        files = new List<string>();
                        Directories[scene.Zone] = files;
                    }
                    files.Add("Scene" + scene.SceneID + ".bin");
                }
                Categories.Add(new Tuple<string, List<Tuple<string, string>>>(category.Name, scenes));
            }

            // Sort
            Directories = Directories.OrderBy(key => key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            foreach (KeyValuePair<string, List<String>> dir in Directories)
                dir.Value.Sort();

            this.scenesTree.ImageList = new ImageList();
            this.scenesTree.ImageList.Images.Add("Folder", Properties.Resources.folder);
            this.scenesTree.ImageList.Images.Add("File", Properties.Resources.file);

            UpdateTree();
            if (Properties.Settings.Default.IsFilesViewDefault)
            {
                this.isFilesView.Checked = true;
            }
            else
            {
                this.isFilesView.Checked = false;
            }
        }

        public void ReloadQuickPanel()
        {
            if (Editor.DataDirectory != null)
            {
                dataLabelToolStripItem.Text = "Data Directory: " + Editor.DataDirectory;
            }
            else
            {
                dataLabelToolStripItem.Text = "Data Directory: NULL";
            }

            recentDataDirList.Nodes.Clear();
            recentDataDirList.Nodes.Add("Recent Data Directories");
            recentDataDirList.Nodes.Add("Saved Places");
            this.recentDataDirList.ImageList = new ImageList();
            this.recentDataDirList.ImageList.Images.Add("Folder", Properties.Resources.folder);
            this.recentDataDirList.ImageList.Images.Add("File", Properties.Resources.file);
            foreach (ToolStripMenuItem dataDir in Editor.Instance._recentDataItems)
            {
                var node = recentDataDirList.Nodes[0].Nodes.Add(dataDir.Text);
                node.Tag = dataDir.Text;
                node.ToolTipText = dataDir.Text;
            }
            recentDataDirList.Nodes[0].ExpandAll();

            if (Properties.Settings.Default.SavedPlaces?.Count > 0 && Editor.DataDirectory != null)
            {
                StringCollection recentFolders = new StringCollection();
                this.recentDataDirList.ImageList.Images.Add("SubFolder", Properties.Resources.folder);
                int index = this.recentDataDirList.ImageList.Images.IndexOfKey("SubFolder");
                recentFolders = Properties.Settings.Default.SavedPlaces;
                foreach (string folder in recentFolders)
                {
                    var node = recentDataDirList.Nodes[1].Nodes.Add(folder, folder, index, index);
                    node.Tag = folder;
                    node.ToolTipText = folder;
                }
                recentDataDirList.Nodes[1].ExpandAll();
            }

        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            int _levelID = -1;
            int _isEncore = 0;
            var cat = _GameConfig.Categories.Where(t => t.Name == scenesTree.SelectedNode.Parent.Text).FirstOrDefault();
            if (cat != null)
            {
                var scene = cat.Scenes.Where(t => $"{t.Zone}\\Scene{t.SceneID}.bin" == scenesTree.SelectedNode.Tag as string).FirstOrDefault();
                Properties.EditorState.Default.Level_ID = scene.LevelID;
                _isEncore = scene.ModeFilter;
                _levelID = scene.LevelID;
            }
            Result = scenesTree.SelectedNode.Tag as string;
            LevelID = _levelID;
            if (_isEncore == 5)
            {
                isEncore = true;
            }
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateTree()
        {
            Show(FilterText.Text);
        }

        private void Show(string filter)
        {
            scenesTree.Nodes.Clear();
            if (isFilesView.Checked)
            {
                foreach (KeyValuePair<string, List<string>> directory in Directories)
                {
                    TreeNode dir_node = new TreeNode(directory.Key);
                    dir_node.ImageKey = "Folder";
                    dir_node.SelectedImageKey = "Folder";
                    dir_node.ContextMenuStrip = contextMenuStrip1;
                    foreach (string file in directory.Value) {
                        TreeNode file_node = new TreeNode(file);
                        file_node.Tag = directory.Key + "/" + file;
                        file_node.ImageKey = "File";
                        file_node.ImageKey = "File";
                        file_node.SelectedImageKey = "File";
                        if (filter == "" || (directory.Key + "/" + file).ToLower().Contains(filter.ToLower()))
                            dir_node.Nodes.Add(file_node);
                    }
                    if (dir_node.Nodes.Count > 0)
                        scenesTree.Nodes.Add(dir_node);
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
                        scenesTree.Nodes.Add(dir_node);
                }
            }
            if (filter != "")
            {
                scenesTree.ExpandAll();
            }
        }

        private void isFilesView_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTree();
        }

        private void FilterText_TextChanged(object sender, EventArgs e)
        {
            UpdateTree();
        }

        private void scenesTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectButton.Enabled = scenesTree.SelectedNode.Tag != null;
        }

        private void scenesTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (selectButton.Enabled)
            {
                selectButton_Click(sender, e);
            }
        }

        private void scenesTree_MouseUp(object sender, MouseEventArgs e)
        {
            if (scenesTree.SelectedNode == null)
            {
                selectButton.Enabled = false;
            }
        }

        private void browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = 
            open.Filter = "Scene File|*.bin";
            if (open.ShowDialog() != DialogResult.Cancel)
            {
                Result = open.FileName;
                Close();
            }
        }

        private void selectable_browse_Click(string initialDir)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = initialDir;
            open.Filter = "Scene File|*.bin";
            if (open.ShowDialog() != DialogResult.Cancel)
            {
                Result = open.FileName;
                Close();
            }
        }

        private void scenesTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                scenesTree.SelectedNode = e.Node;
                if (e.Node.ImageKey == "Folder")
                    contextMenuStrip1.Show(scenesTree, e.Location);
                else if (e.Node.ImageKey == "File")
                    contextMenuStrip2.Show(scenesTree, e.Location);
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new EditSceneSelectInfoForm();
            if (form.ShowDialog() == DialogResult.Yes)
            {
                var cat = _GameConfig.Categories.Where(t => t.Name == scenesTree.SelectedNode.Text).FirstOrDefault();
                if (cat != null)
                {
                    cat.Scenes.Add(form.Scene);
                    LoadFromGameConfig(_GameConfig);
                    if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        _GameConfig.Write(Path.Combine(Editor.DataDirectory, "Game", "GameConfig.bin"));
                }
            }
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var cat = _GameConfig.Categories.Where(t => t.Name == scenesTree.SelectedNode.Parent.Text).FirstOrDefault();
            if (cat != null)
            {
                var scene = cat.Scenes.Where(t => $"{t.Zone}\\Scene{t.SceneID}.bin" == scenesTree.SelectedNode.Tag as string).FirstOrDefault();
                var form = new EditSceneSelectInfoForm(scene);
                if (form.ShowDialog() == DialogResult.Yes)
                {
                    LoadFromGameConfig(_GameConfig);
                    if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        _GameConfig.Write(Path.Combine(Editor.DataDirectory, "Game", "GameConfig.bin"));
                }
            }
        }

        private void deleteSceneInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cat = _GameConfig.Categories.Where(t => t.Name == scenesTree.SelectedNode.Parent.Text).FirstOrDefault();
            if (cat != null)
            {
                var scene = cat.Scenes.FindIndex(t => $"{t.Zone}\\Scene{t.SceneID}.bin" == scenesTree.SelectedNode.Tag as string);
                if (scene + 1 < cat.Scenes.Count && !char.IsDigit(cat.Scenes[scene].Name[0]) && char.IsDigit(cat.Scenes[scene + 1].Name[0]))
                {
                    if (MessageBox.Show("This Scene as other acts attached,\n" +
                        "Deleting this scene will set the next scene as the main scene of the stage, \n" +
                        "Are you sure you want to continue?",
                        "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        return;
                    cat.Scenes[scene + 1].Name = cat.Scenes[scene].Name.
                        Replace(" " + cat.Scenes[scene].SceneID, " "+cat.Scenes[scene + 1].SceneID);
                }
                cat.Scenes.RemoveAt(scene);
                LoadFromGameConfig(_GameConfig);
                if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    _GameConfig.Write(Path.Combine(Editor.DataDirectory, "Game", "GameConfig.bin"));
            }
        }

        private void load_Click(object sender, EventArgs e)
        {
            int NodeType = 0;
            for (int i = 0; i < recentDataDirList.Nodes[0].Nodes.Count; i++)
            {
                if (recentDataDirList.Nodes[0].Nodes[i].IsSelected)
                {
                    if (Editor.Instance.importingObjects == false)
                    {
                        NodeType = 0;
                    }
                    else
                    {
                        NodeType = -1;
                    }

                    if (Editor.DataDirectory != null)
                    {
                        dataLabelToolStripItem.Text = "Data Directory: " + Editor.DataDirectory;
                    }
                    else
                    {
                        dataLabelToolStripItem.Text = "Data Directory: NULL";
                    }
                }
                else
                {
                    
                }
            }

            for (int i = 0; i < recentDataDirList.Nodes[1].Nodes.Count; i++)
            {
                if (recentDataDirList.Nodes[1].Nodes[i].IsSelected)
                {
                    NodeType = 1;
                }
            }

            if (NodeType == 0)
            {
                GameConfig GameConfig = null;
                String SelectedDataDirectory = recentDataDirList.SelectedNode.Tag.ToString();
                {
                    try
                    {
                        GameConfig = new GameConfig(Path.Combine(SelectedDataDirectory, "Game", "GameConfig.bin"));
                    }
                    catch
                    {
                        // Allow the User to be able to have a Maniac Editor Dedicated GameConfig, see if the user has made one
                        try
                        {
                            GameConfig = new GameConfig(Path.Combine(SelectedDataDirectory, "Game", "GameConfig_ME.bin"));
                        }
                        catch
                        {
                            MessageBox.Show("Something is wrong with this GameConfig that we can't support! If for some reason it does work you in Sonic Mania can create another GameConfig.bin called GameConfig_ME.bin and the editor should load that instead allowing you to still be able to use the data folder, however, this is experimental so be careful when doing that.", "GameConfig Error!");
                            Editor.DataDirectory = prevDataDir;
                            prevDataDir = null;

                            if (Editor.DataDirectory != null)
                            {
                                dataLabelToolStripItem.Text = "Data Directory: " + Editor.DataDirectory;
                            }
                            else
                            {
                                dataLabelToolStripItem.Text = "Data Directory: NULL";
                            }

                        }
                    }
                }
                if (GameConfig != null)
                {
                    LoadFromGameConfig(GameConfig);
                    _GameConfig = GameConfig;
                }
            }
            if (NodeType == 1)
            {

            }
            if (NodeType == -1)
            {
                MessageBox.Show("You can't do that while importing objects!");
            }

        }

        private void setButtonEnabledState(bool enabled)
        {
            this.browse.Enabled = enabled;
            this.selectButton.Enabled = enabled;
            this.FilterText.Enabled = enabled;
            this.scenesTree.Enabled = enabled;
            this.isFilesView.Enabled = enabled;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            contextMenuStrip3.Show(ptLowerLeft);
        }

        private void dataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Editor.Instance.importingObjects == false)
            {
                string newDataDirectory = Editor.Instance.GetDataDirectory();
                string returnDataDirectory;

                if (string.IsNullOrWhiteSpace(newDataDirectory)) return;
                if (Editor.Instance.IsDataDirectoryValid(newDataDirectory))
                {
                    Editor.DataDirectory = newDataDirectory;
                    returnDataDirectory = newDataDirectory;
                    bool goodDataDir = Editor.Instance.SetGameConfig();
                    if (goodDataDir == true)
                    {
                        Editor.Instance.AddRecentDataFolder(Editor.DataDirectory);
                        Editor.Instance.RefreshDataDirectories(Properties.Settings.Default.DataDirectories);
                        ReloadQuickPanel();
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

        private void clearDataDirectoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DataDirectories.Clear();
            Editor.Instance.RefreshDataDirectories(Properties.Settings.Default.DataDirectories);
            ReloadQuickPanel();


        }

        private void recentDataDirList_Click(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < recentDataDirList.Nodes[0].Nodes.Count; i++)
                {
                    if (recentDataDirList.Nodes[0].Nodes[i].IsSelected)
                    {
                        dataDirEditContext.Show(recentDataDirList, e.Location);
                    }
                }

                for (int i = 0; i < recentDataDirList.Nodes[1].Nodes.Count; i++)
                {
                    if (recentDataDirList.Nodes[1].Nodes[i].IsSelected)
                    {
                        folderEditContext.Show(recentDataDirList, e.Location);
                    }
                }
            }
        }

        public void savedPlaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String newSavedPlace;
            using (var folderBrowserDialog = new FolderSelectDialog())
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
                var mySettings = Properties.Settings.Default;
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

                mySettings.Save();

                ReloadQuickPanel();
            }
            catch (Exception ex)
            {
                Debug.Print("Failed to add Saved Place to list: " + ex);
            }
        }

        private void removeSavedFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String toRemove = recentDataDirList.SelectedNode.Tag.ToString();
            if (Settings.Default.SavedPlaces.Contains(toRemove))
            {
                Settings.Default.SavedPlaces.Remove(toRemove);
            }
            ReloadQuickPanel();
        }

        private void removeDataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String toRemove = recentDataDirList.SelectedNode.Tag.ToString();
            if (Settings.Default.DataDirectories.Contains(toRemove))
            {
                Settings.Default.DataDirectories.Remove(toRemove);
            }
            Editor.Instance.RefreshDataDirectories(Properties.Settings.Default.DataDirectories);
            ReloadQuickPanel();
        }

        private void recentDataDirList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < recentDataDirList.Nodes[0].Nodes.Count; i++)
                {
                    if (recentDataDirList.Nodes[0].Nodes[i].IsSelected)
                    {
                        prevDataDir = Editor.DataDirectory;
                        Editor.DataDirectory = recentDataDirList.Nodes[0].Nodes[i].Tag.ToString();
                        load_Click(sender, e);
                    }
                }

                for (int i = 0; i < recentDataDirList.Nodes[1].Nodes.Count; i++)
                {
                    if (recentDataDirList.Nodes[1].Nodes[i].IsSelected)
                    {
                        if (_GameConfig != null)
                        {
                            selectable_browse_Click(recentDataDirList.Nodes[1].Nodes[i].Tag.ToString());
                        }
                        else
                        {
                            MessageBox.Show("Please Select/Open a Data Directory First", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                }
            }
        }

        private void preRenderCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (preRenderCheckbox.Checked == true)
                Properties.EditorState.Default.preRenderSceneSelectCheckbox = true;
            else
                Properties.EditorState.Default.preRenderSceneSelectCheckbox = false;
        }
    }
}
