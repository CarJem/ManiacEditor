using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ManiacEditor.Classes.Prefrences
{
    public static class DataStateHistoryStorage
    {

        public class DataStateHistoryCollection
        {
            public List<SaveState> List = new List<SaveState>();

            public class SaveState
            {
                public string DataDirectory
                {
                    get
                    {
                        if (this.ExtraDataDirectories != null && this.ExtraDataDirectories.Count >= 1) return this.ExtraDataDirectories[0];
                        else return string.Empty;
                    }
                    set
                    {
                        if (this.ExtraDataDirectories == null) this.ExtraDataDirectories = new List<string>();
                        else this.ExtraDataDirectories.Clear();
                        this.ExtraDataDirectories.Add(value);
                    }
                }
                public string EntryName = "";
                public string RealEntryName = "";
                public string MasterDataDirectory = "";
                public IList<string> ExtraDataDirectories = new List<string>();


                public SaveState()
                {
                    EntryName = "";
                    MasterDataDirectory = "";
                    ExtraDataDirectories = new List<string>();
                }
            }
            public DataStateHistoryCollection()
            {

            }
        }

        private static Controls.Editor.MainEditor Instance;
        public static DataStateHistoryCollection Collection = new DataStateHistoryCollection();
        private static string SettingsFolder { get => GetRecentsListDirectory(); }

        private static string GetRecentsListDirectory()
        {
            return (Properties.Settings.MyInternalSettings.PortableMode ? Methods.ProgramPaths.SettingsPortableDirectory : Methods.ProgramPaths.SettingsStaticDirectory);
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

        public static void ClearRecentFiles()
        {

        }

        public static void AddRecentFile(DataStateHistoryCollection.SaveState NewEntry)
        {
            try
            {

                if (Collection == null) Collection = new DataStateHistoryCollection();
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
                System.Diagnostics.Debug.Write("Failed to add Data Source to recent list: " + ex);
            }
        }

        public static DataStateHistoryCollection.SaveState GenerateNewEntry()
        {
            int ResourcePackEntryNumber = 1;

            string Title = "";
            string Name = "";

            DataStateHistoryCollection.SaveState section = new DataStateHistoryCollection.SaveState();
            section.MasterDataDirectory = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.MasterDataDirectory;
            section.ExtraDataDirectories = ManiacEditor.Methods.Editor.SolutionPaths.CurrentSceneData.ExtraDataDirectories;

            Title = string.Format("Data Dir: {0}", (section.DataDirectory == string.Empty ? "N/A" : section.DataDirectory));
            Name = string.Format("Master Dat Dir: {0}", section.MasterDataDirectory);

            section.EntryName = Title + Environment.NewLine + Name;

            section.RealEntryName = Title + Environment.NewLine + Name;

            return section;
        }

        public static void LoadFile()
        {
            if (GetFile() == false)
            {
                var ModListFile = File.Create(Path.Combine(SettingsFolder, "RecentDataLists.json"));
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
                DataStateHistoryCollection _Collection = JsonConvert.DeserializeObject<DataStateHistoryCollection>(data);
                if (_Collection != null) Collection = _Collection;
                else Collection = new DataStateHistoryCollection();
            }
            catch
            {
                Collection = new DataStateHistoryCollection();
            }

        }

        public static void SaveFile()
        {
            var path = GetFilePath();
            string data = JsonConvert.SerializeObject(Collection, Formatting.Indented);
            File.WriteAllText(path, data);
        }

        public static bool GetFile()
        {
            if (!File.Exists(Path.Combine(SettingsFolder, "RecentDataLists.json")))
            {
                return false;
            }
            else return true;

        }

        public static string GetFilePath()
        {
            return Path.Combine(Path.Combine(SettingsFolder, "RecentDataLists.json"));
        }
    }


}
