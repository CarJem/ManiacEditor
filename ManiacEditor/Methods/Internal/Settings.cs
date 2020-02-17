using System;
using System.Diagnostics;
using ManiacEditor.Enums;
using System.Linq;

namespace ManiacEditor.Methods.Internal
{
    [Serializable]
    public class Settings
    {
        private static ManiacEditor.Controls.Editor.MainEditor Instance { get; set; }
        public static void UpdateInstance(ManiacEditor.Controls.Editor.MainEditor instance)
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
            ManiacEditor.Methods.Settings.MyPerformance.ShowEditLayerBackground = false;
            ManiacEditor.Methods.Settings.MyPerformance.UseSimplifedWaterRendering = true;
            ManiacEditor.Methods.Settings.MyPerformance.DisableRendererExclusions = true;
            ManiacEditor.Methods.Settings.MyPerformance.NeverLoadEntityTextures = true;
        }
        public static void ApplyBasicPreset()
        {
            ManiacEditor.Methods.Settings.MyPerformance.ShowEditLayerBackground = true;
            ManiacEditor.Methods.Settings.MyPerformance.UseSimplifedWaterRendering = true;
            ManiacEditor.Methods.Settings.MyPerformance.DisableRendererExclusions = true;
            ManiacEditor.Methods.Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static void ApplySuperPreset()
        {
            ManiacEditor.Methods.Settings.MyPerformance.ShowEditLayerBackground = true;
            ManiacEditor.Methods.Settings.MyPerformance.UseSimplifedWaterRendering = false;
            ManiacEditor.Methods.Settings.MyPerformance.DisableRendererExclusions = true;
            ManiacEditor.Methods.Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static void ApplyHyperPreset()
        {
            ManiacEditor.Methods.Settings.MyPerformance.ShowEditLayerBackground = true;
            ManiacEditor.Methods.Settings.MyPerformance.UseSimplifedWaterRendering = false;
            ManiacEditor.Methods.Settings.MyPerformance.DisableRendererExclusions = false;
            ManiacEditor.Methods.Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static bool isMinimalPreset()
        {
            bool isMinimal = false;
            if (ManiacEditor.Methods.Settings.MyPerformance.ShowEditLayerBackground == false)
                    if (ManiacEditor.Methods.Settings.MyPerformance.UseSimplifedWaterRendering == true)
                        if (ManiacEditor.Methods.Settings.MyPerformance.DisableRendererExclusions == true)
                            if (ManiacEditor.Methods.Settings.MyPerformance.NeverLoadEntityTextures == true)
                                isMinimal = true;
            return isMinimal;

        }
        public static bool isBasicPreset()
        {
            bool isBasic = false;
                if (ManiacEditor.Methods.Settings.MyPerformance.ShowEditLayerBackground == true)
                        if (ManiacEditor.Methods.Settings.MyPerformance.UseSimplifedWaterRendering == true)
                            if (ManiacEditor.Methods.Settings.MyPerformance.DisableRendererExclusions == true)
                                if (ManiacEditor.Methods.Settings.MyPerformance.NeverLoadEntityTextures == false)
                                    isBasic = true;
            return isBasic;
        }
        public static bool isSuperPreset()
        {
            bool isSuper = false;
                if (ManiacEditor.Methods.Settings.MyPerformance.ShowEditLayerBackground == true)
                        if (ManiacEditor.Methods.Settings.MyPerformance.UseSimplifedWaterRendering == false)
                            if (ManiacEditor.Methods.Settings.MyPerformance.DisableRendererExclusions == true)
                                if (ManiacEditor.Methods.Settings.MyPerformance.NeverLoadEntityTextures == false)
                                    isSuper = true;
            return isSuper;
        }
        public static bool isHyperPreset()
        {
            bool isHyper = false;
            if (ManiacEditor.Methods.Settings.MyPerformance.ShowEditLayerBackground == true)
                if (ManiacEditor.Methods.Settings.MyPerformance.UseSimplifedWaterRendering == false)
                    if (ManiacEditor.Methods.Settings.MyPerformance.DisableRendererExclusions == false)
                        if (ManiacEditor.Methods.Settings.MyPerformance.NeverLoadEntityTextures == false)
                            isHyper = true;
            return isHyper;
        }
        #endregion

        public static void TryLoadSettings()
        {
            try
            {
                if (ManiacEditor.Methods.Settings.MySettings.UpgradeRequired)
                {
                    Methods.Settings.UpgradeAllSettings();
                    ManiacEditor.Methods.Settings.MySettings.UpgradeRequired = false;
                    Methods.Settings.SaveAllSettings();
                }

                Instance.WindowState = ManiacEditor.Methods.Settings.MySettings.IsMaximized ? System.Windows.WindowState.Maximized : Instance.WindowState;
                Methods.Runtime.GameHandler.GamePath = ManiacEditor.Methods.Settings.MyDefaults.SonicManiaPath;

                if (ManiacEditor.Methods.Settings.MySettings.ModLoaderConfigs?.Count > 0)
                {
                    Instance.EditorToolbar.selectConfigToolStripMenuItem.Items.Clear();
                    for (int i = 0; i < ManiacEditor.Methods.Settings.MySettings.ModLoaderConfigs.Count; i++)
                    {
                        Instance.EditorToolbar.selectConfigToolStripMenuItem.Items.Add(Instance.CreateModConfigMenuItem(i));

                    }
                }
                ApplyDefaults();
            }
            catch (Exception ex)
            {
                Debug.Write("Failed to load settings: " + ex);
            }
        }

        #region Defaults Section

        public static void UseDefaultPrefrences()
        {
            //These Prefrences are applied on Stage Load

            //Default Layer Visibility Preferences
            if (!ManiacEditor.Methods.Settings.MyDefaults.FGLowerDefault) Instance.EditorToolbar.ShowFGLower.IsChecked = false;
            else Instance.EditorToolbar.ShowFGLower.IsChecked = true;
            if (!ManiacEditor.Methods.Settings.MyDefaults.FGLowDefault) Instance.EditorToolbar.ShowFGLow.IsChecked = false;
            else Instance.EditorToolbar.ShowFGLow.IsChecked = true;
            if (!ManiacEditor.Methods.Settings.MyDefaults.FGHighDefault) Instance.EditorToolbar.ShowFGHigh.IsChecked = false;
            else Instance.EditorToolbar.ShowFGHigh.IsChecked = true;
            if (!ManiacEditor.Methods.Settings.MyDefaults.FGHigherDefault) Instance.EditorToolbar.ShowFGHigher.IsChecked = false;
            else Instance.EditorToolbar.ShowFGHigher.IsChecked = true;
            if (!ManiacEditor.Methods.Settings.MyDefaults.EntitiesDefault) Instance.EditorToolbar.ShowEntities.IsChecked = false;
            else Instance.EditorToolbar.ShowEntities.IsChecked = true;
            Instance.EditorToolbar.ShowAnimations.IsChecked = ManiacEditor.Methods.Settings.MyDefaults.AnimationsDefault;
            Classes.Editor.SolutionState.AllowAnimations = ManiacEditor.Methods.Settings.MyDefaults.AnimationsDefault;


            //Default Enabled Annimation Preferences
            Instance.EditorToolbar.movingPlatformsObjectsToolStripMenuItem.IsChecked = ManiacEditor.Methods.Settings.MyDefaults.PlatformAnimationsDefault;
            Classes.Editor.SolutionState.AllowMovingPlatformAnimations = ManiacEditor.Methods.Settings.MyDefaults.PlatformAnimationsDefault;

            Instance.EditorToolbar.spriteFramesToolStripMenuItem.IsChecked = ManiacEditor.Methods.Settings.MyDefaults.SpriteAnimationsDefault;
            Classes.Editor.SolutionState.AllowSpriteAnimations = ManiacEditor.Methods.Settings.MyDefaults.SpriteAnimationsDefault;


            //TO DO: Add Default For this.
            Instance.EditorToolbar.parallaxAnimationMenuItem.IsChecked = false;
            Classes.Editor.SolutionState.ParallaxAnimationChecked = false;

            Classes.Editor.SolutionState.waterColor = ManiacEditor.Methods.Settings.MyDefaults.WaterEntityColorDefault;




            //Default Grid Preferences
            if (ManiacEditor.Methods.Settings.MyDefaults.DefaultGridSizeOption == 0) Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = true;
            else Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = false;
            if (ManiacEditor.Methods.Settings.MyDefaults.DefaultGridSizeOption == 1) Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = true;
            else Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = false;
            if (ManiacEditor.Methods.Settings.MyDefaults.DefaultGridSizeOption == 2) Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = true;
            else Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = false;
            if (ManiacEditor.Methods.Settings.MyDefaults.DefaultGridSizeOption == 3) Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = true;
            else Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = false;

            //Collision Color Presets
            Instance.EditorToolbar.defaultToolStripMenuItem.IsChecked = ManiacEditor.Methods.Settings.MyDefaults.DefaultCollisionColors == 0;
            Instance.EditorToolbar.invertedToolStripMenuItem.IsChecked = ManiacEditor.Methods.Settings.MyDefaults.DefaultCollisionColors == 1;
            Instance.EditorToolbar.customToolStripMenuItem1.IsChecked = ManiacEditor.Methods.Settings.MyDefaults.DefaultCollisionColors == 2;
            Classes.Editor.SolutionState.CollisionPreset = ManiacEditor.Methods.Settings.MyDefaults.DefaultCollisionColors;
            Instance.RefreshCollisionColours();

            if (ManiacEditor.Methods.Settings.MyDefaults.ScrollLockDirectionDefault == false)
            {
                Classes.Editor.SolutionState.ScrollDirection = Axis.X;
                Instance.EditorStatusBar.UpdateStatusPanel();

            }
            else
            {
                Classes.Editor.SolutionState.ScrollDirection = Axis.Y;
                Instance.EditorStatusBar.UpdateStatusPanel();
            }

        }

        public static void ApplyDefaults()
        {
            // These Prefrences are applied on Editor Load
            Classes.Editor.SolutionState.ApplyEditEntitiesTransparency = ManiacEditor.Methods.Settings.MyDefaults.EditEntitiesTransparentLayersDefault;

            Classes.Editor.SolutionState.ScrollLocked = ManiacEditor.Methods.Settings.MyDefaults.ScrollLockDefault;
            Classes.Editor.SolutionState.ScrollDirection = (ManiacEditor.Methods.Settings.MyDefaults.ScrollLockDirectionDefault == true ? Axis.Y : Axis.X);

            ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.xToolStripMenuItem.IsChecked = Classes.Editor.SolutionState.ScrollDirection == Axis.X;
            ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.yToolStripMenuItem.IsChecked = Classes.Editor.SolutionState.ScrollDirection == Axis.Y;

            Classes.Editor.SolutionState.CountTilesSelectedInPixels = ManiacEditor.Methods.Settings.MyDefaults.EnablePixelModeDefault;

            Classes.Editor.SolutionState.ShowEntityPathArrows = ManiacEditor.Methods.Settings.MyDefaults.ShowEntityArrowPathsDefault;

            Classes.Editor.SolutionState.ShowWaterLevel = ManiacEditor.Methods.Settings.MyDefaults.ShowWaterEntityLevelDefault;
            Classes.Editor.SolutionState.AlwaysShowWaterLevel = ManiacEditor.Methods.Settings.MyDefaults.AlwaysShowWaterLevelDefault;
            Classes.Editor.SolutionState.SizeWaterLevelwithBounds = ManiacEditor.Methods.Settings.MyDefaults.SizeWaterLevelWithBoundsDefault;

            Classes.Editor.SolutionState.ShowParallaxSprites = ManiacEditor.Methods.Settings.MyDefaults.ShowFullParallaxSpritesDefault;
            Classes.Editor.SolutionState.PrioritizedEntityViewing = ManiacEditor.Methods.Settings.MyDefaults.PrioritizedObjectRenderingDefault;

            Classes.Editor.SolutionState.ShowEntitySelectionBoxes = ManiacEditor.Methods.Settings.MyDefaults.ShowEntitySelectionBoxesDefault;

            Classes.Editor.SolutionState.DebugStatsVisibleOnPanel = ManiacEditor.Methods.Settings.MyDefaults.ShowDebugStatsDefault;
            Classes.Editor.SolutionState.UseLargeDebugStats = ManiacEditor.Methods.Settings.MyDefaults.LargeDebugStatsDefault;

            if (ManiacEditor.Methods.Settings.MyDefaults.CustomGridSizeValue < 16)
            {
                ManiacEditor.Methods.Settings.MyDefaults.CustomGridSizeValue = 16;
                ManiacEditor.Methods.Options.DefaultPrefrences.Save();
            }
            Classes.Editor.SolutionState.GridCustomSize = ManiacEditor.Methods.Settings.MyDefaults.CustomGridSizeValue;

            if (ManiacEditor.Methods.Settings.MyDefaults.DefaultGridSizeOption == 0) Classes.Editor.SolutionState.GridCustomSize = 16;
            else if (ManiacEditor.Methods.Settings.MyDefaults.DefaultGridSizeOption == 1) Classes.Editor.SolutionState.GridCustomSize = 32;
            else if (ManiacEditor.Methods.Settings.MyDefaults.DefaultGridSizeOption == 2) Classes.Editor.SolutionState.GridCustomSize = 128;
            else if (ManiacEditor.Methods.Settings.MyDefaults.DefaultGridSizeOption == 3) Classes.Editor.SolutionState.GridCustomSize = ManiacEditor.Methods.Settings.MyDefaults.CustomGridSizeValue;


            Classes.Editor.SolutionState.CollisionSAColour = ManiacEditor.Methods.Settings.MyDefaults.CollisionSAColour;
            Classes.Editor.SolutionState.CollisionLRDColour = ManiacEditor.Methods.Settings.MyDefaults.CollisionLRDColour;
            Classes.Editor.SolutionState.CollisionTOColour = ManiacEditor.Methods.Settings.MyDefaults.CollisionTOColour;

            Classes.Editor.SolutionState.GridColor = ManiacEditor.Methods.Settings.MyDefaults.DefaultGridColor;
            Classes.Editor.SolutionState.waterColor = ManiacEditor.Methods.Settings.MyDefaults.WaterEntityColorDefault;

            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.FasterNudgeValueNUD.Value = ManiacEditor.Methods.Settings.MyDefaults.FasterNudgeValue;





            var allLangItems = ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == ManiacEditor.Methods.Settings.MyDefaults.MenuLanguageDefault)
                    {
                        item.IsChecked = true;
                        Classes.Editor.SolutionState.CurrentLanguage = item.Tag.ToString();
                    }
                }


            bool endSearch = false;
            var allButtonItems = ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allButtonItems)
            {
                if (item.Tag != null)
                {
                    if (item.Tag.ToString() == ManiacEditor.Methods.Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
                    {
                        item.IsChecked = true;
                        Classes.Editor.EditorActions.SetManiaMenuInputType(item.Tag.ToString());
                        endSearch = true;
                    }
                    var allSubButtonItems = item.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                    foreach (var subItem in allSubButtonItems)
                    {
                        if (subItem.Tag != null)
                        {
                            if (subItem.Tag.ToString() == ManiacEditor.Methods.Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
                            {
                                subItem.IsChecked = true;
                                Classes.Editor.EditorActions.SetManiaMenuInputType(item.Tag.ToString());
                                endSearch = true;
                            }
                        }
                    }
                }

            }


        }

        #endregion
    }

}
