using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;


namespace ManiacEditor.Interfaces.EditorElements
{
    /// <summary>
    /// Interaction logic for Toolbar.xaml
    /// </summary>
    public partial class Toolbar : UserControl
    {
        public Toolbar()
        {
            InitializeComponent();

            EditFGLower.Click += EditFGLower_Click;
            EditFGLow.Click += EditFGLow_Click;
            EditFGHigh.Click += EditFGHigh_Click;
            EditFGHigher.Click += EditFGHigher_Click;

            EditFGLower.RightClick += EditFGLower_RightClick;
            EditFGLow.RightClick += EditFGLow_RightClick;
            EditFGHigh.RightClick += EditFGHigh_RightClick;
            EditFGHigher.RightClick += EditFGHigher_RightClick;
        }

        #region Action Events (MenuItems, Clicks, etc.)
        #region File Events
        private void NewSceneEvent(object sender, RoutedEventArgs e) { Editor.Instance.FileHandler.NewScene(); }
        public void OpenSceneEvent(object sender, RoutedEventArgs e) { Editor.Instance.FileHandler.OpenScene(); }
        public void SaveSceneEvent(object sender, RoutedEventArgs e) { Editor.Instance.FileHandler.Save(); }
        #endregion

        #region Animations DropDown (WIP)
        private void MovingPlatformsObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.Instance.Options.AllowMovingPlatformAnimations == false)
            {
                movingPlatformsObjectsToolStripMenuItem.IsChecked = true;
                Editor.Instance.Options.AllowMovingPlatformAnimations = true;
            }
            else
            {
                movingPlatformsObjectsToolStripMenuItem.IsChecked = false;
                Editor.Instance.Options.AllowMovingPlatformAnimations = false;
            }

        }

        private void SpriteFramesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.Instance.Options.AllowSpriteAnimations == false)
            {
                spriteFramesToolStripMenuItem.IsChecked = true;
                Editor.Instance.Options.AllowSpriteAnimations = true;
            }
            else
            {
                spriteFramesToolStripMenuItem.IsChecked = false;
                Editor.Instance.Options.AllowSpriteAnimations = false;
            }
        }

        private void ParallaxAnimationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.Instance.Options.ParallaxAnimationChecked == false)
            {
                parallaxAnimationMenuItem.IsChecked = true;
                Editor.Instance.Options.ParallaxAnimationChecked = true;
            }
            else
            {
                parallaxAnimationMenuItem.IsChecked = false;
                Editor.Instance.Options.ParallaxAnimationChecked = false;
            }
        }

        #endregion
        #region Spline Tool Events
        private void SplineShowLineCheckboxCheckChanged(object sender, RoutedEventArgs e)
        {
            Editor.Instance.Options.AdjustSplineGroupOptions(UserStateModel.SplineOption.ShowLines, SplineShowLineCheckbox.IsChecked.Value);
        }

        private void SplineShowPointsCheckboxCheckChanged(object sender, RoutedEventArgs e)
        {
            Editor.Instance.Options.AdjustSplineGroupOptions(UserStateModel.SplineOption.ShowPoints, SplineShowPointsCheckbox.IsChecked.Value);
        }

        private void SplineShowObjectsCheckboxCheckChanged(object sender, RoutedEventArgs e)
        {
            Editor.Instance.Options.AdjustSplineGroupOptions(UserStateModel.SplineOption.ShowObjects, SplineShowObjectsCheckbox.IsChecked.Value);
        }

        bool AllowSplineFreqeunceUpdate = true;
        bool AllowSplineUpdateEvent = true;

        private void SplineOptionsIDChangedEvent(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Editor.Instance.Options != null && Editor.Instance.UI != null && SplinePointSeperationSlider != null && SplinePointSeperationNUD != null && SplineGroupID != null && AllowSplineUpdateEvent)
            {
                SelectedSplineIDChangedEvent(SplineGroupID.Value.Value);
            }
        }

        public void SelectedSplineIDChangedEvent(int value)
        {
            AllowSplineUpdateEvent = false;
            Editor.Instance.Options.AllowSplineOptionsUpdate = false;
            SplineGroupID.Value = value;
            Editor.Instance.Options.SelectedSplineID = value;
            SplineSpawnID.Value = value;
            Editor.Instance.UI.UpdateSplineSettings(value);
            Editor.Instance.Options.AllowSplineOptionsUpdate = true;
            AllowSplineUpdateEvent = true;

        }

        private void SplinePointFrequenceChangedEvent(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!Editor.Instance.Options.AllowSplineOptionsUpdate) return;
            if (Editor.Instance.Options != null && Editor.Instance.UI != null && SplinePointSeperationNUD != null && SplinePointSeperationSlider != null && AllowSplineFreqeunceUpdate)
            {
                AllowSplineFreqeunceUpdate = false;
                int size = (int)SplinePointSeperationNUD.Value;
                SplinePointSeperationSlider.Value = size;
                Editor.Instance.Options.AdjustSplineGroupOptions(UserStateModel.SplineOption.Size, size);
                AllowSplineFreqeunceUpdate = true;
            }
        }

        private void SplinePointFrequenceChangedEvent(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!Editor.Instance.Options.AllowSplineOptionsUpdate) return;
            if (Editor.Instance.Options != null && Editor.Instance.UI != null && SplinePointSeperationSlider != null && SplinePointSeperationNUD != null && AllowSplineFreqeunceUpdate)
            {
                AllowSplineFreqeunceUpdate = false;
                int size = (int)SplinePointSeperationSlider.Value;
                SplinePointSeperationNUD.Value = size;
                Editor.Instance.Options.AdjustSplineGroupOptions(UserStateModel.SplineOption.Size, size);
                AllowSplineFreqeunceUpdate = true;
            }
        }

        private void SplineLineMode_Click(object sender, RoutedEventArgs e)
        {
            Editor.Instance.Options.AdjustSplineGroupOptions(UserStateModel.SplineOption.LineMode, SplineLineMode.IsChecked.Value);
        }

        private void SplineOvalMode_Click(object sender, RoutedEventArgs e)
        {
            Editor.Instance.Options.AdjustSplineGroupOptions(UserStateModel.SplineOption.OvalMode, SplineOvalMode.IsChecked.Value);
        }
        private void SplineSpawnRender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditorSolution.Entities != null && Editor.Instance.Options.AllowSplineOptionsUpdate)
            {
                var selectedItem = SelectedSplineRender.SelectedItem as TextBlock;
                if (selectedItem.Tag == null) return;
                if (selectedItem.Tag is RSDKv5.SceneObject)
                {
                    var obj = selectedItem.Tag as RSDKv5.SceneObject;
                    int splineID = Editor.Instance.Options.SelectedSplineID;
                    Editor.Instance.Options.AdjustSplineGroupOptions(UserStateModel.SplineOption.SpawnObject, EditorSolution.Entities.GenerateEditorEntity(new RSDKv5.SceneEntity(obj, 0)));
                    Editor.Instance.EntitiesToolbar?.UpdateEntityProperties(new List<RSDKv5.SceneEntity>() { Editor.Instance.Options.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity });

                    if (Editor.Instance.Options.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                        SplineRenderObjectName.Content = Editor.Instance.Options.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity.Object.Name.Name;
                    else
                        SplineRenderObjectName.Content = "None";

                }

            }
        }

        private void SplineRenderObjectName_Click(object sender, RoutedEventArgs e)
        {
            if (!SelectedSplineRender.IsDropDownOpen) SelectedSplineRender.IsDropDownOpen = true;
            else SelectedSplineRender.IsDropDownOpen = false;
        }

        #endregion
        #region Draw Tool Options Events
        bool AllowDrawBrushSizeChange = true;
        private void DrawToolSizeChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DrawToolSizeChanged();
        }

        private void DrawToolSizeChanged(bool wasSlider = false)
        {
            if (Editor.Instance != null)
            {
                if (Editor.Instance.Options != null && Editor.Instance.UI != null && DrawTileSizeNUD != null && DrawTileSizeSlider != null && AllowDrawBrushSizeChange)
                {
                    AllowDrawBrushSizeChange = false;
                    int size = (wasSlider ? (int)DrawTileSizeSlider.Value : (int)DrawTileSizeNUD.Value);
                    DrawTileSizeSlider.Value = size;
                    DrawTileSizeNUD.Value = size;
                    Editor.Instance.Options.DrawBrushSize = size;
                    AllowDrawBrushSizeChange = true;
                }
            }
        }

        private void DrawToolSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DrawToolSizeChanged(true);
        }

        #endregion
        private void ToggleMagnetToolEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.UseMagnetMode ^= true; }
        private void UndoEvent(object sender, RoutedEventArgs e) { Editor.Instance.EditorUndo(); }
        private void RedoEvent(object sender, RoutedEventArgs e) { Editor.Instance.EditorRedo(); }
        private void ZoomInEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.ZoomIn(sender, e); }
        private void ZoomOutEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.ZoomOut(sender, e); }
        private void ToggleSelectToolEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.SelectionMode(); }
        private void TogglePointerToolEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.PointerMode(); }
        private void ToggleDrawToolEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.DrawMode(); }
        private void ToggleInteractionToolEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.InteractionMode(); }
        private void ToggleSplineToolEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.SplineMode(); }
        private void ToggleChunksToolEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ChunksMode(); }
        public void ReloadToolStripButton_Click(object sender, RoutedEventArgs e) { Editor.Instance.UI.ReloadSpritesAndTextures(); }
        public void ToggleSlotIDEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ShowTileID ^= true; }
        private void FasterNudgeValueNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e) { if (FasterNudgeValueNUD.Value != null) { Editor.Instance.Options.FasterNudgeAmount = FasterNudgeValueNUD.Value.Value; } }
        public void ApplyEditEntitiesTransparencyEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ApplyEditEntitiesTransparency ^= true; }
        public void ShowCollisionAEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ShowCollisionA ^= true; }
        public void ShowCollisionBEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ShowCollisionB ^= true; }
        private void ShowFlippedTileHelperEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ShowFlippedTileHelper ^= true; }
        public void EnableEncorePaletteEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.UseEncoreColors ^= true; }
        private void RunSceneEvent(object sender, RoutedEventArgs e) { Editor.Instance.InGame.RunScene(); }
        private void UseNormalCollisionEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.CollisionPreset = 0; }
        private void UseInvertedCollisionEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.CollisionPreset = 1; }
        private void UseCustomCollisionEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.CollisionPreset = 2; }


        #region Collision Slider Events
        private void CollisionOpacitySliderValueChangedEvent(object sender, RoutedPropertyChangedEventArgs<double> e) { Editor.Instance.UIEvents?.CollisionOpacitySliderValueChanged(sender, e); }
        #endregion

        #region Magnet Events

        private void Magnet8x8Event(object sender, RoutedEventArgs e) { Editor.Instance.Options.MagnetSize = 8; }
        private void Magnet16x16Event(object sender, RoutedEventArgs e) { Editor.Instance.Options.MagnetSize = 16; }
        private void Magnet32x32Event(object sender, RoutedEventArgs e) { Editor.Instance.Options.MagnetSize = 32; }
        private void Magnet64x64Event(object sender, RoutedEventArgs e) { Editor.Instance.Options.MagnetSize = 64; }
        private void MagnetCustomEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.MagnetSize = -1; }
        private void CustomMagnetSizeAdjuster_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Editor.Instance.Options.CustomMagnetSize = CustomMagnetSizeAdjuster.Value.Value;
        }

        private void EnableMagnetXAxisLockEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.UseMagnetXAxis ^= true; }
        private void EnableMagnetYAxisLockEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.UseMagnetYAxis ^= true; }

        #endregion

        #region Grid Events
        public void ToggleGridEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ShowGrid ^= true; }
        private void SetGrid16x16Event(object sender, RoutedEventArgs e) { Editor.Instance.Options.GridSize = 16; }
        private void SetGrid128x128Event(object sender, RoutedEventArgs e) { Editor.Instance.Options.GridSize = 128; }
        private void SetGrid256x256Event(object sender, RoutedEventArgs e) { Editor.Instance.Options.GridSize = 256; }
        private void SetGridCustomSizeEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.GridSize = -1; }

        private void CustomGridSizeAdjuster_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Editor.Instance.Options.GridCustomSize = CustomGridSizeAdjuster.Value.Value;
            Editor.Instance.Options.GridSize = -1;
        }
        #endregion

        #region Apps
        private void TileManiacEditTileEvent(object sender, RoutedEventArgs e) { EditorLaunch.TileManiacIntergration(); }
        private void RSDKUnpackerEvent(object sender, RoutedEventArgs e) { EditorLaunch.RSDKUnpacker(); }
        private void SonicManiaHeadless(object sender, RoutedEventArgs e) { EditorLaunch.SonicManiaHeadless(); }
        private void MenuAppsCheatEngine_Click(object sender, RoutedEventArgs e) { EditorLaunch.CheatEngine(); }
        private void ModManager(object sender, RoutedEventArgs e) { EditorLaunch.ManiaModManager(); }
        private void TileManiacNormal(object sender, RoutedEventArgs e) { EditorLaunch.TileManiacNormal(); }
        private void InsanicManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.InsanicManiac(); }
        private void RSDKAnnimationEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.RSDKAnnimationEditor(); }
        private void RenderListManagerToolstripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.RenderListManager(); }
        private void ColorPaletteEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.ManiaPal(sender, e); }
        private void ManiaPalMenuItem_SubmenuOpened(object sender, RoutedEventArgs e) { EditorLaunch.ManiaPalSubmenuOpened(sender, e); }
        private void DuplicateObjectIDHealerToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.DuplicateObjectIDHealer(); }
        #endregion

        #region Folders
        private void OpenSceneFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.OpenSceneFolder(); }
        private void OpenManiacEditorFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.OpenManiacEditorFolder(); }
        private void OpenManiacEditorFixedSettingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.OpenManiacEditorFixedSettingsFolder(); }
        private void OpenManiacEditorPortableSettingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.OpenManiacEditorPortableSettingsFolder(); }
        private void OpenDataDirectoryFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.OpenDataDirectory(); }
        private void OpenSonicManiaFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.OpenSonicManiaFolder(); }
        private void OpenASavedPlaceToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e) { EditorLaunch.OpenASavedPlaceDropDownOpening(sender, e); }
        private void OpenASavedPlaceToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e) { EditorLaunch.OpenASavedPlaceDropDownClosed(sender, e); }
        private void OpenAResourcePackFolderToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e) { EditorLaunch.OpenAResourcePackFolderDropDownOpening(sender, e); }
        private void OpenAResourcePackFolderToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e) { EditorLaunch.OpenAResourcePackFolderDropDownClosed(sender, e); }
        #endregion

        #region Settings and Other Menu Events
        public void AboutScreenEvent(object sender, RoutedEventArgs e) { EditorLaunch.AboutScreen(); }
        public void ImportObjectsToolStripMenuItem_Click(Window window = null) { EditorLaunch.ImportObjectsToolStripMenuItem_Click(window); }
        public void ImportObjectsWithMegaList(Window window = null) { EditorLaunch.ImportObjectsWithMegaList(window); }
        public void ImportSoundsEvent(object sender, RoutedEventArgs e) { EditorLaunch.ImportSoundsToolStripMenuItem_Click(sender, e); }
        public void ImportSoundsEvent(Window window = null) { EditorLaunch.ImportSounds(window); }
        private void LayerManagerEvent(object sender, RoutedEventArgs e) { EditorLaunch.LayerManager(sender, e); }
        private void ManiacINIEditorEvent(object sender, RoutedEventArgs e) { EditorLaunch.ManiacINIEditor(sender, e); }
        private void ChangePrimaryBackgroundColorEvent(object sender, RoutedEventArgs e) { EditorLaunch.ChangePrimaryBackgroundColor(sender, e); }
        private void ChangeSecondaryBackgroundColorEvent(object sender, RoutedEventArgs e) { EditorLaunch.ChangeSecondaryBackgroundColor(sender, e); }
        public void ObjectManagerEvent(object sender, RoutedEventArgs e) { EditorLaunch.ObjectManager(); }
        private void InGameOptionsMenuEvent(object sender, RoutedEventArgs e) { EditorLaunch.InGameSettings(); }
        private void WikiLinkEvent(object sender, RoutedEventArgs e) { EditorLaunch.WikiLink(); }
        public void OptionsMenuEvent(object sender, RoutedEventArgs e) { EditorLaunch.OptionsMenu(); }
        private void ControlsMenuEvent(object sender, RoutedEventArgs e) { EditorLaunch.ControlMenu(); }
        #endregion

        #region Game Running Events
        private void MoveThePlayerToHere(object sender, RoutedEventArgs e) { Editor.Instance.InGame.MoveThePlayerToHere(); }
        private void SetPlayerRespawnToHere(object sender, RoutedEventArgs e) { Editor.Instance.InGame.SetPlayerRespawnToHere(); }
        private void MoveCheckpoint(object sender, RoutedEventArgs e) { Editor.Instance.InGame.CheckpointSelected = true; }
        private void RemoveCheckpoint(object sender, RoutedEventArgs e) { Editor.Instance.InGame.UpdateCheckpoint(new System.Drawing.Point(0, 0), false); }
        private void AssetReset(object sender, RoutedEventArgs e) { Editor.Instance.InGame.AssetReset(); }
        private void RestartScene(object sender, RoutedEventArgs e) { Editor.Instance.InGame.RestartScene(); }
        private void TrackThePlayer(object sender, RoutedEventArgs e) { Editor.Instance.InGame.TrackthePlayer(sender, e); }
        private void UpdateInGameMenuItems(object sender, RoutedEventArgs e) { Editor.Instance.InGame.UpdateRunSceneDropdown(); }
        #endregion

        #endregion

        #region Layer Toolbar Events
        private void LayerShowButton_Click(ToggleButton button, string desc)
        {
            if (button.IsChecked.Value)
            {
                button.IsChecked = false;
                button.ToolTip = "Show " + desc;
            }
            else
            {
                button.IsChecked = true;
                button.ToolTip = "Hide " + desc;
            }
        }
        private void ShowFGLow_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowFGLow, "Layer FG Low");
        }
        private void ShowFGHigh_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowFGHigh, "Layer FG High");
        }
        private void ShowFGHigher_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowFGHigher, "Layer FG Higher");
        }
        private void ShowFGLower_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowFGLower, "Layer FG Lower");
        }
        private void ShowEntities_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowEntities, "EditorSolution.Entities");
        }
        private void ShowAnimations_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowAnimations, "Animations");
        }

        private void ShowAnimations_Checked(object sender, RoutedEventArgs e)
        {
            Editor.Instance.Options.AllowAnimations = true;
        }

        private void ShowAnimations_Unchecked(object sender, RoutedEventArgs e)
        {
            Editor.Instance.Options.AllowAnimations = false;
        }

        private void LayerEditButton_Click(EditLayerToggleButton button, MouseButton ClickType)
        {


            if (Editor.Instance.Options.MultiLayerEditMode)
            {
                if (button == EditEntities) EditEntitiesMode();
                else if (ClickType == MouseButton.Left) LayerA();
                else if (ClickType == MouseButton.Right) LayerB();
            }
            else
            {
                if (ClickType == MouseButton.Left) Normal();
            }
            Editor.Instance.UI.UpdateControls();


            void EditEntitiesMode()
            {
                Editor.Instance.Deselect(false);
                if (!button.IsCheckedN.Value)
                {
                    button.IsCheckedN = false;
                }
                else
                {

                    EditEntities.IsCheckedN = true;

                    EditFGLow.IsCheckedN = false;
                    EditFGHigh.IsCheckedN = false;
                    EditFGLower.IsCheckedN = false;
                    EditFGHigher.IsCheckedN = false;
                    EditFGLow.IsCheckedA = false;
                    EditFGHigh.IsCheckedA = false;
                    EditFGLower.IsCheckedA = false;
                    EditFGHigher.IsCheckedA = false;
                    EditFGLow.IsCheckedB = false;
                    EditFGHigh.IsCheckedB = false;
                    EditFGLower.IsCheckedB = false;
                    EditFGHigher.IsCheckedB = false;
                }

                foreach (var elb in Editor.Instance.ExtraLayerEditViewButtons.Values)
                {
                    elb.IsCheckedN = false;
                    elb.IsCheckedA = false;
                    elb.IsCheckedB = false;
                }

            }

            void Normal()
            {
                Editor.Instance.Deselect(false);
                if (!button.IsCheckedN.Value)
                {
                    button.IsCheckedN = false;
                }
                else
                {
                    EditFGLow.IsCheckedN = false;
                    EditFGHigh.IsCheckedN = false;
                    EditFGLower.IsCheckedN = false;
                    EditFGHigher.IsCheckedN = false;
                    EditEntities.IsCheckedN = false;
                    button.IsCheckedN = true;
                }

                foreach (var elb in Editor.Instance.ExtraLayerEditViewButtons.Values)
                {
                    if (elb != button) elb.IsCheckedN = false;
                }



            }

            void LayerA()
            {
                Editor.Instance.Deselect(false);
                if (!button.IsCheckedA.Value)
                {
                    button.IsCheckedA = false;
                }
                else
                {
                    EditFGLow.IsCheckedA = false;
                    EditFGHigh.IsCheckedA = false;
                    EditFGLower.IsCheckedA = false;
                    EditFGHigher.IsCheckedA = false;
                    EditEntities.IsCheckedN = false;
                    button.IsCheckedA = true;
                }

                foreach (var elb in Editor.Instance.ExtraLayerEditViewButtons.Values)
                {
                    if (elb != button) elb.IsCheckedA = false;
                }
            }
            void LayerB()
            {
                Editor.Instance.Deselect(false);
                if (!button.IsCheckedB.Value)
                {
                    button.IsCheckedB = false;
                }
                else
                {
                    EditFGLow.IsCheckedB = false;
                    EditFGHigh.IsCheckedB = false;
                    EditFGLower.IsCheckedB = false;
                    EditFGHigher.IsCheckedB = false;
                    EditEntities.IsCheckedN = false;
                    button.IsCheckedB = true;
                }

                foreach (var elb in Editor.Instance.ExtraLayerEditViewButtons.Values)
                {
                    if (elb != button) elb.IsCheckedB = false;
                }
            }
        }
        private void EditFGLow_Click(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGLow, MouseButton.Left);
        }
        private void EditFGLow_RightClick(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGLow, MouseButton.Right);
        }
        private void EditFGHigh_Click(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGHigh, MouseButton.Left);
        }
        private void EditFGHigh_RightClick(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGHigh, MouseButton.Right);
        }
        private void EditFGLower_Click(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGLower, MouseButton.Left);
        }
        private void EditFGLower_RightClick(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGLower, MouseButton.Right);
        }
        private void EditFGHigher_Click(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGHigher, MouseButton.Left);
        }
        private void EditFGHigher_RightClick(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGHigher, MouseButton.Right);
        }
        private void EditEntities_Click(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditEntities, MouseButton.Left);
        }
        public void SetupLayerButtons()
        {
            TearDownExtraLayerButtons();
            IList<EditLayerToggleButton> _extraLayerEditButtons = new List<EditLayerToggleButton>(); //Used for Extra Layer Edit Buttons
            IList<EditLayerToggleButton> _extraLayerViewButtons = new List<EditLayerToggleButton>(); //Used for Extra Layer View Buttons

            //EDIT BUTTONS
            foreach (EditorSolution.EditorLayer el in EditorSolution.CurrentScene.OtherLayers)
            {
                EditLayerToggleButton tsb = new EditLayerToggleButton()
                {
                    Text = el.Name,
                    LayerName = "Edit" + el.Name
                };
                LayerToolbar.Items.Add(tsb);
                tsb.DualSelect = true;
                tsb.TextForeground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(System.Drawing.Color.LawnGreen.A, System.Drawing.Color.LawnGreen.R, System.Drawing.Color.LawnGreen.G, System.Drawing.Color.LawnGreen.B));
                tsb.RightClick += AdHocLayerEdit_RightClick;
                tsb.IsLayerOptionsEnabled = true;

                tsb.Click += AdHocLayerEdit_Click;

                _extraLayerEditButtons.Add(tsb);
            }

            //EDIT BUTTONS SEPERATOR
            Separator tss = new Separator();
            LayerToolbar.Items.Add(tss);
            Editor.Instance.ExtraLayerSeperators.Add(tss);

            //VIEW BUTTONS
            foreach (EditorSolution.EditorLayer el in EditorSolution.CurrentScene.OtherLayers)
            {
                EditLayerToggleButton tsb = new EditLayerToggleButton()
                {
                    Text = el.Name,
                    LayerName = "Show" + el.Name.Replace(" ", "")
                };
                LayerToolbar.Items.Insert(LayerToolbar.Items.IndexOf(extraViewLayersSeperator), tsb);
                tsb.TextForeground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, System.Drawing.Color.FromArgb(0x33AD35).R, System.Drawing.Color.FromArgb(0x33AD35).G, System.Drawing.Color.FromArgb(0x33AD35).B));
                tsb.IsLayerOptionsEnabled = true;


                _extraLayerViewButtons.Add(tsb);
            }

            //EDIT + VIEW BUTTONS LIST
            for (int i = 0; i < _extraLayerViewButtons.Count; i++)
            {
                Editor.Instance.ExtraLayerEditViewButtons.Add(_extraLayerViewButtons[i], _extraLayerEditButtons[i]);
            }

            UpdateDualButtonsControlsForLayer(Editor.Instance.FGLow, ShowFGLow, EditFGLow);
            UpdateDualButtonsControlsForLayer(Editor.Instance.FGHigh, ShowFGHigh, EditFGHigh);
            UpdateDualButtonsControlsForLayer(Editor.Instance.FGLower, ShowFGLower, EditFGLower);
            UpdateDualButtonsControlsForLayer(Editor.Instance.FGHigher, ShowFGHigher, EditFGHigher);
        }
        public void TearDownExtraLayerButtons()
        {
            foreach (var elb in Editor.Instance.ExtraLayerEditViewButtons)
            {
                LayerToolbar.Items.Remove(elb.Key);
                elb.Value.Click -= AdHocLayerEdit_Click;
                elb.Value.RightClick -= AdHocLayerEdit_RightClick;
                LayerToolbar.Items.Remove(elb.Value);
            }
            Editor.Instance.ExtraLayerEditViewButtons.Clear();


            foreach (var els in Editor.Instance.ExtraLayerSeperators)
            {
                LayerToolbar.Items.Remove(els);
            }
            Editor.Instance.ExtraLayerSeperators.Clear();

        }
        /// <summary>
        /// Given a scene layer, configure the given visibiltiy and edit buttons which will control that layer.
        /// </summary>
        /// <param name="layer">The layer of the scene from which to extract a name.</param>
        /// <param name="visibilityButton">The button which controls the visibility of the layer.</param>
        /// <param name="editButton">The button which controls editing the layer.</param>
        private void UpdateDualButtonsControlsForLayer(EditorSolution.EditorLayer layer, ToggleButton visibilityButton, EditLayerToggleButton editButton)
        {
            bool layerValid = layer != null;
            visibilityButton.IsChecked = layerValid;
            if (layerValid)
            {
                string name = layer.Name;
                visibilityButton.Content = name;
                editButton.Text = name.ToString();
            }
        }
        private void AdHocLayerEdit_RightClick(object sender, RoutedEventArgs e)
        {
            AdHocLayerEdit(sender, MouseButton.Right);
        }
        private void AdHocLayerEdit_Click(object sender, RoutedEventArgs e)
        {
            AdHocLayerEdit(sender, MouseButton.Left);
        }
        private void AdHocLayerEdit(object sender, MouseButton ClickType)
        {
            if (ClickType == MouseButton.Left && !Editor.Instance.Options.MultiLayerEditMode) Normal();
            else if (ClickType == MouseButton.Left && Editor.Instance.Options.MultiLayerEditMode) LayerA();
            else if (ClickType == MouseButton.Right && Editor.Instance.Options.MultiLayerEditMode) LayerB();

            void Normal()
            {
                EditLayerToggleButton tsb = sender as EditLayerToggleButton;
                Editor.Instance.Deselect(false);
                if (tsb.IsCheckedN.Value)
                {
                    if (!ManiacEditor.Settings.MySettings.KeepLayersVisible)
                    {
                        ShowFGLow.IsChecked = false;
                        ShowFGHigh.IsChecked = false;
                        ShowFGLower.IsChecked = false;
                        ShowFGHigher.IsChecked = false;
                    }
                    EditFGLow.ClearCheckedItems(3);
                    EditFGHigh.ClearCheckedItems(3);
                    EditFGLower.ClearCheckedItems(3);
                    EditFGHigher.ClearCheckedItems(3);
                    EditEntities.ClearCheckedItems(3);

                    foreach (var elb in Editor.Instance.ExtraLayerEditViewButtons)
                    {
                        if (elb.Value != tsb)
                        {
                            elb.Value.IsCheckedN = false;
                        }
                    }
                }
            }
            void LayerA()
            {
                EditLayerToggleButton tsb = sender as EditLayerToggleButton;
                Editor.Instance.Deselect(false);
                if (tsb.IsCheckedA.Value)
                {
                    if (!ManiacEditor.Settings.MySettings.KeepLayersVisible)
                    {
                        ShowFGLow.IsChecked = false;
                        ShowFGHigh.IsChecked = false;
                        ShowFGLower.IsChecked = false;
                        ShowFGHigher.IsChecked = false;
                    }
                    EditFGLow.ClearCheckedItems(1);
                    EditFGHigh.ClearCheckedItems(1);
                    EditFGLower.ClearCheckedItems(1);
                    EditFGHigher.ClearCheckedItems(1);
                    EditEntities.ClearCheckedItems(1);

                    foreach (var elb in Editor.Instance.ExtraLayerEditViewButtons)
                    {
                        if (elb.Value != tsb)
                        {
                            elb.Value.IsCheckedA = false;
                        }
                    }
                }
            }
            void LayerB()
            {
                EditLayerToggleButton tsb = sender as EditLayerToggleButton;
                Editor.Instance.Deselect(false);
                if (tsb.IsCheckedB.Value)
                {
                    if (!ManiacEditor.Settings.MySettings.KeepLayersVisible)
                    {
                        ShowFGLow.IsChecked = false;
                        ShowFGHigh.IsChecked = false;
                        ShowFGLower.IsChecked = false;
                        ShowFGHigher.IsChecked = false;
                    }
                    EditFGLow.ClearCheckedItems(2);
                    EditFGHigh.ClearCheckedItems(2);
                    EditFGLower.ClearCheckedItems(2);
                    EditFGHigher.ClearCheckedItems(2);
                    EditEntities.ClearCheckedItems(2);

                    foreach (var elb in Editor.Instance.ExtraLayerEditViewButtons)
                    {
                        if (elb.Value != tsb)
                        {
                            elb.Value.IsCheckedB = false;
                        }
                    }
                }
            }

            Editor.Instance.UI.UpdateControls();
        }
        #endregion

        #region Mod Config List Stuff
        public MenuItem CreateModConfigMenuItem(int i)
        {
            MenuItem newItem = new MenuItem()
            {
                Header = ManiacEditor.Settings.MySettings.ModLoaderConfigsNames[i],
                Tag = ManiacEditor.Settings.MySettings.ModLoaderConfigs[i]
            };
            newItem.Click += ModConfigItemClicked;
            if (newItem.Tag.ToString() == ManiacEditor.Settings.MySettings.LastModConfig) newItem.IsChecked = true;
            return newItem;
        }
        private void ModConfigItemClicked(object sender, RoutedEventArgs e)
        {
            var modConfig_CheckedItem = (sender as MenuItem);
            SelectConfigToolStripMenuItem_Click(modConfig_CheckedItem);
            ManiacEditor.Settings.MySettings.LastModConfig = modConfig_CheckedItem.Tag.ToString();
        }
        public void EditConfigsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Interfaces.WPF_UI.ConfigManager configManager = new Interfaces.WPF_UI.ConfigManager();
            configManager.Owner = Window.GetWindow(this);
            configManager.ShowDialog();

            // TODO: Fix NullReferenceException on Settings.mySettings.modConfigs
            selectConfigToolStripMenuItem.Items.Clear();
            for (int i = 0; i < ManiacEditor.Settings.MySettings.ModLoaderConfigs.Count; i++)
            {
                selectConfigToolStripMenuItem.Items.Add(CreateModConfigMenuItem(i));
            }
        }
        private void SelectConfigToolStripMenuItem_Click(MenuItem modConfig_CheckedItem)
        {
            var allItems = selectConfigToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allItems)
            {
                item.IsChecked = false;
            }
            modConfig_CheckedItem.IsChecked = true;

        }
        #endregion

        #region Custom Color Picker Events
        private void comboBox8_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Grid Default Color
            if (e.NewValue.Value != null)
            {
                Editor.Instance.Options.GridColor = Extensions.ColorConvertToDrawing(e.NewValue.Value);
            }
        }

        private void comboBox7_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Water Color
            if (e.NewValue.Value != null)
            {
                Editor.Instance.Options.waterColor = Extensions.ColorConvertToDrawing(e.NewValue.Value);
            }
        }

        private void comboBox6_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Collision Solid(Top Only) Color
            if (e.NewValue.Value != null)
            {
                Editor.Instance.Options.CollisionTOColour = Extensions.ColorConvertToDrawing(e.NewValue.Value);
                Editor.Instance.RefreshCollisionColours(true);
            }
        }

        private void comboBox5_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Collision Solid(LRD) Color
            if (e.NewValue.Value != null)
            {
                Editor.Instance.Options.CollisionLRDColour = Extensions.ColorConvertToDrawing(e.NewValue.Value);
                Editor.Instance.RefreshCollisionColours(true);
            }
        }

        private void comboBox4_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Collision Solid(All) Color
            if (e.NewValue.Value != null)
            {
                Editor.Instance.Options.CollisionSAColour = Extensions.ColorConvertToDrawing(e.NewValue.Value);
                Editor.Instance.RefreshCollisionColours(true);
            }
        }

        private void CollisionColorPickerClosed(object sender, RoutedEventArgs e)
        {
            Editor.Instance.ReloadSpecificTextures(sender, e);
            Editor.Instance.RefreshCollisionColours(true);
        }


        #endregion

        public void UpdateTooltips()
        {
            New.ToolTip = "New Scene" + KeyBindPraser("New", true);
            Open.ToolTip = "Open Scene" + KeyBindPraser("Open", true);
            Save.ToolTip = "Save Scene" + KeyBindPraser("_Save", true);
            RunSceneButton.ToolTip = "Run Scene" + KeyBindPraser("RunScene", true, true);
            ReloadButton.ToolTip = "Reload Tiles and Sprites" + KeyBindPraser("RefreshResources", true, true);
            PointerToolButton.ToolTip = "Pointer Tool" + KeyBindPraser("PointerTool", true);
            MagnetMode.ToolTip = "Magnet Mode" + KeyBindPraser("MagnetTool", true);
            ZoomInButton.ToolTip = "Zoom In (Ctrl + Wheel Up)";
            ZoomOutButton.ToolTip = "Zoom In (Ctrl + Wheel Down)";
            SelectToolButton.ToolTip = "Selection Tool" + KeyBindPraser("SelectTool", true);
            DrawToolButton.ToolTip = "Draw Tool" + KeyBindPraser("DrawTool", true);
            InteractionToolButton.ToolTip = "Interaction Tool";
            ShowCollisionAButton.ToolTip = "Show Collision Layer A" + KeyBindPraser("ShowPathA", true, true);
            ShowCollisionBButton.ToolTip = "Show Collision Layer B" + KeyBindPraser("ShowPathB", true, true);
            FlipAssistButton.ToolTip = "Show Flipped Tile Helper";
            ChunksToolButton.ToolTip = "Stamp Tool" + KeyBindPraser("StampTool", true);
            SplineToolButton.ToolTip = "Spline Tool" + KeyBindPraser("SplineTool", true);
            EncorePaletteButton.ToolTip = "Show Encore Colors";
            ShowTileIDButton.ToolTip = "Toggle Tile ID Visibility" + KeyBindPraser("ShowTileID", true, true);
            ShowGridButton.ToolTip = "Toggle Grid Visibility" + KeyBindPraser("ShowGrid", true, true);
        }

        public string KeyBindPraser(string keyRefrence, bool tooltip = false, bool nonRequiredBinding = false)
        {
            string nullString = (nonRequiredBinding ? "" : "N/A");
            if (nonRequiredBinding && tooltip) nullString = "None";
            List<string> keyBindList = new List<string>();
            List<string> keyBindModList = new List<string>();

            if (!Extensions.KeyBindsSettingExists(keyRefrence)) return nullString;

            if (Properties.KeyBinds.Default == null) return nullString;

            var keybindDict = Properties.KeyBinds.Default[keyRefrence] as StringCollection;
            if (keybindDict != null)
            {
                keyBindList = keybindDict.Cast<string>().ToList();
            }
            else
            {
                return nullString;
            }

            if (keyBindList == null)
            {
                return nullString;
            }

            if (keyBindList.Count > 1)
            {
                string keyBindLister = "";
                foreach (string key in keyBindList)
                {
                    keyBindLister += String.Format("({0}) ", key);
                }
                if (tooltip) return String.Format(" ({0})", keyBindLister);
                else return keyBindLister;
            }
            else if ((keyBindList.Count == 1) && keyBindList[0] != "None")
            {
                if (tooltip) return String.Format(" ({0})", keyBindList[0]);
                else return keyBindList[0];
            }
            else
            {
                return nullString;
            }


        }
    }
}
