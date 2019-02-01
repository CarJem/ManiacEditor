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
using Microsoft.Xna.Framework;
using Point = System.Drawing.Point;
using System.Linq.Expressions;


namespace ManiacEditor
{
	using MessageBox = System.Windows.Forms.MessageBox;
	using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
	using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;

	public partial class Sanic2Maniac : Form
    {
        public List<Tuple<string, List<Tuple<string, string>>>> Categories = new List<Tuple<string, List<Tuple<string, string>>>>();
        public Dictionary<string, List<string>> Directories = new Dictionary<string, List<string>>();
        public GameConfig _GameConfig;

        //In the case we have a bad gameconfig
        public string prevDataDir;

        public string Result = null;
        public int LevelID = -1;
        public bool isEncore = false;
        Timer timer = new Timer();
        public int selectedCategoryIndex = -1;
        bool filesMode = false;
        public Editor EditorInstance;


        public Sanic2Maniac(GameConfig config = null, Editor instance = null)
        {
            EditorInstance = instance;
            InitializeComponent();
            ReloadQuickPanel();
            if (config != null)
            {
                LoadFromGameConfig(config);
                _GameConfig = config;
            }

            timer.Interval = 10;
            timer.Tick += new EventHandler(UpdateToolstrip);
            timer.Start();
        }

        public void LoadFromGameConfig(GameConfig config)
        {
            Categories.Clear();
            Directories.Clear();
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
        }


        public void UpdateToolstrip(object sender, EventArgs e)
        {
            if (scenesTree.SelectedNode != null)
            {
                selectedCategoryIndex = scenesTree.SelectedNode.Index;
            }
            else
            {
                selectedCategoryIndex = -1;
            }
        }

        public void ReloadQuickPanel()
        {

            recentDataDirList.Nodes.Clear();
            recentDataDirList.Nodes.Add("Maniac Editor Data Directories");
            this.recentDataDirList.ImageList = new ImageList();
            this.recentDataDirList.ImageList.Images.Add("Folder", Properties.Resources.folder);
            this.recentDataDirList.ImageList.Images.Add("File", Properties.Resources.file);
            
            foreach (var dataDir in EditorInstance._recentDataItems)
            {
                var node = recentDataDirList.Nodes[0].Nodes.Add(dataDir.Header.ToString());
                node.Tag = dataDir.Header.ToString();
                node.ToolTipText = dataDir.Header.ToString();
            }
            
            recentDataDirList.Nodes[0].ExpandAll();

        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            int _levelID = -1;
            int _isEncore = 0;
            var cat = _GameConfig.Categories.Where(t => t.Name == scenesTree.SelectedNode.Parent.Text).FirstOrDefault();
            if (cat != null)
            {
                var scene = cat.Scenes.Where(t => $"{t.Zone}\\Scene{t.SceneID}.bin" == scenesTree.SelectedNode.Tag as string).FirstOrDefault();
                EditorInstance.LevelID = scene.LevelID;
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
            Show(textBox1.Text);
        }

        private void Show(string filter)
        {
            scenesTree.Nodes.Clear();
            if (filesMode)
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

        }

        private void scenesTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }

        private void scenesTree_MouseUp(object sender, MouseEventArgs e)
        {

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
                        _GameConfig.Write(Path.Combine(EditorInstance.DataDirectory, "Game", "GameConfig.bin"));
                    ReloadGameConfig();

                }
            }
        }

        private void addCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new EditSceneSelectInfoForm();
            if (form.ShowDialog() == DialogResult.Yes)
            {
                    var scenes = new List <RSDKv5.GameConfig.SceneInfo> ();
                    scenes.Add(form.Scene);
                    
                    var form2 = new EditCategorySelectInfoForm();
                    form2.Scenes = scenes;
                    
                    if (form2.ShowDialog() == DialogResult.Yes)
                    {
                    if (form2.Category != null)
                    {
                        _GameConfig.Categories.Insert(scenesTree.SelectedNode.Index, form2.Category);
                        LoadFromGameConfig(_GameConfig);

                        if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            _GameConfig.Write(Path.Combine(EditorInstance.DataDirectory, "Game", "GameConfig.bin"));
                            ReloadGameConfig();
                    }


                    }
            }
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var cat = _GameConfig.Categories.Where(t => t.Name == scenesTree.SelectedNode.Parent.Text).FirstOrDefault();
            if (cat != null)
            {
                var scene = cat.Scenes.Where(t => t.Index == scenesTree.SelectedNode.Index).FirstOrDefault();
                var form = new EditSceneSelectInfoForm(scene);
                if (form.ShowDialog() == DialogResult.Yes)
                {
                    LoadFromGameConfig(_GameConfig);
                    if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        _GameConfig.Write(Path.Combine(EditorInstance.DataDirectory, "Game", "GameConfig.bin"));
                        ReloadGameConfig();
                }
            }
        }

        private void editCategoryMenuItem_Click(object sender, EventArgs e)
        {
                var Category = _GameConfig.Categories[scenesTree.SelectedNode.Index];
                var form = new EditCategorySelectInfoForm(Category, Category.Scenes);
                if (form.ShowDialog() == DialogResult.Yes)
                {
                    LoadFromGameConfig(_GameConfig);
                    if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        _GameConfig.Write(Path.Combine(EditorInstance.DataDirectory, "Game", "GameConfig.bin"));
                        ReloadGameConfig();
            }
            
        }

        private void deleteSceneInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cat = _GameConfig.Categories.Where(t => t.Name == scenesTree.SelectedNode.Parent.Text).FirstOrDefault();
            if (cat != null)
            {
                var scene = cat.Scenes.FindIndex(t => t.Index == scenesTree.SelectedNode.Index);
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
                    _GameConfig.Write(Path.Combine(EditorInstance.DataDirectory, "Game", "GameConfig.bin"));
                    ReloadGameConfig();
            }
        }

        private void deleteCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _GameConfig.Categories.RemoveAt(scenesTree.SelectedNode.Index);
            LoadFromGameConfig(_GameConfig);
            if (MessageBox.Show("Write Changes to File? Please make sure you didn't delete something on accident!", "Write to File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                _GameConfig.Write(Path.Combine(EditorInstance.DataDirectory, "Game", "GameConfig.bin"));
                ReloadGameConfig();
        }

        private void load_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < recentDataDirList.Nodes[0].Nodes.Count; i++)
            {
                if (recentDataDirList.Nodes[0].Nodes[i].IsSelected)
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
                                EditorInstance.DataDirectory = prevDataDir;
                                prevDataDir = null;

                            }
                        }
                    }
                    if (GameConfig != null)
                    {
                        LoadFromGameConfig(GameConfig);
                        _GameConfig = GameConfig;
                    }
                }
            }

        }

        private void setButtonEnabledState(bool enabled)
        {
            this.scenesTree.Enabled = enabled;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            contextMenuStrip3.Show(ptLowerLeft);
        }

        private void ReloadGameConfig()
        {
            _GameConfig.Write(Path.Combine(Environment.CurrentDirectory, "GameConfig_Temp.bin"));
            _GameConfig = new GameConfig(Path.Combine(Environment.CurrentDirectory, "GameConfig_Temp.bin"));
            File.Delete(Path.Combine(Environment.CurrentDirectory, "GameConfig_Temp.bin"));
            LoadFromGameConfig(_GameConfig);        
        }

        private void dataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditorInstance.importingObjects == false)
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
                        EditorInstance.RefreshDataDirectories(Properties.Settings.Default.DataDirectories);
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
            EditorInstance.RefreshDataDirectories(Properties.Settings.Default.DataDirectories);
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
            if (Settings.mySettings.SavedPlaces.Contains(toRemove))
            {
                Settings.mySettings.SavedPlaces.Remove(toRemove);
            }
            ReloadQuickPanel();
        }

        private void removeDataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String toRemove = recentDataDirList.SelectedNode.Tag.ToString();
            if (Settings.mySettings.DataDirectories.Contains(toRemove))
            {
                Settings.mySettings.DataDirectories.Remove(toRemove);
            }
            EditorInstance.RefreshDataDirectories(Properties.Settings.Default.DataDirectories);
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
                        prevDataDir = EditorInstance.DataDirectory;
                        EditorInstance.DataDirectory = recentDataDirList.Nodes[0].Nodes[i].Tag.ToString();
                        load_Click(sender, e);
                    }
                }
            }
        }

        private void scenesTree_Click(object sender, EventArgs e)
        {

        }

        private void moveCategoryUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = _GameConfig.Categories[scenesTree.SelectedNode.Index];
            int OldIndex = _GameConfig.Categories.IndexOf(item);
            var itemAbove = _GameConfig.Categories[scenesTree.SelectedNode.Index - 1];
            _GameConfig.Categories.RemoveAt(scenesTree.SelectedNode.Index);
            int NewIndex = _GameConfig.Categories.IndexOf(itemAbove);

            if (NewIndex == OldIndex) NewIndex--;

            _GameConfig.Categories.Insert(NewIndex, item);

            LoadFromGameConfig(_GameConfig);
            if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                _GameConfig.Write(Path.Combine(EditorInstance.DataDirectory, "Game", "GameConfig.bin"));
            ReloadGameConfig();
        }

        private void moveCategoryDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = _GameConfig.Categories[scenesTree.SelectedNode.Index];
            int OldIndex = _GameConfig.Categories.IndexOf(item);
            var itemAbove = _GameConfig.Categories[scenesTree.SelectedNode.Index + 1];
            _GameConfig.Categories.RemoveAt(scenesTree.SelectedNode.Index);
            int NewIndex = _GameConfig.Categories.IndexOf(itemAbove);

            if (NewIndex == OldIndex) NewIndex++;

            _GameConfig.Categories.Insert(NewIndex, item);

            LoadFromGameConfig(_GameConfig);
            if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                _GameConfig.Write(Path.Combine(EditorInstance.DataDirectory, "Game", "GameConfig.bin"));
            ReloadGameConfig();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (selectedCategoryIndex != -1)
            {
                if (scenesTree.SelectedNode.Index == 0)
                {
                    moveCategoryUpToolStripMenuItem.Enabled = false;
                }
                else
                {
                    moveCategoryUpToolStripMenuItem.Enabled = true;
                }

                if (scenesTree.SelectedNode.Index == scenesTree.Nodes.Count - 1)
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

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            moveSceneInfoDownToolStripMenuItem.Enabled = false;
            moveSceneInfoUpToolStripMenuItem.Enabled = false;
        }

        private void moveSceneInfoUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cat = _GameConfig.Categories.Where(t => t.Name == scenesTree.SelectedNode.Parent.Text).FirstOrDefault();
            if (cat != null)
            {
                var scene = cat.Scenes.Where(t => t.Index == scenesTree.SelectedNode.Index).FirstOrDefault();
                int OldIndex = cat.Scenes.IndexOf(scene);
                var sceneAbove = cat.Scenes.Where(t => t.Index == scenesTree.SelectedNode.Index - 1).FirstOrDefault();
                cat.Scenes.Remove(scene);
                int NewIndex = cat.Scenes.IndexOf(sceneAbove);

                if (NewIndex == OldIndex) NewIndex--;
                cat.Scenes.Insert(NewIndex, scene);

                LoadFromGameConfig(_GameConfig);
                if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    _GameConfig.Write(Path.Combine(EditorInstance.DataDirectory, "Game", "GameConfig.bin"));
                ReloadGameConfig();
            }
        }

        private void moveSceneInfoDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cat = _GameConfig.Categories.Where(t => t.Name == scenesTree.SelectedNode.Parent.Text).FirstOrDefault();
            if (cat != null)
            {
                
                var scene = cat.Scenes.Where(t => t.Index == scenesTree.SelectedNode.Index).FirstOrDefault();
                int OldIndex = cat.Scenes.IndexOf(scene);
                var sceneBelow = cat.Scenes.Where(t => t.Index == scenesTree.SelectedNode.Index + 1).FirstOrDefault();
                cat.Scenes.Remove(scene);
                int NewIndex = cat.Scenes.IndexOf(sceneBelow);

                if (NewIndex == OldIndex) NewIndex++;
                cat.Scenes.Insert(NewIndex, scene);
                

                LoadFromGameConfig(_GameConfig);
                if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    _GameConfig.Write(Path.Combine(EditorInstance.DataDirectory, "Game", "GameConfig.bin"));
                ReloadGameConfig();
            }
        }

        private void clearDataDirectoriesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to do this? No undos here!", "Delete All Data Directories", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (Settings.mySettings.DataDirectories != null)
                {
                    Settings.mySettings.DataDirectories.Clear();
                    EditorInstance.RefreshDataDirectories(Properties.Settings.Default.DataDirectories);
                    ReloadQuickPanel();
                }


            }

        }

        private void removeAllSavedPlacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to do this? No undos here!", "Delete All Saved Places", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (Settings.mySettings.SavedPlaces != null)
                {
                    Settings.mySettings.SavedPlaces.Clear();
                    ReloadQuickPanel();
                }
            }
        }

        private void Sanic2Maniac_Load(object sender, EventArgs e)
        {

        }
    }
}
