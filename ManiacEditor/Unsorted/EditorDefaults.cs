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
            Classes.Editor.SolutionState.ApplyEditEntitiesTransparency = Core.Settings.MyDefaults.EditEntitiesTransparentLayersDefault;

            Classes.Editor.SolutionState.ScrollLocked = Core.Settings.MyDefaults.ScrollLockDefault;
            Classes.Editor.SolutionState.ScrollDirection = (Core.Settings.MyDefaults.ScrollLockDirectionDefault == true ? 1 : 0);

            Controls.Base.MainEditor.Instance.EditorMenuBar.xToolStripMenuItem.IsChecked = Classes.Editor.SolutionState.ScrollDirection == (int)ScrollDir.X;
            Controls.Base.MainEditor.Instance.EditorMenuBar.yToolStripMenuItem.IsChecked = Classes.Editor.SolutionState.ScrollDirection == (int)ScrollDir.Y;

            Classes.Editor.SolutionState.CountTilesSelectedInPixels = Core.Settings.MyDefaults.EnablePixelModeDefault;

            Classes.Editor.SolutionState.ShowEntityPathArrows = Core.Settings.MyDefaults.ShowEntityArrowPathsDefault;

            Classes.Editor.SolutionState.ShowWaterLevel = Core.Settings.MyDefaults.ShowWaterEntityLevelDefault;
            Classes.Editor.SolutionState.AlwaysShowWaterLevel = Core.Settings.MyDefaults.AlwaysShowWaterLevelDefault;
            Classes.Editor.SolutionState.SizeWaterLevelwithBounds = Core.Settings.MyDefaults.SizeWaterLevelWithBoundsDefault;

            Classes.Editor.SolutionState.ShowParallaxSprites = Core.Settings.MyDefaults.ShowFullParallaxSpritesDefault;
            Classes.Editor.SolutionState.PrioritizedEntityViewing = Core.Settings.MyDefaults.PrioritizedObjectRenderingDefault;

            Classes.Editor.SolutionState.ShowEntitySelectionBoxes = Core.Settings.MyDefaults.ShowEntitySelectionBoxesDefault;

            Classes.Editor.SolutionState.DebugStatsVisibleOnPanel = Core.Settings.MyDefaults.ShowDebugStatsDefault;
            Classes.Editor.SolutionState.UseLargeDebugStats = Core.Settings.MyDefaults.LargeDebugStatsDefault;

            Classes.Editor.SolutionState.GridCustomSize = Core.Settings.MyDefaults.CustomGridSizeValue;
            Controls.Base.MainEditor.Instance.EditorToolbar.CustomGridSizeAdjuster.Value = Classes.Editor.SolutionState.GridCustomSize;

            Classes.Editor.SolutionState.CollisionSAColour = Core.Settings.MyDefaults.CollisionSAColour;
            Classes.Editor.SolutionState.CollisionLRDColour = Core.Settings.MyDefaults.CollisionLRDColour;
            Classes.Editor.SolutionState.CollisionTOColour = Core.Settings.MyDefaults.CollisionTOColour;

            Classes.Editor.SolutionState.GridColor = Core.Settings.MyDefaults.DefaultGridColor;
            Classes.Editor.SolutionState.waterColor = Core.Settings.MyDefaults.WaterEntityColorDefault;

            Controls.Base.MainEditor.Instance.EditorToolbar.FasterNudgeValueNUD.Value = Core.Settings.MyDefaults.FasterNudgeValue;





            var allLangItems = Controls.Base.MainEditor.Instance.EditorMenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == Core.Settings.MyDefaults.MenuLanguageDefault)
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
