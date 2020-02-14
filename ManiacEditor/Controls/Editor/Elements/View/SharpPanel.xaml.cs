using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ManiacEditor.Enums;
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


namespace ManiacEditor.Controls.Editor.Elements.View
{
    /// <summary>
    /// Interaction logic for SharpPanel.xaml
    /// </summary>
    public partial class SharpPanel : UserControl, IDrawArea
    {
        #region Definitions

        private ManiacEditor.Controls.Editor.MainEditor Instance;
        public ManiacEditor.DevicePanel GraphicPanel;
        private System.Windows.Forms.Panel HostPanel;

        #region DPI Definitions
        public double DPIScale { get; set; }
        public int RenderingWidth { get; set; }
        public int RenderingHeight { get; set; }
        #endregion

        #endregion

        #region Init
        public SharpPanel()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.ViewPanelContextMenu.Foreground = (SolidColorBrush)FindResource("NormalText");
                this.ViewPanelContextMenu.Background = (SolidColorBrush)FindResource("NormalBackground");

                SetupScrollBarEvents();
                SetupGraphicsPanel();
                SetupOtherEvents();
            }
        }
        #endregion

        #region Events Init

        public void UpdateInstance(MainEditor editor)
        {
            Instance = editor;
            (Instance as Window).SizeChanged += SharpPanel_SizeChanged;
            (Instance as Window).LocationChanged += SharpPanel_LocationChanged; ;
            (Instance as Window).MouseMove += SharpPanel_MouseMove; ;
        }
        public void InitalizeGraphicsPanel()
        {
            this.Host.Foreground = (SolidColorBrush)FindResource("NormalText");
            this.GraphicPanel.Init(this);
        }
        private void SetupScrollBarEvents()
        {
            vScrollBar1.Scroll += this.VScrollBar1_Scroll;
            vScrollBar1.ValueChanged += this.VScrollBar1_ValueChanged;
            vScrollBar1.MouseEnter += this.VScrollBar1_Entered;
            hScrollBar1.Scroll += this.HScrollBar1_Scroll;
            hScrollBar1.ValueChanged += this.HScrollBar1_ValueChanged;
            hScrollBar1.MouseEnter += this.HScrollBar1_Entered;
        }
        public void SetupScrollBars(bool refreshing = false)
        {
            if ((hScrollBar1 != null || vScrollBar1 != null) && refreshing)
            {
                vScrollBar1.Scroll -= this.VScrollBar1_Scroll;
                vScrollBar1.ValueChanged -= this.VScrollBar1_ValueChanged;
                vScrollBar1.MouseEnter -= this.VScrollBar1_Entered;
                hScrollBar1.Scroll -= this.HScrollBar1_Scroll;
                hScrollBar1.ValueChanged -= this.HScrollBar1_ValueChanged;
                hScrollBar1.MouseEnter -= this.HScrollBar1_Entered;
            }
            hScrollBar1 = new System.Windows.Controls.Primitives.ScrollBar();
            vScrollBar1 = new System.Windows.Controls.Primitives.ScrollBar();
            if (refreshing) SetupScrollBarEvents();
        }
        private void SetupOtherEvents()
        {
            SystemEvents.PowerModeChanged += CheckDeviceState;
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            UpdateScalingforDPI();
        }
        private void GraphicPanel_PreRenderingSetup(object sender, DeviceEventArgs e)
        {
            GraphicPanel.OnRender -= GraphicPanel_PreRenderingSetup;
            this.UpdateZoomLevel((int)Classes.Editor.SolutionState.ZoomLevel, new System.Drawing.Point(0, 0));
        }
        public void SetupGraphicsPanel()
        {
            this.GraphicPanel = new ManiacEditor.DevicePanel(Instance);
            this.GraphicPanel.AllowDrop = true;
            this.GraphicPanel.AutoSize = false;
            this.GraphicPanel.DeviceBackColor = System.Drawing.Color.White;
            this.GraphicPanel.Location = new System.Drawing.Point(-1, 0);
            this.GraphicPanel.Margin = new System.Windows.Forms.Padding(0);
            this.GraphicPanel.Name = "GraphicPanel";
            this.GraphicPanel.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            this.GraphicPanel.Height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            this.GraphicPanel.TabIndex = 10;

            HostPanel = new System.Windows.Forms.Panel();
            HostPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            HostPanel.Controls.Add(GraphicPanel);

            this.Host.Child = HostPanel;

            GraphicPanel.OnRender += this.GraphicPanel_OnRender;
            GraphicPanel.OnRender += this.GraphicPanel_PreRenderingSetup;
            GraphicPanel.OnCreateDevice += this.GraphicPanel_OnResetDevice;
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
        }

        #endregion

        #region Context Menus

        #region Context Menu Events
        private void TileManiacEditTileEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.TileManiacIntergration(); }

        #endregion

        #region Game Running Context Menu Events
        private void MoveThePlayerToHere(object sender, RoutedEventArgs e) { Methods.Runtime.GameHandler.MoveThePlayerToHere(); }
        private void SetPlayerRespawnToHere(object sender, RoutedEventArgs e) { Methods.Runtime.GameHandler.SetPlayerRespawnToHere(); }
        private void MoveCheckpoint(object sender, RoutedEventArgs e) { Methods.Runtime.GameHandler.CheckpointSelected = true; }
        private void RemoveCheckpoint(object sender, RoutedEventArgs e) { Methods.Runtime.GameHandler.UpdateCheckpoint(new System.Drawing.Point(0, 0), false); }
        private void AssetReset(object sender, RoutedEventArgs e) { Methods.Runtime.GameHandler.AssetReset(); }
        private void RestartScene(object sender, RoutedEventArgs e) { Methods.Runtime.GameHandler.RestartScene(); }
        #endregion

        #endregion

        #region DPI / Zooming / Power Status

        #region IDrawArea Methods

        public double GetZoom()
        {
            return Classes.Editor.SolutionState.Zoom;
        }
        public new void Dispose()
        {
            this.GraphicPanel.Dispose();
            this.GraphicPanel = null;
            this.Host.Child.Dispose();
        }
        public System.Drawing.Rectangle GetScreen()
        {
            if (Methods.Settings.MySettings.EntityFreeCam) return new System.Drawing.Rectangle(Classes.Editor.SolutionState.CustomViewPositionX, Classes.Editor.SolutionState.CustomViewPositionY, RenderingWidth, RenderingHeight);
            else return new System.Drawing.Rectangle((int)Classes.Editor.SolutionState.ViewPositionX, (int)Classes.Editor.SolutionState.ViewPositionY, RenderingWidth, RenderingHeight);
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

        #region DPI/Resize Events

        private void Host_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            Console.WriteLine("DPI Change Detected!");
        }

        private void SharpPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeGraphicsPanel();
            Instance.ViewPanel.InfoHUD.UpdatePopupSize();
        }

        private void SharpPanel_MouseMove(object sender, MouseEventArgs e)
        {
            //Instance.ViewPanel.InfoHUD.UpdatePopupVisibility();
        }

        private void SharpPanel_LocationChanged(object sender, EventArgs e)
        {
            Instance.ViewPanel.InfoHUD.UpdatePopupSize();
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            ResizeGraphicsPanel();
            Instance.ViewPanel.InfoHUD.UpdatePopupSize();
        }

        #endregion

        #region DPI Scaling Methods

        private void UpdateDPIScale()
        {
            DPIScale = (double)(100 * System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth) / 100;
        }

        private void UpdateRenderingScale()
        {
            if (Instance != null)
            {
                RenderingWidth = (int)(this.ActualWidth * DPIScale);
                RenderingHeight = (int)(this.ActualHeight * DPIScale);
            }
        }

        public void UpdateScalingforDPI()
        {
            UpdateDPIScale();
            UpdateRenderingScale();
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

            if (Classes.Editor.Solution.CurrentScene != null) ResizeGraphicsPanel();

            UpdateScrollPosXFromZoom(old_zoom, zoom_point, oldShiftX);
            UpdateScrollPosYFromZoom(old_zoom, zoom_point, oldShiftY);


            Classes.Editor.SolutionState.Zooming = false;
            Methods.Internal.UserInterface.UpdateControls();
        }
        public void ResetZoomLevel(bool resizeForm = true)
        {
            Classes.Editor.SolutionState.ZoomLevel = 1;
            this.UpdateZoomLevel((int)Classes.Editor.SolutionState.ZoomLevel, new System.Drawing.Point(0, 0));
        }
        private void UpdateScrollPosXFromZoom(double old_zoom, System.Drawing.Point zoom_point, int oldShiftX)
        {
            if (this.hScrollBar1.IsVisible)
            {
                int value = (int)((zoom_point.X + oldShiftX) / old_zoom * Classes.Editor.SolutionState.Zoom - zoom_point.X);
                value = (int)Math.Min((this.hScrollBar1.Maximum), Math.Max(0, value));
                Classes.Editor.SolutionState.SetViewPositionX(value);
            }
        }
        private void UpdateScrollPosYFromZoom(double old_zoom, System.Drawing.Point zoom_point, int oldShiftY)
        {
            if (this.vScrollBar1.IsVisible)
            {
                int value = (int)((zoom_point.Y + oldShiftY) / old_zoom * Classes.Editor.SolutionState.Zoom - zoom_point.Y);
                value = (int)Math.Min((this.vScrollBar1.Maximum), Math.Max(0, value));
                Classes.Editor.SolutionState.SetViewPositionY(value);
            }
        }
        #endregion

        #region Power Status

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

        #endregion

        #endregion

        #region Graphic Panel

        #region GraphicPanel Event Handlers
        private void GraphicPanel_OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseMove(sender, e); }
        private void GraphicPanel_OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseDown(sender, e); }
        private void GraphicPanel_OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseUp(sender, e); }
        private void GraphicPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseWheel(sender, e); }
        private void GraphicPanel_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) { Methods.Internal.Controls.MouseClick(sender, e); }
        public void GraphicPanel_OnResetDevice(object sender, DeviceEventArgs e) { Device device = e.Device; }
        private void GraphicPanel_OnRender(object sender, DeviceEventArgs e) { GP_Render(); }
        public void GraphicPanel_OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e) { Methods.Internal.Controls.GraphicPanel_OnKeyDown(sender, e); }
        public void GraphicPanel_OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e) { Methods.Internal.Controls.GraphicPanel_OnKeyUp(sender, e); }
        private void GraphicPanel_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) { GP_DragEnter(sender, e); }
        private void GraphicPanel_DragOver(object sender, System.Windows.Forms.DragEventArgs e) { GP_DragOver(sender, e); }
        private void GraphicPanel_DragLeave(object sender, EventArgs e) { GP_DragLeave(sender, e); }
        private void GraphicPanel_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) { GP_DragDrop(sender, e); }

        #endregion

        #region Graphics Panel Drag Events

        private void GP_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Int32)) && ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
            {
                System.Drawing.Point rel = GraphicPanel.PointToScreen(System.Drawing.Point.Empty);
                Classes.Editor.Solution.EditLayerA?.DragOver(new System.Drawing.Point((int)(((e.X - rel.X) + Classes.Editor.SolutionState.ViewPositionX) / Classes.Editor.SolutionState.Zoom), (int)(((e.Y - rel.Y) + Classes.Editor.SolutionState.ViewPositionY) / Classes.Editor.SolutionState.Zoom)), (ushort)Instance.TilesToolbar.SelectedTile);
                GraphicPanel.Render();

            }
        }
        private void GP_DragLeave(object sender, EventArgs e)
        {
            Classes.Editor.Solution.EditLayerA?.EndDragOver(true);
            GraphicPanel.Render();
        }
        private void GP_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            Classes.Editor.Solution.EditLayerA?.EndDragOver(false);
        }
        private void GP_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
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

        #region Graphics Panel Refresh/Update Methods
        public void ResizeGraphicsPanel()
        {
            UpdateScalingforDPI();
            UpdateScrollBarVisibility();
            UpdateScrollViewportSize();
            UpdateScrollBarSizes();
            UpdateScrollBarMaximumValues();
            UpdateGraphicPanelSize();
            if (!Methods.Settings.MySettings.EntityFreeCam) SyncScrollBarsWithMaximum();
        }

        private void UpdateGraphicPanelSize()
        {
            //this.GraphicPanel.Width = RenderingWidth;
            //this.GraphicPanel.Height = RenderingHeight;
            //GraphicPanel.ResetDevice();
        }

        #endregion

        #endregion

        #region ScrollBars

        #region ScrollBar Events
        public void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            //if (!DevicePanel.isRendering && !GraphicPanel.mouseMoved) GraphicPanel.Render();
            //TODO - Determine if we Rather Responsive Over Smooth Transitions
        }
        public void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            //if (!DevicePanel.isRendering && !GraphicPanel.mouseMoved) GraphicPanel.Render();
            //TODO - Determine if we Rather Responsive Over Smooth Transitions
        }
        public void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (Classes.Editor.SolutionState.AnyDragged)
            {
                GraphicPanel.OnMouseMoveEventCreate();
            }

        }
        public void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (Classes.Editor.SolutionState.AnyDragged)
            {
                GraphicPanel.OnMouseMoveEventCreate();
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

        #region ScrollBar Update Methods
        private void SyncScrollBarsWithMaximum()
        {
            this.hScrollBar1.Value = Math.Max(0, Math.Min(this.hScrollBar1.Value, this.hScrollBar1.Maximum));
            this.vScrollBar1.Value = Math.Max(0, Math.Min(this.vScrollBar1.Value, this.vScrollBar1.Maximum));
        }
        private void UpdateScrollViewportSize()
        {
            if (Classes.Editor.SolutionState.IsSceneLoaded() && this.vScrollBar1.Track != null && this.hScrollBar1.Track != null)
            {
                if (this.vScrollBar1.Track.ViewportSize != Classes.Editor.Solution.SceneHeight) this.vScrollBar1.Track.ViewportSize = Classes.Editor.Solution.SceneHeight;
                if (this.hScrollBar1.Track.ViewportSize != Classes.Editor.Solution.SceneWidth) this.hScrollBar1.Track.ViewportSize = Classes.Editor.Solution.SceneWidth;
            }
        }
        private void UpdateScrollBarMaximumValues()
        {
            double height = !double.IsNaN(this.vScrollBar1.ActualHeight) ? this.vScrollBar1.ActualHeight : 0;
            double width = !double.IsNaN(this.hScrollBar1.ActualWidth) ? this.hScrollBar1.ActualWidth : 0;
            vScrollBar1.Maximum = (ManiacEditor.Classes.Editor.Solution.SceneHeight * ManiacEditor.Classes.Editor.SolutionState.Zoom) - height;
            hScrollBar1.Maximum = (ManiacEditor.Classes.Editor.Solution.SceneWidth * ManiacEditor.Classes.Editor.SolutionState.Zoom) - width;
        }
        private void UpdateScrollIncrementsX()
        {
            double width = !double.IsNaN(this.hScrollBar1.Width) ? this.hScrollBar1.Width : 0;
            hScrollBar1.LargeChange = width;
            hScrollBar1.SmallChange = (width == 0 ? 0 : width / 8);
        }
        private void UpdateScrollIncrementsY()
        {
            double height = !double.IsNaN(this.vScrollBar1.Height) ? this.vScrollBar1.Height : 0;
            vScrollBar1.LargeChange = height;
            vScrollBar1.SmallChange = (height == 0 ? 0 : height / 8);
        }
        private void UpdateScrollBarSizes()
        {
            UpdateScrollBarMaximumValues();
            if (this.vScrollBar1.IsVisible)
            {
                int value = (int)Math.Max(0, Math.Min(this.vScrollBar1.Value, this.vScrollBar1.Maximum));
                UpdateScrollIncrementsY();
            }

            if (this.hScrollBar1.IsVisible)
            {
                int value = (int)Math.Max(0, Math.Min(this.hScrollBar1.Value, this.hScrollBar1.Maximum));
                UpdateScrollIncrementsX();
            }
        }
        private void UpdateScrollBarVisibility()
        {
            // TODO: It hides right now few pixels at the edge

            Visibility nvscrollbar = Visibility.Visible;
            Visibility nhscrollbar = Visibility.Visible;

            if (this.hScrollBar1.Maximum == 0) nhscrollbar = Visibility.Hidden;
            if (this.vScrollBar1.Maximum == 0) nvscrollbar = Visibility.Hidden;

            this.vScrollBar1.Visibility = nvscrollbar;
            this.hScrollBar1.Visibility = nhscrollbar;
        }
        #endregion

        #endregion

        #region Rendering

        private void GP_Render()
        {
            bool showEntities = Instance.EditorToolbar.ShowEntities.IsChecked.Value && !Instance.EditorToolbar.EditEntities.IsCheckedAll;
            bool showEntitiesEditing = Instance.EditorToolbar.EditEntities.IsCheckedAll;

            bool PriorityMode = Classes.Editor.SolutionState.PrioritizedEntityViewing;
            bool AboveAllMode = Classes.Editor.SolutionState.EntitiesVisibileAboveAllLayers;

            Instance.ViewPanel.InfoHUD.UpdatePopupVisibility();

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

            if (Classes.Editor.SolutionState.ShowGrid && Classes.Editor.Solution.CurrentScene != null) Instance.EditBackground.DrawGrid(GraphicPanel);


            if (Methods.Runtime.GameHandler.GameRunning) DrawGameElements();

            if (Classes.Editor.SolutionState.AutoScrolling) DrawScroller();

            void DrawBackground()
            {
                if (!ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit()) if (ManiacEditor.Methods.Settings.MyPerformance.HideNormalBackground == false) Instance.EditBackground.Draw(GraphicPanel);
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit()) if (ManiacEditor.Methods.Settings.MyPerformance.ShowEditLayerBackground == true) Instance.EditBackground.DrawEdit(GraphicPanel);
            }

            void DrawScroller()
            {
                if (this.vScrollBar1.IsVisible && this.hScrollBar1.IsVisible) GraphicPanel.Draw2DCursor(Classes.Editor.SolutionState.AutoScrollPosition.X, Classes.Editor.SolutionState.AutoScrollPosition.Y);
                else if (this.vScrollBar1.IsVisible) GraphicPanel.DrawVertCursor(Classes.Editor.SolutionState.AutoScrollPosition.X, Classes.Editor.SolutionState.AutoScrollPosition.Y);
                else if (this.hScrollBar1.IsVisible) GraphicPanel.DrawHorizCursor(Classes.Editor.SolutionState.AutoScrollPosition.X, Classes.Editor.SolutionState.AutoScrollPosition.Y);
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
                Methods.Runtime.GameHandler.DrawGameElements(GraphicPanel);

                if (Methods.Runtime.GameHandler.PlayerSelected) Methods.Runtime.GameHandler.MovePlayer(new System.Drawing.Point(Classes.Editor.SolutionState.LastX, Classes.Editor.SolutionState.LastY), Classes.Editor.SolutionState.Zoom, Methods.Runtime.GameHandler.SelectedPlayer);
                if (Methods.Runtime.GameHandler.CheckpointSelected)
                {
                    System.Drawing.Point clicked_point = new System.Drawing.Point((int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom));
                    Methods.Runtime.GameHandler.UpdateCheckpoint(clicked_point);
                }
            }
        }

        #endregion

    }
}
