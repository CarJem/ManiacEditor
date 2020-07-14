using IWshRuntimeLibrary;
using ManiacEditor.Actions;
using ManiacEditor.Controls;
using ManiacEditor.Controls.Editor;
using ManiacEditor.Controls.Global;
using ManiacEditor.Entity_Renders;
using ManiacEditor.Enums;
using ManiacEditor.Enums;
using ManiacEditor.Events;
using ManiacEditor.Extensions;
using Microsoft.Win32;
using RSDKv5;
using SharpDX.Direct3D9;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ManiacEditor.Methods.Drawing;


namespace ManiacEditor.Controls.Editor
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
            this.UpdateZoomLevel((int)Methods.Solution.SolutionState.Main.ZoomLevel, new System.Drawing.Point(0, 0));
        }
        public void SetupGraphicsPanel()
        {
            this.GraphicPanel = new ManiacEditor.DevicePanel();
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


        private int GetOffsetX()
        {
            return 0;
        }

        private int GetOffsetY()
        {
            return 0;
        }

        public SFML.System.Vector2i GetPosition()
        {
            return new SFML.System.Vector2i((int)(Methods.Solution.SolutionState.Main.ViewPositionX), (int)(Methods.Solution.SolutionState.Main.ViewPositionY));
        }
        public float GetZoom()
        {
            return (float)Methods.Solution.SolutionState.Main.Zoom;
        }
        public new void Dispose()
        {
            this.GraphicPanel.Dispose();
            this.GraphicPanel = null;
            this.Host.Child.Dispose();
        }
        public System.Drawing.Rectangle GetScreen()
        {
            return new System.Drawing.Rectangle((int)(Methods.Solution.SolutionState.Main.ViewPositionX), (int)(Methods.Solution.SolutionState.Main.ViewPositionY), RenderingWidth, RenderingHeight);
        }


        #endregion

        #region DPI/Resize Events

        private void Host_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            ManiacEditor.Extensions.ConsoleExtensions.Print("DPI Change Detected!");
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
                if (Methods.Solution.SolutionState.Main.UnlockCamera)
                {
                    RenderingWidth = (int)(this.ActualWidth * DPIScale);
                    RenderingHeight = (int)(this.ActualHeight * DPIScale);
                }
                else
                {
                    RenderingWidth = (int)(Methods.Solution.CurrentSolution.SceneWidth * GetZoom() * DPIScale);
                    RenderingHeight = (int)(Methods.Solution.CurrentSolution.SceneHeight * GetZoom() * DPIScale);
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
            double old_zoom = Methods.Solution.SolutionState.Main.Zoom;
            Methods.Solution.SolutionState.Main.ZoomLevel = zoom_level;
            switch (Methods.Solution.SolutionState.Main.ZoomLevel)
            {
                case 5: Methods.Solution.SolutionState.Main.Zoom = 4; break;
                case 4: Methods.Solution.SolutionState.Main.Zoom = 3; break;
                case 3: Methods.Solution.SolutionState.Main.Zoom = 2; break;
                case 2: Methods.Solution.SolutionState.Main.Zoom = 3 / 2.0; break;
                case 1: Methods.Solution.SolutionState.Main.Zoom = 5 / 4.0; break;
                case 0: Methods.Solution.SolutionState.Main.Zoom = 1; break;
                case -1: Methods.Solution.SolutionState.Main.Zoom = 2 / 3.0; break;
                case -2: Methods.Solution.SolutionState.Main.Zoom = 1 / 2.0; break;
                case -3: Methods.Solution.SolutionState.Main.Zoom = 1 / 3.0; break;
                case -4: Methods.Solution.SolutionState.Main.Zoom = 1 / 4.0; break;
                case -5: Methods.Solution.SolutionState.Main.Zoom = 1 / 8.0; break;
            }

            Methods.Solution.SolutionState.Main.Zooming = true;

            int oldShiftX = Methods.Solution.SolutionState.Main.ViewPositionX;
            int oldShiftY = Methods.Solution.SolutionState.Main.ViewPositionY;

            if (Methods.Solution.CurrentSolution.CurrentScene != null) UpdateGraphicsPanelControls();

            UpdateScrollPosXFromZoom(old_zoom, zoom_point, oldShiftX);
            UpdateScrollPosYFromZoom(old_zoom, zoom_point, oldShiftY);


            Methods.Solution.SolutionState.Main.Zooming = false;
            Methods.Internal.UserInterface.UpdateControls();
        }
        public void ResetZoomLevel(bool resizeForm = true)
        {
            Methods.Solution.SolutionState.Main.ZoomLevel = 1;
            this.UpdateZoomLevel((int)Methods.Solution.SolutionState.Main.ZoomLevel, new System.Drawing.Point(0, 0));
        }
        private void UpdateScrollPosXFromZoom(double old_zoom, System.Drawing.Point zoom_point, int oldShiftX)
        {
            if (this.hScrollBar1.IsVisible)
            {
                int value = (int)((zoom_point.X + oldShiftX) / old_zoom * Methods.Solution.SolutionState.Main.Zoom - zoom_point.X);
                if (!Methods.Solution.SolutionState.Main.UnlockCamera) value = (int)Math.Min((this.hScrollBar1.Maximum), Math.Max(0, value));
                Methods.Solution.SolutionState.Main.SetViewPositionX(value, true);
                this.hScrollBar1.Value = value;
            }
        }
        private void UpdateScrollPosYFromZoom(double old_zoom, System.Drawing.Point zoom_point, int oldShiftY)
        {
            if (this.vScrollBar1.IsVisible)
            {
                int value = (int)((zoom_point.Y + oldShiftY) / old_zoom * Methods.Solution.SolutionState.Main.Zoom - zoom_point.Y);
                if (!Methods.Solution.SolutionState.Main.UnlockCamera) value = (int)Math.Min((this.vScrollBar1.Maximum), Math.Max(0, value));
                Methods.Solution.SolutionState.Main.SetViewPositionY(value, true);
                this.vScrollBar1.Value = value;
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
            if (e.Data.GetDataPresent(typeof(Int32)) && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit())
            {
                Methods.Solution.CurrentSolution.EditLayerA?.DragOver(GraphicPanel.GetRelativeMousePosition(), (ushort)Instance.TilesToolbar.SelectedTileIndex);
                GraphicPanel.Render();

            }
        }
        private void GP_DragLeave(object sender, EventArgs e)
        {
            Methods.Solution.CurrentSolution.EditLayerA?.EndDragOver(true);
            GraphicPanel.Render();
        }
        private void GP_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            Methods.Solution.CurrentSolution.EditLayerA?.EndDragOver(false);
        }
        private void GP_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Int32)) && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit())
            {
                e.Effect = System.Windows.Forms.DragDropEffects.Move;
                Methods.Solution.CurrentSolution.EditLayerA?.StartDragOver(GraphicPanel.GetRelativeMousePosition(), (ushort)Instance.TilesToolbar.SelectedTileIndex);
                Actions.UndoRedoModel.UpdateEditLayersActions();
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
            UpdateScalingforDPI();
            UpdateScrollBars();
        }

        #endregion

        #endregion

        #region ScrollBars

        #region ScrollBar Events
        public void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            vScrollBar1.Refresh();
        }
        public void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            hScrollBar1.Refresh();
        }
        public void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            vScrollBar1.Refresh();
            Methods.Solution.SolutionState.Main.SetViewPositionY((int)(vScrollBar1.Value), false);
            if (Methods.Solution.SolutionState.Main.AnyDragged())
            {
                GraphicPanel.OnMouseMoveEventCreate();
            }
            ManiacEditor.Methods.Drawing.ObjectDrawing.RequestEntityVisiblityRefresh();
        }
        public void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            hScrollBar1.Refresh();
            Methods.Solution.SolutionState.Main.SetViewPositionX((int)(hScrollBar1.Value), false);
            if (Methods.Solution.SolutionState.Main.AnyDragged())
            {
                GraphicPanel.OnMouseMoveEventCreate();
            }
            ManiacEditor.Methods.Drawing.ObjectDrawing.RequestEntityVisiblityRefresh();
        }
        public void HScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Methods.Solution.SolutionState.Main.ScrollLocked) Methods.Solution.SolutionState.Main.ScrollDirection = Axis.X;
        }
        public void VScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Methods.Solution.SolutionState.Main.ScrollLocked) Methods.Solution.SolutionState.Main.ScrollDirection = Axis.Y;
        }
        #endregion

        #region ScrollBar Update Methods
        private void UpdateScrollBars()
        {
            double zoom = ManiacEditor.Methods.Solution.SolutionState.Main.Zoom;

            double h_width = !double.IsNaN(this.hScrollBar1.ActualWidth) ? this.hScrollBar1.ActualWidth : 0;
            double v_height = !double.IsNaN(this.vScrollBar1.ActualHeight) ? this.vScrollBar1.ActualHeight : 0;

            int scene_width = (int)(ManiacEditor.Methods.Solution.CurrentSolution.SceneWidth * zoom);
            int scene_height = (int)(ManiacEditor.Methods.Solution.CurrentSolution.SceneHeight * zoom);

            int h_large = (int)(5 * ManiacEditor.Methods.Solution.SolutionConstants.TILE_SIZE * zoom);
            int v_large = (int)(5 * ManiacEditor.Methods.Solution.SolutionConstants.TILE_SIZE * zoom);

            int h_small = (int)(1 * ManiacEditor.Methods.Solution.SolutionConstants.TILE_SIZE * zoom);
            int v_small = (int)(1 * ManiacEditor.Methods.Solution.SolutionConstants.TILE_SIZE * zoom);

            double maxY = scene_height - (v_height);
            double maxX = scene_width - (h_width);

            if (maxX < 0) maxX = 0;
            if (maxY < 0) maxY = 0;

            Visibility nvscrollbar = Visibility.Visible;
            Visibility nhscrollbar = Visibility.Visible;

            if (this.hScrollBar1.Maximum == 0 && !Methods.Solution.SolutionState.Main.UnlockCamera) nhscrollbar = Visibility.Hidden;
            if (this.vScrollBar1.Maximum == 0 && !Methods.Solution.SolutionState.Main.UnlockCamera) nvscrollbar = Visibility.Hidden;

            if (vScrollBar1.Visibility != nvscrollbar) vScrollBar1.Visibility = nvscrollbar;
            if (hScrollBar1.Visibility != nhscrollbar) hScrollBar1.Visibility = nhscrollbar;

            /*
            if (vScrollBar1.LargeChange != v_large) vScrollBar1.LargeChange = v_large;
            if (hScrollBar1.LargeChange != h_large) hScrollBar1.LargeChange = h_large;

            if (vScrollBar1.SmallChange != v_small) vScrollBar1.SmallChange = v_small;
            if (hScrollBar1.SmallChange != h_small) hScrollBar1.SmallChange = h_small;
            */

            if (Methods.Solution.SolutionState.Main.IsSceneLoaded() && this.vScrollBar1.Track != null && this.hScrollBar1.Track != null)
            {
                if (vScrollBar1.ViewportSize != maxY) vScrollBar1.ViewportSize = maxY;
                if (hScrollBar1.ViewportSize != maxX) hScrollBar1.ViewportSize = maxX;
            }

            if (!Methods.Solution.SolutionState.Main.UnlockCamera)
            {
                if (vScrollBar1.Maximum != maxY) vScrollBar1.Maximum = maxY;
                if (hScrollBar1.Maximum != maxX) hScrollBar1.Maximum = maxX;
            }
            else
            {
                vScrollBar1.Minimum = double.MinValue;
                hScrollBar1.Minimum = double.MinValue;

                vScrollBar1.Maximum = double.MaxValue;
                hScrollBar1.Maximum = double.MaxValue;

            }
        }
        #endregion

        #endregion

        #region Rendering

        private void GP_Render()
        {
            bool showEntities = Instance.EditorToolbar.ShowEntities.IsChecked.Value && !Instance.EditorToolbar.EditEntities.IsCheckedAll;
            bool showEntitiesEditing = Instance.EditorToolbar.EditEntities.IsCheckedAll;

            bool AboveAllMode = Methods.Solution.SolutionState.Main.EntitiesVisibileAboveAllLayers;

            //Instance.ViewPanel.InfoHUD.UpdatePopupVisibility();
            //Instance.ViewPanel.InfoHUD.UpdateHUDInfo();

            if (Instance.EntitiesToolbar?.NeedRefresh ?? false) Instance.EntitiesToolbar.PropertiesRefresh();
            if (Methods.Solution.CurrentSolution.CurrentScene != null)
            {
                CommonDrawing.DrawBackground(GraphicPanel);

                if (Methods.Solution.CurrentSolution.CurrentScene.OtherLayers.Contains(Methods.Solution.CurrentSolution.EditLayerA)) Methods.Solution.CurrentSolution.EditLayerA.Draw(GraphicPanel);

                if (!Methods.Solution.SolutionState.Main.ExtraLayersMoveToFront) CommonDrawing.DrawExtraLayers(GraphicPanel);

                CommonDrawing.DrawLayer(GraphicPanel, Instance.EditorToolbar.ShowFGLower.IsChecked.Value, Instance.EditorToolbar.EditFGLower.IsCheckedAll, Methods.Solution.CurrentSolution.FGLower);

                CommonDrawing.DrawLayer(GraphicPanel, Instance.EditorToolbar.ShowFGLow.IsChecked.Value, Instance.EditorToolbar.EditFGLow.IsCheckedAll, Methods.Solution.CurrentSolution.FGLow);

                if (showEntities && !AboveAllMode) Methods.Solution.CurrentSolution.Entities.Draw(GraphicPanel);

                CommonDrawing.DrawLayer(GraphicPanel, Instance.EditorToolbar.ShowFGHigh.IsChecked.Value, Instance.EditorToolbar.EditFGHigh.IsCheckedAll, Methods.Solution.CurrentSolution.FGHigh);

                CommonDrawing.DrawLayer(GraphicPanel, Instance.EditorToolbar.ShowFGHigher.IsChecked.Value, Instance.EditorToolbar.EditFGHigher.IsCheckedAll, Methods.Solution.CurrentSolution.FGHigher);

                if (showEntities && AboveAllMode) Methods.Solution.CurrentSolution.Entities.Draw(GraphicPanel);

                if (Methods.Solution.SolutionState.Main.ExtraLayersMoveToFront) CommonDrawing.DrawExtraLayers(GraphicPanel);

                if (CommonDrawing.CanOverlayImage(Instance)) CommonDrawing.DrawOverlayImage(GraphicPanel);

                if (showEntitiesEditing) Methods.Solution.CurrentSolution.Entities.Draw(GraphicPanel);

                Methods.Solution.CurrentSolution.Entities.DrawInternal(GraphicPanel);

            }

            if (Methods.Solution.SolutionState.Main.DraggingSelection) CommonDrawing.DrawSelectionBox(GraphicPanel);
            else CommonDrawing.DrawSelectionBox(GraphicPanel, true);

            if (Methods.Solution.SolutionState.Main.IsDrawMode() && Methods.Solution.SolutionState.Main.IsTilesEdit()) CommonDrawing.DrawBrushBox(GraphicPanel, true);

            if (Methods.Solution.SolutionState.Main.isTileDrawing && Methods.Solution.SolutionState.Main.DrawBrushSize != 1) CommonDrawing.DrawBrushBox(GraphicPanel);

            if (Methods.Solution.SolutionState.Main.ShowGrid && Methods.Solution.CurrentSolution.CurrentScene != null) Instance.EditBackground.DrawGrid(GraphicPanel);

            if (Methods.Solution.SolutionState.Main.UnlockCamera) CommonDrawing.DrawSceneBounds(GraphicPanel);

            if (Methods.Runtime.GameHandler.GameRunning) CommonDrawing.DrawGameElements(GraphicPanel);
        }



        #endregion
    }
}
