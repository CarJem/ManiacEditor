using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows;
using IWshRuntimeLibrary;
using ManiacEditor.Actions;
using ManiacEditor.Entity_Renders;
using ManiacEditor.Controls;
using Microsoft.Scripting.Utils;
using Microsoft.Win32;
using RSDKv5;
using SharpDX.Direct3D9;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ManiacEditor.Controls.Base.Controls;
using ManiacEditor.Enums;
using ManiacEditor.Event_Handlers;
using ManiacEditor.Extensions;

namespace ManiacEditor.Controls
{
    public partial class GraphicsModel : UserControl, IDrawArea
    {
        #region Main Region
        public Controls.Base.MainEditor EditorInstance;
        public ManiacEditor.DevicePanel GraphicPanel;

		public Global.Controls.HScrollBar hScrollBar;
		public Global.Controls.VScrollBar vScrollBar;

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


        public GraphicsModel(Controls.Base.MainEditor instance)
        {
            SystemEvents.PowerModeChanged += CheckDeviceState;
            EditorInstance = instance;
            InitializeComponent();
            UpdateScrollbars();
            SetupGraphicsPanel();
            SetupEvents();
		}

        private void SetupEvents()
        {
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyUp);

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
            if (refreshing) UpdateScrollBars();
        }

        public void SetupGraphicsPanel()
        {
            this.GraphicPanel = new ManiacEditor.DevicePanel(EditorInstance);
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

        public Rectangle GetScreen()
        {
            if (Core.Settings.MySettings.EntityFreeCam) return new Rectangle(Classes.Editor.SolutionState.CustomX, Classes.Editor.SolutionState.CustomY, (int)EditorInstance.ViewPanelForm.ActualWidth, (int)EditorInstance.ViewPanelForm.ActualHeight);
            else return new Rectangle((int)Classes.Editor.SolutionState.ViewPositionX, (int)Classes.Editor.SolutionState.ViewPositionY, (int)EditorInstance.ViewPanelForm.ActualWidth, (int)EditorInstance.ViewPanelForm.ActualHeight);
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

        #region Imported Regions

        #region Mouse Actions Event Handlers
        private void GraphicPanel_OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) { EditorInstance.EditorControls.MouseMove(sender, e); }
        private void GraphicPanel_OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { EditorInstance.EditorControls.MouseDown(sender, e); }
        private void GraphicPanel_OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { EditorInstance.EditorControls.MouseUp(sender, e); }
        private void GraphicPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) { EditorInstance.EditorControls.MouseWheel(sender, e); }
        private void GraphicPanel_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) { EditorInstance.EditorControls.MouseClick(sender, e); }
        #endregion

        #region Scrollbar Events
        private void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) { EditorInstance.ZoomModel.VScrollBar1_Scroll(sender, e); }
        private void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) { EditorInstance.ZoomModel.HScrollBar1_Scroll(sender, e); }
        private void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e) { EditorInstance.ZoomModel.VScrollBar1_ValueChanged(sender, e); }
        private void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e) { EditorInstance.ZoomModel.HScrollBar1_ValueChanged(sender, e); }
        private void VScrollBar1_Entered(object sender, EventArgs e) { EditorInstance.ZoomModel.VScrollBar1_Entered(sender, e); }
        private void HScrollBar1_Entered(object sender, EventArgs e) { EditorInstance.ZoomModel.HScrollBar1_Entered(sender, e); }

        public void UpdateScrollBars()
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

            bool showEntities = EditorInstance.EditorToolbar.ShowEntities.IsChecked.Value && !EditorInstance.EditorToolbar.EditEntities.IsCheckedAll;
            bool showEntitiesEditing = EditorInstance.EditorToolbar.EditEntities.IsCheckedAll;

            bool PriorityMode = Classes.Editor.SolutionState.PrioritizedEntityViewing;
            bool AboveAllMode = Classes.Editor.SolutionState.EntitiesVisibileAboveAllLayers;


            if (EditorInstance.EntitiesToolbar?.NeedRefresh ?? false) EditorInstance.EntitiesToolbar.PropertiesRefresh();
            if (Classes.Editor.Solution.CurrentScene != null)
            {
                DrawBackground();

                //if (UIModes.DebugStatsVisibleOnPanel && Classes.Edit.Scene.EditorSolution.CurrentScene != null) DrawDebugHUD();

                if (Classes.Editor.Solution.CurrentScene.OtherLayers.Contains(Classes.Editor.Solution.EditLayerA)) Classes.Editor.Solution.EditLayerA.Draw(GraphicPanel);

                if (!Classes.Editor.SolutionState.ExtraLayersMoveToFront) DrawExtraLayers();

                DrawLayer(EditorInstance.EditorToolbar.ShowFGLower.IsChecked.Value, EditorInstance.EditorToolbar.EditFGLower.IsCheckedAll, Classes.Editor.Solution.FGLower);

                DrawLayer(EditorInstance.EditorToolbar.ShowFGLow.IsChecked.Value, EditorInstance.EditorToolbar.EditFGLow.IsCheckedAll, Classes.Editor.Solution.FGLow);


                if (showEntities && !AboveAllMode)
                    if (PriorityMode) EntitiesDraw(2);
                    else EntitiesDraw(0);

                DrawLayer(EditorInstance.EditorToolbar.ShowFGHigh.IsChecked.Value, EditorInstance.EditorToolbar.EditFGHigh.IsCheckedAll, Classes.Editor.Solution.FGHigh);

                if (showEntities && PriorityMode && !AboveAllMode) EntitiesDraw(3);

                DrawLayer(EditorInstance.EditorToolbar.ShowFGHigher.IsChecked.Value, EditorInstance.EditorToolbar.EditFGHigher.IsCheckedAll, Classes.Editor.Solution.FGHigher);

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

            if (Classes.Editor.SolutionState.ShowGrid && Classes.Editor.Solution.CurrentScene != null) EditorInstance.BackgroundDX.DrawGrid(GraphicPanel);


            if (Methods.GameHandler.GameRunning) DrawGameElements();

            if (Classes.Editor.SolutionState.Scrolling) DrawScroller();

            void DrawBackground()
            {
                if (!ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit()) EditorInstance.BackgroundDX.Draw(GraphicPanel);
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit()) if (ManiacEditor.Core.Settings.MyPerformance.ShowEditLayerBackground == true) EditorInstance.BackgroundDX.DrawEdit(GraphicPanel);
            }

            void DrawScroller()
            {
                if (vScrollBar1.IsVisible && hScrollBar1.IsVisible) GraphicPanel.Draw2DCursor(Classes.Editor.SolutionState.ScrollPosition.X, Classes.Editor.SolutionState.ScrollPosition.Y);
                else if (vScrollBar1.IsVisible) GraphicPanel.DrawVertCursor(Classes.Editor.SolutionState.ScrollPosition.X, Classes.Editor.SolutionState.ScrollPosition.Y);
                else if (hScrollBar1.IsVisible) GraphicPanel.DrawHorizCursor(Classes.Editor.SolutionState.ScrollPosition.X, Classes.Editor.SolutionState.ScrollPosition.Y);
            }

            void DrawExtraLayers()
            {
                foreach (var elb in EditorInstance.ExtraLayerEditViewButtons)
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
                Classes.Editor.Solution.EditLayerA?.StartDragOver(new System.Drawing.Point((int)(((e.X - rel.X) + Classes.Editor.SolutionState.ViewPositionX) / Classes.Editor.SolutionState.Zoom), (int)(((e.Y - rel.Y) + Classes.Editor.SolutionState.ViewPositionY) / Classes.Editor.SolutionState.Zoom)), (ushort)EditorInstance.TilesToolbar.SelectedTile);
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
                Classes.Editor.Solution.EditLayerA?.DragOver(new System.Drawing.Point((int)(((e.X - rel.X) + Classes.Editor.SolutionState.ViewPositionX) / Classes.Editor.SolutionState.Zoom), (int)(((e.Y - rel.Y) + Classes.Editor.SolutionState.ViewPositionY) / Classes.Editor.SolutionState.Zoom)), (ushort)EditorInstance.TilesToolbar.SelectedTile);
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
            EditorInstance.EditorControls.GraphicPanel_OnKeyDown(sender, e);
        }
        public void GraphicPanel_OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            EditorInstance.EditorControls.GraphicPanel_OnKeyUp(sender, e);
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

        #endregion

    }
}
