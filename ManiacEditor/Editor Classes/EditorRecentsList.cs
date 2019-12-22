using System;
using System.Collections.Generic;
using System.IO;
using IniParser;
using IniParser.Model;

namespace ManiacEditor
{
    public class EditorRecentSceneSourcesList
	{
		private Editor Instance;
        public SceneSaveStateCollection Collection = new SceneSaveStateCollection();
        IniData RecentsListInfo;
        private string SettingsFolder { get => GetRecentsListDirectory(); }

        private string GetRecentsListDirectory()
        {
            return (Properties.Internal.Default.PortableMode ? EditorConstants.SettingsPortableDirectory : EditorConstants.SettingsStaticDirectory);
        }

        public EditorRecentSceneSourcesList(Editor instance)
		{
			Instance = instance;
			LoadFile();
		}

        public void AddRecentFile(SceneSaveStateCollection.SaveState NewEntry)
        {
            try
            {

                if (Collection == null) Collection = new SceneSaveStateCollection();
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

        public SceneSaveStateCollection.SaveState GenerateNewEntry()
        {
            int ResourcePackEntryNumber = 1;

            string Title = "";
            string Name = "";
            if (Instance.LoadedDataPack != "")
            {
                if (!Instance.Paths.Browsed) Title = string.Format("{1}:{2}{4}{3}{0} Data Pack", Instance.LoadedDataPack, Instance.Paths.CurrentZone, Instance.Paths.CurrentSceneID, "/n/n", (Instance.Paths.isEncoreMode ? "+" : ""));
                else Title = string.Format("{1}{2}{0} Data Pack", Instance.LoadedDataPack, Instance.Paths.SceneFilePath, "/n/n");
            }
            else
            {
                if (!Instance.Paths.Browsed) Title = string.Format("{1}:{2}{4}{3}{0}", Instance.DataDirectory, Instance.Paths.CurrentZone, Instance.Paths.CurrentSceneID, "/n/n", (Instance.Paths.isEncoreMode ? "+" : ""));
                else Title = string.Format("{1}{2}{0}", Instance.DataDirectory, Instance.Paths.SceneFilePath, "/n/n");
            }

            Name += Instance.DataDirectory;
            Name += Instance.Paths.SceneFilePath;
            Name += Instance.Options.LevelID;
            Name += Instance.Paths.CurrentName;
            Name += Instance.Paths.CurrentZone;
            Name += Instance.Paths.CurrentScene;
            Name += Instance.Paths.CurrentSceneID;
            Name += Instance.Paths.Browsed.ToString();
            Name += Instance.Paths.isEncoreMode.ToString();



            SceneSaveStateCollection.SaveState section = new SceneSaveStateCollection.SaveState();
            int x1 = (short)(EditorStateModel.ViewPositionX / EditorStateModel.Zoom);
            int y1 = (short)(EditorStateModel.ViewPositionY / EditorStateModel.Zoom);
            section.EntryName = Title;
            section.RealEntryName = Name;
            section.DataDirectory = Instance.DataDirectory;
            section.Result = Instance.Paths.SceneFilePath;
            section.x = x1;
            section.y = y1;
            section.ZoomLevel = EditorStateModel.ZoomLevel;
            section.isEncore = Instance.Paths.isEncoreMode;
            section.LevelID = Instance.Options.LevelID;
            section.CurrentName = Instance.Paths.CurrentName;
            section.CurrentZone = Instance.Paths.CurrentZone;
            section.CurrentSceneID = Instance.Paths.CurrentSceneID;
            section.Browsed = Instance.Paths.Browsed;
            section.LoadedDataPack = Instance.LoadedDataPack;
            foreach (var pack in Instance.ResourcePackList)
            {
                section.ResourcePacks.Add(pack);
                ResourcePackEntryNumber++;
            }

            return section;
        }

		public void LoadFile()
		{
			if (GetFile() == false)
			{
				var ModListFile = File.Create(Path.Combine(SettingsFolder, "RecentsLists.ini"));
				ModListFile.Close();
				if (GetFile() == false) return;
			}
			InterpretInformation();
		}

		public void InterpretInformation()
		{

            Collection = new SceneSaveStateCollection();
			foreach (var section in RecentsListInfo.Sections)
			{
                SceneSaveStateCollection.SaveState saveState = new SceneSaveStateCollection.SaveState();
                saveState.RealEntryName = section.SectionName;
                foreach (var items in section.Keys)
                {
                    if (items.KeyName == "DataFolder") saveState.DataDirectory = items.Value;
                    else if (items.KeyName == "EntryName") saveState.EntryName = items.Value;
                    else if (items.KeyName == "SceneFile") saveState.Result = items.Value;
                    else if (items.KeyName == "PositionX") saveState.x = (Int32.TryParse(items.Value, out int _x) ? _x : 0);
                    else if (items.KeyName == "PositionY") saveState.y = (Int32.TryParse(items.Value, out int _y) ? _y : 0);
                    else if (items.KeyName == "ZoomLevel") saveState.ZoomLevel = (Int32.TryParse(items.Value, out int _zoom) ? _zoom : 0);
                    else if (items.KeyName == "LevelID") saveState.LevelID = (Int32.TryParse(items.Value, out int _levelID) ? _levelID : 0);
                    else if (items.KeyName == "IsEncore") saveState.isEncore = (bool.TryParse(items.Value, out bool state) ? state : false);
                    else if (items.KeyName == "WasBrowsed") saveState.Browsed = (bool.TryParse(items.Value, out bool state) ? state : false);
                    else if (items.KeyName == "SceneID") saveState.CurrentSceneID = items.Value;
                    else if (items.KeyName == "ZoneName") saveState.CurrentZone = items.Value;
                    else if (items.KeyName == "LoadedDataPack") saveState.LoadedDataPack = items.Value;
                    else if (items.KeyName == "SceneName") saveState.CurrentName = items.Value;
                    else if (isDataPack(items.KeyName)) saveState.ResourcePacks.Add(items.Value);
                }
                Collection.List.Add(saveState);

            }

            bool isDataPack(string title)
            {
                if (title.StartsWith("DataPackPart")) return true;
                else return false;
            }
        }

        private SectionData GetSectionData(SceneSaveStateCollection.SaveState item)
        {
            int ResourcePackEntryNumber = 1;
            SectionData section = new SectionData(item.RealEntryName);
            section.Keys.AddKey("EntryName", item.EntryName);
            section.Keys.AddKey("DataFolder", item.DataDirectory);
            section.Keys.AddKey("SceneFile", item.Result);
            section.Keys.AddKey("PositionX", item.x.ToString());
            section.Keys.AddKey("PositionY", item.y.ToString());
            section.Keys.AddKey("ZoomLevel", item.ZoomLevel.ToString());
            section.Keys.AddKey("IsEncore", item.isEncore.ToString());
            section.Keys.AddKey("LevelID", item.LevelID.ToString());
            section.Keys.AddKey("SceneName", item.CurrentName);
            section.Keys.AddKey("ZoneName", item.CurrentZone);
            section.Keys.AddKey("SceneID", item.CurrentSceneID);
            section.Keys.AddKey("WasBrowsed", item.Browsed.ToString());
            foreach (var pack in item.ResourcePacks)
            {
                section.Keys.AddKey("DataPackPart" + ResourcePackEntryNumber.ToString(), pack);
                ResourcePackEntryNumber++;
            }
            return section;
        }

		public void SaveFile()
		{
			IniData SaveData = new IniData();
			foreach (var pair in Collection.List)
			{
                SectionData section = GetSectionData(pair);
                SaveData.Sections.Add(section);
			}
			string path = Path.Combine(SettingsFolder, "RecentsLists.ini");
			var parser = new FileIniDataParser();
			parser.WriteFile(path, SaveData);
		}

		public static FileStream GetRecentsList(string path)
		{
			if (!File.Exists(path)) return null;
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public bool GetFile()
		{
			var parser = new FileIniDataParser();
            if (!File.Exists(Path.Combine(SettingsFolder, "RecentsLists.ini")))
            {
                return false;
            }
			else
			{
                IniData file = parser.ReadFile(Path.Combine(SettingsFolder, "RecentsLists.ini"));
                RecentsListInfo = file;
			}
			return true;
		}
    }
    public class EditorRecentDataSourcesList
    {
        private Editor Instance;
        public DataSaveStateCollection Collection = new DataSaveStateCollection();
        IniData RecentsListInfo;
        private string SettingsFolder { get => GetRecentsListDirectory(); }

        private string GetRecentsListDirectory()
        {
            return (Properties.Internal.Default.PortableMode ? EditorConstants.SettingsPortableDirectory : EditorConstants.SettingsStaticDirectory);
        }

        public EditorRecentDataSourcesList(Editor instance)
        {
            Instance = instance;
            LoadFile();
        }

        public void AddRecentFile(DataSaveStateCollection.SaveState NewEntry)
        {
            try
            {

                if (Collection == null) Collection = new DataSaveStateCollection();
                if (Collection.List.Contains(NewEntry)) Collection.List.Remove(NewEntry);

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
                System.Diagnostics.Debug.Write("Failed to add Data Source to recent list: " + ex);
            }
        }

        public DataSaveStateCollection.SaveState GenerateNewEntry()
        {
            int ResourcePackEntryNumber = 1;

            string Title = "";
            string Name = "";
            if (Instance.LoadedDataPack != "")
            {
                Title = string.Format("{0} Data Pack", Instance.LoadedDataPack);
            }
            else
            {
               Title = string.Format("{0}", Instance.DataDirectory);
            }




            DataSaveStateCollection.SaveState section = new DataSaveStateCollection.SaveState();
            section.DataDirectory = Instance.DataDirectory;
            section.EntryName = Title;
            section.LoadedDataPack = Instance.LoadedDataPack;
            section.isDataPack = Instance.LoadedDataPack != "" && Instance.ResourcePackList.Count > 0;

            Name += Instance.DataDirectory;
            Name += section.isDataPack.ToString();
            Name += Instance.LoadedDataPack;

            section.RealEntryName = Name;



            foreach (var pack in Instance.ResourcePackList)
            {
                section.ResourcePacks.Add(pack);
                ResourcePackEntryNumber++;
            }

            return section;
        }

        public void LoadFile()
        {
            if (GetFile() == false)
            {
                var ModListFile = File.Create(Path.Combine(SettingsFolder, "RecentDataLists.ini"));
                ModListFile.Close();
                if (GetFile() == false) return;
            }
            InterpretInformation();
        }

        public void InterpretInformation()
        {

            Collection = new DataSaveStateCollection();
            foreach (var section in RecentsListInfo.Sections)
            {
                DataSaveStateCollection.SaveState saveState = new DataSaveStateCollection.SaveState();
                saveState.RealEntryName = section.SectionName;
                foreach (var items in section.Keys)
                {
                    if (items.KeyName == "DataFolder") saveState.DataDirectory = items.Value;
                    else if (items.KeyName == "EntryName") saveState.EntryName = items.Value;
                    else if (items.KeyName == "IsDataPack") saveState.isDataPack = (bool.TryParse(items.Value, out bool state) ? state : false);
                    else if (items.KeyName == "LoadedDataPack") saveState.LoadedDataPack = items.Value;
                    else if (isDataPack(items.KeyName)) saveState.ResourcePacks.Add(items.Value);
                }
                Collection.List.Add(saveState);

            }

            bool isDataPack(string title)
            {
                if (title.StartsWith("DataPackPart")) return true;
                else return false;
            }
        }

        private SectionData GetSectionData(DataSaveStateCollection.SaveState item)
        {
            int ResourcePackEntryNumber = 1;
            SectionData section = new SectionData(item.RealEntryName);
            section.Keys.AddKey("EntryName", item.EntryName);
            section.Keys.AddKey("DataFolder", item.DataDirectory);
            section.Keys.AddKey("IsDataPack", item.isDataPack.ToString());
            section.Keys.AddKey("LoadedDataPack", item.LoadedDataPack);
            foreach (var pack in item.ResourcePacks)
            {
                section.Keys.AddKey("DataPackPart" + ResourcePackEntryNumber.ToString(), pack);
                ResourcePackEntryNumber++;
            }
            return section;
        }

        public void SaveFile()
        {
            IniData SaveData = new IniData();
            foreach (var pair in Collection.List)
            {
                SectionData section = GetSectionData(pair);
                SaveData.Sections.Add(section);
            }
            string path = Path.Combine(SettingsFolder, "RecentDataLists.ini");
            var parser = new FileIniDataParser();
            parser.WriteFile(path, SaveData);
        }

        public static FileStream GetRecentsList(string path)
        {
            if (!File.Exists(path)) return null;
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public bool GetFile()
        {
            var parser = new FileIniDataParser();
            if (!File.Exists(Path.Combine(SettingsFolder, "RecentDataLists.ini")))
            {
                return false;
            }
            else
            {
                IniData file = parser.ReadFile(Path.Combine(SettingsFolder, "RecentDataLists.ini"));
                RecentsListInfo = file;
            }
            return true;
        }
    }


    public class DataSaveStateCollection
    {
        public IList<SaveState> List = new List<SaveState>();

        public class SaveState
        {
            public string EntryName = "";
            public string RealEntryName = "";
            public string DataDirectory = "";
            public string LoadedDataPack = "";
            public bool isDataPack = false;
            public IList<string> ResourcePacks = new List<string>();


            public SaveState()
            {
                EntryName = "";
                DataDirectory = "";
                isDataPack = false;
                ResourcePacks = new List<string>();
            }
        }
        public DataSaveStateCollection()
        {

        }
    }

    public class SceneSaveStateCollection
    {
        public IList<SaveState> List = new List<SaveState>();

        public class SaveState
        {
            public string RealEntryName = "";
            public string EntryName = "";
            public string DataDirectory = "";
            public string Result = "";
            public int LevelID;
            public int ZoomLevel;
            public bool isEncore = false;
            public string CurrentZone = "";
            public string CurrentName = "";
            public string LoadedDataPack = "";
            public string CurrentSceneID = "";
            public bool Browsed = false;
            public IList<string> ResourcePacks = new List<string>();
            public int x;
            public int y;


            public SaveState()
            {
                LoadedDataPack = "";
                EntryName = "";
                DataDirectory = "";
                Result = "";
                LevelID = -1;
                isEncore = false;
                CurrentZone = "";
                CurrentName = "";
                CurrentSceneID = "";
                Browsed = false;
                ResourcePacks = new List<string>();
                x = 0;
                y = 0;
            }
        }
        public SceneSaveStateCollection()
        {

        }
    }
}
