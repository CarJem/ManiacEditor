using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor
{
    public class EditorConstants
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
        public static string SettingsStaticDirectory { get => GetStaticSettingsDirectiory(); }
        public static string SettingsPortableDirectory { get => GetPortableSettingsDirectoryName(); }

        private static string GetStaticSettingsDirectiory()
        {
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ManiacEditor Config");
        }

        public static void UpgradeAllSettings()
        {
            Settings.MyPerformance.Upgrade();
            Settings.MyDefaults.Upgrade();
            Settings.MyGameOptions.Upgrade();
            Settings.MyDevSettings.Upgrade();
            Settings.MyKeyBinds.Upgrade();
            Settings.MySettings.Upgrade();
            Properties.Internal.Default.Upgrade();
        }

        public static void SaveAllSettings()
        {
            Settings.MyDefaults.Save();
            Settings.MyDevSettings.Save();
            Settings.MyGameOptions.Save();
            Settings.MyKeyBinds.Save();
            Settings.MyPerformance.Save();
            Settings.MySettings.Save();
            Properties.Internal.Default.Save();
        }

        public static void ReloadAllSettings()
        {
            Settings.MyDefaults.Reload();
            Settings.MyDevSettings.Reload();
            Settings.MyGameOptions.Reload();
            Settings.MyPerformance.Reload();
            Settings.MyKeyBinds.Reload();
            Settings.MySettings.Reload();
            Properties.Internal.Default.Reload();
        }

        public static void ResetAllSettings()
        {
            Settings.MyDefaults.Reset();
            Settings.MyDevSettings.Reset();
            Settings.MyGameOptions.Reset();
            Settings.MyPerformance.Reset();
            Settings.MyKeyBinds.Reset();
            Settings.MySettings.Reset();
        }

        private static string GetPortableSettingsDirectoryName()
        {
            return System.IO.Path.Combine(GetExecutingDirectoryName(), "Settings");
        }

        private static string GetExecutingDirectoryName()
        {
            string exeLocationUrl = System.Reflection.Assembly.GetEntryAssembly().GetName().CodeBase;
            string exeLocation = new Uri(exeLocationUrl).LocalPath;
            return new System.IO.FileInfo(exeLocation).Directory.FullName;
        }
    }
}
