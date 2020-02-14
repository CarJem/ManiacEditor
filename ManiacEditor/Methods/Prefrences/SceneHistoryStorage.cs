using System;
using System.Collections.Generic;
using System.IO;
using IniParser;
using IniParser.Model;

namespace ManiacEditor.Methods.Prefrences
{
    public static class SceneHistoryStorage
	{
		private static Controls.Editor.MainEditor Instance;
        public static ManiacEditor.Classes.Internal.SceneHistoryCollection Collection = new ManiacEditor.Classes.Internal.SceneHistoryCollection();
        static IniData RecentsListInfo;
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
            if (Instance.LoadedDataPack != "")
            {
                if (!ManiacEditor.Classes.Editor.Solution.Paths.Browsed) Title = string.Format("{1}:{2}{4}{3}{0} Data Pack", Instance.LoadedDataPack, ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID, "/n/n", (ManiacEditor.Classes.Editor.Solution.Paths.isEncoreMode ? "+" : ""));
                else Title = string.Format("{1}{2}{0} Data Pack", Instance.LoadedDataPack, ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath, "/n/n");
            }
            else
            {
                if (!ManiacEditor.Classes.Editor.Solution.Paths.Browsed) Title = string.Format("{1}:{2}{4}{3}{0}", Instance.DataDirectory, ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone, ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID, "/n/n", (ManiacEditor.Classes.Editor.Solution.Paths.isEncoreMode ? "+" : ""));
                else Title = string.Format("{1}{2}{0}", Instance.DataDirectory, ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath, "/n/n");
            }

            Name += Instance.DataDirectory;
            Name += ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath;
            Name += Classes.Editor.SolutionState.LevelID;
            Name += ManiacEditor.Classes.Editor.Solution.Paths.CurrentName;
            Name += ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone;
            Name += ManiacEditor.Classes.Editor.Solution.Paths.CurrentScene;
            Name += ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID;
            Name += ManiacEditor.Classes.Editor.Solution.Paths.Browsed.ToString();
            Name += ManiacEditor.Classes.Editor.Solution.Paths.isEncoreMode.ToString();



            ManiacEditor.Classes.Internal.SceneHistoryCollection.SaveState section = new ManiacEditor.Classes.Internal.SceneHistoryCollection.SaveState();
            int x1 = (short)(Classes.Editor.SolutionState.ViewPositionX / Classes.Editor.SolutionState.Zoom);
            int y1 = (short)(Classes.Editor.SolutionState.ViewPositionY / Classes.Editor.SolutionState.Zoom);
            section.EntryName = Title;
            section.RealEntryName = Name;
            section.DataDirectory = Instance.DataDirectory;
            section.Result = ManiacEditor.Classes.Editor.Solution.Paths.SceneFilePath;
            section.x = x1;
            section.y = y1;
            section.ZoomLevel = Classes.Editor.SolutionState.ZoomLevel;
            section.isEncore = ManiacEditor.Classes.Editor.Solution.Paths.isEncoreMode;
            section.LevelID = Classes.Editor.SolutionState.LevelID;
            section.CurrentName = ManiacEditor.Classes.Editor.Solution.Paths.CurrentName;
            section.CurrentZone = ManiacEditor.Classes.Editor.Solution.Paths.CurrentZone;
            section.CurrentSceneID = ManiacEditor.Classes.Editor.Solution.Paths.CurrentSceneID;
            section.Browsed = ManiacEditor.Classes.Editor.Solution.Paths.Browsed;
            section.LoadedDataPack = Instance.LoadedDataPack;
            foreach (var pack in Instance.ResourcePackList)
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
				var ModListFile = File.Create(Path.Combine(SettingsFolder, "RecentsLists.ini"));
				ModListFile.Close();
				if (GetFile() == false) return;
			}
			InterpretInformation();
		}

		public static void InterpretInformation()
		{

            Collection = new ManiacEditor.Classes.Internal.SceneHistoryCollection();
			foreach (var section in RecentsListInfo.Sections)
			{
                ManiacEditor.Classes.Internal.SceneHistoryCollection.SaveState saveState = new ManiacEditor.Classes.Internal.SceneHistoryCollection.SaveState();
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

        private static SectionData GetSectionData(ManiacEditor.Classes.Internal.SceneHistoryCollection.SaveState item)
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

		public static void SaveFile()
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

		public static bool GetFile()
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

}
