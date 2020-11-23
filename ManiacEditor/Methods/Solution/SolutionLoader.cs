using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using RSDKv5;
using GenerationsLib.Core;
using ManiacEditor.Classes.Scene;
using System.IO;


namespace ManiacEditor.Methods.Solution
{
    public static class SolutionLoader
    {
        private static bool EditorLoad()
        {
            if (ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory == null) return false;
            Methods.Drawing.ObjectDrawing.ReleaseResources();
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
            return;

            //TODO: Implement NewScene() Correctly

            /*
            if (UnloadScene() != true) return;
            ManiacEditor.Controls.SceneSelect.NewSceneWindow makerDialog = new ManiacEditor.Controls.SceneSelect.NewSceneWindow();
            makerDialog.Owner = Controls.Editor.MainEditor.GetWindow(Instance);
            if (makerDialog.ShowDialog() == true)
            {
                string directoryPath = Path.GetDirectoryName(makerDialog.SceneFolder);

                Methods.Solution.CurrentSolution.CurrentScene = new Classes.Scene.EditorScene(Instance.ViewPanel.SharpPanel.GraphicPanel, makerDialog.Scene_Width, makerDialog.Scene_Height, makerDialog.BG_Width, makerDialog.BG_Height, Instance);
                Methods.Solution.CurrentSolution.TileConfig = new Tileconfig();
                Methods.Solution.CurrentSolution.CurrentTiles = new Classes.Scene.EditorTiles();
                Methods.Solution.CurrentSolution.StageConfig = new StageConfig();

                string ImagePath = directoryPath + "//16x16Tiles.gif";
                string TilesPath = directoryPath + "//TilesConfig.bin";
                string StagePath = directoryPath + "//StageConfig.bin";

                File.Create(ImagePath).Dispose();
                File.Create(TilesPath).Dispose();
                File.Create(StagePath).Dispose();

                //EditorScene.Write(SceneFilepath);
                Methods.Solution.CurrentSolution.TileConfig.Write(TilesPath);
                //StageConfig.Write(StagePath);
                Methods.Solution.CurrentSolution.CurrentTiles.Write(ImagePath);


                Instance.EditorToolbar.SetupLayerButtons();


                Instance.EditBackground = new Classes.Scene.EditorBackground(Instance);

                Methods.Solution.CurrentSolution.Entities = new Classes.Scene.EditorEntities(Methods.Solution.CurrentSolution.CurrentScene);

                Instance.ViewPanel.SharpPanel.UpdateGraphicsPanelControls();

                Methods.Internal.UserInterface.UpdateControls();

            }
            */
        }
        public static void OpenScene()
        {
            if (UnloadScene() != true) return;
            OpenSceneByItself();
        }
        public static void OpenDataDirectory()
        {
            if (UnloadScene() != true) return;
            OpenSceneFromDataDirectory();
        }
        public static void OpenSceneSelect()
        {
            if (UnloadScene() != true) return;
            OpenSceneUsingSceneSelect();
        }
        public static void OpenSceneForceFully()
        {
            Methods.Solution.SolutionLoader.UnloadScene(true);
            var item = ManiacEditor.Classes.Prefrences.SceneHistoryStorage.Collection.List.FirstOrDefault();
            if (item != null)
            {
                OpenSceneFromSaveState(item);
                Classes.Prefrences.SceneHistoryStorage.AddRecentFile(item);
            }
        }
        public static void Save()
        {
            if (Methods.Solution.CurrentSolution.CurrentScene == null) return;
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) Methods.Solution.SolutionActions.Deselect();

            SaveScene();
            SaveStageConfig();
            SaveChunks();
        }
        public static void SaveAs()
        {
            if (Methods.Solution.CurrentSolution.CurrentScene == null) return;
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) Methods.Solution.SolutionActions.Deselect();

            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "Scene File|*.bin",
                DefaultExt = "bin",
                InitialDirectory = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.SceneDirectory,
                RestoreDirectory = false,
                FileName = System.IO.Path.GetFileName(ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourcePath)
            };
            if (save.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                string saveAsFolder = Path.GetDirectoryName(save.FileName);

                SaveScene(true, save.FileName);
                SaveAsExtras(saveAsFolder);

            }
        }
        public static bool UnloadScene(bool SkipCheck = false)
        {
            if (SkipCheck) return AllowUnload();
            else if (AllowSceneUnloadingDialog() == false) return false;
            else return AllowUnload();

            bool AllowUnload()
            {
                Methods.Solution.CurrentSolution.UnloadScene();
                return true;
            }
        }
        private static bool AllowSceneUnloadingDialog()
        {
            bool AllowSceneChange = false;
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsSceneLoaded() == false)
            {
                AllowSceneChange = true;
                return AllowSceneChange;
            }
            else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsSceneLoaded() == true && ManiacEditor.Properties.Settings.MySettings.DisableSaveWarnings == false)
            {

                if ((Actions.UndoRedoModel.UndoStack.Count != 0 || Actions.UndoRedoModel.RedoStack.Count != 0))
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
        public static bool ExportAsPNG(List<string> Layers)
        {            
            if (Methods.Solution.CurrentSolution.CurrentScene == null) return false;

            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog
            {
                Filter = ".png File|*.png",
                DefaultExt = "png"
            };
            if (save.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                List<EditorLayer> LayerList = new List<EditorLayer>();
                foreach (var entry in Layers)
                {
                    if (Methods.Solution.CurrentSolution.CurrentScene.AllLayers.ToList().Exists(x => x.Name == entry))
                    {
                        LayerList.AddRange(Methods.Solution.CurrentSolution.CurrentScene.AllLayers.Where(x => x.Name == entry));
                    }
                }


                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(Methods.Solution.CurrentSolution.SceneWidth, Methods.Solution.CurrentSolution.SceneHeight))
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    // not all scenes have both a Low and a High foreground
                    // only attempt to render the ones we actually have
                    if (Layers.Contains("FGLower")) Methods.Solution.CurrentSolution.FGLower?.Draw(g);
                    if (Layers.Contains("FGLow")) Methods.Solution.CurrentSolution.FGLow?.Draw(g);
                    if (Layers.Contains("FGHigh")) Methods.Solution.CurrentSolution.FGHigh?.Draw(g);
                    if (Layers.Contains("FGHigher")) Methods.Solution.CurrentSolution.FGHigher?.Draw(g);
                    if (Layers.Contains("Entities")) Methods.Solution.CurrentSolution.Entities?.Draw(g);

                    foreach (var layer in LayerList)
                    {
                        layer.Draw(g);
                    }

                    bitmap.Save(save.FileName);
                }
                return true;
            }
            else return false;
        }
        public static bool ExportLayersAsPNG(List<string> Layers)
        {
            try
            {
                if (Methods.Solution.CurrentSolution.CurrentScene?.AllLayers == null || !Methods.Solution.CurrentSolution.CurrentScene.AllLayers.Any()) return false;

                var dialog = new GenerationsLib.Core.FolderSelectDialog()
                {
                    Title = "Select folder to save each exported layer image to"
                };

                if (!dialog.ShowDialog()) return false;

                int fileCount = 0;


                List<EditorLayer> LayerList = new List<EditorLayer>();
                foreach (var entry in Layers)
                {
                    if (Methods.Solution.CurrentSolution.CurrentScene.AllLayers.ToList().Exists(x => x.Name == entry))
                    {
                        LayerList.AddRange(Methods.Solution.CurrentSolution.CurrentScene.AllLayers.Where(x => x.Name == entry));
                    }
                }

                foreach (var editorLayer in LayerList)
                {
                    string fileName = System.IO.Path.Combine(dialog.FileName, editorLayer.Name + ".png");

                    if (!Methods.Internal.Common.CanWriteFile(fileName))
                    {
                        Methods.Internal.Common.ShowError($"Layer export aborted. {fileCount} images saved.");
                        return false;
                    }

                    using (var bitmap = new System.Drawing.Bitmap(editorLayer.Width * Methods.Solution.SolutionConstants.TILE_SIZE, editorLayer.Height * Methods.Solution.SolutionConstants.TILE_SIZE))
                    using (var g = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        editorLayer.Draw(g);
                        bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                        ++fileCount;
                    }
                }

                MessageBox.Show($"Layer export succeeded. {fileCount} images saved.", "Success!",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError("An error occurred: " + ex.Message);
                return false;
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
                var state = new Structures.SceneState()
                {
                    FilePath = open.FileName,
                    LevelID = -1,
                    IsEncoreMode = false,
                    SceneID = "N/A",
                    SceneDirectory = System.IO.Path.GetDirectoryName(open.FileName),
                    Zone = "N/A",
                    LoadType = Structures.SceneState.LoadMethod.SelfLoaded,
                    Name = "N/A",
                    MasterDataDirectory = SolutionPaths.MasterDataDirectory
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
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(Instance, true);
                select.Owner = Instance;
                select.ShowDialog();
            }
            else
            {
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(Instance, true);
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
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(Instance);
                select.Owner = Instance;
                select.ShowDialog();
            }
            else
            {
                select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(Instance);
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

            ManiacEditor.Methods.Solution.SolutionPaths.SetGameConfig(saveState.SceneState.MasterDataDirectory);

            SetPathData(saveState.SceneState);

            LoadSequence();
        }
        public static void OpenSceneSelectSaveState(ManiacEditor.Classes.Prefrences.DataStateHistoryStorage.DataStateHistoryCollection.SaveState SaveState)
        {
            SolutionPaths.CurrentSceneData.Clear();
            ManiacEditor.Controls.SceneSelect.SceneSelectWindow select;
            ManiacEditor.Methods.Solution.SolutionPaths.SetGameConfig(SaveState.MasterDataDirectory);

            select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(Instance);

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
            if (ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.SceneDirectory.Contains("Data") && ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.SceneDirectory.Contains("Stages"))
            {
                var input = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.SceneDirectory;
                var output = input.Replace("\\Stages\\" + ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone, "");
                if (output.Contains("Data"))
                {
                    ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories.Add(output);
                }
            }
        }
        public static void SetPathData(Structures.SceneState sceneState)
        {
            if (ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData == null) ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData = new Structures.SceneState();
            ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData = new Structures.SceneState()
            {
                Is_IZStage = sceneState.Is_IZStage,
                IZ_SceneKey = sceneState.IZ_SceneKey,
                IZ_StageKey = sceneState.IZ_StageKey,
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
                UnloadScene(true);
                Methods.Internal.Settings.UseDefaultPrefrences();
                ManiacEditor.Methods.Solution.SolutionPaths.SetGameConfig();
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
                if (ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Is_IZStage)
                {
                    ManiacEditor.Methods.Solution.SolutionPaths.SetInfinityConfig();
                    ManiacEditor.Methods.Solution.SolutionPaths.SetInfinityUnlocks();
                    ManiacEditor.Methods.Solution.CurrentSolution.GetIZStage();
                }

                #region Scene File
                try
                {
                    Methods.Solution.CurrentSolution.LevelID = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.LevelID;
                    string getPath = ManiacEditor.Methods.Solution.SolutionPaths.GetScenePath();
                    Methods.Solution.CurrentSolution.CurrentScene = new Classes.Scene.EditorScene(getPath);
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
                    if (ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.IsFullPath)
                    {
                        ManiacEditor.Methods.Solution.SolutionPaths.EncorePalette = Methods.Solution.CurrentSolution.CurrentScene.GetEncorePalette(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.SceneID, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.SceneDirectory, 0);
                        Methods.Solution.SolutionState.Main.EncoreSetupType = Methods.Solution.CurrentSolution.CurrentScene.GetEncoreSetupType(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.SceneID, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.SceneDirectory);
                    }
                    else
                    {
                        ManiacEditor.Methods.Solution.SolutionPaths.EncorePalette = Methods.Solution.CurrentSolution.CurrentScene.GetEncorePalette(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.SceneID, "", 1);
                        Methods.Solution.SolutionState.Main.EncoreSetupType = Methods.Solution.CurrentSolution.CurrentScene.GetEncoreSetupType(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.SceneID, "");
                    }

                    if (ManiacEditor.Methods.Solution.SolutionPaths.EncorePalette[0] != "")
                    {
                        Methods.Solution.SolutionState.Main.IsEncorePaletteLoaded = true;
                        if (ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.IsEncoreMode)
                        {
                            Instance.EditorToolbar.EncorePaletteButton.IsChecked = true;
                            Methods.Solution.SolutionState.Main.UseEncoreColors = true;
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
                    if (Methods.Solution.SolutionState.Main.UseEncoreColors == true && ManiacEditor.Methods.Solution.SolutionPaths.EncorePalette[0] != "") valid = ManiacEditor.Methods.Solution.SolutionPaths.GetStageTiles(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Solution.SolutionPaths.EncorePalette[0], ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.IsFullPath);
                    else valid = ManiacEditor.Methods.Solution.SolutionPaths.GetStageTiles(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone, null, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.IsFullPath);
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
                    bool valid = ManiacEditor.Methods.Solution.SolutionPaths.GetTileConfig(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.IsFullPath);
                    if (valid == false)
                    {
                        //throw new Exception("TileConfig.bin was unable to be found or could not be loaded properly!");
                    }
                }
                catch (Exception ex)
                {
                    //LoadFailed = true;
                    //LoadingFailed(ex);
                    //return;
                }

                #endregion

                #region Stage Config

                try
                {
                    bool valid = ManiacEditor.Methods.Solution.SolutionPaths.GetStageConfig(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.IsFullPath);
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
            Methods.Drawing.ObjectDrawing.RefreshRenderLists();
            SetupManiacINIPrefs();
            SetupDiscordRP(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.FilePath);
            var StageStamps = ManiacEditor.Methods.Solution.SolutionPaths.GetEditorStamps(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone);
            Instance.Chunks = new Classes.Scene.EditorChunks(StageStamps);
            Instance.EditBackground = new Classes.Scene.EditorBackground();
            SetupObjectsList();


            Methods.Internal.UserInterface.SplineControls.UpdateSplineSpawnObjectsList(Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects);
            Methods.Internal.UserInterface.Misc.UpdateStartScreen(false);
            Instance.EditorToolbar.SetupLayerButtons();
            Methods.Internal.UserInterface.UpdateControls();
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

            //Add Everything from the Game Config
            if (Methods.Solution.CurrentSolution.GameConfig != null)
            {
                for (int i = 0; i < Methods.Solution.CurrentSolution.GameConfig.ObjectsNames.Count; i++)
                {
                    Instance.ObjectList.Add(Methods.Solution.CurrentSolution.GameConfig.ObjectsNames[i]);
                    RSDKv5.Objects.AddObjectName(Methods.Solution.CurrentSolution.GameConfig.ObjectsNames[i]);
                }
            }

            //Add Everything from the Stage Config
            if (Methods.Solution.CurrentSolution.StageConfig != null)
            {
                for (int i = 0; i < Methods.Solution.CurrentSolution.StageConfig.ObjectsNames.Count; i++)
                {
                    Instance.ObjectList.Add(Methods.Solution.CurrentSolution.StageConfig.ObjectsNames[i]);
                    RSDKv5.Objects.AddObjectName(Methods.Solution.CurrentSolution.StageConfig.ObjectsNames[i]);
                }
            }

            //Refresh the Scene with the New Object/Attribute Names
            foreach (var sceneObj in Methods.Solution.CurrentSolution.CurrentScene.Objects)
            {
                if (RSDKv5.Objects.ObjectNames.ContainsKey(sceneObj.Name.Name)) sceneObj.Name.Name = RSDKv5.Objects.GetObjectName(sceneObj.Name);
                foreach (var objectAttribute in sceneObj.Attributes)
                {
                    if (RSDKv5.Objects.AttributeNames.ContainsKey(objectAttribute.Name.Name)) objectAttribute.Name.Name = RSDKv5.Objects.GetAttributeName(objectAttribute.Name);
                }
            }

            foreach (var sceneEntity in Methods.Solution.CurrentSolution.CurrentScene.Entities.Entities)
            {
                if (RSDKv5.Objects.ObjectNames.ContainsKey(sceneEntity.Object.Name.Name)) sceneEntity.Object.Name.Name = RSDKv5.Objects.GetObjectName(sceneEntity.Object.Name);
                foreach (var objectAttribute in sceneEntity.Object.Attributes)
                {
                    if (RSDKv5.Objects.AttributeNames.ContainsKey(objectAttribute.Name.Name)) objectAttribute.Name.Name = RSDKv5.Objects.GetAttributeName(objectAttribute.Name);
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
            if (File.Exists(ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourceDirectory + "\\maniac.json"))
            {
                Classes.Prefrences.SceneCurrentSettings.UpdateFilePath();
                Classes.Prefrences.SceneCurrentSettings.LoadFile();

                if (Classes.Prefrences.SceneCurrentSettings.ManiacINIData != null)
                {
                    if (Classes.Prefrences.SceneCurrentSettings.ManiacINIData.ObjectHashes != null)
                    {
                        foreach (var hash in Classes.Prefrences.SceneCurrentSettings.ManiacINIData.ObjectHashes)
                        {
                            RSDKv5.Objects.AddObjectName(hash);
                        }
                    }

                    if (Classes.Prefrences.SceneCurrentSettings.ManiacINIData.AttributeHashes != null)
                    {
                        foreach (var hash in Classes.Prefrences.SceneCurrentSettings.ManiacINIData.AttributeHashes)
                        {
                            RSDKv5.Objects.AddAttributeName(hash);
                        }
                    }
                }
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
                    Methods.Solution.CurrentSolution.CurrentScene.Save(SaveAsFilePath);
                    ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
                else
                {
                    if (Methods.Solution.SolutionState.Main.DataDirectoryReadOnlyMode && ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourceID == -1) return;
                    else Methods.Solution.CurrentSolution.CurrentScene.Save(ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourcePath);
                }

			}
			catch (Exception ex)
			{
                Methods.Internal.Common.ShowError($@"Failed to save the scene to file '{ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourcePath}' Error: {ex.Message}");
			}
		}
		public static void SaveStageConfig(bool saveAsMode = false, string SaveAsFilePath = "")
		{
			try
			{
                if (saveAsMode)
                {
                    Methods.Solution.CurrentSolution.StageConfig?.Write(SaveAsFilePath);
                    ManiacEditor.Methods.Solution.SolutionPaths.StageConfig_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
                else
                {
                    if (Methods.Solution.SolutionState.Main.DataDirectoryReadOnlyMode && ManiacEditor.Methods.Solution.SolutionPaths.StageConfig_Source.SourceID == -1) return;
                    else Methods.Solution.CurrentSolution.StageConfig?.Write(ManiacEditor.Methods.Solution.SolutionPaths.StageConfig_Source.SourcePath);
                }

			}
			catch (Exception ex)
			{
				Methods.Internal.Common.ShowError($@"Failed to save the StageConfig to file '{ManiacEditor.Methods.Solution.SolutionPaths.StageConfig_Source.SourcePath}' Error: {ex.Message}");
			}
		}
        public static void SaveChunks(bool SaveAsMode = false, string SaveAsFilePath = "")
        {
            Instance.Chunks?.Save(SaveAsMode, SaveAsFilePath);
        }
        public static void SaveTilesConfig(bool saveAsMode = false, string SaveAsFilePath = "")
        {
            try
            {
                if (saveAsMode)
                {
                    Methods.Solution.CurrentSolution.TileConfig?.Write(SaveAsFilePath);
                    ManiacEditor.Methods.Solution.SolutionPaths.TileConfig_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError($@"Failed to save the TileConfig to file '{ManiacEditor.Methods.Solution.SolutionPaths.TileConfig_Source.SourcePath}' Error: {ex.Message}");
            }
        }
        public static void Save16x16Tiles(bool saveAsMode = false, string SaveAsFilePath = "")
        {
            try
            {
                if (saveAsMode)
                {
                    Methods.Solution.CurrentSolution.CurrentTiles?.Write(SaveAsFilePath);
                    ManiacEditor.Methods.Solution.SolutionPaths.StageTiles_Source = new SolutionPaths.FileSource(-3, SaveAsFilePath);
                }
            }
            catch (Exception ex)
            {
                Methods.Internal.Common.ShowError($@"Failed to save the 16x16Tiles.gif to file '{ManiacEditor.Methods.Solution.SolutionPaths.StageTiles_Source.SourcePath}' Error: {ex.Message}");
            }
        }
        #endregion

        #region Backup Files

        public static void Backup(SolutionPaths.FileSource item)
        {
            try
            {
                int recursiveChecking = 0;
                string BackupPath = item.SourceDirectory + "\\" + System.IO.Path.GetFileName(item.SourcePath) + ".bak";
                while (System.IO.File.Exists(BackupPath))
                {
                    recursiveChecking++;
                    BackupPath = item.SourceDirectory + "\\" + System.IO.Path.GetFileName(item.SourcePath) + ".bak" + recursiveChecking.ToString();
                }

                System.IO.File.Copy(item.SourcePath, BackupPath);
                MessageBox.Show(string.Format("Backup of \"{0}\" made!{1}Output:{1}\"{2}\"", System.IO.Path.GetFileName(item.SourcePath), Environment.NewLine, BackupPath));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to make backup!{0}Reason:{0}{1}", Environment.NewLine, ex.Message));
            }

        }

        public static void BackupStageConfig()
        {
            Backup(ManiacEditor.Methods.Solution.SolutionPaths.StageConfig_Source);
        }

        public static void BackupStageTiles()
        {
            Backup(ManiacEditor.Methods.Solution.SolutionPaths.StageTiles_Source);
        }

        public static void BackupTileConfig()
        {
            Backup(ManiacEditor.Methods.Solution.SolutionPaths.TileConfig_Source);
        }

        public static void BackupSceneFile()
        {
            Backup(ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source);
        }

        #endregion

    }
}
