using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Path = System.IO.Path;
using System.Collections.Specialized;
using System.Diagnostics;




namespace ManiacEditor.Controls.Utility.Editor.Configuration
{
    /// <summary>
    /// Interaction logic for ConfigManager.xaml
    /// </summary>
    public partial class ConfigManager : Window
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
				if (Properties.Settings.MySettings.ModLoaderConfigs != null && Properties.Settings.MySettings.ModLoaderConfigsNames != null)
				{
					Properties.Settings.MySettings.ModLoaderConfigs.Clear();
					Properties.Settings.MySettings.ModLoaderConfigsNames.Clear();
				}
				string[] filePaths = Directory.GetFiles(Path.GetFullPath(Environment.CurrentDirectory + "\\Config\\"), "*.ini", SearchOption.TopDirectoryOnly);
				if (filePaths != null)
				{
					foreach (string file in filePaths)
					{
						string config = File.ReadAllText(file);
						string fileName = file.Substring(file.LastIndexOf("\\") + 1);
						if (Properties.Settings.MySettings.ModLoaderConfigs == null)
						{
							Properties.Settings.MySettings.ModLoaderConfigs = new StringCollection();
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
				var mySettings = Properties.Settings.MySettings;
				var modConfigs = mySettings.ModLoaderConfigs;

				if (modConfigs == null)
				{
					modConfigs = new StringCollection();
					mySettings.ModLoaderConfigs = modConfigs;
				}

				modConfigs.Insert(0, config);

				Classes.Options.GeneralSettings.Save();

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
				var mySettings2 = Properties.Settings.MySettings;
				var modConfigNames = mySettings2.ModLoaderConfigsNames;

				if (modConfigNames == null)
				{
					modConfigNames = new StringCollection();
					mySettings2.ModLoaderConfigsNames = modConfigNames;
				}

				modConfigNames.Insert(0, config);


				Classes.Options.GeneralSettings.Save();
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
				foreach (String s in Properties.Settings.MySettings.ModLoaderConfigsNames)
				{
					Label configFile = new Label()
					{
						Content = s						
					};
					configFile.GotFocus += ConfigFile_Checked;
					listView1.Items.Add(configFile);
				}
			}
			catch (System.NullReferenceException)
			{
				return;
			}

		}

		private void addButton_click(object sender, RoutedEventArgs e)
		{
			ModConfigEditor creator = new ModConfigEditor();
			creator.ShowDialog();
			if (creator.DialogResult == true)
			{
				listView1.Items.Clear();
				getPaths();
				InitList();
			}

		}

		private void removeButton_Click(object sender, RoutedEventArgs e)
		{
			object toRemove = listView1.SelectedItems[0];
			Label item = toRemove as Label;
			if (item == null) return;
			string nameToRemove = item.Content.ToString();
			int position = listView1.Items.IndexOf(toRemove);

			File.Delete(Environment.CurrentDirectory + "\\Config\\" + nameToRemove);
			listView1.Items.Clear();
			Properties.Settings.MySettings.ModLoaderConfigsNames.RemoveAt(position);
			Properties.Settings.MySettings.ModLoaderConfigs.RemoveAt(position);
			getPaths();
			InitList();

		}

		private void ConfigFile_Checked(object sender, RoutedEventArgs e)
		{
			if (listView1.SelectedItem != null)
			{
				removeButton.IsEnabled = true;
				editButton.IsEnabled = true;
			}
			else
			{
				removeButton.IsEnabled = false;
				editButton.IsEnabled = false;
			}
		}

		private void listView1_ItemChecked(object sender, RoutedEventArgs e)
		{
			if (listView1.SelectedItem != null)
			{
				removeButton.IsEnabled = true;
				editButton.IsEnabled = true;
			}
			else
			{
				removeButton.IsEnabled = false;
				editButton.IsEnabled = false;
			}
		}

		private void reloadButton_Click(object sender, RoutedEventArgs e)
		{
			listView1.Items.Clear();
			getPaths();
			InitList();
		}

		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void editButton_Click(object sender, RoutedEventArgs e)
		{
			object toEdit = listView1.SelectedItems[0];
			int position = listView1.Items.IndexOf(toEdit);

			ModConfigEditor creator = new ModConfigEditor(true, position);
			creator.Owner = GetWindow(this);
			creator.ShowDialog();
			if (creator.DialogResult == true)
			{
				listView1.Items.Clear();
				getPaths();
				InitList();
			}
		}

		private void ListView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (listView1.SelectedItem != null)
			{
				removeButton.IsEnabled = true;
				editButton.IsEnabled = true;
			}
			else
			{
				removeButton.IsEnabled = false;
				editButton.IsEnabled = false;
			}
		}
	}
}
