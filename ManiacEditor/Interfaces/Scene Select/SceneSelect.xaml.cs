using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using ManiacEditor.Properties;
using System.Configuration;
using RSDKv5;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Point = System.Drawing.Point;
using System.Linq.Expressions;
using ManiacEditor.Interfaces;
using ImageList = System.Windows.Forms.ImageList;
using Path = System.IO.Path;
using Timer = System.Windows.Forms.Timer;
using TreeNode = System.Windows.Forms.TreeNode;
using TreeNodeMouseClickEventArgs = System.Windows.Forms.TreeNodeMouseClickEventArgs;
using TreeViewEventArgs = System.Windows.Forms.TreeViewEventArgs;
using MouseButtons = System.Windows.Forms.MouseButtons;
using MessageBox = RSDKrU.MessageBox;

namespace ManiacEditor.Interfaces
{
	/// <summary>
	/// Interaction logic for SceneSelect.xaml
	/// </summary>
	public partial class SceneSelect : UserControl
	{
		public List<Tuple<string, List<Tuple<string, string>>>> Categories = new List<Tuple<string, List<Tuple<string, string>>>>();
		public Dictionary<string, List<Tuple<string, Tuple<GameConfig.SceneInfo, string>>>> Directories = new Dictionary<string, List<Tuple<string, Tuple<GameConfig.SceneInfo, string>>>>();

		public bool isFileViewMode { get => isFilesView.IsChecked.Value; }

		public GameConfig _GameConfig;
		public SceneSelectWindow Window;

		//In the case we have a bad gameconfig
		public string prevDataDir;

		public string Result = null;
		public bool withinAParentForm = false;
		public bool isEncore = false;
		public bool UsingDataPack = false;
		Timer timer = new Timer();
		public int selectedCategoryIndex = -1;
		public Editor EditorInstance;

		public int LevelID = -1;
		public string CurrentZone = "";
		public string CurrentName = "";
		public string CurrentSceneID = "";
		public bool Browsed = false;

		//Winform Stuff
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
		private System.Windows.Forms.TreeView scenesTree;
		private System.Windows.Forms.TreeView recentDataDirList;

		public SceneSelect(GameConfig config = null, Editor instance = null, bool parented = false, SceneSelectWindow window = null)
		{
			if (Settings.MySettings.DataDirectories != null)  Settings.MySettings.DataDirectories.Remove(null);
			if (Settings.MySettings.SavedPlaces != null)  Settings.MySettings.SavedPlaces.Remove(null);

			EditorInstance = instance;
			InitializeComponent();
			SetupWinFormTreeStuff();
			withinAParentForm = parented;
			if (withinAParentForm && window == null) CancelButton.IsEnabled = false;
			Window = window;

			RemoveAllDropDown.Foreground = (SolidColorBrush)FindResource("NormalText");
			RemoveAllDropDown.Background = (SolidColorBrush)FindResource("NormalBackground");

			ReloadQuickPanel();
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

			timer.Interval = 10;
			timer.Tick += new EventHandler(UpdateToolstrip);
			timer.Start();

			scenesTree.Visible = true;

			if (Settings.MySettings.NightMode)
			{
				scenesTree.BackColor = EditorTheming.darkTheme1;
				scenesTree.ForeColor = EditorTheming.darkTheme3;

				recentDataDirList.BackColor = EditorTheming.darkTheme1;
				recentDataDirList.ForeColor = EditorTheming.darkTheme3;
			}


		}

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
			this.scenesTree = new System.Windows.Forms.TreeView();
			this.recentDataDirList = new System.Windows.Forms.TreeView();

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
			this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);										
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
			this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
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
			this.deleteSceneInfoToolStripMenuItem.Click += new System.EventHandler(this.deleteSceneInfoToolStripMenuItem_Click);
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
			this.EditDataPacksToolStripMenuItems.Click += new System.EventHandler(this.EditDataPacksMenuItem_Click);
			// 
			// scenesTree
			// 
			this.scenesTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.scenesTree.BackColor = System.Drawing.Color.White;
			this.scenesTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.scenesTree.ForeColor = System.Drawing.Color.Black;
			this.scenesTree.Name = "scenesTree";
			this.scenesTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scenesTree.TabIndex = 0;
			this.scenesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.scenesTree_AfterSelect);
			this.scenesTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.scenesTree_NodeMouseClick);
			this.scenesTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.scenesTree_NodeMouseDoubleClick);
			this.scenesTree.Click += new System.EventHandler(this.scenesTree_Click);
			this.scenesTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.scenesTree_MouseUp);
			this.scenesTree.HideSelection = false;
			// 
			// recentDataDirList
			// 
			this.recentDataDirList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.recentDataDirList.BackColor = System.Drawing.Color.White;
			this.recentDataDirList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.recentDataDirList.Name = "recentDataDirList";
			this.recentDataDirList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.recentDataDirList.TabIndex = 13;
			this.recentDataDirList.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.recentDataDirList_Click);
			this.recentDataDirList.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.recentDataDirList_NodeMouseDoubleClick);
			this.recentDataDirList.HideSelection = false;


			scenesTree.Show();
			recentDataDirList.Show();
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

					List<Tuple<string, Tuple<GameConfig.SceneInfo, string>>> files;
					if (!Directories.TryGetValue(scene.Zone, out files))
					{
						files = new List<Tuple<string, Tuple<GameConfig.SceneInfo, string>>>();
						Directories[scene.Zone] = files;
					}
					files.Add(new Tuple<string, Tuple<GameConfig.SceneInfo, string>>("Scene" + scene.SceneID + ".bin" + (scene.ModeFilter == 5 ? " (Encore)" : ""), new Tuple<GameConfig.SceneInfo, string>(scene, scene.Zone + "\\Scene" + scene.SceneID + ".bin")));
				}
				Categories.Add(new Tuple<string, List<Tuple<string, string>>>(category.Name, scenes));
			}

			// Sort
			Directories = Directories.OrderBy(key => key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

			this.scenesTree.ImageList = new ImageList();
			this.scenesTree.ImageList.Images.Add("Folder", Properties.Resources.folder);
			this.scenesTree.ImageList.Images.Add("File", Properties.Resources.file);

			UpdateTree();
			if (Settings.MyDefaults.SceneSelectFilesViewDefault)
			{
				this.isFilesView.IsChecked = true;
			}
			else
			{
				this.isFilesView.IsChecked = false;
			}
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
			if (EditorInstance == null) return;

			if (EditorInstance.DataDirectory != null)
			{
				dataLabelToolStripItem.Content = "Data Directory: " + EditorInstance.DataDirectory;
			}
			else
			{
				dataLabelToolStripItem.Content = "Data Directory: NULL";
			}

			recentDataDirList.Nodes.Clear();
			recentDataDirList.Nodes.Add("Recent Data Directories");
			recentDataDirList.Nodes.Add("Saved Places");
			recentDataDirList.Nodes.Add("Data Packs");
			this.recentDataDirList.ImageList = new ImageList();
			this.recentDataDirList.ImageList.Images.Add("Folder", Properties.Resources.folder);
			this.recentDataDirList.ImageList.Images.Add("File", Properties.Resources.file);

			foreach (System.Windows.Controls.MenuItem dataDir in EditorInstance.RecentDataItemsMenu)
			{
				var node = recentDataDirList.Nodes[0].Nodes.Add(dataDir.Tag.ToString());
				node.Tag = dataDir.Tag.ToString();
				node.ToolTipText = dataDir.Tag.ToString();
				node.ImageKey = "DataFolder";
			}

			recentDataDirList.Nodes[0].ExpandAll();

			if (Properties.Settings.Default.SavedPlaces?.Count > 0)
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
					node.ImageKey = "SavedPlace";
				}
				recentDataDirList.Nodes[1].ExpandAll();
			}

			if (EditorInstance.DataPacks != null && EditorInstance.DataPacks.ModListInformation?.Count > 0)
			{
                List<string> modPacks = new List<string>();
                this.recentDataDirList.ImageList.Images.Add("SubFolder", Properties.Resources.folder);
                int index = this.recentDataDirList.ImageList.Images.IndexOfKey("SubFolder");
                modPacks = EditorInstance.DataPacks.DataPackNamesToList();
                foreach (string packs in modPacks)
                {
                    var node = recentDataDirList.Nodes[2].Nodes.Add(packs, packs, index, index);
                    node.Tag = modPacks.IndexOf(packs);
                    node.ToolTipText = packs;
                    node.ImageKey = "DataPack";
                }
                recentDataDirList.Nodes[2].ExpandAll();
            }

		}

		private void selectButton_Click(object sender, EventArgs e)
		{
			EditorInstance.UIModes.LevelID = LevelID;
			if (!isFilesView.IsChecked.Value)
			{
				Result = scenesTree.SelectedNode.Tag as string;
			}
			else
			{
				Tuple<GameConfig.SceneInfo, string> tag = scenesTree.SelectedNode.Tag as Tuple<GameConfig.SceneInfo, string>;
				if (tag != null)
				{
					Result = tag.Item2;
				}
				else return;
			}

			Close();

		}

		private void Close()
		{
            if (!EditorInstance.UIModes.isImportingObjects && withinAParentForm)
            {
                EditorInstance.FileHandler.OpenSceneUsingExistingSceneSelect(this);
            }
			if (Window != null) Window.Close();

		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
			if (withinAParentForm) EditorInstance.UpdateStartScreen(true);
		}

		private void UpdateTree()
		{
			Show(FilterText.Text);
		}

		private void Show(string filter)
		{
			scenesTree.Nodes.Clear();
			if ((bool)isFilesView.IsChecked)
			{
				foreach (KeyValuePair<string, List<Tuple<string, Tuple<GameConfig.SceneInfo, string>>>> directory in Directories)
				{
					TreeNode dir_node = new TreeNode(directory.Key);
					dir_node.ImageKey = "Folder";
					dir_node.SelectedImageKey = "Folder";
					dir_node.ContextMenuStrip = contextMenuStrip1;
					foreach (Tuple<string, Tuple<GameConfig.SceneInfo, string>> file in directory.Value)
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
			browse.IsEnabled = true;
			if (filter != "")
			{
				scenesTree.ExpandAll();
			}
		}

		private void isFilesView_CheckedChanged(object sender, RoutedEventArgs e)
		{
			UpdateTree();
		}

		private void FilterText_TextChanged(object sender, RoutedEventArgs e)
		{
			UpdateTree();
		}

		private void scenesTree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			selectButton.IsEnabled = scenesTree.SelectedNode.Tag != null;
			UpdateCurrentInfo();
		}

		private void scenesTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (selectButton.IsEnabled)
			{
				UpdateCurrentInfo();
				selectButton_Click(sender, e);
			}
		}

		private void scenesTree_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (scenesTree.SelectedNode == null)
			{
				selectButton.IsEnabled = false;
			}
			if (_GameConfig != null) browse.IsEnabled = true;
			else browse.IsEnabled = true;
			UpdateCurrentInfo();
		}

		public void UpdateCurrentInfo()
		{

			if (scenesTree.SelectedNode != null)
			{
				if (scenesTree.SelectedNode.Parent != null)
				{
					if (isFilesView.IsChecked.Value)
					{
						Tuple<GameConfig.SceneInfo, string> tag = scenesTree.SelectedNode.Tag as Tuple<GameConfig.SceneInfo, string>;
						if (tag != null)
						{
							GameConfig.SceneInfo scene = tag.Item1;
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
						var cat = _GameConfig.Categories.Where(t => t.Name == scenesTree.SelectedNode.Parent.Text).FirstOrDefault();
						if (cat != null)
						{
							var scene = cat.Scenes.Where(t => t.Index == scenesTree.SelectedNode.Index).FirstOrDefault();
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

		private void browse_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
			open.Filter = "Scene File|*.bin";
			if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
			{
				Result = open.FileName;
				Browsed = true;
				Close();
			}
		}

		private void selectable_browse_Click(string initialDir)
		{
			System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
			open.InitialDirectory = initialDir;
			open.Filter = "Scene File|*.bin";
			if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
			{
				Result = open.FileName;             
				Browsed = true;
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
			UpdateCurrentInfo();
		}

		private void addToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var form = new EditSceneSelectInfoWindow();
			if (Window != null)
			{
				form.Owner = System.Windows.Window.GetWindow(Window);
			}
			else if (withinAParentForm)
			{
				form.Owner = System.Windows.Window.GetWindow(EditorInstance.StartScreen);
			}
			form.ShowDialog();
			if (form.DialogResult == true)
			{
				var cat = _GameConfig.Categories.Where(t => t.Name == scenesTree.SelectedNode.Text).FirstOrDefault();
				if (cat != null)
				{
					cat.Scenes.Add(form.Scene);
					LoadFromGameConfig(_GameConfig);
					if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
						_GameConfig.Write(EditorInstance.Paths.GameConfig_Source);
					ReloadGameConfig();

				}
			}
		}

		private void addCategoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var form = new EditSceneSelectInfoWindow();
			if (Window != null)
			{
				form.Owner = System.Windows.Window.GetWindow(Window);
			}
			else if (withinAParentForm)
			{
				form.Owner = System.Windows.Window.GetWindow(EditorInstance.StartScreen);
			}
			form.ShowDialog();
			if (form.DialogResult == true)
			{
				var scenes = new List<RSDKv5.GameConfig.SceneInfo>();
				scenes.Add(form.Scene);

				var form2 = new SceneSelectEditCategoryLabelWindow();
				form.Owner = System.Windows.Window.GetWindow(Window);
				form2.Scenes = scenes;
				form2.ShowDialog();

				if (form2.DialogResult == true)
				{
					if (form2.Category != null)
					{
						_GameConfig.Categories.Insert(scenesTree.SelectedNode.Index, form2.Category);
						LoadFromGameConfig(_GameConfig);

						if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
							_GameConfig.Write(EditorInstance.Paths.GameConfig_Source);
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
				var form = new EditSceneSelectInfoWindow(scene);
				if (Window != null)
				{
					form.Owner = System.Windows.Window.GetWindow(Window);
				}
				else if (withinAParentForm)
				{
					form.Owner = System.Windows.Window.GetWindow(EditorInstance.StartScreen);
				}
				form.ShowDialog();
				if (form.DialogResult == true)
				{
					LoadFromGameConfig(_GameConfig);
					if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
						_GameConfig.Write(EditorInstance.Paths.GameConfig_Source);
					ReloadGameConfig();
				}
			}
		}

		private void editCategoryMenuItem_Click(object sender, EventArgs e)
		{
			var Category = _GameConfig.Categories[scenesTree.SelectedNode.Index];
			var form = new SceneSelectEditCategoryLabelWindow(Category, Category.Scenes);
			if (Window != null)
			{
				form.Owner = System.Windows.Window.GetWindow(Window);
			}
			else if (withinAParentForm)
			{
				form.Owner = System.Windows.Window.GetWindow(EditorInstance.StartScreen);
			}
			form.ShowDialog();
			if (form.DialogResult == true)
			{
				LoadFromGameConfig(_GameConfig);
				if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					_GameConfig.Write(EditorInstance.Paths.GameConfig_Source);
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
						"Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
						return;
					cat.Scenes[scene + 1].Name = cat.Scenes[scene].Name.
						Replace(" " + cat.Scenes[scene].SceneID, " " + cat.Scenes[scene + 1].SceneID);
				}
				cat.Scenes.RemoveAt(scene);
				LoadFromGameConfig(_GameConfig);
				if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					_GameConfig.Write(EditorInstance.Paths.GameConfig_Source);
				ReloadGameConfig();
			}
		}

		private void deleteCategoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_GameConfig.Categories.RemoveAt(scenesTree.SelectedNode.Index);
			LoadFromGameConfig(_GameConfig);
			if (MessageBox.Show("Write Changes to File? Please make sure you didn't delete something on accident!", "Write to File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				_GameConfig.Write(EditorInstance.Paths.GameConfig_Source);
			ReloadGameConfig();
		}

		private void load_Click(object sender, RoutedEventArgs e)
		{
            if (EditorInstance.UIModes.isImportingObjects == true)
            {
                MessageBox.Show("You can't do that while importing objects!");
            }
            else
            {
                if (recentDataDirList.SelectedNode.ImageKey == "DataFolder")
                {
                    UnloadDataPack();
                    LoadDataDirectory(recentDataDirList.SelectedNode.Tag.ToString());
                }
                else if (recentDataDirList.SelectedNode.ImageKey == "DataPack")
                {
                    UnloadDataPack();
                    LoadDataPack(recentDataDirList.SelectedNode.Tag.ToString());
                }
                else if (recentDataDirList.SelectedNode.ImageKey == "SavedPlace")
                {
                    if (_GameConfig != null) selectable_browse_Click(recentDataDirList.SelectedNode.Tag.ToString());
                    else MessageBox.Show("Please Select/Open a Data Directory First", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

		private void LoadDataDirectory(string dataDirectory)
		{
            EditorInstance.DataDirectory = dataDirectory;

            bool AllowedToProceed = true;

			if (EditorInstance.DataDirectory != null) dataLabelToolStripItem.Content = "Data Directory: " + EditorInstance.DataDirectory;
			else
			{
				dataLabelToolStripItem.Content = "Data Directory: NULL";
                AllowedToProceed = false;
            }

            if (AllowedToProceed)
			{
				GameConfig GameConfig = null;
				String SelectedDataDirectory;
                SelectedDataDirectory = recentDataDirList.SelectedNode.Tag.ToString();
                GameConfig = EditorInstance.Paths.SetandReturnGameConfig(SelectedDataDirectory);
				if (GameConfig != null)
				{
					LoadFromGameConfig(GameConfig);
					_GameConfig = GameConfig;
				}
                else
                {
                    dataLabelToolStripItem.Content = "Data Directory: NULL";
                }
			}

        }

		private void setButtonEnabledState(bool enabled)
		{
			this.browse.IsEnabled = enabled;
			this.selectButton.IsEnabled = enabled;
			this.FilterText.IsEnabled = enabled;
			this.scenesTree.Enabled = enabled;
			this.isFilesView.IsEnabled = enabled;
		}

		private void addButton_Click(object sender, RoutedEventArgs e)
		{
			dataDirectoryToolStripMenuItem_Click(null, null);
		}

		private void ReloadGameConfig()
		{
			_GameConfig.Write(Path.Combine(Environment.CurrentDirectory, "GameConfig_Temp.bin"));
			_GameConfig = new GameConfig(Path.Combine(Environment.CurrentDirectory, "GameConfig_Temp.bin"));
			File.Delete(Path.Combine(Environment.CurrentDirectory, "GameConfig_Temp.bin"));
			LoadFromGameConfig(_GameConfig);
		}

		private void dataDirectoryToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (EditorInstance.UIModes.isImportingObjects == false)
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
				recentDataDirList.SelectedNode = e.Node;
				if (e.Node.ImageKey == "DataFolder")
					dataDirEditContext.Show(recentDataDirList, e.Location);
				else if (e.Node.ImageKey == "SavedPlace")
					folderEditContext.Show(recentDataDirList, e.Location);
				else if (e.Node.ImageKey == "DataPack")
					DataPackContextMenu.Show(recentDataDirList, e.Location);
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
			if (Settings.MySettings.SavedPlaces.Contains(toRemove))
			{
				Settings.MySettings.SavedPlaces.Remove(toRemove);
			}
			ReloadQuickPanel();
		}

		private void removeDataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			String toRemove = recentDataDirList.SelectedNode.Tag.ToString();
			if (Settings.MySettings.DataDirectories.Contains(toRemove))
			{
				Settings.MySettings.DataDirectories.Remove(toRemove);
			}
			EditorInstance.RefreshDataDirectories(Properties.Settings.Default.DataDirectories);
			ReloadQuickPanel();
		}

        public void UnloadDataPack()
        {
            EditorInstance.ResourcePackList.Clear();
            EditorInstance.UIModes.DataDirectoryReadOnlyMode = false;
            EditorInstance.DataDirectory = null;
            dataPackStatusLabel.Content = "";
            EditorInstance.DataDirectory = "Data Directory: NULL";
        }

        public void LoadDataPack(string tag)
        {
            bool AllowedToProceed = true;

            prevDataDir = EditorInstance.DataDirectory;
            if (EditorInstance.DataPacks == null) return;
            if (!Int32.TryParse(tag, out int Index)) return;
            if (EditorInstance.DataPacks.ModListInformation == null) return;

            var pack = EditorInstance.DataPacks.ModListInformation[Index];

            EditorInstance.LoadedDataPack = pack.Item1;

            foreach(var item in pack.Item2)
            {
                if (item.Item1 == "DataDir") EditorInstance.DataDirectory = item.Item2;
                else if (item.Item1 == "Mod") EditorInstance.ResourcePackList.Add(item.Item2);
                else if (item.Item1 == "ReadOnlyDataFolder" && item.Item2 == "TRUE") EditorInstance.UIModes.DataDirectoryReadOnlyMode = true;
            }

            GameConfig GameConfig = null;
            GameConfig = EditorInstance.Paths.SetandReturnGameConfig();

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
                dataLabelToolStripItem.Content = "Data Directory: NULL";
                dataPackStatusLabel.Content = "";
            }
        }


		private void recentDataDirList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
                recentDataDirList.SelectedNode = e.Node;
                load_Click(sender, null);
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
			if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				_GameConfig.Write(EditorInstance.Paths.GameConfig_Source);
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
			if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				_GameConfig.Write(EditorInstance.Paths.GameConfig_Source);
			ReloadGameConfig();
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			if (isFileViewMode)
			{
				contextMenuStrip1.Enabled = false;
			}
			else
			{
				contextMenuStrip1.Enabled = true;
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


		}

		private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
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
				if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					_GameConfig.Write(EditorInstance.Paths.GameConfig_Source);
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
				if (MessageBox.Show("Write Changes to File?", "Write to File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					_GameConfig.Write(EditorInstance.Paths.GameConfig_Source);
				ReloadGameConfig();
			}
		}

		private void clearDataDirectoriesToolStripMenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to do this? No undos here!", "Delete All Data Directories", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				if (Settings.MySettings.DataDirectories != null)
				{
					Settings.MySettings.DataDirectories.Clear();
					EditorInstance.RefreshDataDirectories(Properties.Settings.Default.DataDirectories);
					ReloadQuickPanel();
				}


			}

		}

		private void removeAllSavedPlacesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to do this? No undos here!", "Delete All Saved Places", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				if (Settings.MySettings.SavedPlaces != null)
				{
					Settings.MySettings.SavedPlaces.Clear();
					ReloadQuickPanel();
				}
			}
		}



		private void removeModToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void removeAllModsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{

		}

		private void EditDataPacksMenuItem_Click(object sender, EventArgs e)
		{
            ModPackEditorButton_Click(null, null);
		}

		private void toolStripDropDownButton1_Click(object sender, RoutedEventArgs e)
		{
			LoadDataDirectory(recentDataDirList.SelectedNode.Tag.ToString());
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var host = new System.Windows.Forms.Integration.WindowsFormsHost();
			host.Child = this.scenesTree;
			this.scenesTreeHost.Children.Add(host);
			var host2 = new System.Windows.Forms.Integration.WindowsFormsHost();
			host2.Child = this.recentDataDirList;
			this.recentDataDirListHost.Children.Add(host2);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (Window != null) Window.DialogResult = false;
		}

		private void OptionsButton_Click(object sender, RoutedEventArgs e)
		{
			optionsButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
			optionsButton.ContextMenu.IsOpen = true;
		}

		private void UserControl_GotFocus(object sender, RoutedEventArgs e)
		{
			ReloadQuickPanel();
		}

        private void ModPackEditorButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackEditor editor = new DataPackEditor(EditorInstance);
            if (Window != null) editor.Owner = Window.Owner;
            else editor.Owner = Application.Current.MainWindow;
            editor.ShowDialog();
            ReloadQuickPanel();
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
