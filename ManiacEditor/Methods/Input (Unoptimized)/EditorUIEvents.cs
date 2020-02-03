using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using ManiacEditor.Controls;
using System.Runtime.InteropServices;
using System.Windows.Controls;

using ManiacEditor.Controls.Utility;
using ManiacEditor.Controls.Utility.Editor;
using ManiacEditor.Controls.Utility.Object_ID_Repair_Tool;
using ManiacEditor.Controls.Utility.Object_Manager;
using ManiacEditor.Controls.Utility.Editor.Dev;
using ManiacEditor.Controls.Utility.Editor.Configuration;
using ManiacEditor.Controls.Utility.Editor.Options;

using ManiacEditor.Enums;


namespace ManiacEditor
{

    public class EditorUIEvents
	{
		private Controls.Base.MainEditor Editor;
        bool lockTextBox = false;

		public EditorUIEvents(Controls.Base.MainEditor instance)
		{
			Editor = instance;

		}



		#region View Tab Buttons
		public void SetMenuButtonType(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
			if (menuItem != null)
			{
				if (menuItem.Tag != null)
				{
                    string tag = menuItem.Tag.ToString();
                    var allItems = Editor.EditorMenuBar.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                    foreach (System.Windows.Controls.MenuItem item in allItems)
                    {
                        if (item.Tag == null || item.Tag.ToString() != menuItem.Tag.ToString()) item.IsChecked = false;
                        else if (item.Tag.ToString() == menuItem.Tag.ToString()) item.IsChecked = true;
                        var allSubItems = Editor.EditorMenuBar.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                        foreach (System.Windows.Controls.MenuItem subItem in allSubItems)
                        {
                            if (subItem.Tag == null || subItem.Tag.ToString() != menuItem.Tag.ToString()) subItem.IsChecked = false;
                            else if (subItem.Tag.ToString() == menuItem.Tag.ToString()) subItem.IsChecked = true;
                        }
					}            
					switch (tag)
					{
						case "Xbox":
							Classes.Editor.SolutionState.CurrentControllerButtons = 2;
							break;
						case "Switch":
							Classes.Editor.SolutionState.CurrentControllerButtons = 4;
							break;
						case "PS4":
							Classes.Editor.SolutionState.CurrentControllerButtons = 3;
							break;
						case "Saturn Black":
							Classes.Editor.SolutionState.CurrentControllerButtons = 5;
							break;
						case "Saturn White":
							Classes.Editor.SolutionState.CurrentControllerButtons = 6;
							break;
						case "Switch Joy L":
							Classes.Editor.SolutionState.CurrentControllerButtons = 7;
							break;
						case "Switch Joy R":
							Classes.Editor.SolutionState.CurrentControllerButtons = 8;
							break;
						case "PC EN/JP":
							Classes.Editor.SolutionState.CurrentControllerButtons = 1;
							break;
						case "PC FR":
							Classes.Editor.SolutionState.CurrentControllerButtons = 9;
							break;
						case "PC IT":
							Classes.Editor.SolutionState.CurrentControllerButtons = 10;
							break;
						case "PC GE":
							Classes.Editor.SolutionState.CurrentControllerButtons = 11;
							break;
						case "PC SP":
							Classes.Editor.SolutionState.CurrentControllerButtons = 12;
							break;
					}
                    menuItem.IsChecked = true;
				}

			}

		}
		public void SetMenuButtonType(string tag)
		{
			switch (tag)
			{
				case "Xbox":
					Classes.Editor.SolutionState.CurrentControllerButtons = 2;
					break;
				case "Switch":
					Classes.Editor.SolutionState.CurrentControllerButtons = 4;
					break;
				case "PS4":
					Classes.Editor.SolutionState.CurrentControllerButtons = 3;
					break;
				case "Saturn Black":
					Classes.Editor.SolutionState.CurrentControllerButtons = 5;
					break;
				case "Saturn White":
					Classes.Editor.SolutionState.CurrentControllerButtons = 6;
					break;
				case "Switch Joy L":
					Classes.Editor.SolutionState.CurrentControllerButtons = 7;
					break;
				case "Switch Joy R":
					Classes.Editor.SolutionState.CurrentControllerButtons = 8;
					break;
				case "PC EN/JP":
					Classes.Editor.SolutionState.CurrentControllerButtons = 1;
					break;
				case "PC FR":
					Classes.Editor.SolutionState.CurrentControllerButtons = 9;
					break;
				case "PC IT":
					Classes.Editor.SolutionState.CurrentControllerButtons = 10;
					break;
				case "PC GE":
					Classes.Editor.SolutionState.CurrentControllerButtons = 11;
					break;
				case "PC SP":
					Classes.Editor.SolutionState.CurrentControllerButtons = 12;
					break;
			}
		}
		public void SetEncorePallete(object sender = null, string path = "")
		{
			if (sender != null)
			{
				var clickedItem = sender as System.Windows.Controls.MenuItem;
				string StartDir = Editor.DataDirectory;
				try
				{
					using (var fd = new OpenFileDialog())
					{
						fd.Filter = "Color Palette File|*.act";
						fd.DefaultExt = ".act";
						fd.Title = "Select an Encore Color Palette";
						fd.InitialDirectory = Path.Combine(StartDir, "Palettes");
						if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							Editor.EncorePalette = Classes.Editor.Solution.CurrentScene.GetEncorePalette("", "", "", "", -1, fd.FileName);
							Classes.Editor.SolutionState.EncoreSetupType = 0;
							if (File.Exists(Editor.EncorePalette[0]))
							{
								Classes.Editor.SolutionState.EncorePaletteExists = true;
                                Classes.Editor.SolutionState.UseEncoreColors = true;
                            }

						}
					}
				}
				catch (Exception ex)
				{
					System.Windows.MessageBox.Show("Unable to set Encore Colors. " + ex.Message);
				}
			}
			else if (path != "")
			{
				Editor.EncorePalette = Classes.Editor.Solution.CurrentScene.GetEncorePalette("", "", "", "", -1, path);
				Classes.Editor.SolutionState.EncoreSetupType = 0;
				if (File.Exists(Editor.EncorePalette[0]))
				{
					Classes.Editor.SolutionState.EncorePaletteExists = true;
					Classes.Editor.SolutionState.UseEncoreColors = true;
				}
				else
				{
					System.Windows.MessageBox.Show("Unable to set Encore Colors. The Specified Path does not exist: " + Environment.NewLine + path);
				}
			}

		}
		public void EntityFilterTextChanged(object sender, TextChangedEventArgs e)
		{
            if (sender is System.Windows.Controls.TextBox && lockTextBox == false)
            {
                lockTextBox = true;
                System.Windows.Controls.TextBox theSender = sender as System.Windows.Controls.TextBox;
                Classes.Editor.SolutionState.entitiesTextFilter = theSender.Text;
                Editor.EditorMenuBar.toolStripTextBox1.Text = Classes.Editor.SolutionState.entitiesTextFilter;
                //Editor.toolStripTextBox2.Text = Editor.entitiesTextFilter;
                Classes.Editor.Solution.Entities.FilterRefreshNeeded = true;
                lockTextBox = false;
            }

            
		}
		public void SetScrollLockDirection()
		{
			if (Classes.Editor.SolutionState.ScrollDirection == (int)ScrollDir.X)
			{
				Classes.Editor.SolutionState.ScrollDirection = (int)ScrollDir.Y;
				Editor.EditorStatusBar.UpdateStatusPanel();
				Editor.EditorMenuBar.xToolStripMenuItem.IsChecked = false;
				Editor.EditorMenuBar.yToolStripMenuItem.IsChecked = true;
			}
			else
			{
				Classes.Editor.SolutionState.ScrollDirection = (int)ScrollDir.X;
				Editor.EditorStatusBar.UpdateStatusPanel();
				Editor.EditorMenuBar.xToolStripMenuItem.IsChecked = true;
				Editor.EditorMenuBar.yToolStripMenuItem.IsChecked = false;
			}
		}
		public void MenuLanguageChanged(object sender, RoutedEventArgs e)
		{
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            Classes.Editor.SolutionState.CurrentLanguage = menuItem.Tag.ToString();
            var allLangItems = Editor.EditorMenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
            {
                if (item.Tag.ToString() != menuItem.Tag.ToString()) item.IsChecked = false;
                else if (item.Tag.ToString() == menuItem.Tag.ToString()) item.IsChecked = true;
            }


		}

        #endregion

        #region Tools Tab Buttons
        public void ChangeLevelID(object sender, RoutedEventArgs e)
		{
            string inputValue = GenerationsLib.WPF.TextPrompt2.ShowDialog("Change Level ID", "This is only temporary and will reset when you reload the scene.", Classes.Editor.SolutionState.LevelID.ToString());
            int.TryParse(inputValue.ToString(), out int output);
			Classes.Editor.SolutionState.LevelID = output;
			Controls.Base.MainEditor.Instance.EditorStatusBar._levelIDLabel.Content = "Level ID: " + Classes.Editor.SolutionState.LevelID.ToString();
		}
		public void MakeShortcutForDataFolderOnly(object sender, RoutedEventArgs e)
		{
			string dataDir = Editor.DataDirectory;
			Classes.Editor.EditorActions.CreateShortcut(dataDir);
		}
		public void MakeShortcutWithCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string dataDir = Editor.DataDirectory;
			string scenePath = Editor.Paths.GetScenePath();
			int rX = (short)(Classes.Editor.SolutionState.ViewPositionX);
			int rY = (short)(Classes.Editor.SolutionState.ViewPositionY);
			double _ZoomLevel = Classes.Editor.SolutionState.ZoomLevel;
			bool isEncoreSet = Classes.Editor.SolutionState.UseEncoreColors;
			int levelSlotNum = Classes.Editor.SolutionState.LevelID;
			Classes.Editor.EditorActions.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum, _ZoomLevel);
		}
		public void MakeShortcutWithoutCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string dataDir = Editor.DataDirectory;
			string scenePath = Editor.Paths.GetScenePath();
			int rX = 0;
			int rY = 0;
			bool isEncoreSet = Classes.Editor.SolutionState.UseEncoreColors;
			int levelSlotNum = Classes.Editor.SolutionState.LevelID;
			Classes.Editor.EditorActions.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum);
		}

		#region Developer Stuff
		public void GoToPosition(object sender, RoutedEventArgs e)
		{
			GoToPositionBox form = new GoToPositionBox(Editor);
			form.Owner = Editor as Window;
			form.ShowDialog();

		}
		public void SoundLooperToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{

		}
		public void MD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MD5HashGen hashmap = new MD5HashGen(Editor);
			hashmap.Show();
		}
		public void FindAndReplaceTool(object sender, RoutedEventArgs e)
		{
			FindandReplaceTool form = new FindandReplaceTool();
			form.ShowDialog();
			if (form.DialogResult == true)
			{
				while (form.GetReadyState() == false)
				{

				}
				int applyState = form.GetApplyState();
				bool copyResults = form.CopyResultsOrNot();
				bool replaceMode = form.IsReplaceMode();
				int find = form.GetFindValue();
				int replace = form.GetReplaceValue();
				bool perserveColllision = form.PerservingCollision();

				if (replaceMode)
				{
					Controls.Base.MainEditor.Instance.FindAndReplace.EditorTileFindReplace(find, replace, applyState, copyResults);//, perserveColllision
				}
				else
				{
					Controls.Base.MainEditor.Instance.FindAndReplace.EditorTileFind(find, applyState, copyResults);
				}

			}

		}

		public void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!Classes.Editor.SolutionState.IsConsoleWindowOpen)
			{
				Classes.Editor.SolutionState.IsConsoleWindowOpen = true;
				Extensions.ExternalExtensions.ShowConsoleWindow();
			}
			else
			{
				Classes.Editor.SolutionState.IsConsoleWindowOpen = false;
				Extensions.ExternalExtensions.HideConsoleWindow();
			}
		}
		public void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Core.Settings.MyDevSettings.DevForceRestartData = Editor.DataDirectory;
			Core.Settings.MyDevSettings.DevForceRestartScene = Editor.Paths.SceneFilePath;
			Core.Settings.MyDevSettings.DevForceRestartX = (short)(Classes.Editor.SolutionState.ViewPositionX / Classes.Editor.SolutionState.Zoom);
			Core.Settings.MyDevSettings.DevForceRestartY = (short)(Classes.Editor.SolutionState.ViewPositionY / Classes.Editor.SolutionState.Zoom);
			Core.Settings.MyDevSettings.DevForceRestartZoomLevel = Classes.Editor.SolutionState.ZoomLevel;
			Core.Settings.MyDevSettings.DevForceRestartIsEncore = Editor.Paths.isEncoreMode;
			Core.Settings.MyDevSettings.DevForceRestartID = Classes.Editor.SolutionState.LevelID;
			Core.Settings.MyDevSettings.DevForceRestartCurrentName = Editor.Paths.CurrentName;
			Core.Settings.MyDevSettings.DevForceRestartCurrentZone = Editor.Paths.CurrentZone;
			Core.Settings.MyDevSettings.DevForceRestartSceneID = Editor.Paths.CurrentSceneID;
            Core.Settings.MyDevSettings.DevForceRestartIsBrowsed = Editor.Paths.Browsed;
            Core.Settings.MyDevSettings.DevForceRestartResourcePacks = new System.Collections.Specialized.StringCollection();
            Core.Settings.MyDevSettings.DevForceRestartResourcePacks.AddRange(Editor.ResourcePackList.ToArray());
        }

		public void EnableAllButtonsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			object[] MTB = Editor.EditorToolbar.MainToolbarButtons.Items.Cast<object>().ToArray();
			object[] LT = Editor.EditorToolbar.LayerToolbar.Items.Cast<object>().ToArray();
			ManiacEditor.Extensions.Extensions.EnableButtonList(MTB);
			ManiacEditor.Extensions.Extensions.EnableButtonList(LT);
		}
		#endregion

		#endregion

		#region Status Bar Items

		#region Quick Button Buttons
		public void SwapEncoreManiaEntityVisibility()
		{
			if (Core.Settings.MyDefaults.ShowEncoreEntities == true && Core.Settings.MyDefaults.ShowManiaEntities == true)
			{
				Core.Settings.MyDefaults.ShowManiaEntities = true;
				Core.Settings.MyDefaults.ShowEncoreEntities = false;
			}
			if (Core.Settings.MyDefaults.ShowEncoreEntities == true && Core.Settings.MyDefaults.ShowManiaEntities == false)
			{
				Core.Settings.MyDefaults.ShowManiaEntities = true;
				Core.Settings.MyDefaults.ShowEncoreEntities = false;
			}
			else
			{
				Core.Settings.MyDefaults.ShowManiaEntities = false;
				Core.Settings.MyDefaults.ShowEncoreEntities = true;
			}

		}
		#endregion

		#endregion

	}
}
