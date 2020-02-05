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

namespace ManiacEditor.Controls.Base.Elements
{
    /// <summary>
    /// Interaction logic for ViewPanel.xaml
    /// </summary>
    public partial class ViewPanel : UserControl
    {
        private MainEditor Instance { get; set; } = null;
        public ViewPanel()
        {
            InitializeComponent();

            this.Loaded += ViewPanel_Loaded;

            this.ViewPanelContextMenu.Foreground = (SolidColorBrush)FindResource("NormalText");
            this.ViewPanelContextMenu.Background = (SolidColorBrush)FindResource("NormalBackground");
        }

        public void UpdateInstance(MainEditor editor)
        {
            Instance = editor;
            editor.LocationChanged += Editor_LocationChanged;
            editor.SizeChanged += Editor_SizeChanged;
        }

        private void LeftToolbarToolbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Instance != null)
            {
                if (Instance.StartScreen.Visibility != Visibility.Visible)
                {
                    if (Instance.EditorViewPanel.LeftToolbarToolbox.SelectedIndex == 0)
                    {
                        Methods.Internal.UserInterface.UpdateToolbars(false, false);
                    }
                    else
                    {
                        Methods.Internal.UserInterface.UpdateToolbars(false, true);
                    }
                    ManiacEditor.Controls.Base.MainEditor.Instance.Editor_Resize(null, null);
                }
            }


        }

        #region Splitter Events
        private void Spliter_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Instance != null) Instance.DeviceModel.GraphicsResize(sender, e);
        }
        private void Spliter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Instance != null) Instance.DeviceModel.SetZoomLevel(Classes.Editor.SolutionState.ZoomLevel, new System.Drawing.Point(Classes.Editor.SolutionState.ViewPositionX, Classes.Editor.SolutionState.ViewPositionY), 0.0, false);
        }
        #endregion

        #region Cleaned Regions

        #region Context Menu Events
        private void TileManiacEditTileEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.TileManiacIntergration(); }

        #endregion

        #region Game Running Context Menu Events
        private void MoveThePlayerToHere(object sender, RoutedEventArgs e) { Methods.GameHandler.MoveThePlayerToHere(); }
        private void SetPlayerRespawnToHere(object sender, RoutedEventArgs e) { Methods.GameHandler.SetPlayerRespawnToHere(); }
        private void MoveCheckpoint(object sender, RoutedEventArgs e) { Methods.GameHandler.CheckpointSelected = true; }
        private void RemoveCheckpoint(object sender, RoutedEventArgs e) { Methods.GameHandler.UpdateCheckpoint(new System.Drawing.Point(0, 0), false); }
        private void AssetReset(object sender, RoutedEventArgs e) { Methods.GameHandler.AssetReset(); }
        private void RestartScene(object sender, RoutedEventArgs e) { Methods.GameHandler.RestartScene(); }
        #endregion

        #endregion

        #region Debug HUD Events and Methods
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


        private void Editor_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePopupVisibility();
        }

        private void Editor_LocationChanged(object sender, EventArgs e)
        {
            UpdatePopupVisibility();
        }

        public void UpdatePopupVisibility()
        {
            ViewPanelHUD.CustomPopupPlacementCallback = placePopup;

            if (IsUserVisible(ViewPanelForm) && Classes.Editor.SolutionState.IsSceneLoaded())
            {
                ViewPanelHUD.IsOpen = true;
            }
            else ViewPanelHUD.IsOpen = false;

            if (!double.IsNaN(ViewPanelHUD.Height)) DebugHUD.Height = ViewPanelHUD.Height;
            if (!double.IsNaN(ViewPanelHUD.Width)) DebugHUD.Width = ViewPanelHUD.Width;

            ViewPanelHUD.VerticalOffset = DebugHUD.ActualHeight;
            var offset = ViewPanelHUD.HorizontalOffset;
            // "bump" the offset to cause the popup to reposition itself on its own
            ViewPanelHUD.HorizontalOffset = offset + 1;
            ViewPanelHUD.HorizontalOffset = offset;

        }

        public CustomPopupPlacement[] placePopup(Size popupSize,
                                           Size targetSize,
                                           Point offset)
        {
            CustomPopupPlacement placement1 =
               new CustomPopupPlacement(new Point(0, 0), PopupPrimaryAxis.Vertical);

            /*
            CustomPopupPlacement placement2 =
                new CustomPopupPlacement(new Point(10, 20), PopupPrimaryAxis.Horizontal);*/

            CustomPopupPlacement[] ttplaces = new CustomPopupPlacement[] { placement1 };
            return ttplaces;
        }

        private void ViewPanel_Loaded(object sender, RoutedEventArgs e)
        {
            ViewPanelHUD.CustomPopupPlacementCallback = placePopup;
            ViewPanelHUD.PlacementTarget = ViewPanelForm;
            ViewPanelHUD.Placement = PlacementMode.AbsolutePoint;
            ViewPanelHUD.VerticalOffset = DebugHUD.ActualHeight;
            Window w = Window.GetWindow(ViewPanelForm);
            // w should not be Null now!
            if (null != w)
            {
                w.FocusableChanged += delegate (object sender2, DependencyPropertyChangedEventArgs args)
                {
                    UpdatePopupVisibility();
                };
                w.IsVisibleChanged += delegate (object sender2, DependencyPropertyChangedEventArgs args)
                {
                    UpdatePopupVisibility();
                };
                w.LocationChanged += delegate (object sender2, EventArgs args)
                {
                    UpdatePopupVisibility();
                };
                // Also handle the window being resized (so the popup's position stays
                //  relative to its target element if the target element moves upon 
                //  window resize)
                w.SizeChanged += delegate (object sender3, SizeChangedEventArgs e2)
                {
                    UpdatePopupVisibility();
                };
            }
            UpdatePopupVisibility();
        }

        private void ViewPanelHUD_Closed(object sender, EventArgs e)
        {

        }

        private void ViewPanelHUD_Opened(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
