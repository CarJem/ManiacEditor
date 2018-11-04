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
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace ManiacEditor
{

    public partial class ConfigManager : Form
    {
        public ConfigManager()
        {
            InitializeComponent();
            getPaths();
            InitList();
        }

        public void getPaths()
        {
            try
            {
                if (Properties.Settings.Default.modConfigs != null && Properties.Settings.Default.modConfigsNames != null)
                {
                    Properties.Settings.Default.modConfigs.Clear();
                    Properties.Settings.Default.modConfigsNames.Clear();
                }
                string[] filePaths = Directory.GetFiles(Path.GetFullPath(Environment.CurrentDirectory + "\\Config\\"), "*.ini", SearchOption.TopDirectoryOnly);
                if (filePaths != null)
                {
                    foreach (string file in filePaths)
                    {
                        string config = File.ReadAllText(file);
                        string fileName = file.Substring(file.LastIndexOf("\\") + 1);
                        if (Properties.Settings.Default.modConfigs == null)
                        {
                            Properties.Settings.Default.modConfigs = new StringCollection();
                        }
                        addModConfig(config);
                        addModConfigName(fileName);
                    }
                }
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                return;
            }




        }

        public void addModConfig(string config)
        {
            try
            {
                var mySettings = Properties.Settings.Default;
                var modConfigs = mySettings.modConfigs;

                if (modConfigs == null)
                {
                    modConfigs = new StringCollection();
                    mySettings.modConfigs = modConfigs;
                }

               modConfigs.Insert(0, config);

                mySettings.Save();

            }
            catch (Exception ex)
            {
                Debug.Write("Failed to add config file to list" + ex);
            }
        }

        public void addModConfigName(string config)
        {
            try
            {
                var mySettings2 = Properties.Settings.Default;
                var modConfigNames = mySettings2.modConfigsNames;

                if (modConfigNames == null)
                {
                    modConfigNames = new StringCollection();
                    mySettings2.modConfigsNames = modConfigNames;
                }

                modConfigNames.Insert(0, config);

                mySettings2.Save();
        }
            catch (Exception ex)
            {
                Debug.Write("Failed to add config file name to list" + ex);
            }
}

        public void InitList()
        {
            try
            {
                foreach (String s in Properties.Settings.Default.modConfigsNames)
                {
                    listView1.Items.Add(s);
                }
            }
            catch (System.NullReferenceException)
            {
                return;
            }

        }

        private void addButton_click(object sender, EventArgs e)
        {
            ModConfigCreator creator = new ModConfigCreator();
            creator.ShowDialog();
            if (creator.DialogResult == DialogResult.OK)
            {
                    listView1.Items.Clear();
                    getPaths();
                    InitList();
                    Properties.EditorState.Default.addConfig = false;
            }

        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            string nameToRemove = listView1.SelectedItems[0].Text;
            ListViewItem toRemove = listView1.SelectedItems[0];
            int position = listView1.Items.IndexOf(toRemove);

            File.Delete(Environment.CurrentDirectory + "\\Config\\" + nameToRemove);
            listView1.Items.Clear();
            Properties.Settings.Default.modConfigsNames.RemoveAt(position);
            Properties.Settings.Default.modConfigs.RemoveAt(position);
            getPaths();
            InitList();

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                removeButton.Enabled = true;
                editButton.Enabled = true;
            }
            else
            {
                removeButton.Enabled = false;
                editButton.Enabled = false;
            }
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                removeButton.Enabled = true;
                editButton.Enabled = true;
            }
            else
            {
                removeButton.Enabled = false;
                editButton.Enabled = false;
            }
        }

        private void reloadButton_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            getPaths();
            InitList();
        }

        private void ConfigManager_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            string nameToRemove = listView1.SelectedItems[0].Text;
            ListViewItem toRemove = listView1.SelectedItems[0];
            int position = listView1.Items.IndexOf(toRemove);

            ModConfigCreator creator = new ModConfigCreator(true, position);
            creator.ShowDialog();
            if (creator.DialogResult == DialogResult.OK)
            {
                listView1.Items.Clear();
                getPaths();
                InitList();
                Properties.EditorState.Default.addConfig = false;
            }
        }
    }
}
