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

        public StartupInformation(Editor instance)
        {
            EditorInstance = instance;
            InitializeComponent();
        }


        private void StartupInformation_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.NeverShowThisAgain)
            {
                panel1.Controls.Clear();
            }

            if (!Properties.Settings.Default.ShowFirstTimeSetup)
            {
                panel2.Controls.Clear();
            }

            if (Settings.mySettings.DataDirectories?.Count > 0)
            {
                LoadRecentDataDirectories(Settings.mySettings.DataDirectories);
            }
        }

        private void LoadRecentDataDirectories(StringCollection recentDataDirectories)
        {
            listView1.Items.Clear();
            listView1.SmallImageList = new ImageList();
            listView1.SmallImageList.Images.Add("Folder", Properties.Resources.folder);
            foreach (var dataDirectory in recentDataDirectories)
            {
                listView1.Items.Add(CreateDataDirectoryListLink(dataDirectory));
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
            //modeLabel.Text = "Minimal - The very lowest you can go if you are don't have enough power.";
        }

        private void radioButton2_MouseHover(object sender, EventArgs e)
        {
            //modeLabel.Text = "Basic - A step up from minimal. Take caution x86 users!";
        }

        private void radioButton3_MouseHover(object sender, EventArgs e)
        {
            //modeLabel.Text = "Super - The best feature set without killing it! NOT FOR x86 USERS!";
        }

        private void radioButton4_MouseHover(object sender, EventArgs e)
        {
            //modeLabel.Text = "Hyper - Kicking things into OVERDRIVE! NOT FOR x86 USERS";
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RecentDataFolderItemClicked(listView1.SelectedItems[0].Tag.ToString());
        }

        private void RecentDataFolderItemClicked(string dataDirectory)
        {
            var dataDirectories = Settings.mySettings.DataDirectories;
            Settings.mySettings.GamePath = Editor.GamePath;
            if (EditorInstance.IsDataDirectoryValid(dataDirectory))
            {
                EditorInstance.ResetDataDirectoryToAndResetScene(dataDirectory);
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
    }
}
