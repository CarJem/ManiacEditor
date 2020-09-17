using System;
using System.Linq;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using RSDKv5;
using Cyotek.Windows.Forms;
using Color = System.Drawing.Color;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ManiacEditor.Controls.Misc;
using ManiacEditor.Controls.Misc.Configuration;
using ManiacEditor.Controls.Misc.Dev;
using ManiacEditor.Controls.Options;
using ManiacEditor.Controls.Object_Manager;
using ManiacEditor.Classes.Scene;

namespace ManiacEditor.Methods
{
    public static class ProgramLauncher
    {
        #region Variables/DLL Imports
        private static Controls.Editor.MainEditor Editor;
        public static ManiacED_ManiaPal.Connector ManiaPalConnector;

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

        public static void UpdateInstance(Controls.Editor.MainEditor instance)
        {
            Editor = instance;
        }

        #region Apps

        public static void CheatEngine()
        {
            String cheatEngineProcessName = Path.GetFileNameWithoutExtension(ManiacEditor.Properties.Settings.MyDefaults.CheatEnginePath);
            IntPtr hWnd = FindWindow(cheatEngineProcessName, null); // this gives you the handle of the window you need.
            Process processes = Process.GetProcessesByName(cheatEngineProcessName).FirstOrDefault();
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
                if (string.IsNullOrEmpty(ManiacEditor.Properties.Settings.MyDefaults.CheatEnginePath)) UpdateCheatEnginePath();
                else
                {
                    if (!File.Exists(ManiacEditor.Properties.Settings.MyDefaults.CheatEnginePath))
                    {
                        ManiacEditor.Properties.Settings.MyDefaults.CheatEnginePath = "";
                        return;
                    }
                }

                if (File.Exists(ManiacEditor.Properties.Settings.MyDefaults.CheatEnginePath))
                    Process.Start(ManiacEditor.Properties.Settings.MyDefaults.CheatEnginePath);
            }
        }

        public static void EditTileGraphics()
        {
            EditorTiles.EditTileGraphics(Methods.Solution.SolutionState.Main.LastSelectedTileID);
        }

        public static void TileManiacNormal()
        {
            if (Editor.TileManiacInstance == null || Editor.TileManiacInstance.IsEditorClosed) Editor.TileManiacInstance = new ManiacEditor.Controls.TileManiac.CollisionEditor();
            Editor.TileManiacInstance.Show();
            if (Methods.Solution.CurrentSolution.TileConfig != null && Methods.Solution.CurrentSolution.CurrentTiles != null)
            {
                if (Editor.TileManiacInstance.Visibility != Visibility.Visible || Editor.TileManiacInstance.TileConfig == null)
                {
                    Editor.TileManiacInstance.LoadTileConfigViaIntergration(Methods.Solution.CurrentSolution.TileConfig, ManiacEditor.Methods.Solution.SolutionPaths.TileConfig_Source.ToString());
                }
                else
                {
                    Editor.TileManiacInstance.Activate();
                }

            }

        }
        public static void TileManiacIntergration()
        {
            try
            {
                if (Editor.TileManiacInstance == null || Editor.TileManiacInstance.IsEditorClosed) Editor.TileManiacInstance = new ManiacEditor.Controls.TileManiac.CollisionEditor();
                if (Editor.TileManiacInstance.Visibility != Visibility.Visible)
                {
                    Editor.TileManiacInstance.Show();
                }
                if (Methods.Solution.CurrentSolution.TileConfig != null && Methods.Solution.CurrentSolution.CurrentTiles != null)
                {
                    if (Editor.TileManiacInstance.Visibility != Visibility.Visible || Editor.TileManiacInstance.TileConfig == null)
                    {
                        Editor.TileManiacInstance.LoadTileConfigViaIntergration(Methods.Solution.CurrentSolution.TileConfig, ManiacEditor.Methods.Solution.SolutionPaths.TileConfig_Source.ToString(), Methods.Solution.SolutionState.Main.LastSelectedTileID);
                    }
                    else
                    {
                        Editor.TileManiacInstance.SetCollisionIndex(Methods.Solution.SolutionState.Main.LastSelectedTileID);
                        Editor.TileManiacInstance.Activate();
                    }

                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                return;
            }

        }
        public static void ManiaModManager()
        {
            String modProcessName = Path.GetFileNameWithoutExtension(ManiacEditor.Properties.Settings.MyDefaults.ModLoaderPath);
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
                if (string.IsNullOrEmpty(ManiacEditor.Properties.Settings.MyDefaults.ModLoaderPath)) UpdateModManagerPath();
                else
                {
                    if (!File.Exists(ManiacEditor.Properties.Settings.MyDefaults.ModLoaderPath))
                    {
                        ManiacEditor.Properties.Settings.MyDefaults.ModLoaderPath = "";
                        return;
                    }
                }

                if (File.Exists(ManiacEditor.Properties.Settings.MyDefaults.ModLoaderPath))
                    Process.Start(ManiacEditor.Properties.Settings.MyDefaults.ModLoaderPath);
            }
        }
        public static void RSDKAnimationEditor()
        {
            /*
            String aniProcessName = Path.GetFileNameWithoutExtension(ManiacEditor.Properties.Settings.MyDefaults.AnimationEditorPath);
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
                if (string.IsNullOrEmpty(ManiacEditor.Properties.Settings.MyDefaults.AnimationEditorPath)) UpdateRSDKAnimationEditorPath();
                else
                {
                    if (!File.Exists(ManiacEditor.Properties.Settings.MyDefaults.AnimationEditorPath))
                    {
                        ManiacEditor.Properties.Settings.MyDefaults.AnimationEditorPath = "";
                        return;
                    }
                }

                ProcessStartInfo psi;
                psi = new ProcessStartInfo(ManiacEditor.Properties.Settings.MyDefaults.AnimationEditorPath);
                Process.Start(psi);
            }
            */
            //AnimationEditor.Program.MainMethod();
        }
        public static void ManiaPal(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem button = sender as System.Windows.Controls.MenuItem;
            bool isGameConfig = false;
            bool GC_NULL = false;
            bool SC_NULL = false;
            string SC_Path = "";
            string GC_Path = "";

            if (button != null && button == Editor.MenuBar.maniaPalGameConfigToolStripMenuItem)
            {
                if (Methods.Solution.CurrentSolution.GameConfig == null) GC_NULL = true;
                else GC_Path = Methods.Solution.CurrentSolution.GameConfig.FilePath;
                isGameConfig = true;
            }
            else
            {
                if (Methods.Solution.CurrentSolution.StageConfig == null) SC_NULL = true;
                else SC_Path = Methods.Solution.CurrentSolution.StageConfig.FilePath;
                isGameConfig = false;
            }


            if (ManiaPalConnector == null) ManiaPalConnector = new ManiacED_ManiaPal.Connector();

            ManiaPalConnector.SetLoadingInformation(GC_Path, SC_Path, SC_NULL, GC_NULL);
            ManiaPalConnector.Activate(isGameConfig);



        }
        public static void ManiaPalSubmenuOpened(object sender, RoutedEventArgs e)
        {
            Editor.MenuBar.maniaPalHint.Header = "HINT: The Button that houses this dropdown" + Environment.NewLine + "will focus ManiaPal if it is opened already" + Environment.NewLine + "(without reloading the currently loaded colors)";
        }
        public static void SonicManiaHeadless()
        {
            Methods.Runtime.GameHandler.RunSequence(null, null, false);
        }  

        #endregion

        #region App Dialogs

        public static void UpdateCheatEnginePath()
        {
            var ofd = new OpenFileDialog
            {
                Title = "Select Cheat Engine Executable",
                Filter = "Windows PE Executable|*.exe"
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ManiacEditor.Properties.Settings.MyDefaults.CheatEnginePath = ofd.FileName;
        }

        public static void UpdateRSDKAnimationEditorPath()
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = "Select RSDK Animation Editor.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Properties.Settings.MyDefaults.AnimationEditorPath = ofd.FileName;
        }

        public static void UpdateSonicManiaPath()
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = "Select Sonic Mania.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Properties.Settings.MyDefaults.SonicManiaPath = ofd.FileName;
        }

        public static void UpdateModManagerPath()
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = "Select Mania Mod Manager.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Properties.Settings.MyDefaults.ModLoaderPath = ofd.FileName;
        }

        #endregion

        #region Image Editor

        public static string GetProgramPath(string input)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Multiselect = false,
                Title = "Select Image Editor Program..."
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileName;
            }
            else return string.Empty;
        }
        public static string GetProgramArguments(string input)
        {
            string title = "Enter Program Launch Arguments";
            string message = "Enter your program's launch arguments:" + Environment.NewLine + "(\"{0}\" = FILEPATH, \"{1}\" = FILEPATH (without Quotes))";
            return GenerationsLib.WPF.TextPrompt2.ShowDialog(title, message, input);
        }
        public static void UpdateImageEditorPath(bool Save = true)
        {
            ManiacEditor.Properties.Settings.MyDefaults.ImageEditorPath = GetProgramPath(ManiacEditor.Properties.Settings.MyDefaults.ImageEditorPath);
            if (Save) ManiacEditor.Properties.Settings.SaveAllSettings();
        }
        public static void UpdateImageEditorArguments(bool Save = true)
        {
            ManiacEditor.Properties.Settings.MyDefaults.ImageEditorArguments = GetProgramArguments(ManiacEditor.Properties.Settings.MyDefaults.ImageEditorArguments);
            if (Save) ManiacEditor.Properties.Settings.SaveAllSettings();
        }
        public static void ValidateImageEditorPathAndArguments()
        {
            if (ManiacEditor.Properties.Settings.MyDefaults.ImageEditorPath == null) UpdateImageEditorPath();
            if (ManiacEditor.Properties.Settings.MyDefaults.ImageEditorArguments == null) UpdateImageEditorArguments();

            ManiacEditor.Properties.Settings.SaveAllSettings();
        }
        public static void OpenImage(string filePath)
        {
            ValidateImageEditorPathAndArguments();

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = ManiacEditor.Properties.Settings.MyDefaults.ImageEditorPath,
                    Arguments = string.Format(ManiacEditor.Properties.Settings.MyDefaults.ImageEditorArguments, "\"" + filePath + "\"", filePath)
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }

        #endregion

        #region Files/Folders

        public static void OpenFolder(string folderPath)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = folderPath,
                UseShellExecute = true,
                Verb = "open"
            });
        }
        public static void OpenSceneFolder()
        {
            if (ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source != null && ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourceDirectory != "")
            {
                string SceneFilename_mod = ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourceDirectory.Replace('/', '\\');
                OpenFolder(SceneFilename_mod);
            }
            else
            {
                System.Windows.MessageBox.Show("Scene File does not exist or simply isn't loaded!", "ERROR");
            }

        }
        public static void OpenManiacEditorFolder()
        {
            OpenFolder(Environment.CurrentDirectory);
        }
        public static void OpenManiacEditorFixedSettingsFolder()
        {
            OpenFolder(ProgramPaths.SettingsStaticDirectory);
        }
        public static void OpenManiacEditorPortableSettingsFolder()
        {
            OpenFolder(ProgramPaths.SettingsPortableDirectory);
        }
        public static void OpenDataDirectory()
        {
            string DataDirectory_mod = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.DataDirectory.Replace('/', '\\');
            if (DataDirectory_mod != null && DataDirectory_mod != "" && Directory.Exists(DataDirectory_mod))
            {
                OpenFolder(DataDirectory_mod);
            }
            else
            {
                System.Windows.MessageBox.Show("Data Directory does not exist or simply isn't loaded!", "ERROR");
            }

        }
        public static void OpenStageTiles()
        {
            string StageTiles_Path = Path.Combine(ManiacEditor.Methods.Solution.SolutionPaths.StageTiles_Source.SourcePath, "16x16Tiles.gif").Replace('/', '\\');
            if (StageTiles_Path != null && StageTiles_Path != "" && File.Exists(StageTiles_Path))
            {
                OpenImage(StageTiles_Path);
            }
            else
            {
                System.Windows.MessageBox.Show("Stage Tiles does not exist or simply isn't loaded!", "ERROR");
            }
        }
        public static void OpenMasterDataDirectory()
        {
            string DataDirectory_mod = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory.Replace('/', '\\');
            if (DataDirectory_mod != null && DataDirectory_mod != "" && Directory.Exists(DataDirectory_mod))
            {
                OpenFolder(DataDirectory_mod);
            }
            else
            {
                System.Windows.MessageBox.Show("Data Directory does not exist or simply isn't loaded!", "ERROR");
            }

        }
        public static void OpenSonicManiaFolder()
        {
            if (ManiacEditor.Properties.Settings.MyDefaults.SonicManiaPath != null && ManiacEditor.Properties.Settings.MyDefaults.SonicManiaPath != "" && File.Exists(ManiacEditor.Properties.Settings.MyDefaults.SonicManiaPath))
            {
                string GameFolder = ManiacEditor.Properties.Settings.MyDefaults.SonicManiaPath;
                string GameFolder_mod = GameFolder.Replace('/', '\\');
                Process.Start("explorer.exe", "/select, " + GameFolder_mod);
            }
            else
            {
                System.Windows.MessageBox.Show("Game Folder does not exist or isn't set!", "ERROR");
            }

        }

        #region Saved Place
        public static void OpenASavedPlaceDropDownOpening(object sender, RoutedEventArgs e)
        {
            if (Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces != null && Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces.Count > 0)
            {
                Editor.MenuBar.openASavedPlaceToolStripMenuItem.Items.Clear();
                var allItems = Editor.MenuBar.openASavedPlaceToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                foreach (string savedPlace in Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces)
                {
                    var savedPlaceItem = new System.Windows.Controls.MenuItem()
                    {
                        Header = savedPlace,
                        Tag = savedPlace
                    };
                    savedPlaceItem.Click += OpenASavedPlaceTrigger;
                    Editor.MenuBar.openASavedPlaceToolStripMenuItem.Items.Add(savedPlaceItem);
                }
            }

        }

        public static void OpenASavedPlaceTrigger(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;
            string savedPlaceDir = item.Header.ToString().Replace('/', '\\');
            if (Directory.Exists(savedPlaceDir))
            {
                OpenFolder(savedPlaceDir);
            }
            else
            {
                System.Windows.MessageBox.Show("This Folder does not exist! " + string.Format("({0})", savedPlaceDir), "ERROR");
            }
        }

        public static void OpenASavedPlaceDropDownClosed(object sender, RoutedEventArgs e)
        {
            Editor.MenuBar.openASavedPlaceToolStripMenuItem.Items.Clear();
            Editor.MenuBar.openASavedPlaceToolStripMenuItem.Items.Add("No Saved Places");
        }
        #endregion

        #endregion

        #region Scene Tools

        #region Scene Tab Buttons
        public static void ImportObjectsFromScene(Window window = null)
        {
            try
            {
                RSDKv5.Scene sourceScene = GetSceneForObjectImporting(window);
                if (sourceScene == null) return;
                var objectImporter = new ObjectImporter(sourceScene.Objects, Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects, Methods.Solution.CurrentSolution.StageConfig, Editor);
                if (window != null) objectImporter.Owner = window;
                objectImporter.ShowDialog();

                if (objectImporter.DialogResult != true)
                    return; // nothing to do

                // user clicked Import, get to it!
                Methods.Internal.UserInterface.UpdateControls();
                Editor.EntitiesToolbar?.RefreshSpawningObjects(Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects);
                Methods.Internal.UserInterface.SplineControls.UpdateSplineSpawnObjectsList(Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects);

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Unable to import Objects. " + ex.Message);
            }
        }
        public static void ImportObjectsWithMegaList(Window window = null)
        {
            try
            {
                GenerationsLib.Core.FolderSelectDialog ofd = new GenerationsLib.Core.FolderSelectDialog();
                ofd.Title = "Select a clean Data Folder";
                if (ofd.ShowDialog() == true)
                {
                    string gameConfigPath = System.IO.Path.Combine(ofd.FileName, "Game", "GameConfig.bin");
                    if (File.Exists(gameConfigPath))
                    {
                        GameConfig SourceConfig = new GameConfig(gameConfigPath);
                        var objectImporter = new ObjectImporter(ofd.FileName, SourceConfig, Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects, Methods.Solution.CurrentSolution.StageConfig, Editor);
                        if (window != null) objectImporter.Owner = window;
                        objectImporter.ShowDialog();

                        if (objectImporter.DialogResult != true)
                            return; // nothing to do

                        // user clicked Import, get to it!
                        Methods.Internal.UserInterface.UpdateControls();
                        Editor.EntitiesToolbar?.RefreshSpawningObjects(Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects);
                        Methods.Internal.UserInterface.SplineControls.UpdateSplineSpawnObjectsList(Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects);
                    }
                }


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Unable to import Objects. " + ex.Message);
            }
        }
        public static void ExportObjectsFromScene(Window window = null)
        {
            try
            {
                RSDKv5.Scene sourceScene = GetSceneForObjectImporting(window);
                if (sourceScene == null) return;

                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Filter = "Exported TXT | *.txt",
                    Title = "Save Exported Results As..."
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Methods.Entities.ObjectCollection.ExportObjectsFromSceneToFile(saveFileDialog.FileName, sourceScene.Objects, Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Unable to export Objects. " + ex.Message);
            }
        }
        public static void ExportObjectsWithMegaList(Window window = null)
        {
            try
            {
                GenerationsLib.Core.FolderSelectDialog ofd = new GenerationsLib.Core.FolderSelectDialog();
                ofd.Title = "Select a clean Data Folder";
                if (ofd.ShowDialog() == true)
                {
                    string gameConfigPath = System.IO.Path.Combine(ofd.FileName, "Game", "GameConfig.bin");
                    if (File.Exists(gameConfigPath))
                    {
                        GameConfig SourceConfig = new GameConfig(gameConfigPath);
                        SaveFileDialog saveFileDialog = new SaveFileDialog()
                        {
                            Filter = "Exported TXT | *.txt",
                            Title = "Save Exported Results As..."
                        };
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            Methods.Entities.ObjectCollection.ExportObjectsFromDataFolderToFile(saveFileDialog.FileName, ofd.FileName, SourceConfig, Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects);
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Unable to export Objects. " + ex.Message);
            }
        }
        public static Scene GetSceneForObjectImporting(Window window = null)
        {
            string selectedScene;

            ManiacEditor.Controls.SceneSelect.SceneSelectWindow select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(Editor, false, false);
            select.Owner = Window.GetWindow(window);
            select.ShowDialog();
            if (select.SceneSelect.SceneState.FilePath == null)
                return null;
            selectedScene = select.SceneSelect.SceneState.FilePath;

            if (!File.Exists(selectedScene))
            {
                string[] splitted = selectedScene.Split('/');

                string part1 = splitted[0];
                string part2 = splitted[1];

                selectedScene = Path.Combine(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, "Stages", part1, part2);
            }
            return new Scene(selectedScene);
        }
        public static void ImportSounds(object sender, RoutedEventArgs e)
        {
            ImportSounds(Window.GetWindow(Editor));
        }
        public static void ImportSounds(Window window = null)
        {
            try
            {
                StageConfig sourceStageConfig = null;
                using (var fd = new OpenFileDialog())
                {
                    fd.Filter = "Stage Config File|*.bin";
                    fd.DefaultExt = ".bin";
                    fd.Title = "Select Stage Config File";
                    fd.InitialDirectory = Path.Combine(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, "Stages");
                    if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            sourceStageConfig = new StageConfig(fd.FileName);
                        }
                        catch
                        {
                            System.Windows.MessageBox.Show("Ethier this isn't a stage config, or this stage config is ethier corrupted or unreadable in Maniac.");
                            return;
                        }

                    }
                }
                if (null == sourceStageConfig) return;

                var soundImporter = new SoundImporter(sourceStageConfig, Methods.Solution.CurrentSolution.StageConfig);
                soundImporter.ShowDialog();

                if (soundImporter.DialogResult != true)
                    return; // nothing to do


                // changing the sound list doesn't require us to do anything either

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Unable to import sounds. " + ex.Message);
            }
        }
        public static void ManiacINIEditor(object sender, RoutedEventArgs e)
        {
            Controls.Toolbox.SceneSettings editor = new Controls.Toolbox.SceneSettings(Editor);
            if (editor.Owner != null) editor.Owner = Window.GetWindow(Editor);
            else editor.Owner = System.Windows.Application.Current.MainWindow;
            editor.ShowDialog();

            Editor.EditorToolbar.SetupLayerButtons();
            Editor.ViewPanel.SharpPanel.ResetZoomLevel();
            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void TileManager()
        {
            var tm = new Controls.Toolbox.TileManager();
            tm.Owner = Window.GetWindow(Editor);
            tm.ShowDialog();

            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void LayerManager(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionActions.Deselect(true);

            var lm = new Controls.Toolbox.LayerManager(Methods.Solution.CurrentSolution.CurrentScene);
            lm.Owner = Window.GetWindow(Editor);
            lm.ShowDialog();


            Editor.EditorToolbar.SetupLayerButtons();
            Editor.ViewPanel.SharpPanel.ResetZoomLevel();
            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void ExportGUI(object sender, RoutedEventArgs e)
        {
            var eG = new Controls.Toolbox.ExportAsImageGUI(Methods.Solution.CurrentSolution.CurrentScene);
            eG.Owner = Window.GetWindow(Editor);
            eG.ShowDialog();
        }
        public static void ObjectManager()
        {
            var objectManager = new ObjectManager(Editor);
            objectManager.Owner = Window.GetWindow(Editor);
            objectManager.ShowDialog();
        }

        public static void AboutScreen()
        {
            var aboutBox = new Controls.About.AboutWindow();
            aboutBox.Owner = Editor;
            aboutBox.ShowDialog();
        }

        public static void OptionsMenu()
        {
            var optionMenu = new OptionsMenu();
            optionMenu.Owner = Editor;
            optionMenu.ShowDialog();
        }

        public static void ControlMenu()
        {
            var optionMenu = new OptionsMenu();
            optionMenu.Owner = Editor;
            optionMenu.MainTabControl.SelectedIndex = 2;
            optionMenu.ShowDialog();
        }

        public static void WikiLink()
        {
            System.Diagnostics.Process.Start("https://maniaceditor-generationsedition.readthedocs.io/en/latest/");
        }

        public static void InGameSettings()
        {
            CheatCodeManager cheatCodeManager = new CheatCodeManager();
            cheatCodeManager.Owner = Editor;
            cheatCodeManager.ShowDialog();
        }

        public static void ChangeEditorBackgroundColors()
        {
            var colorA = Methods.Solution.CurrentSolution.CurrentScene.EditorMetadata.BackgroundColor1;
            var colorB = Methods.Solution.CurrentSolution.CurrentScene.EditorMetadata.BackgroundColor2;
            Controls.Toolbox.SceneBGColorPickerTool colorSelect = new Controls.Toolbox.SceneBGColorPickerTool(colorA, colorB);
            bool? result = colorSelect.ShowDialog();
            if (colorSelect.ColorsChanged == true)
            {
                Methods.Solution.CurrentSolution.CurrentScene.EditorMetadata.BackgroundColor1 = colorSelect.BackgroundColor1;
                Methods.Solution.CurrentSolution.CurrentScene.EditorMetadata.BackgroundColor2 = colorSelect.BackgroundColor2;
            }
        }

        #endregion

        #endregion

        #region Editor Tools

        public static void ClipboardManager(Window window = null)
        {
            var cm = new Controls.Clipboard_Manager.ClipboardManager();
            if (window != null) cm.Owner = window;
            cm.Show();

            Methods.Internal.UserInterface.UpdateControls();
        }

        #endregion

        #region Dev

        public static void ManiacConsoleToggle()
        {
            Extensions.ConsoleExtensions.ToggleManiacConsole();
        }

        public static void ConsoleToggle()
        {
            if (!Methods.Solution.SolutionState.Main.IsConsoleWindowOpen)
            {
                Methods.Solution.SolutionState.Main.IsConsoleWindowOpen = true;
                Extensions.ConsoleExtensions.ShowConsoleWindow();
            }
            else
            {
                Methods.Solution.SolutionState.Main.IsConsoleWindowOpen = false;
                Extensions.ConsoleExtensions.HideConsoleWindow();
            }
        }

        public static void DevTerm()
        {
            var DevController = new DeveloperTerminal(Editor);
            DevController.Owner = Window.GetWindow(Editor);
            DevController.Show();
        }

        #endregion
    }
}
