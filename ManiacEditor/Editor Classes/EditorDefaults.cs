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
            EditorStateModel.ApplyEditEntitiesTransparency = Settings.MyDefaults.EditEntitiesTransparentLayersDefault;

            EditorStateModel.ScrollLocked = Settings.MyDefaults.ScrollLockDefault;
            EditorStateModel.ScrollDirection = (Settings.MyDefaults.ScrollLockDirectionDefault == true ? 1 : 0);

            Editor.Instance.EditorMenuBar.xToolStripMenuItem.IsChecked = EditorStateModel.ScrollDirection == (int)ScrollDir.X;
            Editor.Instance.EditorMenuBar.yToolStripMenuItem.IsChecked = EditorStateModel.ScrollDirection == (int)ScrollDir.Y;

            EditorStateModel.CountTilesSelectedInPixels = Settings.MyDefaults.EnablePixelModeDefault;

            EditorStateModel.ShowEntityPathArrows = Settings.MyDefaults.ShowEntityArrowPathsDefault;

            EditorStateModel.ShowWaterLevel = Settings.MyDefaults.ShowWaterEntityLevelDefault;
            EditorStateModel.AlwaysShowWaterLevel = Settings.MyDefaults.AlwaysShowWaterLevelDefault;
            EditorStateModel.SizeWaterLevelwithBounds = Settings.MyDefaults.SizeWaterLevelWithBoundsDefault;

            EditorStateModel.ShowParallaxSprites = Settings.MyDefaults.ShowFullParallaxSpritesDefault;
            EditorStateModel.PrioritizedEntityViewing = Settings.MyDefaults.PrioritizedObjectRenderingDefault;

            EditorStateModel.ShowEntitySelectionBoxes = Settings.MyDefaults.ShowEntitySelectionBoxesDefault;

            EditorStateModel.DebugStatsVisibleOnPanel = Settings.MyDefaults.ShowDebugStatsDefault;
            EditorStateModel.UseLargeDebugStats = Settings.MyDefaults.LargeDebugStatsDefault;

            EditorStateModel.GridCustomSize = Settings.MyDefaults.CustomGridSizeValue;
            Editor.Instance.EditorToolbar.CustomGridSizeAdjuster.Value = EditorStateModel.GridCustomSize;

            EditorStateModel.CollisionSAColour = Settings.MyDefaults.CollisionSAColour;
            EditorStateModel.CollisionLRDColour = Settings.MyDefaults.CollisionLRDColour;
            EditorStateModel.CollisionTOColour = Settings.MyDefaults.CollisionTOColour;

            EditorStateModel.GridColor = Settings.MyDefaults.DefaultGridColor;
            EditorStateModel.waterColor = Settings.MyDefaults.WaterEntityColorDefault;

            Editor.Instance.EditorToolbar.FasterNudgeValueNUD.Value = Settings.MyDefaults.FasterNudgeValue;





            var allLangItems = Editor.Instance.EditorMenuBar.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == Settings.MyDefaults.MenuLanguageDefault)
                    {
                        item.IsChecked = true;
                        EditorStateModel.CurrentLanguage = item.Tag.ToString();
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
