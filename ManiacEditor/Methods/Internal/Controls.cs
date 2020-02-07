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
using ManiacEditor.Controls.TileManiac;
using ManiacEditor.Controls.Editor;


namespace ManiacEditor.Methods.Internal
{
    public static class Controls
    {
        private static MainEditor Instance;
        public static void UpdateInstance(MainEditor _instance)
        {
            Instance = _instance;
        }

        private static bool CtrlPressed() { return System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Control); }
        private static bool ShiftPressed() { return System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Shift); }

        #region Mouse Controls

        #region Scroller Mode Methods
        private static bool ForceUpdateScrollerMousePos { get; set; } = false;
        public static void SetScrollerBorderApperance(int direction = -1)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var Active = (System.Windows.Media.Brush)converter.ConvertFromString("Red");
            var NotActive = (System.Windows.Media.Brush)converter.ConvertFromString("Transparent");

            Instance.ViewPanel.ScrollGrid.ScrollBorderN.Fill = NotActive;
            Instance.ViewPanel.ScrollGrid.ScrollBorderS.Fill = NotActive;
            Instance.ViewPanel.ScrollGrid.ScrollBorderE.Fill = NotActive;
            Instance.ViewPanel.ScrollGrid.ScrollBorderW.Fill = NotActive;
            Instance.ViewPanel.ScrollGrid.ScrollBorderNW.Fill = NotActive;
            Instance.ViewPanel.ScrollGrid.ScrollBorderSW.Fill = NotActive;
            Instance.ViewPanel.ScrollGrid.ScrollBorderSE.Fill = NotActive;
            Instance.ViewPanel.ScrollGrid.ScrollBorderNE.Fill = NotActive;

            switch (direction)
            {
                case 0:
                    Instance.ViewPanel.ScrollGrid.ScrollBorderN.Fill = Active;
                    break;
                case 1:
                    Instance.ViewPanel.ScrollGrid.ScrollBorderNE.Fill = Active;
                    break;
                case 2:
                    Instance.ViewPanel.ScrollGrid.ScrollBorderE.Fill = Active;
                    break;
                case 3:
                    Instance.ViewPanel.ScrollGrid.ScrollBorderSE.Fill = Active;
                    break;
                case 4:
                    Instance.ViewPanel.ScrollGrid.ScrollBorderS.Fill = Active;
                    break;
                case 5:
                    Instance.ViewPanel.ScrollGrid.ScrollBorderSW.Fill = Active;
                    break;
                case 6:
                    Instance.ViewPanel.ScrollGrid.ScrollBorderW.Fill = Active;
                    break;
                case 7:
                    Instance.ViewPanel.ScrollGrid.ScrollBorderNW.Fill = Active;
                    break;
                case 8:
                    Instance.ViewPanel.ScrollGrid.ScrollBorderW.Fill = Active;
                    Instance.ViewPanel.ScrollGrid.ScrollBorderE.Fill = Active;
                    break;
                case 9:
                    Instance.ViewPanel.ScrollGrid.ScrollBorderN.Fill = Active;
                    Instance.ViewPanel.ScrollGrid.ScrollBorderS.Fill = Active;
                    break;
                case 10:
                    Instance.ViewPanel.ScrollGrid.ScrollBorderN.Fill = Active;
                    Instance.ViewPanel.ScrollGrid.ScrollBorderS.Fill = Active;
                    Instance.ViewPanel.ScrollGrid.ScrollBorderE.Fill = Active;
                    Instance.ViewPanel.ScrollGrid.ScrollBorderW.Fill = Active;
                    Instance.ViewPanel.ScrollGrid.ScrollBorderNW.Fill = Active;
                    Instance.ViewPanel.ScrollGrid.ScrollBorderSW.Fill = Active;
                    Instance.ViewPanel.ScrollGrid.ScrollBorderSE.Fill = Active;
                    Instance.ViewPanel.ScrollGrid.ScrollBorderNE.Fill = Active;
                    break;
                default:
                    break;

            }

            Instance.ViewPanel.ScrollGrid.ScrollBorderN.InvalidateVisual();
            Instance.ViewPanel.ScrollGrid.ScrollBorderS.InvalidateVisual();
            Instance.ViewPanel.ScrollGrid.ScrollBorderE.InvalidateVisual();
            Instance.ViewPanel.ScrollGrid.ScrollBorderW.InvalidateVisual();
            Instance.ViewPanel.ScrollGrid.ScrollBorderNW.InvalidateVisual();
            Instance.ViewPanel.ScrollGrid.ScrollBorderSW.InvalidateVisual();
            Instance.ViewPanel.ScrollGrid.ScrollBorderSE.InvalidateVisual();
            Instance.ViewPanel.ScrollGrid.ScrollBorderNE.InvalidateVisual();


        }
        public static void EnforceCursorPosition()
        {
            if (Core.Settings.MySettings.ScrollerAutoCenters)
            {
                ForceUpdateScrollerMousePos = true;
                System.Windows.Point pointFromParent = Instance.ViewPanel.SharpPanel.TranslatePoint(new System.Windows.Point(0, 0), Instance);
                Extensions.ExternalExtensions.SetCursorPos((int)(Instance.Left + pointFromParent.X) + (int)(Instance.ViewPanel.SharpPanel.ActualWidth / 2), (int)(Instance.Left + pointFromParent.Y) + (int)(Instance.ViewPanel.SharpPanel.ActualHeight / 2));
            }

        }
        public static void UpdateScrollerPosition(System.Windows.Forms.MouseEventArgs e)
        {
            Classes.Editor.SolutionState.ScrollPosition = new Point(e.X - Classes.Editor.SolutionState.ViewPositionX, e.Y - Classes.Editor.SolutionState.ViewPositionY);
            ForceUpdateScrollerMousePos = false;
        }
        public static void ToggleScrollerMode(System.Windows.Forms.MouseEventArgs e)
        {

            if (!Classes.Editor.SolutionState.WheelClicked)
            {
                //Turn Scroller Mode On
                Classes.Editor.SolutionState.WheelClicked = true;
                Classes.Editor.SolutionState.Scrolling = true;
                Classes.Editor.SolutionState.ScrollingDragged = false;
                Classes.Editor.SolutionState.ScrollPosition = new Point(e.X - Classes.Editor.SolutionState.ViewPositionX, e.Y - Classes.Editor.SolutionState.ViewPositionY);
                if (Instance.DeviceModel.vScrollBar1.IsVisible && Instance.DeviceModel.hScrollBar1.IsVisible)
                {
                    Instance.Cursor = System.Windows.Input.Cursors.ScrollAll;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.ALL);
                }
                else if (Instance.DeviceModel.vScrollBar1.IsVisible)
                {
                    Instance.Cursor = System.Windows.Input.Cursors.ScrollWE;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.WE);
                }
                else if (Instance.DeviceModel.hScrollBar1.IsVisible)
                {
                    Instance.Cursor = System.Windows.Input.Cursors.ScrollNS;
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
                    Instance.Cursor = System.Windows.Input.Cursors.Arrow;
                    SetScrollerBorderApperance();
                }
            }

        }
        public static void ScrollerMouseMove(MouseEventArgs e)
        {
            if (Classes.Editor.SolutionState.WheelClicked)
            {
                Classes.Editor.SolutionState.ScrollingDragged = true;

            }

            double xMove = (Instance.DeviceModel.hScrollBar1.IsVisible) ? e.X - Classes.Editor.SolutionState.ViewPositionX - Classes.Editor.SolutionState.ScrollPosition.X : 0;
            double yMove = (Instance.DeviceModel.vScrollBar1.IsVisible) ? e.Y - Classes.Editor.SolutionState.ViewPositionY - Classes.Editor.SolutionState.ScrollPosition.Y : 0;

            if (Math.Abs(xMove) < 15) xMove = 0;
            if (Math.Abs(yMove) < 15) yMove = 0;

            if (xMove > 0)
            {
                if (yMove > 0)
                {
                    Instance.Cursor = System.Windows.Input.Cursors.ScrollSE;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.SE);
                }
                else if (yMove < 0)
                {
                    Instance.Cursor = System.Windows.Input.Cursors.ScrollNE;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.NE);
                }
                else
                {
                    Instance.Cursor = System.Windows.Input.Cursors.ScrollE;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.E);
                }

            }
            else if (xMove < 0)
            {
                if (yMove > 0)
                {
                    Instance.Cursor = System.Windows.Input.Cursors.ScrollSW;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.SW);
                }
                else if (yMove < 0)
                {
                    Instance.Cursor = System.Windows.Input.Cursors.ScrollNW;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.NW);
                }
                else
                {
                    Instance.Cursor = System.Windows.Input.Cursors.ScrollW;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.W);
                }

            }
            else
            {

                if (yMove > 0)
                {
                    Instance.Cursor = System.Windows.Input.Cursors.ScrollS;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.S);
                }
                else if (yMove < 0)
                {
                    Instance.Cursor = System.Windows.Input.Cursors.ScrollN;
                    SetScrollerBorderApperance((int)ScrollerModeDirection.N);
                }
                else
                {
                    if (Instance.DeviceModel.vScrollBar1.IsVisible && Instance.DeviceModel.hScrollBar1.IsVisible)
                    {
                        Instance.Cursor = System.Windows.Input.Cursors.ScrollAll;
                        SetScrollerBorderApperance((int)ScrollerModeDirection.ALL);
                    }
                    else if (Instance.DeviceModel.vScrollBar1.IsVisible)
                    {
                        Instance.Cursor = System.Windows.Input.Cursors.ScrollNS;
                        SetScrollerBorderApperance((int)ScrollerModeDirection.NS);
                    }
                    else if (Instance.DeviceModel.hScrollBar1.IsVisible)
                    {
                        Instance.Cursor = System.Windows.Input.Cursors.ScrollWE;
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
            if (x > Instance.DeviceModel.hScrollBar1.Maximum) x = Instance.DeviceModel.hScrollBar1.Maximum;
            if (y > Instance.DeviceModel.vScrollBar1.Maximum) y = Instance.DeviceModel.vScrollBar1.Maximum;


            if (x != position.X || y != position.Y)
            {

                if (Instance.DeviceModel.vScrollBar1.IsVisible)
                {
                    Instance.DeviceModel.vScrollBar1.Value = y;
                }
                if (Instance.DeviceModel.hScrollBar1.IsVisible)
                {
                    Instance.DeviceModel.hScrollBar1.Value = x;
                }

                Instance.DeviceModel.GraphicPanel.OnMouseMoveEventCreate();

            }
            Instance.DeviceModel.GraphicPanel.Render();

        }
        public static void ScrollerMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                if (Core.Settings.MySettings.ScrollerPressReleaseMode) ToggleScrollerMode(e);
            }

        }
        public static void ScrollerMouseDown(MouseEventArgs e)
        {

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
        #endregion

        #region Main Mouse Methods
        public static void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!Classes.Editor.SolutionState.Scrolling) Instance.DeviceModel.GraphicPanel.Focus();

            if (e.Button == MouseButtons.Left) MouseDownLeft(e);
            else if (e.Button == MouseButtons.Right) MouseDownRight(e);
            else if (e.Button == MouseButtons.Middle) MouseDownMiddle(e);
        }
        public static void MouseDownRight(System.Windows.Forms.MouseEventArgs e)
        {
            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && !ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit()) TilesEditMouseDown(e);
            else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit()) EntitiesEditMouseDown(e);
        }
        public static void MouseDownLeft(System.Windows.Forms.MouseEventArgs e)
        {
            if (ManiacEditor.Classes.Editor.SolutionState.IsEditing() && !Classes.Editor.SolutionState.Dragged)
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && !Instance.EditorToolbar.InteractionToolButton.IsChecked.Value && !ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit()) TilesEditMouseDown(e);
                if (ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit() && ManiacEditor.Classes.Editor.SolutionState.IsSceneLoaded()) ChunksEditMouseDown(e);
                else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit()) EntitiesEditMouseDown(e);
            }
            InteractiveMouseDown(e);
        }
        public static void MouseDownMiddle(System.Windows.Forms.MouseEventArgs e)
        {
            EnforceCursorPosition();
            ToggleScrollerMode(e);
        }
        public static void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Instance.ViewPanel.InfoHUD.UpdatePopupVisibility();

            if (ForceUpdateScrollerMousePos) UpdateScrollerPosition(e);
            if (Classes.Editor.SolutionState.Scrolling) ScrollerMouseMove(e);
            if (Classes.Editor.SolutionState.Scrolling || Classes.Editor.SolutionState.ScrollingDragged || Classes.Editor.SolutionState.DraggingSelection || Classes.Editor.SolutionState.Dragged) Instance.DeviceModel.GraphicPanel.Render();

            Instance.EditorStatusBar.UpdatePositionLabel(e);

            if (Methods.GameHandler.GameRunning) InteractiveMouseMove(e);

            if (Classes.Editor.SolutionState.RegionX1 != -1)
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && !Instance.EditorToolbar.InteractionToolButton.IsChecked.Value && !ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit()) TilesEditMouseMoveDraggingStarted(e);
                else if (ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit()) ChunksEditMouseMoveDraggingStarted(e);
                else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit()) EntitiesEditMouseMoveDraggingStarted(e);

                Classes.Editor.SolutionState.RegionX1 = -1;
                Classes.Editor.SolutionState.RegionY1 = -1;
            }

            else if (e.Button == MouseButtons.Middle) EnforceCursorPosition();

            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && !ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit()) TilesEditMouseMove(e);
            else if (ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit()) ChunksEditMouseMove(e);
            else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit()) EntitiesEditMouseMove(e);

            MouseMovementControls(e);

            Classes.Editor.SolutionState.LastX = e.X;
            Classes.Editor.SolutionState.LastY = e.Y;
        }
        public static void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
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
                        if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && !ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit()) TilesEditMouseUp(e);
                        else if (ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit()) ChunksEditMouseUp(e);
                        else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit()) EntitiesEditMouseUp(e);
                    }
                    Methods.Internal.UserInterface.SetSelectOnlyButtonsState();
                    Classes.Editor.SolutionState.RegionX1 = -1;
                    Classes.Editor.SolutionState.RegionY1 = -1;
                }
                if (Classes.Editor.SolutionState.Dragged && (Classes.Editor.SolutionState.DraggedX != 0 || Classes.Editor.SolutionState.DraggedY != 0)) Actions.UndoRedoModel.UpdateUndoRedo();
                Classes.Editor.SolutionState.Dragged = false;
            }
            ScrollerMouseUp(e);

            Methods.Internal.UserInterface.UpdateEditLayerActions();
            Methods.Internal.UserInterface.UpdateControls();


        }
        public static void MouseUpDraggingSelection(System.Windows.Forms.MouseEventArgs e)
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

                if (ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit())
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

                    if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit()) Classes.Editor.Solution.Entities.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                Methods.Internal.UserInterface.SetSelectOnlyButtonsState();
                Methods.Internal.UserInterface.UpdateEditLayerActions();

            }
            Classes.Editor.SolutionState.DraggingSelection = false;
            Classes.Editor.Solution.EditLayerA?.EndTempSelection();
            Classes.Editor.Solution.EditLayerB?.EndTempSelection();

            if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit()) Classes.Editor.Solution.Entities.EndTempSelection();
        }
        public static void MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Instance.DeviceModel.GraphicPanel.Focus();
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
                Classes.Editor.SolutionState.ZoomLevel += change;
                if (Classes.Editor.SolutionState.ZoomLevel > maxZoom) Classes.Editor.SolutionState.ZoomLevel = maxZoom;
                if (Classes.Editor.SolutionState.ZoomLevel < minZoom) Classes.Editor.SolutionState.ZoomLevel = minZoom;

                Instance.DeviceModel.SetZoomLevel(Classes.Editor.SolutionState.ZoomLevel, new Point(e.X - Classes.Editor.SolutionState.ViewPositionX, e.Y - Classes.Editor.SolutionState.ViewPositionY));
            }
            void Normal()
            {
                if (Instance.DeviceModel.vScrollBar1.IsVisible || Instance.DeviceModel.hScrollBar1.IsVisible) ScrollMove();
                if (Core.Settings.MySettings.EntityFreeCam) FreeCamScroll();

                void ScrollMove()
                {
                    if (Classes.Editor.SolutionState.ScrollDirection == (int)ScrollDir.Y && !Classes.Editor.SolutionState.ScrollLocked) ScrollY();
                    else if (Classes.Editor.SolutionState.ScrollDirection == (int)ScrollDir.X && !Classes.Editor.SolutionState.ScrollLocked) ScrollX();
                    else if (Classes.Editor.SolutionState.ScrollLocked)
                        if (Classes.Editor.SolutionState.ScrollDirection == (int)ScrollDir.Y) ScrollY();
                        else ScrollX();


                    void ScrollX()
                    {
                        if (ShiftPressed())
                        {
                            if (Instance.DeviceModel.vScrollBar1.IsVisible) VScroll();
                            else HScroll();
                        }
                        else
                        {
                            if (Instance.DeviceModel.hScrollBar1.IsVisible) HScroll();
                            else VScroll();
                        }

                    }

                    void ScrollY()
                    {
                        if (ShiftPressed())
                        {
                            if (Instance.DeviceModel.hScrollBar1.IsVisible) HScroll();
                            else VScroll();
                        }
                        else
                        {
                            if (Instance.DeviceModel.vScrollBar1.IsVisible) VScroll();
                            else HScroll();
                        }

                    }
                }
                void FreeCamScroll()
                {
                    if (Classes.Editor.SolutionState.ScrollDirection == (int)ScrollDir.X) Classes.Editor.SolutionState.CustomX -= e.Delta;
                    else Classes.Editor.SolutionState.CustomY -= e.Delta;
                }
            }
            void VScroll()
            {
                double y = Instance.DeviceModel.vScrollBar1.Value - e.Delta;
                if (y < 0) y = 0;
                if (y > Instance.DeviceModel.vScrollBar1.Maximum) y = Instance.DeviceModel.vScrollBar1.Maximum;
                Instance.DeviceModel.vScrollBar1.Value = y;
            }
            void HScroll()
            {
                double x = Instance.DeviceModel.hScrollBar1.Value - e.Delta;
                if (x < 0) x = 0;
                if (x > Instance.DeviceModel.hScrollBar1.Maximum) x = Instance.DeviceModel.hScrollBar1.Maximum;
                Instance.DeviceModel.hScrollBar1.Value = x;
            }
        }
        public static void MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Instance.DeviceModel.GraphicPanel.Focus();
            if (e.Button == MouseButtons.Right)
            {
                if (Instance.EditorToolbar.InteractionToolButton.IsChecked.Value) InteractiveContextMenu(e);
                else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit() && !Instance.EditorToolbar.DrawToolButton.IsChecked.Value && !Instance.EditorToolbar.SplineToolButton.IsChecked.Value && (!Classes.Editor.SolutionState.RightClicktoSwapSlotID || Classes.Editor.Solution.Entities.SelectedEntities.Count <= 1)) EntitiesEditContextMenu(e);
                else if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && !Instance.EditorToolbar.DrawToolButton.IsChecked.Value) TilesEditContextMenu(e);
            }

        }
        public static void SetClickedXY(System.Windows.Forms.MouseEventArgs e) { Classes.Editor.SolutionState.RegionX1 = e.X; Classes.Editor.SolutionState.RegionY1 = e.Y; }
        public static void SetClickedXY(Point e) { Classes.Editor.SolutionState.RegionX1 = e.X; Classes.Editor.SolutionState.RegionY1 = e.Y; }
        public static void MouseMovementControls(System.Windows.Forms.MouseEventArgs e)
        {
            if (Classes.Editor.SolutionState.DraggingSelection || Classes.Editor.SolutionState.Dragged) EdgeMove();
            if (Classes.Editor.SolutionState.DraggingSelection) SetSelectionBounds();
            else if (Classes.Editor.SolutionState.Dragged) DragMoveItems();


            void EdgeMove()
            {
                System.Windows.Point position = new System.Windows.Point(Classes.Editor.SolutionState.ViewPositionX, Classes.Editor.SolutionState.ViewPositionY); ;
                double ScreenMaxX = position.X + Instance.DeviceModel.splitContainer1.Panel1.Width - (int)Instance.DeviceModel.vScrollBar.ActualWidth;
                double ScreenMaxY = position.Y + Instance.DeviceModel.splitContainer1.Panel1.Height - (int)Instance.DeviceModel.hScrollBar.ActualHeight;
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
                if (x > Instance.DeviceModel.hScrollBar1.Maximum) x = Instance.DeviceModel.hScrollBar1.Maximum;
                if (y > Instance.DeviceModel.vScrollBar1.Maximum) y = Instance.DeviceModel.vScrollBar1.Maximum;

                if (x != position.X || y != position.Y)
                {
                    if (Instance.DeviceModel.vScrollBar1.IsVisible)
                    {
                        Instance.DeviceModel.vScrollBar1.Value = y;
                    }
                    if (Instance.DeviceModel.hScrollBar1.IsVisible)
                    {
                        Instance.DeviceModel.hScrollBar1.Value = x;
                    }
                    Instance.DeviceModel.GraphicPanel.OnMouseMoveEventCreate();
                    // FIX: Determine if this is Needed
                    //if (!Classes.Edit.SolutionState.Scrolling) Editor.Instance.FormsModel.GraphicPanel.Render();



                }
            }
            void SetSelectionBounds()
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit()) ChunkMode();
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

                        Methods.Internal.UserInterface.UpdateTilesOptions();
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

                        Methods.Internal.UserInterface.UpdateTilesOptions();

                        if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit()) Classes.Editor.Solution.Entities.TempSelection(new Rectangle(Classes.Editor.SolutionState.TempSelectX1, Classes.Editor.SolutionState.TempSelectY1, Classes.Editor.SolutionState.TempSelectX2 - Classes.Editor.SolutionState.TempSelectX1, Classes.Editor.SolutionState.TempSelectY2 - Classes.Editor.SolutionState.TempSelectY1), CtrlPressed());
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
                if (Classes.Editor.SolutionState.UseMagnetMode && ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
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


                if (!ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit())
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
                if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
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
                        Methods.Internal.UserInterface.UpdateEntitiesToolbarList();
                        Methods.Internal.UserInterface.SetSelectOnlyButtonsState();
                    }
                    Instance.EntitiesToolbar.UpdateCurrentEntityProperites();
                }
                Classes.Editor.SolutionState.StartDragged = false;
            }
        }
        #endregion

        #region Tiles Edit Mouse Methods
        public static void TilesEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
            {
                TilesEditDrawTool(e, false);
            }
        }
        public static void TilesEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
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

            else if (!Instance.EditorToolbar.SelectToolButton.IsChecked.Value && !ShiftPressed() && !CtrlPressed() && (Classes.Editor.Solution.EditLayerA?.HasTileAt(clicked_point) ?? false) || (Classes.Editor.Solution.EditLayerB?.HasTileAt(clicked_point) ?? false))
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
                    ManiacEditor.Classes.Editor.EditorActions.Deselect();
                Methods.Internal.UserInterface.UpdateEditLayerActions();

                Classes.Editor.SolutionState.DraggingSelection = true;
                Classes.Editor.SolutionState.RegionX2 = Classes.Editor.SolutionState.RegionX1;
                Classes.Editor.SolutionState.RegionY2 = Classes.Editor.SolutionState.RegionY1;
            }
        }
        public static void TilesEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    TilesEditDrawTool(e, true);
                }
                else SetClickedXY(e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    TilesEditDrawTool(e, true);
                }
            }
        }
        public static void TilesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
            Classes.Editor.Solution.EditLayerA?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Classes.Editor.Solution.EditLayerB?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
        }
        public static void TilesEditContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            string newLine = Environment.NewLine;
            Point chunkPos = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinates(e.X / Classes.Editor.SolutionState.Zoom, e.Y / Classes.Editor.SolutionState.Zoom);
            Point tilePos;
            if (e.X == 0 || e.Y == 0) tilePos = new Point(0, 0);
            else tilePos = new Point(e.X / 16, e.Y / 16);

            Instance.EditorStatusBar.PixelPositionMenuItem.Header = "Pixel Position:" + newLine + String.Format("X: {0}, Y: {1}", e.X, e.Y);
            Instance.EditorStatusBar.ChunkPositionMenuItem.Header = "Chunk Position:" + newLine + String.Format("X: {0}, Y: {1}", chunkPos.X, chunkPos.Y);
            Instance.EditorStatusBar.TilePositionMenuItem.Header = "Tile Position:" + newLine + String.Format("X: {0}, Y: {1}", tilePos.X, tilePos.Y);


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
            Instance.EditorStatusBar.TileManiacIntergrationItem.IsEnabled = (tile < 1023);
            Instance.EditorStatusBar.TileManiacIntergrationItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);

            System.Windows.Controls.ContextMenu info = new System.Windows.Controls.ContextMenu();
            info.ItemsSource = Instance.EditorStatusBar.TilesContext.Items;
            info.Foreground = (System.Windows.Media.SolidColorBrush)Instance.FindResource("NormalText");
            info.Background = (System.Windows.Media.SolidColorBrush)Instance.FindResource("NormalBackground");
            info.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
            info.StaysOpen = false;
            info.IsOpen = true;
        }
        public static void TilesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
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
                    ManiacEditor.Classes.Editor.EditorActions.DeleteSelected();
                }
                else
                {
                    double size = (Classes.Editor.SolutionState.DrawBrushSize / 2) * Classes.Editor.Constants.TILE_SIZE;
                    Classes.Editor.Solution.EditLayerA?.Select(new Rectangle((int)(p.X - size), (int)(p.Y - size), Classes.Editor.SolutionState.DrawBrushSize * Classes.Editor.Constants.TILE_SIZE, Classes.Editor.SolutionState.DrawBrushSize * Classes.Editor.Constants.TILE_SIZE));
                    Classes.Editor.Solution.EditLayerB?.Select(new Rectangle((int)(p.X - size), (int)(p.Y - size), Classes.Editor.SolutionState.DrawBrushSize * Classes.Editor.Constants.TILE_SIZE, Classes.Editor.SolutionState.DrawBrushSize * Classes.Editor.Constants.TILE_SIZE));
                    ManiacEditor.Classes.Editor.EditorActions.DeleteSelected();
                }
            }

            void PlaceTile()
            {
                if (Classes.Editor.SolutionState.DrawBrushSize == 1)
                {
                    if (Instance.TilesToolbar.SelectedTile != -1)
                    {
                        if (Classes.Editor.Solution.EditLayerA.GetTileAt(p) != Instance.TilesToolbar.SelectedTile)
                        {
                            ManiacEditor.Classes.Editor.EditorActions.EditorPlaceTile(p, Instance.TilesToolbar.SelectedTile, Classes.Editor.Solution.EditLayerA);
                        }
                        else if (!Classes.Editor.Solution.EditLayerA.IsPointSelected(p))
                        {
                            Classes.Editor.Solution.EditLayerA.Select(p);
                        }
                    }
                }
                else
                {
                    if (Instance.TilesToolbar.SelectedTile != -1)
                    {
                        ManiacEditor.Classes.Editor.EditorActions.EditorPlaceTile(p, Instance.TilesToolbar.SelectedTile, Classes.Editor.Solution.EditLayerA, true);
                    }
                }
            }
        }
        #endregion

        #region Entities Edit Mouse Methods

        public static void EntitiesEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value) EntitiesEditDrawTool(e);
        }
        public static void EntitiesEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
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
                    ManiacEditor.Classes.Editor.EditorActions.Deselect();
                Classes.Editor.SolutionState.DraggingSelection = true;
                Classes.Editor.SolutionState.RegionX2 = Classes.Editor.SolutionState.RegionX1;
                Classes.Editor.SolutionState.RegionY2 = Classes.Editor.SolutionState.RegionY1;

            }
        }
        public static void EntitiesEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
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
                        Methods.Internal.UserInterface.SetSelectOnlyButtonsState();
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
                else if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value) EntitiesEditDrawTool(e, true);
            }
            if (Instance.EditorToolbar.SplineToolButton.IsChecked.Value) SplineTool(e);
        }
        public static void EntitiesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
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
        public static void EntitiesEditContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
            string newLine = Environment.NewLine;
            if (Classes.Editor.Solution.Entities.GetEntityAt(clicked_point) != null)
            {
                var currentEntity = Classes.Editor.Solution.Entities.GetEntityAt(clicked_point);

                Instance.EditorStatusBar.EntityNameItem.Header = String.Format("Entity Name: {0}", currentEntity.Name);
                Instance.EditorStatusBar.EntitySlotIDItem.Header = String.Format("Slot ID: {0} {1} Runtime Slot ID: {2}", currentEntity.Entity.SlotID, Environment.NewLine, Classes.Editor.Solution.Entities.GetRealSlotID(currentEntity.Entity));
                Instance.EditorStatusBar.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", currentEntity.Entity.Position.X.High, currentEntity.Entity.Position.Y.High);
            }
            else
            {
                Instance.EditorStatusBar.EntityNameItem.Header = String.Format("Entity Name: {0}", "N/A");
                Instance.EditorStatusBar.EntitySlotIDItem.Header = String.Format("Slot ID: {0} {1} Runtime Slot ID: {2}", "N/A", Environment.NewLine, "N/A");
                Instance.EditorStatusBar.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", e.X, e.Y);
            }
            System.Windows.Controls.ContextMenu info = new System.Windows.Controls.ContextMenu();

            info.Style = (System.Windows.Style)Instance.FindResource("NormalText");
            info.ItemsSource = Instance.EditorStatusBar.EntityContext.Items;
            info.Foreground = (System.Windows.Media.SolidColorBrush)Instance.FindResource("NormalText");
            info.Background = (System.Windows.Media.SolidColorBrush)Instance.FindResource("NormalBackground");
            info.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
            info.StaysOpen = false;
            info.IsOpen = true;
        }
        public static void EntitiesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
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
                    ManiacEditor.Classes.Editor.EditorActions.Deselect();
                    Classes.Editor.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                }
                else
                {
                    Instance.EntitiesToolbar.SpawnObject();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                if (Classes.Editor.Solution.Entities.IsEntityAt(clicked_point, true) == true)
                {
                    ManiacEditor.Classes.Editor.EditorActions.Deselect();
                    Classes.Editor.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                    Classes.Editor.Solution.Entities.DeleteSelected();
                    ManiacEditor.Classes.Editor.EditorActions.UpdateLastEntityAction();
                }
            }
        }
        public static void SplineTool(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                if (Classes.Editor.Solution.Entities.IsEntityAt(clicked_point) == true)
                {
                    ManiacEditor.Classes.Editor.EditorActions.Deselect();
                    Classes.Editor.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                }
                else
                {
                    Classes.Editor.Solution.Entities.SpawnInternalSplineObject(new Position((short)clicked_point.X, (short)clicked_point.Y));
                    ManiacEditor.Classes.Editor.EditorActions.UpdateLastEntityAction();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                Classes.Editor.Scene.Sets.EditorEntity atPoint = Classes.Editor.Solution.Entities.GetEntityAt(clicked_point);
                if (atPoint != null && atPoint.Entity.Object.Name.Name == "Spline")
                {
                    ManiacEditor.Classes.Editor.EditorActions.Deselect();
                    Classes.Editor.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                    Classes.Editor.Solution.Entities.DeleteInternallySelected();
                    ManiacEditor.Classes.Editor.EditorActions.UpdateLastEntityAction();
                }
            }
        }

        #endregion

        #region Chunks Edit Mouse Controls

        public static void ChunksEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            Point p = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
            Point pC = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinates(p.X, p.Y);

            if (e.Button == MouseButtons.Left)
            {
                if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    int selectedIndex = Instance.TilesToolbar.ChunkList.SelectedIndex;
                    // Place Stamp
                    if (selectedIndex != -1)
                    {
                        if (!Instance.Chunks.DoesChunkMatch(pC, Instance.Chunks.StageStamps.StampList[selectedIndex], Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB))
                        {
                            Instance.Chunks.PasteStamp(pC, selectedIndex, Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB);
                        }

                    }
                }
            }

            else if (e.Button == MouseButtons.Right)
            {
                if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {

                    if (!Instance.Chunks.IsChunkEmpty(pC, Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB))
                    {
                        // Remove Stamp Sized Area
                        Instance.Chunks.PasteStamp(pC, 0, Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB, true);
                    }
                }

            }
        }
        public static void ChunksEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
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
                    ManiacEditor.Classes.Editor.EditorActions.Deselect();
                Methods.Internal.UserInterface.UpdateEditLayerActions();

                Classes.Editor.SolutionState.DraggingSelection = true;
                Classes.Editor.SolutionState.RegionX2 = e.X;
                Classes.Editor.SolutionState.RegionY2 = e.Y;
            }
        }
        public static void ChunksEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    Point p = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                    Point pC = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinates(p.X, p.Y);

                    if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    {
                        int selectedIndex = Instance.TilesToolbar.ChunkList.SelectedIndex;
                        // Place Stamp
                        if (selectedIndex != -1)
                        {
                            if (!Instance.Chunks.DoesChunkMatch(pC, Instance.Chunks.StageStamps.StampList[selectedIndex], Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB))
                            {
                                Instance.Chunks.PasteStamp(pC, selectedIndex, Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB);
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
                if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    Point p = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                    Point chunk_point = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
                    Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

                    // Remove Stamp Sized Area
                    if (!Classes.Editor.Solution.EditLayerA.DoesChunkContainASelectedTile(p)) Classes.Editor.Solution.EditLayerA?.Select(clicked_chunk);
                    if (Classes.Editor.Solution.EditLayerB != null && !Classes.Editor.Solution.EditLayerB.DoesChunkContainASelectedTile(p)) Classes.Editor.Solution.EditLayerB?.Select(clicked_chunk);
                    ManiacEditor.Classes.Editor.EditorActions.DeleteSelected();
                }
            }
        }
        public static void ChunksEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
            Point chunk_point = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);
            Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

            Classes.Editor.Solution.EditLayerA?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Classes.Editor.Solution.EditLayerB?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Methods.Internal.UserInterface.UpdateEditLayerActions();
        }

        #endregion

        #region Interactive Mouse Controls

        public static void InteractiveMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.GameHandler.PlayerSelected)
            {
                Methods.GameHandler.MovePlayer(new Point(e.X, e.Y), Classes.Editor.SolutionState.Zoom, Methods.GameHandler.SelectedPlayer);
            }

            if (Methods.GameHandler.CheckpointSelected)
            {
                Point clicked_point = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                Methods.GameHandler.UpdateCheckpoint(clicked_point, true);
            }
        }
        public static void InteractiveMouseDown(System.Windows.Forms.MouseEventArgs e)
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
        public static void InteractiveMouseUp(System.Windows.Forms.MouseEventArgs e)
        {

        }
        public static void InteractiveContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
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
                Instance.ViewPanel.SharpPanel.editTile0WithTileManiacToolStripMenuItem.IsEnabled = (tile < 1023);
                Instance.ViewPanel.SharpPanel.moveThePlayerToHereToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.removeCheckpointToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning && Methods.GameHandler.CheckpointEnabled;
                Instance.ViewPanel.SharpPanel.assetResetToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.restartSceneToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.moveCheckpointToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning && Methods.GameHandler.CheckpointEnabled;


                Instance.ViewPanel.SharpPanel.editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);
                Instance.ViewPanel.SharpPanel.ViewPanelContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                Instance.ViewPanel.SharpPanel.ViewPanelContextMenu.IsOpen = true;
            }
            else
            {
                Point clicked_point_tile = new Point((int)(e.X / Classes.Editor.SolutionState.Zoom), (int)(e.Y / Classes.Editor.SolutionState.Zoom));
                string tile = "N/A";
                Instance.ViewPanel.SharpPanel.editTile0WithTileManiacToolStripMenuItem.IsEnabled = false;
                Instance.ViewPanel.SharpPanel.moveThePlayerToHereToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.moveCheckpointToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning;

                Instance.ViewPanel.SharpPanel.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.removeCheckpointToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.assetResetToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.restartSceneToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.moveCheckpointToolStripMenuItem.IsEnabled = Methods.GameHandler.GameRunning;

                Instance.ViewPanel.SharpPanel.editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);
                Instance.ViewPanel.SharpPanel.ViewPanelContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                Instance.ViewPanel.SharpPanel.ViewPanelContextMenu.IsOpen = true;
            }
        }

        #endregion

        #endregion

        #region Keyboard Controls

        #region Keyboard Inputs
        public static void GraphicPanel_OnKeyUp(object sender, KeyEventArgs e)
        {
            // Tiles Toolbar Flip Horizontal
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.FlipHTiles, true))
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Instance.TilesToolbar.SetSelectTileOption(0, false);
            }
            // Tiles Toolbar Flip Vertical
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.FlipVTiles, true))
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Instance.TilesToolbar.SetSelectTileOption(1, false);
            }
        }
        public static void GraphicPanel_OnKeyDown(object sender, KeyEventArgs e)
        {
            bool parallaxAnimationInProgress = Classes.Editor.SolutionState.AllowAnimations && Classes.Editor.SolutionState.ParallaxAnimationChecked;
            if (parallaxAnimationInProgress) return;

            // Faster Nudge Toggle
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.NudgeFaster))
            {
                Classes.Editor.SolutionState.EnableFasterNudge ^= true;
            }
            // Scroll Lock Toggle
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.ScrollLock))
            {
                Classes.Editor.SolutionState.ScrollLocked ^= true;
            }
            // Switch Scroll Lock Type
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.ScrollLockTypeSwitch))
            {
                Classes.Editor.EditorActions.SetScrollLockDirection();

            }
            // Tiles Toolbar Flip Vertical
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.FlipVTiles, true))
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Instance.TilesToolbar.SetSelectTileOption(1, true);
            }
            // Tiles Toolbar Flip Horizontal
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.FlipHTiles, true))
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Instance.TilesToolbar.SetSelectTileOption(0, true);
            }
            // Open Click (Alt: Open Data Dir)
            else if ((Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.OpenDataDir)))
            {
                ManiacEditor.Classes.Editor.SolutionLoader.OpenDataDirectory();
            }
            else if ((Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.Open)))
            {
                ManiacEditor.Classes.Editor.SolutionLoader.OpenScene();
            }
            // New Click
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.New))
            {
                //ManiacEditor.Classes.Editor.SolutionLoader.NewScene();
            }
            // Save Click (Alt: Save As)
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.SaveAs))
            {
                ManiacEditor.Classes.Editor.SolutionLoader.Save();
            }
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds._Save))
            {
                ManiacEditor.Classes.Editor.SolutionLoader.SaveAs();
            }
            // Undo
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.Undo))
            {
                ManiacEditor.Classes.Editor.EditorActions.EditorUndo();
            }
            // Redo
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.Redo))
            {
                ManiacEditor.Classes.Editor.EditorActions.EditorRedo();
            }
            // Developer Interface
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.DeveloperInterface))
            {
                ManiacEditor.Classes.Editor.EditorActions.EditorUndo();
            }
            // Save for Force Open on Startup
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.ForceOpenOnStartup))
            {
                ManiacEditor.Classes.Editor.EditorActions.EditorRedo();
            }
            else if (ManiacEditor.Classes.Editor.SolutionState.IsSceneLoaded())
            {
                GraphicPanel_OnKeyDownLoaded(sender, e);
            }
            // Editing Key Shortcuts
            if (ManiacEditor.Classes.Editor.SolutionState.IsEditing())
            {
                GraphicPanel_OnKeyDownEditing(sender, e);
            }
            OnKeyDownTools(sender, e);
        }
        public static void GraphicPanel_OnKeyDownLoaded(object sender, KeyEventArgs e)
        {
            // Reset Zoom Level
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.ResetZoomLevel))
            {
                Instance.DeviceModel.SetZoomLevel(0, new Point(0, 0));
            }
            //Refresh Tiles and Sprites
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.RefreshResources))
            {
                Methods.Internal.UserInterface.ReloadSpritesAndTextures();
            }
            //Run Scene
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.RunScene))
            {
                Methods.GameHandler.RunScene();
            }
            //Show Path A
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.ShowPathA) && ManiacEditor.Classes.Editor.SolutionState.IsSceneLoaded())
            {
                Classes.Editor.SolutionState.ShowCollisionA ^= true;
            }
            //Show Path B
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.ShowPathB))
            {
                Classes.Editor.SolutionState.ShowCollisionB ^= true;
            }
            //Unload Scene
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.UnloadScene))
            {
                Instance.MenuBar.UnloadSceneEvent(null, null);
            }
            //Toggle Grid Visibility
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.ShowGrid))
            {
                Classes.Editor.SolutionState.ShowGrid ^= true;
            }
            //Toggle Tile ID Visibility
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.ShowTileID))
            {
                Classes.Editor.SolutionState.ShowTileID ^= true;
            }
            //Status Box Toggle
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.StatusBoxToggle))
            {
                Classes.Editor.SolutionState.DebugStatsVisibleOnPanel ^= true;
                Instance.ViewPanel.InfoHUD.UpdatePopupVisibility();
            }
        }
        public static void GraphicPanel_OnKeyDownEditing(object sender, KeyEventArgs e)
        {
            //Paste
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.Paste))
            {
                Classes.Editor.EditorActions.Paste();
            }
            //Paste to Chunk
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.PasteToChunk))
            {
                Classes.Editor.EditorActions.PasteToChunks();
            }
            //Select All
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.SelectAll))
            {
                Classes.Editor.EditorActions.SelectAll();
            }
            // Selected Key Shortcuts   
            if (ManiacEditor.Classes.Editor.SolutionState.IsSelected())
            {
                GraphicPanel_OnKeyDownSelectedEditing(sender, e);
            }
        }
        public static void GraphicPanel_OnKeyDownSelectedEditing(object sender, KeyEventArgs e)
        {
            // Delete
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.Delete))
            {
                ManiacEditor.Classes.Editor.EditorActions.DeleteSelected();
            }

            // Moving
            else if (e.KeyData == Keys.Up || e.KeyData == Keys.Down || e.KeyData == Keys.Left || e.KeyData == Keys.Right)
            {
                ManiacEditor.Classes.Editor.EditorActions.MoveEntityOrTiles(sender, e);
            }

            //Cut 
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.Cut))
            {
                Classes.Editor.EditorActions.Cut();
            }
            //Copy
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.Copy))
            {
                Classes.Editor.EditorActions.Copy();
            }
            //Duplicate
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.Duplicate))
            {
                Classes.Editor.EditorActions.Duplicate();
            }
            //Delete
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.Delete))
            {
                Classes.Editor.EditorActions.Delete();
            }
            // Flip Vertical Individual
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.FlipVIndv))
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
                    Classes.Editor.EditorActions.FlipVerticalIndividual();
            }
            // Flip Horizontal Individual
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.FlipHIndv))
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
                    Classes.Editor.EditorActions.FlipHorizontalIndividual();
            }
            // Flip Vertical
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.FlipV))
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
                    Classes.Editor.EditorActions.FlipVertical();
                else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
                    ManiacEditor.Classes.Editor.EditorActions.FlipEntities(FlipDirection.Veritcal);
            }

            // Flip Horizontal
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.FlipH))
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
                    Classes.Editor.EditorActions.FlipHorizontal();
                else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
                    ManiacEditor.Classes.Editor.EditorActions.FlipEntities(FlipDirection.Horizontal);
            }
        }
        public static void OnKeyDownTools(object sender, KeyEventArgs e)
        {
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.PointerTool) && Instance.EditorToolbar.PointerToolButton.IsEnabled) Classes.Editor.SolutionState.PointerMode(true);
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.SelectTool) && Instance.EditorToolbar.SelectToolButton.IsEnabled) Classes.Editor.SolutionState.SelectionMode(true);
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.DrawTool) && Instance.EditorToolbar.DrawToolButton.IsEnabled) Classes.Editor.SolutionState.DrawMode(true);
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.MagnetTool) && Instance.EditorToolbar.MagnetMode.IsEnabled) Classes.Editor.SolutionState.UseMagnetMode ^= true;
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.SplineTool) && Instance.EditorToolbar.SplineToolButton.IsEnabled) Classes.Editor.SolutionState.SplineMode(true);
            else if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.StampTool) && Instance.EditorToolbar.ChunksToolButton.IsEnabled) Classes.Editor.SolutionState.ChunksMode();

        }
        #endregion

        #region Tile Maniac Keyboard Inputs
        public static void TileManiac_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacNewInstance))
            {
                CollisionEditor.Instance.newInstanceToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacOpen))
            {
                CollisionEditor.Instance.OpenToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacSave))
            {
                CollisionEditor.Instance.saveToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacSaveAs))
            {
                CollisionEditor.Instance.saveAsToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacSaveUncompressed))
            {
                CollisionEditor.Instance.saveUncompressedToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacSaveAsUncompressed))
            {
                CollisionEditor.Instance.saveAsUncompressedToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacbackupConfig))
            {
                CollisionEditor.Instance.tileConfigbinToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacbackupImage))
            {
                CollisionEditor.Instance.x16TilesgifToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacExportColMask))
            {
                CollisionEditor.Instance.exportCurrentCollisionMaskAsToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacOpenSingleColMask))
            {
                CollisionEditor.Instance.openSingleCollisionMaskToolStripMenuItem_Click_1(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacImportFromOlderRSDK))
            {
                CollisionEditor.Instance.importFromOlderRSDKVersionToolStripMenuItem_Click(null, null);
            }

            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacCopy))
            {
                CollisionEditor.Instance.copyToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacPastetoOther))
            {
                CollisionEditor.Instance.copyToOtherPathToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacPaste))
            {
                CollisionEditor.Instance.pasteToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacMirrorMode))
            {
                CollisionEditor.Instance.mirrorPathsToolStripMenuItem1.IsChecked = !CollisionEditor.Instance.mirrorPathsToolStripMenuItem1.IsChecked;
                CollisionEditor.Instance.mirrorPathsToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacRestorePathA))
            {
                CollisionEditor.Instance.pathAToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacRestorePathB))
            {
                CollisionEditor.Instance.pathBToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacRestorePaths))
            {
                CollisionEditor.Instance.bothToolStripMenuItem_Click(null, null);
            }

            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacShowPathB))
            {
                CollisionEditor.Instance.showPathBToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacShowGrid))
            {
                CollisionEditor.Instance.showGridToolStripMenuItem.IsChecked = !CollisionEditor.Instance.showGridToolStripMenuItem.IsChecked;
                CollisionEditor.Instance.showGridToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacClassicMode))
            {
                CollisionEditor.Instance.classicViewModeToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacWindowAlwaysOnTop))
            {
                CollisionEditor.Instance.windowAlwaysOnTop.IsChecked = !CollisionEditor.Instance.windowAlwaysOnTop.IsChecked;
                CollisionEditor.Instance.WindowAlwaysOnTop_Click(null, null);
            }

            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacSplitFile))
            {
                CollisionEditor.Instance.splitFileToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacFlipTileH))
            {
                CollisionEditor.Instance.flipTileHorizontallyToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacFlipTileV))
            {
                CollisionEditor.Instance.flipTileVerticallyToolStripMenuItem_Click(null, null);
            }

            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacHomeFolderOpen))
            {
                CollisionEditor.Instance.openCollisionHomeFolderToolStripMenuItem_Click(null, null);
            }

            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacAbout))
            {
                CollisionEditor.Instance.aboutToolStripMenuItem1_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Core.Settings.MyKeyBinds.TileManiacSettings))
            {
                CollisionEditor.Instance.settingsToolStripMenuItem_Click(null, null);
            }
        }
        public static void TileManiac_OnKeyUp(object sender, KeyEventArgs e)
        {

        }
        #endregion

        #endregion
    }
}
