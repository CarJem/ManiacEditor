using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using ManiacEditor.Actions;
using ManiacEditor.Enums;
using ManiacEditor.Properties;
using RSDKv5;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Reflection;

namespace ManiacEditor
{
    public class EditorControls
    {
        private Editor Editor;

        public EditorControls(Editor instance)
        {
            Editor = instance;
			UpdateTooltips();
			UpdateMenuItems();
		}


        //Shorthanding Settings
        readonly Properties.Settings mySettings = Properties.Settings.Default;
        Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;

		public void GraphicPanel_OnKeyUp(object sender, KeyEventArgs e)
		{
			// Tiles Toolbar Flip Horizontal
			if (isCombo(e, myKeyBinds.FlipHTiles, true))
			{
				if (Editor.IsTilesEdit() && Editor.PlaceTilesButton.IsChecked.Value)
					Editor.TilesToolbar.SetSelectTileOption(0, false);
			}
			// Tiles Toolbar Flip Vertical
			else if (isCombo(e, myKeyBinds.FlipVTiles, true))
			{
				if (Editor.IsTilesEdit() && Editor.PlaceTilesButton.IsChecked.Value)
					Editor.TilesToolbar.SetSelectTileOption(1, false);
			}
		}

		public void GraphicPanel_OnKeyDown(object sender, KeyEventArgs e)
        {
            // Faster Nudge Toggle
            if (isCombo(e, myKeyBinds.NudgeFaster))
            {
                Editor.NudgeFasterButton_Click(sender, null);
            }
            // Scroll Lock Toggle
            else if (isCombo(e, myKeyBinds.ScrollLock))
            {
                Editor.ScrollLockButton_Click(sender, null);
            }
            // Switch Scroll Lock Type
            else if (isCombo(e, myKeyBinds.ScrollLockTypeSwitch))
            {
                Editor.SwapScrollLockDirectionToolStripMenuItem_Click(sender, null);

            }
			// Tiles Toolbar Flip Vertical
			else if (isCombo(e, myKeyBinds.FlipVTiles, true))
			{
                if (Editor.IsTilesEdit() && Editor.PlaceTilesButton.IsChecked.Value)
                    Editor.TilesToolbar.SetSelectTileOption(1, true);
            }
			// Tiles Toolbar Flip Horizontal
			else if (isCombo(e, myKeyBinds.FlipHTiles, true))
			{
                if (Editor.IsTilesEdit() && Editor.PlaceTilesButton.IsChecked.Value)
                    Editor.TilesToolbar.SetSelectTileOption(0, true);
            }
            // Open Click (Alt: Open Data Dir)
            else if ((isCombo(e, myKeyBinds.OpenDataDir)))
            {
                Editor.OpenDataDirectoryToolStripMenuItem_Click(null, null);
            }
            else if ((isCombo(e, myKeyBinds.Open)))
			{
                Editor.Open_Click(null, null);
            }
            // New Click
            else if (isCombo(e, myKeyBinds.New))
            {
                //Editor.New_Click(null, null);
            }
            // Save Click (Alt: Save As)
            else if (isCombo(e, myKeyBinds.SaveAs))
            {
                Editor.SaveAsToolStripMenuItem_Click(null, null);
            }
            else if (isCombo(e, myKeyBinds._Save))
            {
                Editor.Save_Click(null, null);
            }          
			// Undo
			else if (isCombo(e, myKeyBinds.Undo))
            {
                Editor.EditorUndo();
            }
            // Redo
            else if (isCombo(e, myKeyBinds.Redo))
            {
                Editor.EditorRedo();
            }
			else if (Editor.IsSceneLoaded())
			{
				GraphicPanel_OnKeyDownLoaded(sender, e);
			}
            // Editing Key Shortcuts
            if (Editor.IsEditing())
            {
                GraphicPanel_OnKeyDownEditing(sender, e);
            }
        }

		public void GraphicPanel_OnKeyDownLoaded(object sender, KeyEventArgs e)
		{
            // Reset Zoom Level
            if (isCombo(e, myKeyBinds.ResetZoomLevel))
			{
				Editor.SetZoomLevel(0, new Point(0, 0));
			}
			//Refresh Tiles and Sprites
			else if (isCombo(e, myKeyBinds.RefreshResources))
			{
				Editor.ReloadToolStripButton_Click(null, null);
			}
			//Run Scene
			else if (isCombo(e, myKeyBinds.RunScene))
			{
				Editor.SetZoomLevel(0, new Point(0, 0));
			}
			//Show Path A
			else if (isCombo(e, myKeyBinds.ShowPathA) && Editor.IsSceneLoaded())
			{
				Editor.ShowCollisionAButton_Click(null, null);
			}
			//Show Path B
			else if (isCombo(e, myKeyBinds.ShowPathB))
			{
				Editor.ShowCollisionBButton_Click(null, null);
			}
			//Unload Scene
			else if (isCombo(e, myKeyBinds.UnloadScene))
			{
				Editor.UnloadSceneToolStripMenuItem_Click(null, null);
			}
			//Toggle Grid Visibility
			else if (isCombo(e, myKeyBinds.ShowGrid))
			{
				Editor.ShowGridButton_Click(null, null);
			}
			//Toggle Tile ID Visibility
			else if (isCombo(e, myKeyBinds.ShowTileID))
			{
				Editor.ShowTileIDButton_Click(null, null);
			}
		}

		public void GraphicPanel_OnKeyDownEditing(object sender, KeyEventArgs e)
        {
			//Paste
			if (isCombo(e, myKeyBinds.Paste))
			{
                Editor.PasteToolStripMenuItem_Click(sender, null);
            }
			//Select All
			if (isCombo(e, myKeyBinds.SelectAll))
			{
                Editor.SelectAllToolStripMenuItem_Click(sender, null);
            }
            // Selected Key Shortcuts   
            if (Editor.IsSelected())
            {
                GraphicPanel_OnKeyDownSelectedEditing(sender, e);
            }
        }

        public void GraphicPanel_OnKeyDownSelectedEditing(object sender, KeyEventArgs e)
        {
			// Delete
			if (isCombo(e, myKeyBinds.Delete))
            {
                Editor.DeleteSelected();
            }

            // Moving
            else if (e.KeyData == Keys.Up || e.KeyData == Keys.Down || e.KeyData == Keys.Left || e.KeyData == Keys.Right)
            {
                Editor.MoveEntityOrTiles(sender, e);
            }

			//Cut 
			if (isCombo(e, myKeyBinds.Cut))
			{
				Editor.CutToolStripMenuItem_Click(sender, null);
			}
			//Copy
			else if (isCombo(e, myKeyBinds.Copy))
			{
				Editor.CopyToolStripMenuItem_Click(sender, null);
			}
			//Duplicate
			else if (isCombo(e, myKeyBinds.Duplicate))
			{
				Editor.DuplicateToolStripMenuItem_Click(sender, null);
			}
			// Flip Vertical Individual
			else if (isCombo(e, myKeyBinds.FlipVIndv))
			{
				if (Editor.IsTilesEdit())
					Editor.FlipVerticalIndividualToolStripMenuItem_Click(sender, null);
			}
			// Flip Horizontal Individual
			else if (isCombo(e, myKeyBinds.FlipHIndv))
			{
				if (Editor.IsTilesEdit())
					Editor.FlipHorizontalIndividualToolStripMenuItem_Click(sender, null);
			}
			// Flip Vertical
			else if (isCombo(e, myKeyBinds.FlipV))
			{
				if (Editor.IsTilesEdit())
					Editor.FlipVerticalToolStripMenuItem_Click(sender, null);
				else if (Editor.IsEntitiesEdit())
					Editor.FlipEntities(FlipDirection.Veritcal);
			}

			// Flip Horizontal
			else if (isCombo(e, myKeyBinds.FlipH))
			{
				if (Editor.IsTilesEdit())
					Editor.FlipHorizontalToolStripMenuItem_Click(sender, null);
				else if (Editor.IsEntitiesEdit())
					Editor.FlipEntities(FlipDirection.Horizontal);
			}
		}

		public bool isCombo(KeyEventArgs e, StringCollection keyCollection, bool singleKey = false)
		{
			foreach(string key in keyCollection)
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
			KeysConverter kc = new KeysConverter();

			if (e.KeyData == (Keys)kc.ConvertFromString(key)) return true;
			else return false;
		}

		public bool isComboCode(KeyEventArgs e, string key)
		{
			KeysConverter kc = new KeysConverter();

			if (e.KeyCode == (Keys)kc.ConvertFromString(key)) return true;
			else return false;
		}

		public string KeyBindPraser(string keyRefrence, bool tooltip = false)
		{
			List<string> keyBindList = new List<string>();
			List<string> keyBindModList = new List<string>();

			if (!Extensions.KeyBindsSettingExists(keyRefrence)) return "N/A";

			var keybindDict = Settings.myKeyBinds[keyRefrence] as StringCollection;
			if (keybindDict != null)
			{
				keyBindList = keybindDict.Cast<string>().ToList();
			}
			else
			{
				return "N/A";
			}

			if (keyBindList == null)
			{
				return "N/A";
			}

			if (keyBindList.Count > 1)
			{
				string keyBindLister = "";
				foreach(string key in keyBindList)
				{
					keyBindLister += String.Format("({0}) ", key);
				}
				if (tooltip) return String.Format(" ({0})", keyBindLister);
				else return keyBindLister;
			}
			else if ((keyBindList.Count == 1))
			{
				if (tooltip) return String.Format(" ({0})", keyBindList[0]);
				else return keyBindList[0];
			}
			else
			{
				return "N/A";
			}


		}

		public void UpdateMenuItems()
		{
			Editor.newToolStripMenuItem.InputGestureText = KeyBindPraser("New");
			Editor.openToolStripMenuItem.InputGestureText = KeyBindPraser("Open");
			Editor.openDataDirectoryToolStripMenuItem.InputGestureText = KeyBindPraser("OpenDataDir");
			Editor.saveToolStripMenuItem.InputGestureText = KeyBindPraser("_Save");
			Editor.saveAsToolStripMenuItem.InputGestureText = KeyBindPraser("SaveAs");
			Editor.undoToolStripMenuItem.InputGestureText = KeyBindPraser("Undo");
			Editor.redoToolStripMenuItem.InputGestureText = KeyBindPraser("Redo");
			Editor.cutToolStripMenuItem.InputGestureText = KeyBindPraser("Cut");
			Editor.copyToolStripMenuItem.InputGestureText = KeyBindPraser("Copy");
			Editor.pasteToolStripMenuItem.InputGestureText = KeyBindPraser("Paste");
			Editor.duplicateToolStripMenuItem.InputGestureText = KeyBindPraser("Duplicate");
			Editor.selectAllToolStripMenuItem.InputGestureText = KeyBindPraser("SelectAll");
			Editor.deleteToolStripMenuItem.InputGestureText = KeyBindPraser("Delete");
			Editor.statusNAToolStripMenuItem.InputGestureText = KeyBindPraser("ScrollLock");
			Editor.nudgeSelectionFasterToolStripMenuItem.InputGestureText = KeyBindPraser("NudgeFaster");
			Editor.swapScrollLockDirectionToolStripMenuItem.InputGestureText = KeyBindPraser("ScrollLockTypeSwitch");
			Editor.swapScrollLockDirMenuToolstripButton.InputGestureText = KeyBindPraser("ScrollLockTypeSwitch");
			Editor.resetZoomLevelToolstripMenuItem.InputGestureText = KeyBindPraser("ResetZoomLevel");
			Editor.unloadSceneToolStripMenuItem.InputGestureText = KeyBindPraser("UnloadScene");
			Editor.flipVerticalIndvidualToolStripMenuItem.InputGestureText = KeyBindPraser("FlipVIndv");
			Editor.flipHorizontalIndvidualToolStripMenuItem.InputGestureText = KeyBindPraser("FlipHIndv");
			Editor.flipHorizontalToolStripMenuItem.InputGestureText = KeyBindPraser("FlipH");
			Editor.flipVerticalToolStripMenuItem.InputGestureText = KeyBindPraser("FlipV");
		}

		public void UpdateTooltips()
		{
			Editor.New.ToolTip = "New Scene" + KeyBindPraser("New", true);
			Editor.Open.ToolTip = "Open Scene" + KeyBindPraser("Open", true);
			Editor.RecentDataDirectories.ToolTip = "Open Recent Data Folder";
			Editor.Save.ToolTip = "Save Scene" + KeyBindPraser("_Save", true);
			Editor.RunSceneButton.ToolTip = "Run Scene" + KeyBindPraser("RunScene", true);
			Editor.ReloadButton.ToolTip = "Reload Tiles and Sprites" + KeyBindPraser("RefreshResources", true);
			Editor.PointerButton.ToolTip = "Select/Move Tool";
			Editor.MagnetMode.ToolTip = "Magnet Mode";
			Editor.positionLabel.ToolTip = "The position relative to your mouse (Pixels Only for Now)";
			Editor.selectionSizeLabel.ToolTip = "The Size of the Selection";
			Editor.selectedPositionLabel.ToolTip = "The Position of the Selected Tile";
			Editor.selectionBoxSizeLabel.ToolTip = "The Size of the Selection Box";
			Editor.pixelModeButton.ToolTip = "Change the Positional/Selection Values to Pixel or Tile Based Values";
			Editor.nudgeFasterButton.ToolTip = "Move entities/tiles in a larger increment. (Configurable in Options)\r\nShortcut Key: " + KeyBindPraser("NudgeFaster");
			Editor.scrollLockButton.ToolTip = "Prevent the Mouse Wheel from Scrolling with the vertical scroll bar\r\nShortcut Key: " + KeyBindPraser("ScrollLock");
			Editor.ZoomInButton.ToolTip = "Zoom In (Ctrl + Wheel Up)";
			Editor.ZoomOutButton.ToolTip = "Zoom In (Ctrl + Wheel Down)";
			Editor.FreezeDeviceButton.ToolTip = "Freeze Device";
			Editor.SelectToolButton.ToolTip = "Selection Tool (To select groups of tiles and not dragged the clicked tile)";
			Editor.PlaceTilesButton.ToolTip = "Place tiles (Right click [+drag] - place, Left click [+drag] - delete)";
			Editor.InteractionToolButton.ToolTip = "Interaction Tool";
			Editor.ShowCollisionAButton.ToolTip = "Show Collision Layer A" + KeyBindPraser("ShowPathA", true);
			Editor.ShowCollisionBButton.ToolTip = "Show Collision Layer B" + KeyBindPraser("ShowPathB", true);
			Editor.FlipAssistButton.ToolTip = "Show Flipped Tile Helper";
			Editor.ChunksToolButton.ToolTip = "Stamp Tool";
			Editor.EncorePaletteButton.ToolTip = "Show Encore Colors";
			Editor.ShowTileIDButton.ToolTip = "Toggle Tile ID Visibility" + KeyBindPraser("ShowTileID", true);
			Editor.ShowGridButton.ToolTip = "Toggle Grid Visibility" + KeyBindPraser("ShowGrid", true);


		}
	}


}
