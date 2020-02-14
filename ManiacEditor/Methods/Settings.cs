
using ManiacEditor.Methods.Options;
namespace ManiacEditor.Methods
{
    public static class Settings
    {
        public static GeneralSettings MySettings { get => Methods.Options.GeneralSettings.Default; }
        public static DefaultPrefrences MyDefaults { get => Methods.Options.DefaultPrefrences.Default; }
        public static DevelopmentStates MyDevSettings { get => Methods.Options.DevelopmentStates.Default; }
        public static VideoConfiguration MyPerformance { get => Methods.Options.VideoConfiguration.Default; }
        public static GameplaySettings MyGameOptions { get => Methods.Options.GameplaySettings.Default; }
        public static InputPreferences MyKeyBinds { get => Methods.Options.InputPreferences.Default; }

        public static DefaultInputPreferences MyDefaultKeyBinds;
        public static InternalSwitches MyInternalSettings { get => Methods.Options.InternalSwitches.Default; }

        public static void Init()
        {
            Methods.Options.InternalSwitches.Init();
            Methods.Options.GeneralSettings.Init();
            Methods.Options.DefaultPrefrences.Init();
            Methods.Options.DevelopmentStates.Init();
            Methods.Options.VideoConfiguration.Init();
            Methods.Options.GameplaySettings.Init();
            Methods.Options.InputPreferences.Init();
            MyDefaultKeyBinds = new DefaultInputPreferences();
        }

        public static void UpgradeAllSettings()
        {
            Methods.Options.VideoConfiguration.Upgrade();
            Methods.Options.DefaultPrefrences.Upgrade();
            Methods.Options.GameplaySettings.Upgrade();
            Methods.Options.DevelopmentStates.Upgrade();
            Methods.Options.InputPreferences.Upgrade();
            Methods.Options.GeneralSettings.Upgrade();
            Methods.Options.InternalSwitches.Upgrade();
        }

        public static void SaveAllSettings()
        {
            Methods.Options.VideoConfiguration.Save();
            Methods.Options.DefaultPrefrences.Save();
            Methods.Options.GameplaySettings.Save();
            Methods.Options.DevelopmentStates.Save();
            Methods.Options.InputPreferences.Save();
            Methods.Options.GeneralSettings.Save();
            Methods.Options.InternalSwitches.Save();
        }

        public static void ReloadAllSettings()
        {
            Methods.Options.VideoConfiguration.Reload();
            Methods.Options.DefaultPrefrences.Reload();
            Methods.Options.GameplaySettings.Reload();
            Methods.Options.DevelopmentStates.Reload();
            Methods.Options.InputPreferences.Reload();
            Methods.Options.GeneralSettings.Reload();
            Methods.Options.InternalSwitches.Reload();
        }

        public static void ResetAllSettings()
        {
            Methods.Options.VideoConfiguration.Reset();
            Methods.Options.DefaultPrefrences.Reset();
            Methods.Options.GameplaySettings.Reset();
            Methods.Options.DevelopmentStates.Reset();
            Methods.Options.InputPreferences.Reset();
            Methods.Options.GeneralSettings.Reset();
            Methods.Options.InternalSwitches.Reset();
        }
    }

}
