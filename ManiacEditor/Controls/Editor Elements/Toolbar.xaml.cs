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
using ManiacEditor.Controls.Global;

using ManiacEditor.Controls.Misc;
using ManiacEditor.Controls.Misc.Configuration;

using ManiacEditor.Extensions;
using System.ComponentModel;



namespace ManiacEditor.Controls.Editor_Elements
{
    /// <summary>
    /// Interaction logic for Toolbar.xaml
    /// </summary>
    public partial class Toolbar : UserControl
    {
        #region Variables
        private bool IsFullyInitialized { get; set; } = false;
        public bool AllowNUDUpdate { get; set; } = true;

        #region Extra Layer Buttons
        public IDictionary<Controls.Global.EditLayerToggleButton, Controls.Global.EditLayerToggleButton> ExtraLayerEditViewButtons { get; set; }
        public IList<Separator> ExtraLayerSeperators { get; set; }
        #endregion

        #endregion

        #region Init
        public Toolbar()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                EditFGLower.Click += EditFGLower_Click;
                EditFGLow.Click += EditFGLow_Click;
                EditFGHigh.Click += EditFGHigh_Click;
                EditFGHigher.Click += EditFGHigher_Click;

                EditFGLower.RightClick += EditFGLower_RightClick;
                EditFGLow.RightClick += EditFGLow_RightClick;
                EditFGHigh.RightClick += EditFGHigh_RightClick;
                EditFGHigher.RightClick += EditFGHigher_RightClick;
                IsFullyInitialized = true;
            }

        }

        private static Editor.MainEditor Instance;
        public static void UpdateInstance(Editor.MainEditor _instance)
        {
            Instance = _instance;
        }
        #endregion

        #region File Events
        private void NewSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Solution.SolutionLoader.NewScene(); }
        public void OpenSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Solution.SolutionLoader.OpenScene(); }
        public void SaveSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Solution.SolutionLoader.Save(); }
        #endregion

        #region Apps Item Events
        private void ModManager(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ManiaModManager(); }
        #endregion

        #region General Events
        private void UndoEvent(object sender, RoutedEventArgs e) { Actions.UndoRedoModel.Undo(); }
        private void RedoEvent(object sender, RoutedEventArgs e) { Actions.UndoRedoModel.Redo(); }
        private void ZoomInEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionActions.ZoomIn(); }
        private void ZoomOutEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionActions.ZoomOut(); }
        #endregion

        #region Global Tools
        private void ToggleSelectToolEvent(object sender, RoutedEventArgs e) 
        { 
           if (IsFullyInitialized) Methods.Solution.SolutionState.Main.SelectionMode(); 
        }
        private void TogglePointerToolEvent(object sender, RoutedEventArgs e) 
        {
            if (IsFullyInitialized) Methods.Solution.SolutionState.Main.PointerMode(); 
        }
        private void ToggleDrawToolEvent(object sender, RoutedEventArgs e) 
        {
            if (IsFullyInitialized) Methods.Solution.SolutionState.Main.DrawMode(); 
        }
        private void ToggleSplineToolEvent(object sender, RoutedEventArgs e) 
        {
            if (IsFullyInitialized) Methods.Solution.SolutionState.Main.SplineMode(); 
        }
        private void ToggleChunksToolEvent(object sender, RoutedEventArgs e) 
        {
            if (IsFullyInitialized) Methods.Solution.SolutionState.Main.ChunksMode(true); 
        }
        #endregion

        #region Spline Tool Events
        private void SplineShowLineCheckboxCheckChanged(object sender, RoutedEventArgs e)
        {
            if (IsFullyInitialized) Methods.Solution.SolutionState.Main.AdjustSplineGroupOptions(Methods.Solution.SolutionState.StateModel.SplineOption.ShowLines, SplineShowLineCheckbox.IsChecked.Value);
        }
        private void SplineShowPointsCheckboxCheckChanged(object sender, RoutedEventArgs e)
        {
            if (IsFullyInitialized) Methods.Solution.SolutionState.Main.AdjustSplineGroupOptions(Methods.Solution.SolutionState.StateModel.SplineOption.ShowPoints, SplineShowPointsCheckbox.IsChecked.Value);
        }
        private void SplineShowObjectsCheckboxCheckChanged(object sender, RoutedEventArgs e)
        {
            if (IsFullyInitialized) Methods.Solution.SolutionState.Main.AdjustSplineGroupOptions(Methods.Solution.SolutionState.StateModel.SplineOption.ShowObjects, SplineShowObjectsCheckbox.IsChecked.Value);
        }

        bool AllowSplineFreqeunceUpdate = true;
        bool AllowSplineUpdateEvent = true;

        private void SplineOptionsIDChangedEvent(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IsFullyInitialized)
            {
                if (SplinePointSeperationSlider != null && SplinePointSeperationNUD != null && SplineGroupID != null && AllowSplineUpdateEvent)
                {
                    SelectedSplineIDChangedEvent(SplineGroupID.Value.Value);
                }
            }

        }
        public void SelectedSplineIDChangedEvent(int value)
        {
            if (IsFullyInitialized)
            {
                AllowSplineUpdateEvent = false;
                Methods.Solution.SolutionState.Main.AllowSplineOptionsUpdate = false;
                SplineGroupID.Value = value;
                Methods.Solution.SolutionState.Main.SelectedSplineID = value;
                SplineSpawnID.Value = value;
                Methods.Internal.UserInterface.SplineControls.UpdateSplineSettings(value);
                Methods.Solution.SolutionState.Main.AllowSplineOptionsUpdate = true;
                AllowSplineUpdateEvent = true;
            }

        }
        private async void SplinePointFrequenceChangedEvent(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IsFullyInitialized)
            {
                if (!Methods.Solution.SolutionState.Main.AllowSplineOptionsUpdate) return;
                if (SplinePointSeperationNUD != null && SplinePointSeperationSlider != null && AllowSplineFreqeunceUpdate)
                {
                    AllowSplineFreqeunceUpdate = false;
                    int size = (int)SplinePointSeperationNUD.Value;
                    SplinePointSeperationSlider.Value = size;
                    await Task.Run(() => Methods.Solution.SolutionState.Main.AdjustSplineGroupOptions(Methods.Solution.SolutionState.StateModel.SplineOption.Size, size));
                    AllowSplineFreqeunceUpdate = true;
                }
            }
        }
        private async void SplinePointFrequenceChangedEvent(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsFullyInitialized)
            {
                if (!Methods.Solution.SolutionState.Main.AllowSplineOptionsUpdate) return;
                if (SplinePointSeperationSlider != null && SplinePointSeperationNUD != null && AllowSplineFreqeunceUpdate)
                {
                    AllowSplineFreqeunceUpdate = false;
                    int size = (int)SplinePointSeperationSlider.Value;
                    SplinePointSeperationNUD.Value = size;
                    await Task.Run(() => Methods.Solution.SolutionState.Main.AdjustSplineGroupOptions(Methods.Solution.SolutionState.StateModel.SplineOption.Size, size));
                    AllowSplineFreqeunceUpdate = true;
                }
            }
        }
        private void SplineLineMode_Click(object sender, RoutedEventArgs e)
        {
            if (IsFullyInitialized) Methods.Solution.SolutionState.Main.AdjustSplineGroupOptions(Methods.Solution.SolutionState.StateModel.SplineOption.LineMode, SplineLineMode.IsChecked.Value);
        }
        private void SplineOvalMode_Click(object sender, RoutedEventArgs e)
        {
            if (IsFullyInitialized) Methods.Solution.SolutionState.Main.AdjustSplineGroupOptions(Methods.Solution.SolutionState.StateModel.SplineOption.OvalMode, SplineOvalMode.IsChecked.Value);
        }
        private void SplineSpawnRender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsFullyInitialized) return;
            if (Methods.Solution.CurrentSolution.Entities != null && Methods.Solution.SolutionState.Main.AllowSplineOptionsUpdate)
            {
                var selectedItem = SelectedSplineRender.SelectedItem as TextBlock;
                if (selectedItem.Tag == null) return;
                if (selectedItem.Tag is RSDKv5.SceneObject)
                {
                    var obj = selectedItem.Tag as RSDKv5.SceneObject;
                    int splineID = Methods.Solution.SolutionState.Main.SelectedSplineID;
                    Methods.Solution.SolutionState.Main.AdjustSplineGroupOptions(Methods.Solution.SolutionState.StateModel.SplineOption.SpawnObject, Methods.Solution.CurrentSolution.Entities.GenerateEditorEntity(new RSDKv5.SceneEntity(obj, 0)));
                    Instance.EntitiesToolbar?.UpdateToolbar(new List<Classes.Scene.EditorEntity>() { Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate });

                    if (Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                        SplineRenderObjectName.Content = Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Object.Name.Name;
                    else
                        SplineRenderObjectName.Content = "None";

                }

            }
        }
        private void SplineRenderObjectName_Click(object sender, RoutedEventArgs e)
        {
            if (!IsFullyInitialized) return;
            if (!SelectedSplineRender.IsDropDownOpen) SelectedSplineRender.IsDropDownOpen = true;
            else SelectedSplineRender.IsDropDownOpen = false;
        }
        private async void RenderSelectedSpline_Click(object sender, RoutedEventArgs e)
        {
            if (!IsFullyInitialized) return;
            if (Methods.Solution.SolutionState.Main.SplineOptionsGroup[Methods.Solution.SolutionState.Main.SelectedSplineID].SplineObjectRenderingTemplate != null && Methods.Solution.SolutionState.Main.SplineOptionsGroup[Methods.Solution.SolutionState.Main.SelectedSplineID].SplineTotalNumberOfObjects >= 2)
            {
                await Task.Run(() => Methods.Entities.SplineSpawning.RenderSplineByID(Methods.Solution.SolutionState.Main.SelectedSplineID));
                Methods.Internal.UserInterface.SplineControls.UpdateSplineToolbox();
                Methods.Internal.UserInterface.UpdateControls();
            }

        }

        #endregion

        #region Draw Tool Size Events
        bool AllowDrawBrushSizeChange = true;
        private void DrawToolSizeChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DrawToolSizeChanged();
        }
        private void DrawToolSizeChanged(bool wasSlider = false)
        {
            if (Instance != null)
            {
                if (DrawTileSizeNUD != null && DrawTileSizeSlider != null && AllowDrawBrushSizeChange)
                {
                    AllowDrawBrushSizeChange = false;
                    int size = (wasSlider ? (int)DrawTileSizeSlider.Value : (int)DrawTileSizeNUD.Value);
                    DrawTileSizeSlider.Value = size;
                    DrawTileSizeNUD.Value = size;
                    Methods.Solution.SolutionState.Main.DrawBrushSize = size;
                    AllowDrawBrushSizeChange = true;
                }
            }
        }
        private void DrawToolSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DrawToolSizeChanged(true);
        }

        #endregion

        #region Collision Events
        public void ShowCollisionAEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.ShowCollisionA ^= true; }
        public void ShowCollisionBEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.ShowCollisionB ^= true; }
        private void UseNormalCollisionEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.CollisionPreset = 0; }
        private void UseInvertedCollisionEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.CollisionPreset = 1; }
        private void UseCustomCollisionEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.CollisionPreset = 2; }
        private void CollisionOpacitySliderValueChangedEvent(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        #endregion

        #region Magnet Events

        private void ToggleMagnetToolEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.UseMagnetMode ^= true; }
        private void Magnet8x8Event(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.MagnetSize = 8; }
        private void Magnet16x16Event(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.MagnetSize = 16; }
        private void Magnet32x32Event(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.MagnetSize = 32; }
        private void Magnet64x64Event(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.MagnetSize = 64; }
        private void MagnetCustomEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.MagnetSize = -1; }
        private void CustomMagnetSizeAdjuster_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IsFullyInitialized && AllowNUDUpdate)
            {
                Methods.Solution.SolutionState.Main.CustomMagnetSize = CustomMagnetSizeAdjuster.Value.Value;
            }
        }

        private void EnableMagnetXAxisLockEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.UseMagnetXAxis ^= true; }
        private void EnableMagnetYAxisLockEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.UseMagnetYAxis ^= true; }

        #endregion

        #region Grid Events
        public void ToggleGridEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.ShowGrid ^= true; }
        private void SetGrid16x16Event(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.GridSize = 16; }
        private void SetGrid128x128Event(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.GridSize = 128; }
        private void SetGrid256x256Event(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.GridSize = 256; }
        private void SetGridCustomSizeEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.GridSize = -1; }

        private void CustomGridSizeAdjuster_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IsFullyInitialized && AllowNUDUpdate)
            {
                Methods.Solution.SolutionState.Main.GridCustomSize = CustomGridSizeAdjuster.Value.Value;
                Methods.Solution.SolutionState.Main.GridSize = -1;
            }

        }
        #endregion

        #region Misc Events

        public void ReloadToolStripButton_Click(object sender, RoutedEventArgs e) { Methods.Internal.UserInterface.ReloadSpritesAndTextures(); }
        public void ToggleShowTileIDEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.ShowTileID ^= true; }
        private void FasterNudgeValueNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e) { if (FasterNudgeValueNUD.Value != null) { Methods.Solution.SolutionState.Main.FasterNudgeAmount = FasterNudgeValueNUD.Value.Value; } }
        private void ShowFlippedTileHelperEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.ShowFlippedTileHelper ^= true; }
        public void EnableEncorePaletteEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.UseEncoreColors ^= true; }
        private void RunSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.RunScene(); }

        #endregion

        #region Settings and Other Menu Events
        public void AboutScreenEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.AboutScreen(); }
        public void ImportObjectsToolStripMenuItem_Click(Window window = null) { Methods.ProgramLauncher.ImportObjectsToolStripMenuItem_Click(window); }
        public void ImportObjectsWithMegaList(Window window = null) { Methods.ProgramLauncher.ImportObjectsWithMegaList(window); }
        public void ImportSoundsEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ImportSoundsToolStripMenuItem_Click(sender, e); }
        public void ImportSoundsEvent(Window window = null) { Methods.ProgramLauncher.ImportSounds(window); }
        private void LayerManagerEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.LayerManager(sender, e); }
        private void ManiacINIEditorEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ManiacINIEditor(sender, e); }
        public void ObjectManagerEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ObjectManager(); }
        private void InGameOptionsMenuEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.InGameSettings(); }
        private void WikiLinkEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.WikiLink(); }
        public void OptionsMenuEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OptionsMenu(); }
        private void ControlsMenuEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ControlMenu(); }
        #endregion

        #region Game Running Events
        private void MoveThePlayerToHere(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.MoveThePlayerToHere(); }
        private void SetPlayerRespawnToHere(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.SetPlayerRespawnToHere(); }
        private void MoveCheckpoint(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.CheckpointSelected = true; }
        private void RemoveCheckpoint(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.UpdateCheckpoint(new System.Drawing.Point(0, 0), false); }
        private void AssetReset(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.AssetReset(); }
        private void RestartScene(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.RestartScene(); }
        private void TrackThePlayer(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.TrackthePlayer(sender, e); }
        private void UpdateInGameMenuItems(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.UpdateRunSceneDropdown(); }
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
            LayerShowButton_Click(ShowEntities, "Entities");
        }

        private void LayerEditButton_Click(EditLayerToggleButton button, MouseButton ClickType)
        {
            if (button == EditEntities) EditEntitiesMode();
            else if (ClickType == MouseButton.Left) LayerA();
            else if (ClickType == MouseButton.Right) LayerB();

            Methods.Internal.UserInterface.UpdateControls();


            void EditEntitiesMode()
            {
                Methods.Solution.SolutionActions.Deselect(false);
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

                foreach (var elb in ExtraLayerEditViewButtons.Values)
                {
                    elb.IsCheckedN = false;
                    elb.IsCheckedA = false;
                    elb.IsCheckedB = false;
                }

            }

            void LayerA()
            {
                Methods.Solution.SolutionActions.Deselect(false);
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

                foreach (var elb in ExtraLayerEditViewButtons.Values)
                {
                    if (elb != button) elb.IsCheckedA = false;
                }
            }
            void LayerB()
            {
                Methods.Solution.SolutionActions.Deselect(false);
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

                foreach (var elb in ExtraLayerEditViewButtons.Values)
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
            foreach (Classes.Scene.EditorLayer el in Methods.Solution.CurrentSolution.CurrentScene.OtherLayers)
            {
                EditLayerToggleButton tsb = new EditLayerToggleButton()
                {
                    Text = el.Name,
                    LayerName = "Edit" + el.Name
                };
                LayerToolbar.Items.Add(tsb);
                tsb.DualSelect = true;
                tsb.TextForeground = Methods.Internal.Theming.GetSCBResource("Maniac_ExtraEditLayer_LabelText");
                tsb.RightClick += AdHocLayerEdit_RightClick;
                tsb.IsLayerOptionsEnabled = true;

                tsb.Click += AdHocLayerEdit_Click;

                _extraLayerEditButtons.Add(tsb);
            }

            //EDIT BUTTONS SEPERATOR
            Separator tss = new Separator();
            LayerToolbar.Items.Add(tss);
            ExtraLayerSeperators.Add(tss);

            //VIEW BUTTONS
            foreach (Classes.Scene.EditorLayer el in Methods.Solution.CurrentSolution.CurrentScene.OtherLayers)
            {
                EditLayerToggleButton tsb = new EditLayerToggleButton()
                {
                    Text = el.Name,
                    LayerName = "Show" + el.Name.Replace(" ", "")
                };
                LayerToolbar.Items.Insert(LayerToolbar.Items.IndexOf(extraViewLayersSeperator), tsb);
                tsb.TextForeground = Methods.Internal.Theming.GetSCBResource("Maniac_ExtraViewLayer_LabelText");
                tsb.IsLayerOptionsEnabled = true;
                tsb.IsLayerControlsHidden = true;


                _extraLayerViewButtons.Add(tsb);
            }

            //EDIT + VIEW BUTTONS LIST
            for (int i = 0; i < _extraLayerViewButtons.Count; i++)
            {
                ExtraLayerEditViewButtons.Add(_extraLayerViewButtons[i], _extraLayerEditButtons[i]);
            }

            UpdateDualButtonsControlsForLayer(Methods.Solution.CurrentSolution.FGLow, ShowFGLow, EditFGLow);
            UpdateDualButtonsControlsForLayer(Methods.Solution.CurrentSolution.FGHigh, ShowFGHigh, EditFGHigh);
            UpdateDualButtonsControlsForLayer(Methods.Solution.CurrentSolution.FGLower, ShowFGLower, EditFGLower);
            UpdateDualButtonsControlsForLayer(Methods.Solution.CurrentSolution.FGHigher, ShowFGHigher, EditFGHigher);
        }
        public void TearDownExtraLayerButtons()
        {
            foreach (var elb in ExtraLayerEditViewButtons)
            {
                LayerToolbar.Items.Remove(elb.Key);
                elb.Value.Click -= AdHocLayerEdit_Click;
                elb.Value.RightClick -= AdHocLayerEdit_RightClick;
                LayerToolbar.Items.Remove(elb.Value);
            }
            ExtraLayerEditViewButtons.Clear();


            foreach (var els in ExtraLayerSeperators)
            {
                LayerToolbar.Items.Remove(els);
            }
            ExtraLayerSeperators.Clear();

        }
        /// <summary>
        /// Given a scene layer, configure the given visibiltiy and edit buttons which will control that layer.
        /// </summary>
        /// <param name="layer">The layer of the scene from which to extract a name.</param>
        /// <param name="visibilityButton">The button which controls the visibility of the layer.</param>
        /// <param name="editButton">The button which controls editing the layer.</param>
        private void UpdateDualButtonsControlsForLayer(Classes.Scene.EditorLayer layer, ToggleButton visibilityButton, EditLayerToggleButton editButton)
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
            if (ClickType == MouseButton.Left) LayerA();
            else if (ClickType == MouseButton.Right) LayerB();

            Methods.Internal.UserInterface.UpdateControls();

            void Normal()
            {
                EditLayerToggleButton tsb = sender as EditLayerToggleButton;
                Methods.Solution.SolutionActions.Deselect(false);
                if (tsb.IsCheckedN.Value)
                {
                    if (!ManiacEditor.Properties.Settings.MySettings.KeepLayersVisible)
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

                    foreach (var elb in ExtraLayerEditViewButtons)
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
                Methods.Solution.SolutionActions.Deselect(false);
                if (tsb.IsCheckedA.Value)
                {
                    if (!ManiacEditor.Properties.Settings.MySettings.KeepLayersVisible)
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

                    foreach (var elb in ExtraLayerEditViewButtons)
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
                Methods.Solution.SolutionActions.Deselect(false);
                if (tsb.IsCheckedB.Value)
                {
                    if (!ManiacEditor.Properties.Settings.MySettings.KeepLayersVisible)
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

                    foreach (var elb in ExtraLayerEditViewButtons)
                    {
                        if (elb.Value != tsb)
                        {
                            elb.Value.IsCheckedB = false;
                        }
                    }
                }
            }
        }
        #endregion

        #region Mod Config List Stuff
        public MenuItem CreateModConfigMenuItem(int i)
        {
            MenuItem newItem = new MenuItem()
            {
                Header = Classes.Prefrences.CommonPathsStorage.Collection.ModLoaderConfigsNames[i],
                Tag = Classes.Prefrences.CommonPathsStorage.Collection.ModLoaderConfigs[i]
            };
            newItem.Click += ModConfigItemClicked;
            if (newItem.Tag.ToString() == ManiacEditor.Properties.Settings.MySettings.LastModConfig) newItem.IsChecked = true;
            return newItem;
        }
        private void ModConfigItemClicked(object sender, RoutedEventArgs e)
        {
            var modConfig_CheckedItem = (sender as MenuItem);
            SelectConfigToolStripMenuItem_Click(modConfig_CheckedItem);
            ManiacEditor.Properties.Settings.MySettings.LastModConfig = modConfig_CheckedItem.Tag.ToString();
        }
        public void EditConfigsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager configManager = new ConfigManager();
            configManager.Owner = Window.GetWindow(this);
            configManager.ShowDialog();

            selectConfigToolStripMenuItem.Items.Clear();
            if (Classes.Prefrences.CommonPathsStorage.Collection.ModLoaderConfigs == null) Classes.Prefrences.CommonPathsStorage.Collection.ModLoaderConfigs = new StringCollection();
            for (int i = 0; i < Classes.Prefrences.CommonPathsStorage.Collection.ModLoaderConfigs.Count; i++)
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
            if (!IsFullyInitialized) return;
            if (e != null && e.NewValue.Value != null)
            {
                Methods.Solution.SolutionState.Main.GridColor = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
            }
        }
        private void comboBox7_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (!IsFullyInitialized) return;
            if (e != null && e.NewValue.Value != null)
            {
                Methods.Solution.SolutionState.Main.waterColor = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
            }
        }
        private void comboBox6_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (!IsFullyInitialized) return;
            if (e != null && e.NewValue.Value != null && Instance != null)
            {
                Methods.Solution.SolutionState.Main.Custom_CollisionTopOnlySolid_Color = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
                Methods.Solution.SolutionState.Main.RefreshCollisionColours();
            }
        }
        private void comboBox5_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (!IsFullyInitialized) return;
            if (e != null && e.NewValue.Value != null && Instance != null)
            {
                Methods.Solution.SolutionState.Main.Custom_CollisionLRDSolid_Color = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
                Methods.Solution.SolutionState.Main.RefreshCollisionColours();
            }
        }
        private void comboBox4_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (!IsFullyInitialized) return;
            if (e != null && e.NewValue.Value != null && Instance != null)
            {
                Methods.Solution.SolutionState.Main.Custom_CollisionAllSolid_Color = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
                Methods.Solution.SolutionState.Main.RefreshCollisionColours();
            }
        }
        private void CollisionColorPickerClosed(object sender, RoutedEventArgs e)
        {
            if (!IsFullyInitialized) return;
            Methods.Solution.SolutionState.Main.RefreshCollisionColours();
        }


        #endregion

        #region UI Refresh

        public void UpdateGameRunningButton(bool enabled = true)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                RunSceneButton.IsEnabled = enabled;
                RunSceneDropDown.IsEnabled = enabled && RunSceneButton.IsEnabled;

                if (Methods.Runtime.GameHandler.GameRunning || System.Diagnostics.Process.GetProcessesByName("SonicMania").FirstOrDefault() != null)
                {
                    if (Methods.Runtime.GameHandler.GameRunning) RunSceneIcon.Fill = System.Windows.Media.Brushes.Blue;
                    else RunSceneIcon.Fill = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    RunSceneIcon.Fill = System.Windows.Media.Brushes.Gray;
                }
            }));

        }
        public void SetEditButtonsState(bool enabled)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                EditFGLow.IsEnabled = enabled && Methods.Solution.CurrentSolution.FGLow != null;
                EditFGHigh.IsEnabled = enabled && Methods.Solution.CurrentSolution.FGHigh != null;
                EditFGLower.IsEnabled = enabled && Methods.Solution.CurrentSolution.FGLower != null;
                EditFGHigher.IsEnabled = enabled && Methods.Solution.CurrentSolution.FGHigher != null;
                EditEntities.IsEnabled = enabled;

                EditFGLow.IsCheckedA = enabled && EditFGLow.IsCheckedA.Value;
                EditFGHigh.IsCheckedA = enabled && EditFGHigh.IsCheckedA.Value;
                EditFGLower.IsCheckedA = enabled && EditFGLower.IsCheckedA.Value;
                EditFGHigher.IsCheckedA = enabled && EditFGHigher.IsCheckedA.Value;

                EditFGLow.IsCheckedB = enabled && EditFGLow.IsCheckedB.Value;
                EditFGHigh.IsCheckedB = enabled && EditFGHigh.IsCheckedB.Value;
                EditFGLower.IsCheckedB = enabled && EditFGLower.IsCheckedB.Value;
                EditFGHigher.IsCheckedB = enabled && EditFGHigher.IsCheckedB.Value;

                foreach (var layerButtons in ExtraLayerEditViewButtons)
                {
                    layerButtons.Value.IsCheckedA = layerButtons.Value.IsCheckedA.Value && enabled;
                    layerButtons.Value.IsCheckedB = layerButtons.Value.IsCheckedB.Value && enabled;
                }

                EditEntities.IsCheckedN = enabled && EditEntities.IsCheckedN.Value;

                SetLayerEditButtonsState(enabled);

                MagnetMode.IsEnabled = enabled && ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit();
                MagnetMode.IsChecked = Methods.Solution.SolutionState.Main.UseMagnetMode && ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit();
                MagnetModeSplitButton.IsEnabled = enabled && ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit();
                Methods.Solution.SolutionState.Main.UseMagnetMode = ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit() && MagnetMode.IsChecked.Value;



                UndoButton.IsEnabled = enabled && Actions.UndoRedoModel.UndoStack.Count > 0;
                RedoButton.IsEnabled = enabled && Actions.UndoRedoModel.RedoStack.Count > 0;



                PointerToolButton.IsEnabled = enabled;
                SelectToolButton.IsEnabled = enabled && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit();

                DrawToolButton.IsEnabled = enabled && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() || ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit();
                DrawToolDropdown.IsEnabled = enabled && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() || ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit();

                ChunksToolButton.IsEnabled = enabled && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit();

                SplineToolButton.IsEnabled = enabled && ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit();
                SplineToolDropdown.IsEnabled = enabled && ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit();

                SplineToolButton.IsChecked = SplineToolButton.IsChecked.Value && ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit();

                bool isAnyOtherToolChecked()
                {
                    bool isPointer = (bool)PointerToolButton.IsChecked.Value;
                    bool isSelect = (bool)SelectToolButton.IsChecked.Value;
                    bool isDraw = (bool)DrawToolButton.IsChecked.Value;
                    bool isSpline = (bool)SplineToolButton.IsChecked.Value;

                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit())
                    {
                        if (isDraw || isSpline)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (isDraw || isSelect)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }


                PointerToolButton.IsChecked = isAnyOtherToolChecked();
                ChunksToolButton.IsChecked = (bool)ChunksToolButton.IsChecked && !ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit();
                if (Instance.TilesToolbar != null)
                {
                    if (ChunksToolButton.IsChecked.Value) Instance.TilesToolbar.TabControl.SelectedIndex = 1;
                    else Instance.TilesToolbar.TabControl.SelectedIndex = 0;
                }

                ShowGridButton.IsEnabled = enabled && Methods.Solution.CurrentSolution.StageConfig != null;
                ShowCollisionAButton.IsEnabled = enabled && Methods.Solution.CurrentSolution.TileConfig != null;
                ShowCollisionBButton.IsEnabled = enabled && Methods.Solution.CurrentSolution.TileConfig != null;
                ShowTileIDButton.IsEnabled = enabled && Methods.Solution.CurrentSolution.StageConfig != null;
                EncorePaletteButton.IsEnabled = enabled && Methods.Solution.SolutionState.Main.EncorePaletteExists;
                FlipAssistButton.IsEnabled = enabled;
            }));
        }
        private void SetLayerEditButtonsState(bool enabled)
        {
            SetEditLayerA();
            SetEditLayerB();

            void SetEditLayerA()
            {
                if (enabled && EditFGLow.IsCheckedA.Value) Methods.Solution.CurrentSolution.EditLayerA = Methods.Solution.CurrentSolution.FGLow;
                else if (enabled && EditFGHigh.IsCheckedA.Value) Methods.Solution.CurrentSolution.EditLayerA = Methods.Solution.CurrentSolution.FGHigh;
                else if (enabled && EditFGHigher.IsCheckedA.Value) Methods.Solution.CurrentSolution.EditLayerA = Methods.Solution.CurrentSolution.FGHigher;
                else if (enabled && EditFGLower.IsCheckedA.Value) Methods.Solution.CurrentSolution.EditLayerA = Methods.Solution.CurrentSolution.FGLower;
                else if (enabled && ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedA.Value))
                {

                    var selectedExtraLayerButton = ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedA.Value);
                    int index = ExtraLayerEditViewButtons.IndexOf(selectedExtraLayerButton);
                    var editorLayer = Methods.Solution.CurrentSolution.CurrentScene.OtherLayers.ElementAt(index);

                    Methods.Solution.CurrentSolution.EditLayerA = editorLayer;
                }
                else Methods.Solution.CurrentSolution.EditLayerA = null;
            }
            void SetEditLayerB()
            {
                if (enabled && EditFGLow.IsCheckedB.Value) Methods.Solution.CurrentSolution.EditLayerB = Methods.Solution.CurrentSolution.FGLow;
                else if (enabled && EditFGHigh.IsCheckedB.Value) Methods.Solution.CurrentSolution.EditLayerB = Methods.Solution.CurrentSolution.FGHigh;
                else if (enabled && EditFGHigher.IsCheckedB.Value) Methods.Solution.CurrentSolution.EditLayerB = Methods.Solution.CurrentSolution.FGHigher;
                else if (enabled && EditFGLower.IsCheckedB.Value) Methods.Solution.CurrentSolution.EditLayerB = Methods.Solution.CurrentSolution.FGLower;
                else if (enabled && ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedB.Value))
                {
                    var selectedExtraLayerButton = ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedB.Value);
                    var editorLayer = Methods.Solution.CurrentSolution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Methods.Solution.CurrentSolution.EditLayerB = editorLayer;
                }
                else Methods.Solution.CurrentSolution.EditLayerB = null;
            }

        }
        public void SetSceneOnlyButtonsState(bool enabled)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                ShowFGHigh.IsEnabled = enabled && Methods.Solution.CurrentSolution.FGHigh != null;
                ShowFGLow.IsEnabled = enabled && Methods.Solution.CurrentSolution.FGLow != null;
                ShowFGHigher.IsEnabled = enabled && Methods.Solution.CurrentSolution.FGHigher != null;
                ShowFGLower.IsEnabled = enabled && Methods.Solution.CurrentSolution.FGLower != null;
                ShowEntities.IsEnabled = enabled;

                ShowGridToggleButton.IsEnabled = enabled;
                ShowGridButton.IsEnabled = enabled;
                CollisionSettingsDropdown.IsEnabled = enabled;
                OtherDropdown.IsEnabled = enabled;

                ReloadButton.IsEnabled = enabled;

                Save.IsEnabled = enabled;

                if (Properties.Settings.MyPerformance.ReduceZoom)
                {
                    ZoomInButton.IsEnabled = enabled && Methods.Solution.SolutionState.Main.ZoomLevel < 5;
                    ZoomOutButton.IsEnabled = enabled && Methods.Solution.SolutionState.Main.ZoomLevel > -2;
                }
                else
                {
                    ZoomInButton.IsEnabled = enabled && Methods.Solution.SolutionState.Main.ZoomLevel < 5;
                    ZoomOutButton.IsEnabled = enabled && Methods.Solution.SolutionState.Main.ZoomLevel > -5;
                }
            }));
            UpdateGameRunningButton(enabled);
        }
        public void UpdateTooltips()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
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
                ShowCollisionAButton.ToolTip = "Show Collision Layer A" + KeyBindPraser("ShowPathA", true, true);
                ShowCollisionBButton.ToolTip = "Show Collision Layer B" + KeyBindPraser("ShowPathB", true, true);
                FlipAssistButton.ToolTip = "Show Flipped Tile Helper";
                ChunksToolButton.ToolTip = "Stamp Tool" + KeyBindPraser("StampTool", true);
                SplineToolButton.ToolTip = "Spline Tool" + KeyBindPraser("SplineTool", true);
                EncorePaletteButton.ToolTip = "Show Encore Colors";
                ShowTileIDButton.ToolTip = "Toggle Tile ID Visibility" + KeyBindPraser("ShowTileID", true, true);
                ShowGridButton.ToolTip = "Toggle Grid Visibility" + KeyBindPraser("ShowGrid", true, true);
            }));

        }
        public string KeyBindPraser(string keyRefrence, bool tooltip = false, bool nonRequiredBinding = false)
        {
            string nullString = (nonRequiredBinding ? "" : "N/A");
            if (nonRequiredBinding && tooltip) nullString = "None";
            List<string> keyBindList = new List<string>();
            List<string> keyBindModList = new List<string>();

            if (!Extensions.Extensions.KeyBindsSettingExists(keyRefrence)) return nullString;

            if (Properties.Settings.MyKeyBinds == null) return nullString;

            var keybindDict = Properties.Settings.MyKeyBinds.GetInput(keyRefrence) as List<string>;
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

        #endregion

        #region Context Menu Click
        private void ContextMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (sender as Button);
            if (!btn.ContextMenu.IsOpen)
            {
                //Open the Context menu when Button is clicked
                btn.ContextMenu.IsEnabled = true;
                btn.ContextMenu.PlacementTarget = (sender as Button);
                btn.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                btn.ContextMenu.IsOpen = true;

            }
        }

        #endregion

        private void EditorBGColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue && IsFullyInitialized && Instance != null)
            {
                Instance.ViewPanel.SharpPanel.GraphicPanel.DeviceBackColor = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
            }

        }
    }
}
