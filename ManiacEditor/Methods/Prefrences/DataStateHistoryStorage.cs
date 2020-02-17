using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ManiacEditor.Methods.Prefrences
{
    public static class DataStateHistoryStorage
    {
        private static Controls.Editor.MainEditor Instance;
        public static ManiacEditor.Classes.Internal.DataStateHistoryCollection Collection = new ManiacEditor.Classes.Internal.DataStateHistoryCollection();
        private static string SettingsFolder { get => GetRecentsListDirectory(); }

        private static string GetRecentsListDirectory()
        {
            return (Methods.Settings.MyInternalSettings.PortableMode ? Classes.Editor.Constants.SettingsPortableDirectory : Classes.Editor.Constants.SettingsStaticDirectory);
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

        public static ManiacEditor.Classes.Internal.DataStateHistoryCollection.SaveState GenerateNewEntry()
        {
            int ResourcePackEntryNumber = 1;

            string Title = "";
            string Name = "";
            if (ManiacEditor.Classes.Editor.SolutionPaths.LoadedDataPack != "")
            {
                Title = string.Format("{0} Data Pack", ManiacEditor.Classes.Editor.SolutionPaths.LoadedDataPack);
            }
            else
            {
                Title = string.Format("{0}", ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory);
            }




            ManiacEditor.Classes.Internal.DataStateHistoryCollection.SaveState section = new ManiacEditor.Classes.Internal.DataStateHistoryCollection.SaveState();
            section.DataDirectory = ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory;
            section.EntryName = Title;
            section.LoadedDataPack = ManiacEditor.Classes.Editor.SolutionPaths.LoadedDataPack;
            section.isDataPack = ManiacEditor.Classes.Editor.SolutionPaths.LoadedDataPack != "" && ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.ResourcePacks.Count > 0;

            Name += ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory;
            Name += section.isDataPack.ToString();
            Name += ManiacEditor.Classes.Editor.SolutionPaths.LoadedDataPack;

            section.RealEntryName = Name;



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
                ManiacEditor.Classes.Internal.DataStateHistoryCollection _Collection = JsonConvert.DeserializeObject<ManiacEditor.Classes.Internal.DataStateHistoryCollection>(data);
                if (_Collection != null) Collection = _Collection;
                else Collection = new Classes.Internal.DataStateHistoryCollection();
            }
            catch
            {
                Collection = new Classes.Internal.DataStateHistoryCollection();
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
