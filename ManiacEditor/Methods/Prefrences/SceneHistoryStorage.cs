using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ManiacEditor.Methods.Prefrences
{
    public static class SceneHistoryStorage
	{
		private static Controls.Editor.MainEditor Instance;
        public static ManiacEditor.Classes.Internal.SceneHistoryCollection Collection = new ManiacEditor.Classes.Internal.SceneHistoryCollection();
        private static string SettingsFolder { get => ManiacEditor.Classes.Editor.Constants.GetSettingsDirectory(); }


        public static void UpdateInstance(Controls.Editor.MainEditor instance)
        {
            Instance = instance;
            LoadFile();
        }

        public static void Initilize(Controls.Editor.MainEditor instance)
        {
            UpdateInstance(instance);
            LoadFile();
        }

        public static void AddRecentFile(ManiacEditor.Classes.Internal.SceneHistoryCollection.SaveState NewEntry)
        {
            try
            {

                if (Collection == null) Collection = new ManiacEditor.Classes.Internal.SceneHistoryCollection();
                if (Collection.List.Contains(NewEntry) || Collection.List.Contains(NewEntry)) Collection.List.Remove(NewEntry);

                if (Collection.List.Count >= 10)
                {
                    for (int i = 9; i < Collection.List.Count; i++)
                    {
                        Collection.List.RemoveAt(i);
                    }
                }

                Collection.List.Insert(0, NewEntry);

                SaveFile();
                LoadFile();


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("Failed to add data folder to recent list: " + ex);
            }
        }

        public static ManiacEditor.Classes.Internal.SceneHistoryCollection.SaveState GenerateNewEntry()
        {
            int ResourcePackEntryNumber = 1;

            string Title = "";
            string Name = "";
            if (ManiacEditor.Classes.Editor.SolutionPaths.LoadedDataPack != "")
            {
                if (!ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsFullPath) Title = string.Format("{1}:{2}{4}{3}{0} Data Pack", ManiacEditor.Classes.Editor.SolutionPaths.LoadedDataPack, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneID, "/n/n", (ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsEncoreMode ? "+" : ""));
                else Title = string.Format("{1}{2}{0} Data Pack", ManiacEditor.Classes.Editor.SolutionPaths.LoadedDataPack, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.FilePath, "/n/n");
            }
            else if (ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.WasSelfLoaded)
            {
                Title = string.Format("{1}\\{2}{4}{3}{0}", System.IO.Path.GetDirectoryName(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.FilePath), ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone, System.IO.Path.GetFileName(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.FilePath), "/n/n", (ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsEncoreMode ? "+" : ""));                
            }
            else
            {
                if (!ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsFullPath) Title = string.Format("{1}:{2}{4}{3}{0}", ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneID, "/n/n", (ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsEncoreMode ? "+" : ""));
                else Title = string.Format("{1}{2}{0}", ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.FilePath, "/n/n");
            }

            Name += ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory;
            Name += ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.FilePath;
            Name += Classes.Editor.SolutionState.LevelID;
            Name += ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Name;
            Name += ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone;
            Name += ManiacEditor.Classes.Editor.SolutionPaths.CurrentScene;
            Name += ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.SceneID;
            Name += ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsFullPath.ToString();
            Name += ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.IsEncoreMode.ToString();



            ManiacEditor.Classes.Internal.SceneHistoryCollection.SaveState section = new ManiacEditor.Classes.Internal.SceneHistoryCollection.SaveState();
            int x1 = (short)(Classes.Editor.SolutionState.ViewPositionX / Classes.Editor.SolutionState.Zoom);
            int y1 = (short)(Classes.Editor.SolutionState.ViewPositionY / Classes.Editor.SolutionState.Zoom);
            section.EntryName = Title;
            section.RealEntryName = Name;
            section.SceneState = ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData;
            section.x = x1;
            section.y = y1;
            section.ZoomLevel = Classes.Editor.SolutionState.ZoomLevel;
            section.LoadedDataPack = ManiacEditor.Classes.Editor.SolutionPaths.LoadedDataPack;
            foreach (var pack in ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.ResourcePacks)
            {
                section.ResourcePacks.Add(pack);
                ResourcePackEntryNumber++;
            }

            return section;
        }

		public static void LoadFile()
		{
			if (GetFile() == false)
			{
				var ModListFile = File.Create(Path.Combine(SettingsFolder, "RecentsLists.json"));
				ModListFile.Close();
				if (GetFile() == false) return;
			}
			InterpretInformation();
		}

		public static void InterpretInformation()
		{
            try
            {
                string path = GetFilePath();
                string data = File.ReadAllText(path);
                ManiacEditor.Classes.Internal.SceneHistoryCollection _Collection = JsonConvert.DeserializeObject<ManiacEditor.Classes.Internal.SceneHistoryCollection>(data);
                if (_Collection != null) Collection = _Collection;
                else Collection = new Classes.Internal.SceneHistoryCollection();

            }
            catch
            {
                Collection = new Classes.Internal.SceneHistoryCollection();
            }

        }

        public static string GetFilePath()
        {
            return Path.Combine(Path.Combine(SettingsFolder, "RecentsLists.json"));
        }

        public static void SaveFile()
		{
            var path = GetFilePath();
            string data = JsonConvert.SerializeObject(Collection, Formatting.Indented);
            File.WriteAllText(path, data);
        }

		public static bool GetFile()
		{
            if (!File.Exists(Path.Combine(SettingsFolder, "RecentsLists.json")))
            {
                return false;
            }
			else
			{
                return true;
			}

		}
    }

}
