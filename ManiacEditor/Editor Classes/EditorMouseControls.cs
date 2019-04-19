using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ManiacEditor.Actions;
using RSDKv5;

namespace ManiacEditor
{
    public class EditorMouseControls
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

        private int SelectionY2 { get => Editor.Instance.StateModel.SelectionY2; set => Editor.Instance.StateModel.SelectionY2 = value; }
        private int SelectionX2 { get => Editor.Instance.StateModel.SelectionX2; set => Editor.Instance.StateModel.SelectionX2 = value; }
        private int SelectionY1 { get => Editor.Instance.StateModel.SelectionY1; set => Editor.Instance.StateModel.SelectionY1 = value; }
        private int SelectionX1 { get => Editor.Instance.StateModel.SelectionX1; set => Editor.Instance.StateModel.SelectionX1 = value; }

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
        private bool IsEntitiesEdit() { return Editor.Instance.IsEntitiesEdit(); }
        private bool IsEditing() { return Editor.Instance.IsEditing(); }
        private bool IsSceneLoaded() { return Editor.Instance.IsSceneLoaded(); }

        private bool scrollingDragged { get => Editor.Instance.StateModel.scrollingDragged; set => Editor.Instance.StateModel.scrollingDragged = value; }
        private bool scrolling { get => Editor.Instance.StateModel.scrolling; set => Editor.Instance.StateModel.scrolling = value; }
        private bool isTileDrawing { get => Editor.Instance.StateModel.isTileDrawing; set => Editor.Instance.StateModel.isTileDrawing = value; }
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

        //Shorthanding Settings
        readonly Properties.Settings mySettings = Properties.Settings.Default;
        Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;
        public EditorMouseControls()
        {

        }
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
        public void UpdateUndoRedo()
        {
            if (IsEntitiesEdit())
            {
                if (Editor.Instance.Entities.SelectedEntities.Count > 0)
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
                if (Editor.Instance.Entities.SelectedInternalEntities.Count > 0)
                {
                    IAction action = new ActionMoveEntities(Editor.Instance.Entities.SelectedInternalEntities.ToList(), new Point(draggedX, draggedY));
                    if (Editor.Instance.Entities.LastActionInternal != null)
                    {
                        // If it is move & duplicate, merge them together
                        var taction = new ActionsGroup();
                        taction.AddAction(Editor.Instance.Entities.LastActionInternal);
                        Editor.Instance.Entities.LastActionInternal = null;
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
        #region Mouse Down Controls
        public void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!scrolling) Editor.Instance.FormsModel.GraphicPanel.Focus();

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
            if (IsEditing() && !dragged)
            {
                if (IsTilesEdit() && !Editor.Instance.InteractionToolButton.IsChecked.Value && !IsChunksEdit()) TilesEditMouseDown(e);
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
            if (scrolling || scrollingDragged || draggingSelection || dragged) Editor.Instance.FormsModel.GraphicPanel.Render();

            UpdatePositionLabel(e);           

            if (GameRunning) InteractiveMouseMove(e);

            if (SelectionX1 != -1)
            {
                if (IsTilesEdit() && !Editor.Instance.InteractionToolButton.IsChecked.Value && !IsChunksEdit()) TilesEditMouseMoveDraggingStarted(e);
                else if (IsChunksEdit()) ChunksEditMouseMoveDraggingStarted(e);
                else if (IsEntitiesEdit()) EntitiesEditMouseMoveDraggingStarted(e);

                SelectionX1 = -1;
                SelectionY1 = -1;
            }

            if (scrolling) ScrollerMouseMove(e);
            else if (e.Button == MouseButtons.Middle) EnforceCursorPosition();

            if (IsTilesEdit() && !IsChunksEdit()) TilesEditMouseMove(e);
            else if (IsChunksEdit()) ChunksEditMouseMove(e);
            else if (IsEntitiesEdit()) EntitiesEditMouseMove(e);

            MouseMovementControls(e);

            lastX = e.X;
            lastY = e.Y;
        }
        #region Mouse Up Controls
        public void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isTileDrawing = false;
            if (draggingSelection) MouseUpDraggingSelection(e);
            else
            {
                if (SelectionX1 != -1)
                {
                    // So it was just click
                    if (e.Button == MouseButtons.Left)
                    {
                        if (IsTilesEdit() && !IsChunksEdit()) TilesEditMouseUp(e);
                        else if (IsChunksEdit()) ChunksEditMouseUp(e);
                        else if (IsEntitiesEdit()) EntitiesEditMouseUp(e);
                    }
                    Editor.Instance.UI.SetSelectOnlyButtonsState();
                    SelectionX1 = -1;
                    SelectionY1 = -1;
                }
                if (dragged && (draggedX != 0 || draggedY != 0)) UpdateUndoRedo();
                dragged = false;
            }
            ScrollerMouseUp(e);

            Editor.Instance.UI.UpdateEditLayerActions();
            Editor.Instance.UI.UpdateControls();


        }
        public void MouseUpDraggingSelection(System.Windows.Forms.MouseEventArgs e)
        {
            if (SelectionX2 != e.X && SelectionY2 != e.Y)
            {
                int x1 = (int)(SelectionX2 / Zoom), x2 = (int)(e.X / Zoom);
                int y1 = (int)(SelectionY2 / Zoom), y2 = (int)(e.Y / Zoom);
                if (x1 > x2)
                {
                    x1 = (int)(e.X / Zoom);
                    x2 = (int)(SelectionX2 / Zoom);
                }
                if (y1 > y2)
                {
                    y1 = (int)(e.Y / Zoom);
                    y2 = (int)(SelectionY2 / Zoom);
                }

                if (IsChunksEdit())
                {
                    Point selectStart = EditorLayer.GetChunkCoordinatesTopEdge(select_x1, select_y1);
                    Point selectEnd = EditorLayer.GetChunkCoordinatesBottomEdge(select_x2, select_y2);

                    Editor.Instance.EditLayerA?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                    Editor.Instance.EditLayerB?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                else
                {
                    Editor.Instance.EditLayerA?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                    Editor.Instance.EditLayerB?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());

                    if (IsEntitiesEdit()) Editor.Instance.Entities.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                Editor.Instance.UI.SetSelectOnlyButtonsState();
                Editor.Instance.UI.UpdateEditLayerActions();

            }
            draggingSelection = false;
            Editor.Instance.EditLayerA?.EndTempSelection();
            Editor.Instance.EditLayerB?.EndTempSelection();

            if (IsEntitiesEdit()) Editor.Instance.Entities.EndTempSelection();
        }

        #endregion
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
            Editor.Instance.FormsModel.GraphicPanel.Focus();
            if (e.Button == MouseButtons.Right)
            {
                if (Editor.Instance.InteractionToolButton.IsChecked.Value) InteractiveContextMenu(e);
                else if (IsEntitiesEdit() && !Editor.Instance.DrawToolButton.IsChecked.Value && !Editor.Instance.SplineToolButton.IsChecked.Value && (!Editor.Instance.UIModes.RightClicktoSwapSlotID || Editor.Instance.Entities.SelectedEntities.Count <= 1)) EntitiesEditContextMenu(e);
                else if (IsTilesEdit() && !Editor.Instance.DrawToolButton.IsChecked.Value) TilesEditContextMenu(e);
            }

        }
        public void SetClickedXY(System.Windows.Forms.MouseEventArgs e) { SelectionX1 = e.X; SelectionY1 = e.Y; }
        public void SetClickedXY(Point e) { SelectionX1 = e.X; SelectionY1 = e.Y; }


        #region Tiles Edit Mouse Controls

        public void TilesEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Editor.Instance.DrawToolButton.IsChecked.Value)
            {
                TilesEditDrawTool(e, false);
            }
        }
        public void TilesEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(SelectionX1 / Zoom), (int)(SelectionY1 / Zoom));
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
                SelectionX2 = SelectionX1;
                SelectionY2 = SelectionY1;
            }
        }
        public void TilesEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                if (Editor.Instance.DrawToolButton.IsChecked.Value)
                {
                    TilesEditDrawTool(e, true);
                }
                else SetClickedXY(e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Editor.Instance.DrawToolButton.IsChecked.Value)
                {
                    TilesEditDrawTool(e, true);
                }
            }
        }
        public void TilesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
            Editor.Instance.EditLayerA?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Editor.Instance.EditLayerB?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
        }
        public void TilesEditContextMenu(System.Windows.Forms.MouseEventArgs e)
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

        #region Universal Tool Actions

        public void TilesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
        {
            Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
            if (click)
            {
                if (e.Button == MouseButtons.Left)
                {
                    isTileDrawing = true;
                    PlaceTile();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    isTileDrawing = true;
                    RemoveTile();
                }
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    isTileDrawing = true;
                    PlaceTile();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    isTileDrawing = true;
                    RemoveTile();
                }
            }

            void RemoveTile()
            {
                // Remove tile
                if (Editor.Instance.UIModes.DrawBrushSize == 1)
                {
                    Editor.Instance.EditLayerA?.Select(p);
                    Editor.Instance.EditLayerB?.Select(p);
                    Editor.Instance.DeleteSelected();
                } 
                else
                {
                    double size = (Editor.Instance.UIModes.DrawBrushSize / 2) * EditorConstants.TILE_SIZE;
                    Editor.Instance.EditLayerA?.Select(new Rectangle((int)(p.X - size), (int)(p.Y - size), Editor.Instance.UIModes.DrawBrushSize * EditorConstants.TILE_SIZE, Editor.Instance.UIModes.DrawBrushSize * EditorConstants.TILE_SIZE));
                    Editor.Instance.EditLayerB?.Select(new Rectangle((int)(p.X - size), (int)(p.Y - size), Editor.Instance.UIModes.DrawBrushSize * EditorConstants.TILE_SIZE, Editor.Instance.UIModes.DrawBrushSize * EditorConstants.TILE_SIZE));
                    Editor.Instance.DeleteSelected();
                }
            }

            void PlaceTile()
            {
                if (Editor.Instance.UIModes.DrawBrushSize == 1)
                {
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
                else
                {
                    if (Editor.Instance.TilesToolbar.SelectedTile != -1)
                    {
                        Editor.Instance.EditorPlaceTile(p, Editor.Instance.TilesToolbar.SelectedTile, Editor.Instance.EditLayerA, true);
                    }
                }
            }
        }

        #endregion


        #endregion

        #region Entities Edit Mouse Controls

        public void EntitiesEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Editor.Instance.DrawToolButton.IsChecked.Value) EntitiesEditDrawTool(e);
        }
        public void EntitiesEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(SelectionX1 / Zoom), (int)(SelectionY1 / Zoom));
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
                SelectionX2 = SelectionX1;
                SelectionY2 = SelectionY1;

            }
        }
        public void EntitiesEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!Editor.Instance.DrawToolButton.IsChecked.Value)
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
                else if (Editor.Instance.DrawToolButton.IsChecked.Value) EntitiesEditDrawTool(e, true);
            }
            if (Editor.Instance.SplineToolButton.IsChecked.Value) SplineTool(e);
        }
        public void EntitiesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
            if (e.Button == MouseButtons.Left)
            {
                Editor.Instance.Entities.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Editor.Instance.Entities.SelectedEntities.Count == 2 && Editor.Instance.UIModes.RightClicktoSwapSlotID)
                {
                    Editor.Instance.Entities.SwapSlotIDsFromPair();
                }
            }
        }
        public void EntitiesEditContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
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


        #region Universal Tool Actions

        public void EntitiesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
        {
            if (click)
            {
                lastX = e.X;
                lastY = e.Y;
            }
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                if (Editor.Instance.Entities.IsEntityAt(clicked_point, true) == true)
                {
                    Editor.Instance.Deselect();
                    Editor.Instance.Entities.GetEntityAt(clicked_point).Selected = true;
                }
                else
                {
                    Editor.Instance.EntitiesToolbar.SpawnObject();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                if (Editor.Instance.Entities.IsEntityAt(clicked_point, true) == true)
                {
                    Editor.Instance.Deselect();
                    Editor.Instance.Entities.GetEntityAt(clicked_point).Selected = true;
                    Editor.Instance.Entities.DeleteSelected();
                    Editor.Instance.UpdateLastEntityAction();
                }
            }
        }

        public void SplineTool(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                if (Editor.Instance.Entities.IsEntityAt(clicked_point) == true)
                {
                    Editor.Instance.Deselect();
                    Editor.Instance.Entities.GetEntityAt(clicked_point).Selected = true;
                }
                else
                {
                    Editor.Instance.Entities.SpawnInternalSplineObject(new Position((short)clicked_point.X, (short)clicked_point.Y));
                    Editor.Instance.UpdateLastEntityAction();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                EditorEntity atPoint = Editor.Instance.Entities.GetEntityAt(clicked_point);
                if (atPoint != null && atPoint.Entity.Object.Name.Name == "Spline")
                {
                    Editor.Instance.Deselect();
                    Editor.Instance.Entities.GetEntityAt(clicked_point).Selected = true;
                    Editor.Instance.Entities.DeleteInternallySelected();
                    Editor.Instance.UpdateLastEntityAction();
                }
            }
        }

        #endregion

        #endregion

        #region Chunks Edit Mouse Controls

        public void ChunksEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
            Point pC = EditorLayer.GetChunkCoordinates(p.X, p.Y);

            if (e.Button == MouseButtons.Left)
            {
                if (Editor.Instance.DrawToolButton.IsChecked.Value)
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
                if (Editor.Instance.DrawToolButton.IsChecked.Value)
                {

                    if (!Editor.Instance.Chunks.IsChunkEmpty(pC, Editor.Instance.EditLayerA, Editor.Instance.EditLayerB))
                    {
                        // Remove Stamp Sized Area
                        Editor.Instance.Chunks.PasteStamp(pC, 0, Editor.Instance.EditLayerA, Editor.Instance.EditLayerB, true);
                    }
                }

            }
        }
        public void ChunksEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
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
                SelectionX2 = e.X;
                SelectionY2 = e.Y;
            }
        }
        public void ChunksEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Editor.Instance.DrawToolButton.IsChecked.Value)
                {
                    Point p = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
                    Point pC = EditorLayer.GetChunkCoordinates(p.X, p.Y);

                    if (Editor.Instance.DrawToolButton.IsChecked.Value)
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
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Editor.Instance.DrawToolButton.IsChecked.Value)
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
        }
        public void ChunksEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Zoom), (int)(e.Y / Zoom));
            Point chunk_point = EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);
            Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

            Editor.Instance.EditLayerA?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Editor.Instance.EditLayerB?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Editor.Instance.UI.UpdateEditLayerActions();
        }

        #endregion

        #region Interactive Mouse Controls

        public void InteractiveMouseMove(System.Windows.Forms.MouseEventArgs e)
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

        #endregion

        #region Scroller Mouse Controls
        public void ScrollerMouseMove(MouseEventArgs e)
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
            if (draggingSelection || Editor.Instance.StateModel.dragged) EdgeMove();
            if (draggingSelection) SetSelectionBounds();
            else if (Editor.Instance.StateModel.dragged) DragMoveItems();


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
                    if (SelectionX2 != e.X && SelectionY2 != e.Y)
                    {

                        select_x1 = (int)(SelectionX2 / Zoom);
                        select_x2 = (int)(e.X / Zoom);
                        select_y1 = (int)(SelectionY2 / Zoom);
                        select_y2 = (int)(e.Y / Zoom);
                        if (select_x1 > select_x2)
                        {
                            select_x1 = (int)(e.X / Zoom);
                            select_x2 = (int)(SelectionX2 / Zoom);
                        }
                        if (select_y1 > select_y2)
                        {
                            select_y1 = (int)(e.Y / Zoom);
                            select_y2 = (int)(SelectionY2 / Zoom);
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
                    if (SelectionX2 != e.X && SelectionY2 != e.Y)
                    {
                        select_x1 = (int)(SelectionX2 / Zoom);
                        select_x2 = (int)(e.X / Zoom);
                        select_y1 = (int)(SelectionY2 / Zoom);
                        select_y2 = (int)(e.Y / Zoom);
                        if (select_x1 > select_x2)
                        {
                            select_x1 = (int)(e.X / Zoom);
                            select_x2 = (int)(SelectionX2 / Zoom);
                        }
                        if (select_y1 > select_y2)
                        {
                            select_y1 = (int)(e.Y / Zoom);
                            select_y2 = (int)(SelectionY2 / Zoom);
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
                        int x = Editor.Instance.Entities.GetSelectedEntity().Entity.Position.X.High;
                        int y = Editor.Instance.Entities.GetSelectedEntity().Entity.Position.Y.High;

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

        #endregion
    }
}
