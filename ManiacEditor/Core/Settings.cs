
using ManiacEditor.Core.Options;
namespace ManiacEditor.Core
{
    public static class Settings
    {
        public static GeneralSettings MySettings { get => Core.Options.GeneralSettings.Default; }
        public static DefaultPrefrences MyDefaults { get => Core.Options.DefaultPrefrences.Default; }
        public static DevelopmentStates MyDevSettings { get => Core.Options.DevelopmentStates.Default; }
        public static VideoConfiguration MyPerformance { get => Core.Options.VideoConfiguration.Default; }
        public static GameplaySettings MyGameOptions { get => Core.Options.GameplaySettings.Default; }
        public static InputPreferences MyKeyBinds { get => Core.Options.InputPreferences.Default; }

        public static DefaultInputPreferences MyDefaultKeyBinds;
        public static InternalSwitches MyInternalSettings { get => Core.Options.InternalSwitches.Default; }

        public static void Init()
        {
            Core.Options.InternalSwitches.Init();
            Core.Options.GeneralSettings.Init();
            Core.Options.DefaultPrefrences.Init();
            Core.Options.DevelopmentStates.Init();
            Core.Options.VideoConfiguration.Init();
            Core.Options.GameplaySettings.Init();
            Core.Options.InputPreferences.Init();
            MyDefaultKeyBinds = new DefaultInputPreferences();
        }

        public static void UpgradeAllSettings()
        {
            Core.Options.VideoConfiguration.Upgrade();
            Core.Options.DefaultPrefrences.Upgrade();
            Core.Options.GameplaySettings.Upgrade();
            Core.Options.DevelopmentStates.Upgrade();
            Core.Options.InputPreferences.Upgrade();
            Core.Options.GeneralSettings.Upgrade();
            Core.Options.InternalSwitches.Upgrade();
        }

        public static void SaveAllSettings()
        {
            Core.Options.VideoConfiguration.Save();
            Core.Options.DefaultPrefrences.Save();
            Core.Options.GameplaySettings.Save();
            Core.Options.DevelopmentStates.Save();
            Core.Options.InputPreferences.Save();
            Core.Options.GeneralSettings.Save();
            Core.Options.InternalSwitches.Save();
        }

        public static void ReloadAllSettings()
        {
            Core.Options.VideoConfiguration.Reload();
            Core.Options.DefaultPrefrences.Reload();
            Core.Options.GameplaySettings.Reload();
            Core.Options.DevelopmentStates.Reload();
            Core.Options.InputPreferences.Reload();
            Core.Options.GeneralSettings.Reload();
            Core.Options.InternalSwitches.Reload();
        }

        public static void ResetAllSettings()
        {
            Core.Options.VideoConfiguration.Reset();
            Core.Options.DefaultPrefrences.Reset();
            Core.Options.GameplaySettings.Reset();
            Core.Options.DevelopmentStates.Reset();
            Core.Options.InputPreferences.Reset();
            Core.Options.GeneralSettings.Reset();
            Core.Options.InternalSwitches.Reset();
        }
    }

}
