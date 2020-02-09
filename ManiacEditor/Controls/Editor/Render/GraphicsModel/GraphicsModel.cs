using IWshRuntimeLibrary;
using ManiacEditor.Actions;
using ManiacEditor.Controls;
using ManiacEditor.Controls.Editor.Controls;
using ManiacEditor.Entity_Renders;
using ManiacEditor.Enums;
using ManiacEditor.Event_Handlers;
using ManiacEditor.Extensions;
using Microsoft.Scripting.Utils;
using Microsoft.Win32;
using RSDKv5;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace ManiacEditor.Controls
{
    public partial class GraphicsModel : UserControl, IDrawArea
    {
        #region Definitions

        public Controls.Editor.MainEditor Instance;
        public ManiacEditor.DevicePanel GraphicPanel;

		public Global.Controls.HScrollBar hScrollBar;
		public Global.Controls.VScrollBar vScrollBar;

        #region DPI Definitions
        public double DPIScale { get; set; }
        public int RenderingWidth { get; set; }
        public int RenderingHeight { get; set; }
        #endregion

        #region ScrollBar Definitions
        public System.Windows.Controls.Primitives.ScrollBar vScrollBar1 { get => GetScrollBarV(); }
		public System.Windows.Controls.Primitives.ScrollBar hScrollBar1 { get => GetScrollBarH(); }
        private System.Windows.Controls.Primitives.ScrollBar GetScrollBarV() { return vScrollBar.scroller; }
        private System.Windows.Controls.Primitives.ScrollBar GetScrollBarH() { return hScrollBar.scroller; }
        #endregion

        #endregion

        #region Init
        public GraphicsModel(Controls.Editor.MainEditor instance)
        {
            Instance = instance;
            InitializeComponent();
            SetupScrollBars();
            SetupGraphicsPanel();
            SetupEvents();
		}
        #endregion

        #region Events Init
        private void SetupEvents()
        {
            (Instance as Window).SizeChanged += GraphicsModel_SizeChanged;
            SystemEvents.PowerModeChanged += CheckDeviceState;
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            this.KeyDown += this.GraphicsModel_KeyDown;
            this.KeyUp += this.GraphicsModel_KeyUp;
            this.Resize += Editor_Resize;

            UpdateDPIScale();
            UpdateRenderingScale();

            vScrollBar1.Scroll += this.VScrollBar1_Scroll;
            vScrollBar1.ValueChanged += this.VScrollBar1_ValueChanged;
            vScrollBar1.MouseEnter += this.VScrollBar1_Entered;
            hScrollBar1.Scroll += this.HScrollBar1_Scroll;
            hScrollBar1.ValueChanged += this.HScrollBar1_ValueChanged;
            hScrollBar1.MouseEnter += this.HScrollBar1_Entered;

            GraphicPanel.OnRender += this.GraphicPanel_OnRender;
            GraphicPanel.OnCreateDevice += this.OnResetDevice;
            GraphicPanel.DragDrop += this.GraphicPanel_DragDrop;
            GraphicPanel.DragEnter += this.GraphicPanel_DragEnter;
            GraphicPanel.DragOver += this.GraphicPanel_DragOver;
            GraphicPanel.DragLeave += this.GraphicPanel_DragLeave;
            GraphicPanel.KeyDown += this.GraphicPanel_OnKeyDown;
            GraphicPanel.KeyUp += this.GraphicPanel_OnKeyUp;
            GraphicPanel.MouseClick += this.GraphicPanel_MouseClick;
            GraphicPanel.MouseDown += this.GraphicPanel_OnMouseDown;
            GraphicPanel.MouseMove += this.GraphicPanel_OnMouseMove;
            GraphicPanel.MouseUp += this.GraphicPanel_OnMouseUp;
            GraphicPanel.MouseWheel += this.GraphicPanel_MouseWheel;

            this.GraphicPanel.Width = SystemInformation.PrimaryMonitorSize.Width;
            this.GraphicPanel.Height = SystemInformation.PrimaryMonitorSize.Height;
        }
        public void SetupGraphicsPanel()
        {
            this.GraphicPanel = new ManiacEditor.DevicePanel(Instance);
            this.GraphicPanel.AllowDrop = true;
            this.GraphicPanel.AutoSize = true;
            this.GraphicPanel.DeviceBackColor = System.Drawing.Color.White;
            this.GraphicPanel.Location = new System.Drawing.Point(-1, 0);
            this.GraphicPanel.Margin = new System.Windows.Forms.Padding(0);
            this.GraphicPanel.Name = "GraphicPanel";
            this.GraphicPanel.Size = new System.Drawing.Size(643, 449);
            this.GraphicPanel.TabIndex = 10;
            this.viewPanel.Controls.Add(this.GraphicPanel);
        }
        public void SetupScrollBars(bool refreshing = false)
        {
            hScrollBar = new Global.Controls.HScrollBar();
            vScrollBar = new Global.Controls.VScrollBar();
            hScrollBar1Host.Child = hScrollBar;
            vScrollBar1Host.Child = vScrollBar;
            if (refreshing) SetupScrollBarEvents();
        }
        public void SetupScrollBarEvents()
        {
            this.vScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.VScrollBar1_Scroll);
            this.vScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.VScrollBar1_ValueChanged);
            this.vScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.VScrollBar1_Entered);
            this.hScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.HScrollBar1_Scroll);
            this.hScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.HScrollBar1_ValueChanged);
            this.hScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.HScrollBar1_Entered);
        }

        #endregion

        #region IDrawArea Methods

        public double GetZoom()
        {
            return Classes.Editor.SolutionState.Zoom;
        }
        public new void Dispose()
        {
            this.GraphicPanel.Dispose();
            this.GraphicPanel = null;
            hScrollBar = null;
            vScrollBar = null;
            this.Controls.Clear();
            base.Dispose(true);
        }
        public Rectangle GetScreen()
        {
            if (Core.Settings.MySettings.EntityFreeCam) return new Rectangle(Classes.Editor.SolutionState.CustomViewPositionX, Classes.Editor.SolutionState.CustomViewPositionY, RenderingWidth, RenderingHeight);
            else return new Rectangle((int)Classes.Editor.SolutionState.ViewPositionX, (int)Classes.Editor.SolutionState.ViewPositionY, RenderingWidth, RenderingHeight);
        }
        public void DisposeTextures()
        {
            // Make sure to dispose the textures of the extra layers too
            Classes.Editor.Solution.CurrentTiles?.Dispose();
            if (Classes.Editor.Solution.FGHigh != null) Classes.Editor.Solution.FGHigh?.DisposeTextures();
            if (Classes.Editor.Solution.FGLow != null) Classes.Editor.Solution.FGLow?.DisposeTextures();
            if (Classes.Editor.Solution.FGHigher != null) Classes.Editor.Solution.FGHigher?.DisposeTextures();
            if (Classes.Editor.Solution.FGLower != null) Classes.Editor.Solution.FGLower?.DisposeTextures();

            if (Classes.Editor.Solution.CurrentScene != null)
            {
                foreach (var el in Classes.Editor.Solution.CurrentScene?.OtherLayers)
                {
                    el.DisposeTextures();
                }
            }

        }

        #endregion

        #region DPI Events

        private void GraphicsModel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateScalingforDPI();
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            UpdateScalingforDPI();
        }

        private void Editor_Resize(object sender, EventArgs e)
        {
            UpdateScalingforDPI();
        }

        #endregion

        #region DPI Scaling Methods

        private void UpdateDPIScale()
        {
            DPIScale = (double)(100 * Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth) / 100;
        }

        private void UpdateRenderingScale()
        {
            if (Instance != null)
            {
                RenderingWidth = (int)(Instance.ViewPanel.SharpPanel.ActualWidth * DPIScale);
                RenderingHeight = (int)(Instance.ViewPanel.SharpPanel.ActualHeight * DPIScale);
            }
        }

        public void UpdateScalingforDPI()
        {
            UpdateDPIScale();
            UpdateRenderingScale();
        }

        #endregion

        #region Other Events
        private void EditorView_Load(object sender, EventArgs e)
        {

        }
        #endregion

        #region GraphicPanel Event Handlers
        private void GraphicPanel_OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseMove(sender, e); }
        private void GraphicPanel_OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseDown(sender, e); }
        private void GraphicPanel_OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseUp(sender, e); }
        private void GraphicPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseWheel(sender, e); }
        private void GraphicPanel_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseClick(sender, e); }
        #endregion

        #region ScrollBar Events
        public void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            UpdateScrollPosY((int)e.NewValue);
        }
        public void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            UpdateScrollPosX((int)e.NewValue);
        }
        public void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            UpdateScrollPosY();
            if (Classes.Editor.SolutionState.AnyDragged)
            {
                Instance.DeviceModel.GraphicPanel.OnMouseMoveEventCreate();
            }

        }
        public void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            UpdateScrollPosX();
            if (Classes.Editor.SolutionState.AnyDragged)
            {
                Instance.DeviceModel.GraphicPanel.OnMouseMoveEventCreate();
            }

        }
        public void HScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Classes.Editor.SolutionState.ScrollLocked) Classes.Editor.SolutionState.ScrollDirection = Axis.X;
        }
        public void VScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Classes.Editor.SolutionState.ScrollLocked) Classes.Editor.SolutionState.ScrollDirection = Axis.Y;
        }
        #endregion

        #region Graphics Panel Events
        public void OnResetDevice(object sender, DeviceEventArgs e)
        {
            Device device = e.Device;
        }
        public void CheckDeviceState(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    SetDeviceSleepState(false);
                    break;
                case PowerModes.Resume:
                    SetDeviceSleepState(true);
                    break;
            }
        }
        private void SetDeviceSleepState(bool state)
        {
            GraphicPanel.bRender = state;
            if (state == true)
            {
                Methods.Internal.UserInterface.ReloadSpritesAndTextures();
            }
        }
        private void GraphicPanel_OnRender(object sender, DeviceEventArgs e)
        {
            bool showEntities = Instance.EditorToolbar.ShowEntities.IsChecked.Value && !Instance.EditorToolbar.EditEntities.IsCheckedAll;
            bool showEntitiesEditing = Instance.EditorToolbar.EditEntities.IsCheckedAll;

            bool PriorityMode = Classes.Editor.SolutionState.PrioritizedEntityViewing;
            bool AboveAllMode = Classes.Editor.SolutionState.EntitiesVisibileAboveAllLayers;


            if (Instance.EntitiesToolbar?.NeedRefresh ?? false) Instance.EntitiesToolbar.PropertiesRefresh();
            if (Classes.Editor.Solution.CurrentScene != null)
            {
                DrawBackground();

                //if (UIModes.DebugStatsVisibleOnPanel && Classes.Edit.Scene.EditorSolution.CurrentScene != null) DrawDebugHUD();

                if (Classes.Editor.Solution.CurrentScene.OtherLayers.Contains(Classes.Editor.Solution.EditLayerA)) Classes.Editor.Solution.EditLayerA.Draw(GraphicPanel);

                if (!Classes.Editor.SolutionState.ExtraLayersMoveToFront) DrawExtraLayers();

                DrawLayer(Instance.EditorToolbar.ShowFGLower.IsChecked.Value, Instance.EditorToolbar.EditFGLower.IsCheckedAll, Classes.Editor.Solution.FGLower);

                DrawLayer(Instance.EditorToolbar.ShowFGLow.IsChecked.Value, Instance.EditorToolbar.EditFGLow.IsCheckedAll, Classes.Editor.Solution.FGLow);


                if (showEntities && !AboveAllMode)
                    if (PriorityMode) EntitiesDraw(2);
                    else EntitiesDraw(0);

                DrawLayer(Instance.EditorToolbar.ShowFGHigh.IsChecked.Value, Instance.EditorToolbar.EditFGHigh.IsCheckedAll, Classes.Editor.Solution.FGHigh);

                if (showEntities && PriorityMode && !AboveAllMode) EntitiesDraw(3);

                DrawLayer(Instance.EditorToolbar.ShowFGHigher.IsChecked.Value, Instance.EditorToolbar.EditFGHigher.IsCheckedAll, Classes.Editor.Solution.FGHigher);

                if (Classes.Editor.SolutionState.ExtraLayersMoveToFront) DrawExtraLayers();

                if (showEntitiesEditing || AboveAllMode)
                    if (PriorityMode) EntitiesDraw(1);
                    else EntitiesDraw(0);

                if (Classes.Editor.Solution.CurrentScene != null) Classes.Editor.Solution.Entities.DrawInternalObjects(GraphicPanel);

                if (Classes.Editor.SolutionState.EntitySelectionBoxesAlwaysPrioritized && (showEntities || showEntitiesEditing)) Classes.Editor.Solution.Entities.DrawSelectionBoxes(GraphicPanel);

            }

            if (Classes.Editor.SolutionState.DraggingSelection) DrawSelectionBox();
            else DrawSelectionBox(true);

            if (Classes.Editor.SolutionState.isTileDrawing && Classes.Editor.SolutionState.DrawBrushSize != 1) DrawBrushBox();

            if (Classes.Editor.SolutionState.ShowGrid && Classes.Editor.Solution.CurrentScene != null) Instance.BackgroundDX.DrawGrid(GraphicPanel);


            if (Methods.GameHandler.GameRunning) DrawGameElements();

            if (Classes.Editor.SolutionState.AutoScrolling) DrawScroller();

            void DrawBackground()
            {
                if (!ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit()) if (ManiacEditor.Core.Settings.MyPerformance.HideNormalBackground == false) Instance.BackgroundDX.Draw(GraphicPanel);
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit()) if (ManiacEditor.Core.Settings.MyPerformance.ShowEditLayerBackground == true) Instance.BackgroundDX.DrawEdit(GraphicPanel);
            }

            void DrawScroller()
            {
                if (vScrollBar1.IsVisible && hScrollBar1.IsVisible) GraphicPanel.Draw2DCursor(Classes.Editor.SolutionState.AutoScrollPosition.X, Classes.Editor.SolutionState.AutoScrollPosition.Y);
                else if (vScrollBar1.IsVisible) GraphicPanel.DrawVertCursor(Classes.Editor.SolutionState.AutoScrollPosition.X, Classes.Editor.SolutionState.AutoScrollPosition.Y);
                else if (hScrollBar1.IsVisible) GraphicPanel.DrawHorizCursor(Classes.Editor.SolutionState.AutoScrollPosition.X, Classes.Editor.SolutionState.AutoScrollPosition.Y);
            }

            void DrawExtraLayers()
            {
                foreach (var elb in Instance.ExtraLayerEditViewButtons)
                {
                    if (elb.Value.IsCheckedAll || elb.Key.IsCheckedAll)
                    {
                        var _extraViewLayer = Classes.Editor.Solution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(elb.Key.Text));
                        _extraViewLayer.Draw(GraphicPanel);
                    }
                }
            }

            void EntitiesDraw(int mode)
            {
                switch (mode)
                {
                    case 0:
                        Classes.Editor.Solution.Entities.Draw(GraphicPanel);
                        break;
                    case 1:
                        Classes.Editor.Solution.Entities.DrawPriority(GraphicPanel, -1);
                        Classes.Editor.Solution.Entities.DrawPriority(GraphicPanel, 0);
                        Classes.Editor.Solution.Entities.DrawPriority(GraphicPanel, 1);
                        Classes.Editor.Solution.Entities.DrawPriority(GraphicPanel, 2);
                        Classes.Editor.Solution.Entities.DrawPriority(GraphicPanel, 3);
                        break;
                    case 2:
                        Classes.Editor.Solution.Entities.DrawPriority(GraphicPanel, -1);
                        Classes.Editor.Solution.Entities.DrawPriority(GraphicPanel, 0);
                        Classes.Editor.Solution.Entities.DrawPriority(GraphicPanel, 1);
                        break;
                    case 3:
                        Classes.Editor.Solution.Entities.DrawPriority(GraphicPanel, 2);
                        Classes.Editor.Solution.Entities.DrawPriority(GraphicPanel, 3);
                        break;
                }
            }

            /*
            void DrawDebugHUD()
            {
                Point point = new Point((short)(15), (short)(15));

                DebugTextHUD.DrawEditorHUDText(this, GraphicPanel, point.X, point.Y, StateModel.GetDataFolder(), true, 255, 15);
                DebugTextHUD.DrawEditorHUDText(this, GraphicPanel, point.X, point.Y + 12 * 1, StateModel.GetMasterDataFolder(), true, 255, 22);
                DebugTextHUD.DrawEditorHUDText(this, GraphicPanel, point.X, point.Y + 12 * 2, StateModel.GetScenePath(), true, 255, 11);
                DebugTextHUD.DrawEditorHUDText(this, GraphicPanel, point.X, point.Y + 12 * 3, StateModel.GetSceneFilePath(), true, 255, 12);
                DebugTextHUD.DrawEditorHUDText(this, GraphicPanel, point.X, point.Y + 12 * 4, StateModel.GetZoom(), true, 255, 11);
                DebugTextHUD.DrawEditorHUDText(this, GraphicPanel, point.X, point.Y + 12 * 5, StateModel.GetSetupObject(), true, 255, 13);
                DebugTextHUD.DrawEditorHUDText(this, GraphicPanel, point.X, point.Y + 12 * 6, StateModel.GetSelectedZone(), true, 255, 14);

                DebugTextHUD.DrawEditorHUDText(this, GraphicPanel, point.X, point.Y + 12 * 8, "Use " + EditorControls.KeyBindPraser("StatusBoxToggle") + " to Toggle this Information", true, 255, EditorControls.KeyBindPraser("StatusBoxToggle").Length, 4);
            }*/

            void DrawSelectionBox(bool resetSelection = false)
            {
                if (!resetSelection)
                {
                    int bound_x1 = (int)(Classes.Editor.SolutionState.RegionX2 / Classes.Editor.SolutionState.Zoom); int bound_x2 = (int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom);
                    int bound_y1 = (int)(Classes.Editor.SolutionState.RegionY2 / Classes.Editor.SolutionState.Zoom); int bound_y2 = (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom);
                    if (bound_x1 != bound_x2 && bound_y1 != bound_y2)
                    {
                        if (bound_x1 > bound_x2)
                        {
                            bound_x1 = (int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom);
                            bound_x2 = (int)(Classes.Editor.SolutionState.RegionX2 / Classes.Editor.SolutionState.Zoom);
                        }
                        if (bound_y1 > bound_y2)
                        {
                            bound_y1 = (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom);
                            bound_y2 = (int)(Classes.Editor.SolutionState.RegionY2 / Classes.Editor.SolutionState.Zoom);
                        }
                        if (ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit())
                        {
                            bound_x1 = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(bound_x1, bound_y1).X;
                            bound_y1 = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesTopEdge(bound_x1, bound_y1).Y;
                            bound_x2 = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesBottomEdge(bound_x2, bound_y2).X;
                            bound_y2 = Classes.Editor.Scene.Sets.EditorLayer.GetChunkCoordinatesBottomEdge(bound_x2, bound_y2).Y;
                        }


                    }

                    GraphicPanel.DrawRectangle(bound_x1, bound_y1, bound_x2, bound_y2, System.Drawing.Color.FromArgb(100, System.Drawing.Color.Purple));
                    GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x2, bound_y1, System.Drawing.Color.Purple);
                    GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x1, bound_y2, System.Drawing.Color.Purple);
                    GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x2, bound_y1, System.Drawing.Color.Purple);
                    GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x1, bound_y2, System.Drawing.Color.Purple);
                }
                else
                {
                    Classes.Editor.SolutionState.TempSelectX1 = 0; Classes.Editor.SolutionState.TempSelectX2 = 0; Classes.Editor.SolutionState.TempSelectY1 = 0; Classes.Editor.SolutionState.TempSelectY2 = 0;
                }
            }

            void DrawBrushBox()
            {

                int offset = (Classes.Editor.SolutionState.DrawBrushSize / 2) * Classes.Editor.Constants.TILE_SIZE;
                int x1 = (int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom) - offset;
                int x2 = (int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom) + offset;
                int y1 = (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom) - offset;
                int y2 = (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom) + offset;


                int bound_x1 = (int)(x1); int bound_x2 = (int)(x2);
                int bound_y1 = (int)(y1); int bound_y2 = (int)(y2);
                if (bound_x1 != bound_x2 && bound_y1 != bound_y2)
                {
                    if (bound_x1 > bound_x2)
                    {
                        bound_x1 = (int)(x2);
                        bound_x2 = (int)(x1);
                    }
                    if (bound_y1 > bound_y2)
                    {
                        bound_y1 = (int)(y2);
                        bound_y2 = (int)(y1);
                    }
                }

                GraphicPanel.DrawRectangle(bound_x1, bound_y1, bound_x2, bound_y2, System.Drawing.Color.FromArgb(100, System.Drawing.Color.Purple));
                GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x2, bound_y1, System.Drawing.Color.Purple);
                GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x1, bound_y2, System.Drawing.Color.Purple);
                GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x2, bound_y1, System.Drawing.Color.Purple);
                GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x1, bound_y2, System.Drawing.Color.Purple);
            }

            void DrawLayer(bool ShowLayer, bool EditLayer, Classes.Editor.Scene.Sets.EditorLayer layer)
            {
                if (ShowLayer || EditLayer) layer.Draw(GraphicPanel);
            }

            void DrawGameElements()
            {
                Methods.GameHandler.DrawGameElements(GraphicPanel);

                if (Methods.GameHandler.PlayerSelected) Methods.GameHandler.MovePlayer(new System.Drawing.Point(Classes.Editor.SolutionState.LastX, Classes.Editor.SolutionState.LastY), Classes.Editor.SolutionState.Zoom, Methods.GameHandler.SelectedPlayer);
                if (Methods.GameHandler.CheckpointSelected)
                {
                    System.Drawing.Point clicked_point = new System.Drawing.Point((int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom));
                    Methods.GameHandler.UpdateCheckpoint(clicked_point);
                }
            }
        }
        private void GraphicPanel_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Int32)) && ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
            {
                System.Drawing.Point rel = GraphicPanel.PointToScreen(System.Drawing.Point.Empty);
                e.Effect = System.Windows.Forms.DragDropEffects.Move;
                Classes.Editor.Solution.EditLayerA?.StartDragOver(new System.Drawing.Point((int)(((e.X - rel.X) + Classes.Editor.SolutionState.ViewPositionX) / Classes.Editor.SolutionState.Zoom), (int)(((e.Y - rel.Y) + Classes.Editor.SolutionState.ViewPositionY) / Classes.Editor.SolutionState.Zoom)), (ushort)Instance.TilesToolbar.SelectedTile);
                Methods.Internal.UserInterface.UpdateEditLayerActions();
            }
            else
            {
                e.Effect = System.Windows.Forms.DragDropEffects.None;
            }
        }
        private void GraphicPanel_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Int32)) && ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
            {
                System.Drawing.Point rel = GraphicPanel.PointToScreen(System.Drawing.Point.Empty);
                Classes.Editor.Solution.EditLayerA?.DragOver(new System.Drawing.Point((int)(((e.X - rel.X) + Classes.Editor.SolutionState.ViewPositionX) / Classes.Editor.SolutionState.Zoom), (int)(((e.Y - rel.Y) + Classes.Editor.SolutionState.ViewPositionY) / Classes.Editor.SolutionState.Zoom)), (ushort)Instance.TilesToolbar.SelectedTile);
                GraphicPanel.Render();

            }
        }
        private void GraphicPanel_DragLeave(object sender, EventArgs e)
        {
            Classes.Editor.Solution.EditLayerA?.EndDragOver(true);
            GraphicPanel.Render();
        }
        private void GraphicPanel_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            Classes.Editor.Solution.EditLayerA?.EndDragOver(false);
        }
        public void GraphicPanel_OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Methods.Internal.Controls.GraphicPanel_OnKeyDown(sender, e);
        }
        public void GraphicPanel_OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Methods.Internal.Controls.GraphicPanel_OnKeyUp(sender, e);
        }
        #endregion

        #region Graphics Model Keyboard Events
        private void GraphicsModel_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!GraphicPanel.Focused) GraphicPanel_OnKeyDown(sender, e);
        }
        private void GraphicsModel_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!GraphicPanel.Focused) GraphicPanel_OnKeyUp(sender, e);
        }
        #endregion

        #region Zooming Methods
        public void UpdateZoomLevel(int zoom_level, System.Drawing.Point zoom_point)
        {
            double old_zoom = Classes.Editor.SolutionState.Zoom;
            Classes.Editor.SolutionState.ZoomLevel = zoom_level;
            switch (Classes.Editor.SolutionState.ZoomLevel)
            {
                case 5: Classes.Editor.SolutionState.Zoom = 4; break;
                case 4: Classes.Editor.SolutionState.Zoom = 3; break;
                case 3: Classes.Editor.SolutionState.Zoom = 2; break;
                case 2: Classes.Editor.SolutionState.Zoom = 3 / 2.0; break;
                case 1: Classes.Editor.SolutionState.Zoom = 5 / 4.0; break;
                case 0: Classes.Editor.SolutionState.Zoom = 1; break;
                case -1: Classes.Editor.SolutionState.Zoom = 2 / 3.0; break;
                case -2: Classes.Editor.SolutionState.Zoom = 1 / 2.0; break;
                case -3: Classes.Editor.SolutionState.Zoom = 1 / 3.0; break;
                case -4: Classes.Editor.SolutionState.Zoom = 1 / 4.0; break;
                case -5: Classes.Editor.SolutionState.Zoom = 1 / 8.0; break;
            }

            Classes.Editor.SolutionState.Zooming = true;

            int oldShiftX = Classes.Editor.SolutionState.ViewPositionX;
            int oldShiftY = Classes.Editor.SolutionState.ViewPositionY;

            if (Classes.Editor.Solution.CurrentScene != null) UpdateViewSize((int)(Classes.Editor.Solution.SceneWidth * Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.Solution.SceneHeight * Classes.Editor.SolutionState.Zoom));

            UpdateScrollPosXFromZoom(old_zoom, zoom_point, oldShiftX);
            UpdateScrollPosYFromZoom(old_zoom, zoom_point, oldShiftY);


            Classes.Editor.SolutionState.Zooming = false;
            Methods.Internal.UserInterface.UpdateControls();
        }
        private void UpdateScrollPosXFromZoom(double old_zoom, System.Drawing.Point zoom_point, int oldShiftX)
        {
            if (Instance.DeviceModel.hScrollBar1.IsVisible)
            {
                Classes.Editor.SolutionState.ViewPositionX = (int)((zoom_point.X + oldShiftX) / old_zoom * Classes.Editor.SolutionState.Zoom - zoom_point.X);
                Classes.Editor.SolutionState.ViewPositionX = (int)Math.Min((Instance.DeviceModel.hScrollBar1.Maximum), Math.Max(0, Classes.Editor.SolutionState.ViewPositionX));
            }
        }
        private void UpdateScrollPosYFromZoom(double old_zoom, System.Drawing.Point zoom_point, int oldShiftY)
        {
            if (Instance.DeviceModel.vScrollBar1.IsVisible)
            {
                Classes.Editor.SolutionState.ViewPositionY = (int)((zoom_point.Y + oldShiftY) / old_zoom * Classes.Editor.SolutionState.Zoom - zoom_point.Y);
                Classes.Editor.SolutionState.ViewPositionY = (int)Math.Min((Instance.DeviceModel.vScrollBar1.Maximum), Math.Max(0, Classes.Editor.SolutionState.ViewPositionY));
            }
        }
        #endregion

        #region Graphics Panel Refresh/Update Methods
        public void ResizeGraphicsModel(object sender, RoutedEventArgs e)
        {
            UpdateScalingforDPI();
            UpdateScrollBarVisibility();
            UpdateScreenSize();
            UpdateScrollViewportSize();
            UpdateScrollBarSizes();
            AssureGraphicPanelScreenFit();
        }
        public void ResetViewSize()
        {
            int trueWidth = (int)(Classes.Editor.Solution.SceneWidth * Classes.Editor.SolutionState.Zoom);
            int trueHeight = (int)(Classes.Editor.Solution.SceneHeight * Classes.Editor.SolutionState.Zoom);
            UpdateViewSize(trueWidth, trueHeight);
        }
        public void UpdateViewSize(int width = 0, int height = 0, bool resizeForm = true)
        {
            if (Core.Settings.MySettings.EntityFreeCam)
            {
                width = 10000000;
                height = 10000000;
            }
            UpdateScrollBarMaximums(width, height);
            UpdateGraphicPanelCanvasSize(width, height);
            if (resizeForm) ResizeGraphicsModel(null, null);
            if (!Core.Settings.MySettings.EntityFreeCam) SyncScrollBarsWithMaximum();

        }
        private void AssureGraphicPanelScreenFit()
        {
            while (Classes.Editor.SolutionState.ScreenWidth > Instance.DeviceModel.GraphicPanel.Width)
                ResizeGraphicPanel(Instance.DeviceModel.GraphicPanel.Width * 2, Instance.DeviceModel.GraphicPanel.Height);
            while (Classes.Editor.SolutionState.ScreenHeight > Instance.DeviceModel.GraphicPanel.Height)
                ResizeGraphicPanel(Instance.DeviceModel.GraphicPanel.Width, Instance.DeviceModel.GraphicPanel.Height * 2);


            void ResizeGraphicPanel(int width = 0, int height = 0)
            {
                UpdateGraphicPanelPhysicalSize(width, height);
                Instance.DeviceModel.GraphicPanel.ResetDevice();
                UpdateGraphicPanelCanvasSize(width, height);
            }
        }
        private void UpdateScreenSize()
        {
            if (Instance.DeviceModel.vScrollBar1.IsVisible) Classes.Editor.SolutionState.ScreenHeight = (int)Instance.DeviceModel.vScrollBar1Host.Height;
            else Classes.Editor.SolutionState.ScreenHeight = Instance.DeviceModel.GraphicPanel.Height;
            if (Instance.DeviceModel.hScrollBar1.IsVisible) Classes.Editor.SolutionState.ScreenWidth = (int)Instance.DeviceModel.hScrollBar1Host.Width;
            else Classes.Editor.SolutionState.ScreenWidth = Instance.DeviceModel.GraphicPanel.Width;
        }
        private void UpdateGraphicPanelPhysicalSize(int? width, int? height)
        {
            if (width != null && height != null)
            {
                Instance.DeviceModel.GraphicPanel.Width = (int)width;
                Instance.DeviceModel.GraphicPanel.Height = (int)height;
            }
        }
        private void UpdateGraphicPanelCanvasSize(int? width = null, int? height = null)
        {
            if (width != null && height != null)
            {
                Instance.DeviceModel.GraphicPanel.DrawWidth = Math.Min((int)width, Instance.DeviceModel.GraphicPanel.Width);
                Instance.DeviceModel.GraphicPanel.DrawHeight = Math.Min((int)height, Instance.DeviceModel.GraphicPanel.Height);
            }
            else
            {
                Instance.DeviceModel.GraphicPanel.DrawWidth = Math.Min((int)Instance.DeviceModel.hScrollBar1.Maximum, Instance.DeviceModel.GraphicPanel.Width);
                Instance.DeviceModel.GraphicPanel.DrawHeight = Math.Min((int)Instance.DeviceModel.vScrollBar1.Maximum, Instance.DeviceModel.GraphicPanel.Height);
            }
        }
        #endregion

        #region ScrollBar Update Methods
        public void SyncScrollBarsWithPos()
        {
            Classes.Editor.SolutionState.ViewPositionX = (int)Instance.DeviceModel.hScrollBar1.Value;
            Classes.Editor.SolutionState.ViewPositionY = (int)Instance.DeviceModel.vScrollBar1.Value;
        }
        private void SyncScrollBarsWithMaximum()
        {
            Instance.DeviceModel.hScrollBar1.Value = Math.Max(0, Math.Min(Instance.DeviceModel.hScrollBar1.Value, Instance.DeviceModel.hScrollBar1.Maximum));
            Instance.DeviceModel.vScrollBar1.Value = Math.Max(0, Math.Min(Instance.DeviceModel.vScrollBar1.Value, Instance.DeviceModel.vScrollBar1.Maximum));
        }
        private void UpdateScrollPosX(int? inputValue = null)
        {
            int value;
            if (inputValue != null) value = (int)inputValue;
            else value = (int)Instance.DeviceModel.hScrollBar1.Value;

            Classes.Editor.SolutionState.ViewPositionX = value;
            Instance.DeviceModel.vScrollBar1.Value = Classes.Editor.SolutionState.ViewPositionY; //TODO - Determine if this is Required
        }
        private void UpdateScrollPosY(int? inputValue = null)
        {
            int value;
            if (inputValue != null) value = (int)inputValue;
            else value = (int)Instance.DeviceModel.vScrollBar1.Value;

            Classes.Editor.SolutionState.ViewPositionY = value;
            Instance.DeviceModel.hScrollBar1.Value = Classes.Editor.SolutionState.ViewPositionX;  //TODO - Determine if this is Required
        }
        private void UpdateScrollViewportSize()
        {
            if (Classes.Editor.SolutionState.IsSceneLoaded())
            {
                if (Instance.DeviceModel.vScrollBar1.Track.ViewportSize != Classes.Editor.Solution.SceneHeight) Instance.DeviceModel.vScrollBar1.Track.ViewportSize = Classes.Editor.Solution.SceneHeight;
                if (Instance.DeviceModel.hScrollBar1.Track.ViewportSize != Classes.Editor.Solution.SceneWidth) Instance.DeviceModel.hScrollBar1.Track.ViewportSize = Classes.Editor.Solution.SceneWidth;
            }
        }
        private void UpdateScrollBarMaximums(int? width, int? height)
        {
            if (width != null && height != null)
            {
                Instance.DeviceModel.vScrollBar1.Maximum = (int)height - Instance.DeviceModel.vScrollBar1Host.Height;
                Instance.DeviceModel.hScrollBar1.Maximum = (int)width - Instance.DeviceModel.hScrollBar1Host.Width;
            }
        }
        private void UpdateScrollIncrementsX()
        {
            Instance.DeviceModel.hScrollBar1.LargeChange = Instance.DeviceModel.hScrollBar1Host.Width;
            Instance.DeviceModel.hScrollBar1.SmallChange = Instance.DeviceModel.hScrollBar1Host.Width / 8;
        }
        private void UpdateScrollIncrementsY()
        {
            Instance.DeviceModel.vScrollBar1.LargeChange = Instance.DeviceModel.vScrollBar1Host.Height;
            Instance.DeviceModel.vScrollBar1.SmallChange = Instance.DeviceModel.vScrollBar1Host.Height / 8;
        }
        private void UpdateScrollBarSizes()
        {
            if (Instance.DeviceModel.vScrollBar1.IsVisible)
            {
                int value = (int)Math.Max(0, Math.Min(Instance.DeviceModel.vScrollBar1.Value, Instance.DeviceModel.vScrollBar1.Maximum));
                UpdateScrollIncrementsY();
                UpdateScrollPosY(value);
            }
            else
            {
                UpdateScrollPosY(0);
            }

            if (Instance.DeviceModel.hScrollBar1.IsVisible)
            {
                int value = (int)Math.Max(0, Math.Min(Instance.DeviceModel.hScrollBar1.Value, Instance.DeviceModel.hScrollBar1.Maximum));
                UpdateScrollIncrementsX();
                UpdateScrollPosX();
            }
            else
            {
                UpdateScrollPosX(0);
            }
        }

        private void UpdateScrollBarVisibility()
        {
            // TODO: It hides right now few pixels at the edge

            Visibility nvscrollbar = Visibility.Visible;
            Visibility nhscrollbar = Visibility.Visible;

            if (Instance.DeviceModel.hScrollBar1.Maximum == 0) nhscrollbar = Visibility.Hidden;
            if (Instance.DeviceModel.vScrollBar1.Maximum == 0) nvscrollbar = Visibility.Hidden;

            Instance.DeviceModel.vScrollBar1.Visibility = nvscrollbar;
            Instance.DeviceModel.vScrollBar1Host.Child.Visibility = nvscrollbar;
            Instance.DeviceModel.hScrollBar1Host.Child.Visibility = nhscrollbar;
            Instance.DeviceModel.hScrollBar1.Visibility = nhscrollbar;
        }
        #endregion
    }
}
