using System.Collections.Generic;

namespace ManiacEditor
{
    public class UserStateModel
    {
        #region View Options

        public bool EntitiesVisibileAboveAllLayers
        {
            get
            {
                return _EntitiesVisibileAboveAllLayers;
            }
            set
            {
                _EntitiesVisibileAboveAllLayers = value;
                Editor.Instance.EditorMenuBar.SelectionBoxesAlwaysPrioritized.IsChecked = value;
            }
        }
        private bool _EntitiesVisibileAboveAllLayers = false;

        public bool PrioritizedEntityViewing
        {
            get
            {
                return _PrioritizedEntityViewing;
            }
            set
            {
                _PrioritizedEntityViewing = value;
                Editor.Instance.EditorMenuBar.prioritizedViewingToolStripMenuItem.IsChecked = value;
            }
        }
        private bool _PrioritizedEntityViewing = false;

        public bool ParallaxAnimationChecked
        {
            get
            {
                return _ParallaxAnimationChecked;
            }
            set
            {
                _ParallaxAnimationChecked = value;
                Editor.Instance.UI.UpdateControls();
            }
        }
        private bool _ParallaxAnimationChecked = false;

        public bool AllowAnimations
        {
            get
            {
                return _AllowAnimations;
            }
            set
            {
                _AllowAnimations = value;
                Editor.Instance.UI.UpdateControls();
            }
        }
        private bool _AllowAnimations = true;

        public bool AllowSpriteAnimations
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
        private bool _AllowSpriteAnimations = true;

        public bool AllowMovingPlatformAnimations
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
        private bool _AllowMovingPlatformAnimations = true;

        public bool EntitySelectionBoxesAlwaysPrioritized
        {
            get
            {
                return _EntitySelectionBoxesAlwaysPrioritized;
            }
            set
            {
                Editor.Instance.Options._EntitySelectionBoxesAlwaysPrioritized = value;
                Editor.Instance.EditorMenuBar.SelectionBoxesAlwaysPrioritized.IsChecked = value;
            }
        }
        private bool _EntitySelectionBoxesAlwaysPrioritized = false;

        public bool ShowTileID
        {
            get { return _ShowTileID; }
            set
            {
                Editor.Instance.EditorToolbar.ShowTileIDButton.IsChecked = value;
                Editor.Instance.ReloadSpecificTextures(null, null);
                _ShowTileID = value;
            }
        }
        private bool _ShowTileID;

        public bool ShowGrid
        {
            get { return _ShowGrid; }
            set
            {
                Editor.Instance.EditorToolbar.ShowGridToggleButton.IsChecked = value;
                Editor.Instance.Options._ShowGrid = value;
            }
        }
        private bool _ShowGrid;

        public bool UseEncoreColors
        {
            get { return _UseEncoreColors; }
            set
            {
                Editor.Instance.DisposeTextures();
                Editor.Instance.EditorToolbar.EncorePaletteButton.IsChecked = value;
                Editor.Instance.Options._UseEncoreColors = value;
                Classes.Edit.Scene.EditorSolution.CurrentTiles.StageTiles?.Image.Reload((value ? Editor.Instance.EncorePalette[0] : null));
                Editor.Instance.TilesToolbar?.Reload((value ? Editor.Instance.EncorePalette[0] : null));
                Editor.Instance.EntityDrawing.ReleaseResources();
            }
        }
        private bool _UseEncoreColors = false;

        public bool ShowCollisionA
        {
            get { return _ShowCollisionA; }
            set
            {
                Editor.Instance.EditorToolbar.ShowCollisionAButton.IsChecked = value;
                Editor.Instance.Options._ShowCollisionA = value;
                Editor.Instance.EditorToolbar.ShowCollisionBButton.IsChecked = false;
                Editor.Instance.Options._ShowCollisionB = false;
                Editor.Instance.ReloadSpecificTextures(null, null);
            }
        }
        private bool _ShowCollisionA;

        public bool ShowCollisionB
        {
            get { return _ShowCollisionB; }
            set
            {
                Editor.Instance.EditorToolbar.ShowCollisionAButton.IsChecked = false;
                Editor.Instance.Options._ShowCollisionA = false;
                Editor.Instance.EditorToolbar.ShowCollisionBButton.IsChecked = value;
                Editor.Instance.Options._ShowCollisionB = value;
                Editor.Instance.ReloadSpecificTextures(null, null);
            }
        }
        private bool _ShowCollisionB;

        public bool ShowParallaxSprites
        {
            get { return _ShowParallaxSprites; }
            set
            {
                _ShowParallaxSprites = value;
                Editor.Instance.EditorMenuBar.showParallaxSpritesToolStripMenuItem.IsChecked = value;
            }
        }
        private bool _ShowParallaxSprites = false;

        public bool ApplyEditEntitiesTransparency
        {
            get { return _ApplyEditEntitiesTransparency; }
            set
            {
                Editor.Instance.Options._ApplyEditEntitiesTransparency = value;
                Editor.Instance.EditorMenuBar.EditEntitiesTransparencyToggle.IsChecked = value;
                Editor.Instance.EditorStatusBar.QuickEditEntitiesTransparentLayers.IsChecked = value;
            }
        }
        private bool _ApplyEditEntitiesTransparency = false;

        public bool ShowEntitySelectionBoxes
        {
            get { return _ShowEntitySelectionBoxes; }
            set
            {
                Editor.Instance.Options._ShowEntitySelectionBoxes = value;
                Editor.Instance.EditorMenuBar.showEntitySelectionBoxesToolStripMenuItem.IsChecked = value;
            }
        }
        private bool _ShowEntitySelectionBoxes = false;

        public bool ShowEntityPathArrows
        {
            get
            {
                return _ShowEntityPathArrows;
            }
            set
            {
                _ShowEntityPathArrows = value;
                Editor.Instance.EditorMenuBar.showEntityPathArrowsToolstripItem.IsChecked = value;
            }
        }
        private bool _ShowEntityPathArrows = true;

        public bool ShowWaterLevel
        {
            get
            {
                return _ShowWaterLevel;
            }
            set
            {
                _ShowWaterLevel = value;
                Editor.Instance.EditorMenuBar.showWaterLevelToolStripMenuItem.IsChecked = value;
            }
        }
        private bool _ShowWaterLevel = false;

        public bool AlwaysShowWaterLevel
        {
            get
            {
                return _AlwaysShowWaterLevel;
            }
            set
            {
                _AlwaysShowWaterLevel = value;
                Editor.Instance.EditorMenuBar.waterLevelAlwaysShowItem.IsChecked = value;
            }
        }
        private bool _AlwaysShowWaterLevel = false;

        public bool SizeWaterLevelwithBounds
        {
            get 
            {
                return _SizeWaterLevelwithBounds;
            }
            set
            {
                _SizeWaterLevelwithBounds = value;
                Editor.Instance.EditorMenuBar.sizeWithBoundsWhenNotSelectedToolStripMenuItem.IsChecked = value;
            }
        }
        private bool _SizeWaterLevelwithBounds = false;

        public bool ExtraLayersMoveToFront
        {
            get
            {
                return _ExtraLayersMoveToFront;
            }
            set
            {
                Editor.Instance.Options._ExtraLayersMoveToFront = value;
                Editor.Instance.EditorMenuBar.moveExtraLayersToFrontToolStripMenuItem.IsChecked = value;
            }
        }
        private bool _ExtraLayersMoveToFront = false;

        public bool ShowFlippedTileHelper
        {
            get
            {
                return _ShowFlippedTileHelper;
            }
            set
            {
                _ShowFlippedTileHelper = value;
                Editor.Instance.ReloadSpecificTextures(null, null);
            }
        }
        private bool _ShowFlippedTileHelper = false;

        #endregion

        #region Control Options

        public int MagnetSize
        {
            get
            {
                return _MagnetSize;
            }
            set
            {
                bool isCustom = false;
                Editor.Instance.EditorToolbar.x8ToolStripMenuItem.IsChecked = false;
                Editor.Instance.EditorToolbar.x16ToolStripMenuItem1.IsChecked = false;
                Editor.Instance.EditorToolbar.x32ToolStripMenuItem.IsChecked = false;
                Editor.Instance.EditorToolbar.x64ToolStripMenuItem.IsChecked = false;
                Editor.Instance.EditorToolbar.MagnetCustomSizeToolStripMenuItem.IsChecked = false;

                if (value == 8) Editor.Instance.EditorToolbar.x8ToolStripMenuItem.IsChecked = true;
                else if (value == 16) Editor.Instance.EditorToolbar.x16ToolStripMenuItem1.IsChecked = true;
                else if (value == 32) Editor.Instance.EditorToolbar.x32ToolStripMenuItem.IsChecked = true;
                else if (value == 64) Editor.Instance.EditorToolbar.x64ToolStripMenuItem.IsChecked = true;
                else if (value == -1)
                {
                    isCustom = true;
                    Editor.Instance.EditorToolbar.MagnetCustomSizeToolStripMenuItem.IsChecked = true;
                }

                Editor.Instance.EditorToolbar.CustomMagnetLabel.Text = string.Format(Editor.Instance.EditorToolbar.CustomMagnetLabel.Tag.ToString(), CustomMagnetSize);

                if (!isCustom) _MagnetSize = value;
                else _MagnetSize = CustomMagnetSize;
            }
        }
        private int _MagnetSize = 16;
        public int CustomMagnetSize = 16;

        public bool CopyAir
        {
            get
            {
                return _CopyAir;
            }
            set
            {
                _CopyAir = value;
                Editor.Instance.EditorMenuBar.copyAirToggle.IsChecked = value;
            }
        }
        private bool _CopyAir = false;

        public bool RightClicktoSwapSlotID
        {
            get
            {
                return _RightClicktoSwapSlotID;
            }
            set
            {
                Editor.Instance.Options._RightClicktoSwapSlotID = value;
                Editor.Instance.EditorMenuBar.rightClicktoSwapSlotIDs.IsChecked = value;
            }
        }
        private bool _RightClicktoSwapSlotID = false;

        public int FasterNudgeAmount
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
        private int _FasterNudgeAmount = 5;

        public bool UseMagnetMode
        {
            get { return _UseMagnetMode; }
            set
            {
                _UseMagnetMode = value;
                Editor.Instance.EditorToolbar.MagnetMode.IsChecked = value;
            }
        }
        private bool _UseMagnetMode = false;

        public bool UseMagnetXAxis
        {
            get { return _UseMagnetXAxis; }
            set
            {
                _UseMagnetXAxis = value;
                Editor.Instance.EditorToolbar.enableXAxisToolStripMenuItem.IsChecked = value;
            }
        }
        private bool _UseMagnetXAxis = true;

        public bool UseMagnetYAxis
        {
            get { return _UseMagnetYAxis; }
            set
            {
                _UseMagnetYAxis = value;
                Editor.Instance.EditorToolbar.enableYAxisToolStripMenuItem.IsChecked = value;
            }
        }
        private bool _UseMagnetYAxis = true;

        public bool EnableFasterNudge
        {
            get
            {
                return _EnableFasterNudge;
            }
            set
            {
                Editor.Instance.EditorStatusBar.nudgeFasterButton.IsChecked = value;
                Editor.Instance.EditorMenuBar.nudgeSelectionFasterToolStripMenuItem.IsChecked = value;
                _EnableFasterNudge = value;
            }
        }
        private bool _EnableFasterNudge = false;

        public bool ScrollLocked
        {
            get
            {
                return _ScrollLocked;
            }
            set
            {
                _ScrollLocked = value;
                Editor.Instance.EditorStatusBar.scrollLockButton.IsChecked = value;
                Editor.Instance.EditorMenuBar.statusNAToolStripMenuItem.IsChecked = value;
            }
        }
        private bool _ScrollLocked = true;

        #endregion

        #region Information Settings

        //TODO : May be Redundant
        public bool QuitWithoutSavingWarningRequired { get; set; } = false;
        public bool ShowingDataDirectory
        {
            get
            {
                return _ShowingDataDirectory;
            }
            set
            {
                {
                    _ShowingDataDirectory = value;
                }
            }

        }
        private bool _ShowingDataDirectory = false;

        public bool CountTilesSelectedInPixels
        {
            get
            {
                return _CountTilesSelectedInPixels;
            }
            set
            {
                _CountTilesSelectedInPixels = value;
                Editor.Instance.EditorStatusBar.pixelModeButton.IsChecked = value;
                Editor.Instance.EditorMenuBar.pixelModeToolStripMenuItem.IsChecked = value;
            }
        }
        private bool _CountTilesSelectedInPixels = false;


        #endregion

        #region Prefrence/Status Values

        public int SelectedTileID { get; set; } = -1; //For Tile Maniac Intergration via Right Click in Editor View Panel
        public string CurrentLanguage { get; set; } = "EN"; //Current Selected Language
        public int ScrollDirection { get; set; } = 1;
        public int PlayerBeingTracked { get; set; } = -1;
        public int CurrentControllerButtons { get; set; } = 2; //For Setting the Menu Control Button Images.
        public int LevelID { get; set; } = -1; //Self Explanatory
        public int LastQuickButtonState { get; set; } = 0; //Gets the Last Quick Button State, so we can tell what action was used last

        #endregion



        #region Unoptimized Technical Options
        public bool RemoveStageConfigEntriesAllowed { get => GetRemoveStageConfigEntriesAllowed(); set => SetRemoveStageConfigEntriesAllowed(value); }
        private bool _RemoveStageConfigEntriesAllowed = true;
        private bool GetRemoveStageConfigEntriesAllowed()
        {
            return _RemoveStageConfigEntriesAllowed;
        }
        private void SetRemoveStageConfigEntriesAllowed(bool value)
        {
            Editor.Instance.Options._RemoveStageConfigEntriesAllowed = value;
        }
        public bool IsConsoleWindowOpen { get => GetIsConsoleWindowOpen(); set => SetIsConsoleWindowOpen(value); }
        private bool _IsConsoleWindowOpen = false;
        private bool GetIsConsoleWindowOpen()
        {
            return _IsConsoleWindowOpen;
        }
        private void SetIsConsoleWindowOpen(bool value)
        {
            Editor.Instance.Options._IsConsoleWindowOpen = value;
        }
        
        
        public bool DataDirectoryReadOnlyMode { get => GetDataDirectoryReadOnlyMode(); set => SetDataDirectoryReadOnlyMode(value); }
        private bool _DataDirectoryReadOnlyMode = false;
        private bool GetDataDirectoryReadOnlyMode()
        {
            return _DataDirectoryReadOnlyMode;
        }
        private void SetDataDirectoryReadOnlyMode(bool value)
        {
            Editor.Instance.Options._DataDirectoryReadOnlyMode = value;
        }

        #endregion

        #region Unoptimized Tool Modes
        public void PointerMode(bool? value = null)
        {
            if (value != null) SetToolModes(0, value.Value);
            else SetToolModes(0, Editor.Instance.EditorToolbar.PointerToolButton.IsChecked.Value);
        }
        public void SelectionMode(bool? value = null)
        {
            if (value != null) SetToolModes(1, value.Value);
            else SetToolModes(1, Editor.Instance.EditorToolbar.SelectToolButton.IsChecked.Value);
        }
        public void DrawMode(bool? value = null)
        {
            if (value != null) SetToolModes(2, value.Value);
            else SetToolModes(2, Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value);
        }
        public void InteractionMode(bool? value = null)
        {
            SetToolModes(3, Editor.Instance.EditorToolbar.InteractionToolButton.IsChecked.Value);
        }

                //Determines the Draw Tool's Brush Size (Tiles Only)
        public int DrawBrushSize { get => GetDrawBrushSize(); set => SetDrawBrushSize(value); }
        private int _DrawBrushSize = 1;

        private int GetDrawBrushSize()
        {
            return _DrawBrushSize;
        }
        private void SetDrawBrushSize(int value)
        {
            _DrawBrushSize = value;
        }

        

        public void SplineMode(bool? value = null)
        {
            if (value != null) SetToolModes(4, value.Value);
            else SetToolModes(4, Editor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value);
        }
        public void ChunksMode()
        {
            if (Editor.Instance.IsTilesEdit()) Editor.Instance.EditorToolbar.ChunksToolButton.IsChecked ^= true;
            Editor.Instance.UI.UpdateControls();
        }
        public void SetToolModes(int selectedID, bool value)
        {
            Editor.Instance.EditorToolbar.PointerToolButton.IsChecked = (selectedID == 0 ? value : false);
            Editor.Instance.EditorToolbar.SelectToolButton.IsChecked = (selectedID == 1 ? value : false);
            Editor.Instance.EditorToolbar.DrawToolButton.IsChecked = (selectedID == 2 ? value : false);
            Editor.Instance.EditorToolbar.InteractionToolButton.IsChecked = (selectedID == 3 ? value : false);
            Editor.Instance.EditorToolbar.SplineToolButton.IsChecked = (selectedID == 4 ? value : false);
            Editor.Instance.UI.UpdateControls();
        }
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
                if (Editor.Instance.UI != null) Editor.Instance.UI.UpdateControls();
            }


            
            public bool SplineToolShowLines { get; set; } = true; //Self Explanatory
            public int SplineCurrentPointsDrawn { get; set; } = 0; //Self Explanatory
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
                        Editor.Instance.EditorToolbar.SplineLineMode.IsChecked = true;
                        _SplineLineMode = true;
                        Editor.Instance.EditorToolbar.SplineOvalMode.IsChecked = false;
                        _SplineOvalMode = false;
                    }
                    else
                    {
                        Editor.Instance.EditorToolbar.SplineLineMode.IsChecked = false;
                        _SplineLineMode = false;
                        Editor.Instance.EditorToolbar.SplineOvalMode.IsChecked = true;
                        _SplineOvalMode = true;
                    }
                }
                else
                {
                    if (state)
                    {
                        Editor.Instance.EditorToolbar.SplineLineMode.IsChecked = false;
                        _SplineLineMode = false;
                        Editor.Instance.EditorToolbar.SplineOvalMode.IsChecked = true;
                        _SplineOvalMode = true;
                    }
                    else
                    {
                        Editor.Instance.EditorToolbar.SplineLineMode.IsChecked = true;
                        _SplineLineMode = true;
                        Editor.Instance.EditorToolbar.SplineOvalMode.IsChecked = false;
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

        #region Unoptimized Multi Layer Edit Mode
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
            Editor.Instance.Options._MultiLayerEditMode = value;
            Editor.Instance.EditorMenuBar.multiLayerSelectionToolStripMenuItem.IsChecked = value;


            bool enabled = (value == true ? true : false);
            Editor.Instance.EditorToolbar.EditFGLower.DualSelect = enabled;
            Editor.Instance.EditorToolbar.EditFGLow.DualSelect = enabled;
            Editor.Instance.EditorToolbar.EditFGHigh.DualSelect = enabled;
            Editor.Instance.EditorToolbar.EditFGHigher.DualSelect = enabled;

            Editor.Instance.EditorToolbar.EditFGLower.SwapDefaultToA(!enabled);
            Editor.Instance.EditorToolbar.EditFGLow.SwapDefaultToA(!enabled);
            Editor.Instance.EditorToolbar.EditFGHigh.SwapDefaultToA(!enabled);
            Editor.Instance.EditorToolbar.EditFGHigher.SwapDefaultToA(!enabled);

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
            Editor.Instance.EditorToolbar.EditFGLower.DualSelect = enabled;
            Editor.Instance.EditorToolbar.EditFGLow.DualSelect = enabled;
            Editor.Instance.EditorToolbar.EditFGHigh.DualSelect = enabled;
            Editor.Instance.EditorToolbar.EditFGHigher.DualSelect = enabled;

            Editor.Instance.EditorToolbar.EditFGLower.SwapDefaultToA(!enabled);
            Editor.Instance.EditorToolbar.EditFGLow.SwapDefaultToA(!enabled);
            Editor.Instance.EditorToolbar.EditFGHigh.SwapDefaultToA(!enabled);
            Editor.Instance.EditorToolbar.EditFGHigher.SwapDefaultToA(!enabled);

            foreach (var elb in Editor.Instance.ExtraLayerEditViewButtons.Values)
            {
                elb.DualSelect = enabled;
                elb.SwapDefaultToA(!enabled);
            }

            if (!enabled) Editor.Instance.EditLayerB = null;

            if (updateControls) Editor.Instance.UI.UpdateControls();
        }

        #endregion

        #region To-Remove Settings
        public bool UseLargeDebugStats { get => GetUseLargeDebugStats(); set => SetUseLargeDebugStats(value); }
        private bool _UseLargeDebugStats = false;
        private bool GetUseLargeDebugStats()
        {
            return _UseLargeDebugStats;
        }
        private void SetUseLargeDebugStats(bool value)
        {
            _UseLargeDebugStats = value;
            Editor.Instance.EditorMenuBar.useLargeTextToolStripMenuItem.IsChecked = value;
        }
        
        public bool DebugStatsVisibleOnPanel { get => GetDebugStatsVisibleOnPanel(); set => SetDebugStatsVisibleOnPanel(value); }
        private bool _DebugStatsVisibleOnPanel = false;
        private bool GetDebugStatsVisibleOnPanel()
        {
            return _DebugStatsVisibleOnPanel;
        }
        private void SetDebugStatsVisibleOnPanel(bool value)
        {
            _DebugStatsVisibleOnPanel = value;
            Editor.Instance.EditorMenuBar.showStatsToolStripMenuItem.IsChecked = value;
        }
        #endregion

        #region Unoptimized Grid Size
        //Determines the Grid's Size
        public int GridSize { get => GetGridSize(); set => SetGridSize(value); }
        private int _GridSize = 16;
        public int GridCustomSize { get => GetCustomSize(); set => ChangeCustomSize(value); }
        private int _GridCustomSize = Properties.Defaults.Default.CustomGridSizeValue;

        private void ChangeCustomSize(int value)
        {
            _GridCustomSize = value;
            Editor.Instance.EditorToolbar.CustomGridLabel.Text = string.Format(Editor.Instance.EditorToolbar.CustomGridLabel.Tag.ToString(), _GridCustomSize);
        }

        private int GetCustomSize()
        {
            Editor.Instance.EditorToolbar.CustomGridLabel.Text = string.Format(Editor.Instance.EditorToolbar.CustomGridLabel.Tag.ToString(), _GridCustomSize);
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

            Editor.Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = false;
            Editor.Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = false;
            Editor.Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = false;
            Editor.Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = false;

            if (value == 16) Editor.Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = true;
            else if (value == 128) Editor.Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = true;
            else if (value == 256) Editor.Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = true;
            else if (value == -1)
            {
                isCustom = true;
                Editor.Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = true;
            }

            Editor.Instance.EditorToolbar.CustomGridLabel.Text = string.Format(Editor.Instance.EditorToolbar.CustomGridLabel.Tag.ToString(), GridCustomSize);

            if (!isCustom) _GridSize = value;
            else _GridSize = GridCustomSize;
        }
        #endregion

        #region Unoptimized Collision View Mode (Colors)
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

            Editor.Instance.EditorToolbar.invertedToolStripMenuItem.IsChecked = false;
            Editor.Instance.EditorToolbar.customToolStripMenuItem1.IsChecked = false;
            Editor.Instance.EditorToolbar.defaultToolStripMenuItem.IsChecked = false;

            if (value == 0) Editor.Instance.EditorToolbar.defaultToolStripMenuItem.IsChecked = true;
            else if(value == 1) Editor.Instance.EditorToolbar.invertedToolStripMenuItem.IsChecked = true;
            else if (value == 2) Editor.Instance.EditorToolbar.customToolStripMenuItem1.IsChecked = true;


            Editor.Instance.ReloadSpecificTextures(null, null);
            Editor.Instance.RefreshCollisionColours(true);
        }
        

        
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

        

        
        public System.Drawing.Color waterColor { get => GetWaterEntityColor(); set => SetWaterEntityColor(value); }
        private System.Drawing.Color _WaterEntityColor = System.Drawing.Color.Blue;
        private System.Drawing.Color GetWaterEntityColor() { return _WaterEntityColor; }
        private void SetWaterEntityColor(System.Drawing.Color value) { _WaterEntityColor = value; }

        public System.Drawing.Color GridColor { get => GetGridColor(); set => SetGridColor(value); }
        private System.Drawing.Color _GridColor = System.Drawing.Color.Red;
        private System.Drawing.Color GetGridColor() { return _GridColor; }
        private void SetGridColor(System.Drawing.Color value) { _GridColor = value; }
        #endregion

        #region To-Improve Implementation Variables
        public bool AddStageConfigEntriesAllowed { get; set; } = true; //Self Explanatory
        public bool isImportingObjects { get; set; } = false; //Determines if we are importing objects so we can disable all the other Scene Select Options
        public bool EncorePaletteExists { get; set; } = false; // Determines if an Encore Pallete Exists

        #endregion


        public bool Duplicate1
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
        private bool _Duplicate1;



        #region Unoptimized Misc Stuff
        public bool collisionOpacityChanged { get; set; } = false;
        public int EncoreSetupType { get; set; } //Used to determine what kind of encore setup the stage uses
        public string entitiesTextFilter { get; set; } = ""; //Used to hide objects that don't match the discription
        public string LevelSelectCharS { get; set; } = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ*+,-./: \'\"";
        public string MenuCharS { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ?!.";
        public string MenuCharS_Small { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ?.:'\"!-,&¡<>¿"; //49 out of 121
        public char[] MenuChar { get; set; }
        public char[] LevelSelectChar { get; set; }
        public char[] MenuChar_Small { get; set; }
        #endregion

        public UserStateModel()
		{

		}


    }
}
