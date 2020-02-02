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
            ManiacEditor.Core.Settings.MyPerformance.Upgrade();
            ManiacEditor.Core.Settings.MyDefaults.Upgrade();
            ManiacEditor.Core.Settings.MyGameOptions.Upgrade();
            ManiacEditor.Core.Settings.MyDevSettings.Upgrade();
            ManiacEditor.Core.Settings.MyKeyBinds.Upgrade();
            ManiacEditor.Core.Settings.MySettings.Upgrade();
            Properties.Internal.Default.Upgrade();
        }

        public static void SaveAllSettings()
        {
            ManiacEditor.Core.Settings.MyDefaults.Save();
            ManiacEditor.Core.Settings.MyDevSettings.Save();
            ManiacEditor.Core.Settings.MyGameOptions.Save();
            ManiacEditor.Core.Settings.MyKeyBinds.Save();
            ManiacEditor.Core.Settings.MyPerformance.Save();
            ManiacEditor.Core.Settings.MySettings.Save();
            Properties.Internal.Default.Save();
        }

        public static void ReloadAllSettings()
        {
            ManiacEditor.Core.Settings.MyDefaults.Reload();
            ManiacEditor.Core.Settings.MyDevSettings.Reload();
            ManiacEditor.Core.Settings.MyGameOptions.Reload();
            ManiacEditor.Core.Settings.MyPerformance.Reload();
            ManiacEditor.Core.Settings.MyKeyBinds.Reload();
            ManiacEditor.Core.Settings.MySettings.Reload();
            Properties.Internal.Default.Reload();
        }

        public static void ResetAllSettings()
        {
            ManiacEditor.Core.Settings.MyDefaults.Reset();
            ManiacEditor.Core.Settings.MyDevSettings.Reset();
            ManiacEditor.Core.Settings.MyGameOptions.Reset();
            ManiacEditor.Core.Settings.MyPerformance.Reset();
            ManiacEditor.Core.Settings.MyKeyBinds.Reset();
            ManiacEditor.Core.Settings.MySettings.Reset();
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
