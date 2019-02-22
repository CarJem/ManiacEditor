using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSDKv5;
using System.IO;

namespace ManiacEditor
{
	public class EditorPath
	{
		private Editor Instance;
		public string GameConfig_Source = ""; //Holds the Path the Actual Path for the GameConfig
		public string TileConfig_Source = ""; //Holds the Path the Actual Path for the TileConfig
		public string StageConfig_Source = ""; //Holds the Path the Actual Path for the GameConfig
		public string StageTiles_Source = ""; //Holds the Path the Actual Path for the 16x16Tiles.gif
		public string SceneFile_Source = ""; //Hold the Path Starting From the Root Data Folder to the Scene File
		public string Stamps_Source = ""; //Hold the Path to the Root EditorStamps.bin
		public string SceneFile_Directory = ""; //Hold the Path Starting From the Root Data Folder to the Scene Folder
		public string CurrentScene = ""; //Tells us what Stage Folder is Loaded
		public string SceneFile_DataFolder_Source = ""; //Tells us which Data Folder the Scene File is Located in

		//Scene Select Information
		public string SceneFilePath = "";
		public int CurrentLevelID = -1;
		public bool isEncoreMode = false;
		public string SceneDirectory = "";
		public string CurrentZone { get; set; }
		public string CurrentName = "";
		public string CurrentSceneID = "";
		public bool Browsed = false;

		//Relative Information (To Data Folder Root)
		public string RelativeSceneFile = "";
		public string RealtiveSceneFolder = "";

		public EditorPath(Editor instance)
		{
			Instance = instance;
		}

		#region GameConfig
		public bool SetGameConfig(string DataDirectory)
		{
			try
			{
				Instance.GameConfig = new GameConfig(Path.Combine(DataDirectory, "Game", "GameConfig.bin"));
				GameConfig_Source = Path.Combine(DataDirectory, "Game", "GameConfig.bin");
				return true;
			}
			catch
			{
				// Allow the User to be able to have a Maniac Editor Dedicated GameConfig, see if the user has made one
				try
				{
					Instance.GameConfig = new GameConfig(Path.Combine(DataDirectory, "Game", "GameConfig_ME.bin"));
					GameConfig_Source = Path.Combine(DataDirectory, "Game", "GameConfig_ME.bin");
					return true;
				}
				catch
				{
					System.Windows.MessageBox.Show("Something is wrong with this GameConfig that we can't support! If for some reason it does work for you in Sonic Mania, you can create another GameConfig.bin called GameConfig_ME.bin and the editor should load that instead (assuming it's a clean GameConfig or one that works) allowing you to still be able to use the data folder, however, this is experimental so be careful when doing that.", "GameConfig Error!");
					return false;
				}


			}

		}

		public bool SetGameConfig()
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

		public bool IsDataDirectoryValid(string directoryToCheck)
		{
			return File.Exists(Path.Combine(directoryToCheck, "Game", "GameConfig.bin"));
		}
		#endregion

		#region TileConfig
		public bool GetTileConfig(string Zone)
		{
			bool validTileConfigFound = false;
			string validTileConfigPathDir = "";
			foreach (string dataDir in Instance.ResourcePackList)
			{
				if (IsTileConfigValid(dataDir))
				{
					validTileConfigFound = true;
					validTileConfigPathDir = dataDir;
					break;
				}
			}
			if (!validTileConfigFound)
			{
				return SetTileConfig(Instance.DataDirectory);
			}
			else
			{
				return SetTileConfig(validTileConfigPathDir);
			}
		}

		public bool SetTileConfig(string configPath)
		{
			try
			{
				Instance.TilesConfig = new TileConfig(Path.Combine(configPath, "Stages", CurrentZone, "TileConfig.bin"));
				TileConfig_Source = Path.Combine(configPath, "Stages", CurrentZone, "TileConfig.bin");
				return true;
			}
			catch
			{
				return false;
			}

		}

		public bool IsTileConfigValid(string directoryToCheck)
		{
			return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentZone, "TileConfig.bin"));
		}
		#endregion

		#region StageTiles
		public bool GetStageTiles(string Zone, string colors = null)
		{
			bool validStageTilesFound = false;
			string validStageTilesPathDir = "";
			foreach (string dataDir in Instance.ResourcePackList)
			{
				if (IsStageTilesValid(dataDir))
				{
					validStageTilesFound = true;
					validStageTilesPathDir = dataDir;
					break;
				}
			}
			if (!validStageTilesFound)
			{
				return SetStageTiles(Instance.DataDirectory, Zone, colors);
			}
			else
			{
				return SetStageTiles(validStageTilesPathDir, Zone, colors);
			}
		}

		public bool SetStageTiles(string tilePath, string Zone, string colors = null)
		{
			try
			{
				Instance.StageTiles = new StageTiles(Path.Combine(tilePath, "Stages", CurrentZone), colors);
				StageTiles_Source = Path.Combine(tilePath, "Stages", CurrentZone);
				return true;
			}
			catch
			{
				return false;
			}

		}

		public bool IsStageTilesValid(string directoryToCheck)
		{
			return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentZone, "16x16Tiles.gif"));
		}
		#endregion

		#region Data Directory
		public string GetDataDirectory()
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
		public string SetScenePath(string dataFolderPath)
		{
			SceneFile_Source = Path.Combine(dataFolderPath, "Stages", CurrentZone, GetSceneFilename(CurrentSceneID));
			SceneFile_Directory = Path.GetDirectoryName(SceneFile_Source);
			SceneFile_DataFolder_Source = dataFolderPath;

			RelativeSceneFile = Path.Combine("Stages", CurrentZone, GetSceneFilename(CurrentSceneID));
			RealtiveSceneFolder = Path.Combine("Stages", CurrentZone);

			return SceneFile_Source;
		}

		public string GetScenePath()
		{
			bool ValidSceneFileFound = false;
			string ValidSceneFilePathDir = "";
			foreach (string dataDir in Instance.ResourcePackList)
			{
				if (IsSceneValid(dataDir))
				{
					ValidSceneFileFound = true;
					ValidSceneFilePathDir = dataDir;
					break;
				}
			}
			if (!ValidSceneFileFound)
			{
				return SetScenePath(Instance.DataDirectory);
			}
			else
			{
				return SetScenePath(ValidSceneFilePathDir);
			}
		}

		public bool IsSceneValid(string directoryToCheck)
		{
			return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentZone, "StageConfig.bin"));
		}

		public string GetScenePathFromFile(string Result)
		{
			SceneFile_Source = Result;
			SceneFile_Directory = Path.GetDirectoryName(Result);
			CurrentZone = Path.GetFileName(SceneDirectory);
			return Result;
		}

		public string GetSceneFilename(string SceneID)
		{
			return "Scene" + SceneID + ".bin";
		}

		#endregion

		#region StageConfig
		public bool GetStageConfig(string Zone)
		{
			bool validStageConfigFound = false;
			string validStageConfigPathDir = "";
			foreach (string dataDir in Instance.ResourcePackList)
			{
				if (IsStageConfigValid(dataDir))
				{
					validStageConfigFound = true;
					validStageConfigPathDir = dataDir;
					break;
				}
			}
			if (!validStageConfigFound)
			{
				return SetStageConfig(Instance.DataDirectory);
			}
			else
			{
				return SetStageConfig(validStageConfigPathDir);
			}
		}

		public bool SetStageConfig(string configPath)
		{
			try
			{
				Instance.StageConfig = new StageConfig(Path.Combine(configPath, "Stages", CurrentZone, "StageConfig.bin"));
				StageConfig_Source = Path.Combine(configPath, "Stages", CurrentZone, "StageConfig.bin");
				return true;
			}
			catch
			{
				return false;
			}

		}

		public bool IsStageConfigValid(string directoryToCheck)
		{
			return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentZone, "StageConfig.bin"));
		}
		#endregion

		#region Editor Stamps
		public Stamps GetEditorStamps(string Zone)
		{
			bool validEditorStampsFound = false;
			string validEditorStampsPathDir = "";
			foreach (string dataDir in Instance.ResourcePackList)
			{
				if (IsEditorStampsValid(dataDir))
				{
					validEditorStampsFound = true;
					validEditorStampsPathDir = dataDir;
					break;
				}
			}
			if (!validEditorStampsFound)
			{
				return SetEditorStamps(Instance.DataDirectory);
			}
			else
			{
				return SetEditorStamps(validEditorStampsPathDir);
			}
		}

		public Stamps SetEditorStamps(string configPath)
		{
			try
			{
				Stamps StageStamps = new Stamps(Path.Combine(configPath, "Stages", CurrentZone, "ManiacStamps.bin"));
				Stamps_Source = Path.Combine(configPath, "Stages", CurrentZone, "ManiacStamps.bin");
				return StageStamps;
			}
			catch
			{
				return null;
			}

		}

		public bool IsEditorStampsValid(string directoryToCheck)
		{
			return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentZone, "ManiacStamps.bin"));
		}
		#endregion

	public void UnloadScene()
	{
		GameConfig_Source = "";
		TileConfig_Source = ""; 
		StageConfig_Source = "";
		StageTiles_Source = "";
		SceneFile_Source = "";
		Stamps_Source = "";
		SceneFile_Directory = "";
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
}
