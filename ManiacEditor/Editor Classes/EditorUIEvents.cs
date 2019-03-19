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

		public void ObjectManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var objectManager = new ManiacEditor.Interfaces.ObjectManager(Editor.EditorScene.Objects, Editor.StageConfig, Editor);
			objectManager.Owner = Window.GetWindow(Editor);
			objectManager.ShowDialog();
		}

		#region Backup SubMenu
		public void StageConfigToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.UIModes.BackupType = 4;
			BackupToolStripMenuItem_Click(null, null);
			Editor.UIModes.BackupType = 0;
		}

		public void NormalToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.UIModes.BackupType = 1;
			BackupToolStripMenuItem_Click(null, null);
			Editor.UIModes.BackupType = 0;
		}
		#endregion

		#endregion

		#region Edit Tab Buttons

		public void chunkToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.TilesClipboard != null)
			{
				Editor.Chunks.ConvertClipboardtoMultiLayerChunk(Editor.TilesClipboard.Item1, Editor.TilesClipboard.Item2);
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
				Editor.Entities.Select(new Rectangle(0, 0, 32768, 32768), true, false);
			}
			Editor.UI.SetSelectOnlyButtonsState();
			Editor.StateModel.ClickedX = -1;
			Editor.StateModel.ClickedY = -1;
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


			Editor.UI.UpdateControls();
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
					Editor.Entities.PasteFromClipboard(new Point(16, 16), Editor.Entities.CopyToClipboard(true));
					Editor.UpdateLastEntityAction();
				}
				catch (EditorEntities.TooManyEntitiesException)
				{
					RSDKrU.MessageBox.Show("Too many entities! (limit: 2048)");
					return;
				}
				Editor.UpdateEntitiesToolbarList();
				Editor.UI.SetSelectOnlyButtonsState();
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
				Editor.UI.UpdateControls();
				Editor.UpdateEditLayerActions();
			}
			else if (Editor.IsEntitiesEdit())
			{
				if (Editor.EntitiesToolbar.IsFocused.Equals(false))
				{
					Editor.CopyEntitiesToClipboard();
					Editor.DeleteSelected();
					Editor.UI.UpdateControls();
					Editor.UpdateEntitiesToolbarList();
				}
			}
		}

		public void PasteToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.IsTilesEdit())
			{
				// check if there are tiles on the Windows clipboard; if so, use those
				if (Settings.mySettings.EnableWindowsClipboard && System.Windows.Clipboard.ContainsData("ManiacTiles"))
				{
					var pasteData = (Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>) System.Windows.Clipboard.GetDataObject().GetData("ManiacTiles");
					Point pastePoint = new Point((int)(Editor.StateModel.lastX / Editor.StateModel.Zoom) + EditorConstants.TILE_SIZE - 1, (int)(Editor.StateModel.lastY / Editor.StateModel.Zoom) + EditorConstants.TILE_SIZE - 1);
					if (Editor.EditLayerA != null) Editor.EditLayerA.PasteFromClipboard(pastePoint, pasteData.Item1);
					if (Editor.EditLayerB != null) Editor.EditLayerB.PasteFromClipboard(pastePoint, pasteData.Item2);

					Editor.UpdateEditLayerActions();
				}

				// if there's none, use the internal clipboard
				else if (Editor.TilesClipboard != null)
				{
					Point pastePoint = new Point((int)(Editor.StateModel.lastX / Editor.StateModel.Zoom) + EditorConstants.TILE_SIZE - 1, (int)(Editor.StateModel.lastY / Editor.StateModel.Zoom) + EditorConstants.TILE_SIZE - 1);
					if (Editor.EditLayerA != null) Editor.EditLayerA.PasteFromClipboard(pastePoint, Editor.TilesClipboard.Item1);
					if (Editor.EditLayerB != null) Editor.EditLayerB.PasteFromClipboard(pastePoint, Editor.TilesClipboard.Item2);
					Editor.UpdateEditLayerActions();
				}

			}
			else if (Editor.IsEntitiesEdit())
			{
				if (Editor.EntitiesToolbar.IsFocused.Equals(false))
				{
					try
					{

						// check if there are entities on the Windows clipboard; if so, use those
						if (Settings.mySettings.EnableWindowsClipboard && System.Windows.Clipboard.ContainsData("ManiacEntities"))
						{

							Editor.Entities.PasteFromClipboard(new Point((int)(Editor.StateModel.lastX / Editor.StateModel.Zoom), (int)(Editor.StateModel.lastY / Editor.StateModel.Zoom)), (List<EditorEntity>)System.Windows.Clipboard.GetDataObject().GetData("ManiacEntities"));
							Editor.UpdateLastEntityAction();
						}

						// if there's none, use the internal clipboard
						else if (Editor.entitiesClipboard != null)
						{
							Editor.Entities.PasteFromClipboard(new Point((int)(Editor.StateModel.lastX / Editor.StateModel.Zoom), (int)(Editor.StateModel.lastY / Editor.StateModel.Zoom)), Editor.entitiesClipboard);
							Editor.UpdateLastEntityAction();
						}
					}
					catch (EditorEntities.TooManyEntitiesException)
					{
						RSDKrU.MessageBox.Show("Too many entities! (limit: 2048)");
						return;
					}
					Editor.UpdateEntitiesToolbarList();
					Editor.UI.SetSelectOnlyButtonsState();
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

		public void SetMenuButtons(string tag)
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

		public void ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.UIModes.entityVisibilityType == 0)
			{
				Editor.showEntitiesAboveAllOtherLayersToolStripMenuItem.IsChecked = true;
                Editor.UIModes.entityVisibilityType = 1;
			}
			else
			{
				Editor.showEntitiesAboveAllOtherLayersToolStripMenuItem.IsChecked = false;
                Editor.UIModes.entityVisibilityType = 0;
			}

		}

		public void ChangeEncorePaleteToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{

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

		public void ToolStripTextBox1_TextChanged(object sender, TextChangedEventArgs e)
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

		public void ToggleEncoreManiaObjectVisibilityToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ToggleEncoreManiaEntitiesToolStripMenuItem_Click(sender, e);
		}

		public void SetScrollLockDirection()
		{
			if (Editor.UIModes.ScrollDirection == (int)ScrollDir.X)
			{
				Editor.UIModes.ScrollDirection = (int)ScrollDir.Y;
				Editor.UpdateStatusPanel(null, null);
				Editor.xToolStripMenuItem.IsChecked = false;
				Editor.yToolStripMenuItem.IsChecked = true;
			}
			else
			{
				Editor.UIModes.ScrollDirection = (int)ScrollDir.X;
				Editor.UpdateStatusPanel(null, null);
				Editor.xToolStripMenuItem.IsChecked = true;
				Editor.yToolStripMenuItem.IsChecked = false;
			}
		}

		public void LangToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var allLangItems = Editor.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
			foreach (var item in allLangItems) item.IsChecked = false;
			System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
			Editor.UIModes.CurrentLanguage = menuItem.Tag.ToString();
			menuItem.IsChecked = true;
		}

		#region Collision Options
		public void DefaultToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.CollisionPreset != 0)
			{
				Editor.invertedToolStripMenuItem.IsChecked = false;
				Editor.customToolStripMenuItem1.IsChecked = false;
				Editor.defaultToolStripMenuItem.IsChecked = true;
				Editor.CollisionPreset = 0;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
			else
			{
				Editor.defaultToolStripMenuItem.IsChecked = true;
				Editor.invertedToolStripMenuItem.IsChecked = false;
				Editor.customToolStripMenuItem1.IsChecked = false;
				Editor.CollisionPreset = 0;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
		}
		public void InvertedToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.CollisionPreset != 1)
			{
				Editor.defaultToolStripMenuItem.IsChecked = false;
				Editor.customToolStripMenuItem1.IsChecked = false;
				Editor.invertedToolStripMenuItem.IsChecked = true;
				Editor.CollisionPreset = 1;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
			else
			{
				Editor.defaultToolStripMenuItem.IsChecked = true;
				Editor.invertedToolStripMenuItem.IsChecked = false;
				Editor.customToolStripMenuItem1.IsChecked = false;
				Editor.CollisionPreset = 0;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
		}
		public void CustomToolStripMenuItem1_Click(object sender, RoutedEventArgs e)
		{
			if (Editor.CollisionPreset != 2)
			{
				Editor.defaultToolStripMenuItem.IsChecked = false;
				Editor.invertedToolStripMenuItem.IsChecked = false;
				Editor.customToolStripMenuItem1.IsChecked = true;
				Editor.CollisionPreset = 2;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
			else
			{
				Editor.defaultToolStripMenuItem.IsChecked = true;
				Editor.invertedToolStripMenuItem.IsChecked = false;
				Editor.customToolStripMenuItem1.IsChecked = false;
				Editor.CollisionPreset = 0;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
		}
		public void CollisionOpacitySlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			if (Editor.UIModes.collisionOpacityChanged)
			{
				Editor.UIModes.collisionOpacityChanged = false;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
		}
		public void CollisionOpacitySlider_LostFocus(object sender, RoutedEventArgs e)
		{
			if (Editor.UIModes.collisionOpacityChanged)
			{
				Editor.UIModes.collisionOpacityChanged = false;
				Editor.ReloadSpecificTextures(sender, e);
				Editor.RefreshCollisionColours(true);
			}
		}
		public void CollisionOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Editor.UIModes.collisionOpacityChanged = true;
		}
		#endregion

		#endregion

		#region Scene Tab Buttons
		public void ImportObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e, Window window = null)
		{
			Editor.UIModes.importingObjects = true;
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
				Editor.UI.UpdateControls();
				Editor.EntitiesToolbar?.RefreshSpawningObjects(Editor.EditorScene.Objects);

			}
			catch (Exception ex)
			{
				RSDKrU.MessageBox.Show("Unable to import Objects. " + ex.Message);
			}
			Editor.UIModes.importingObjects = false;
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

        public void ManiacinieditorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacINIEditor editor = new ManiacINIEditor(Editor);
            if (editor.Owner != null) editor.Owner = Window.GetWindow(Editor);
            else editor.Owner = System.Windows.Application.Current.MainWindow;
            editor.ShowDialog();
        }

        public void LayerManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.Deselect(true);

			var lm = new ManiacEditor.Interfaces.LayerManager(Editor.EditorScene);
			lm.Owner = Window.GetWindow(Editor);
			lm.ShowDialog();

			Editor.SetupLayerButtons();
			Editor.ResetViewSize();
			Editor.UI.UpdateControls();
		}

		public void PrimaryColorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ColorPickerDialog colorSelect = new ColorPickerDialog
			{
				Color = Color.FromArgb(Editor.EditorScene.EditorMetadata.BackgroundColor1.R, Editor.EditorScene.EditorMetadata.BackgroundColor1.G, Editor.EditorScene.EditorMetadata.BackgroundColor1.B)
			};
			Editor.Theming.UseExternalDarkTheme(colorSelect);
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
			Editor.Theming.UseExternalDarkTheme(colorSelect);
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
		public void changeLevelIDToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string inputValue = RSDKrU.TextPrompt2.ShowDialog("Change Level ID", "This is only temporary and will reset when you reload the scene.", Editor.UIModes.LevelID.ToString());
			int.TryParse(inputValue.ToString(), out int output);
			Editor.UIModes.LevelID = output;
			Editor._levelIDLabel.Content = "Level ID: " + Editor.UIModes.LevelID.ToString();
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
			int rX = (short)(Editor.StateModel.ShiftX);
			int rY = (short)(Editor.StateModel.ShiftY);
			double _ZoomLevel = Editor.StateModel.ZoomLevel;
			bool isEncoreSet = Editor.UIModes.UseEncoreColors;
			int levelSlotNum = Editor.UIModes.LevelID;
			Editor.CreateShortcut(dataDir, scenePath, "", rX, rY, isEncoreSet, levelSlotNum, _ZoomLevel);
		}
		public void WithoutCurrentCoordinatesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
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
		public void GoToToolStripMenuItem_Click(object sender, RoutedEventArgs e)
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
			Settings.mySettings.DevForceRestartData = Editor.DataDirectory;
			Settings.mySettings.DevForceRestartScene = Editor.Paths.SceneFilePath;
			Settings.mySettings.DevForceRestartX = (short)(Editor.StateModel.ShiftX / Editor.StateModel.Zoom);
			Settings.mySettings.DevForeRestartY = (short)(Editor.StateModel.ShiftY / Editor.StateModel.Zoom);
			Settings.mySettings.DevForceRestartZoomLevel = Editor.StateModel.ZoomLevel;
			Settings.mySettings.DevForceRestartEncore = Editor.Paths.isEncoreMode;
			Settings.mySettings.DeveForceRestartLevelID = Editor.UIModes.LevelID;
			Settings.mySettings.DevForceRestartCurrentName = Editor.Paths.CurrentName;
			Settings.mySettings.DevForceRestartCurrentZone = Editor.Paths.CurrentZone;
			Settings.mySettings.DevForceRestartCurrentSceneID = Editor.Paths.CurrentSceneID;
			Settings.mySettings.DevForceRestartBrowsed = Editor.Paths.Browsed;
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

		public void ZoomInButton_Click(object sender, RoutedEventArgs e)
		{
			Editor.StateModel.ZoomLevel += 1;
			if (Editor.StateModel.ZoomLevel >= 5) Editor.StateModel.ZoomLevel = 5;
			if (Editor.StateModel.ZoomLevel <= -5) Editor.StateModel.ZoomLevel = -5;

			Editor.SetZoomLevel(Editor.StateModel.ZoomLevel, new Point(0, 0));
		}

		public void ZoomOutButton_Click(object sender, RoutedEventArgs e)
		{
			Editor.StateModel.ZoomLevel -= 1;
			if (Editor.StateModel.ZoomLevel >= 5) Editor.StateModel.ZoomLevel = 5;
			if (Editor.StateModel.ZoomLevel <= -5) Editor.StateModel.ZoomLevel = -5;

			Editor.SetZoomLevel(Editor.StateModel.ZoomLevel, new Point(0, 0));
		}

		public void RunSceneButton_DropDownOpening(object sender, RoutedEventArgs e)
		{
			Editor.trackThePlayerToolStripMenuItem.IsEnabled = Editor.InGame.GameRunning;
			Editor.assetResetToolStripMenuItem1.IsEnabled = Editor.InGame.GameRunning;
			Editor.moveThePlayerToHereToolStripMenuItem.IsEnabled = Editor.InGame.GameRunning;
			Editor.restartSceneToolStripMenuItem1.IsEnabled = Editor.InGame.GameRunning;
			Editor.selectConfigToolStripMenuItem.IsEnabled = !Editor.InGame.GameRunning;
		}

        public void GameOptionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CheatCodeManager cheatCodeManager = new CheatCodeManager();
            cheatCodeManager.Owner = Editor;
            cheatCodeManager.ShowDialog();
        }

        #region Grid Options
        public void GridCheckStateCheck()
		{
			if (Editor.x16ToolStripMenuItem.IsChecked == true)
			{
				EditorConstants.GRID_TILE_SIZE = 16;
			}
			if (Editor.x128ToolStripMenuItem.IsChecked == true)
			{
				EditorConstants.GRID_TILE_SIZE = 128;
			}
			if (Editor.x256ToolStripMenuItem.IsChecked == true)
			{
				EditorConstants.GRID_TILE_SIZE = 256;
			}
			if (Editor.customToolStripMenuItem.IsChecked == true)
			{
				EditorConstants.GRID_TILE_SIZE = Settings.mySettings.CustomGridSizeValue;
			}
		}
		public void X16ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditorConstants.GRID_TILE_SIZE = 16;
			ResetGridOptions();
			Editor.x16ToolStripMenuItem.IsChecked = true;
		}
		public void X128ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditorConstants.GRID_TILE_SIZE = 128;
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
			EditorConstants.GRID_TILE_SIZE = 256;
			ResetGridOptions();
			Editor.x256ToolStripMenuItem.IsChecked = true;
		}
		public void CustomToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditorConstants.GRID_TILE_SIZE = Settings.mySettings.CustomGridSizeValue;
			ResetGridOptions();
			Editor.customToolStripMenuItem.IsChecked = true;
		}
		#endregion

		public void OpenDataDirectoryMenuButton(object sender, RoutedEventArgs e)
		{
			if (Editor.RecentDataItemsMenu != null)
			{
				string dataDirectory = Editor.RecentDataItemsMenu[1].Tag.ToString();
				if (dataDirectory != null || dataDirectory != "")
				{
					Editor.RecentDataDirectoryClicked(sender, e, dataDirectory);
				}
			}
		}


		#endregion

		#region Status Bar Items

		#region Quick Button Buttons
		public void ToggleEncoreManiaEntitiesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Editor.UIModes.LastQuickButtonState = 3;
			if (Settings.mySettings.showEncoreEntities == true && Settings.mySettings.showManiaEntities == true)
			{
				Settings.mySettings.showManiaEntities = true;
				Settings.mySettings.showEncoreEntities = false;
			}
			if (Settings.mySettings.showEncoreEntities == true && Settings.mySettings.showManiaEntities == false)
			{
				Settings.mySettings.showManiaEntities = true;
				Settings.mySettings.showEncoreEntities = false;
			}
			else
			{
				Settings.mySettings.showManiaEntities = false;
				Settings.mySettings.showEncoreEntities = true;
			}

		}
		#endregion

		#endregion

	}
}
