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
        public static string SettingsStaticDirectory 
        { 
            get 
            {
                return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ManiacEditor Config");
            } 
        }
        public static string SettingsPortableDirectory
        {
            get
            {
                return System.IO.Path.Combine(GetExecutingDirectoryName(), "Settings");
            }
        }


        public static string DefaultPrefrencesFilePath
        {
            get
            {
                if (Core.Settings.MyInternalSettings.PortableMode)
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
                if (Core.Settings.MyInternalSettings.PortableMode)
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
                if (Core.Settings.MyInternalSettings.PortableMode)
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
                if (Core.Settings.MyInternalSettings.PortableMode)
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
                if (Core.Settings.MyInternalSettings.PortableMode)
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
                if (Core.Settings.MyInternalSettings.PortableMode)
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



        public static string GetLoggingFolder()
        {
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ManiacEditor", "logs");
        }
        public static string DownloadRequestsFolder
        {
            get
            {
                return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ManiacEditor", "UpdateRequests");
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
