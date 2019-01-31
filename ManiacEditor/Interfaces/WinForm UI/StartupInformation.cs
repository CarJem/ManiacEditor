using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManiacEditor
{
    public partial class StartupInformation : Form
    {
        bool updateAvaliable = false;
        bool sourceBuild = false;
        public Editor EditorInstance;

        public string SelectedSavedPlace = "";
        public string SelectedModFolder  = "";

        public StartupInformation(Editor instance)
        {
            EditorInstance = instance;
            InitializeComponent();
            if (Settings.mySettings.NightMode)
            {
                linkLabel1.LinkColor = Editor.darkTheme4;
                linkLabel2.LinkColor = Editor.darkTheme4;
                linkLabel3.LinkColor = Editor.darkTheme4;
                linkLabel4.LinkColor = Editor.darkTheme4;
                linkLabel5.LinkColor = Editor.darkTheme4;
                linkLabel6.LinkColor = Editor.darkTheme4;
                linkLabel7.LinkColor = Editor.darkTheme4;
                linkLabel8.LinkColor = Editor.darkTheme4;
                button1.ForeColor = Color.Black;
                button2.ForeColor = Color.Black;
                button4.ForeColor = Color.Black;
                treeView1.ForeColor = Editor.darkTheme3;
            }
			this.TopLevel = false;
		}


        private void StartupInformation_Load(object sender, EventArgs e)
        {
            Label comingSoon = new Label()
            {
                Text = "Functionality" + Environment.NewLine + "Coming Soon!",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            treeView1.Controls.Add(comingSoon);


            if (Properties.Settings.Default.NeverShowThisAgain)
            {
                panel1.Controls.Clear();
                panel1.Visible = false;
            }

            if (!Properties.Settings.Default.UseForcefulStartup)
            {
                linkLabel3.Visible = false;
                checkBox1.Visible = false;
            }

            if (Properties.Settings.Default.ShowFirstTimeSetup)
            {
                firstTimeSettingsPanel.Controls.Clear();
                firstTimeSettingsPanel.Visible = false;
            }

            if (Settings.mySettings.DataDirectories?.Count > 0)
            {
                LoadRecentDataDirectories(Settings.mySettings.DataDirectories);
            }
            UpdateLabel();
        }

        private void LoadRecentDataDirectories(StringCollection recentDataDirectories)
        {
            listView1.Items.Clear();
            listView1.SmallImageList = new ImageList();
            listView1.SmallImageList.Images.Add("Folder", Properties.Resources.folder);

            treeView1.Nodes.Clear();
            treeView1.Nodes.Add("Saved Places");
            treeView1.Nodes.Add("Mods");
            this.treeView1.ImageList = new ImageList();
            this.treeView1.ImageList.Images.Add("Folder", Properties.Resources.folder);
            this.treeView1.ImageList.Images.Add("File", Properties.Resources.file);

            foreach (var dataDirectory in recentDataDirectories)
            {
                listView1.Items.Add(CreateDataDirectoryListLink(dataDirectory));
            }

            if (Properties.Settings.Default.SavedPlaces?.Count > 0)
            {
                StringCollection recentFolders = new StringCollection();
                this.treeView1.ImageList.Images.Add("SubFolder", Properties.Resources.folder);
                int index = this.treeView1.ImageList.Images.IndexOfKey("SubFolder");
                recentFolders = Properties.Settings.Default.SavedPlaces;
                foreach (string folder in recentFolders)
                {
                    var node = treeView1.Nodes[0].Nodes.Add(folder, folder, index, index);
                    node.Tag = folder;
                    node.ToolTipText = folder;
                    node.ImageKey = "SavedPlace";
                }
                treeView1.Nodes[0].ExpandAll();
            }

            if (Properties.Settings.Default.ModFolders?.Count > 0)
            {
                StringCollection modFolders = new StringCollection();
                StringCollection modFolderNames = new StringCollection();
                this.treeView1.ImageList.Images.Add("SubFolder", Properties.Resources.folder);
                int index = this.treeView1.ImageList.Images.IndexOfKey("SubFolder");
                modFolders = Properties.Settings.Default.ModFolders;
                modFolderNames = Properties.Settings.Default.ModFolderCustomNames;
                foreach (string folder in modFolders)
                {
                    int nameIndex = modFolders.IndexOf(folder);
                    string title;
                    if (modFolderNames[nameIndex].Equals(""))
                    {
                        title = folder;
                    }
                    else
                    {
                        title = modFolderNames[nameIndex];
                    }

                    var node = treeView1.Nodes[1].Nodes.Add(folder, title, index, index);
                    node.Tag = folder;
                    node.ToolTipText = folder;
                    node.ImageKey = "ModFolder";
                }
                treeView1.Nodes[1].ExpandAll();
            }

        }

        private ListViewItem CreateDataDirectoryListLink(string target)
        {
            ListViewItem newItem = new ListViewItem(target)
            {
                Tag = target,
                ImageKey = "Folder"
            };
            return newItem;
        }

        public void UpdateStatusLabel(int condition, EditorUpdater updater)
        {
            switch (condition)
            {
                case 0:
                    updateAvaliable = false;
                    break;
                case 1:
                    updateAvaliable = true;
                    break;
                case 2:
                    updateAvaliable = false;
                    sourceBuild = true;
                    break;
                default:
                    updateAvaliable = false;
                    break;
            }
            if (updateAvaliable)
            {
                this.updateInfoLabel.Text = "An Update is Avaliable!" + Environment.NewLine + Environment.NewLine + string.Format("Local Version: {0}", updater.GetVersion()) + Environment.NewLine + string.Format("Current Version: {0}", updater.GetCurrentVersion()) + Environment.NewLine + Environment.NewLine + string.Format("Latest Version Details: {0}", updater.GetBuildMessage());
            }
            else
            {
                if (sourceBuild)
                {
                    this.updateInfoLabel.Text = "Your using the source. No need to update!" + Environment.NewLine + Environment.NewLine + string.Format("Local Version: {0}", updater.GetVersion()) + Environment.NewLine + string.Format("Current Version: {0}", updater.GetCurrentVersion()) + Environment.NewLine + Environment.NewLine + string.Format("Latest Version Details: {0}", updater.GetBuildMessage());
                }
                else
                {
                    this.updateInfoLabel.Text = "No Updates Found!" + Environment.NewLine + Environment.NewLine + string.Format("Local Version: {0}", updater.GetVersion()) + Environment.NewLine + string.Format("Current Version: {0}", updater.GetCurrentVersion()) + Environment.NewLine + Environment.NewLine + string.Format("Latest Version Details: {0}", updater.GetBuildMessage());
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/CarJem/ManiacEditor-GenerationsEdition/releases");
            Process.Start(sInfo);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://ci.appveyor.com/project/CarJem/maniaceditor-generationsedition");
            Process.Start(sInfo);
        }

        private void radioButton1_MouseHover(object sender, EventArgs e)
        {
            modeLabel.Text = "Minimal - The very lowest you can go if you are don't have enough power.";
        }

        private void radioButton2_MouseHover(object sender, EventArgs e)
        {
            modeLabel.Text = "Basic - A step up from minimal. Take caution x86 users!";
        }

        private void radioButton3_MouseHover(object sender, EventArgs e)
        {
            modeLabel.Text = "Super - The best feature set without killing it! NOT FOR x86 USERS!";
        }

        private void radioButton4_MouseHover(object sender, EventArgs e)
        {
            modeLabel.Text = "Hyper - Kicking things into OVERDRIVE! NOT FOR x86 USERS";
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RecentDataFolderItemClicked(listView1.SelectedItems[0].Tag.ToString());
        }

        private void RecentDataFolderItemClicked(string dataDirectory, bool forceBrowse = false, bool forceSceneSelect = false)
        {
            var dataDirectories = Settings.mySettings.DataDirectories;
            Settings.mySettings.GamePath = EditorInstance.GamePath;
            if (EditorInstance.IsDataDirectoryValid(dataDirectory))
            {
                EditorInstance.ResetDataDirectoryToAndResetScene(dataDirectory, forceBrowse, forceSceneSelect);
            }
            else
            {
                MessageBox.Show($"The specified Data Directory {dataDirectory} is not valid.",
                                "Invalid Data Directory!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                dataDirectories.Remove(dataDirectory);
                EditorInstance.RefreshDataDirectories(dataDirectories);
                LoadRecentDataDirectories(dataDirectories);

            }
            Settings.mySettings.Save();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            panel1.Visible = false;
        }

        private void dontCareOption_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void setPresetsButton_Click(object sender, EventArgs e)
        {
            if (minimalOption.Checked) EditorSettings.ApplyPreset(0);
            else if (basicOption.Checked) EditorSettings.ApplyPreset(1);
            else if (superOption.Checked) EditorSettings.ApplyPreset(2);
            else if (hyperOption.Checked) EditorSettings.ApplyPreset(3);

            firstTimeSettingsPanel.Controls.Clear();
            firstTimeSettingsPanel.Visible = false;
            Settings.mySettings.ShowFirstTimeSetup = false;
        }

        public void ApplyPreset(int type)
        {

        }

        private void minimalOption_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if (minimalOption.Checked) EditorSettings.ApplyPreset(0);
            else if (basicOption.Checked) EditorSettings.ApplyPreset(1);
            else if (superOption.Checked) EditorSettings.ApplyPreset(2);
            else if (hyperOption.Checked) EditorSettings.ApplyPreset(3);

            if (EditorSettings.isMinimalPreset()) minimalOption.ForeColor = Color.Red;
            else minimalOption.ForeColor = Color.White;

            if (EditorSettings.isBasicPreset()) basicOption.ForeColor = Color.Red;
            else basicOption.ForeColor = Color.White;

            if (EditorSettings.isSuperPreset()) superOption.ForeColor = Color.Red;
            else superOption.ForeColor = Color.White;

            if (EditorSettings.isHyperPreset()) hyperOption.ForeColor = Color.Red;
            else hyperOption.ForeColor = Color.White;
            */
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EditorInstance.OpenSceneForceFully();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0) RecentDataFolderItemClicked(listView1.SelectedItems[0].Tag.ToString(), false, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0) RecentDataFolderItemClicked(listView1.SelectedItems[0].Tag.ToString(), true, false);
        }

        private void treeView1_DoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.ImageKey == "SavedPlace") UpdateSavedPlace(e.Node.Tag.ToString());
            else if (e.Node != null && e.Node.ImageKey == "ModFolder") UpdateModFolder(e.Node.Tag.ToString());
        }

        private void UpdateSavedPlace(string tag)
        {
            SelectedSavedPlace = tag;
            UpdateLabel();
        }

        private void UpdateModFolder(string tag)
        {
            SelectedModFolder = tag;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            dataDirInfoLabel.Text = "Selected Saved Place: " + (SelectedSavedPlace != "" ? SelectedSavedPlace : "N/A") + Environment.NewLine + "Selected Mod: " + (SelectedModFolder != "" ? SelectedModFolder : "N/A");
        }

        private void treeView1_Layout(object sender, LayoutEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            SelectedModFolder = "";
            SelectedSavedPlace = "";
            UpdateLabel();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EditorInstance.AboutToolStripMenuItem_Click(null, null);
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EditorInstance.OptionToolStripMenuItem_Click(null, null);
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.google.com/document/d/1NBvcqzvOzqeTVzgAYBR0ttAc5vLoFaQ4yh_cdf-7ceQ/edit?usp=sharing");
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/CarJem/ManiacEditor-GenerationsEdition");
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://ci.appveyor.com/project/CarJem/maniaceditor-generationsedition");
        }
    }
}
