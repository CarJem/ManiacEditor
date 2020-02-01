using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ManiacEditor.Actions;
using RSDKv5;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;

namespace ManiacEditor
{
    public class EditorControl
    {
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private bool IsChunksEdit() { return Editor.Instance.IsChunksEdit(); }
        private bool IsTilesEdit() { return Editor.Instance.IsTilesEdit(); }
        private bool IsEntitiesEdit() { return Editor.Instance.IsEntitiesEdit(); }
        private bool IsEditing() { return Editor.Instance.IsEditing(); }
        private bool IsSceneLoaded() { return Editor.Instance.IsSceneLoaded(); }


        private bool GameRunning { get => Editor.Instance.InGame.GameRunning; set => Editor.Instance.InGame.GameRunning = value; }

        private int ScrollDirection { get => Classes.Editor.SolutionState.ScrollDirection; }
        private bool ScrollLocked { get => Classes.Editor.SolutionState.ScrollLocked; }

        private bool CtrlPressed() { return Editor.Instance.CtrlPressed(); }
        private bool ShiftPressed() { return Editor.Instance.ShiftPressed(); }
        private bool IsSelected() { return Editor.Instance.IsSelected(); }

        bool ForceUpdateMousePos { get; set; } = false;


        public EditorControl()
        {
            UpdateTooltips();
            Editor.Instance.EditorMenuBar.UpdateMenuItems();
        }

        #region Mouse Controls

        public void ToggleScrollerMode(System.Windows.Forms.MouseEventArgs e)
        {

            if (!Classes.Editor.SolutionState.WheelClicked)
            {
                //Turn Scroller Mode On
                Classes.Editor.SolutionState.WheelClicked = true;
                Classes.Editor.SolutionState.Scrolling = true;
                Classes.Editor.SolutionState.ScrollingDragged = false;
                Classes.Editor.SolutionState.ScrollPosition = new Point(e.X - Classes.Editor.SolutionState.ViewPositionX, e.Y - Classes.Editor.SolutionState.ViewPositionY);
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
                    Classes.Editor.SolutionState.Scrolling = false;
                }
            }
            else
            {
                //Turn Scroller Mode Off
                Classes.Editor.SolutionState.WheelClicked = false;
                if (Classes.Editor.SolutionState.ScrollingDragged)
                {
                    Classes.Editor.SolutionState.Scrolling = false;
                    Editor.Instance.Cursor = System.Windows.Input.Cursors.Arrow;
                    SetScrollerBorderApperance();
                }
            }

        }
        public void UpdateUndoRedo()
        {
            if (IsEntitiesEdit())
            {
                if (Classes.Editor.Solution.Entities.SelectedEntities.Count > 0)
                {
                    IAction action = new ActionMoveEntities(Classes.Editor.Solution.Entities.SelectedEntities.ToList(), new Point(Classes.Editor.SolutionState.DraggedX, Classes.Editor.SolutionState.DraggedY));
                    if (Classes.Editor.Solution.Entities.LastAction != null)
                    {
                        // If it is move & duplicate, merge them together
                        var taction = new ActionsGroup();
                        taction.AddAction(Classes.Editor.Solution.Entities.LastAction);
                        Classes.Editor.Solution.Entities.LastAction = null;
                        taction.AddAction(action);
                        taction.Close();
                        action = taction;
                    }
                    Editor.Instance.UndoStack.Push(action);
                    Editor.Instance.RedoStack.Clear();
                    Editor.Instance.UI.UpdateControls();
                }
                if (Classes.Editor.Solution.Entities.SelectedInternalEntities.Count > 0)
                {
                    IAction action = new ActionMoveEntities(Classes.Editor.Solution.Entities.SelectedInternalEntities.ToList(), new Point(Classes.Editor.SolutionState.DraggedX, Classes.Editor.SolutionState.DraggedY));
                    if (Classes.Editor.Solution.Entities.LastActionInternal != null)
                    {
                        // If it is move & duplicate, merge them together
                        var taction = new ActionsGroup();
                        taction.AddAction(Classes.Editor.Solution.Entities.LastActionInternal);
                        Classes.Editor.Solution.Entities.LastActionInternal = null;
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

            Editor.Instance.ScrollBorderN.InvalidateVisual();
            Editor.Instance.ScrollBorderS.InvalidateVisual();
            Editor.Instance.ScrollBorderE.InvalidateVisual();
            Editor.Instance.ScrollBorderW.InvalidateVisual();
            Editor.Instance.ScrollBorderNW.InvalidateVisual();
            Editor.Instance.ScrollBorderSW.InvalidateVisual();
            Editor.Instance.ScrollBorderSE.InvalidateVisual();
            Editor.Instance.ScrollBorderNE.InvalidateVisual();


        }
        public void EnforceCursorPosition()
        {
            if (Settings.MySettings.ScrollerAutoCenters)
            {
                ForceUpdateMousePos = true;
                System.Windows.Point pointFromParent = Editor.Instance.ViewPanelForm.TranslatePoint(new System.Windows.Point(0, 0), Editor.Instance);
                SetCursorPos((int)(Editor.Instance.Left + pointFromParent.X) + (int)(Editor.Instance.ViewPanelForm.ActualWidth / 2), (int)(Editor.Instance.Left + pointFromParent.Y) + (int)(Editor.Instance.ViewPanelForm.ActualHeight / 2));
            }

        }
        public void UpdateScrollerPosition(System.Windows.Forms.MouseEventArgs e)
        {
            Classes.Editor.SolutionState.ScrollPosition = new Point(e.X - Classes.Editor.SolutionState.ViewPositionX, e.Y - Classes.Editor.SolutionState.ViewPositionY);
            ForceUpdateMousePos = false;
        }
        #region Mouse Down Controls
        public void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!Classes.Editor.SolutionState.Scrolling) Editor.Instance.FormsModel.GraphicPanel.Focus();

            if (e.Button == MouseButtons.Left) MouseDownLeft(e);
            else if (e.Button == MouseButtons.Right) MouseDownRight(e);
            else if (e.Button == MouseButtons.Middle) MouseDownMiddle(e);
        }

        public void MouseDownRight(System.Windows.Forms.MouseEventArgs e)
        {
            if (IsTilesEdit() && !IsChunksEdit()) TilesEditMouseDown(e);
            else if (IsEntitiesEdit()) EntitiesEditMouseDown(e);
        }

        public void MouseDownLeft(System.Windows.Forms.MouseEventArgs e)
        {
            if (IsEditing() && !Classes.Editor.SolutionState.Dragged)
            {
                if (IsTilesEdit() && !Editor.Instance.EditorToolbar.InteractionToolButton.IsChecked.Value && !IsChunksEdit()) TilesEditMouseDown(e);
                if (IsChunksEdit() && IsSceneLoaded()) ChunksEditMouseDown(e);
                else if (IsEntitiesEdit()) EntitiesEditMouseDown(e);
            }
            InteractiveMouseDown(e);
        }

        public void MouseDownMiddle(System.Windows.Forms.MouseEventArgs e)
        {
            EnforceCursorPosition();
            ToggleScrollerMode(e);
        }

        #endregion
        public void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (ForceUpdateMousePos) UpdateScrollerPosition(e);
            if (Classes.Editor.SolutionState.Scrolling) ScrollerMouseMove(e);
            if (Classes.Editor.SolutionState.Scrolling || Classes.Editor.SolutionState.ScrollingDragged || Classes.Editor.SolutionState.DraggingSelection || Classes.Editor.SolutionState.Dragged) Editor.Instance.FormsModel.GraphicPanel.Render();

            Editor.Instance.EditorStatusBar.UpdatePositionLabel(e);

            if (GameRunning) InteractiveMouseMove(e);

            if (Classes.Editor.SolutionState.RegionX1 != -1)
            {
                if (IsTilesEdit() && !Editor.Instance.EditorToolbar.InteractionToolButton.IsChecked.Value && !IsChunksEdit()) TilesEditMouseMoveDraggingStarted(e);
                else if (IsChunksEdit()) ChunksEditMouseMoveDraggingStarted(e);
                else if (IsEntitiesEdit()) EntitiesEditMouseMoveDraggingStarted(e);

                Classes.Editor.SolutionState.RegionX1 = -1;
                Classes.Editor.SolutionState.RegionY1 = -1;
            }

            else if (e.Button == MouseButtons.Middle) EnforceCursorPosition();

            if (IsTilesEdit() && !IsChunksEdit()) TilesEditMouseMove(e);
            else if (IsChunksEdit()) ChunksEditMouseMove(e);
            else if (IsEntitiesEdit()) EntitiesEditMouseMove(e);

            MouseMovementControls(e);

            Classes.Editor.SolutionState.LastX = e.X;
            Classes.Editor.SolutionState.LastY = e.Y;
        }
        #region Mouse Up Controls
        public void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Classes.Editor.SolutionState.isTileDrawing = false;
            if (Classes.Editor.SolutionState.DraggingSelection) MouseUpDraggingSelection(e);
            else
            {
                if (Classes.Editor.SolutionState.RegionX1 != -1)
                {
                    // So it was just click
                    if (e.Button == MouseButtons.Left)
                    {
                        if (IsTilesEdit() && !IsChunksEdit()) TilesEditMouseUp(e);
                        else if (IsChunksEdit()) ChunksEditMouseUp(e);
                        else if (IsEntitiesEdit()) EntitiesEditMouseUp(e);
                    }
                    Editor.Instance.UI.SetSelectOnlyButtonsState();
                    Classes.Editor.SolutionState.RegionX1 = -1;
                    Classes.Editor.SolutionState.RegionY1 = -1;
                }
                if (Classes.Editor.SolutionState.Dragged && (Classes.Editor.SolutionState.DraggedX != 0 || Classes.Editor.SolutionState.DraggedY != 0)) UpdateUndoRedo();
                Classes.Editor.SolutionState.Dragged = false;
            }
            ScrollerMouseUp(e);

            Editor.Instance.UI.UpdateEditLayerActions();
            Editor.Instance.UI.UpdateControls();


        }
        public void MouseUpDraggingSelection(System.Windows.Forms.MouseEventArgs e)
        {
            if (Classes.Editor.SolutionState.RegionX2 != e.X && Classes.Editor.SolutionState.RegionY2 != e.Y)
            {
                int x1 = (int)(Classes.Editor.SolutionState.RegionX2 / Classes.Editor.SolutionState.Zoom), x2 = (int)(e.X / Classes.Editor.SolutionState.Zoom);
                int y1 = (int)(Classes.Editor.SolutionState.RegionY2 / Classes.Editor.SolutionState.Zoom), y2 = (int)(e.Y / Classes.Editor.SolutionState.Zoom);
                if (x1 > x2)
                {
                    x1 = (int)(e.X / Classes.Editor.SolutionState.Zoom);
                    x2 = (int)(Classes.Editor.SolutionState.RegionX2 / Classes.Editor.SolutionState.Zoom);
                }
                if (y1 > y2)
                {
                    y1 = (int)(e.Y / Classes.Editor.SolutionState.Zoom);
                    y2 = (int)(Classes.Editor.SolutionState.RegionY2 / Classes.Editor.SolutionState.Zoom);
                }

                if (IsChunksEdit())
                {
                    Point selectStart = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(Classes.Editor.SolutionState.TempSelectX1, Classes.Editor.SolutionState.TempSelectY1);
                    Point selectEnd = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesBottomEdge(Classes.Editor.SolutionState.TempSelectX2, Classes.Editor.SolutionState.TempSelectY2);

                    Classes.Editor.Solution.EditLayerA?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                    Classes.Editor.Solution.EditLayerB?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                else
                {
                    Classes.Editor.Solution.EditLayerA?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                    Classes.Editor.Solution.EditLayerB?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());

                    if (IsEntitiesEdit()) Classes.Editor.Solution.Entities.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                Editor.Instance.UI.SetSelectOnlyButtonsState();
                Editor.Instance.UI.UpdateEditLayerActions();

            }
            Classes.Editor.SolutionState.DraggingSelection = false;
            Classes.Editor.Solution.EditLayerA?.EndTempSelection();
            Classes.Editor.Solution.EditLayerB?.EndTempSelection();

            if (IsEntitiesEdit()) Classes.Editor.Solution.Entities.EndTempSelection();
        }

        #endregion
        public void MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Editor.Instance.FormsModel.GraphicPanel.Focus();
            if (CtrlPressed()) Ctrl();
            else Normal();

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
                Classes.Editor.SolutionState.ZoomLevel += change;
                if (Classes.Editor.SolutionState.ZoomLevel > maxZoom) Classes.Editor.SolutionState.ZoomLevel = maxZoom;
                if (Classes.Editor.SolutionState.ZoomLevel < minZoom) Classes.Editor.SolutionState.ZoomLevel = minZoom;

                Editor.Instance.ZoomModel.SetZoomLevel(Classes.Editor.SolutionState.ZoomLevel, new Point(e.X - Classes.Editor.SolutionState.ViewPositionX, e.Y - Classes.Editor.SolutionState.ViewPositionY));
            }
            void Normal()
            {
                if (Editor.Instance.FormsModel.vScrollBar1.IsVisible || Editor.Instance.FormsModel.hScrollBar1.IsVisible) ScrollMove();
                if (Settings.MySettings.EntityFreeCam) FreeCamScroll();

                void ScrollMove()
                {
                    if (ScrollDirection == (int)ScrollDir.Y && !ScrollLocked) ScrollY();
                    else if (ScrollDirection == (int)ScrollDir.X && !ScrollLocked) ScrollX();
                    else if (ScrollLocked)
                        if (ScrollDirection == (int)ScrollDir.Y) ScrollY();
                        else ScrollX();


                    void ScrollX()
                    {
                        if (ShiftPressed())
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

                    void ScrollY()
                    {
                        if (ShiftPressed())
                        {
                            if (Editor.Instance.FormsModel.hScrollBar1.IsVisible) HScroll();
                            else VScroll();
                        }
                        else
                        {
                            if (Editor.Instance.FormsModel.vScrollBar1.IsVisible) VScroll();
                            else HScroll();
                        }

                    }
                }
                void FreeCamScroll()
                {
                    if (ScrollDirection == (int)ScrollDir.X) Classes.Editor.SolutionState.CustomX -= e.Delta;
                    else Classes.Editor.SolutionState.CustomY -= e.Delta;
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
            Editor.Instance.FormsModel.GraphicPanel.Focus();
            if (e.Button == MouseButtons.Right)
            {
                if (Editor.Instance.EditorToolbar.InteractionToolButton.IsChecked.Value) InteractiveContextMenu(e);
                else if (IsEntitiesEdit() && !Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value && !Editor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value && (!Classes.Editor.SolutionState.RightClicktoSwapSlotID || Classes.Editor.Solution.Entities.SelectedEntities.Count <= 1)) EntitiesEditContextMenu(e);
                else if (IsTilesEdit() && !Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value) TilesEditContextMenu(e);
            }

        }
        public void SetClickedXY(System.Windows.Forms.MouseEventArgs e) { Classes.Editor.SolutionState.RegionX1 = e.X; Classes.Editor.SolutionState.RegionY1 = e.Y; }
        public void SetClickedXY(Point e) { Classes.Editor.SolutionState.RegionX1 = e.X; Classes.Editor.SolutionState.RegionY1 = e.Y; }


        #region Tiles Edit Mouse Controls

        public void TilesEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
            {
                TilesEditDrawTool(e, false);
            }
        }
        public void TilesEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(Classes.Editor.SolutionState.RegionX1 / Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.SolutionState.RegionY1 / Classes.Editor.SolutionState.Zoom));
            bool PointASelected = Classes.Editor.Solution.EditLayerA?.IsPointSelected(clicked_point) ?? false;
            bool PointBSelected = Classes.Editor.Solution.EditLayerB?.IsPointSelected(clicked_point) ?? false;
            if (PointASelected || PointBSelected)
            {
                // Start dragging the tiles
                Classes.Editor.SolutionState.Dragged = true;
                Classes.Editor.SolutionState.StartDragged = true;
                Classes.Editor.Solution.EditLayerA?.StartDrag();
                Classes.Editor.Solution.EditLayerB?.StartDrag();
            }

            else if (!Editor.Instance.EditorToolbar.SelectToolButton.IsChecked.Value && !ShiftPressed() && !CtrlPressed() && (Classes.Editor.Solution.EditLayerA?.HasTileAt(clicked_point) ?? false) || (Classes.Editor.Solution.EditLayerB?.HasTileAt(clicked_point) ?? false))
            {
                // Start dragging the single selected tile
                Classes.Editor.Solution.EditLayerA?.Select(clicked_point);
                Classes.Editor.Solution.EditLayerB?.Select(clicked_point);
                Classes.Editor.SolutionState.Dragged = true;
                Classes.Editor.SolutionState.StartDragged = true;
                Classes.Editor.Solution.EditLayerA?.StartDrag();
                Classes.Editor.Solution.EditLayerB?.StartDrag();
            }

            else
            {
                // Start drag selection
                //EditLayer.Select(clicked_point, ShiftPressed || CtrlPressed, CtrlPressed);
                if (!ShiftPressed() && !CtrlPressed())
                    Editor.Instance.Deselect();
                Editor.Instance.UI.UpdateEditLayerActions();

                Classes.Editor.SolutionState.DraggingSelection = true;
                Classes.Editor.SolutionState.RegionX2 = Classes.Editor.SolutionState.RegionX1;
                Classes.Editor.SolutionState.RegionY2 = Classes.Editor.SolutionState.RegionY1;
            }
        }
        public void TilesEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                if (Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    TilesEditDrawTool(e, true);
                }
                else SetClickedXY(e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    TilesEditDrawTool(e, true);
                }
            }
        }
        public void TilesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
            Classes.Editor.Solution.EditLayerA?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Classes.Editor.Solution.EditLayerB?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
        }
        public void TilesEditContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            string newLine = Environment.NewLine;
            Point chunkPos = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinates(e.X / Classes.Editor.SolutionState.Zoom, e.Y / Classes.Editor.SolutionState.Zoom);
            Point tilePos;
            if (e.X == 0 || e.Y == 0) tilePos = new Point(0, 0);
            else tilePos = new Point(e.X / 16, e.Y / 16);

            Editor.Instance.EditorStatusBar.PixelPositionMenuItem.Header = "Pixel Position:" + newLine + String.Format("X: {0}, Y: {1}", e.X, e.Y);
            Editor.Instance.EditorStatusBar.ChunkPositionMenuItem.Header = "Chunk Position:" + newLine + String.Format("X: {0}, Y: {1}", chunkPos.X, chunkPos.Y);
            Editor.Instance.EditorStatusBar.TilePositionMenuItem.Header = "Tile Position:" + newLine + String.Format("X: {0}, Y: {1}", tilePos.X, tilePos.Y);


            Point clicked_point_tile = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
            int tile;
            int tileA = (ushort)(Classes.Editor.Solution.EditLayerA?.GetTileAt(clicked_point_tile) & 0x3ff);
            int tileB = 0;
            if (Classes.Editor.Solution.EditLayerB != null)
            {
                tileB = (ushort)(Classes.Editor.Solution.EditLayerB?.GetTileAt(clicked_point_tile) & 0x3ff);
                if (tileA > 1023 && tileB < 1023) tile = tileB;
                else tile = tileA;
            }
            else tile = tileA;

            Classes.Editor.SolutionState.SelectedTileID = tile;
            Editor.Instance.EditorStatusBar.TileManiacIntergrationItem.IsEnabled = (tile < 1023);
            Editor.Instance.EditorStatusBar.TileManiacIntergrationItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);

            System.Windows.Controls.ContextMenu info = new System.Windows.Controls.ContextMenu();
            info.ItemsSource = Editor.Instance.EditorStatusBar.TilesContext.Items;
            info.Foreground = (System.Windows.Media.SolidColorBrush)Editor.Instance.FindResource("NormalText");
            info.Background = (System.Windows.Media.SolidColorBrush)Editor.Instance.FindResource("NormalBackground");
            info.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
            info.StaysOpen = false;
            info.IsOpen = true;
        }

        #region Universal Tool Actions

        public void TilesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
        {
            Point p = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
            if (click)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Classes.Editor.SolutionState.isTileDrawing = true;
                    PlaceTile();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Classes.Editor.SolutionState.isTileDrawing = true;
                    RemoveTile();
                }
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    Classes.Editor.SolutionState.isTileDrawing = true;
                    PlaceTile();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Classes.Editor.SolutionState.isTileDrawing = true;
                    RemoveTile();
                }
            }

            void RemoveTile()
            {
                // Remove tile
                if (Classes.Editor.SolutionState.DrawBrushSize == 1)
                {
                    Classes.Editor.Solution.EditLayerA?.Select(p);
                    Classes.Editor.Solution.EditLayerB?.Select(p);
                    Editor.Instance.DeleteSelected();
                }
                else
                {
                    double size = (Classes.Editor.SolutionState.DrawBrushSize / 2) * Classes.Editor.Constants.TILE_SIZE;
                    Classes.Editor.Solution.EditLayerA?.Select(new Rectangle((int)(p.X - size), (int)(p.Y - size), Classes.Editor.SolutionState.DrawBrushSize * Classes.Editor.Constants.TILE_SIZE, Classes.Editor.SolutionState.DrawBrushSize * Classes.Editor.Constants.TILE_SIZE));
                    Classes.Editor.Solution.EditLayerB?.Select(new Rectangle((int)(p.X - size), (int)(p.Y - size), Classes.Editor.SolutionState.DrawBrushSize * Classes.Editor.Constants.TILE_SIZE, Classes.Editor.SolutionState.DrawBrushSize * Classes.Editor.Constants.TILE_SIZE));
                    Editor.Instance.DeleteSelected();
                }
            }

            void PlaceTile()
            {
                if (Classes.Editor.SolutionState.DrawBrushSize == 1)
                {
                    if (Editor.Instance.TilesToolbar.SelectedTile != -1)
                    {
                        if (Classes.Editor.Solution.EditLayerA.GetTileAt(p) != Editor.Instance.TilesToolbar.SelectedTile)
                        {
                            Editor.Instance.EditorPlaceTile(p, Editor.Instance.TilesToolbar.SelectedTile, Classes.Editor.Solution.EditLayerA);
                        }
                        else if (!Classes.Editor.Solution.EditLayerA.IsPointSelected(p))
                        {
                            Classes.Editor.Solution.EditLayerA.Select(p);
                        }
                    }
                }
                else
                {
                    if (Editor.Instance.TilesToolbar.SelectedTile != -1)
                    {
                        Editor.Instance.EditorPlaceTile(p, Editor.Instance.TilesToolbar.SelectedTile, Classes.Editor.Solution.EditLayerA, true);
                    }
                }
            }
        }

        #endregion


        #endregion

        #region Entities Edit Mouse Controls

        public void EntitiesEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value) EntitiesEditDrawTool(e);
        }
        public void EntitiesEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(Classes.Editor.SolutionState.RegionX1 / Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.SolutionState.RegionY1 / Classes.Editor.SolutionState.Zoom));
            if (Classes.Editor.Solution.Entities.GetEntityAt(clicked_point)?.Selected ?? false)
            {
                SetClickedXY(e);
                // Start dragging the entity
                Classes.Editor.SolutionState.Dragged = true;
                Classes.Editor.SolutionState.DraggedX = 0;
                Classes.Editor.SolutionState.DraggedY = 0;
                Classes.Editor.SolutionState.StartDragged = true;

            }
            else
            {
                // Start drag selection
                if (!ShiftPressed() && !CtrlPressed())
                    Editor.Instance.Deselect();
                Classes.Editor.SolutionState.DraggingSelection = true;
                Classes.Editor.SolutionState.RegionX2 = Classes.Editor.SolutionState.RegionX1;
                Classes.Editor.SolutionState.RegionY2 = Classes.Editor.SolutionState.RegionY1;

            }
        }
        public void EntitiesEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                    if (Classes.Editor.Solution.Entities.GetEntityAt(clicked_point)?.Selected ?? false)
                    {
                        // We will have to check if this dragging or clicking
                        SetClickedXY(e);
                    }
                    else if (!ShiftPressed() && !CtrlPressed() && Classes.Editor.Solution.Entities.GetEntityAt(clicked_point) != null)
                    {
                        Classes.Editor.Solution.Entities.Select(clicked_point);
                        Editor.Instance.UI.SetSelectOnlyButtonsState();
                        // Start dragging the single selected entity
                        Classes.Editor.SolutionState.Dragged = true;
                        Classes.Editor.SolutionState.DraggedX = 0;
                        Classes.Editor.SolutionState.DraggedY = 0;
                        Classes.Editor.SolutionState.StartDragged = true;
                    }
                    else
                    {
                        SetClickedXY(e);
                    }
                }
                else if (Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value) EntitiesEditDrawTool(e, true);
            }
            if (Editor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value) SplineTool(e);
        }
        public void EntitiesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
            if (e.Button == MouseButtons.Left)
            {
                Classes.Editor.Solution.Entities.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Classes.Editor.Solution.Entities.SelectedEntities.Count == 2 && Classes.Editor.SolutionState.RightClicktoSwapSlotID)
                {
                    Classes.Editor.Solution.Entities.SwapSlotIDsFromPair();
                }
            }
        }
        public void EntitiesEditContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
            string newLine = Environment.NewLine;
            if (Classes.Editor.Solution.Entities.GetEntityAt(clicked_point) != null)
            {
                var currentEntity = Classes.Editor.Solution.Entities.GetEntityAt(clicked_point);

                Editor.Instance.EditorStatusBar.EntityNameItem.Header = String.Format("Entity Name: {0}", currentEntity.Name);
                Editor.Instance.EditorStatusBar.EntitySlotIDItem.Header = String.Format("Slot ID: {0} {1} Runtime Slot ID: {2}", currentEntity.Entity.SlotID, Environment.NewLine, Classes.Editor.Solution.Entities.GetRealSlotID(currentEntity.Entity));
                Editor.Instance.EditorStatusBar.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", currentEntity.Entity.Position.X.High, currentEntity.Entity.Position.Y.High);
            }
            else
            {
                Editor.Instance.EditorStatusBar.EntityNameItem.Header = String.Format("Entity Name: {0}", "N/A");
                Editor.Instance.EditorStatusBar.EntitySlotIDItem.Header = String.Format("Slot ID: {0} {1} Runtime Slot ID: {2}", "N/A", Environment.NewLine, "N/A");
                Editor.Instance.EditorStatusBar.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", e.X, e.Y);
            }
            System.Windows.Controls.ContextMenu info = new System.Windows.Controls.ContextMenu();


            info.ItemsSource = Editor.Instance.EditorStatusBar.EntityContext.Items;
            info.Foreground = (System.Windows.Media.SolidColorBrush)Editor.Instance.FindResource("NormalText");
            info.Background = (System.Windows.Media.SolidColorBrush)Editor.Instance.FindResource("NormalBackground");
            info.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
            info.StaysOpen = false;
            info.IsOpen = true;
        }


        #region Universal Tool Actions

        public void EntitiesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
        {
            if (click)
            {
                Classes.Editor.SolutionState.LastX = e.X;
                Classes.Editor.SolutionState.LastY = e.Y;
            }
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                if (Classes.Editor.Solution.Entities.IsEntityAt(clicked_point, true) == true)
                {
                    Editor.Instance.Deselect();
                    Classes.Editor.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                }
                else
                {
                    Editor.Instance.EntitiesToolbar.SpawnObject();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                if (Classes.Editor.Solution.Entities.IsEntityAt(clicked_point, true) == true)
                {
                    Editor.Instance.Deselect();
                    Classes.Editor.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                    Classes.Editor.Solution.Entities.DeleteSelected();
                    Editor.Instance.UpdateLastEntityAction();
                }
            }
        }

        public void SplineTool(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                if (Classes.Editor.Solution.Entities.IsEntityAt(clicked_point) == true)
                {
                    Editor.Instance.Deselect();
                    Classes.Editor.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                }
                else
                {
                    Classes.Editor.Solution.Entities.SpawnInternalSplineObject(new Position((short)clicked_point.X, (short)clicked_point.Y));
                    Editor.Instance.UpdateLastEntityAction();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                Classes.Editor.Scene.Sets.EditorEntity atPoint = Classes.Editor.Solution.Entities.GetEntityAt(clicked_point);
                if (atPoint != null && atPoint.Entity.Object.Name.Name == "Spline")
                {
                    Editor.Instance.Deselect();
                    Classes.Editor.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                    Classes.Editor.Solution.Entities.DeleteInternallySelected();
                    Editor.Instance.UpdateLastEntityAction();
                }
            }
        }

        #endregion

        #endregion

        #region Chunks Edit Mouse Controls

        public void ChunksEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            Point p = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
            Point pC = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinates(p.X, p.Y);

            if (e.Button == MouseButtons.Left)
            {
                if (Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    int selectedIndex = Editor.Instance.TilesToolbar.ChunkList.SelectedIndex;
                    // Place Stamp
                    if (selectedIndex != -1)
                    {
                        if (!Editor.Instance.Chunks.DoesChunkMatch(pC, Editor.Instance.Chunks.StageStamps.StampList[selectedIndex], Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB))
                        {
                            Editor.Instance.Chunks.PasteStamp(pC, selectedIndex, Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB);
                        }

                    }
                }
            }

            else if (e.Button == MouseButtons.Right)
            {
                if (Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {

                    if (!Editor.Instance.Chunks.IsChunkEmpty(pC, Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB))
                    {
                        // Remove Stamp Sized Area
                        Editor.Instance.Chunks.PasteStamp(pC, 0, Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB, true);
                    }
                }

            }
        }
        public void ChunksEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
            Point chunk_point = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);

            bool PointASelected = Classes.Editor.Solution.EditLayerA?.DoesChunkContainASelectedTile(chunk_point) ?? false;
            bool PointBSelected = Classes.Editor.Solution.EditLayerB?.DoesChunkContainASelectedTile(chunk_point) ?? false;
            if (PointASelected || PointBSelected)
            {
                // Start dragging the tiles
                Classes.Editor.SolutionState.Dragged = true;
                Classes.Editor.SolutionState.StartDragged = true;
                Classes.Editor.Solution.EditLayerA?.StartDrag();
                Classes.Editor.Solution.EditLayerB?.StartDrag();
            }
            else
            {
                // Start drag selection
                if (!ShiftPressed() && !CtrlPressed())
                    Editor.Instance.Deselect();
                Editor.Instance.UI.UpdateEditLayerActions();

                Classes.Editor.SolutionState.DraggingSelection = true;
                Classes.Editor.SolutionState.RegionX2 = e.X;
                Classes.Editor.SolutionState.RegionY2 = e.Y;
            }
        }
        public void ChunksEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    Point p = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                    Point pC = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinates(p.X, p.Y);

                    if (Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    {
                        int selectedIndex = Editor.Instance.TilesToolbar.ChunkList.SelectedIndex;
                        // Place Stamp
                        if (selectedIndex != -1)
                        {
                            if (!Editor.Instance.Chunks.DoesChunkMatch(pC, Editor.Instance.Chunks.StageStamps.StampList[selectedIndex], Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB))
                            {
                                Editor.Instance.Chunks.PasteStamp(pC, selectedIndex, Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB);
                            }

                        }
                    }
                    else
                    {
                        SetClickedXY(e);
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    Point p = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                    Point chunk_point = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
                    Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

                    // Remove Stamp Sized Area
                    if (!Classes.Editor.Solution.EditLayerA.DoesChunkContainASelectedTile(p)) Classes.Editor.Solution.EditLayerA?.Select(clicked_chunk);
                    if (Classes.Editor.Solution.EditLayerB != null && !Classes.Editor.Solution.EditLayerB.DoesChunkContainASelectedTile(p)) Classes.Editor.Solution.EditLayerB?.Select(clicked_chunk);
                    Editor.Instance.DeleteSelected();
                }
            }
        }
        public void ChunksEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
            Point chunk_point = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);
            Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

            Classes.Editor.Solution.EditLayerA?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Classes.Editor.Solution.EditLayerB?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Editor.Instance.UI.UpdateEditLayerActions();
        }

        #endregion

        #region Interactive Mouse Controls

        public void InteractiveMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Editor.Instance.InGame.PlayerSelected)
            {
                Editor.Instance.InGame.MovePlayer(new Point(e.X, e.Y), Classes.Editor.SolutionState.Zoom, Editor.Instance.InGame.SelectedPlayer);
            }

            if (Editor.Instance.InGame.CheckpointSelected)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                Editor.Instance.InGame.UpdateCheckpoint(clicked_point, true);
            }
        }
        public void InteractiveMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
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
        public void InteractiveMouseUp(System.Windows.Forms.MouseEventArgs e)
        {

        }
        public void InteractiveContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            if (IsTilesEdit())
            {
                Point clicked_point_tile = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                int tile;
                int tileA = (ushort)(Classes.Editor.Solution.EditLayerA?.GetTileAt(clicked_point_tile) & 0x3ff);
                int tileB = 0;
                if (Classes.Editor.Solution.EditLayerB != null)
                {
                    tileB = (ushort)(Classes.Editor.Solution.EditLayerB?.GetTileAt(clicked_point_tile) & 0x3ff);
                    if (tileA > 1023 && tileB < 1023) tile = tileB;
                    else tile = tileA;
                }
                else tile = tileA;


                Classes.Editor.SolutionState.SelectedTileID = tile;
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
                Point clicked_point_tile = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                string tile = "N/A";
                Editor.Instance.editTile0WithTileManiacToolStripMenuItem.IsEnabled = false;
                Editor.Instance.moveThePlayerToHereToolStripMenuItem.IsEnabled = GameRunning;
                Editor.Instance.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
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

        #endregion

        #region Scroller Mouse Controls
        public void ScrollerMouseMove(MouseEventArgs e)
        {
            if (Classes.Editor.SolutionState.WheelClicked)
            {
                Classes.Editor.SolutionState.ScrollingDragged = true;

            }

            double xMove = (Editor.Instance.FormsModel.hScrollBar1.IsVisible) ? e.X - Classes.Editor.SolutionState.ViewPositionX - Classes.Editor.SolutionState.ScrollPosition.X : 0;
            double yMove = (Editor.Instance.FormsModel.vScrollBar1.IsVisible) ? e.Y - Classes.Editor.SolutionState.ViewPositionY - Classes.Editor.SolutionState.ScrollPosition.Y : 0;

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

            System.Windows.Point position = new System.Windows.Point(Classes.Editor.SolutionState.ViewPositionX, Classes.Editor.SolutionState.ViewPositionY);
            double x = xMove / 10 + position.X;
            double y = yMove / 10 + position.Y;

            Classes.Editor.SolutionState.CustomX += (int)xMove / 10;
            Classes.Editor.SolutionState.CustomY += (int)yMove / 10;

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

        public void ScrollerMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                if (Settings.MySettings.ScrollerPressReleaseMode) ToggleScrollerMode(e);
            }

        }

        public void ScrollerMouseDown(MouseEventArgs e)
        {

        }

        #endregion

        #region Other Mouse Controls

        public void MouseMovementControls(System.Windows.Forms.MouseEventArgs e)
        {
            if (Classes.Editor.SolutionState.DraggingSelection || Classes.Editor.SolutionState.Dragged) EdgeMove();
            if (Classes.Editor.SolutionState.DraggingSelection) SetSelectionBounds();
            else if (Classes.Editor.SolutionState.Dragged) DragMoveItems();


            void EdgeMove()
            {
                System.Windows.Point position = new System.Windows.Point(Classes.Editor.SolutionState.ViewPositionX, Classes.Editor.SolutionState.ViewPositionY); ;
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
                    // FIX: Determine if this is Needed
                    //if (!Classes.Edit.SolutionState.Scrolling) Editor.Instance.FormsModel.GraphicPanel.Render();



                }
            }
            void SetSelectionBounds()
            {
                if (IsChunksEdit()) ChunkMode();
                else Normal();

                void ChunkMode()
                {
                    if (Classes.Editor.SolutionState.RegionX2 != e.X && Classes.Editor.SolutionState.RegionY2 != e.Y)
                    {

                        Classes.Editor.SolutionState.TempSelectX1 = (int)(Classes.Editor.SolutionState.RegionX2 / Classes.Editor.SolutionState.Zoom);
                        Classes.Editor.SolutionState.TempSelectX2 = (int)(e.X / Classes.Editor.SolutionState.Zoom);
                        Classes.Editor.SolutionState.TempSelectY1 = (int)(Classes.Editor.SolutionState.RegionY2 / Classes.Editor.SolutionState.Zoom);
                        Classes.Editor.SolutionState.TempSelectY2 = (int)(e.Y / Classes.Editor.SolutionState.Zoom);
                        if (Classes.Editor.SolutionState.TempSelectX1 > Classes.Editor.SolutionState.TempSelectX2)
                        {
                            Classes.Editor.SolutionState.TempSelectX1 = (int)(e.X / Classes.Editor.SolutionState.Zoom);
                            Classes.Editor.SolutionState.TempSelectX2 = (int)(Classes.Editor.SolutionState.RegionX2 / Classes.Editor.SolutionState.Zoom);
                        }
                        if (Classes.Editor.SolutionState.TempSelectY1 > Classes.Editor.SolutionState.TempSelectY2)
                        {
                            Classes.Editor.SolutionState.TempSelectY1 = (int)(e.Y / Classes.Editor.SolutionState.Zoom);
                            Classes.Editor.SolutionState.TempSelectY2 = (int)(Classes.Editor.SolutionState.RegionY2 / Classes.Editor.SolutionState.Zoom);
                        }

                        Point selectStart = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(Classes.Editor.SolutionState.TempSelectX1, Classes.Editor.SolutionState.TempSelectY1);
                        Point selectEnd = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesBottomEdge(Classes.Editor.SolutionState.TempSelectX2, Classes.Editor.SolutionState.TempSelectY2);

                        Classes.Editor.Solution.EditLayerA?.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());
                        Classes.Editor.Solution.EditLayerB?.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());

                        Editor.Instance.UI.UpdateTilesOptions();
                    }
                }
                void Normal()
                {
                    if (Classes.Editor.SolutionState.RegionX2 != e.X && Classes.Editor.SolutionState.RegionY2 != e.Y)
                    {
                        Classes.Editor.SolutionState.TempSelectX1 = (int)(Classes.Editor.SolutionState.RegionX2 / Classes.Editor.SolutionState.Zoom);
                        Classes.Editor.SolutionState.TempSelectX2 = (int)(e.X / Classes.Editor.SolutionState.Zoom);
                        Classes.Editor.SolutionState.TempSelectY1 = (int)(Classes.Editor.SolutionState.RegionY2 / Classes.Editor.SolutionState.Zoom);
                        Classes.Editor.SolutionState.TempSelectY2 = (int)(e.Y / Classes.Editor.SolutionState.Zoom);
                        if (Classes.Editor.SolutionState.TempSelectX1 > Classes.Editor.SolutionState.TempSelectX2)
                        {
                            Classes.Editor.SolutionState.TempSelectX1 = (int)(e.X / Classes.Editor.SolutionState.Zoom);
                            Classes.Editor.SolutionState.TempSelectX2 = (int)(Classes.Editor.SolutionState.RegionX2 / Classes.Editor.SolutionState.Zoom);
                        }
                        if (Classes.Editor.SolutionState.TempSelectY1 > Classes.Editor.SolutionState.TempSelectY2)
                        {
                            Classes.Editor.SolutionState.TempSelectY1 = (int)(e.Y / Classes.Editor.SolutionState.Zoom);
                            Classes.Editor.SolutionState.TempSelectY2 = (int)(Classes.Editor.SolutionState.RegionY2 / Classes.Editor.SolutionState.Zoom);
                        }
                        Classes.Editor.Solution.EditLayerA?.TempSelection(new Rectangle(Classes.Editor.SolutionState.TempSelectX1, Classes.Editor.SolutionState.TempSelectY1, Classes.Editor.SolutionState.TempSelectX2 - Classes.Editor.SolutionState.TempSelectX1, Classes.Editor.SolutionState.TempSelectY2 - Classes.Editor.SolutionState.TempSelectY1), CtrlPressed());
                        Classes.Editor.Solution.EditLayerB?.TempSelection(new Rectangle(Classes.Editor.SolutionState.TempSelectX1, Classes.Editor.SolutionState.TempSelectY1, Classes.Editor.SolutionState.TempSelectX2 - Classes.Editor.SolutionState.TempSelectX1, Classes.Editor.SolutionState.TempSelectY2 - Classes.Editor.SolutionState.TempSelectY1), CtrlPressed());

                        Editor.Instance.UI.UpdateTilesOptions();

                        if (IsEntitiesEdit()) Classes.Editor.Solution.Entities.TempSelection(new Rectangle(Classes.Editor.SolutionState.TempSelectX1, Classes.Editor.SolutionState.TempSelectY1, Classes.Editor.SolutionState.TempSelectX2 - Classes.Editor.SolutionState.TempSelectX1, Classes.Editor.SolutionState.TempSelectY2 - Classes.Editor.SolutionState.TempSelectY1), CtrlPressed());
                    }
                }
            }
            void DragMoveItems()
            {
                int oldGridX = (int)((Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom) / Classes.Editor.SolutionState.MagnetSize) * Classes.Editor.SolutionState.MagnetSize;
                int oldGridY = (int)((Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom) / Classes.Editor.SolutionState.MagnetSize) * Classes.Editor.SolutionState.MagnetSize;
                int newGridX = (int)((e.X / Classes.Editor.SolutionState.Zoom) / Classes.Editor.SolutionState.MagnetSize) * Classes.Editor.SolutionState.MagnetSize;
                int newGridY = (int)((e.Y / Classes.Editor.SolutionState.Zoom) / Classes.Editor.SolutionState.MagnetSize) * Classes.Editor.SolutionState.MagnetSize;
                Point oldPointGrid = new Point(0, 0);
                Point newPointGrid = new Point(0, 0);
                if (Classes.Editor.SolutionState.UseMagnetMode && IsEntitiesEdit())
                {
                    if (Classes.Editor.SolutionState.UseMagnetXAxis == true && Classes.Editor.SolutionState.UseMagnetYAxis == true)
                    {
                        oldPointGrid = new Point(oldGridX, oldGridY);
                        newPointGrid = new Point(newGridX, newGridY);
                    }
                    if (Classes.Editor.SolutionState.UseMagnetXAxis && !Classes.Editor.SolutionState.UseMagnetYAxis)
                    {
                        oldPointGrid = new Point(oldGridX, (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom));
                        newPointGrid = new Point(newGridX, (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                    }
                    if (!Classes.Editor.SolutionState.UseMagnetXAxis && Classes.Editor.SolutionState.UseMagnetYAxis)
                    {
                        oldPointGrid = new Point((int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom), oldGridY);
                        newPointGrid = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), newGridY);
                    }
                    if (!Classes.Editor.SolutionState.UseMagnetXAxis && !Classes.Editor.SolutionState.UseMagnetYAxis)
                    {
                        oldPointGrid = new Point((int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom));
                        newPointGrid = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                    }
                }
                Point oldPoint = new Point((int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom));
                Point newPoint = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));


                if (!IsChunksEdit())
                {
                    Classes.Editor.Solution.EditLayerA?.MoveSelected(oldPoint, newPoint, CtrlPressed());
                    Classes.Editor.Solution.EditLayerB?.MoveSelected(oldPoint, newPoint, CtrlPressed());
                }
                else
                {
                    Point oldPointAligned = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(oldPoint.X, oldPoint.Y);
                    Point newPointAligned = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(newPoint.X, newPoint.Y);
                    Classes.Editor.Solution.EditLayerA?.MoveSelected(oldPointAligned, newPointAligned, CtrlPressed(), true);
                    Classes.Editor.Solution.EditLayerB?.MoveSelected(oldPointAligned, newPointAligned, CtrlPressed(), true);
                }


                // FIX: Determine if this is Needed.
                //Editor.Instance.UI.UpdateEditLayerActions();
                if (IsEntitiesEdit())
                {
                    if (Classes.Editor.SolutionState.UseMagnetMode)
                    {
                        int x = Classes.Editor.Solution.Entities.GetSelectedEntity().Entity.Position.X.High;
                        int y = Classes.Editor.Solution.Entities.GetSelectedEntity().Entity.Position.Y.High;

                        if (x % Classes.Editor.SolutionState.MagnetSize != 0 && Classes.Editor.SolutionState.UseMagnetXAxis)
                        {
                            int offsetX = x % Classes.Editor.SolutionState.MagnetSize;
                            oldPointGrid.X -= offsetX;
                        }
                        if (y % Classes.Editor.SolutionState.MagnetSize != 0 && Classes.Editor.SolutionState.UseMagnetYAxis)
                        {
                            int offsetY = y % Classes.Editor.SolutionState.MagnetSize;
                            oldPointGrid.Y -= offsetY;
                        }
                    }


                    try
                    {

                        if (Classes.Editor.SolutionState.UseMagnetMode)
                        {
                            Classes.Editor.Solution.Entities.MoveSelected(oldPointGrid, newPointGrid, CtrlPressed() && Classes.Editor.SolutionState.StartDragged);
                        }
                        else
                        {
                            Classes.Editor.Solution.Entities.MoveSelected(oldPoint, newPoint, CtrlPressed() && Classes.Editor.SolutionState.StartDragged);
                        }

                    }
                    catch (Classes.Editor.Scene.EditorEntities.TooManyEntitiesException)
                    {
                        System.Windows.MessageBox.Show("Too many entities! (limit: 2048)");
                        Classes.Editor.SolutionState.Dragged = false;
                        return;
                    }
                    if (Classes.Editor.SolutionState.UseMagnetMode)
                    {
                        Classes.Editor.SolutionState.DraggedX += newPointGrid.X - oldPointGrid.X;
                        Classes.Editor.SolutionState.DraggedY += newPointGrid.Y - oldPointGrid.Y;
                    }
                    else
                    {
                        Classes.Editor.SolutionState.DraggedX += newPoint.X - oldPoint.X;
                        Classes.Editor.SolutionState.DraggedY += newPoint.Y - oldPoint.Y;
                    }
                    if (CtrlPressed() && Classes.Editor.SolutionState.StartDragged)
                    {
                        Editor.Instance.UI.UpdateEntitiesToolbarList();
                        Editor.Instance.UI.SetSelectOnlyButtonsState();
                    }
                    Editor.Instance.EntitiesToolbar.UpdateCurrentEntityProperites();
                }
                Classes.Editor.SolutionState.StartDragged = false;
            }
        }

        #endregion
        #endregion

        #region Keyboard Controls


        //Shorthanding Settings
        readonly Properties.Settings mySettings = Properties.Settings.Default;
        Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;

        #region Keyboard Inputs
        public void GraphicPanel_OnKeyUp(object sender, KeyEventArgs e)
        {
            // Tiles Toolbar Flip Horizontal
            if (isCombo(e, myKeyBinds.FlipHTiles, true))
            {
                if (IsTilesEdit() && Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Editor.Instance.TilesToolbar.SetSelectTileOption(0, false);
            }
            // Tiles Toolbar Flip Vertical
            else if (isCombo(e, myKeyBinds.FlipVTiles, true))
            {
                if (IsTilesEdit() && Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Editor.Instance.TilesToolbar.SetSelectTileOption(1, false);
            }
        }

        public void GraphicPanel_OnKeyDown(object sender, KeyEventArgs e)
        {
            bool parallaxAnimationInProgress = Classes.Editor.SolutionState.AllowAnimations && Classes.Editor.SolutionState.ParallaxAnimationChecked;
            if (parallaxAnimationInProgress) return;

            // Faster Nudge Toggle
            if (isCombo(e, myKeyBinds.NudgeFaster))
            {
                Editor.Instance.ToggleFasterNudgeEvent(sender, null);
            }
            // Scroll Lock Toggle
            else if (isCombo(e, myKeyBinds.ScrollLock))
            {
                Classes.Editor.SolutionState.ScrollLocked ^= true;
            }
            // Switch Scroll Lock Type
            else if (isCombo(e, myKeyBinds.ScrollLockTypeSwitch))
            {
                Editor.Instance.UIEvents.SetScrollLockDirection();

            }
            // Tiles Toolbar Flip Vertical
            else if (isCombo(e, myKeyBinds.FlipVTiles, true))
            {
                if (IsTilesEdit() && Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Editor.Instance.TilesToolbar.SetSelectTileOption(1, true);
            }
            // Tiles Toolbar Flip Horizontal
            else if (isCombo(e, myKeyBinds.FlipHTiles, true))
            {
                if (IsTilesEdit() && Editor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
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
                Editor.Instance.EditorMenuBar.UnloadSceneEvent(null, null);
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
            if (isCombo(e, myKeyBinds.PointerTool) && Editor.Instance.EditorToolbar.PointerToolButton.IsEnabled) Classes.Editor.SolutionState.PointerMode(true);
            else if (isCombo(e, myKeyBinds.SelectTool) && Editor.Instance.EditorToolbar.SelectToolButton.IsEnabled) Classes.Editor.SolutionState.SelectionMode(true);
            else if (isCombo(e, myKeyBinds.DrawTool) && Editor.Instance.EditorToolbar.DrawToolButton.IsEnabled) Classes.Editor.SolutionState.DrawMode(true);
            else if (isCombo(e, myKeyBinds.MagnetTool) && Editor.Instance.EditorToolbar.MagnetMode.IsEnabled) Classes.Editor.SolutionState.UseMagnetMode ^= true;
            else if (isCombo(e, myKeyBinds.SplineTool) && Editor.Instance.EditorToolbar.SplineToolButton.IsEnabled) Classes.Editor.SolutionState.SplineMode(true);
            else if (isCombo(e, myKeyBinds.StampTool) && Editor.Instance.EditorToolbar.ChunksToolButton.IsEnabled) Classes.Editor.SolutionState.ChunksMode();

        }
        #endregion

        #region Tile Maniac Controls

        public void TileManiac_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (isCombo(e, myKeyBinds.TileManiacNewInstance))
            {
                MainWindow.Instance.newInstanceToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacOpen))
            {
                MainWindow.Instance.OpenToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacSave))
            {
                MainWindow.Instance.saveToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacSaveAs))
            {
                MainWindow.Instance.saveAsToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacSaveUncompressed))
            {
                MainWindow.Instance.saveUncompressedToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacSaveAsUncompressed))
            {
                MainWindow.Instance.saveAsUncompressedToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacbackupConfig))
            {
                MainWindow.Instance.tileConfigbinToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacbackupImage))
            {
                MainWindow.Instance.x16TilesgifToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacExportColMask))
            {
                MainWindow.Instance.exportCurrentCollisionMaskAsToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacOpenSingleColMask))
            {
                MainWindow.Instance.openSingleCollisionMaskToolStripMenuItem_Click_1(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacImportFromOlderRSDK))
            {
                MainWindow.Instance.importFromOlderRSDKVersionToolStripMenuItem_Click(null, null);
            }

            if (isCombo(e, myKeyBinds.TileManiacCopy))
            {
                MainWindow.Instance.copyToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacPastetoOther))
            {
                MainWindow.Instance.copyToOtherPathToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacPaste))
            {
                MainWindow.Instance.pasteToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacMirrorMode))
            {
                MainWindow.Instance.mirrorPathsToolStripMenuItem1.IsChecked = !MainWindow.Instance.mirrorPathsToolStripMenuItem1.IsChecked;
                MainWindow.Instance.mirrorPathsToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacRestorePathA))
            {
                MainWindow.Instance.pathAToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacRestorePathB))
            {
                MainWindow.Instance.pathBToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacRestorePaths))
            {
                MainWindow.Instance.bothToolStripMenuItem_Click(null, null);
            }

            if (isCombo(e, myKeyBinds.TileManiacShowPathB))
            {
                MainWindow.Instance.showPathBToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacShowGrid))
            {
                MainWindow.Instance.showGridToolStripMenuItem.IsChecked = !MainWindow.Instance.showGridToolStripMenuItem.IsChecked;
                MainWindow.Instance.showGridToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacClassicMode))
            {
                MainWindow.Instance.classicViewModeToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacWindowAlwaysOnTop))
            {
                MainWindow.Instance.windowAlwaysOnTop.IsChecked = !MainWindow.Instance.windowAlwaysOnTop.IsChecked;
                MainWindow.Instance.WindowAlwaysOnTop_Click(null, null);
            }

            if (isCombo(e, myKeyBinds.TileManiacSplitFile))
            {
                MainWindow.Instance.splitFileToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacFlipTileH))
            {
                MainWindow.Instance.flipTileHorizontallyToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacFlipTileV))
            {
                MainWindow.Instance.flipTileVerticallyToolStripMenuItem_Click(null, null);
            }

            if (isCombo(e, myKeyBinds.TileManiacHomeFolderOpen))
            {
                MainWindow.Instance.openCollisionHomeFolderToolStripMenuItem_Click(null, null);
            }

            if (isCombo(e, myKeyBinds.TileManiacAbout))
            {
                MainWindow.Instance.aboutToolStripMenuItem1_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacSettings))
            {
                MainWindow.Instance.settingsToolStripMenuItem_Click(null, null);
            }
        }

        public void TileManiac_OnKeyUp(object sender, KeyEventArgs e)
        {

        }

        public void TileManiac_UpdateMenuItems()
        {
            MainWindow.Instance.newInstanceMenuItem.InputGestureText = KeyBindPraser("NewInstance");
            MainWindow.Instance.openMenuItem.InputGestureText = KeyBindPraser("TileManiacOpen");
            MainWindow.Instance.saveMenuItem.InputGestureText = KeyBindPraser("TileManiacSave");
            MainWindow.Instance.saveAsMenuItem.InputGestureText = KeyBindPraser("TileManiacSaveAs");
            MainWindow.Instance.saveAsUncompressedMenuItem.InputGestureText = KeyBindPraser("TileManiacSaveAsUncompressed");
            MainWindow.Instance.saveUncompressedMenuItem.InputGestureText = KeyBindPraser("TileManiacSaveUncompressed");
            MainWindow.Instance.backupTilesConfigMenuItem.InputGestureText = KeyBindPraser("TileManiacbackupConfig", false, true);
            MainWindow.Instance.backupTilesMenuItem.InputGestureText = KeyBindPraser("TileManiacbackupImage", false, true);
            MainWindow.Instance.importMenuItem.InputGestureText = KeyBindPraser("TileManiacImportFromOlderRSDK", false, true);
            MainWindow.Instance.OpenSingleColMaskMenuItem.InputGestureText = KeyBindPraser("TileManiacOpenSingleColMask", false, true);
            MainWindow.Instance.exportCurrentMaskMenuItem.InputGestureText = KeyBindPraser("TileManiacExportColMask", false, true);

            MainWindow.Instance.copyMenuItem.InputGestureText = KeyBindPraser("TileManiacCopy");
            MainWindow.Instance.copyToOtherPathMenuItem.InputGestureText = KeyBindPraser("TileManiacPastetoOther");
            MainWindow.Instance.pasteMenuItem.InputGestureText = KeyBindPraser("TileManiacPaste");
            MainWindow.Instance.mirrorPathsToolStripMenuItem1.InputGestureText = KeyBindPraser("TileManiacMirrorMode");
            MainWindow.Instance.restorePathAMenuItem.InputGestureText = KeyBindPraser("TileManiacRestorePathA", false, true);
            MainWindow.Instance.restorePathBMenuItem.InputGestureText = KeyBindPraser("TileManiacRestorePathB", false, true);
            MainWindow.Instance.restoreBothMenuItem.InputGestureText = KeyBindPraser("TileManiacRestorePaths", false, true);

            MainWindow.Instance.showPathBToolStripMenuItem.InputGestureText = KeyBindPraser("TileManiacShowPathB");
            MainWindow.Instance.showGridToolStripMenuItem.InputGestureText = KeyBindPraser("TileManiacShowGrid");
            MainWindow.Instance.classicViewModeToolStripMenuItem.InputGestureText = KeyBindPraser("TileManiacClassicMode", false, true);
            MainWindow.Instance.windowAlwaysOnTop.InputGestureText = KeyBindPraser("TileManiacWindowAlwaysOnTop");


            MainWindow.Instance.splitFileMenuItem.InputGestureText = KeyBindPraser("TileManiacSplitFile", false, true);
            MainWindow.Instance.flipTileHMenuItem.InputGestureText = KeyBindPraser("TileManiacFlipTileH", false, true);
            MainWindow.Instance.flipTileVMenuItem.InputGestureText = KeyBindPraser("TileManiacFlipTileV", false, true);

            MainWindow.Instance.openCollisionHomeFolderToolStripMenuItem.InputGestureText = KeyBindPraser("TileManiacHomeFolderOpen", false, true);

            MainWindow.Instance.aboutMenuItem.InputGestureText = KeyBindPraser("TileManiacAbout", false, true);
            MainWindow.Instance.settingsMenuItem.InputGestureText = KeyBindPraser("TileManiacSettings", false, true);
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

        public void UpdateTooltips()
        {
            Editor.Instance.EditorStatusBar.UpdateTooltips();
            Editor.Instance.EditorToolbar.UpdateTooltips();

        }

        #endregion
        #endregion
    }
}
