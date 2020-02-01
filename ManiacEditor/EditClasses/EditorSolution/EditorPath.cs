using EditClasses.Solution;
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
		public string SceneFile_Source = ""; //Holds the Path Starting From the Root Data Folder to the Scene File
		public string Stamps_Source = ""; //Holds the Path to the Root EditorStamps.bin


        //SourceID:
        // -3 - Browsed
        // -2 - Unset
        // -1 - Data Folder
        // 0* - Resource Pack Based on Index
        public int GameConfig_SourceID = -2; //Holds the Path the Actual Path for the GameConfig
        public int TileConfig_SourceID = -2; //Holds the Path the Actual Path for the TileConfig
        public int StageConfig_SourceID = -2; //Holds the Path the Actual Path for the GameConfig
        public int StageTiles_SourceID = -2; //Holds the Path the Actual Path for the 16x16Tiles.gif
        public int SceneFile_SourceID = -2; //Hold the Path Starting From the Root Data Folder to the Scene File
        public int Stamps_SourceID = -2; //Hold the Path to the Root EditorStamps.bin



        public string SceneFile_Directory = ""; //Hold the Path Starting From the Root Data Folder to the Scene Folder
		public string CurrentScene = ""; //Tells us what Stage Folder is Loaded
		public string SceneFile_DataFolder_Source = ""; //Tells us which Data Folder the Scene File is Located in

		//Scene Select Information
		public string SceneFilePath = "";
		public int CurrentLevelID = -1;
		public bool isEncoreMode = false;
		public string SceneDirectory = "";
		public string CurrentZone = "";
		public string CurrentName = "";
		public string CurrentSceneID = "";
		public bool Browsed = false;

		//Relative Information (To Data Folder Root)
		public string RelativeSceneFile = "";
		public string RealtiveSceneFolder = "";

        public bool BaseFolderReadOnlyMode = false;
        public bool ReadOnlyDirectory = false;
        public bool ReadOnlySceneFile = false;

		public EditorPath(Editor instance)
		{
			Instance = instance;
		}

		#region GameConfig
		public bool SetGameConfig(string DataDirectory)
		{
			try
			{
				CurrentSolution.GameConfig = new Gameconfig(Path.Combine(DataDirectory, "Game", "GameConfig.bin"));
				GameConfig_Source = Path.Combine(DataDirectory, "Game", "GameConfig.bin");
				return true;
			}
			catch
			{
				// Allow the User to be able to have a Maniac Editor Dedicated GameConfig, see if the user has made one
				try
				{
                    CurrentSolution.GameConfig = new Gameconfig(Path.Combine(DataDirectory, "Game", "GameConfig_ME.bin"));
					GameConfig_Source = Path.Combine(DataDirectory, "Game", "GameConfig_ME.bin");
					return true;
				}
				catch
				{
					System.Windows.Forms.MessageBox.Show("Something is wrong with this GameConfig that we can't support! If for some reason it does work for you in Sonic Mania, you can create another GameConfig.bin called GameConfig_ME.bin and the editor should load that instead (assuming it's a clean GameConfig or one that works) allowing you to still be able to use the data folder, however, this is experimental so be careful when doing that.", "GameConfig Error!");
					return false;
				}


			}

		}

        public Gameconfig SetandReturnGameConfig(string DataDirectory)
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

        public Gameconfig SetandReturnGameConfig()
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

        public bool IsDataDirectoryValid(string directoryToCheck)
		{
			return File.Exists(Path.Combine(directoryToCheck, "Game", "GameConfig.bin"));
		}
		#endregion

		#region TileConfig
		public bool GetTileConfig(string Zone, bool browsed = false)
		{
			bool validTileConfigFound = false;
			string validTileConfigPathDir = "";
			foreach (string dataDir in Instance.ResourcePackList)
			{
				if (IsTileConfigValid(dataDir))
				{
                    TileConfig_SourceID = Instance.ResourcePackList.IndexOf(dataDir);
					validTileConfigFound = true;
					validTileConfigPathDir = dataDir;
					break;
				}
			}
			if (!validTileConfigFound)
			{
                if (browsed)
                {
                    bool result = SetTileConfigFromFilePath(SceneFile_Directory);
                    if (result == false)
                    {
                        TileConfig_SourceID = -1;
                        return result = SetTileConfig(Instance.DataDirectory);
                    }
                    else
                    {
                        TileConfig_SourceID = -3;
                    }
                    return result;
                }
                else
                {
                    TileConfig_SourceID = -1;
                    return SetTileConfig(Instance.DataDirectory);
                }
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
                if (File.Exists(Path.Combine(configPath, "Stages", CurrentZone, "TileConfig.bin")))
                {
                    CurrentSolution.TileConfig = new RSDKv5.Tileconfig(Path.Combine(configPath, "Stages", CurrentZone, "TileConfig.bin"));
                    TileConfig_Source = Path.Combine(configPath, "Stages", CurrentZone, "TileConfig.bin");
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

        public bool SetTileConfigFromFilePath(string filepath)
        {
            try
            {
                CurrentSolution.TileConfig = new RSDKv5.Tileconfig(Path.Combine(filepath, "TileConfig.bin"));
                TileConfig_Source = Path.Combine(filepath, "TileConfig.bin");
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
        public bool GetStageTiles(string Zone, string colors = null, bool browsed = false)
		{
			bool validStageTilesFound = false;
			string validStageTilesPathDir = "";
			foreach (string dataDir in Instance.ResourcePackList)
			{
				if (IsStageTilesValid(dataDir))
				{
                    StageTiles_SourceID = Instance.ResourcePackList.IndexOf(dataDir);
                    validStageTilesFound = true;
					validStageTilesPathDir = dataDir;
					break;
				}
			}
			if (!validStageTilesFound)
			{

                if (browsed)
                {
                    bool result = SetStageTilesFromFilePath(SceneFile_Directory, colors);
                    if (result == false)
                    {
                        StageTiles_SourceID = -1;
                        return result = SetStageTiles(Instance.DataDirectory, Zone, colors);
                    }
                    else
                    {
                        StageTiles_SourceID = -3;
                        return result;
                    }

                }
                else
                {
                    StageTiles_SourceID = -1;
                    return SetStageTiles(Instance.DataDirectory, Zone, colors);
                }
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
				CurrentSolution.CurrentTiles.StageTiles = new StageTiles(Path.Combine(tilePath, "Stages", CurrentZone), colors);
				StageTiles_Source = Path.Combine(tilePath, "Stages", CurrentZone);
				return true;
			}
			catch
			{
				return false;
			}

		}

        public bool SetStageTilesFromFilePath(string filePath, string colors = null)
        {
            try
            {
                CurrentSolution.CurrentTiles.StageTiles = new StageTiles(Path.Combine(filePath), colors);
                StageTiles_Source = Path.Combine(filePath);
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
                    SceneFile_SourceID = Instance.ResourcePackList.IndexOf(dataDir);
                    ValidSceneFileFound = true;
					ValidSceneFilePathDir = dataDir;
					break;
				}
			}
			if (!ValidSceneFileFound)
			{
                SceneFile_SourceID = -1;
                return SetScenePath(Instance.DataDirectory);
			}
			else
			{
				return SetScenePath(ValidSceneFilePathDir);
			}
		}

		public bool IsSceneValid(string directoryToCheck)
		{
			return File.Exists(Path.Combine(directoryToCheck, "Stages", CurrentZone, GetSceneFilename(CurrentSceneID)));
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
		public bool GetStageConfig(string Zone, bool browsed = false)
		{
			bool validStageConfigFound = false;
			string validStageConfigPathDir = "";
			foreach (string dataDir in Instance.ResourcePackList)
			{
				if (IsStageConfigValid(dataDir))
				{
                    StageConfig_SourceID = Instance.ResourcePackList.IndexOf(dataDir);
                    validStageConfigFound = true;
					validStageConfigPathDir = dataDir;
					break;
				}
			}
			if (!validStageConfigFound)
			{
                if (browsed)
                {
                    bool result = SetStageConfigFromFilePath(SceneFile_Directory);
                    if (result == false)
                    {
                        StageConfig_SourceID = -1;
                        return result = SetStageConfig(Instance.DataDirectory);
                    }
                    else
                    {
                        StageConfig_SourceID = -3;
                        return result;
                    }

                }
                else
                {
                    StageConfig_SourceID = -1;
                    return SetStageConfig(Instance.DataDirectory);
                }

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
				CurrentSolution.StageConfig = new StageConfig(Path.Combine(configPath, "Stages", CurrentZone, "StageConfig.bin"));
				StageConfig_Source = Path.Combine(configPath, "Stages", CurrentZone, "StageConfig.bin");
				return true;
			}
			catch
			{
                
				return false;
			}

		}

        public bool SetStageConfigFromFilePath(string filepath)
        {
            try
            {
                CurrentSolution.StageConfig = new StageConfig(Path.Combine(filepath, "StageConfig.bin"));
                StageConfig_Source = Path.Combine(filepath, "StageConfig.bin");
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
            Stamps_SourceID = SceneFile_SourceID;
            Stamps_Source = Path.Combine(SceneFile_Directory, CurrentSolution.CurrentScene.EditorMetadata.StampName.Replace("\0", ""));
            if (IsEditorStampsValid())
            {
                return new Stamps(Path.Combine(SceneFile_Directory, CurrentSolution.CurrentScene.EditorMetadata.StampName.Replace("\0", "")));
            }
            else
            {
                //Check if default name exists (for backwards compatability)
                if (File.Exists(Path.Combine(SceneFile_Directory, "ManiacStamps.bin")))
                {
                    SetEditorStampsName("ManiacStamps.bin");
                    return new Stamps(Path.Combine(SceneFile_Directory, "ManiacStamps.bin"));
                }
                else
                {
                    return new Stamps();
                }
            }
        }

        public void SetEditorStampsName(string Name)
        {
            Stamps_SourceID = SceneFile_SourceID;
            CurrentSolution.CurrentScene.EditorMetadata.StampName = Name;
        }

        public bool IsEditorStampsValid()
		{
			return File.Exists(Path.Combine(SceneFile_Directory, CurrentSolution.CurrentScene.EditorMetadata.StampName.Replace("\0", "")));
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
        GameConfig_SourceID = -2;
        TileConfig_SourceID = -2;
        StageConfig_SourceID = -2;
        StageTiles_SourceID = -2;
        SceneFile_SourceID = -2;
        Stamps_SourceID = -2;
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
