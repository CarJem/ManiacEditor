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
using ManiacEditor.Structures;
using System.Windows.Forms;
using Scene = RSDKv5.Scene;

namespace ManiacEditor.Methods.Solution
{
	public static class SolutionPaths
	{
		#region Source Variables
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
		public static FileSource InfinityConfig_Source { get; set; } = new FileSource();
		public static FileSource InfinityUnlocks_Source { get; set; } = new FileSource();
		public static FileSource GameConfig_Source { get; set; } = new FileSource();
		public static FileSource TileConfig_Source { get; set; } = new FileSource();
		public static FileSource StageConfig_Source { get; set; } = new FileSource();
		public static FileSource StageTiles_Source { get; set; } = new FileSource();
		public static FileSource SceneFile_Source { get; set; } = new FileSource();
		public static FileSource Stamps_Source { get; set; } = new FileSource();
		public static SceneState CurrentSceneData { get; set; } = new SceneState();
		public static string MasterDataDirectory
		{
			get
			{
				return Classes.Prefrences.CommonPathsStorage.Collection.DefaultMasterDataDirectory;
			}
			set
			{
				Classes.Prefrences.CommonPathsStorage.Collection.DefaultMasterDataDirectory = value;
				Classes.Prefrences.CommonPathsStorage.SaveFile();
			}
		}

		#endregion

		#region Misc Variables
		public static string[] EncorePalette { get; set; } = new string[6]; //Used to store the location of the encore palletes
		#endregion

		#region Infinity Unlocks
		public static bool SetInfinityUnlocks(string DataDirectory)
		{
			try
			{
				string ParentFolder = Directory.GetParent(DataDirectory).FullName;
				string path = Path.Combine(ParentFolder, "Unlocks.xml");
				Methods.Solution.CurrentSolution.InfinityUnlocks = new InfinityUnlocks(path);
				InfinityUnlocks_Source = new FileSource(path);
				return true;
			}
			catch
			{
				return false;
			}

		}
		public static bool SetInfinityUnlocks()
		{
			bool validDataDirectoryFound = false;
			string validDataDirectoryPath = "";
			foreach (string dataDir in ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories)
			{
				if (IsInfinityUnlocksValid(dataDir))
				{
					validDataDirectoryFound = true;
					validDataDirectoryPath = dataDir;
					break;
				}
			}
			if (!validDataDirectoryFound) return SetInfinityUnlocks(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory);
			else return SetInfinityUnlocks(validDataDirectoryPath);
		}
		public static InfinityUnlocks GetInfinityUnlocks(string DataDirectory)
		{
			try
			{
				string ParentFolder = Directory.GetParent(DataDirectory).FullName;
				string path = Path.Combine(ParentFolder, "Unlocks.xml");
				var InfinityUnlocks = new InfinityUnlocks(path);
				return InfinityUnlocks;
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("Something went Wrong!" + Environment.NewLine + ex.Message, "InfinityUnlocks Error!");
				return null;
			}

		}
		public static bool IsInfinityUnlocksValid(string directoryToCheck)
		{

			if (Directory.Exists(directoryToCheck) && Directory.GetParent(directoryToCheck) != null)
			{
				string ParentFolder = Directory.GetParent(directoryToCheck).FullName;
				return File.Exists(Path.Combine(ParentFolder, "Unlocks.xml"));
			}
			else
			{
				return false;
			}

		}

		#endregion

		#region Infinity Config
		public static bool SetInfinityConfig(string DataDirectory)
		{
			try
			{
				string ParentFolder = Directory.GetParent(DataDirectory).FullName;
				string path = Path.Combine(ParentFolder, "Stages.xml");
				Methods.Solution.CurrentSolution.InfinityConfig = new InfinityConfig(path);
				InfinityConfig_Source = new FileSource(path);
				return true;
			}
			catch
			{
				return false;
			}

		}
		public static bool SetInfinityConfig()
		{
			bool validDataDirectoryFound = false;
			string validDataDirectoryPath = "";
			foreach (string dataDir in ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories)
			{
				if (IsInfinityConfigValid(dataDir))
				{
					validDataDirectoryFound = true;
					validDataDirectoryPath = dataDir;
					break;
				}
			}
			if (!validDataDirectoryFound) return SetInfinityConfig(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory);
			else return SetInfinityConfig(validDataDirectoryPath);
		}
		public static InfinityConfig GetInfinityConfig(string DataDirectory)
		{
			try
			{
				string ParentFolder = Directory.GetParent(DataDirectory).FullName;
				string path = Path.Combine(ParentFolder, "Stages.xml");
				var InfinityConfig = new InfinityConfig(path);
				return InfinityConfig;
			}
			catch (Exception ex)
			{
				return null;
			}

		}
		public static bool IsInfinityConfigValid(string directoryToCheck)
		{

			if (Directory.Exists(directoryToCheck) && Directory.GetParent(directoryToCheck) != null)
            {
				string ParentFolder = Directory.GetParent(directoryToCheck).FullName;
				return File.Exists(Path.Combine(ParentFolder, "Stages.xml"));
			}
			else
            {
				return false;
            }

		}

		#endregion

		#region Game Config
		public static bool SetGameConfig(string DataDirectory)
		{
			try
			{
				Methods.Solution.CurrentSolution.GameConfig = new GameConfig(Path.Combine(DataDirectory, "Game", "GameConfig.bin"));
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
			foreach (string dataDir in ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories)
			{
				if (DoesDataDirHaveGameConfig(dataDir))
				{
					validDataDirectoryFound = true;
					validDataDirectoryPath = dataDir;
					break;
				}
			}
			if (!validDataDirectoryFound) return SetGameConfig(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory);
			else return SetGameConfig(validDataDirectoryPath);
		}
		public static GameConfig GetGameConfig(string DataDirectory)
		{
			try
			{
				string path = Path.Combine(DataDirectory, "Game", "GameConfig.bin");
				var GameConfig = new GameConfig(path);
				return GameConfig;
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("Something went Wrong!" + Environment.NewLine + ex.Message, "GameConfig Error!");
				return null;
			}

		}
		public static bool DoesDataDirHaveGameConfig(string directoryToCheck)
		{
			string path = Path.Combine(directoryToCheck, "Game", "GameConfig.bin");
			return File.Exists(path);
		}

		#endregion

		#region Tile Config
		public static bool GetTileConfig(string Zone, bool browsed = false)
		{
			bool ValidFileFound = false;
			int sourceID = -2;
			string ValidPathDir = "";


			if (browsed) return GetBrowsedTileConfig(ref ValidFileFound, ref ValidPathDir, ref sourceID, ref Zone);
			else return GetNormalTileConfig(ref ValidFileFound, ref ValidPathDir, ref sourceID, ref Zone);
		}

		public static bool GetBrowsedTileConfig(ref bool ValidFileFound, ref string ValidPathDir, ref int sourceID, ref string Zone)
		{
			bool result = SetTileConfigFromFilePath(SceneFile_Source.SourceDirectory);
			if (result == false) return GetNormalTileConfig(ref ValidFileFound, ref ValidPathDir, ref sourceID, ref Zone);

			sourceID = -3;
			return result;

		}
		public static bool GetNormalTileConfig(ref bool ValidFileFound, ref string ValidPathDir, ref int sourceID, ref string Zone)
		{
			GetExtraTileConfig(ref sourceID, ref ValidFileFound, ref ValidPathDir);
			if (ValidFileFound) return SetTileConfig(ValidPathDir, sourceID);
			else return GetMasterTileConfig(ref sourceID, ref Zone);
		}
		public static bool GetMasterTileConfig(ref int sourceID, ref string Zone)
		{
			sourceID = -1;
			return SetTileConfig(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, sourceID);
		}
		public static void GetExtraTileConfig(ref int sourceID, ref bool ValidFileFound, ref string ValidPathDir)
		{
			foreach (string dataDir in ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories)
			{
				if (IsTileConfigValid(dataDir))
				{
					sourceID = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories.IndexOf(dataDir);
					ValidFileFound = true;
					ValidPathDir = dataDir;
					break;
				}
			}
		}

		public static bool SetTileConfig(string configPath, int sourceID)
		{
			try
			{
				if (File.Exists(Path.Combine(configPath, "Stages", CurrentSceneData.Zone, "TileConfig.bin")))
				{
					Methods.Solution.CurrentSolution.TileConfig = new RSDKv5.Tileconfig(Path.Combine(configPath, "Stages", CurrentSceneData.Zone, "TileConfig.bin"));
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
				Methods.Solution.CurrentSolution.TileConfig = new RSDKv5.Tileconfig(Path.Combine(filepath, "TileConfig.bin"));
				TileConfig_Source = new FileSource(-3, Path.Combine(filepath, "TileConfig.bin"));
				return true;
			}
			catch (Events.TileConfigException ex)
			{
				throw ex;
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
			bool ValidFileFound = false;
			string ValidPathDir = "";

			if (browsed) return GetBrowsedStageTiles(ref ValidFileFound, ref ValidPathDir, ref colors, ref sourceID, ref Zone);
			else return GetNormalStageTiles(ref ValidFileFound, ref ValidPathDir, ref sourceID, ref Zone, ref colors);
		}
		public static bool GetNormalStageTiles(ref bool ValidFileFound, ref string ValidPathDir, ref int sourceID, ref string Zone, ref string colors)
		{
			GetExtraStageTiles(ref sourceID, ref ValidFileFound, ref ValidPathDir);
			if (ValidFileFound) return SetStageTiles(ValidPathDir, Zone, colors, sourceID);
			else return GetMasterStageTiles(ref sourceID, ref Zone, ref colors);
		}
		public static bool GetBrowsedStageTiles(ref bool ValidFileFound, ref string ValidPathDir, ref string colors, ref int sourceID, ref string Zone)
		{
			bool result = SetStageTilesFromFilePath(SceneFile_Source.SourceDirectory, colors);
			if (result == false) return GetNormalStageTiles(ref ValidFileFound, ref ValidPathDir, ref sourceID, ref Zone, ref colors);
			sourceID = -3;
			return result;

		}
		public static bool GetMasterStageTiles(ref int sourceID, ref string Zone, ref string colors)
		{
			sourceID = -1;
			return SetStageTiles(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, Zone, colors, sourceID);
		}
		public static void GetExtraStageTiles(ref int sourceID, ref bool ValidFileFound, ref string ValidPathDir)
		{
			foreach (string dataDir in ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories)
			{
				if (IsStageTilesValid(dataDir))
				{
					sourceID = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories.IndexOf(dataDir);
					ValidFileFound = true;
					ValidPathDir = dataDir;
					break;
				}
			}
		}


		public static bool SetStageTiles(string tilePath, string Zone, string colors, int sourceID)
		{
			try
			{
				Methods.Solution.CurrentSolution.CurrentTiles = new Classes.Scene.EditorTiles(Path.Combine(tilePath, "Stages", CurrentSceneData.Zone), colors);
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
				Methods.Solution.CurrentSolution.CurrentTiles = new Classes.Scene.EditorTiles(Path.Combine(filePath), colors);
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


		#endregion

		#region Scene File Location + Folder
		public static string SetScenePath(string dataFolderPath, int sourceId)
		{
			if (IsSceneValid(dataFolderPath))
			{
				SceneFile_Source = new FileSource(sourceId, Path.Combine(dataFolderPath, "Stages", CurrentSceneData.Zone, GetSceneFilename(CurrentSceneData.SceneID)));

				return SceneFile_Source.SourcePath;
			}
			else
			{
				if (File.Exists(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.FilePath))
				{
					SceneFile_Source = new FileSource(-3, ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.FilePath);
					CurrentSceneData.Zone = Path.GetFileName(CurrentSceneData.SceneDirectory);
					return ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.FilePath;
				}
				else
				{
					throw new Exception(string.Format("Can't Find any scene file related to the following:", ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.FilePath));
				}
			}

		}
		public static string GetScenePath()
		{
			bool ValidSceneFileFound = false;
			int sourceId = -2;
			string ValidSceneFilePathDir = "";
			foreach (string dataDir in ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories)
			{
				if (IsSceneValid(dataDir))
				{
					sourceId = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories.IndexOf(dataDir);
					ValidSceneFileFound = true;
					ValidSceneFilePathDir = dataDir;
					break;
				}
			}
			if (!ValidSceneFileFound)
			{
				sourceId = -1;
				return SetScenePath(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, sourceId);
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
				bool exists = File.Exists(path);
				return exists;
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
			bool ValidFileFound = false;
			int sourceID = -2;
			string ValidPathDir = "";


			if (browsed) return GetBrowsedStageConfig(ref ValidFileFound, ref ValidPathDir, ref sourceID, ref Zone);
			else return GetNormalStageConfig(ref ValidFileFound, ref ValidPathDir, ref sourceID, ref Zone);
		}

		public static bool GetNormalStageConfig(ref bool ValidFileFound, ref string ValidPathDir, ref int sourceID, ref string Zone)
		{
			GetExtraStageConfig(ref sourceID, ref ValidFileFound, ref ValidPathDir);
			if (ValidFileFound) return SetStageConfig(ValidPathDir, sourceID);
			else return GetMasterStageConfig(ref sourceID, ref Zone);
		}
		public static bool GetBrowsedStageConfig(ref bool ValidFileFound, ref string ValidPathDir, ref int sourceID, ref string Zone)
		{
			bool result = SetStageConfigFromFilePath(SceneFile_Source.SourceDirectory);
			if (result == false) return GetNormalStageConfig(ref ValidFileFound, ref ValidPathDir, ref sourceID, ref Zone);

			sourceID = -3;
			return result;

		}
		public static bool GetMasterStageConfig(ref int sourceID, ref string Zone)
		{
			sourceID = -1;
			return SetStageConfig(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory, sourceID);
		}
		public static void GetExtraStageConfig(ref int sourceID, ref bool ValidFileFound, ref string ValidPathDir)
		{
			foreach (string dataDir in ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories)
			{
				if (IsStageConfigValid(dataDir))
				{
					sourceID = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.ExtraDataDirectories.IndexOf(dataDir);
					ValidFileFound = true;
					ValidPathDir = dataDir;
					break;
				}
			}
		}

		public static bool SetStageConfig(string configPath, int sourceId)
		{
			try
			{
				Methods.Solution.CurrentSolution.StageConfig = new StageConfig(Path.Combine(configPath, "Stages", CurrentSceneData.Zone, "StageConfig.bin"));
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
				Methods.Solution.CurrentSolution.StageConfig = new StageConfig(Path.Combine(filepath, "StageConfig.bin"));
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
		public static Classes.Scene.EditorChunks.TexturedStamps GetEditorStamps(string Zone)
		{
			int sourceId = SceneFile_Source.SourceID;
			string sourceFile = Methods.Solution.CurrentSolution.CurrentScene.EditorMetadata.StampName.Replace("\0", "");
			if (sourceFile == string.Empty) sourceFile = "ManiacStamps.bin";
			string sourcePath = Path.Combine(SceneFile_Source.SourceDirectory, sourceFile);

			Stamps_Source = new FileSource(sourceId, sourcePath);
			if (IsEditorStampsValid(sourceFile))
			{
				return new Classes.Scene.EditorChunks.TexturedStamps(sourcePath);
			}
			else
			{
				//Check if default name exists (for backwards compatability)
				if (File.Exists(Path.Combine(SceneFile_Source.SourceDirectory, "ManiacStamps.bin")))
				{
					SetEditorStampsName("ManiacStamps.bin");
					return new Classes.Scene.EditorChunks.TexturedStamps(Path.Combine(SceneFile_Source.SourceDirectory, "ManiacStamps.bin"));
				}
				else
				{
					return new Classes.Scene.EditorChunks.TexturedStamps();
				}
			}
		}
		public static void SetEditorStampsName(string Name)
		{
			Methods.Solution.CurrentSolution.CurrentScene.EditorMetadata.StampName = Name;
		}
		public static bool IsEditorStampsValid(string sourceFile)
		{
			return File.Exists(Path.Combine(SceneFile_Source.SourceDirectory, sourceFile));
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
			InfinityConfig_Source = new FileSource();
		}
        #endregion
    }
}
