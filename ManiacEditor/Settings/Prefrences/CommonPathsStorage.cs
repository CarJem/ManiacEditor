using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Specialized;
using System.IO;
using Newtonsoft.Json;

namespace ManiacEditor.Classes.Prefrences
{
    public static class CommonPathsStorage 
    {
        private static Controls.Editor.MainEditor Instance;
        public static CommonPathsCollection Collection = new CommonPathsCollection();
        public class CommonPathsCollection
        {
            public System.Collections.Specialized.StringCollection RecentDataDirectories { get; set; }
            public System.Collections.Specialized.StringCollection SavedDataDirectories { get; set; }
            public string DefaultMasterDataDirectory { get; set; }
            public System.Collections.Specialized.StringCollection SavedPlaces { get; set; }
            public System.Collections.Specialized.StringCollection ModLoaderConfigs { get; set; }
            public System.Collections.Specialized.StringCollection ModLoaderConfigsNames { get; set; }
        }

        #region Collection Methods
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
        public static void LoadFile()
        {
            if (GetFile() == false)
            {
                var ModListFile = File.Create(Path.Combine(SettingsFolder, "CommonPaths.json"));
                ModListFile.Close();
                if (GetFile() == false) return;
            }
            InterpretInformation();
        }
        public static bool GetFile()
        {
            if (!File.Exists(Path.Combine(SettingsFolder, "CommonPaths.json")))
            {
                return false;
            }
            else return true;

        }
        public static void InterpretInformation()
        {
            try
            {
                string path = GetFilePath();
                string data = File.ReadAllText(path);
                CommonPathsCollection _Collection = JsonConvert.DeserializeObject<CommonPathsCollection>(data);
                if (_Collection != null) Collection = _Collection;
                else Collection = new CommonPathsCollection();
            }
            catch
            {
                Collection = new CommonPathsCollection();
            }

        }
        public static void SaveFile()
        {
            var path = GetFilePath();
            string data = JsonConvert.SerializeObject(Collection, Formatting.Indented);
            File.WriteAllText(path, data);
        }
        public static string GetFilePath()
        {
            return Path.Combine(Path.Combine(SettingsFolder, "CommonPaths.json"));
        }

        #endregion

        #region Managament Methods

        public static void RemoveNullEntries()
        {
            if (Classes.Prefrences.CommonPathsStorage.Collection.RecentDataDirectories != null) Classes.Prefrences.CommonPathsStorage.Collection.RecentDataDirectories.Remove(null);
            if (Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces != null) Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces.Remove(null);
            SaveFile();
        }
        public static void RemoveSavedPlace(string toRemove)
        {
            if (Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces.Contains(toRemove))
            {
                Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces.Remove(toRemove);
            }
            SaveFile();
        }
        public static void RemoveRecentDataFolder(string toRemove)
        {
            if (Classes.Prefrences.CommonPathsStorage.Collection.RecentDataDirectories.Contains(toRemove))
            {
                Classes.Prefrences.CommonPathsStorage.Collection.RecentDataDirectories.Remove(toRemove);
            }
            SaveFile();
        }
        public static void RemoveSavedDataFolder(string toRemove)
        {
            if (Classes.Prefrences.CommonPathsStorage.Collection.SavedDataDirectories.Contains(toRemove))
            {
                Classes.Prefrences.CommonPathsStorage.Collection.SavedDataDirectories.Remove(toRemove);
            }
            SaveFile();
        }
        public static void RemoveAllSavedPlaces()
        {
            if (Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces != null)
            {
                Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces.Clear();
            }
            SaveFile();
        }
        public static void RemoveAllDataFolders()
        {
            if (Classes.Prefrences.CommonPathsStorage.Collection.RecentDataDirectories != null)
            {
                Classes.Prefrences.CommonPathsStorage.Collection.RecentDataDirectories.Clear();
            }
            SaveFile();
        }
        public static void AddANewSavedPlace(string savedFolder)
        {
            try
            {
                var mySettings = Classes.Prefrences.CommonPathsStorage.Collection;
                var savedPlaces = mySettings.SavedPlaces;

                if (savedPlaces == null)
                {
                    savedPlaces = new StringCollection();
                    mySettings.SavedPlaces = savedPlaces;
                }

                if (savedPlaces.Contains(savedFolder))
                {
                    savedPlaces.Remove(savedFolder);
                }

                savedPlaces.Insert(0, savedFolder);

                Classes.Prefrences.CommonPathsStorage.SaveFile();
            }
            catch (Exception ex)
            {
                ManiacEditor.Extensions.ConsoleExtensions.Print("Failed to add Saved Place to list: " + ex);
            }
        }
        public static void AddSavedDataFolder(string dataDirectory)
        {
            try
            {
                if (Collection.SavedDataDirectories == null) Collection.SavedDataDirectories = new StringCollection();
                var dataDirectories = Collection.SavedDataDirectories;

                if (dataDirectories == null)
                {
                    dataDirectories = new StringCollection();
                    Collection.SavedDataDirectories = dataDirectories;
                }

                if (dataDirectories.Contains(dataDirectory)) dataDirectories.Remove(dataDirectory);

                dataDirectories.Insert(0, dataDirectory);

                SaveFile();
            }
            catch (Exception ex)
            {
                Debug.Write("Failed to add data folder to saved list: " + ex);
            }
        }
        public static void AddRecentDataFolder(string dataDirectory)
        {
            if (dataDirectory == string.Empty) return;
            try
            {
                if (Collection.RecentDataDirectories == null) Collection.RecentDataDirectories = new StringCollection();
                var dataDirectories = Collection.RecentDataDirectories;

                if (dataDirectories == null)
                {
                    dataDirectories = new StringCollection();
                    Collection.RecentDataDirectories = dataDirectories;
                }

                if (dataDirectories.Contains(dataDirectory)) dataDirectories.Remove(dataDirectory);

                if (dataDirectories.Count >= 10)
                {
                    for (int i = 9; i < dataDirectories.Count; i++) dataDirectories.RemoveAt(i);
                }

                dataDirectories.Insert(0, dataDirectory);

                SaveFile();
            }
            catch (Exception ex)
            {
                Debug.Write("Failed to add data folder to recent list: " + ex);
            }
        }

        #endregion
    }
}
