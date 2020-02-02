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
        public static string SettingsStaticDirectory { get => GetStaticSettingsDirectiory(); }
        public static string SettingsPortableDirectory { get => GetPortableSettingsDirectoryName(); }

        private static string GetStaticSettingsDirectiory()
        {
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ManiacEditor Config");
        }

        public static void UpgradeAllSettings()
        {
            Core.Settings.MyPerformance.Upgrade();
            Core.Settings.MyDefaults.Upgrade();
            Core.Settings.MyGameOptions.Upgrade();
            Core.Settings.MyDevSettings.Upgrade();
            Core.Settings.MyKeyBinds.Upgrade();
            Core.Settings.MySettings.Upgrade();
            Properties.Internal.Default.Upgrade();
        }

        public static void SaveAllSettings()
        {
            Core.Settings.MyDefaults.Save();
            Core.Settings.MyDevSettings.Save();
            Core.Settings.MyGameOptions.Save();
            Core.Settings.MyKeyBinds.Save();
            Core.Settings.MyPerformance.Save();
            Core.Settings.MySettings.Save();
            Properties.Internal.Default.Save();
        }

        public static void ReloadAllSettings()
        {
            Core.Settings.MyDefaults.Reload();
            Core.Settings.MyDevSettings.Reload();
            Core.Settings.MyGameOptions.Reload();
            Core.Settings.MyPerformance.Reload();
            Core.Settings.MyKeyBinds.Reload();
            Core.Settings.MySettings.Reload();
            Properties.Internal.Default.Reload();
        }

        public static void ResetAllSettings()
        {
            Core.Settings.MyDefaults.Reset();
            Core.Settings.MyDevSettings.Reset();
            Core.Settings.MyGameOptions.Reset();
            Core.Settings.MyPerformance.Reset();
            Core.Settings.MyKeyBinds.Reset();
            Core.Settings.MySettings.Reset();
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
