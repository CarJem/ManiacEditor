using System;
using System.Diagnostics;
using ManiacEditor.Enums;
using System.Linq;

namespace ManiacEditor.Methods.Internal
{
    [Serializable]
    public class Settings
    {
        private static Controls.Base.MainEditor Instance { get; set; }
        public static void UpdateInstance(Controls.Base.MainEditor instance)
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
            ManiacEditor.Core.Settings.MyPerformance.ShowEditLayerBackground = false;
            ManiacEditor.Core.Settings.MyPerformance.UseSimplifedWaterRendering = true;
            ManiacEditor.Core.Settings.MyPerformance.DisableRendererExclusions = true;
            ManiacEditor.Core.Settings.MyPerformance.NeverLoadEntityTextures = true;
        }
        public static void ApplyBasicPreset()
        {
            ManiacEditor.Core.Settings.MyPerformance.ShowEditLayerBackground = true;
            ManiacEditor.Core.Settings.MyPerformance.UseSimplifedWaterRendering = true;
            ManiacEditor.Core.Settings.MyPerformance.DisableRendererExclusions = true;
            ManiacEditor.Core.Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static void ApplySuperPreset()
        {
            ManiacEditor.Core.Settings.MyPerformance.ShowEditLayerBackground = true;
            ManiacEditor.Core.Settings.MyPerformance.UseSimplifedWaterRendering = false;
            ManiacEditor.Core.Settings.MyPerformance.DisableRendererExclusions = true;
            ManiacEditor.Core.Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static void ApplyHyperPreset()
        {
            ManiacEditor.Core.Settings.MyPerformance.ShowEditLayerBackground = true;
            ManiacEditor.Core.Settings.MyPerformance.UseSimplifedWaterRendering = false;
            ManiacEditor.Core.Settings.MyPerformance.DisableRendererExclusions = false;
            ManiacEditor.Core.Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static bool isMinimalPreset()
        {
            bool isMinimal = false;
            if (ManiacEditor.Core.Settings.MyPerformance.ShowEditLayerBackground == false)
                    if (ManiacEditor.Core.Settings.MyPerformance.UseSimplifedWaterRendering == true)
                        if (ManiacEditor.Core.Settings.MyPerformance.DisableRendererExclusions == true)
                            if (ManiacEditor.Core.Settings.MyPerformance.NeverLoadEntityTextures == true)
                                isMinimal = true;
            return isMinimal;

        }
        public static bool isBasicPreset()
        {
            bool isBasic = false;
                if (ManiacEditor.Core.Settings.MyPerformance.ShowEditLayerBackground == true)
                        if (ManiacEditor.Core.Settings.MyPerformance.UseSimplifedWaterRendering == true)
                            if (ManiacEditor.Core.Settings.MyPerformance.DisableRendererExclusions == true)
                                if (ManiacEditor.Core.Settings.MyPerformance.NeverLoadEntityTextures == false)
                                    isBasic = true;
            return isBasic;
        }
        public static bool isSuperPreset()
        {
            bool isSuper = false;
                if (ManiacEditor.Core.Settings.MyPerformance.ShowEditLayerBackground == true)
                        if (ManiacEditor.Core.Settings.MyPerformance.UseSimplifedWaterRendering == false)
                            if (ManiacEditor.Core.Settings.MyPerformance.DisableRendererExclusions == true)
                                if (ManiacEditor.Core.Settings.MyPerformance.NeverLoadEntityTextures == false)
                                    isSuper = true;
            return isSuper;
        }
        public static bool isHyperPreset()
        {
            bool isHyper = false;
            if (ManiacEditor.Core.Settings.MyPerformance.ShowEditLayerBackground == true)
                if (ManiacEditor.Core.Settings.MyPerformance.UseSimplifedWaterRendering == false)
                    if (ManiacEditor.Core.Settings.MyPerformance.DisableRendererExclusions == false)
                        if (ManiacEditor.Core.Settings.MyPerformance.NeverLoadEntityTextures == false)
                            isHyper = true;
            return isHyper;
        }
        #endregion

        public static void TryLoadSettings()
        {
            try
            {
                if (ManiacEditor.Core.Settings.MySettings.UpgradeRequired)
                {
                    Classes.Editor.Constants.UpgradeAllSettings();
                    ManiacEditor.Core.Settings.MySettings.UpgradeRequired = false;
                    Classes.Editor.Constants.SaveAllSettings();
                }

                Instance.WindowState = ManiacEditor.Core.Settings.MySettings.IsMaximized ? System.Windows.WindowState.Maximized : Instance.WindowState;
                Methods.GameHandler.GamePath = ManiacEditor.Core.Settings.MyDefaults.SonicManiaPath;

                if (ManiacEditor.Core.Settings.MySettings.ModLoaderConfigs?.Count > 0)
                {
                    Instance.EditorToolbar.selectConfigToolStripMenuItem.Items.Clear();
                    for (int i = 0; i < ManiacEditor.Core.Settings.MySettings.ModLoaderConfigs.Count; i++)
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
            if (!ManiacEditor.Core.Settings.MyDefaults.FGLowerDefault) Instance.EditorToolbar.ShowFGLower.IsChecked = false;
            else Instance.EditorToolbar.ShowFGLower.IsChecked = true;
            if (!ManiacEditor.Core.Settings.MyDefaults.FGLowDefault) Instance.EditorToolbar.ShowFGLow.IsChecked = false;
            else Instance.EditorToolbar.ShowFGLow.IsChecked = true;
            if (!ManiacEditor.Core.Settings.MyDefaults.FGHighDefault) Instance.EditorToolbar.ShowFGHigh.IsChecked = false;
            else Instance.EditorToolbar.ShowFGHigh.IsChecked = true;
            if (!ManiacEditor.Core.Settings.MyDefaults.FGHigherDefault) Instance.EditorToolbar.ShowFGHigher.IsChecked = false;
            else Instance.EditorToolbar.ShowFGHigher.IsChecked = true;
            if (!ManiacEditor.Core.Settings.MyDefaults.EntitiesDefault) Instance.EditorToolbar.ShowEntities.IsChecked = false;
            else Instance.EditorToolbar.ShowEntities.IsChecked = true;
            Instance.EditorToolbar.ShowAnimations.IsChecked = ManiacEditor.Core.Settings.MyDefaults.AnimationsDefault;
            Classes.Editor.SolutionState.AllowAnimations = ManiacEditor.Core.Settings.MyDefaults.AnimationsDefault;


            //Default Enabled Annimation Preferences
            Instance.EditorToolbar.movingPlatformsObjectsToolStripMenuItem.IsChecked = ManiacEditor.Core.Settings.MyDefaults.PlatformAnimationsDefault;
            Classes.Editor.SolutionState.AllowMovingPlatformAnimations = ManiacEditor.Core.Settings.MyDefaults.PlatformAnimationsDefault;

            Instance.EditorToolbar.spriteFramesToolStripMenuItem.IsChecked = ManiacEditor.Core.Settings.MyDefaults.SpriteAnimationsDefault;
            Classes.Editor.SolutionState.AllowSpriteAnimations = ManiacEditor.Core.Settings.MyDefaults.SpriteAnimationsDefault;


            //TO DO: Add Default For this.
            Instance.EditorToolbar.parallaxAnimationMenuItem.IsChecked = false;
            Classes.Editor.SolutionState.ParallaxAnimationChecked = false;

            Classes.Editor.SolutionState.waterColor = ManiacEditor.Core.Settings.MyDefaults.WaterEntityColorDefault;




            //Default Grid Preferences
            if (ManiacEditor.Core.Settings.MyDefaults.DefaultGridSizeOption == 0) Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = true;
            else Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = false;
            if (ManiacEditor.Core.Settings.MyDefaults.DefaultGridSizeOption == 1) Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = true;
            else Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = false;
            if (ManiacEditor.Core.Settings.MyDefaults.DefaultGridSizeOption == 2) Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = true;
            else Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = false;
            if (ManiacEditor.Core.Settings.MyDefaults.DefaultGridSizeOption == 3) Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = true;
            else Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = false;

            //Collision Color Presets
            Instance.EditorToolbar.defaultToolStripMenuItem.IsChecked = ManiacEditor.Core.Settings.MyDefaults.DefaultCollisionColors == 0;
            Instance.EditorToolbar.invertedToolStripMenuItem.IsChecked = ManiacEditor.Core.Settings.MyDefaults.DefaultCollisionColors == 1;
            Instance.EditorToolbar.customToolStripMenuItem1.IsChecked = ManiacEditor.Core.Settings.MyDefaults.DefaultCollisionColors == 2;
            Classes.Editor.SolutionState.CollisionPreset = ManiacEditor.Core.Settings.MyDefaults.DefaultCollisionColors;
            Instance.RefreshCollisionColours();

            if (ManiacEditor.Core.Settings.MyDefaults.ScrollLockDirectionDefault == false)
            {
                Classes.Editor.SolutionState.ScrollDirection = (int)ScrollDir.X;
                Instance.EditorStatusBar.UpdateStatusPanel();

            }
            else
            {
                Classes.Editor.SolutionState.ScrollDirection = (int)ScrollDir.Y;
                Instance.EditorStatusBar.UpdateStatusPanel();
            }

        }

        public static void ApplyDefaults()
        {
            // These Prefrences are applied on Editor Load
            Classes.Editor.SolutionState.ApplyEditEntitiesTransparency = ManiacEditor.Core.Settings.MyDefaults.EditEntitiesTransparentLayersDefault;

            Classes.Editor.SolutionState.ScrollLocked = ManiacEditor.Core.Settings.MyDefaults.ScrollLockDefault;
            Classes.Editor.SolutionState.ScrollDirection = (ManiacEditor.Core.Settings.MyDefaults.ScrollLockDirectionDefault == true ? 1 : 0);

            Controls.Base.MainEditor.Instance.EditorMenuBar.xToolStripMenuItem.IsChecked = Classes.Editor.SolutionState.ScrollDirection == (int)ScrollDir.X;
            Controls.Base.MainEditor.Instance.EditorMenuBar.yToolStripMenuItem.IsChecked = Classes.Editor.SolutionState.ScrollDirection == (int)ScrollDir.Y;

            Classes.Editor.SolutionState.CountTilesSelectedInPixels = ManiacEditor.Core.Settings.MyDefaults.EnablePixelModeDefault;

            Classes.Editor.SolutionState.ShowEntityPathArrows = ManiacEditor.Core.Settings.MyDefaults.ShowEntityArrowPathsDefault;

            Classes.Editor.SolutionState.ShowWaterLevel = ManiacEditor.Core.Settings.MyDefaults.ShowWaterEntityLevelDefault;
            Classes.Editor.SolutionState.AlwaysShowWaterLevel = ManiacEditor.Core.Settings.MyDefaults.AlwaysShowWaterLevelDefault;
            Classes.Editor.SolutionState.SizeWaterLevelwithBounds = ManiacEditor.Core.Settings.MyDefaults.SizeWaterLevelWithBoundsDefault;

            Classes.Editor.SolutionState.ShowParallaxSprites = ManiacEditor.Core.Settings.MyDefaults.ShowFullParallaxSpritesDefault;
            Classes.Editor.SolutionState.PrioritizedEntityViewing = ManiacEditor.Core.Settings.MyDefaults.PrioritizedObjectRenderingDefault;

            Classes.Editor.SolutionState.ShowEntitySelectionBoxes = ManiacEditor.Core.Settings.MyDefaults.ShowEntitySelectionBoxesDefault;

            Classes.Editor.SolutionState.DebugStatsVisibleOnPanel = ManiacEditor.Core.Settings.MyDefaults.ShowDebugStatsDefault;
            Classes.Editor.SolutionState.UseLargeDebugStats = ManiacEditor.Core.Settings.MyDefaults.LargeDebugStatsDefault;

            Classes.Editor.SolutionState.GridCustomSize = ManiacEditor.Core.Settings.MyDefaults.CustomGridSizeValue;
            Controls.Base.MainEditor.Instance.EditorToolbar.CustomGridSizeAdjuster.Value = Classes.Editor.SolutionState.GridCustomSize;

            Classes.Editor.SolutionState.CollisionSAColour = ManiacEditor.Core.Settings.MyDefaults.CollisionSAColour;
            Classes.Editor.SolutionState.CollisionLRDColour = ManiacEditor.Core.Settings.MyDefaults.CollisionLRDColour;
            Classes.Editor.SolutionState.CollisionTOColour = ManiacEditor.Core.Settings.MyDefaults.CollisionTOColour;

            Classes.Editor.SolutionState.GridColor = ManiacEditor.Core.Settings.MyDefaults.DefaultGridColor;
            Classes.Editor.SolutionState.waterColor = ManiacEditor.Core.Settings.MyDefaults.WaterEntityColorDefault;

            Controls.Base.MainEditor.Instance.EditorToolbar.FasterNudgeValueNUD.Value = ManiacEditor.Core.Settings.MyDefaults.FasterNudgeValue;





            var allLangItems = Controls.Base.MainEditor.Instance.EditorMenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == ManiacEditor.Core.Settings.MyDefaults.MenuLanguageDefault)
                    {
                        item.IsChecked = true;
                        Classes.Editor.SolutionState.CurrentLanguage = item.Tag.ToString();
                    }
                }


            bool endSearch = false;
            var allButtonItems = Controls.Base.MainEditor.Instance.EditorMenuBar.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allButtonItems)
            {
                if (item.Tag != null)
                {
                    if (item.Tag.ToString() == ManiacEditor.Core.Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
                    {
                        item.IsChecked = true;
                        Controls.Base.MainEditor.Instance.MenuButtonChangedEvent(item.Tag.ToString());
                        endSearch = true;
                    }
                    var allSubButtonItems = item.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                    foreach (var subItem in allSubButtonItems)
                    {
                        if (subItem.Tag != null)
                        {
                            if (subItem.Tag.ToString() == ManiacEditor.Core.Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
                            {
                                subItem.IsChecked = true;
                                Controls.Base.MainEditor.Instance.MenuButtonChangedEvent(subItem.Tag.ToString());
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
