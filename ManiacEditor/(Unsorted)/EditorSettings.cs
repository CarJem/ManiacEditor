using System;
using System.Diagnostics;
using ManiacEditor.Enums;


namespace ManiacEditor
{
    [Serializable]
    public class EditorSettings
    {
        private Controls.Base.MainEditor Instance;
        public EditorSettings(Controls.Base.MainEditor instance)
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
            Core.Settings.MyPerformance.ShowEditLayerBackground = false;
            Core.Settings.MyPerformance.UseSimplifedWaterRendering = true;
            Core.Settings.MyPerformance.DisableRendererExclusions = true;
            Core.Settings.MyPerformance.NeverLoadEntityTextures = true;
        }
        public static void ApplyBasicPreset()
        {
            Core.Settings.MyPerformance.ShowEditLayerBackground = true;
            Core.Settings.MyPerformance.UseSimplifedWaterRendering = true;
            Core.Settings.MyPerformance.DisableRendererExclusions = true;
            Core.Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static void ApplySuperPreset()
        {
            Core.Settings.MyPerformance.ShowEditLayerBackground = true;
            Core.Settings.MyPerformance.UseSimplifedWaterRendering = false;
            Core.Settings.MyPerformance.DisableRendererExclusions = true;
            Core.Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static void ApplyHyperPreset()
        {
            Core.Settings.MyPerformance.ShowEditLayerBackground = true;
            Core.Settings.MyPerformance.UseSimplifedWaterRendering = false;
            Core.Settings.MyPerformance.DisableRendererExclusions = false;
            Core.Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static bool isMinimalPreset()
        {
            bool isMinimal = false;
            if (Core.Settings.MyPerformance.ShowEditLayerBackground == false)
                    if (Core.Settings.MyPerformance.UseSimplifedWaterRendering == true)
                        if (Core.Settings.MyPerformance.DisableRendererExclusions == true)
                            if (Core.Settings.MyPerformance.NeverLoadEntityTextures == true)
                                isMinimal = true;
            return isMinimal;

        }
        public static bool isBasicPreset()
        {
            bool isBasic = false;
                if (Core.Settings.MyPerformance.ShowEditLayerBackground == true)
                        if (Core.Settings.MyPerformance.UseSimplifedWaterRendering == true)
                            if (Core.Settings.MyPerformance.DisableRendererExclusions == true)
                                if (Core.Settings.MyPerformance.NeverLoadEntityTextures == false)
                                    isBasic = true;
            return isBasic;
        }
        public static bool isSuperPreset()
        {
            bool isSuper = false;
                if (Core.Settings.MyPerformance.ShowEditLayerBackground == true)
                        if (Core.Settings.MyPerformance.UseSimplifedWaterRendering == false)
                            if (Core.Settings.MyPerformance.DisableRendererExclusions == true)
                                if (Core.Settings.MyPerformance.NeverLoadEntityTextures == false)
                                    isSuper = true;
            return isSuper;
        }
        public static bool isHyperPreset()
        {
            bool isHyper = false;
            if (Core.Settings.MyPerformance.ShowEditLayerBackground == true)
                if (Core.Settings.MyPerformance.UseSimplifedWaterRendering == false)
                    if (Core.Settings.MyPerformance.DisableRendererExclusions == false)
                        if (Core.Settings.MyPerformance.NeverLoadEntityTextures == false)
                            isHyper = true;
            return isHyper;
        }
        #endregion

        public void TryLoadSettings()
        {
            try
            {
                if (Core.Settings.MySettings.UpgradeRequired)
                {
                    Classes.Core.Constants.UpgradeAllSettings();
                    Core.Settings.MySettings.UpgradeRequired = false;
                    Classes.Core.Constants.SaveAllSettings();
                }

                Instance.WindowState = Core.Settings.MySettings.IsMaximized ? System.Windows.WindowState.Maximized : Instance.WindowState;
                Instance.InGame.GamePath = Core.Settings.MyDefaults.SonicManiaPath;

                if (Core.Settings.MySettings.ModLoaderConfigs?.Count > 0)
                {
                    Instance.EditorToolbar.selectConfigToolStripMenuItem.Items.Clear();
                    for (int i = 0; i < Core.Settings.MySettings.ModLoaderConfigs.Count; i++)
                    {
                        Instance.EditorToolbar.selectConfigToolStripMenuItem.Items.Add(Instance.CreateModConfigMenuItem(i));

                    }
                }
                Controls.Base.MainEditor.Instance.Defaulter.ApplyDefaults();
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
            if (!Core.Settings.MyDefaults.FGLowerDefault) Instance.EditorToolbar.ShowFGLower.IsChecked = false;
            else Instance.EditorToolbar.ShowFGLower.IsChecked = true;
            if (!Core.Settings.MyDefaults.FGLowDefault) Instance.EditorToolbar.ShowFGLow.IsChecked = false;
            else Instance.EditorToolbar.ShowFGLow.IsChecked = true;
            if (!Core.Settings.MyDefaults.FGHighDefault) Instance.EditorToolbar.ShowFGHigh.IsChecked = false;
            else Instance.EditorToolbar.ShowFGHigh.IsChecked = true;
            if (!Core.Settings.MyDefaults.FGHigherDefault) Instance.EditorToolbar.ShowFGHigher.IsChecked = false;
            else Instance.EditorToolbar.ShowFGHigher.IsChecked = true;
            if (!Core.Settings.MyDefaults.EntitiesDefault) Instance.EditorToolbar.ShowEntities.IsChecked = false;
            else Instance.EditorToolbar.ShowEntities.IsChecked = true;
            Instance.EditorToolbar.ShowAnimations.IsChecked = Core.Settings.MyDefaults.AnimationsDefault;
            Classes.Core.SolutionState.AllowAnimations = Core.Settings.MyDefaults.AnimationsDefault;


            //Default Enabled Annimation Preferences
            Instance.EditorToolbar.movingPlatformsObjectsToolStripMenuItem.IsChecked = Core.Settings.MyDefaults.PlatformAnimationsDefault;
            Classes.Core.SolutionState.AllowMovingPlatformAnimations = Core.Settings.MyDefaults.PlatformAnimationsDefault;

            Instance.EditorToolbar.spriteFramesToolStripMenuItem.IsChecked = Core.Settings.MyDefaults.SpriteAnimationsDefault;
            Classes.Core.SolutionState.AllowSpriteAnimations = Core.Settings.MyDefaults.SpriteAnimationsDefault;


            //TO DO: Add Default For this.
            Instance.EditorToolbar.parallaxAnimationMenuItem.IsChecked = false;
            Classes.Core.SolutionState.ParallaxAnimationChecked = false;

            Classes.Core.SolutionState.waterColor = Core.Settings.MyDefaults.WaterEntityColorDefault;




            //Default Grid Preferences
            if (Core.Settings.MyDefaults.DefaultGridSizeOption == 0) Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = true;
            else Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = false;
            if (Core.Settings.MyDefaults.DefaultGridSizeOption == 1) Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = true;
            else Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = false;
            if (Core.Settings.MyDefaults.DefaultGridSizeOption == 2) Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = true;
            else Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = false;
            if (Core.Settings.MyDefaults.DefaultGridSizeOption == 3) Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = true;
            else Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = false;

            //Collision Color Presets
            Instance.EditorToolbar.defaultToolStripMenuItem.IsChecked = Core.Settings.MyDefaults.DefaultCollisionColors == 0;
            Instance.EditorToolbar.invertedToolStripMenuItem.IsChecked = Core.Settings.MyDefaults.DefaultCollisionColors == 1;
            Instance.EditorToolbar.customToolStripMenuItem1.IsChecked = Core.Settings.MyDefaults.DefaultCollisionColors == 2;
            Classes.Core.SolutionState.CollisionPreset = Core.Settings.MyDefaults.DefaultCollisionColors;
            Instance.RefreshCollisionColours();

            if (Core.Settings.MyDefaults.ScrollLockDirectionDefault == false)
            {
                Classes.Core.SolutionState.ScrollDirection = (int)ScrollDir.X;
                Instance.EditorStatusBar.UpdateStatusPanel();

            }
            else
            {
                Classes.Core.SolutionState.ScrollDirection = (int)ScrollDir.Y;
                Instance.EditorStatusBar.UpdateStatusPanel();
            }

        }
    }

}
