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
using ManiacEditor.Controls.TileManiac;
using ManiacEditor.Enums;
using ManiacEditor.Extensions;

namespace ManiacEditor
{
    public class EditorControl
    {
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private bool IsChunksEdit() { return Controls.Base.MainEditor.Instance.IsChunksEdit(); }
        private bool IsTilesEdit() { return Controls.Base.MainEditor.Instance.IsTilesEdit(); }
        private bool IsEntitiesEdit() { return Controls.Base.MainEditor.Instance.IsEntitiesEdit(); }
        private bool IsEditing() { return Controls.Base.MainEditor.Instance.IsEditing(); }
        private bool IsSceneLoaded() { return Controls.Base.MainEditor.Instance.IsSceneLoaded(); }


        private bool GameRunning { get => Methods.GameHandler.GameRunning; set => Methods.GameHandler.GameRunning = value; }

        private int ScrollDirection { get => Classes.Core.SolutionState.ScrollDirection; }
        private bool ScrollLocked { get => Classes.Core.SolutionState.ScrollLocked; }

        private bool CtrlPressed() { return Controls.Base.MainEditor.Instance.CtrlPressed(); }
        private bool ShiftPressed() { return Controls.Base.MainEditor.Instance.ShiftPressed(); }
        private bool IsSelected() { return Controls.Base.MainEditor.Instance.IsSelected(); }

        bool ForceUpdateMousePos { get; set; } = false;


        public EditorControl()
        {
            UpdateTooltips();
            Controls.Base.MainEditor.Instance.EditorMenuBar.UpdateMenuItems();
        }

        #region Mouse Controls

        public void ToggleScrollerMode(System.Windows.Forms.MouseEventArgs e)
        {

            if (!Classes.Core.SolutionState.WheelClicked)
            {
                //Turn Scroller Mode On
                Classes.Core.SolutionState.WheelClicked = true;
                Classes.Core.SolutionState.Scrolling = true;
                Classes.Core.SolutionState.ScrollingDragged = false;
                Classes.Core.SolutionState.ScrollPosition = new Point(e.X - Classes.Core.SolutionState.ViewPositionX, e.Y - Classes.Core.SolutionState.ViewPositionY);
                if (Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.IsVisible && Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.IsVisible)
                {
                    Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollAll;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.ALL);
                }
                else if (Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.IsVisible)
                {
                    Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollWE;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.WE);
                }
                else if (Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.IsVisible)
                {
                    Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollNS;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.NS);
                }
                else
                {
                    Classes.Core.SolutionState.Scrolling = false;
                }
            }
            else
            {
                //Turn Scroller Mode Off
                Classes.Core.SolutionState.WheelClicked = false;
                if (Classes.Core.SolutionState.ScrollingDragged)
                {
                    Classes.Core.SolutionState.Scrolling = false;
                    Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.Arrow;
                    SetScrollerBorderApperance();
                }
            }

        }
        public void UpdateUndoRedo()
        {
            if (IsEntitiesEdit())
            {
                if (Classes.Core.Solution.Entities.SelectedEntities.Count > 0)
                {
                    IAction action = new ActionMoveEntities(Classes.Core.Solution.Entities.SelectedEntities.ToList(), new Point(Classes.Core.SolutionState.DraggedX, Classes.Core.SolutionState.DraggedY));
                    if (Classes.Core.Solution.Entities.LastAction != null)
                    {
                        // If it is move & duplicate, merge them together
                        var taction = new ActionsGroup();
                        taction.AddAction(Classes.Core.Solution.Entities.LastAction);
                        Classes.Core.Solution.Entities.LastAction = null;
                        taction.AddAction(action);
                        taction.Close();
                        action = taction;
                    }
                    Controls.Base.MainEditor.Instance.UndoStack.Push(action);
                    Controls.Base.MainEditor.Instance.RedoStack.Clear();
                    Controls.Base.MainEditor.Instance.UI.UpdateControls();
                }
                if (Classes.Core.Solution.Entities.SelectedInternalEntities.Count > 0)
                {
                    IAction action = new ActionMoveEntities(Classes.Core.Solution.Entities.SelectedInternalEntities.ToList(), new Point(Classes.Core.SolutionState.DraggedX, Classes.Core.SolutionState.DraggedY));
                    if (Classes.Core.Solution.Entities.LastActionInternal != null)
                    {
                        // If it is move & duplicate, merge them together
                        var taction = new ActionsGroup();
                        taction.AddAction(Classes.Core.Solution.Entities.LastActionInternal);
                        Classes.Core.Solution.Entities.LastActionInternal = null;
                        taction.AddAction(action);
                        taction.Close();
                        action = taction;
                    }
                    Controls.Base.MainEditor.Instance.UndoStack.Push(action);
                    Controls.Base.MainEditor.Instance.RedoStack.Clear();
                    Controls.Base.MainEditor.Instance.UI.UpdateControls();
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

            Controls.Base.MainEditor.Instance.ScrollBorderN.Fill = NotActive;
            Controls.Base.MainEditor.Instance.ScrollBorderS.Fill = NotActive;
            Controls.Base.MainEditor.Instance.ScrollBorderE.Fill = NotActive;
            Controls.Base.MainEditor.Instance.ScrollBorderW.Fill = NotActive;
            Controls.Base.MainEditor.Instance.ScrollBorderNW.Fill = NotActive;
            Controls.Base.MainEditor.Instance.ScrollBorderSW.Fill = NotActive;
            Controls.Base.MainEditor.Instance.ScrollBorderSE.Fill = NotActive;
            Controls.Base.MainEditor.Instance.ScrollBorderNE.Fill = NotActive;

            switch (direction)
            {
                case 0:
                    Controls.Base.MainEditor.Instance.ScrollBorderN.Fill = Active;
                    break;
                case 1:
                    Controls.Base.MainEditor.Instance.ScrollBorderNE.Fill = Active;
                    break;
                case 2:
                    Controls.Base.MainEditor.Instance.ScrollBorderE.Fill = Active;
                    break;
                case 3:
                    Controls.Base.MainEditor.Instance.ScrollBorderSE.Fill = Active;
                    break;
                case 4:
                    Controls.Base.MainEditor.Instance.ScrollBorderS.Fill = Active;
                    break;
                case 5:
                    Controls.Base.MainEditor.Instance.ScrollBorderSW.Fill = Active;
                    break;
                case 6:
                    Controls.Base.MainEditor.Instance.ScrollBorderW.Fill = Active;
                    break;
                case 7:
                    Controls.Base.MainEditor.Instance.ScrollBorderNW.Fill = Active;
                    break;
                case 8:
                    Controls.Base.MainEditor.Instance.ScrollBorderW.Fill = Active;
                    Controls.Base.MainEditor.Instance.ScrollBorderE.Fill = Active;
                    break;
                case 9:
                    Controls.Base.MainEditor.Instance.ScrollBorderN.Fill = Active;
                    Controls.Base.MainEditor.Instance.ScrollBorderS.Fill = Active;
                    break;
                case 10:
                    Controls.Base.MainEditor.Instance.ScrollBorderN.Fill = Active;
                    Controls.Base.MainEditor.Instance.ScrollBorderS.Fill = Active;
                    Controls.Base.MainEditor.Instance.ScrollBorderE.Fill = Active;
                    Controls.Base.MainEditor.Instance.ScrollBorderW.Fill = Active;
                    Controls.Base.MainEditor.Instance.ScrollBorderNW.Fill = Active;
                    Controls.Base.MainEditor.Instance.ScrollBorderSW.Fill = Active;
                    Controls.Base.MainEditor.Instance.ScrollBorderSE.Fill = Active;
                    Controls.Base.MainEditor.Instance.ScrollBorderNE.Fill = Active;
                    break;
                default:
                    break;

            }

            Controls.Base.MainEditor.Instance.ScrollBorderN.InvalidateVisual();
            Controls.Base.MainEditor.Instance.ScrollBorderS.InvalidateVisual();
            Controls.Base.MainEditor.Instance.ScrollBorderE.InvalidateVisual();
            Controls.Base.MainEditor.Instance.ScrollBorderW.InvalidateVisual();
            Controls.Base.MainEditor.Instance.ScrollBorderNW.InvalidateVisual();
            Controls.Base.MainEditor.Instance.ScrollBorderSW.InvalidateVisual();
            Controls.Base.MainEditor.Instance.ScrollBorderSE.InvalidateVisual();
            Controls.Base.MainEditor.Instance.ScrollBorderNE.InvalidateVisual();


        }
        public void EnforceCursorPosition()
        {
            if (Core.Settings.MySettings.ScrollerAutoCenters)
            {
                ForceUpdateMousePos = true;
                System.Windows.Point pointFromParent = Controls.Base.MainEditor.Instance.ViewPanelForm.TranslatePoint(new System.Windows.Point(0, 0), Controls.Base.MainEditor.Instance);
                SetCursorPos((int)(Controls.Base.MainEditor.Instance.Left + pointFromParent.X) + (int)(Controls.Base.MainEditor.Instance.ViewPanelForm.ActualWidth / 2), (int)(Controls.Base.MainEditor.Instance.Left + pointFromParent.Y) + (int)(Controls.Base.MainEditor.Instance.ViewPanelForm.ActualHeight / 2));
            }

        }
        public void UpdateScrollerPosition(System.Windows.Forms.MouseEventArgs e)
        {
            Classes.Core.SolutionState.ScrollPosition = new Point(e.X - Classes.Core.SolutionState.ViewPositionX, e.Y - Classes.Core.SolutionState.ViewPositionY);
            ForceUpdateMousePos = false;
        }
        #region Mouse Down Controls
        public void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!Classes.Core.SolutionState.Scrolling) Controls.Base.MainEditor.Instance.DeviceModel.GraphicPanel.Focus();

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
            if (IsEditing() && !Classes.Core.SolutionState.Dragged)
            {
                if (IsTilesEdit() && !Controls.Base.MainEditor.Instance.EditorToolbar.InteractionToolButton.IsChecked.Value && !IsChunksEdit()) TilesEditMouseDown(e);
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
            if (Classes.Core.SolutionState.Scrolling) ScrollerMouseMove(e);
            if (Classes.Core.SolutionState.Scrolling || Classes.Core.SolutionState.ScrollingDragged || Classes.Core.SolutionState.DraggingSelection || Classes.Core.SolutionState.Dragged) Controls.Base.MainEditor.Instance.DeviceModel.GraphicPanel.Render();

            Controls.Base.MainEditor.Instance.EditorStatusBar.UpdatePositionLabel(e);

            if (GameRunning) InteractiveMouseMove(e);

            if (Classes.Core.SolutionState.RegionX1 != -1)
            {
                if (IsTilesEdit() && !Controls.Base.MainEditor.Instance.EditorToolbar.InteractionToolButton.IsChecked.Value && !IsChunksEdit()) TilesEditMouseMoveDraggingStarted(e);
                else if (IsChunksEdit()) ChunksEditMouseMoveDraggingStarted(e);
                else if (IsEntitiesEdit()) EntitiesEditMouseMoveDraggingStarted(e);

                Classes.Core.SolutionState.RegionX1 = -1;
                Classes.Core.SolutionState.RegionY1 = -1;
            }

            else if (e.Button == MouseButtons.Middle) EnforceCursorPosition();

            if (IsTilesEdit() && !IsChunksEdit()) TilesEditMouseMove(e);
            else if (IsChunksEdit()) ChunksEditMouseMove(e);
            else if (IsEntitiesEdit()) EntitiesEditMouseMove(e);

            MouseMovementControls(e);

            Classes.Core.SolutionState.LastX = e.X;
            Classes.Core.SolutionState.LastY = e.Y;
        }
        #region Mouse Up Controls
        public void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Classes.Core.SolutionState.isTileDrawing = false;
            if (Classes.Core.SolutionState.DraggingSelection) MouseUpDraggingSelection(e);
            else
            {
                if (Classes.Core.SolutionState.RegionX1 != -1)
                {
                    // So it was just click
                    if (e.Button == MouseButtons.Left)
                    {
                        if (IsTilesEdit() && !IsChunksEdit()) TilesEditMouseUp(e);
                        else if (IsChunksEdit()) ChunksEditMouseUp(e);
                        else if (IsEntitiesEdit()) EntitiesEditMouseUp(e);
                    }
                    Controls.Base.MainEditor.Instance.UI.SetSelectOnlyButtonsState();
                    Classes.Core.SolutionState.RegionX1 = -1;
                    Classes.Core.SolutionState.RegionY1 = -1;
                }
                if (Classes.Core.SolutionState.Dragged && (Classes.Core.SolutionState.DraggedX != 0 || Classes.Core.SolutionState.DraggedY != 0)) UpdateUndoRedo();
                Classes.Core.SolutionState.Dragged = false;
            }
            ScrollerMouseUp(e);

            Controls.Base.MainEditor.Instance.UI.UpdateEditLayerActions();
            Controls.Base.MainEditor.Instance.UI.UpdateControls();


        }
        public void MouseUpDraggingSelection(System.Windows.Forms.MouseEventArgs e)
        {
            if (Classes.Core.SolutionState.RegionX2 != e.X && Classes.Core.SolutionState.RegionY2 != e.Y)
            {
                int x1 = (int)(Classes.Core.SolutionState.RegionX2 / Classes.Core.SolutionState.Zoom), x2 = (int)(e.X / Classes.Core.SolutionState.Zoom);
                int y1 = (int)(Classes.Core.SolutionState.RegionY2 / Classes.Core.SolutionState.Zoom), y2 = (int)(e.Y / Classes.Core.SolutionState.Zoom);
                if (x1 > x2)
                {
                    x1 = (int)(e.X / Classes.Core.SolutionState.Zoom);
                    x2 = (int)(Classes.Core.SolutionState.RegionX2 / Classes.Core.SolutionState.Zoom);
                }
                if (y1 > y2)
                {
                    y1 = (int)(e.Y / Classes.Core.SolutionState.Zoom);
                    y2 = (int)(Classes.Core.SolutionState.RegionY2 / Classes.Core.SolutionState.Zoom);
                }

                if (IsChunksEdit())
                {
                    Point selectStart = Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(Classes.Core.SolutionState.TempSelectX1, Classes.Core.SolutionState.TempSelectY1);
                    Point selectEnd = Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinatesBottomEdge(Classes.Core.SolutionState.TempSelectX2, Classes.Core.SolutionState.TempSelectY2);

                    Classes.Core.Solution.EditLayerA?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                    Classes.Core.Solution.EditLayerB?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                else
                {
                    Classes.Core.Solution.EditLayerA?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                    Classes.Core.Solution.EditLayerB?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());

                    if (IsEntitiesEdit()) Classes.Core.Solution.Entities.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                Controls.Base.MainEditor.Instance.UI.SetSelectOnlyButtonsState();
                Controls.Base.MainEditor.Instance.UI.UpdateEditLayerActions();

            }
            Classes.Core.SolutionState.DraggingSelection = false;
            Classes.Core.Solution.EditLayerA?.EndTempSelection();
            Classes.Core.Solution.EditLayerB?.EndTempSelection();

            if (IsEntitiesEdit()) Classes.Core.Solution.Entities.EndTempSelection();
        }

        #endregion
        public void MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Controls.Base.MainEditor.Instance.DeviceModel.GraphicPanel.Focus();
            if (CtrlPressed()) Ctrl();
            else Normal();

            void Ctrl()
            {
                int maxZoom;
                int minZoom;
                if (Core.Settings.MyPerformance.ReduceZoom)
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
                Classes.Core.SolutionState.ZoomLevel += change;
                if (Classes.Core.SolutionState.ZoomLevel > maxZoom) Classes.Core.SolutionState.ZoomLevel = maxZoom;
                if (Classes.Core.SolutionState.ZoomLevel < minZoom) Classes.Core.SolutionState.ZoomLevel = minZoom;

                Controls.Base.MainEditor.Instance.ZoomModel.SetZoomLevel(Classes.Core.SolutionState.ZoomLevel, new Point(e.X - Classes.Core.SolutionState.ViewPositionX, e.Y - Classes.Core.SolutionState.ViewPositionY));
            }
            void Normal()
            {
                if (Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.IsVisible || Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.IsVisible) ScrollMove();
                if (Core.Settings.MySettings.EntityFreeCam) FreeCamScroll();

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
                            if (Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.IsVisible) VScroll();
                            else HScroll();
                        }
                        else
                        {
                            if (Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.IsVisible) HScroll();
                            else VScroll();
                        }

                    }

                    void ScrollY()
                    {
                        if (ShiftPressed())
                        {
                            if (Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.IsVisible) HScroll();
                            else VScroll();
                        }
                        else
                        {
                            if (Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.IsVisible) VScroll();
                            else HScroll();
                        }

                    }
                }
                void FreeCamScroll()
                {
                    if (ScrollDirection == (int)ScrollDir.X) Classes.Core.SolutionState.CustomX -= e.Delta;
                    else Classes.Core.SolutionState.CustomY -= e.Delta;
                }
            }
            void VScroll()
            {
                double y = Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.Value - e.Delta;
                if (y < 0) y = 0;
                if (y > Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.Maximum) y = Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.Maximum;
                Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.Value = y;
            }
            void HScroll()
            {
                double x = Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.Value - e.Delta;
                if (x < 0) x = 0;
                if (x > Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.Maximum) x = Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.Maximum;
                Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.Value = x;
            }
        }
        public void MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Controls.Base.MainEditor.Instance.DeviceModel.GraphicPanel.Focus();
            if (e.Button == MouseButtons.Right)
            {
                if (Controls.Base.MainEditor.Instance.EditorToolbar.InteractionToolButton.IsChecked.Value) InteractiveContextMenu(e);
                else if (IsEntitiesEdit() && !Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value && !Controls.Base.MainEditor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value && (!Classes.Core.SolutionState.RightClicktoSwapSlotID || Classes.Core.Solution.Entities.SelectedEntities.Count <= 1)) EntitiesEditContextMenu(e);
                else if (IsTilesEdit() && !Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value) TilesEditContextMenu(e);
            }

        }
        public void SetClickedXY(System.Windows.Forms.MouseEventArgs e) { Classes.Core.SolutionState.RegionX1 = e.X; Classes.Core.SolutionState.RegionY1 = e.Y; }
        public void SetClickedXY(Point e) { Classes.Core.SolutionState.RegionX1 = e.X; Classes.Core.SolutionState.RegionY1 = e.Y; }


        #region Tiles Edit Mouse Controls

        public void TilesEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
            {
                TilesEditDrawTool(e, false);
            }
        }
        public void TilesEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(Classes.Core.SolutionState.RegionX1 / Classes.Core.SolutionState.Zoom), (int)(Classes.Core.SolutionState.RegionY1 / Classes.Core.SolutionState.Zoom));
            bool PointASelected = Classes.Core.Solution.EditLayerA?.IsPointSelected(clicked_point) ?? false;
            bool PointBSelected = Classes.Core.Solution.EditLayerB?.IsPointSelected(clicked_point) ?? false;
            if (PointASelected || PointBSelected)
            {
                // Start dragging the tiles
                Classes.Core.SolutionState.Dragged = true;
                Classes.Core.SolutionState.StartDragged = true;
                Classes.Core.Solution.EditLayerA?.StartDrag();
                Classes.Core.Solution.EditLayerB?.StartDrag();
            }

            else if (!Controls.Base.MainEditor.Instance.EditorToolbar.SelectToolButton.IsChecked.Value && !ShiftPressed() && !CtrlPressed() && (Classes.Core.Solution.EditLayerA?.HasTileAt(clicked_point) ?? false) || (Classes.Core.Solution.EditLayerB?.HasTileAt(clicked_point) ?? false))
            {
                // Start dragging the single selected tile
                Classes.Core.Solution.EditLayerA?.Select(clicked_point);
                Classes.Core.Solution.EditLayerB?.Select(clicked_point);
                Classes.Core.SolutionState.Dragged = true;
                Classes.Core.SolutionState.StartDragged = true;
                Classes.Core.Solution.EditLayerA?.StartDrag();
                Classes.Core.Solution.EditLayerB?.StartDrag();
            }

            else
            {
                // Start drag selection
                //EditLayer.Select(clicked_point, ShiftPressed || CtrlPressed, CtrlPressed);
                if (!ShiftPressed() && !CtrlPressed())
                    Controls.Base.MainEditor.Instance.Deselect();
                Controls.Base.MainEditor.Instance.UI.UpdateEditLayerActions();

                Classes.Core.SolutionState.DraggingSelection = true;
                Classes.Core.SolutionState.RegionX2 = Classes.Core.SolutionState.RegionX1;
                Classes.Core.SolutionState.RegionY2 = Classes.Core.SolutionState.RegionY1;
            }
        }
        public void TilesEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
                if (Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    TilesEditDrawTool(e, true);
                }
                else SetClickedXY(e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    TilesEditDrawTool(e, true);
                }
            }
        }
        public void TilesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
            Classes.Core.Solution.EditLayerA?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Classes.Core.Solution.EditLayerB?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
        }
        public void TilesEditContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            string newLine = Environment.NewLine;
            Point chunkPos = Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinates(e.X / Classes.Core.SolutionState.Zoom, e.Y / Classes.Core.SolutionState.Zoom);
            Point tilePos;
            if (e.X == 0 || e.Y == 0) tilePos = new Point(0, 0);
            else tilePos = new Point(e.X / 16, e.Y / 16);

            Controls.Base.MainEditor.Instance.EditorStatusBar.PixelPositionMenuItem.Header = "Pixel Position:" + newLine + String.Format("X: {0}, Y: {1}", e.X, e.Y);
            Controls.Base.MainEditor.Instance.EditorStatusBar.ChunkPositionMenuItem.Header = "Chunk Position:" + newLine + String.Format("X: {0}, Y: {1}", chunkPos.X, chunkPos.Y);
            Controls.Base.MainEditor.Instance.EditorStatusBar.TilePositionMenuItem.Header = "Tile Position:" + newLine + String.Format("X: {0}, Y: {1}", tilePos.X, tilePos.Y);


            Point clicked_point_tile = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
            int tile;
            int tileA = (ushort)(Classes.Core.Solution.EditLayerA?.GetTileAt(clicked_point_tile) & 0x3ff);
            int tileB = 0;
            if (Classes.Core.Solution.EditLayerB != null)
            {
                tileB = (ushort)(Classes.Core.Solution.EditLayerB?.GetTileAt(clicked_point_tile) & 0x3ff);
                if (tileA > 1023 && tileB < 1023) tile = tileB;
                else tile = tileA;
            }
            else tile = tileA;

            Classes.Core.SolutionState.SelectedTileID = tile;
            Controls.Base.MainEditor.Instance.EditorStatusBar.TileManiacIntergrationItem.IsEnabled = (tile < 1023);
            Controls.Base.MainEditor.Instance.EditorStatusBar.TileManiacIntergrationItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);

            System.Windows.Controls.ContextMenu info = new System.Windows.Controls.ContextMenu();
            info.ItemsSource = Controls.Base.MainEditor.Instance.EditorStatusBar.TilesContext.Items;
            info.Foreground = (System.Windows.Media.SolidColorBrush)Controls.Base.MainEditor.Instance.FindResource("NormalText");
            info.Background = (System.Windows.Media.SolidColorBrush)Controls.Base.MainEditor.Instance.FindResource("NormalBackground");
            info.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
            info.StaysOpen = false;
            info.IsOpen = true;
        }

        #region Universal Tool Actions

        public void TilesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
        {
            Point p = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
            if (click)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Classes.Core.SolutionState.isTileDrawing = true;
                    PlaceTile();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Classes.Core.SolutionState.isTileDrawing = true;
                    RemoveTile();
                }
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    Classes.Core.SolutionState.isTileDrawing = true;
                    PlaceTile();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Classes.Core.SolutionState.isTileDrawing = true;
                    RemoveTile();
                }
            }

            void RemoveTile()
            {
                // Remove tile
                if (Classes.Core.SolutionState.DrawBrushSize == 1)
                {
                    Classes.Core.Solution.EditLayerA?.Select(p);
                    Classes.Core.Solution.EditLayerB?.Select(p);
                    Controls.Base.MainEditor.Instance.DeleteSelected();
                }
                else
                {
                    double size = (Classes.Core.SolutionState.DrawBrushSize / 2) * Classes.Core.Constants.TILE_SIZE;
                    Classes.Core.Solution.EditLayerA?.Select(new Rectangle((int)(p.X - size), (int)(p.Y - size), Classes.Core.SolutionState.DrawBrushSize * Classes.Core.Constants.TILE_SIZE, Classes.Core.SolutionState.DrawBrushSize * Classes.Core.Constants.TILE_SIZE));
                    Classes.Core.Solution.EditLayerB?.Select(new Rectangle((int)(p.X - size), (int)(p.Y - size), Classes.Core.SolutionState.DrawBrushSize * Classes.Core.Constants.TILE_SIZE, Classes.Core.SolutionState.DrawBrushSize * Classes.Core.Constants.TILE_SIZE));
                    Controls.Base.MainEditor.Instance.DeleteSelected();
                }
            }

            void PlaceTile()
            {
                if (Classes.Core.SolutionState.DrawBrushSize == 1)
                {
                    if (Controls.Base.MainEditor.Instance.TilesToolbar.SelectedTile != -1)
                    {
                        if (Classes.Core.Solution.EditLayerA.GetTileAt(p) != Controls.Base.MainEditor.Instance.TilesToolbar.SelectedTile)
                        {
                            Controls.Base.MainEditor.Instance.EditorPlaceTile(p, Controls.Base.MainEditor.Instance.TilesToolbar.SelectedTile, Classes.Core.Solution.EditLayerA);
                        }
                        else if (!Classes.Core.Solution.EditLayerA.IsPointSelected(p))
                        {
                            Classes.Core.Solution.EditLayerA.Select(p);
                        }
                    }
                }
                else
                {
                    if (Controls.Base.MainEditor.Instance.TilesToolbar.SelectedTile != -1)
                    {
                        Controls.Base.MainEditor.Instance.EditorPlaceTile(p, Controls.Base.MainEditor.Instance.TilesToolbar.SelectedTile, Classes.Core.Solution.EditLayerA, true);
                    }
                }
            }
        }

        #endregion


        #endregion

        #region Entities Edit Mouse Controls

        public void EntitiesEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value) EntitiesEditDrawTool(e);
        }
        public void EntitiesEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(Classes.Core.SolutionState.RegionX1 / Classes.Core.SolutionState.Zoom), (int)(Classes.Core.SolutionState.RegionY1 / Classes.Core.SolutionState.Zoom));
            if (Classes.Core.Solution.Entities.GetEntityAt(clicked_point)?.Selected ?? false)
            {
                SetClickedXY(e);
                // Start dragging the entity
                Classes.Core.SolutionState.Dragged = true;
                Classes.Core.SolutionState.DraggedX = 0;
                Classes.Core.SolutionState.DraggedY = 0;
                Classes.Core.SolutionState.StartDragged = true;

            }
            else
            {
                // Start drag selection
                if (!ShiftPressed() && !CtrlPressed())
                    Controls.Base.MainEditor.Instance.Deselect();
                Classes.Core.SolutionState.DraggingSelection = true;
                Classes.Core.SolutionState.RegionX2 = Classes.Core.SolutionState.RegionX1;
                Classes.Core.SolutionState.RegionY2 = Classes.Core.SolutionState.RegionY1;

            }
        }
        public void EntitiesEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    Point clicked_point = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
                    if (Classes.Core.Solution.Entities.GetEntityAt(clicked_point)?.Selected ?? false)
                    {
                        // We will have to check if this dragging or clicking
                        SetClickedXY(e);
                    }
                    else if (!ShiftPressed() && !CtrlPressed() && Classes.Core.Solution.Entities.GetEntityAt(clicked_point) != null)
                    {
                        Classes.Core.Solution.Entities.Select(clicked_point);
                        Controls.Base.MainEditor.Instance.UI.SetSelectOnlyButtonsState();
                        // Start dragging the single selected entity
                        Classes.Core.SolutionState.Dragged = true;
                        Classes.Core.SolutionState.DraggedX = 0;
                        Classes.Core.SolutionState.DraggedY = 0;
                        Classes.Core.SolutionState.StartDragged = true;
                    }
                    else
                    {
                        SetClickedXY(e);
                    }
                }
                else if (Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value) EntitiesEditDrawTool(e, true);
            }
            if (Controls.Base.MainEditor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value) SplineTool(e);
        }
        public void EntitiesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
            if (e.Button == MouseButtons.Left)
            {
                Classes.Core.Solution.Entities.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Classes.Core.Solution.Entities.SelectedEntities.Count == 2 && Classes.Core.SolutionState.RightClicktoSwapSlotID)
                {
                    Classes.Core.Solution.Entities.SwapSlotIDsFromPair();
                }
            }
        }
        public void EntitiesEditContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
            string newLine = Environment.NewLine;
            if (Classes.Core.Solution.Entities.GetEntityAt(clicked_point) != null)
            {
                var currentEntity = Classes.Core.Solution.Entities.GetEntityAt(clicked_point);

                Controls.Base.MainEditor.Instance.EditorStatusBar.EntityNameItem.Header = String.Format("Entity Name: {0}", currentEntity.Name);
                Controls.Base.MainEditor.Instance.EditorStatusBar.EntitySlotIDItem.Header = String.Format("Slot ID: {0} {1} Runtime Slot ID: {2}", currentEntity.Entity.SlotID, Environment.NewLine, Classes.Core.Solution.Entities.GetRealSlotID(currentEntity.Entity));
                Controls.Base.MainEditor.Instance.EditorStatusBar.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", currentEntity.Entity.Position.X.High, currentEntity.Entity.Position.Y.High);
            }
            else
            {
                Controls.Base.MainEditor.Instance.EditorStatusBar.EntityNameItem.Header = String.Format("Entity Name: {0}", "N/A");
                Controls.Base.MainEditor.Instance.EditorStatusBar.EntitySlotIDItem.Header = String.Format("Slot ID: {0} {1} Runtime Slot ID: {2}", "N/A", Environment.NewLine, "N/A");
                Controls.Base.MainEditor.Instance.EditorStatusBar.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", e.X, e.Y);
            }
            System.Windows.Controls.ContextMenu info = new System.Windows.Controls.ContextMenu();


            info.ItemsSource = Controls.Base.MainEditor.Instance.EditorStatusBar.EntityContext.Items;
            info.Foreground = (System.Windows.Media.SolidColorBrush)Controls.Base.MainEditor.Instance.FindResource("NormalText");
            info.Background = (System.Windows.Media.SolidColorBrush)Controls.Base.MainEditor.Instance.FindResource("NormalBackground");
            info.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
            info.StaysOpen = false;
            info.IsOpen = true;
        }


        #region Universal Tool Actions

        public void EntitiesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
        {
            if (click)
            {
                Classes.Core.SolutionState.LastX = e.X;
                Classes.Core.SolutionState.LastY = e.Y;
            }
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
                if (Classes.Core.Solution.Entities.IsEntityAt(clicked_point, true) == true)
                {
                    Controls.Base.MainEditor.Instance.Deselect();
                    Classes.Core.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                }
                else
                {
                    Controls.Base.MainEditor.Instance.EntitiesToolbar.SpawnObject();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
                if (Classes.Core.Solution.Entities.IsEntityAt(clicked_point, true) == true)
                {
                    Controls.Base.MainEditor.Instance.Deselect();
                    Classes.Core.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                    Classes.Core.Solution.Entities.DeleteSelected();
                    Controls.Base.MainEditor.Instance.UpdateLastEntityAction();
                }
            }
        }

        public void SplineTool(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
                if (Classes.Core.Solution.Entities.IsEntityAt(clicked_point) == true)
                {
                    Controls.Base.MainEditor.Instance.Deselect();
                    Classes.Core.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                }
                else
                {
                    Classes.Core.Solution.Entities.SpawnInternalSplineObject(new Position((short)clicked_point.X, (short)clicked_point.Y));
                    Controls.Base.MainEditor.Instance.UpdateLastEntityAction();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
                Classes.Core.Scene.Sets.EditorEntity atPoint = Classes.Core.Solution.Entities.GetEntityAt(clicked_point);
                if (atPoint != null && atPoint.Entity.Object.Name.Name == "Spline")
                {
                    Controls.Base.MainEditor.Instance.Deselect();
                    Classes.Core.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                    Classes.Core.Solution.Entities.DeleteInternallySelected();
                    Controls.Base.MainEditor.Instance.UpdateLastEntityAction();
                }
            }
        }

        #endregion

        #endregion

        #region Chunks Edit Mouse Controls

        public void ChunksEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            Point p = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
            Point pC = Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinates(p.X, p.Y);

            if (e.Button == MouseButtons.Left)
            {
                if (Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    int selectedIndex = Controls.Base.MainEditor.Instance.TilesToolbar.ChunkList.SelectedIndex;
                    // Place Stamp
                    if (selectedIndex != -1)
                    {
                        if (!Controls.Base.MainEditor.Instance.Chunks.DoesChunkMatch(pC, Controls.Base.MainEditor.Instance.Chunks.StageStamps.StampList[selectedIndex], Classes.Core.Solution.EditLayerA, Classes.Core.Solution.EditLayerB))
                        {
                            Controls.Base.MainEditor.Instance.Chunks.PasteStamp(pC, selectedIndex, Classes.Core.Solution.EditLayerA, Classes.Core.Solution.EditLayerB);
                        }

                    }
                }
            }

            else if (e.Button == MouseButtons.Right)
            {
                if (Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {

                    if (!Controls.Base.MainEditor.Instance.Chunks.IsChunkEmpty(pC, Classes.Core.Solution.EditLayerA, Classes.Core.Solution.EditLayerB))
                    {
                        // Remove Stamp Sized Area
                        Controls.Base.MainEditor.Instance.Chunks.PasteStamp(pC, 0, Classes.Core.Solution.EditLayerA, Classes.Core.Solution.EditLayerB, true);
                    }
                }

            }
        }
        public void ChunksEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
            Point chunk_point = Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);

            bool PointASelected = Classes.Core.Solution.EditLayerA?.DoesChunkContainASelectedTile(chunk_point) ?? false;
            bool PointBSelected = Classes.Core.Solution.EditLayerB?.DoesChunkContainASelectedTile(chunk_point) ?? false;
            if (PointASelected || PointBSelected)
            {
                // Start dragging the tiles
                Classes.Core.SolutionState.Dragged = true;
                Classes.Core.SolutionState.StartDragged = true;
                Classes.Core.Solution.EditLayerA?.StartDrag();
                Classes.Core.Solution.EditLayerB?.StartDrag();
            }
            else
            {
                // Start drag selection
                if (!ShiftPressed() && !CtrlPressed())
                    Controls.Base.MainEditor.Instance.Deselect();
                Controls.Base.MainEditor.Instance.UI.UpdateEditLayerActions();

                Classes.Core.SolutionState.DraggingSelection = true;
                Classes.Core.SolutionState.RegionX2 = e.X;
                Classes.Core.SolutionState.RegionY2 = e.Y;
            }
        }
        public void ChunksEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    Point p = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
                    Point pC = Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinates(p.X, p.Y);

                    if (Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    {
                        int selectedIndex = Controls.Base.MainEditor.Instance.TilesToolbar.ChunkList.SelectedIndex;
                        // Place Stamp
                        if (selectedIndex != -1)
                        {
                            if (!Controls.Base.MainEditor.Instance.Chunks.DoesChunkMatch(pC, Controls.Base.MainEditor.Instance.Chunks.StageStamps.StampList[selectedIndex], Classes.Core.Solution.EditLayerA, Classes.Core.Solution.EditLayerB))
                            {
                                Controls.Base.MainEditor.Instance.Chunks.PasteStamp(pC, selectedIndex, Classes.Core.Solution.EditLayerA, Classes.Core.Solution.EditLayerB);
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
                if (Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    Point p = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
                    Point chunk_point = Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
                    Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

                    // Remove Stamp Sized Area
                    if (!Classes.Core.Solution.EditLayerA.DoesChunkContainASelectedTile(p)) Classes.Core.Solution.EditLayerA?.Select(clicked_chunk);
                    if (Classes.Core.Solution.EditLayerB != null && !Classes.Core.Solution.EditLayerB.DoesChunkContainASelectedTile(p)) Classes.Core.Solution.EditLayerB?.Select(clicked_chunk);
                    Controls.Base.MainEditor.Instance.DeleteSelected();
                }
            }
        }
        public void ChunksEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
            Point chunk_point = Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);
            Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

            Classes.Core.Solution.EditLayerA?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Classes.Core.Solution.EditLayerB?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Controls.Base.MainEditor.Instance.UI.UpdateEditLayerActions();
        }

        #endregion

        #region Interactive Mouse Controls

        public void InteractiveMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.GameHandler.PlayerSelected)
            {
                Methods.GameHandler.MovePlayer(new Point(e.X, e.Y), Classes.Core.SolutionState.Zoom, Methods.GameHandler.SelectedPlayer);
            }

            if (Methods.GameHandler.CheckpointSelected)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
                Methods.GameHandler.UpdateCheckpoint(clicked_point, true);
            }
        }
        public void InteractiveMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Methods.GameHandler.PlayerSelected)
                {
                    Methods.GameHandler.PlayerSelected = false;
                    Methods.GameHandler.SelectedPlayer = 0;
                }
                if (Methods.GameHandler.CheckpointSelected)
                {
                    Methods.GameHandler.CheckpointSelected = false;
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
                Point clicked_point_tile = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
                int tile;
                int tileA = (ushort)(Classes.Core.Solution.EditLayerA?.GetTileAt(clicked_point_tile) & 0x3ff);
                int tileB = 0;
                if (Classes.Core.Solution.EditLayerB != null)
                {
                    tileB = (ushort)(Classes.Core.Solution.EditLayerB?.GetTileAt(clicked_point_tile) & 0x3ff);
                    if (tileA > 1023 && tileB < 1023) tile = tileB;
                    else tile = tileA;
                }
                else tile = tileA;


                Classes.Core.SolutionState.SelectedTileID = tile;
                Controls.Base.MainEditor.Instance.editTile0WithTileManiacToolStripMenuItem.IsEnabled = (tile < 1023);
                Controls.Base.MainEditor.Instance.moveThePlayerToHereToolStripMenuItem.IsEnabled = GameRunning;
                Controls.Base.MainEditor.Instance.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
                Controls.Base.MainEditor.Instance.removeCheckpointToolStripMenuItem.IsEnabled = GameRunning && Methods.GameHandler.CheckpointEnabled;
                Controls.Base.MainEditor.Instance.assetResetToolStripMenuItem.IsEnabled = GameRunning;
                Controls.Base.MainEditor.Instance.restartSceneToolStripMenuItem.IsEnabled = GameRunning;
                Controls.Base.MainEditor.Instance.moveCheckpointToolStripMenuItem.IsEnabled = GameRunning && Methods.GameHandler.CheckpointEnabled;


                Controls.Base.MainEditor.Instance.editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);
                Controls.Base.MainEditor.Instance.ViewPanelContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                Controls.Base.MainEditor.Instance.ViewPanelContextMenu.IsOpen = true;
            }
            else
            {
                Point clicked_point_tile = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
                string tile = "N/A";
                Controls.Base.MainEditor.Instance.editTile0WithTileManiacToolStripMenuItem.IsEnabled = false;
                Controls.Base.MainEditor.Instance.moveThePlayerToHereToolStripMenuItem.IsEnabled = GameRunning;
                Controls.Base.MainEditor.Instance.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
                Controls.Base.MainEditor.Instance.moveCheckpointToolStripMenuItem.IsEnabled = GameRunning;

                Controls.Base.MainEditor.Instance.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = GameRunning;
                Controls.Base.MainEditor.Instance.removeCheckpointToolStripMenuItem.IsEnabled = GameRunning;
                Controls.Base.MainEditor.Instance.assetResetToolStripMenuItem.IsEnabled = GameRunning;
                Controls.Base.MainEditor.Instance.restartSceneToolStripMenuItem.IsEnabled = GameRunning;
                Controls.Base.MainEditor.Instance.moveCheckpointToolStripMenuItem.IsEnabled = GameRunning;

                Controls.Base.MainEditor.Instance.editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);
                Controls.Base.MainEditor.Instance.ViewPanelContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                Controls.Base.MainEditor.Instance.ViewPanelContextMenu.IsOpen = true;
            }
        }

        #endregion

        #region Scroller Mouse Controls
        public void ScrollerMouseMove(MouseEventArgs e)
        {
            if (Classes.Core.SolutionState.WheelClicked)
            {
                Classes.Core.SolutionState.ScrollingDragged = true;

            }

            double xMove = (Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.IsVisible) ? e.X - Classes.Core.SolutionState.ViewPositionX - Classes.Core.SolutionState.ScrollPosition.X : 0;
            double yMove = (Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.IsVisible) ? e.Y - Classes.Core.SolutionState.ViewPositionY - Classes.Core.SolutionState.ScrollPosition.Y : 0;

            if (Math.Abs(xMove) < 15) xMove = 0;
            if (Math.Abs(yMove) < 15) yMove = 0;

            if (xMove > 0)
            {
                if (yMove > 0)
                {
                    Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollSE;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.SE);
                }
                else if (yMove < 0)
                {
                    Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollNE;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.NE);
                }
                else
                {
                    Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollE;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.E);
                }

            }
            else if (xMove < 0)
            {
                if (yMove > 0)
                {
                    Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollSW;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.SW);
                }
                else if (yMove < 0)
                {
                    Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollNW;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.NW);
                }
                else
                {
                    Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollW;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.W);
                }

            }
            else
            {

                if (yMove > 0)
                {
                    Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollS;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.S);
                }
                else if (yMove < 0)
                {
                    Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollN;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.N);
                }
                else
                {
                    if (Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.IsVisible && Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.IsVisible)
                    {
                        Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollAll;
                        SetScrollerBorderApperance((int)ScrollerModeDirection.ALL);
                    }
                    else if (Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.IsVisible)
                    {
                        Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollNS;
                        SetScrollerBorderApperance((int)ScrollerModeDirection.NS);
                    }
                    else if (Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.IsVisible)
                    {
                        Controls.Base.MainEditor.Instance.Cursor = System.Windows.Input.Cursors.ScrollWE;
                        SetScrollerBorderApperance((int)ScrollerModeDirection.WE);
                    }
                }

            }

            System.Windows.Point position = new System.Windows.Point(Classes.Core.SolutionState.ViewPositionX, Classes.Core.SolutionState.ViewPositionY);
            double x = xMove / 10 + position.X;
            double y = yMove / 10 + position.Y;

            Classes.Core.SolutionState.CustomX += (int)xMove / 10;
            Classes.Core.SolutionState.CustomY += (int)yMove / 10;

            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x > Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.Maximum) x = Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.Maximum;
            if (y > Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.Maximum) y = Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.Maximum;


            if (x != position.X || y != position.Y)
            {

                if (Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.IsVisible)
                {
                    Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.Value = y;
                }
                if (Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.IsVisible)
                {
                    Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.Value = x;
                }

                Controls.Base.MainEditor.Instance.DeviceModel.GraphicPanel.OnMouseMoveEventCreate();

            }
            Controls.Base.MainEditor.Instance.DeviceModel.GraphicPanel.Render();

        }

        public void ScrollerMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                if (Core.Settings.MySettings.ScrollerPressReleaseMode) ToggleScrollerMode(e);
            }

        }

        public void ScrollerMouseDown(MouseEventArgs e)
        {

        }

        #endregion

        #region Other Mouse Controls

        public void MouseMovementControls(System.Windows.Forms.MouseEventArgs e)
        {
            if (Classes.Core.SolutionState.DraggingSelection || Classes.Core.SolutionState.Dragged) EdgeMove();
            if (Classes.Core.SolutionState.DraggingSelection) SetSelectionBounds();
            else if (Classes.Core.SolutionState.Dragged) DragMoveItems();


            void EdgeMove()
            {
                System.Windows.Point position = new System.Windows.Point(Classes.Core.SolutionState.ViewPositionX, Classes.Core.SolutionState.ViewPositionY); ;
                double ScreenMaxX = position.X + Controls.Base.MainEditor.Instance.DeviceModel.splitContainer1.Panel1.Width - (int)Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar.ActualWidth;
                double ScreenMaxY = position.Y + Controls.Base.MainEditor.Instance.DeviceModel.splitContainer1.Panel1.Height - (int)Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar.ActualHeight;
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
                if (x > Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.Maximum) x = Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.Maximum;
                if (y > Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.Maximum) y = Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.Maximum;

                if (x != position.X || y != position.Y)
                {
                    if (Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.IsVisible)
                    {
                        Controls.Base.MainEditor.Instance.DeviceModel.vScrollBar1.Value = y;
                    }
                    if (Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.IsVisible)
                    {
                        Controls.Base.MainEditor.Instance.DeviceModel.hScrollBar1.Value = x;
                    }
                    Controls.Base.MainEditor.Instance.DeviceModel.GraphicPanel.OnMouseMoveEventCreate();
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
                    if (Classes.Core.SolutionState.RegionX2 != e.X && Classes.Core.SolutionState.RegionY2 != e.Y)
                    {

                        Classes.Core.SolutionState.TempSelectX1 = (int)(Classes.Core.SolutionState.RegionX2 / Classes.Core.SolutionState.Zoom);
                        Classes.Core.SolutionState.TempSelectX2 = (int)(e.X / Classes.Core.SolutionState.Zoom);
                        Classes.Core.SolutionState.TempSelectY1 = (int)(Classes.Core.SolutionState.RegionY2 / Classes.Core.SolutionState.Zoom);
                        Classes.Core.SolutionState.TempSelectY2 = (int)(e.Y / Classes.Core.SolutionState.Zoom);
                        if (Classes.Core.SolutionState.TempSelectX1 > Classes.Core.SolutionState.TempSelectX2)
                        {
                            Classes.Core.SolutionState.TempSelectX1 = (int)(e.X / Classes.Core.SolutionState.Zoom);
                            Classes.Core.SolutionState.TempSelectX2 = (int)(Classes.Core.SolutionState.RegionX2 / Classes.Core.SolutionState.Zoom);
                        }
                        if (Classes.Core.SolutionState.TempSelectY1 > Classes.Core.SolutionState.TempSelectY2)
                        {
                            Classes.Core.SolutionState.TempSelectY1 = (int)(e.Y / Classes.Core.SolutionState.Zoom);
                            Classes.Core.SolutionState.TempSelectY2 = (int)(Classes.Core.SolutionState.RegionY2 / Classes.Core.SolutionState.Zoom);
                        }

                        Point selectStart = Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(Classes.Core.SolutionState.TempSelectX1, Classes.Core.SolutionState.TempSelectY1);
                        Point selectEnd = Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinatesBottomEdge(Classes.Core.SolutionState.TempSelectX2, Classes.Core.SolutionState.TempSelectY2);

                        Classes.Core.Solution.EditLayerA?.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());
                        Classes.Core.Solution.EditLayerB?.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());

                        Controls.Base.MainEditor.Instance.UI.UpdateTilesOptions();
                    }
                }
                void Normal()
                {
                    if (Classes.Core.SolutionState.RegionX2 != e.X && Classes.Core.SolutionState.RegionY2 != e.Y)
                    {
                        Classes.Core.SolutionState.TempSelectX1 = (int)(Classes.Core.SolutionState.RegionX2 / Classes.Core.SolutionState.Zoom);
                        Classes.Core.SolutionState.TempSelectX2 = (int)(e.X / Classes.Core.SolutionState.Zoom);
                        Classes.Core.SolutionState.TempSelectY1 = (int)(Classes.Core.SolutionState.RegionY2 / Classes.Core.SolutionState.Zoom);
                        Classes.Core.SolutionState.TempSelectY2 = (int)(e.Y / Classes.Core.SolutionState.Zoom);
                        if (Classes.Core.SolutionState.TempSelectX1 > Classes.Core.SolutionState.TempSelectX2)
                        {
                            Classes.Core.SolutionState.TempSelectX1 = (int)(e.X / Classes.Core.SolutionState.Zoom);
                            Classes.Core.SolutionState.TempSelectX2 = (int)(Classes.Core.SolutionState.RegionX2 / Classes.Core.SolutionState.Zoom);
                        }
                        if (Classes.Core.SolutionState.TempSelectY1 > Classes.Core.SolutionState.TempSelectY2)
                        {
                            Classes.Core.SolutionState.TempSelectY1 = (int)(e.Y / Classes.Core.SolutionState.Zoom);
                            Classes.Core.SolutionState.TempSelectY2 = (int)(Classes.Core.SolutionState.RegionY2 / Classes.Core.SolutionState.Zoom);
                        }
                        Classes.Core.Solution.EditLayerA?.TempSelection(new Rectangle(Classes.Core.SolutionState.TempSelectX1, Classes.Core.SolutionState.TempSelectY1, Classes.Core.SolutionState.TempSelectX2 - Classes.Core.SolutionState.TempSelectX1, Classes.Core.SolutionState.TempSelectY2 - Classes.Core.SolutionState.TempSelectY1), CtrlPressed());
                        Classes.Core.Solution.EditLayerB?.TempSelection(new Rectangle(Classes.Core.SolutionState.TempSelectX1, Classes.Core.SolutionState.TempSelectY1, Classes.Core.SolutionState.TempSelectX2 - Classes.Core.SolutionState.TempSelectX1, Classes.Core.SolutionState.TempSelectY2 - Classes.Core.SolutionState.TempSelectY1), CtrlPressed());

                        Controls.Base.MainEditor.Instance.UI.UpdateTilesOptions();

                        if (IsEntitiesEdit()) Classes.Core.Solution.Entities.TempSelection(new Rectangle(Classes.Core.SolutionState.TempSelectX1, Classes.Core.SolutionState.TempSelectY1, Classes.Core.SolutionState.TempSelectX2 - Classes.Core.SolutionState.TempSelectX1, Classes.Core.SolutionState.TempSelectY2 - Classes.Core.SolutionState.TempSelectY1), CtrlPressed());
                    }
                }
            }
            void DragMoveItems()
            {
                int oldGridX = (int)((Classes.Core.SolutionState.LastX / Classes.Core.SolutionState.Zoom) / Classes.Core.SolutionState.MagnetSize) * Classes.Core.SolutionState.MagnetSize;
                int oldGridY = (int)((Classes.Core.SolutionState.LastY / Classes.Core.SolutionState.Zoom) / Classes.Core.SolutionState.MagnetSize) * Classes.Core.SolutionState.MagnetSize;
                int newGridX = (int)((e.X / Classes.Core.SolutionState.Zoom) / Classes.Core.SolutionState.MagnetSize) * Classes.Core.SolutionState.MagnetSize;
                int newGridY = (int)((e.Y / Classes.Core.SolutionState.Zoom) / Classes.Core.SolutionState.MagnetSize) * Classes.Core.SolutionState.MagnetSize;
                Point oldPointGrid = new Point(0, 0);
                Point newPointGrid = new Point(0, 0);
                if (Classes.Core.SolutionState.UseMagnetMode && IsEntitiesEdit())
                {
                    if (Classes.Core.SolutionState.UseMagnetXAxis == true && Classes.Core.SolutionState.UseMagnetYAxis == true)
                    {
                        oldPointGrid = new Point(oldGridX, oldGridY);
                        newPointGrid = new Point(newGridX, newGridY);
                    }
                    if (Classes.Core.SolutionState.UseMagnetXAxis && !Classes.Core.SolutionState.UseMagnetYAxis)
                    {
                        oldPointGrid = new Point(oldGridX, (int)(Classes.Core.SolutionState.LastY / Classes.Core.SolutionState.Zoom));
                        newPointGrid = new Point(newGridX, (int)(e.Y / Classes.Core.SolutionState.Zoom));
                    }
                    if (!Classes.Core.SolutionState.UseMagnetXAxis && Classes.Core.SolutionState.UseMagnetYAxis)
                    {
                        oldPointGrid = new Point((int)(Classes.Core.SolutionState.LastX / Classes.Core.SolutionState.Zoom), oldGridY);
                        newPointGrid = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), newGridY);
                    }
                    if (!Classes.Core.SolutionState.UseMagnetXAxis && !Classes.Core.SolutionState.UseMagnetYAxis)
                    {
                        oldPointGrid = new Point((int)(Classes.Core.SolutionState.LastX / Classes.Core.SolutionState.Zoom), (int)(Classes.Core.SolutionState.LastY / Classes.Core.SolutionState.Zoom));
                        newPointGrid = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));
                    }
                }
                Point oldPoint = new Point((int)(Classes.Core.SolutionState.LastX / Classes.Core.SolutionState.Zoom), (int)(Classes.Core.SolutionState.LastY / Classes.Core.SolutionState.Zoom));
                Point newPoint = new Point((int)(e.X / Classes.Core.SolutionState.Zoom), (int)(e.Y / Classes.Core.SolutionState.Zoom));


                if (!IsChunksEdit())
                {
                    Classes.Core.Solution.EditLayerA?.MoveSelected(oldPoint, newPoint, CtrlPressed());
                    Classes.Core.Solution.EditLayerB?.MoveSelected(oldPoint, newPoint, CtrlPressed());
                }
                else
                {
                    Point oldPointAligned = Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(oldPoint.X, oldPoint.Y);
                    Point newPointAligned = Classes.Core.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(newPoint.X, newPoint.Y);
                    Classes.Core.Solution.EditLayerA?.MoveSelected(oldPointAligned, newPointAligned, CtrlPressed(), true);
                    Classes.Core.Solution.EditLayerB?.MoveSelected(oldPointAligned, newPointAligned, CtrlPressed(), true);
                }


                // FIX: Determine if this is Needed.
                //Editor.Instance.UI.UpdateEditLayerActions();
                if (IsEntitiesEdit())
                {
                    if (Classes.Core.SolutionState.UseMagnetMode)
                    {
                        int x = Classes.Core.Solution.Entities.GetSelectedEntity().Entity.Position.X.High;
                        int y = Classes.Core.Solution.Entities.GetSelectedEntity().Entity.Position.Y.High;

                        if (x % Classes.Core.SolutionState.MagnetSize != 0 && Classes.Core.SolutionState.UseMagnetXAxis)
                        {
                            int offsetX = x % Classes.Core.SolutionState.MagnetSize;
                            oldPointGrid.X -= offsetX;
                        }
                        if (y % Classes.Core.SolutionState.MagnetSize != 0 && Classes.Core.SolutionState.UseMagnetYAxis)
                        {
                            int offsetY = y % Classes.Core.SolutionState.MagnetSize;
                            oldPointGrid.Y -= offsetY;
                        }
                    }


                    try
                    {

                        if (Classes.Core.SolutionState.UseMagnetMode)
                        {
                            Classes.Core.Solution.Entities.MoveSelected(oldPointGrid, newPointGrid, CtrlPressed() && Classes.Core.SolutionState.StartDragged);
                        }
                        else
                        {
                            Classes.Core.Solution.Entities.MoveSelected(oldPoint, newPoint, CtrlPressed() && Classes.Core.SolutionState.StartDragged);
                        }

                    }
                    catch (Classes.Core.Scene.EditorEntities.TooManyEntitiesException)
                    {
                        System.Windows.MessageBox.Show("Too many entities! (limit: 2048)");
                        Classes.Core.SolutionState.Dragged = false;
                        return;
                    }
                    if (Classes.Core.SolutionState.UseMagnetMode)
                    {
                        Classes.Core.SolutionState.DraggedX += newPointGrid.X - oldPointGrid.X;
                        Classes.Core.SolutionState.DraggedY += newPointGrid.Y - oldPointGrid.Y;
                    }
                    else
                    {
                        Classes.Core.SolutionState.DraggedX += newPoint.X - oldPoint.X;
                        Classes.Core.SolutionState.DraggedY += newPoint.Y - oldPoint.Y;
                    }
                    if (CtrlPressed() && Classes.Core.SolutionState.StartDragged)
                    {
                        Controls.Base.MainEditor.Instance.UI.UpdateEntitiesToolbarList();
                        Controls.Base.MainEditor.Instance.UI.SetSelectOnlyButtonsState();
                    }
                    Controls.Base.MainEditor.Instance.EntitiesToolbar.UpdateCurrentEntityProperites();
                }
                Classes.Core.SolutionState.StartDragged = false;
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
                if (IsTilesEdit() && Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Controls.Base.MainEditor.Instance.TilesToolbar.SetSelectTileOption(0, false);
            }
            // Tiles Toolbar Flip Vertical
            else if (isCombo(e, myKeyBinds.FlipVTiles, true))
            {
                if (IsTilesEdit() && Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Controls.Base.MainEditor.Instance.TilesToolbar.SetSelectTileOption(1, false);
            }
        }

        public void GraphicPanel_OnKeyDown(object sender, KeyEventArgs e)
        {
            bool parallaxAnimationInProgress = Classes.Core.SolutionState.AllowAnimations && Classes.Core.SolutionState.ParallaxAnimationChecked;
            if (parallaxAnimationInProgress) return;

            // Faster Nudge Toggle
            if (isCombo(e, myKeyBinds.NudgeFaster))
            {
                Controls.Base.MainEditor.Instance.ToggleFasterNudgeEvent(sender, null);
            }
            // Scroll Lock Toggle
            else if (isCombo(e, myKeyBinds.ScrollLock))
            {
                Classes.Core.SolutionState.ScrollLocked ^= true;
            }
            // Switch Scroll Lock Type
            else if (isCombo(e, myKeyBinds.ScrollLockTypeSwitch))
            {
                Controls.Base.MainEditor.Instance.UIEvents.SetScrollLockDirection();

            }
            // Tiles Toolbar Flip Vertical
            else if (isCombo(e, myKeyBinds.FlipVTiles, true))
            {
                if (IsTilesEdit() && Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Controls.Base.MainEditor.Instance.TilesToolbar.SetSelectTileOption(1, true);
            }
            // Tiles Toolbar Flip Horizontal
            else if (isCombo(e, myKeyBinds.FlipHTiles, true))
            {
                if (IsTilesEdit() && Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Controls.Base.MainEditor.Instance.TilesToolbar.SetSelectTileOption(0, true);
            }
            // Open Click (Alt: Open Data Dir)
            else if ((isCombo(e, myKeyBinds.OpenDataDir)))
            {
                Controls.Base.MainEditor.Instance.OpenDataDirectoryEvent(null, null);
            }
            else if ((isCombo(e, myKeyBinds.Open)))
            {
                Controls.Base.MainEditor.Instance.OpenSceneEvent(null, null);
            }
            // New Click
            else if (isCombo(e, myKeyBinds.New))
            {
                //Editor.Instance.New_Click(null, null);
            }
            // Save Click (Alt: Save As)
            else if (isCombo(e, myKeyBinds.SaveAs))
            {
                Controls.Base.MainEditor.Instance.SaveSceneAsEvent(null, null);
            }
            else if (isCombo(e, myKeyBinds._Save))
            {
                Controls.Base.MainEditor.Instance.SaveSceneEvent(null, null);
            }
            // Undo
            else if (isCombo(e, myKeyBinds.Undo))
            {
                Controls.Base.MainEditor.Instance.EditorUndo();
            }
            // Redo
            else if (isCombo(e, myKeyBinds.Redo))
            {
                Controls.Base.MainEditor.Instance.EditorRedo();
            }
            // Developer Interface
            else if (isCombo(e, myKeyBinds.DeveloperInterface))
            {
                Controls.Base.MainEditor.Instance.EditorUndo();
            }
            // Save for Force Open on Startup
            else if (isCombo(e, myKeyBinds.ForceOpenOnStartup))
            {
                Controls.Base.MainEditor.Instance.EditorRedo();
            }
            else if (Controls.Base.MainEditor.Instance.IsSceneLoaded())
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
                Controls.Base.MainEditor.Instance.ZoomModel.SetZoomLevel(0, new Point(0, 0));
            }
            //Refresh Tiles and Sprites
            else if (isCombo(e, myKeyBinds.RefreshResources))
            {
                Controls.Base.MainEditor.Instance.ReloadToolStripButton_Click(null, null);
            }
            //Run Scene
            else if (isCombo(e, myKeyBinds.RunScene))
            {
                Methods.GameHandler.RunScene();
            }
            //Show Path A
            else if (isCombo(e, myKeyBinds.ShowPathA) && Controls.Base.MainEditor.Instance.IsSceneLoaded())
            {
                Controls.Base.MainEditor.Instance.ShowCollisionAEvent(null, null);
            }
            //Show Path B
            else if (isCombo(e, myKeyBinds.ShowPathB))
            {
                Controls.Base.MainEditor.Instance.ShowCollisionBEvent(null, null);
            }
            //Unload Scene
            else if (isCombo(e, myKeyBinds.UnloadScene))
            {
                Controls.Base.MainEditor.Instance.EditorMenuBar.UnloadSceneEvent(null, null);
            }
            //Toggle Grid Visibility
            else if (isCombo(e, myKeyBinds.ShowGrid))
            {
                Controls.Base.MainEditor.Instance.ToggleGridEvent(null, null);
            }
            //Toggle Tile ID Visibility
            else if (isCombo(e, myKeyBinds.ShowTileID))
            {
                Controls.Base.MainEditor.Instance.ToggleSlotIDEvent(null, null);
            }
            //Refresh Tiles and Sprites
            else if (isCombo(e, myKeyBinds.StatusBoxToggle))
            {
                Controls.Base.MainEditor.Instance.ToggleDebugHUDEvent(null, null);
            }
        }

        public void GraphicPanel_OnKeyDownEditing(object sender, KeyEventArgs e)
        {
            //Paste
            if (isCombo(e, myKeyBinds.Paste))
            {
                Controls.Base.MainEditor.Instance.PasteEvent(sender, null);
            }
            //Paste to Chunk
            if (isCombo(e, myKeyBinds.PasteToChunk))
            {
                Controls.Base.MainEditor.Instance.PasteToChunksEvent(sender, null);
            }
            //Select All
            if (isCombo(e, myKeyBinds.SelectAll))
            {
                Controls.Base.MainEditor.Instance.SelectAllEvent(sender, null);
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
                Controls.Base.MainEditor.Instance.DeleteSelected();
            }

            // Moving
            else if (e.KeyData == Keys.Up || e.KeyData == Keys.Down || e.KeyData == Keys.Left || e.KeyData == Keys.Right)
            {
                Controls.Base.MainEditor.Instance.MoveEntityOrTiles(sender, e);
            }

            //Cut 
            if (isCombo(e, myKeyBinds.Cut))
            {
                Controls.Base.MainEditor.Instance.CutEvent(sender, null);
            }
            //Copy
            else if (isCombo(e, myKeyBinds.Copy))
            {
                Controls.Base.MainEditor.Instance.CopyEvent(sender, null);
            }
            //Duplicate
            else if (isCombo(e, myKeyBinds.Duplicate))
            {
                Controls.Base.MainEditor.Instance.DuplicateEvent(sender, null);
            }
            // Flip Vertical Individual
            else if (isCombo(e, myKeyBinds.FlipVIndv))
            {
                if (IsTilesEdit())
                    Controls.Base.MainEditor.Instance.FlipVerticalIndividualEvent(sender, null);
            }
            // Flip Horizontal Individual
            else if (isCombo(e, myKeyBinds.FlipHIndv))
            {
                if (IsTilesEdit())
                    Controls.Base.MainEditor.Instance.FlipHorizontalIndividualEvent(sender, null);
            }
            // Flip Vertical
            else if (isCombo(e, myKeyBinds.FlipV))
            {
                if (IsTilesEdit())
                    Controls.Base.MainEditor.Instance.FlipVerticalEvent(sender, null);
                else if (IsEntitiesEdit())
                    Controls.Base.MainEditor.Instance.FlipEntities(FlipDirection.Veritcal);
            }

            // Flip Horizontal
            else if (isCombo(e, myKeyBinds.FlipH))
            {
                if (IsTilesEdit())
                    Controls.Base.MainEditor.Instance.FlipHorizontalEvent(sender, null);
                else if (IsEntitiesEdit())
                    Controls.Base.MainEditor.Instance.FlipEntities(FlipDirection.Horizontal);
            }
        }

        public void OnKeyDownTools(object sender, KeyEventArgs e)
        {
            if (isCombo(e, myKeyBinds.PointerTool) && Controls.Base.MainEditor.Instance.EditorToolbar.PointerToolButton.IsEnabled) Classes.Core.SolutionState.PointerMode(true);
            else if (isCombo(e, myKeyBinds.SelectTool) && Controls.Base.MainEditor.Instance.EditorToolbar.SelectToolButton.IsEnabled) Classes.Core.SolutionState.SelectionMode(true);
            else if (isCombo(e, myKeyBinds.DrawTool) && Controls.Base.MainEditor.Instance.EditorToolbar.DrawToolButton.IsEnabled) Classes.Core.SolutionState.DrawMode(true);
            else if (isCombo(e, myKeyBinds.MagnetTool) && Controls.Base.MainEditor.Instance.EditorToolbar.MagnetMode.IsEnabled) Classes.Core.SolutionState.UseMagnetMode ^= true;
            else if (isCombo(e, myKeyBinds.SplineTool) && Controls.Base.MainEditor.Instance.EditorToolbar.SplineToolButton.IsEnabled) Classes.Core.SolutionState.SplineMode(true);
            else if (isCombo(e, myKeyBinds.StampTool) && Controls.Base.MainEditor.Instance.EditorToolbar.ChunksToolButton.IsEnabled) Classes.Core.SolutionState.ChunksMode();

        }
        #endregion

        #region Tile Maniac Controls

        public void TileManiac_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (isCombo(e, myKeyBinds.TileManiacNewInstance))
            {
                CollisionEditor.Instance.newInstanceToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacOpen))
            {
                CollisionEditor.Instance.OpenToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacSave))
            {
                CollisionEditor.Instance.saveToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacSaveAs))
            {
                CollisionEditor.Instance.saveAsToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacSaveUncompressed))
            {
                CollisionEditor.Instance.saveUncompressedToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacSaveAsUncompressed))
            {
                CollisionEditor.Instance.saveAsUncompressedToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacbackupConfig))
            {
                CollisionEditor.Instance.tileConfigbinToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacbackupImage))
            {
                CollisionEditor.Instance.x16TilesgifToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacExportColMask))
            {
                CollisionEditor.Instance.exportCurrentCollisionMaskAsToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacOpenSingleColMask))
            {
                CollisionEditor.Instance.openSingleCollisionMaskToolStripMenuItem_Click_1(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacImportFromOlderRSDK))
            {
                CollisionEditor.Instance.importFromOlderRSDKVersionToolStripMenuItem_Click(null, null);
            }

            if (isCombo(e, myKeyBinds.TileManiacCopy))
            {
                CollisionEditor.Instance.copyToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacPastetoOther))
            {
                CollisionEditor.Instance.copyToOtherPathToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacPaste))
            {
                CollisionEditor.Instance.pasteToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacMirrorMode))
            {
                CollisionEditor.Instance.mirrorPathsToolStripMenuItem1.IsChecked = !CollisionEditor.Instance.mirrorPathsToolStripMenuItem1.IsChecked;
                CollisionEditor.Instance.mirrorPathsToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacRestorePathA))
            {
                CollisionEditor.Instance.pathAToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacRestorePathB))
            {
                CollisionEditor.Instance.pathBToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacRestorePaths))
            {
                CollisionEditor.Instance.bothToolStripMenuItem_Click(null, null);
            }

            if (isCombo(e, myKeyBinds.TileManiacShowPathB))
            {
                CollisionEditor.Instance.showPathBToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacShowGrid))
            {
                CollisionEditor.Instance.showGridToolStripMenuItem.IsChecked = !CollisionEditor.Instance.showGridToolStripMenuItem.IsChecked;
                CollisionEditor.Instance.showGridToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacClassicMode))
            {
                CollisionEditor.Instance.classicViewModeToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacWindowAlwaysOnTop))
            {
                CollisionEditor.Instance.windowAlwaysOnTop.IsChecked = !CollisionEditor.Instance.windowAlwaysOnTop.IsChecked;
                CollisionEditor.Instance.WindowAlwaysOnTop_Click(null, null);
            }

            if (isCombo(e, myKeyBinds.TileManiacSplitFile))
            {
                CollisionEditor.Instance.splitFileToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacFlipTileH))
            {
                CollisionEditor.Instance.flipTileHorizontallyToolStripMenuItem_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacFlipTileV))
            {
                CollisionEditor.Instance.flipTileVerticallyToolStripMenuItem_Click(null, null);
            }

            if (isCombo(e, myKeyBinds.TileManiacHomeFolderOpen))
            {
                CollisionEditor.Instance.openCollisionHomeFolderToolStripMenuItem_Click(null, null);
            }

            if (isCombo(e, myKeyBinds.TileManiacAbout))
            {
                CollisionEditor.Instance.aboutToolStripMenuItem1_Click(null, null);
            }
            if (isCombo(e, myKeyBinds.TileManiacSettings))
            {
                CollisionEditor.Instance.settingsToolStripMenuItem_Click(null, null);
            }
        }

        public void TileManiac_OnKeyUp(object sender, KeyEventArgs e)
        {

        }

        public void TileManiac_UpdateMenuItems()
        {
            CollisionEditor.Instance.newInstanceMenuItem.InputGestureText = KeyBindPraser("NewInstance");
            CollisionEditor.Instance.openMenuItem.InputGestureText = KeyBindPraser("TileManiacOpen");
            CollisionEditor.Instance.saveMenuItem.InputGestureText = KeyBindPraser("TileManiacSave");
            CollisionEditor.Instance.saveAsMenuItem.InputGestureText = KeyBindPraser("TileManiacSaveAs");
            CollisionEditor.Instance.saveAsUncompressedMenuItem.InputGestureText = KeyBindPraser("TileManiacSaveAsUncompressed");
            CollisionEditor.Instance.saveUncompressedMenuItem.InputGestureText = KeyBindPraser("TileManiacSaveUncompressed");
            CollisionEditor.Instance.backupTilesConfigMenuItem.InputGestureText = KeyBindPraser("TileManiacbackupConfig", false, true);
            CollisionEditor.Instance.backupTilesMenuItem.InputGestureText = KeyBindPraser("TileManiacbackupImage", false, true);
            CollisionEditor.Instance.importMenuItem.InputGestureText = KeyBindPraser("TileManiacImportFromOlderRSDK", false, true);
            CollisionEditor.Instance.OpenSingleColMaskMenuItem.InputGestureText = KeyBindPraser("TileManiacOpenSingleColMask", false, true);
            CollisionEditor.Instance.exportCurrentMaskMenuItem.InputGestureText = KeyBindPraser("TileManiacExportColMask", false, true);

            CollisionEditor.Instance.copyMenuItem.InputGestureText = KeyBindPraser("TileManiacCopy");
            CollisionEditor.Instance.copyToOtherPathMenuItem.InputGestureText = KeyBindPraser("TileManiacPastetoOther");
            CollisionEditor.Instance.pasteMenuItem.InputGestureText = KeyBindPraser("TileManiacPaste");
            CollisionEditor.Instance.mirrorPathsToolStripMenuItem1.InputGestureText = KeyBindPraser("TileManiacMirrorMode");
            CollisionEditor.Instance.restorePathAMenuItem.InputGestureText = KeyBindPraser("TileManiacRestorePathA", false, true);
            CollisionEditor.Instance.restorePathBMenuItem.InputGestureText = KeyBindPraser("TileManiacRestorePathB", false, true);
            CollisionEditor.Instance.restoreBothMenuItem.InputGestureText = KeyBindPraser("TileManiacRestorePaths", false, true);

            CollisionEditor.Instance.showPathBToolStripMenuItem.InputGestureText = KeyBindPraser("TileManiacShowPathB");
            CollisionEditor.Instance.showGridToolStripMenuItem.InputGestureText = KeyBindPraser("TileManiacShowGrid");
            CollisionEditor.Instance.classicViewModeToolStripMenuItem.InputGestureText = KeyBindPraser("TileManiacClassicMode", false, true);
            CollisionEditor.Instance.windowAlwaysOnTop.InputGestureText = KeyBindPraser("TileManiacWindowAlwaysOnTop");


            CollisionEditor.Instance.splitFileMenuItem.InputGestureText = KeyBindPraser("TileManiacSplitFile", false, true);
            CollisionEditor.Instance.flipTileHMenuItem.InputGestureText = KeyBindPraser("TileManiacFlipTileH", false, true);
            CollisionEditor.Instance.flipTileVMenuItem.InputGestureText = KeyBindPraser("TileManiacFlipTileV", false, true);

            CollisionEditor.Instance.openCollisionHomeFolderToolStripMenuItem.InputGestureText = KeyBindPraser("TileManiacHomeFolderOpen", false, true);

            CollisionEditor.Instance.aboutMenuItem.InputGestureText = KeyBindPraser("TileManiacAbout", false, true);
            CollisionEditor.Instance.settingsMenuItem.InputGestureText = KeyBindPraser("TileManiacSettings", false, true);
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

            if (!Extensions.Extensions.KeyBindsSettingExists(keyRefrence)) return nullString;

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
            Controls.Base.MainEditor.Instance.EditorStatusBar.UpdateTooltips();
            Controls.Base.MainEditor.Instance.EditorToolbar.UpdateTooltips();

        }

        #endregion
        #endregion
    }
}
