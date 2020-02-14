using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RSDKv5;
using ManiacEditor.Actions;
using SharpDX.Direct3D9;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Scene = RSDKv5.Scene;

namespace ManiacEditor.Classes.Editor
{
    public static class Solution
    {
        public static Tileconfig TileConfig;
        public static Scene.EditorTiles CurrentTiles;
        public static Scene.EditorScene CurrentScene;
        public static Classes.Editor.Scene.EditorEntities Entities;
        public static StageConfig StageConfig;
        public static Gameconfig GameConfig;

        #region Layers
        public static Classes.Editor.Scene.Sets.EditorLayer FGHigher => CurrentScene?.HighDetails;
        public static Classes.Editor.Scene.Sets.EditorLayer FGHigh => CurrentScene?.ForegroundHigh;
        public static Classes.Editor.Scene.Sets.EditorLayer FGLow => CurrentScene?.ForegroundLow;
        public static Classes.Editor.Scene.Sets.EditorLayer FGLower => CurrentScene?.LowDetails;
        public static Classes.Editor.Scene.Sets.EditorLayer ScratchLayer => CurrentScene?.Scratch;
        public static Classes.Editor.Scene.Sets.EditorLayer EditLayerA { get; set; }
        public static Classes.Editor.Scene.Sets.EditorLayer EditLayerB { get; set; }
        #endregion

        #region Screen Size
        public static int SceneWidth => (CurrentScene != null ? CurrentScene.Layers.Max(sl => sl.Width) * 16 : 0);
        public static int SceneHeight => (CurrentScene != null ? CurrentScene.Layers.Max(sl => sl.Height) * 16 : 0);
		#endregion

		#region IO Paths
		public static class Paths
		{

			public class FileSource
			{
				//SourceID:
				// -3 - Browsed
				// -2 - Unset
				// -1 - Data Folder
				// 0* - Resource Pack Based on Index
				public int SourceID { get; set; } = -2;
				public string SourcePath { get; set; } = "";
				//Holds the Actual Path for the File
				public string SourceDirectory { get; set; } = "";
				//Holds the the Actual Path for the File's Directory

				public override string ToString()
				{
					return SourcePath;
				}

				public FileSource()
				{

				}

				public FileSource(string path)
				{
					SourcePath = path;
					SourceDirectory = System.IO.Path.GetDirectoryName(path);
				}

				public FileSource(int id, string path)
				{
					SourceID = id;
					SourcePath = path;
					SourceDirectory = System.IO.Path.GetDirectoryName(path);
				}
			}

			private static ManiacEditor.Controls.Editor.MainEditor Instance;
			public static FileSource GameConfig_Source { get; set; } = new FileSource();
			public static FileSource TileConfig_Source { get; set; } = new FileSource();
			public static FileSource StageConfig_Source { get; set; } = new FileSource();
			public static FileSource StageTiles_Source { get; set; } = new FileSource();
			public static FileSource SceneFile_Source { get; set; } = new FileSource();
			public static FileSource Stamps_Source { get; set; } = new FileSource();



			public static string CurrentScene { get; set; } = ""; //Tells us what Stage Folder is Loaded
			public static string SceneFile_DataFolder_Source { get; set; } = ""; //Tells us which Data Folder the Scene File is Located in

			//Scene Select Information
			public static string SceneFilePath { get; set; } = "";
			public static int CurrentLevelID { get; set; } = -1;
			public static bool isEncoreMode { get; set; } = false;
			public static string SceneDirectory { get; set; } = "";
			public static string CurrentZone { get; set; } = "";
			public static string CurrentName { get; set; } = "";
			public static string CurrentSceneID { get; set; } = "";
			public static bool Browsed { get; set; } = false;

			//Relative Information (To Data Folder Root)
			public static string RelativeSceneFile { get; set; } = "";
			public static string RealtiveSceneFolder { get; set; } = "";

			public static bool BaseFolderReadOnlyMode { get; set; } = false;
			public static bool ReadOnlyDirectory { get; set; } = false;
			public static bool ReadOnlySceneFile { get; set; } = false;

			public static void UpdateInstance(ManiacEditor.Controls.Editor.MainEditor instance)
			{
				Instance = instance;
			}

			#region Game Config
			public static bool SetGameConfig(string DataDirectory)
			{
				try
				{
					Classes.Editor.Solution.GameConfig = new Gameconfig(Path.Combine(DataDirectory, "Game", "GameConfig.bin"));
					GameConfig_Source = new FileSource(Path.Combine(DataDirectory, "Game", "GameConfig.bin"));
					return true;
				}
				catch
				{
					// Allow the User to be able to have a Maniac Editor Dedicated GameConfig, see if the user has made one
					try
					{
						Classes.Editor.Solution.GameConfig = new Gameconfig(Path.Combine(DataDirectory, "Game", "GameConfig_ME.bin"));
						GameConfig_Source = new FileSource(Path.Combine(DataDirectory, "Game", "GameConfig_ME.bin"));
						return true;
					}
					catch
					{
						System.Windows.Forms.MessageBox.Show("Something is wrong with this GameConfig that we can't support! If for some reason it does work for you in Sonic Mania, you can create another GameConfig.bin called GameConfig_ME.bin and the editor should load that instead (assuming it's a clean GameConfig or one that works) allowing you to still be able to use the data folder, however, this is experimental so be careful when doing that.", "GameConfig Error!");
						return false;
					}


				}

			}
			public static Gameconfig SetandReturnGameConfig(string DataDirectory)
			{
				try
				{
					var GameConfig = new Gameconfig(Path.Combine(DataDirectory, "Game", "GameConfig.bin"));
					return GameConfig;
				}
				catch
				{
					// Allow the User to be able to have a Maniac Editor Dedicated GameConfig, see if the user has made one
					try
					{
						var GameConfig = new Gameconfig(Path.Combine(DataDirectory, "Game", "GameConfig_ME.bin"));
						return GameConfig;
					}
					catch
					{
						System.Windows.MessageBox.Show("Something is wrong with this GameConfig that we can't support! If for some reason it does work for you in Sonic Mania, you can create another GameConfig.bin called GameConfig_ME.bin and the editor should load that instead (assuming it's a clean GameConfig or one that works) allowing you to still be able to use the data folder, however, this is experimental so be careful when doing that.", "GameConfig Error!");
						return null;
					}


				}

			}
			public static bool SetGameConfig()
			{
				bool validDataDirectoryFound = false;
				string validDataDirectoryPath = "";
				foreach (string dataDir in Instance.ResourcePackList)
				{
					if (IsDataDirectoryValid(dataDir))
					{
						validDataDirectoryFound = true;
						validDataDirectoryPath = dataDir;
						break;
					}
				}
				if (!validDataDirectoryFound)
				{
					return SetGameConfig(Instance.DataDirectory);
				}
				else
				{
					return SetGameConfig(validDataDirectoryPath);
				}
			}
			public static Gameconfig SetandReturnGameConfig()
			{
				bool validDataDirectoryFound = false;
				string validDataDirectoryPath = "";
				foreach (string dataDir in Instance.ResourcePackList)
				{
					if (IsDataDirectoryValid(dataDir))
					{
						validDataDirectoryFound = true;
						validDataDirectoryPath = dataDir;
						break;
					}
				}
				if (!validDataDirectoryFound)
				{
					return SetandReturnGameConfig(Instance.DataDirectory);
				}
				else
				{
					return SetandReturnGameConfig(validDataDirectoryPath);
				}
			}
			public static bool IsDataDirectoryValid(string directoryToCheck)
			{
				return File.Exists(Path.Combine(directoryToCheck, "Game", "GameConfig.bin"));
			}
			#endregion

			#region Tile Config
			public static bool GetTileConfig(string Zone, bool browsed = false)
			{
				bool validTileConfigFound = false;
				int sourceID = -2;
				string validTileConfigPathDir = "";
				foreach (string dataDir in Instance.ResourcePackList)
				{
					if (IsTileConfigValid(dataDir))
					{
						sourceID = Instance.ResourcePackList.IndexOf(dataDir);
						validTileConfigFound = true;
						validTileConfigPathDir = dataDir;
						break;
					}
				}
				if (!validTileConfigFound)
				{
					if (browsed)
					{
						bool result = SetTileConfigFromFilePath(SceneFile_Source.SourceDirectory);
						if (result == false)
						{
							sourceID = -1;
							return result = SetTileConfig(Instance.DataDirectory, sourceID);
						}
						else
						{
							sourceID = -3;
						}
						return result;
					}
					else
					{
						sourceID = -1;
						return SetTileConfig(Instance.DataDirectory, sourceID);
					}
				}
				else
				{
					return SetTileConfig(validTileConfigPathDir, sourceID);
				}
			}
			public static bool SetTileConfig(string configPath, int sourceID)
			{
				try
				{
					if (File.Exists(Path.Combine(configPath, "Stages", CurrentZone, "TileConfig.bin")))
					{
						Classes.Editor.Solution.TileConfig = new RSDKv5.Tileconfig(Path.Combine(configPath, "Stages", CurrentZone, "TileConfig.bin"));
						TileConfig_Source = new FileSource(sourceID, Path.Combine(configPath, "Stages", CurrentZone, "TileConfig.bin"));
						return true;
					}
					else
					{
						return false;
					}

				}
				catch
				{
					return false;
				}

			}
			public static bool SetTileConfigFromFilePath(string filepath)
			{
				try
				{
					Classes.Editor.Solution.TileConfig = new RSDKv5.Tileconfig(Path.Combine(filepath, "TileConfig.bin"));
					TileConfig_Source = new FileSource(-3, Path.Combine(filepath, "TileConfig.bin"));
					return true;
				}
				catch
				{
					return false;
				}

			}
			public static bool IsTileConfigValid(string directoryToCheck)
			{
				return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentZone, "TileConfig.bin"));
			}
			#endregion

			#region Stage Tiles
			public static bool GetStageTiles(string Zone, string colors = null, bool browsed = false)
			{
				int sourceID = -2;
				bool validStageTilesFound = false;
				string validStageTilesPathDir = "";
				foreach (string dataDir in Instance.ResourcePackList)
				{
					if (IsStageTilesValid(dataDir))
					{
						sourceID = Instance.ResourcePackList.IndexOf(dataDir);
						validStageTilesFound = true;
						validStageTilesPathDir = dataDir;
						break;
					}
				}
				if (!validStageTilesFound)
				{

					if (browsed)
					{
						bool result = SetStageTilesFromFilePath(SceneFile_Source.SourceDirectory, colors);
						if (result == false)
						{
							sourceID = -1;
							return result = SetStageTiles(Instance.DataDirectory, Zone, colors, sourceID);
						}
						else
						{
							sourceID = -3;
							return result;
						}

					}
					else
					{
						sourceID = -1;
						return SetStageTiles(Instance.DataDirectory, Zone, colors, sourceID);
					}
				}
				else
				{
					return SetStageTiles(validStageTilesPathDir, Zone, colors, sourceID);
				}
			}
			public static bool SetStageTiles(string tilePath, string Zone, string colors, int sourceID)
			{
				try
				{
					Classes.Editor.Solution.CurrentTiles = new Scene.EditorTiles(Path.Combine(tilePath, "Stages", CurrentZone), colors);
					StageTiles_Source = new FileSource(sourceID, Path.Combine(tilePath, "Stages", CurrentZone));
					return true;
				}
				catch
				{
					return false;
				}

			}
			public static bool SetStageTilesFromFilePath(string filePath, string colors = null)
			{
				try
				{
					Classes.Editor.Solution.CurrentTiles = new Scene.EditorTiles(Path.Combine(filePath), colors);
					StageTiles_Source = new FileSource(-3, Path.Combine(filePath));
					return true;
				}
				catch
				{
					return false;
				}

			}
			public static bool IsStageTilesValid(string directoryToCheck)
			{
				return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentZone, "16x16Tiles.gif"));
			}
			#endregion

			#region Data Directory
			public static string GetDataDirectory()
			{
				bool validDataDirectoryFound = false;
				string validDataDirectoryPath = "";
				foreach (string dataDir in Instance.ResourcePackList)
				{
					if (IsDataDirectoryValid(dataDir))
					{
						validDataDirectoryFound = true;
						validDataDirectoryPath = dataDir;
						break;
					}
				}
				if (!validDataDirectoryFound)
				{
					return Instance.DataDirectory;
				}
				else
				{
					return validDataDirectoryPath;
				}
			}

			#endregion

			#region Scene File Location + Folder
			public static string SetScenePath(string dataFolderPath, int sourceId)
			{
				SceneFile_Source = new FileSource(sourceId, Path.Combine(dataFolderPath, "Stages", CurrentZone, GetSceneFilename(CurrentSceneID)));
				SceneFile_DataFolder_Source = dataFolderPath;

				RelativeSceneFile = Path.Combine("Stages", CurrentZone, GetSceneFilename(CurrentSceneID));
				RealtiveSceneFolder = Path.Combine("Stages", CurrentZone);

				return SceneFile_Source.SourcePath;
			}

			public static string GetScenePath()
			{
				bool ValidSceneFileFound = false;
				int sourceId = -2;
				string ValidSceneFilePathDir = "";
				foreach (string dataDir in Instance.ResourcePackList)
				{
					if (IsSceneValid(dataDir))
					{
						sourceId = Instance.ResourcePackList.IndexOf(dataDir);
						ValidSceneFileFound = true;
						ValidSceneFilePathDir = dataDir;
						break;
					}
				}
				if (!ValidSceneFileFound)
				{
					sourceId = -1;
					return SetScenePath(Instance.DataDirectory, sourceId);
				}
				else
				{
					return SetScenePath(ValidSceneFilePathDir, sourceId);
				}
			}

			public static bool IsSceneValid(string directoryToCheck)
			{
				return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentZone, GetSceneFilename(CurrentSceneID)));
			}

			public static string GetScenePathFromFile(string Result)
			{
				SceneFile_Source = new FileSource(-3, Result);
				CurrentZone = Path.GetFileName(SceneDirectory);
				return Result;
			}

			public static string GetSceneFilename(string SceneID)
			{
				return "Scene" + SceneID + ".bin";
			}

			#endregion

			#region Stage Config
			public static bool GetStageConfig(string Zone, bool browsed = false)
			{
				bool validStageConfigFound = false;
				int sourceId = -2;
				string validStageConfigPathDir = "";
				foreach (string dataDir in Instance.ResourcePackList)
				{
					if (IsStageConfigValid(dataDir))
					{
						sourceId = Instance.ResourcePackList.IndexOf(dataDir);
						validStageConfigFound = true;
						validStageConfigPathDir = dataDir;
						break;
					}
				}
				if (!validStageConfigFound)
				{
					if (browsed)
					{
						bool result = SetStageConfigFromFilePath(SceneFile_Source.SourceDirectory);
						if (result == false)
						{
							sourceId = -1;
							return result = SetStageConfig(Instance.DataDirectory, sourceId);
						}
						else
						{
							sourceId = -3;
							return result;
						}

					}
					else
					{
						sourceId = -1;
						return SetStageConfig(Instance.DataDirectory, sourceId);
					}

				}
				else
				{
					return SetStageConfig(validStageConfigPathDir, sourceId);
				}
			}
			public static bool SetStageConfig(string configPath, int sourceId)
			{
				try
				{
					Classes.Editor.Solution.StageConfig = new StageConfig(Path.Combine(configPath, "Stages", CurrentZone, "StageConfig.bin"));
					StageConfig_Source = new FileSource(sourceId, Path.Combine(configPath, "Stages", CurrentZone, "StageConfig.bin"));
					return true;
				}
				catch
				{

					return false;
				}

			}
			public static bool SetStageConfigFromFilePath(string filepath)
			{
				try
				{
					Classes.Editor.Solution.StageConfig = new StageConfig(Path.Combine(filepath, "StageConfig.bin"));
					StageConfig_Source = new FileSource(-3, Path.Combine(filepath, "StageConfig.bin"));
					return true;
				}
				catch
				{

					return false;
				}

			}
			public static bool IsStageConfigValid(string directoryToCheck)
			{
				return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentZone, "StageConfig.bin"));
			}
			#endregion

			#region Editor Stamps
			public static Stamps GetEditorStamps(string Zone)
			{
				int sourceId = SceneFile_Source.SourceID;
				Stamps_Source = new FileSource(sourceId, Path.Combine(SceneFile_Source.SourceDirectory, Classes.Editor.Solution.CurrentScene.EditorMetadata.StampName.Replace("\0", "")));
				if (IsEditorStampsValid())
				{
					return new Stamps(Path.Combine(SceneFile_Source.SourceDirectory, Classes.Editor.Solution.CurrentScene.EditorMetadata.StampName.Replace("\0", "")));
				}
				else
				{
					//Check if default name exists (for backwards compatability)
					if (File.Exists(Path.Combine(SceneFile_Source.SourceDirectory, "ManiacStamps.bin")))
					{
						SetEditorStampsName("ManiacStamps.bin");
						return new Stamps(Path.Combine(SceneFile_Source.SourceDirectory, "ManiacStamps.bin"));
					}
					else
					{
						return new Stamps();
					}
				}
			}
			public static void SetEditorStampsName(string Name)
			{
				Classes.Editor.Solution.CurrentScene.EditorMetadata.StampName = Name;
			}
			public static bool IsEditorStampsValid()
			{
				return File.Exists(Path.Combine(SceneFile_Source.SourceDirectory, Classes.Editor.Solution.CurrentScene.EditorMetadata.StampName.Replace("\0", "")));
			}
			#endregion

			public static void UnloadScene()
			{
				GameConfig_Source = new FileSource();
				TileConfig_Source = new FileSource();
				StageConfig_Source = new FileSource();
				StageTiles_Source = new FileSource();
				SceneFile_Source = new FileSource();
				Stamps_Source = new FileSource();
				CurrentScene = "";
				SceneFilePath = "";
				CurrentLevelID = -1;
				isEncoreMode = false;
				SceneDirectory = "";
				CurrentZone = "";
				CurrentName = "";
				CurrentSceneID = "";
				Browsed = false;
				RelativeSceneFile = "";
				RealtiveSceneFolder = "";
			}

		}
        #endregion

        #region Other Methods
        public static void UnloadScene()
        {
            Classes.Editor.Solution.CurrentScene?.Dispose();
            Classes.Editor.Solution.CurrentScene = null;
            Classes.Editor.Solution.StageConfig = null;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorStatusBar._levelIDLabel.Content = "Level ID: NULL";
            Classes.Editor.SolutionState.LevelID = -1;
            Classes.Editor.SolutionState.EncorePaletteExists = false;
            Classes.Editor.SolutionState.EncoreSetupType = 0;
            Methods.Prefrences.SceneCurrentSettings.ClearSettings();
            ManiacEditor.Controls.Editor.MainEditor.Instance.userDefinedEntityRenderSwaps = new Dictionary<string, string>();
            ManiacEditor.Controls.Editor.MainEditor.Instance.userDefinedSpritePaths = new List<string>();
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EncorePaletteButton.IsChecked = false;
			Paths.UnloadScene();
            Classes.Editor.SolutionState.QuitWithoutSavingWarningRequired = false;

            if (Classes.Editor.Solution.CurrentTiles != null) Classes.Editor.Solution.CurrentTiles.Dispose();
            Classes.Editor.Solution.CurrentTiles = null;

            ManiacEditor.Controls.Editor.MainEditor.Instance.TearDownExtraLayerButtons();

            ManiacEditor.Controls.Editor.MainEditor.Instance.Background = null;

            ManiacEditor.Controls.Editor.MainEditor.Instance.Chunks = null;

            Methods.Entities.EntityAnimator.AnimationTiming.Clear();


            /*if (entitiesClipboard != null)
            {
                foreach (Classes.Edit.Scene.Sets.EditorEntity entity in entitiesClipboard)
                    entity.PrepareForExternalCopy();
            }*/


            // Clear local clipboards
            //TilesClipboard = null;
            ManiacEditor.Controls.Editor.MainEditor.Instance.entitiesClipboard = null;

            Classes.Editor.Solution.Entities = null;

            Classes.Editor.SolutionState.Zoom = 1;
            Classes.Editor.SolutionState.ZoomLevel = 0;

            ManiacEditor.Controls.Editor.MainEditor.Instance.UndoStack.Clear();
            ManiacEditor.Controls.Editor.MainEditor.Instance.RedoStack.Clear();

            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGLow.ClearCheckedItems();
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGHigh.ClearCheckedItems();
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGLower.ClearCheckedItems();
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGHigher.ClearCheckedItems();
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditEntities.ClearCheckedItems();

            ManiacEditor.Controls.Editor.MainEditor.Instance.ViewPanel.SharpPanel.ResizeGraphicsPanel();

            Methods.Internal.UserInterface.UpdateControls();

            // clear memory a little more aggressively 
            ManiacEditor.Controls.Editor.MainEditor.Instance.EntityDrawing.ReleaseResources();
            GC.Collect();
            Classes.Editor.Solution.TileConfig = null;

            ManiacEditor.Controls.Editor.MainEditor.Instance.UpdateStartScreen(true);
        }
        #endregion
    }
}
