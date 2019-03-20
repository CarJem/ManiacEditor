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
    public class EditorLaunch
    {
        private Editor Editor;
        public ManiacED_ManiaPal.Connector ManiaPalConnector;
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
        #endregion
        public EditorLaunch(Editor instance)
        {
            Editor = instance;
        }

        #region Apps
        public void TileManiacNormal()
        {
            if (Editor.TileManiacInstance == null || Editor.TileManiacInstance.IsClosed) Editor.TileManiacInstance = new TileManiac.MainWindow();
            Editor.TileManiacInstance.Show();
            if (Editor.TileConfig != null && Editor.EditorTiles.StageTiles != null)
            {
                if (Editor.TileManiacInstance.Visibility != Visibility.Visible || Editor.TileManiacInstance.tcf == null)
                {
                    Editor.TileManiacInstance.LoadTileConfigViaIntergration(Editor.TileConfig, Editor.Paths.TileConfig_Source);
                }
                else
                {
                    Editor.TileManiacInstance.Activate();
                }

            }

        }

        public void TileManiacIntergration()
        {
            try
            {
                if (Editor.TileManiacInstance == null || Editor.TileManiacInstance.IsClosed) Editor.TileManiacInstance = new TileManiac.MainWindow();
                if (Editor.TileManiacInstance.Visibility != Visibility.Visible)
                {
                    Editor.TileManiacInstance.Show();
                }
                Editor.TileManiacInstance.SetIntergrationNightMode(Properties.Settings.Default.NightMode);
                if (Editor.TileConfig != null && Editor.EditorTiles.StageTiles != null)
                {
                    if (Editor.TileManiacInstance.Visibility != Visibility.Visible || Editor.TileManiacInstance.tcf == null)
                    {
                        Editor.TileManiacInstance.LoadTileConfigViaIntergration(Editor.TileConfig, Editor.Paths.TileConfig_Source, Editor.UIModes.SelectedTileID);
                    }
                    else
                    {
                        Editor.TileManiacInstance.SetCollisionIndex(Editor.UIModes.SelectedTileID);
                        Editor.TileManiacInstance.Activate();
                    }

                }
            }
            catch (Exception ex)
            {
                RSDKrU.MessageBox.Show(ex.ToString());
                return;
            }

        }

        public void ManiaModManager()
        {
            String modProcessName = Path.GetFileNameWithoutExtension(ManiacEditor.Settings.mySettings.RunModLoaderPath);
            IntPtr hWnd = FindWindow(modProcessName, null); // this gives you the handle of the window you need.
            Process processes = Process.GetProcessesByName(modProcessName).FirstOrDefault();
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
                // Ask where the Mania Mod Manager is located when not set
                if (string.IsNullOrEmpty(ManiacEditor.Settings.mySettings.RunModLoaderPath))
                {
                    var ofd = new OpenFileDialog
                    {
                        Title = "Select Mania Mod Manager.exe",
                        Filter = "Windows PE Executable|*.exe"
                    };
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        ManiacEditor.Settings.mySettings.RunModLoaderPath = ofd.FileName;
                }
                else
                {
                    if (!File.Exists(ManiacEditor.Settings.mySettings.RunGamePath))
                    {
                        ManiacEditor.Settings.mySettings.RunModLoaderPath = "";
                        return;
                    }
                }

                if (File.Exists(ManiacEditor.Settings.mySettings.RunModLoaderPath))
                    Process.Start(ManiacEditor.Settings.mySettings.RunModLoaderPath);
            }
        }

        public void InsanicManiac()
        {
            //Sanic2Maniac sanic = new Sanic2Maniac(null, this);
            //sanic.Show();
        }
        public void RSDKAnnimationEditor()
        {
            String aniProcessName = Path.GetFileNameWithoutExtension(Settings.mySettings.RunAniEdPath);
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
                if (string.IsNullOrEmpty(Settings.mySettings.RunAniEdPath))
                {
                    var ofd = new OpenFileDialog
                    {
                        Title = "Select RSDK Animation Editor.exe",
                        Filter = "Windows Executable|*.exe"
                    };
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        Settings.mySettings.RunAniEdPath = ofd.FileName;
                }
                else
                {
                    if (!File.Exists(Settings.mySettings.RunGamePath))
                    {
                        Settings.mySettings.RunAniEdPath = "";
                        return;
                    }
                }

                ProcessStartInfo psi;
                psi = new ProcessStartInfo(Settings.mySettings.RunAniEdPath);
                Process.Start(psi);
            }
        }
        public void RenderListManager()
        {
            RenderListEditor editor = new RenderListEditor(Editor);
            editor.Owner = Editor.Owner;
            editor.ShowDialog();
        }
        public void ManiaPal(object sender, RoutedEventArgs e)
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
        public void ManiaPalSubmenuOpened(object sender, RoutedEventArgs e)
        {
            Editor.maniaPalHint.Header = "HINT: The Button that houses this dropdown" + Environment.NewLine + "will focus ManiaPal if it is opened already" + Environment.NewLine + "(without reloading the currently loaded colors)";
        }
        public void DuplicateObjectIDHealer()
        {
            MessageBoxResult result = RSDKrU.MessageBox.Show("WARNING: Once you do this the editor will restart immediately, make sure your progress is closed and saved!", "WARNING", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (result == MessageBoxResult.OK)
            {
                string Result = null;
                OpenFileDialog open = new OpenFileDialog() { };
                open.Filter = "Scene File|*.bin";
                if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                {
                    Result = open.FileName;
                }

                if (Result == null)
                    return;

                Editor.UnloadScene();
                Editor.Settings.UseDefaultPrefrences();

                ObjectIDHealer healer = new ObjectIDHealer();
                Editor.ShowConsoleWindow();
                healer.startHealing(open.FileName);
                Editor.HideConsoleWindow();
            }
        }

        #endregion

        #region Folders
        public void OpenSceneFolder()
        {
            if (Editor.Paths.SceneFile_Directory != null && Editor.Paths.SceneFile_Directory != "")
            {
                string SceneFilename_mod = Editor.Paths.SceneFile_Directory.Replace('/', '\\');
                Process.Start("explorer.exe", "/select, " + SceneFilename_mod);
            }
            else
            {
                RSDKrU.MessageBox.Show("Scene File does not exist or simply isn't loaded!", "ERROR");
            }

        }
        public void OpenDataDirectory()
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

        public void OpenSonicManiaFolder()
        {
            if (Settings.mySettings.RunGamePath != null && Settings.mySettings.RunGamePath != "" && File.Exists(Settings.mySettings.RunGamePath))
            {
                string GameFolder = Settings.mySettings.RunGamePath;
                string GameFolder_mod = GameFolder.Replace('/', '\\');
                Process.Start("explorer.exe", "/select, " + GameFolder_mod);
            }
            else
            {
                RSDKrU.MessageBox.Show("Game Folder does not exist or isn't set!", "ERROR");
            }

        }
        public void OpenASavedPlaceDropDownOpening(object sender, RoutedEventArgs e)
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

        public void OpenASavedPlaceDropDownClosed(object sender, RoutedEventArgs e)
        {
            Editor.openASavedPlaceToolStripMenuItem.Items.Clear();
            Editor.openASavedPlaceToolStripMenuItem.Items.Add("No Saved Places");
        }

        #endregion

        #region Scene Tools

        #region Scene Tab Buttons
        public void ImportObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e, Window window = null)
        {
            Editor.UIModes.isImportingObjects = true;
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
            Editor.UIModes.isImportingObjects = false;
        }

        public void ImportSoundsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImportSounds(sender, e, Window.GetWindow(Editor));
        }
        public void ImportSounds(object sender, RoutedEventArgs e, Window window = null)
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

        public void ManiacINIEditor(object sender, RoutedEventArgs e)
        {
            ManiacINIEditor editor = new ManiacINIEditor(Editor);
            if (editor.Owner != null) editor.Owner = Window.GetWindow(Editor);
            else editor.Owner = System.Windows.Application.Current.MainWindow;
            editor.ShowDialog();
        }

        public void LayerManager(object sender, RoutedEventArgs e)
        {
            Editor.Deselect(true);

            var lm = new ManiacEditor.Interfaces.LayerManager(Editor.EditorScene);
            lm.Owner = Window.GetWindow(Editor);
            lm.ShowDialog();

            Editor.SetupLayerButtons();
            Editor.ZoomModel.ResetViewSize();
            Editor.UI.UpdateControls();
        }

        public void ObjectManager()
        {
            var objectManager = new ManiacEditor.Interfaces.ObjectManager(Editor.EditorScene.Objects, Editor.StageConfig, Editor);
            objectManager.Owner = Window.GetWindow(Editor);
            objectManager.ShowDialog();
        }

        public void AboutScreen()
        {
            var aboutBox = new ManiacEditor.Interfaces.AboutWindow();
            aboutBox.Owner = Editor;
            aboutBox.ShowDialog();
        }

        public void OptionsMenu()
        {
            var optionMenu = new ManiacEditor.Interfaces.OptionsMenu(Editor);
            optionMenu.Owner = Editor;
            optionMenu.ShowDialog();
        }

        public void ControlMenu()
        {
            var optionMenu = new ManiacEditor.Interfaces.OptionsMenu(Editor);
            optionMenu.Owner = Editor;
            optionMenu.MainTabControl.SelectedIndex = 2;
            optionMenu.ShowDialog();
        }

        public void WikiLink()
        {
            System.Diagnostics.Process.Start("https://docs.google.com/document/d/1NBvcqzvOzqeTVzgAYBR0ttAc5vLoFaQ4yh_cdf-7ceQ/edit?usp=sharing");
        }

        public void InGameSettings()
        {
            CheatCodeManager cheatCodeManager = new CheatCodeManager();
            cheatCodeManager.Owner = Editor;
            cheatCodeManager.ShowDialog();
        }

        public void ChangePrimaryBackgroundColor(object sender, RoutedEventArgs e)
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

        public void ChangeSecondaryBackgroundColor(object sender, RoutedEventArgs e)
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

        #endregion

        #region Dev

        public void DevTerm()
        {
            var DevController = new ManiacEditor.Interfaces.DeveloperTerminal(Editor);
            DevController.Owner = Window.GetWindow(Editor);
            DevController.Show();
        }

        #endregion
    }
}
