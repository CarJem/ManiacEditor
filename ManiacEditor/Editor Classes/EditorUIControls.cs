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


        [DllImport("User32.dll")]
		private static extern bool SetCursorPos(int X, int Y);

		#region Editor Definitions

		public int previousX = 0;
		public int previousY = 0;
		private int select_x1 { get => Editor.Instance.StateModel.select_x1; set => Editor.Instance.StateModel.select_x1 = value; }
		private int select_x2 { get => Editor.Instance.StateModel.select_x2; set => Editor.Instance.StateModel.select_x2 = value; }
		private int select_y1 { get => Editor.Instance.StateModel.select_y1; set => Editor.Instance.StateModel.select_y1 = value; }
		private int select_y2 { get => Editor.Instance.StateModel.select_y2; set => Editor.Instance.StateModel.select_y2 = value; }

		private int selectingY { get => Editor.Instance.StateModel.selectingY; set => Editor.Instance.StateModel.selectingY = value; }
		private int selectingX { get => Editor.Instance.StateModel.selectingX; set => Editor.Instance.StateModel.selectingX = value; }
		private int ClickedY { get => Editor.Instance.StateModel.ClickedY; set => Editor.Instance.StateModel.ClickedY = value; }
		private int ClickedX { get => Editor.Instance.StateModel.ClickedX; set => Editor.Instance.StateModel.ClickedX = value; }

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
				if (IsTilesEdit() && Editor.Instance.PlaceTilesButton.IsChecked.Value)
					Editor.Instance.TilesToolbar.SetSelectTileOption(0, false);
			}
			// Tiles Toolbar Flip Vertical
			else if (isCombo(e, myKeyBinds.FlipVTiles, true))
			{
				if (IsTilesEdit() && Editor.Instance.PlaceTilesButton.IsChecked.Value)
					Editor.Instance.TilesToolbar.SetSelectTileOption(1, false);
			}
		}

		public void GraphicPanel_OnKeyDown(object sender, KeyEventArgs e)
        {
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
                if (IsTilesEdit() && Editor.Instance.PlaceTilesButton.IsChecked.Value)
                    Editor.Instance.TilesToolbar.SetSelectTileOption(1, true);
            }
			// Tiles Toolbar Flip Horizontal
			else if (isCombo(e, myKeyBinds.FlipHTiles, true))
			{
                if (IsTilesEdit() && Editor.Instance.PlaceTilesButton.IsChecked.Value)
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

			if (!Editor.Instance.StateModel.wheelClicked)
			{
				//Turn Scroller Mode On
				Editor.Instance.StateModel.wheelClicked = true;
				scrolling = true;
				scrollingDragged = false;
				Editor.Instance.StateModel.scrollPosition = new Point(e.X - ShiftX, e.Y - ShiftY);
				if (Editor.Instance.FormsModel.vScrollBar1.IsVisible && Editor.Instance.FormsModel.hScrollBar1.IsVisible)
				{
					Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollAll;
					SetScrollerBorderApperance((int)ScrollerModeDirection.ALL);
				}
				else if (Editor.Instance.FormsModel.vScrollBar1.IsVisible)
				{
					Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollWE;
					SetScrollerBorderApperance((int)ScrollerModeDirection.WE);
				}
				else if (Editor.Instance.FormsModel.hScrollBar1.IsVisible)
				{
					Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollNS;
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
				Editor.Instance.StateModel.wheelClicked = false;
				if (scrollingDragged)
				{
					scrolling = false;
					Editor.Instance.Cursor = System.Windows.Input.Cursors.Arrow;
					SetScrollerBorderApperance();
				}
			}

		}
		public void UpdatePositionLabel(System.Windows.Forms.MouseEventArgs e)
		{

			if (Editor.Instance.UIModes.EnablePixelCountMode == false)
			{
				Editor.Instance.positionLabel.Content = "X: " + (int)(e.X / Zoom) + " Y: " + (int)(e.Y / Zoom);
			}
			else
			{
				Editor.Instance.positionLabel.Content = "X: " + (int)((e.X / Zoom) / 16) + " Y: " + (int)((e.Y / Zoom) / 16);
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

			Editor.Instance.ScrollBorderN.Fill = NotActive;
			Editor.Instance.ScrollBorderS.Fill = NotActive;
			Editor.Instance.ScrollBorderE.Fill = NotActive;
			Editor.Instance.ScrollBorderW.Fill = NotActive;
			Editor.Instance.ScrollBorderNW.Fill = NotActive;
			Editor.Instance.ScrollBorderSW.Fill = NotActive;
			Editor.Instance.ScrollBorderSE.Fill = NotActive;
			Editor.Instance.ScrollBorderNE.Fill = NotActive;

			switch (direction)
			{
				case 0:
					Editor.Instance.ScrollBorderN.Fill = Active;
					break;
				case 1:
					Editor.Instance.ScrollBorderNE.Fill = Active;
					break;
				case 2:
					Editor.Instance.ScrollBorderE.Fill = Active;
					break;
				case 3:
					Editor.Instance.ScrollBorderSE.Fill = Active;
					break;
				case 4:
					Editor.Instance.ScrollBorderS.Fill = Active;
					break;
				case 5:
					Editor.Instance.ScrollBorderSW.Fill = Active;
					break;
				case 6:
					Editor.Instance.ScrollBorderW.Fill = Active;
					break;
				case 7:
					Editor.Instance.ScrollBorderNW.Fill = Active;
					break;
				case 8:
					Editor.Instance.ScrollBorderW.Fill = Active;
					Editor.Instance.ScrollBorderE.Fill = Active;
					break;
				case 9:
					Editor.Instance.ScrollBorderN.Fill = Active;
					Editor.Instance.ScrollBorderS.Fill = Active;
					break;
				case 10:
					Editor.Instance.ScrollBorderN.Fill = Active;
					Editor.Instance.ScrollBorderS.Fill = Active;
					Editor.Instance.ScrollBorderE.Fill = Active;
					Editor.Instance.ScrollBorderW.Fill = Active;
					Editor.Instance.ScrollBorderNW.Fill = Active;
					Editor.Instance.ScrollBorderSW.Fill = Active;
					Editor.Instance.ScrollBorderSE.Fill = Active;
					Editor.Instance.ScrollBorderNE.Fill = Active;
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
				System.Windows.Point pointFromParent = Editor.Instance.ViewPanelForm.TranslatePoint(new System.Windows.Point(0, 0), Editor.Instance);
				SetCursorPos((int)(Editor.Instance.Left + pointFromParent.X) + (int)(Editor.Instance.ViewPanelForm.ActualWidth / 2), (int)(Editor.Instance.Left + pointFromParent.Y) + (int)(Editor.Instance.ViewPanelForm.ActualHeight / 2));
			}

		}

		public void UpdateScrollerPosition(System.Windows.Forms.MouseEventArgs e)
		{
			Editor.Instance.StateModel.scrollPosition = new Point(e.X - ShiftX, e.Y - ShiftY);
			ForceUpdateMousePos = false;
		}


		public void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!scrolling) Editor.Instance.FormsModel.GraphicPanel.Focus();

            if (e.Button == MouseButtons.Left) Left();
            else if (e.Button == MouseButtons.Right) Right();
            else if (e.Button == MouseButtons.Middle) Middle();



            void Left()
			{
				if (IsEditing() && !dragged) Editing();
				InGame();

				void Editing()
				{
					if (IsTilesEdit() && !Editor.Instance.InteractionToolButton.IsChecked.Value && !IsChunksEdit()) TilesEdit();
					if (IsChunksEdit() && IsSceneLoaded()) ChunksEdit();
					else if (IsEntitiesEdit()) EntitiesEdit();

					void TilesEdit()
					{
						if (Editor.Instance.PlaceTilesButton.IsChecked.Value)
						{
							// Place tile
							if (Editor.Instance.TilesToolbar.SelectedTile != -1)
							{
								Editor.Instance.EditorPlaceTile(new Point((int)(e.X / Zoom), (int)(e.Y / Zoom)), Editor.Instance.TilesToolbar.SelectedTile, Editor.Instance.EditLayerA);
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

                        if (Editor.Instance.PlaceTilesButton.IsChecked.Value)
                        {
                            int selectedIndex = Editor.Instance.TilesToolbar.ChunkList.SelectedIndex;
                            // Place Stamp
                            if (selectedIndex != -1)
                            {
                                if (!Editor.Instance.Chunks.DoesChunkMatch(pC, Editor.Instance.Chunks.StageStamps.StampList[selectedIndex], Editor.Instance.EditLayerA, Editor.Instance.EditLayerB))
                                {
                                    Editor.Instance.Chunks.PasteStamp(pC, selectedIndex, Editor.Instance.EditLayerA, Editor.Instance.EditLayerB);
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
						if (Editor.Instance.Entities.GetEntityAt(clicked_point)?.Selected ?? false)
						{
							// We will have to check if this dragging or clicking
							SetClickedXY(e);
						}
						else if (!ShiftPressed() && !CtrlPressed() && Editor.Instance.Entities.GetEntityAt(clicked_point) != null)
						{
							Editor.Instance.Entities.Select(clicked_point);
							Editor.Instance.UI.SetSelectOnlyButtonsState();
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
					if (Editor.Instance.InGame.PlayerSelected)
					{
						Editor.Instance.InGame.PlayerSelected = false;
						Editor.Instance.InGame.SelectedPlayer = 0;
					}
					if (Editor.Instance.InGame.CheckpointSelected)
					{
						Editor.Instance.InGame.CheckpointSelected = false;
					}
				}
			}
			void Right()
			{
				if (IsTilesEdit() && Editor.Instance.PlaceTilesButton.IsChecked.Value && !IsChunksEdit()) TilesEdit();
				else if (IsChunksEdit() && Editor.Instance.PlaceTilesButton.IsChecked.Value) ChunksEdit();
				void TilesEdit()
				{
					// Remove tile
					Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                    if (!Editor.Instance.EditLayerA.IsPointSelected(p)) Editor.Instance.EditLayerA?.Select(p);
                    if (Editor.Instance.EditLayerB != null && !Editor.Instance.EditLayerB.IsPointSelected(p)) Editor.Instance.EditLayerB?.Select(p);
                    Editor.Instance.DeleteSelected();
				}
				void ChunksEdit()
				{

                    Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                    Point chunk_point = EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
                    Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

                    // Remove Stamp Sized Area
                    if (!Editor.Instance.EditLayerA.DoesChunkContainASelectedTile(p)) Editor.Instance.EditLayerA?.Select(clicked_chunk);
                    if (Editor.Instance.EditLayerB != null && !Editor.Instance.EditLayerB.DoesChunkContainASelectedTile(p)) Editor.Instance.EditLayerB?.Select(clicked_chunk);
                    Editor.Instance.DeleteSelected();
                }
			}
            void Middle()
            {
                EnforceCursorPosition();
                ToggleScrollerMode(e);
            }
        }
		public void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (ForceUpdateMousePos) UpdateScrollerPosition(e);
            if (scrolling || scrollingDragged || draggingSelection || dragged) Editor.Instance.FormsModel.GraphicPanel.Render();

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
				if (Editor.Instance.InGame.PlayerSelected)
				{
					Editor.Instance.InGame.MovePlayer(new Point(e.X, e.Y), Zoom, Editor.Instance.InGame.SelectedPlayer);
				}

				if (Editor.Instance.InGame.CheckpointSelected)
				{
					Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					Editor.Instance.InGame.UpdateCheckpoint(clicked_point, true);
				}
			}
			void DraggingStarting()
			{
				if (IsTilesEdit() && !Editor.Instance.InteractionToolButton.IsChecked.Value && !IsChunksEdit())
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
					bool PointASelected = Editor.Instance.EditLayerA?.IsPointSelected(clicked_point) ?? false;
					bool PointBSelected = Editor.Instance.EditLayerB?.IsPointSelected(clicked_point) ?? false;
					if (PointASelected || PointBSelected)
					{
						// Start dragging the tiles
						dragged = true;
						startDragged = true;
						Editor.Instance.EditLayerA?.StartDrag();
						Editor.Instance.EditLayerB?.StartDrag();
					}

					else if (!Editor.Instance.SelectToolButton.IsChecked.Value && !ShiftPressed() && !CtrlPressed() && (Editor.Instance.EditLayerA?.HasTileAt(clicked_point) ?? false) || (Editor.Instance.EditLayerB?.HasTileAt(clicked_point) ?? false))
					{
						// Start dragging the single selected tile
						Editor.Instance.EditLayerA?.Select(clicked_point);
						Editor.Instance.EditLayerB?.Select(clicked_point);
						dragged = true;
						startDragged = true;
						Editor.Instance.EditLayerA?.StartDrag();
						Editor.Instance.EditLayerB?.StartDrag();
					}

					else
					{
						// Start drag selection
						//EditLayer.Select(clicked_point, ShiftPressed || CtrlPressed, CtrlPressed);
						if (!ShiftPressed() && !CtrlPressed())
							Editor.Instance.Deselect();
                        Editor.Instance.UI.UpdateEditLayerActions();

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

                    bool PointASelected = Editor.Instance.EditLayerA?.DoesChunkContainASelectedTile(chunk_point) ?? false;
                    bool PointBSelected = Editor.Instance.EditLayerB?.DoesChunkContainASelectedTile(chunk_point) ?? false;
                    if (PointASelected || PointBSelected)
                    {
                        // Start dragging the tiles
                        dragged = true;
                        startDragged = true;
                        Editor.Instance.EditLayerA?.StartDrag();
                        Editor.Instance.EditLayerB?.StartDrag();
                    }
                    else
                    {
                        // Start drag selection
                        if (!ShiftPressed() && !CtrlPressed())
                            Editor.Instance.Deselect();
                        Editor.Instance.UI.UpdateEditLayerActions();

                        draggingSelection = true;
                        selectingX = ClickedX;
                        selectingY = ClickedY;
                    }
				}
				void EntitiesEdit()
				{
					// There was just a click now we can determine that this click is dragging
					Point clicked_point = new Point((int)(ClickedX / Zoom), (int)(ClickedY / Zoom));
					if (Editor.Instance.Entities.GetEntityAt(clicked_point)?.Selected ?? false)
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
							Editor.Instance.Deselect();
						draggingSelection = true;
						selectingX = ClickedX;
						selectingY = ClickedY;

					}
				}
			}
			void Scroller()
			{
                
				if (Editor.Instance.StateModel.wheelClicked)
				{
					scrollingDragged = true;

				}

                double xMove = (Editor.Instance.FormsModel.hScrollBar1.IsVisible) ? e.X - ShiftX - Editor.Instance.StateModel.scrollPosition.X : 0;
                double yMove = (Editor.Instance.FormsModel.vScrollBar1.IsVisible) ? e.Y - ShiftY - Editor.Instance.StateModel.scrollPosition.Y : 0;

				if (Math.Abs(xMove) < 15) xMove = 0;
				if (Math.Abs(yMove) < 15) yMove = 0;

				if (xMove > 0)
				{
					if (yMove > 0)
					{
						Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollSE;
						SetScrollerBorderApperance((int)ScrollerModeDirection.SE);
					}
					else if (yMove < 0)
					{
						Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollNE;
						SetScrollerBorderApperance((int)ScrollerModeDirection.NE);
					}
					else
					{
						Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollE;
						SetScrollerBorderApperance((int)ScrollerModeDirection.E);
					}

				}
				else if (xMove < 0)
				{
					if (yMove > 0)
					{
						Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollSW;
						SetScrollerBorderApperance((int)ScrollerModeDirection.SW);
					}
					else if (yMove < 0)
					{
						Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollNW;
						SetScrollerBorderApperance((int)ScrollerModeDirection.NW);
					}
					else
					{
						Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollW;
						SetScrollerBorderApperance((int)ScrollerModeDirection.W);
					}

				}
				else
				{

					if (yMove > 0)
					{
						Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollS;
						SetScrollerBorderApperance((int)ScrollerModeDirection.S);
					}
					else if (yMove < 0)
					{
						Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollN;
						SetScrollerBorderApperance((int)ScrollerModeDirection.N);
					}
					else
					{
						if (Editor.Instance.FormsModel.vScrollBar1.IsVisible && Editor.Instance.FormsModel.hScrollBar1.IsVisible)
						{
							Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollAll;
							SetScrollerBorderApperance((int)ScrollerModeDirection.ALL);
						}
						else if (Editor.Instance.FormsModel.vScrollBar1.IsVisible)
						{
							Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollNS;
							SetScrollerBorderApperance((int)ScrollerModeDirection.NS);
						}
						else if (Editor.Instance.FormsModel.hScrollBar1.IsVisible)
						{
							Editor.Instance.Cursor = System.Windows.Input.Cursors.ScrollWE;
							SetScrollerBorderApperance((int)ScrollerModeDirection.WE);
						}
					}

				}

                System.Windows.Point position = new System.Windows.Point(ShiftX, ShiftY);
				double x = xMove / 10 + position.X;
                double y = yMove / 10 + position.Y;

				Editor.Instance.StateModel.CustomX += (int)xMove / 10;
				Editor.Instance.StateModel.CustomY += (int)yMove / 10;

				if (x < 0) x = 0;
				if (y < 0) y = 0;
                if (x > Editor.Instance.FormsModel.hScrollBar1.Maximum) x = Editor.Instance.FormsModel.hScrollBar1.Maximum;
				if (y > Editor.Instance.FormsModel.vScrollBar1.Maximum) y = Editor.Instance.FormsModel.vScrollBar1.Maximum;


				if (x != position.X || y != position.Y)
				{

					if (Editor.Instance.FormsModel.vScrollBar1.IsVisible)
					{
						Editor.Instance.FormsModel.vScrollBar1.Value = y;
					}
					if (Editor.Instance.FormsModel.hScrollBar1.IsVisible)
					{
						Editor.Instance.FormsModel.hScrollBar1.Value = x;
					}

					Editor.Instance.FormsModel.GraphicPanel.OnMouseMoveEventCreate();

				}
                Editor.Instance.FormsModel.GraphicPanel.Render();
			}
			void Editing()
			{
				if (IsTilesEdit() && !IsChunksEdit()) TilesEdit();
				else if (IsChunksEdit()) ChunksEdit();
				if (draggingSelection || Editor.Instance.StateModel.dragged) EdgeMove();
				if (draggingSelection) SetSelectionBounds();
				else if (Editor.Instance.StateModel.dragged) DragMoveItems();

				void TilesEdit()
				{
					Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					if (Editor.Instance.PlaceTilesButton.IsChecked.Value)
					{
						if (e.Button == MouseButtons.Left)
						{
							// Place tile
							if (Editor.Instance.TilesToolbar.SelectedTile != -1)
							{
								if (Editor.Instance.EditLayerA.GetTileAt(p) != Editor.Instance.TilesToolbar.SelectedTile)
								{
									Editor.Instance.EditorPlaceTile(p, Editor.Instance.TilesToolbar.SelectedTile, Editor.Instance.EditLayerA);
								}
								else if (!Editor.Instance.EditLayerA.IsPointSelected(p))
								{
									Editor.Instance.EditLayerA.Select(p);
								}
							}
						}
						else if (e.Button == MouseButtons.Right)
						{
							// Remove tile
							if (!(bool)Editor.Instance.EditLayerA?.IsPointSelected(p) || !(bool)Editor.Instance.EditLayerB?.IsPointSelected(p))
							{
								Editor.Instance.EditLayerA?.Select(p);
								Editor.Instance.EditLayerB?.Select(p);
							}
							Editor.Instance.DeleteSelected();

						}
					}
				}
				void ChunksEdit()
				{
					Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
					Point pC = EditorLayer.GetChunkCoordinates(p.X, p.Y);

					if (e.Button == MouseButtons.Left)
					{
						if (Editor.Instance.PlaceTilesButton.IsChecked.Value)
						{
							int selectedIndex = Editor.Instance.TilesToolbar.ChunkList.SelectedIndex;
							// Place Stamp
							if (selectedIndex != -1)
							{
								if (!Editor.Instance.Chunks.DoesChunkMatch(pC, Editor.Instance.Chunks.StageStamps.StampList[selectedIndex], Editor.Instance.EditLayerA, Editor.Instance.EditLayerB))
								{
									Editor.Instance.Chunks.PasteStamp(pC, selectedIndex, Editor.Instance.EditLayerA, Editor.Instance.EditLayerB);
								}

							}
						}
					}

					else if (e.Button == MouseButtons.Right)
					{
						if (Editor.Instance.PlaceTilesButton.IsChecked.Value)
						{

                            if (!Editor.Instance.Chunks.IsChunkEmpty(pC, Editor.Instance.EditLayerA, Editor.Instance.EditLayerB))
                            {
                                // Remove Stamp Sized Area
                                Editor.Instance.Chunks.PasteStamp(pC, 0, Editor.Instance.EditLayerA, Editor.Instance.EditLayerB, true);
                            }
                        }

					}
				}
				void EdgeMove()
				{
					System.Windows.Point position = new System.Windows.Point(ShiftX, ShiftY); ;
					double ScreenMaxX = position.X + Editor.Instance.FormsModel.splitContainer1.Panel1.Width - (int)Editor.Instance.FormsModel.vScrollBar.ActualWidth;
                    double ScreenMaxY = position.Y + Editor.Instance.FormsModel.splitContainer1.Panel1.Height - (int)Editor.Instance.FormsModel.hScrollBar.ActualHeight;
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
					if (x > Editor.Instance.FormsModel.hScrollBar1.Maximum) x = Editor.Instance.FormsModel.hScrollBar1.Maximum;
					if (y > Editor.Instance.FormsModel.vScrollBar1.Maximum) y = Editor.Instance.FormsModel.vScrollBar1.Maximum;

					if (x != position.X || y != position.Y)
					{
						if (Editor.Instance.FormsModel.vScrollBar1.IsVisible)
						{
							Editor.Instance.FormsModel.vScrollBar1.Value = y;
						}
						if (Editor.Instance.FormsModel.hScrollBar1.IsVisible)
						{
							Editor.Instance.FormsModel.hScrollBar1.Value = x;
						}
						Editor.Instance.FormsModel.GraphicPanel.OnMouseMoveEventCreate();
                        if (!scrolling) Editor.Instance.FormsModel.GraphicPanel.Render();



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

                            Editor.Instance.EditLayerA?.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());
                            Editor.Instance.EditLayerB?.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());

                            Editor.Instance.UI.UpdateTilesOptions();
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
							Editor.Instance.EditLayerA?.TempSelection(new Rectangle(select_x1, select_y1, select_x2 - select_x1, select_y2 - select_y1), CtrlPressed());
							Editor.Instance.EditLayerB?.TempSelection(new Rectangle(select_x1, select_y1, select_x2 - select_x1, select_y2 - select_y1), CtrlPressed());

							Editor.Instance.UI.UpdateTilesOptions();

							if (IsEntitiesEdit()) Editor.Instance.Entities.TempSelection(new Rectangle(select_x1, select_y1, select_x2 - select_x1, select_y2 - select_y1), CtrlPressed());
						}
					}
				}
				void DragMoveItems()
				{
					int oldGridX = (int)((lastX / Zoom) / Editor.Instance.UIModes.MagnetSize) * Editor.Instance.UIModes.MagnetSize;
					int oldGridY = (int)((lastY / Zoom) / Editor.Instance.UIModes.MagnetSize) * Editor.Instance.UIModes.MagnetSize;
					int newGridX = (int)((e.X / Zoom) / Editor.Instance.UIModes.MagnetSize) * Editor.Instance.UIModes.MagnetSize;
					int newGridY = (int)((e.Y / Zoom) / Editor.Instance.UIModes.MagnetSize) * Editor.Instance.UIModes.MagnetSize;
					Point oldPointGrid = new Point(0, 0);
					Point newPointGrid = new Point(0, 0);
					if (Editor.Instance.UIModes.UseMagnetMode && IsEntitiesEdit())
					{
						if (Editor.Instance.UIModes.UseMagnetXAxis == true && Editor.Instance.UIModes.UseMagnetYAxis == true)
						{
							oldPointGrid = new Point(oldGridX, oldGridY);
							newPointGrid = new Point(newGridX, newGridY);
						}
						if (Editor.Instance.UIModes.UseMagnetXAxis && !Editor.Instance.UIModes.UseMagnetYAxis)
						{
							oldPointGrid = new Point(oldGridX, (int)(lastY / Zoom));
							newPointGrid = new Point(newGridX, (int)(e.Y / Zoom));
						}
						if (!Editor.Instance.UIModes.UseMagnetXAxis && Editor.Instance.UIModes.UseMagnetYAxis)
						{
							oldPointGrid = new Point((int)(lastX / Zoom), oldGridY);
							newPointGrid = new Point((int)(e.X / Zoom), newGridY);
						}
						if (!Editor.Instance.UIModes.UseMagnetXAxis && !Editor.Instance.UIModes.UseMagnetYAxis)
						{
							oldPointGrid = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
							newPointGrid = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
						}
					}
					Point oldPoint = new Point((int)(lastX / Zoom), (int)(lastY / Zoom));
					Point newPoint = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));


                    if (!IsChunksEdit())
                    {
                        Editor.Instance.EditLayerA?.MoveSelected(oldPoint, newPoint, CtrlPressed());
                        Editor.Instance.EditLayerB?.MoveSelected(oldPoint, newPoint, CtrlPressed());
                    }
                    else
                    {
                        Point oldPointAligned = EditorLayer.GetChunkCoordinatesTopEdge(oldPoint.X, oldPoint.Y);
                        Point newPointAligned = EditorLayer.GetChunkCoordinatesTopEdge(newPoint.X, newPoint.Y);
                        Editor.Instance.EditLayerA?.MoveSelected(oldPointAligned, newPointAligned, CtrlPressed(), true);
                        Editor.Instance.EditLayerB?.MoveSelected(oldPointAligned, newPointAligned, CtrlPressed(), true);
                    }


					Editor.Instance.UI.UpdateEditLayerActions();
					if (IsEntitiesEdit())
					{
						if (Editor.Instance.UIModes.UseMagnetMode)
						{
							int x = Editor.Instance.Entities.SelectedEntities[0].Entity.Position.X.High;
							int y = Editor.Instance.Entities.SelectedEntities[0].Entity.Position.Y.High;

							if (x % Editor.Instance.UIModes.MagnetSize != 0 && Editor.Instance.UIModes.UseMagnetXAxis)
							{
								int offsetX = x % Editor.Instance.UIModes.MagnetSize;
								oldPointGrid.X -= offsetX;
							}
							if (y % Editor.Instance.UIModes.MagnetSize != 0 && Editor.Instance.UIModes.UseMagnetYAxis)
							{
								int offsetY = y % Editor.Instance.UIModes.MagnetSize;
								oldPointGrid.Y -= offsetY;
							}
						}


						try
						{

							if (Editor.Instance.UIModes.UseMagnetMode)
							{
								Editor.Instance.Entities.MoveSelected(oldPointGrid, newPointGrid, CtrlPressed() && Editor.Instance.StateModel.startDragged);
							}
							else
							{
								Editor.Instance.Entities.MoveSelected(oldPoint, newPoint, CtrlPressed() && Editor.Instance.StateModel.startDragged);
							}

						}
						catch (EditorEntities.TooManyEntitiesException)
						{
							RSDKrU.MessageBox.Show("Too many entities! (limit: 2048)");
							Editor.Instance.StateModel.dragged = false;
							return;
						}
						if (Editor.Instance.UIModes.UseMagnetMode)
						{
							draggedX += newPointGrid.X - oldPointGrid.X;
							draggedY += newPointGrid.Y - oldPointGrid.Y;
						}
						else
						{
							draggedX += newPoint.X - oldPoint.X;
							draggedY += newPoint.Y - oldPoint.Y;
						}
						if (CtrlPressed() && Editor.Instance.StateModel.startDragged)
						{
							Editor.Instance.UI.UpdateEntitiesToolbarList();
							Editor.Instance.UI.SetSelectOnlyButtonsState();
						}
						Editor.Instance.EntitiesToolbar.UpdateCurrentEntityProperites();
					}
					Editor.Instance.StateModel.startDragged = false;
				}
			}
			void Common()
			{
				UpdatePositionLabel(e);
                Editor.Instance.UI.UpdateGameRunningButton(Editor.Instance.EditorScene != null);

            }
		}
		public void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) Left();
			else if (e.Button == MouseButtons.Right) Right();
			else if (e.Button == MouseButtons.Middle) Middle();



			Editor.Instance.UI.UpdateControls();

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
                            
                            Editor.Instance.EditLayerA?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                            Editor.Instance.EditLayerB?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());

							Editor.Instance.UI.SetSelectOnlyButtonsState();
							Editor.Instance.UI.UpdateEditLayerActions();

						}
						draggingSelection = false;
						Editor.Instance.EditLayerA?.EndTempSelection();
						Editor.Instance.EditLayerB?.EndTempSelection();

						if (IsEntitiesEdit()) Editor.Instance.Entities.EndTempSelection();
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
							Editor.Instance.EditLayerA?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.Instance.EditLayerB?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());

							if (IsEntitiesEdit()) Editor.Instance.Entities.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.Instance.UI.SetSelectOnlyButtonsState();
							Editor.Instance.UI.UpdateEditLayerActions();

						}
						draggingSelection = false;
						Editor.Instance.EditLayerA?.EndTempSelection();
						Editor.Instance.EditLayerB?.EndTempSelection();

						if (IsEntitiesEdit()) Editor.Instance.Entities.EndTempSelection();
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
						Editor.Instance.UI.SetSelectOnlyButtonsState();
						ClickedX = -1;
						ClickedY = -1;

						void TilesEdit()
						{
							Editor.Instance.EditLayerA?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.Instance.EditLayerB?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.Instance.UI.UpdateEditLayerActions();
						}
						void ChunksEdit()
                        {
                            Point chunk_point = EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);
                            Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

                            Editor.Instance.EditLayerA?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.Instance.EditLayerB?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
							Editor.Instance.UI.UpdateEditLayerActions();
						}
						void EntitiesEdit()
						{
							Editor.Instance.Entities.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());

						}
					}
					void UpdateUndoRedo()
					{
						if (IsEntitiesEdit())
						{
							IAction action = new ActionMoveEntities(Editor.Instance.Entities.SelectedEntities.ToList(), new Point(draggedX, draggedY));
							if (Editor.Instance.Entities.LastAction != null)
							{
								// If it is move & duplicate, merge them together
								var taction = new ActionsGroup();
								taction.AddAction(Editor.Instance.Entities.LastAction);
								Editor.Instance.Entities.LastAction = null;
								taction.AddAction(action);
								taction.Close();
								action = taction;
							}
							Editor.Instance.UndoStack.Push(action);
							Editor.Instance.RedoStack.Clear();
							Editor.Instance.UI.UpdateControls();
						}
					}
				}
			}
			void Right()
			{
                if (IsEntitiesEdit())
				{
					if (Editor.Instance.Entities.SelectedEntities.Count == 2 && Editor.Instance.UIModes.RightClicktoSwapSlotID)
					{

                        Editor.Instance.Entities.SwapSlotIDsFromPair();
					}
				}

			}
			void Middle()
			{
				if (Settings.MySettings.ScrollerPressReleaseMode) ToggleScrollerMode(e);
			}
		}
		public void MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Editor.Instance.FormsModel.GraphicPanel.Focus();
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

				Editor.Instance.ZoomModel.SetZoomLevel(ZoomLevel, new Point(e.X - ShiftX, e.Y - ShiftY));
			}
			void NoCtrl()
			{
				if (Editor.Instance.FormsModel.vScrollBar1.IsVisible || Editor.Instance.FormsModel.hScrollBar1.IsVisible) ScrollMove();
				if (mySettings.EntityFreeCam) FreeCamScroll();

				void ScrollMove()
				{
					if (ScrollDirection == (int)ScrollDir.Y && !ScrollLocked)
					{
                        if (Editor.Instance.FormsModel.vScrollBar1.IsVisible) VScroll();
                        else HScroll();
                    }
					else if (ScrollDirection == (int)ScrollDir.X && !ScrollLocked)
					{
                        if (Editor.Instance.FormsModel.hScrollBar1.IsVisible) HScroll();
                        else VScroll();
					}
					else if (ScrollLocked)
					{
						if (ScrollDirection == (int)ScrollDir.Y)
						{
                            if (Editor.Instance.FormsModel.vScrollBar1.IsVisible) VScroll();
                            else HScroll();
                        }
						else
						{
                            if (Editor.Instance.FormsModel.hScrollBar1.IsVisible) HScroll();
                            else VScroll();
                        }

					}
				}
				void FreeCamScroll()
				{
					if (ScrollDirection == (int)ScrollDir.X) Editor.Instance.StateModel.CustomX -= e.Delta;
					else Editor.Instance.StateModel.CustomY -= e.Delta;
				}
			}

            void VScroll()
            {
                double y = Editor.Instance.FormsModel.vScrollBar1.Value - e.Delta;
                if (y < 0) y = 0;
                if (y > Editor.Instance.FormsModel.vScrollBar1.Maximum) y = Editor.Instance.FormsModel.vScrollBar1.Maximum;
                Editor.Instance.FormsModel.vScrollBar1.Value = y;
            }

            void HScroll()
            {
                double x = Editor.Instance.FormsModel.hScrollBar1.Value - e.Delta;
                if (x < 0) x = 0;
                if (x > Editor.Instance.FormsModel.hScrollBar1.Maximum) x = Editor.Instance.FormsModel.hScrollBar1.Maximum;
                Editor.Instance.FormsModel.hScrollBar1.Value = x;
            }
        }
		public void MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
            Editor.Instance.FormsModel.GraphicPanel.Focus();
            if (e.Button == MouseButtons.Right) ContextMenus();

            void ContextMenus()
            {


                if (Editor.Instance.InteractionToolButton.IsChecked.Value) InteractionTool();
                else if (IsEntitiesEdit() && (!Editor.Instance.UIModes.RightClicktoSwapSlotID || Editor.Instance.Entities.SelectedEntities.Count <= 1)) EntitiesContextMenu();
                else if (IsTilesEdit() && !Editor.Instance.PlaceTilesButton.IsChecked.Value) TilesContextMenu();

                void InteractionTool()
                {
                    if (IsTilesEdit())
                    {
                        Point clicked_point_tile = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                        int tile;
                        int tileA = (ushort)(Editor.Instance.EditLayerA?.GetTileAt(clicked_point_tile) & 0x3ff);
                        int tileB = 0;
                        if (Editor.Instance.EditLayerB != null)
                        {
                            tileB = (ushort)(Editor.Instance.EditLayerB?.GetTileAt(clicked_point_tile) & 0x3ff);
                            if (tileA > 1023 && tileB < 1023) tile = tileB;
                            else tile = tileA;
                        }
                        else tile = tileA;


                        Editor.Instance.UIModes.SelectedTileID = tile;
                        Editor.Instance.editTile0WithTileManiacToolStripMenuItem.IsEnabled = (tile < 1023);
                        Editor.Instance.moveThePlayerToHereToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.Instance.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.Instance.removeCheckpointToolStripMenuItem.IsEnabled = GameRunning && Editor.Instance.InGame.CheckpointEnabled;
                        Editor.Instance.assetResetToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.Instance.restartSceneToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.Instance.moveCheckpointToolStripMenuItem.IsEnabled = GameRunning && Editor.Instance.InGame.CheckpointEnabled;


                        Editor.Instance.editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);
                        Editor.Instance.ViewPanelContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                        Editor.Instance.ViewPanelContextMenu.IsOpen = true;
                    }
                    else
                    {
                        Point clicked_point_tile = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                        string tile = "N/A";
                        Editor.Instance.editTile0WithTileManiacToolStripMenuItem.IsEnabled = false;
                        Editor.Instance.moveThePlayerToHereToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.Instance.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.Instance.moveThisPlayerToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.Instance.moveCheckpointToolStripMenuItem.IsEnabled = GameRunning;

                        Editor.Instance.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.Instance.removeCheckpointToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.Instance.assetResetToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.Instance.restartSceneToolStripMenuItem.IsEnabled = GameRunning;
                        Editor.Instance.moveCheckpointToolStripMenuItem.IsEnabled = GameRunning;

                        Editor.Instance.editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);
                        Editor.Instance.ViewPanelContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                        Editor.Instance.ViewPanelContextMenu.IsOpen = true;
                    }
                }

                void EntitiesContextMenu()
                {
                    string newLine = Environment.NewLine;
                    if (Editor.Instance.Entities.GetEntityAt(clicked_point) != null)
                    {
                        var currentEntity = Editor.Instance.Entities.GetEntityAt(clicked_point);

                        Editor.Instance.EntityNameItem.Header = String.Format("Entity Name: {0}", currentEntity.Name);
                        Editor.Instance.EntitySlotIDItem.Header = String.Format("Slot ID: {0} {1} Runtime Slot ID: {2}", currentEntity.Entity.SlotID, Environment.NewLine, Editor.Instance.Entities.GetRealSlotID(currentEntity.Entity));
                        Editor.Instance.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", currentEntity.Entity.Position.X.High, currentEntity.Entity.Position.Y.High);
                    }
                    else
                    {
                        Editor.Instance.EntityNameItem.Header = String.Format("Entity Name: {0}", "N/A");
                        Editor.Instance.EntitySlotIDItem.Header = String.Format("Slot ID: {0} {1} Runtime Slot ID: {2}", "N/A", Environment.NewLine, "N/A");
                        Editor.Instance.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", e.X, e.Y);
                    }
                    System.Windows.Controls.ContextMenu info = new System.Windows.Controls.ContextMenu();


                    info.ItemsSource = Editor.Instance.EntityContext.Items;
                    info.Foreground = (System.Windows.Media.SolidColorBrush)Editor.Instance.FindResource("NormalText");
                    info.Background = (System.Windows.Media.SolidColorBrush)Editor.Instance.FindResource("NormalBackground");
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

                    Editor.Instance.PixelPositionMenuItem.Header = "Pixel Position:" + newLine + String.Format("X: {0}, Y: {1}", e.X, e.Y);
                    Editor.Instance.ChunkPositionMenuItem.Header = "Chunk Position:" + newLine + String.Format("X: {0}, Y: {1}", chunkPos.X, chunkPos.Y);
                    Editor.Instance.TilePositionMenuItem.Header = "Tile Position:" + newLine + String.Format("X: {0}, Y: {1}", tilePos.X, tilePos.Y);


                    Point clicked_point_tile = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                    int tile;
                    int tileA = (ushort)(Editor.Instance.EditLayerA?.GetTileAt(clicked_point_tile) & 0x3ff);
                    int tileB = 0;
                    if (Editor.Instance.EditLayerB != null)
                    {
                        tileB = (ushort)(Editor.Instance.EditLayerB?.GetTileAt(clicked_point_tile) & 0x3ff);
                        if (tileA > 1023 && tileB < 1023) tile = tileB;
                        else tile = tileA;
                    }
                    else tile = tileA;

                    Editor.Instance.UIModes.SelectedTileID = tile;
                    Editor.Instance.TileManiacIntergrationItem.IsEnabled = (tile < 1023);
                    Editor.Instance.TileManiacIntergrationItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);

                    System.Windows.Controls.ContextMenu info = new System.Windows.Controls.ContextMenu();
                    info.ItemsSource = Editor.Instance.TilesContext.Items;
                    info.Foreground = (System.Windows.Media.SolidColorBrush)Editor.Instance.FindResource("NormalText");
                    info.Background = (System.Windows.Media.SolidColorBrush)Editor.Instance.FindResource("NormalBackground");
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
			Editor.Instance.copyAirToggle.InputGestureText = KeyBindPraser("CopyAirTiles", false, true);
		}

		public void UpdateTooltips()
		{
			Editor.Instance.New.ToolTip = "New Scene" + KeyBindPraser("New", true);
			Editor.Instance.Open.ToolTip = "Open Scene" + KeyBindPraser("Open", true);
			Editor.Instance.Save.ToolTip = "Save Scene" + KeyBindPraser("_Save", true);
			Editor.Instance.RunSceneButton.ToolTip = "Run Scene" + KeyBindPraser("RunScene", true, true);
			Editor.Instance.ReloadButton.ToolTip = "Reload Tiles and Sprites" + KeyBindPraser("RefreshResources", true, true);
			Editor.Instance.PointerToolButton.ToolTip = "Select/Move Tool";
			Editor.Instance.MagnetMode.ToolTip = "Magnet Mode";
			Editor.Instance.positionLabel.ToolTip = "The position relative to your mouse (Pixels Only for Now)";
			Editor.Instance.selectionSizeLabel.ToolTip = "The Size of the Selection";
			Editor.Instance.selectedPositionLabel.ToolTip = "The Position of the Selected Tile";
			Editor.Instance.selectionBoxSizeLabel.ToolTip = "The Size of the Selection Box";
			Editor.Instance.pixelModeButton.ToolTip = "Change the Positional/Selection Values to Pixel or Tile Based Values";
			Editor.Instance.nudgeFasterButton.ToolTip = "Move entities/tiles in a larger increment. (Configurable in Options)\r\nShortcut Key: " + KeyBindPraser("NudgeFaster");
			Editor.Instance.scrollLockButton.ToolTip = "Prevent the Mouse Wheel from Scrolling with the vertical scroll bar\r\nShortcut Key: " + KeyBindPraser("ScrollLock");
			Editor.Instance.ZoomInButton.ToolTip = "Zoom In (Ctrl + Wheel Up)";
			Editor.Instance.ZoomOutButton.ToolTip = "Zoom In (Ctrl + Wheel Down)";
			Editor.Instance.FreezeDeviceButton.ToolTip = "Freeze Device";
			Editor.Instance.SelectToolButton.ToolTip = "Selection Tool (To select groups of tiles and not dragged the clicked tile)";
			Editor.Instance.PlaceTilesButton.ToolTip = "Place tiles (Right click [+drag] - place, Left click [+drag] - delete)";
			Editor.Instance.InteractionToolButton.ToolTip = "Interaction Tool";
			Editor.Instance.ShowCollisionAButton.ToolTip = "Show Collision Layer A" + KeyBindPraser("ShowPathA", true, true);
			Editor.Instance.ShowCollisionBButton.ToolTip = "Show Collision Layer B" + KeyBindPraser("ShowPathB", true, true);
			Editor.Instance.FlipAssistButton.ToolTip = "Show Flipped Tile Helper";
			Editor.Instance.ChunksToolButton.ToolTip = "Stamp Tool";
			Editor.Instance.EncorePaletteButton.ToolTip = "Show Encore Colors";
			Editor.Instance.ShowTileIDButton.ToolTip = "Toggle Tile ID Visibility" + KeyBindPraser("ShowTileID", true, true);
			Editor.Instance.ShowGridButton.ToolTip = "Toggle Grid Visibility" + KeyBindPraser("ShowGrid", true, true);

		}

		#endregion

	}


}
