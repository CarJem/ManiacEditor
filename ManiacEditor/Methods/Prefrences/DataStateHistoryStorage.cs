using System;
using System.Collections.Generic;
using System.IO;
using IniParser;
using IniParser.Model;

namespace ManiacEditor.Methods.Prefrences
{
    public static class DataStateHistoryStorage
    {
        private static Controls.Editor.MainEditor Instance;
        public static ManiacEditor.Classes.Internal.DataStateHistoryCollection Collection = new ManiacEditor.Classes.Internal.DataStateHistoryCollection();
        static IniData RecentsListInfo;
        private static string SettingsFolder { get => GetRecentsListDirectory(); }

        private static string GetRecentsListDirectory()
        {
            return (Core.Settings.MyInternalSettings.PortableMode ? Classes.Editor.Constants.SettingsPortableDirectory : Classes.Editor.Constants.SettingsStaticDirectory);
        }

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

        public static void AddRecentFile(ManiacEditor.Classes.Internal.DataStateHistoryCollection.SaveState NewEntry)
        {
            try
            {

                if (Collection == null) Collection = new ManiacEditor.Classes.Internal.DataStateHistoryCollection();
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

        public static ManiacEditor.Classes.Internal.DataStateHistoryCollection.SaveState GenerateNewEntry()
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




            ManiacEditor.Classes.Internal.DataStateHistoryCollection.SaveState section = new ManiacEditor.Classes.Internal.DataStateHistoryCollection.SaveState();
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

        public static void LoadFile()
        {
            if (GetFile() == false)
            {
                var ModListFile = File.Create(Path.Combine(SettingsFolder, "RecentDataLists.ini"));
                ModListFile.Close();
                if (GetFile() == false) return;
            }
            InterpretInformation();
        }

        public static void InterpretInformation()
        {

            Collection = new ManiacEditor.Classes.Internal.DataStateHistoryCollection();
            foreach (var section in RecentsListInfo.Sections)
            {
                ManiacEditor.Classes.Internal.DataStateHistoryCollection.SaveState saveState = new ManiacEditor.Classes.Internal.DataStateHistoryCollection.SaveState();
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

        private static SectionData GetSectionData(ManiacEditor.Classes.Internal.DataStateHistoryCollection.SaveState item)
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

        public static void SaveFile()
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

        public static bool GetFile()
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


}
