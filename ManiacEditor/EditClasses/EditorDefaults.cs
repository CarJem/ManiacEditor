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
            EditClasses.EditorState.ApplyEditEntitiesTransparency = Settings.MyDefaults.EditEntitiesTransparentLayersDefault;

            EditClasses.EditorState.ScrollLocked = Settings.MyDefaults.ScrollLockDefault;
            EditClasses.EditorState.ScrollDirection = (Settings.MyDefaults.ScrollLockDirectionDefault == true ? 1 : 0);

            Editor.Instance.EditorMenuBar.xToolStripMenuItem.IsChecked = EditClasses.EditorState.ScrollDirection == (int)ScrollDir.X;
            Editor.Instance.EditorMenuBar.yToolStripMenuItem.IsChecked = EditClasses.EditorState.ScrollDirection == (int)ScrollDir.Y;

            EditClasses.EditorState.CountTilesSelectedInPixels = Settings.MyDefaults.EnablePixelModeDefault;

            EditClasses.EditorState.ShowEntityPathArrows = Settings.MyDefaults.ShowEntityArrowPathsDefault;

            EditClasses.EditorState.ShowWaterLevel = Settings.MyDefaults.ShowWaterEntityLevelDefault;
            EditClasses.EditorState.AlwaysShowWaterLevel = Settings.MyDefaults.AlwaysShowWaterLevelDefault;
            EditClasses.EditorState.SizeWaterLevelwithBounds = Settings.MyDefaults.SizeWaterLevelWithBoundsDefault;

            EditClasses.EditorState.ShowParallaxSprites = Settings.MyDefaults.ShowFullParallaxSpritesDefault;
            EditClasses.EditorState.PrioritizedEntityViewing = Settings.MyDefaults.PrioritizedObjectRenderingDefault;

            EditClasses.EditorState.ShowEntitySelectionBoxes = Settings.MyDefaults.ShowEntitySelectionBoxesDefault;

            EditClasses.EditorState.DebugStatsVisibleOnPanel = Settings.MyDefaults.ShowDebugStatsDefault;
            EditClasses.EditorState.UseLargeDebugStats = Settings.MyDefaults.LargeDebugStatsDefault;

            EditClasses.EditorState.GridCustomSize = Settings.MyDefaults.CustomGridSizeValue;
            Editor.Instance.EditorToolbar.CustomGridSizeAdjuster.Value = EditClasses.EditorState.GridCustomSize;

            EditClasses.EditorState.CollisionSAColour = Settings.MyDefaults.CollisionSAColour;
            EditClasses.EditorState.CollisionLRDColour = Settings.MyDefaults.CollisionLRDColour;
            EditClasses.EditorState.CollisionTOColour = Settings.MyDefaults.CollisionTOColour;

            EditClasses.EditorState.GridColor = Settings.MyDefaults.DefaultGridColor;
            EditClasses.EditorState.waterColor = Settings.MyDefaults.WaterEntityColorDefault;

            Editor.Instance.EditorToolbar.FasterNudgeValueNUD.Value = Settings.MyDefaults.FasterNudgeValue;





            var allLangItems = Editor.Instance.EditorMenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == Settings.MyDefaults.MenuLanguageDefault)
                    {
                        item.IsChecked = true;
                        EditClasses.EditorState.CurrentLanguage = item.Tag.ToString();
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
