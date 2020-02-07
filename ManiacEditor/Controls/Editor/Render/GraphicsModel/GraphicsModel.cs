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
        #region Main Region
        public Controls.Editor.MainEditor Instance;
        public ManiacEditor.DevicePanel GraphicPanel;

		public Global.Controls.HScrollBar hScrollBar;
		public Global.Controls.VScrollBar vScrollBar;

        public double DPIScale { get; set; }

        public int RenderingWidth { get; set; }
        public int RenderingHeight { get; set; }

		public System.Windows.Controls.Primitives.ScrollBar vScrollBar1 { get => GetScrollBarV(); }
		public System.Windows.Controls.Primitives.ScrollBar hScrollBar1 { get => GetScrollBarH(); }

        private System.Windows.Controls.Primitives.ScrollBar GetScrollBarV()
        {
            return vScrollBar.scroller;
        }

        private System.Windows.Controls.Primitives.ScrollBar GetScrollBarH()
        {
            return hScrollBar.scroller;
        }


        public GraphicsModel(Controls.Editor.MainEditor instance)
        {
            SystemEvents.PowerModeChanged += CheckDeviceState;
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            Instance = instance;
            (Instance as Window).SizeChanged += GraphicsModel_SizeChanged;
            InitializeComponent();
            UpdateScrollbars();
            SetupGraphicsPanel();
            SetupEvents();
		}

        private void GraphicsModel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateDPIScale();
            UpdateRenderingScale();
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            UpdateDPIScale();
            UpdateRenderingScale();
        }

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

        private void Editor_Resize(object sender, EventArgs e)
        {
            UpdateRenderingScale();
        }

        private void SetupEvents()
        {
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyUp);
            this.Resize += Editor_Resize;

            UpdateDPIScale();
            UpdateRenderingScale();

            vScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.VScrollBar1_Scroll);
            vScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.VScrollBar1_ValueChanged);
            vScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.VScrollBar1_Entered);
            hScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.HScrollBar1_Scroll);
            hScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.HScrollBar1_ValueChanged);
            hScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.HScrollBar1_Entered);

            GraphicPanel.OnRender += new ManiacEditor.Event_Handlers.RenderEventHandler(this.GraphicPanel_OnRender);
            GraphicPanel.OnCreateDevice += new ManiacEditor.Event_Handlers.CreateDeviceEventHandler(this.OnResetDevice);
            GraphicPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragDrop);
            GraphicPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragEnter);
            GraphicPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragOver);
            GraphicPanel.DragLeave += new System.EventHandler(this.GraphicPanel_DragLeave);
            GraphicPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GraphicPanel_OnKeyDown);
            GraphicPanel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GraphicPanel_OnKeyUp);
            GraphicPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_MouseClick);
            GraphicPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseDown);
            GraphicPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseMove);
            GraphicPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseUp);
            GraphicPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_MouseWheel);

            this.GraphicPanel.Width = SystemInformation.PrimaryMonitorSize.Width;
            this.GraphicPanel.Height = SystemInformation.PrimaryMonitorSize.Height;
        }

        public void UpdateScrollbars(bool refreshing = false)
        {
            hScrollBar = new Global.Controls.HScrollBar();
            vScrollBar = new Global.Controls.VScrollBar();
            hScrollBar1Host.Child = hScrollBar;
            vScrollBar1Host.Child = vScrollBar;
            if (refreshing) UpdateScrollBarEvents();
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

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            LOGPIXELSY = 90,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }

        public Rectangle GetScreen()
        {
            if (Core.Settings.MySettings.EntityFreeCam) return new Rectangle(Classes.Editor.SolutionState.CustomX, Classes.Editor.SolutionState.CustomY, RenderingWidth, RenderingHeight);
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

		private void EditorView_Load(object sender, EventArgs e)
		{

		}
        #endregion

        #region Mouse Actions Event Handlers
        private void GraphicPanel_OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseMove(sender, e); }
        private void GraphicPanel_OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseDown(sender, e); }
        private void GraphicPanel_OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseUp(sender, e); }
        private void GraphicPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseWheel(sender, e); }
        private void GraphicPanel_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseClick(sender, e); }
        #endregion

        #region Scrollbar Events

        private bool AllowScrollUpdate = true;
        public void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Editor.SolutionState.ViewPositionY = (int)e.NewValue;
                UpdateScrollBarValues();
            }
            Instance.DeviceModel.GraphicPanel.Render();
        }
        public void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Editor.SolutionState.ViewPositionX = (int)e.NewValue;
                UpdateScrollBarValues();
            }
            Instance.DeviceModel.GraphicPanel.Render();
        }
        public void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Editor.SolutionState.ViewPositionY = (int)Instance.DeviceModel.vScrollBar1.Value;
                UpdateScrollBarValues();
            }
            //TODO: Determine if we still need this
            //if (!(Classes.Edit.SolutionState.Zooming || Classes.Edit.SolutionState.DraggingSelection || Classes.Edit.SolutionState.Dragged || Classes.Edit.SolutionState.Scrolling)) Editor.FormsModel.GraphicPanel.Render();
            if (Classes.Editor.SolutionState.DraggingSelection)
            {
                Instance.DeviceModel.GraphicPanel.OnMouseMoveEventCreate();
            }

        }
        public void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Editor.SolutionState.ViewPositionX = (int)Instance.DeviceModel.hScrollBar1.Value;
                UpdateScrollBarValues();
            }
            //TODO: Determine if we still need this
            //if (!(Classes.Edit.SolutionState.Zooming || Classes.Edit.SolutionState.DraggingSelection || Classes.Edit.SolutionState.Dragged || Classes.Edit.SolutionState.Scrolling)) Editor.FormsModel.GraphicPanel.Render();
            if (Classes.Editor.SolutionState.DraggingSelection)
            {
                Instance.DeviceModel.GraphicPanel.OnMouseMoveEventCreate();
            }

        }
        public void HScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Classes.Editor.SolutionState.ScrollLocked)
            {
                Classes.Editor.SolutionState.ScrollDirection = (int)ScrollDir.X;
            }
        }
        public void VScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Classes.Editor.SolutionState.ScrollLocked)
            {
                Classes.Editor.SolutionState.ScrollDirection = (int)ScrollDir.Y;
            }
        }
        public void UpdateScrollBarEvents()
        {
            this.vScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.VScrollBar1_Scroll);
            this.vScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.VScrollBar1_ValueChanged);
            this.vScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.VScrollBar1_Entered);
            this.hScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.HScrollBar1_Scroll);
            this.hScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.HScrollBar1_ValueChanged);
            this.hScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.HScrollBar1_Entered);
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

            if (Classes.Editor.SolutionState.Scrolling) DrawScroller();

            void DrawBackground()
            {
                if (!ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit()) Instance.BackgroundDX.Draw(GraphicPanel);
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit()) if (ManiacEditor.Core.Settings.MyPerformance.ShowEditLayerBackground == true) Instance.BackgroundDX.DrawEdit(GraphicPanel);
            }

            void DrawScroller()
            {
                if (vScrollBar1.IsVisible && hScrollBar1.IsVisible) GraphicPanel.Draw2DCursor(Classes.Editor.SolutionState.ScrollPosition.X, Classes.Editor.SolutionState.ScrollPosition.Y);
                else if (vScrollBar1.IsVisible) GraphicPanel.DrawVertCursor(Classes.Editor.SolutionState.ScrollPosition.X, Classes.Editor.SolutionState.ScrollPosition.Y);
                else if (hScrollBar1.IsVisible) GraphicPanel.DrawHorizCursor(Classes.Editor.SolutionState.ScrollPosition.X, Classes.Editor.SolutionState.ScrollPosition.Y);
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

        #region Key Events

        private void Editor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!GraphicPanel.Focused)
            {
                GraphicPanel_OnKeyDown(sender, e);
            }
        }
        private void Editor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!GraphicPanel.Focused)
            {
                GraphicPanel_OnKeyUp(sender, e);
            }
        }

        #endregion

        #region Zoom Model Regions
        public void UpdateScrollBarValues()
        {
            //TODO: Determine if we still need this
            /*AllowScrollUpdate = false;
            Editor.editorView.hScrollBar1.Value = (int)Editor.editorView.hScrollBar1.Value;
            Editor.editorView.vScrollBar1.Value = (int)Editor.editorView.vScrollBar1.Value;
            AllowScrollUpdate = true;*/
        }
        public void GraphicsResize(object sender, RoutedEventArgs e)
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

            if (Instance.DeviceModel.vScrollBar1.IsVisible)
            {
                Instance.DeviceModel.vScrollBar1.LargeChange = Instance.DeviceModel.vScrollBar1Host.Height;
                Instance.DeviceModel.vScrollBar1.SmallChange = Instance.DeviceModel.vScrollBar1Host.Height / 8;
                Classes.Editor.SolutionState.ScreenHeight = (int)Instance.DeviceModel.vScrollBar1Host.Height;
                Instance.DeviceModel.vScrollBar1.Value = Math.Max(0, Math.Min(Instance.DeviceModel.vScrollBar1.Value, Instance.DeviceModel.vScrollBar1.Maximum));
                if (Instance.DeviceModel.vScrollBar1.Track.ViewportSize != Classes.Editor.Solution.SceneHeight) Instance.DeviceModel.vScrollBar1.Track.ViewportSize = Classes.Editor.Solution.SceneHeight;
            }
            else
            {
                Classes.Editor.SolutionState.ScreenHeight = Instance.DeviceModel.GraphicPanel.Height;
                Classes.Editor.SolutionState.ViewPositionY = 0;
                Instance.DeviceModel.vScrollBar1.Value = 0;
            }
            if (Instance.DeviceModel.hScrollBar1.IsVisible)
            {
                Instance.DeviceModel.hScrollBar1.LargeChange = Instance.DeviceModel.hScrollBar1Host.Width;
                Instance.DeviceModel.hScrollBar1.SmallChange = Instance.DeviceModel.hScrollBar1Host.Width / 8;
                Classes.Editor.SolutionState.ScreenWidth = (int)Instance.DeviceModel.hScrollBar1Host.Width;
                Instance.DeviceModel.hScrollBar1.Value = Math.Max(0, Math.Min(Instance.DeviceModel.hScrollBar1.Value, Instance.DeviceModel.hScrollBar1.Maximum));
                if (Instance.DeviceModel.hScrollBar1.Track.ViewportSize != Classes.Editor.Solution.SceneWidth) Instance.DeviceModel.hScrollBar1.Track.ViewportSize = Classes.Editor.Solution.SceneWidth;
            }
            else
            {
                Classes.Editor.SolutionState.ScreenWidth = Instance.DeviceModel.GraphicPanel.Width;
                Classes.Editor.SolutionState.ViewPositionX = 0;
                Instance.DeviceModel.hScrollBar1.Value = 0;
            }

            while (Classes.Editor.SolutionState.ScreenWidth > Instance.DeviceModel.GraphicPanel.Width)
                ResizeGraphicPanel(Instance.DeviceModel.GraphicPanel.Width * 2, Instance.DeviceModel.GraphicPanel.Height);
            while (Classes.Editor.SolutionState.ScreenHeight > Instance.DeviceModel.GraphicPanel.Height)
                ResizeGraphicPanel(Instance.DeviceModel.GraphicPanel.Width, Instance.DeviceModel.GraphicPanel.Height * 2);
        }
        public void SetViewSize(int width = 0, int height = 0, bool resizeForm = true)
        {
            if (Core.Settings.MySettings.EntityFreeCam)
            {
                width = 10000000;
                height = 10000000;
            }

            Instance.DeviceModel.vScrollBar1.Maximum = height - Instance.DeviceModel.vScrollBar1Host.Height;
            Instance.DeviceModel.hScrollBar1.Maximum = width - Instance.DeviceModel.hScrollBar1Host.Width;

            Instance.DeviceModel.GraphicPanel.DrawWidth = Math.Min((int)width, Instance.DeviceModel.GraphicPanel.Width);
            Instance.DeviceModel.GraphicPanel.DrawHeight = Math.Min((int)height, Instance.DeviceModel.GraphicPanel.Height);

            if (resizeForm) GraphicsResize(null, null);

            if (!Core.Settings.MySettings.EntityFreeCam)
            {
                Instance.DeviceModel.hScrollBar1.Value = Math.Max(0, Math.Min(Instance.DeviceModel.hScrollBar1.Value, Instance.DeviceModel.hScrollBar1.Maximum));
                Instance.DeviceModel.vScrollBar1.Value = Math.Max(0, Math.Min(Instance.DeviceModel.vScrollBar1.Value, Instance.DeviceModel.vScrollBar1.Maximum));
            }

        }
        public void SetZoomLevel(int zoom_level, System.Drawing.Point zoom_point, double zoom_level_d = 0.0, bool updateControls = true)
        {
            double old_zoom = Classes.Editor.SolutionState.Zoom;



            if (zoom_level_d == 0.0)
            {
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
            }
            else
            {
                Classes.Editor.SolutionState.ZoomLevel = (int)zoom_level_d;
                Classes.Editor.SolutionState.Zoom = zoom_level_d;
            }


            Classes.Editor.SolutionState.Zooming = true;

            int oldShiftX = Classes.Editor.SolutionState.ViewPositionX;
            int oldShiftY = Classes.Editor.SolutionState.ViewPositionY;

            if (Classes.Editor.Solution.CurrentScene != null)
                SetViewSize((int)(Classes.Editor.Solution.SceneWidth * Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.Solution.SceneHeight * Classes.Editor.SolutionState.Zoom), updateControls);


            if (Instance.DeviceModel.hScrollBar1.IsVisible)
            {
                Classes.Editor.SolutionState.ViewPositionX = (int)((zoom_point.X + oldShiftX) / old_zoom * Classes.Editor.SolutionState.Zoom - zoom_point.X);
                Classes.Editor.SolutionState.ViewPositionX = (int)Math.Min((Instance.DeviceModel.hScrollBar1.Maximum), Math.Max(0, Classes.Editor.SolutionState.ViewPositionX));
                Instance.DeviceModel.hScrollBar1.Value = Classes.Editor.SolutionState.ViewPositionX;
            }
            if (Instance.DeviceModel.vScrollBar1.IsVisible)
            {
                Classes.Editor.SolutionState.ViewPositionY = (int)((zoom_point.Y + oldShiftY) / old_zoom * Classes.Editor.SolutionState.Zoom - zoom_point.Y);
                Classes.Editor.SolutionState.ViewPositionY = (int)Math.Min((Instance.DeviceModel.vScrollBar1.Maximum), Math.Max(0, Classes.Editor.SolutionState.ViewPositionY));
                Instance.DeviceModel.vScrollBar1.Value = Classes.Editor.SolutionState.ViewPositionY;
            }


            Classes.Editor.SolutionState.Zooming = false;

            if (updateControls) Methods.Internal.UserInterface.UpdateControls();
        }
        public void ResetViewSize()
        {
            SetViewSize((int)(Classes.Editor.Solution.SceneWidth * Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.Solution.SceneHeight * Classes.Editor.SolutionState.Zoom));
        }
        public void ResizeGraphicPanel(int width = 0, int height = 0)
        {
            if (Core.Settings.MySettings.EntityFreeCam)
            {
                width = Classes.Editor.Solution.SceneWidth;
                height = Classes.Editor.Solution.SceneHeight;
            }

            Instance.DeviceModel.GraphicPanel.Width = width;
            Instance.DeviceModel.GraphicPanel.Height = height;

            Instance.DeviceModel.GraphicPanel.ResetDevice();

            Instance.DeviceModel.GraphicPanel.DrawWidth = Math.Min((int)Instance.DeviceModel.hScrollBar1.Maximum, Instance.DeviceModel.GraphicPanel.Width);
            Instance.DeviceModel.GraphicPanel.DrawHeight = Math.Min((int)Instance.DeviceModel.vScrollBar1.Maximum, Instance.DeviceModel.GraphicPanel.Height);
        }

        #endregion

    }
}
