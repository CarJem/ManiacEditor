using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Methods
{
    public static class ProgramPaths
    {
        #region Setting Paths
        public static string GetSettingsDirectory()
        {
            return (Properties.Settings.MyInternalSettings.PortableMode ? SettingsPortableDirectory : SettingsStaticDirectory);
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
                if (Properties.Settings.MyInternalSettings.PortableMode)
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
                if (Properties.Settings.MyInternalSettings.PortableMode)
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
                if (Properties.Settings.MyInternalSettings.PortableMode)
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
                if (Properties.Settings.MyInternalSettings.PortableMode)
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
                if (Properties.Settings.MyInternalSettings.PortableMode)
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
                if (Properties.Settings.MyInternalSettings.PortableMode)
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

        #region General Paths
        public static string GetAppDataFolder()
        {
            if (Properties.Settings.MyInternalSettings.PortableMode)
            {
                return GetExecutingDirectoryName();
            }
            else
            {
                return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ManiacEditor");
            }
        }
        public static string GetLoggingFolder
        {
            get
            {
                if (Properties.Settings.MyInternalSettings.PortableMode)
                {
                    string path = System.IO.Path.Combine(GetExecutingDirectoryName(), "Logs");
                    if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
                    return path;
                }
                else
                {
                    string path = System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ManiacEditor"), "Logs");
                    if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
                    return path;
                }
            }

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
        public static string GetExecutingDirectoryName()
        {
            string exeLocationUrl = System.Reflection.Assembly.GetEntryAssembly().GetName().CodeBase;
            string exeLocation = new Uri(exeLocationUrl).LocalPath;
            return new System.IO.FileInfo(exeLocation).Directory.FullName;
        }

        #endregion
    }
}
