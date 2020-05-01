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
using System.Threading.Tasks;


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
        private static bool AltPressed() { return System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Alt); }

        #region Mouse Controls

        #region Mouse Auto-Scrolling Methods
        private static AutoScrollDirection LastAutoScrollDirection { get; set; } = AutoScrollDirection.NONE;
        public static void SetAutoScrollerApperance(AutoScrollDirection direction = AutoScrollDirection.NONE)
        {
            if (LastAutoScrollDirection != direction)
            {
                var converter = new System.Windows.Media.BrushConverter();
                var Active = (System.Windows.Media.Brush)converter.ConvertFromString("Red");
                var NotActive = (System.Windows.Media.Brush)converter.ConvertFromString("Transparent");

                switch (direction)
                {
                    case AutoScrollDirection.N: Instance.Cursor = System.Windows.Input.Cursors.ScrollN; break;
                    case AutoScrollDirection.S: Instance.Cursor = System.Windows.Input.Cursors.ScrollS; break;
                    case AutoScrollDirection.E: Instance.Cursor = System.Windows.Input.Cursors.ScrollE; break;
                    case AutoScrollDirection.W: Instance.Cursor = System.Windows.Input.Cursors.ScrollW; break;
                    case AutoScrollDirection.NW: Instance.Cursor = System.Windows.Input.Cursors.ScrollNW; break;
                    case AutoScrollDirection.SW: Instance.Cursor = System.Windows.Input.Cursors.ScrollSW; break;
                    case AutoScrollDirection.SE: Instance.Cursor = System.Windows.Input.Cursors.ScrollSE; break;
                    case AutoScrollDirection.NE: Instance.Cursor = System.Windows.Input.Cursors.ScrollNE; break;
                    case AutoScrollDirection.ALL: Instance.Cursor = System.Windows.Input.Cursors.ScrollAll; break;
                    case AutoScrollDirection.NONE: Instance.Cursor = System.Windows.Input.Cursors.Arrow; break;
                    default: Instance.Cursor = System.Windows.Input.Cursors.Arrow; break;
                }

                LastAutoScrollDirection = direction;

                if (direction == AutoScrollDirection.N || direction == AutoScrollDirection.ALL) Instance.ViewPanel.ScrollGrid.ScrollBorderN.Fill = Active;
                else Instance.ViewPanel.ScrollGrid.ScrollBorderN.Fill = NotActive;
                if (direction == AutoScrollDirection.S || direction == AutoScrollDirection.ALL) Instance.ViewPanel.ScrollGrid.ScrollBorderS.Fill = Active;
                else Instance.ViewPanel.ScrollGrid.ScrollBorderS.Fill = NotActive;
                if (direction == AutoScrollDirection.E || direction == AutoScrollDirection.ALL) Instance.ViewPanel.ScrollGrid.ScrollBorderE.Fill = Active;
                else Instance.ViewPanel.ScrollGrid.ScrollBorderE.Fill = NotActive;
                if (direction == AutoScrollDirection.W || direction == AutoScrollDirection.ALL) Instance.ViewPanel.ScrollGrid.ScrollBorderW.Fill = Active;
                else Instance.ViewPanel.ScrollGrid.ScrollBorderW.Fill = NotActive;
                if (direction == AutoScrollDirection.NW || direction == AutoScrollDirection.ALL) Instance.ViewPanel.ScrollGrid.ScrollBorderNW.Fill = Active;
                else Instance.ViewPanel.ScrollGrid.ScrollBorderNW.Fill = NotActive;
                if (direction == AutoScrollDirection.SW || direction == AutoScrollDirection.ALL) Instance.ViewPanel.ScrollGrid.ScrollBorderSW.Fill = Active;
                else Instance.ViewPanel.ScrollGrid.ScrollBorderSW.Fill = NotActive;
                if (direction == AutoScrollDirection.SE || direction == AutoScrollDirection.ALL) Instance.ViewPanel.ScrollGrid.ScrollBorderSE.Fill = Active;
                else Instance.ViewPanel.ScrollGrid.ScrollBorderSE.Fill = NotActive;
                if (direction == AutoScrollDirection.NE || direction == AutoScrollDirection.ALL) Instance.ViewPanel.ScrollGrid.ScrollBorderNE.Fill = Active;
                else Instance.ViewPanel.ScrollGrid.ScrollBorderNE.Fill = NotActive;

                Instance.ViewPanel.ScrollGrid.ScrollBorderN.InvalidateVisual();
                Instance.ViewPanel.ScrollGrid.ScrollBorderS.InvalidateVisual();
                Instance.ViewPanel.ScrollGrid.ScrollBorderE.InvalidateVisual();
                Instance.ViewPanel.ScrollGrid.ScrollBorderW.InvalidateVisual();
                Instance.ViewPanel.ScrollGrid.ScrollBorderNW.InvalidateVisual();
                Instance.ViewPanel.ScrollGrid.ScrollBorderSW.InvalidateVisual();
                Instance.ViewPanel.ScrollGrid.ScrollBorderSE.InvalidateVisual();
                Instance.ViewPanel.ScrollGrid.ScrollBorderNE.InvalidateVisual();
            }
        }

        public static void ToggleAutoScrollerMode(System.Windows.Forms.MouseEventArgs e, bool isRelease = false)
        {
            if (Properties.Settings.MySettings.ScrollerPressReleaseMode)
            {
                if (isRelease) ScrollerModeOff();
                else ScrollerModeOn();
            }
            else if (!isRelease)
            {
                if (!Methods.Solution.SolutionState.Main.AutoScrolling) ScrollerModeOn();
                else ScrollerModeOff();
            }



            void ScrollerModeOn()
            {
                //Turn Scroller Mode On
                Methods.Solution.SolutionState.Main.AutoScrolling = true;
                Methods.Solution.SolutionState.Main.AutoScrollPosition = new Point(e.X - Methods.Solution.SolutionState.Main.ViewPositionX, e.Y - Methods.Solution.SolutionState.Main.ViewPositionY);
                if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible && Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) SetAutoScrollerApperance(AutoScrollDirection.ALL);
                else if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible) SetAutoScrollerApperance(AutoScrollDirection.WE);
                else if (Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) SetAutoScrollerApperance(AutoScrollDirection.NS);
            }
            void ScrollerModeOff()
            {
                //Turn Scroller Mode Off
                Methods.Solution.SolutionState.Main.AutoScrolling = false;
                SetAutoScrollerApperance(AutoScrollDirection.NONE);
            }
        }
        #endregion

        #region Scroller Mode Events
        public static void ScrollerMouseMove(MouseEventArgs e)
        {
            double xMove = (Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) ? e.X - Methods.Solution.SolutionState.Main.ViewPositionX - Methods.Solution.SolutionState.Main.AutoScrollPosition.X : 0;
            double yMove = (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible) ? e.Y - Methods.Solution.SolutionState.Main.ViewPositionY - Methods.Solution.SolutionState.Main.AutoScrollPosition.Y : 0;

            if (Math.Abs(xMove) < 15) xMove = 0;
            if (Math.Abs(yMove) < 15) yMove = 0;

            if (xMove > 0)
            {
                if (yMove > 0) SetAutoScrollerApperance(AutoScrollDirection.SE);
                else if (yMove < 0) SetAutoScrollerApperance(AutoScrollDirection.NE);
                else SetAutoScrollerApperance(AutoScrollDirection.E);
            }
            else if (xMove < 0)
            {
                if (yMove > 0) SetAutoScrollerApperance(AutoScrollDirection.SW);
                else if (yMove < 0) SetAutoScrollerApperance(AutoScrollDirection.NW);
                else SetAutoScrollerApperance(AutoScrollDirection.W);
            }
            else
            {
                if (yMove > 0) SetAutoScrollerApperance(AutoScrollDirection.S);
                else if (yMove < 0) SetAutoScrollerApperance(AutoScrollDirection.N);
                else
                {
                    if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible && Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) SetAutoScrollerApperance(AutoScrollDirection.ALL);
                    else if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible) SetAutoScrollerApperance(AutoScrollDirection.NS);
                    else if (Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) SetAutoScrollerApperance(AutoScrollDirection.WE);
                }
            }

            System.Windows.Point position = new System.Windows.Point(Methods.Solution.SolutionState.Main.ViewPositionX, Methods.Solution.SolutionState.Main.ViewPositionY);
            double x = xMove / 10 + position.X;
            double y = yMove / 10 + position.Y;

            if (!Methods.Solution.SolutionState.Main.UnlockCamera)
            {
                if (x < 0) x = 0;
                if (y < 0) y = 0;
                if (x > Instance.ViewPanel.SharpPanel.hScrollBar1.Maximum) x = Instance.ViewPanel.SharpPanel.hScrollBar1.Maximum;
                if (y > Instance.ViewPanel.SharpPanel.vScrollBar1.Maximum) y = Instance.ViewPanel.SharpPanel.vScrollBar1.Maximum;
            }

            if (x != position.X || y != position.Y)
            {
                if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible) Instance.ViewPanel.SharpPanel.vScrollBar1.Value = y;
                if (Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) Instance.ViewPanel.SharpPanel.hScrollBar1.Value = x;
                Instance.ViewPanel.SharpPanel.GraphicPanel.OnMouseMoveEventCreate();
                Instance.ViewPanel.SharpPanel.GraphicPanel.Render();
            }
        }

        #endregion


        #region General Mouse Methods
        public static void SetClickedXY(System.Windows.Forms.MouseEventArgs e) { Methods.Solution.SolutionState.Main.RegionX1 = e.X; Methods.Solution.SolutionState.Main.RegionY1 = e.Y; }
        public static void SetClickedXY(Point e) { Methods.Solution.SolutionState.Main.RegionX1 = e.X; Methods.Solution.SolutionState.Main.RegionY1 = e.Y; }
        #endregion


        #region Mouse Move Events
        public static void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Solution.SolutionState.Main.AutoScrolling) ScrollerMouseMove(e);
            Instance.EditorStatusBar.UpdatePositionLabel(e);
            if (Methods.Runtime.GameHandler.GameRunning) InteractiveMouseMove(e);

            if (Methods.Solution.SolutionState.Main.RegionX1 != -1)
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) TilesEditMouseMoveDraggingStarted(e);
                else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) EntitiesEditMouseMoveDraggingStarted(e);

                Methods.Solution.SolutionState.Main.RegionX1 = -1;
                Methods.Solution.SolutionState.Main.RegionY1 = -1;
            }

            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) TilesEditMouseMove(e);
            else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) EntitiesEditMouseMove(e);

            MouseMovementControls(e);

            Methods.Solution.SolutionState.Main.LastX = e.X;
            Methods.Solution.SolutionState.Main.LastY = e.Y;
        }

        #endregion

        #region Mouse Move Methods
        public static void MouseMovementControls(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Solution.SolutionState.Main.DraggingSelection || Methods.Solution.SolutionState.Main.Dragged) MouseMove_EdgeMove(e);
            if (Methods.Solution.SolutionState.Main.DraggingSelection) MouseMove_SetSelectionBounds(e);
            else if (Methods.Solution.SolutionState.Main.Dragged) MouseMove_DragMoveItems(e);
        }
        private static void MouseMove_DragMoveItems(System.Windows.Forms.MouseEventArgs e)
        {
            int oldGridX = (int)((Methods.Solution.SolutionState.Main.LastX / Methods.Solution.SolutionState.Main.Zoom) / Methods.Solution.SolutionState.Main.MagnetSize) * Methods.Solution.SolutionState.Main.MagnetSize;
            int oldGridY = (int)((Methods.Solution.SolutionState.Main.LastY / Methods.Solution.SolutionState.Main.Zoom) / Methods.Solution.SolutionState.Main.MagnetSize) * Methods.Solution.SolutionState.Main.MagnetSize;
            int newGridX = (int)((e.X / Methods.Solution.SolutionState.Main.Zoom) / Methods.Solution.SolutionState.Main.MagnetSize) * Methods.Solution.SolutionState.Main.MagnetSize;
            int newGridY = (int)((e.Y / Methods.Solution.SolutionState.Main.Zoom) / Methods.Solution.SolutionState.Main.MagnetSize) * Methods.Solution.SolutionState.Main.MagnetSize;
            Point oldPointGrid = new Point(0, 0);
            Point newPointGrid = new Point(0, 0);
            if (Methods.Solution.SolutionState.Main.UseMagnetMode && ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit())
            {
                if (Methods.Solution.SolutionState.Main.UseMagnetXAxis == true && Methods.Solution.SolutionState.Main.UseMagnetYAxis == true)
                {
                    oldPointGrid = new Point(oldGridX, oldGridY);
                    newPointGrid = new Point(newGridX, newGridY);
                }
                if (Methods.Solution.SolutionState.Main.UseMagnetXAxis && !Methods.Solution.SolutionState.Main.UseMagnetYAxis)
                {
                    oldPointGrid = new Point(oldGridX, (int)(Methods.Solution.SolutionState.Main.LastY / Methods.Solution.SolutionState.Main.Zoom));
                    newPointGrid = new Point(newGridX, (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                }
                if (!Methods.Solution.SolutionState.Main.UseMagnetXAxis && Methods.Solution.SolutionState.Main.UseMagnetYAxis)
                {
                    oldPointGrid = new Point((int)(Methods.Solution.SolutionState.Main.LastX / Methods.Solution.SolutionState.Main.Zoom), oldGridY);
                    newPointGrid = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), newGridY);
                }
                if (!Methods.Solution.SolutionState.Main.UseMagnetXAxis && !Methods.Solution.SolutionState.Main.UseMagnetYAxis)
                {
                    oldPointGrid = Methods.Solution.SolutionState.Main.GetLastXY();
                    newPointGrid = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                }
            }
            Point oldPoint = Methods.Solution.SolutionState.Main.GetLastXY();
            Point newPoint = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));


            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit())
            {
                Point OldPointA = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(oldPoint.X, oldPoint.Y);
                Point NewPointA = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(newPoint.X, newPoint.Y);
                Methods.Solution.CurrentSolution.EditLayerA?.MoveSelected(OldPointA, NewPointA, AltPressed());
                Methods.Solution.CurrentSolution.EditLayerB?.MoveSelected(OldPointA, NewPointA, AltPressed());
            }
            else
            {
                Methods.Solution.CurrentSolution.EditLayerA?.MoveSelected(oldPoint, newPoint, AltPressed());
                Methods.Solution.CurrentSolution.EditLayerB?.MoveSelected(oldPoint, newPoint, AltPressed());
            }

            Instance.ViewPanel.SharpPanel.GraphicPanel.Render();
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit())
            {
                if (Methods.Solution.SolutionState.Main.UseMagnetMode)
                {
                    int x = Methods.Solution.CurrentSolution.Entities.GetSelectedEntity().Position.X.High;
                    int y = Methods.Solution.CurrentSolution.Entities.GetSelectedEntity().Position.Y.High;

                    if (x % Methods.Solution.SolutionState.Main.MagnetSize != 0 && Methods.Solution.SolutionState.Main.UseMagnetXAxis)
                    {
                        int offsetX = x % Methods.Solution.SolutionState.Main.MagnetSize;
                        oldPointGrid.X -= offsetX;
                    }
                    if (y % Methods.Solution.SolutionState.Main.MagnetSize != 0 && Methods.Solution.SolutionState.Main.UseMagnetYAxis)
                    {
                        int offsetY = y % Methods.Solution.SolutionState.Main.MagnetSize;
                        oldPointGrid.Y -= offsetY;
                    }
                }


                try
                {

                    if (Methods.Solution.SolutionState.Main.UseMagnetMode)
                    {
                        Methods.Solution.CurrentSolution.Entities.MoveSelected(oldPointGrid, newPointGrid, AltPressed() && Methods.Solution.SolutionState.Main.StartDragged);
                    }
                    else
                    {
                        Methods.Solution.CurrentSolution.Entities.MoveSelected(oldPoint, newPoint, AltPressed() && Methods.Solution.SolutionState.Main.StartDragged);
                    }

                }
                catch (Classes.Scene.EditorEntities.TooManyEntitiesException)
                {
                    System.Windows.MessageBox.Show("Too many entities! (limit: 2048)");
                    Methods.Solution.SolutionState.Main.Dragged = false;
                    return;
                }
                if (Methods.Solution.SolutionState.Main.UseMagnetMode)
                {
                    Methods.Solution.SolutionState.Main.DraggedX += newPointGrid.X - oldPointGrid.X;
                    Methods.Solution.SolutionState.Main.DraggedY += newPointGrid.Y - oldPointGrid.Y;
                }
                else
                {
                    Methods.Solution.SolutionState.Main.DraggedX += newPoint.X - oldPoint.X;
                    Methods.Solution.SolutionState.Main.DraggedY += newPoint.Y - oldPoint.Y;
                }
                Instance.EntitiesToolbar.UpdateSelectedProperties();
            }
            Methods.Solution.SolutionState.Main.StartDragged = false;
        }
        private static void MouseMove_SetSelectionBounds(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Solution.SolutionState.Main.RegionX2 != e.X && Methods.Solution.SolutionState.Main.RegionY2 != e.Y)
            {
                Methods.Solution.SolutionState.Main.TempSelectX1 = (int)(Methods.Solution.SolutionState.Main.RegionX2 / Methods.Solution.SolutionState.Main.Zoom);
                Methods.Solution.SolutionState.Main.TempSelectX2 = (int)(e.X / Methods.Solution.SolutionState.Main.Zoom);
                Methods.Solution.SolutionState.Main.TempSelectY1 = (int)(Methods.Solution.SolutionState.Main.RegionY2 / Methods.Solution.SolutionState.Main.Zoom);
                Methods.Solution.SolutionState.Main.TempSelectY2 = (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom);
                if (Methods.Solution.SolutionState.Main.TempSelectX1 > Methods.Solution.SolutionState.Main.TempSelectX2)
                {
                    Methods.Solution.SolutionState.Main.TempSelectX1 = (int)(e.X / Methods.Solution.SolutionState.Main.Zoom);
                    Methods.Solution.SolutionState.Main.TempSelectX2 = (int)(Methods.Solution.SolutionState.Main.RegionX2 / Methods.Solution.SolutionState.Main.Zoom);
                }
                if (Methods.Solution.SolutionState.Main.TempSelectY1 > Methods.Solution.SolutionState.Main.TempSelectY2)
                {
                    Methods.Solution.SolutionState.Main.TempSelectY1 = (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom);
                    Methods.Solution.SolutionState.Main.TempSelectY2 = (int)(Methods.Solution.SolutionState.Main.RegionY2 / Methods.Solution.SolutionState.Main.Zoom);
                }

                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit())
                {
                    Point selectStart = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY1);
                    Point selectEnd = Classes.Scene.EditorLayer.GetChunkCoordinatesBottomEdge(Methods.Solution.SolutionState.Main.TempSelectX2, Methods.Solution.SolutionState.Main.TempSelectY2);
                    Methods.Solution.CurrentSolution.EditLayerA?.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());
                    Methods.Solution.CurrentSolution.EditLayerB?.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());
                }
                else
                {
                    Methods.Solution.CurrentSolution.EditLayerA?.TempSelection(new Rectangle(Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY1, Methods.Solution.SolutionState.Main.TempSelectX2 - Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY2 - Methods.Solution.SolutionState.Main.TempSelectY1), CtrlPressed());
                    Methods.Solution.CurrentSolution.EditLayerB?.TempSelection(new Rectangle(Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY1, Methods.Solution.SolutionState.Main.TempSelectX2 - Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY2 - Methods.Solution.SolutionState.Main.TempSelectY1), CtrlPressed());
                }

                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) Methods.Solution.CurrentSolution.Entities.TempSelection(new Rectangle(Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY1, Methods.Solution.SolutionState.Main.TempSelectX2 - Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY2 - Methods.Solution.SolutionState.Main.TempSelectY1), CtrlPressed());
            }
        }
        private static void MouseMove_EdgeMove(System.Windows.Forms.MouseEventArgs e)
        {
            Point CurrentPos = new Point(Methods.Solution.SolutionState.Main.ViewPositionX, Methods.Solution.SolutionState.Main.ViewPositionY);

            double ScreenMaxX = CurrentPos.X + (int)Instance.ViewPanel.SharpPanel.ActualWidth;
            double ScreenMaxY = CurrentPos.Y + (int)Instance.ViewPanel.SharpPanel.ActualHeight;
            double ScreenMinX = CurrentPos.X;
            double ScreenMinY = CurrentPos.Y;

            double x = CurrentPos.X;
            double y = CurrentPos.Y;

            if (e.X > ScreenMaxX) x += (e.X - ScreenMaxX) / 10;
            else if (e.X < ScreenMinX) x += (e.X - ScreenMinX) / 10;
            if (e.Y > ScreenMaxY) y += (e.Y - ScreenMaxY) / 10;
            else if (e.Y < ScreenMinY) y += (e.Y - ScreenMinY) / 10;

            if (!Methods.Solution.SolutionState.Main.UnlockCamera)
            {
                if (x < 0) x = 0;
                if (y < 0) y = 0;
                if (x > Instance.ViewPanel.SharpPanel.hScrollBar1.Maximum) x = Instance.ViewPanel.SharpPanel.hScrollBar1.Maximum;
                if (y > Instance.ViewPanel.SharpPanel.vScrollBar1.Maximum) y = Instance.ViewPanel.SharpPanel.vScrollBar1.Maximum;
            }

            if (x != CurrentPos.X || y != CurrentPos.Y)
            {
                if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible) Instance.ViewPanel.SharpPanel.vScrollBar1.Value = y;
                if (Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) Instance.ViewPanel.SharpPanel.hScrollBar1.Value = x;
                Instance.ViewPanel.SharpPanel.GraphicPanel.OnMouseMoveEventCreate();
                if (Methods.Solution.SolutionState.Main.Dragged || Methods.Solution.SolutionState.Main.DraggingSelection) Instance.ViewPanel.SharpPanel.GraphicPanel.Render();
            }
        }
        public static void TilesEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Solution.SolutionState.Main.IsDrawMode())
            {
                TilesEditDrawTool(e, false);
            }
        }
        public static void TilesEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging

            int x = (int)(Methods.Solution.SolutionState.Main.RegionX1 / Methods.Solution.SolutionState.Main.Zoom);
            int y = (int)(Methods.Solution.SolutionState.Main.RegionY1 / Methods.Solution.SolutionState.Main.Zoom);

            Point clicked_point = new Point(x, y);
            Point chunk_point = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);

            bool PointASelected = false;
            bool PointBSelected = false;

            bool HasTileAtA = Methods.Solution.CurrentSolution.EditLayerA?.HasTileAt(clicked_point) ?? false;
            bool HasTileAtB = Methods.Solution.CurrentSolution.EditLayerB?.HasTileAt(clicked_point) ?? false;

            if (Methods.Solution.SolutionState.Main.IsChunksEdit())
            {
                PointASelected = Methods.Solution.CurrentSolution.EditLayerA?.DoesChunkContainASelectedTile(chunk_point) ?? false;
                PointBSelected = Methods.Solution.CurrentSolution.EditLayerB?.DoesChunkContainASelectedTile(chunk_point) ?? false;
            }
            else
            {
                PointASelected = Methods.Solution.CurrentSolution.EditLayerA?.IsPointSelected(clicked_point) ?? false;
                PointBSelected = Methods.Solution.CurrentSolution.EditLayerB?.IsPointSelected(clicked_point) ?? false;
            }


            bool CanDragSelected = (PointASelected || PointBSelected);
            bool CanDragNonSelected = !Methods.Solution.SolutionState.Main.IsSelectMode() && !ShiftPressed() && !CtrlPressed() && (HasTileAtA || HasTileAtB);


            if (CanDragSelected)
            {
                // Start dragging the tiles
                Methods.Solution.SolutionState.Main.Dragged = true;
                Methods.Solution.SolutionState.Main.StartDragged = true;
                Methods.Solution.CurrentSolution.EditLayerA?.StartDrag();
                Methods.Solution.CurrentSolution.EditLayerB?.StartDrag();
            }

            else if (CanDragNonSelected)
            {
                // Start dragging the single selected tile

                if (Methods.Solution.SolutionState.Main.IsChunksEdit())
                {
                    Methods.Solution.CurrentSolution.EditLayerA?.SelectChunk(clicked_point);
                    Methods.Solution.CurrentSolution.EditLayerB?.SelectChunk(clicked_point);
                }
                else
                {
                    Methods.Solution.CurrentSolution.EditLayerA?.Select(clicked_point);
                    Methods.Solution.CurrentSolution.EditLayerB?.Select(clicked_point);
                }
                Methods.Solution.SolutionState.Main.Dragged = true;
                Methods.Solution.SolutionState.Main.StartDragged = true;
                Methods.Solution.CurrentSolution.EditLayerA?.StartDrag();
                Methods.Solution.CurrentSolution.EditLayerB?.StartDrag();
            }

            else
            {
                // Start drag selection
                Methods.Solution.CurrentSolution.EditLayerA?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
                Methods.Solution.CurrentSolution.EditLayerB?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
                if (!ShiftPressed() && !CtrlPressed()) ManiacEditor.Methods.Solution.SolutionActions.Deselect();

                Methods.Solution.SolutionState.Main.DraggingSelection = true;
                Methods.Solution.SolutionState.Main.RegionX2 = Methods.Solution.SolutionState.Main.RegionX1;
                Methods.Solution.SolutionState.Main.RegionY2 = Methods.Solution.SolutionState.Main.RegionY1;
            }
        }
        public static void EntitiesEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Solution.SolutionState.Main.IsDrawMode()) EntitiesEditDrawTool(e);
        }
        public static void EntitiesEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(Methods.Solution.SolutionState.Main.RegionX1 / Methods.Solution.SolutionState.Main.Zoom), (int)(Methods.Solution.SolutionState.Main.RegionY1 / Methods.Solution.SolutionState.Main.Zoom));
            if (Methods.Solution.CurrentSolution.Entities.GetEntityAt(clicked_point)?.Selected ?? false)
            {
                SetClickedXY(e);
                // Start dragging the entity
                Methods.Solution.SolutionState.Main.Dragged = true;
                Methods.Solution.SolutionState.Main.DraggedX = 0;
                Methods.Solution.SolutionState.Main.DraggedY = 0;
                Methods.Solution.SolutionState.Main.StartDragged = true;

            }
            else
            {
                // Start drag selection
                if (!ShiftPressed() && !CtrlPressed())
                    ManiacEditor.Methods.Solution.SolutionActions.Deselect();
                Methods.Solution.SolutionState.Main.DraggingSelection = true;
                Methods.Solution.SolutionState.Main.RegionX2 = Methods.Solution.SolutionState.Main.RegionX1;
                Methods.Solution.SolutionState.Main.RegionY2 = Methods.Solution.SolutionState.Main.RegionY1;

            }
        }
        public static void InteractiveMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Runtime.GameHandler.PlayerSelected)
            {
                Task.Run(() =>
                {
                    Methods.Runtime.GameHandler.MovePlayer(new Point(e.X, e.Y), Methods.Solution.SolutionState.Main.Zoom, Methods.Runtime.GameHandler.SelectedPlayer);
                });
            }

            if (Methods.Runtime.GameHandler.CheckpointSelected)
            {
                Task.Run(() =>
                {
                    Point clicked_point = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                    Methods.Runtime.GameHandler.UpdateCheckpoint(clicked_point, true);
                });
            }
        }
        #endregion


        #region Mouse Up Events
        public static void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            EndTileDraw();
            if (e.Button == MouseButtons.Middle) ToggleAutoScrollerMode(e, true);
            if (Methods.Solution.SolutionState.Main.DraggingSelection) MouseUpDraggingSelection(e);
            else
            {
                if (Methods.Solution.SolutionState.Main.RegionX1 != -1)
                {
                    // So it was just click
                    MouseClick(sender, e);
                }
                if (Methods.Solution.SolutionState.Main.Dragged && (Methods.Solution.SolutionState.Main.DraggedX != 0 || Methods.Solution.SolutionState.Main.DraggedY != 0)) Actions.UndoRedoModel.UpdateEditEntitiesActions();
                Methods.Solution.SolutionState.Main.DraggingSelection = false;
                Methods.Solution.SolutionState.Main.Dragged = false;
            }
            Actions.UndoRedoModel.UpdateEditLayerActions();
            Methods.Internal.UserInterface.UpdateControls(UserInterface.UpdateType.MouseClick);


        }
        public static void MouseUpDraggingSelection(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Solution.SolutionState.Main.RegionX2 != e.X && Methods.Solution.SolutionState.Main.RegionY2 != e.Y)
            {
                int x1 = (int)(Methods.Solution.SolutionState.Main.RegionX2 / Methods.Solution.SolutionState.Main.Zoom), x2 = (int)(e.X / Methods.Solution.SolutionState.Main.Zoom);
                int y1 = (int)(Methods.Solution.SolutionState.Main.RegionY2 / Methods.Solution.SolutionState.Main.Zoom), y2 = (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom);
                if (x1 > x2)
                {
                    x1 = (int)(e.X / Methods.Solution.SolutionState.Main.Zoom);
                    x2 = (int)(Methods.Solution.SolutionState.Main.RegionX2 / Methods.Solution.SolutionState.Main.Zoom);
                }
                if (y1 > y2)
                {
                    y1 = (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom);
                    y2 = (int)(Methods.Solution.SolutionState.Main.RegionY2 / Methods.Solution.SolutionState.Main.Zoom);
                }

                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit())
                {
                    Point selectStart = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY1);
                    Point selectEnd = Classes.Scene.EditorLayer.GetChunkCoordinatesBottomEdge(Methods.Solution.SolutionState.Main.TempSelectX2, Methods.Solution.SolutionState.Main.TempSelectY2);

                    Methods.Solution.CurrentSolution.EditLayerA?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                    Methods.Solution.CurrentSolution.EditLayerB?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                else
                {
                    Methods.Solution.CurrentSolution.EditLayerA?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                    Methods.Solution.CurrentSolution.EditLayerB?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());

                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) Methods.Solution.CurrentSolution.Entities.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                Methods.Internal.UserInterface.UpdateControls(UserInterface.UpdateType.MouseClick);
                Actions.UndoRedoModel.UpdateEditLayerActions();

            }
            Methods.Solution.SolutionState.Main.DraggingSelection = false;
            Methods.Solution.CurrentSolution.EditLayerA?.EndTempSelection();
            Methods.Solution.CurrentSolution.EditLayerB?.EndTempSelection();

            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) Methods.Solution.CurrentSolution.Entities.EndTempSelection();
        }
        #endregion

        #region Mouse Up Methods
        public static void TilesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));

            if (!Methods.Solution.SolutionState.Main.DraggingSelection)
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit())
                {
                    Methods.Solution.CurrentSolution.EditLayerA?.SelectChunk(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
                    Methods.Solution.CurrentSolution.EditLayerB?.SelectChunk(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                else
                {
                    Methods.Solution.CurrentSolution.EditLayerA?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
                    Methods.Solution.CurrentSolution.EditLayerB?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
            }
        }
        public static void EntitiesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
            if (e.Button == MouseButtons.Left)
            {
                Methods.Solution.CurrentSolution.Entities.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
            }
            else if (e.Button == MouseButtons.Right)
            {

            }
        }

        public static void InteractiveMouseUp(System.Windows.Forms.MouseEventArgs e)
        {

        }

        #endregion


        #region Mouse Down Events
        public static void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!Methods.Solution.SolutionState.Main.AutoScrolling) Instance.ViewPanel.SharpPanel.GraphicPanel.Focus();

            if (e.Button == MouseButtons.Left) MouseDownLeft(e);
            else if (e.Button == MouseButtons.Right) MouseDownRight(e);
            else if (e.Button == MouseButtons.Middle) MouseDownMiddle(e);
        }
        public static void MouseDownRight(System.Windows.Forms.MouseEventArgs e)
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) TilesEditMouseDown(e);
            else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) EntitiesEditMouseDown(e);
        }
        public static void MouseDownLeft(System.Windows.Forms.MouseEventArgs e)
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEditing() && !Methods.Solution.SolutionState.Main.Dragged)
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) TilesEditMouseDown(e);
                else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) EntitiesEditMouseDown(e);
            }
            InteractiveMouseDown(e);
        }
        public static void MouseDownMiddle(System.Windows.Forms.MouseEventArgs e)
        {
            ToggleAutoScrollerMode(e);
        }
        #endregion

        #region Mouse Down Methods
        public static void TilesEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                if (Methods.Solution.SolutionState.Main.IsDrawMode())
                {
                    Methods.Solution.CurrentSolution.EditLayerA?.StartDraw();
                    Methods.Solution.CurrentSolution.EditLayerB?.StartDraw();
                    TilesEditDrawTool(e, true);
                }
                else SetClickedXY(e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Methods.Solution.SolutionState.Main.IsDrawMode())
                {
                    Methods.Solution.CurrentSolution.EditLayerA?.StartDraw();
                    Methods.Solution.CurrentSolution.EditLayerB?.StartDraw();
                    TilesEditDrawTool(e, true);
                }
            }
        }
        public static void EntitiesEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!Methods.Solution.SolutionState.Main.IsDrawMode())
                {
                    Point clicked_point = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                    if (Methods.Solution.CurrentSolution.Entities.GetEntityAt(clicked_point)?.Selected ?? false)
                    {
                        // We will have to check if this dragging or clicking
                        SetClickedXY(e);
                    }
                    else if (!ShiftPressed() && !CtrlPressed() && Methods.Solution.CurrentSolution.Entities.GetEntityAt(clicked_point) != null)
                    {
                        Methods.Solution.CurrentSolution.Entities.Select(clicked_point);
                        Methods.Internal.UserInterface.UpdateControls(UserInterface.UpdateType.MouseHeld);
                        // Start dragging the single selected entity
                        Methods.Solution.SolutionState.Main.Dragged = true;
                        Methods.Solution.SolutionState.Main.DraggedX = 0;
                        Methods.Solution.SolutionState.Main.DraggedY = 0;
                        Methods.Solution.SolutionState.Main.StartDragged = true;
                    }
                    else
                    {
                        SetClickedXY(e);
                    }
                }
                else if (Methods.Solution.SolutionState.Main.IsDrawMode()) EntitiesEditDrawTool(e, true);
            }
            if (Instance.EditorToolbar.SplineToolButton.IsChecked.Value) SplineToolMouseDown(e);
        }
        public static void InteractiveMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Methods.Runtime.GameHandler.PlayerSelected)
                {
                    Methods.Runtime.GameHandler.PlayerSelected = false;
                    Methods.Runtime.GameHandler.SelectedPlayer = 0;
                }
                if (Methods.Runtime.GameHandler.CheckpointSelected)
                {
                    Methods.Runtime.GameHandler.CheckpointSelected = false;
                }
            }
        }
        public static void ChunksEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Methods.Solution.SolutionState.Main.IsDrawMode())
                {
                    Point p = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                    Point pC = Classes.Scene.EditorLayer.GetChunkCoordinates(p.X, p.Y);

                    if (Methods.Solution.SolutionState.Main.IsDrawMode())
                    {
                        int selectedIndex = Instance.TilesToolbar.ChunkList.SelectedIndex;
                        // Place Stamp
                        if (selectedIndex != -1)
                        {
                            if (!Instance.Chunks.DoesChunkMatch(pC, Instance.Chunks.GetStamp(selectedIndex), Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB))
                            {
                                Instance.Chunks.PasteStamp(pC, selectedIndex, Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB);
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
                if (Methods.Solution.SolutionState.Main.IsDrawMode())
                {
                    Point p = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                    Point chunk_point = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
                    Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

                    // Remove Stamp Sized Area
                    if (!Methods.Solution.CurrentSolution.EditLayerA.DoesChunkContainASelectedTile(p)) Methods.Solution.CurrentSolution.EditLayerA?.Select(clicked_chunk);
                    if (Methods.Solution.CurrentSolution.EditLayerB != null && !Methods.Solution.CurrentSolution.EditLayerB.DoesChunkContainASelectedTile(p)) Methods.Solution.CurrentSolution.EditLayerB?.Select(clicked_chunk);
                    ManiacEditor.Methods.Solution.SolutionActions.Delete();
                }
            }
        }
        public static void SplineToolMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                if (Methods.Solution.CurrentSolution.Entities.IsEntityAt(clicked_point) == true)
                {
                    ManiacEditor.Methods.Solution.SolutionActions.Deselect();
                    Methods.Solution.CurrentSolution.Entities.GetEntityAt(clicked_point).Selected = true;
                }
                else
                {
                    
                    Methods.Solution.CurrentSolution.Entities.SpawnInternalSplineObject(new Position((short)clicked_point.X, (short)clicked_point.Y), Instance.EditorToolbar.SplineSpawnID.Value.Value);
                    Actions.UndoRedoModel.UpdateEditEntityActions();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point clicked_point = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                Classes.Scene.EditorEntity atPoint = Methods.Solution.CurrentSolution.Entities.GetEntityAt(clicked_point);
                if (atPoint != null && atPoint.Object.Name.Name == "Spline")
                {
                    ManiacEditor.Methods.Solution.SolutionActions.Deselect();
                    Methods.Solution.CurrentSolution.Entities.GetEntityAt(clicked_point).Selected = true;
                    Methods.Solution.CurrentSolution.Entities.DeleteSelected(true);
                }
            }
        }
        #endregion


        #region Mouse Wheel Events
        public static void MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Instance.ViewPanel.SharpPanel.GraphicPanel.Focus();
            if (CtrlPressed()) MouseWheelZooming(sender, e);
            else MouseWheelScrolling(sender, e);
        }
        private static void MouseWheelZooming(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int maxZoom;
            int minZoom;
            if (Properties.Settings.MyPerformance.ReduceZoom)
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
            Methods.Solution.SolutionState.Main.ZoomLevel += change;
            if (Methods.Solution.SolutionState.Main.ZoomLevel > maxZoom) Methods.Solution.SolutionState.Main.ZoomLevel = maxZoom;
            if (Methods.Solution.SolutionState.Main.ZoomLevel < minZoom) Methods.Solution.SolutionState.Main.ZoomLevel = minZoom;

            Instance.ViewPanel.SharpPanel.UpdateZoomLevel(Methods.Solution.SolutionState.Main.ZoomLevel, new Point(e.X - Methods.Solution.SolutionState.Main.ViewPositionX, e.Y - Methods.Solution.SolutionState.Main.ViewPositionY));
        }
        private static void MouseWheelScrolling(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible || Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) ScrollMove();

            void ScrollMove()
            {
                if (Methods.Solution.SolutionState.Main.ScrollDirection == Axis.Y)
                {
                    if (ShiftPressed())
                    {
                        if (Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) MouseWheelScrollingX(sender, e);
                        else MouseWheelScrollingY(sender, e);
                    }
                    else
                    {
                        if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible) MouseWheelScrollingY(sender, e);
                        else MouseWheelScrollingX(sender, e);
                    }
                }
                else if (Methods.Solution.SolutionState.Main.ScrollDirection == Axis.X)
                {
                    if (ShiftPressed())
                    {
                        if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible) MouseWheelScrollingY(sender, e);
                        else MouseWheelScrollingX(sender, e);
                    }
                    else
                    {
                        if (Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) MouseWheelScrollingX(sender, e);
                        else MouseWheelScrollingY(sender, e);
                    }
                }
            }
        }
        private static void MouseWheelScrollingY(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            double y = Instance.ViewPanel.SharpPanel.vScrollBar1.Value - e.Delta;
            if (!Methods.Solution.SolutionState.Main.UnlockCamera)
            {
                if (y < 0) y = 0;
                if (y > Instance.ViewPanel.SharpPanel.vScrollBar1.Maximum) y = Instance.ViewPanel.SharpPanel.vScrollBar1.Maximum;
            }
            Instance.ViewPanel.SharpPanel.vScrollBar1.Value = y;
        }
        private static void MouseWheelScrollingX(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            double x = Instance.ViewPanel.SharpPanel.hScrollBar1.Value - e.Delta;
            if (!Methods.Solution.SolutionState.Main.UnlockCamera)
            {
                if (x < 0) x = 0;
                if (x > Instance.ViewPanel.SharpPanel.hScrollBar1.Maximum) x = Instance.ViewPanel.SharpPanel.hScrollBar1.Maximum;
            }
            Instance.ViewPanel.SharpPanel.hScrollBar1.Value = x;
        }
        #endregion


        #region Mouse Click Events/Methods
        public static void MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Instance.ViewPanel.SharpPanel.GraphicPanel.Focus();
            if (e.Button == MouseButtons.Left && !Methods.Solution.SolutionState.Main.Dragged)
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) TilesEditMouseUp(e);
                else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) EntitiesEditMouseUp(e);
            }
            if (e.Button == MouseButtons.Right)
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit() && !Methods.Solution.SolutionState.Main.IsDrawMode() && !Instance.EditorToolbar.SplineToolButton.IsChecked.Value) EntitiesEditContextMenu(e);
                else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && !Methods.Solution.SolutionState.Main.IsDrawMode()) TilesEditContextMenu(e);
                else if (!ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()&& !Methods.Solution.SolutionState.Main.IsDrawMode()) InteractiveContextMenu(e);
            }
            Methods.Internal.UserInterface.UpdateControls(UserInterface.UpdateType.MouseClick);
            Methods.Solution.SolutionState.Main.RegionX1 = -1;
            Methods.Solution.SolutionState.Main.RegionY1 = -1;




        }
        public static void TilesEditContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            string newLine = Environment.NewLine;
            Point chunkPos = Classes.Scene.EditorLayer.GetChunkCoordinates((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
            Point tilePos;
            if (e.X == 0 || e.Y == 0) tilePos = new Point(0, 0);
            else tilePos = new Point(e.X / 16, e.Y / 16);

            Instance.EditorStatusBar.PixelPositionMenuItem.Header = "Pixel Position:" + newLine + String.Format("X: {0}, Y: {1}", e.X, e.Y);
            Instance.EditorStatusBar.ChunkPositionMenuItem.Header = "Chunk Position:" + newLine + String.Format("X: {0}, Y: {1}", chunkPos.X, chunkPos.Y);
            Instance.EditorStatusBar.TilePositionMenuItem.Header = "Tile Position:" + newLine + String.Format("X: {0}, Y: {1}", tilePos.X, tilePos.Y);


            Point clicked_point_tile = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
            int tile;
            int tileA = (ushort)(Methods.Solution.CurrentSolution.EditLayerA?.GetTileAt(clicked_point_tile) & 0x3ff);
            int tileB = 0;
            if (Methods.Solution.CurrentSolution.EditLayerB != null)
            {
                tileB = (ushort)(Methods.Solution.CurrentSolution.EditLayerB?.GetTileAt(clicked_point_tile) & 0x3ff);
                if (tileA > 1023 && tileB < 1023) tile = tileB;
                else tile = tileA;
            }
            else tile = tileA;

            Methods.Solution.SolutionState.Main.LastSelectedTileID = tile;
            Instance.EditorStatusBar.TileManiacIntergrationItem.IsEnabled = (tile < 1023);
            Instance.EditorStatusBar.TileManiacIntergrationItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);

            System.Windows.Controls.ContextMenu info = new System.Windows.Controls.ContextMenu();
            info.Style = (System.Windows.Style)Instance.FindResource("DefaultContextMenuStyle");
            info.ItemsSource = Instance.EditorStatusBar.TilesContext.Items;
            info.Foreground = (System.Windows.Media.SolidColorBrush)Instance.FindResource("NormalText");
            info.Background = (System.Windows.Media.SolidColorBrush)Instance.FindResource("NormalBackground");
            info.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
            info.StaysOpen = false;
            info.IsOpen = true;
        }
        public static void EntitiesEditContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
            string newLine = Environment.NewLine;
            if (Methods.Solution.CurrentSolution.Entities.GetEntityAt(clicked_point) != null)
            {
                var currentEntity = Methods.Solution.CurrentSolution.Entities.GetEntityAt(clicked_point);

                Instance.EditorStatusBar.EntityNameItem.Header = String.Format("Entity Name: {0}", currentEntity.Name);
                Instance.EditorStatusBar.EntitySlotIDItem.Header = String.Format("Slot ID: {0}", currentEntity.SlotID);
                Instance.EditorStatusBar.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", currentEntity.Position.X.High, currentEntity.Position.Y.High);
            }
            else
            {
                Instance.EditorStatusBar.EntityNameItem.Header = String.Format("Entity Name: {0}", "N/A");
                Instance.EditorStatusBar.EntitySlotIDItem.Header = String.Format("Slot ID: {0} {1} Runtime Slot ID: {2}", "N/A", Environment.NewLine, "N/A");
                Instance.EditorStatusBar.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", e.X, e.Y);
            }
            System.Windows.Controls.ContextMenu info = new System.Windows.Controls.ContextMenu();
            info.Style = (System.Windows.Style)Instance.FindResource("DefaultContextMenuStyle");
            info.ItemsSource = Instance.EditorStatusBar.EntityContext.Items;
            info.Foreground = (System.Windows.Media.SolidColorBrush)Instance.FindResource("NormalText");
            info.Background = (System.Windows.Media.SolidColorBrush)Instance.FindResource("NormalBackground");
            info.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
            info.StaysOpen = false;
            info.IsOpen = true;
        }
        public static void InteractiveContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit())
            {
                Point clicked_point_tile = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                int tile;
                int tileA = (ushort)(Methods.Solution.CurrentSolution.EditLayerA?.GetTileAt(clicked_point_tile) & 0x3ff);
                int tileB = 0;
                if (Methods.Solution.CurrentSolution.EditLayerB != null)
                {
                    tileB = (ushort)(Methods.Solution.CurrentSolution.EditLayerB?.GetTileAt(clicked_point_tile) & 0x3ff);
                    if (tileA > 1023 && tileB < 1023) tile = tileB;
                    else tile = tileA;
                }
                else tile = tileA;


                Methods.Solution.SolutionState.Main.LastSelectedTileID = tile;
                Instance.ViewPanel.SharpPanel.editTile0WithTileManiacToolStripMenuItem.IsEnabled = (tile < 1023);
                Instance.ViewPanel.SharpPanel.moveThePlayerToHereToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.removeCheckpointToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning && Methods.Runtime.GameHandler.CheckpointEnabled;
                Instance.ViewPanel.SharpPanel.assetResetToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.restartSceneToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.moveCheckpointToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning && Methods.Runtime.GameHandler.CheckpointEnabled;


                Instance.ViewPanel.SharpPanel.editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);
                Instance.ViewPanel.SharpPanel.ViewPanelContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                Instance.ViewPanel.SharpPanel.ViewPanelContextMenu.IsOpen = true;
            }
            else
            {
                Point clicked_point_tile = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                string tile = "N/A";
                Instance.ViewPanel.SharpPanel.editTile0WithTileManiacToolStripMenuItem.IsEnabled = false;
                Instance.ViewPanel.SharpPanel.moveThePlayerToHereToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.moveCheckpointToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning;

                Instance.ViewPanel.SharpPanel.setPlayerRespawnToHereToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.removeCheckpointToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.assetResetToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.restartSceneToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning;
                Instance.ViewPanel.SharpPanel.moveCheckpointToolStripMenuItem.IsEnabled = Methods.Runtime.GameHandler.GameRunning;

                Instance.ViewPanel.SharpPanel.editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Collision of Tile {0} in Tile Maniac", tile);
                Instance.ViewPanel.SharpPanel.ViewPanelContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                Instance.ViewPanel.SharpPanel.ViewPanelContextMenu.IsOpen = true;
            }
        }
        #endregion


        #region Draw Tool Events/Methods
        public static void EndTileDraw()
        {
            Methods.Solution.SolutionState.Main.isTileDrawing = false;
            Methods.Solution.CurrentSolution.EditLayerA?.EndDraw();
            Methods.Solution.CurrentSolution.EditLayerB?.EndDraw();
        }
        public static void TilesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
        {
            Point p = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
            Point pC = Classes.Scene.EditorLayer.GetChunkCoordinates(p.X, p.Y);
            if (click)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Methods.Solution.SolutionState.Main.isTileDrawing = true;
                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit()) PlaceChunk();
                    else PlaceTile();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Methods.Solution.SolutionState.Main.isTileDrawing = true;
                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit()) RemoveChunk();
                    else RemoveTile();
                }
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    Methods.Solution.SolutionState.Main.isTileDrawing = true;
                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit()) PlaceChunk();
                    else PlaceTile();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Methods.Solution.SolutionState.Main.isTileDrawing = true;
                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit()) RemoveChunk();
                    else RemoveTile();
                }
            }

            void RemoveChunk()
            {
                if (!Instance.Chunks.IsChunkEmpty(pC, Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB))
                {
                    // Remove Stamp Sized Area
                    Instance.Chunks.PasteStamp(pC, 0, Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB, true);
                }
            }

            void PlaceChunk()
            {
                int selectedIndex = Instance.TilesToolbar.ChunkList.SelectedIndex;
                // Place Stamp
                if (selectedIndex != -1)
                {
                    if (!Instance.Chunks.DoesChunkMatch(pC, Instance.Chunks.GetStamp(selectedIndex), Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB))
                    {
                        Instance.Chunks.PasteStamp(pC, selectedIndex, Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB);
                    }
                }
            }

            void RemoveTile()
            {
                // Remove tile
                if (Methods.Solution.SolutionState.Main.DrawBrushSize == 1)
                {
                    Methods.Solution.CurrentSolution.EditLayerA?.EraseTiles(p);
                    Methods.Solution.CurrentSolution.EditLayerB?.EraseTiles(p);
                }
                else
                {
                    double size = (Methods.Solution.SolutionState.Main.DrawBrushSize / 2) * Methods.Solution.SolutionConstants.TILE_SIZE;
                    Methods.Solution.CurrentSolution.EditLayerA?.EraseTiles(new Rectangle((int)(p.X - size), (int)(p.Y - size), Methods.Solution.SolutionState.Main.DrawBrushSize * Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionState.Main.DrawBrushSize * Methods.Solution.SolutionConstants.TILE_SIZE));
                    Methods.Solution.CurrentSolution.EditLayerB?.EraseTiles(new Rectangle((int)(p.X - size), (int)(p.Y - size), Methods.Solution.SolutionState.Main.DrawBrushSize * Methods.Solution.SolutionConstants.TILE_SIZE, Methods.Solution.SolutionState.Main.DrawBrushSize * Methods.Solution.SolutionConstants.TILE_SIZE));
                }
            }

            void PlaceTile()
            {
                if (Methods.Solution.SolutionState.Main.DrawBrushSize == 1)
                {
                    if (Instance.TilesToolbar.SelectedTileIndex != -1)
                    {
                        if (Methods.Solution.CurrentSolution.EditLayerA.GetTileAt(p) != Instance.TilesToolbar.SelectedTileIndex) Methods.Solution.CurrentSolution.EditLayerA.PlaceTile(p, Instance.TilesToolbar.SelectedTile);
                        else if (!Methods.Solution.CurrentSolution.EditLayerA.IsPointSelected(p)) Methods.Solution.CurrentSolution.EditLayerA.Select(p);
                    }
                }
                else
                {
                    if (Instance.TilesToolbar.SelectedTileIndex != -1) Methods.Solution.CurrentSolution.EditLayerA.PlaceTile(p, Instance.TilesToolbar.SelectedTile, true);
                }
            }
        }
        public static void EntitiesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
        {
            if (click)
            {
                Methods.Solution.SolutionState.Main.LastX = e.X;
                Methods.Solution.SolutionState.Main.LastY = e.Y;
            }
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                if (Methods.Solution.CurrentSolution.Entities.IsEntityAt(clicked_point, true) == true)
                {
                    ManiacEditor.Methods.Solution.SolutionActions.Deselect();
                    Methods.Solution.CurrentSolution.Entities.GetEntityAt(clicked_point).Selected = true;
                }
                else
                {
                    Instance.EntitiesToolbar.SpawnObject();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point clicked_point = new Point((int)(e.X / Methods.Solution.SolutionState.Main.Zoom), (int)(e.Y / Methods.Solution.SolutionState.Main.Zoom));
                if (Methods.Solution.CurrentSolution.Entities.IsEntityAt(clicked_point, true) == true)
                {
                    ManiacEditor.Methods.Solution.SolutionActions.Deselect();
                    Methods.Solution.CurrentSolution.Entities.GetEntityAt(clicked_point).Selected = true;
                    Methods.Solution.CurrentSolution.Entities.DeleteSelected();
                }
            }
        }
        #endregion

        #endregion

        #region Keyboard Controls

        #region Keyboard Inputs
        public static void GraphicPanel_OnKeyUp(object sender, KeyEventArgs e)
        {
            // Tiles Toolbar Flip Horizontal
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipHTiles, true))
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && Methods.Solution.SolutionState.Main.IsDrawMode())
                    Instance.TilesToolbar.SetSelectTileOption(0, false);
            }
            // Tiles Toolbar Flip Vertical
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipVTiles, true))
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && Methods.Solution.SolutionState.Main.IsDrawMode())
                    Instance.TilesToolbar.SetSelectTileOption(1, false);
            }
        }
        public static void GraphicPanel_OnKeyDown(object sender, KeyEventArgs e)
        {
            // Faster Nudge Toggle
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.NudgeFaster))
            {
                Methods.Solution.SolutionState.Main.EnableFasterNudge ^= true;
            }
            // Scroll Lock Toggle
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ScrollLock))
            {
                Methods.Solution.SolutionState.Main.ScrollLocked ^= true;
            }
            // Switch Scroll Lock Type
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ScrollLockTypeSwitch))
            {
                Methods.Solution.SolutionActions.SetScrollLockDirection();

            }
            // Tiles Toolbar Flip Vertical
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipVTiles, true))
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && Methods.Solution.SolutionState.Main.IsDrawMode())
                    Instance.TilesToolbar.SetSelectTileOption(1, true);
            }
            // Tiles Toolbar Flip Horizontal
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipHTiles, true))
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && Methods.Solution.SolutionState.Main.IsDrawMode())
                    Instance.TilesToolbar.SetSelectTileOption(0, true);
            }
            // Open Click (Alt: Open Data Dir)
            else if ((Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.OpenDataDir)))
            {
                ManiacEditor.Methods.Solution.SolutionLoader.OpenSceneSelect();
            }
            else if ((Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Open)))
            {
                ManiacEditor.Methods.Solution.SolutionLoader.OpenScene();
            }
            // New Click
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.New))
            {
                //ManiacEditor.Methods.Editor.SolutionLoader.NewScene();
            }
            // Save Click (Alt: Save As)
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.SaveAs))
            {
                ManiacEditor.Methods.Solution.SolutionLoader.Save();
            }
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds._Save))
            {
                ManiacEditor.Methods.Solution.SolutionLoader.SaveAs();
            }
            // Undo
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Undo))
            {
                ManiacEditor.Actions.UndoRedoModel.Undo();
            }
            // Redo
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Redo))
            {
                ManiacEditor.Actions.UndoRedoModel.Redo();
            }
            // Developer Interface
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.DeveloperInterface))
            {

            }
            // Save for Force Open on Startup
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ForceOpenOnStartup))
            {

            }
            else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsSceneLoaded())
            {
                GraphicPanel_OnKeyDownLoaded(sender, e);
            }
            // Editing Key Shortcuts
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEditing())
            {
                GraphicPanel_OnKeyDownEditing(sender, e);
            }
            OnKeyDownTools(sender, e);
        }
        public static void GraphicPanel_OnKeyDownLoaded(object sender, KeyEventArgs e)
        {
            // Reset Zoom Level
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ResetZoomLevel))
            {
                Instance.ViewPanel.SharpPanel.UpdateZoomLevel(0, new Point(0, 0));
            }
            //Refresh Tiles and Sprites
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.RefreshResources))
            {
                Methods.Internal.UserInterface.ReloadSpritesAndTextures();
            }
            //Run Scene
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.RunScene))
            {
                Methods.Runtime.GameHandler.RunScene();
            }
            //Show Path A
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ShowPathA) && ManiacEditor.Methods.Solution.SolutionState.Main.IsSceneLoaded())
            {
                Methods.Solution.SolutionState.Main.ShowCollisionA ^= true;
            }
            //Show Path B
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ShowPathB))
            {
                Methods.Solution.SolutionState.Main.ShowCollisionB ^= true;
            }
            //Unload Scene
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.UnloadScene))
            {
                ManiacEditor.Methods.Solution.SolutionLoader.UnloadScene();
            }
            //Toggle Grid Visibility
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ShowGrid))
            {
                Methods.Solution.SolutionState.Main.ShowGrid ^= true;
            }
            //Toggle Tile ID Visibility
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ShowTileID))
            {
                Methods.Solution.SolutionState.Main.ShowTileID ^= true;
            }
            //Status Box Toggle
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.StatusBoxToggle))
            {
                Methods.Solution.SolutionState.Main.DebugStatsVisibleOnPanel ^= true;
                Instance.ViewPanel.InfoHUD.UpdatePopupSize();
            }
        }
        public static void GraphicPanel_OnKeyDownEditing(object sender, KeyEventArgs e)
        {
            //Paste
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Paste))
            {
                Methods.Solution.SolutionActions.Paste();
            }
            //Paste to Chunk
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.PasteToChunk))
            {
                Methods.Solution.SolutionActions.PasteToChunks();
            }
            //Select All
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.SelectAll))
            {
                Methods.Solution.SolutionActions.SelectAll();
            }
            // Selected Key Shortcuts   
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsSelected())
            {
                GraphicPanel_OnKeyDownSelectedEditing(sender, e);
            }
        }
        public static void GraphicPanel_OnKeyDownSelectedEditing(object sender, KeyEventArgs e)
        {
            // Delete
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Delete))
            {
                ManiacEditor.Methods.Solution.SolutionActions.Delete();
            }

            // Moving
            else if (e.KeyData == Keys.Up || e.KeyData == Keys.Down || e.KeyData == Keys.Left || e.KeyData == Keys.Right)
            {
                ManiacEditor.Methods.Solution.SolutionActions.Move(e);
            }

            //Cut 
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Cut))
            {
                Methods.Solution.SolutionActions.Cut();
            }
            //Copy
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Copy))
            {
                Methods.Solution.SolutionActions.Copy();
            }
            //Duplicate
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Duplicate))
            {
                Methods.Solution.SolutionActions.Duplicate();
            }
            //Delete
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Delete))
            {
                Methods.Solution.SolutionActions.Delete();
            }
            // Flip Vertical Individual
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipVIndv))
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit())
                    Methods.Solution.SolutionActions.FlipVerticalIndividual();
            }
            // Flip Horizontal Individual
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipHIndv))
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit())
                    Methods.Solution.SolutionActions.FlipHorizontalIndividual();
            }
            // Flip Vertical
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipV))
            {
                Methods.Solution.SolutionActions.FlipVertical();
            }

            // Flip Horizontal
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipH))
            {
                Methods.Solution.SolutionActions.FlipHorizontal();
            }
        }
        public static void OnKeyDownTools(object sender, KeyEventArgs e)
        {
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.PointerTool) && Instance.EditorToolbar.PointerToolButton.IsEnabled) Methods.Solution.SolutionState.Main.PointerMode(true);
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.SelectTool) && Instance.EditorToolbar.SelectToolButton.IsEnabled) Methods.Solution.SolutionState.Main.SelectionMode(true);
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.DrawTool) && Instance.EditorToolbar.DrawToolButton.IsEnabled) Methods.Solution.SolutionState.Main.DrawMode(true);
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.MagnetTool) && Instance.EditorToolbar.MagnetMode.IsEnabled) Methods.Solution.SolutionState.Main.UseMagnetMode ^= true;
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.SplineTool) && Instance.EditorToolbar.SplineToolButton.IsEnabled) Methods.Solution.SolutionState.Main.SplineMode(true);
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.StampTool) && Instance.EditorToolbar.ChunksToolButton.IsEnabled) Methods.Solution.SolutionState.Main.ChunksMode();
        }
        #endregion

        #region Tile Maniac Keyboard Inputs
        public static void TileManiac_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacNewInstance))
            {
                CollisionEditor.Instance.newInstanceToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacOpen))
            {
                CollisionEditor.Instance.OpenCollision();
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacSave))
            {
                CollisionEditor.Instance.saveToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacSaveAs))
            {
                CollisionEditor.Instance.saveAsToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacSaveUncompressed))
            {
                CollisionEditor.Instance.saveUncompressedToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacSaveAsUncompressed))
            {
                CollisionEditor.Instance.saveAsUncompressedToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacbackupConfig))
            {
                CollisionEditor.Instance.tileConfigbinToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacbackupImage))
            {
                CollisionEditor.Instance.x16TilesgifToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacExportColMask))
            {
                CollisionEditor.Instance.exportCurrentCollisionMaskAsToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacOpenSingleColMask))
            {
                CollisionEditor.Instance.openSingleCollisionMaskToolStripMenuItem_Click_1(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacImportFromOlderRSDK))
            {
                CollisionEditor.Instance.importFromOlderRSDKVersionToolStripMenuItem_Click(null, null);
            }

            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacCopy))
            {
                CollisionEditor.Instance.copyToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacPastetoOther))
            {
                CollisionEditor.Instance.copyToOtherPathToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacPaste))
            {
                CollisionEditor.Instance.pasteToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacMirrorMode))
            {
                CollisionEditor.Instance.mirrorPathsToolStripMenuItem1.IsChecked = !CollisionEditor.Instance.mirrorPathsToolStripMenuItem1.IsChecked;
                CollisionEditor.Instance.mirrorPathsToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacRestorePathA))
            {
                CollisionEditor.Instance.pathAToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacRestorePathB))
            {
                CollisionEditor.Instance.pathBToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacRestorePaths))
            {
                CollisionEditor.Instance.bothToolStripMenuItem_Click(null, null);
            }

            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacShowPathB))
            {
                CollisionEditor.Instance.showPathBToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacShowGrid))
            {
                CollisionEditor.Instance.showGridToolStripMenuItem.IsChecked = !CollisionEditor.Instance.showGridToolStripMenuItem.IsChecked;
                CollisionEditor.Instance.showGridToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacSplitFile))
            {
                CollisionEditor.Instance.splitFileToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacFlipTileH))
            {
                CollisionEditor.Instance.flipTileHorizontallyToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacFlipTileV))
            {
                CollisionEditor.Instance.flipTileVerticallyToolStripMenuItem_Click(null, null);
            }

            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacHomeFolderOpen))
            {
                CollisionEditor.Instance.openCollisionHomeFolderToolStripMenuItem_Click(null, null);
            }

            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacAbout))
            {
                CollisionEditor.Instance.aboutToolStripMenuItem1_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacSettings))
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
