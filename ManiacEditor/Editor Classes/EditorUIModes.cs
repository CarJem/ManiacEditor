using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor
{
    public class EditorUIModes
    {
        #region ShowTileID
        public bool ShowTileID { get => GetShowTileIDMode(); set => SetShowTileIDMode(value); } //Show Tile ID Status
        private bool _ShowTileID;
        private bool GetShowTileIDMode()
        {
            return _ShowTileID;
        }
        private void SetShowTileIDMode(bool value)
        {
            Editor.Instance.ShowTileIDButton.IsChecked = value;
            Editor.Instance.ReloadSpecificTextures(null, null);
            _ShowTileID = value;
        }
        #endregion
        #region ShowGrid
        public bool ShowGrid { get => GetShowGridMode(); set => SetShowGridMode(value); } // Show Grid Mode Status
        private bool _ShowGrid;
        private bool GetShowGridMode()
        {
            return _ShowGrid;
        }
        private void SetShowGridMode(bool value)
        {
            Editor.Instance.ShowGridButton.IsChecked = value;
            Editor.Instance.UIModes._ShowGrid = value;
        }
        #endregion
        #region UseEncoreColors
        public bool UseEncoreColors { get => GetUseEncoreColorsState(); set => SetUseEncoreColorsState(value); } // Show Encore Color Status
        private bool _UseEncoreColors = false;
        private bool GetUseEncoreColorsState()
        {
            return _UseEncoreColors;
        }
        private void SetUseEncoreColorsState(bool value)
        {
            Editor.Instance.DisposeTextures();
            Editor.Instance.EncorePaletteButton.IsChecked = value;
            Editor.Instance.UIModes._UseEncoreColors = value;
            Editor.Instance.EditorTiles.StageTiles?.Image.Reload((value ? Editor.Instance.EncorePalette[0] : null));
            Editor.Instance.TilesToolbar?.Reload((value ? Editor.Instance.EncorePalette[0] : null));
            Editor.Instance.EntityDrawing.ReleaseResources();
        }

        #endregion
        #region ShowCollisionA

        public bool ShowCollisionA { get => GetShowCollisionA(); set => SetShowCollisionA(value); } //Show Collision Path A Status

        private bool _ShowCollisionA;
        private bool GetShowCollisionA()
        {
            return _ShowCollisionA;
        }
        private void SetShowCollisionA(bool value)
        {
            Editor.Instance.ShowCollisionAButton.IsChecked = value;
            Editor.Instance.UIModes._ShowCollisionA = value;
            Editor.Instance.ShowCollisionBButton.IsChecked = false;
            Editor.Instance.UIModes._ShowCollisionB = false;
            Editor.Instance.ReloadSpecificTextures(null, null);
        }
        #endregion
        #region ShowCollisionB

        public bool ShowCollisionB { get => GetShowCollisionB(); set => SetShowCollisionB(value); } //Show Collision Path B Status

        private bool _ShowCollisionB;
        private bool GetShowCollisionB()
        {
            return _ShowCollisionB;
        }
        private void SetShowCollisionB(bool value)
        {

            Editor.Instance.ShowCollisionAButton.IsChecked = false;
            Editor.Instance.UIModes._ShowCollisionA = false;
            Editor.Instance.ShowCollisionBButton.IsChecked = value;
            Editor.Instance.UIModes._ShowCollisionB = value;
            Editor.Instance.ReloadSpecificTextures(null, null);
        }
        #endregion
        public int BackupType = 0; //Determines What Kind of Backup to Make
        #region UseMagnetMode
        public bool UseMagnetMode { get => GetUseMagnetMode(); set => SetUseMagnetMode(value); } // Determines the state of Magnet Mode
        private bool _UseMagnetMode = false;
        private bool GetUseMagnetMode()
        {
            return _UseMagnetMode;
        }
        private void SetUseMagnetMode(bool value)
        {
            Editor.Instance.UIModes._UseMagnetMode = value;
            Editor.Instance.MagnetMode.IsChecked = value;
        }
        #endregion
        #region UseMagnetXAxis
        public bool UseMagnetXAxis { get => GetUseMagnetXAxis(); set => SetUseMagnetXAxis(value); } //Determines if the Magnet should use the X Axis
        private bool _UseMagnetXAxis = true;
        private bool GetUseMagnetXAxis()
        {
            return _UseMagnetXAxis;
        }
        private void SetUseMagnetXAxis(bool value)
        {
            Editor.Instance.UIModes._UseMagnetXAxis = value;
            Editor.Instance.enableXAxisToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region UseMagnetYAxis
        public bool UseMagnetYAxis { get => GetUseMagnetYAxis(); set => SetUseMagnetYAxis(value); } //Determines if the Magnet should use the Y Axis Axis
        private bool _UseMagnetYAxis = true;
        private bool GetUseMagnetYAxis()
        {
            return _UseMagnetYAxis;
        }
        private void SetUseMagnetYAxis(bool value)
        {
            Editor.Instance.UIModes._UseMagnetYAxis = value;
            Editor.Instance.enableYAxisToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region ShowEntityPathArrows
        public bool ShowEntityPathArrows { get => GetShowEntityPathArrows(); set => SetShowEntityPathArrows(value); } //Determines if we want to see Object Arrow Paths
        private bool _ShowEntityPathArrows = true;
        private bool GetShowEntityPathArrows()
        {
            return _ShowEntityPathArrows;
        }
        private void SetShowEntityPathArrows(bool value)
        {
            Editor.Instance.UIModes._ShowEntityPathArrows = value;
            Editor.Instance.showEntityPathArrowsToolstripItem.IsChecked = value;
        }
        #endregion
        #region ShowWaterLevel
        public bool ShowWaterLevel { get => GetShowWaterLevel(); set => SetShowWaterLevel(value); } //Determines if the Water Object should show it's Water Level
        private bool _ShowWaterLevel = false;
        private bool GetShowWaterLevel()
        {
            return _ShowWaterLevel;
        }
        private void SetShowWaterLevel(bool value)
        {
            Editor.Instance.UIModes._ShowWaterLevel = value;
            Editor.Instance.showWaterLevelToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region AlwaysShowWaterLevel
        public bool AlwaysShowWaterLevel { get => GetAlwaysShowWaterLevel(); set => SetAlwaysShowWaterLevel(value); } //Determines if the Water Level Should be Shown at all times regardless of the object being selected
        private bool _AlwaysShowWaterLevel = false;
        private bool GetAlwaysShowWaterLevel()
        {
            return _AlwaysShowWaterLevel;
        }
        private void SetAlwaysShowWaterLevel(bool value)
        {
            Editor.Instance.UIModes._AlwaysShowWaterLevel = value;
            Editor.Instance.waterLevelAlwaysShowItem.IsChecked = value;
        }
        #endregion
        #region SizeWaterLevelwithBounds
        public bool SizeWaterLevelwithBounds { get => GetSizeWaterLevelwithBounds(); set => SetSizeWaterLevelwithBounds(value); } //Determines if the water level width should match those of the object's bounds
        private bool _SizeWaterLevelwithBounds = false;
        private bool GetSizeWaterLevelwithBounds()
        {
            return _SizeWaterLevelwithBounds;
        }
        private void SetSizeWaterLevelwithBounds(bool value)
        {
            Editor.Instance.UIModes._SizeWaterLevelwithBounds = value;
            Editor.Instance.sizeWithBoundsWhenNotSelectedToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region ExtraLayersMoveToFront
        public bool ExtraLayersMoveToFront { get => GetExtraLayersMoveToFront(); set => SetExtraLayersMoveToFront(value); }  //Determines if we should render the extra layers in front of everything on behind everything
        private bool _ExtraLayersMoveToFront = false;
        private bool GetExtraLayersMoveToFront()
        {
            return _ExtraLayersMoveToFront;
        }
        private void SetExtraLayersMoveToFront(bool value)
        {
            Editor.Instance.UIModes._ExtraLayersMoveToFront = value;
            Editor.Instance.moveExtraLayersToFrontToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region ShowFlippedTileHelper
        public bool ShowFlippedTileHelper { get => GetShowFlippedTileHelper(); set => SetShowFlippedTileHelper(value); }
        private bool _ShowFlippedTileHelper = false;
        private bool GetShowFlippedTileHelper()
        {
            return _ShowFlippedTileHelper;
        }
        private void SetShowFlippedTileHelper(bool value)
        {
            Editor.Instance.UIModes._ShowFlippedTileHelper = value;
            Editor.Instance.ReloadSpecificTextures(null, null);
        }
        #endregion
        #region ShowingDataDirectory
        public bool ShowingDataDirectory { get => GetShowingDataDirectory(); set => SetShowingDataDirectory(value); }
        private bool _ShowingDataDirectory = false;
        private bool GetShowingDataDirectory()
        {
            return _ShowingDataDirectory;
        }
        private void SetShowingDataDirectory(bool value)
        {
            Editor.Instance.UIModes._ShowingDataDirectory = value;
        }
        #endregion
        #region ShowParallaxSprites
        public bool ShowParallaxSprites { get => GetShowParallaxSprites(); set => SetShowParallaxSprites(value); }
        private bool _ShowParallaxSprites = false;
        private bool GetShowParallaxSprites()
        {
            return _ShowParallaxSprites;
        }
        private void SetShowParallaxSprites(bool value)
        {
            Editor.Instance.UIModes._ShowParallaxSprites = value;
            Editor.Instance.showParallaxSpritesToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region ApplyEditEntitiesTransparency
        public bool ApplyEditEntitiesTransparency { get => GetApplyEditEntitiesTransparency(); set => SetApplyEditEntitiesTransparency(value); }
        private bool _ApplyEditEntitiesTransparency = false;
        private bool GetApplyEditEntitiesTransparency()
        {
            return _ApplyEditEntitiesTransparency;
        }
        private void SetApplyEditEntitiesTransparency(bool value)
        {
            Editor.Instance.UIModes._ApplyEditEntitiesTransparency = value;
            Editor.Instance.EditEntitiesTransparencyToggle.IsChecked = value;
            Editor.Instance.QuickEditEntitiesTransparentLayers.IsChecked = value;
        }
        #endregion
        #region ShowEntitySelectionBoxes
        public bool ShowEntitySelectionBoxes { get => GetShowEntitySelectionBoxes(); set => SetShowEntitySelectionBoxes(value); }
        private bool _ShowEntitySelectionBoxes = false;
        private bool GetShowEntitySelectionBoxes()
        {
            return _ShowEntitySelectionBoxes;
        }
        private void SetShowEntitySelectionBoxes(bool value)
        {
            Editor.Instance.UIModes._ShowEntitySelectionBoxes = value;
            Editor.Instance.showEntitySelectionBoxesToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region EnablePixelCountMode
        public bool EnablePixelCountMode { get => GetEnablePixelCountMode(); set => SetEnablePixelCountMode(value); }
        private bool _EnablePixelCountMode = false;
        private bool GetEnablePixelCountMode()
        {
            return _EnablePixelCountMode;
        }
        private void SetEnablePixelCountMode(bool value)
        {
            Editor.Instance.UIModes._EnablePixelCountMode = value;
            Editor.Instance.pixelModeButton.IsChecked = value;
            Editor.Instance.pixelModeToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region IsConsoleWindowOpen
        public bool IsConsoleWindowOpen { get => GetIsConsoleWindowOpen(); set => SetIsConsoleWindowOpen(value); }
        private bool _IsConsoleWindowOpen = false;
        private bool GetIsConsoleWindowOpen()
        {
            return _IsConsoleWindowOpen;
        }
        private void SetIsConsoleWindowOpen(bool value)
        {
            Editor.Instance.UIModes._IsConsoleWindowOpen = value;
        }
        #endregion
        #region RightClicktoSwapSlotID
        public bool RightClicktoSwapSlotID { get => GetRightClicktoSwapSlotID(); set => SetRightClicktoSwapSlotID(value); }
        private bool _RightClicktoSwapSlotID = false;
        private bool GetRightClicktoSwapSlotID()
        {
            return _RightClicktoSwapSlotID;
        }
        private void SetRightClicktoSwapSlotID(bool value)
        {
            Editor.Instance.UIModes._RightClicktoSwapSlotID = value;
            Editor.Instance.rightClicktoSwapSlotIDs.IsChecked = value;
        }
        #endregion
        #region EntitySelectionBoxesAlwaysPrioritized
        public bool EntitySelectionBoxesAlwaysPrioritized { get => GetEntitySelectionBoxesAlwaysPrioritized(); set => SetEntitySelectionBoxesAlwaysPrioritized(value); }
        private bool _EntitySelectionBoxesAlwaysPrioritized = false;
        private bool GetEntitySelectionBoxesAlwaysPrioritized()
        {
            return _EntitySelectionBoxesAlwaysPrioritized;
        }
        private void SetEntitySelectionBoxesAlwaysPrioritized(bool value)
        {
            Editor.Instance.UIModes._EntitySelectionBoxesAlwaysPrioritized = value;
            Editor.Instance.SelectionBoxesAlwaysPrioritized.IsChecked = value;
        }
        #endregion
        #region DataDirectoryReadOnlyMode
        public bool DataDirectoryReadOnlyMode { get => GetDataDirectoryReadOnlyMode(); set => SetDataDirectoryReadOnlyMode(value); }
        private bool _DataDirectoryReadOnlyMode = false;
        private bool GetDataDirectoryReadOnlyMode()
        {
            return _DataDirectoryReadOnlyMode;
        }
        private void SetDataDirectoryReadOnlyMode(bool value)
        {
            Editor.Instance.UIModes._DataDirectoryReadOnlyMode = value;
        }
        #endregion
        #region ScrollLocked
        public bool ScrollLocked { get => GetScrollLocked(); set => SetScrollLocked(value); }
        private bool _ScrollLocked = true;
        private bool GetScrollLocked()
        {
            return _ScrollLocked;
        }
        private void SetScrollLocked(bool value)
        {
            Editor.Instance.UIModes._ScrollLocked = value;
            Editor.Instance.scrollLockButton.IsChecked = value;
            Editor.Instance.statusNAToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region FasterNudge

        #region Toggle

        public bool EnableFasterNudge { get => GetEnableFasterNudge(); set => SetEnableFasterNudge(value); }
        private bool _EnableFasterNudge = false;
        private bool GetEnableFasterNudge()
        {
            return _EnableFasterNudge;
        }
        private void SetEnableFasterNudge(bool value)
        {
            Editor.Instance.nudgeFasterButton.IsChecked = value;
            Editor.Instance.nudgeSelectionFasterToolStripMenuItem.IsChecked = value;
            _EnableFasterNudge = value;
        }

        #endregion


        #region Amount

        public int FasterNudgeAmount { get => GetFasterNudgeAmount(); set => SetFasterNudgeAmount(value); }
        private int _FasterNudgeAmount = 5;
        private int GetFasterNudgeAmount()
        {
            return _FasterNudgeAmount;
        }
        private void SetFasterNudgeAmount(int value)
        {
            _FasterNudgeAmount = value;
        }

        #endregion

        #endregion
        #region MovingPlatformsChecked
        public bool MovingPlatformsChecked { get => GetMovingPlatformsChecked(); set => SetMovingPlatformsChecked(value); }
        private bool _MovingPlatformsChecked = true;
        private bool GetMovingPlatformsChecked()
        {
            return _MovingPlatformsChecked;
        }
        private void SetMovingPlatformsChecked(bool value)
        {
            Editor.Instance.UIModes._MovingPlatformsChecked = value;
        }
        #endregion
        #region AnnimationsChecked
        public bool AnnimationsChecked { get => GetAnnimationsChecked(); set => SetAnnimationsChecked(value); }
        private bool _AnnimationsChecked = true;
        private bool GetAnnimationsChecked()
        {
            return _AnnimationsChecked;
        }
        private void SetAnnimationsChecked(bool value)
        {
            Editor.Instance.UIModes._AnnimationsChecked = value;
        }
        #endregion
        #region RemoveStageConfigEntriesAllowed
        public bool RemoveStageConfigEntriesAllowed { get => GetRemoveStageConfigEntriesAllowed(); set => SetRemoveStageConfigEntriesAllowed(value); }
        private bool _RemoveStageConfigEntriesAllowed = true;
        private bool GetRemoveStageConfigEntriesAllowed()
        {
            return _RemoveStageConfigEntriesAllowed;
        }
        private void SetRemoveStageConfigEntriesAllowed(bool value)
        {
            Editor.Instance.UIModes._RemoveStageConfigEntriesAllowed = value;
        }
        #endregion
        #region CopyAir
        public bool CopyAir { get => GetCopyAirMode(); set => SetCopyAirMode(value); }
        private bool _CopyAir = false;
        private bool GetCopyAirMode()
        {
            return _CopyAir;
        }
        private void SetCopyAirMode(bool value)
        {
            _CopyAir = value;
            Editor.Instance.copyAirToggle.IsChecked = value;
        }
        #endregion
        #region Tools/Brushes
        public void PointerMode(bool? value = null)
        {
            if (value != null) SetToolModes(0, value.Value);
            else SetToolModes(0, Editor.Instance.PointerToolButton.IsChecked.Value);
        }
        public void SelectionMode(bool? value = null)
        {
            if (value != null) SetToolModes(1, value.Value);
            else SetToolModes(1, Editor.Instance.SelectToolButton.IsChecked.Value);
        }
        public void DrawMode(bool? value = null)
        {
            if (value != null) SetToolModes(2, value.Value);
            else SetToolModes(2, Editor.Instance.DrawToolButton.IsChecked.Value);
        }
        public void InteractionMode(bool? value = null)
        {
            SetToolModes(3, Editor.Instance.InteractionToolButton.IsChecked.Value);
        }

        public void SplineMode(bool? value = null)
        {
            if (value != null) SetToolModes(4, value.Value);
            else SetToolModes(4, Editor.Instance.SplineToolButton.IsChecked.Value);
        }
        public void ChunksMode()
        {
            if (Editor.Instance.IsTilesEdit()) Editor.Instance.ChunksToolButton.IsChecked ^= true;
            Editor.Instance.UI.UpdateControls();
        }
        public void SetToolModes(int selectedID, bool value)
        {
            Editor.Instance.PointerToolButton.IsChecked = (selectedID == 0 ? value : false);
            Editor.Instance.SelectToolButton.IsChecked = (selectedID == 1 ? value : false);
            Editor.Instance.DrawToolButton.IsChecked = (selectedID == 2 ? value : false);
            Editor.Instance.InteractionToolButton.IsChecked = (selectedID == 3 ? value : false);
            Editor.Instance.SplineToolButton.IsChecked = (selectedID == 4 ? value : false);
            Editor.Instance.UI.UpdateControls();
        }

        #region Spline Stuff
        public class SplineOptions
        {
            #region Spline Related Modes
            #region Spline Point Seperation Size 
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
                if (Editor.Instance.UI != null) Editor.Instance.UI.UpdateControls();
            }


            #endregion
            public bool SplineToolShowLines { get; set; } = true; //Self Explanatory
            public int SplineCurrentPointsDrawn { get; set; } = 0; //Self Explanatory
            public int SplineTotalNumberOfObjects { get; set; } = 0; //Self Explanatory
            public bool SplineToolShowPoints { get; set; } = true; //Self Explanatory
            public bool SplineToolShowObject { get; set; } = false; //Self Explanatory

            #region Spline Line Modes
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
                        Editor.Instance.SplineLineMode.IsChecked = true;
                        _SplineLineMode = true;
                        Editor.Instance.SplineOvalMode.IsChecked = false;
                        _SplineOvalMode = false;
                    }
                    else
                    {
                        Editor.Instance.SplineLineMode.IsChecked = false;
                        _SplineLineMode = false;
                        Editor.Instance.SplineOvalMode.IsChecked = true;
                        _SplineOvalMode = true;
                    }
                }
                else
                {
                    if (state)
                    {
                        Editor.Instance.SplineLineMode.IsChecked = false;
                        _SplineLineMode = false;
                        Editor.Instance.SplineOvalMode.IsChecked = true;
                        _SplineOvalMode = true;
                    }
                    else
                    {
                        Editor.Instance.SplineLineMode.IsChecked = true;
                        _SplineLineMode = true;
                        Editor.Instance.SplineOvalMode.IsChecked = false;
                        _SplineOvalMode = false;
                    }
                }
            }

            public EditorEntity SplineObjectRenderingTemplate { get; set; }

            public string SplineRenderingObjectName { get => GetSplineRenderingObjectName(); }

            private string GetSplineRenderingObjectName()
            {
                return SplineObjectRenderingTemplate.Entity.Object.Name.Name;
            }

            #endregion

            #endregion
        }
        public Dictionary<int, SplineOptions> SplineOptionsGroup { get => GetSplineOptionsGroup(); set => SetSplineOptionsGroup(value); }

        private Dictionary<int, SplineOptions> _SplineOptionsGroup = new Dictionary<int, SplineOptions>();

        public void AddSplineOptionsGroup(int splineID)
        {
            SplineOptionsGroup.Add(splineID, new SplineOptions());
        }

        private Dictionary<int, SplineOptions> GetSplineOptionsGroup()
        {
            return _SplineOptionsGroup;
        }

        private void SetSplineOptionsGroup(Dictionary<int, SplineOptions> value)
        {
            _SplineOptionsGroup = value;
        }


        public bool AllowSplineOptionsUpdate = true;

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

        public void AdjustSplineGroupOptions(SplineOption option, object value)
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
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineObjectRenderingTemplate = (EditorEntity)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineObjectRenderingTemplate = (EditorEntity)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;

            }
        }

        #region Selected Spline ID
        public int SelectedSplineID { get => GetSplineSelectedID(); set => SetSplineSelectedID(value); }
        private int _SelectedSplineID = 0;

        private int GetSplineSelectedID()
        {
            return _SelectedSplineID;
        }
        private void SetSplineSelectedID(int value)
        {
            _SelectedSplineID = value;
        }
        #endregion

        #endregion



        #endregion
        #region Multi Layer Mode
        public bool MultiLayerEditMode { get => GetMultiLayerEditMode(); set => SetMultiLayerEditMode(value); }
        private bool _MultiLayerEditMode = true;
        private bool _MultiLayerFirstTimeCheck = true;
        private bool GetMultiLayerEditMode()
        {
            if (_MultiLayerFirstTimeCheck)
            {
                _MultiLayerFirstTimeCheck = false;
                SetMultiLayerEditMode(_MultiLayerEditMode);
            }

            return _MultiLayerEditMode;
        }
        private void SetMultiLayerEditMode(bool value)
        {
            Editor.Instance.UIModes._MultiLayerEditMode = value;
            Editor.Instance.multiLayerSelectionToolStripMenuItem.IsChecked = value;


            bool enabled = (value == true ? true : false);
            Editor.Instance.EditFGLower.DualSelect = enabled;
            Editor.Instance.EditFGLow.DualSelect = enabled;
            Editor.Instance.EditFGHigh.DualSelect = enabled;
            Editor.Instance.EditFGHigher.DualSelect = enabled;

            Editor.Instance.EditFGLower.SwapDefaultToA(!enabled);
            Editor.Instance.EditFGLow.SwapDefaultToA(!enabled);
            Editor.Instance.EditFGHigh.SwapDefaultToA(!enabled);
            Editor.Instance.EditFGHigher.SwapDefaultToA(!enabled);

            foreach (var elb in Editor.Instance.ExtraLayerEditViewButtons.Values)
            {
                elb.DualSelect = enabled;
                elb.SwapDefaultToA(!enabled);
            }

            if (!enabled) Editor.Instance.EditLayerB = null;

            Editor.Instance.UI.UpdateControls();
        }

        public void UpdateMultiLayerSelectMode(bool updateControls = false)
        {
            bool enabled = (_MultiLayerEditMode == true ? true : false);
            Editor.Instance.EditFGLower.DualSelect = enabled;
            Editor.Instance.EditFGLow.DualSelect = enabled;
            Editor.Instance.EditFGHigh.DualSelect = enabled;
            Editor.Instance.EditFGHigher.DualSelect = enabled;

            Editor.Instance.EditFGLower.SwapDefaultToA(!enabled);
            Editor.Instance.EditFGLow.SwapDefaultToA(!enabled);
            Editor.Instance.EditFGHigh.SwapDefaultToA(!enabled);
            Editor.Instance.EditFGHigher.SwapDefaultToA(!enabled);

            foreach (var elb in Editor.Instance.ExtraLayerEditViewButtons.Values)
            {
                elb.DualSelect = enabled;
                elb.SwapDefaultToA(!enabled);
            }

            if (!enabled) Editor.Instance.EditLayerB = null;

            if (updateControls) Editor.Instance.UI.UpdateControls();
        }
        #endregion
        #region Use Large Debug Text
        public bool UseLargeDebugStats { get => GetUseLargeDebugStats(); set => SetUseLargeDebugStats(value); }
        private bool _UseLargeDebugStats = false;
        private bool GetUseLargeDebugStats()
        {
            return _UseLargeDebugStats;
        }
        private void SetUseLargeDebugStats(bool value)
        {
            _UseLargeDebugStats = value;
            Editor.Instance.useLargeTextToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region Show Debug Text
        public bool DebugStatsVisibleOnPanel { get => GetDebugStatsVisibleOnPanel(); set => SetDebugStatsVisibleOnPanel(value); }
        private bool _DebugStatsVisibleOnPanel = false;
        private bool GetDebugStatsVisibleOnPanel()
        {
            return _DebugStatsVisibleOnPanel;
        }
        private void SetDebugStatsVisibleOnPanel(bool value)
        {
            _DebugStatsVisibleOnPanel = value;
            Editor.Instance.showStatsToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region Prioritized Entity Viewing
        public bool PrioritizedEntityViewing { get => GetPrioritizedEntityViewing(); set => SetPrioritizedEntityViewing(value); }
        private bool _PrioritizedEntityViewing = false;
        private bool GetPrioritizedEntityViewing()
        {
            return _PrioritizedEntityViewing;
        }
        private void SetPrioritizedEntityViewing(bool value)
        {
            _PrioritizedEntityViewing = value;
            Editor.Instance.prioritizedViewingToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region Magnet Mode Size 
        //Determines the Magnets Size
        public int MagnetSize { get => GetMagnetSize(); set => SetMagnetSize(value); }
        private int _MagnetSize = 16;
        public int CustomMagnetSize = 16;
        private int GetMagnetSize()
        {
            return _MagnetSize;
        }
        private void SetMagnetSize(int value)
        {
            bool isCustom = false;
            Editor.Instance.x8ToolStripMenuItem.IsChecked = false;
            Editor.Instance.x16ToolStripMenuItem1.IsChecked = false;
            Editor.Instance.x32ToolStripMenuItem.IsChecked = false;
            Editor.Instance.x64ToolStripMenuItem.IsChecked = false;
            Editor.Instance.MagnetCustomSizeToolStripMenuItem.IsChecked = false;

            if (value == 8) Editor.Instance.x8ToolStripMenuItem.IsChecked = true;
            else if (value == 16) Editor.Instance.x16ToolStripMenuItem1.IsChecked = true;
            else if (value == 32) Editor.Instance.x32ToolStripMenuItem.IsChecked = true;
            else if (value == 64) Editor.Instance.x64ToolStripMenuItem.IsChecked = true;
            else if (value == -1)
            {
                isCustom = true;
                Editor.Instance.MagnetCustomSizeToolStripMenuItem.IsChecked = true;
            }

            Editor.Instance.CustomMagnetLabel.Text = string.Format(Editor.Instance.CustomMagnetLabel.Tag.ToString(), CustomMagnetSize);

            if (!isCustom) _MagnetSize = value;
            else _MagnetSize = CustomMagnetSize;
        }

        #endregion
        #region Grid Size 
        //Determines the Magnets Size
        public int GridSize { get => GetGridSize(); set => SetGridSize(value); }
        private int _GridSize = 16;
        public int GridCustomSize { get => GetCustomSize(); set => ChangeCustomSize(value); }
        private int _GridCustomSize = Properties.Defaults.Default.CustomGridSizeValue;

        private void ChangeCustomSize(int value)
        {
            _GridCustomSize = value;
            Editor.Instance.CustomGridLabel.Text = string.Format(Editor.Instance.CustomGridLabel.Tag.ToString(), _GridCustomSize);
        }

        private int GetCustomSize()
        {
            Editor.Instance.CustomGridLabel.Text = string.Format(Editor.Instance.CustomGridLabel.Tag.ToString(), _GridCustomSize);
            return _GridCustomSize;
        }

        private int GetGridSize()
        {
            return _GridSize;
        }
        private void SetGridSize(int value)
        {
            bool isCustom = false;
            _GridSize = value;

            Editor.Instance.Grid16x16SizeMenuItem.IsChecked = false;
            Editor.Instance.Grid128x128SizeMenuItem.IsChecked = false;
            Editor.Instance.Grid256x256SizeMenuItem.IsChecked = false;
            Editor.Instance.GridCustomSizeMenuItem.IsChecked = false;

            if (value == 16) Editor.Instance.Grid16x16SizeMenuItem.IsChecked = true;
            else if (value == 128) Editor.Instance.Grid128x128SizeMenuItem.IsChecked = true;
            else if (value == 256) Editor.Instance.Grid256x256SizeMenuItem.IsChecked = true;
            else if (value == -1)
            {
                isCustom = true;
                Editor.Instance.GridCustomSizeMenuItem.IsChecked = true;
            }

            Editor.Instance.CustomGridLabel.Text = string.Format(Editor.Instance.CustomGridLabel.Tag.ToString(), GridCustomSize);

            if (!isCustom) _GridSize = value;
            else _GridSize = GridCustomSize;
        }
        #endregion
        #region Entities Visibile Above All Other Layers
        public bool EntitiesVisibileAboveAllLayers { get => GetEntitiesVisibileAboveAllLayers(); set => SetEntitiesVisibileAboveAllLayers(value); }
        private bool _EntitiesVisibileAboveAllLayers = false;
        private bool GetEntitiesVisibileAboveAllLayers()
        {
            return _EntitiesVisibileAboveAllLayers;
        }
        private void SetEntitiesVisibileAboveAllLayers(bool value)
        {
            Editor.Instance.UIModes._EntitiesVisibileAboveAllLayers = value;
            Editor.Instance.SelectionBoxesAlwaysPrioritized.IsChecked = value;
        }

        #endregion
        #region Collision Preset 
        //Determines the Collision View Mode
        public int CollisionPreset { get => GetCollisionPreset(); set => SetCollisionPreset(value); }
        private int _CollisionPreset = 0;
        private int GetCollisionPreset()
        {
            return _CollisionPreset;
        }
        private void SetCollisionPreset(int value)
        {
            _CollisionPreset = value;

            Editor.Instance.invertedToolStripMenuItem.IsChecked = false;
            Editor.Instance.customToolStripMenuItem1.IsChecked = false;
            Editor.Instance.defaultToolStripMenuItem.IsChecked = false;

            if (value == 0) Editor.Instance.defaultToolStripMenuItem.IsChecked = true;
            else if(value == 1) Editor.Instance.invertedToolStripMenuItem.IsChecked = true;
            else if (value == 2) Editor.Instance.customToolStripMenuItem1.IsChecked = true;


            Editor.Instance.ReloadSpecificTextures(null, null);
            Editor.Instance.RefreshCollisionColours(true);
        }
        #endregion

        #region Collision Colors

        public System.Drawing.Color CollisionTOColour { get => GetCollisionTOColour(); set => SetCollisionTOColour(value); }
        private System.Drawing.Color _CollisionTOColour = System.Drawing.Color.Yellow;
        private System.Drawing.Color GetCollisionTOColour() { return _CollisionTOColour; }
        private void SetCollisionTOColour(System.Drawing.Color value) {  _CollisionTOColour = value; }

        public System.Drawing.Color CollisionLRDColour { get => GetCollisionLRDColour(); set => SetCollisionLRDColour(value); }
        private System.Drawing.Color _CollisionLRDColour = System.Drawing.Color.Red;
        private System.Drawing.Color GetCollisionLRDColour() { return _CollisionLRDColour; }
        private void SetCollisionLRDColour(System.Drawing.Color value) { _CollisionLRDColour = value; }

        public System.Drawing.Color CollisionSAColour { get => GetCollisionSAColour(); set => SetCollisionSAColour(value); }
        private System.Drawing.Color _CollisionSAColour = System.Drawing.Color.White;
        private System.Drawing.Color GetCollisionSAColour() { return _CollisionSAColour; }
        private void SetCollisionSAColour(System.Drawing.Color value) { _CollisionSAColour = value; }

        #endregion

        #region Grid, Water And Other Colors

        public System.Drawing.Color waterColor { get => GetWaterEntityColor(); set => SetWaterEntityColor(value); }
        private System.Drawing.Color _WaterEntityColor = System.Drawing.Color.Blue;
        private System.Drawing.Color GetWaterEntityColor() { return _WaterEntityColor; }
        private void SetWaterEntityColor(System.Drawing.Color value) { _WaterEntityColor = value; }

        public System.Drawing.Color GridColor { get => GetGridColor(); set => SetGridColor(value); }
        private System.Drawing.Color _GridColor = System.Drawing.Color.Red;
        private System.Drawing.Color GetGridColor() { return _GridColor; }
        private void SetGridColor(System.Drawing.Color value) { _GridColor = value; }

        #endregion

        public bool AddStageConfigEntriesAllowed { get; set; } = true; //Self Explanatory
        public bool isImportingObjects { get; set; } = false; //Determines if we are importing objects so we can disable all the other Scene Select Options
        public bool EncorePaletteExists { get; set; } = false; // Determines if an Encore Pallete Exists
        public bool ForceWarp { get; set; } = false; //For Shortcuts and Force Open.
        public bool ShortcutHasZoom { get; set; } = false; //For Shortcuts and Force Open.
        public bool collisionOpacityChanged { get; set; } = false;

        public static bool UpdateUpdaterMessage = false;

        public bool RequireSaveCheck { get; set; } = false;


        public System.Drawing.Point TempWarpCoords = new System.Drawing.Point(0, 0); //Temporary Warp Position for Shortcuts and Force Open

        public int ScrollDirection { get; set; } = 1;


        public int EncoreSetupType { get; set; } //Used to determine what kind of encore setup the stage uses
        public int selectPlayerObject_GoTo { get; set; } = 0; //Used to determine which player object to go to
        public int PlayerBeingTracked { get; set; } = -1;
        public int CurrentControllerButtons { get; set; } = 2; //For Setting the Menu Control Button Images.
        public int LevelID { get; set; } = -1; //Self Explanatory
        public int LastQuickButtonState { get; set; } = 0; //Gets the Last Quick Button State, so we can tell what action was used last
        public int SelectedTileID { get; set; } = -1; //For Tile Maniac Intergration via Right Click in Editor View Panel
        public string CurrentLanguage { get; set; } = "EN"; //Current Selected Language
        public string INILayerNameLower { get; set; } = ""; //Reserved String for INI Default Layer Prefrences
        public string INILayerNameHigher { get; set; } = ""; //Reserved String for INI Default Layer Prefrences
        public string entitiesTextFilter { get; set; } = ""; //Used to hide objects that don't match the discription
        public string LevelSelectCharS { get; set; } = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ*+,-./: \'\"";
        public string MenuCharS { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ?!.";
        public string MenuCharS_Small { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ?.:'\"!-,&¡<>¿"; //49 out of 121
        public string MultiLayerA { get; set; } = "";
        public string MultiLayerB { get; set; } = "";

        public char[] MenuChar { get; set; }
        public char[] LevelSelectChar { get; set; }
        public char[] MenuChar_Small { get; set; }

        public double ShortcutZoomValue { get; set; } = 0.0;

        public EditorUIModes()
		{

		}


    }
}
