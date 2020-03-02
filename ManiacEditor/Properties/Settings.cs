using ManiacEditor.Classes.Options;
namespace ManiacEditor.Properties
{
    public static class Settings
    {
        public static GeneralSettings MySettings { get => Classes.Options.GeneralSettings.Default; }
        public static DefaultPrefrences MyDefaults { get => Classes.Options.DefaultPrefrences.Default; }
        public static DevelopmentStates MyDevSettings { get => Classes.Options.DevelopmentStates.Default; }
        public static VideoConfiguration MyPerformance { get => Classes.Options.VideoConfiguration.Default; }
        public static GameplaySettings MyGameOptions { get => Classes.Options.GameplaySettings.Default; }
        public static InputPreferences MyKeyBinds { get => Classes.Options.InputPreferences.Default; }

        public static DefaultInputPreferences MyDefaultKeyBinds;
        public static InternalSwitches MyInternalSettings { get => Classes.Options.InternalSwitches.Default; }

        public static void Init()
        {
            Classes.Options.InternalSwitches.Init();
            Classes.Options.GeneralSettings.Init();
            Classes.Options.DefaultPrefrences.Init();
            Classes.Options.DevelopmentStates.Init();
            Classes.Options.VideoConfiguration.Init();
            Classes.Options.GameplaySettings.Init();
            Classes.Options.InputPreferences.Init();
            MyDefaultKeyBinds = new DefaultInputPreferences();
        }

        public static void UpgradeAllSettings()
        {
            Classes.Options.VideoConfiguration.Upgrade();
            Classes.Options.DefaultPrefrences.Upgrade();
            Classes.Options.GameplaySettings.Upgrade();
            Classes.Options.DevelopmentStates.Upgrade();
            Classes.Options.InputPreferences.Upgrade();
            Classes.Options.GeneralSettings.Upgrade();
            Classes.Options.InternalSwitches.Upgrade();
        }

        public static void SaveAllSettings()
        {
            Classes.Options.VideoConfiguration.Save();
            Classes.Options.DefaultPrefrences.Save();
            Classes.Options.GameplaySettings.Save();
            Classes.Options.DevelopmentStates.Save();
            Classes.Options.InputPreferences.Save();
            Classes.Options.GeneralSettings.Save();
            Classes.Options.InternalSwitches.Save();
        }

        public static void ReloadAllSettings()
        {
            Classes.Options.VideoConfiguration.Reload();
            Classes.Options.DefaultPrefrences.Reload();
            Classes.Options.GameplaySettings.Reload();
            Classes.Options.DevelopmentStates.Reload();
            Classes.Options.InputPreferences.Reload();
            Classes.Options.GeneralSettings.Reload();
            Classes.Options.InternalSwitches.Reload();
        }

        public static void ResetAllSettings()
        {
            Classes.Options.VideoConfiguration.Reset();
            Classes.Options.DefaultPrefrences.Reset();
            Classes.Options.GameplaySettings.Reset();
            Classes.Options.DevelopmentStates.Reset();
            Classes.Options.InputPreferences.Reset();
            Classes.Options.GeneralSettings.Reset();
            Classes.Options.InternalSwitches.Reset();
        }
    }

}
