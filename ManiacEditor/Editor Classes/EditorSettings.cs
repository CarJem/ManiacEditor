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
            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
                //Properties.Settings.Default[currentProperty.Name]
            }
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
            Settings.mySettings.allowForSmoothSelection = false;
            Settings.mySettings.AllowMoreRenderUpdates = false;
            Settings.mySettings.ShowEditLayerBackground = false;
            Settings.mySettings.SimplifiedWaterLevelRendering = true;
            Settings.mySettings.PrioritizedObjectRendering = false;
            Settings.mySettings.DisableRenderExlusions = true;
            Settings.mySettings.NeverLoadEntityTextures = true;
            Settings.mySettings.preRenderTURBOMode = false;
        }
        public static void ApplyBasicPreset()
        {
            Settings.mySettings.allowForSmoothSelection = false;
            Settings.mySettings.AllowMoreRenderUpdates = false;
            Settings.mySettings.ShowEditLayerBackground = true;
            Settings.mySettings.PrioritizedObjectRendering = false;
            Settings.mySettings.SimplifiedWaterLevelRendering = true;
            Settings.mySettings.DisableRenderExlusions = true;
            Settings.mySettings.NeverLoadEntityTextures = false;
            Settings.mySettings.preRenderTURBOMode = false;
        }
        public static void ApplySuperPreset()
        {
            Settings.mySettings.allowForSmoothSelection = true;
            Settings.mySettings.AllowMoreRenderUpdates = true;
            Settings.mySettings.ShowEditLayerBackground = true;
            Settings.mySettings.PrioritizedObjectRendering = true;
            Settings.mySettings.SimplifiedWaterLevelRendering = false;
            Settings.mySettings.DisableRenderExlusions = false;
            Settings.mySettings.NeverLoadEntityTextures = false;
            Settings.mySettings.preRenderTURBOMode = false;
        }
        public static void ApplyHyperPreset()
        {
            Settings.mySettings.allowForSmoothSelection = true;
            Settings.mySettings.AllowMoreRenderUpdates = true;
            Settings.mySettings.ShowEditLayerBackground = true;
            Settings.mySettings.preRenderTURBOMode = true;
            Settings.mySettings.PrioritizedObjectRendering = true;
            Settings.mySettings.SimplifiedWaterLevelRendering = false;
            Settings.mySettings.DisableRenderExlusions = false;
            Settings.mySettings.NeverLoadEntityTextures = false;
        }
        public static bool isMinimalPreset()
        {
            bool isMinimal = false;
            if (Settings.mySettings.allowForSmoothSelection == false)
            {
                if (Settings.mySettings.AllowMoreRenderUpdates == false)
                    if (Settings.mySettings.ShowEditLayerBackground == false)
                        if (Settings.mySettings.PrioritizedObjectRendering == false)
                            if (Settings.mySettings.SimplifiedWaterLevelRendering == true)
                            if (Settings.mySettings.DisableRenderExlusions == true)
                                if (Settings.mySettings.NeverLoadEntityTextures == true)
                                    if (Settings.mySettings.preRenderTURBOMode == false)
                                        isMinimal = true;
            }
            return isMinimal;

        }
        public static bool isBasicPreset()
        {
            bool isBasic = false;
            if (Settings.mySettings.allowForSmoothSelection == false)
            {
                if (Settings.mySettings.AllowMoreRenderUpdates == false)
                    if (Settings.mySettings.ShowEditLayerBackground == true)
                        if (Settings.mySettings.PrioritizedObjectRendering == false)
                            if (Settings.mySettings.SimplifiedWaterLevelRendering == true)
                            if (Settings.mySettings.DisableRenderExlusions == true)
                                if (Settings.mySettings.NeverLoadEntityTextures == false)
                                    if (Settings.mySettings.preRenderTURBOMode == false)
                                        isBasic = true;
            }
            return isBasic;
        }
        public static bool isSuperPreset()
        {
            bool isSuper = false;
            if (Settings.mySettings.allowForSmoothSelection == true)
            {
                if (Settings.mySettings.AllowMoreRenderUpdates == true)
                    if (Settings.mySettings.ShowEditLayerBackground == true)
                        if (Settings.mySettings.PrioritizedObjectRendering == true)
                            if (Settings.mySettings.SimplifiedWaterLevelRendering == false)
                            if (Settings.mySettings.DisableRenderExlusions == false)
                                if (Settings.mySettings.NeverLoadEntityTextures == false)
                                    if (Settings.mySettings.preRenderTURBOMode == false)
                                        isSuper = true;
            }
            return isSuper;
        }
        public static bool isHyperPreset()
        {
            bool isHyper = false;
            if (Settings.mySettings.allowForSmoothSelection == true)
            {
                if (Settings.mySettings.AllowMoreRenderUpdates == true)
                    if (Settings.mySettings.ShowEditLayerBackground == true)
                        if (Settings.mySettings.preRenderTURBOMode == true)
                            if (Settings.mySettings.PrioritizedObjectRendering == true)
                                if (Settings.mySettings.SimplifiedWaterLevelRendering == false)
                                    if (Settings.mySettings.DisableRenderExlusions == false)
                                        if (Settings.mySettings.NeverLoadEntityTextures == false)
                                            isHyper = true;
            }
            return isHyper;
        }
        #endregion

        public void TryLoadSettings()
        {
            try
            {
                if (Settings.mySettings.UpgradeRequired)
                {
                    Settings.mySettings.Upgrade();
                    Settings.mySettings.UpgradeRequired = false;
                    Settings.mySettings.Save();
                }

                Instance.WindowState = Settings.mySettings.IsMaximized ? System.Windows.WindowState.Maximized : Instance.WindowState;
                Instance.InGame.GamePath = Settings.mySettings.GamePath;


                Instance.RefreshDataDirectories(Settings.mySettings.DataDirectories);


                if (Settings.mySettings.modConfigs?.Count > 0)
                {
                    Instance.selectConfigToolStripMenuItem.Items.Clear();
                    for (int i = 0; i < Settings.mySettings.modConfigs.Count; i++)
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
            Instance.UIModes.ApplyEditEntitiesTransparency = Settings.mySettings.EditEntitiesTransparencyDefault;

            Instance.UIModes.ScrollLocked = Settings.mySettings.ScrollLockEnabledDefault;
            Instance.UIModes.ScrollDirection = Settings.mySettings.ScrollLockXYDefault;

            Instance.xToolStripMenuItem.IsChecked = Instance.UIModes.ScrollDirection == (int)ScrollDir.X;
            Instance.yToolStripMenuItem.IsChecked = Instance.UIModes.ScrollDirection == (int)ScrollDir.Y;

            Instance.UIModes.EnablePixelCountMode = Settings.mySettings.EnablePixelModeDefault;

            Instance.UIModes.ShowEntityPathArrows = Settings.mySettings.ShowEntityArrowPathsDefault;

            Instance.UIModes.ShowWaterLevel = Settings.mySettings.showWaterLevelDefault;
            Instance.UIModes.AlwaysShowWaterLevel = Settings.mySettings.AlwaysShowWaterLevelDefault;
            Instance.UIModes.SizeWaterLevelwithBounds = Settings.mySettings.SizeWaterLevelWithBoundsDefault;

            Instance.UIModes.ShowParallaxSprites = Settings.mySettings.ShowFullParallaxEntityRenderDefault;
            Instance.UIModes.PrioritizedEntityViewing = Settings.mySettings.PrioritizedObjectRendering;

            Instance.UIModes.ShowEntitySelectionBoxes = Settings.mySettings.ShowEntitySelectionBoxesDefault;

            Instance.UIModes.DebugStatsVisibleOnPanel = Settings.mySettings.ShowStatsViewerDefault;
            Instance.UIModes.UseLargeDebugStats = Settings.mySettings.StatsViewerLargeTextDefault;



            var allLangItems = Instance.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == Settings.mySettings.LangDefault)
                    {
                        item.IsChecked = true;
                        Instance.UIModes.CurrentLanguage = item.Tag.ToString();
                    }
                }

            var allLangItems2 = Instance.menuLanguageToolStripMenuItem2.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems2)
                if (item != null)
                {
                    if (item.Tag.ToString() == Settings.mySettings.LangDefault)
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
                    if (item.Tag.ToString() == Settings.mySettings.ButtonLayoutDefault && !endSearch)
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
                            if (subItem.Tag.ToString() == Settings.mySettings.ButtonLayoutDefault && !endSearch)
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
                    if (item.Tag.ToString() == Settings.mySettings.ButtonLayoutDefault && !endSearch)
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
                            if (subItem.Tag.ToString() == Settings.mySettings.ButtonLayoutDefault && !endSearch)
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
            if (!Settings.mySettings.FGLowerDefault) Instance.ShowFGLower.IsChecked = false;
            else Instance.ShowFGLower.IsChecked = true;
            if (!Settings.mySettings.FGLowDefault) Instance.ShowFGLow.IsChecked = false;
            else Instance.ShowFGLow.IsChecked = true;
            if (!Settings.mySettings.FGHighDefault) Instance.ShowFGHigh.IsChecked = false;
            else Instance.ShowFGHigh.IsChecked = true;
            if (!Settings.mySettings.FGHigherDefault) Instance.ShowFGHigher.IsChecked = false;
            else Instance.ShowFGHigher.IsChecked = true;
            if (!Settings.mySettings.EntitiesDefault) Instance.ShowEntities.IsChecked = false;
            else Instance.ShowEntities.IsChecked = true;
            if (!Settings.mySettings.AnimationsDefault) Instance.ShowAnimations.IsChecked = false;
            else Instance.ShowAnimations.IsChecked = true;

            //Default Enabled Annimation Preferences
            Instance.movingPlatformsObjectsToolStripMenuItem.IsChecked = Settings.mySettings.MovingPlatformsDefault;
            Instance.UIModes.MovingPlatformsChecked = Settings.mySettings.MovingPlatformsDefault;

            Instance.spriteFramesToolStripMenuItem.IsChecked = Settings.mySettings.AnimatedSpritesDefault;
            Instance.UIModes.AnnimationsChecked = Settings.mySettings.AnimatedSpritesDefault;

            Instance.UIModes.waterColor = Settings.mySettings.WaterColorDefault;




            //Default Grid Preferences
            if (!Settings.mySettings.x16Default) Instance.Grid16x16SizeMenuItem.IsChecked = false;
            else Instance.Grid16x16SizeMenuItem.IsChecked = true;
            if (!Settings.mySettings.x128Default) Instance.Grid128x128SizeMenuItem.IsChecked = false;
            else Instance.Grid128x128SizeMenuItem.IsChecked = true;
            if (!Settings.mySettings.x256Default) Instance.Grid256x256SizeMenuItem.IsChecked = false;
            else Instance.Grid256x256SizeMenuItem.IsChecked = true;
            if (!Settings.mySettings.CustomGridDefault) Instance.GridCustomSizeMenuItem.IsChecked = false;
            else Instance.GridCustomSizeMenuItem.IsChecked = true;

            //Collision Color Presets
            Instance.defaultToolStripMenuItem.IsChecked = Settings.mySettings.CollisionColorsDefault == 0;
            Instance.invertedToolStripMenuItem.IsChecked = Settings.mySettings.CollisionColorsDefault == 1;
            Instance.customToolStripMenuItem1.IsChecked = Settings.mySettings.CollisionColorsDefault == 2;
            Instance.UIModes.CollisionPreset = Settings.mySettings.CollisionColorsDefault;
            Instance.RefreshCollisionColours();

            if (Settings.mySettings.ScrollLockXYDefault.Equals(ScrollDir.X))
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
