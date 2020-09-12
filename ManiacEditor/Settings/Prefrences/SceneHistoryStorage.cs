using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using ManiacEditor.Structures;

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
                if (Collection.List.Exists(x => x.SceneState.FilePath == NewEntry.SceneState.FilePath)) Collection.List.RemoveAll(x => x.SceneState.FilePath == NewEntry.SceneState.FilePath);

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
            if (ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.WasSelfLoaded)
            {
                string FilePath = System.IO.Path.GetDirectoryName(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.FilePath);
                string NewLine = "/n/n";

                Title = Extensions.Extensions.ShrinkPath(FilePath, 50);
                Title += Environment.NewLine;
                Title += "[EXTERNAL]";
            }
            else
            {
                string FileName = System.IO.Path.GetFileName(ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.FilePath);
                string Zone = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone;
                string Encore = (ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.IsEncoreMode ? "+" : "");
                string DataFolder = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.DataDirectory;
                string MasterDataFolder = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory;
                string NewLine = "/n/n";

                Title = "..." + Zone + "\\" + FileName + Encore;
                Title += Environment.NewLine;

                if (DataFolder != null && DataFolder != string.Empty) Title += Extensions.Extensions.ShrinkPath(DataFolder, 50) + string.Format(" [{0}]", Path.GetFileName(Path.GetDirectoryName(DataFolder)));
                else Title += Extensions.Extensions.ShrinkPath(MasterDataFolder, 50) + string.Format(" [{0}]", Path.GetFileName(Path.GetDirectoryName(MasterDataFolder)));
            }

            SceneHistoryCollection.SaveState section = new SceneHistoryCollection.SaveState();
            section.EntryName = Title;
            section.RealEntryName = Name;
            section.SceneState = ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData;
            section.SceneState.ExtraDataDirectories = section.SceneState.ExtraDataDirectories.Distinct().ToList();
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
