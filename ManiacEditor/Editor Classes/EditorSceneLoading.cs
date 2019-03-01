using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageBox = RSDKrU.MessageBox;
using System.Windows;
using System.IO;
using RSDKv5;
using System.Text.RegularExpressions;

namespace ManiacEditor
{
	public class EditorSceneLoading
	{
		private Editor Instance;
		public EditorSceneLoading(Editor instance)
		{
			Instance = instance;
		}

		private bool EditorLoad()
		{
			if (Instance.DataDirectory == null)
			{
				return false;
			}
			Instance.EditorEntity_ini.ReleaseResources();
			return true;
		}

		public void OpenSceneUsingSceneSelect()
		{
			ManiacEditor.Interfaces.SceneSelectWindow select;

			if (!EditorLoad())
			{
				select = new ManiacEditor.Interfaces.SceneSelectWindow(null, Instance);
				select.Owner = Instance;
				select.ShowDialog();
			}
			else
			{
				select = new ManiacEditor.Interfaces.SceneSelectWindow(Instance.GameConfig, Instance);
				select.Owner = Instance;
				select.ShowDialog();
			}

			if (select.DialogResult != true) return;
			if (PreLoad() == false) return;

			GetSceneSelectData(select.SceneSelect, select.SceneSelect.Browsed);

			if (Instance.EditorPath.Browsed)
			{
				LoadFromFiles();
			}
			else
			{
				LoadFromSceneSelect();
			}

		}

		public void OpenSceneForcefullyUsingSceneSelect(string dataDirectory)
		{
			ManiacEditor.Interfaces.SceneSelectWindow select;
			Instance.EditorPath.SetGameConfig(dataDirectory);

			if (!EditorLoad())
			{
				select = new ManiacEditor.Interfaces.SceneSelectWindow(null, Instance);
				select.Owner = Instance;
				select.ShowDialog();
			}
			else
			{
				select = new ManiacEditor.Interfaces.SceneSelectWindow(Instance.GameConfig, Instance);
				select.Owner = Instance;
				select.ShowDialog();
			}

			if (select.DialogResult != true) return;
			if (PreLoad() == false) return;

			GetSceneSelectData(select.SceneSelect, select.SceneSelect.Browsed);

			if (Instance.EditorPath.Browsed)
			{
                AddTemporaryResourcePack();
                LoadFromFiles();
			}
			else
			{
				LoadFromSceneSelect();
			}

		}

		public void OpenSceneForcefully(string dataDirectory, string Result, int LevelID, bool isEncore, string CurrentName, string CurrentZone, string CurrentSceneID, bool browsedFile)
		{
			if (PreLoad() == false) return;

			if (Instance.EditorPath.SetGameConfig(dataDirectory) == false) return;

			if (browsedFile == true)
			{
				Instance.EditorPath.SceneFilePath = Result;
				Instance.EditorPath.CurrentLevelID = LevelID;
				Instance.EditorPath.isEncoreMode = isEncore;
				Instance.EditorPath.SceneDirectory = Path.GetDirectoryName(Instance.EditorPath.SceneFilePath);
				Instance.EditorPath.CurrentZone = Path.GetFileName(Instance.EditorPath.SceneDirectory);
				Instance.EditorPath.CurrentName = CurrentName;
				Instance.EditorPath.CurrentSceneID = CurrentSceneID;
				Instance.EditorPath.Browsed = browsedFile;
				AddTemporaryResourcePack();
			}
			else
			{
				Instance.EditorPath.SceneFilePath = Result;
				Instance.EditorPath.CurrentLevelID = LevelID;
				Instance.EditorPath.isEncoreMode = isEncore;
				Instance.EditorPath.SceneDirectory = Path.GetDirectoryName(Instance.EditorPath.SceneFilePath);
				Instance.EditorPath.CurrentZone = CurrentZone;
				Instance.EditorPath.CurrentName = CurrentName;
				Instance.EditorPath.CurrentSceneID = CurrentSceneID;
				Instance.EditorPath.Browsed = browsedFile;
			}

			if (Instance.EditorPath.Browsed)
			{
                AddTemporaryResourcePack();
                LoadFromFiles();
			}
			else
			{
				LoadFromSceneSelect();
			}

		}

		public void OpenSceneUsingExistingSceneSelect(ManiacEditor.Interfaces.SceneSelect select)
		{
			if (PreLoad() == false) return;

			GetSceneSelectData(select, select.Browsed);

			if (select.Browsed)
			{
                LoadFromFiles();
			}
			else
			{
				LoadFromSceneSelect();
			}
		}

		public void GetSceneSelectData(ManiacEditor.Interfaces.SceneSelect select, bool browsedFile = false)
		{
			if (browsedFile == true)
			{
				Instance.EditorPath.SceneFilePath = select.Result;
				Instance.EditorPath.CurrentLevelID = select.LevelID;
				Instance.EditorPath.isEncoreMode = select.isEncore;
				Instance.EditorPath.SceneDirectory = Path.GetDirectoryName(Instance.EditorPath.SceneFilePath);
				Instance.EditorPath.CurrentZone = Path.GetFileName(Instance.EditorPath.SceneDirectory);
				Instance.EditorPath.CurrentName = select.CurrentName;
				Instance.EditorPath.CurrentSceneID = select.CurrentSceneID;
				Instance.EditorPath.Browsed = select.Browsed;
				AddTemporaryResourcePack();
			}
			else
			{
				Instance.EditorPath.SceneFilePath = select.Result;
				Instance.EditorPath.CurrentLevelID = select.LevelID;
				Instance.EditorPath.isEncoreMode = select.isEncore;
				Instance.EditorPath.SceneDirectory = Path.GetDirectoryName(Instance.EditorPath.SceneFilePath);
				Instance.EditorPath.CurrentZone = select.CurrentZone;
				Instance.EditorPath.CurrentName = select.CurrentName;
				Instance.EditorPath.CurrentSceneID = select.CurrentSceneID;
				Instance.EditorPath.Browsed = select.Browsed;
			}
		}

		public void AddTemporaryResourcePack()
		{
			if (Instance.EditorPath.SceneDirectory.Contains("Data") && Instance.EditorPath.SceneDirectory.Contains("Stages"))
			{
				var input = Instance.EditorPath.SceneDirectory;
				var output = input.Replace("\\Stages\\" + Instance.EditorPath.CurrentZone, "");
				if (output.Contains("Data"))
				{
					Instance.ResourcePackList.Add(output);
				}
			}
		}

		public void LoadFromSceneSelect()
		{
			bool LoadFailed = false;
			try
			{
				//Using Instance Means the Stuff Hasn't Stated 
				Instance.LevelID = Instance.EditorPath.CurrentLevelID;
				Instance.EditorScene = new EditorScene(Instance.EditorPath.GetScenePath(), Instance.editorView.GraphicPanel, Instance);

				//ACT File (Encore Colors)
				Instance.EncorePalette = Instance.EditorScene.GetEncorePalette(Instance.EditorPath.CurrentZone, Instance.DataDirectory, Instance.EditorPath.CurrentSceneID, "", 1);
				Instance.EncoreSetupType = Instance.EditorScene.GetEncoreSetupType(Instance.EditorPath.CurrentZone, Instance.DataDirectory, Instance.EditorPath.CurrentSceneID, "");
				if (Instance.EncorePalette[0] != "")
				{
					Instance.encorePaletteExists = true;
					if (Instance.EditorPath.isEncoreMode)
					{
						Instance.EncorePaletteButton.IsChecked = true;
						Instance.useEncoreColors = true;
					}
				}

				//Stage Tiles
				if (Instance.useEncoreColors == true && Instance.EncorePalette[0] != "") Instance.EditorPath.GetStageTiles(Instance.EditorPath.CurrentZone, Instance.EncorePalette[0]);
				else Instance.EditorPath.GetStageTiles(Instance.EditorPath.CurrentZone);

				//Tile Config
				Instance.CollisionLayerA.Clear();
				Instance.CollisionLayerB.Clear();
				Instance.EditorPath.GetTileConfig(Instance.EditorPath.CurrentZone);
				if (Instance.TilesConfig != null)
				{
					for (int i = 0; i < 1024; i++)
					{
						Instance.CollisionLayerA.Add(Instance.TilesConfig.CollisionPath1[i].DrawCMask(System.Drawing.Color.FromArgb(0, 0, 0, 0), Instance.CollisionAllSolid));
						Instance.CollisionLayerB.Add(Instance.TilesConfig.CollisionPath2[i].DrawCMask(System.Drawing.Color.FromArgb(0, 0, 0, 0), Instance.CollisionAllSolid));
					}
				}

				Instance.EditorPath.GetStageConfig(Instance.EditorPath.CurrentZone);
			}
			catch (Exception ex)
			{
				LoadFailed = true;
				MessageBox.Show("Load failed. Error: " + ex.ToString());
				return;
			}

			if (!LoadFailed) AfterLoad();

		}

		public void AfterLoad()
		{
			SetupObjectsList();
			SetupDiscordRP(Instance.EditorPath.SceneFilePath);
			Stamps StageStamps = Instance.EditorPath.GetEditorStamps(Instance.EditorPath.CurrentZone);
			Instance.EditorChunk = new EditorChunk(Instance, Instance.EditorTiles.StageTiles, StageStamps);
			Instance.EditorBackground = new EditorBackground(Instance);
			Instance.entities = new EditorEntities(Instance.EditorScene, Instance);

            ReadManiacINIFile();
			Instance.UpdateStartScreen(false);
			Instance.UpdateDataFolderLabel(null, null);
			Instance.SetupLayerButtons();
			Instance.SetViewSize((int)(Instance.SceneWidth * Instance.Zoom), (int)(Instance.SceneHeight * Instance.Zoom));
			Instance.UpdateControls(true);
		}

		public bool PreLoad()
		{
			Instance.UnloadScene();
			Instance.UseDefaultPrefrences();
            Instance.EditorTiles = new EditorTiles(Instance);
            return Instance.SetGameConfig();
		}

		public void SetupObjectsList()
		{
			Instance.ObjectList.Clear();
			for (int i = 0; i < Instance.GameConfig.ObjectsNames.Count; i++)
			{
				Instance.ObjectList.Add(Instance.GameConfig.ObjectsNames[i]);
			}
			for (int i = 0; i < Instance.StageConfig.ObjectsNames.Count; i++)
			{
				Instance.ObjectList.Add(Instance.StageConfig.ObjectsNames[i]);
			}
		}

		public void SetupDiscordRP(string SceneFile)
		{
			Instance.Discord.ScenePath = SceneFile;
			Instance.Discord.UpdateDiscord("Editing " + SceneFile);
		}

		public void LoadFromFiles()
		{
			bool LoadFailed = false;
			try
			{
				Instance.LevelID = Instance.EditorPath.CurrentLevelID;
				Instance.EditorScene = new EditorScene(Instance.EditorPath.GetScenePathFromFile(Instance.EditorPath.SceneFilePath), Instance.editorView.GraphicPanel, Instance);


				//ACT File (Encore Colors)
				Instance.EncorePalette = Instance.EditorScene.GetEncorePalette(Instance.EditorPath.CurrentZone, Instance.DataDirectory, Instance.EditorPath.CurrentSceneID, Instance.EditorPath.SceneDirectory, 0);
				Instance.EncoreSetupType = Instance.EditorScene.GetEncoreSetupType(Instance.EditorPath.CurrentZone, Instance.DataDirectory, Instance.EditorPath.CurrentSceneID, Instance.EditorPath.SceneDirectory);
				if (Instance.EncorePalette[0] != "")
				{
					Instance.encorePaletteExists = true;
					if (Instance.EditorPath.isEncoreMode)
					{
						Instance.EncorePaletteButton.IsChecked = true;
						Instance.useEncoreColors = true;
					}
				}

				//Stage Tiles
				if (Instance.useEncoreColors == true && Instance.EncorePalette[0] != "") Instance.EditorPath.GetStageTiles(Instance.EditorPath.CurrentZone, Instance.EncorePalette[0], Instance.EditorPath.Browsed);
				else Instance.EditorPath.GetStageTiles(Instance.EditorPath.CurrentZone, null, Instance.EditorPath.Browsed);

				//Tile Config
				Instance.CollisionLayerA.Clear();
				Instance.CollisionLayerB.Clear();
				Instance.EditorPath.GetTileConfig(Instance.EditorPath.CurrentZone, Instance.EditorPath.Browsed);
				if (Instance.TilesConfig != null)
				{
					for (int i = 0; i < 1024; i++)
					{
						Instance.CollisionLayerA.Add(Instance.TilesConfig.CollisionPath1[i].DrawCMask(System.Drawing.Color.FromArgb(0, 0, 0, 0), Instance.CollisionAllSolid));
						Instance.CollisionLayerB.Add(Instance.TilesConfig.CollisionPath2[i].DrawCMask(System.Drawing.Color.FromArgb(0, 0, 0, 0), Instance.CollisionAllSolid));
					}
				}

				Instance.EditorPath.GetStageConfig(Instance.EditorPath.CurrentZone, Instance.EditorPath.Browsed);
			}
			catch (Exception ex)
			{
				LoadFailed = true;
				MessageBox.Show("Load failed. Error: " + ex.ToString());
				return;
			}


			if (!LoadFailed) AfterLoad();
		}

		public void ReadManiacINIFile()
		{
			if (File.Exists(Instance.EditorPath.SceneDirectory + "\\maniac.ini"))
			{
				bool allowToRead = false;
				using (Stream stream = EditorSettings.GetSceneIniResource(Instance.EditorPath.SceneDirectory + "\\maniac.ini"))
				{
					if (stream != null)
					{
						EditorSettings.GetSceneINISettings(stream);
						allowToRead = true;
					}
					else
					{
						System.Diagnostics.Debug.Print("Something went wrong trying to read the Maniac.INI File");
						allowToRead = false;
					}
				}
				if (allowToRead)
				{
					try
					{
						EditorSettings.SetINIDefaultPrefrences(Instance);
					}
					catch (Exception ex)
					{
						MessageBox.Show("Failed to Inturpret INI File. Error: " + ex.ToString() + " " + Instance.EditorPath.SceneDirectory);
						EditorSettings.CleanPrefrences();
					}

				}


			}
		}

		public void Save()
		{
			if (Instance.EditorScene == null) return;
			if (Instance.IsTilesEdit()) Instance.Deselect();

			SaveScene();
			SaveStageConfig();
			SaveChunks();
		}

		public void SaveScene()
		{
			try
			{
				Instance.EditorScene.Save(Instance.EditorPath.SceneFile_Source);
			}
			catch (Exception ex)
			{
				Instance.ShowError($@"Failed to save the scene to file '{Instance.EditorPath.SceneFile_Source}' Error: {ex.Message}");
			}
		}

		public void SaveStageConfig()
		{
			try
			{
				Instance.StageConfig?.Write(Instance.EditorPath.StageConfig_Source);
			}
			catch (Exception ex)
			{
				Instance.ShowError($@"Failed to save the StageConfig to file '{Instance.EditorPath.StageConfig_Source}'
Error: {ex.Message}");
			}
		}

		public void SaveChunks()
		{
			try
			{
				if (Instance.EditorChunk.StageStamps?.loadstate == RSDKv5.Stamps.LoadState.Upgrade)
				{
					MessageBoxResult result = RSDKrU.MessageBox.Show("This Editor Chunk File needs to be updated to a newer version of the format. This will happen almost instantly, however you will be unable to use your chunks in a previous version of maniac on this is done. Would you like to continue?" + Environment.NewLine + "(Click Yes to Save, Click No to Continue without Saving Your Chunks)", "Chunk File Format Upgrade Required", MessageBoxButton.YesNo, MessageBoxImage.Warning);
					if (result == MessageBoxResult.Yes)
					{
						Instance.EditorChunk.StageStamps?.Write(Instance.EditorPath.Stamps_Source);
					}
				}
				else
				{
					Instance.EditorChunk.StageStamps?.Write(Instance.EditorPath.Stamps_Source);
				}


			}
			catch (Exception ex)
			{
				Instance.ShowError($@"Failed to save StageStamps to file '{Instance.EditorPath.SceneFile_Source}' Error: {ex.Message}");
			}
		}
	}
}
