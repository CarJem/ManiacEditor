using System.Linq;
using ManiacEditor.Enums;

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
            Classes.Core.SolutionState.ApplyEditEntitiesTransparency = Core.Settings.MyDefaults.EditEntitiesTransparentLayersDefault;

            Classes.Core.SolutionState.ScrollLocked = Core.Settings.MyDefaults.ScrollLockDefault;
            Classes.Core.SolutionState.ScrollDirection = (Core.Settings.MyDefaults.ScrollLockDirectionDefault == true ? 1 : 0);

            Controls.Base.MainEditor.Instance.EditorMenuBar.xToolStripMenuItem.IsChecked = Classes.Core.SolutionState.ScrollDirection == (int)ScrollDir.X;
            Controls.Base.MainEditor.Instance.EditorMenuBar.yToolStripMenuItem.IsChecked = Classes.Core.SolutionState.ScrollDirection == (int)ScrollDir.Y;

            Classes.Core.SolutionState.CountTilesSelectedInPixels = Core.Settings.MyDefaults.EnablePixelModeDefault;

            Classes.Core.SolutionState.ShowEntityPathArrows = Core.Settings.MyDefaults.ShowEntityArrowPathsDefault;

            Classes.Core.SolutionState.ShowWaterLevel = Core.Settings.MyDefaults.ShowWaterEntityLevelDefault;
            Classes.Core.SolutionState.AlwaysShowWaterLevel = Core.Settings.MyDefaults.AlwaysShowWaterLevelDefault;
            Classes.Core.SolutionState.SizeWaterLevelwithBounds = Core.Settings.MyDefaults.SizeWaterLevelWithBoundsDefault;

            Classes.Core.SolutionState.ShowParallaxSprites = Core.Settings.MyDefaults.ShowFullParallaxSpritesDefault;
            Classes.Core.SolutionState.PrioritizedEntityViewing = Core.Settings.MyDefaults.PrioritizedObjectRenderingDefault;

            Classes.Core.SolutionState.ShowEntitySelectionBoxes = Core.Settings.MyDefaults.ShowEntitySelectionBoxesDefault;

            Classes.Core.SolutionState.DebugStatsVisibleOnPanel = Core.Settings.MyDefaults.ShowDebugStatsDefault;
            Classes.Core.SolutionState.UseLargeDebugStats = Core.Settings.MyDefaults.LargeDebugStatsDefault;

            Classes.Core.SolutionState.GridCustomSize = Core.Settings.MyDefaults.CustomGridSizeValue;
            Controls.Base.MainEditor.Instance.EditorToolbar.CustomGridSizeAdjuster.Value = Classes.Core.SolutionState.GridCustomSize;

            Classes.Core.SolutionState.CollisionSAColour = Core.Settings.MyDefaults.CollisionSAColour;
            Classes.Core.SolutionState.CollisionLRDColour = Core.Settings.MyDefaults.CollisionLRDColour;
            Classes.Core.SolutionState.CollisionTOColour = Core.Settings.MyDefaults.CollisionTOColour;

            Classes.Core.SolutionState.GridColor = Core.Settings.MyDefaults.DefaultGridColor;
            Classes.Core.SolutionState.waterColor = Core.Settings.MyDefaults.WaterEntityColorDefault;

            Controls.Base.MainEditor.Instance.EditorToolbar.FasterNudgeValueNUD.Value = Core.Settings.MyDefaults.FasterNudgeValue;





            var allLangItems = Controls.Base.MainEditor.Instance.EditorMenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == Core.Settings.MyDefaults.MenuLanguageDefault)
                    {
                        item.IsChecked = true;
                        Classes.Core.SolutionState.CurrentLanguage = item.Tag.ToString();
                    }
                }


            bool endSearch = false;
            var allButtonItems = Controls.Base.MainEditor.Instance.EditorMenuBar.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allButtonItems)
            {
                if (item.Tag != null)
                {
                    if (item.Tag.ToString() == Core.Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
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
                            if (subItem.Tag.ToString() == Core.Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
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
    }
}
