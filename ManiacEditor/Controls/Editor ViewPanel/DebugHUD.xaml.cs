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
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Diagnostics;
using ManiacEditor.Controls.Editor;

namespace ManiacEditor.Controls.Editor_ViewPanel
{
    /// <summary>
    /// Interaction logic for DebugHUD.xaml
    /// </summary>
    public partial class DebugHUD : UserControl
    {
        private MainEditor Instance { get; set; }
        private System.Windows.Forms.Timer t { get; set; }
        public DebugHUD()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                t = new System.Windows.Forms.Timer();
                t.Tick += T_Tick;
            }

        }

        private void T_Tick(object sender, EventArgs e)
        {
            UpdateHUDInfo();
        }

        public void UpdateInstance(MainEditor editor)
        {
            Instance = editor;
            InitilizeHUD();
        }

        private void InitilizeHUD()
        {
            ViewPanelHUD.Placement = PlacementMode.AbsolutePoint;
            Window w = Window.GetWindow(Instance.ViewPanel.SharpPanel);
            if (null != w)
            {
                w.FocusableChanged += delegate (object sender2, DependencyPropertyChangedEventArgs args)
                {
                    UpdatePopupSize();
                };
                w.IsVisibleChanged += delegate (object sender2, DependencyPropertyChangedEventArgs args)
                {
                    UpdatePopupSize();
                };
                w.LocationChanged += delegate (object sender2, EventArgs args)
                {
                    UpdatePopupSize();
                };
                w.SizeChanged += delegate (object sender3, SizeChangedEventArgs e2)
                {
                    UpdatePopupSize();
                };
            }
            UpdatePopupSize();
        }

        private bool HUDItemNeedsUpdate(string item1, string item2)
        {
            if (item1 == item2) return false;
            else return true;
        }

        private void UpdateHUDInfo()
        {
            if (Instance != null && !System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (HUDItemNeedsUpdate(FPSCounter.Text, GetFPS())) FPSCounter.Text = GetFPS();
                if (HUDItemNeedsUpdate(MemoryCounter.Text, GetMemoryUsage())) MemoryCounter.Text = GetMemoryUsage();
                if (HUDItemNeedsUpdate(PhysicalMemoryUsage.Text, GetPhysicalMemoryUsage())) PhysicalMemoryUsage.Text = GetPhysicalMemoryUsage();

                if (HUDItemNeedsUpdate(DataFolder.Text, GetDataFolder())) DataFolder.Text = GetDataFolder();
                if (HUDItemNeedsUpdate(MasterDataFolder.Text, GetMasterDataFolder())) MasterDataFolder.Text = GetMasterDataFolder();
                if (HUDItemNeedsUpdate(ScenePath.Text, GetScenePath())) ScenePath.Text = GetScenePath();
                if (HUDItemNeedsUpdate(SceneFilePath.Text, GetSceneFilePath())) SceneFilePath.Text = GetSceneFilePath();
                if (HUDItemNeedsUpdate(ZoomValue.Text, GetZoom())) ZoomValue.Text = GetZoom();
                if (HUDItemNeedsUpdate(SetupObject.Text, GetSetupObject())) SetupObject.Text = GetSetupObject();
                if (HUDItemNeedsUpdate(SelectedZone.Text, GetSelectedZone())) SelectedZone.Text = GetSelectedZone();

                string infoToggle = string.Format("Use {0} to Toggle this Information", Extensions.KeyEventExts.KeyBindPraser("StatusBoxToggle"));
                if (HUDItemNeedsUpdate(InfoToggle.Text, infoToggle)) InfoToggle.Text = infoToggle;
            }
        }


        #region Debug HUD Information

        public string GetSceneTileConfigPath()
        {
            if (ManiacEditor.Methods.Solution.SolutionPaths.TileConfig_Source != null && ManiacEditor.Methods.Solution.SolutionPaths.TileConfig_Source.SourcePath != "") return "Scene TileConfig Path: " + System.IO.Path.Combine(ManiacEditor.Methods.Solution.SolutionPaths.TileConfig_Source.SourcePath, "TileConfig.bin").ToString();
            else return "Scene TileConfig Path: N/A";
        }

        public string GetFPS()
        {
            try
            {
                int fps = (int)Instance.ViewPanel.SharpPanel.GraphicPanel.FPS;
                return fps.ToString();
            }
            catch
            {
                return "FF";
            }

        }

        public string GetMemoryUsage()
        {
            Process proc = Process.GetCurrentProcess();
            long memory = proc.PrivateMemorySize64;
            double finalMem = ConvertBytesToMegabytes(memory);
            return ((int)finalMem).ToString();
        }

        public string GetPhysicalMemoryUsage()
        {
            Process proc = Process.GetCurrentProcess();
            long memoryWorkSet = proc.WorkingSet64;
            double finalMem = ConvertBytesToMegabytes(memoryWorkSet);
            return "Physical Memory Usage: " + finalMem.ToString() + " MB";
        }

        public string GetDeviceType()
        {
            Process proc = Process.GetCurrentProcess();
            long memoryWorkSet = proc.WorkingSet64;
            double finalMem = ConvertBytesToMegabytes(memoryWorkSet);
            return "Physical Memory Usage: " + finalMem.ToString() + " MB";
        }

        public string GetDevicePramaters()
        {
            Process proc = Process.GetCurrentProcess();
            long memoryWorkSet = proc.WorkingSet64;
            double finalMem = ConvertBytesToMegabytes(memoryWorkSet);
            return "Physical Memory Usage: " + finalMem.ToString() + " MB";
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        public string GetZoom()
        {
            return Math.Round(Methods.Solution.SolutionState.Zoom, 2).ToString();
        }

        public string GetSelectedZone()
        {
            if (ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone != null && ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone != "") return "Selected Zone: " + ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.Zone;
            else return "Selected Zone: N/A";
        }

        public string GetSceneFilePath()
        {
            if (ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source != null && ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourcePath != "") return "Scene File: " + ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourcePath;
            else return "Scene File: N/A";
        }

        public string GetScenePath()
        {

            if (ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source != null && ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourceDirectory != "") return "Scene Path: " + ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourceDirectory;
            else return "Scene Path: N/A";
        }

        public string GetDataFolder()
        {   
            if (Instance != null)
            {
                if (ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory != null && ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory != "") return "Data Directory: " + ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.DataDirectory;
                else return "Data Directory: N/A";
            }
            return "Data Directory: N/A";

        }
        public string GetMasterDataFolder()
        {
            if (ManiacEditor.Methods.Solution.SolutionPaths.DefaultMasterDataDirectory != null && ManiacEditor.Methods.Solution.SolutionPaths.DefaultMasterDataDirectory != "") return "Master Data Directory: " + ManiacEditor.Methods.Solution.SolutionPaths.CurrentSceneData.MasterDataDirectory;
            else return "Master Data Directory: N/A";
        }

        public string GetSetupObject()
        {
            if (Methods.Solution.CurrentSolution.Entities != null && Methods.Solution.CurrentSolution.Entities.SetupObject != null && Methods.Solution.CurrentSolution.Entities.SetupObject != "")
            {
                return "Setup Object: " + Methods.Solution.CurrentSolution.Entities.SetupObject;
            }
            else
            {
                return "Setup Object: N/A";
            }

        }

        public string GetPosition()
        {
            int x = (int)((Methods.Solution.SolutionState.ViewPositionX / Methods.Solution.SolutionState.Zoom) / 16);
            int y = (int)((Methods.Solution.SolutionState.ViewPositionY / Methods.Solution.SolutionState.Zoom) / 16);
            return string.Format("Position: {0}, {1}", x, y);
        }

        #endregion

        #region Debug HUD Events and Methods

        #region P/Invoke imports & definitions

        private const int HWND_TOPMOST = -1;
        private const int HWND_NOTOPMOST = -2;
        private const int HWND_BOTTOM = 1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int GW_HWNDPREV = 3;
        private const int GW_HWNDLAST = 1;


        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion

        private void UpdatePosition(Popup item, int x, int y, int width, int height)
        {
            if (item.IsOpen)
            {
                IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(item.Child)).Handle;
                SetWindowPos(hwnd, HWND_NOTOPMOST, x, y, width, height, SWP_NOSIZE | SWP_NOACTIVATE);
            }


        }

        private bool IsUserVisible(UIElement element)
        {
            if (!element.IsVisible)
                return false;
            var container = VisualTreeHelper.GetParent(element) as FrameworkElement;
            if (container == null) throw new ArgumentNullException("container");

            Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.RenderSize.Width, element.RenderSize.Height));
            Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.IntersectsWith(bounds);
        }

        public void UpdatePopupVisibility()
        {
            if (IsUserVisible(Instance.ViewPanel.SharpPanel) && Methods.Solution.SolutionState.IsSceneLoaded() && Methods.Solution.SolutionState.DebugStatsVisibleOnPanel)
            {
                ViewPanelHUD.IsOpen = true;
                UpdatePopupSize();
            }
            else ViewPanelHUD.IsOpen = false;
        }

        public void UpdatePopupSize()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ViewPanelHUD.IsOpen)
                {
                    int scrollWidth = double.IsNaN(Instance.ViewPanel.SharpPanel.vScrollBar1.ActualWidth) ? 0 : (int)Instance.ViewPanel.SharpPanel.vScrollBar1.ActualWidth;
                    int scrollHeight = double.IsNaN(Instance.ViewPanel.SharpPanel.hScrollBar1.ActualHeight) ? 0 : (int)Instance.ViewPanel.SharpPanel.hScrollBar1.ActualHeight;

                    int desiredWidth = (int)Instance.ViewPanel.SharpPanel.ActualWidth - scrollWidth - 10;
                    int desiredHeight = (int)Instance.ViewPanel.SharpPanel.ActualHeight - scrollHeight - 10;

                    if (ViewPanelHUD.MaxWidth != desiredWidth || ViewPanelHUD.MaxHeight != desiredHeight)
                    {
                        //Display.MaxWidth = (Instance.ViewPanel.SharpPanel.ActualWidth > 0 ? desiredWidth : 0);
                        //Display.MaxHeight = (Instance.ViewPanel.SharpPanel.ActualHeight > 0 ? desiredHeight : 0);

                        //Display.MinWidth = (Instance.ViewPanel.SharpPanel.ActualWidth > 0 ? desiredWidth : 0);
                        //Display.MinHeight = (Instance.ViewPanel.SharpPanel.ActualHeight > 0 ? desiredHeight : 0);

                        ViewPanelHUD.Width = (Instance.ViewPanel.SharpPanel.ActualWidth > 0 ? desiredWidth : 0);
                        ViewPanelHUD.Height = (Instance.ViewPanel.SharpPanel.ActualHeight > 0 ? desiredHeight : 0);

                        ViewPanelHUD.MaxWidth = (Instance.ViewPanel.SharpPanel.ActualWidth > 0 ? desiredWidth : 0);
                        ViewPanelHUD.MaxHeight = (Instance.ViewPanel.SharpPanel.ActualHeight > 0 ? desiredHeight : 0);

                        ViewPanelHUD.MinWidth = (Instance.ViewPanel.SharpPanel.ActualWidth > 0 ? desiredWidth : 0);
                        ViewPanelHUD.MinHeight = (Instance.ViewPanel.SharpPanel.ActualHeight > 0 ? desiredHeight : 0);
                    }

                    if (Instance.ViewPanel.SharpPanel.IsLoaded)
                    {
                        try
                        {
                            Point relativePoint = Instance.ViewPanel.SharpPanel.PointToScreen(new Point(0, 0));
                            UpdatePosition(ViewPanelHUD, (int)relativePoint.X, (int)relativePoint.Y, (int)Instance.ViewPanel.SharpPanel.Width, (int)Instance.ViewPanel.SharpPanel.Height);
                            ViewPanelHUD.AllowsTransparency = true;
                        }
                        catch { }

                    }
                }
            }));
        }

        private void ViewPanelHUD_Closed(object sender, EventArgs e)
        {
            UpdatePopupSize();
            if (t != null) t.Stop();
        }

        private void ViewPanelHUD_Opened(object sender, EventArgs e)
        {
            UpdatePopupSize();
            if (t != null) t.Start();
        }

        #endregion
    }
}
