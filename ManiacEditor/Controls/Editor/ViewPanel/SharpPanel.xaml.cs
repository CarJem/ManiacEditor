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
using ManiacEditor.Controls.Global;
using ManiacEditor.Entity_Renders;
using ManiacEditor.Enums;
using ManiacEditor.EventHandlers;
using ManiacEditor.Extensions;
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


namespace ManiacEditor.Controls.Editor.ViewPanel
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
            (Instance as Window).LocationChanged += SharpPanel_LocationChanged;
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
            this.UpdateZoomLevel((int)Methods.Editor.SolutionState.ZoomLevel, new System.Drawing.Point(0, 0));
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
            return Methods.Editor.SolutionState.Zoom;
        }
        public new void Dispose()
        {
            this.GraphicPanel.Dispose();
            this.GraphicPanel = null;
            this.Host.Child.Dispose();
        }
        public System.Drawing.Rectangle GetScreen()
        {
            return new System.Drawing.Rectangle((int)Methods.Editor.SolutionState.ViewPositionX, (int)Methods.Editor.SolutionState.ViewPositionY, RenderingWidth, RenderingHeight);
        }
        public void DisposeTextures()
        {
            // Make sure to dispose the textures of the extra layers too
            Methods.Editor.Solution.CurrentTiles?.Dispose();
            if (Methods.Editor.Solution.FGHigh != null) Methods.Editor.Solution.FGHigh?.DisposeTextures();
            if (Methods.Editor.Solution.FGLow != null) Methods.Editor.Solution.FGLow?.DisposeTextures();
            if (Methods.Editor.Solution.FGHigher != null) Methods.Editor.Solution.FGHigher?.DisposeTextures();
            if (Methods.Editor.Solution.FGLower != null) Methods.Editor.Solution.FGLower?.DisposeTextures();

            if (Methods.Editor.Solution.CurrentScene != null)
            {
                foreach (var el in Methods.Editor.Solution.CurrentScene?.OtherLayers)
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
            UpdateGraphicsPanelControls();
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
            UpdateGraphicsPanelControls();
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
                if (Methods.Editor.SolutionState.UnlockCamera)
                {
                    RenderingWidth = (int)(this.ActualWidth * DPIScale);
                    RenderingHeight = (int)(this.ActualHeight * DPIScale);
                }
                else
                {
                    RenderingWidth = (int)(Methods.Editor.Solution.SceneWidth * GetZoom() * DPIScale);
                    RenderingHeight = (int)(Methods.Editor.Solution.SceneHeight * GetZoom() * DPIScale);
                }
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
            double old_zoom = Methods.Editor.SolutionState.Zoom;
            Methods.Editor.SolutionState.ZoomLevel = zoom_level;
            switch (Methods.Editor.SolutionState.ZoomLevel)
            {
                case 5: Methods.Editor.SolutionState.Zoom = 4; break;
                case 4: Methods.Editor.SolutionState.Zoom = 3; break;
                case 3: Methods.Editor.SolutionState.Zoom = 2; break;
                case 2: Methods.Editor.SolutionState.Zoom = 3 / 2.0; break;
                case 1: Methods.Editor.SolutionState.Zoom = 5 / 4.0; break;
                case 0: Methods.Editor.SolutionState.Zoom = 1; break;
                case -1: Methods.Editor.SolutionState.Zoom = 2 / 3.0; break;
                case -2: Methods.Editor.SolutionState.Zoom = 1 / 2.0; break;
                case -3: Methods.Editor.SolutionState.Zoom = 1 / 3.0; break;
                case -4: Methods.Editor.SolutionState.Zoom = 1 / 4.0; break;
                case -5: Methods.Editor.SolutionState.Zoom = 1 / 8.0; break;
            }

            Methods.Editor.SolutionState.Zooming = true;

            int oldShiftX = Methods.Editor.SolutionState.ViewPositionX;
            int oldShiftY = Methods.Editor.SolutionState.ViewPositionY;

            if (Methods.Editor.Solution.CurrentScene != null) UpdateGraphicsPanelControls();

            UpdateScrollPosXFromZoom(old_zoom, zoom_point, oldShiftX);
            UpdateScrollPosYFromZoom(old_zoom, zoom_point, oldShiftY);


            Methods.Editor.SolutionState.Zooming = false;
            Methods.Internal.UserInterface.UpdateControls();
        }
        public void ResetZoomLevel(bool resizeForm = true)
        {
            Methods.Editor.SolutionState.ZoomLevel = 1;
            this.UpdateZoomLevel((int)Methods.Editor.SolutionState.ZoomLevel, new System.Drawing.Point(0, 0));
        }
        private void UpdateScrollPosXFromZoom(double old_zoom, System.Drawing.Point zoom_point, int oldShiftX)
        {
            if (this.hScrollBar1.IsVisible)
            {
                int value = (int)((zoom_point.X + oldShiftX) / old_zoom * Methods.Editor.SolutionState.Zoom - zoom_point.X);
                if (!Methods.Editor.SolutionState.UnlockCamera) value = (int)Math.Min((this.hScrollBar1.Maximum), Math.Max(0, value));
                Methods.Editor.SolutionState.SetViewPositionX(value);
            }
        }
        private void UpdateScrollPosYFromZoom(double old_zoom, System.Drawing.Point zoom_point, int oldShiftY)
        {
            if (this.vScrollBar1.IsVisible)
            {
                int value = (int)((zoom_point.Y + oldShiftY) / old_zoom * Methods.Editor.SolutionState.Zoom - zoom_point.Y);
                if (!Methods.Editor.SolutionState.UnlockCamera) value = (int)Math.Min((this.vScrollBar1.Maximum), Math.Max(0, value));
                Methods.Editor.SolutionState.SetViewPositionY(value);
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
            GraphicPanel.AllowLoopToRender = state;
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
        public void GraphicPanel_OnResetDevice(object sender, DeviceEventArgs e) {  }
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
            if (e.Data.GetDataPresent(typeof(Int32)) && ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
            {
                System.Drawing.Point rel = GraphicPanel.PointToScreen(System.Drawing.Point.Empty);
                Methods.Editor.Solution.EditLayerA?.DragOver(new System.Drawing.Point((int)(((e.X - rel.X) + Methods.Editor.SolutionState.ViewPositionX) / Methods.Editor.SolutionState.Zoom), (int)(((e.Y - rel.Y) + Methods.Editor.SolutionState.ViewPositionY) / Methods.Editor.SolutionState.Zoom)), (ushort)Instance.TilesToolbar.SelectedTileIndex);
                GraphicPanel.Render();

            }
        }
        private void GP_DragLeave(object sender, EventArgs e)
        {
            Methods.Editor.Solution.EditLayerA?.EndDragOver(true);
            GraphicPanel.Render();
        }
        private void GP_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            Methods.Editor.Solution.EditLayerA?.EndDragOver(false);
        }
        private void GP_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Int32)) && ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit())
            {
                System.Drawing.Point rel = GraphicPanel.PointToScreen(System.Drawing.Point.Empty);
                e.Effect = System.Windows.Forms.DragDropEffects.Move;
                Methods.Editor.Solution.EditLayerA?.StartDragOver(new System.Drawing.Point((int)(((e.X - rel.X) + Methods.Editor.SolutionState.ViewPositionX) / Methods.Editor.SolutionState.Zoom), (int)(((e.Y - rel.Y) + Methods.Editor.SolutionState.ViewPositionY) / Methods.Editor.SolutionState.Zoom)), (ushort)Instance.TilesToolbar.SelectedTileIndex);
                Actions.UndoRedoModel.UpdateEditLayerActions();
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
            if (GraphicPanel.Focused) GraphicPanel_OnKeyDown(sender, e);
        }
        private void GraphicsModel_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (GraphicPanel.Focused) GraphicPanel_OnKeyUp(sender, e);
        }
        #endregion

        #region Graphics Panel Refresh/Update Methods

        bool isDisabled = true;
        public void UpdateGraphicsPanelControls()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateScalingforDPI();
                UpdateScrollBarVisibility();
                UpdateScrollViewportSize();
                UpdateScrollBarSizes();
                UpdateScrollBarMaximumValues();
                UpdateGraphicPanelViewSize();
                if (!Methods.Editor.SolutionState.UnlockCamera) SyncScrollBarsWithMaximum();
            }));
        }

        private void UpdateGraphicPanelViewSize()
        {

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
            if (Methods.Editor.SolutionState.AnyDragged)
            {
                GraphicPanel.OnMouseMoveEventCreate();
            }
            ManiacEditor.Methods.Internal.RefreshModel.RequestEntityVisiblityRefresh();
        }
        public void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (Methods.Editor.SolutionState.AnyDragged)
            {
                GraphicPanel.OnMouseMoveEventCreate();
            }
            ManiacEditor.Methods.Internal.RefreshModel.RequestEntityVisiblityRefresh();
        }
        public void HScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Methods.Editor.SolutionState.ScrollLocked) Methods.Editor.SolutionState.ScrollDirection = Axis.X;
        }
        public void VScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Methods.Editor.SolutionState.ScrollLocked) Methods.Editor.SolutionState.ScrollDirection = Axis.Y;
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
            if (Methods.Editor.SolutionState.IsSceneLoaded() && this.vScrollBar1.Track != null && this.hScrollBar1.Track != null)
            {
                if (this.vScrollBar1.Track.ViewportSize != Methods.Editor.Solution.SceneHeight) this.vScrollBar1.Track.ViewportSize = Methods.Editor.Solution.SceneHeight;
                if (this.hScrollBar1.Track.ViewportSize != Methods.Editor.Solution.SceneWidth) this.hScrollBar1.Track.ViewportSize = Methods.Editor.Solution.SceneWidth;
            }
        }
        private void UpdateScrollBarMaximumValues()
        {
            if (!Methods.Editor.SolutionState.UnlockCamera)
            {
                double height = !double.IsNaN(this.vScrollBar1.ActualHeight) ? this.vScrollBar1.ActualHeight : 0;
                double width = !double.IsNaN(this.hScrollBar1.ActualWidth) ? this.hScrollBar1.ActualWidth : 0;

                vScrollBar1.Minimum = 0;
                hScrollBar1.Minimum = 0;

                vScrollBar1.Maximum = (ManiacEditor.Methods.Editor.Solution.SceneHeight * ManiacEditor.Methods.Editor.SolutionState.Zoom) - height;
                hScrollBar1.Maximum = (ManiacEditor.Methods.Editor.Solution.SceneWidth * ManiacEditor.Methods.Editor.SolutionState.Zoom) - width;
            }
            else
            {
                vScrollBar1.Minimum = double.MinValue;
                hScrollBar1.Minimum = double.MinValue;

                vScrollBar1.Maximum = double.MaxValue;
                hScrollBar1.Maximum = double.MaxValue;

            }

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

            if (this.hScrollBar1.Maximum == 0 && !Methods.Editor.SolutionState.UnlockCamera) nhscrollbar = Visibility.Hidden;
            if (this.vScrollBar1.Maximum == 0 && !Methods.Editor.SolutionState.UnlockCamera) nvscrollbar = Visibility.Hidden;

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

            bool AboveAllMode = Methods.Editor.SolutionState.EntitiesVisibileAboveAllLayers;

            Instance.ViewPanel.InfoHUD.UpdatePopupVisibility();

            if (Instance.EntitiesToolbar?.NeedRefresh ?? false) Instance.EntitiesToolbar.PropertiesRefresh();
            if (Methods.Editor.Solution.CurrentScene != null)
            {
                DrawBackground();

                if (Methods.Editor.Solution.CurrentScene.OtherLayers.Contains(Methods.Editor.Solution.EditLayerA)) Methods.Editor.Solution.EditLayerA.Draw(GraphicPanel);

                if (!Methods.Editor.SolutionState.ExtraLayersMoveToFront) DrawExtraLayers();

                DrawLayer(Instance.EditorToolbar.ShowFGLower.IsChecked.Value, Instance.EditorToolbar.EditFGLower.IsCheckedAll, Methods.Editor.Solution.FGLower);

                DrawLayer(Instance.EditorToolbar.ShowFGLow.IsChecked.Value, Instance.EditorToolbar.EditFGLow.IsCheckedAll, Methods.Editor.Solution.FGLow);

                if (showEntities && !AboveAllMode) Methods.Editor.Solution.Entities.Draw(GraphicPanel);

                DrawLayer(Instance.EditorToolbar.ShowFGHigh.IsChecked.Value, Instance.EditorToolbar.EditFGHigh.IsCheckedAll, Methods.Editor.Solution.FGHigh);

                DrawLayer(Instance.EditorToolbar.ShowFGHigher.IsChecked.Value, Instance.EditorToolbar.EditFGHigher.IsCheckedAll, Methods.Editor.Solution.FGHigher);

                if (showEntities && AboveAllMode) Methods.Editor.Solution.Entities.Draw(GraphicPanel);

                if (Methods.Editor.SolutionState.ExtraLayersMoveToFront) DrawExtraLayers();

                if (showEntitiesEditing) Methods.Editor.Solution.Entities.Draw(GraphicPanel);

                Methods.Editor.Solution.Entities.DrawInternal(GraphicPanel);

            }

            if (Methods.Editor.SolutionState.DraggingSelection) DrawSelectionBox();
            else DrawSelectionBox(true);

            if (Methods.Editor.SolutionState.isTileDrawing && Methods.Editor.SolutionState.DrawBrushSize != 1) DrawBrushBox();

            if (Methods.Editor.SolutionState.ShowGrid && Methods.Editor.Solution.CurrentScene != null) Instance.EditBackground.DrawGrid(GraphicPanel);

            if (Methods.Editor.SolutionState.UnlockCamera) DrawSceneBounds();

            if (Methods.Runtime.GameHandler.GameRunning) DrawGameElements();

            void DrawBackground()
            {
                if (!ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit()) if (ManiacEditor.Properties.Settings.MyPerformance.HideNormalBackground == false) Instance.EditBackground.Draw(GraphicPanel);
                if (ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit()) if (ManiacEditor.Properties.Settings.MyPerformance.ShowEditLayerBackground == true) Instance.EditBackground.Draw(GraphicPanel, true);
            }

            void DrawExtraLayers()
            {
                foreach (var elb in Instance.EditorToolbar.ExtraLayerEditViewButtons)
                {
                    if (elb.Value.IsCheckedAll || elb.Key.IsCheckedAll)
                    {
                        int index = Instance.EditorToolbar.ExtraLayerEditViewButtons.IndexOf(elb);
                        var _extraViewLayer = Methods.Editor.Solution.CurrentScene.OtherLayers.ElementAt(index);
                        _extraViewLayer.Draw(GraphicPanel);
                    }
                }
            }

            void DrawSceneBounds()
            {
                int x1 = 0;
                int x2 = Methods.Editor.Solution.SceneWidth;
                int y1 = 0;
                int y2 = Methods.Editor.Solution.SceneHeight;


                GraphicPanel.DrawLine(x1, y1, x1, y2, System.Drawing.Color.White);
                GraphicPanel.DrawLine(x1, y1, x2, y1, System.Drawing.Color.White);
                GraphicPanel.DrawLine(x2, y2, x1, y2, System.Drawing.Color.White);
                GraphicPanel.DrawLine(x2, y2, x2, y1, System.Drawing.Color.White);
            }

            void DrawSelectionBox(bool resetSelection = false)
            {
                if (!resetSelection)
                {
                    int bound_x1 = (int)(Methods.Editor.SolutionState.RegionX2 / Methods.Editor.SolutionState.Zoom); int bound_x2 = (int)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom);
                    int bound_y1 = (int)(Methods.Editor.SolutionState.RegionY2 / Methods.Editor.SolutionState.Zoom); int bound_y2 = (int)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom);
                    if (bound_x1 != bound_x2 && bound_y1 != bound_y2)
                    {
                        if (bound_x1 > bound_x2)
                        {
                            bound_x1 = (int)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom);
                            bound_x2 = (int)(Methods.Editor.SolutionState.RegionX2 / Methods.Editor.SolutionState.Zoom);
                        }
                        if (bound_y1 > bound_y2)
                        {
                            bound_y1 = (int)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom);
                            bound_y2 = (int)(Methods.Editor.SolutionState.RegionY2 / Methods.Editor.SolutionState.Zoom);
                        }
                        if (ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit())
                        {
                            bound_x1 = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(bound_x1, bound_y1).X;
                            bound_y1 = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(bound_x1, bound_y1).Y;
                            bound_x2 = Classes.Scene.EditorLayer.GetChunkCoordinatesBottomEdge(bound_x2, bound_y2).X;
                            bound_y2 = Classes.Scene.EditorLayer.GetChunkCoordinatesBottomEdge(bound_x2, bound_y2).Y;
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
                    Methods.Editor.SolutionState.TempSelectX1 = 0; Methods.Editor.SolutionState.TempSelectX2 = 0; Methods.Editor.SolutionState.TempSelectY1 = 0; Methods.Editor.SolutionState.TempSelectY2 = 0;
                }
            }

            void DrawBrushBox()
            {

                int offset = (Methods.Editor.SolutionState.DrawBrushSize / 2) * Methods.Editor.EditorConstants.TILE_SIZE;
                int x1 = (int)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom) - offset;
                int x2 = (int)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom) + offset;
                int y1 = (int)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom) - offset;
                int y2 = (int)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom) + offset;


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

            void DrawLayer(bool ShowLayer, bool EditLayer, Classes.Scene.EditorLayer layer)
            {
                if (layer != null)
                {
                    if (ShowLayer || EditLayer) layer.Draw(GraphicPanel);
                }
            }

            void DrawGameElements()
            {
                Methods.Runtime.GameHandler.DrawGameElements(GraphicPanel);

                if (Methods.Runtime.GameHandler.PlayerSelected) Methods.Runtime.GameHandler.MovePlayer(new System.Drawing.Point(Methods.Editor.SolutionState.LastX, Methods.Editor.SolutionState.LastY), Methods.Editor.SolutionState.Zoom, Methods.Runtime.GameHandler.SelectedPlayer);
                if (Methods.Runtime.GameHandler.CheckpointSelected)
                {
                    System.Drawing.Point clicked_point = new System.Drawing.Point((int)(Methods.Editor.SolutionState.LastX / Methods.Editor.SolutionState.Zoom), (int)(Methods.Editor.SolutionState.LastY / Methods.Editor.SolutionState.Zoom));
                    Methods.Runtime.GameHandler.UpdateCheckpoint(clicked_point);
                }
            }
        }

        #endregion

    }
}
