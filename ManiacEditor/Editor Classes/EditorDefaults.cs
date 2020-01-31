using System.Linq;

namespace ManiacEditor
{
    public class EditorDefaults
	{
		public EditorDefaults()
		{

        }

        public void ApplyDefaults()
        {
            // These Prefrences are applied on Editor Load
            Editor.Instance.Options.ApplyEditEntitiesTransparency = Settings.MyDefaults.EditEntitiesTransparentLayersDefault;

            Editor.Instance.Options.ScrollLocked = Settings.MyDefaults.ScrollLockDefault;
            Editor.Instance.Options.ScrollDirection = (Settings.MyDefaults.ScrollLockDirectionDefault == true ? 1 : 0);

            Editor.Instance.EditorMenuBar.xToolStripMenuItem.IsChecked = Editor.Instance.Options.ScrollDirection == (int)ScrollDir.X;
            Editor.Instance.EditorMenuBar.yToolStripMenuItem.IsChecked = Editor.Instance.Options.ScrollDirection == (int)ScrollDir.Y;

            Editor.Instance.Options.CountTilesSelectedInPixels = Settings.MyDefaults.EnablePixelModeDefault;

            Editor.Instance.Options.ShowEntityPathArrows = Settings.MyDefaults.ShowEntityArrowPathsDefault;

            Editor.Instance.Options.ShowWaterLevel = Settings.MyDefaults.ShowWaterEntityLevelDefault;
            Editor.Instance.Options.AlwaysShowWaterLevel = Settings.MyDefaults.AlwaysShowWaterLevelDefault;
            Editor.Instance.Options.SizeWaterLevelwithBounds = Settings.MyDefaults.SizeWaterLevelWithBoundsDefault;

            Editor.Instance.Options.ShowParallaxSprites = Settings.MyDefaults.ShowFullParallaxSpritesDefault;
            Editor.Instance.Options.PrioritizedEntityViewing = Settings.MyDefaults.PrioritizedObjectRenderingDefault;

            Editor.Instance.Options.ShowEntitySelectionBoxes = Settings.MyDefaults.ShowEntitySelectionBoxesDefault;

            Editor.Instance.Options.DebugStatsVisibleOnPanel = Settings.MyDefaults.ShowDebugStatsDefault;
            Editor.Instance.Options.UseLargeDebugStats = Settings.MyDefaults.LargeDebugStatsDefault;

            Editor.Instance.Options.GridCustomSize = Settings.MyDefaults.CustomGridSizeValue;
            Editor.Instance.EditorToolbar.CustomGridSizeAdjuster.Value = Editor.Instance.Options.GridCustomSize;

            Editor.Instance.Options.CollisionSAColour = Settings.MyDefaults.CollisionSAColour;
            Editor.Instance.Options.CollisionLRDColour = Settings.MyDefaults.CollisionLRDColour;
            Editor.Instance.Options.CollisionTOColour = Settings.MyDefaults.CollisionTOColour;

            Editor.Instance.Options.GridColor = Settings.MyDefaults.DefaultGridColor;
            Editor.Instance.Options.waterColor = Settings.MyDefaults.WaterEntityColorDefault;

            Editor.Instance.EditorToolbar.FasterNudgeValueNUD.Value = Settings.MyDefaults.FasterNudgeValue;





            var allLangItems = Editor.Instance.EditorMenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == Settings.MyDefaults.MenuLanguageDefault)
                    {
                        item.IsChecked = true;
                        Editor.Instance.Options.CurrentLanguage = item.Tag.ToString();
                    }
                }


            bool endSearch = false;
            var allButtonItems = Editor.Instance.EditorMenuBar.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allButtonItems)
            {
                if (item.Tag != null)
                {
                    if (item.Tag.ToString() == Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
                    {
                        item.IsChecked = true;
                        Editor.Instance.MenuButtonChangedEvent(item.Tag.ToString());
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
                                Editor.Instance.MenuButtonChangedEvent(subItem.Tag.ToString());
                                endSearch = true;
                            }
                        }
                    }
                }

            }


        }
    }
}
