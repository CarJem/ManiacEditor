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

		[DllImport("User32.dll")]
		private static extern bool SetCursorPos(int X, int Y);

		#region Editor Definitions

		public int previousX = 0;
		public int previousY = 0;
		private int select_x1 { get => Editor.select_x1; set => Editor.select_x1 = value; }
		private int select_x2 { get => Editor.select_x2; set => Editor.select_x2 = value; }
		private int select_y1 { get => Editor.select_y1; set => Editor.select_y1 = value; }
		private int select_y2 { get => Editor.select_y2; set => Editor.select_y2 = value; }

		private int selectingY { get => Editor.selectingY; set => Editor.selectingY = value; }
		private int selectingX { get => Editor.selectingX; set => Editor.selectingX = value; }
		private int ClickedY { get => Editor.ClickedY; set => Editor.ClickedY = value; }
		private int ClickedX { get => Editor.ClickedX; set => Editor.ClickedX = value; }

		private int ShiftY { get => Editor.ShiftY; set => Editor.ShiftY = value; }
		private int ShiftX { get => Editor.ShiftX; set => Editor.ShiftX = value; }

		private int draggedY { get => Editor.draggedY; set => Editor.draggedY = value; }
		private int draggedX { get => Editor.draggedX; set => Editor.draggedX = value; }

		private int lastY { get => Editor.lastY; set => Editor.lastY = value; }
		private int lastX { get => Editor.lastX; set => Editor.lastX = value; }

		private double Zoom { get => Editor.Zoom; set => Editor.Zoom = value; }
		private int ZoomLevel { get => Editor.ZoomLevel; set => Editor.ZoomLevel = value; }

		private bool IsChunksEdit() { return Editor.IsChunksEdit(); }
		private bool IsTilesEdit() { return Editor.IsTilesEdit(); }
		private bool IsEntitiesEdit() { return Editor.IsEntitiesEdit();}
		private bool IsEditing() { return Editor.IsEditing(); }
		private bool IsSceneLoaded() { return Editor.IsSceneLoaded(); }

		private bool scrollingDragged { get => Editor.scrollingDragged; set => Editor.scrollingDragged = value; }
		private bool scrolling { get => Editor.scrolling; set => Editor.scrolling = value; }
		private bool dragged { get => Editor.dragged; set => Editor.dragged = value; }
		private bool startDragged { get => Editor.startDragged; set => Editor.startDragged = value; }
		private bool draggingSelection { get => Editor.draggingSelection; set => Editor.draggingSelection = value; }
		private bool GameRunning { get => Editor.GameRunning; set => Editor.GameRunning = value; }

		private int ScrollDirection { get => Editor.ScrollDirection; }
		private bool ScrollLocked { get => Editor.ScrollLocked; }

		private bool CtrlPressed() { return Editor.CtrlPressed(); }
		private bool ShiftPressed() { return Editor.ShiftPressed(); }
		private bool IsSelected() { return Editor.IsSelected(); }

		#endregion


		public EditorControls(Editor instance)
        {
            Editor = instance;
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
				if (IsTilesEdit() && Editor.PlaceTilesButton.IsChecked.Value)
					Editor.TilesToolbar.SetSelectTileOption(0, false);
			}
			// Tiles Toolbar Flip Vertical
			else if (isCombo(e, myKeyBinds.FlipVTiles, true))
			{
				if (IsTilesEdit() && Editor.PlaceTilesButton.IsChecked.Value)
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
			// Pointer ToolTip Toggle
			if (isCombo(e, myKeyBinds.PointerTooltipToggle))
			{
				Editor.TooltipButton_Click(sender, null);
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
                if (IsTilesEdit() && Editor.PlaceTilesButton.IsChecked.Value)
                    Editor.TilesToolbar.SetSelectTileOption(1, true);
            }
			// Tiles Toolbar Flip Horizontal
			else if (isCombo(e, myKeyBinds.FlipHTiles, true))
			{
                if (IsTilesEdit() && Editor.PlaceTilesButton.IsChecked.Value)
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
			// Developer Interface
			else if (isCombo(e, myKeyBinds.DeveloperInterface))
			{
				Editor.EditorUndo();
			}
			// Save for Force Open on Startup
			else if (isCombo(e, myKeyBinds.ForceOpenOnStartup))
			{
				Editor.EditorRedo();
			}
			else if (Editor.IsSceneLoaded())
			{
				GraphicPanel_OnKeyDownLoaded(sender, e);
			}
            // Editing Key Shortcuts
            if (IsEditing())
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
                Editor.Interactions.RunScene_Click(null, null);
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
			//Refresh Tiles and Sprites
			else if (isCombo(e, myKeyBinds.StatusBoxToggle))
			{
				Editor.statsToolStripMenuItem_Click(null, null);
			}
		}

		public void GraphicPanel_OnKeyDownEditing(object sender, KeyEventArgs e)
        {
			//Paste
			if (isCombo(e, myKeyBinds.Paste))
			{
                Editor.PasteToolStripMenuItem_Click(sender, null);
            }
			//Paste to Chunk
			if (isCombo(e, myKeyBinds.PasteToChunk))
			{
				Editor.chunkToolStripMenuItem_Click(sender, null);
			}
			//Select All
			if (isCombo(e, myKeyBinds.SelectAll))
			{
                Editor.SelectAllToolStripMenuItem_Click(sender, null);
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
				if (IsTilesEdit())
					Editor.FlipVerticalIndividualToolStripMenuItem_Click(sender, null);
			}
			// Flip Horizontal Individual
			else if (isCombo(e, myKeyBinds.FlipHIndv))
			{
				if (IsTilesEdit())
					Editor.FlipHorizontalIndividualToolStripMenuItem_Click(sender, null);
			}
			// Flip Vertical
			else if (isCombo(e, myKeyBinds.FlipV))
			{
				if (IsTilesEdit())
					Editor.FlipVerticalToolStripMenuItem_Click(sender, null);
				else if (IsEntitiesEdit())
					Editor.FlipEntities(FlipDirection.Veritcal);
			}

			// Flip Horizontal
			else if (isCombo(e, myKeyBinds.FlipH))
			{
				if (IsTilesEdit())
					Editor.FlipHorizontalToolStripMenuItem_Click(sender, null);
				else if (IsEntitiesEdit())
					Editor.FlipEntities(FlipDirection.Horizontal);
			}
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
				if (key.Contains("Del")) key = key.Replace("Del", "Delete");
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

		#region Mouse Controls

		bool ForceUpdateMousePos = false;

		public void ToggleScrollerMode(System.Windows.Forms.MouseEventArgs e)
		{

			if (!Editor.wheelClicked)
			{
				//Turn Scroller Mode On
				Editor.wheelClicked = true;
				scrolling = true;
				scrollingDragged = false;
				Editor.scrollPosition = new Point(e.X - ShiftX, e.Y - ShiftY);
				if (Editor.editorView.vScrollBar1.IsVisible && Editor.editorView.hScrollBar1.IsVisible)
				{
					Editor.Cursor = System.Windows.Input.Cursors.ScrollAll;
					SetScrollerBorderApperance((int)ScrollerModeDirection.ALL);
				}
				else if (Editor.editorView.vScrollBar1.IsVisible)
				{
					Editor.Cursor = System.Windows.Input.Cursors.ScrollWE;
					SetScrollerBorderApperance((int)ScrollerModeDirection.WE);
				}
				else if (Editor.editorView.hScrollBar1.IsVisible)
				{
					Editor.Cursor = System.Windows.Input.Cursors.ScrollNS;
					SetScrollerBorderApperance((int)ScrollerModeDirection.NS);
				}
				else
				{
					scrolling = false;
				}
			}
			else
			{
				//Turn Scroller Mode Off
				Editor.wheelClicked = false;
				if (scrollingDragged)
				{
					scrolling = false;
					Editor.Cursor = System.Windows.Input.Cursors.Arrow;
					SetScrollerBorderApperance();
				}
			}

		}
		public void GenerateMouseToolTip(System.Windows.Forms.MouseEventArgs e)
		{
			if (!Extensions.MouseIsOverGraphicsPanel(Editor.editorView.GraphicPanel))
			{
				RemoveToolTip();
			}
			if (Editor.showMouseTooltip)
			{
				if (previousX != e.X && previousY != e.Y)
				{
					RemoveToolTip();

					System.Windows.Controls.ToolTip tip = new System.Windows.Controls.ToolTip();
					tip.Content += "Pixel Position:";
					tip.Content += Environment.NewLine + String.Format("X: {0}, Y: {1}", e.X, e.Y);

					if (Editor.EditLayerA != null)
					{
						tip.Content += Environment.NewLine + "Chunk Position:";
						Point chunkPos = Editor.EditLayerA.GetChunkCoordinates(e.X / Zoom, e.Y / Zoom);
						tip.Content += Environment.NewLine + String.Format("X: {0}, Y: {1}", chunkPos.X, chunkPos.Y);
						tip.Content += Environment.NewLine + "Tile Position:";
						Point tilePos;
						if (e.X == 0 || e.Y == 0) tilePos = new Point(0, 0);
						else tilePos = new Point(e.X / 16, e.Y / 16);
						tip.Content += Environment.NewLine + String.Format("X: {0}, Y: {1}", tilePos.X, tilePos.Y);
					}
					tip.Content += Environment.NewLine;
					tip.Content += Environment.NewLine + String.Format("Copy Air: {0}", (Editor.CopyAir ? "ON" : "OFF"));
					tip.Content += Environment.NewLine + String.Format("Swap Slot ID Mode: {0}", (Editor.rightClicktoSwapSlotID ? "ON" : "OFF"));
					Editor.ViewPanelContextMenu.ToolTip = tip;
					if (Extensions.MouseIsOverGraphicsPanel(Editor.editorView.GraphicPanel)) tip.IsOpen = true;
					previousX = e.X;
					previousY = e.Y;
				}
			}


			void RemoveToolTip()
			{
				System.Windows.Controls.ToolTip oldtip = (System.Windows.Controls.ToolTip)Editor.ViewPanelContextMenu.ToolTip;
				if (oldtip != null) oldtip.IsOpen = false;
			}


		}
		public void UpdatePositionLabel(System.Windows.Forms.MouseEventArgs e)
		{

			if (Editor.EnablePixelCountMode == false)
			{
				Editor.positionLabel.Content = "X: " + (int)(e.X / Zoom) + " Y: " + (int)(e.Y / Zoom);
			}
			else
			{
				Editor.positionLabel.Content = "X: " + (int)((e.X / Zoom) / 16) + " Y: " + (int)((e.Y / Zoom) / 16);
			}
		}

		public enum ScrollerModeDirection : int
		{
			N = 0,
			NE = 1,
			E = 2,
			SE = 3,
			S = 4,
			SW = 5,
			W = 6,
			NW = 7,
			WE = 8,
			NS = 9,
			ALL = 10
		}
		public void SetScrollerBorderApperance(int direction = -1)
		{
			var converter = new System.Windows.Media.BrushConverter();
			var Active = (System.Windows.Media.Brush)converter.ConvertFromString("Red");
			var NotActive = (System.Windows.Media.Brush)converter.ConvertFromString("Transparent");

			Editor.ScrollBorderN.Fill = NotActive;
			Editor.ScrollBorderS.Fill = NotActive;
			Editor.ScrollBorderE.Fill = NotActive;
			Editor.ScrollBorderW.Fill = NotActive;
			Editor.ScrollBorderNW.Fill = NotActive;
			Editor.ScrollBorderSW.Fill = NotActive;
			Editor.ScrollBorderSE.Fill = NotActive;
			Editor.ScrollBorderNE.Fill = NotActive;

			switch (direction)
			{
				case 0:
					Editor.ScrollBorderN.Fill = Active;
					break;
				case 1:
					Editor.ScrollBorderNE.Fill = Active;
					break;
				case 2:
					Editor.ScrollBorderE.Fill = Active;
					break;
				case 3:
					Editor.ScrollBorderSE.Fill = Active;
					break;
				case 4:
					Editor.ScrollBorderS.Fill = Active;
					break;
				case 5:
					Editor.ScrollBorderSW.Fill = Active;
					break;
				case 6:
					Editor.ScrollBorderW.Fill = Active;
					break;
				case 7:
					Editor.ScrollBorderNW.Fill = Active;
					break;
				case 8:
					Editor.ScrollBorderW.Fill = Active;
					Editor.ScrollBorderE.Fill = Active;
					break;
				case 9:
					Editor.ScrollBorderN.Fill = Active;
					Editor.ScrollBorderS.Fill = Active;
					break;
				case 10:
					Editor.ScrollBorderN.Fill = Active;
					Editor.ScrollBorderS.Fill = Active;
					Editor.ScrollBorderE.Fill = Active;
					Editor.ScrollBorderW.Fill = Active;
					Editor.ScrollBorderNW.Fill = Active;
					Editor.ScrollBorderSW.Fill = Active;
					Editor.ScrollBorderSE.Fill = Active;
					Editor.ScrollBorderNE.Fill = Active;
					break;
				default:
					break;

			}

		}
		public void EnforceCursorPosition()
		{
			if (mySettings.ScrollerAutoCenters)
			{
				ForceUpdateMousePos = true;
				System.Windows.Point pointFromParent = Editor.ViewPanelForm.TranslatePoint(new System.Windows.Point(0, 0), Editor);
				SetCursorPos((int)(Editor.Left + pointFromParent.X) + (int)(Editor.ViewPanelForm.ActualWidth / 2), (int)(Editor.Left + pointFromParent.Y) + (int)(Editor.ViewPanelForm.ActualHeight / 2));
			}

		}

		public void UpdateScrollerPosition(System.Windows.Forms.MouseEventArgs e)
		{
			Editor.scrollPosition = new Point(e.X - ShiftX, e.Y - ShiftY);
			ForceUpdateMousePos = false;
		}


		public void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!scrolling) Editor.editorView.GraphicPanel.Focus();

			if (e.Button == MouseButtons.Left) Left();
			else if (e.Button == MouseButtons.Right) Right();
			else if (e.Button == MouseButtons.Middle) EnforceCursorPosition();


			void Left()
			{
				if (IsEditing() && !dragged) Editing();
				InGame();
				//if (scrolling) Scrolling();

				void Editing()
				{
					if (IsTilesEdit() && !Editor.InteractionToolButton.IsChecked.Value && !IsChunksEdit()) TilesEdit();
					if (IsChunksEdit() && IsSceneLoaded()) ChunksEdit();
					else if (IsEntitiesEdit()) EntitiesEdit();

					void TilesEdit()
					{
						if (Editor.PlaceTilesButton.IsChecked.Value)
						{
							// Place tile
							if (Editor.TilesToolbar.SelectedTile != -1)
							{
								Editor.EditorPlaceTile(new Point((int)(e.X / Zoom), (int)(e.Y / Zoom)), Editor.TilesToolbar.SelectedTile, Editor.EditLayerA);
							}
						}
						else
						{
							SetClickedXY(e);
						}
					}
					void ChunksEdit()
					{
						Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
						Point pC = Editor.EditLayerA.GetChunkCoordinates(p.X, p.Y);

						if (Editor.PlaceTilesButton.IsChecked.Value)
						{
							int selectedIndex = Editor.TilesToolbar.ChunkList.SelectedIndex;
							// Place Stamp
							if (selectedIndex != -1)
							{
								if (!Editor.EditorChunk.DoesChunkMatch(pC, Editor.EditorChunk.StageStamps.StampList[selectedIndex], Editor.EditLayerA))
								{
									Editor.EditorChunk.PasteStamp(pC, selectedIndex, Editor.EditLayerA, Editor.EditLayerB);
								}

							}
						}
						else if (Editor.PointerButton.IsChecked.Value)
						{
							if (!Editor.EditorChunk.IsChunkEmpty(pC, Editor.EditLayerA) || !Editor.EditorChunk.IsChunkEmpty(pC, Editor.EditLayerB))
							{
								//Select a Chunk Sized Area
								Editor.EditLayerA?.Select(new Rectangle(pC.X * 128, pC.Y * 128, 8 * 16, 8 * 16), true, true);
								Editor.EditLayerB?.Select(new Rectangle(pC.X * 128, pC.Y * 128, 8 * 16, 8 * 16), true, true);

							}
							else if (Editor.EditorChunk.IsChunkEmpty(pC, Editor.EditLayer))
							{
								//Deselect Everything
								Editor.EditLayerA?.Deselect();
								Editor.EditLayerB?.Deselect();
							}
							//SetClickedXY(new Point((int)((pC.X * 128) * Zoom), (int)((pC.Y * 128) * Zoom)));
						}
						else if (Editor.SelectToolButton.IsChecked.Value)
						{
							SetClickedXY(new Point((int)((pC.X * 128) * Zoom), (int)((pC.Y * 128) * Zoom)));
						}
					}
					void EntitiesEdit()
					{
						Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
						if (Editor.entities.GetEntityAt(clicked_point)?.Selected ?? false)
						{
							// We will have to check if this dragging or clicking
							SetClickedXY(e);
						}
						else if (!ShiftPressed() && !CtrlPressed() && Editor.entities.GetEntityAt(clicked_point) != null)
						{
							Editor.entities.Select(clicked_point);
							Editor.SetSelectOnlyButtonsState();
							// Start dragging the single selected entity
							dragged = true;
							draggedX = 0;
							draggedY = 0;
							startDragged = true;
						}
						else
						{
							SetClickedXY(e);
						}
					}
				}
				void InGame()
				{
					if (Editor.playerSelected)
					{
						Editor.playerSelected = false;
						Editor.selectedPlayer = 0;
					}
					if (Editor.checkpointSelected)
					{
						Editor.checkpointSelected = false;
					}
				}
				void Scrolling()
				{
				    ToggleScrollerMode(e);
				}
			}
			void Right()
			{
				if (IsTilesEdit() && Editor.PlaceTilesButton.IsChecked.Value && !IsChunksEdit()) TilesEdit();
				else if (IsChunksEdit() && Editor.PlaceTilesButton.IsChecked.Value) ChunksEdit();
				void TilesEdit()
				{
					// Remove tile
					Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					if (!Editor.EditLayerA.IsPointSelected(p) || !Editor.EditLayerB.IsPointSelected(p))
					{
						Editor.EditLayerA?.Select(p);
						Editor.EditLayerB?.Select(p);
					}
					Editor.DeleteSelected();
				}
				void ChunksEdit()
				{
					Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					Point pC = Editor.EditLayer.GetChunkCoordinates(p.X, p.Y);

					if (!Editor.EditorChunk.IsChunkEmpty(pC, Editor.EditLayer))
					{
						// Remove Stamp Sized Area
						Editor.EditorChunk.PasteStamp(pC, 0, Editor.EditLayerA, Editor.EditLayerB, true);
					}
				}
			}
		}
		public void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (ForceUpdateMousePos) UpdateScrollerPosition(e);
			if (Editor.InstanceID != -1 && !Editor.KickStartMegaManiacRenderLoopFinished) Editor.KickStartMegaManiacRenderLoop = true;
            if (scrolling || scrollingDragged) Editor.editorView.GraphicPanel.Render();

            Common();

			if (GameRunning) InGame();
			if (ClickedX != -1) DraggingStarting();
			if (scrolling) Scroller();
			else if (e.Button == MouseButtons.Middle) EnforceCursorPosition();
			if (IsEditing()) Editing();

			if (IsChunksEdit())
			{
				Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
				Point pC = Editor.EditLayer.GetChunkCoordinates(p.X, p.Y);
				lastX = (int)((pC.X * 128) * Zoom);
				lastY = (int)((pC.Y * 128) * Zoom);
			}
			else
			{
				lastX = e.X;
				lastY = e.Y;
			}


			void InGame()
			{
				if (Editor.playerSelected)
				{
					Editor.EditorGame.MovePlayer(new Point(e.X, e.Y), Zoom, Editor.selectedPlayer);
				}

				if (Editor.checkpointSelected)
				{
					Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					Editor.EditorGame.UpdateCheckpoint(clicked_point, true);
				}
			}
			void DraggingStarting()
			{
				if (IsTilesEdit() && !Editor.InteractionToolButton.IsChecked.Value && !IsChunksEdit())
				{
					TilesEdit();
				}
				else if (IsChunksEdit())
				{
					ChunksEdit();
				}
				else if (IsEntitiesEdit())
				{
					EntitiesEdit();
				}
				ClickedX = -1;
				ClickedY = -1;

				void TilesEdit()
				{
					// There was just a click now we can determine that this click is dragging
					Point clicked_point = new Point((int)(ClickedX / Zoom), (int)(ClickedY / Zoom));
					bool PointASelected = Editor.EditLayerA?.IsPointSelected(clicked_point) ?? false;
					bool PointBSelected = Editor.EditLayerB?.IsPointSelected(clicked_point) ?? false;
					if (PointASelected || PointBSelected)
					{
						// Start dragging the tiles
						dragged = true;
						startDragged = true;
						Editor.EditLayerA?.StartDrag();
						Editor.EditLayerB?.StartDrag();
					}

					else if (!Editor.SelectToolButton.IsChecked.Value && !ShiftPressed() && !CtrlPressed() && (Editor.EditLayerA?.HasTileAt(clicked_point) ?? false) || (Editor.EditLayerB?.HasTileAt(clicked_point) ?? false))
					{
						// Start dragging the single selected tile
						Editor.EditLayerA?.Select(clicked_point);
						Editor.EditLayerB?.Select(clicked_point);
						dragged = true;
						startDragged = true;
						Editor.EditLayerA?.StartDrag();
						Editor.EditLayerB?.StartDrag();
					}

					else
					{
						// Start drag selection
						//EditLayer.Select(clicked_point, ShiftPressed || CtrlPressed, CtrlPressed);
						if (!ShiftPressed() && !CtrlPressed())
							Editor.Deselect();
                        Editor.UpdateEditLayerActions();

						draggingSelection = true;
						selectingX = ClickedX;
						selectingY = ClickedY;
					}
				}
				void ChunksEdit()
				{

					// Start drag selection
					if (!ShiftPressed() && !CtrlPressed())
						Editor.Deselect();
					Editor.UpdateEditLayerActions();

					draggingSelection = true;
					selectingX = ClickedX;
					selectingY = ClickedY;
				}
				void EntitiesEdit()
				{
					// There was just a click now we can determine that this click is dragging
					Point clicked_point = new Point((int)(ClickedX / Zoom), (int)(ClickedY / Zoom));
					if (Editor.entities.GetEntityAt(clicked_point)?.Selected ?? false)
					{
						SetClickedXY(e);
						// Start dragging the entity
						dragged = true;
						draggedX = 0;
						draggedY = 0;
						startDragged = true;

					}
					else
					{
						// Start drag selection
						if (!ShiftPressed() && !CtrlPressed())
							Editor.Deselect();
						draggingSelection = true;
						selectingX = ClickedX;
						selectingY = ClickedY;

					}
				}
			}
			void Scroller()
			{
                
				if (Editor.wheelClicked)
				{
					scrollingDragged = true;

				}

                double xMove = (Editor.editorView.hScrollBar1.IsVisible) ? e.X - ShiftX - Editor.scrollPosition.X : 0;
                double yMove = (Editor.editorView.vScrollBar1.IsVisible) ? e.Y - ShiftY - Editor.scrollPosition.Y : 0;

				if (Math.Abs(xMove) < 15) xMove = 0;
				if (Math.Abs(yMove) < 15) yMove = 0;

				if (xMove > 0)
				{
					if (yMove > 0)
					{
						Editor.Cursor = System.Windows.Input.Cursors.ScrollSE;
						SetScrollerBorderApperance((int)ScrollerModeDirection.SE);
					}
					else if (yMove < 0)
					{
						Editor.Cursor = System.Windows.Input.Cursors.ScrollNE;
						SetScrollerBorderApperance((int)ScrollerModeDirection.NE);
					}
					else
					{
						Editor.Cursor = System.Windows.Input.Cursors.ScrollE;
						SetScrollerBorderApperance((int)ScrollerModeDirection.E);
					}

				}
				else if (xMove < 0)
				{
					if (yMove > 0)
					{
						Editor.Cursor = System.Windows.Input.Cursors.ScrollSW;
						SetScrollerBorderApperance((int)ScrollerModeDirection.SW);
					}
					else if (yMove < 0)
					{
						Editor.Cursor = System.Windows.Input.Cursors.ScrollNW;
						SetScrollerBorderApperance((int)ScrollerModeDirection.NW);
					}
					else
					{
						Editor.Cursor = System.Windows.Input.Cursors.ScrollW;
						SetScrollerBorderApperance((int)ScrollerModeDirection.W);
					}

				}
				else
				{

					if (yMove > 0)
					{
						Editor.Cursor = System.Windows.Input.Cursors.ScrollS;
						SetScrollerBorderApperance((int)ScrollerModeDirection.S);
					}
					else if (yMove < 0)
					{
						Editor.Cursor = System.Windows.Input.Cursors.ScrollN;
						SetScrollerBorderApperance((int)ScrollerModeDirection.N);
					}
					else
					{
						if (Editor.editorView.vScrollBar1.IsVisible && Editor.editorView.hScrollBar1.IsVisible)
						{
							Editor.Cursor = System.Windows.Input.Cursors.ScrollAll;
							SetScrollerBorderApperance((int)ScrollerModeDirection.ALL);
						}
						else if (Editor.editorView.vScrollBar1.IsVisible)
						{
							Editor.Cursor = System.Windows.Input.Cursors.ScrollNS;
							SetScrollerBorderApperance((int)ScrollerModeDirection.NS);
						}
						else if (Editor.editorView.hScrollBar1.IsVisible)
						{
							Editor.Cursor = System.Windows.Input.Cursors.ScrollWE;
							SetScrollerBorderApperance((int)ScrollerModeDirection.WE);
						}
					}

				}

                System.Windows.Point position = new System.Windows.Point(ShiftX, ShiftY);
				double x = xMove / 10 + position.X;
                double y = yMove / 10 + position.Y;

				Editor.CustomX += (int)xMove / 10;
				Editor.CustomY += (int)yMove / 10;

				if (x < 0) x = 0;
				if (y < 0) y = 0;
                if (x > Editor.editorView.hScrollBar1.Maximum) x = Editor.editorView.hScrollBar1.Maximum;
				if (y > Editor.editorView.vScrollBar1.Maximum) y = Editor.editorView.vScrollBar1.Maximum;


				if (x != position.X || y != position.Y)
				{

					if (Editor.editorView.vScrollBar1.IsVisible)
					{
						Editor.editorView.vScrollBar1.Value = y;
					}
					if (Editor.editorView.hScrollBar1.IsVisible)
					{
						Editor.editorView.hScrollBar1.Value = x;
					}

					Editor.editorView.GraphicPanel.OnMouseMoveEventCreate();

				}
                Editor.editorView.GraphicPanel.Render();
			}
			void Editing()
			{
				if (IsTilesEdit() && !IsChunksEdit()) TilesEdit();
				else if (IsChunksEdit()) ChunksEdit();
				if (draggingSelection || Editor.dragged) EdgeMove();
				if (draggingSelection) SetSelectionBounds();
				else if (Editor.dragged) DragMoveItems();

				void TilesEdit()
				{
					Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					if (Editor.PlaceTilesButton.IsChecked.Value)
					{
						if (e.Button == MouseButtons.Left)
						{
							// Place tile
							if (Editor.TilesToolbar.SelectedTile != -1)
							{
								if (Editor.EditLayer.GetTileAt(p) != Editor.TilesToolbar.SelectedTile)
								{
									Editor.EditorPlaceTile(p, Editor.TilesToolbar.SelectedTile, Editor.EditLayerA);
								}
								else if (!Editor.EditLayer.IsPointSelected(p))
								{
									Editor.EditLayer.Select(p);
								}
							}
						}
						else if (e.Button == MouseButtons.Right)
						{
							// Remove tile
							if (!(bool)Editor.EditLayerA?.IsPointSelected(p) || !(bool)Editor.EditLayerB?.IsPointSelected(p))
							{
								Editor.EditLayerA.Select(p);
								Editor.EditLayerB.Select(p);
							}
							Editor.DeleteSelected();

						}
					}
				}
				void ChunksEdit()
				{
					Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					Point pC = Editor.EditLayer.GetChunkCoordinates(p.X, p.Y);

					if (e.Button == MouseButtons.Left)
					{
						if (Editor.PlaceTilesButton.IsChecked.Value)
						{
							int selectedIndex = Editor.TilesToolbar.ChunkList.SelectedIndex;
							// Place Stamp
							if (selectedIndex != -1)
							{
								if (!Editor.EditorChunk.DoesChunkMatch(pC, Editor.EditorChunk.StageStamps.StampList[selectedIndex], Editor.EditLayer))
								{
									Editor.EditorChunk.PasteStamp(pC, selectedIndex, Editor.EditLayerA, Editor.EditLayerB);
								}

							}
						}
					}

					else if (e.Button == MouseButtons.Right)
					{
						if (Editor.PlaceTilesButton.IsChecked.Value)
						{
							if (!Editor.EditorChunk.IsChunkEmpty(pC, Editor.EditLayer))
							{
								// Remove Stamp Sized Area
								Editor.EditorChunk.PasteStamp(pC, 0, Editor.EditLayerA, Editor.EditLayerB, true);
							}
						}

					}
				}
				void EdgeMove()
				{
					System.Windows.Point position = new System.Windows.Point(ShiftX, ShiftY); ;
					double ScreenMaxX = position.X + Editor.editorView.splitContainer1.Panel1.Width - (int)Editor.editorView.vScrollBar.ActualWidth;
                    double ScreenMaxY = position.Y + Editor.editorView.splitContainer1.Panel1.Height - (int)Editor.editorView.hScrollBar.ActualHeight;
                    double ScreenMinX = position.X;
                    double ScreenMinY = position.Y;

                    double x = position.X;
                    double y = position.Y;

					if (e.X > ScreenMaxX)
					{
						x += (e.X - ScreenMaxX) / 10;
					}
					else if (e.X < ScreenMinX)
					{
						x += (e.X - ScreenMinX) / 10;
					}
					if (e.Y > ScreenMaxY)
					{
						y += (e.Y - ScreenMaxY) / 10;
					}
					else if (e.Y < ScreenMinY)
					{
						y += (e.Y - ScreenMinY) / 10;
					}

					if (x < 0) x = 0;
					if (y < 0) y = 0;
					if (x > Editor.editorView.hScrollBar1.Maximum) x = Editor.editorView.hScrollBar1.Maximum;
					if (y > Editor.editorView.vScrollBar1.Maximum) y = Editor.editorView.vScrollBar1.Maximum;

					if (x != position.X || y != position.Y)
					{
						if (Editor.editorView.vScrollBar1.IsVisible)
						{
							Editor.editorView.vScrollBar1.Value = y;
						}
						if (Editor.editorView.hScrollBar1.IsVisible)
						{
							Editor.editorView.hScrollBar1.Value = x;
						}
						Editor.editorView.GraphicPanel.OnMouseMoveEventCreate();
                        if (!scrolling) Editor.editorView.GraphicPanel.Render();



					}
				}
				void SetSelectionBounds()
				{
					if (IsChunksEdit()) ChunkMode();
					else Normal();

					void ChunkMode()
					{
						Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
						Point pC = Editor.EditLayer.GetChunkCoordinates(p.X, p.Y);
						Point eC = new Point((int)(pC.X * 128 * Zoom), (int)(pC.Y * 128 * Zoom));

						if (selectingX != eC.X && selectingY != eC.Y)
						{
							select_x1 = (int)(selectingX / Zoom);
							select_x2 = (int)(eC.X / Zoom);
							select_y1 = (int)(selectingY / Zoom);
							select_y2 = (int)(eC.Y / Zoom);
							if (select_x1 > select_x2)
							{
								select_x1 = (int)(eC.X / Zoom);
								select_x2 = (int)(selectingX / Zoom);
							}
							if (select_y1 > select_y2)
							{
								select_y1 = (int)(eC.Y / Zoom);
								select_y2 = (int)(selectingY / Zoom);
							}
							Editor.EditLayerA?.TempSelection(new Rectangle(select_x1, select_y1, select_x2 - select_x1, select_y2 - select_y1), CtrlPressed());
							Editor.EditLayerB?.TempSelection(new Rectangle(select_x1, select_y1, select_x2 - select_x1, select_y2 - select_y1), CtrlPressed());

							Editor.UpdateTilesOptions();
						}
					}
					void Normal()
					{
						if (selectingX != e.X && selectingY != e.Y)
						{
							select_x1 = (int)(selectingX / Zoom);
							select_x2 = (int)(e.X / Zoom);
							select_y1 = (int)(selectingY / Zoom);
							select_y2 = (int)(e.Y / Zoom);
							if (select_x1 > select_x2)
							{
								select_x1 = (int)(e.X / Zoom);
								select_x2 = (int)(selectingX / Zoom);
							}
							if (select_y1 > select_y2)
							{
								select_y1 = (int)(e.Y / Zoom);
								select_y2 = (int)(selectingY / Zoom);
							}
							Editor.EditLayerA?.TempSelection(new Rectangle(select_x1, select_y1, select_x2 - select_x1, select_y2 - select_y1), CtrlPressed());
							Editor.EditLayerB?.TempSelection(new Rectangle(select_x1, select_y1, select_x2 - select_x1, select_y2 - select_y1), CtrlPressed());

							Editor.UpdateTilesOptions();

							if (IsEntitiesEdit()) Editor.entities.TempSelection(new Rectangle(select_x1, select_y1, select_x2 - select_x1, select_y2 - select_y1), CtrlPressed());
						}
					}
				}
				void DragMoveItems()
				{
					int oldGridX = (int)((lastX / Zoom) / Editor.magnetSize) * Editor.magnetSize;
					int oldGridY = (int)((lastY / Zoom) / Editor.magnetSize) * Editor.magnetSize;
					int newGridX = (int)((e.X / Zoom) / Editor.magnetSize) * Editor.magnetSize;
					int newGridY = (int)((e.Y / Zoom) / Editor.magnetSize) * Editor.magnetSize;
					Point oldPointGrid = new Point(0, 0);
					Point newPointGrid = new Point(0, 0);
					if (Editor.UseMagnetMode && IsEntitiesEdit())
					{
						if (Editor.useMagnetXAxis == true && Editor.useMagnetYAxis == true)
						{
							oldPointGrid = new Point(oldGridX, oldGridY);
							newPointGrid = new Point(newGridX, newGridY);
						}
						if (Editor.useMagnetXAxis && !Editor.useMagnetYAxis)
						{
							oldPointGrid = new Point(oldGridX, (int)(lastY / Zoom));
							newPointGrid = new Point(newGridX, (int)(e.Y / Zoom));
						}
						if (!Editor.useMagnetXAxis && Editor.useMagnetYAxis)
						{
							oldPointGrid = new Point((int)(lastX / Zoom), oldGridY);
							newPointGrid = new Point((int)(e.X / Zoom), newGridY);
						}
						if (!Editor.useMagnetXAxis && !Editor.useMagnetYAxis)
						{
							oldPointGrid = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
							newPointGrid = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
						}
					}
					Point oldPoint = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
					Point newPoint = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));


					Editor.EditLayerA?.MoveSelected(oldPoint, newPoint, CtrlPressed());
					Editor.EditLayerB?.MoveSelected(oldPoint, newPoint, CtrlPressed());

					Editor.UpdateEditLayerActions();
					if (IsEntitiesEdit())
					{
						if (Editor.UseMagnetMode)
						{
							int x = Editor.entities.SelectedEntities[0].Entity.Position.X.High;
							int y = Editor.entities.SelectedEntities[0].Entity.Position.Y.High;

							if (x % Editor.magnetSize != 0 && Editor.useMagnetXAxis)
							{
								int offsetX = x % Editor.magnetSize;
								oldPointGrid.X -= offsetX;
							}
							if (y % Editor.magnetSize != 0 && Editor.useMagnetYAxis)
							{
								int offsetY = y % Editor.magnetSize;
								oldPointGrid.Y -= offsetY;
							}
						}


						try
						{

							if (Editor.UseMagnetMode)
							{
								Editor.entities.MoveSelected(oldPointGrid, newPointGrid, CtrlPressed() && Editor.startDragged);
							}
							else
							{
								Editor.entities.MoveSelected(oldPoint, newPoint, CtrlPressed() && Editor.startDragged);
							}

						}
						catch (EditorEntities.TooManyEntitiesException)
						{
							RSDKrU.MessageBox.Show("Too many entities! (limit: 2048)");
							Editor.dragged = false;
							return;
						}
						if (Editor.UseMagnetMode)
						{
							draggedX += newPointGrid.X - oldPointGrid.X;
							draggedY += newPointGrid.Y - oldPointGrid.Y;
						}
						else
						{
							draggedX += newPoint.X - oldPoint.X;
							draggedY += newPoint.Y - oldPoint.Y;
						}
						if (CtrlPressed() && Editor.startDragged)
						{
							Editor.UpdateEntitiesToolbarList();
							Editor.SetSelectOnlyButtonsState();
						}
						Editor.entitiesToolbar.UpdateCurrentEntityProperites();
					}
					Editor.startDragged = false;
				}
			}
			void Common()
			{
				UpdatePositionLabel(e);
				GenerateMouseToolTip(e);
			}
		}
		public void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) Left();
			else if (e.Button == MouseButtons.Right) Right();
			else if (e.Button == MouseButtons.Middle) Middle();



			Editor.UpdateControls();

			void Left()
			{
				if (IsEditing())
				{
					if (draggingSelection) DraggingSel();
					else NotDragging();
				}
				
				void DraggingSel()
				{
					if (IsChunksEdit()) ChunksMode();
					else Normal();

					void ChunksMode()
					{
						Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
						Point pC = Editor.EditLayer.GetChunkCoordinates(p.X, p.Y);
						Point eC = new Point((int)((pC.X * 128) * Zoom), (int)((pC.Y * 128) * Zoom));

						if (selectingX != (int)(eC.X) && selectingY != (int)(eC.Y))
						{

							int x1 = (int)(selectingX / Zoom), x2 = (int)(eC.X / Zoom);
							int y1 = (int)(selectingY / Zoom), y2 = (int)(eC.Y / Zoom);
							if (x1 > x2)
							{
								x1 = (int)(eC.X / Zoom);
								x2 = (int)(selectingX / Zoom);
							}
							if (y1 > y2)
							{
								y1 = (int)(eC.Y / Zoom);
								y2 = (int)(selectingY / Zoom);
							}
							Editor.EditLayerA?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.EditLayerB?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());

							if (IsEntitiesEdit()) Editor.entities.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.SetSelectOnlyButtonsState();
							Editor.UpdateEditLayerActions();

						}
						draggingSelection = false;
						Editor.EditLayerA?.EndTempSelection();
						Editor.EditLayerB?.EndTempSelection();

						if (IsEntitiesEdit()) Editor.entities.EndTempSelection();
					}
					void Normal()
					{
						if (selectingX != e.X && selectingY != e.Y)
						{

							int x1 = (int)(selectingX / Zoom), x2 = (int)(e.X / Zoom);
							int y1 = (int)(selectingY / Zoom), y2 = (int)(e.Y / Zoom);
							if (x1 > x2)
							{
								x1 = (int)(e.X / Zoom);
								x2 = (int)(selectingX / Zoom);
							}
							if (y1 > y2)
							{
								y1 = (int)(e.Y / Zoom);
								y2 = (int)(selectingY / Zoom);
							}
							Editor.EditLayerA?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.EditLayerB?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());

							if (IsEntitiesEdit()) Editor.entities.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.SetSelectOnlyButtonsState();
							Editor.UpdateEditLayerActions();

						}
						draggingSelection = false;
						Editor.EditLayerA?.EndTempSelection();
						Editor.EditLayerB?.EndTempSelection();

						if (IsEntitiesEdit()) Editor.entities.EndTempSelection();
					}

				}
				void NotDragging()
				{
					if (ClickedX != -1) Click();
					if (dragged && (draggedX != 0 || draggedY != 0)) UpdateUndoRedo();
					dragged = false;

					void Click()
					{
						// So it was just click
						Point clicked_point = new Point((int)(ClickedX / Zoom), (int)(ClickedY / Zoom));

						if (IsTilesEdit() && !IsChunksEdit()) TilesEdit();
						else if (IsChunksEdit()) ChunksEdit();
						else if (IsEntitiesEdit()) EntitiesEdit();
						Editor.SetSelectOnlyButtonsState();
						ClickedX = -1;
						ClickedY = -1;

						void TilesEdit()
						{
							Editor.EditLayerA?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.EditLayerB?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.UpdateEditLayerActions();
						}
						void ChunksEdit()
						{
							Point chunk_point = Editor.EditLayer.GetChunkCoordinates(clicked_point.X, clicked_point.Y);
							Point clicked_chunk = new Point(chunk_point.X * 128, chunk_point.Y * 128);
							Editor.EditLayerA?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.EditLayerB?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.UpdateEditLayerActions();
						}
						void EntitiesEdit()
						{
							Editor.entities.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());

						}
					}
					void UpdateUndoRedo()
					{
						if (IsEntitiesEdit())
						{
							IAction action = new ActionMoveEntities(Editor.entities.SelectedEntities.ToList(), new Point(draggedX, draggedY));
							if (Editor.entities.LastAction != null)
							{
								// If it is move & duplicate, merge them together
								var taction = new ActionsGroup();
								taction.AddAction(Editor.entities.LastAction);
								Editor.entities.LastAction = null;
								taction.AddAction(action);
								taction.Close();
								action = taction;
							}
							Editor.undo.Push(action);
							Editor.redo.Clear();
							Editor.UpdateControls();
						}
					}
				}
			}
			void Right()
			{
				if (IsEntitiesEdit())
				{
					if (Editor.entities.SelectedEntities.Count == 2 && Editor.rightClicktoSwapSlotID)
					{
						var entity1 = Editor.entities.selectedEntities[0];
						var entity2 = Editor.entities.selectedEntities[1];
						ushort slotID1 = entity1.Entity.SlotID;
						ushort slotID2 = entity2.Entity.SlotID;
						entity1.Entity.SlotID = slotID2;
						entity2.Entity.SlotID = slotID1;
					}
				}

			}
			void Middle()
			{
				ToggleScrollerMode(e);
			}
		}
		public void MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Editor.editorView.GraphicPanel.Focus();
			if (CtrlPressed()) Ctrl();
			else NoCtrl();

			void Ctrl()
			{
				int maxZoom;
				int minZoom;
				if (mySettings.ReduceZoom)
				{
					maxZoom = 5;
					minZoom = -2;
				}
				else
				{
					maxZoom = 5;
					minZoom = -5;
				}
				int change = e.Delta / 120;
				ZoomLevel += change;
				if (ZoomLevel > maxZoom) ZoomLevel = maxZoom;
				if (ZoomLevel < minZoom) ZoomLevel = minZoom;

				Editor.SetZoomLevel(ZoomLevel, new Point(e.X - ShiftX, e.Y - ShiftY));
			}
			void NoCtrl()
			{
				if (Editor.editorView.vScrollBar1.IsVisible || Editor.editorView.hScrollBar1.IsVisible) ScrollMove();
				if (mySettings.EntityFreeCam) FreeCamScroll();

				void ScrollMove()
				{
					if (ScrollDirection == (int)ScrollDir.Y && !ScrollLocked)
					{
                        if (Editor.editorView.vScrollBar1.IsVisible) VScroll();
                        else HScroll();
                    }
					else if (ScrollDirection == (int)ScrollDir.X && !ScrollLocked)
					{
                        if (Editor.editorView.hScrollBar1.IsVisible) HScroll();
                        else VScroll();
					}
					else if (ScrollLocked)
					{
						if (ScrollDirection == (int)ScrollDir.Y)
						{
                            if (Editor.editorView.vScrollBar1.IsVisible) VScroll();
                            else HScroll();
                        }
						else
						{
                            if (Editor.editorView.hScrollBar1.IsVisible) HScroll();
                            else VScroll();
                        }

					}
				}
				void FreeCamScroll()
				{
					if (ScrollDirection == (int)ScrollDir.X) Editor.CustomX -= e.Delta;
					else Editor.CustomY -= e.Delta;
				}
			}

            void VScroll()
            {
                double y = Editor.editorView.vScrollBar1.Value - e.Delta;
                if (y < 0) y = 0;
                if (y > Editor.editorView.vScrollBar1.Maximum) y = Editor.editorView.vScrollBar1.Maximum;
                Editor.editorView.vScrollBar1.Value = y;
            }

            void HScroll()
            {
                double x = Editor.editorView.hScrollBar1.Value - e.Delta;
                if (x < 0) x = 0;
                if (x > Editor.editorView.hScrollBar1.Maximum) x = Editor.editorView.hScrollBar1.Maximum;
                Editor.editorView.hScrollBar1.Value = x;
            }
        }
		public void MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Editor.editorView.GraphicPanel.Focus();
			if (e.Button == MouseButtons.Right && Editor.InteractionToolButton.IsChecked.Value) InteractionTool();

			void InteractionTool() {
				if (IsTilesEdit())
				{
					Point clicked_point_tile = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					int tile;
					int tileA = (ushort)(Editor.EditLayerA?.GetTileAt(clicked_point_tile) & 0x3ff);
					int tileB = 0;
					if (Editor.EditLayerB != null)
					{
						tileB = (ushort)(Editor.EditLayerB?.GetTileAt(clicked_point_tile) & 0x3ff);
						if (tileA > 1023 && tileB < 1023) tile = tileB;
						else tile = tileA;
					}
					else tile = tileA;


					Editor.SelectedTileID = tile;
					Editor.editTile0WithTileManiacToolStripMenuItem.IsEnabled = (tile < 1023);
					Editor.moveThePlayerToHereToolStripMenuItem.IsEnabled = GameRunning;
					Editor.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
					Editor.removeCheckpointToolStripMenuItem.IsEnabled = GameRunning && Editor.EditorGame.CheckpointEnabled;
					Editor.assetResetToolStripMenuItem.IsEnabled = GameRunning;
					Editor.restartSceneToolStripMenuItem.IsEnabled = GameRunning;
					Editor.moveCheckpointToolStripMenuItem.IsEnabled = GameRunning && Editor.EditorGame.CheckpointEnabled;


					Editor.editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);
					Editor.ViewPanelContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
					Editor.ViewPanelContextMenu.IsOpen = true;
				}
				else
				{
					Point clicked_point_tile = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					string tile = "N/A";
					Editor.editTile0WithTileManiacToolStripMenuItem.IsEnabled = false;
					Editor.moveThePlayerToHereToolStripMenuItem.IsEnabled = GameRunning;
					Editor.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
					Editor.moveThisPlayerToolStripMenuItem.IsEnabled = GameRunning;
					Editor.moveCheckpointToolStripMenuItem.IsEnabled = GameRunning;

					Editor.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
					Editor.removeCheckpointToolStripMenuItem.IsEnabled = GameRunning;
					Editor.assetResetToolStripMenuItem.IsEnabled = GameRunning;
					Editor.restartSceneToolStripMenuItem.IsEnabled = GameRunning;
					Editor.moveCheckpointToolStripMenuItem.IsEnabled = GameRunning;

					Editor.editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);
					Editor.ViewPanelContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
					Editor.ViewPanelContextMenu.IsOpen = true;
				}
			}
		}
		public void SetClickedXY(System.Windows.Forms.MouseEventArgs e) { ClickedX = e.X; ClickedY = e.Y; }
		public void SetClickedXY(Point e) { ClickedX = e.X; ClickedY = e.Y;}
		#endregion

		#region Tooltips + Menu Items

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
			Editor.nudgeSelectionFasterToolStripMenuItem.InputGestureText = KeyBindPraser("NudgeFaster", false, true);
			Editor.swapScrollLockDirectionToolStripMenuItem.InputGestureText = KeyBindPraser("ScrollLockTypeSwitch", false, true);
			Editor.swapScrollLockDirMenuToolstripButton.InputGestureText = KeyBindPraser("ScrollLockTypeSwitch", false, true);
			Editor.resetZoomLevelToolstripMenuItem.InputGestureText = KeyBindPraser("ResetZoomLevel");
			Editor.unloadSceneToolStripMenuItem.InputGestureText = KeyBindPraser("UnloadScene", false, true);
			Editor.flipVerticalIndvidualToolStripMenuItem.InputGestureText = KeyBindPraser("FlipVIndv");
			Editor.flipHorizontalIndvidualToolStripMenuItem.InputGestureText = KeyBindPraser("FlipHIndv");
			Editor.flipHorizontalToolStripMenuItem.InputGestureText = KeyBindPraser("FlipH");
			Editor.flipVerticalToolStripMenuItem.InputGestureText = KeyBindPraser("FlipV");
			Editor.chunkToolStripMenuItem.InputGestureText = KeyBindPraser("PasteToChunk", false, true);
			Editor.developerInterfaceToolStripMenuItem.InputGestureText = KeyBindPraser("DeveloperInterface", false, true);
			Editor.saveForForceOpenOnStartupToolStripMenuItem.InputGestureText = KeyBindPraser("ForceOpenOnStartup", false, true);
			Editor.copyAirToggle.InputGestureText = KeyBindPraser("CopyAirTiles", false, true);
			Editor.PointerTooltipToggleToolStripMenuItem.InputGestureText = KeyBindPraser("PointerTooltipToggle", false, true);
		}

		public void UpdateTooltips()
		{
			Editor.New.ToolTip = "New Scene" + KeyBindPraser("New", true);
			Editor.Open.ToolTip = "Open Scene" + KeyBindPraser("Open", true);
			Editor.RecentDataDirectories.ToolTip = "Open Recent Data Folder";
			Editor.Save.ToolTip = "Save Scene" + KeyBindPraser("_Save", true);
			Editor.RunSceneButton.ToolTip = "Run Scene" + KeyBindPraser("RunScene", true, true);
			Editor.ReloadButton.ToolTip = "Reload Tiles and Sprites" + KeyBindPraser("RefreshResources", true, true);
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
			Editor.ShowCollisionAButton.ToolTip = "Show Collision Layer A" + KeyBindPraser("ShowPathA", true, true);
			Editor.ShowCollisionBButton.ToolTip = "Show Collision Layer B" + KeyBindPraser("ShowPathB", true, true);
			Editor.FlipAssistButton.ToolTip = "Show Flipped Tile Helper";
			Editor.ChunksToolButton.ToolTip = "Stamp Tool";
			Editor.EncorePaletteButton.ToolTip = "Show Encore Colors";
			Editor.ShowTileIDButton.ToolTip = "Toggle Tile ID Visibility" + KeyBindPraser("ShowTileID", true, true);
			Editor.ShowGridButton.ToolTip = "Toggle Grid Visibility" + KeyBindPraser("ShowGrid", true, true);
			Editor.tooltipButton.ToolTip = "Shows a Tooltip that displays helpful information at the cursor's position\r\nShortcut Key: " + KeyBindPraser("PointerTooltipToggle", true, true);


		}

		#endregion

	}


}
