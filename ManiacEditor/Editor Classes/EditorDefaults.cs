using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Editor.Instance.UIModes.ApplyEditEntitiesTransparency = Settings.MyDefaults.EditEntitiesTransparentLayersDefault;

            Editor.Instance.UIModes.ScrollLocked = Settings.MyDefaults.ScrollLockDefault;
            Editor.Instance.UIModes.ScrollDirection = (Settings.MyDefaults.ScrollLockDirectionDefault == true ? 1 : 0);

            Editor.Instance.xToolStripMenuItem.IsChecked = Editor.Instance.UIModes.ScrollDirection == (int)ScrollDir.X;
            Editor.Instance.yToolStripMenuItem.IsChecked = Editor.Instance.UIModes.ScrollDirection == (int)ScrollDir.Y;

            Editor.Instance.UIModes.EnablePixelCountMode = Settings.MyDefaults.EnablePixelModeDefault;

            Editor.Instance.UIModes.ShowEntityPathArrows = Settings.MyDefaults.ShowEntityArrowPathsDefault;

            Editor.Instance.UIModes.ShowWaterLevel = Settings.MyDefaults.ShowWaterEntityLevelDefault;
            Editor.Instance.UIModes.AlwaysShowWaterLevel = Settings.MyDefaults.AlwaysShowWaterLevelDefault;
            Editor.Instance.UIModes.SizeWaterLevelwithBounds = Settings.MyDefaults.SizeWaterLevelWithBoundsDefault;

            Editor.Instance.UIModes.ShowParallaxSprites = Settings.MyDefaults.ShowFullParallaxSpritesDefault;
            Editor.Instance.UIModes.PrioritizedEntityViewing = Settings.MyDefaults.PrioritizedObjectRenderingDefault;

            Editor.Instance.UIModes.ShowEntitySelectionBoxes = Settings.MyDefaults.ShowEntitySelectionBoxesDefault;

            Editor.Instance.UIModes.DebugStatsVisibleOnPanel = Settings.MyDefaults.ShowDebugStatsDefault;
            Editor.Instance.UIModes.UseLargeDebugStats = Settings.MyDefaults.LargeDebugStatsDefault;

            Editor.Instance.UIModes.GridCustomSize = Settings.MyDefaults.CustomGridSizeValue;
            Editor.Instance.CustomGridSizeAdjuster.Value = Editor.Instance.UIModes.GridCustomSize;

            Editor.Instance.UIModes.CollisionSAColour = Settings.MyDefaults.CollisionSAColour;
            Editor.Instance.UIModes.CollisionLRDColour = Settings.MyDefaults.CollisionLRDColour;
            Editor.Instance.UIModes.CollisionTOColour = Settings.MyDefaults.CollisionTOColour;

            Editor.Instance.UIModes.GridColor = Settings.MyDefaults.DefaultGridColor;
            Editor.Instance.UIModes.waterColor = Settings.MyDefaults.WaterEntityColorDefault;

            Editor.Instance.FasterNudgeValueNUD.Value = Settings.MyDefaults.FasterNudgeValue;





            var allLangItems = Editor.Instance.menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allLangItems)
                if (item != null)
                {
                    if (item.Tag.ToString() == Settings.MyDefaults.MenuLanguageDefault)
                    {
                        item.IsChecked = true;
                        Editor.Instance.UIModes.CurrentLanguage = item.Tag.ToString();
                    }
                }


            bool endSearch = false;
            var allButtonItems = Editor.Instance.menuButtonsToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
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
