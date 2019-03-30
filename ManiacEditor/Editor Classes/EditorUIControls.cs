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
    public class EditorUIControl
    {
        private Editor Editor;


        [DllImport("User32.dll")]
		private static extern bool SetCursorPos(int X, int Y);

		#region Editor Definitions

		public int previousX = 0;
		public int previousY = 0;
		private int select_x1 { get => Editor.StateModel.select_x1; set => Editor.StateModel.select_x1 = value; }
		private int select_x2 { get => Editor.StateModel.select_x2; set => Editor.StateModel.select_x2 = value; }
		private int select_y1 { get => Editor.StateModel.select_y1; set => Editor.StateModel.select_y1 = value; }
		private int select_y2 { get => Editor.StateModel.select_y2; set => Editor.StateModel.select_y2 = value; }

		private int selectingY { get => Editor.StateModel.selectingY; set => Editor.StateModel.selectingY = value; }
		private int selectingX { get => Editor.StateModel.selectingX; set => Editor.StateModel.selectingX = value; }
		private int ClickedY { get => Editor.StateModel.ClickedY; set => Editor.StateModel.ClickedY = value; }
		private int ClickedX { get => Editor.StateModel.ClickedX; set => Editor.StateModel.ClickedX = value; }

		private int ShiftY { get => Editor.StateModel.ShiftY; set => Editor.StateModel.ShiftY = value; }
		private int ShiftX { get => Editor.StateModel.ShiftX; set => Editor.StateModel.ShiftX = value; }

		private int draggedY { get => Editor.StateModel.draggedY; set => Editor.StateModel.draggedY = value; }
		private int draggedX { get => Editor.StateModel.draggedX; set => Editor.StateModel.draggedX = value; }

		private int lastY { get => Editor.StateModel.lastY; set => Editor.StateModel.lastY = value; }
		private int lastX { get => Editor.StateModel.lastX; set => Editor.StateModel.lastX = value; }

		private double Zoom { get => Editor.StateModel.Zoom; set => Editor.StateModel.Zoom = value; }
		private int ZoomLevel { get => Editor.StateModel.ZoomLevel; set => Editor.StateModel.ZoomLevel = value; }

		private bool IsChunksEdit() { return Editor.IsChunksEdit(); }
		private bool IsTilesEdit() { return Editor.IsTilesEdit(); }
		private bool IsEntitiesEdit() { return Editor.IsEntitiesEdit();}
		private bool IsEditing() { return Editor.IsEditing(); }
		private bool IsSceneLoaded() { return Editor.IsSceneLoaded(); }

		private bool scrollingDragged { get => Editor.StateModel.scrollingDragged; set => Editor.StateModel.scrollingDragged = value; }
		private bool scrolling { get => Editor.StateModel.scrolling; set => Editor.StateModel.scrolling = value; }
		private bool dragged { get => Editor.StateModel.dragged; set => Editor.StateModel.dragged = value; }
		private bool startDragged { get => Editor.StateModel.startDragged; set => Editor.StateModel.startDragged = value; }
		private bool draggingSelection { get => Editor.StateModel.draggingSelection; set => Editor.StateModel.draggingSelection = value; }
		private bool GameRunning { get => Editor.InGame.GameRunning; set => Editor.InGame.GameRunning = value; }

		private int ScrollDirection { get => Editor.UIModes.ScrollDirection; }
		private bool ScrollLocked { get => Editor.UIModes.ScrollLocked; }

		private bool CtrlPressed() { return Editor.CtrlPressed(); }
		private bool ShiftPressed() { return Editor.ShiftPressed(); }
		private bool IsSelected() { return Editor.IsSelected(); }

		#endregion


		public EditorUIControl(Editor instance)
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
                Editor.ToggleFasterNudgeEvent(sender, null);
            }
			// Scroll Lock Toggle
			else if (isCombo(e, myKeyBinds.ScrollLock))
            {
                Editor.UIModes.ScrollLocked ^= true;
            }
            // Switch Scroll Lock Type
            else if (isCombo(e, myKeyBinds.ScrollLockTypeSwitch))
            {
                Editor.UIEvents.SetScrollLockDirection();

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
                Editor.OpenDataDirectoryEvent(null, null);
            }
            else if ((isCombo(e, myKeyBinds.Open)))
			{
                Editor.OpenSceneEvent(null, null);
            }
            // New Click
            else if (isCombo(e, myKeyBinds.New))
            {
                //Editor.New_Click(null, null);
            }
            // Save Click (Alt: Save As)
            else if (isCombo(e, myKeyBinds.SaveAs))
            {
                Editor.SaveSceneAsEvent(null, null);
            }
            else if (isCombo(e, myKeyBinds._Save))
            {
                Editor.SaveSceneEvent(null, null);
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
				Editor.ZoomModel.SetZoomLevel(0, new Point(0, 0));
			}
			//Refresh Tiles and Sprites
			else if (isCombo(e, myKeyBinds.RefreshResources))
			{
				Editor.ReloadToolStripButton_Click(null, null);
			}
			//Run Scene
			else if (isCombo(e, myKeyBinds.RunScene))
			{
                Editor.InGame.RunScene();
			}
			//Show Path A
			else if (isCombo(e, myKeyBinds.ShowPathA) && Editor.IsSceneLoaded())
			{
				Editor.ShowCollisionAEvent(null, null);
			}
			//Show Path B
			else if (isCombo(e, myKeyBinds.ShowPathB))
			{
				Editor.ShowCollisionBEvent(null, null);
			}
			//Unload Scene
			else if (isCombo(e, myKeyBinds.UnloadScene))
			{
				Editor.UnloadSceneEvent(null, null);
			}
			//Toggle Grid Visibility
			else if (isCombo(e, myKeyBinds.ShowGrid))
			{
				Editor.ToggleGridEvent(null, null);
			}
			//Toggle Tile ID Visibility
			else if (isCombo(e, myKeyBinds.ShowTileID))
			{
				Editor.ToggleSlotIDEvent(null, null);
			}
			//Refresh Tiles and Sprites
			else if (isCombo(e, myKeyBinds.StatusBoxToggle))
			{
				Editor.ToggleDebugHUDEvent(null, null);
			}
		}

		public void GraphicPanel_OnKeyDownEditing(object sender, KeyEventArgs e)
        {
			//Paste
			if (isCombo(e, myKeyBinds.Paste))
			{
                Editor.PasteEvent(sender, null);
            }
			//Paste to Chunk
			if (isCombo(e, myKeyBinds.PasteToChunk))
			{
				Editor.PasteToChunksEvent(sender, null);
			}
			//Select All
			if (isCombo(e, myKeyBinds.SelectAll))
			{
                Editor.SelectAllEvent(sender, null);
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
				Editor.CutEvent(sender, null);
			}
			//Copy
			else if (isCombo(e, myKeyBinds.Copy))
			{
				Editor.CopyEvent(sender, null);
			}
			//Duplicate
			else if (isCombo(e, myKeyBinds.Duplicate))
			{
				Editor.DuplicateEvent(sender, null);
			}
			// Flip Vertical Individual
			else if (isCombo(e, myKeyBinds.FlipVIndv))
			{
				if (IsTilesEdit())
					Editor.FlipVerticalIndividualEvent(sender, null);
			}
			// Flip Horizontal Individual
			else if (isCombo(e, myKeyBinds.FlipHIndv))
			{
				if (IsTilesEdit())
					Editor.FlipHorizontalIndividualEvent(sender, null);
			}
			// Flip Vertical
			else if (isCombo(e, myKeyBinds.FlipV))
			{
				if (IsTilesEdit())
					Editor.FlipVerticalEvent(sender, null);
				else if (IsEntitiesEdit())
					Editor.FlipEntities(FlipDirection.Veritcal);
			}

			// Flip Horizontal
			else if (isCombo(e, myKeyBinds.FlipH))
			{
				if (IsTilesEdit())
					Editor.FlipHorizontalEvent(sender, null);
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

			if (!Editor.StateModel.wheelClicked)
			{
				//Turn Scroller Mode On
				Editor.StateModel.wheelClicked = true;
				scrolling = true;
				scrollingDragged = false;
				Editor.StateModel.scrollPosition = new Point(e.X - ShiftX, e.Y - ShiftY);
				if (Editor.FormsModel.vScrollBar1.IsVisible && Editor.FormsModel.hScrollBar1.IsVisible)
				{
					Editor.Cursor = System.Windows.Input.Cursors.ScrollAll;
					SetScrollerBorderApperance((int)ScrollerModeDirection.ALL);
				}
				else if (Editor.FormsModel.vScrollBar1.IsVisible)
				{
					Editor.Cursor = System.Windows.Input.Cursors.ScrollWE;
					SetScrollerBorderApperance((int)ScrollerModeDirection.WE);
				}
				else if (Editor.FormsModel.hScrollBar1.IsVisible)
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
				Editor.StateModel.wheelClicked = false;
				if (scrollingDragged)
				{
					scrolling = false;
					Editor.Cursor = System.Windows.Input.Cursors.Arrow;
					SetScrollerBorderApperance();
				}
			}

		}
		public void UpdatePositionLabel(System.Windows.Forms.MouseEventArgs e)
		{

			if (Editor.UIModes.EnablePixelCountMode == false)
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
			Editor.StateModel.scrollPosition = new Point(e.X - ShiftX, e.Y - ShiftY);
			ForceUpdateMousePos = false;
		}


		public void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!scrolling) Editor.FormsModel.GraphicPanel.Focus();

			if (e.Button == MouseButtons.Left) Left();
			else if (e.Button == MouseButtons.Right) Right();
			else if (e.Button == MouseButtons.Middle) EnforceCursorPosition();


			void Left()
			{
				if (IsEditing() && !dragged) Editing();
				InGame();

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
                        Point pC = EditorLayer.GetChunkCoordinates(p.X, p.Y);

                        if (Editor.PlaceTilesButton.IsChecked.Value)
                        {
                            int selectedIndex = Editor.TilesToolbar.ChunkList.SelectedIndex;
                            // Place Stamp
                            if (selectedIndex != -1)
                            {
                                if (!Editor.Chunks.DoesChunkMatch(pC, Editor.Chunks.StageStamps.StampList[selectedIndex], Editor.EditLayerA, Editor.EditLayerB))
                                {
                                    Editor.Chunks.PasteStamp(pC, selectedIndex, Editor.EditLayerA, Editor.EditLayerB);
                                }

                            }
                        }
                        else
                        {
                            SetClickedXY(e);
                        }
                    }
					void EntitiesEdit()
					{
						Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
						if (Editor.Entities.GetEntityAt(clicked_point)?.Selected ?? false)
						{
							// We will have to check if this dragging or clicking
							SetClickedXY(e);
						}
						else if (!ShiftPressed() && !CtrlPressed() && Editor.Entities.GetEntityAt(clicked_point) != null)
						{
							Editor.Entities.Select(clicked_point);
							Editor.UI.SetSelectOnlyButtonsState();
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
					if (Editor.InGame.PlayerSelected)
					{
						Editor.InGame.PlayerSelected = false;
						Editor.InGame.SelectedPlayer = 0;
					}
					if (Editor.InGame.CheckpointSelected)
					{
						Editor.InGame.CheckpointSelected = false;
					}
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
                    if (!Editor.EditLayerA.IsPointSelected(p)) Editor.EditLayerA?.Select(p);
                    if (Editor.EditLayerB != null && !Editor.EditLayerB.IsPointSelected(p)) Editor.EditLayerB?.Select(p);
                    Editor.DeleteSelected();
				}
				void ChunksEdit()
				{

                    Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                    Point chunk_point = EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
                    Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

                    // Remove Stamp Sized Area
                    if (!Editor.EditLayerA.DoesChunkContainASelectedTile(p)) Editor.EditLayerA?.Select(clicked_chunk);
                    if (Editor.EditLayerB != null && !Editor.EditLayerB.DoesChunkContainASelectedTile(p)) Editor.EditLayerB?.Select(clicked_chunk);
                    Editor.DeleteSelected();
                }
			}
		}
		public void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (ForceUpdateMousePos) UpdateScrollerPosition(e);
            if (scrolling || scrollingDragged || draggingSelection || dragged) Editor.FormsModel.GraphicPanel.Render();

            Common();

			if (GameRunning) InGame();
			if (ClickedX != -1) DraggingStarting();
			if (scrolling) Scroller();
			else if (e.Button == MouseButtons.Middle) EnforceCursorPosition();
			if (IsEditing()) Editing();

			if (IsChunksEdit())
			{
                lastX = e.X;
                lastY = e.Y;
            }
			else
			{
				lastX = e.X;
				lastY = e.Y;
			}


			void InGame()
			{
				if (Editor.InGame.PlayerSelected)
				{
					Editor.InGame.MovePlayer(new Point(e.X, e.Y), Zoom, Editor.InGame.SelectedPlayer);
				}

				if (Editor.InGame.CheckpointSelected)
				{
					Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					Editor.InGame.UpdateCheckpoint(clicked_point, true);
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
                        Editor.UI.UpdateEditLayerActions();

						draggingSelection = true;
						selectingX = ClickedX;
						selectingY = ClickedY;
					}
				}
				void ChunksEdit()
				{
                    // There was just a click now we can determine that this click is dragging
                    Point clicked_point = new Point((int)(ClickedX / Zoom), (int)(ClickedY / Zoom));
                    Point chunk_point = EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);

                    bool PointASelected = Editor.EditLayerA?.DoesChunkContainASelectedTile(chunk_point) ?? false;
                    bool PointBSelected = Editor.EditLayerB?.DoesChunkContainASelectedTile(chunk_point) ?? false;
                    if (PointASelected || PointBSelected)
                    {
                        // Start dragging the tiles
                        dragged = true;
                        startDragged = true;
                        Editor.EditLayerA?.StartDrag();
                        Editor.EditLayerB?.StartDrag();
                    }
                    else
                    {
                        // Start drag selection
                        if (!ShiftPressed() && !CtrlPressed())
                            Editor.Deselect();
                        Editor.UI.UpdateEditLayerActions();

                        draggingSelection = true;
                        selectingX = ClickedX;
                        selectingY = ClickedY;
                    }
				}
				void EntitiesEdit()
				{
					// There was just a click now we can determine that this click is dragging
					Point clicked_point = new Point((int)(ClickedX / Zoom), (int)(ClickedY / Zoom));
					if (Editor.Entities.GetEntityAt(clicked_point)?.Selected ?? false)
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
                
				if (Editor.StateModel.wheelClicked)
				{
					scrollingDragged = true;

				}

                double xMove = (Editor.FormsModel.hScrollBar1.IsVisible) ? e.X - ShiftX - Editor.StateModel.scrollPosition.X : 0;
                double yMove = (Editor.FormsModel.vScrollBar1.IsVisible) ? e.Y - ShiftY - Editor.StateModel.scrollPosition.Y : 0;

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
						if (Editor.FormsModel.vScrollBar1.IsVisible && Editor.FormsModel.hScrollBar1.IsVisible)
						{
							Editor.Cursor = System.Windows.Input.Cursors.ScrollAll;
							SetScrollerBorderApperance((int)ScrollerModeDirection.ALL);
						}
						else if (Editor.FormsModel.vScrollBar1.IsVisible)
						{
							Editor.Cursor = System.Windows.Input.Cursors.ScrollNS;
							SetScrollerBorderApperance((int)ScrollerModeDirection.NS);
						}
						else if (Editor.FormsModel.hScrollBar1.IsVisible)
						{
							Editor.Cursor = System.Windows.Input.Cursors.ScrollWE;
							SetScrollerBorderApperance((int)ScrollerModeDirection.WE);
						}
					}

				}

                System.Windows.Point position = new System.Windows.Point(ShiftX, ShiftY);
				double x = xMove / 10 + position.X;
                double y = yMove / 10 + position.Y;

				Editor.StateModel.CustomX += (int)xMove / 10;
				Editor.StateModel.CustomY += (int)yMove / 10;

				if (x < 0) x = 0;
				if (y < 0) y = 0;
                if (x > Editor.FormsModel.hScrollBar1.Maximum) x = Editor.FormsModel.hScrollBar1.Maximum;
				if (y > Editor.FormsModel.vScrollBar1.Maximum) y = Editor.FormsModel.vScrollBar1.Maximum;


				if (x != position.X || y != position.Y)
				{

					if (Editor.FormsModel.vScrollBar1.IsVisible)
					{
						Editor.FormsModel.vScrollBar1.Value = y;
					}
					if (Editor.FormsModel.hScrollBar1.IsVisible)
					{
						Editor.FormsModel.hScrollBar1.Value = x;
					}

					Editor.FormsModel.GraphicPanel.OnMouseMoveEventCreate();

				}
                Editor.FormsModel.GraphicPanel.Render();
			}
			void Editing()
			{
				if (IsTilesEdit() && !IsChunksEdit()) TilesEdit();
				else if (IsChunksEdit()) ChunksEdit();
				if (draggingSelection || Editor.StateModel.dragged) EdgeMove();
				if (draggingSelection) SetSelectionBounds();
				else if (Editor.StateModel.dragged) DragMoveItems();

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
								if (Editor.EditLayerA.GetTileAt(p) != Editor.TilesToolbar.SelectedTile)
								{
									Editor.EditorPlaceTile(p, Editor.TilesToolbar.SelectedTile, Editor.EditLayerA);
								}
								else if (!Editor.EditLayerA.IsPointSelected(p))
								{
									Editor.EditLayerA.Select(p);
								}
							}
						}
						else if (e.Button == MouseButtons.Right)
						{
							// Remove tile
							if (!(bool)Editor.EditLayerA?.IsPointSelected(p) || !(bool)Editor.EditLayerB?.IsPointSelected(p))
							{
								Editor.EditLayerA?.Select(p);
								Editor.EditLayerB?.Select(p);
							}
							Editor.DeleteSelected();

						}
					}
				}
				void ChunksEdit()
				{
					Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					Point pC = EditorLayer.GetChunkCoordinates(p.X, p.Y);

					if (e.Button == MouseButtons.Left)
					{
						if (Editor.PlaceTilesButton.IsChecked.Value)
						{
							int selectedIndex = Editor.TilesToolbar.ChunkList.SelectedIndex;
							// Place Stamp
							if (selectedIndex != -1)
							{
								if (!Editor.Chunks.DoesChunkMatch(pC, Editor.Chunks.StageStamps.StampList[selectedIndex], Editor.EditLayerA, Editor.EditLayerB))
								{
									Editor.Chunks.PasteStamp(pC, selectedIndex, Editor.EditLayerA, Editor.EditLayerB);
								}

							}
						}
					}

					else if (e.Button == MouseButtons.Right)
					{
						if (Editor.PlaceTilesButton.IsChecked.Value)
						{

                            if (!Editor.Chunks.IsChunkEmpty(pC, Editor.EditLayerA, Editor.EditLayerB))
                            {
                                // Remove Stamp Sized Area
                                Editor.Chunks.PasteStamp(pC, 0, Editor.EditLayerA, Editor.EditLayerB, true);
                            }
                        }

					}
				}
				void EdgeMove()
				{
					System.Windows.Point position = new System.Windows.Point(ShiftX, ShiftY); ;
					double ScreenMaxX = position.X + Editor.FormsModel.splitContainer1.Panel1.Width - (int)Editor.FormsModel.vScrollBar.ActualWidth;
                    double ScreenMaxY = position.Y + Editor.FormsModel.splitContainer1.Panel1.Height - (int)Editor.FormsModel.hScrollBar.ActualHeight;
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
					if (x > Editor.FormsModel.hScrollBar1.Maximum) x = Editor.FormsModel.hScrollBar1.Maximum;
					if (y > Editor.FormsModel.vScrollBar1.Maximum) y = Editor.FormsModel.vScrollBar1.Maximum;

					if (x != position.X || y != position.Y)
					{
						if (Editor.FormsModel.vScrollBar1.IsVisible)
						{
							Editor.FormsModel.vScrollBar1.Value = y;
						}
						if (Editor.FormsModel.hScrollBar1.IsVisible)
						{
							Editor.FormsModel.hScrollBar1.Value = x;
						}
						Editor.FormsModel.GraphicPanel.OnMouseMoveEventCreate();
                        if (!scrolling) Editor.FormsModel.GraphicPanel.Render();



					}
				}
				void SetSelectionBounds()
				{
					if (IsChunksEdit()) ChunkMode();
					else Normal();

					void ChunkMode()
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

                            Point selectStart = EditorLayer.GetChunkCoordinatesTopEdge(select_x1, select_y1);
                            Point selectEnd = EditorLayer.GetChunkCoordinatesBottomEdge(select_x2, select_y2);

                            Editor.EditLayerA?.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());
                            Editor.EditLayerB?.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());

                            Editor.UI.UpdateTilesOptions();
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

							Editor.UI.UpdateTilesOptions();

							if (IsEntitiesEdit()) Editor.Entities.TempSelection(new Rectangle(select_x1, select_y1, select_x2 - select_x1, select_y2 - select_y1), CtrlPressed());
						}
					}
				}
				void DragMoveItems()
				{
					int oldGridX = (int)((lastX / Zoom) / Editor.UIModes.MagnetSize) * Editor.UIModes.MagnetSize;
					int oldGridY = (int)((lastY / Zoom) / Editor.UIModes.MagnetSize) * Editor.UIModes.MagnetSize;
					int newGridX = (int)((e.X / Zoom) / Editor.UIModes.MagnetSize) * Editor.UIModes.MagnetSize;
					int newGridY = (int)((e.Y / Zoom) / Editor.UIModes.MagnetSize) * Editor.UIModes.MagnetSize;
					Point oldPointGrid = new Point(0, 0);
					Point newPointGrid = new Point(0, 0);
					if (Editor.UIModes.UseMagnetMode && IsEntitiesEdit())
					{
						if (Editor.UIModes.UseMagnetXAxis == true && Editor.UIModes.UseMagnetYAxis == true)
						{
							oldPointGrid = new Point(oldGridX, oldGridY);
							newPointGrid = new Point(newGridX, newGridY);
						}
						if (Editor.UIModes.UseMagnetXAxis && !Editor.UIModes.UseMagnetYAxis)
						{
							oldPointGrid = new Point(oldGridX, (int)(lastY / Zoom));
							newPointGrid = new Point(newGridX, (int)(e.Y / Zoom));
						}
						if (!Editor.UIModes.UseMagnetXAxis && Editor.UIModes.UseMagnetYAxis)
						{
							oldPointGrid = new Point((int)(lastX / Zoom), oldGridY);
							newPointGrid = new Point((int)(e.X / Zoom), newGridY);
						}
						if (!Editor.UIModes.UseMagnetXAxis && !Editor.UIModes.UseMagnetYAxis)
						{
							oldPointGrid = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
							newPointGrid = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
						}
					}
					Point oldPoint = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
					Point newPoint = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));


                    if (!IsChunksEdit())
                    {
                        Editor.EditLayerA?.MoveSelected(oldPoint, newPoint, CtrlPressed());
                        Editor.EditLayerB?.MoveSelected(oldPoint, newPoint, CtrlPressed());
                    }
                    else
                    {
                        Point oldPointAligned = EditorLayer.GetChunkCoordinatesTopEdge(oldPoint.X, oldPoint.Y);
                        Point newPointAligned = EditorLayer.GetChunkCoordinatesTopEdge(newPoint.X, newPoint.Y);
                        Editor.EditLayerA?.MoveSelected(oldPointAligned, newPointAligned, CtrlPressed(), true);
                        Editor.EditLayerB?.MoveSelected(oldPointAligned, newPointAligned, CtrlPressed(), true);
                    }


					Editor.UI.UpdateEditLayerActions();
					if (IsEntitiesEdit())
					{
						if (Editor.UIModes.UseMagnetMode)
						{
							int x = Editor.Entities.SelectedEntities[0].Entity.Position.X.High;
							int y = Editor.Entities.SelectedEntities[0].Entity.Position.Y.High;

							if (x % Editor.UIModes.MagnetSize != 0 && Editor.UIModes.UseMagnetXAxis)
							{
								int offsetX = x % Editor.UIModes.MagnetSize;
								oldPointGrid.X -= offsetX;
							}
							if (y % Editor.UIModes.MagnetSize != 0 && Editor.UIModes.UseMagnetYAxis)
							{
								int offsetY = y % Editor.UIModes.MagnetSize;
								oldPointGrid.Y -= offsetY;
							}
						}


						try
						{

							if (Editor.UIModes.UseMagnetMode)
							{
								Editor.Entities.MoveSelected(oldPointGrid, newPointGrid, CtrlPressed() && Editor.StateModel.startDragged);
							}
							else
							{
								Editor.Entities.MoveSelected(oldPoint, newPoint, CtrlPressed() && Editor.StateModel.startDragged);
							}

						}
						catch (EditorEntities.TooManyEntitiesException)
						{
							RSDKrU.MessageBox.Show("Too many entities! (limit: 2048)");
							Editor.StateModel.dragged = false;
							return;
						}
						if (Editor.UIModes.UseMagnetMode)
						{
							draggedX += newPointGrid.X - oldPointGrid.X;
							draggedY += newPointGrid.Y - oldPointGrid.Y;
						}
						else
						{
							draggedX += newPoint.X - oldPoint.X;
							draggedY += newPoint.Y - oldPoint.Y;
						}
						if (CtrlPressed() && Editor.StateModel.startDragged)
						{
							Editor.UI.UpdateEntitiesToolbarList();
							Editor.UI.SetSelectOnlyButtonsState();
						}
						Editor.EntitiesToolbar.UpdateCurrentEntityProperites();
					}
					Editor.StateModel.startDragged = false;
				}
			}
			void Common()
			{
				UpdatePositionLabel(e);
                Editor.UI.UpdateGameRunningButton(Editor.EditorScene != null);

            }
		}
		public void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) Left();
			else if (e.Button == MouseButtons.Right) Right();
			else if (e.Button == MouseButtons.Middle) Middle();



			Editor.UI.UpdateControls();

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
						if (selectingX != (int)(e.X) && selectingY != (int)(e.Y))
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
                            Point selectStart = EditorLayer.GetChunkCoordinatesTopEdge(select_x1, select_y1);
                            Point selectEnd = EditorLayer.GetChunkCoordinatesBottomEdge(select_x2, select_y2);
                            
                            Editor.EditLayerA?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                            Editor.EditLayerB?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());

							Editor.UI.SetSelectOnlyButtonsState();
							Editor.UI.UpdateEditLayerActions();

						}
						draggingSelection = false;
						Editor.EditLayerA?.EndTempSelection();
						Editor.EditLayerB?.EndTempSelection();

						if (IsEntitiesEdit()) Editor.Entities.EndTempSelection();
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

							if (IsEntitiesEdit()) Editor.Entities.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.UI.SetSelectOnlyButtonsState();
							Editor.UI.UpdateEditLayerActions();

						}
						draggingSelection = false;
						Editor.EditLayerA?.EndTempSelection();
						Editor.EditLayerB?.EndTempSelection();

						if (IsEntitiesEdit()) Editor.Entities.EndTempSelection();
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
						Editor.UI.SetSelectOnlyButtonsState();
						ClickedX = -1;
						ClickedY = -1;

						void TilesEdit()
						{
							Editor.EditLayerA?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.EditLayerB?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.UI.UpdateEditLayerActions();
						}
						void ChunksEdit()
                        {
                            Point chunk_point = EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);
                            Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

                            Editor.EditLayerA?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.EditLayerB?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.UI.UpdateEditLayerActions();
						}
						void EntitiesEdit()
						{
							Editor.Entities.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());

						}
					}
					void UpdateUndoRedo()
					{
						if (IsEntitiesEdit())
						{
							IAction action = new ActionMoveEntities(Editor.Entities.SelectedEntities.ToList(), new Point(draggedX, draggedY));
							if (Editor.Entities.LastAction != null)
							{
								// If it is move & duplicate, merge them together
								var taction = new ActionsGroup();
								taction.AddAction(Editor.Entities.LastAction);
								Editor.Entities.LastAction = null;
								taction.AddAction(action);
								taction.Close();
								action = taction;
							}
							Editor.UndoStack.Push(action);
							Editor.RedoStack.Clear();
							Editor.UI.UpdateControls();
						}
					}
				}
			}
			void Right()
			{
                if (IsEntitiesEdit())
				{
					if (Editor.Entities.SelectedEntities.Count == 2 && Editor.UIModes.RightClicktoSwapSlotID)
					{

                        Editor.Entities.SwapSlotIDsFromPair();
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
			Editor.FormsModel.GraphicPanel.Focus();
			if (CtrlPressed()) Ctrl();
			else NoCtrl();

			void Ctrl()
			{
				int maxZoom;
				int minZoom;
				if (Settings.MyPerformance.ReduceZoom)
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

				Editor.ZoomModel.SetZoomLevel(ZoomLevel, new Point(e.X - ShiftX, e.Y - ShiftY));
			}
			void NoCtrl()
			{
				if (Editor.FormsModel.vScrollBar1.IsVisible || Editor.FormsModel.hScrollBar1.IsVisible) ScrollMove();
				if (mySettings.EntityFreeCam) FreeCamScroll();

				void ScrollMove()
				{
					if (ScrollDirection == (int)ScrollDir.Y && !ScrollLocked)
					{
                        if (Editor.FormsModel.vScrollBar1.IsVisible) VScroll();
                        else HScroll();
                    }
					else if (ScrollDirection == (int)ScrollDir.X && !ScrollLocked)
					{
                        if (Editor.FormsModel.hScrollBar1.IsVisible) HScroll();
                        else VScroll();
					}
					else if (ScrollLocked)
					{
						if (ScrollDirection == (int)ScrollDir.Y)
						{
                            if (Editor.FormsModel.vScrollBar1.IsVisible) VScroll();
                            else HScroll();
                        }
						else
						{
                            if (Editor.FormsModel.hScrollBar1.IsVisible) HScroll();
                            else VScroll();
                        }

					}
				}
				void FreeCamScroll()
				{
					if (ScrollDirection == (int)ScrollDir.X) Editor.StateModel.CustomX -= e.Delta;
					else Editor.StateModel.CustomY -= e.Delta;
				}
			}

            void VScroll()
            {
                double y = Editor.FormsModel.vScrollBar1.Value - e.Delta;
                if (y < 0) y = 0;
                if (y > Editor.FormsModel.vScrollBar1.Maximum) y = Editor.FormsModel.vScrollBar1.Maximum;
                Editor.FormsModel.vScrollBar1.Value = y;
            }

            void HScroll()
            {
                double x = Editor.FormsModel.hScrollBar1.Value - e.Delta;
                if (x < 0) x = 0;
                if (x > Editor.FormsModel.hScrollBar1.Maximum) x = Editor.FormsModel.hScrollBar1.Maximum;
                Editor.FormsModel.hScrollBar1.Value = x;
            }
        }
		public void MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
            Editor.FormsModel.GraphicPanel.Focus();
            if (e.Button == MouseButtons.Right) ContextMenus();

            void ContextMenus()
            {


                if (Editor.InteractionToolButton.IsChecked.Value) InteractionTool();
                else if (IsEntitiesEdit() && (!Editor.UIModes.RightClicktoSwapSlotID || Editor.Entities.SelectedEntities.Count <= 1)) EntitiesContextMenu();
                else if (IsTilesEdit() && !Editor.PlaceTilesButton.IsChecked.Value) TilesContextMenu();

                void InteractionTool()
                {
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


                        Editor.UIModes.SelectedTileID = tile;
                        Editor.editTile0WithTileManiacToolStripMenuItem.IsEnabled = (tile < 1023);
                        Editor.moveThePlayerToHereToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.removeCheckpointToolStripMenuItem.IsEnabled = GameRunning && Editor.InGame.CheckpointEnabled;
                        Editor.assetResetToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.restartSceneToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.moveCheckpointToolStripMenuItem.IsEnabled = GameRunning && Editor.InGame.CheckpointEnabled;


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

                void EntitiesContextMenu()
                {
                    string newLine = Environment.NewLine;
                    if (Editor.Entities.GetEntityAt(clicked_point) != null)
                    {
                        var currentEntity = Editor.Entities.GetEntityAt(clicked_point);

                        Editor.EntityNameItem.Header = String.Format("Entity Name: {0}", currentEntity.Name);
                        Editor.EntitySlotIDItem.Header = String.Format("Slot ID: {0} {1} Runtime Slot ID: {2}", currentEntity.Entity.SlotID, Environment.NewLine, Editor.Entities.GetRealSlotID(currentEntity.Entity));
                        Editor.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", currentEntity.Entity.Position.X.High, currentEntity.Entity.Position.Y.High);
                    }
                    else
                    {
                        Editor.EntityNameItem.Header = String.Format("Entity Name: {0}", "N/A");
                        Editor.EntitySlotIDItem.Header = String.Format("Slot ID: {0} {1} Runtime Slot ID: {2}", "N/A", Environment.NewLine, "N/A");
                        Editor.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", e.X, e.Y);
                    }
                    System.Windows.Controls.ContextMenu info = new System.Windows.Controls.ContextMenu();


                    info.ItemsSource = Editor.EntityContext.Items;
                    info.Foreground = (System.Windows.Media.SolidColorBrush)Editor.FindResource("NormalText");
                    info.Background = (System.Windows.Media.SolidColorBrush)Editor.FindResource("NormalBackground");
                    info.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                    info.StaysOpen = false;
                    info.IsOpen = true;
                }

                void TilesContextMenu()
                {
                    string newLine = Environment.NewLine;
                    Point chunkPos = EditorLayer.GetChunkCoordinates(e.X / Zoom, e.Y / Zoom);
                    Point tilePos;
                    if (e.X == 0 || e.Y == 0) tilePos = new Point(0, 0);
                    else tilePos = new Point(e.X / 16, e.Y / 16);

                    Editor.PixelPositionMenuItem.Header = "Pixel Position:" + newLine + String.Format("X: {0}, Y: {1}", e.X, e.Y);
                    Editor.ChunkPositionMenuItem.Header = "Chunk Position:" + newLine + String.Format("X: {0}, Y: {1}", chunkPos.X, chunkPos.Y);
                    Editor.TilePositionMenuItem.Header = "Tile Position:" + newLine + String.Format("X: {0}, Y: {1}", tilePos.X, tilePos.Y);


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

                    Editor.UIModes.SelectedTileID = tile;
                    Editor.TileManiacIntergrationItem.IsEnabled = (tile < 1023);
                    Editor.TileManiacIntergrationItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);

                    System.Windows.Controls.ContextMenu info = new System.Windows.Controls.ContextMenu();
                    info.ItemsSource = Editor.TilesContext.Items;
                    info.Foreground = (System.Windows.Media.SolidColorBrush)Editor.FindResource("NormalText");
                    info.Background = (System.Windows.Media.SolidColorBrush)Editor.FindResource("NormalBackground");
                    info.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                    info.StaysOpen = false;
                    info.IsOpen = true;
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
			Editor.QuickSwapScrollDirection.InputGestureText = KeyBindPraser("ScrollLockTypeSwitch", false, true);
			Editor.swapScrollLockDirMenuToolstripButton.InputGestureText = KeyBindPraser("ScrollLockTypeSwitch", false, true);
			Editor.resetZoomLevelToolstripMenuItem.InputGestureText = KeyBindPraser("ResetZoomLevel");
			Editor.unloadSceneToolStripMenuItem.InputGestureText = KeyBindPraser("UnloadScene", false, true);
			Editor.flipVerticalIndvidualToolStripMenuItem.InputGestureText = KeyBindPraser("FlipVIndv");
			Editor.flipHorizontalIndvidualToolStripMenuItem.InputGestureText = KeyBindPraser("FlipHIndv");
			Editor.flipHorizontalToolStripMenuItem.InputGestureText = KeyBindPraser("FlipH");
			Editor.flipVerticalToolStripMenuItem.InputGestureText = KeyBindPraser("FlipV");
			Editor.pasteTochunkToolStripMenuItem.InputGestureText = KeyBindPraser("PasteToChunk", false, true);
			Editor.developerInterfaceToolStripMenuItem.InputGestureText = KeyBindPraser("DeveloperInterface", false, true);
			Editor.saveForForceOpenOnStartupToolStripMenuItem.InputGestureText = KeyBindPraser("ForceOpenOnStartup", false, true);
			Editor.copyAirToggle.InputGestureText = KeyBindPraser("CopyAirTiles", false, true);
		}

		public void UpdateTooltips()
		{
			Editor.New.ToolTip = "New Scene" + KeyBindPraser("New", true);
			Editor.Open.ToolTip = "Open Scene" + KeyBindPraser("Open", true);
			Editor.Save.ToolTip = "Save Scene" + KeyBindPraser("_Save", true);
			Editor.RunSceneButton.ToolTip = "Run Scene" + KeyBindPraser("RunScene", true, true);
			Editor.ReloadButton.ToolTip = "Reload Tiles and Sprites" + KeyBindPraser("RefreshResources", true, true);
			Editor.PointerToolButton.ToolTip = "Select/Move Tool";
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

		}

		#endregion

	}


}
