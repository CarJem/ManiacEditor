using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using RSDKv5;
using GenerationsLib.Core;
using ManiacEditor.Classes.Editor.Scene;
using System.IO;


namespace ManiacEditor.Classes.Editor
{
    public static class SolutionLoader
    {
        private static Controls.Editor.MainEditor Instance;
        public static void UpdateInstance(Controls.Editor.MainEditor instance)
        {
            Instance = instance;
        }

        private static bool EditorLoad()
        {
            if (Instance.DataDirectory == null)
            {
                return false;
            }
            Instance.EntityDrawing.ReleaseResources();
            return true;
        }

        #region Editor Commands

        public static void NewScene()
        {
            if (AllowSceneUnloading() != true) return;
            Classes.Editor.Solution.UnloadScene();
            ManiacEditor.Controls.SceneSelect.NewSceneWindow makerDialog = new ManiacEditor.Controls.SceneSelect.NewSceneWindow();
            makerDialog.Owner = Controls.Editor.MainEditor.GetWindow(Instance);
            if (makerDialog.ShowDialog() == true)
            {
                string directoryPath = Path.GetDirectoryName(makerDialog.SceneFolder);

                Classes.Editor.Solution.CurrentScene = new Classes.Editor.Scene.EditorScene(Instance.DeviceModel.GraphicPanel, makerDialog.Scene_Width, makerDialog.Scene_Height, makerDialog.BG_Width, makerDialog.BG_Height, Instance);
                Classes.Editor.Solution.TileConfig = new Tileconfig();
                Classes.Editor.Solution.CurrentTiles = new Classes.Editor.Scene.EditorTiles();
                Classes.Editor.Solution.StageConfig = new StageConfig();

                string ImagePath = directoryPath + "//16x16Tiles.gif";
                string TilesPath = directoryPath + "//TilesConfig.bin";
                string StagePath = directoryPath + "//StageConfig.bin";

                File.Create(ImagePath).Dispose();
                File.Create(TilesPath).Dispose();
                File.Create(StagePath).Dispose();

                //EditorScene.Write(SceneFilepath);
                Classes.Editor.Solution.TileConfig.Write(TilesPath);
                //StageConfig.Write(StagePath);
                Classes.Editor.Solution.CurrentTiles.Write(ImagePath);


                Instance.UpdateDataFolderLabel(null, null);

                Instance.SetupLayerButtons();


                Instance.BackgroundDX = new Classes.Editor.Scene.EditorBackground(Instance);

                Classes.Editor.Solution.Entities = new Classes.Editor.Scene.EditorEntities(Classes.Editor.Solution.CurrentScene);

                Instance.DeviceModel.UpdateViewSize((int)(Classes.Editor.Solution.SceneWidth * Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.Solution.SceneHeight * Classes.Editor.SolutionState.Zoom));

                Methods.Internal.UserInterface.UpdateControls(true);
            }
        }
        public static void OpenScene()
        {
            if (ManiacEditor.Classes.Editor.SolutionLoader.AllowSceneUnloading() != true) return;
            Classes.Editor.Solution.UnloadScene();

            OpenSceneUsingSceneSelect();
        }
        public static void OpenDataDirectory()
        {
            if (ManiacEditor.Classes.Editor.SolutionLoader.AllowSceneUnloading() != true) return;

            string newDataDirectory = Instance.GetDataDirectory();
            if (null == newDataDirectory) return;
            if (newDataDirectory.Equals(Instance.DataDirectory)) return;

            if (Instance.IsDataDirectoryValid(newDataDirectory)) Instance.ResetDataDirectoryToAndResetScene(newDataDirectory);
            else MessageBox.Show($@"{newDataDirectory} is not a valid Data Directory.", "Invalid Data Directory!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void Save()
        {
            if (Classes.Editor.Solution.CurrentScene == null) return;
            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit()) Classes.Editor.EditorActions.Deselect();

            SaveScene();
            SaveStageConfig();
            SaveChunks();
        }
        public static void SaveAs()
        {
            if (Classes.Editor.Solution.CurrentScene == null) return;
            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit()) Classes.Editor.EditorActions.Deselect();

            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "Scene File|*.bin",
                DefaultExt = "bin",
                InitialDirectory = ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory,
                RestoreDirectory = false,
                FileName = System.IO.Path.GetFileName(ManiacEditor.Classes.Editor.Solution.Paths.SceneFile_Source.SourcePath)
            };
            if (save.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                string saveAsFolder = Path.GetDirectoryName(save.FileName);

                SaveScene(true, save.FileName);
                SaveAsExtras(saveAsFolder);

            }
        }
        public static void UnloadScene(bool SkipCheck = false)
        {
            if (AllowSceneUnloading(SkipCheck) != true) return;
            Classes.Editor.Solution.UnloadScene();
        }
        public static bool AllowSceneUnloading(bool SkipCheck = false)
        {
            if (SkipCheck) return true;
            bool AllowSceneChange = false;
            if (ManiacEditor.Classes.Editor.SolutionState.IsSceneLoaded() == false)
            {
                AllowSceneChange = true;
                return AllowSceneChange;
            }
            else if (ManiacEditor.Classes.Editor.SolutionState.IsSceneLoaded() == true && ManiacEditor.Core.Settings.MySettings.DisableSaveWarnings == false)
            {

                if ((Instance.UndoStack.Count != 0 || Instance.RedoStack.Count != 0) || Classes.Editor.SolutionState.QuitWithoutSavingWarningRequired == true)
                {
                    var exitBox = new Controls.Dialog.UnloadingSceneWarning();
                    exitBox.Owner = Window.GetWindow(Instance);
                    exitBox.ShowDialog();
                    var exitBoxResult = exitBox.WindowResult;
                    if (exitBoxResult == Controls.Dialog.UnloadingSceneWarning.WindowDialogResult.Yes)
                    {
                        SaveScene();
                        AllowSceneChange = true;
                    }
                    else if (exitBoxResult == Controls.Dialog.UnloadingSceneWarning.WindowDialogResult.No)
                    {
                        AllowSceneChange = true;
                    }
                    else
                    {
                        AllowSceneChange = false;
                    }
                }
                else
                {
                    AllowSceneChange = true;
                }

            }
            else
            {
                AllowSceneChange = true;
            }
            return AllowSceneChange;
        }
        public static void ExportAsPNG()
        {
            
            if (Classes.Editor.Solution.CurrentScene == null) return;

            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog
            {
                Filter = ".png File|*.png",
                DefaultExt = "png"
            };
            if (save.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(Classes.Editor.Solution.SceneWidth, Classes.Editor.Solution.SceneHeight))
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    // not all scenes have both a Low and a High foreground
                    // only attempt to render the ones we actually have
                    Classes.Editor.Solution.FGLower?.Draw(g);
                    Classes.Editor.Solution.FGLow?.Draw(g);
                    Classes.Editor.Solution.FGHigh?.Draw(g);
                    Classes.Editor.Solution.FGHigher?.Draw(g);
                    Classes.Editor.Solution.Entities?.Draw(g);

                    bitmap.Save(save.FileName);
                }

            }
        }
        public static void ExportLayersAsPNG()
        {
            try
            {
                if (Classes.Editor.Solution.CurrentScene?._editorLayers == null || !Classes.Editor.Solution.CurrentScene._editorLayers.Any()) return;

                var dialog = new GenerationsLib.Core.FolderSelectDialog()
                {
                    Title = "Select folder to save each exported layer image to"
                };

                if (!dialog.ShowDialog()) return;

                int fileCount = 0;

                foreach (var editorLayer in Classes.Editor.Solution.CurrentScene.AllLayers)
                {
                    string fileName = System.IO.Path.Combine(dialog.FileName, editorLayer.Name + ".png");

                    if (!Methods.Internal.Common.CanWriteFile(fileName))
                    {
                        Methods.Internal.Common.ShowError($"Layer export aborted. {fileCount} images saved.");
                        return;
                    }

                    using (var bitmap = new System.Drawing.Bitmap(editorLayer.Width * Classes.Editor.Constants.TILE_SIZE, editorLayer.Height * Classes.Editor.Constants.TILE_SIZE))
                    using (var g = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        editorLayer.Draw(g);
                        bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                        ++fileCount;
                    }
                }

                MessageBox.Show($"Layer export succeeded. {fileCount} images saved.", "Success!",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError("An error occurred: " + ex.Message);
            }
        }
        public static void ExportObjLayoutAsPNG()
        {
            int i = 0;
            try
            {
                if (Classes.Editor.Solution.CurrentScene?._editorLayers == null || !Classes.Editor.Solution.CurrentScene._editorLayers.Any()) return;

                var dialog = new FolderSelectDialog()
                {
                    Title = "Select folder to save each exported object layout image to"
                };

                if (!dialog.ShowDialog()) return;

                int fileCount = 0;

                string fileName = System.IO.Path.Combine(dialog.FileName, "Objects.png");

                using (var bitmap = new System.Drawing.Bitmap(1024 * Classes.Editor.Constants.TILE_SIZE, 256 * Classes.Editor.Constants.TILE_SIZE))
                {
                    using (var g = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        for (i = 0; i < Classes.Editor.Solution.Entities.Entities.Count; i++)
                        {
                            //if (!Instance.CanWriteFile(fileName))
                            // {
                            //    Methods.Internal.Common.ShowError($"Layout export aborted. {fileCount} images saved.");
                            //    return;
                            //}
                            try
                            {
                                Classes.Editor.Solution.Entities.Entities[i].ExportDraw(g,false);
                            }
                            catch
                            {

                            }
                        }
                    }
                    bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                    ++fileCount;
                }

                System.Windows.MessageBox.Show($"Layer export succeeded. {fileCount} images saved.", "Success!",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError("An error occurred: " + ex.Message);
            }
        }

        #endregion

        #region Opening
        public static void OpenSceneUsingSceneSelect()
        {
            ManiacEditor.Controls.SceneSelect.SceneSelectWindow select;

            if (!EditorLoad())
            {
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(null, Instance);
                select.Owner = Instance;
                select.ShowDialog();
            }
            else
            {
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(Classes.Editor.Solution.GameConfig, Instance);
                select.Owner = Instance;
                select.ShowDialog();
            }

            if (select.DialogResult != true) return;
            if (PreLoad() == false) return;

            GetSceneSelectData(select.SceneSelect, select.SceneSelect.Browsed);

            //if (ManiacEditor.Classes.Editor.Solution.Paths.Browsed)
            //{
            //    LoadFromFiles();
            //}
            //else
            //{
                LoadMethodRevolution();
            //}

        }

        public static void OpenSceneSelectFromPreviousConfiguration(ManiacEditor.Classes.Internal.DataStateHistoryCollection.SaveState SaveState)
        {
            ManiacEditor.Controls.SceneSelect.SceneSelectWindow select;
            ManiacEditor.Classes.Editor.Solution.Paths.SetGameConfig(SaveState.DataDirectory);

            select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow((EditorLoad() ? Classes.Editor.Solution.GameConfig : null), Instance);

            select.Owner = Instance;

            if (SaveState.isDataPack)
            {
                select.SceneSelect.UnloadDataPack();
                select.SceneSelect.LoadDataPackFromName(SaveState.LoadedDataPack);
            }
            else
            {
                select.SceneSelect.UnloadDataPack();
            }

            select.ShowDialog();


            if (select.DialogResult != true) return;
            if (PreLoad() == false) return;

            GetSceneSelectData(select.SceneSelect, select.SceneSelect.Browsed);

            if (ManiacEditor.Classes.Editor.Solution.Paths.Browsed)
            {
                AddTemporaryResourcePack();
            //  LoadFromFiles();
            }
            //else
            //{
                LoadMethodRevolution();
            //}
        }

        public static void OpenSceneSelectWithPrefrences(string dataDirectory)
        {
            ManiacEditor.Controls.SceneSelect.SceneSelectWindow select;
            ManiacEditor.Classes.Editor.Solution.Paths.SetGameConfig(dataDirectory);

            if (!EditorLoad())
            {
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(null, Instance);
                select.Owner = Instance;
                select.ShowDialog();
            }
            else
            {
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(Classes.Editor.Solution.GameConfig, Instance);
                select.Owner = Instance;
                select.ShowDialog();
            }

            if (select.DialogResult != true) return;
            if (PreLoad() == false) return;

            GetSceneSelectData(select.SceneSelect, select.SceneSelect.Browsed);

            if (ManiacEditor.Classes.Editor.Solution.Paths.Browsed)
            {
                AddTemporaryResourcePack();
            //    LoadFromFiles();
            }
            //else
            //{
                LoadMethodRevolution();
            //}

        }

        public static void OpenSceneFromSaveState(Classes.Internal.SceneHistoryCollection.SaveState saveState)
        {
            OpenSceneFromSaveState(saveState.DataDirectory, saveState.Result, saveState.LevelID, saveState.isEncore, saveState.CurrentName, saveState.CurrentZone, saveState.CurrentSceneID, saveState.Browsed, saveState.ResourcePacks);
        }

        public static void OpenSceneFromSaveState(string dataDirectory, string Result, int LevelID, bool isEncore, string CurrentName, string CurrentZone, string CurrentSceneID, bool browsedFile, IList<string> ResourcePacks)
        {
            if (PreLoad() == false) return;

            if (ManiacEditor.Classes.Editor.Solution.Paths.SetGameConfig(dataDirectory) == false) return;

            GetSceneSelectData(dataDirectory, Result, LevelID, isEncore, CurrentName, CurrentZone, CurrentSceneID, browsedFile, true);

            Instance.ResourcePackList = ResourcePacks;

            //if (ManiacEditor.Classes.Editor.Solution.Paths.Browsed)
            //{
            //    LoadFromFiles();
            //}
            //else
            //{
                LoadMethodRevolution();
            //}

        }

        public static void OpenSceneUsingExistingSceneSelect(ManiacEditor.Controls.SceneSelect.SceneSelectHost select)
        {
            if (PreLoad() == false) return;

            GetSceneSelectData(select, select.Browsed);

            //if (select.Browsed)
            //{
            //    LoadFromFiles();
            //}
            //else
            //{
                LoadMethodRevolution();
            //}
        }

        public static void GetSceneSelectData(ManiacEditor.Controls.SceneSelect.SceneSelectHost select, bool browsedFile = false, bool skipResourcePacks = false)
        {
            if (browsedFile == true)
            {
                ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath = select.SelectedSceneResult;
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentLevelID = select.LevelID;
                ManiacEditor.Classes.Editor.Solution.Paths.isEncoreMode = select.isEncore;
                ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory = Path.GetDirectoryName(ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath);
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone = Path.GetFileName(ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory);
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentName = select.CurrentName;
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID = select.CurrentSceneID;
                ManiacEditor.Classes.Editor.Solution.Paths.Browsed = select.Browsed;
                if (!skipResourcePacks) AddTemporaryResourcePack();
            }
            else
            {
                ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath = select.SelectedSceneResult;
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentLevelID = select.LevelID;
                ManiacEditor.Classes.Editor.Solution.Paths.isEncoreMode = select.isEncore;
                ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory = Path.GetDirectoryName(ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath);
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone = select.CurrentZone;
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentName = select.CurrentName;
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID = select.CurrentSceneID;
                ManiacEditor.Classes.Editor.Solution.Paths.Browsed = select.Browsed;
            }
        }

        public static void GetSceneSelectData(string dataDirectory, string Result, int LevelID, bool isEncore, string CurrentName, string CurrentZone, string CurrentSceneID, bool browsedFile, bool skipResourcePacks = false)
        {
            if (browsedFile == true)
            {
                ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath = Result;
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentLevelID = LevelID;
                ManiacEditor.Classes.Editor.Solution.Paths.isEncoreMode = isEncore;
                ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory = Path.GetDirectoryName(ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath);
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone = Path.GetFileName(ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory);
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentName = CurrentName;
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID = CurrentSceneID;
                ManiacEditor.Classes.Editor.Solution.Paths.Browsed = browsedFile;
                if (!skipResourcePacks) AddTemporaryResourcePack();
            }
            else
            {
                ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath = Result;
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentLevelID = LevelID;
                ManiacEditor.Classes.Editor.Solution.Paths.isEncoreMode = isEncore;
                ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory = Path.GetDirectoryName(ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath);
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone = CurrentZone;
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentName = CurrentName;
                ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID = CurrentSceneID;
                ManiacEditor.Classes.Editor.Solution.Paths.Browsed = browsedFile;
            }
        }

        public static void AddTemporaryResourcePack()
        {
            if (ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory.Contains("Data") && ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory.Contains("Stages"))
            {
                var input = ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory;
                var output = input.Replace("\\Stages\\" + ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, "");
                if (output.Contains("Data"))
                {
                    Instance.ResourcePackList.Add(output);
                }
            }
        }
        #endregion

        #region Revolutionized Loading System

        public static void LoadMethodRevolution()
        {
            bool LoadFailed = false;
            try
            {
                #region Scene File
                try
                {
                    Classes.Editor.SolutionState.LevelID = ManiacEditor.Classes.Editor.Solution.Paths.CurrentLevelID;
                    string sceneFilePath = (ManiacEditor.Classes.Editor.Solution.Paths.Browsed ? ManiacEditor.Classes.Editor.Solution.Paths.GetScenePathFromFile(ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath) : ManiacEditor.Classes.Editor.Solution.Paths.GetScenePath());
                    Classes.Editor.Solution.CurrentScene = new Classes.Editor.Scene.EditorScene(sceneFilePath, Instance.DeviceModel.GraphicPanel, Instance);
                }
                catch (Exception ex)
                {
                    LoadFailed = true;
                    LoadingFailed(ex);
                    return;
                }
                #endregion

                #region Encore Color Palette
                try
                {
                    //ACT File (Encore Colors)
                    if (ManiacEditor.Classes.Editor.Solution.Paths.Browsed)
                    {
                        Instance.EncorePalette = Classes.Editor.Solution.CurrentScene.GetEncorePalette(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, Instance.DataDirectory, ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID, ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory, 0);
                        Classes.Editor.SolutionState.EncoreSetupType = Classes.Editor.Solution.CurrentScene.GetEncoreSetupType(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, Instance.DataDirectory, ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID, ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory);
                    }
                    else
                    {
                        Instance.EncorePalette = Classes.Editor.Solution.CurrentScene.GetEncorePalette(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, Instance.DataDirectory, ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID, "", 1);
                        Classes.Editor.SolutionState.EncoreSetupType = Classes.Editor.Solution.CurrentScene.GetEncoreSetupType(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, Instance.DataDirectory, ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID, "");
                    }

                    if (Instance.EncorePalette[0] != "")
                    {
                        Classes.Editor.SolutionState.EncorePaletteExists = true;
                        if (ManiacEditor.Classes.Editor.Solution.Paths.isEncoreMode)
                        {
                            Instance.EditorToolbar.EncorePaletteButton.IsChecked = true;
                            Classes.Editor.SolutionState.UseEncoreColors = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoadFailed = true;
                    LoadingFailed(ex);
                    return;
                }
                #endregion

                #region Stage Tiles
                try
                {
                    bool valid;
                    if (Classes.Editor.SolutionState.UseEncoreColors == true && Instance.EncorePalette[0] != "") valid = ManiacEditor.Classes.Editor.Solution.Paths.GetStageTiles(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, Instance.EncorePalette[0]);
                    else valid = ManiacEditor.Classes.Editor.Solution.Paths.GetStageTiles(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone);
                    if (valid == false)
                    {
                        throw new Exception("Stage Config was unable to be found or was invalid!");
                    }
                }
                catch (Exception ex)
                {
                    LoadFailed = true;
                    LoadingFailed(ex);
                    return;
                }
                #endregion

                #region Stage/Tile Config
                ManiacEditor.Classes.Editor.Solution.Paths.GetTileConfig(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, ManiacEditor.Classes.Editor.Solution.Paths.Browsed);
                ManiacEditor.Classes.Editor.Solution.Paths.GetStageConfig(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, ManiacEditor.Classes.Editor.Solution.Paths.Browsed);
                #endregion
            }
            catch (Exception ex)
            {
                LoadFailed = true;
                LoadingFailed(ex);
                return;
            }
            if (!LoadFailed) AfterLoad();
        }

        #endregion

        #region Loading
        public static void LoadFromSceneSelect()
        {
            bool LoadFailed = false;
            try
            {
                //Using Instance Means the Stuff Hasn't Stated 
                Classes.Editor.SolutionState.LevelID = ManiacEditor.Classes.Editor.Solution.Paths.CurrentLevelID;
                Classes.Editor.Solution.CurrentScene = new Classes.Editor.Scene.EditorScene(ManiacEditor.Classes.Editor.Solution.Paths.GetScenePath(), Instance.DeviceModel.GraphicPanel, Instance);

                //ACT File (Encore Colors)
                Instance.EncorePalette = Classes.Editor.Solution.CurrentScene.GetEncorePalette(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, Instance.DataDirectory, ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID, "", 1);
                Classes.Editor.SolutionState.EncoreSetupType = Classes.Editor.Solution.CurrentScene.GetEncoreSetupType(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, Instance.DataDirectory, ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID, "");
                if (Instance.EncorePalette[0] != "")
                {
                    Classes.Editor.SolutionState.EncorePaletteExists = true;
                    if (ManiacEditor.Classes.Editor.Solution.Paths.isEncoreMode)
                    {
                        Instance.EditorToolbar.EncorePaletteButton.IsChecked = true;
                        Classes.Editor.SolutionState.UseEncoreColors = true;
                    }
                }

                //Stage Tiles
                bool valid;
                if (Classes.Editor.SolutionState.UseEncoreColors == true && Instance.EncorePalette[0] != "") valid = ManiacEditor.Classes.Editor.Solution.Paths.GetStageTiles(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, Instance.EncorePalette[0]);
                else valid = ManiacEditor.Classes.Editor.Solution.Paths.GetStageTiles(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone);
                if (valid == false)
                {
                    LoadingFailed("Stage Config was unable to be found or was invalid!");
                    return;
                }

                //Tile Config
                ManiacEditor.Classes.Editor.Solution.Paths.GetTileConfig(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone);
                ManiacEditor.Classes.Editor.Solution.Paths.GetStageConfig(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone);
            }
            catch (Exception ex)
            {
                LoadFailed = true;
                LoadingFailed(ex);
                return;
            }

            if (!LoadFailed) AfterLoad();

        }
        public static void LoadFromFiles()
        {
            bool LoadFailed = false;
            try
            {
                Classes.Editor.SolutionState.LevelID = ManiacEditor.Classes.Editor.Solution.Paths.CurrentLevelID;
                Classes.Editor.Solution.CurrentScene = new Classes.Editor.Scene.EditorScene(ManiacEditor.Classes.Editor.Solution.Paths.GetScenePathFromFile(ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath), Instance.DeviceModel.GraphicPanel, Instance);


                //ACT File (Encore Colors)
                Instance.EncorePalette = Classes.Editor.Solution.CurrentScene.GetEncorePalette(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, Instance.DataDirectory, ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID, ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory, 0);
                Classes.Editor.SolutionState.EncoreSetupType = Classes.Editor.Solution.CurrentScene.GetEncoreSetupType(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, Instance.DataDirectory, ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID, ManiacEditor.Classes.Editor.Solution.Paths.SceneDirectory);
                if (Instance.EncorePalette[0] != "")
                {
                    Classes.Editor.SolutionState.EncorePaletteExists = true;
                    if (ManiacEditor.Classes.Editor.Solution.Paths.isEncoreMode)
                    {
                        Instance.EditorToolbar.EncorePaletteButton.IsChecked = true;
                        Classes.Editor.SolutionState.UseEncoreColors = true;
                    }
                }

                //Stage Tiles
                bool valid;
                if (Classes.Editor.SolutionState.UseEncoreColors == true && Instance.EncorePalette[0] != "") valid = ManiacEditor.Classes.Editor.Solution.Paths.GetStageTiles(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, Instance.EncorePalette[0], ManiacEditor.Classes.Editor.Solution.Paths.Browsed);
                else valid = ManiacEditor.Classes.Editor.Solution.Paths.GetStageTiles(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, null, ManiacEditor.Classes.Editor.Solution.Paths.Browsed);
                if (valid == false)
                {
                    LoadingFailed("Stage Config was ethier Invalid or Not Found!");
                    return;
                }

                //Tile Config
                ManiacEditor.Classes.Editor.Solution.Paths.GetTileConfig(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, ManiacEditor.Classes.Editor.Solution.Paths.Browsed);
                ManiacEditor.Classes.Editor.Solution.Paths.GetStageConfig(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, ManiacEditor.Classes.Editor.Solution.Paths.Browsed);
            }
            catch (Exception ex)
            {
                LoadFailed = true;
                LoadingFailed(ex);
                return;
            }


            if (!LoadFailed) AfterLoad();
        }
        public static void AfterLoad()
        {
            try
            {
                SetupObjectsList();
                SetupDiscordRP(ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath);
                Stamps StageStamps = ManiacEditor.Classes.Editor.Solution.Paths.GetEditorStamps(ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone);
                Instance.Chunks = new Classes.Editor.Scene.EditorChunks(Instance, Classes.Editor.Solution.CurrentTiles, StageStamps);
                Instance.BackgroundDX = new Classes.Editor.Scene.EditorBackground(Instance);
                Classes.Editor.Solution.Entities = new Classes.Editor.Scene.EditorEntities(Classes.Editor.Solution.CurrentScene);

                Methods.Internal.UserInterface.UpdateSplineSpawnObjectsList(Classes.Editor.Solution.CurrentScene.Objects);

                ReadManiacINIFile();
                Instance.UpdateStartScreen(false);
                Instance.UpdateDataFolderLabel(null, null);
                Instance.SetupLayerButtons();
                Classes.Editor.SolutionState.UpdateMultiLayerSelectMode();
                Methods.Internal.UserInterface.UpdateControls(true);
                Methods.Prefrences.SceneHistoryStorage.AddRecentFile(Methods.Prefrences.SceneHistoryStorage.GenerateNewEntry());
                ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.AddRecentFile(ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.GenerateNewEntry());
                Instance.DeviceModel.UpdateViewSize((int)(Classes.Editor.Solution.SceneWidth * Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.Solution.SceneHeight * Classes.Editor.SolutionState.Zoom));
                Instance.DeviceModel.ResetViewSize();

            }
            catch (Exception ex)
            {
                LoadingFailed(ex);
                return;
            }

        }
        public static bool PreLoad()
        {
            Classes.Editor.Solution.UnloadScene();
            Methods.Internal.Settings.UseDefaultPrefrences();
            return Instance.SetGameConfig();
        }
        public static void SetupObjectsList()
        {
            Instance.ObjectList.Clear();
            for (int i = 0; i < Classes.Editor.Solution.GameConfig.ObjectsNames.Count; i++)
            {
                Instance.ObjectList.Add(Classes.Editor.Solution.GameConfig.ObjectsNames[i]);
            }
            for (int i = 0; i < Classes.Editor.Solution.StageConfig.ObjectsNames.Count; i++)
            {
                Instance.ObjectList.Add(Classes.Editor.Solution.StageConfig.ObjectsNames[i]);
            }
        }
        public static void SetupDiscordRP(string SceneFile)
        {
            DiscordRP.UpdateDiscord(SceneFile);
        }
        public static void ReadManiacINIFile()
        {
            Methods.Prefrences.SceneCurrentSettings.ClearSettings();
            if (File.Exists(ManiacEditor.Classes.Editor.Solution.Paths.SceneFile_Source.SourceDirectory + "\\maniac.json"))
            {
                Methods.Prefrences.SceneCurrentSettings.UpdateFilePath();
                Methods.Prefrences.SceneCurrentSettings.LoadFile();

            }
        }
        public static void LoadingFailed(Exception ex)
        {
            MessageBox.Show("Load failed. Error: " + ex.ToString());
            UnloadScene(true);
        }
        public static void LoadingFailed(string ex)
        {
            MessageBox.Show("Load failed. Error: " + ex);
            UnloadScene(true);
        }
        #endregion

        #region Saving
        public static void SaveAsExtras(string saveAsFolder)
        {
            string stageConfig = Path.Combine(saveAsFolder, "StageConfig.bin");
            string stamps = Path.Combine(saveAsFolder, "ManiacStamps.bin");
            string stageTiles = Path.Combine(saveAsFolder, "16x16Tiles.gif");
            string tilesConfig = Path.Combine(saveAsFolder, "TileConfig.bin");

            bool stageConfigExists = File.Exists(stageConfig);
            bool stampsExists = File.Exists(stamps);
            bool stageTilesExists = File.Exists(stageTiles);
            bool tilesConfigExists = File.Exists(tilesConfig);

            if (!stageConfigExists || !stampsExists || !stageTilesExists || !tilesConfigExists)
            {
                string newLine = Environment.NewLine;
                string missingFiles = "The following files do not exist in this location:";
                if (!stageConfigExists) missingFiles += newLine + "StageConfig.bin";
                if (!stampsExists) missingFiles += newLine + "ManiacStamps.bin";
                if (!stageTilesExists) missingFiles += newLine + "16x16Tiles.gif";
                if (!tilesConfigExists) missingFiles += newLine + "TileConfig.bin";
                missingFiles += newLine + newLine + "Would you like to make these files? (If you don't this may screw up the loading system)";
                MessageBoxResult result = MessageBox.Show(missingFiles, "Missing Files", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (!stageConfigExists) SaveStageConfig(true, stageConfig);
                    if (!stampsExists) SaveChunks(true, stamps);
                    if (!stageTilesExists) Save16x16Tiles(true, stageTiles);
                    if (!tilesConfigExists) SaveTilesConfig(true, tilesConfig);
                }
            }


        }
		public static void SaveScene(bool saveAsMode = false, string SaveAsFilePath = "")
		{
			try
			{
                if (saveAsMode)
                {
                    Classes.Editor.Solution.CurrentScene.Save(SaveAsFilePath);
                    ManiacEditor.Classes.Editor.Solution.Paths.SceneFile_Source = new Solution.Paths.FileSource(-3, SaveAsFilePath);
                }
                else
                {
                    if (Classes.Editor.SolutionState.DataDirectoryReadOnlyMode && ManiacEditor.Classes.Editor.Solution.Paths.SceneFile_Source.SourceID == -1) return;
                    else Classes.Editor.Solution.CurrentScene.Save(ManiacEditor.Classes.Editor.Solution.Paths.SceneFile_Source.SourcePath);
                }

			}
			catch (Exception ex)
			{
                Methods.Internal.Common.ShowError($@"Failed to save the scene to file '{ManiacEditor.Classes.Editor.Solution.Paths.SceneFile_Source.SourcePath}' Error: {ex.Message}");
			}
		}
		public static void SaveStageConfig(bool saveAsMode = false, string SaveAsFilePath = "")
		{
			try
			{
                if (saveAsMode)
                {
                    Classes.Editor.Solution.StageConfig?.Write(SaveAsFilePath);
                    ManiacEditor.Classes.Editor.Solution.Paths.StageConfig_Source = new Solution.Paths.FileSource(-3, SaveAsFilePath);
                }
                else
                {
                    if (Classes.Editor.SolutionState.DataDirectoryReadOnlyMode && ManiacEditor.Classes.Editor.Solution.Paths.StageConfig_Source.SourceID == -1) return;
                    else Classes.Editor.Solution.StageConfig?.Write(ManiacEditor.Classes.Editor.Solution.Paths.StageConfig_Source.SourcePath);
                }

			}
			catch (Exception ex)
			{
				Methods.Internal.Common.ShowError($@"Failed to save the StageConfig to file '{ManiacEditor.Classes.Editor.Solution.Paths.StageConfig_Source.SourcePath}' Error: {ex.Message}");
			}
		}
        public static void SaveChunks(bool saveAsMode = false, string SaveAsFilePath = "")
        {
            try
            {
                if (Instance.Chunks.StageStamps?.loadstate == RSDKv5.Stamps.LoadState.Upgrade)
                {
                    MessageBoxResult result = MessageBox.Show("This Editor Chunk File needs to be updated to a newer version of the format. This will happen almost instantly, however you will be unable to use your chunks in a previous version of maniac on this is done. Would you like to continue?" + Environment.NewLine + "(Click Yes to Save, Click No to Continue without Saving Your Chunks)", "Chunk File Format Upgrade Required", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result != MessageBoxResult.Yes) return;
                }
                if (saveAsMode)
                {
                    Instance.Chunks.StageStamps?.Write(SaveAsFilePath);
                    ManiacEditor.Classes.Editor.Solution.Paths.Stamps_Source = new Solution.Paths.FileSource(-3, SaveAsFilePath);
                }
                else
                {
                    if (Classes.Editor.SolutionState.DataDirectoryReadOnlyMode && ManiacEditor.Classes.Editor.Solution.Paths.Stamps_Source.SourceID == -1) return;
                    else Instance.Chunks.StageStamps?.Write(ManiacEditor.Classes.Editor.Solution.Paths.Stamps_Source.SourcePath);
                }



            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError($@"Failed to save StageStamps to file '{ManiacEditor.Classes.Editor.Solution.Paths.Stamps_Source.SourcePath}' Error: {ex.Message}");
            }
        }
        public static void SaveTilesConfig(bool saveAsMode = false, string SaveAsFilePath = "")
        {
            try
            {
                if (saveAsMode)
                {
                    Classes.Editor.Solution.TileConfig?.Write(SaveAsFilePath);
                    ManiacEditor.Classes.Editor.Solution.Paths.TileConfig_Source = new Solution.Paths.FileSource(-3, SaveAsFilePath);
                }
            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError($@"Failed to save the TileConfig to file '{ManiacEditor.Classes.Editor.Solution.Paths.TileConfig_Source.SourcePath}' Error: {ex.Message}");
            }
        }
        public static void Save16x16Tiles(bool saveAsMode = false, string SaveAsFilePath = "")
        {
            try
            {
                if (saveAsMode)
                {
                    Classes.Editor.Solution.CurrentTiles?.Write(SaveAsFilePath);
                    ManiacEditor.Classes.Editor.Solution.Paths.StageTiles_Source = new Solution.Paths.FileSource(-3, SaveAsFilePath);
                }
            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError($@"Failed to save the 16x16Tiles.gif to file '{ManiacEditor.Classes.Editor.Solution.Paths.StageTiles_Source.SourcePath}' Error: {ex.Message}");
            }
        }
        #endregion

        #region Broken Backup/Recovery Tool (BROKEN)

        //TODO : Fix this Bloody Mess Over Here

        public static void BackupRecoverButton_Click(object sender, RoutedEventArgs e)
        {
            string Result = null, ResultOriginal = null, ResultOld = null;
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog
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

            Classes.Editor.Solution.UnloadScene();
            Methods.Internal.Settings.UseDefaultPrefrences();
            File.Replace(Result, ResultOriginal, ResultOld);

        }

        public static void StageConfigBackup(object sender, RoutedEventArgs e)
        {
            //StageConfigBackup(sender, e);
        }

        public static void SceneBackup(object sender, RoutedEventArgs e)
        {
            //SceneBackup(sender, e);
        }

        #endregion

    }
}
