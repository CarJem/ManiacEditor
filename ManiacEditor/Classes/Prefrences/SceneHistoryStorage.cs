using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using ManiacEditor.Classes.General;

namespace ManiacEditor.Classes.Prefrences
{
    public static class SceneHistoryStorage
	{
        public class SceneHistoryCollection
        {
            public List<SaveState> List = new List<SaveState>();

            public class SaveState
            {
                public string RealEntryName = "";
                public string EntryName = "";
                public SceneState SceneState = new SceneState();
                public int ZoomLevel;
                public string LoadedDataPack = "";
                public IList<string> ResourcePacks = new List<string>();
                public int x;
                public int y;


                public SaveState()
                {
                    LoadedDataPack = "";
                    EntryName = "";
                    SceneState = new SceneState();
                    ResourcePacks = new List<string>();
                    x = 0;
                    y = 0;
                }
            }
            public SceneHistoryCollection()
            {

            }
        }

        private static Controls.Editor.MainEditor Instance;
        public static SceneHistoryCollection Collection = new SceneHistoryCollection();
        private static string SettingsFolder { get => Methods.ProgramPaths.GetSettingsDirectory(); }


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

        public static void AddRecentFile(SceneHistoryCollection.SaveState NewEntry)
        {
            try
            {

                if (Collection == null) Collection = new SceneHistoryCollection();
                if (Collection.List.Exists(x => x.RealEntryName == NewEntry.RealEntryName)) Collection.List.RemoveAll(x => x.RealEntryName == NewEntry.RealEntryName);

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

        public static SceneHistoryCollection.SaveState GenerateNewEntry()
        {
            int ResourcePackEntryNumber = 1;

            string Title = "";
            string Name = "";
            if (ManiacEditor.Methods.Editor.SolutionPaths.LoadedDataPack != "")
            {
                if (!ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsFullPath) Title = string.Format("{1}:{2}{4}{3}{0} Data Pack", ManiacEditor.Methods.Editor.SolutionPaths.LoadedDataPack, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneID, "/n/n", (ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsEncoreMode ? "+" : ""));
                else Title = string.Format("{1}{2}{0} Data Pack", ManiacEditor.Methods.Editor.SolutionPaths.LoadedDataPack, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.FilePath, "/n/n");
            }
            else if (ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.WasSelfLoaded)
            {
                Title = string.Format("{1}\\{2}{4}{3}{0}", System.IO.Path.GetDirectoryName(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.FilePath), ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, System.IO.Path.GetFileName(ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.FilePath), "/n/n", (ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsEncoreMode ? "+" : ""));                
            }
            else
            {
                if (!ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsFullPath) Title = string.Format("{1}:{2}{4}{3}{0}", ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneID, "/n/n", (ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsEncoreMode ? "+" : ""));
                else Title = string.Format("{1}{2}{0}", ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory, ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.FilePath, "/n/n");
            }

            Name += ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory;
            Name += ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.FilePath;
            Name += Methods.Editor.SolutionState.LevelID;
            Name += ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Name;
            Name += ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.Zone;
            Name += ManiacEditor.Methods.Editor.SolutionPaths.CurrentScene;
            Name += ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.SceneID;
            Name += ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsFullPath.ToString();
            Name += ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.IsEncoreMode.ToString();



            SceneHistoryCollection.SaveState section = new SceneHistoryCollection.SaveState();
            int x1 = (short)(Methods.Editor.SolutionState.ViewPositionX / Methods.Editor.SolutionState.OldZoom);
            int y1 = (short)(Methods.Editor.SolutionState.ViewPositionY / Methods.Editor.SolutionState.OldZoom);
            section.EntryName = Title;
            section.RealEntryName = Name;
            section.SceneState = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData;
            section.x = x1;
            section.y = y1;
            section.ZoomLevel = Methods.Editor.SolutionState.ZoomLevel;
            section.LoadedDataPack = ManiacEditor.Methods.Editor.SolutionPaths.LoadedDataPack;
            foreach (var pack in ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.ExtraDataDirectories)
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
                SceneHistoryCollection _Collection = JsonConvert.DeserializeObject<SceneHistoryCollection>(data);
                if (_Collection != null) Collection = _Collection;
                else Collection = new SceneHistoryCollection();

            }
            catch
            {
                Collection = new SceneHistoryCollection();
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
