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
        private static bool EditorLoad()
        {
            if (ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory == null)
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
            Classes.Editor.Solution.UnloadScene();
            ManiacEditor.Controls.SceneSelect.NewSceneWindow makerDialog = new ManiacEditor.Controls.SceneSelect.NewSceneWindow();
            makerDialog.Owner = Controls.Editor.MainEditor.GetWindow(Instance);
            if (makerDialog.ShowDialog() == true)
            {
                string directoryPath = Path.GetDirectoryName(makerDialog.SceneFolder);

                Classes.Editor.Solution.CurrentScene = new Classes.Editor.Scene.EditorScene(Instance.ViewPanel.SharpPanel.GraphicPanel, makerDialog.Scene_Width, makerDialog.Scene_Height, makerDialog.BG_Width, makerDialog.BG_Height, Instance);
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


                Instance.EditBackground = new Classes.Editor.Scene.EditorBackground(Instance);

                Classes.Editor.Solution.Entities = new Classes.Editor.Scene.EditorEntities(Classes.Editor.Solution.CurrentScene);

                Instance.ViewPanel.SharpPanel.ResizeGraphicsPanel();

                Methods.Internal.UserInterface.UpdateControls(true);
            }
        }
        public static void OpenScene()
        {
            if (ManiacEditor.Classes.Editor.SolutionLoader.AllowSceneUnloading() != true) return;
            Classes.Editor.Solution.UnloadScene();

            OpenSceneByItself();
        }
        public static void OpenSceneSelect()
        {
            OpenSceneUsingSceneSelect();
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
                InitialDirectory = ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneDirectory,
                RestoreDirectory = false,
                FileName = System.IO.Path.GetFileName(ManiacEditor.Classes.Editor.SolutionPaths.SceneFile_Source.SourcePath)
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
            else if (ManiacEditor.Classes.Editor.SolutionState.IsSceneLoaded() == true && ManiacEditor.Methods.Settings.MySettings.DisableSaveWarnings == false)
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

        #region Open Scene By Itself
        public static void OpenSceneByItself()
        {
            SolutionPaths.CurrentSceneData.Clear();
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.Filter = "Scene File|*.bin";
            if (open.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                if (PreLoad() == false) return;
                var state = new Internal.SceneState()
                {
                    FilePath = open.FileName,
                    LevelID = -1,
                    IsEncoreMode = false,
                    SceneID = "N/A",
                    SceneDirectory = System.IO.Path.GetDirectoryName(open.FileName),
                    Zone = "N/A",
                    LoadType = Internal.SceneState.LoadMethod.SelfLoaded,
                    Name = "N/A"
                };
                SetPathData(state);
                LoadSequence();
            }

        }
        #endregion

        #region Open Scene Using Scene Select
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
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(Classes.Editor.Solution.GameConfig, Instance);
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

        #endregion

        #region Open Scene From Save State

        public static void OpenSceneFromSaveState(Classes.Internal.SceneHistoryCollection.SaveState saveState)
        {
            SolutionPaths.CurrentSceneData.Clear();
            if (PreLoad() == false) return;

            ManiacEditor.Classes.Editor.SolutionPaths.SetGameConfig(saveState.SceneState.DataDirectory);

            SetPathData(saveState.SceneState);

            LoadSequence();
        }

        #endregion

        #region Open Scene From Data Pack Save State
        public static void OpenSceneSelectFromPreviousDataPackSetup(ManiacEditor.Classes.Internal.DataStateHistoryCollection.SaveState SaveState)
        {
            SolutionPaths.CurrentSceneData.Clear();
            ManiacEditor.Controls.SceneSelect.SceneSelectWindow select;
            ManiacEditor.Classes.Editor.SolutionPaths.SetGameConfig(SaveState.DataDirectory);

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

            SetPathData(select.SceneSelect.SceneState);
            LoadSequence();
        }

        #endregion

        #region Set Path Data
        public static void AddTemporaryResourcePack()
        {
            if (ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneDirectory.Contains("Data") && ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneDirectory.Contains("Stages"))
            {
                var input = ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneDirectory;
                var output = input.Replace("\\Stages\\" + ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone, "");
                if (output.Contains("Data"))
                {
                    ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.ResourcePacks.Add(output);
                }
            }
        }
        public static void SetPathData(Internal.SceneState sceneState)
        {
            if (ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData == null) ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData = new Internal.SceneState();
            ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData = new Internal.SceneState()
            {
                FilePath = sceneState.FilePath,
                LevelID = sceneState.LevelID,
                IsEncoreMode = sceneState.IsEncoreMode,
                SceneID = sceneState.SceneID,
                SceneDirectory = sceneState.SceneDirectory,
                Zone = sceneState.Zone,
                LoadType = sceneState.LoadType,
                Name = sceneState.Name,
                ResourcePacks = sceneState.ResourcePacks
            };
            ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SetDataDirectory(sceneState.DataDirectory);
        }

        #endregion

        #region Loading
        public static bool PreLoad()
        {
            try
            {
                Classes.Editor.Solution.UnloadScene();
                Methods.Internal.Settings.UseDefaultPrefrences();
                ManiacEditor.Classes.Editor.SolutionPaths.SetGameConfig();
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
                    Classes.Editor.SolutionState.LevelID = ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.LevelID;
                    string getPath = ManiacEditor.Classes.Editor.SolutionPaths.GetScenePath();
                    Classes.Editor.Solution.CurrentScene = new Classes.Editor.Scene.EditorScene(getPath, Instance.ViewPanel.SharpPanel.GraphicPanel, Instance);
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
                    if (ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsFullPath)
                    {
                        ManiacEditor.Classes.Editor.SolutionPaths.EncorePalette = Classes.Editor.Solution.CurrentScene.GetEncorePalette(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneID, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneDirectory, 0);
                        Classes.Editor.SolutionState.EncoreSetupType = Classes.Editor.Solution.CurrentScene.GetEncoreSetupType(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneID, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneDirectory);
                    }
                    else
                    {
                        ManiacEditor.Classes.Editor.SolutionPaths.EncorePalette = Classes.Editor.Solution.CurrentScene.GetEncorePalette(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneID, "", 1);
                        Classes.Editor.SolutionState.EncoreSetupType = Classes.Editor.Solution.CurrentScene.GetEncoreSetupType(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneID, "");
                    }

                    if (ManiacEditor.Classes.Editor.SolutionPaths.EncorePalette[0] != "")
                    {
                        Classes.Editor.SolutionState.EncorePaletteExists = true;
                        if (ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsEncoreMode)
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
                    if (Classes.Editor.SolutionState.UseEncoreColors == true && ManiacEditor.Classes.Editor.SolutionPaths.EncorePalette[0] != "") valid = ManiacEditor.Classes.Editor.SolutionPaths.GetStageTiles(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Classes.Editor.SolutionPaths.EncorePalette[0], ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsFullPath);
                    else valid = ManiacEditor.Classes.Editor.SolutionPaths.GetStageTiles(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone, null, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsFullPath);
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
                ManiacEditor.Classes.Editor.SolutionPaths.GetTileConfig(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsFullPath);
                ManiacEditor.Classes.Editor.SolutionPaths.GetStageConfig(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsFullPath);
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
            SetupDiscordRP(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.FilePath);
            Stamps StageStamps = ManiacEditor.Classes.Editor.SolutionPaths.GetEditorStamps(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone);
            Instance.Chunks = new Classes.Editor.Scene.EditorChunks(Instance, Classes.Editor.Solution.CurrentTiles, StageStamps);
            Instance.EditBackground = new Classes.Editor.Scene.EditorBackground(Instance);
            Classes.Editor.Solution.Entities = new Classes.Editor.Scene.EditorEntities(Classes.Editor.Solution.CurrentScene);

            Methods.Internal.UserInterface.UpdateSplineSpawnObjectsList(Classes.Editor.Solution.CurrentScene.Objects);

            SetupManiacINIPrefs();
            Instance.UpdateStartScreen(false);
            Instance.UpdateDataFolderLabel(null, null);
            Instance.SetupLayerButtons();
            Classes.Editor.SolutionState.UpdateMultiLayerSelectMode();
            Methods.Internal.UserInterface.UpdateControls(true);
            Methods.Prefrences.SceneHistoryStorage.AddRecentFile(Methods.Prefrences.SceneHistoryStorage.GenerateNewEntry());
            ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.AddRecentFile(ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.GenerateNewEntry());
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
            if (Classes.Editor.Solution.GameConfig != null)
            {
                for (int i = 0; i < Classes.Editor.Solution.GameConfig.ObjectsNames.Count; i++)
                {
                    Instance.ObjectList.Add(Classes.Editor.Solution.GameConfig.ObjectsNames[i]);
                }
            }
            if (Classes.Editor.Solution.StageConfig != null)
            {
                for (int i = 0; i < Classes.Editor.Solution.StageConfig.ObjectsNames.Count; i++)
                {
                    Instance.ObjectList.Add(Classes.Editor.Solution.StageConfig.ObjectsNames[i]);
                }
            }


        }
        public static void SetupDiscordRP(string SceneFile)
        {
            DiscordRP.UpdateDiscord(SceneFile);
        }
        public static void SetupManiacINIPrefs()
        {
            Methods.Prefrences.SceneCurrentSettings.ClearSettings();
            if (File.Exists(ManiacEditor.Classes.Editor.SolutionPaths.SceneFile_Source.SourceDirectory + "\\maniac.json"))
            {
                Methods.Prefrences.SceneCurrentSettings.UpdateFilePath();
                Methods.Prefrences.SceneCurrentSettings.LoadFile();

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
                    Classes.Editor.Solution.CurrentScene.Save(SaveAsFilePath);
                    ManiacEditor.Classes.Editor.SolutionPaths.SceneFile_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
                else
                {
                    if (Classes.Editor.SolutionState.DataDirectoryReadOnlyMode && ManiacEditor.Classes.Editor.SolutionPaths.SceneFile_Source.SourceID == -1) return;
                    else Classes.Editor.Solution.CurrentScene.Save(ManiacEditor.Classes.Editor.SolutionPaths.SceneFile_Source.SourcePath);
                }

			}
			catch (Exception ex)
			{
                Methods.Internal.Common.ShowError($@"Failed to save the scene to file '{ManiacEditor.Classes.Editor.SolutionPaths.SceneFile_Source.SourcePath}' Error: {ex.Message}");
			}
		}
		public static void SaveStageConfig(bool saveAsMode = false, string SaveAsFilePath = "")
		{
			try
			{
                if (saveAsMode)
                {
                    Classes.Editor.Solution.StageConfig?.Write(SaveAsFilePath);
                    ManiacEditor.Classes.Editor.SolutionPaths.StageConfig_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
                else
                {
                    if (Classes.Editor.SolutionState.DataDirectoryReadOnlyMode && ManiacEditor.Classes.Editor.SolutionPaths.StageConfig_Source.SourceID == -1) return;
                    else Classes.Editor.Solution.StageConfig?.Write(ManiacEditor.Classes.Editor.SolutionPaths.StageConfig_Source.SourcePath);
                }

			}
			catch (Exception ex)
			{
				Methods.Internal.Common.ShowError($@"Failed to save the StageConfig to file '{ManiacEditor.Classes.Editor.SolutionPaths.StageConfig_Source.SourcePath}' Error: {ex.Message}");
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
                    ManiacEditor.Classes.Editor.SolutionPaths.Stamps_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
                else
                {
                    if (Classes.Editor.SolutionState.DataDirectoryReadOnlyMode && ManiacEditor.Classes.Editor.SolutionPaths.Stamps_Source.SourceID == -1) return;
                    else Instance.Chunks.StageStamps?.Write(ManiacEditor.Classes.Editor.SolutionPaths.Stamps_Source.SourcePath);
                }



            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError($@"Failed to save StageStamps to file '{ManiacEditor.Classes.Editor.SolutionPaths.Stamps_Source.SourcePath}' Error: {ex.Message}");
            }
        }
        public static void SaveTilesConfig(bool saveAsMode = false, string SaveAsFilePath = "")
        {
            try
            {
                if (saveAsMode)
                {
                    Classes.Editor.Solution.TileConfig?.Write(SaveAsFilePath);
                    ManiacEditor.Classes.Editor.SolutionPaths.TileConfig_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError($@"Failed to save the TileConfig to file '{ManiacEditor.Classes.Editor.SolutionPaths.TileConfig_Source.SourcePath}' Error: {ex.Message}");
            }
        }
        public static void Save16x16Tiles(bool saveAsMode = false, string SaveAsFilePath = "")
        {
            try
            {
                if (saveAsMode)
                {
                    Classes.Editor.Solution.CurrentTiles?.Write(SaveAsFilePath);
                    ManiacEditor.Classes.Editor.SolutionPaths.StageTiles_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError($@"Failed to save the 16x16Tiles.gif to file '{ManiacEditor.Classes.Editor.SolutionPaths.StageTiles_Source.SourcePath}' Error: {ex.Message}");
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
