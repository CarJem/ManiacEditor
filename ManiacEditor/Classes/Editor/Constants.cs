using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Classes.Editor
{
    public static class Constants
    {
        public const int ENTITY_NAME_BOX_WIDTH = 20;
        public const int ENTITY_NAME_BOX_HEIGHT = 20;
        public const int ENTITY_NAME_BOX_HALF_WIDTH = ENTITY_NAME_BOX_WIDTH / 2;
        public const int ENTITY_NAME_BOX_HALF_HEIGHT = ENTITY_NAME_BOX_HEIGHT / 2;
        public const int TILES_CHUNK_SIZE = 16;
        public const int TILE_SIZE = 16;
        public const int BOX_SIZE = 8;
        public const int TILE_BOX_SIZE = 1;
        public const int x128_CHUNK_SIZE = 128;

        #region Folder/File Paths

        #region Settings Files/Folders

        public static string GetSettingsDirectory()
        {
            return (Methods.Settings.MyInternalSettings.PortableMode ? Classes.Editor.Constants.SettingsPortableDirectory : Classes.Editor.Constants.SettingsStaticDirectory);
        }

        public static string SettingsStaticDirectory 
        { 
            get 
            {
                string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ManiacEditor", "Settings");
                //string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ManiacEditor Config");
                if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
                return path;
            } 
        }
        public static string SettingsPortableDirectory
        {
            get
            {
                string path = System.IO.Path.Combine(GetExecutingDirectoryName(), "Settings");
                if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
                return path;
            }
        }


        public static string DefaultPrefrencesFilePath
        {
            get
            {
                if (Methods.Settings.MyInternalSettings.PortableMode)
                {
                    return System.IO.Path.Combine(SettingsPortableDirectory, "defaults.json");
                }
                else
                {
                    return System.IO.Path.Combine(SettingsStaticDirectory, "defaults.json");
                }
            }
        }
        public static string DevelopmentStatesFilePath
        {
            get
            {
                if (Methods.Settings.MyInternalSettings.PortableMode)
                {
                    return System.IO.Path.Combine(SettingsPortableDirectory, "dev_settings.json");
                }
                else
                {
                    return System.IO.Path.Combine(SettingsStaticDirectory, "dev_settings.json");
                }
            }
        }
        public static string GameplaySettingsFilePath
        {
            get
            {
                if (Methods.Settings.MyInternalSettings.PortableMode)
                {
                    return System.IO.Path.Combine(SettingsPortableDirectory, "gameplay_options.json");
                }
                else
                {
                    return System.IO.Path.Combine(SettingsStaticDirectory, "gameplay_options.json");
                }
            }
        }
        public static string GeneralSettingsFilePath
        {
            get
            {
                if (Methods.Settings.MyInternalSettings.PortableMode)
                {
                    return System.IO.Path.Combine(SettingsPortableDirectory, "general_settings.json");
                }
                else
                {
                    return System.IO.Path.Combine(SettingsStaticDirectory, "general_settings.json");
                }
            }
        }
        public static string InputPreferencesFilePath
        {
            get
            {
                if (Methods.Settings.MyInternalSettings.PortableMode)
                {
                    return System.IO.Path.Combine(SettingsPortableDirectory, "input_mappings.json");
                }
                else
                {
                    return System.IO.Path.Combine(SettingsStaticDirectory, "input_mappings.json");
                }
            }
        }
        public static string VideoConfigurationFilePath
        {
            get
            {
                if (Methods.Settings.MyInternalSettings.PortableMode)
                {
                    return System.IO.Path.Combine(SettingsPortableDirectory, "video_settings.json");
                }
                else
                {
                    return System.IO.Path.Combine(SettingsStaticDirectory, "video_settings.json");
                }
            }
        }
        public static string InternalSwitchesFilePath
        {
            get
            {
                return System.IO.Path.Combine(SettingsPortableDirectory, "internal_switches.json");
            }
        } 
        #endregion


        public static string GetAppDataFolder()
        {
            if (Methods.Settings.MyInternalSettings.PortableMode)
            {
                return GetExecutingDirectoryName();
            }
            else
            {
                return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ManiacEditor");
            }
        }

        public static string GetLoggingFolder()
        {
            string path = System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ManiacEditor"), "Logs");
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
            return path;
        }
        public static string DownloadRequestsFolder
        {
            get
            {
                string path = System.IO.Path.Combine(GetAppDataFolder(), "UpdateRequests");
                if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
                return path;
            }
        }
        private static string GetExecutingDirectoryName()
        {
            string exeLocationUrl = System.Reflection.Assembly.GetEntryAssembly().GetName().CodeBase;
            string exeLocation = new Uri(exeLocationUrl).LocalPath;
            return new System.IO.FileInfo(exeLocation).Directory.FullName;
        }

        #endregion
    }
}
