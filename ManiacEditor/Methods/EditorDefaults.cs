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
            Classes.Editor.SolutionState.ApplyEditEntitiesTransparency = Settings.MyDefaults.EditEntitiesTransparentLayersDefault;

            Classes.Editor.SolutionState.ScrollLocked = Settings.MyDefaults.ScrollLockDefault;
            Classes.Editor.SolutionState.ScrollDirection = (Settings.MyDefaults.ScrollLockDirectionDefault == true ? 1 : 0);

            Interfaces.Base.MapEditor.Instance.EditorMenuBar.xToolStripMenuItem.IsChecked = Classes.Editor.SolutionState.ScrollDirection == (int)ScrollDir.X;
            Interfaces.Base.MapEditor.Instance.EditorMenuBar.yToolStripMenuItem.IsChecked = Classes.Editor.SolutionState.ScrollDirection == (int)ScrollDir.Y;

            Classes.Editor.SolutionState.CountTilesSelectedInPixels = Settings.MyDefaults.EnablePixelModeDefault;

            Classes.Editor.SolutionState.ShowEntityPathArrows = Settings.MyDefaults.ShowEntityArrowPathsDefault;

            Classes.Editor.SolutionState.ShowWaterLevel = Settings.MyDefaults.ShowWaterEntityLevelDefault;
            Classes.Editor.SolutionState.AlwaysShowWaterLevel = Settings.MyDefaults.AlwaysShowWaterLevelDefault;
            Classes.Editor.SolutionState.SizeWaterLevelwithBounds = Settings.MyDefaults.SizeWaterLevelWithBoundsDefault;

            Classes.Editor.SolutionState.ShowParallaxSprites = Settings.MyDefaults.ShowFullParallaxSpritesDefault;
            Classes.Editor.SolutionState.PrioritizedEntityViewing = Settings.MyDefaults.PrioritizedObjectRenderingDefault;

            Classes.Editor.SolutionState.ShowEntitySelectionBoxes = Settings.MyDefaults.ShowEntitySelectionBoxesDefault;

            Classes.Editor.SolutionState.DebugStatsVisibleOnPanel = Settings.MyDefaults.ShowDebugStatsDefault;
            Classes.Editor.SolutionState.UseLargeDebugStats = Settings.MyDefaults.LargeDebugStatsDefault;

            Classes.Editor.SolutionState.GridCustomSize = Settings.MyDefaults.CustomGridSizeValue;
            Interfaces.Base.MapEditor.Instance.EditorToolbar.CustomGridSizeAdjuster.Value = Classes.Editor.SolutionState.GridCustomSize;

            Classes.Editor.SolutionState.CollisionSAColour = Settings.MyDefaults.CollisionSAColour;
            Classes.Editor.SolutionState.CollisionLRDColour = Settings.MyDefaults.CollisionLRDColour;
            Classes.Editor.SolutionState.CollisionTOColour = Settings.MyDefaults.CollisionTOColour;

            Classes.Editor.SolutionState.GridColor = Settings.MyDefaults.DefaultGridColor;
            Classes.Editor.SolutionState.waterColor = Settings.MyDefaults.WaterEntityColorDefault;

            Interfaces.Base.MapEditor.Instance.EditorToolbar.FasterNudgeValueNUD.Value = Settings.MyDefaults.FasterNudgeValue;





            var allLangItems = Interfaces.Base.MapEditor.Instance.EditorMenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == Settings.MyDefaults.MenuLanguageDefault)
                    {
                        item.IsChecked = true;
                        Classes.Editor.SolutionState.CurrentLanguage = item.Tag.ToString();
                    }
                }


            bool endSearch = false;
            var allButtonItems = Interfaces.Base.MapEditor.Instance.EditorMenuBar.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allButtonItems)
            {
                if (item.Tag != null)
                {
                    if (item.Tag.ToString() == Settings.MyDefaults.MenuButtonLayoutDefault && !endSearch)
                    {
                        item.IsChecked = true;
                        Interfaces.Base.MapEditor.Instance.MenuButtonChangedEvent(item.Tag.ToString());
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
                                Interfaces.Base.MapEditor.Instance.MenuButtonChangedEvent(subItem.Tag.ToString());
                                endSearch = true;
                            }
                        }
                    }
                }

            }


        }
    }
}
