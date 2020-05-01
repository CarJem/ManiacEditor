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
            ManiacEditor.Properties.Settings.MyPerformance.ShowEditLayerBackground = false;
            ManiacEditor.Properties.Settings.MyPerformance.UseSimplifedWaterRendering = true;
            ManiacEditor.Properties.Settings.MyPerformance.DisableRendererExclusions = true;
            ManiacEditor.Properties.Settings.MyPerformance.NeverLoadEntityTextures = true;
        }
        public static void ApplyBasicPreset()
        {
            ManiacEditor.Properties.Settings.MyPerformance.ShowEditLayerBackground = true;
            ManiacEditor.Properties.Settings.MyPerformance.UseSimplifedWaterRendering = true;
            ManiacEditor.Properties.Settings.MyPerformance.DisableRendererExclusions = true;
            ManiacEditor.Properties.Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static void ApplySuperPreset()
        {
            ManiacEditor.Properties.Settings.MyPerformance.ShowEditLayerBackground = true;
            ManiacEditor.Properties.Settings.MyPerformance.UseSimplifedWaterRendering = false;
            ManiacEditor.Properties.Settings.MyPerformance.DisableRendererExclusions = true;
            ManiacEditor.Properties.Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static void ApplyHyperPreset()
        {
            ManiacEditor.Properties.Settings.MyPerformance.ShowEditLayerBackground = true;
            ManiacEditor.Properties.Settings.MyPerformance.UseSimplifedWaterRendering = false;
            ManiacEditor.Properties.Settings.MyPerformance.DisableRendererExclusions = false;
            ManiacEditor.Properties.Settings.MyPerformance.NeverLoadEntityTextures = false;
        }
        public static bool isMinimalPreset()
        {
            bool isMinimal = false;
            if (ManiacEditor.Properties.Settings.MyPerformance.ShowEditLayerBackground == false)
                    if (ManiacEditor.Properties.Settings.MyPerformance.UseSimplifedWaterRendering == true)
                        if (ManiacEditor.Properties.Settings.MyPerformance.DisableRendererExclusions == true)
                            if (ManiacEditor.Properties.Settings.MyPerformance.NeverLoadEntityTextures == true)
                                isMinimal = true;
            return isMinimal;

        }
        public static bool isBasicPreset()
        {
            bool isBasic = false;
                if (ManiacEditor.Properties.Settings.MyPerformance.ShowEditLayerBackground == true)
                        if (ManiacEditor.Properties.Settings.MyPerformance.UseSimplifedWaterRendering == true)
                            if (ManiacEditor.Properties.Settings.MyPerformance.DisableRendererExclusions == true)
                                if (ManiacEditor.Properties.Settings.MyPerformance.NeverLoadEntityTextures == false)
                                    isBasic = true;
            return isBasic;
        }
        public static bool isSuperPreset()
        {
            bool isSuper = false;
                if (ManiacEditor.Properties.Settings.MyPerformance.ShowEditLayerBackground == true)
                        if (ManiacEditor.Properties.Settings.MyPerformance.UseSimplifedWaterRendering == false)
                            if (ManiacEditor.Properties.Settings.MyPerformance.DisableRendererExclusions == true)
                                if (ManiacEditor.Properties.Settings.MyPerformance.NeverLoadEntityTextures == false)
                                    isSuper = true;
            return isSuper;
        }
        public static bool isHyperPreset()
        {
            bool isHyper = false;
            if (ManiacEditor.Properties.Settings.MyPerformance.ShowEditLayerBackground == true)
                if (ManiacEditor.Properties.Settings.MyPerformance.UseSimplifedWaterRendering == false)
                    if (ManiacEditor.Properties.Settings.MyPerformance.DisableRendererExclusions == false)
                        if (ManiacEditor.Properties.Settings.MyPerformance.NeverLoadEntityTextures == false)
                            isHyper = true;
            return isHyper;
        }
        #endregion

        public static void TryLoadSettings()
        {
            try
            {
                if (ManiacEditor.Properties.Settings.MySettings.UpgradeRequired)
                {
                    Properties.Settings.UpgradeAllSettings();
                    ManiacEditor.Properties.Settings.MySettings.UpgradeRequired = false;
                    Properties.Settings.SaveAllSettings();
                }

                Instance.WindowState = ManiacEditor.Properties.Settings.MySettings.IsMaximized ? System.Windows.WindowState.Maximized : Instance.WindowState;
                Methods.Runtime.GameHandler.GamePath = ManiacEditor.Properties.Settings.MyDefaults.SonicManiaPath;

                if (Classes.Prefrences.CommonPathsStorage.Collection.ModLoaderConfigs?.Count > 0)
                {
                    Instance.EditorToolbar.selectConfigToolStripMenuItem.Items.Clear();
                    for (int i = 0; i < Classes.Prefrences.CommonPathsStorage.Collection.ModLoaderConfigs.Count; i++)
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
            if (!ManiacEditor.Properties.Settings.MyDefaults.FGLowerDefault) Instance.EditorToolbar.ShowFGLower.IsChecked = false;
            else Instance.EditorToolbar.ShowFGLower.IsChecked = true;
            if (!ManiacEditor.Properties.Settings.MyDefaults.FGLowDefault) Instance.EditorToolbar.ShowFGLow.IsChecked = false;
            else Instance.EditorToolbar.ShowFGLow.IsChecked = true;
            if (!ManiacEditor.Properties.Settings.MyDefaults.FGHighDefault) Instance.EditorToolbar.ShowFGHigh.IsChecked = false;
            else Instance.EditorToolbar.ShowFGHigh.IsChecked = true;
            if (!ManiacEditor.Properties.Settings.MyDefaults.FGHigherDefault) Instance.EditorToolbar.ShowFGHigher.IsChecked = false;
            else Instance.EditorToolbar.ShowFGHigher.IsChecked = true;
            if (!ManiacEditor.Properties.Settings.MyDefaults.EntitiesDefault) Instance.EditorToolbar.ShowEntities.IsChecked = false;
            else Instance.EditorToolbar.ShowEntities.IsChecked = true;




            Methods.Solution.SolutionState.Main.waterColor = ManiacEditor.Properties.Settings.MyDefaults.WaterEntityColorDefault;




            //Default Grid Preferences
            if (ManiacEditor.Properties.Settings.MyDefaults.DefaultGridSizeOption == 0)
            {
                Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = true;
                Methods.Solution.SolutionState.Main.GridSize = 16;
            }
            else Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = false;
            if (ManiacEditor.Properties.Settings.MyDefaults.DefaultGridSizeOption == 1)
            {
                Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = true;
                Methods.Solution.SolutionState.Main.GridSize = 128;
            }
            else Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = false;
            if (ManiacEditor.Properties.Settings.MyDefaults.DefaultGridSizeOption == 2)
            {
                Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = true;
                Methods.Solution.SolutionState.Main.GridSize = 256;
            }
            else Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = false;
            if (ManiacEditor.Properties.Settings.MyDefaults.DefaultGridSizeOption == 3)
            {
                Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = true;
                Methods.Solution.SolutionState.Main.GridSize = -1;
            }
            else Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = false;



            //Collision Color Presets
            Instance.EditorToolbar.CollisionDefaultMenuItem.IsChecked = ManiacEditor.Properties.Settings.MyDefaults.DefaultCollisionColors == 0;
            Instance.EditorToolbar.CollisionInvertedMenuItem.IsChecked = ManiacEditor.Properties.Settings.MyDefaults.DefaultCollisionColors == 1;
            Instance.EditorToolbar.CollisionCustomMenuItem.IsChecked = ManiacEditor.Properties.Settings.MyDefaults.DefaultCollisionColors == 2;
            Methods.Solution.SolutionState.Main.CollisionPreset = ManiacEditor.Properties.Settings.MyDefaults.DefaultCollisionColors;
            Methods.Solution.SolutionState.Main.RefreshCollisionColours();

            if (ManiacEditor.Properties.Settings.MyDefaults.ScrollLockDirectionDefault == false)
            {
                Methods.Solution.SolutionState.Main.ScrollDirection = Axis.X;
                Instance.EditorStatusBar.UpdateStatusPanel();

            }
            else
            {
                Methods.Solution.SolutionState.Main.ScrollDirection = Axis.Y;
                Instance.EditorStatusBar.UpdateStatusPanel();
            }

        }

        public static void ApplyDefaults()
        {
            // These Prefrences are applied on Editor Load
            Methods.Solution.SolutionState.Main.ApplyEditEntitiesTransparency = ManiacEditor.Properties.Settings.MyDefaults.EditEntitiesTransparentLayersDefault;

            Methods.Solution.SolutionState.Main.ScrollLocked = ManiacEditor.Properties.Settings.MyDefaults.ScrollLockDefault;
            Methods.Solution.SolutionState.Main.ScrollDirection = (ManiacEditor.Properties.Settings.MyDefaults.ScrollLockDirectionDefault == true ? Axis.Y : Axis.X);

            Instance.MenuBar.xToolStripMenuItem.IsChecked = Methods.Solution.SolutionState.Main.ScrollDirection == Axis.X;
            Instance.MenuBar.yToolStripMenuItem.IsChecked = Methods.Solution.SolutionState.Main.ScrollDirection == Axis.Y;

            Methods.Solution.SolutionState.Main.CountTilesSelectedInPixels = ManiacEditor.Properties.Settings.MyDefaults.EnablePixelModeDefault;

            Methods.Solution.SolutionState.Main.ShowEntityPathArrows = ManiacEditor.Properties.Settings.MyDefaults.ShowEntityArrowPathsDefault;

            Methods.Solution.SolutionState.Main.ShowWaterLevel = ManiacEditor.Properties.Settings.MyDefaults.ShowWaterEntityLevelDefault;
            Methods.Solution.SolutionState.Main.AlwaysShowWaterLevel = ManiacEditor.Properties.Settings.MyDefaults.AlwaysShowWaterLevelDefault;
            Methods.Solution.SolutionState.Main.SizeWaterLevelwithBounds = ManiacEditor.Properties.Settings.MyDefaults.SizeWaterLevelWithBoundsDefault;

            Methods.Solution.SolutionState.Main.ShowParallaxSprites = ManiacEditor.Properties.Settings.MyDefaults.ShowFullParallaxSpritesDefault;

            Methods.Solution.SolutionState.Main.ShowEntitySelectionBoxes = ManiacEditor.Properties.Settings.MyDefaults.ShowEntitySelectionBoxesDefault;

            Methods.Solution.SolutionState.Main.DebugStatsVisibleOnPanel = ManiacEditor.Properties.Settings.MyDefaults.ShowDebugStatsDefault;

            if (ManiacEditor.Properties.Settings.MyDefaults.CustomGridSizeValue < 16)
            {
                ManiacEditor.Properties.Settings.MyDefaults.CustomGridSizeValue = 16;
                ManiacEditor.Classes.Options.DefaultPrefrences.Save();
            }
            Methods.Solution.SolutionState.Main.GridCustomSize = ManiacEditor.Properties.Settings.MyDefaults.CustomGridSizeValue;

            if (ManiacEditor.Properties.Settings.MyDefaults.DefaultGridSizeOption == 0) Methods.Solution.SolutionState.Main.GridCustomSize = 16;
            else if (ManiacEditor.Properties.Settings.MyDefaults.DefaultGridSizeOption == 1) Methods.Solution.SolutionState.Main.GridCustomSize = 32;
            else if (ManiacEditor.Properties.Settings.MyDefaults.DefaultGridSizeOption == 2) Methods.Solution.SolutionState.Main.GridCustomSize = 128;
            else if (ManiacEditor.Properties.Settings.MyDefaults.DefaultGridSizeOption == 3) Methods.Solution.SolutionState.Main.GridCustomSize = ManiacEditor.Properties.Settings.MyDefaults.CustomGridSizeValue;


            Methods.Solution.SolutionState.Main.Custom_CollisionAllSolid_Color = ManiacEditor.Properties.Settings.MyDefaults.CollisionSAColour;
            Methods.Solution.SolutionState.Main.Custom_CollisionLRDSolid_Color = ManiacEditor.Properties.Settings.MyDefaults.CollisionLRDColour;
            Methods.Solution.SolutionState.Main.Custom_CollisionTopOnlySolid_Color = ManiacEditor.Properties.Settings.MyDefaults.CollisionTOColour;

            Methods.Solution.SolutionState.Main.GridColor = ManiacEditor.Properties.Settings.MyDefaults.DefaultGridColor;
            Methods.Solution.SolutionState.Main.waterColor = ManiacEditor.Properties.Settings.MyDefaults.WaterEntityColorDefault;

            Instance.EditorToolbar.FasterNudgeValueNUD.Value = ManiacEditor.Properties.Settings.MyDefaults.FasterNudgeValue;





            var allLangItems = Instance.MenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == ManiacEditor.Properties.Settings.MyDefaults.MenuLanguageDefault)
                    {
                        item.IsChecked = true;
                        Methods.Solution.SolutionState.Main.CurrentManiaUILanguage = item.Tag.ToString();
                    }
                }


            bool endSearch = false;
            var allButtonItems = Instance.MenuBar.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allButtonItems)
            {
                if (item.Tag != null)
                {
                    if (item.Tag.ToString() == ManiacEditor.Properties.Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
                    {
                        item.IsChecked = true;
                        Methods.Solution.SolutionActions.SetManiaMenuInputType(item.Tag.ToString());
                        endSearch = true;
                    }
                    var allSubButtonItems = item.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
                    foreach (var subItem in allSubButtonItems)
                    {
                        if (subItem.Tag != null)
                        {
                            if (subItem.Tag.ToString() == ManiacEditor.Properties.Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
                            {
                                subItem.IsChecked = true;
                                Methods.Solution.SolutionActions.SetManiaMenuInputType(item.Tag.ToString());
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
