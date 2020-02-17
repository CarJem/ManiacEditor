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
using ManiacEditor.Classes.Internal;
using System.Windows.Forms;
using Scene = RSDKv5.Scene;

namespace ManiacEditor.Classes.Editor
{
	public static class SolutionPaths
	{
		#region Variables

		#region Sources
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

		#endregion

		#region Scene Select Information

		public static SceneState CurrentSceneData { get; set; } = new SceneState();
		#endregion

		#region Relative Information (To Data Folder Root)
		public static string RelativeSceneFile { get; set; } = "";
		public static string RealtiveSceneFolder { get; set; } = "";

		public static bool BaseFolderReadOnlyMode { get; set; } = false;
		public static bool ReadOnlyDirectory { get; set; } = false;
		public static bool ReadOnlySceneFile { get; set; } = false;

		#endregion

		#region Resource/Data Packs
		public static string LoadedDataPack { get; set; } = "";
		#endregion

		#region Data Directories


		public static string MasterDataDirectory { get; set; } = Environment.CurrentDirectory + "\\Data"; //Used as a way of allowing mods to not have to lug all the files in their folder just to load in Maniac.


		#endregion

		#region Misc Variables
		public static string[] EncorePalette { get; set; } = new string[6]; //Used to store the location of the encore palletes
		#endregion

		#endregion

		#region Methods

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
				return false;
			}

		}
		public static bool SetGameConfig()
		{
			bool validDataDirectoryFound = false;
			string validDataDirectoryPath = "";
			foreach (string dataDir in ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.ResourcePacks)
			{
				if (DoesDataDirHaveGameConfig(dataDir))
				{
					validDataDirectoryFound = true;
					validDataDirectoryPath = dataDir;
					break;
				}
			}
			if (!validDataDirectoryFound) return SetGameConfig(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory);
			else return SetGameConfig(validDataDirectoryPath);
		}

		public static Gameconfig GetGameConfig(string DataDirectory)
		{
			try
			{
				string path = Path.Combine(DataDirectory, "Game", "GameConfig.bin");
				var GameConfig = new Gameconfig(path);
				return GameConfig;
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("Something went Wrong!" + Environment.NewLine + ex.Message, "GameConfig Error!");
				return null;
			}

		}
		public static Gameconfig GetGameConfig(SceneState sceneState)
		{
			bool validDataDirectoryFound = false;
			string validDataDirectoryPath = "";
			foreach (string dataDir in sceneState.ResourcePacks)
			{
				if (DoesDataDirHaveGameConfig(dataDir))
				{
					validDataDirectoryFound = true;
					validDataDirectoryPath = dataDir;
					break;
				}
			}
			if (!validDataDirectoryFound) return GetGameConfig(sceneState.DataDirectory);
			else return GetGameConfig(validDataDirectoryPath);
		}


		#endregion

		#region Tile Config
		public static bool GetTileConfig(string Zone, bool browsed = false)
		{
			bool validTileConfigFound = false;
			int sourceID = -2;
			string validTileConfigPathDir = "";
			foreach (string dataDir in ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.ResourcePacks)
			{
				if (IsTileConfigValid(dataDir))
				{
					sourceID = ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.ResourcePacks.IndexOf(dataDir);
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
						return result = SetTileConfig(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, sourceID);
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
					return SetTileConfig(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, sourceID);
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
				if (File.Exists(Path.Combine(configPath, "Stages", CurrentSceneData.Zone, "TileConfig.bin")))
				{
					Classes.Editor.Solution.TileConfig = new RSDKv5.Tileconfig(Path.Combine(configPath, "Stages", CurrentSceneData.Zone, "TileConfig.bin"));
					TileConfig_Source = new FileSource(sourceID, Path.Combine(configPath, "Stages", CurrentSceneData.Zone, "TileConfig.bin"));
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
			return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentSceneData.Zone, "TileConfig.bin"));
		}
		#endregion

		#region Stage Tiles
		public static bool GetStageTiles(string Zone, string colors = null, bool browsed = false)
		{
			int sourceID = -2;
			bool validStageTilesFound = false;
			string validStageTilesPathDir = "";
			foreach (string dataDir in ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.ResourcePacks)
			{
				if (IsStageTilesValid(dataDir))
				{
					sourceID = ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.ResourcePacks.IndexOf(dataDir);
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
						return result = SetStageTiles(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, Zone, colors, sourceID);
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
					return SetStageTiles(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, Zone, colors, sourceID);
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
				Classes.Editor.Solution.CurrentTiles = new Scene.EditorTiles(Path.Combine(tilePath, "Stages", CurrentSceneData.Zone), colors);
				StageTiles_Source = new FileSource(sourceID, Path.Combine(tilePath, "Stages", CurrentSceneData.Zone));
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
			return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentSceneData.Zone, "16x16Tiles.gif"));
		}
		#endregion

		#region Data Directory

		public static string SelectDataDirectory()
		{
			using (var folderBrowserDialog = new GenerationsLib.Core.FolderSelectDialog())
			{
				folderBrowserDialog.Title = "Select Data Folder";

				if (!folderBrowserDialog.ShowDialog())
					return null;

				return folderBrowserDialog.FileName;
			}
		}
		public static bool DoesDataDirHaveGameConfig(string directoryToCheck)
		{
			string path = Path.Combine(directoryToCheck, "Game", "GameConfig.bin");
			return File.Exists(path);
		}

		#endregion

		#region Scene File Location + Folder
		public static string SetScenePath(string dataFolderPath, int sourceId)
		{
			if (IsSceneValid(dataFolderPath))
			{
				SceneFile_Source = new FileSource(sourceId, Path.Combine(dataFolderPath, "Stages", CurrentSceneData.Zone, GetSceneFilename(CurrentSceneData.SceneID)));
				SceneFile_DataFolder_Source = dataFolderPath;

				RelativeSceneFile = Path.Combine("Stages", CurrentSceneData.Zone, GetSceneFilename(CurrentSceneData.SceneID));
				RealtiveSceneFolder = Path.Combine("Stages", CurrentSceneData.Zone);

				return SceneFile_Source.SourcePath;
			}
			else
			{
				if (File.Exists(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.FilePath))
				{
					SceneFile_Source = new FileSource(-3, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.FilePath);
					CurrentSceneData.Zone = Path.GetFileName(CurrentSceneData.SceneDirectory);
					return ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.FilePath;
				}
				else
				{
					throw new Exception(string.Format("Can't Find any scene file related to the following:", ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.FilePath));
				}
			}

		}
		public static string GetScenePath()
		{
			bool ValidSceneFileFound = false;
			int sourceId = -2;
			string ValidSceneFilePathDir = "";
			foreach (string dataDir in ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.ResourcePacks)
			{
				if (IsSceneValid(dataDir))
				{
					sourceId = ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.ResourcePacks.IndexOf(dataDir);
					ValidSceneFileFound = true;
					ValidSceneFilePathDir = dataDir;
					break;
				}
			}
			if (!ValidSceneFileFound)
			{
				sourceId = -1;
				return SetScenePath(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, sourceId);
			}
			else
			{
				return SetScenePath(ValidSceneFilePathDir, sourceId);
			}
		}
		public static bool IsSceneValid(string directoryToCheck)
		{
			try
			{
				string path = Path.Combine(directoryToCheck, "Stages", CurrentSceneData.Zone, GetSceneFilename(CurrentSceneData.SceneID));
				return File.Exists(path);
			}
			catch
			{
				return false;
			}
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
			foreach (string dataDir in ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.ResourcePacks)
			{
				if (IsStageConfigValid(dataDir))
				{
					sourceId = ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.ResourcePacks.IndexOf(dataDir);
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
						return result = SetStageConfig(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, sourceId);
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
					return SetStageConfig(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, sourceId);
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
				Classes.Editor.Solution.StageConfig = new StageConfig(Path.Combine(configPath, "Stages", CurrentSceneData.Zone, "StageConfig.bin"));
				StageConfig_Source = new FileSource(sourceId, Path.Combine(configPath, "Stages", CurrentSceneData.Zone, "StageConfig.bin"));
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
			return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentSceneData.Zone, "StageConfig.bin"));
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

		#region Misc
		public static void UpdateInstance(ManiacEditor.Controls.Editor.MainEditor instance)
		{
			Instance = instance;
		}
		public static void UnloadScene()
		{
			GameConfig_Source = new FileSource();
			TileConfig_Source = new FileSource();
			StageConfig_Source = new FileSource();
			StageTiles_Source = new FileSource();
			SceneFile_Source = new FileSource();
			Stamps_Source = new FileSource();
			CurrentScene = "";
			RelativeSceneFile = "";
			RealtiveSceneFolder = "";
		}
        #endregion

        #endregion
    }
}
