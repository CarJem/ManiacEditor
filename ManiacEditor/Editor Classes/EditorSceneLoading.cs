using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageBox = RSDKrU.MessageBox;
using System.Windows;
using System.IO;
using RSDKv5;

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

			Instance.EditorPath.SceneFilePath = select.SceneSelect.Result;
			Instance.EditorPath.CurrentLevelID = select.SceneSelect.LevelID;
			Instance.EditorPath.isEncoreMode = select.SceneSelect.isEncore;
			Instance.EditorPath.SceneDirectory = Path.GetDirectoryName(Instance.EditorPath.SceneFilePath);
			Instance.EditorPath.CurrentZone = select.SceneSelect.CurrentZone;
			Instance.EditorPath.CurrentName = select.SceneSelect.CurrentName;
			Instance.EditorPath.CurrentSceneID = select.SceneSelect.CurrentSceneID;
			Instance.EditorPath.Browsed = select.SceneSelect.Browsed;

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

			Instance.EditorPath.SceneFilePath = select.SceneSelect.Result;
			Instance.EditorPath.CurrentLevelID = select.SceneSelect.LevelID;
			Instance.EditorPath.isEncoreMode = select.SceneSelect.isEncore;
			Instance.EditorPath.SceneDirectory = Path.GetDirectoryName(Instance.EditorPath.SceneFilePath);
			Instance.EditorPath.CurrentZone = select.SceneSelect.CurrentZone;
			Instance.EditorPath.CurrentName = select.SceneSelect.CurrentName;
			Instance.EditorPath.CurrentSceneID = select.SceneSelect.CurrentSceneID;
			Instance.EditorPath.Browsed = select.SceneSelect.Browsed;

			if (Instance.EditorPath.Browsed)
			{
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

			Instance.EditorPath.SceneFilePath = select.Result;
			Instance.EditorPath.CurrentLevelID = select.LevelID;
			Instance.EditorPath.isEncoreMode = select.isEncore;
			Instance.EditorPath.SceneDirectory = Path.GetDirectoryName(Instance.EditorPath.SceneFilePath);
			Instance.EditorPath.CurrentZone = select.CurrentZone;
			Instance.EditorPath.CurrentName = select.CurrentName;
			Instance.EditorPath.CurrentSceneID = select.CurrentSceneID;
			Instance.EditorPath.Browsed = select.Browsed;

			if (Instance.EditorPath.Browsed)
			{
				LoadFromFiles();
			}
			else
			{
				LoadFromSceneSelect();
			}
		}

		public void LoadFromSceneSelect()
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
		
			AfterLoad();

		}

		public void AfterLoad()
		{
			SetupObjectsList();
			SetupDiscordRP(Instance.EditorPath.SceneFilePath);
			Stamps StageStamps = Instance.EditorPath.GetEditorStamps(Instance.EditorPath.CurrentZone);
			Instance.EditorChunk = new EditorChunk(Instance, Instance.StageTiles, StageStamps);
			Instance.EditorBackground = new EditorBackground(Instance);
			Instance.entities = new EditorEntities(Instance.EditorScene, Instance);



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
			Instance.LevelID = Instance.EditorPath.CurrentLevelID;
			Instance.EditorScene = new EditorScene(Instance.EditorPath.GetScenePathFromFile(Instance.EditorPath.SceneFilePath), Instance.editorView.GraphicPanel, Instance);

			//ACT File (Encore Colors)
			Instance.EncorePalette = Instance.EditorScene.GetEncorePalette(Instance.EditorPath.CurrentZone, Instance.DataDirectory, Instance.EditorPath.CurrentSceneID, Instance.EditorPath.SceneFilePath, 0);
			Instance.EncoreSetupType = Instance.EditorScene.GetEncoreSetupType(Instance.EditorPath.CurrentZone, Instance.DataDirectory, Instance.EditorPath.CurrentSceneID, Instance.EditorPath.SceneFilePath);
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

			AfterLoad();
		}

		public void ReadManiacINIFile()
		{
			if (File.Exists(Instance.EditorPath.SceneFilePath + "\\maniac.ini"))
			{
				bool allowToRead = false;
				using (Stream stream = EditorSettings.GetSceneIniResource(Instance.EditorPath.SceneFilePath + "\\maniac.ini"))
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
						MessageBox.Show("Failed to Inturpret INI File. Error: " + ex.ToString() + " " + Instance.EditorPath.SceneFilePath);
						EditorSettings.CleanPrefrences();
					}

				}


			}
		}
	}
}
