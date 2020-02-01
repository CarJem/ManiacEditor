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
            Classes.Edit.SolutionState.ApplyEditEntitiesTransparency = Settings.MyDefaults.EditEntitiesTransparentLayersDefault;

            Classes.Edit.SolutionState.ScrollLocked = Settings.MyDefaults.ScrollLockDefault;
            Classes.Edit.SolutionState.ScrollDirection = (Settings.MyDefaults.ScrollLockDirectionDefault == true ? 1 : 0);

            Editor.Instance.EditorMenuBar.xToolStripMenuItem.IsChecked = Classes.Edit.SolutionState.ScrollDirection == (int)ScrollDir.X;
            Editor.Instance.EditorMenuBar.yToolStripMenuItem.IsChecked = Classes.Edit.SolutionState.ScrollDirection == (int)ScrollDir.Y;

            Classes.Edit.SolutionState.CountTilesSelectedInPixels = Settings.MyDefaults.EnablePixelModeDefault;

            Classes.Edit.SolutionState.ShowEntityPathArrows = Settings.MyDefaults.ShowEntityArrowPathsDefault;

            Classes.Edit.SolutionState.ShowWaterLevel = Settings.MyDefaults.ShowWaterEntityLevelDefault;
            Classes.Edit.SolutionState.AlwaysShowWaterLevel = Settings.MyDefaults.AlwaysShowWaterLevelDefault;
            Classes.Edit.SolutionState.SizeWaterLevelwithBounds = Settings.MyDefaults.SizeWaterLevelWithBoundsDefault;

            Classes.Edit.SolutionState.ShowParallaxSprites = Settings.MyDefaults.ShowFullParallaxSpritesDefault;
            Classes.Edit.SolutionState.PrioritizedEntityViewing = Settings.MyDefaults.PrioritizedObjectRenderingDefault;

            Classes.Edit.SolutionState.ShowEntitySelectionBoxes = Settings.MyDefaults.ShowEntitySelectionBoxesDefault;

            Classes.Edit.SolutionState.DebugStatsVisibleOnPanel = Settings.MyDefaults.ShowDebugStatsDefault;
            Classes.Edit.SolutionState.UseLargeDebugStats = Settings.MyDefaults.LargeDebugStatsDefault;

            Classes.Edit.SolutionState.GridCustomSize = Settings.MyDefaults.CustomGridSizeValue;
            Editor.Instance.EditorToolbar.CustomGridSizeAdjuster.Value = Classes.Edit.SolutionState.GridCustomSize;

            Classes.Edit.SolutionState.CollisionSAColour = Settings.MyDefaults.CollisionSAColour;
            Classes.Edit.SolutionState.CollisionLRDColour = Settings.MyDefaults.CollisionLRDColour;
            Classes.Edit.SolutionState.CollisionTOColour = Settings.MyDefaults.CollisionTOColour;

            Classes.Edit.SolutionState.GridColor = Settings.MyDefaults.DefaultGridColor;
            Classes.Edit.SolutionState.waterColor = Settings.MyDefaults.WaterEntityColorDefault;

            Editor.Instance.EditorToolbar.FasterNudgeValueNUD.Value = Settings.MyDefaults.FasterNudgeValue;





            var allLangItems = Editor.Instance.EditorMenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == Settings.MyDefaults.MenuLanguageDefault)
                    {
                        item.IsChecked = true;
                        Classes.Edit.SolutionState.CurrentLanguage = item.Tag.ToString();
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
