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

namespace ManiacEditor
{

	public class EditorInteractions
	{
		private Editor Editor;


		public ManiacED_ManiaPal.Connector ManiaPalConnector;

		// Stuff Used for Command Line Tool to Fix Duplicate Object ID's
		#region DLL Import Stuff
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
		private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

		[DllImport("USER32.DLL")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		private enum ShowWindowEnum
		{
			Hide = 0,
			ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
			Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
			Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
			Restore = 9, ShowDefault = 10, ForceMinimized = 11
		};

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
		#endregion

		//Shorthanding Setting Files
		public Properties.Settings mySettings = Properties.Settings.Default;
		public Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;
		public EditorInteractions(Editor instance)
		{
			Editor = instance;

		}

		#region File Tab Buttons

		public void New_Click(object sender, RoutedEventArgs e)
		{
			Editor.UnloadScene();
			NewSceneWindow makerDialog = new NewSceneWindow();
			makerDialog.Owner = Editor.GetWindow(Editor);
			if (makerDialog.ShowDialog() == true)
			{
				string directoryPath = Path.GetDirectoryName(makerDialog.SceneFolder);
				//Editor.SelectedZone = (new DirectoryInfo(directoryPath).Name).Replace("\\", "");
				//Editor.SelectedScene = Path.GetFileName(makerDialog.SceneFolder);
				//Editor.SceneFilename = "Scene1.bin";
				//Editor.SceneFilepath = Path.Combine(directoryPath) + "//Scene1.bin";

				Editor.EditorScene = new EditorScene(Editor.editorView.GraphicPanel, makerDialog.Scene_Width, makerDialog.Scene_Height, makerDialog.BG_Width, makerDialog.BG_Height, Editor);
				Editor.TilesConfig = new TileConfig();
				Editor.EditorTiles.StageTiles = new StageTiles();
				Editor.StageConfig = new StageConfig();

				string ImagePath = directoryPath + "//16x16Tiles.gif";
				string TilesPath = directoryPath + "//TilesConfig.bin";
				string StagePath = directoryPath + "//StageConfig.bin";

				//File.Create(Editor.SceneFilepath).Dispose();
				File.Create(ImagePath).Dispose();
				File.Create(TilesPath).Dispose();
				File.Create(StagePath).Dispose();

				//EditorScene.Write(SceneFilepath);
				Editor.TilesConfig.Write(TilesPath);
				//StageConfig.Write(StagePath);
				Editor.EditorTiles.StageTiles.Write(ImagePath);


				Editor.UpdateDataFolderLabel(null, null);

				Editor.SetupLayerButtons();


				Editor.EditorBackground = new EditorBackground(Editor);

				Editor.entities = new EditorEntities(Editor.EditorScene, Editor);

				Editor.SetViewSize((int)(Editor.SceneWidth * Editor.Zoom), (int)(Editor.SceneHeight * Editor.Zoom));

				Editor.UpdateControls(true);
			}

		}

		public void Open_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.AllowSceneUnloading() != true) return;
			Editor.UnloadScene();

			Editor.OpenScene(mySettings.forceBrowse);

		}

		public void OpenToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Open_Click(sender, e);
		}

		public void OpenDataDirectoryToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.AllowSceneUnloading() != true) return;

			string newDataDirectory = Editor.GetDataDirectory();
			if (null == newDataDirectory) return;
			if (newDataDirectory.Equals(Editor.DataDirectory)) return;

			if (Editor.IsDataDirectoryValid(newDataDirectory))
				Editor.ResetDataDirectoryToAndResetScene(newDataDirectory);
			else
				RSDKrU.MessageBox.Show($@"{newDataDirectory} is not
a valid Data Directory.",
								"Invalid Data Directory!",
								MessageBoxButton.OK,
								MessageBoxImage.Error);

		}

		public void Save_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditorSceneLoading.Save();
		}

		public void ExitToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.InstanceID == -1)
			{
				Editor.Close();
			}
			else
			{
				Editor.CloseMegaManiacTab = true;
			}
		}

		public void SaveAspngToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.EditorScene == null) return;

			System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog
			{
				Filter = ".png File|*.png",
				DefaultExt = "png"
			};
			if (save.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
			{
				Editor.isExportingImage = true;
				using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(Editor.SceneWidth, Editor.SceneHeight))
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
				{
					// not all scenes have both a Low and a High foreground
					// only attempt to render the ones we actually have
					Editor.FGLower?.Draw(g);
					Editor.FGLow?.Draw(g);
					Editor.FGHigh?.Draw(g);
					Editor.FGHigher?.Draw(g);
					//entities?.GraphicsDraw(editorView.GraphicPanel, g);

					bitmap.Save(save.FileName);
				}

			}
		}

		public void ExportEachLayerAspngToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (Editor.EditorScene?._editorLayers == null || !Editor.EditorScene._editorLayers.Any()) return;

				var dialog = new FolderSelectDialog()
				{
					Title = "Select folder to save each exported layer image to"
				};

				if (!dialog.ShowDialog()) return;

				int fileCount = 0;

				foreach (var editorLayer in Editor.EditorScene.AllLayers)
				{
					string fileName = System.IO.Path.Combine(dialog.FileName, editorLayer.Name + ".png");

					if (!Editor.CanWriteFile(fileName))
					{
						Editor.ShowError($"Layer export aborted. {fileCount} images saved.");
						return;
					}

					using (var bitmap = new System.Drawing.Bitmap(editorLayer.Width * EditorLayer.TILE_SIZE, editorLayer.Height * EditorLayer.TILE_SIZE))
					using (var g = System.Drawing.Graphics.FromImage(bitmap))
					{
						editorLayer.Draw(g);
						bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
						++fileCount;
					}
				}

				RSDKrU.MessageBox.Show($"Layer export succeeded. {fileCount} images saved.", "Success!",
								MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				Editor.ShowError("An error occurred: " + ex.Message);
			}
		}

		public void SaveAsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.EditorScene == null) return;

			if (Editor.IsTilesEdit())
			{
				// Apply changes
				Editor.Deselect();
			}

			System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog
			{
				Filter = "Scene File|*.bin",
				DefaultExt = "bin",
				InitialDirectory = Editor.EditorPath.SceneDirectory,
				RestoreDirectory = false,
				FileName = System.IO.Path.GetFileName(Editor.EditorPath.SceneFile_Source)
			};
			if (save.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
			{
				Editor.EditorScene.Write(save.FileName);
			}
		}

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
			Editor.UseDefaultPrefrences();
			File.Replace(Result, ResultOriginal, ResultOld);

		}

		public void ObjectManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var objectManager = new ManiacEditor.Interfaces.ObjectManager(Editor.EditorScene.Objects, Editor.StageConfig, Editor);
			objectManager.Owner = Window.GetWindow(Editor);
			objectManager.ShowDialog();
		}

		public void UnloadSceneToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.UnloadScene();
		}

		#region Backup SubMenu
		public void StageConfigToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.backupType = 4;
			BackupToolStripMenuItem_Click(null, null);
			Editor.backupType = 0;
		}

		public void NormalToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.backupType = 1;
			BackupToolStripMenuItem_Click(null, null);
			Editor.backupType = 0;
		}
		#endregion

		#endregion

		#region Edit Tab Buttons

		public void chunkToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.TilesClipboard != null)
			{
				Editor.EditorChunk.ConvertClipboardtoMultiLayerChunk(Editor.TilesClipboard.Item1, Editor.TilesClipboard.Item2);
				Editor.TilesToolbar?.ChunksReload();
			}

		}

		public void SelectAllToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.IsTilesEdit() && !Editor.IsChunksEdit())
			{
				Editor.EditLayerA?.Select(new Rectangle(0, 0, 32768, 32768), true, false);
				//EditLayerB?.Select(new Rectangle(0, 0, 32768, 32768), true, false);
				Editor.UpdateEditLayerActions();
			}
			else if (Editor.IsEntitiesEdit())
			{
				Editor.entities.Select(new Rectangle(0, 0, 32768, 32768), true, false);
			}
			Editor.SetSelectOnlyButtonsState();
			Editor.ClickedX = -1;
			Editor.ClickedY = -1;
		}

		public void FlipHorizontalToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditLayerA?.FlipPropertySelected(FlipDirection.Horizontal);
			Editor.EditLayerB?.FlipPropertySelected(FlipDirection.Horizontal);
			Editor.UpdateEditLayerActions();
		}

		public void FlipHorizontalIndividualToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditLayerA?.FlipPropertySelected(FlipDirection.Horizontal, true);
			Editor.EditLayerB?.FlipPropertySelected(FlipDirection.Horizontal, true);
			Editor.UpdateEditLayerActions();
		}

		public void DeleteToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.DeleteSelected();
		}

		public void CopyToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.IsTilesEdit())
				Editor.CopyTilesToClipboard();


			else if (Editor.IsEntitiesEdit())
				Editor.CopyEntitiesToClipboard();


			Editor.UpdateControls();
		}

		public void DuplicateToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.IsTilesEdit())
			{
				Editor.EditLayerA?.PasteFromClipboard(new Point(16, 16), Editor.EditLayerA?.CopyToClipboard(true));
				Editor.EditLayerB?.PasteFromClipboard(new Point(16, 16), Editor.EditLayerB?.CopyToClipboard(true));
				Editor.UpdateEditLayerActions();
			}
			else if (Editor.IsEntitiesEdit())
			{
				try
				{
					Editor.entities.PasteFromClipboard(new Point(16, 16), Editor.entities.CopyToClipboard(true));
					Editor.UpdateLastEntityAction();
				}
				catch (EditorEntities.TooManyEntitiesException)
				{
					RSDKrU.MessageBox.Show("Too many entities! (limit: 2048)");
					return;
				}
				Editor.UpdateEntitiesToolbarList();
				Editor.SetSelectOnlyButtonsState();
				Editor.UpdateEntitiesToolbarList();

			}
		}

		public void UndoToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditorUndo();
		}

		public void RedoToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditorRedo();
		}

		public void CutToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.IsTilesEdit())
			{
				Editor.CopyTilesToClipboard();
				Editor.DeleteSelected();
				Editor.UpdateControls();
				Editor.UpdateEditLayerActions();
			}
			else if (Editor.IsEntitiesEdit())
			{
				if (Editor.entitiesToolbar.IsFocused.Equals(false))
				{
					Editor.CopyEntitiesToClipboard();
					Editor.DeleteSelected();
					Editor.UpdateControls();
					Editor.UpdateEntitiesToolbarList();
				}
			}
		}

		public void PasteToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.IsTilesEdit())
			{
				// check if there are tiles on the Windows clipboard; if so, use those
				if (mySettings.EnableWindowsClipboard && System.Windows.Clipboard.ContainsData("ManiacTiles"))
				{
					var pasteData = (Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>) System.Windows.Clipboard.GetDataObject().GetData("ManiacTiles");
					Point pastePoint = new Point((int)(Editor.lastX / Editor.Zoom) + EditorLayer.TILE_SIZE - 1, (int)(Editor.lastY / Editor.Zoom) + EditorLayer.TILE_SIZE - 1);
					if (Editor.EditLayerA != null) Editor.EditLayerA.PasteFromClipboard(pastePoint, pasteData.Item1);
					if (Editor.EditLayerB != null) Editor.EditLayerB.PasteFromClipboard(pastePoint, pasteData.Item2);

					Editor.UpdateEditLayerActions();
				}

				// if there's none, use the internal clipboard
				else if (Editor.TilesClipboard != null)
				{
					Point pastePoint = new Point((int)(Editor.lastX / Editor.Zoom) + EditorLayer.TILE_SIZE - 1, (int)(Editor.lastY / Editor.Zoom) + EditorLayer.TILE_SIZE - 1);
					if (Editor.EditLayerA != null) Editor.EditLayerA.PasteFromClipboard(pastePoint, Editor.TilesClipboard.Item1);
					if (Editor.EditLayerB != null) Editor.EditLayerB.PasteFromClipboard(pastePoint, Editor.TilesClipboard.Item2);
					Editor.UpdateEditLayerActions();
				}

			}
			else if (Editor.IsEntitiesEdit())
			{
				if (Editor.entitiesToolbar.IsFocused.Equals(false))
				{
					try
					{

						// check if there are entities on the Windows clipboard; if so, use those
						if (mySettings.EnableWindowsClipboard && System.Windows.Clipboard.ContainsData("ManiacEntities"))
						{

							Editor.entities.PasteFromClipboard(new Point((int)(Editor.lastX / Editor.Zoom), (int)(Editor.lastY / Editor.Zoom)), (List<EditorEntity>)System.Windows.Clipboard.GetDataObject().GetData("ManiacEntities"));
							Editor.UpdateLastEntityAction();
						}

						// if there's none, use the internal clipboard
						else if (Editor.entitiesClipboard != null)
						{
							Editor.entities.PasteFromClipboard(new Point((int)(Editor.lastX / Editor.Zoom), (int)(Editor.lastY / Editor.Zoom)), Editor.entitiesClipboard);
							Editor.UpdateLastEntityAction();
						}
					}
					catch (EditorEntities.TooManyEntitiesException)
					{
						RSDKrU.MessageBox.Show("Too many entities! (limit: 2048)");
						return;
					}
					Editor.UpdateEntitiesToolbarList();
					Editor.SetSelectOnlyButtonsState();
				}
			}
		}

		public void FlipVerticalToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditLayerA?.FlipPropertySelected(FlipDirection.Veritcal);
			Editor.EditLayerB?.FlipPropertySelected(FlipDirection.Veritcal);
			Editor.UpdateEditLayerActions();
		}

		public void FlipVerticalIndividualToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditLayerA?.FlipPropertySelected(FlipDirection.Veritcal, true);
			Editor.EditLayerB?.FlipPropertySelected(FlipDirection.Veritcal, true);
			Editor.UpdateEditLayerActions();
		}

		#endregion

		#region View Tab Buttons

		public void statsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.DebugStatsVisibleOnPanel = !Editor.DebugStatsVisibleOnPanel;
			Editor.showStatsToolStripMenuItem.IsChecked = Editor.DebugStatsVisibleOnPanel;
		}

		public void PointerTooltipToggleToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.TooltipButton_Click(sender, e);
		}

		public void ResetZoomLevelToolstripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.SetZoomLevel(0, new Point(0, 0));
		}

		public void useLargeTextToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.UseLargeDebugStats = !Editor.UseLargeDebugStats;
			Editor.useLargeTextToolStripMenuItem.IsChecked = Editor.DebugStatsVisibleOnPanel;
		}

		public void SetMenuButtons(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
			if (menuItem != null)
			{
				if (menuItem.Tag != null)
				{
					var allItems = Editor.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
					foreach (System.Windows.Controls.MenuItem item in allItems)
					{
						item.IsChecked = false;
						var allSubItems = Editor.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
						foreach (System.Windows.Controls.MenuItem subItem in allSubItems)
						{
							subItem.IsChecked = false;
						}
					}
					string tag = menuItem.Tag.ToString();
					switch (tag)
					{
						case "Xbox":
							Editor.CurrentControllerButtons = 2;
							break;
						case "Switch":
							Editor.CurrentControllerButtons = 4;
							break;
						case "PS4":
							Editor.CurrentControllerButtons = 3;
							break;
						case "Saturn Black":
							Editor.CurrentControllerButtons = 5;
							break;
						case "Saturn White":
							Editor.CurrentControllerButtons = 6;
							break;
						case "Switch Joy L":
							Editor.CurrentControllerButtons = 7;
							break;
						case "Switch Joy R":
							Editor.CurrentControllerButtons = 8;
							break;
						case "PC EN/JP":
							Editor.CurrentControllerButtons = 1;
							break;
						case "PC FR":
							Editor.CurrentControllerButtons = 9;
							break;
						case "PC IT":
							Editor.CurrentControllerButtons = 10;
							break;
						case "PC GE":
							Editor.CurrentControllerButtons = 11;
							break;
						case "PC SP":
							Editor.CurrentControllerButtons = 12;
							break;
					}
					menuItem.IsChecked = true;
				}

			}

		}

		public void SetMenuButtons(string tag)
		{
			switch (tag)
			{
				case "Xbox":
					Editor.CurrentControllerButtons = 2;
					break;
				case "Switch":
					Editor.CurrentControllerButtons = 4;
					break;
				case "PS4":
					Editor.CurrentControllerButtons = 3;
					break;
				case "Saturn Black":
					Editor.CurrentControllerButtons = 5;
					break;
				case "Saturn White":
					Editor.CurrentControllerButtons = 6;
					break;
				case "Switch Joy L":
					Editor.CurrentControllerButtons = 7;
					break;
				case "Switch Joy R":
					Editor.CurrentControllerButtons = 8;
					break;
				case "PC EN/JP":
					Editor.CurrentControllerButtons = 1;
					break;
				case "PC FR":
					Editor.CurrentControllerButtons = 9;
					break;
				case "PC IT":
					Editor.CurrentControllerButtons = 10;
					break;
				case "PC GE":
					Editor.CurrentControllerButtons = 11;
					break;
				case "PC SP":
					Editor.CurrentControllerButtons = 12;
					break;
			}
		}

		public void ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.entityVisibilityType == 0)
			{
				Editor.showEntitiesAboveAllOtherLayersToolStripMenuItem.IsChecked = true;
				Editor.entityVisibilityType = 1;
			}
			else
			{
				Editor.showEntitiesAboveAllOtherLayersToolStripMenuItem.IsChecked = false;
				Editor.entityVisibilityType = 0;
			}

		}

		public void prioritizedViewingToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!mySettings.PrioritizedObjectRendering)
			{
				mySettings.PrioritizedObjectRendering = true;
				Editor.prioritizedViewingToolStripMenuItem.IsChecked = true;
			}
			else
			{
				mySettings.PrioritizedObjectRendering = false;
				Editor.prioritizedViewingToolStripMenuItem.IsChecked = false;
			}

		}

		public void ChangeEncorePaleteToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetEncorePallete(sender);
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
							Editor.EncoreSetupType = 0;
							if (File.Exists(Editor.EncorePalette[0]))
							{
								Editor.encorePaletteExists = true;
								Editor.EncorePaletteButton.IsChecked = true;
								Editor.useEncoreColors = true;
								Editor.ReloadSpecificTextures(null, null);
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
				Editor.EncoreSetupType = 0;
				if (File.Exists(Editor.EncorePalette[0]))
				{
					Editor.encorePaletteExists = true;
					Editor.EncorePaletteButton.IsChecked = true;
					Editor.useEncoreColors = true;
					Editor.ReloadSpecificTextures(null, null);
				}
				else
				{
					RSDKrU.MessageBox.Show("Unable to set Encore Colors. The Specified Path does not exist: " + Environment.NewLine + path);
				}
			}

		}

		public void MoveExtraLayersToFrontToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!Editor.extraLayersMoveToFront)
			{
				Editor.moveExtraLayersToFrontToolStripMenuItem.IsChecked = true;
				Editor.extraLayersMoveToFront = true;
			}
			else
			{
				Editor.moveExtraLayersToFrontToolStripMenuItem.IsChecked = false;
				Editor.extraLayersMoveToFront = false;
			}
		}

		public void ToolStripTextBox1_TextChanged(object sender, RoutedEventArgs e)
		{
			Editor.entitiesTextFilter = Editor.toolStripTextBox1.Text;
			Editor.entities.FilterRefreshNeeded = true;
		}

		public void ShowEntitySelectionBoxesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.showEntitySelectionBoxes)
			{
				Editor.showEntitySelectionBoxesToolStripMenuItem.IsChecked = false;
				Editor.showEntitySelectionBoxes = false;
			}
			else
			{
				Editor.showEntitySelectionBoxesToolStripMenuItem.IsChecked = true;
				Editor.showEntitySelectionBoxes = true;
			}
		}

		public void ShowWaterLevelToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!Editor.showWaterLevel)
			{
				Editor.showWaterLevelToolStripMenuItem.IsChecked = true;
				Editor.showWaterLevel = true;
			}
			else
			{
				Editor.showWaterLevelToolStripMenuItem.IsChecked = false;
				Editor.showWaterLevel = false;
			}
		}

		public void WaterLevelAlwaysShowItem_Click(object sender, RoutedEventArgs e)
		{

			if (!Editor.alwaysShowWaterLevel)
			{
				Editor.waterLevelAlwaysShowItem.IsChecked = true;
				Editor.alwaysShowWaterLevel = true;
			}
			else
			{
				Editor.waterLevelAlwaysShowItem.IsChecked = false;
				Editor.alwaysShowWaterLevel = false;
			}
		}

		public void SizeWithBoundsWhenNotSelectedToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!Editor.sizeWaterLevelwithBounds)
			{
				Editor.sizeWithBoundsWhenNotSelectedToolStripMenuItem.IsChecked = false;
				Editor.sizeWaterLevelwithBounds = true;
			}
			else
			{
				Editor.sizeWithBoundsWhenNotSelectedToolStripMenuItem.IsChecked = false;
				Editor.sizeWaterLevelwithBounds = false;
			}
		}

		public void ToggleEncoreManiaObjectVisibilityToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ToggleEncoreManiaEntitiesToolStripMenuItem_Click(sender, e);
		}

		public void ShowParallaxSpritesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.showParallaxSprites)
			{
				Editor.showEntityPathArrowsToolstripItem.IsChecked = false;
				Editor.showParallaxSprites = false;
			}
			else
			{
				Editor.showEntityPathArrowsToolstripItem.IsChecked = true;
				Editor.showParallaxSprites = true;
			}
		}

		public void XToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetScrollLockDirection();
		}

		public void YToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetScrollLockDirection();
		}

		public void SetScrollLockDirection()
		{
			if (Editor.ScrollDirection == (int)ScrollDir.X)
			{
				Editor.ScrollDirection = (int)ScrollDir.Y;
				Editor.UpdateStatusPanel(null, null);
				Editor.xToolStripMenuItem.IsChecked = false;
				Editor.yToolStripMenuItem.IsChecked = true;
			}
			else
			{
				Editor.ScrollDirection = (int)ScrollDir.X;
				Editor.UpdateStatusPanel(null, null);
				Editor.xToolStripMenuItem.IsChecked = true;
				Editor.yToolStripMenuItem.IsChecked = false;
			}
		}

		public void ShowEntityPathToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!Editor.showEntityPathArrows)
			{
				Editor.showEntityPathArrowsToolstripItem.IsChecked = true;
				Editor.showEntityPathArrows = true;
			}
			else
			{
				Editor.showEntityPathArrowsToolstripItem.IsChecked = false;
				Editor.showEntityPathArrows = false;
			}
		}

		public void LangToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var allLangItems = Editor.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
			foreach (var item in allLangItems) item.IsChecked = false;
			System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
			Editor.CurrentLanguage = menuItem.Tag.ToString();
			menuItem.IsChecked = true;
		}

		#region Collision Options
		public void DefaultToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.collisionPreset != 0)
			{
				Editor.invertedToolStripMenuItem.IsChecked = false;
				Editor.customToolStripMenuItem1.IsChecked = false;
				Editor.defaultToolStripMenuItem.IsChecked = true;
				Editor.collisionPreset = 0;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
			else
			{
				Editor.defaultToolStripMenuItem.IsChecked = true;
				Editor.invertedToolStripMenuItem.IsChecked = false;
				Editor.customToolStripMenuItem1.IsChecked = false;
				Editor.collisionPreset = 0;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
		}
		public void InvertedToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.collisionPreset != 1)
			{
				Editor.defaultToolStripMenuItem.IsChecked = false;
				Editor.customToolStripMenuItem1.IsChecked = false;
				Editor.invertedToolStripMenuItem.IsChecked = true;
				Editor.collisionPreset = 1;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
			else
			{
				Editor.defaultToolStripMenuItem.IsChecked = true;
				Editor.invertedToolStripMenuItem.IsChecked = false;
				Editor.customToolStripMenuItem1.IsChecked = false;
				Editor.collisionPreset = 0;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
		}
		public void CustomToolStripMenuItem1_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.collisionPreset != 2)
			{
				Editor.defaultToolStripMenuItem.IsChecked = false;
				Editor.invertedToolStripMenuItem.IsChecked = false;
				Editor.customToolStripMenuItem1.IsChecked = true;
				Editor.collisionPreset = 2;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
			else
			{
				Editor.defaultToolStripMenuItem.IsChecked = true;
				Editor.invertedToolStripMenuItem.IsChecked = false;
				Editor.customToolStripMenuItem1.IsChecked = false;
				Editor.collisionPreset = 0;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
		}
		public void CollisionOpacitySlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			if (Editor.collisionOpacityChanged)
			{
				Editor.collisionOpacityChanged = false;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
		}
		public void CollisionOpacitySlider_LostFocus(object sender, RoutedEventArgs e)
		{
			if (Editor.collisionOpacityChanged)
			{
				Editor.collisionOpacityChanged = false;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
		}
		public void CollisionOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Editor.collisionOpacityChanged = true;
		}
		#endregion

		#endregion

		#region Scene Tab Buttons
		public void ImportObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e, Window window = null)
		{
			Editor.importingObjects = true;
			try
			{
				Scene sourceScene = Editor.GetSceneSelection();
				if (sourceScene == null) return;
				var objectImporter = new ManiacEditor.Interfaces.ObjectImporter(sourceScene.Objects, Editor.EditorScene.Objects, Editor.StageConfig, Editor);
				if (window != null) objectImporter.Owner = window;
				objectImporter.ShowDialog();

				if (objectImporter.DialogResult != true)
					return; // nothing to do

				// user clicked Import, get to it!
				Editor.UpdateControls();
				Editor.entitiesToolbar?.RefreshSpawningObjects(Editor.EditorScene.Objects);

			}
			catch (Exception ex)
			{
				RSDKrU.MessageBox.Show("Unable to import Objects. " + ex.Message);
			}
			Editor.importingObjects = false;
		}

		public void ImportSoundsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ImportSoundsToolStripMenuItem_Click(sender, e, Window.GetWindow(Editor));
		}
		public void ImportSoundsToolStripMenuItem_Click(object sender, RoutedEventArgs e, Window window = null)
		{
			try
			{
				StageConfig sourceStageConfig = null;
				using (var fd = new OpenFileDialog())
				{
					fd.Filter = "Stage Config File|*.bin";
					fd.DefaultExt = ".bin";
					fd.Title = "Select Stage Config File";
					fd.InitialDirectory = Path.Combine(Editor.DataDirectory, "Stages");
					if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						try
						{
							sourceStageConfig = new StageConfig(fd.FileName);
						}
						catch
						{
							RSDKrU.MessageBox.Show("Ethier this isn't a stage config, or this stage config is ethier corrupted or unreadable in Maniac.");
							return;
						}

					}
				}
				if (null == sourceStageConfig) return;

				var soundImporter = new ManiacEditor.Interfaces.SoundImporter(sourceStageConfig, Editor.StageConfig);
				soundImporter.ShowDialog();

				if (soundImporter.DialogResult != true)
					return; // nothing to do


				// changing the sound list doesn't require us to do anything either

			}
			catch (Exception ex)
			{
				RSDKrU.MessageBox.Show("Unable to import sounds. " + ex.Message);
			}
		}

		public void LayerManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.Deselect(true);

			var lm = new ManiacEditor.Interfaces.LayerManager(Editor.EditorScene);
			lm.Owner = Window.GetWindow(Editor);
			lm.ShowDialog();

			Editor.SetupLayerButtons();
			Editor.ResetViewSize();
			Editor.UpdateControls();
		}

		public void PrimaryColorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ColorPickerDialog colorSelect = new ColorPickerDialog
			{
				Color = Color.FromArgb(Editor.EditorScene.EditorMetadata.BackgroundColor1.R, Editor.EditorScene.EditorMetadata.BackgroundColor1.G, Editor.EditorScene.EditorMetadata.BackgroundColor1.B)
			};
			Editor.UseExternalDarkTheme(colorSelect);
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				{
					RSDKv5.Color returnColor = new RSDKv5.Color
					{
						R = colorSelect.Color.R,
						A = colorSelect.Color.A,
						B = colorSelect.Color.B,
						G = colorSelect.Color.G
					};
					Editor.EditorScene.EditorMetadata.BackgroundColor1 = returnColor;
				}

			}
		}

		public void SecondaryColorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ColorPickerDialog colorSelect = new ColorPickerDialog
			{
				Color = Color.FromArgb(Editor.EditorScene.EditorMetadata.BackgroundColor2.R, Editor.EditorScene.EditorMetadata.BackgroundColor2.G, Editor.EditorScene.EditorMetadata.BackgroundColor2.B)
			};
			Editor.UseExternalDarkTheme(colorSelect);
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				{
					RSDKv5.Color returnColor = new RSDKv5.Color
					{
						R = colorSelect.Color.R,
						A = colorSelect.Color.A,
						B = colorSelect.Color.B,
						G = colorSelect.Color.G
					};
					Editor.EditorScene.EditorMetadata.BackgroundColor2 = returnColor;
				}

			}
		}

		#endregion

		#region Tools Tab Buttons
		public void OptimizeEntitySlotIDsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.EditorScene != null)
			{
				Editor.entities.OptimizeSlotIDs();
			}
		}
		public void RightClicktoSwapSlotIDs_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.rightClicktoSwapSlotIDs.IsChecked)
			{
				Editor.rightClicktoSwapSlotID = true;
			}
			else
			{
				Editor.rightClicktoSwapSlotID = false;
			}
		}
		public void CopyAirToggle_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.copyAirToggle.IsChecked)
			{
				Editor.CopyAir = true;
			}
			else
			{
				Editor.CopyAir = false;
			}
		}
		public void changeLevelIDToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string inputValue = RSDKrU.TextPrompt2.ShowDialog("Change Level ID", "This is only temporary and will reset when you reload the scene.", Editor.LevelID.ToString());
			int.TryParse(inputValue.ToString(), out int output);
			Editor.LevelID = output;
			Editor._levelIDLabel.Content = "Level ID: " + Editor.LevelID.ToString();
		}
		public void MakeForDataFolderOnlyToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string dataDir = Editor.DataDirectory;
			Editor.CreateShortcut(dataDir);
		}
		public void WithCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string dataDir = Editor.DataDirectory;
			string scenePath = Editor.Discord.ScenePath;
			int rX = (short)(Editor.ShiftX);
			int rY = (short)(Editor.ShiftY);
			double _ZoomLevel = Editor.ZoomLevel;
			bool isEncoreSet = Editor.useEncoreColors;
			int levelSlotNum = Editor.LevelID;
			Editor.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum, _ZoomLevel);
		}
		public void WithoutCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string dataDir = Editor.DataDirectory;
			string scenePath = Editor.Discord.ScenePath;
			int rX = 0;
			int rY = 0;
			bool isEncoreSet = Editor.useEncoreColors;
			int levelSlotNum = Editor.LevelID;
			Editor.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum);
		}
		public void MultiLayerSelectionToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.MultiLayerEditMode = !Editor.MultiLayerEditMode;
			Editor.multiLayerSelectionToolStripMenuItem.IsChecked = Editor.MultiLayerEditMode;


			bool enabled = (Editor.MultiLayerEditMode == true ? true : false);
			Editor.EditFGLower.DualSelect = enabled;
			Editor.EditFGLow.DualSelect = enabled;
			Editor.EditFGHigh.DualSelect = enabled;
			Editor.EditFGHigher.DualSelect = enabled;

			Editor.EditFGLower.SwapDefaultToA(!enabled);
			Editor.EditFGLow.SwapDefaultToA(!enabled);
			Editor.EditFGHigh.SwapDefaultToA(!enabled);
			Editor.EditFGHigher.SwapDefaultToA(!enabled);

			foreach (var elb in Editor.ExtraLayerEditViewButtons.Values)
			{
				elb.DualSelect = enabled;
				elb.SwapDefaultToA(!enabled);
			}

			if (!enabled) Editor.EditLayerB = null;

			Editor.UpdateControls();


		}
		public void FindUnusedTiles(object sender, RoutedEventArgs e)
		{
			Editor.FindAndReplace.FindUnusedTiles();
		}

		#region Developer Stuff
		public void GoToToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			GoToPositionBox form = new GoToPositionBox();
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
		public void PreLoadSceneButton_Click(object sender, RoutedEventArgs e)
		{
			//Disabled By Checking for Result OK
		}
		public void DeveloperTerminalToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var DevController = new ManiacEditor.Interfaces.DeveloperTerminal(Editor);
			DevController.Owner = Window.GetWindow(Editor);
			DevController.Show();
		}
		public void MD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Interfaces.WPF_UI.Options___Dev.MD5HashGen hashmap = new ManiacEditor.Interfaces.WPF_UI.Options___Dev.MD5HashGen(Editor);
			hashmap.Show();
		}
		public void PlayerSpawnToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.selectPlayerObject_GoTo = -1;
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
				if (Editor.selectPlayerObject_GoTo != -1)
				{
					int objectIndex = Editor.selectPlayerObject_GoTo;
					int xPos = (int)((int)Editor.playerObjectPosition[objectIndex].Position.X.High);
					int yPos = (int)((int)Editor.playerObjectPosition[objectIndex].Position.Y.High);
					Editor.GoToPosition(xPos, yPos);
				}
			}
		}
		public void FindToolStripMenuItem1_Click(object sender, RoutedEventArgs e)
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
					Editor.EditorTileFindReplace(find, replace, applyState, copyResults);//, perserveColllision
				}
				else
				{
					Editor.EditorTileFind(find, applyState, copyResults);
				}

			}

		}

		public void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!Editor.isConsoleWindowOpen)
			{
				Editor.isConsoleWindowOpen = true;
				ShowConsoleWindow();
			}
			else
			{
				Editor.isConsoleWindowOpen = false;
				HideConsoleWindow();
			}
		}
		public void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			mySettings.DevForceRestartData = Editor.DataDirectory;
			mySettings.DevForceRestartScene = Editor.EditorPath.SceneFilePath;
			mySettings.DevForceRestartX = (short)(Editor.ShiftX / Editor.Zoom);
			mySettings.DevForeRestartY = (short)(Editor.ShiftY / Editor.Zoom);
			mySettings.DevForceRestartZoomLevel = Editor.ZoomLevel;
			mySettings.DevForceRestartEncore = Editor.EditorPath.isEncoreMode;
			mySettings.DeveForceRestartLevelID = Editor.LevelID;
			mySettings.DevForceRestartCurrentName = Editor.EditorPath.CurrentName;
			mySettings.DevForceRestartCurrentZone = Editor.EditorPath.CurrentZone;
			mySettings.DevForceRestartCurrentSceneID = Editor.EditorPath.CurrentSceneID;
			mySettings.DevForceRestartBrowsed = Editor.EditorPath.Browsed;
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

		#region Apps Tab Buttons

		public void TileManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.mainform == null || Editor.mainform.IsClosed) Editor.mainform = new TileManiacWPF.MainWindow();
			Editor.mainform.Show();
			if (Editor.TilesConfig != null && Editor.EditorTiles.StageTiles != null)
			{
				if (Editor.mainform.Visibility != Visibility.Visible || Editor.mainform.tcf == null)
				{
					Editor.mainform.LoadTileConfigViaIntergration(Editor.TilesConfig, Editor.EditorPath.TileConfig_Source);
				}
				else
				{
					Editor.mainform.Activate();
				}

			}

		}
		public void InsanicManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			//Sanic2Maniac sanic = new Sanic2Maniac(null, this);
			//sanic.Show();
		}
		public void RSDKAnnimationEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			String aniProcessName = Path.GetFileNameWithoutExtension(mySettings.RunAniEdPath);
			IntPtr hWnd = FindWindow(aniProcessName, null); // this gives you the handle of the window you need.
			Process processes = Process.GetProcessesByName(aniProcessName).FirstOrDefault();
			if (processes != null)
			{
				// check if the window is hidden / minimized
				if (processes.MainWindowHandle == IntPtr.Zero)
				{
					// the window is hidden so try to restore it before setting focus.
					ShowWindow(processes.Handle, ShowWindowEnum.Restore);
				}

				// set user the focus to the window
				SetForegroundWindow(processes.MainWindowHandle);
			}
			else
			{

				// Ask where RSDK Annimation Editor is located when not set
				if (string.IsNullOrEmpty(mySettings.RunAniEdPath))
				{
					var ofd = new OpenFileDialog
					{
						Title = "Select RSDK Animation Editor.exe",
						Filter = "Windows Executable|*.exe"
					};
					if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						mySettings.RunAniEdPath = ofd.FileName;
				}
				else
				{
					if (!File.Exists(mySettings.RunGamePath))
					{
						mySettings.RunAniEdPath = "";
						return;
					}
				}

				ProcessStartInfo psi;
				psi = new ProcessStartInfo(mySettings.RunAniEdPath);
				Process.Start(psi);
			}
		}
		public void ColorPaletteEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.MenuItem button = sender as System.Windows.Controls.MenuItem;
			bool isGameConfig = false;
			bool GC_NULL = false;
			bool SC_NULL = false;
			string SC_Path = "";
			string GC_Path = "";

			if (button != null && button == Editor.maniaPalGameConfigToolStripMenuItem)
			{
				if (Editor.GameConfig == null) GC_NULL = true;
				else GC_Path = Editor.GameConfig.FilePath;
				isGameConfig = true;
			}
			else
			{
				if (Editor.StageConfig == null) SC_NULL = true;
				else SC_Path = Editor.StageConfig.FilePath;
				isGameConfig = false;
			}


			if (ManiaPalConnector == null) ManiaPalConnector = new ManiacED_ManiaPal.Connector();

			ManiaPalConnector.SetLoadingInformation(GC_Path, SC_Path, SC_NULL, GC_NULL);
			ManiaPalConnector.Activate(isGameConfig);



		}
		public void ManiaPalMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
		{
			Editor.maniaPalHint.Header = "HINT: The Button that houses this dropdown" + Environment.NewLine + "will focus ManiaPal if it is opened already" + Environment.NewLine + "(without reloading the currently loaded colors)";
		}
		public void DuplicateObjectIDHealerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = RSDKrU.MessageBox.Show("WARNING: Once you do this the editor will restart immediately, make sure your progress is closed and saved!", "WARNING", MessageBoxButton.OKCancel, MessageBoxImage.Information);
			if (result == MessageBoxResult.OK)
			{
				Editor.RepairScene();
			}
		}

		#endregion

		#region Folders Tab Buttons
		public void OpenSceneFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.EditorPath.SceneDirectory != null && Editor.EditorPath.SceneDirectory != "")
			{
				string SceneFilename_mod = Editor.EditorPath.SceneDirectory.Replace('/', '\\');
				Process.Start("explorer.exe", "/select, " + SceneFilename_mod);
			}
			else
			{
				RSDKrU.MessageBox.Show("Scene File does not exist or simply isn't loaded!", "ERROR");
			}

		}

		public void OpenDataDirectoryFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string DataDirectory_mod = Editor.DataDirectory.Replace('/', '\\');
			if (DataDirectory_mod != null && DataDirectory_mod != "" && Directory.Exists(DataDirectory_mod))
			{
				Process.Start("explorer.exe", "/select, " + DataDirectory_mod);
			}
			else
			{
				RSDKrU.MessageBox.Show("Data Directory does not exist or simply isn't loaded!", "ERROR");
			}

		}

		public void OpenSonicManiaFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (mySettings.RunGamePath != null && mySettings.RunGamePath != "" && File.Exists(mySettings.RunGamePath))
			{
				string GameFolder = mySettings.RunGamePath;
				string GameFolder_mod = GameFolder.Replace('/', '\\');
				Process.Start("explorer.exe", "/select, " + GameFolder_mod);
			}
			else
			{
				RSDKrU.MessageBox.Show("Game Folder does not exist or isn't set!", "ERROR");
			}

		}

		public void OpenModDataDirectoryToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			/*if (Editor.ModDataDirectory != "")
			{
				string ModDataDir = Editor.ModDataDirectory.Replace('/', '\\');
				Process.Start("explorer.exe", "/select, " + ModDataDir);
			}
			else
			{
				RSDKrU.MessageBox.Show("Mod Data Directory Not Loaded!", "ERROR");
			}*/


		}
		public void OpenASavedPlaceToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e)
		{
			if (Settings.mySettings.SavedPlaces != null && Settings.mySettings.SavedPlaces.Count > 0)
			{
				Editor.openASavedPlaceToolStripMenuItem.Items.Clear();
				var allItems = Editor.openASavedPlaceToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
				foreach (string savedPlace in Settings.mySettings.SavedPlaces)
				{
					var savedPlaceItem = new System.Windows.Controls.MenuItem()
					{
						Header = savedPlace,
						Tag = savedPlace
					};
					savedPlaceItem.Click += OpenASavedPlaceTrigger;
					Editor.openASavedPlaceToolStripMenuItem.Items.Add(savedPlaceItem);
				}
			}

		}

		public void OpenASavedPlaceTrigger(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;
			string savedPlaceDir = item.Header.ToString().Replace('/', '\\');
			Process.Start("explorer.exe", "/select, " + savedPlaceDir);
		}

		public void OpenASavedPlaceToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e)
		{
			Editor.openASavedPlaceToolStripMenuItem.Items.Clear();
			Editor.openASavedPlaceToolStripMenuItem.Items.Add("No Saved Places");
			//openASavedPlaceToolStripMenuItem.DropDownItems[0].IsEnabled = false;
		}

		#endregion

		#region Other Tab Buttons
		public void AboutToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var aboutBox = new ManiacEditor.Interfaces.AboutWindow();
			aboutBox.Owner = Window.GetWindow(Editor);
			aboutBox.ShowDialog();
		}

		public void WikiToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://docs.google.com/document/d/1NBvcqzvOzqeTVzgAYBR0ttAc5vLoFaQ4yh_cdf-7ceQ/edit?usp=sharing");
		}

		public void OptionToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			/*
            using (var optionBox = new OptionBox(this))
            {
                optionBox.ShowDialog();
            }
			*/
			var optionMenu = new ManiacEditor.Interfaces.OptionsMenu(Editor);
			optionMenu.Owner = Window.GetWindow(Editor);
			optionMenu.ShowDialog();
		}

		public void ControlsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var optionMenu = new ManiacEditor.Interfaces.OptionsMenu(Editor);
			optionMenu.Owner = Window.GetWindow(Editor);
			optionMenu.MainTabControl.SelectedIndex = 2;
			optionMenu.ShowDialog();
		}
		#endregion

		#region Main Toolstrip Buttons
		public void NewToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			New_Click(sender, e);
		}

		public void SToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Save_Click(sender, e);
		}

		public void MagnetMode_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.UseMagnetMode)
			{
				Editor.UseMagnetMode = false;
				Editor.MagnetMode.IsChecked = false;
			}
			else
			{
				Editor.UseMagnetMode = true;
				Editor.MagnetMode.IsChecked = true;
			}
		}

		public void UndoButton_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditorUndo();
		}

		public void RedoButton_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditorRedo();
		}

		public void ZoomInButton_Click(object sender, RoutedEventArgs e)
		{
			Editor.ZoomLevel += 1;
			if (Editor.ZoomLevel >= 5) Editor.ZoomLevel = 5;
			if (Editor.ZoomLevel <= -5) Editor.ZoomLevel = -5;

			Editor.SetZoomLevel(Editor.ZoomLevel, new Point(0, 0));
		}

		public void ZoomOutButton_Click(object sender, RoutedEventArgs e)
		{
			Editor.ZoomLevel -= 1;
			if (Editor.ZoomLevel >= 5) Editor.ZoomLevel = 5;
			if (Editor.ZoomLevel <= -5) Editor.ZoomLevel = -5;

			Editor.SetZoomLevel(Editor.ZoomLevel, new Point(0, 0));
		}

		public void SelectTool_Click(object sender, RoutedEventArgs e)
		{
			//SelectToolButton.IsChecked = !SelectToolButton.IsChecked;
			Editor.PointerButton.IsChecked = false;
			Editor.PlaceTilesButton.IsChecked = false;
			Editor.InteractionToolButton.IsChecked = false;
			Editor.UpdateControls();
		}

		public void PointerButton_Click(object sender, RoutedEventArgs e)
		{
			//PointerButton.IsChecked = !PointerButton.IsChecked;
			Editor.SelectToolButton.IsChecked = false;
			Editor.PlaceTilesButton.IsChecked = false;
			Editor.InteractionToolButton.IsChecked = false;
			Editor.UpdateControls();
		}

		public void PlaceTilesButton_Click(object sender, RoutedEventArgs e)
		{
			//PlaceTilesButton.IsChecked = !PlaceTilesButton.IsChecked;
			Editor.SelectToolButton.IsChecked = false;
			Editor.PointerButton.IsChecked = false;
			Editor.InteractionToolButton.IsChecked = false;
			Editor.UpdateControls();
		}

		public void InteractionToolButton_Click(object sender, RoutedEventArgs e)
		{
			//InteractionToolButton.IsChecked = !InteractionToolButton.IsChecked;
			Editor.PlaceTilesButton.IsChecked = false;
			Editor.SelectToolButton.IsChecked = false;
			Editor.PointerButton.IsChecked = false;
			Editor.UpdateControls();
		}

		public void ChunkToolButton_Click(object sender, RoutedEventArgs e)
		{
			//ChunksToolButton.IsChecked = !ChunksToolButton.IsChecked;
			Editor.UpdateControls();
		}

		public void ReloadToolStripButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// release all our resources, and force a reload of the tiles
				// Entities should take care of themselves
				Editor.DisposeTextures();
				Editor.EditorEntity_ini.ReleaseResources();
				//EditorEntity_ini.rendersWithErrors.Clear();

				//Reload for Encore Palletes, otherwise reload the image normally
				if (Editor.useEncoreColors == true)
				{
					Editor.EditorTiles.StageTiles?.Image.Reload(Editor.EncorePalette[0]);
					Editor.TilesToolbar?.Reload(Editor.EncorePalette[0]);
				}
				else
				{
					Editor.EditorTiles.StageTiles?.Image.Reload();
					Editor.TilesToolbar?.Reload();
				}

				Editor.TilesConfig = new TileConfig(Editor.EditorPath.TileConfig_Source);

				Editor.CollisionLayerA.Clear();
				Editor.CollisionLayerB.Clear();

				for (int i = 0; i < 1024; i++)
				{
					Editor.CollisionLayerA.Add(Editor.TilesConfig.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), Editor.CollisionAllSolid));
					Editor.CollisionLayerB.Add(Editor.TilesConfig.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), Editor.CollisionAllSolid));
				}



			}
			catch (Exception ex)
			{
				RSDKrU.MessageBox.Show(ex.Message);
			}
		}

		public void RunScene_Click(object sender, RoutedEventArgs e)
		{
			IntPtr hWnd = FindWindow("SonicMania", null); // this gives you the handle of the window you need.
			Process processes = Process.GetProcessesByName("SonicMania").FirstOrDefault();
			if (sender == Editor.RunSceneButton && Editor.GameRunning)
			{
				Editor.RunSceneDropDown.IsSubmenuOpen = true;
				return;
			}
			if (processes != null)
			{
				// check if the window is hidden / minimized
				if (processes.MainWindowHandle == IntPtr.Zero)
				{
					// the window is hidden so try to restore it before setting focus.
					ShowWindow(processes.Handle, ShowWindowEnum.Restore);
				}

				// set user the focus to the window
				SetForegroundWindow(processes.MainWindowHandle);
				if (!Editor.GameRunning)
				{
					Editor.Dispatcher.Invoke(new Action(() => RunSequence(sender, e, true)));
				}
			}
			else
			{
				if (!Editor.GameRunning)
				{
					if (mySettings.RunModLoaderPath != null && mySettings.modConfigs?.Count > 0)
					{
						string ConfigPath = mySettings.RunGamePath;
						System.Windows.Controls.MenuItem dropDownItem = Editor.selectConfigToolStripMenuItem.Items[0] as System.Windows.Controls.MenuItem;
						ConfigPath = ConfigPath.Replace('/', '\\');
						ConfigPath = ConfigPath.Replace("SonicMania.exe", "//mods//ManiaModLoader.ini");
						var allItems = Editor.selectConfigToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
						foreach (System.Windows.Controls.MenuItem item in allItems)
						{
							if (item.IsChecked)
							{
								dropDownItem = item;
							}
						}
						File.WriteAllText(ConfigPath, dropDownItem.Tag.ToString());
					}
					Editor.Dispatcher.Invoke(new Action(() => RunSequence(sender, e)));
				}

			}
		}

		public void RunSceneButton_DropDownOpening(object sender, RoutedEventArgs e)
		{
			Editor.trackThePlayerToolStripMenuItem.IsEnabled = Editor.GameRunning;
			Editor.assetResetToolStripMenuItem1.IsEnabled = Editor.GameRunning;
			Editor.moveThePlayerToHereToolStripMenuItem.IsEnabled = Editor.GameRunning;
			Editor.restartSceneToolStripMenuItem1.IsEnabled = Editor.GameRunning;
			Editor.selectConfigToolStripMenuItem.IsEnabled = !Editor.GameRunning;
		}

		public void ShowTileIDButton_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.showTileID == false)
			{
				Editor.ShowTileIDButton.IsChecked = true;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.showTileID = true;
			}
			else
			{
				Editor.ShowTileIDButton.IsChecked = false;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.showTileID = false;
			}
		}

		#region Grid Options
		public void ShowGridButton_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.showGrid == false)
			{
				Editor.ShowGridButton.IsChecked = true;
				Editor.showGrid = true;
				Editor.GridCheckStateCheck();

			}
			else
			{
				Editor.ShowGridButton.IsChecked = false;
				Editor.showGrid = false;
			}
		}
		public void GridCheckStateCheck()
		{
			if (Editor.x16ToolStripMenuItem.IsChecked == true)
			{
				Editor.EditorBackground.GRID_TILE_SIZE = 16;
			}
			if (Editor.x128ToolStripMenuItem.IsChecked == true)
			{
				Editor.EditorBackground.GRID_TILE_SIZE = 128;
			}
			if (Editor.x256ToolStripMenuItem.IsChecked == true)
			{
				Editor.EditorBackground.GRID_TILE_SIZE = 256;
			}
			if (Editor.customToolStripMenuItem.IsChecked == true)
			{
				Editor.EditorBackground.GRID_TILE_SIZE = mySettings.CustomGridSizeValue;
			}
		}
		public void X16ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditorBackground.GRID_TILE_SIZE = 16;
			ResetGridOptions();
			Editor.x16ToolStripMenuItem.IsChecked = true;
		}
		public void X128ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditorBackground.GRID_TILE_SIZE = 128;
			ResetGridOptions();
			Editor.x128ToolStripMenuItem.IsChecked = true;
		}
		public void ResetGridOptions()
		{
			Editor.x16ToolStripMenuItem.IsChecked = false;
			Editor.x128ToolStripMenuItem.IsChecked = false;
			Editor.x256ToolStripMenuItem.IsChecked = false;
			Editor.customToolStripMenuItem.IsChecked = false;
		}
		public void X256ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditorBackground.GRID_TILE_SIZE = 256;
			ResetGridOptions();
			Editor.x256ToolStripMenuItem.IsChecked = true;
		}
		public void CustomToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.EditorBackground.GRID_TILE_SIZE = mySettings.CustomGridSizeValue;
			ResetGridOptions();
			Editor.customToolStripMenuItem.IsChecked = true;
		}
		#endregion

		public void ShowCollisionAButton_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.showCollisionA == false)
			{
				Editor.ShowCollisionAButton.IsChecked = true;
				Editor.showCollisionA = true;
				Editor.ShowCollisionBButton.IsChecked = false;
				Editor.showCollisionB = false;
				Editor.ReloadSpecificTextures(sender, e);
			}
			else
			{
				Editor.ShowCollisionAButton.IsChecked = false;
				Editor.showCollisionA = false;
				Editor.ShowCollisionBButton.IsChecked = false;
				Editor.showCollisionB = false;
				Editor.ReloadSpecificTextures(sender, e);
			}
		}

		public void ShowCollisionBButton_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.showCollisionB == false)
			{
				Editor.ShowCollisionBButton.IsChecked = true;
				Editor.showCollisionB = true;
				Editor.ShowCollisionAButton.IsChecked = false;
				Editor.showCollisionA = false;
				Editor.ReloadSpecificTextures(sender, e);
			}
			else
			{
				Editor.ShowCollisionBButton.IsChecked = false;
				Editor.showCollisionB = false;
				Editor.ShowCollisionAButton.IsChecked = false;
				Editor.showCollisionA = false;
				Editor.ReloadSpecificTextures(sender, e);
			}
		}

		public void OpenDataDirectoryMenuButton(object sender, RoutedEventArgs e)
		{
			if (Editor._recentDataItems != null)
			{
				string dataDirectory = Editor._recentDataItems[1].Tag.ToString();
				if (dataDirectory != null || dataDirectory != "")
				{
					Editor.RecentDataDirectoryClicked(sender, e, dataDirectory);
				}
			}
		}
		public void ResetDeviceButton_Click_1(object sender, RoutedEventArgs e)
		{
			if (Editor.editorView.GraphicPanel.bRender)
			{
				Editor.editorView.GraphicPanel.bRender = false;
			}
			else
			{
				ReloadToolStripButton_Click(null, null);
				Editor.editorView.GraphicPanel.bRender = true;
			}
		}

		public void ShowFlippedTileHelper_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.showFlippedTileHelper == false)
			{
				Editor.ReloadSpecificTextures(sender, e);
				Editor.showFlippedTileHelper = true;

			}
			else
			{
				Editor.ReloadSpecificTextures(sender, e);
				Editor.showFlippedTileHelper = false;
			}
		}

		public void EnableEncorePalette_Click(bool? manualEnable = null)
		{
			Editor.DisposeTextures();
			bool mode = (manualEnable != null ? (bool)manualEnable : !Editor.useEncoreColors);
			if (mode == true)
			{
				Editor.EncorePaletteButton.IsChecked = true;
				Editor.useEncoreColors = true;
				Editor.EditorTiles.StageTiles?.Image.Reload(Editor.EncorePalette[0]);
				Editor.TilesToolbar?.Reload(Editor.EncorePalette[0]);
			}
			else
			{
				Editor.EncorePaletteButton.IsChecked = false;
				Editor.useEncoreColors = false;
				Editor.EditorTiles.StageTiles?.Image.Reload();
				Editor.TilesToolbar?.Reload();
			}
			Editor.EditorEntity_ini.ReleaseResources();
		}


		#endregion

		#region Status Bar Items
		public void PixelModeButton_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.EnablePixelCountMode == false)
			{
				Editor.pixelModeButton.IsChecked = true;
				Editor.pixelModeToolStripMenuItem.IsChecked = true;
				Editor.EnablePixelCountMode = true;

			}
			else
			{
				Editor.pixelModeButton.IsChecked = false;
				Editor.pixelModeToolStripMenuItem.IsChecked = false;
				Editor.EnablePixelCountMode = false;
			}

		}
		public void TooltipButton_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.showMouseTooltip == false)
			{
				Editor.PointerTooltipToggleToolStripMenuItem.IsChecked = true;
				Editor.tooltipButton.IsChecked = true;
				Editor.showMouseTooltip = true;

			}
			else
			{
				Editor.PointerTooltipToggleToolStripMenuItem.IsChecked = false;
				Editor.tooltipButton.IsChecked = false;
				Editor.showMouseTooltip = false;
			}
		}
		public void ScrollLockButton_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.ScrollLocked == false)
			{
				Editor.scrollLockButton.IsChecked = true;
				Editor.ScrollLocked = true;
				Editor.statusNAToolStripMenuItem.IsChecked = true;
			}
			else
			{
				Editor.scrollLockButton.IsChecked = false;
				Editor.statusNAToolStripMenuItem.IsChecked = false;
				Editor.ScrollLocked = false;
			}

		}
		public void NudgeFasterButton_Click(object sender, RoutedEventArgs e)
		{
			if (mySettings.EnableFasterNudge == false)
			{
				Editor.nudgeFasterButton.IsChecked = true;
				Editor.nudgeSelectionFasterToolStripMenuItem.IsChecked = true;
				mySettings.EnableFasterNudge = true;
			}
			else
			{
				Editor.nudgeFasterButton.IsChecked = false;
				Editor.nudgeSelectionFasterToolStripMenuItem.IsChecked = false;
				mySettings.EnableFasterNudge = false;
			}
		}

		#region Quick Button Buttons
		public void MoreSettingsButton_ButtonClick(object sender, RoutedEventArgs e)
		{
			switch (Editor.LastQuickButtonState)
			{
				case 1:
					SwapScrollLockDirectionToolStripMenuItem_Click(sender, e);
					break;
				case 2:
					EditEntitesTransparencyToolStripMenuItem_Click(sender, e);
					break;
				case 3:
					ToggleEncoreManiaEntitiesToolStripMenuItem_Click(sender, e);
					break;
				default:
					SwapScrollLockDirectionToolStripMenuItem_Click(sender, e);
					break;


			}
		}
		public void SwapScrollLockDirectionToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.LastQuickButtonState = 1;
			XToolStripMenuItem_Click(sender, e);
		}
		public void EditEntitesTransparencyToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (sender != Editor.transparentLayersForEditingEntitiesToolStripMenuItem)
			{
				Editor.LastQuickButtonState = 2;
			}
			if (Editor.applyEditEntitiesTransparency == false)
			{
				Editor.applyEditEntitiesTransparency = true;
				Editor.transparentLayersForEditingEntitiesToolStripMenuItem.IsChecked = true;
				Editor.editEntitesTransparencyToolStripMenuItem.IsChecked = true;
			}
			else
			{
				Editor.applyEditEntitiesTransparency = false;
				Editor.transparentLayersForEditingEntitiesToolStripMenuItem.IsChecked = false;
				Editor.editEntitesTransparencyToolStripMenuItem.IsChecked = false;
			}
		}
		public void ToggleEncoreManiaEntitiesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.LastQuickButtonState = 3;
			if (mySettings.showEncoreEntities == true && mySettings.showManiaEntities == true)
			{
				mySettings.showManiaEntities = true;
				mySettings.showEncoreEntities = false;
			}
			if (mySettings.showEncoreEntities == true && mySettings.showManiaEntities == false)
			{
				mySettings.showManiaEntities = true;
				mySettings.showEncoreEntities = false;
			}
			else
			{
				mySettings.showManiaEntities = false;
				mySettings.showEncoreEntities = true;
			}

		}
		#endregion

		#endregion



		#region Run Mania Sequence

		// TODO: Perfect Scene Autobooting
		public void RunSequence(object sender, EventArgs e, bool attachMode = false)
		{
			// Ask where Sonic Mania is located when not set
			string path = "steam://run/584400";
			bool ready = false;
			if (mySettings.UsePrePlusOffsets)
			{
				if (string.IsNullOrEmpty(mySettings.RunGamePath))
				{
					var ofd = new OpenFileDialog
					{
						Title = "Select SonicMania.exe",
						Filter = "Windows PE Executable|*.exe"
					};
					if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						mySettings.RunGamePath = ofd.FileName;
				}
				else
				{
					if (!File.Exists(mySettings.RunGamePath))
					{
						mySettings.RunGamePath = "";
						return;
					}
				}
				path = mySettings.RunGamePath;
			}
			ProcessStartInfo psi;

			if (mySettings.RunGameInsteadOfScene)
			{
				psi = new ProcessStartInfo(path);
			}
			else
			{
				//if (mySettings.UsePrePlusOffsets == true)
				//{
				//	//psi = new ProcessStartInfo(path, $"stage={Editor.SelectedZone};scene={Editor.SelectedScene[5]};");
				//}
				//else
				//{
					psi = new ProcessStartInfo(path);
				//}

			}
			if (path != "" || attachMode)
			{
				string maniaDir = Path.GetDirectoryName(path);
				// Check if the mod loader is installed
				string modLoaderDLL = maniaDir + "//d3d9.dll";
				if (File.Exists(modLoaderDLL))
					psi.WorkingDirectory = maniaDir;
				else
					psi.WorkingDirectory = Path.GetDirectoryName(Editor.DataDirectory);
				Process p;
				if (!attachMode)
				{
					p = Process.Start(psi);
				}
				else
				{
					var mania = Process.GetProcessesByName("SonicMania.exe");
					p = mania.FirstOrDefault();
				}
				Editor.GameRunning = true;

				int CurrentScene_ptr = 0x00E48758;          // &CurrentScene
				int GameState_ptr = 0x00E48776;             // &GameState
				int IsGameRunning_ptr = 0x0065D1C8;
				int Player1_ControllerID_ptr = 0x0085EB44;  // &Player1.ControllerID
				int Player2_ControllerID_ptr = 0x0085EF9C;  // &Player2.ControllerID
				if (mySettings.UsePrePlusOffsets)
				{
					CurrentScene_ptr = 0x00CCF6F8;
					// TODO: Get Pre Plus GameState address
					IsGameRunning_ptr = 0x00628094;
					Player1_ControllerID_ptr = 0x00A4C860;
				}

				if (mySettings.UsePrePlusOffsets)
				{
					Editor.UpdateControls();
					UseCheatCodes(p);
					ready = true;
				}
				else
				{

					// For Mania Plus, The best way to boot the game is by using the steam command.
					// After Calling the Steam command, We need to wait until Steam responds and Starts the game.
					// Once the game process starts up, We quickly attach to it and apply all the needed patches

					// Wait for Steam to complete startup
					new Thread(() =>
					{
						Process[] Procs;
						while ((Procs = Process.GetProcessesByName("SonicMania")).Length == 0)
							Thread.Sleep(1);
						Editor.Dispatcher.Invoke(new Action(() =>
						{
							p = Procs[0];
							// Attach and Apply Cheats
							UseCheatCodes(p);
							Editor.UpdateControls();
							ready = true;


							// Wait until there is a Running Scene.
							while (Editor.GameMemory.ReadByte(GameState_ptr) != 0x01)
								Thread.Sleep(1);

							// Swap the Scene
							if (Editor.LevelID != -1)
							{
								Editor.GameMemory.WriteByte(CurrentScene_ptr, (byte)Editor.LevelID);
								// Restart the Scene
								Editor.GameMemory.WriteByte(GameState_ptr, 0);
							}



						}));
					}).Start();
				}


				new Thread(() =>
				{
					while (!ready)
						Thread.Sleep(10);
					/* Level != Main Menu*/
					while (Editor.GameMemory.ReadByte(CurrentScene_ptr) != 0x02 || Properties.Settings.Default.DisableRunSceneMenuQuit == true)
					{
						// Check if the user closed the game
						if (p.HasExited || !Editor.GameRunning)
						{
							Editor.GameRunning = false;
							if (Editor.IsVisible)
							{
								Editor.Dispatcher.Invoke(new Action(() => Editor.UpdateControls()));
							}
							return;
						}
						UseCheatCodes(p);
						// Makes sure the process is attached and patches are applied
						// Set Player 1 Controller Set to 1 (If we set it to AnyController (0x00) we can't use Debug Mode In-Game)
						if (Editor.GameMemory.ReadByte(Player1_ControllerID_ptr) != 0x00 && Properties.Settings.Default.DisableRunSceneAutoInput == false)
						{
							Editor.GameMemory.WriteByte(Player1_ControllerID_ptr, 0x00); //setting this to 0x00 causes the inability to use debug mode
							Editor.GameMemory.WriteByte(Player2_ControllerID_ptr, 0xFF);
						}
						Thread.Sleep(300);
					}
					// User is on the Main Menu
					// Close the game
					Editor.GameMemory.WriteByte(IsGameRunning_ptr, 0);
					Editor.GameRunning = false;
					Editor.Dispatcher.Invoke(new Action(() => Editor.UpdateControls()));
				}).Start();
			}
		}

		public void UseCheatCodes(Process p)
		{
			if (mySettings.UsePrePlusOffsets)
			{
				// Patches
				Editor.GameMemory.Attach(p);
			}
			else
			{
				Editor.GameMemory.Attach(p);

				// Mania Plus Patches
				Editor.GameMemory.WriteByte(0x00E48768, 0x01); // Enable Debug
				Editor.GameMemory.WriteByte(0x006F1806, 0x01); // Allow DevMenu
				Editor.GameMemory.WriteByte(0x005FDD00, 0xEB); // Disable Background Pausing
			}
		}

		#endregion
	}
}
