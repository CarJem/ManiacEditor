using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor
{
	public class EditorUITools
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
            Editor.UITools._ShowGrid = value;
            Editor.UIEvents.GridCheckStateCheck();
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
            Editor.UITools._UseEncoreColors = value;
            Editor.EditorTiles.StageTiles?.Image.Reload((value == true ? Editor.EncorePalette[0] : null));
            Editor.TilesToolbar?.Reload((value == true ? Editor.EncorePalette[0] : null));
            Editor.EditorEntity_ini.ReleaseResources();
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
            Editor.UITools._ShowCollisionA = value;
            Editor.ShowCollisionBButton.IsChecked = false;
            Editor.UITools._ShowCollisionB = false;
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
            Editor.UITools._ShowCollisionA = false;
            Editor.ShowCollisionBButton.IsChecked = value;
            Editor.UITools._ShowCollisionB = value;
            Editor.ReloadSpecificTextures(null, null);
        }
        #endregion
        public int BackupType = 0; //Determines What Kind of Backup to Make
        public bool UseMagnetMode = false; // Determines the state of Magnet Mode
        public bool UseMagnetXAxis = true; //Determines if the Magnet should use the X Axis
        public bool UseMagnetYAxis = true; //Determines if the Magnet should use the Y Axis
        public bool ShowEntityPathArrows = true; //Determines if we want to see Object Arrow Paths
        public bool ShowWaterLevel = false; //Determines if the Water Object should show it's Water Level.
        public bool AlwaysShowWaterLevel = false; //Determines if the Water Level Should be Shown at all times regardless of the object being selected
        public bool SizeWaterLevelwithBounds = false; //Determines if the water level width should match those of the object's bounds
        public bool ExtraLayersMoveToFront = false; //Determines if we should render the extra layers in front of everything on behind everything
        public bool ShowFlippedTileHelper = false; //Determines if we should Show Flip Assist or Not
        public bool ShowingDataDirectory = false; //Determines who's turn it is when swaping the label's entry to display ethier the Data Directory and Mod Folder.
        public bool ShowParallaxSprites = false; //Determines if we should show the parallax sprites
        public bool ApplyEditEntitiesTransparency = false; //Determines if the other layers should be semi-transparent when editing entities.
        public bool ShowEntitySelectionBoxes = false; //Determines if we should show the entity selection boxes.
        public bool EnablePixelCountMode = false; //Count the selection in pixels per tile or not
        public bool IsConsoleWindowOpen = false; //Show the Console Window or not
        public bool RightClicktoSwapSlotID = false; //Swap Entity Slot ID's with Right Click
        public bool EntitySelectionBoxesAlwaysPrioritized = true;

        public EditorUITools(Editor instance)
		{
			Editor = instance;
		}


    }
}
