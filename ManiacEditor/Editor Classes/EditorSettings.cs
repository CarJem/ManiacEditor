using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using IniParser;
using IniParser.Model;


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
                    Settings.MySettings.Upgrade();
                    Settings.MySettings.UpgradeRequired = false;
                    Settings.MySettings.Save();

                    Settings.MyPerformance.Upgrade();
                    Settings.MyPerformance.Save();
                    Settings.MyDefaults.Upgrade();
                    Settings.MyDefaults.Save();
                    Settings.MyGameOptions.Upgrade();
                    Settings.MyGameOptions.Save();
                    Settings.MyDevSettings.Upgrade();
                    Settings.MyDevSettings.Save();
                    Settings.MyKeyBinds.Upgrade();
                    Settings.MyKeyBinds.Save();

                }

                Instance.WindowState = Settings.MySettings.IsMaximized ? System.Windows.WindowState.Maximized : Instance.WindowState;
                Instance.InGame.GamePath = Settings.MyDefaults.SonicManiaPath;


                Instance.RefreshDataDirectories(Settings.MySettings.DataDirectories);


                if (Settings.MySettings.ModLoaderConfigs?.Count > 0)
                {
                    Instance.selectConfigToolStripMenuItem.Items.Clear();
                    for (int i = 0; i < Settings.MySettings.ModLoaderConfigs.Count; i++)
                    {
                        Instance.selectConfigToolStripMenuItem.Items.Add(Instance.CreateModConfigMenuItem(i));

                    }
                }

                ApplyDefaults();





            }
            catch (Exception ex)
            {
                Debug.Write("Failed to load settings: " + ex);
            }
        }
        public void ApplyDefaults()
        {
            // These Prefrences are applied on Editor Load
            Instance.UIModes.ApplyEditEntitiesTransparency = Settings.MyDefaults.EditEntitiesTransparentLayersDefault;

            Instance.UIModes.ScrollLocked = Settings.MyDefaults.ScrollLockDefault;
            Instance.UIModes.ScrollDirection = (Settings.MyDefaults.ScrollLockDirectionDefault == true ? 1 : 0);

            Instance.xToolStripMenuItem.IsChecked = Instance.UIModes.ScrollDirection == (int)ScrollDir.X;
            Instance.yToolStripMenuItem.IsChecked = Instance.UIModes.ScrollDirection == (int)ScrollDir.Y;

            Instance.UIModes.EnablePixelCountMode = Settings.MyDefaults.EnablePixelModeDefault;

            Instance.UIModes.ShowEntityPathArrows = Settings.MyDefaults.ShowEntityArrowPathsDefault;

            Instance.UIModes.ShowWaterLevel = Settings.MyDefaults.ShowWaterEntityLevelDefault;
            Instance.UIModes.AlwaysShowWaterLevel = Settings.MyDefaults.AlwaysShowWaterLevelDefault;
            Instance.UIModes.SizeWaterLevelwithBounds = Settings.MyDefaults.SizeWaterLevelWithBoundsDefault;

            Instance.UIModes.ShowParallaxSprites = Settings.MyDefaults.ShowFullParallaxSpritesDefault;
            Instance.UIModes.PrioritizedEntityViewing = Settings.MyDefaults.PrioritizedObjectRenderingDefault;

            Instance.UIModes.ShowEntitySelectionBoxes = Settings.MyDefaults.ShowEntitySelectionBoxesDefault;

            Instance.UIModes.DebugStatsVisibleOnPanel = Settings.MyDefaults.ShowDebugStatsDefault;
            Instance.UIModes.UseLargeDebugStats = Settings.MyDefaults.LargeDebugStatsDefault;



            var allLangItems = Instance.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == Settings.MyDefaults.MenuLanguageDefault)
                    {
                        item.IsChecked = true;
                        Instance.UIModes.CurrentLanguage = item.Tag.ToString();
                    }
                }

            var allLangItems2 = Instance.menuLanguageToolStripMenuItem2.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems2)
                if (item != null)
                {
                    if (item.Tag.ToString() == Settings.MyDefaults.MenuLanguageDefault)
                    {
                        item.IsChecked = true;
                        Instance.UIModes.CurrentLanguage = item.Tag.ToString();
                    }
                }


            bool endSearch = false;
            var allButtonItems = Instance.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allButtonItems)
            {
                if (item.Tag != null)
                {
                    if (item.Tag.ToString() == Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
                    {
                        item.IsChecked = true;
                        Instance.MenuButtonChangedEvent(item.Tag.ToString());
                        endSearch = true;
                    }
                    var allSubButtonItems = item.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                    foreach (var subItem in allSubButtonItems)
                    {
                        if (subItem.Tag != null)
                        {
                            if (subItem.Tag.ToString() == Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
                            {
                                subItem.IsChecked = true;
                                Instance.MenuButtonChangedEvent(subItem.Tag.ToString());
                                endSearch = true;
                            }
                        }
                    }
                }

            }
            endSearch = false;
            var allButtonItems2 = Instance.menuButtonsToolStripMenuItem2.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allButtonItems2)
            {
                if (item.Tag != null)
                {
                    if (item.Tag.ToString() == Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
                    {
                        item.IsChecked = true;
                        Instance.MenuButtonChangedEvent(item.Tag.ToString());
                        endSearch = true;
                    }
                    var allSubButtonItems = item.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                    foreach (var subItem in allSubButtonItems)
                    {
                        if (subItem.Tag != null)
                        {
                            if (subItem.Tag.ToString() == Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
                            {
                                subItem.IsChecked = true;
                                Instance.MenuButtonChangedEvent(subItem.Tag.ToString());
                                endSearch = true;
                            }
                        }
                    }
                }

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
            if (!Settings.MyDefaults.AnimationsDefault) Instance.ShowAnimations.IsChecked = false;
            else Instance.ShowAnimations.IsChecked = true;
            if (!Settings.MyDefaults.AnimationsDefault) Instance.ShowAnimations.IsChecked = false;
            else Instance.ShowAnimations.IsChecked = true;

            //Default Enabled Annimation Preferences
            Instance.movingPlatformsObjectsToolStripMenuItem.IsChecked = Settings.MyDefaults.PlatformAnimationsDefault;
            Instance.UIModes.MovingPlatformsChecked = Settings.MyDefaults.PlatformAnimationsDefault;

            Instance.spriteFramesToolStripMenuItem.IsChecked = Settings.MyDefaults.SpriteAnimationsDefault;
            Instance.UIModes.AnnimationsChecked = Settings.MyDefaults.SpriteAnimationsDefault;

            Instance.UIModes.waterColor = Settings.MyDefaults.WaterEntityColorDefault;




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
            Instance.UIModes.CollisionPreset = Settings.MyDefaults.DefaultCollisionColors;
            Instance.RefreshCollisionColours();

            if (Settings.MyDefaults.ScrollLockDirectionDefault == false)
            {
                Instance.UIModes.ScrollDirection = (int)ScrollDir.X;
                Instance.UI.UpdateStatusPanel();

            }
            else
            {
                Instance.UIModes.ScrollDirection = (int)ScrollDir.Y;
                Instance.UI.UpdateStatusPanel();
            }

        }
    }

}
