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
        #region Definitions
        private static MainEditor Instance;
        public static void UpdateInstance(MainEditor _instance)
        {
            Instance = _instance;
        }

        private static bool CtrlPressed() { return System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Control); }
        private static bool ShiftPressed() { return System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Shift); }
        private static bool AltPressed() { return System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Alt); }

        private static Timer RepeatTimer { get; set; } = new Timer() { Interval = 100 };
        private static bool CanHandleKeyDown { get; set; } = true;
        private static Keys LastKeyDown { get; set; }
        private static bool InitilizedKeyDefinitions { get; set; } = false;
        #endregion

        #region Constructors

        private static void InitilizeKeyDefinitions()
        {
            RepeatTimer.Tick += EndRepeat;
            InitilizedKeyDefinitions = true;
        }

        #endregion

        #region Key Repeat Methods
        private static void StartRepeat(KeyEventArgs e)
        {
            if (!InitilizedKeyDefinitions) InitilizeKeyDefinitions();
            LastKeyDown = e.KeyCode;
            RepeatTimer.Start();
            CanHandleKeyDown = false;

        }
        private static void EndRepeat(object sender, EventArgs e) { EndRepeat(); }
        private static void EndRepeat()
        {
            if (!InitilizedKeyDefinitions) InitilizeKeyDefinitions();
            LastKeyDown = Keys.None;
            RepeatTimer.Stop();
            CanHandleKeyDown = true;
        }

        #endregion

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
            Methods.Solution.SolutionState.Main.UpdateLastXY(e.X, e.Y);
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
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit())
            {
                int oldGridX = (int)((Methods.Solution.SolutionState.Main.LastX) / Methods.Solution.SolutionState.Main.MagnetSize) * Methods.Solution.SolutionState.Main.MagnetSize;
                int oldGridY = (int)((Methods.Solution.SolutionState.Main.LastY) / Methods.Solution.SolutionState.Main.MagnetSize) * Methods.Solution.SolutionState.Main.MagnetSize;
                int newGridX = (int)((e.X) / Methods.Solution.SolutionState.Main.MagnetSize) * Methods.Solution.SolutionState.Main.MagnetSize;
                int newGridY = (int)((e.Y) / Methods.Solution.SolutionState.Main.MagnetSize) * Methods.Solution.SolutionState.Main.MagnetSize;
                Point oldPointGrid = new Point(0, 0);
                Point newPointGrid = new Point(0, 0);


                if (Methods.Solution.SolutionState.Main.UseMagnetMode)
                {
                    if (Methods.Solution.SolutionState.Main.UseMagnetXAxis == true && Methods.Solution.SolutionState.Main.UseMagnetYAxis == true)
                    {
                        oldPointGrid = new Point(oldGridX, oldGridY);
                        newPointGrid = new Point(newGridX, newGridY);
                    }
                    if (Methods.Solution.SolutionState.Main.UseMagnetXAxis && !Methods.Solution.SolutionState.Main.UseMagnetYAxis)
                    {
                        oldPointGrid = new Point(oldGridX, (int)(Methods.Solution.SolutionState.Main.LastY));
                        newPointGrid = new Point(newGridX, (int)(e.Y));
                    }
                    if (!Methods.Solution.SolutionState.Main.UseMagnetXAxis && Methods.Solution.SolutionState.Main.UseMagnetYAxis)
                    {
                        oldPointGrid = new Point((int)(Methods.Solution.SolutionState.Main.LastX), oldGridY);
                        newPointGrid = new Point((int)(e.X), newGridY);
                    }
                    if (!Methods.Solution.SolutionState.Main.UseMagnetXAxis && !Methods.Solution.SolutionState.Main.UseMagnetYAxis)
                    {
                        oldPointGrid = Methods.Solution.SolutionState.Main.GetLastXY();
                        newPointGrid = new Point((int)(e.X), (int)(e.Y));
                    }
                }

                Point oldPoint = Methods.Solution.SolutionState.Main.GetLastXY();
                Point newPoint = new Point((int)(e.X), (int)(e.Y));

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
                    System.Windows.MessageBox.Show(string.Format("Too many entities! (limit: {0})", ManiacEditor.Methods.Solution.SolutionConstants.ENTITY_LIMIT));
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
                
            }
            else
            {
                Point oldPoint = Methods.Solution.SolutionState.Main.GetLastXY();
                Point newPoint = new Point((int)(e.X), (int)(e.Y));


                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit())
                {
                    Point OldPointA = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(oldPoint.X, oldPoint.Y);
                    Point NewPointA = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(newPoint.X, newPoint.Y);
                    Methods.Solution.SolutionMultiLayer.MoveSelected(OldPointA, NewPointA, AltPressed());
                }
                else
                {
                    Methods.Solution.SolutionMultiLayer.MoveSelected(oldPoint, newPoint, AltPressed());
                }

                if (Instance.ViewPanel.SharpPanel.GraphicPanel.AllowLoopToRender) Instance.ViewPanel.SharpPanel.GraphicPanel.Render();
            }
            Methods.Solution.SolutionState.Main.StartDragged = false;

        }
        private static void MouseMove_SetSelectionBounds(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Solution.SolutionState.Main.RegionX2 != e.X && Methods.Solution.SolutionState.Main.RegionY2 != e.Y)
            {
                Methods.Solution.SolutionState.Main.TempSelectX1 = (int)(Methods.Solution.SolutionState.Main.RegionX2);
                Methods.Solution.SolutionState.Main.TempSelectX2 = (int)(e.X);
                Methods.Solution.SolutionState.Main.TempSelectY1 = (int)(Methods.Solution.SolutionState.Main.RegionY2);
                Methods.Solution.SolutionState.Main.TempSelectY2 = (int)(e.Y);
                if (Methods.Solution.SolutionState.Main.TempSelectX1 > Methods.Solution.SolutionState.Main.TempSelectX2)
                {
                    Methods.Solution.SolutionState.Main.TempSelectX1 = (int)(e.X);
                    Methods.Solution.SolutionState.Main.TempSelectX2 = (int)(Methods.Solution.SolutionState.Main.RegionX2);
                }
                if (Methods.Solution.SolutionState.Main.TempSelectY1 > Methods.Solution.SolutionState.Main.TempSelectY2)
                {
                    Methods.Solution.SolutionState.Main.TempSelectY1 = (int)(e.Y);
                    Methods.Solution.SolutionState.Main.TempSelectY2 = (int)(Methods.Solution.SolutionState.Main.RegionY2);
                }

                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit())
                {
                    Point selectStart = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY1);
                    Point selectEnd = Classes.Scene.EditorLayer.GetChunkCoordinatesBottomEdge(Methods.Solution.SolutionState.Main.TempSelectX2, Methods.Solution.SolutionState.Main.TempSelectY2);
                    Methods.Solution.SolutionMultiLayer.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());
                }
                else
                {
                    Methods.Solution.SolutionMultiLayer.TempSelection(new Rectangle(Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY1, Methods.Solution.SolutionState.Main.TempSelectX2 - Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY2 - Methods.Solution.SolutionState.Main.TempSelectY1), CtrlPressed());
                }

                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) Methods.Solution.CurrentSolution.Entities.TempSelection(new Rectangle(Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY1, Methods.Solution.SolutionState.Main.TempSelectX2 - Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY2 - Methods.Solution.SolutionState.Main.TempSelectY1), CtrlPressed());
            }
        }
        private static void MouseMove_EdgeMove(System.Windows.Forms.MouseEventArgs e)
        {
            Point CurrentPos = new Point(Methods.Solution.SolutionState.Main.ViewPositionX, Methods.Solution.SolutionState.Main.ViewPositionY);

            double ScreenMaxX = CurrentPos.X + (int)Instance.ViewPanel.SharpPanel.ActualWidth * Methods.Solution.SolutionState.Main.Zoom;
            double ScreenMaxY = CurrentPos.Y + (int)Instance.ViewPanel.SharpPanel.ActualHeight * Methods.Solution.SolutionState.Main.Zoom;
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

            int x = (int)(Methods.Solution.SolutionState.Main.RegionX1);
            int y = (int)(Methods.Solution.SolutionState.Main.RegionY1);

            Point clicked_point = new Point(x, y);
            Point chunk_point = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);

            bool CanDragSelected;

            bool HasTileAt = Methods.Solution.SolutionMultiLayer.HasTileAt(clicked_point);

            if (Methods.Solution.SolutionState.Main.IsChunksEdit()) CanDragSelected = Methods.Solution.SolutionMultiLayer.DoesChunkContainASelectedTile(chunk_point);
            else CanDragSelected = Methods.Solution.SolutionMultiLayer.IsPointSelected(clicked_point);

            bool CanDragNonSelected = !Methods.Solution.SolutionState.Main.IsSelectMode() && !ShiftPressed() && !CtrlPressed() && HasTileAt;


            if (CanDragSelected)
            {
                // Start dragging the tiles
                Methods.Solution.SolutionState.Main.Dragged = true;
                Methods.Solution.SolutionState.Main.StartDragged = true;
                Methods.Solution.SolutionMultiLayer.StartDrag();
            }

            else if (CanDragNonSelected)
            {
                // Start dragging the single selected tile

                if (Methods.Solution.SolutionState.Main.IsChunksEdit()) Methods.Solution.SolutionMultiLayer.SelectChunk(clicked_point);
                else Methods.Solution.SolutionMultiLayer.Select(clicked_point);
                Methods.Solution.SolutionState.Main.Dragged = true;
                Methods.Solution.SolutionState.Main.StartDragged = true;
                Methods.Solution.SolutionMultiLayer.StartDrag();
            }

            else
            {
                // Start drag selection
                Methods.Solution.SolutionMultiLayer.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
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
            Point clicked_point = new Point((int)(Methods.Solution.SolutionState.Main.RegionX1), (int)(Methods.Solution.SolutionState.Main.RegionY1));
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
                    Methods.Runtime.GameHandler.MovePlayer(new Point(e.X, e.Y), Methods.Runtime.GameHandler.SelectedPlayer);
                });
            }

            if (Methods.Runtime.GameHandler.CheckpointSelected)
            {
                Task.Run(() =>
                {
                    Point clicked_point = new Point((int)(e.X), (int)(e.Y));
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
            Actions.UndoRedoModel.UpdateEditLayersActions();
            Methods.Internal.UserInterface.UpdateControls(UserInterface.UpdateType.MouseClick);


        }
        public static void MouseUpDraggingSelection(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Solution.SolutionState.Main.RegionX2 != e.X && Methods.Solution.SolutionState.Main.RegionY2 != e.Y)
            {
                int x1 = (int)(Methods.Solution.SolutionState.Main.RegionX2), x2 = (int)(e.X);
                int y1 = (int)(Methods.Solution.SolutionState.Main.RegionY2), y2 = (int)(e.Y);
                if (x1 > x2)
                {
                    x1 = (int)(e.X);
                    x2 = (int)(Methods.Solution.SolutionState.Main.RegionX2);
                }
                if (y1 > y2)
                {
                    y1 = (int)(e.Y);
                    y2 = (int)(Methods.Solution.SolutionState.Main.RegionY2);
                }

                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit())
                {
                    Point selectStart = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(Methods.Solution.SolutionState.Main.TempSelectX1, Methods.Solution.SolutionState.Main.TempSelectY1);
                    Point selectEnd = Classes.Scene.EditorLayer.GetChunkCoordinatesBottomEdge(Methods.Solution.SolutionState.Main.TempSelectX2, Methods.Solution.SolutionState.Main.TempSelectY2);
                    Methods.Solution.SolutionMultiLayer.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                else
                {
                    Methods.Solution.SolutionMultiLayer.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) Methods.Solution.CurrentSolution.Entities.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                Methods.Internal.UserInterface.UpdateControls(UserInterface.UpdateType.MouseClick);
                Actions.UndoRedoModel.UpdateEditLayersActions();

            }
            Methods.Solution.SolutionState.Main.DraggingSelection = false;
            Methods.Solution.SolutionMultiLayer.EndTempSelection();

            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) Methods.Solution.CurrentSolution.Entities.EndTempSelection();
        }
        #endregion

        #region Mouse Up Methods
        public static void TilesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X), (int)(e.Y));

            if (!Methods.Solution.SolutionState.Main.DraggingSelection)
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit()) Methods.Solution.SolutionMultiLayer.SelectChunk(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
                else Methods.Solution.SolutionMultiLayer.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
            }
        }
        public static void EntitiesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X), (int)(e.Y));
            if (e.Button == MouseButtons.Left)
            {
                Methods.Solution.CurrentSolution.Entities.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
            }
            else if (e.Button == MouseButtons.Right)
            {

            }

            if (Instance.EntitiesToolbar != null) Instance.EntitiesToolbar.UpdateSelectedProperties();
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
                Point clicked_point = new Point((int)(e.X), (int)(e.Y));
                if (Methods.Solution.SolutionState.Main.IsDrawMode())
                {
                    Methods.Solution.SolutionMultiLayer.StartDraw();
                    TilesEditDrawTool(e, true);
                }
                else SetClickedXY(e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Methods.Solution.SolutionState.Main.IsDrawMode())
                {
                    Methods.Solution.SolutionMultiLayer.StartDraw();
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
                    Point clicked_point = new Point((int)(e.X), (int)(e.Y));
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
                    if (Methods.Solution.SolutionState.Main.IsDrawMode()) Methods.Solution.SolutionMultiLayer.StampPlace(Instance, e);
                    else SetClickedXY(e);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Methods.Solution.SolutionState.Main.IsDrawMode()) Methods.Solution.SolutionMultiLayer.StampRemove(e);
            }
        }
        public static void SplineToolMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X), (int)(e.Y));
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
                Point clicked_point = new Point((int)(e.X), (int)(e.Y));
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

            Instance.ViewPanel.SharpPanel.UpdateZoomLevel(Methods.Solution.SolutionState.Main.ZoomLevel, new Point(Methods.Solution.SolutionState.Main.ViewPositionX - e.X, Methods.Solution.SolutionState.Main.ViewPositionY - e.Y));
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
            Point chunkPos = Classes.Scene.EditorLayer.GetChunkCoordinates((int)(e.X), (int)(e.Y));
            Point tilePos;
            if (e.X == 0 || e.Y == 0) tilePos = new Point(0, 0);
            else tilePos = new Point(e.X / 16, e.Y / 16);

            Instance.EditorStatusBar.PixelPositionMenuItem.Header = "Pixel Position:" + newLine + String.Format("X: {0}, Y: {1}", e.X, e.Y);
            Instance.EditorStatusBar.ChunkPositionMenuItem.Header = "Chunk Position:" + newLine + String.Format("X: {0}, Y: {1}", chunkPos.X, chunkPos.Y);
            Instance.EditorStatusBar.TilePositionMenuItem.Header = "Tile Position:" + newLine + String.Format("X: {0}, Y: {1}", tilePos.X, tilePos.Y);


            Point clicked_point_tile = new Point((int)(e.X), (int)(e.Y));
            int tile = (ushort)(Methods.Solution.SolutionMultiLayer.GetTileAt(clicked_point_tile) & 0x3ff);

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
            Point clicked_point = new Point((int)(e.X), (int)(e.Y));
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
                Point clicked_point_tile = new Point((int)(e.X), (int)(e.Y));
                int tile = (ushort)(Methods.Solution.SolutionMultiLayer.GetTileAt(clicked_point_tile) & 0x3ff);


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
                Point clicked_point_tile = new Point((int)(e.X), (int)(e.Y));
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
            Methods.Solution.SolutionMultiLayer.EndDraw();
        }
        public static void TilesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
        {
            Point p = new Point((int)(e.X), (int)(e.Y));
            Point pC = Classes.Scene.EditorLayer.GetChunkCoordinates(p.X, p.Y);
            if (click)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Methods.Solution.SolutionState.Main.isTileDrawing = true;
                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit()) Methods.Solution.SolutionMultiLayer.PlaceChunk(Instance, pC);
                    else Methods.Solution.SolutionMultiLayer.PlaceTile(Instance, p);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Methods.Solution.SolutionState.Main.isTileDrawing = true;
                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit()) Methods.Solution.SolutionMultiLayer.RemoveChunk(Instance, pC);
                    else Methods.Solution.SolutionMultiLayer.RemoveTile(Instance, p);
                }
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    Methods.Solution.SolutionState.Main.isTileDrawing = true;
                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit()) Methods.Solution.SolutionMultiLayer.PlaceChunk(Instance, pC);
                    else Methods.Solution.SolutionMultiLayer.PlaceTile(Instance, p);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Methods.Solution.SolutionState.Main.isTileDrawing = true;
                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit()) Methods.Solution.SolutionMultiLayer.RemoveChunk(Instance, pC);
                    else Methods.Solution.SolutionMultiLayer.RemoveTile(Instance, p);
                }
            }


        }
        public static void EntitiesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
        {
            if (click)
            {
                Methods.Solution.SolutionState.Main.UpdateLastXY(e.X, e.Y);
            }
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X), (int)(e.Y));
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
                Point clicked_point = new Point((int)(e.X), (int)(e.Y));
                if (Methods.Solution.CurrentSolution.Entities.IsEntityAt(clicked_point, true) == true)
                {
                    ManiacEditor.Methods.Solution.SolutionActions.Deselect();
                    Methods.Solution.CurrentSolution.Entities.GetEntityAt(clicked_point).Selected = true;
                    Methods.Solution.CurrentSolution.Entities.DeleteSelected();
                }
            }
        }
        #endregion

        #region Keyboard Inputs
        public static void GraphicPanel_OnKeyUp(object sender, KeyEventArgs e)
        {
            EndRepeat();

            // Tiles Toolbar Flip Vertical (OFF)
            if (!ShiftPressed() && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && Methods.Solution.SolutionState.Main.IsDrawMode()) Instance.TilesToolbar.SetSelectTileOption(1, false);
            // Tiles Toolbar Flip Horizontal (OFF)
            if (!CtrlPressed() && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && Methods.Solution.SolutionState.Main.IsDrawMode()) Instance.TilesToolbar.SetSelectTileOption(0, false);
        }
        public static void GraphicPanel_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (CanHandleKeyDown || LastKeyDown != e.KeyCode) StartRepeat(e);
            else return;

            // Tiles Toolbar Flip Vertical (ON)
            if (ShiftPressed() && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && Methods.Solution.SolutionState.Main.IsDrawMode()) Instance.TilesToolbar.SetSelectTileOption(1, true);
            // Tiles Toolbar Flip Horizontal (ON)
            if (CtrlPressed() && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && Methods.Solution.SolutionState.Main.IsDrawMode()) Instance.TilesToolbar.SetSelectTileOption(0, true);


            // Faster Nudge Toggle
            if (e.Control && e.KeyCode == Keys.F1) Methods.Solution.SolutionState.Main.EnableFasterNudge ^= true;
            // Scroll Lock Toggle
            else if (e.Control && e.KeyCode == Keys.F2) Methods.Solution.SolutionActions.SetScrollLockDirection();
            // Switch Scroll Lock Type
            else if (e.Control && e.KeyCode == Keys.F3) Methods.Solution.SolutionActions.SetScrollLockDirection();
            // Open Scene Select
            else if (e.Control && e.Alt && e.KeyCode == Keys.O) ManiacEditor.Methods.Solution.SolutionLoader.OpenSceneSelect();
            // Open Scene
            else if (e.Control && e.KeyCode == Keys.O) ManiacEditor.Methods.Solution.SolutionLoader.OpenScene();

            // New Click
            //else if (e.Control && e.KeyCode == Keys.N) ManiacEditor.Methods.Editor.SolutionLoader.NewScene();

            // Save Click (Alt: Save As)
            else if (e.Control && e.Alt && e.KeyCode == Keys.S) ManiacEditor.Methods.Solution.SolutionLoader.Save();
            else if (e.Control && e.KeyCode == Keys.S) ManiacEditor.Methods.Solution.SolutionLoader.SaveAs();
            // Undo
            else if (e.Control && e.KeyCode == Keys.Z) ManiacEditor.Actions.UndoRedoModel.Undo();
            // Redo
            else if (e.Control && e.KeyCode == Keys.Y) ManiacEditor.Actions.UndoRedoModel.Redo();
            // Reset Zoom Level
            else if ((e.Control && e.KeyCode == Keys.D0) || (e.Control && e.KeyCode == Keys.NumPad0)) Instance.ViewPanel.SharpPanel.UpdateZoomLevel(0, new Point(0, 0));
            //Refresh Tiles and Sprites
            else if (e.KeyCode == Keys.F5) Methods.Internal.UserInterface.ReloadSpritesAndTextures();
            //Run Scene
            else if (e.Control && e.KeyCode == Keys.R) Methods.Runtime.GameHandler.RunScene();
            //Unload Scene
            else if (e.Control && e.KeyCode == Keys.U) ManiacEditor.Methods.Solution.SolutionLoader.UnloadScene();
            //Toggle Grid Visibility
            else if (e.Control && e.KeyCode == Keys.G) Methods.Solution.SolutionState.Main.ShowGrid ^= true;
            //Toggle Tile ID Visibility
            else if (e.Shift && e.KeyCode == Keys.D3) Methods.Solution.SolutionState.Main.ShowTileID ^= true;
            //Status Box Toggle
            else if (e.KeyCode == Keys.F3)
            {
                Methods.Solution.SolutionState.Main.DebugStatsVisibleOnPanel ^= true;
                Instance.ViewPanel.InfoHUD.UpdatePopupSize();
            }

            //Show Path A
            else if (e.Control && e.KeyCode == Keys.D1) Methods.Solution.SolutionState.Main.ShowCollisionA ^= true;
            //Show Path B
            else if (e.Control && e.KeyCode == Keys.D2) Methods.Solution.SolutionState.Main.ShowCollisionB ^= true;

            //Toolbox Tool Switch
            else if ((e.KeyCode == Keys.D1) && Instance.EditorToolbar.PointerToolButton.IsEnabled) Methods.Solution.SolutionState.Main.PointerMode(true);
            else if ((e.KeyCode == Keys.D2) && Instance.EditorToolbar.SelectToolButton.IsEnabled) Methods.Solution.SolutionState.Main.SelectionMode(true);
            else if ((e.KeyCode == Keys.D3) && Instance.EditorToolbar.DrawToolButton.IsEnabled) Methods.Solution.SolutionState.Main.DrawMode(true);
            else if ((e.KeyCode == Keys.D6) && Instance.EditorToolbar.MagnetMode.IsEnabled) Methods.Solution.SolutionState.Main.UseMagnetMode ^= true;
            else if ((e.KeyCode == Keys.D4) && Instance.EditorToolbar.SplineToolButton.IsEnabled) Methods.Solution.SolutionState.Main.SplineMode(true);
            else if ((e.KeyCode == Keys.D5) && Instance.EditorToolbar.ChunksToolButton.IsEnabled) Methods.Solution.SolutionState.Main.ChunksMode();

            // Moving
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) ManiacEditor.Methods.Solution.SolutionActions.Move(e); 
            //Cut 
            else if (e.Control && e.KeyCode == Keys.X) Methods.Solution.SolutionActions.Cut(); 
            //Paste to Chunk
            else if (e.Control && e.Shift && e.KeyCode == Keys.V) Methods.Solution.SolutionActions.PasteToChunks(); 
            //Paste
            else if (e.Control && e.KeyCode == Keys.V) Methods.Solution.SolutionActions.Paste();
            //Select All
            else if (e.Control && e.KeyCode == Keys.A) Methods.Solution.SolutionActions.SelectAll(); 
            //Copy
            else if (e.Control && e.KeyCode == Keys.C) Methods.Solution.SolutionActions.Copy();
            //Duplicate
            else if (e.Control && e.KeyCode == Keys.D) Methods.Solution.SolutionActions.Duplicate();
            //Delete
            else if (e.KeyCode == Keys.Delete) ManiacEditor.Methods.Solution.SolutionActions.Delete();
            // Flip Vertical Individual
            else if ((e.Control && e.KeyCode == Keys.F) && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) Methods.Solution.SolutionActions.FlipVerticalIndividual();
            // Flip Horizontal Individual
            else if ((e.Control && e.KeyCode == Keys.M) && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) Methods.Solution.SolutionActions.FlipHorizontalIndividual();
            // Flip Vertical
            else if (e.KeyCode == Keys.F) Methods.Solution.SolutionActions.FlipVertical();
            // Flip Horizontal
            else if (e.KeyCode == Keys.M) Methods.Solution.SolutionActions.FlipHorizontal();
        }
        #endregion

        #region Tile Maniac Keyboard Inputs
        public static void TileManiac_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N) CollisionEditor.Instance.newInstanceToolStripMenuItem_Click(null, null);
            else if (e.Control && e.KeyCode == Keys.O) CollisionEditor.Instance.OpenCollision();
            else if (e.Control && e.Alt && e.KeyCode == Keys.S) CollisionEditor.Instance.saveAsToolStripMenuItem_Click(null, null);
            else if (e.Control && e.KeyCode == Keys.S) CollisionEditor.Instance.saveToolStripMenuItem_Click(null, null);
            else if (e.Control && e.Alt && e.KeyCode == Keys.V) CollisionEditor.Instance.copyToOtherPathToolStripMenuItem_Click(null, null);
            else if (e.Control && e.KeyCode == Keys.C) CollisionEditor.Instance.copyToolStripMenuItem_Click(null, null);
            else if (e.Control && e.KeyCode == Keys.V) CollisionEditor.Instance.pasteToolStripMenuItem_Click(null, null);
            else if (e.Control && e.KeyCode == Keys.M)
            {
                CollisionEditor.Instance.mirrorPathsToolStripMenuItem1.IsChecked = !CollisionEditor.Instance.mirrorPathsToolStripMenuItem1.IsChecked;
                CollisionEditor.Instance.mirrorPathsToolStripMenuItem_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.B) CollisionEditor.Instance.showPathBToolStripMenuItem_Click(null, null);
            else if (e.Control && e.KeyCode == Keys.G)
            {
                CollisionEditor.Instance.showGridToolStripMenuItem.IsChecked = !CollisionEditor.Instance.showGridToolStripMenuItem.IsChecked;
                CollisionEditor.Instance.showGridToolStripMenuItem_Click(null, null);
            }
        }
        public static void TileManiac_OnKeyUp(object sender, KeyEventArgs e)
        {

        }
        #endregion
    }
}
