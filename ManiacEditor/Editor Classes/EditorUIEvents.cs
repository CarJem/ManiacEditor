using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using RSDKv5;
using System.Drawing;
using Point = System.Drawing.Point;
using ManiacEditor.Interfaces;
using Cyotek.Windows.Forms;
using Color = System.Drawing.Color;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Windows.Controls;

namespace ManiacEditor
{

	public class EditorUIEvents
	{
		private Editor Editor;
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

		public EditorUIEvents(Editor instance)
		{
			Editor = instance;

		}

		#region File Tab Buttons

		public void BackupToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.BackupTool(null, null);
		}

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

			Editor.UnloadScene();
			Editor.Settings.UseDefaultPrefrences();
			File.Replace(Result, ResultOriginal, ResultOld);

		}

		#region Backup SubMenu
		public void StageConfigBackup(object sender, RoutedEventArgs e)
		{
			Editor.UIModes.BackupType = 4;
			BackupToolStripMenuItem_Click(null, null);
			Editor.UIModes.BackupType = 0;
		}

		public void SceneBackup(object sender, RoutedEventArgs e)
		{
			Editor.UIModes.BackupType = 1;
			BackupToolStripMenuItem_Click(null, null);
			Editor.UIModes.BackupType = 0;
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
                if (Editor.EditLayerA != null) Editor.EditLayerA?.SelectAll();
                if (Editor.EditLayerB != null) Editor.EditLayerB?.SelectAll();
                Editor.UI.UpdateEditLayerActions();
			}
			else if (Editor.IsEntitiesEdit())
			{
                Editor.Entities.SelectAll();
            }
			Editor.UI.SetSelectOnlyButtonsState();
			Editor.StateModel.SelectionX1 = -1;
			Editor.StateModel.SelectionY1 = -1;
		}

		public void FlipHorizontal()
		{
			Editor.EditLayerA?.FlipPropertySelected(FlipDirection.Horizontal);
			Editor.EditLayerB?.FlipPropertySelected(FlipDirection.Horizontal);
			Editor.UI.UpdateEditLayerActions();
		}

		public void FlipHorizontalIndividual()
		{
			Editor.EditLayerA?.FlipPropertySelected(FlipDirection.Horizontal, true);
			Editor.EditLayerB?.FlipPropertySelected(FlipDirection.Horizontal, true);
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
                Editor.EditLayerA?.PasteFromClipboard(new Point(16, 16), Editor.EditLayerA?.CopyToClipboard(true));
                Editor.EditLayerB?.PasteFromClipboard(new Point(16, 16), Editor.EditLayerB?.CopyToClipboard(true));
                Editor.UI.UpdateEditLayerActions();
			}
			else if (Editor.IsEntitiesEdit())
			{
				try
				{
					Editor.Entities.PasteFromClipboard(new Point(16, 16), Editor.Entities.CopyToClipboard(true));
					Editor.UpdateLastEntityAction();
				}
				catch (EditorEntities.TooManyEntitiesException)
				{
					RSDKrU.MessageBox.Show("Too many entities! (limit: 2048)");
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
					if (Editor.EditLayerA != null) Editor.EditLayerA.PasteFromClipboard(pastePoint, pasteData.Item1);
					if (Editor.EditLayerB != null) Editor.EditLayerB.PasteFromClipboard(pastePoint, pasteData.Item2);

					Editor.UI.UpdateEditLayerActions();
				}

				// if there's none, use the internal clipboard
				else if (Editor.TilesClipboard != null)
				{
                    Point pastePoint = GetPastePoint();
                    if (Editor.EditLayerA != null) Editor.EditLayerA.PasteFromClipboard(pastePoint, Editor.TilesClipboard.Item1);
					if (Editor.EditLayerB != null) Editor.EditLayerB.PasteFromClipboard(pastePoint, Editor.TilesClipboard.Item2);
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

                    Point p = new Point((int)(Editor.StateModel.lastX / Editor.StateModel.Zoom), (int)(Editor.StateModel.lastY / Editor.StateModel.Zoom));
                    return EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
                }
                else
                {
                    return new Point((int)(Editor.StateModel.lastX / Editor.StateModel.Zoom) + EditorConstants.TILE_SIZE - 1, (int)(Editor.StateModel.lastY / Editor.StateModel.Zoom) + EditorConstants.TILE_SIZE - 1);

                }
            }
		}

		public void FlipVertical()
		{
			Editor.EditLayerA?.FlipPropertySelected(FlipDirection.Veritcal);
			Editor.EditLayerB?.FlipPropertySelected(FlipDirection.Veritcal);
			Editor.UI.UpdateEditLayerActions();
		}

		public void FlipVerticalIndividual()
		{
			Editor.EditLayerA?.FlipPropertySelected(FlipDirection.Veritcal, true);
			Editor.EditLayerB?.FlipPropertySelected(FlipDirection.Veritcal, true);
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
                    var allItems = Editor.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                    foreach (System.Windows.Controls.MenuItem item in allItems)
                    {
                        if (item.Tag == null || item.Tag.ToString() != menuItem.Tag.ToString()) item.IsChecked = false;
                        else if (item.Tag.ToString() == menuItem.Tag.ToString()) item.IsChecked = true;
                        var allSubItems = Editor.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                        foreach (System.Windows.Controls.MenuItem subItem in allSubItems)
                        {
                            if (subItem.Tag == null || subItem.Tag.ToString() != menuItem.Tag.ToString()) subItem.IsChecked = false;
                            else if (subItem.Tag.ToString() == menuItem.Tag.ToString()) subItem.IsChecked = true;
                        }
					}            
					switch (tag)
					{
						case "Xbox":
							Editor.UIModes.CurrentControllerButtons = 2;
							break;
						case "Switch":
							Editor.UIModes.CurrentControllerButtons = 4;
							break;
						case "PS4":
							Editor.UIModes.CurrentControllerButtons = 3;
							break;
						case "Saturn Black":
							Editor.UIModes.CurrentControllerButtons = 5;
							break;
						case "Saturn White":
							Editor.UIModes.CurrentControllerButtons = 6;
							break;
						case "Switch Joy L":
							Editor.UIModes.CurrentControllerButtons = 7;
							break;
						case "Switch Joy R":
							Editor.UIModes.CurrentControllerButtons = 8;
							break;
						case "PC EN/JP":
							Editor.UIModes.CurrentControllerButtons = 1;
							break;
						case "PC FR":
							Editor.UIModes.CurrentControllerButtons = 9;
							break;
						case "PC IT":
							Editor.UIModes.CurrentControllerButtons = 10;
							break;
						case "PC GE":
							Editor.UIModes.CurrentControllerButtons = 11;
							break;
						case "PC SP":
							Editor.UIModes.CurrentControllerButtons = 12;
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
					Editor.UIModes.CurrentControllerButtons = 2;
					break;
				case "Switch":
					Editor.UIModes.CurrentControllerButtons = 4;
					break;
				case "PS4":
					Editor.UIModes.CurrentControllerButtons = 3;
					break;
				case "Saturn Black":
					Editor.UIModes.CurrentControllerButtons = 5;
					break;
				case "Saturn White":
					Editor.UIModes.CurrentControllerButtons = 6;
					break;
				case "Switch Joy L":
					Editor.UIModes.CurrentControllerButtons = 7;
					break;
				case "Switch Joy R":
					Editor.UIModes.CurrentControllerButtons = 8;
					break;
				case "PC EN/JP":
					Editor.UIModes.CurrentControllerButtons = 1;
					break;
				case "PC FR":
					Editor.UIModes.CurrentControllerButtons = 9;
					break;
				case "PC IT":
					Editor.UIModes.CurrentControllerButtons = 10;
					break;
				case "PC GE":
					Editor.UIModes.CurrentControllerButtons = 11;
					break;
				case "PC SP":
					Editor.UIModes.CurrentControllerButtons = 12;
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
							Editor.EncorePalette = Editor.EditorScene.GetEncorePalette("", "", "", "", -1, fd.FileName);
							Editor.UIModes.EncoreSetupType = 0;
							if (File.Exists(Editor.EncorePalette[0]))
							{
								Editor.UIModes.EncorePaletteExists = true;
                                Editor.UIModes.UseEncoreColors = true;
                            }

						}
					}
				}
				catch (Exception ex)
				{
					RSDKrU.MessageBox.Show("Unable to set Encore Colors. " + ex.Message);
				}
			}
			else if (path != "")
			{
				Editor.EncorePalette = Editor.EditorScene.GetEncorePalette("", "", "", "", -1, path);
				Editor.UIModes.EncoreSetupType = 0;
				if (File.Exists(Editor.EncorePalette[0]))
				{
					Editor.UIModes.EncorePaletteExists = true;
					Editor.UIModes.UseEncoreColors = true;
				}
				else
				{
					RSDKrU.MessageBox.Show("Unable to set Encore Colors. The Specified Path does not exist: " + Environment.NewLine + path);
				}
			}

		}

		public void EntityFilterTextChanged(object sender, TextChangedEventArgs e)
		{
            if (sender is System.Windows.Controls.TextBox && lockTextBox == false)
            {
                lockTextBox = true;
                System.Windows.Controls.TextBox theSender = sender as System.Windows.Controls.TextBox;
                Editor.UIModes.entitiesTextFilter = theSender.Text;
                Editor.toolStripTextBox1.Text = Editor.UIModes.entitiesTextFilter;
                //Editor.toolStripTextBox2.Text = Editor.entitiesTextFilter;
                Editor.Entities.FilterRefreshNeeded = true;
                lockTextBox = false;
            }

            
		}

		public void SetScrollLockDirection()
		{
			if (Editor.UIModes.ScrollDirection == (int)ScrollDir.X)
			{
				Editor.UIModes.ScrollDirection = (int)ScrollDir.Y;
				Editor.UI.UpdateStatusPanel();
				Editor.xToolStripMenuItem.IsChecked = false;
				Editor.yToolStripMenuItem.IsChecked = true;
			}
			else
			{
				Editor.UIModes.ScrollDirection = (int)ScrollDir.X;
				Editor.UI.UpdateStatusPanel();
				Editor.xToolStripMenuItem.IsChecked = true;
				Editor.yToolStripMenuItem.IsChecked = false;
			}
		}

		public void MenuLanguageChanged(object sender, RoutedEventArgs e)
		{
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            Editor.UIModes.CurrentLanguage = menuItem.Tag.ToString();
            var allLangItems = Editor.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
            {
                if (item.Tag.ToString() != menuItem.Tag.ToString()) item.IsChecked = false;
                else if (item.Tag.ToString() == menuItem.Tag.ToString()) item.IsChecked = true;
            }


		}

		#region Collision Options
		public void CollisionOpacitySliderDragEnd(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			if (Editor.UIModes.collisionOpacityChanged)
			{
				Editor.UIModes.collisionOpacityChanged = false;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
		}
		public void CollisionOpacitySliderLostFocus(object sender, RoutedEventArgs e)
		{
			if (Editor.UIModes.collisionOpacityChanged)
			{
				Editor.UIModes.collisionOpacityChanged = false;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
		}
		public void CollisionOpacitySliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Editor.UIModes.collisionOpacityChanged = true;
		}
		#endregion

		#endregion

		#region Tools Tab Buttons
		public void ChangeLevelID(object sender, RoutedEventArgs e)
		{
			string inputValue = RSDKrU.TextPrompt2.ShowDialog("Change Level ID", "This is only temporary and will reset when you reload the scene.", Editor.UIModes.LevelID.ToString());
			int.TryParse(inputValue.ToString(), out int output);
			Editor.UIModes.LevelID = output;
			Editor._levelIDLabel.Content = "Level ID: " + Editor.UIModes.LevelID.ToString();
		}
		public void MakeShortcutForDataFolderOnly(object sender, RoutedEventArgs e)
		{
			string dataDir = Editor.DataDirectory;
			Editor.CreateShortcut(dataDir);
		}
		public void MakeShortcutWithCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string dataDir = Editor.DataDirectory;
			string scenePath = Editor.Discord.ScenePath;
			int rX = (short)(Editor.StateModel.ShiftX);
			int rY = (short)(Editor.StateModel.ShiftY);
			double _ZoomLevel = Editor.StateModel.ZoomLevel;
			bool isEncoreSet = Editor.UIModes.UseEncoreColors;
			int levelSlotNum = Editor.UIModes.LevelID;
			Editor.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum, _ZoomLevel);
		}
		public void MakeShortcutWithoutCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string dataDir = Editor.DataDirectory;
			string scenePath = Editor.Discord.ScenePath;
			int rX = 0;
			int rY = 0;
			bool isEncoreSet = Editor.UIModes.UseEncoreColors;
			int levelSlotNum = Editor.UIModes.LevelID;
			Editor.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum);
		}

		#region Developer Stuff
		public void GoToPosition(object sender, RoutedEventArgs e)
		{
			GoToPositionBox form = new GoToPositionBox(Editor);
			if (form.ShowDialog().Value == true)
			{
				int x = form.goTo_X;
				int y = form.goTo_Y;
				if (form.tilesMode)
				{
					x *= 16;
					y *= 16;
				}
				Editor.GoToPosition(x, y);
			}

		}
		public void SoundLooperToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SoundLooper form = new SoundLooper();
			form.ShowDialog();
		}
		public void MD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Interfaces.WPF_UI.Options___Dev.MD5HashGen hashmap = new ManiacEditor.Interfaces.WPF_UI.Options___Dev.MD5HashGen(Editor);
			hashmap.Show();
		}
		public void PlayerSpawnToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.UIModes.selectPlayerObject_GoTo = -1;
			if (Editor.playerObjectPosition.Count == 0) return;

			if (Editor.playerObjectPosition.Count == 1)
			{
				//Set Zoom Level to Position so we can Move to that location
				int xPos = (int)(Editor.playerObjectPosition[0].Position.X.High);
				int yPos = (int)(Editor.playerObjectPosition[0].Position.Y.High);
				Editor.GoToPosition(xPos, yPos);
			}
			else
			{
				GoToPlayerBox goToPlayerBox = new GoToPlayerBox(Editor);
				goToPlayerBox.ShowDialog();
				if (Editor.UIModes.selectPlayerObject_GoTo != -1)
				{
					int objectIndex = Editor.UIModes.selectPlayerObject_GoTo;
					int xPos = (int)((int)Editor.playerObjectPosition[objectIndex].Position.X.High);
					int yPos = (int)((int)Editor.playerObjectPosition[objectIndex].Position.Y.High);
					Editor.GoToPosition(xPos, yPos);
				}
			}
		}
		public void FindAndReplaceTool(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Interfaces.WPF_UI.Editor_Tools.FindandReplaceTool form = new ManiacEditor.Interfaces.WPF_UI.Editor_Tools.FindandReplaceTool();
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
					Editor.Instance.FindAndReplace.EditorTileFindReplace(find, replace, applyState, copyResults);//, perserveColllision
				}
				else
				{
					Editor.Instance.FindAndReplace.EditorTileFind(find, applyState, copyResults);
				}

			}

		}

		public void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!Editor.UIModes.IsConsoleWindowOpen)
			{
				Editor.UIModes.IsConsoleWindowOpen = true;
				ShowConsoleWindow();
			}
			else
			{
				Editor.UIModes.IsConsoleWindowOpen = false;
				HideConsoleWindow();
			}
		}
		public void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Settings.MyDevSettings.DevForceRestartData = Editor.DataDirectory;
			Settings.MyDevSettings.DevForceRestartScene = Editor.Paths.SceneFilePath;
			Settings.MyDevSettings.DevForceRestartX = (short)(Editor.StateModel.ShiftX / Editor.StateModel.Zoom);
			Settings.MyDevSettings.DevForceRestartY = (short)(Editor.StateModel.ShiftY / Editor.StateModel.Zoom);
			Settings.MyDevSettings.DevForceRestartZoomLevel = Editor.StateModel.ZoomLevel;
			Settings.MyDevSettings.DevForceRestartIsEncore = Editor.Paths.isEncoreMode;
			Settings.MyDevSettings.DevForceRestartID = Editor.UIModes.LevelID;
			Settings.MyDevSettings.DevForceRestartCurrentName = Editor.Paths.CurrentName;
			Settings.MyDevSettings.DevForceRestartCurrentZone = Editor.Paths.CurrentZone;
			Settings.MyDevSettings.DevForceRestartSceneID = Editor.Paths.CurrentSceneID;
            Settings.MyDevSettings.DevForceRestartIsBrowsed = Editor.Paths.Browsed;
            Settings.MyDevSettings.DevForceRestartResourcePacks = new System.Collections.Specialized.StringCollection();
            Settings.MyDevSettings.DevForceRestartResourcePacks.AddRange(Editor.ResourcePackList.ToArray());
        }

		public void EnableAllButtonsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			object[] MTB = Editor.MainToolbarButtons.Items.Cast<object>().ToArray();
			object[] LT = Editor.LayerToolbar.Items.Cast<object>().ToArray();
			ManiacEditor.Extensions.EnableButtonList(MTB);
			ManiacEditor.Extensions.EnableButtonList(LT);
		}
		#endregion

		#endregion

		#region Main Toolstrip Buttons

		public void ZoomIn(object sender, RoutedEventArgs e)
		{
			Editor.StateModel.ZoomLevel += 1;
			if (Editor.StateModel.ZoomLevel >= 5) Editor.StateModel.ZoomLevel = 5;
			if (Editor.StateModel.ZoomLevel <= -5) Editor.StateModel.ZoomLevel = -5;

			Editor.ZoomModel.SetZoomLevel(Editor.StateModel.ZoomLevel, new Point(0, 0));
		}

		public void ZoomOut(object sender, RoutedEventArgs e)
		{
			Editor.StateModel.ZoomLevel -= 1;
			if (Editor.StateModel.ZoomLevel >= 5) Editor.StateModel.ZoomLevel = 5;
			if (Editor.StateModel.ZoomLevel <= -5) Editor.StateModel.ZoomLevel = -5;

			Editor.ZoomModel.SetZoomLevel(Editor.StateModel.ZoomLevel, new Point(0, 0));
		}


		#endregion

		#region Status Bar Items

		#region Quick Button Buttons
		public void SwapEncoreManiaEntityVisibility()
		{
			if (Settings.MyDefaults.ShowEncoreEntities == true && Settings.MyDefaults.ShowManiaEntities == true)
			{
				Settings.MyDefaults.ShowManiaEntities = true;
				Settings.MyDefaults.ShowEncoreEntities = false;
			}
			if (Settings.MyDefaults.ShowEncoreEntities == true && Settings.MyDefaults.ShowManiaEntities == false)
			{
				Settings.MyDefaults.ShowManiaEntities = true;
				Settings.MyDefaults.ShowEncoreEntities = false;
			}
			else
			{
				Settings.MyDefaults.ShowManiaEntities = false;
				Settings.MyDefaults.ShowEncoreEntities = true;
			}

		}
		#endregion

		#endregion

	}
}
