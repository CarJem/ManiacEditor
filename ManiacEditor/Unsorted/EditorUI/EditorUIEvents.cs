﻿using System;
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



		// Stuff Used for Command Line Tool to Fix Duplicate Object ID's


		public static void ShowConsoleWindow()
		{
			var handle = GetConsoleWindow();

			if (handle == IntPtr.Zero)
			{
				AllocConsole();
			}
			else
			{
				ShowWindow(handle, SW_SHOW);
			}
		}

		public static void HideConsoleWindow()
		{
			var handle = GetConsoleWindow();

			ShowWindow(handle, SW_HIDE);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AllocConsole();

		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AttachConsole(int dwProcessId);

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		const int SW_HIDE = 0;
		const int SW_SHOW = 5;

		public EditorUIEvents(Controls.Base.MainEditor instance)
		{
			Editor = instance;

		}

		#region File Tab Buttons

		public void BackupRecoverButton_Click(object sender, RoutedEventArgs e)
		{
			string Result = null, ResultOriginal = null, ResultOld = null;
			OpenFileDialog open = new OpenFileDialog
			{
				Filter = "Backup Scene|*.bin.bak|Old Scene|*.bin.old|Crash Backup Scene|*.bin.crash.bak"
			};
			if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
			{
				Result = open.FileName;
				ResultOriginal = Result.Split('.')[0] + ".bin";
				ResultOld = ResultOriginal + ".old";
				int i = 1;
				while ((File.Exists(ResultOld)))
				{
					ResultOld = ResultOriginal.Substring(0, ResultOriginal.Length - 4) + "." + i + ".bin.old";
					i++;
				}



			}

			if (Result == null)
				return;

            Classes.Core.Solution.UnloadScene();
            Editor.Settings.UseDefaultPrefrences();
			File.Replace(Result, ResultOriginal, ResultOld);

		}

		#region Backup SubMenu
		public void StageConfigBackup(object sender, RoutedEventArgs e)
		{
			StageConfigBackup(sender, e);
		}

		public void SceneBackup(object sender, RoutedEventArgs e)
		{
			SceneBackup(sender, e);
		}
		#endregion

		#endregion

		#region Edit Tab Buttons

		public void PasteToChunks()
		{
			if (Editor.TilesClipboard != null)
			{
				Editor.Chunks.ConvertClipboardtoMultiLayerChunk(Editor.TilesClipboard.Item1, Editor.TilesClipboard.Item2);
				Editor.TilesToolbar?.ChunksReload();
			}

		}

		public void SelectAll()
		{
			if (Editor.IsTilesEdit() && !Editor.IsChunksEdit())
			{
                if (Classes.Core.Solution.EditLayerA != null) Classes.Core.Solution.EditLayerA?.SelectAll();
                if (Classes.Core.Solution.EditLayerB != null) Classes.Core.Solution.EditLayerB?.SelectAll();
                Editor.UI.UpdateEditLayerActions();
			}
			else if (Editor.IsEntitiesEdit())
			{
                Classes.Core.Solution.Entities.SelectAll();
            }
			Editor.UI.SetSelectOnlyButtonsState();
			Classes.Core.SolutionState.RegionX1 = -1;
            Classes.Core.SolutionState.RegionY1 = -1;
		}

		public void FlipHorizontal()
		{
			Classes.Core.Solution.EditLayerA?.FlipPropertySelected(FlipDirection.Horizontal);
			Classes.Core.Solution.EditLayerB?.FlipPropertySelected(FlipDirection.Horizontal);
			Editor.UI.UpdateEditLayerActions();
		}

		public void FlipHorizontalIndividual()
		{
			Classes.Core.Solution.EditLayerA?.FlipPropertySelected(FlipDirection.Horizontal, true);
			Classes.Core.Solution.EditLayerB?.FlipPropertySelected(FlipDirection.Horizontal, true);
			Editor.UI.UpdateEditLayerActions();
		}

		public void Delete()
		{
			Editor.DeleteSelected();
		}

		public void Copy()
		{
			if (Editor.IsTilesEdit())
				Editor.CopyTilesToClipboard();


			else if (Editor.IsEntitiesEdit())
				Editor.CopyEntitiesToClipboard();


			Editor.UI.UpdateControls();
		}

		public void Duplicate()
		{
			if (Editor.IsTilesEdit())
			{
                Classes.Core.Solution.EditLayerA?.PasteFromClipboard(new Point(16, 16), Classes.Core.Solution.EditLayerA?.CopyToClipboard(true));
                Classes.Core.Solution.EditLayerB?.PasteFromClipboard(new Point(16, 16), Classes.Core.Solution.EditLayerB?.CopyToClipboard(true));
                Editor.UI.UpdateEditLayerActions();
			}
			else if (Editor.IsEntitiesEdit())
			{
				try
				{
					Classes.Core.Solution.Entities.PasteFromClipboard(new Point(16, 16), Classes.Core.Solution.Entities.CopyToClipboard(true));
					Editor.UpdateLastEntityAction();
				}
				catch (Classes.Core.Scene.EditorEntities.TooManyEntitiesException)
				{
					System.Windows.MessageBox.Show("Too many entities! (limit: 2048)");
					return;
				}
				Editor.UI.SetSelectOnlyButtonsState();
				Editor.UI.UpdateEntitiesToolbarList();

			}
		}

		public void Cut()
		{
			if (Editor.IsTilesEdit())
			{
				Editor.CopyTilesToClipboard();
				Editor.DeleteSelected();
				Editor.UI.UpdateControls();
				Editor.UI.UpdateEditLayerActions();
			}
			else if (Editor.IsEntitiesEdit())
			{
				if (Editor.EntitiesToolbar.IsFocused.Equals(false))
				{
					Editor.CopyEntitiesToClipboard();
					Editor.DeleteSelected();
					Editor.UI.UpdateControls();
					Editor.UI.UpdateEntitiesToolbarList();
				}
			}
		}

		public void Paste()
		{
			if (Editor.IsTilesEdit())
			{
				// check if there are tiles on the Windows clipboard; if so, use those
				if (System.Windows.Clipboard.ContainsData("ManiacTiles"))
				{
					var pasteData = (Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>) System.Windows.Clipboard.GetDataObject().GetData("ManiacTiles");
                    Point pastePoint = GetPastePoint();
					if (Classes.Core.Solution.EditLayerA != null) Classes.Core.Solution.EditLayerA.PasteFromClipboard(pastePoint, pasteData.Item1);
					if (Classes.Core.Solution.EditLayerB != null) Classes.Core.Solution.EditLayerB.PasteFromClipboard(pastePoint, pasteData.Item2);

					Editor.UI.UpdateEditLayerActions();
				}

				// if there's none, use the internal clipboard
				else if (Editor.TilesClipboard != null)
				{
                    Point pastePoint = GetPastePoint();
                    if (Classes.Core.Solution.EditLayerA != null) Classes.Core.Solution.EditLayerA.PasteFromClipboard(pastePoint, Editor.TilesClipboard.Item1);
					if (Classes.Core.Solution.EditLayerB != null) Classes.Core.Solution.EditLayerB.PasteFromClipboard(pastePoint, Editor.TilesClipboard.Item2);
					Editor.UI.UpdateEditLayerActions();
				}

			}
			else if (Editor.IsEntitiesEdit())
			{
                Editor.PasteEntitiesToClipboard();
            }

            Point GetPastePoint()
            {
                if (Editor.IsChunksEdit())
                {

                    Point p = new Point((int)(Classes.Core.SolutionState.LastX / Classes.Core.SolutionState.Zoom), (int)(Classes.Core.SolutionState.LastY / Classes.Core.SolutionState.Zoom));
                    return Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
                }
                else
                {
                    return new Point((int)(Classes.Core.SolutionState.LastX / Classes.Core.SolutionState.Zoom) + Classes.Core.Constants.TILE_SIZE - 1, (int)(Classes.Core.SolutionState.LastY / Classes.Core.SolutionState.Zoom) + Classes.Core.Constants.TILE_SIZE - 1);

                }
            }
		}

		public void FlipVertical()
		{
			Classes.Core.Solution.EditLayerA?.FlipPropertySelected(FlipDirection.Veritcal);
			Classes.Core.Solution.EditLayerB?.FlipPropertySelected(FlipDirection.Veritcal);
			Editor.UI.UpdateEditLayerActions();
		}

		public void FlipVerticalIndividual()
		{
			Classes.Core.Solution.EditLayerA?.FlipPropertySelected(FlipDirection.Veritcal, true);
			Classes.Core.Solution.EditLayerB?.FlipPropertySelected(FlipDirection.Veritcal, true);
			Editor.UI.UpdateEditLayerActions();
		}

		#endregion

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
							Classes.Core.SolutionState.CurrentControllerButtons = 2;
							break;
						case "Switch":
							Classes.Core.SolutionState.CurrentControllerButtons = 4;
							break;
						case "PS4":
							Classes.Core.SolutionState.CurrentControllerButtons = 3;
							break;
						case "Saturn Black":
							Classes.Core.SolutionState.CurrentControllerButtons = 5;
							break;
						case "Saturn White":
							Classes.Core.SolutionState.CurrentControllerButtons = 6;
							break;
						case "Switch Joy L":
							Classes.Core.SolutionState.CurrentControllerButtons = 7;
							break;
						case "Switch Joy R":
							Classes.Core.SolutionState.CurrentControllerButtons = 8;
							break;
						case "PC EN/JP":
							Classes.Core.SolutionState.CurrentControllerButtons = 1;
							break;
						case "PC FR":
							Classes.Core.SolutionState.CurrentControllerButtons = 9;
							break;
						case "PC IT":
							Classes.Core.SolutionState.CurrentControllerButtons = 10;
							break;
						case "PC GE":
							Classes.Core.SolutionState.CurrentControllerButtons = 11;
							break;
						case "PC SP":
							Classes.Core.SolutionState.CurrentControllerButtons = 12;
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
					Classes.Core.SolutionState.CurrentControllerButtons = 2;
					break;
				case "Switch":
					Classes.Core.SolutionState.CurrentControllerButtons = 4;
					break;
				case "PS4":
					Classes.Core.SolutionState.CurrentControllerButtons = 3;
					break;
				case "Saturn Black":
					Classes.Core.SolutionState.CurrentControllerButtons = 5;
					break;
				case "Saturn White":
					Classes.Core.SolutionState.CurrentControllerButtons = 6;
					break;
				case "Switch Joy L":
					Classes.Core.SolutionState.CurrentControllerButtons = 7;
					break;
				case "Switch Joy R":
					Classes.Core.SolutionState.CurrentControllerButtons = 8;
					break;
				case "PC EN/JP":
					Classes.Core.SolutionState.CurrentControllerButtons = 1;
					break;
				case "PC FR":
					Classes.Core.SolutionState.CurrentControllerButtons = 9;
					break;
				case "PC IT":
					Classes.Core.SolutionState.CurrentControllerButtons = 10;
					break;
				case "PC GE":
					Classes.Core.SolutionState.CurrentControllerButtons = 11;
					break;
				case "PC SP":
					Classes.Core.SolutionState.CurrentControllerButtons = 12;
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
							Editor.EncorePalette = Classes.Core.Solution.CurrentScene.GetEncorePalette("", "", "", "", -1, fd.FileName);
							Classes.Core.SolutionState.EncoreSetupType = 0;
							if (File.Exists(Editor.EncorePalette[0]))
							{
								Classes.Core.SolutionState.EncorePaletteExists = true;
                                Classes.Core.SolutionState.UseEncoreColors = true;
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
				Editor.EncorePalette = Classes.Core.Solution.CurrentScene.GetEncorePalette("", "", "", "", -1, path);
				Classes.Core.SolutionState.EncoreSetupType = 0;
				if (File.Exists(Editor.EncorePalette[0]))
				{
					Classes.Core.SolutionState.EncorePaletteExists = true;
					Classes.Core.SolutionState.UseEncoreColors = true;
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
                Classes.Core.SolutionState.entitiesTextFilter = theSender.Text;
                Editor.EditorMenuBar.toolStripTextBox1.Text = Classes.Core.SolutionState.entitiesTextFilter;
                //Editor.toolStripTextBox2.Text = Editor.entitiesTextFilter;
                Classes.Core.Solution.Entities.FilterRefreshNeeded = true;
                lockTextBox = false;
            }

            
		}

		public void SetScrollLockDirection()
		{
			if (Classes.Core.SolutionState.ScrollDirection == (int)ScrollDir.X)
			{
				Classes.Core.SolutionState.ScrollDirection = (int)ScrollDir.Y;
				Editor.EditorStatusBar.UpdateStatusPanel();
				Editor.EditorMenuBar.xToolStripMenuItem.IsChecked = false;
				Editor.EditorMenuBar.yToolStripMenuItem.IsChecked = true;
			}
			else
			{
				Classes.Core.SolutionState.ScrollDirection = (int)ScrollDir.X;
				Editor.EditorStatusBar.UpdateStatusPanel();
				Editor.EditorMenuBar.xToolStripMenuItem.IsChecked = true;
				Editor.EditorMenuBar.yToolStripMenuItem.IsChecked = false;
			}
		}

		public void MenuLanguageChanged(object sender, RoutedEventArgs e)
		{
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            Classes.Core.SolutionState.CurrentLanguage = menuItem.Tag.ToString();
            var allLangItems = Editor.EditorMenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
            {
                if (item.Tag.ToString() != menuItem.Tag.ToString()) item.IsChecked = false;
                else if (item.Tag.ToString() == menuItem.Tag.ToString()) item.IsChecked = true;
            }


		}

		#region Collision Options
		public void CollisionOpacitySliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
            Classes.Core.SolutionState.collisionOpacityChanged = true;
            Editor.ReloadSpecificTextures(sender, e);
            Editor.RefreshCollisionColours(true);
        }
        #endregion

        #endregion

        #region Tools Tab Buttons
        public void ChangeLevelID(object sender, RoutedEventArgs e)
		{
            string inputValue = GenerationsLib.WPF.TextPrompt2.ShowDialog("Change Level ID", "This is only temporary and will reset when you reload the scene.", Classes.Core.SolutionState.LevelID.ToString());
            int.TryParse(inputValue.ToString(), out int output);
			Classes.Core.SolutionState.LevelID = output;
			Controls.Base.MainEditor.Instance.EditorStatusBar._levelIDLabel.Content = "Level ID: " + Classes.Core.SolutionState.LevelID.ToString();
		}
		public void MakeShortcutForDataFolderOnly(object sender, RoutedEventArgs e)
		{
			string dataDir = Editor.DataDirectory;
			Editor.CreateShortcut(dataDir);
		}
		public void MakeShortcutWithCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string dataDir = Editor.DataDirectory;
			string scenePath = Editor.Paths.GetScenePath();
			int rX = (short)(Classes.Core.SolutionState.ViewPositionX);
			int rY = (short)(Classes.Core.SolutionState.ViewPositionY);
			double _ZoomLevel = Classes.Core.SolutionState.ZoomLevel;
			bool isEncoreSet = Classes.Core.SolutionState.UseEncoreColors;
			int levelSlotNum = Classes.Core.SolutionState.LevelID;
			Editor.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum, _ZoomLevel);
		}
		public void MakeShortcutWithoutCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string dataDir = Editor.DataDirectory;
			string scenePath = Editor.Paths.GetScenePath();
			int rX = 0;
			int rY = 0;
			bool isEncoreSet = Classes.Core.SolutionState.UseEncoreColors;
			int levelSlotNum = Classes.Core.SolutionState.LevelID;
			Editor.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum);
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
			if (!Classes.Core.SolutionState.IsConsoleWindowOpen)
			{
				Classes.Core.SolutionState.IsConsoleWindowOpen = true;
				ShowConsoleWindow();
			}
			else
			{
				Classes.Core.SolutionState.IsConsoleWindowOpen = false;
				HideConsoleWindow();
			}
		}
		public void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Core.Settings.MyDevSettings.DevForceRestartData = Editor.DataDirectory;
			Core.Settings.MyDevSettings.DevForceRestartScene = Editor.Paths.SceneFilePath;
			Core.Settings.MyDevSettings.DevForceRestartX = (short)(Classes.Core.SolutionState.ViewPositionX / Classes.Core.SolutionState.Zoom);
			Core.Settings.MyDevSettings.DevForceRestartY = (short)(Classes.Core.SolutionState.ViewPositionY / Classes.Core.SolutionState.Zoom);
			Core.Settings.MyDevSettings.DevForceRestartZoomLevel = Classes.Core.SolutionState.ZoomLevel;
			Core.Settings.MyDevSettings.DevForceRestartIsEncore = Editor.Paths.isEncoreMode;
			Core.Settings.MyDevSettings.DevForceRestartID = Classes.Core.SolutionState.LevelID;
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
			ManiacEditor.Extensions.EnableButtonList(MTB);
			ManiacEditor.Extensions.EnableButtonList(LT);
		}
		#endregion

		#endregion

		#region Main Toolstrip Buttons

		public void ZoomIn(object sender, RoutedEventArgs e)
		{
			Classes.Core.SolutionState.ZoomLevel += 1;
			if (Classes.Core.SolutionState.ZoomLevel >= 5) Classes.Core.SolutionState.ZoomLevel = 5;
			if (Classes.Core.SolutionState.ZoomLevel <= -5) Classes.Core.SolutionState.ZoomLevel = -5;

			Editor.ZoomModel.SetZoomLevel(Classes.Core.SolutionState.ZoomLevel, new Point(0, 0));
		}

		public void ZoomOut(object sender, RoutedEventArgs e)
		{
			Classes.Core.SolutionState.ZoomLevel -= 1;
			if (Classes.Core.SolutionState.ZoomLevel >= 5) Classes.Core.SolutionState.ZoomLevel = 5;
			if (Classes.Core.SolutionState.ZoomLevel <= -5) Classes.Core.SolutionState.ZoomLevel = -5;

			Editor.ZoomModel.SetZoomLevel(Classes.Core.SolutionState.ZoomLevel, new Point(0, 0));
		}


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
