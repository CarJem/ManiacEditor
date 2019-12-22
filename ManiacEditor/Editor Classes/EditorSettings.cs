using System;
using System.Diagnostics;


namespace ManiacEditor
{
    [Serializable]
    public class EditorSettings
    {
        private Editor Instance;
        public EditorSettings(Editor instance)
        {
            Instance = instance;
        }

        public static void exportSettings()
        {

        }
        public static void importSettings()
        {

        }

        #region Preset Section
        public static void ApplyPreset(int state)
        {
            switch (state)
            {
                case 0:
                    ApplyMinimalPreset();
                    break;
                case 1:
                    ApplyBasicPreset();
                    break;
                case 2:
                    ApplySuperPreset();
                    break;
                case 3:
                    ApplyHyperPreset();
                    break;
            }
        }
        public static void ApplyPreset(string state)
        {
            switch (state)
            {
                case "Minimal":
                    ApplyMinimalPreset();
                    break;
                case "Basic":
                    ApplyBasicPreset();
                    break;
                case "Super":
                    ApplySuperPreset();
                    break;
                case "Hyper":
                    ApplyHyperPreset();
                    break;
            }
        }
        public static void ApplyMinimalPreset()
        {
            Settings.MyPerformance.ShowEditLayerBackground = false;
            Settings.MyPerformance.UseSimplifedWaterRendering = true;
            Settings.MyPerformance.DisableRendererExclusions = true;
            Settings.MyPerformance.NeverLoadEntityTextures = true;
        }
        public static void ApplyBasicPreset()
        {
            Settings.MyPerformance.ShowEditLayerBackground = true;
            Settings.MyPerformance.UseSimplifedWaterRendering = true;
            Settings.MyPerformance.DisableRendererExclusions = true;
            Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static void ApplySuperPreset()
        {
            Settings.MyPerformance.ShowEditLayerBackground = true;
            Settings.MyPerformance.UseSimplifedWaterRendering = false;
            Settings.MyPerformance.DisableRendererExclusions = true;
            Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static void ApplyHyperPreset()
        {
            Settings.MyPerformance.ShowEditLayerBackground = true;
            Settings.MyPerformance.UseSimplifedWaterRendering = false;
            Settings.MyPerformance.DisableRendererExclusions = false;
            Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static bool isMinimalPreset()
        {
            bool isMinimal = false;
            if (Settings.MyPerformance.ShowEditLayerBackground == false)
                    if (Settings.MyPerformance.UseSimplifedWaterRendering == true)
                        if (Settings.MyPerformance.DisableRendererExclusions == true)
                            if (Settings.MyPerformance.NeverLoadEntityTextures == true)
                                isMinimal = true;
            return isMinimal;

        }
        public static bool isBasicPreset()
        {
            bool isBasic = false;
                if (Settings.MyPerformance.ShowEditLayerBackground == true)
                        if (Settings.MyPerformance.UseSimplifedWaterRendering == true)
                            if (Settings.MyPerformance.DisableRendererExclusions == true)
                                if (Settings.MyPerformance.NeverLoadEntityTextures == false)
                                    isBasic = true;
            return isBasic;
        }
        public static bool isSuperPreset()
        {
            bool isSuper = false;
                if (Settings.MyPerformance.ShowEditLayerBackground == true)
                        if (Settings.MyPerformance.UseSimplifedWaterRendering == false)
                            if (Settings.MyPerformance.DisableRendererExclusions == true)
                                if (Settings.MyPerformance.NeverLoadEntityTextures == false)
                                    isSuper = true;
            return isSuper;
        }
        public static bool isHyperPreset()
        {
            bool isHyper = false;
            if (Settings.MyPerformance.ShowEditLayerBackground == true)
                if (Settings.MyPerformance.UseSimplifedWaterRendering == false)
                    if (Settings.MyPerformance.DisableRendererExclusions == false)
                        if (Settings.MyPerformance.NeverLoadEntityTextures == false)
                            isHyper = true;
            return isHyper;
        }
        #endregion

        public void TryLoadSettings()
        {
            try
            {
                if (Settings.MySettings.UpgradeRequired)
                {
                    EditorConstants.UpgradeAllSettings();
                    Settings.MySettings.UpgradeRequired = false;
                    EditorConstants.SaveAllSettings();
                }

                Instance.WindowState = Settings.MySettings.IsMaximized ? System.Windows.WindowState.Maximized : Instance.WindowState;
                Instance.InGame.GamePath = Settings.MyDefaults.SonicManiaPath;

                if (Settings.MySettings.ModLoaderConfigs?.Count > 0)
                {
                    Instance.selectConfigToolStripMenuItem.Items.Clear();
                    for (int i = 0; i < Settings.MySettings.ModLoaderConfigs.Count; i++)
                    {
                        Instance.selectConfigToolStripMenuItem.Items.Add(Instance.CreateModConfigMenuItem(i));

                    }
                }
                Editor.Instance.Defaulter.ApplyDefaults();
            }
            catch (Exception ex)
            {
                Debug.Write("Failed to load settings: " + ex);
            }
        }

        public void UseDefaultPrefrences()
        {
            //These Prefrences are applied on Stage Load

            //Default Layer Visibility Preferences
            if (!Settings.MyDefaults.FGLowerDefault) Instance.ShowFGLower.IsChecked = false;
            else Instance.ShowFGLower.IsChecked = true;
            if (!Settings.MyDefaults.FGLowDefault) Instance.ShowFGLow.IsChecked = false;
            else Instance.ShowFGLow.IsChecked = true;
            if (!Settings.MyDefaults.FGHighDefault) Instance.ShowFGHigh.IsChecked = false;
            else Instance.ShowFGHigh.IsChecked = true;
            if (!Settings.MyDefaults.FGHigherDefault) Instance.ShowFGHigher.IsChecked = false;
            else Instance.ShowFGHigher.IsChecked = true;
            if (!Settings.MyDefaults.EntitiesDefault) Instance.ShowEntities.IsChecked = false;
            else Instance.ShowEntities.IsChecked = true;
            Instance.ShowAnimations.IsChecked = Settings.MyDefaults.AnimationsDefault;
            Instance.Options.AnimationsEnabled = Settings.MyDefaults.AnimationsDefault;


            //Default Enabled Annimation Preferences
            Instance.movingPlatformsObjectsToolStripMenuItem.IsChecked = Settings.MyDefaults.PlatformAnimationsDefault;
            Instance.Options.MovingPlatformsChecked = Settings.MyDefaults.PlatformAnimationsDefault;

            Instance.spriteFramesToolStripMenuItem.IsChecked = Settings.MyDefaults.SpriteAnimationsDefault;
            Instance.Options.SpriteAnimationsChecked = Settings.MyDefaults.SpriteAnimationsDefault;


            //TO DO: Add Default For this.
            Instance.parallaxAnimationMenuItem.IsChecked = false;
            Instance.Options.ParallaxAnimationChecked = false;

            Instance.Options.waterColor = Settings.MyDefaults.WaterEntityColorDefault;




            //Default Grid Preferences
            if (Settings.MyDefaults.DefaultGridSizeOption == 0) Instance.Grid16x16SizeMenuItem.IsChecked = true;
            else Instance.Grid16x16SizeMenuItem.IsChecked = false;
            if (Settings.MyDefaults.DefaultGridSizeOption == 1) Instance.Grid128x128SizeMenuItem.IsChecked = true;
            else Instance.Grid128x128SizeMenuItem.IsChecked = false;
            if (Settings.MyDefaults.DefaultGridSizeOption == 2) Instance.Grid256x256SizeMenuItem.IsChecked = true;
            else Instance.Grid256x256SizeMenuItem.IsChecked = false;
            if (Settings.MyDefaults.DefaultGridSizeOption == 3) Instance.GridCustomSizeMenuItem.IsChecked = true;
            else Instance.GridCustomSizeMenuItem.IsChecked = false;

            //Collision Color Presets
            Instance.defaultToolStripMenuItem.IsChecked = Settings.MyDefaults.DefaultCollisionColors == 0;
            Instance.invertedToolStripMenuItem.IsChecked = Settings.MyDefaults.DefaultCollisionColors == 1;
            Instance.customToolStripMenuItem1.IsChecked = Settings.MyDefaults.DefaultCollisionColors == 2;
            Instance.Options.CollisionPreset = Settings.MyDefaults.DefaultCollisionColors;
            Instance.RefreshCollisionColours();

            if (Settings.MyDefaults.ScrollLockDirectionDefault == false)
            {
                Instance.Options.ScrollDirection = (int)ScrollDir.X;
                Instance.UI.UpdateStatusPanel();

            }
            else
            {
                Instance.Options.ScrollDirection = (int)ScrollDir.Y;
                Instance.UI.UpdateStatusPanel();
            }

        }
    }

}
