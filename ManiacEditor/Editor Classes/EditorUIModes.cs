using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor
{
	public class EditorUIModes
    {
		private Editor Editor;
        #region ShowTileID
        public bool ShowTileID { get => GetShowTileIDMode(); set => SetShowTileIDMode(value); } //Show Tile ID Status
        private bool _ShowTileID;
        private bool GetShowTileIDMode()
        {
            return _ShowTileID;
        }
        private void SetShowTileIDMode(bool value)
        {
            Editor.ShowTileIDButton.IsChecked = value;
            Editor.ReloadSpecificTextures(null, null);
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
            Editor.ShowGridButton.IsChecked = value;
            Editor.UIModes._ShowGrid = value;
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
            Editor.DisposeTextures();
            Editor.EncorePaletteButton.IsChecked = value;
            Editor.UIModes._UseEncoreColors = value;
            Editor.EditorTiles.StageTiles?.Image.Reload((value ? Editor.EncorePalette[0] : null));
            Editor.TilesToolbar?.Reload((value ? Editor.EncorePalette[0] : null));
            Editor.EntityDrawing.ReleaseResources();
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
            Editor.ShowCollisionAButton.IsChecked = value;
            Editor.UIModes._ShowCollisionA = value;
            Editor.ShowCollisionBButton.IsChecked = false;
            Editor.UIModes._ShowCollisionB = false;
            Editor.ReloadSpecificTextures(null, null);
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
            
            Editor.ShowCollisionAButton.IsChecked = false;
            Editor.UIModes._ShowCollisionA = false;
            Editor.ShowCollisionBButton.IsChecked = value;
            Editor.UIModes._ShowCollisionB = value;
            Editor.ReloadSpecificTextures(null, null);
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
            Editor.UIModes._UseMagnetMode = value;
            Editor.MagnetMode.IsChecked = value;
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
            Editor.UIModes._UseMagnetXAxis = value;
            Editor.enableXAxisToolStripMenuItem.IsChecked = value;
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
            Editor.UIModes._UseMagnetYAxis = value;
            Editor.enableYAxisToolStripMenuItem.IsChecked = value;
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
            Editor.UIModes._ShowEntityPathArrows = value;
            Editor.showEntityPathArrowsToolstripItem.IsChecked = value;
            Editor.showEntityPathArrowsToolstripItem2.IsChecked = value;
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
            Editor.UIModes._ShowWaterLevel = value;
            Editor.showWaterLevelToolStripMenuItem.IsChecked = value;
            Editor.showWaterLevelToolStripMenuItem2.IsChecked = value;
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
            Editor.UIModes._AlwaysShowWaterLevel = value;
            Editor.waterLevelAlwaysShowItem.IsChecked = value;
            Editor.waterLevelAlwaysShowItem2.IsChecked = value;
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
            Editor.UIModes._SizeWaterLevelwithBounds = value;
            Editor.sizeWithBoundsWhenNotSelectedToolStripMenuItem.IsChecked = value;
            Editor.sizeWithBoundsWhenNotSelectedToolStripMenuItem2.IsChecked = value;
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
            Editor.UIModes._ExtraLayersMoveToFront = value;
            Editor.moveExtraLayersToFrontToolStripMenuItem.IsChecked = true;
            Editor.moveExtraLayersToFrontToolStripMenuItem2.IsChecked = true;
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
            Editor.UIModes._ShowFlippedTileHelper = value;
            Editor.ReloadSpecificTextures(null, null);
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
            Editor.UIModes._ShowingDataDirectory = value;
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
            Editor.UIModes._ShowParallaxSprites = value;
            Editor.showParallaxSpritesToolStripMenuItem.IsChecked = value;
            Editor.showParallaxSpritesToolStripMenuItem2.IsChecked = value;
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
            Editor.UIModes._ApplyEditEntitiesTransparency = value;
            Editor.EditEntitiesTransparencyToggle.IsChecked = value;
            Editor.EditEntitiesTransparencyToggle2.IsChecked = value;
            Editor.QuickEditEntitiesTransparentLayers.IsChecked = value;
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
            Editor.UIModes._ShowEntitySelectionBoxes = value;
            Editor.showEntitySelectionBoxesToolStripMenuItem.IsChecked = value;
            Editor.showEntitySelectionBoxesToolStripMenuItem2.IsChecked = value;
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
            Editor.UIModes._EnablePixelCountMode = value;
            Editor.pixelModeButton.IsChecked = value;
            Editor.pixelModeToolStripMenuItem.IsChecked = value;
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
            Editor.UIModes._IsConsoleWindowOpen = value;
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
            Editor.UIModes._RightClicktoSwapSlotID = value;
            Editor.rightClicktoSwapSlotIDs.IsChecked = value;
            Editor.rightClicktoSwapSlotIDs2.IsChecked = value;
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
            Editor.UIModes._EntitySelectionBoxesAlwaysPrioritized = value;
            Editor.SelectionBoxesAlwaysPrioritized.IsChecked = value;
            Editor.SelectionBoxesAlwaysPrioritized2.IsChecked = value;
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
            Editor.UIModes._DataDirectoryReadOnlyMode = value;
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
            Editor.UIModes._ScrollLocked = value;
            Editor.scrollLockButton.IsChecked = value;
            Editor.statusNAToolStripMenuItem.IsChecked = value;
        }
        #endregion
        #region EnableFasterNudge
        public bool EnableFasterNudge { get => GetEnableFasterNudge(); set => SetEnableFasterNudge(value); }
        private bool _EnableFasterNudge = false;
        private bool GetEnableFasterNudge()
        {
            return _EnableFasterNudge;
        }
        private void SetEnableFasterNudge(bool value)
        {
            Editor.nudgeFasterButton.IsChecked = value;
            Editor.nudgeSelectionFasterToolStripMenuItem.IsChecked = value;
            _EnableFasterNudge = value;
        }
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
            Editor.UIModes._MovingPlatformsChecked = value;
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
            Editor.UIModes._AnnimationsChecked = value;
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
            Editor.UIModes._RemoveStageConfigEntriesAllowed = value;
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
            Editor.copyAirToggle.IsChecked = value;
            Editor.copyAirToggle2.IsChecked = value;
        }
        #endregion
        #region Tools/Brushes
        public void PointerMode()
        {
            SetToolModes(0, Editor.PointerToolButton.IsChecked.Value);
        }
        public void SelectionMode()
        {
            SetToolModes(1, Editor.SelectToolButton.IsChecked.Value);
        }
        public void DrawMode()
        {
            SetToolModes(2, Editor.PlaceTilesButton.IsChecked.Value);
        }
        public void InteractionMode()
        {
            SetToolModes(3, Editor.InteractionToolButton.IsChecked.Value);
        }
        public void ChunksMode()
        {
            Editor.UI.UpdateControls();
        }
        public void SetToolModes(int selectedID, bool value)
        {
            Editor.PointerToolButton.IsChecked = (selectedID == 0 ? value : false);
            Editor.SelectToolButton.IsChecked = (selectedID == 1 ? value : false);
            Editor.PlaceTilesButton.IsChecked = (selectedID == 2 ? value : false);
            Editor.InteractionToolButton.IsChecked = (selectedID == 3 ? value : false);
            Editor.UI.UpdateControls();
        }
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
            Editor.UIModes._MultiLayerEditMode = value;
            Editor.multiLayerSelectionToolStripMenuItem.IsChecked = value;
            Editor.multiLayerSelectionToolStripMenuItem2.IsChecked = value;


            bool enabled = (value == true ? true : false);
            Editor.EditFGLower.DualSelect = enabled;
            Editor.EditFGLow.DualSelect = enabled;
            Editor.EditFGHigh.DualSelect = enabled;
            Editor.EditFGHigher.DualSelect = enabled;

            Editor.EditFGLower.SwapDefaultToA(!enabled);
            Editor.EditFGLow.SwapDefaultToA(!enabled);
            Editor.EditFGHigh.SwapDefaultToA(!enabled);
            Editor.EditFGHigher.SwapDefaultToA(!enabled);

            foreach (var elb in Editor.ExtraLayerEditViewButtons.Values)
            {
                elb.DualSelect = enabled;
                elb.SwapDefaultToA(!enabled);
            }

            if (!enabled) Editor.EditLayerB = null;

            Editor.UI.UpdateControls();
        }

        public void UpdateMultiLayerSelectMode(bool updateControls = false)
        {
            bool enabled = (_MultiLayerEditMode == true ? true : false);
            Editor.EditFGLower.DualSelect = enabled;
            Editor.EditFGLow.DualSelect = enabled;
            Editor.EditFGHigh.DualSelect = enabled;
            Editor.EditFGHigher.DualSelect = enabled;

            Editor.EditFGLower.SwapDefaultToA(!enabled);
            Editor.EditFGLow.SwapDefaultToA(!enabled);
            Editor.EditFGHigh.SwapDefaultToA(!enabled);
            Editor.EditFGHigher.SwapDefaultToA(!enabled);

            foreach (var elb in Editor.ExtraLayerEditViewButtons.Values)
            {
                elb.DualSelect = enabled;
                elb.SwapDefaultToA(!enabled);
            }

            if (!enabled) Editor.EditLayerB = null;

            if (updateControls) Editor.UI.UpdateControls();
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
            Editor.useLargeTextToolStripMenuItem.IsChecked = value;
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
            Editor.showStatsToolStripMenuItem.IsChecked = value;
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
            Editor.prioritizedViewingToolStripMenuItem.IsChecked = value;
            Editor.prioritizedViewingToolStripMenuItem2.IsChecked = value;
        }
        #endregion
        #region Magnet Mode Size 
        //Determines the Magnets Size
        public int MagnetSize { get => GetMagnetSize(); set => SetMagnetSize(value); }
        private int _MagnetSize = 16;
        private int GetMagnetSize()
        {
            return _MagnetSize;
        }
        private void SetMagnetSize(int value)
        {
            Editor.x8ToolStripMenuItem.IsChecked = false;
            Editor.x16ToolStripMenuItem1.IsChecked = false;
            Editor.x32ToolStripMenuItem.IsChecked = false;
            Editor.x64ToolStripMenuItem.IsChecked = false;

            if (value == 8) Editor.x8ToolStripMenuItem.IsChecked = true;
            else if (value == 16) Editor.x16ToolStripMenuItem1.IsChecked = true;
            else if (value == 32) Editor.x32ToolStripMenuItem.IsChecked = true;
            else if (value == 64) Editor.x64ToolStripMenuItem.IsChecked = true;
        }
        #endregion
        #region Grid Size 
        //Determines the Magnets Size
        public int GridSize { get => GetGridSize(); set => SetGridSize(value); }
        private int _GridSize = 16;
        private int GetGridSize()
        {
            return _GridSize;
        }
        private void SetGridSize(int value)
        {
            _GridSize = value;

            Editor.Grid16x16SizeMenuItem.IsChecked = false;
            Editor.Grid128x128SizeMenuItem.IsChecked = false;
            Editor.Grid256x256SizeMenuItem.IsChecked = false;
            Editor.GridCustomSizeMenuItem.IsChecked = false;

            if (value == 16) Editor.Grid16x16SizeMenuItem.IsChecked = true;
            else if (value == 128) Editor.Grid128x128SizeMenuItem.IsChecked = true;
            else if (value == 256) Editor.Grid256x256SizeMenuItem.IsChecked = true;
            else Editor.GridCustomSizeMenuItem.IsChecked = true;
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
            Editor.UIModes._EntitiesVisibileAboveAllLayers = value;
            Editor.SelectionBoxesAlwaysPrioritized.IsChecked = value;
            Editor.SelectionBoxesAlwaysPrioritized2.IsChecked = value;
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

            Editor.invertedToolStripMenuItem.IsChecked = false;
            Editor.customToolStripMenuItem1.IsChecked = false;
            Editor.defaultToolStripMenuItem.IsChecked = false;

            if (value == 0) Editor.invertedToolStripMenuItem.IsChecked = true;
            else if (value == 1) Editor.customToolStripMenuItem1.IsChecked = true;
            else if (value == 2) Editor.defaultToolStripMenuItem.IsChecked = true;

            Editor.ReloadSpecificTextures(null, null);
        }
        #endregion
        public bool AddStageConfigEntriesAllowed { get; set; } = true; //Self Explanatory
        public bool isImportingObjects { get; set; } = false; //Determines if we are importing objects so we can disable all the other Scene Select Options
        public bool EncorePaletteExists { get; set; } = false; // Determines if an Encore Pallete Exists
        public bool ForceWarp { get; set; } = false; //For Shortcuts and Force Open.
        public bool ShortcutHasZoom { get; set; } = false; //For Shortcuts and Force Open.
        public bool collisionOpacityChanged { get; set; } = false;

        public static bool UpdateUpdaterMessage = false;





        public System.Drawing.Point TempWarpCoords = new System.Drawing.Point(0, 0); //Temporary Warp Position for Shortcuts and Force Open

        public int ScrollDirection { get; set; } = 1;


        public int EncoreSetupType { get; set; } //Used to determine what kind of encore setup the stage uses
        public int selectPlayerObject_GoTo { get; set; } = 0; //Used to determine which player object to go to
        public int PlayerBeingTracked { get; set; } = -1;
        public int CurrentControllerButtons { get; set; } = 2; //For Setting the Menu Control Button Images.
        public int LevelID { get; set; } = -1; //Self Explanatory
        public int LastQuickButtonState { get; set; } = 0; //Gets the Last Quick Button State, so we can tell what action was used last
        public int SelectedTileID { get; set; } = -1; //For Tile Maniac Intergration via Right Click in Editor View Panel

        public System.Drawing.Color waterColor = new System.Drawing.Color(); // The color used for the Water Entity
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



        public EditorUIModes(Editor instance)
		{
			Editor = instance;
		}


    }
}
