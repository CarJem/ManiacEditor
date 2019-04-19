using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ManiacEditor
{
    public class EditorUIControl
    {


        [DllImport("User32.dll")]
		private static extern bool SetCursorPos(int X, int Y);

		#region Editor Definitions

		public int previousX = 0;
		public int previousY = 0;
		private int select_x1 { get => Editor.Instance.StateModel.select_x1; set => Editor.Instance.StateModel.select_x1 = value; }
		private int select_x2 { get => Editor.Instance.StateModel.select_x2; set => Editor.Instance.StateModel.select_x2 = value; }
		private int select_y1 { get => Editor.Instance.StateModel.select_y1; set => Editor.Instance.StateModel.select_y1 = value; }
		private int select_y2 { get => Editor.Instance.StateModel.select_y2; set => Editor.Instance.StateModel.select_y2 = value; }

		private int selectingY { get => Editor.Instance.StateModel.SelectionY2; set => Editor.Instance.StateModel.SelectionY2 = value; }
		private int selectingX { get => Editor.Instance.StateModel.SelectionX2; set => Editor.Instance.StateModel.SelectionX2 = value; }
		private int ClickedY { get => Editor.Instance.StateModel.SelectionY1; set => Editor.Instance.StateModel.SelectionY1 = value; }
		private int ClickedX { get => Editor.Instance.StateModel.SelectionX1; set => Editor.Instance.StateModel.SelectionX1 = value; }

		private int ShiftY { get => Editor.Instance.StateModel.ShiftY; set => Editor.Instance.StateModel.ShiftY = value; }
		private int ShiftX { get => Editor.Instance.StateModel.ShiftX; set => Editor.Instance.StateModel.ShiftX = value; }

		private int draggedY { get => Editor.Instance.StateModel.draggedY; set => Editor.Instance.StateModel.draggedY = value; }
		private int draggedX { get => Editor.Instance.StateModel.draggedX; set => Editor.Instance.StateModel.draggedX = value; }

		private int lastY { get => Editor.Instance.StateModel.lastY; set => Editor.Instance.StateModel.lastY = value; }
		private int lastX { get => Editor.Instance.StateModel.lastX; set => Editor.Instance.StateModel.lastX = value; }

		private double Zoom { get => Editor.Instance.StateModel.Zoom; set => Editor.Instance.StateModel.Zoom = value; }
		private int ZoomLevel { get => Editor.Instance.StateModel.ZoomLevel; set => Editor.Instance.StateModel.ZoomLevel = value; }

		private bool IsChunksEdit() { return Editor.Instance.IsChunksEdit(); }
		private bool IsTilesEdit() { return Editor.Instance.IsTilesEdit(); }
		private bool IsEntitiesEdit() { return Editor.Instance.IsEntitiesEdit();}
		private bool IsEditing() { return Editor.Instance.IsEditing(); }
		private bool IsSceneLoaded() { return Editor.Instance.IsSceneLoaded(); }

		private bool scrollingDragged { get => Editor.Instance.StateModel.scrollingDragged; set => Editor.Instance.StateModel.scrollingDragged = value; }
		private bool scrolling { get => Editor.Instance.StateModel.scrolling; set => Editor.Instance.StateModel.scrolling = value; }
		private bool dragged { get => Editor.Instance.StateModel.dragged; set => Editor.Instance.StateModel.dragged = value; }
		private bool startDragged { get => Editor.Instance.StateModel.startDragged; set => Editor.Instance.StateModel.startDragged = value; }
		private bool draggingSelection { get => Editor.Instance.StateModel.draggingSelection; set => Editor.Instance.StateModel.draggingSelection = value; }
		private bool GameRunning { get => Editor.Instance.InGame.GameRunning; set => Editor.Instance.InGame.GameRunning = value; }

		private int ScrollDirection { get => Editor.Instance.UIModes.ScrollDirection; }
		private bool ScrollLocked { get => Editor.Instance.UIModes.ScrollLocked; }

		private bool CtrlPressed() { return Editor.Instance.CtrlPressed(); }
		private bool ShiftPressed() { return Editor.Instance.ShiftPressed(); }
		private bool IsSelected() { return Editor.Instance.IsSelected(); }

		#endregion


		public EditorUIControl()
        {
			UpdateTooltips();
			UpdateMenuItems();
		}


        //Shorthanding Settings
        readonly Properties.Settings mySettings = Properties.Settings.Default;
        Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;

		#region Keyboard Inputs
		public void GraphicPanel_OnKeyUp(object sender, KeyEventArgs e)
		{
			// Tiles Toolbar Flip Horizontal
			if (isCombo(e, myKeyBinds.FlipHTiles, true))
			{
				if (IsTilesEdit() && Editor.Instance.DrawToolButton.IsChecked.Value)
					Editor.Instance.TilesToolbar.SetSelectTileOption(0, false);
			}
			// Tiles Toolbar Flip Vertical
			else if (isCombo(e, myKeyBinds.FlipVTiles, true))
			{
				if (IsTilesEdit() && Editor.Instance.DrawToolButton.IsChecked.Value)
					Editor.Instance.TilesToolbar.SetSelectTileOption(1, false);
			}
		}

		public void GraphicPanel_OnKeyDown(object sender, KeyEventArgs e)
        {
            bool parallaxAnimationInProgress = Editor.Instance.UIModes.AnimationsEnabled && Editor.Instance.UIModes.ParallaxAnimationChecked;
            if (parallaxAnimationInProgress) return;

            // Faster Nudge Toggle
            if (isCombo(e, myKeyBinds.NudgeFaster))
            {
                Editor.Instance.ToggleFasterNudgeEvent(sender, null);
            }
			// Scroll Lock Toggle
			else if (isCombo(e, myKeyBinds.ScrollLock))
            {
                Editor.Instance.UIModes.ScrollLocked ^= true;
            }
            // Switch Scroll Lock Type
            else if (isCombo(e, myKeyBinds.ScrollLockTypeSwitch))
            {
                Editor.Instance.UIEvents.SetScrollLockDirection();

            }
			// Tiles Toolbar Flip Vertical
			else if (isCombo(e, myKeyBinds.FlipVTiles, true))
			{
                if (IsTilesEdit() && Editor.Instance.DrawToolButton.IsChecked.Value)
                    Editor.Instance.TilesToolbar.SetSelectTileOption(1, true);
            }
			// Tiles Toolbar Flip Horizontal
			else if (isCombo(e, myKeyBinds.FlipHTiles, true))
			{
                if (IsTilesEdit() && Editor.Instance.DrawToolButton.IsChecked.Value)
                    Editor.Instance.TilesToolbar.SetSelectTileOption(0, true);
            }
            // Open Click (Alt: Open Data Dir)
            else if ((isCombo(e, myKeyBinds.OpenDataDir)))
            {
                Editor.Instance.OpenDataDirectoryEvent(null, null);
            }
            else if ((isCombo(e, myKeyBinds.Open)))
			{
                Editor.Instance.OpenSceneEvent(null, null);
            }
            // New Click
            else if (isCombo(e, myKeyBinds.New))
            {
                //Editor.Instance.New_Click(null, null);
            }
            // Save Click (Alt: Save As)
            else if (isCombo(e, myKeyBinds.SaveAs))
            {
                Editor.Instance.SaveSceneAsEvent(null, null);
            }
            else if (isCombo(e, myKeyBinds._Save))
            {
                Editor.Instance.SaveSceneEvent(null, null);
            }          
			// Undo
			else if (isCombo(e, myKeyBinds.Undo))
            {
                Editor.Instance.EditorUndo();
            }
            // Redo
            else if (isCombo(e, myKeyBinds.Redo))
            {
                Editor.Instance.EditorRedo();
            }
			// Developer Interface
			else if (isCombo(e, myKeyBinds.DeveloperInterface))
			{
				Editor.Instance.EditorUndo();
			}
			// Save for Force Open on Startup
			else if (isCombo(e, myKeyBinds.ForceOpenOnStartup))
			{
				Editor.Instance.EditorRedo();
			}
			else if (Editor.Instance.IsSceneLoaded())
			{
				GraphicPanel_OnKeyDownLoaded(sender, e);
			}
            // Editing Key Shortcuts
            if (IsEditing())
            {
                GraphicPanel_OnKeyDownEditing(sender, e);
            }
            OnKeyDownTools(sender, e);
        }

		public void GraphicPanel_OnKeyDownLoaded(object sender, KeyEventArgs e)
		{
            // Reset Zoom Level
            if (isCombo(e, myKeyBinds.ResetZoomLevel))
			{
				Editor.Instance.ZoomModel.SetZoomLevel(0, new Point(0, 0));
			}
			//Refresh Tiles and Sprites
			else if (isCombo(e, myKeyBinds.RefreshResources))
			{
				Editor.Instance.ReloadToolStripButton_Click(null, null);
			}
			//Run Scene
			else if (isCombo(e, myKeyBinds.RunScene))
			{
                Editor.Instance.InGame.RunScene();
			}
			//Show Path A
			else if (isCombo(e, myKeyBinds.ShowPathA) && Editor.Instance.IsSceneLoaded())
			{
				Editor.Instance.ShowCollisionAEvent(null, null);
			}
			//Show Path B
			else if (isCombo(e, myKeyBinds.ShowPathB))
			{
				Editor.Instance.ShowCollisionBEvent(null, null);
			}
			//Unload Scene
			else if (isCombo(e, myKeyBinds.UnloadScene))
			{
				Editor.Instance.UnloadSceneEvent(null, null);
			}
			//Toggle Grid Visibility
			else if (isCombo(e, myKeyBinds.ShowGrid))
			{
				Editor.Instance.ToggleGridEvent(null, null);
			}
			//Toggle Tile ID Visibility
			else if (isCombo(e, myKeyBinds.ShowTileID))
			{
				Editor.Instance.ToggleSlotIDEvent(null, null);
			}
			//Refresh Tiles and Sprites
			else if (isCombo(e, myKeyBinds.StatusBoxToggle))
			{
				Editor.Instance.ToggleDebugHUDEvent(null, null);
			}
		}

		public void GraphicPanel_OnKeyDownEditing(object sender, KeyEventArgs e)
        {
			//Paste
			if (isCombo(e, myKeyBinds.Paste))
			{
                Editor.Instance.PasteEvent(sender, null);
            }
			//Paste to Chunk
			if (isCombo(e, myKeyBinds.PasteToChunk))
			{
				Editor.Instance.PasteToChunksEvent(sender, null);
			}
			//Select All
			if (isCombo(e, myKeyBinds.SelectAll))
			{
                Editor.Instance.SelectAllEvent(sender, null);
            }
            // Selected Key Shortcuts   
            if (IsSelected())
            {
                GraphicPanel_OnKeyDownSelectedEditing(sender, e);
            }
        }

        public void GraphicPanel_OnKeyDownSelectedEditing(object sender, KeyEventArgs e)
        {
			// Delete
			if (isCombo(e, myKeyBinds.Delete))
            {
                Editor.Instance.DeleteSelected();
            }

            // Moving
            else if (e.KeyData == Keys.Up || e.KeyData == Keys.Down || e.KeyData == Keys.Left || e.KeyData == Keys.Right)
            {
                Editor.Instance.MoveEntityOrTiles(sender, e);
            }

			//Cut 
			if (isCombo(e, myKeyBinds.Cut))
			{
				Editor.Instance.CutEvent(sender, null);
			}
			//Copy
			else if (isCombo(e, myKeyBinds.Copy))
			{
				Editor.Instance.CopyEvent(sender, null);
			}
			//Duplicate
			else if (isCombo(e, myKeyBinds.Duplicate))
			{
				Editor.Instance.DuplicateEvent(sender, null);
			}
			// Flip Vertical Individual
			else if (isCombo(e, myKeyBinds.FlipVIndv))
			{
				if (IsTilesEdit())
					Editor.Instance.FlipVerticalIndividualEvent(sender, null);
			}
			// Flip Horizontal Individual
			else if (isCombo(e, myKeyBinds.FlipHIndv))
			{
				if (IsTilesEdit())
					Editor.Instance.FlipHorizontalIndividualEvent(sender, null);
			}
			// Flip Vertical
			else if (isCombo(e, myKeyBinds.FlipV))
			{
				if (IsTilesEdit())
					Editor.Instance.FlipVerticalEvent(sender, null);
				else if (IsEntitiesEdit())
					Editor.Instance.FlipEntities(FlipDirection.Veritcal);
			}

			// Flip Horizontal
			else if (isCombo(e, myKeyBinds.FlipH))
			{
				if (IsTilesEdit())
					Editor.Instance.FlipHorizontalEvent(sender, null);
				else if (IsEntitiesEdit())
					Editor.Instance.FlipEntities(FlipDirection.Horizontal);
			}
		}

        public void OnKeyDownTools(object sender, KeyEventArgs e)
        {
            if (isCombo(e, myKeyBinds.PointerTool) && Editor.Instance.PointerToolButton.IsEnabled) Editor.Instance.UIModes.PointerMode(true);
            else if (isCombo(e, myKeyBinds.SelectTool) && Editor.Instance.SelectToolButton.IsEnabled) Editor.Instance.UIModes.SelectionMode(true);
            else if (isCombo(e, myKeyBinds.DrawTool) && Editor.Instance.DrawToolButton.IsEnabled) Editor.Instance.UIModes.DrawMode(true);
            else if (isCombo(e, myKeyBinds.MagnetTool) && Editor.Instance.MagnetMode.IsEnabled) Editor.Instance.UIModes.UseMagnetMode ^= true;
            else if (isCombo(e, myKeyBinds.SplineTool) && Editor.Instance.SplineToolButton.IsEnabled) Editor.Instance.UIModes.SplineMode(true);
            else if (isCombo(e, myKeyBinds.StampTool) && Editor.Instance.ChunksToolButton.IsEnabled) Editor.Instance.UIModes.ChunksMode();

        }
        #endregion

        #region Keybind Checking and Prasing
        public bool isCombo(KeyEventArgs e, StringCollection keyCollection, bool singleKey = false)
		{

			if (keyCollection == null) return false;
			foreach (string key in keyCollection)
			{
				if (!singleKey)
				{
					if (isComboData(e, key))
					{
						return true;
					}
				}
				else
				{
					if (isComboCode(e, key))
					{
						return true;
					}
				}

			}
			return false;
		}

		public bool isComboData(KeyEventArgs e, string key)
		{
			try
			{
				if (key.Contains("Ctrl")) key = key.Replace("Ctrl", "Control");
				if (key.Contains("Del") && !key.Contains("Delete")) key = key.Replace("Del", "Delete");
				KeysConverter kc = new KeysConverter();

				if (e.KeyData == (Keys)kc.ConvertFromString(key)) return true;
				else return false;
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

		public bool isComboCode(KeyEventArgs e, string key)
		{
			try
			{
				if (key.Contains("Ctrl")) key = key.Replace("Ctrl", "Control");
				if (key.Contains("Del")) key = key.Replace("Del", "Delete");
				KeysConverter kc = new KeysConverter();

				if (e.KeyCode == (Keys)kc.ConvertFromString(key)) return true;
				else return false;
			}
			catch (Exception ex)
			{
				throw ex;
			}

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
		#endregion

        #region Tooltips + Menu Items

        public void UpdateMenuItems()
		{
			Editor.Instance.newToolStripMenuItem.InputGestureText = KeyBindPraser("New");
			Editor.Instance.openToolStripMenuItem.InputGestureText = KeyBindPraser("Open");
			Editor.Instance.openDataDirectoryToolStripMenuItem.InputGestureText = KeyBindPraser("OpenDataDir");
			Editor.Instance.saveToolStripMenuItem.InputGestureText = KeyBindPraser("_Save");
			Editor.Instance.saveAsToolStripMenuItem.InputGestureText = KeyBindPraser("SaveAs");
			Editor.Instance.undoToolStripMenuItem.InputGestureText = KeyBindPraser("Undo");
			Editor.Instance.redoToolStripMenuItem.InputGestureText = KeyBindPraser("Redo");
			Editor.Instance.cutToolStripMenuItem.InputGestureText = KeyBindPraser("Cut");
			Editor.Instance.copyToolStripMenuItem.InputGestureText = KeyBindPraser("Copy");
			Editor.Instance.pasteToolStripMenuItem.InputGestureText = KeyBindPraser("Paste");
			Editor.Instance.duplicateToolStripMenuItem.InputGestureText = KeyBindPraser("Duplicate");
			Editor.Instance.selectAllToolStripMenuItem.InputGestureText = KeyBindPraser("SelectAll");
			Editor.Instance.deleteToolStripMenuItem.InputGestureText = KeyBindPraser("Delete");
			Editor.Instance.statusNAToolStripMenuItem.InputGestureText = KeyBindPraser("ScrollLock");
			Editor.Instance.nudgeSelectionFasterToolStripMenuItem.InputGestureText = KeyBindPraser("NudgeFaster", false, true);
			Editor.Instance.QuickSwapScrollDirection.InputGestureText = KeyBindPraser("ScrollLockTypeSwitch", false, true);
			Editor.Instance.swapScrollLockDirMenuToolstripButton.InputGestureText = KeyBindPraser("ScrollLockTypeSwitch", false, true);
			Editor.Instance.resetZoomLevelToolstripMenuItem.InputGestureText = KeyBindPraser("ResetZoomLevel");
			Editor.Instance.unloadSceneToolStripMenuItem.InputGestureText = KeyBindPraser("UnloadScene", false, true);
			Editor.Instance.flipVerticalIndvidualToolStripMenuItem.InputGestureText = KeyBindPraser("FlipVIndv");
			Editor.Instance.flipHorizontalIndvidualToolStripMenuItem.InputGestureText = KeyBindPraser("FlipHIndv");
			Editor.Instance.flipHorizontalToolStripMenuItem.InputGestureText = KeyBindPraser("FlipH");
			Editor.Instance.flipVerticalToolStripMenuItem.InputGestureText = KeyBindPraser("FlipV");
			Editor.Instance.pasteTochunkToolStripMenuItem.InputGestureText = KeyBindPraser("PasteToChunk", false, true);
			Editor.Instance.developerInterfaceToolStripMenuItem.InputGestureText = KeyBindPraser("DeveloperInterface", false, true);
			Editor.Instance.saveForForceOpenOnStartupToolStripMenuItem.InputGestureText = KeyBindPraser("ForceOpenOnStartup", false, true);
            Editor.Instance.CopyAirLabel.Text = string.Format("Copy Air Tiles {1} ({0})", KeyBindPraser("CopyAirTiles", false, true), Environment.NewLine);
		}

		public void UpdateTooltips()
		{
			Editor.Instance.New.ToolTip = "New Scene" + KeyBindPraser("New", true);
			Editor.Instance.Open.ToolTip = "Open Scene" + KeyBindPraser("Open", true);
			Editor.Instance.Save.ToolTip = "Save Scene" + KeyBindPraser("_Save", true);
			Editor.Instance.RunSceneButton.ToolTip = "Run Scene" + KeyBindPraser("RunScene", true, true);
			Editor.Instance.ReloadButton.ToolTip = "Reload Tiles and Sprites" + KeyBindPraser("RefreshResources", true, true);
			Editor.Instance.PointerToolButton.ToolTip = "Pointer Tool" + KeyBindPraser("PointerTool", true);
            Editor.Instance.MagnetMode.ToolTip = "Magnet Mode" + KeyBindPraser("MagnetTool", true);
			Editor.Instance.positionLabel.ToolTip = "The position relative to your mouse (Pixels Only for Now)";
			Editor.Instance.selectionSizeLabel.ToolTip = "The Size of the Selection";
			Editor.Instance.selectedPositionLabel.ToolTip = "The Position of the Selected Tile";
			Editor.Instance.selectionBoxSizeLabel.ToolTip = "The Size of the Selection Box";
			Editor.Instance.pixelModeButton.ToolTip = "Change the Positional/Selection Values to Pixel or Tile Based Values";
			Editor.Instance.nudgeFasterButton.ToolTip = "Move entities/tiles in a larger increment. (Configurable in Options)\r\nShortcut Key: " + KeyBindPraser("NudgeFaster");
			Editor.Instance.scrollLockButton.ToolTip = "Prevent the Mouse Wheel from Scrolling with the vertical scroll bar\r\nShortcut Key: " + KeyBindPraser("ScrollLock");
			Editor.Instance.ZoomInButton.ToolTip = "Zoom In (Ctrl + Wheel Up)";
			Editor.Instance.ZoomOutButton.ToolTip = "Zoom In (Ctrl + Wheel Down)";
			Editor.Instance.SelectToolButton.ToolTip = "Selection Tool" + KeyBindPraser("SelectTool", true);
			Editor.Instance.DrawToolButton.ToolTip = "Draw Tool" + KeyBindPraser("DrawTool", true);
			Editor.Instance.InteractionToolButton.ToolTip = "Interaction Tool";
			Editor.Instance.ShowCollisionAButton.ToolTip = "Show Collision Layer A" + KeyBindPraser("ShowPathA", true, true);
			Editor.Instance.ShowCollisionBButton.ToolTip = "Show Collision Layer B" + KeyBindPraser("ShowPathB", true, true);
			Editor.Instance.FlipAssistButton.ToolTip = "Show Flipped Tile Helper";
			Editor.Instance.ChunksToolButton.ToolTip = "Stamp Tool" + KeyBindPraser("StampTool", true);
            Editor.Instance.SplineToolButton.ToolTip = "Spline Tool" + KeyBindPraser("SplineTool", true);
            Editor.Instance.EncorePaletteButton.ToolTip = "Show Encore Colors";
			Editor.Instance.ShowTileIDButton.ToolTip = "Toggle Tile ID Visibility" + KeyBindPraser("ShowTileID", true, true);
			Editor.Instance.ShowGridButton.ToolTip = "Toggle Grid Visibility" + KeyBindPraser("ShowGrid", true, true);

		}

		#endregion

	}


}
