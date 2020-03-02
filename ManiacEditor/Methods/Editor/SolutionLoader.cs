using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using RSDKv5;
using GenerationsLib.Core;
using ManiacEditor.Classes.Scene;
using System.IO;


namespace ManiacEditor.Methods.Editor
{
    public static class SolutionLoader
    {
        private static bool EditorLoad()
        {
            if (ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory == null)
            {
                return false;
            }
            Instance.EntityDrawing.ReleaseResources();
            return true;
        }

        #region Instance
        private static Controls.Editor.MainEditor Instance;
        public static void UpdateInstance(Controls.Editor.MainEditor instance)
        {
            Instance = instance;
        }
        #endregion

        #region Editor Commands

        public static void NewScene()
        {
            if (AllowSceneUnloading() != true) return;
            Methods.Editor.Solution.UnloadScene();
            ManiacEditor.Controls.SceneSelect.NewSceneWindow makerDialog = new ManiacEditor.Controls.SceneSelect.NewSceneWindow();
            makerDialog.Owner = Controls.Editor.MainEditor.GetWindow(Instance);
            if (makerDialog.ShowDialog() == true)
            {
                string directoryPath = Path.GetDirectoryName(makerDialog.SceneFolder);

                Methods.Editor.Solution.CurrentScene = new Classes.Scene.EditorScene(Instance.ViewPanel.SharpPanel.GraphicPanel, makerDialog.Scene_Width, makerDialog.Scene_Height, makerDialog.BG_Width, makerDialog.BG_Height, Instance);
                Methods.Editor.Solution.TileConfig = new Tileconfig();
                Methods.Editor.Solution.CurrentTiles = new Classes.Scene.EditorTiles();
                Methods.Editor.Solution.StageConfig = new StageConfig();

                string ImagePath = directoryPath + "//16x16Tiles.gif";
                string TilesPath = directoryPath + "//TilesConfig.bin";
                string StagePath = directoryPath + "//StageConfig.bin";

                File.Create(ImagePath).Dispose();
                File.Create(TilesPath).Dispose();
                File.Create(StagePath).Dispose();

                //EditorScene.Write(SceneFilepath);
                Methods.Editor.Solution.TileConfig.Write(TilesPath);
                //StageConfig.Write(StagePath);
                Methods.Editor.Solution.CurrentTiles.Write(ImagePath);


                Instance.UpdateDataFolderLabel(null, null);

                Instance.SetupLayerButtons();


                Instance.EditBackground = new Classes.Scene.EditorBackground(Instance);

                Methods.Editor.Solution.Entities = new Classes.Scene.EditorEntities(Methods.Editor.Solution.CurrentScene);

                Instance.ViewPanel.SharpPanel.ResizeGraphicsPanel();

                Methods.Internal.UserInterface.UpdateControls(true);
            }
        }
        public static void OpenScene()
        {
            if (ManiacEditor.Methods.Editor.SolutionLoader.AllowSceneUnloading() != true) return;
            UnloadScene(true);

            OpenSceneByItself();
        }
        public static void OpenDataDirectory()
        {
            if (ManiacEditor.Methods.Editor.SolutionLoader.AllowSceneUnloading() != true) return;
            UnloadScene(true);

            OpenSceneFromDataDirectory();
        }
        public static void OpenSceneSelect()
        {
            if (ManiacEditor.Methods.Editor.SolutionLoader.AllowSceneUnloading() != true) return;
            UnloadScene(true);

            OpenSceneUsingSceneSelect();
        }
        public static void Save()
        {
            if (Methods.Editor.Solution.CurrentScene == null) return;
            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit()) Methods.Editor.EditorActions.Deselect();

            SaveScene();
            SaveStageConfig();
            SaveChunks();
        }
        public static void SaveAs()
        {
            if (Methods.Editor.Solution.CurrentScene == null) return;
            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit()) Methods.Editor.EditorActions.Deselect();

            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "Scene File|*.bin",
                DefaultExt = "bin",
                InitialDirectory = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneDirectory,
                RestoreDirectory = false,
                FileName = System.IO.Path.GetFileName(ManiacEditor.Methods.Editor.SolutionPaths.SceneFile_Source.SourcePath)
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
            Methods.Editor.Solution.UnloadScene();
        }
        public static bool AllowSceneUnloading(bool SkipCheck = false)
        {
            if (SkipCheck) return true;
            bool AllowSceneChange = false;
            if (ManiacEditor.Methods.Editor.SolutionState.IsSceneLoaded() == false)
            {
                AllowSceneChange = true;
                return AllowSceneChange;
            }
            else if (ManiacEditor.Methods.Editor.SolutionState.IsSceneLoaded() == true && ManiacEditor.Properties.Settings.MySettings.DisableSaveWarnings == false)
            {

                if ((Instance.UndoStack.Count != 0 || Instance.RedoStack.Count != 0) || Methods.Editor.SolutionState.QuitWithoutSavingWarningRequired == true)
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
            
            if (Methods.Editor.Solution.CurrentScene == null) return;

            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog
            {
                Filter = ".png File|*.png",
                DefaultExt = "png"
            };
            if (save.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(Methods.Editor.Solution.SceneWidth, Methods.Editor.Solution.SceneHeight))
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    // not all scenes have both a Low and a High foreground
                    // only attempt to render the ones we actually have
                    Methods.Editor.Solution.FGLower?.Draw(g);
                    Methods.Editor.Solution.FGLow?.Draw(g);
                    Methods.Editor.Solution.FGHigh?.Draw(g);
                    Methods.Editor.Solution.FGHigher?.Draw(g);
                    Methods.Editor.Solution.Entities?.Draw(g);

                    bitmap.Save(save.FileName);
                }

            }
        }
        public static void ExportLayersAsPNG()
        {
            try
            {
                if (Methods.Editor.Solution.CurrentScene?._editorLayers == null || !Methods.Editor.Solution.CurrentScene._editorLayers.Any()) return;

                var dialog = new GenerationsLib.Core.FolderSelectDialog()
                {
                    Title = "Select folder to save each exported layer image to"
                };

                if (!dialog.ShowDialog()) return;

                int fileCount = 0;

                foreach (var editorLayer in Methods.Editor.Solution.CurrentScene.AllLayers)
                {
                    string fileName = System.IO.Path.Combine(dialog.FileName, editorLayer.Name + ".png");

                    if (!Methods.Internal.Common.CanWriteFile(fileName))
                    {
                        Methods.Internal.Common.ShowError($"Layer export aborted. {fileCount} images saved.");
                        return;
                    }

                    using (var bitmap = new System.Drawing.Bitmap(editorLayer.Width * Methods.Editor.EditorConstants.TILE_SIZE, editorLayer.Height * Methods.Editor.EditorConstants.TILE_SIZE))
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
                if (Methods.Editor.Solution.CurrentScene?._editorLayers == null || !Methods.Editor.Solution.CurrentScene._editorLayers.Any()) return;

                var dialog = new FolderSelectDialog()
                {
                    Title = "Select folder to save each exported object layout image to"
                };

                if (!dialog.ShowDialog()) return;

                int fileCount = 0;

                string fileName = System.IO.Path.Combine(dialog.FileName, "Objects.png");

                using (var bitmap = new System.Drawing.Bitmap(1024 * Methods.Editor.EditorConstants.TILE_SIZE, 256 * Methods.Editor.EditorConstants.TILE_SIZE))
                {
                    using (var g = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        for (i = 0; i < Methods.Editor.Solution.Entities.Entities.Count; i++)
                        {
                            //if (!Instance.CanWriteFile(fileName))
                            // {
                            //    Methods.Internal.Common.ShowError($"Layout export aborted. {fileCount} images saved.");
                            //    return;
                            //}
                            try
                            {
                                Methods.Editor.Solution.Entities.Entities.ToList()[i].ExportDraw(g,false);
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
        public static void OpenSceneByItself()
        {
            SolutionPaths.CurrentSceneData.Clear();
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.Filter = "Scene File|*.bin";
            if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                if (PreLoad() == false) return;
                var state = new Classes.General.SceneState()
                {
                    FilePath = open.FileName,
                    LevelID = -1,
                    IsEncoreMode = false,
                    SceneID = "N/A",
                    SceneDirectory = System.IO.Path.GetDirectoryName(open.FileName),
                    Zone = "N/A",
                    LoadType = Classes.General.SceneState.LoadMethod.SelfLoaded,
                    Name = "N/A",
                    MasterDataDirectory = SolutionPaths.DefaultMasterDataDirectory
                };
                SetPathData(state);
                LoadSequence();
            }

        }
        public static void OpenSceneFromDataDirectory()
        {
            SolutionPaths.CurrentSceneData.Clear();
            ManiacEditor.Controls.SceneSelect.SceneSelectWindow select;

            if (!EditorLoad())
            {
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(null, Instance, true);
                select.Owner = Instance;
                select.ShowDialog();
            }
            else
            {
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(Methods.Editor.Solution.GameConfig, Instance, true);
                select.Owner = Instance;
                select.ShowDialog();
            }

            if (select.DialogResult != true) return;
            if (PreLoad() == false) return;

            SetPathData(select.SceneSelect.SceneState);

            LoadSequence();

        }
        public static void OpenSceneUsingSceneSelect()
        {
            SolutionPaths.CurrentSceneData.Clear();
            ManiacEditor.Controls.SceneSelect.SceneSelectWindow select;

            if (!EditorLoad())
            {
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(null, Instance);
                select.Owner = Instance;
                select.ShowDialog();
            }
            else
            {
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(Methods.Editor.Solution.GameConfig, Instance);
                select.Owner = Instance;
                select.ShowDialog();
            }

            if (select.DialogResult != true) return;
            if (PreLoad() == false) return;

            SetPathData(select.SceneSelect.SceneState);

            LoadSequence();

        }
        public static void OpenSceneUsingSceneSelect(ManiacEditor.Controls.SceneSelect.SceneSelectHost select)
        {
            SolutionPaths.CurrentSceneData.Clear();
            if (PreLoad() == false) return;

            SetPathData(select.SceneState);

            LoadSequence();
        }
        public static void OpenSceneFromSaveState(Classes.Prefrences.SceneHistoryStorage.SceneHistoryCollection.SaveState saveState)
        {
            SolutionPaths.CurrentSceneData.Clear();
            if (PreLoad() == false) return;

            ManiacEditor.Methods.Editor.SolutionPaths.SetGameConfig(saveState.SceneState.MasterDataDirectory);

            SetPathData(saveState.SceneState);

            LoadSequence();
        }
        public static void OpenSceneSelectSaveState(ManiacEditor.Classes.Prefrences.DataStateHistoryStorage.DataStateHistoryCollection.SaveState SaveState)
        {
            SolutionPaths.CurrentSceneData.Clear();
            ManiacEditor.Controls.SceneSelect.SceneSelectWindow select;
            ManiacEditor.Methods.Editor.SolutionPaths.SetGameConfig(SaveState.MasterDataDirectory);

            select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow((EditorLoad() ? Methods.Editor.Solution.GameConfig : null), Instance);

            select.Owner = Instance;
            select.SceneSelect.LoadSaveState(SaveState);
            select.ShowDialog();


            if (select.DialogResult != true) return;
            if (PreLoad() == false) return;

            SetPathData(select.SceneSelect.SceneState);
            LoadSequence();
        }

        #endregion

        #region Set Path Data
        public static void AddTemporaryResourcePack()
        {
            if (ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneDirectory.Contains("Data") && ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneDirectory.Contains("Stages"))
            {
                var input = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneDirectory;
                var output = input.Replace("\\Stages\\" + ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, "");
                if (output.Contains("Data"))
                {
                    ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.ExtraDataDirectories.Add(output);
                }
            }
        }
        public static void SetPathData(Classes.General.SceneState sceneState)
        {
            if (ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData == null) ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData = new Classes.General.SceneState();
            ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData = new Classes.General.SceneState()
            {
                FilePath = sceneState.FilePath,
                LevelID = sceneState.LevelID,
                IsEncoreMode = sceneState.IsEncoreMode,
                SceneID = sceneState.SceneID,
                SceneDirectory = sceneState.SceneDirectory,
                Zone = sceneState.Zone,
                LoadType = sceneState.LoadType,
                Name = sceneState.Name,
                ExtraDataDirectories = sceneState.ExtraDataDirectories,
                MasterDataDirectory = sceneState.MasterDataDirectory
            };
        }

        #endregion

        #region Loading
        public static bool PreLoad()
        {
            try
            {
                Methods.Editor.Solution.UnloadScene();
                Methods.Internal.Settings.UseDefaultPrefrences();
                ManiacEditor.Methods.Editor.SolutionPaths.SetGameConfig();
                return true;
            }
            catch (Exception ex)
            {
                LoadingFailed(ex);
                return false;
            }

        }
        public static void LoadSequence()
        {
            bool LoadFailed = false;
            try
            {
                #region Scene File
                try
                {
                    Methods.Editor.SolutionState.LevelID = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.LevelID;
                    string getPath = ManiacEditor.Methods.Editor.SolutionPaths.GetScenePath();
                    Methods.Editor.Solution.CurrentScene = new Classes.Scene.EditorScene(getPath, Instance.ViewPanel.SharpPanel.GraphicPanel, Instance);
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
                    if (ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsFullPath)
                    {
                        ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette = Methods.Editor.Solution.CurrentScene.GetEncorePalette(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneID, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneDirectory, 0);
                        Methods.Editor.SolutionState.EncoreSetupType = Methods.Editor.Solution.CurrentScene.GetEncoreSetupType(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneID, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneDirectory);
                    }
                    else
                    {
                        ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette = Methods.Editor.Solution.CurrentScene.GetEncorePalette(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneID, "", 1);
                        Methods.Editor.SolutionState.EncoreSetupType = Methods.Editor.Solution.CurrentScene.GetEncoreSetupType(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneID, "");
                    }

                    if (ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette[0] != "")
                    {
                        Methods.Editor.SolutionState.EncorePaletteExists = true;
                        if (ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsEncoreMode)
                        {
                            Instance.EditorToolbar.EncorePaletteButton.IsChecked = true;
                            Methods.Editor.SolutionState.UseEncoreColors = true;
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
                    if (Methods.Editor.SolutionState.UseEncoreColors == true && ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette[0] != "") valid = ManiacEditor.Methods.Editor.SolutionPaths.GetStageTiles(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette[0], ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsFullPath);
                    else valid = ManiacEditor.Methods.Editor.SolutionPaths.GetStageTiles(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, null, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsFullPath);
                    if (valid == false)
                    {
                        throw new Exception("16x16Tiles.gif was unable to be found or could not be loaded properly!");
                    }
                }
                catch (Exception ex)
                {
                    LoadFailed = true;
                    LoadingFailed(ex);
                    return;
                }
                #endregion

                #region Tile Config

                try
                {
                    bool valid = ManiacEditor.Methods.Editor.SolutionPaths.GetTileConfig(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsFullPath);
                    if (valid == false)
                    {
                        throw new Exception("TileConfig.bin was unable to be found or could not be loaded properly!");
                    }
                }
                catch (Exception ex)
                {
                    LoadFailed = true;
                    LoadingFailed(ex);
                    return;
                }

                #endregion

                #region Stage Config

                try
                {
                    bool valid = ManiacEditor.Methods.Editor.SolutionPaths.GetStageConfig(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsFullPath);
                    if (valid == false)
                    {
                        throw new Exception("StageConfig.bin was unable to be found!");
                    }
                }
                catch (Exception ex)
                {
                    LoadFailed = true;
                    LoadingFailed(ex);
                    return;
                }

                #endregion
            }
            catch (Exception ex)
            {
                LoadFailed = true;
                LoadingFailed(ex);
                return;
            }
            if (!LoadFailed) PostLoad();
        }
        public static void PostLoad()
        {
            /*
            try
            {


            }
            catch (Exception ex)
            {
                LoadingFailed(ex);
                return;
            }*/

            SetupObjectsList();
            SetupDiscordRP(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.FilePath);
            Stamps StageStamps = ManiacEditor.Methods.Editor.SolutionPaths.GetEditorStamps(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone);
            Instance.Chunks = new Classes.Scene.EditorChunks(Instance, Methods.Editor.Solution.CurrentTiles, StageStamps);
            Instance.EditBackground = new Classes.Scene.EditorBackground(Instance);
            Methods.Editor.Solution.Entities = new Classes.Scene.EditorEntities(Methods.Editor.Solution.CurrentScene);

            Methods.Internal.UserInterface.UpdateSplineSpawnObjectsList(Methods.Editor.Solution.CurrentScene.Objects);

            SetupManiacINIPrefs();
            Instance.UpdateStartScreen(false);
            Instance.UpdateDataFolderLabel(null, null);
            Instance.SetupLayerButtons();
            Methods.Editor.SolutionState.UpdateMultiLayerSelectMode();
            Methods.Internal.UserInterface.UpdateControls(true);
            Classes.Prefrences.SceneHistoryStorage.AddRecentFile(Classes.Prefrences.SceneHistoryStorage.GenerateNewEntry());
            ManiacEditor.Classes.Prefrences.DataStateHistoryStorage.AddRecentFile(ManiacEditor.Classes.Prefrences.DataStateHistoryStorage.GenerateNewEntry());
            Instance.ViewPanel.SharpPanel.ResetZoomLevel();

        }
        public static void LoadingFailed(Exception ex)
        {
            bool justCrash = false;
            if (justCrash)
            {
                throw ex;
            }
            else
            {
                MessageBox.Show("Load failed. Error: " + ex.ToString());
                UnloadScene(true);
            }

        }
        #endregion

        #region Loading Setups

        public static void SetupObjectsList()
        {
            Instance.ObjectList.Clear();
            if (Methods.Editor.Solution.GameConfig != null)
            {
                for (int i = 0; i < Methods.Editor.Solution.GameConfig.ObjectsNames.Count; i++)
                {
                    Instance.ObjectList.Add(Methods.Editor.Solution.GameConfig.ObjectsNames[i]);
                }
            }
            if (Methods.Editor.Solution.StageConfig != null)
            {
                for (int i = 0; i < Methods.Editor.Solution.StageConfig.ObjectsNames.Count; i++)
                {
                    Instance.ObjectList.Add(Methods.Editor.Solution.StageConfig.ObjectsNames[i]);
                }
            }


        }
        public static void SetupDiscordRP(string SceneFile)
        {
            DiscordRP.UpdateDiscord(SceneFile);
        }
        public static void SetupManiacINIPrefs()
        {
            Classes.Prefrences.SceneCurrentSettings.ClearSettings();
            if (File.Exists(ManiacEditor.Methods.Editor.SolutionPaths.SceneFile_Source.SourceDirectory + "\\maniac.json"))
            {
                Classes.Prefrences.SceneCurrentSettings.UpdateFilePath();
                Classes.Prefrences.SceneCurrentSettings.LoadFile();

            }
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
                    Methods.Editor.Solution.CurrentScene.Save(SaveAsFilePath);
                    ManiacEditor.Methods.Editor.SolutionPaths.SceneFile_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
                else
                {
                    if (Methods.Editor.SolutionState.DataDirectoryReadOnlyMode && ManiacEditor.Methods.Editor.SolutionPaths.SceneFile_Source.SourceID == -1) return;
                    else Methods.Editor.Solution.CurrentScene.Save(ManiacEditor.Methods.Editor.SolutionPaths.SceneFile_Source.SourcePath);
                }

			}
			catch (Exception ex)
			{
                Methods.Internal.Common.ShowError($@"Failed to save the scene to file '{ManiacEditor.Methods.Editor.SolutionPaths.SceneFile_Source.SourcePath}' Error: {ex.Message}");
			}
		}
		public static void SaveStageConfig(bool saveAsMode = false, string SaveAsFilePath = "")
		{
			try
			{
                if (saveAsMode)
                {
                    Methods.Editor.Solution.StageConfig?.Write(SaveAsFilePath);
                    ManiacEditor.Methods.Editor.SolutionPaths.StageConfig_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
                else
                {
                    if (Methods.Editor.SolutionState.DataDirectoryReadOnlyMode && ManiacEditor.Methods.Editor.SolutionPaths.StageConfig_Source.SourceID == -1) return;
                    else Methods.Editor.Solution.StageConfig?.Write(ManiacEditor.Methods.Editor.SolutionPaths.StageConfig_Source.SourcePath);
                }

			}
			catch (Exception ex)
			{
				Methods.Internal.Common.ShowError($@"Failed to save the StageConfig to file '{ManiacEditor.Methods.Editor.SolutionPaths.StageConfig_Source.SourcePath}' Error: {ex.Message}");
			}
		}
        public static void SaveChunks(bool saveAsMode = false, string SaveAsFilePath = "")
        {
            try
            {
                if (Instance.Chunks.StageStamps != null)
                {
                    if (Instance.Chunks.StageStamps?.loadstate == RSDKv5.Stamps.LoadState.Upgrade)
                    {
                        MessageBoxResult result = MessageBox.Show("This Editor Chunk File needs to be updated to a newer version of the format. This will happen almost instantly, however you will be unable to use your chunks in a previous version of maniac on this is done. Would you like to continue?" + Environment.NewLine + "(Click Yes to Save, Click No to Continue without Saving Your Chunks)", "Chunk File Format Upgrade Required", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result != MessageBoxResult.Yes) return;
                    }
                    if (saveAsMode)
                    {
                        Instance.Chunks.StageStamps?.Write(SaveAsFilePath);
                        ManiacEditor.Methods.Editor.SolutionPaths.Stamps_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                    }
                    else
                    {
                        if (Methods.Editor.SolutionState.DataDirectoryReadOnlyMode && ManiacEditor.Methods.Editor.SolutionPaths.Stamps_Source.SourceID == -1) return;
                        else Instance.Chunks.StageStamps?.Write(ManiacEditor.Methods.Editor.SolutionPaths.Stamps_Source.SourcePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError($@"Failed to save StageStamps to file '{ManiacEditor.Methods.Editor.SolutionPaths.Stamps_Source.SourcePath}' Error: {ex.Message}");
            }
        }
        public static void SaveTilesConfig(bool saveAsMode = false, string SaveAsFilePath = "")
        {
            try
            {
                if (saveAsMode)
                {
                    Methods.Editor.Solution.TileConfig?.Write(SaveAsFilePath);
                    ManiacEditor.Methods.Editor.SolutionPaths.TileConfig_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError($@"Failed to save the TileConfig to file '{ManiacEditor.Methods.Editor.SolutionPaths.TileConfig_Source.SourcePath}' Error: {ex.Message}");
            }
        }
        public static void Save16x16Tiles(bool saveAsMode = false, string SaveAsFilePath = "")
        {
            try
            {
                if (saveAsMode)
                {
                    Methods.Editor.Solution.CurrentTiles?.Write(SaveAsFilePath);
                    ManiacEditor.Methods.Editor.SolutionPaths.StageTiles_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError($@"Failed to save the 16x16Tiles.gif to file '{ManiacEditor.Methods.Editor.SolutionPaths.StageTiles_Source.SourcePath}' Error: {ex.Message}");
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

            Methods.Editor.Solution.UnloadScene();
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
