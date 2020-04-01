using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace ManiacEditor.Methods.Editor
{
    public class SolutionState
    {
        #region Init
        private static ManiacEditor.Controls.Editor.MainEditor Instance;
        public SolutionState(ManiacEditor.Controls.Editor.MainEditor instance)
        {
            Instance = instance;
        }
        #endregion

        #region Editor Status Variables

        public static bool IsEditing()
        {
            return IsTilesEdit() || IsEntitiesEdit() || IsChunksEdit();
        }
        public static bool IsSceneLoaded()
        {
            return Methods.Editor.Solution.CurrentScene == null ? false : true;
        }
        public static bool IsNormalTilesEdit()
        {
            return Instance.Dispatcher.Invoke<bool>(new Func<bool>(() =>
            {
                return !Instance.EditorToolbar.ChunksToolButton.IsChecked.Value && Methods.Editor.Solution.EditLayerA != null;
            }));
        }
        public static bool IsTilesEdit()
        {
            return Methods.Editor.Solution.EditLayerA != null;
        }
        public static bool IsChunksEdit()
        {
            return Instance.Dispatcher.Invoke<bool>(new Func<bool>(() =>
            {
                return Instance.EditorToolbar.ChunksToolButton.IsChecked.Value && Methods.Editor.Solution.EditLayerA != null;
            }));
        }
        public static bool IsEntitiesEdit()
        {
            return Instance.EditorToolbar.EditEntities.IsCheckedN.Value || Instance.EditorToolbar.EditEntities.IsCheckedA.Value || Instance.EditorToolbar.EditEntities.IsCheckedB.Value;
        }
        public static bool IsSelected(bool dualModeSelect = false)
        {
            if (IsTilesEdit())
            {

                bool SelectedA = Methods.Editor.Solution.EditLayerA?.SelectedTiles.Count > 0 || Methods.Editor.Solution.EditLayerA?.TempSelectionTiles.Count > 0;
                bool SelectedB = Methods.Editor.Solution.EditLayerB?.SelectedTiles.Count > 0 || Methods.Editor.Solution.EditLayerB?.TempSelectionTiles.Count > 0;
                if (dualModeSelect) return SelectedA && SelectedB;
                else return SelectedA || SelectedB;
            }
            else if (IsEntitiesEdit())
            {
                return Methods.Editor.Solution.Entities.IsAnythingSelected();
            }
            return false;
        }

        #endregion

        #region Dragging Variables

        public static int DraggedX { get; set; }
        public static int DraggedY { get; set; }
        public static bool DraggingSelection { get; set; } = false; //Determines if we are dragging a selection
        public static bool Dragged { get; set; } = false;
        public static bool StartDragged { get; set; } = false;

        public static bool AnyDragged
        {
            get
            {
                return (DraggingSelection || Dragged || AutoScrollingDragged || AutoScrolling);
            }
        }

        #endregion

        #region Auto Scrolling Variables

        public static bool AutoScrolling { get; set; } = false; //Determines if the User is Scrolling
        public static bool AutoScrollingDragged { get; set; } = false;
        public static Point AutoScrollPosition { get; set; } //For Getting the Scroll Position

        #endregion

        #region Zooming Variables

        public static bool Zooming { get; set; } = false;  //Detects if we are zooming

        private static double _Zoom = 1;
        public static double Zoom
        {
            get
            {
                return _Zoom;
            }
            set
            {
                //TODO : Fix Zooming with New Rendering System
                _Zoom = value;
            }
        }

        private static int _ZoomLevel = 1;
        public static int ZoomLevel
        {
            get
            {
                return _ZoomLevel;
            }
            set
            {
                //TODO : Fix Zooming with New Rendering System
                _ZoomLevel = value;
            }
        }

        #endregion

        #region View Position Controls
        public static int ViewPositionX { get => GetViewPositionX(); }
        public static int ViewPositionY { get => GetViewPositionY(); }
        public static bool UnlockCamera
        {
            get
            {
                return _UnlockCamera;
            }
            set
            {
                _UnlockCamera = value;
                Instance.MenuBar.UnlockCameraToolStripMenuItem.IsChecked = _UnlockCamera;
                SetViewPositionX(0);
                SetViewPositionY(0);
                Instance.ViewPanel.SharpPanel.UpdateGraphicsPanelControls();
            }
        }

        private static bool _UnlockCamera { get; set; } = false;


        public static int GetViewPositionX()
        {
            if (Instance.ViewPanel.SharpPanel.hScrollBar1 != null)
            {
                int valueToReturn = 0;
                Instance.ViewPanel.SharpPanel.hScrollBar1.Dispatcher.Invoke(() => 
                {
                    valueToReturn = (int)Instance.ViewPanel.SharpPanel.hScrollBar1.Value;
                });
                return valueToReturn;
            }
            else return 0;
        }

        public static int GetViewPositionY()
        {
            if (Instance.ViewPanel.SharpPanel.vScrollBar1 != null)
            {
                int valueToReturn = 0;
                Instance.ViewPanel.SharpPanel.vScrollBar1.Dispatcher.Invoke(() =>
                {
                    valueToReturn = (int)Instance.ViewPanel.SharpPanel.vScrollBar1.Value;
                });
                return valueToReturn;
            }
            else return 0;
        }

        public static void SetViewPositionX(int value)
        {
            if (Instance.ViewPanel.SharpPanel.hScrollBar1 != null)
            {
                Instance.ViewPanel.SharpPanel.hScrollBar1.Value = value;
                ManiacEditor.Methods.Internal.RefreshModel.RequestEntityVisiblityRefresh(true);
            }
        }

        public static void SetViewPositionY(int value)
        {
            if (Instance.ViewPanel.SharpPanel.vScrollBar1 != null)
            {
                Instance.ViewPanel.SharpPanel.vScrollBar1.Value = value;
                ManiacEditor.Methods.Internal.RefreshModel.RequestEntityVisiblityRefresh(true);
            }
        }

        #endregion

        #region Selection Regions Variables
        public static int RegionX1 { get; set; } = -1;
        public static int RegionY1 { get; set; } = -1;
        public static int RegionX2 { get; set; }
        public static int RegionY2 { get; set; }

        public static int TempSelectX1 { get; set; }
        public static int TempSelectX2 { get; set; }
        public static int TempSelectY1 { get; set; }
        public static int TempSelectY2 { get; set; }

        public static int LastX { get; set; }
        public static int LastY { get; set; }
        #endregion

        #region Tile Selection Variables

        public static int SelectedTilesCount { get; set; } //Used to get the Amount of Selected Tiles in a Selection
        public static int DeselectTilesCount { get; set; } //Used in combination with SelectedTilesCount to get the definitive amount of Selected Tiles
        public static int SelectedTileX { get; set; } = 0; //Used to get a single Selected Tile's X
        public static int SelectedTileY { get; set; } = 0; //Used to get a single Selected Tile's Y
        public static bool isTileDrawing { get; set; } = false;

        #endregion

        #region Grid View Variables
        public static bool ShowGrid
        {
            get { return _ShowGrid; }
            set
            {
                if (Solution.CurrentScene == null) value = false;
                 Instance.EditorToolbar.ShowGridToggleButton.IsChecked = value;
                _ShowGrid = value;
            }
        }
        private static bool _ShowGrid;
        public static int GridSize { get => GetGridSize(); set => SetGridSize(value); }
        private static int _GridSize = 16;
        public static int GridCustomSize { get => GetCustomSize(); set => ChangeCustomSize(value); }
        private static int _GridCustomSize = Properties.Settings.MyDefaults.CustomGridSizeValue;

        private static void ChangeCustomSize(int value)
        {
            _GridCustomSize = value;
            Instance.EditorToolbar.CustomGridSizeLabel.Text = string.Format(Instance.EditorToolbar.CustomGridSizeLabel.Tag.ToString(), _GridCustomSize);
            Instance.EditorToolbar.AllowNUDUpdate = false;
            Instance.EditorToolbar.CustomGridSizeAdjuster.Value = _GridCustomSize;
            Instance.EditorToolbar.AllowNUDUpdate = true;
        }

        private static int GetCustomSize()
        {
            Instance.EditorToolbar.CustomGridSizeLabel.Text = string.Format(Instance.EditorToolbar.CustomGridSizeLabel.Tag.ToString(), _GridCustomSize);
            return _GridCustomSize;
        }

        private static int GetGridSize()
        {
            return _GridSize;
        }
        private static void SetGridSize(int value)
        {
            bool isCustom = false;
            _GridSize = value;

            Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = false;
            Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = false;
            Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = false;
            Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = false;

            if (value == 16) Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = true;
            else if (value == 128) Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = true;
            else if (value == 256) Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = true;
            else if (value == -1)
            {
                isCustom = true;
                Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = true;
            }

            Instance.EditorToolbar.CustomGridSizeLabel.Text = string.Format(Instance.EditorToolbar.CustomGridSizeLabel.Tag.ToString(), GridCustomSize);

            if (!isCustom) _GridSize = value;
            else _GridSize = GridCustomSize;
        }

        public static System.Drawing.Color GridColor { get => GetGridColor(); set => SetGridColor(value); }
        private static System.Drawing.Color _GridColor = System.Drawing.Color.Red;
        private static System.Drawing.Color GetGridColor() { return _GridColor; }
        private static void SetGridColor(System.Drawing.Color value) { _GridColor = value; }
        #endregion

        #region Magnet Mode Variables

        public static int MagnetSize
        {
            get
            {
                return _MagnetSize;
            }
            set
            {
                bool isCustom = false;
                Instance.EditorToolbar.Magnet8x8ModeMenuItem.IsChecked = false;
                Instance.EditorToolbar.Magnet16x16ModeMenuItem.IsChecked = false;
                Instance.EditorToolbar.Magnet32x32ModeMenuItem.IsChecked = false;
                Instance.EditorToolbar.Magnet64x64ModeMenuItem.IsChecked = false;
                Instance.EditorToolbar.MagnetCustomModeMenuItem.IsChecked = false;

                if (value == 8) Instance.EditorToolbar.Magnet8x8ModeMenuItem.IsChecked = true;
                else if (value == 16) Instance.EditorToolbar.Magnet16x16ModeMenuItem.IsChecked = true;
                else if (value == 32) Instance.EditorToolbar.Magnet32x32ModeMenuItem.IsChecked = true;
                else if (value == 64) Instance.EditorToolbar.Magnet64x64ModeMenuItem.IsChecked = true;
                else if (value == -1)
                {
                    isCustom = true;
                    Instance.EditorToolbar.MagnetCustomModeMenuItem.IsChecked = true;
                }

                Instance.EditorToolbar.CustomMagnetSizeLabel.Text = string.Format(Instance.EditorToolbar.CustomMagnetSizeLabel.Tag.ToString(), _CustomMagnetSize);

                if (!isCustom) _MagnetSize = value;
                else _MagnetSize = _CustomMagnetSize;
            }
        }
        private static int _MagnetSize = 16;
        public static int CustomMagnetSize
        {
            get
            {
                return _CustomMagnetSize;
            }
            set
            {
                if (Instance != null)
                {
                    _CustomMagnetSize = value;
                    MagnetSize = -1;
                    Instance.EditorToolbar.AllowNUDUpdate = false;
                    Instance.EditorToolbar.CustomMagnetSizeAdjuster.Value = _CustomMagnetSize;
                    Instance.EditorToolbar.AllowNUDUpdate = true;
                }
            }
        }
        private static int _CustomMagnetSize = 16;


        public static bool UseMagnetXAxis
        {
            get { return _UseMagnetXAxis; }
            set
            {
                _UseMagnetXAxis = value;
                Instance.EditorToolbar.MagnetXAxisLockMenuItem.IsChecked = value;
            }
        }
        private static bool _UseMagnetXAxis = true;

        public static bool UseMagnetYAxis
        {
            get { return _UseMagnetYAxis; }
            set
            {
                _UseMagnetYAxis = value;
                Instance.EditorToolbar.MagnetYAxisLockMenuItem.IsChecked = value;
            }
        }
        private static bool _UseMagnetYAxis = true;



        #endregion

        #region General Options Variables
        public static bool CopyAir
        {
            get
            {
                return _CopyAir;
            }
            set
            {
                _CopyAir = value;
                Instance.MenuBar.copyAirToggle.IsChecked = value;
            }
        }
        private static bool _CopyAir = false;

        public static int FasterNudgeAmount
        {
            get
            {
                return _FasterNudgeAmount;
            }
            set
            {
                _FasterNudgeAmount = value;
            }
        }
        private static int _FasterNudgeAmount = 5;

        public static bool UseMagnetMode
        {
            get { return _UseMagnetMode; }
            set
            {
                _UseMagnetMode = value;
                Instance.EditorToolbar.MagnetMode.IsChecked = value;
            }
        }
        private static bool _UseMagnetMode = false;

        public static bool EnableFasterNudge
        {
            get
            {
                return _EnableFasterNudge;
            }
            set
            {
                Instance.EditorStatusBar.nudgeFasterButton.IsChecked = value;
                Instance.MenuBar.nudgeSelectionFasterToolStripMenuItem.IsChecked = value;
                _EnableFasterNudge = value;
            }
        }
        private static bool _EnableFasterNudge = false;

        public static bool ScrollLocked
        {
            get
            {
                return _ScrollLocked;
            }
            set
            {
                _ScrollLocked = value;
                Instance.EditorStatusBar.scrollLockButton.IsChecked = value;
                Instance.MenuBar.statusNAToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _ScrollLocked = true;
        #endregion

        #region Tool Mode Methods
        public static void PointerMode(bool? value = null)
        {
            if (value != null) SetToolModes(0, value.Value);
            else SetToolModes(0, Instance.EditorToolbar.PointerToolButton.IsChecked.Value);
        }
        public static void SelectionMode(bool? value = null)
        {
            if (value != null) SetToolModes(1, value.Value);
            else SetToolModes(1, Instance.EditorToolbar.SelectToolButton.IsChecked.Value);
        }
        public static void DrawMode(bool? value = null)
        {
            if (value != null) SetToolModes(2, value.Value);
            else SetToolModes(2, Instance.EditorToolbar.DrawToolButton.IsChecked.Value);
        }

        //Determines the Draw Tool's Brush Size (Tiles Only)
        public static int DrawBrushSize { get => GetDrawBrushSize(); set => SetDrawBrushSize(value); }
        private static int _DrawBrushSize = 1;

        private static int GetDrawBrushSize()
        {
            return _DrawBrushSize;
        }
        private static void SetDrawBrushSize(int value)
        {
            _DrawBrushSize = value;
        }



        public static void SplineMode(bool? value = null)
        {
            if (value != null) SetToolModes(4, value.Value);
            else SetToolModes(4, Instance.EditorToolbar.SplineToolButton.IsChecked.Value);
        }

        public static void ChunksMode(bool isClick = false)
        {
            if (!isClick)
            {
                bool isEnabled = Instance.EditorToolbar.ChunksToolButton.IsChecked.Value;
                if (isEnabled) Instance.EditorToolbar.ChunksToolButton.IsChecked = true;
                else Instance.EditorToolbar.ChunksToolButton.IsChecked = false;
            }

            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void SetToolModes(int selectedID, bool value)
        {
            Instance.EditorToolbar.PointerToolButton.IsChecked = (selectedID == 0 ? value : false);
            Instance.EditorToolbar.SelectToolButton.IsChecked = (selectedID == 1 ? value : false);
            Instance.EditorToolbar.DrawToolButton.IsChecked = (selectedID == 2 ? value : false);
            Instance.EditorToolbar.SplineToolButton.IsChecked = (selectedID == 4 ? value : false);
            Methods.Internal.UserInterface.UpdateControls();
        }
        #endregion

        #region Collision View Variables

        public static bool ShowCollisionA
        {
            get { return _ShowCollisionA; }
            set
            {
                Instance.EditorToolbar.ShowCollisionAButton.IsChecked = value;
                _ShowCollisionA = value;
                Instance.EditorToolbar.ShowCollisionBButton.IsChecked = false;
                _ShowCollisionB = false;
                Instance.ReloadSpecificTextures(null, null);
            }
        }
        private static bool _ShowCollisionA;

        public static bool ShowCollisionB
        {
            get { return _ShowCollisionB; }
            set
            {
                Instance.EditorToolbar.ShowCollisionAButton.IsChecked = false;
                _ShowCollisionA = false;
                Instance.EditorToolbar.ShowCollisionBButton.IsChecked = value;
                _ShowCollisionB = value;
                Instance.ReloadSpecificTextures(null, null);
            }
        }
        private static bool _ShowCollisionB;

        #endregion

        #region Animation Variables

        public static bool ParallaxAnimationChecked
        {
            get
            {
                return _ParallaxAnimationChecked;
            }
            set
            {
                _ParallaxAnimationChecked = value;
                Methods.Internal.UserInterface.UpdateControls();
            }
        }
        private static bool _ParallaxAnimationChecked = false;

        public static bool AllowAnimations
        {
            get
            {
                return _AllowAnimations;
            }
            set
            {
                _AllowAnimations = value;
                Methods.Internal.UserInterface.UpdateControls();
            }
        }
        private static bool _AllowAnimations = true;

        public static bool AllowSpriteAnimations
        {
            get
            {
                return _AllowSpriteAnimations;
            }
            set
            {
                _AllowSpriteAnimations = value;
            }
        }
        private static bool _AllowSpriteAnimations = true;

        public static bool AllowMovingPlatformAnimations
        {
            get
            {
                return _AllowMovingPlatformAnimations;
            }
            set
            {
                _AllowMovingPlatformAnimations = value;
            }
        }
        private static bool _AllowMovingPlatformAnimations = true;

        #endregion

        #region Scene View Variables

        public static bool ShowTileID
        {
            get { return _ShowTileID; }
            set
            {
                Instance.EditorToolbar.ShowTileIDButton.IsChecked = value;
                Instance.ReloadSpecificTextures(null, null);
                _ShowTileID = value;
            }
        }
        private static bool _ShowTileID;
        public static bool UseEncoreColors
        {
            get { return _UseEncoreColors; }
            set
            {
                Instance.DisposeTextures();
                Instance.EditorToolbar.EncorePaletteButton.IsChecked = value;
                _UseEncoreColors = value;
                Methods.Editor.Solution.CurrentTiles?.Image.Reload((value ? ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette[0] : null));
                Instance.TilesToolbar?.Reload((value ? ManiacEditor.Methods.Editor.SolutionPaths.EncorePalette[0] : null));
                Methods.Entities.EntityDrawing.ReleaseResources();
            }
        }
        private static bool _UseEncoreColors = false;
        public static bool ExtraLayersMoveToFront
        {
            get
            {
                return _ExtraLayersMoveToFront;
            }
            set
            {
                _ExtraLayersMoveToFront = value;
                Instance.MenuBar.moveExtraLayersToFrontToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _ExtraLayersMoveToFront = false;
        public static bool ShowFlippedTileHelper
        {
            get
            {
                return _ShowFlippedTileHelper;
            }
            set
            {
                _ShowFlippedTileHelper = value;
                Instance.ReloadSpecificTextures(null, null);
            }
        }
        private static bool _ShowFlippedTileHelper = false;

        #endregion

        #region Object View Variables

        public static bool EntitiesVisibileAboveAllLayers
        {
            get
            {
                return _EntitiesVisibileAboveAllLayers;
            }
            set
            {
                _EntitiesVisibileAboveAllLayers = value;
                Instance.MenuBar.SelectionBoxesAlwaysPrioritized.IsChecked = value;
            }
        }
        private static bool _EntitiesVisibileAboveAllLayers = false;
        public static bool EntitySelectionBoxesAlwaysPrioritized
        {
            get
            {
                return _EntitySelectionBoxesAlwaysPrioritized;
            }
            set
            {
                _EntitySelectionBoxesAlwaysPrioritized = value;
                Instance.MenuBar.SelectionBoxesAlwaysPrioritized.IsChecked = value;
            }
        }
        private static bool _EntitySelectionBoxesAlwaysPrioritized = false;
        public static bool ShowParallaxSprites
        {
            get { return _ShowParallaxSprites; }
            set
            {
                _ShowParallaxSprites = value;
                Instance.MenuBar.showParallaxSpritesToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _ShowParallaxSprites = false;
        public static bool ApplyEditEntitiesTransparency
        {
            get { return _ApplyEditEntitiesTransparency; }
            set
            {
                _ApplyEditEntitiesTransparency = value;
                Instance.MenuBar.EditEntitiesTransparencyToggle.IsChecked = value;
                Instance.EditorStatusBar.QuickEditEntitiesTransparentLayers.IsChecked = value;
            }
        }
        private static bool _ApplyEditEntitiesTransparency = false;
        public static bool ShowEntitySelectionBoxes
        {
            get { return _ShowEntitySelectionBoxes; }
            set
            {
                _ShowEntitySelectionBoxes = value;
                Instance.MenuBar.showEntitySelectionBoxesToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _ShowEntitySelectionBoxes = false;
        public static bool ShowEntityPathArrows
        {
            get
            {
                return _ShowEntityPathArrows;
            }
            set
            {
                _ShowEntityPathArrows = value;
                Instance.MenuBar.showEntityPathArrowsToolstripItem.IsChecked = value;
            }
        }
        private static bool _ShowEntityPathArrows = true;
        public static bool ShowWaterLevel
        {
            get
            {
                return _ShowWaterLevel;
            }
            set
            {
                _ShowWaterLevel = value;
                Instance.MenuBar.showWaterLevelToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _ShowWaterLevel = false;
        public static bool AlwaysShowWaterLevel
        {
            get
            {
                return _AlwaysShowWaterLevel;
            }
            set
            {
                _AlwaysShowWaterLevel = value;
                Instance.MenuBar.waterLevelAlwaysShowItem.IsChecked = value;
            }
        }
        private static bool _AlwaysShowWaterLevel = false;
        public static bool SizeWaterLevelwithBounds
        {
            get
            {
                return _SizeWaterLevelwithBounds;
            }
            set
            {
                _SizeWaterLevelwithBounds = value;
                Instance.MenuBar.sizeWithBoundsWhenNotSelectedToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _SizeWaterLevelwithBounds = false;

        public static System.Drawing.Color waterColor { get => GetWaterEntityColor(); set => SetWaterEntityColor(value); }
        private static System.Drawing.Color _WaterEntityColor = System.Drawing.Color.Blue;
        private static System.Drawing.Color GetWaterEntityColor() { return _WaterEntityColor; }
        private static void SetWaterEntityColor(System.Drawing.Color value) { _WaterEntityColor = value; }

        #endregion

        #region Editor View Variables

        public static bool CountTilesSelectedInPixels
        {
            get
            {
                return _CountTilesSelectedInPixels;
            }
            set
            {
                _CountTilesSelectedInPixels = value;
                Instance.EditorStatusBar.EnablePixelModeButton.IsChecked = value;
                Instance.MenuBar.EnablePixelModeMenuItem.IsChecked = value;
            }
        }
        private static bool _CountTilesSelectedInPixels = false;

        #endregion

        #region Save Warning Variables
        public static bool QuitWithoutSavingWarningRequired { get; set; } = false;
        #endregion

        #region Unoptimized Spline Options
        public class SplineOptions
        {
            //Determines the Spline Point Frequency
            public int SplineSize { get => GetSplineSize(); set => SetSplineSize(value); }
            private int _SplineSize = 128;
            private int GetSplineSize()
            {
                return _SplineSize;
            }
            private void SetSplineSize(int value)
            {
                _SplineSize = value;
                Instance.Dispatcher.Invoke(() => Methods.Internal.UserInterface.UpdateControls());
            }



            public bool SplineToolShowLines { get; set; } = true; //Self Explanatory
            public int SplineNumberOfObjectsRendered { get; set; } = 0; //Self Explanatory
            public int SplineTotalNumberOfObjects { get; set; } = 0; //Self Explanatory
            public bool SplineToolShowPoints { get; set; } = true; //Self Explanatory
            public bool SplineToolShowObject { get; set; } = false; //Self Explanatory

            public bool SplineLineMode { get => GetSplineLineMode(); set => SetSplineLineMode(value); } //Self Explanatory
            public bool SplineOvalMode { get => GetSplineOvalMode(); set => SetSplineOvalMode(value); } //Self Explanatory

            private bool _SplineLineMode = false;
            private bool _SplineOvalMode = true;
            private bool GetSplineLineMode()
            {
                return _SplineLineMode;
            }
            private void SetSplineLineMode(bool value)
            {
                _SplineLineMode = value;
                UpdateSplineStates(1, value);
            }

            private bool GetSplineOvalMode()
            {
                return _SplineOvalMode;
            }
            private void SetSplineOvalMode(bool value)
            {
                _SplineOvalMode = value;
                UpdateSplineStates(0, value);
            }

            private void UpdateSplineStates(int mode, bool state)
            {
                if (mode == 1)
                {
                    if (state)
                    {
                        Instance.EditorToolbar.SplineLineMode.IsChecked = true;
                        _SplineLineMode = true;
                        Instance.EditorToolbar.SplineOvalMode.IsChecked = false;
                        _SplineOvalMode = false;
                    }
                    else
                    {
                        Instance.EditorToolbar.SplineLineMode.IsChecked = false;
                        _SplineLineMode = false;
                        Instance.EditorToolbar.SplineOvalMode.IsChecked = true;
                        _SplineOvalMode = true;
                    }
                }
                else
                {
                    if (state)
                    {
                        Instance.EditorToolbar.SplineLineMode.IsChecked = false;
                        _SplineLineMode = false;
                        Instance.EditorToolbar.SplineOvalMode.IsChecked = true;
                        _SplineOvalMode = true;
                    }
                    else
                    {
                        Instance.EditorToolbar.SplineLineMode.IsChecked = true;
                        _SplineLineMode = true;
                        Instance.EditorToolbar.SplineOvalMode.IsChecked = false;
                        _SplineOvalMode = false;
                    }
                }
            }

            public Classes.Scene.EditorEntity SplineObjectRenderingTemplate { get; set; }

            public string SplineRenderingObjectName { get => GetSplineRenderingObjectName(); }

            private string GetSplineRenderingObjectName()
            {
                return SplineObjectRenderingTemplate.Object.Name.Name;
            }




        }
        public static Dictionary<int, SplineOptions> SplineOptionsGroup { get => GetSplineOptionsGroup(); set => SetSplineOptionsGroup(value); }

        private static Dictionary<int, SplineOptions> _SplineOptionsGroup = new Dictionary<int, SplineOptions>();

        public static void AddSplineOptionsGroup(int splineID)
        {
            SplineOptionsGroup.Add(splineID, new SplineOptions());
        }

        private static Dictionary<int, SplineOptions> GetSplineOptionsGroup()
        {
            return _SplineOptionsGroup;
        }

        private static void SetSplineOptionsGroup(Dictionary<int, SplineOptions> value)
        {
            _SplineOptionsGroup = value;
        }


        public static bool AllowSplineOptionsUpdate = true;

        public enum SplineOption
        {
            Size,
            LineMode,
            OvalMode,
            ShowLines,
            ShowObjects,
            ShowPoints,
            SpawnObject
        }

        public static void AdjustSplineGroupOptions(SplineOption option, object value)
        {
            if (!AllowSplineOptionsUpdate) return;
            switch (option)
            {
                case SplineOption.Size:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineSize = (int)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineSize = (int)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;
                case SplineOption.LineMode:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineLineMode = (bool)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineLineMode = (bool)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;
                case SplineOption.OvalMode:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineOvalMode = (bool)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineOvalMode = (bool)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;
                case SplineOption.ShowLines:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineToolShowLines = (bool)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineToolShowLines = (bool)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;
                case SplineOption.ShowObjects:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineToolShowObject = (bool)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineToolShowObject = (bool)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;
                case SplineOption.ShowPoints:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineToolShowPoints = (bool)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineToolShowPoints = (bool)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;
                case SplineOption.SpawnObject:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineObjectRenderingTemplate = (Classes.Scene.EditorEntity)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineObjectRenderingTemplate = (Classes.Scene.EditorEntity)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;

            }
        }

        public static int SelectedSplineID { get => GetSplineSelectedID(); set => SetSplineSelectedID(value); }
        private static int _SelectedSplineID = 0;
        private static int GetSplineSelectedID()
        {
            return _SelectedSplineID;
        }
        private static void SetSplineSelectedID(int value)
        {
            _SelectedSplineID = value;
        }
        #endregion

        #region Collision View Variables
        public static int CollisionPreset { get => GetCollisionPreset(); set => SetCollisionPreset(value); }
        private static int _CollisionPreset = 0;
        private static int GetCollisionPreset()
        {
            return _CollisionPreset;
        }
        private static void SetCollisionPreset(int value)
        {
            _CollisionPreset = value;

            Instance.EditorToolbar.CollisionInvertedMenuItem.IsChecked = false;
            Instance.EditorToolbar.CollisionCustomMenuItem.IsChecked = false;
            Instance.EditorToolbar.CollisionDefaultMenuItem.IsChecked = false;

            if (value == 0) Instance.EditorToolbar.CollisionDefaultMenuItem.IsChecked = true;
            else if (value == 1) Instance.EditorToolbar.CollisionInvertedMenuItem.IsChecked = true;
            else if (value == 2) Instance.EditorToolbar.CollisionCustomMenuItem.IsChecked = true;


            Instance.ReloadSpecificTextures(null, null);
            Instance.RefreshCollisionColours(true);
        }

        #region Collision Colors
        public static System.Drawing.Color CollisionTOColour { get => GetCollisionTOColour(); set => SetCollisionTOColour(value); }
        private static System.Drawing.Color _CollisionTOColour = System.Drawing.Color.Yellow;
        private static System.Drawing.Color GetCollisionTOColour() { return _CollisionTOColour; }
        private static void SetCollisionTOColour(System.Drawing.Color value) { _CollisionTOColour = value; }

        public static System.Drawing.Color CollisionLRDColour { get => GetCollisionLRDColour(); set => SetCollisionLRDColour(value); }
        private static System.Drawing.Color _CollisionLRDColour = System.Drawing.Color.Red;
        private static System.Drawing.Color GetCollisionLRDColour() { return _CollisionLRDColour; }
        private static void SetCollisionLRDColour(System.Drawing.Color value) { _CollisionLRDColour = value; }

        public static System.Drawing.Color CollisionSAColour { get => GetCollisionSAColour(); set => SetCollisionSAColour(value); }
        private static System.Drawing.Color _CollisionSAColour = System.Drawing.Color.White;
        private static System.Drawing.Color GetCollisionSAColour() { return _CollisionSAColour; }
        private static void SetCollisionSAColour(System.Drawing.Color value) { _CollisionSAColour = value; }
        #endregion
        #endregion

        #region Misc

        #region Prefrence/Status Values

        public static int SelectedTileID { get; set; } = -1; //For Tile Maniac Intergration via Right Click in Editor View Panel
        public static string CurrentLanguage { get; set; } = "EN"; //Current Selected Language
        public static Enums.Axis ScrollDirection { get; set; } = Enums.Axis.Y;
        public static int PlayerBeingTracked { get; set; } = -1;
        public static int CurrentControllerButtons { get; set; } = 2; //For Setting the Menu Control Button Images.
        public static int LevelID { get; set; } = -1; //Self Explanatory
        public static int LastQuickButtonState { get; set; } = 0; //Gets the Last Quick Button State, so we can tell what action was used last

        #endregion

        #region Unoptimized Technical Options
        public static bool RemoveStageConfigEntriesAllowed { get => GetRemoveStageConfigEntriesAllowed(); set => SetRemoveStageConfigEntriesAllowed(value); }
        private static bool _RemoveStageConfigEntriesAllowed = true;
        private static bool GetRemoveStageConfigEntriesAllowed()
        {
            return _RemoveStageConfigEntriesAllowed;
        }
        private static void SetRemoveStageConfigEntriesAllowed(bool value)
        {
            _RemoveStageConfigEntriesAllowed = value;
        }
        public static bool IsConsoleWindowOpen { get => GetIsConsoleWindowOpen(); set => SetIsConsoleWindowOpen(value); }
        private static bool _IsConsoleWindowOpen = false;
        private static bool GetIsConsoleWindowOpen()
        {
            return _IsConsoleWindowOpen;
        }
        private static void SetIsConsoleWindowOpen(bool value)
        {
            _IsConsoleWindowOpen = value;
        }
        public static bool DataDirectoryReadOnlyMode { get => GetDataDirectoryReadOnlyMode(); set => SetDataDirectoryReadOnlyMode(value); }
        private static bool _DataDirectoryReadOnlyMode = false;
        private static bool GetDataDirectoryReadOnlyMode()
        {
            return _DataDirectoryReadOnlyMode;
        }
        private static void SetDataDirectoryReadOnlyMode(bool value)
        {
            _DataDirectoryReadOnlyMode = value;
        }

        #endregion

        #region Unoptimized Multi Layer Edit Mode
        public static bool MultiLayerEditMode { get => GetMultiLayerEditMode(); set => SetMultiLayerEditMode(value); }
        private static bool _MultiLayerEditMode = true;
        private static bool _MultiLayerFirstTimeCheck = true;
        private static bool GetMultiLayerEditMode()
        {
            if (_MultiLayerFirstTimeCheck)
            {
                _MultiLayerFirstTimeCheck = false;
                SetMultiLayerEditMode(_MultiLayerEditMode);
            }

            return _MultiLayerEditMode;
        }
        private static void SetMultiLayerEditMode(bool value)
        {
            _MultiLayerEditMode = value;
            Instance.MenuBar.multiLayerSelectionToolStripMenuItem.IsChecked = value;


            bool enabled = (value == true ? true : false);
            Instance.EditorToolbar.EditFGLower.DualSelect = enabled;
            Instance.EditorToolbar.EditFGLow.DualSelect = enabled;
            Instance.EditorToolbar.EditFGHigh.DualSelect = enabled;
            Instance.EditorToolbar.EditFGHigher.DualSelect = enabled;

            Instance.EditorToolbar.EditFGLower.SwapDefaultToA(!enabled);
            Instance.EditorToolbar.EditFGLow.SwapDefaultToA(!enabled);
            Instance.EditorToolbar.EditFGHigh.SwapDefaultToA(!enabled);
            Instance.EditorToolbar.EditFGHigher.SwapDefaultToA(!enabled);

            foreach (var elb in Instance.EditorToolbar.ExtraLayerEditViewButtons.Values)
            {
                elb.DualSelect = enabled;
                elb.SwapDefaultToA(!enabled);
            }

            if (!enabled) Methods.Editor.Solution.EditLayerB = null;

            Methods.Internal.UserInterface.UpdateControls();
        }

        public static void UpdateMultiLayerSelectMode(bool updateControls = false)
        {
            bool enabled = (_MultiLayerEditMode == true ? true : false);
            Instance.EditorToolbar.EditFGLower.DualSelect = enabled;
            Instance.EditorToolbar.EditFGLow.DualSelect = enabled;
            Instance.EditorToolbar.EditFGHigh.DualSelect = enabled;
            Instance.EditorToolbar.EditFGHigher.DualSelect = enabled;

            Instance.EditorToolbar.EditFGLower.SwapDefaultToA(!enabled);
            Instance.EditorToolbar.EditFGLow.SwapDefaultToA(!enabled);
            Instance.EditorToolbar.EditFGHigh.SwapDefaultToA(!enabled);
            Instance.EditorToolbar.EditFGHigher.SwapDefaultToA(!enabled);

            foreach (var elb in Instance.EditorToolbar.ExtraLayerEditViewButtons.Values)
            {
                elb.DualSelect = enabled;
                elb.SwapDefaultToA(!enabled);
            }

            if (!enabled) Methods.Editor.Solution.EditLayerB = null;

            if (updateControls) Methods.Internal.UserInterface.UpdateControls();
        }

        #endregion

        #region To-Remove Settings
        public static bool UseLargeDebugStats { get => GetUseLargeDebugStats(); set => SetUseLargeDebugStats(value); }
        private static bool _UseLargeDebugStats = false;
        private static bool GetUseLargeDebugStats()
        {
            return _UseLargeDebugStats;
        }
        private static void SetUseLargeDebugStats(bool value)
        {
            _UseLargeDebugStats = value;
            Instance.MenuBar.useLargeTextToolStripMenuItem.IsChecked = value;
        }

        public static bool DebugStatsVisibleOnPanel { get => GetDebugStatsVisibleOnPanel(); set => SetDebugStatsVisibleOnPanel(value); }
        private static bool _DebugStatsVisibleOnPanel = false;
        private static bool GetDebugStatsVisibleOnPanel()
        {
            return _DebugStatsVisibleOnPanel;
        }
        private static void SetDebugStatsVisibleOnPanel(bool value)
        {
            _DebugStatsVisibleOnPanel = value;
            Instance.MenuBar.showStatsToolStripMenuItem.IsChecked = value;
            Instance.ViewPanel.InfoHUD.UpdatePopupSize();
        }
        #endregion

        #region To-Improve Implementation Variables
        public static bool AddStageConfigEntriesAllowed { get; set; } = true; //Self Explanatory
        public static bool isImportingObjects { get; set; } = false; //Determines if we are importing objects so we can disable all the other Scene Select Options
        public static bool EncorePaletteExists { get; set; } = false; // Determines if an Encore Pallete Exists

        #endregion

        public static bool Duplicate1
        {
            get
            {
                return _Duplicate1;
            }
            set
            {
                _Duplicate1 = value;
            }
        }
        private static bool _Duplicate1;

        #region Unoptimized Misc Stuff
        public static bool collisionOpacityChanged { get; set; } = false;
        public static int EncoreSetupType { get; set; } //Used to determine what kind of encore setup the stage uses
        public static string ObjectFilter { get; set; } = ""; //Used to hide objects that don't match the discription
        public static string LevelSelectCharS { get; set; } = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ*+,-./: \'\"";
        public static string MenuCharS { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ?!.";
        public static string MenuCharS_Small { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ?.:'\"!-,&¡<>¿"; //49 out of 121
        public static char[] MenuChar
        {
            get
            {
                return MenuCharS.ToCharArray();
            }
        }
        public static char[] LevelSelectChar
        {
            get
            {
                return LevelSelectCharS.ToCharArray();
            }
        }
        public static char[] MenuChar_Small
        {
            get
            {
                return MenuCharS_Small.ToCharArray();
            }
        }
        #endregion

        #endregion
    }
}
