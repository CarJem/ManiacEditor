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

        #region Mouse Controls
        
        #region Mouse Auto-Scrolling Methods
        private static bool ForceAutoScrollMousePosition { get; set; } = false;
        public static void SetAutoScrollerApperance(AutoScrollDirection direction = AutoScrollDirection.NONE)
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
        public static void EnforceCursorPosition()
        {
            if (Properties.Settings.MySettings.ScrollerAutoCenters)
            {
                ForceAutoScrollMousePosition = true;
                System.Windows.Point pointFromParent = Instance.ViewPanel.SharpPanel.TranslatePoint(new System.Windows.Point(0, 0), Instance);
                Extensions.ExternalExtensions.SetCursorPos((int)(Instance.Left + pointFromParent.X) + (int)(Instance.ViewPanel.SharpPanel.ActualWidth / 2), (int)(Instance.Left + pointFromParent.Y) + (int)(Instance.ViewPanel.SharpPanel.ActualHeight / 2));
            }
        }
        public static void UpdateAutoScrollerPosition(System.Windows.Forms.MouseEventArgs e)
        {
            Methods.Editor.SolutionState.AutoScrollPosition = new Point(e.X - Methods.Editor.SolutionState.ViewPositionX, e.Y - Methods.Editor.SolutionState.ViewPositionY);
            ForceAutoScrollMousePosition = false;
        }
        public static void ToggleAutoScrollerMode(System.Windows.Forms.MouseEventArgs e)
        {
            if (!Methods.Editor.SolutionState.AutoScrolling) ScrollerModeOn();
            else ScrollerModeOff();

            void ScrollerModeOn()
            {
                //Turn Scroller Mode On
                Methods.Editor.SolutionState.AutoScrolling = true;
                Methods.Editor.SolutionState.AutoScrollingDragged = false;
                Methods.Editor.SolutionState.AutoScrollPosition = new Point(e.X - Methods.Editor.SolutionState.ViewPositionX, e.Y - Methods.Editor.SolutionState.ViewPositionY);
                if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible && Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) SetAutoScrollerApperance(AutoScrollDirection.ALL);
                else if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible) SetAutoScrollerApperance(AutoScrollDirection.WE);
                else if (Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) SetAutoScrollerApperance(AutoScrollDirection.NS);
                else Methods.Editor.SolutionState.AutoScrolling = false;
            }
            void ScrollerModeOff()
            {
                //Turn Scroller Mode Off
                Methods.Editor.SolutionState.AutoScrolling = false;
                Methods.Editor.SolutionState.AutoScrollingDragged = false;
                SetAutoScrollerApperance(AutoScrollDirection.NONE);
            }
        }
        #endregion

        #region Scroller Mode Events
        public static void ScrollerMouseMove(MouseEventArgs e)
        {
            if (ForceAutoScrollMousePosition) UpdateAutoScrollerPosition(e);
            if (Methods.Editor.SolutionState.AutoScrolling) Methods.Editor.SolutionState.AutoScrollingDragged = true;

            double xMove = (Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) ? e.X - Methods.Editor.SolutionState.ViewPositionX - Methods.Editor.SolutionState.AutoScrollPosition.X : 0;
            double yMove = (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible) ? e.Y - Methods.Editor.SolutionState.ViewPositionY - Methods.Editor.SolutionState.AutoScrollPosition.Y : 0;

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

            System.Windows.Point position = new System.Windows.Point(Methods.Editor.SolutionState.ViewPositionX, Methods.Editor.SolutionState.ViewPositionY);
            double x = xMove / 10 + position.X;
            double y = yMove / 10 + position.Y;

            if (!Methods.Editor.SolutionState.UnlockCamera)
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
        public static void ScrollerMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle) if (Properties.Settings.MySettings.ScrollerPressReleaseMode) ToggleAutoScrollerMode(e);
        }
        public static void ScrollerMouseDown(MouseEventArgs e)
        {

        }

        #endregion


        #region General Mouse Methods
        public static void SetClickedXY(System.Windows.Forms.MouseEventArgs e) { Methods.Editor.SolutionState.RegionX1 = e.X; Methods.Editor.SolutionState.RegionY1 = e.Y; }
        public static void SetClickedXY(Point e) { Methods.Editor.SolutionState.RegionX1 = e.X; Methods.Editor.SolutionState.RegionY1 = e.Y; }
        #endregion


        #region Mouse Move Events
        public static void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Editor.SolutionState.AutoScrolling) ScrollerMouseMove(e);
            Instance.EditorStatusBar.UpdatePositionLabel(e);
            if (Methods.Runtime.GameHandler.GameRunning) InteractiveMouseMove(e);

            if (Methods.Editor.SolutionState.RegionX1 != -1)
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() && !Instance.EditorToolbar.InteractionToolButton.IsChecked.Value && !ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit()) TilesEditMouseMoveDraggingStarted(e);
                else if (ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit()) ChunksEditMouseMoveDraggingStarted(e);
                else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit()) EntitiesEditMouseMoveDraggingStarted(e);

                Methods.Editor.SolutionState.RegionX1 = -1;
                Methods.Editor.SolutionState.RegionY1 = -1;
            }

            else if (e.Button == MouseButtons.Middle) EnforceCursorPosition();

            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() && !ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit()) TilesEditMouseMove(e);
            else if (ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit()) ChunksEditMouseMove(e);
            else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit()) EntitiesEditMouseMove(e);

            MouseMovementControls(e);

            Methods.Editor.SolutionState.LastX = e.X;
            Methods.Editor.SolutionState.LastY = e.Y;
        }
        public static void MouseMovementControls(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Editor.SolutionState.DraggingSelection || Methods.Editor.SolutionState.Dragged) EdgeMove();
            if (Methods.Editor.SolutionState.DraggingSelection) SetSelectionBounds();
            else if (Methods.Editor.SolutionState.Dragged) DragMoveItems();


            void EdgeMove()
            {
                System.Windows.Point position = new System.Windows.Point(Methods.Editor.SolutionState.ViewPositionX, Methods.Editor.SolutionState.ViewPositionY); ;
                double ScreenMaxX = position.X + (int)Instance.ViewPanel.SharpPanel.RenderingWidth - (int)Instance.ViewPanel.SharpPanel.vScrollBar1.ActualWidth;
                double ScreenMaxY = position.Y + (int)Instance.ViewPanel.SharpPanel.RenderingHeight - (int)Instance.ViewPanel.SharpPanel.hScrollBar1.ActualHeight;
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

                if (!Methods.Editor.SolutionState.UnlockCamera)
                {
                    if (x < 0) x = 0;
                    if (y < 0) y = 0;
                    if (x > Instance.ViewPanel.SharpPanel.hScrollBar1.Maximum) x = Instance.ViewPanel.SharpPanel.hScrollBar1.Maximum;
                    if (y > Instance.ViewPanel.SharpPanel.vScrollBar1.Maximum) y = Instance.ViewPanel.SharpPanel.vScrollBar1.Maximum;
                }


                if (x != position.X || y != position.Y)
                {
                    if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible)
                    {
                        Instance.ViewPanel.SharpPanel.vScrollBar1.Value = y;
                    }
                    if (Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible)
                    {
                        Instance.ViewPanel.SharpPanel.hScrollBar1.Value = x;
                    }
                    Instance.ViewPanel.SharpPanel.GraphicPanel.OnMouseMoveEventCreate();
                    if (Methods.Editor.SolutionState.AnyDragged) Instance.ViewPanel.SharpPanel.GraphicPanel.Render();



                }
            }
            void SetSelectionBounds()
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit()) ChunkMode();
                else Normal();

                void ChunkMode()
                {
                    if (Methods.Editor.SolutionState.RegionX2 != e.X && Methods.Editor.SolutionState.RegionY2 != e.Y)
                    {

                        Methods.Editor.SolutionState.TempSelectX1 = (int)(Methods.Editor.SolutionState.RegionX2 / Methods.Editor.SolutionState.Zoom);
                        Methods.Editor.SolutionState.TempSelectX2 = (int)(e.X / Methods.Editor.SolutionState.Zoom);
                        Methods.Editor.SolutionState.TempSelectY1 = (int)(Methods.Editor.SolutionState.RegionY2 / Methods.Editor.SolutionState.Zoom);
                        Methods.Editor.SolutionState.TempSelectY2 = (int)(e.Y / Methods.Editor.SolutionState.Zoom);
                        if (Methods.Editor.SolutionState.TempSelectX1 > Methods.Editor.SolutionState.TempSelectX2)
                        {
                            Methods.Editor.SolutionState.TempSelectX1 = (int)(e.X / Methods.Editor.SolutionState.Zoom);
                            Methods.Editor.SolutionState.TempSelectX2 = (int)(Methods.Editor.SolutionState.RegionX2 / Methods.Editor.SolutionState.Zoom);
                        }
                        if (Methods.Editor.SolutionState.TempSelectY1 > Methods.Editor.SolutionState.TempSelectY2)
                        {
                            Methods.Editor.SolutionState.TempSelectY1 = (int)(e.Y / Methods.Editor.SolutionState.Zoom);
                            Methods.Editor.SolutionState.TempSelectY2 = (int)(Methods.Editor.SolutionState.RegionY2 / Methods.Editor.SolutionState.Zoom);
                        }

                        Point selectStart = Classes.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(Methods.Editor.SolutionState.TempSelectX1, Methods.Editor.SolutionState.TempSelectY1);
                        Point selectEnd = Classes.Scene.Sets.EditorLayer.GetChunkCoordinatesBottomEdge(Methods.Editor.SolutionState.TempSelectX2, Methods.Editor.SolutionState.TempSelectY2);

                        Methods.Editor.Solution.EditLayerA?.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());
                        Methods.Editor.Solution.EditLayerB?.TempSelection(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), CtrlPressed());

                        Methods.Internal.UserInterface.UpdateTilesOptions();
                    }
                }
                void Normal()
                {
                    if (Methods.Editor.SolutionState.RegionX2 != e.X && Methods.Editor.SolutionState.RegionY2 != e.Y)
                    {
                        Methods.Editor.SolutionState.TempSelectX1 = (int)(Methods.Editor.SolutionState.RegionX2 / Methods.Editor.SolutionState.Zoom);
                        Methods.Editor.SolutionState.TempSelectX2 = (int)(e.X / Methods.Editor.SolutionState.Zoom);
                        Methods.Editor.SolutionState.TempSelectY1 = (int)(Methods.Editor.SolutionState.RegionY2 / Methods.Editor.SolutionState.Zoom);
                        Methods.Editor.SolutionState.TempSelectY2 = (int)(e.Y / Methods.Editor.SolutionState.Zoom);
                        if (Methods.Editor.SolutionState.TempSelectX1 > Methods.Editor.SolutionState.TempSelectX2)
                        {
                            Methods.Editor.SolutionState.TempSelectX1 = (int)(e.X / Methods.Editor.SolutionState.Zoom);
                            Methods.Editor.SolutionState.TempSelectX2 = (int)(Methods.Editor.SolutionState.RegionX2 / Methods.Editor.SolutionState.Zoom);
                        }
                        if (Methods.Editor.SolutionState.TempSelectY1 > Methods.Editor.SolutionState.TempSelectY2)
                        {
                            Methods.Editor.SolutionState.TempSelectY1 = (int)(e.Y / Methods.Editor.SolutionState.Zoom);
                            Methods.Editor.SolutionState.TempSelectY2 = (int)(Methods.Editor.SolutionState.RegionY2 / Methods.Editor.SolutionState.Zoom);
                        }
                        Methods.Editor.Solution.EditLayerA?.TempSelection(new Rectangle(Methods.Editor.SolutionState.TempSelectX1, Methods.Editor.SolutionState.TempSelectY1, Methods.Editor.SolutionState.TempSelectX2 - Methods.Editor.SolutionState.TempSelectX1, Methods.Editor.SolutionState.TempSelectY2 - Methods.Editor.SolutionState.TempSelectY1), CtrlPressed());
                        Methods.Editor.Solution.EditLayerB?.TempSelection(new Rectangle(Methods.Editor.SolutionState.TempSelectX1, Methods.Editor.SolutionState.TempSelectY1, Methods.Editor.SolutionState.TempSelectX2 - Methods.Editor.SolutionState.TempSelectX1, Methods.Editor.SolutionState.TempSelectY2 - Methods.Editor.SolutionState.TempSelectY1), CtrlPressed());

                        Methods.Internal.UserInterface.UpdateTilesOptions();

                        if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit()) Methods.Editor.Solution.Entities.TempSelection(new Rectangle(Methods.Editor.SolutionState.TempSelectX1, Methods.Editor.SolutionState.TempSelectY1, Methods.Editor.SolutionState.TempSelectX2 - Methods.Editor.SolutionState.TempSelectX1, Methods.Editor.SolutionState.TempSelectY2 - Methods.Editor.SolutionState.TempSelectY1), CtrlPressed());
                    }
                }
            }
            void DragMoveItems()
            {
                int oldGridX = (int)((Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom) / Methods.Editor.SolutionState.MagnetSize) * Methods.Editor.SolutionState.MagnetSize;
                int oldGridY = (int)((Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom) / Methods.Editor.SolutionState.MagnetSize) * Methods.Editor.SolutionState.MagnetSize;
                int newGridX = (int)((e.X / Methods.Editor.SolutionState.Zoom) / Methods.Editor.SolutionState.MagnetSize) * Methods.Editor.SolutionState.MagnetSize;
                int newGridY = (int)((e.Y / Methods.Editor.SolutionState.Zoom) / Methods.Editor.SolutionState.MagnetSize) * Methods.Editor.SolutionState.MagnetSize;
                Point oldPointGrid = new Point(0, 0);
                Point newPointGrid = new Point(0, 0);
                if (Methods.Editor.SolutionState.UseMagnetMode && ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
                {
                    if (Methods.Editor.SolutionState.UseMagnetXAxis == true && Methods.Editor.SolutionState.UseMagnetYAxis == true)
                    {
                        oldPointGrid = new Point(oldGridX, oldGridY);
                        newPointGrid = new Point(newGridX, newGridY);
                    }
                    if (Methods.Editor.SolutionState.UseMagnetXAxis && !Methods.Editor.SolutionState.UseMagnetYAxis)
                    {
                        oldPointGrid = new Point(oldGridX, (int)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom));
                        newPointGrid = new Point(newGridX, (int)(e.Y / Methods.Editor.SolutionState.Zoom));
                    }
                    if (!Methods.Editor.SolutionState.UseMagnetXAxis && Methods.Editor.SolutionState.UseMagnetYAxis)
                    {
                        oldPointGrid = new Point((int)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom), oldGridY);
                        newPointGrid = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), newGridY);
                    }
                    if (!Methods.Editor.SolutionState.UseMagnetXAxis && !Methods.Editor.SolutionState.UseMagnetYAxis)
                    {
                        oldPointGrid = new Point((int)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom), (int)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom));
                        newPointGrid = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
                    }
                }
                Point oldPoint = new Point((int)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom), (int)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom));
                Point newPoint = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));


                if (!ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit())
                {
                    Methods.Editor.Solution.EditLayerA?.MoveSelected(oldPoint, newPoint, CtrlPressed());
                    Methods.Editor.Solution.EditLayerB?.MoveSelected(oldPoint, newPoint, CtrlPressed());
                }
                else
                {
                    Point oldPointAligned = Classes.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(oldPoint.X, oldPoint.Y);
                    Point newPointAligned = Classes.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(newPoint.X, newPoint.Y);
                    Methods.Editor.Solution.EditLayerA?.MoveSelected(oldPointAligned, newPointAligned, CtrlPressed(), true);
                    Methods.Editor.Solution.EditLayerB?.MoveSelected(oldPointAligned, newPointAligned, CtrlPressed(), true);
                }

                Instance.ViewPanel.SharpPanel.GraphicPanel.Render();
                // FIX: Determine if this is Needed.
                //Editor.Instance.UI.UpdateEditLayerActions();
                if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
                {
                    if (Methods.Editor.SolutionState.UseMagnetMode)
                    {
                        int x = Methods.Editor.Solution.Entities.GetSelectedEntity().Entity.Position.X.High;
                        int y = Methods.Editor.Solution.Entities.GetSelectedEntity().Entity.Position.Y.High;

                        if (x % Methods.Editor.SolutionState.MagnetSize != 0 && Methods.Editor.SolutionState.UseMagnetXAxis)
                        {
                            int offsetX = x % Methods.Editor.SolutionState.MagnetSize;
                            oldPointGrid.X -= offsetX;
                        }
                        if (y % Methods.Editor.SolutionState.MagnetSize != 0 && Methods.Editor.SolutionState.UseMagnetYAxis)
                        {
                            int offsetY = y % Methods.Editor.SolutionState.MagnetSize;
                            oldPointGrid.Y -= offsetY;
                        }
                    }


                    try
                    {

                        if (Methods.Editor.SolutionState.UseMagnetMode)
                        {
                            Methods.Editor.Solution.Entities.MoveSelected(oldPointGrid, newPointGrid, CtrlPressed() && Methods.Editor.SolutionState.StartDragged);
                        }
                        else
                        {
                            Methods.Editor.Solution.Entities.MoveSelected(oldPoint, newPoint, CtrlPressed() && Methods.Editor.SolutionState.StartDragged);
                        }

                    }
                    catch (Classes.Scene.EditorEntities.TooManyEntitiesException)
                    {
                        System.Windows.MessageBox.Show("Too many entities! (limit: 2048)");
                        Methods.Editor.SolutionState.Dragged = false;
                        return;
                    }
                    if (Methods.Editor.SolutionState.UseMagnetMode)
                    {
                        Methods.Editor.SolutionState.DraggedX += newPointGrid.X - oldPointGrid.X;
                        Methods.Editor.SolutionState.DraggedY += newPointGrid.Y - oldPointGrid.Y;
                    }
                    else
                    {
                        Methods.Editor.SolutionState.DraggedX += newPoint.X - oldPoint.X;
                        Methods.Editor.SolutionState.DraggedY += newPoint.Y - oldPoint.Y;
                    }
                    if (CtrlPressed() && Methods.Editor.SolutionState.StartDragged)
                    {
                        Methods.Internal.UserInterface.UpdateEntitiesToolbarList();
                        Methods.Internal.UserInterface.SetSelectOnlyButtonsState();
                    }
                    Instance.EntitiesToolbar.UpdateSelectedProperties();
                }
                Methods.Editor.SolutionState.StartDragged = false;
            }
        }
        #endregion

        #region Mouse Move Methods
        public static void TilesEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
            {
                TilesEditDrawTool(e, false);
            }
            Methods.Internal.UserInterface.UpdateTilesOptions();
        }
        public static void TilesEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(Methods.Editor.SolutionState.RegionX1 / Methods.Editor.SolutionState.Zoom), (int)(Methods.Editor.SolutionState.RegionY1 / Methods.Editor.SolutionState.Zoom));
            bool PointASelected = Methods.Editor.Solution.EditLayerA?.IsPointSelected(clicked_point) ?? false;
            bool PointBSelected = Methods.Editor.Solution.EditLayerB?.IsPointSelected(clicked_point) ?? false;
            if (PointASelected || PointBSelected)
            {
                // Start dragging the tiles
                Methods.Editor.SolutionState.Dragged = true;
                Methods.Editor.SolutionState.StartDragged = true;
                Methods.Editor.Solution.EditLayerA?.StartDrag();
                Methods.Editor.Solution.EditLayerB?.StartDrag();
            }

            else if (!Instance.EditorToolbar.SelectToolButton.IsChecked.Value && !ShiftPressed() && !CtrlPressed() && (Methods.Editor.Solution.EditLayerA?.HasTileAt(clicked_point) ?? false) || (Methods.Editor.Solution.EditLayerB?.HasTileAt(clicked_point) ?? false))
            {
                // Start dragging the single selected tile
                Methods.Editor.Solution.EditLayerA?.Select(clicked_point);
                Methods.Editor.Solution.EditLayerB?.Select(clicked_point);
                Methods.Editor.SolutionState.Dragged = true;
                Methods.Editor.SolutionState.StartDragged = true;
                Methods.Editor.Solution.EditLayerA?.StartDrag();
                Methods.Editor.Solution.EditLayerB?.StartDrag();
            }

            else
            {
                // Start drag selection
                //EditLayer.Select(clicked_point, ShiftPressed || CtrlPressed, CtrlPressed);
                if (!ShiftPressed() && !CtrlPressed())
                    ManiacEditor.Methods.Editor.EditorActions.Deselect();
                Methods.Internal.UserInterface.UpdateEditLayerActions();

                Methods.Editor.SolutionState.DraggingSelection = true;
                Methods.Editor.SolutionState.RegionX2 = Methods.Editor.SolutionState.RegionX1;
                Methods.Editor.SolutionState.RegionY2 = Methods.Editor.SolutionState.RegionY1;
            }
        }
        public static void EntitiesEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value) EntitiesEditDrawTool(e);
        }
        public static void EntitiesEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(Methods.Editor.SolutionState.RegionX1 / Methods.Editor.SolutionState.Zoom), (int)(Methods.Editor.SolutionState.RegionY1 / Methods.Editor.SolutionState.Zoom));
            if (Methods.Editor.Solution.Entities.GetEntityAt(clicked_point)?.Selected ?? false)
            {
                SetClickedXY(e);
                // Start dragging the entity
                Methods.Editor.SolutionState.Dragged = true;
                Methods.Editor.SolutionState.DraggedX = 0;
                Methods.Editor.SolutionState.DraggedY = 0;
                Methods.Editor.SolutionState.StartDragged = true;

            }
            else
            {
                // Start drag selection
                if (!ShiftPressed() && !CtrlPressed())
                    ManiacEditor.Methods.Editor.EditorActions.Deselect();
                Methods.Editor.SolutionState.DraggingSelection = true;
                Methods.Editor.SolutionState.RegionX2 = Methods.Editor.SolutionState.RegionX1;
                Methods.Editor.SolutionState.RegionY2 = Methods.Editor.SolutionState.RegionY1;

            }
        }
        public static void ChunksEditMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            Point p = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
            Point pC = Classes.Scene.Sets.EditorLayer.GetChunkCoordinates(p.X, p.Y);

            if (e.Button == MouseButtons.Left)
            {
                if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    int selectedIndex = Instance.TilesToolbar.ChunkList.SelectedIndex;
                    // Place Stamp
                    if (selectedIndex != -1)
                    {
                        if (!Instance.Chunks.DoesChunkMatch(pC, Instance.Chunks.StageStamps.StampList[selectedIndex], Methods.Editor.Solution.EditLayerA, Methods.Editor.Solution.EditLayerB))
                        {
                            Instance.Chunks.PasteStamp(pC, selectedIndex, Methods.Editor.Solution.EditLayerA, Methods.Editor.Solution.EditLayerB);
                        }

                    }
                }
            }

            else if (e.Button == MouseButtons.Right)
            {
                if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {

                    if (!Instance.Chunks.IsChunkEmpty(pC, Methods.Editor.Solution.EditLayerA, Methods.Editor.Solution.EditLayerB))
                    {
                        // Remove Stamp Sized Area
                        Instance.Chunks.PasteStamp(pC, 0, Methods.Editor.Solution.EditLayerA, Methods.Editor.Solution.EditLayerB, true);
                    }
                }

            }
        }
        public static void ChunksEditMouseMoveDraggingStarted(System.Windows.Forms.MouseEventArgs e)
        {
            // There was just a click now we can determine that this click is dragging
            Point clicked_point = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
            Point chunk_point = Classes.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);

            bool PointASelected = Methods.Editor.Solution.EditLayerA?.DoesChunkContainASelectedTile(chunk_point) ?? false;
            bool PointBSelected = Methods.Editor.Solution.EditLayerB?.DoesChunkContainASelectedTile(chunk_point) ?? false;
            if (PointASelected || PointBSelected)
            {
                // Start dragging the tiles
                Methods.Editor.SolutionState.Dragged = true;
                Methods.Editor.SolutionState.StartDragged = true;
                Methods.Editor.Solution.EditLayerA?.StartDrag();
                Methods.Editor.Solution.EditLayerB?.StartDrag();
            }
            else
            {
                // Start drag selection
                if (!ShiftPressed() && !CtrlPressed())
                    ManiacEditor.Methods.Editor.EditorActions.Deselect();
                Methods.Internal.UserInterface.UpdateEditLayerActions();

                Methods.Editor.SolutionState.DraggingSelection = true;
                Methods.Editor.SolutionState.RegionX2 = e.X;
                Methods.Editor.SolutionState.RegionY2 = e.Y;
            }
        }
        public static void InteractiveMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Runtime.GameHandler.PlayerSelected)
            {
                Task.Run(() =>
                {
                    Methods.Runtime.GameHandler.MovePlayer(new Point(e.X, e.Y), Methods.Editor.SolutionState.Zoom, Methods.Runtime.GameHandler.SelectedPlayer);
                });
            }

            if (Methods.Runtime.GameHandler.CheckpointSelected)
            {
                Task.Run(() =>
                {
                    Point clicked_point = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
                    Methods.Runtime.GameHandler.UpdateCheckpoint(clicked_point, true);
                });
            }
        }
        #endregion


        #region Mouse Up Events
        public static void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Methods.Editor.SolutionState.isTileDrawing = false;
            if (Methods.Editor.SolutionState.DraggingSelection) MouseUpDraggingSelection(e);
            else
            {
                if (Methods.Editor.SolutionState.RegionX1 != -1)
                {
                    // So it was just click
                    if (e.Button == MouseButtons.Left)
                    {
                        if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() && !ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit()) TilesEditMouseUp(e);
                        else if (ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit()) ChunksEditMouseUp(e);
                        else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit()) EntitiesEditMouseUp(e);
                    }
                    Methods.Internal.UserInterface.SetSelectOnlyButtonsState();
                    Methods.Editor.SolutionState.RegionX1 = -1;
                    Methods.Editor.SolutionState.RegionY1 = -1;
                }
                if (Methods.Editor.SolutionState.Dragged && (Methods.Editor.SolutionState.DraggedX != 0 || Methods.Editor.SolutionState.DraggedY != 0)) Actions.UndoRedoModel.UpdateUndoRedo();
                Methods.Editor.SolutionState.Dragged = false;
            }
            ScrollerMouseUp(e);

            Methods.Internal.UserInterface.UpdateEditLayerActions();
            Methods.Internal.UserInterface.UpdateControls();


        }
        public static void MouseUpDraggingSelection(System.Windows.Forms.MouseEventArgs e)
        {
            if (Methods.Editor.SolutionState.RegionX2 != e.X && Methods.Editor.SolutionState.RegionY2 != e.Y)
            {
                int x1 = (int)(Methods.Editor.SolutionState.RegionX2 / Methods.Editor.SolutionState.Zoom), x2 = (int)(e.X / Methods.Editor.SolutionState.Zoom);
                int y1 = (int)(Methods.Editor.SolutionState.RegionY2 / Methods.Editor.SolutionState.Zoom), y2 = (int)(e.Y / Methods.Editor.SolutionState.Zoom);
                if (x1 > x2)
                {
                    x1 = (int)(e.X / Methods.Editor.SolutionState.Zoom);
                    x2 = (int)(Methods.Editor.SolutionState.RegionX2 / Methods.Editor.SolutionState.Zoom);
                }
                if (y1 > y2)
                {
                    y1 = (int)(e.Y / Methods.Editor.SolutionState.Zoom);
                    y2 = (int)(Methods.Editor.SolutionState.RegionY2 / Methods.Editor.SolutionState.Zoom);
                }

                if (ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit())
                {
                    Point selectStart = Classes.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(Methods.Editor.SolutionState.TempSelectX1, Methods.Editor.SolutionState.TempSelectY1);
                    Point selectEnd = Classes.Scene.Sets.EditorLayer.GetChunkCoordinatesBottomEdge(Methods.Editor.SolutionState.TempSelectX2, Methods.Editor.SolutionState.TempSelectY2);

                    Methods.Editor.Solution.EditLayerA?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                    Methods.Editor.Solution.EditLayerB?.Select(new Rectangle(selectStart.X, selectStart.Y, selectEnd.X - selectStart.X, selectEnd.Y - selectStart.Y), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                else
                {
                    Methods.Editor.Solution.EditLayerA?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                    Methods.Editor.Solution.EditLayerB?.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());

                    if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit()) Methods.Editor.Solution.Entities.Select(new Rectangle(x1, y1, x2 - x1, y2 - y1), ShiftPressed() || CtrlPressed(), CtrlPressed());
                }
                Methods.Internal.UserInterface.SetSelectOnlyButtonsState();
                Methods.Internal.UserInterface.UpdateEditLayerActions();

            }
            Methods.Editor.SolutionState.DraggingSelection = false;
            Methods.Editor.Solution.EditLayerA?.EndTempSelection();
            Methods.Editor.Solution.EditLayerB?.EndTempSelection();

            if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit()) Methods.Editor.Solution.Entities.EndTempSelection();
        }
        #endregion

        #region Mouse Up Methods
        public static void TilesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
            Methods.Editor.Solution.EditLayerA?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Methods.Editor.Solution.EditLayerB?.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
        }
        public static void EntitiesEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
            if (e.Button == MouseButtons.Left)
            {
                Methods.Editor.Solution.Entities.Select(clicked_point, ShiftPressed() || CtrlPressed(), CtrlPressed());
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Methods.Editor.Solution.Entities.SelectedEntities.Count == 2 && Methods.Editor.SolutionState.RightClicktoSwapSlotID)
                {
                    Methods.Editor.Solution.Entities.SwapSlotIDsFromPair();
                }
            }
        }
        public static void ChunksEditMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point clicked_point = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
            Point chunk_point = Classes.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(clicked_point.X, clicked_point.Y);
            Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

            Methods.Editor.Solution.EditLayerA?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Methods.Editor.Solution.EditLayerB?.Select(clicked_chunk, ShiftPressed() || CtrlPressed(), CtrlPressed());
            Methods.Internal.UserInterface.UpdateEditLayerActions();
        }
        public static void InteractiveMouseUp(System.Windows.Forms.MouseEventArgs e)
        {

        }

        #endregion


        #region Mouse Down Events
        public static void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!Methods.Editor.SolutionState.AutoScrolling) Instance.ViewPanel.SharpPanel.GraphicPanel.Focus();

            if (e.Button == MouseButtons.Left) MouseDownLeft(e);
            else if (e.Button == MouseButtons.Right) MouseDownRight(e);
            else if (e.Button == MouseButtons.Middle) MouseDownMiddle(e);

            Instance.ViewPanel.SharpPanel.GraphicPanel.Render();
        }
        public static void MouseDownRight(System.Windows.Forms.MouseEventArgs e)
        {
            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() && !ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit()) TilesEditMouseDown(e);
            else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit()) EntitiesEditMouseDown(e);
        }
        public static void MouseDownLeft(System.Windows.Forms.MouseEventArgs e)
        {
            if (ManiacEditor.Methods.Editor.SolutionState.IsEditing() && !Methods.Editor.SolutionState.Dragged)
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() && !Instance.EditorToolbar.InteractionToolButton.IsChecked.Value && !ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit()) TilesEditMouseDown(e);
                if (ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit() && ManiacEditor.Methods.Editor.SolutionState.IsSceneLoaded()) ChunksEditMouseDown(e);
                else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit()) EntitiesEditMouseDown(e);
            }
            InteractiveMouseDown(e);
        }
        public static void MouseDownMiddle(System.Windows.Forms.MouseEventArgs e)
        {
            EnforceCursorPosition();
            ToggleAutoScrollerMode(e);
        }
        #endregion

        #region Mouse Down Methods
        public static void TilesEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
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
        public static void EntitiesEditMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    Point clicked_point = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
                    if (Methods.Editor.Solution.Entities.GetEntityAt(clicked_point)?.Selected ?? false)
                    {
                        // We will have to check if this dragging or clicking
                        SetClickedXY(e);
                    }
                    else if (!ShiftPressed() && !CtrlPressed() && Methods.Editor.Solution.Entities.GetEntityAt(clicked_point) != null)
                    {
                        Methods.Editor.Solution.Entities.Select(clicked_point);
                        Methods.Internal.UserInterface.SetSelectOnlyButtonsState();
                        // Start dragging the single selected entity
                        Methods.Editor.SolutionState.Dragged = true;
                        Methods.Editor.SolutionState.DraggedX = 0;
                        Methods.Editor.SolutionState.DraggedY = 0;
                        Methods.Editor.SolutionState.StartDragged = true;
                    }
                    else
                    {
                        SetClickedXY(e);
                    }
                }
                else if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value) EntitiesEditDrawTool(e, true);
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
                if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                {
                    Point p = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
                    Point pC = Classes.Scene.Sets.EditorLayer.GetChunkCoordinates(p.X, p.Y);

                    if (Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    {
                        int selectedIndex = Instance.TilesToolbar.ChunkList.SelectedIndex;
                        // Place Stamp
                        if (selectedIndex != -1)
                        {
                            if (!Instance.Chunks.DoesChunkMatch(pC, Instance.Chunks.StageStamps.StampList[selectedIndex], Methods.Editor.Solution.EditLayerA, Methods.Editor.Solution.EditLayerB))
                            {
                                Instance.Chunks.PasteStamp(pC, selectedIndex, Methods.Editor.Solution.EditLayerA, Methods.Editor.Solution.EditLayerB);
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
                    Point p = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
                    Point chunk_point = Classes.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(p.X, p.Y);
                    Rectangle clicked_chunk = new Rectangle(chunk_point.X, chunk_point.Y, 128, 128);

                    // Remove Stamp Sized Area
                    if (!Methods.Editor.Solution.EditLayerA.DoesChunkContainASelectedTile(p)) Methods.Editor.Solution.EditLayerA?.Select(clicked_chunk);
                    if (Methods.Editor.Solution.EditLayerB != null && !Methods.Editor.Solution.EditLayerB.DoesChunkContainASelectedTile(p)) Methods.Editor.Solution.EditLayerB?.Select(clicked_chunk);
                    ManiacEditor.Methods.Editor.EditorActions.DeleteSelected();
                }
            }
        }
        public static void SplineToolMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
                if (Methods.Editor.Solution.Entities.IsEntityAt(clicked_point) == true)
                {
                    ManiacEditor.Methods.Editor.EditorActions.Deselect();
                    Methods.Editor.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                }
                else
                {
                    Methods.Editor.Solution.Entities.SpawnInternalSplineObject(new Position((short)clicked_point.X, (short)clicked_point.Y));
                    ManiacEditor.Methods.Editor.EditorActions.UpdateLastEntityAction();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point clicked_point = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
                Classes.Scene.Sets.EditorEntity atPoint = Methods.Editor.Solution.Entities.GetEntityAt(clicked_point);
                if (atPoint != null && atPoint.Entity.Object.Name.Name == "Spline")
                {
                    ManiacEditor.Methods.Editor.EditorActions.Deselect();
                    Methods.Editor.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                    Methods.Editor.Solution.Entities.DeleteInternallySelected();
                    ManiacEditor.Methods.Editor.EditorActions.UpdateLastEntityAction();
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
            Methods.Editor.SolutionState.ZoomLevel += change;
            if (Methods.Editor.SolutionState.ZoomLevel > maxZoom) Methods.Editor.SolutionState.ZoomLevel = maxZoom;
            if (Methods.Editor.SolutionState.ZoomLevel < minZoom) Methods.Editor.SolutionState.ZoomLevel = minZoom;

            Instance.ViewPanel.SharpPanel.UpdateZoomLevel(Methods.Editor.SolutionState.ZoomLevel, new Point(e.X - Methods.Editor.SolutionState.ViewPositionX, e.Y - Methods.Editor.SolutionState.ViewPositionY));
        }
        private static void MouseWheelScrolling(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Instance.ViewPanel.SharpPanel.vScrollBar1.IsVisible || Instance.ViewPanel.SharpPanel.hScrollBar1.IsVisible) ScrollMove();

            void ScrollMove()
            {
                if (Methods.Editor.SolutionState.ScrollDirection == Axis.Y)
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
                else if (Methods.Editor.SolutionState.ScrollDirection == Axis.X)
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
            if (!Methods.Editor.SolutionState.UnlockCamera)
            {
                if (y < 0) y = 0;
                if (y > Instance.ViewPanel.SharpPanel.vScrollBar1.Maximum) y = Instance.ViewPanel.SharpPanel.vScrollBar1.Maximum;
            }
            Instance.ViewPanel.SharpPanel.vScrollBar1.Value = y;
        }
        private static void MouseWheelScrollingX(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            double x = Instance.ViewPanel.SharpPanel.hScrollBar1.Value - e.Delta;
            if (!Methods.Editor.SolutionState.UnlockCamera)
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
            if (e.Button == MouseButtons.Right)
            {
                if (Instance.EditorToolbar.InteractionToolButton.IsChecked.Value) InteractiveContextMenu(e);
                else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit() && !Instance.EditorToolbar.DrawToolButton.IsChecked.Value && !Instance.EditorToolbar.SplineToolButton.IsChecked.Value && (!Methods.Editor.SolutionState.RightClicktoSwapSlotID || Methods.Editor.Solution.Entities.SelectedEntities.Count <= 1)) EntitiesEditContextMenu(e);
                else if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() && !Instance.EditorToolbar.DrawToolButton.IsChecked.Value) TilesEditContextMenu(e);
            }

        }
        public static void TilesEditContextMenu(System.Windows.Forms.MouseEventArgs e)
        {
            string newLine = Environment.NewLine;
            Point chunkPos = Classes.Scene.Sets.EditorLayer.GetChunkCoordinates(e.X / Methods.Editor.SolutionState.Zoom, e.Y / Methods.Editor.SolutionState.Zoom);
            Point tilePos;
            if (e.X == 0 || e.Y == 0) tilePos = new Point(0, 0);
            else tilePos = new Point(e.X / 16, e.Y / 16);

            Instance.EditorStatusBar.PixelPositionMenuItem.Header = "Pixel Position:" + newLine + String.Format("X: {0}, Y: {1}", e.X, e.Y);
            Instance.EditorStatusBar.ChunkPositionMenuItem.Header = "Chunk Position:" + newLine + String.Format("X: {0}, Y: {1}", chunkPos.X, chunkPos.Y);
            Instance.EditorStatusBar.TilePositionMenuItem.Header = "Tile Position:" + newLine + String.Format("X: {0}, Y: {1}", tilePos.X, tilePos.Y);


            Point clicked_point_tile = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
            int tile;
            int tileA = (ushort)(Methods.Editor.Solution.EditLayerA?.GetTileAt(clicked_point_tile) & 0x3ff);
            int tileB = 0;
            if (Methods.Editor.Solution.EditLayerB != null)
            {
                tileB = (ushort)(Methods.Editor.Solution.EditLayerB?.GetTileAt(clicked_point_tile) & 0x3ff);
                if (tileA > 1023 && tileB < 1023) tile = tileB;
                else tile = tileA;
            }
            else tile = tileA;

            Methods.Editor.SolutionState.SelectedTileID = tile;
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
            Point clicked_point = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
            string newLine = Environment.NewLine;
            if (Methods.Editor.Solution.Entities.GetEntityAt(clicked_point) != null)
            {
                var currentEntity = Methods.Editor.Solution.Entities.GetEntityAt(clicked_point);

                Instance.EditorStatusBar.EntityNameItem.Header = String.Format("Entity Name: {0}", currentEntity.Name);
                Instance.EditorStatusBar.EntitySlotIDItem.Header = String.Format("Slot ID: {0} {1} Runtime Slot ID: {2}", currentEntity.SlotID, Environment.NewLine, Methods.Editor.Solution.Entities.GetRealSlotID(currentEntity.Entity));
                Instance.EditorStatusBar.EntityPositionItem.Header = String.Format("X: {0}, Y: {1}", currentEntity.Entity.Position.X.High, currentEntity.Entity.Position.Y.High);
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
            if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
            {
                Point clicked_point_tile = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
                int tile;
                int tileA = (ushort)(Methods.Editor.Solution.EditLayerA?.GetTileAt(clicked_point_tile) & 0x3ff);
                int tileB = 0;
                if (Methods.Editor.Solution.EditLayerB != null)
                {
                    tileB = (ushort)(Methods.Editor.Solution.EditLayerB?.GetTileAt(clicked_point_tile) & 0x3ff);
                    if (tileA > 1023 && tileB < 1023) tile = tileB;
                    else tile = tileA;
                }
                else tile = tileA;


                Methods.Editor.SolutionState.SelectedTileID = tile;
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
                Point clicked_point_tile = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
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
        public static void TilesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
        {
            Point p = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
            if (click)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Methods.Editor.SolutionState.isTileDrawing = true;
                    PlaceTile();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Methods.Editor.SolutionState.isTileDrawing = true;
                    RemoveTile();
                }
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    Methods.Editor.SolutionState.isTileDrawing = true;
                    PlaceTile();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Methods.Editor.SolutionState.isTileDrawing = true;
                    RemoveTile();
                }
            }

            void RemoveTile()
            {
                // Remove tile
                if (Methods.Editor.SolutionState.DrawBrushSize == 1)
                {
                    Methods.Editor.Solution.EditLayerA?.Select(p);
                    Methods.Editor.Solution.EditLayerB?.Select(p);
                    ManiacEditor.Methods.Editor.EditorActions.DeleteSelected();
                }
                else
                {
                    double size = (Methods.Editor.SolutionState.DrawBrushSize / 2) * Methods.Editor.EditorConstants.TILE_SIZE;
                    Methods.Editor.Solution.EditLayerA?.Select(new Rectangle((int)(p.X - size), (int)(p.Y - size), Methods.Editor.SolutionState.DrawBrushSize * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.SolutionState.DrawBrushSize * Methods.Editor.EditorConstants.TILE_SIZE));
                    Methods.Editor.Solution.EditLayerB?.Select(new Rectangle((int)(p.X - size), (int)(p.Y - size), Methods.Editor.SolutionState.DrawBrushSize * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.SolutionState.DrawBrushSize * Methods.Editor.EditorConstants.TILE_SIZE));
                    ManiacEditor.Methods.Editor.EditorActions.DeleteSelected();
                }
            }

            void PlaceTile()
            {
                if (Methods.Editor.SolutionState.DrawBrushSize == 1)
                {
                    if (Instance.TilesToolbar.SelectedTileIndex != -1)
                    {
                        if (Methods.Editor.Solution.EditLayerA.GetTileAt(p) != Instance.TilesToolbar.SelectedTileIndex)
                        {
                            ManiacEditor.Methods.Editor.EditorActions.EditorPlaceTile(p, Instance.TilesToolbar.SelectedTile, Methods.Editor.Solution.EditLayerA);
                        }
                        else if (!Methods.Editor.Solution.EditLayerA.IsPointSelected(p))
                        {
                            Methods.Editor.Solution.EditLayerA.Select(p);
                        }
                    }
                }
                else
                {
                    if (Instance.TilesToolbar.SelectedTileIndex != -1)
                    {
                        ManiacEditor.Methods.Editor.EditorActions.EditorPlaceTile(p, Instance.TilesToolbar.SelectedTile, Methods.Editor.Solution.EditLayerA, true);
                    }
                }
            }
        }
        public static void EntitiesEditDrawTool(System.Windows.Forms.MouseEventArgs e, bool click = false)
        {
            if (click)
            {
                Methods.Editor.SolutionState.LastX = e.X;
                Methods.Editor.SolutionState.LastY = e.Y;
            }
            if (e.Button == MouseButtons.Left)
            {
                Point clicked_point = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
                if (Methods.Editor.Solution.Entities.IsEntityAt(clicked_point, true) == true)
                {
                    ManiacEditor.Methods.Editor.EditorActions.Deselect();
                    Methods.Editor.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                }
                else
                {
                    Instance.EntitiesToolbar.SpawnObject();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point clicked_point = new Point((int)(e.X / Methods.Editor.SolutionState.Zoom), (int)(e.Y / Methods.Editor.SolutionState.Zoom));
                if (Methods.Editor.Solution.Entities.IsEntityAt(clicked_point, true) == true)
                {
                    ManiacEditor.Methods.Editor.EditorActions.Deselect();
                    Methods.Editor.Solution.Entities.GetEntityAt(clicked_point).Selected = true;
                    Methods.Editor.Solution.Entities.DeleteSelected();
                    ManiacEditor.Methods.Editor.EditorActions.UpdateLastEntityAction();
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
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() && Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Instance.TilesToolbar.SetSelectTileOption(0, false);
            }
            // Tiles Toolbar Flip Vertical
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipVTiles, true))
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() && Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Instance.TilesToolbar.SetSelectTileOption(1, false);
            }
        }
        public static void GraphicPanel_OnKeyDown(object sender, KeyEventArgs e)
        {
            bool parallaxAnimationInProgress = Methods.Editor.SolutionState.AllowAnimations && Methods.Editor.SolutionState.ParallaxAnimationChecked;
            if (parallaxAnimationInProgress) return;

            // Faster Nudge Toggle
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.NudgeFaster))
            {
                Methods.Editor.SolutionState.EnableFasterNudge ^= true;
            }
            // Scroll Lock Toggle
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ScrollLock))
            {
                Methods.Editor.SolutionState.ScrollLocked ^= true;
            }
            // Switch Scroll Lock Type
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ScrollLockTypeSwitch))
            {
                Methods.Editor.EditorActions.SetScrollLockDirection();

            }
            // Tiles Toolbar Flip Vertical
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipVTiles, true))
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() && Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Instance.TilesToolbar.SetSelectTileOption(1, true);
            }
            // Tiles Toolbar Flip Horizontal
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipHTiles, true))
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() && Instance.EditorToolbar.DrawToolButton.IsChecked.Value)
                    Instance.TilesToolbar.SetSelectTileOption(0, true);
            }
            // Open Click (Alt: Open Data Dir)
            else if ((Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.OpenDataDir)))
            {
                ManiacEditor.Methods.Editor.SolutionLoader.OpenSceneSelect();
            }
            else if ((Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Open)))
            {
                ManiacEditor.Methods.Editor.SolutionLoader.OpenScene();
            }
            // New Click
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.New))
            {
                //ManiacEditor.Methods.Editor.SolutionLoader.NewScene();
            }
            // Save Click (Alt: Save As)
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.SaveAs))
            {
                ManiacEditor.Methods.Editor.SolutionLoader.Save();
            }
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds._Save))
            {
                ManiacEditor.Methods.Editor.SolutionLoader.SaveAs();
            }
            // Undo
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Undo))
            {
                ManiacEditor.Methods.Editor.EditorActions.EditorUndo();
            }
            // Redo
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Redo))
            {
                ManiacEditor.Methods.Editor.EditorActions.EditorRedo();
            }
            // Developer Interface
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.DeveloperInterface))
            {
                ManiacEditor.Methods.Editor.EditorActions.EditorUndo();
            }
            // Save for Force Open on Startup
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ForceOpenOnStartup))
            {
                ManiacEditor.Methods.Editor.EditorActions.EditorRedo();
            }
            else if (ManiacEditor.Methods.Editor.SolutionState.IsSceneLoaded())
            {
                GraphicPanel_OnKeyDownLoaded(sender, e);
            }
            // Editing Key Shortcuts
            if (ManiacEditor.Methods.Editor.SolutionState.IsEditing())
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
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ShowPathA) && ManiacEditor.Methods.Editor.SolutionState.IsSceneLoaded())
            {
                Methods.Editor.SolutionState.ShowCollisionA ^= true;
            }
            //Show Path B
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ShowPathB))
            {
                Methods.Editor.SolutionState.ShowCollisionB ^= true;
            }
            //Unload Scene
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.UnloadScene))
            {
                Instance.MenuBar.UnloadSceneEvent(null, null);
            }
            //Toggle Grid Visibility
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ShowGrid))
            {
                Methods.Editor.SolutionState.ShowGrid ^= true;
            }
            //Toggle Tile ID Visibility
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.ShowTileID))
            {
                Methods.Editor.SolutionState.ShowTileID ^= true;
            }
            //Status Box Toggle
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.StatusBoxToggle))
            {
                Methods.Editor.SolutionState.DebugStatsVisibleOnPanel ^= true;
                Instance.ViewPanel.InfoHUD.UpdatePopupSize();
            }
        }
        public static void GraphicPanel_OnKeyDownEditing(object sender, KeyEventArgs e)
        {
            //Paste
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Paste))
            {
                Methods.Editor.EditorActions.Paste();
            }
            //Paste to Chunk
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.PasteToChunk))
            {
                Methods.Editor.EditorActions.PasteToChunks();
            }
            //Select All
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.SelectAll))
            {
                Methods.Editor.EditorActions.SelectAll();
            }
            // Selected Key Shortcuts   
            if (ManiacEditor.Methods.Editor.SolutionState.IsSelected())
            {
                GraphicPanel_OnKeyDownSelectedEditing(sender, e);
            }
        }
        public static void GraphicPanel_OnKeyDownSelectedEditing(object sender, KeyEventArgs e)
        {
            // Delete
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Delete))
            {
                ManiacEditor.Methods.Editor.EditorActions.DeleteSelected();
            }

            // Moving
            else if (e.KeyData == Keys.Up || e.KeyData == Keys.Down || e.KeyData == Keys.Left || e.KeyData == Keys.Right)
            {
                ManiacEditor.Methods.Editor.EditorActions.MoveEntityOrTiles(sender, e);
            }

            //Cut 
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Cut))
            {
                Methods.Editor.EditorActions.Cut();
            }
            //Copy
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Copy))
            {
                Methods.Editor.EditorActions.Copy();
            }
            //Duplicate
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Duplicate))
            {
                Methods.Editor.EditorActions.Duplicate();
            }
            //Delete
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.Delete))
            {
                Methods.Editor.EditorActions.Delete();
            }
            // Flip Vertical Individual
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipVIndv))
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
                    Methods.Editor.EditorActions.FlipVerticalIndividual();
            }
            // Flip Horizontal Individual
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipHIndv))
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
                    Methods.Editor.EditorActions.FlipHorizontalIndividual();
            }
            // Flip Vertical
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipV))
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
                    Methods.Editor.EditorActions.FlipVertical();
                else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
                    ManiacEditor.Methods.Editor.EditorActions.FlipEntities(FlipDirection.Veritcal);
            }

            // Flip Horizontal
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.FlipH))
            {
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
                    Methods.Editor.EditorActions.FlipHorizontal();
                else if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
                    ManiacEditor.Methods.Editor.EditorActions.FlipEntities(FlipDirection.Horizontal);
            }
        }
        public static void OnKeyDownTools(object sender, KeyEventArgs e)
        {
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.PointerTool) && Instance.EditorToolbar.PointerToolButton.IsEnabled) Methods.Editor.SolutionState.PointerMode(true);
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.SelectTool) && Instance.EditorToolbar.SelectToolButton.IsEnabled) Methods.Editor.SolutionState.SelectionMode(true);
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.DrawTool) && Instance.EditorToolbar.DrawToolButton.IsEnabled) Methods.Editor.SolutionState.DrawMode(true);
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.MagnetTool) && Instance.EditorToolbar.MagnetMode.IsEnabled) Methods.Editor.SolutionState.UseMagnetMode ^= true;
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.SplineTool) && Instance.EditorToolbar.SplineToolButton.IsEnabled) Methods.Editor.SolutionState.SplineMode(true);
            else if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.StampTool) && Instance.EditorToolbar.ChunksToolButton.IsEnabled) Methods.Editor.SolutionState.ChunksMode();
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
                CollisionEditor.Instance.OpenToolStripMenuItem_Click(null, null);
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
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacClassicMode))
            {
                CollisionEditor.Instance.classicViewModeToolStripMenuItem_Click(null, null);
            }
            if (Extensions.KeyEventExts.isCombo(e, Properties.Settings.MyKeyBinds.TileManiacWindowAlwaysOnTop))
            {
                CollisionEditor.Instance.windowAlwaysOnTop.IsChecked = !CollisionEditor.Instance.windowAlwaysOnTop.IsChecked;
                CollisionEditor.Instance.WindowAlwaysOnTop_Click(null, null);
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
