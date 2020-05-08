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
using System.Collections.Specialized;
using ManiacEditor.Enums;
using System.Windows.Threading;

namespace ManiacEditor.Controls.Editor_Elements
{
    /// <summary>
    /// Interaction logic for StatusBar.xaml
    /// </summary>
    public partial class StatusBar : UserControl
    {

        public StatusBar()
        {
            InitializeComponent();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                UpdatePositionLabel();

                this.EntityContext.Foreground = (SolidColorBrush)FindResource("NormalText");
                this.EntityContext.Background = (SolidColorBrush)FindResource("NormalBackground");

                this.TilesContext.Foreground = (SolidColorBrush)FindResource("NormalText");
                this.TilesContext.Background = (SolidColorBrush)FindResource("NormalBackground");
            }
        }
        #region Update Position Label / Status Panel

        private DispatcherOperation CurrentPositionUpdateOperation { get; set; }
        private DispatcherOperation StatusPanelUpdateOpteration { get; set; }
        public void UpdatePositionLabel(System.Windows.Forms.MouseEventArgs m = null)
        {
            if (CurrentPositionUpdateOperation != null && CurrentPositionUpdateOperation.Status != DispatcherOperationStatus.Completed) return;
            else
            {
                var action = new Action(() =>
                {
                    System.Drawing.Point e;
                    if (m != null) e = m.Location;
                    else e = new System.Drawing.Point(0, 0);

                    string text;
                    if (!Methods.Solution.SolutionState.Main.CountTilesSelectedInPixels) text = "X: " + (int)(e.X)  + " Y: " + (int)(e.Y);
                    else text = "X: " + (int)(e.X / 16) + " Y: " + (int)(e.Y / 16);
                    positionLabel.Content = text;
                });
                CurrentPositionUpdateOperation = positionLabel.Dispatcher.InvokeAsync(action, DispatcherPriority.SystemIdle);
            }




        }
        public void UpdateStatusPanel()
        {
            if (StatusPanelUpdateOpteration != null && StatusPanelUpdateOpteration.Status != DispatcherOperationStatus.Completed) return;
            else
            {
                var action = new Action(() =>
                {
                    LevelID_Label.Content = "Level ID: " + Methods.Solution.CurrentSolution.LevelID.ToString();

                    if (seperator1.Visibility != Visibility.Visible) seperator1.Visibility = Visibility.Visible;
                    if (seperator2.Visibility != Visibility.Visible) seperator2.Visibility = Visibility.Visible;
                    if (seperator3.Visibility != Visibility.Visible) seperator3.Visibility = Visibility.Visible;
                    if (seperator4.Visibility != Visibility.Visible) seperator4.Visibility = Visibility.Visible;
                    if (seperator5.Visibility != Visibility.Visible) seperator5.Visibility = Visibility.Visible;
                    if (seperator6.Visibility != Visibility.Visible) seperator6.Visibility = Visibility.Visible;
                    if (seperator7.Visibility != Visibility.Visible) seperator7.Visibility = Visibility.Visible;

                    if (Methods.Solution.SolutionState.Main.CountTilesSelectedInPixels == false)
                    {
                        selectedPositionLabel.Content = "Selected Tile Position: X: " + (int)Methods.Solution.SolutionState.Main.SelectedTileX + ", Y: " + (int)Methods.Solution.SolutionState.Main.SelectedTileY;
                        selectedPositionLabel.ToolTip = "The Position of the Selected Tile";
                    }
                    else
                    {
                        selectedPositionLabel.Content = "Selected Tile Pixel Position: " + "X: " + (int)Methods.Solution.SolutionState.Main.SelectedTileX * 16 + ", Y: " + (int)Methods.Solution.SolutionState.Main.SelectedTileY * 16;
                        selectedPositionLabel.ToolTip = "The Pixel Position of the Selected Tile";
                    }
                    if (Methods.Solution.SolutionState.Main.CountTilesSelectedInPixels == false)
                    {
                        selectionSizeLabel.Content = "Amount of Tiles in Selection: " + (Methods.Solution.SolutionState.Main.SelectedTilesCount);
                        selectionSizeLabel.ToolTip = "The Size of the Selection";
                    }
                    else
                    {
                        selectionSizeLabel.Content = "Length of Pixels in Selection: " + Methods.Solution.SolutionState.Main.SelectedTilesCount * 16;
                        selectionSizeLabel.ToolTip = "The Length of all the Tiles (by Pixels) in the Selection";
                    }

                    selectionBoxSizeLabel.Content = "Selection Box Size: X: " + (Methods.Solution.SolutionState.Main.TempSelectX2 - Methods.Solution.SolutionState.Main.TempSelectX1) + ", Y: " + (Methods.Solution.SolutionState.Main.TempSelectY2 - Methods.Solution.SolutionState.Main.TempSelectY1);

                    scrollLockDirLabel.Content = "Scroll Direction: " + (Methods.Solution.SolutionState.Main.ScrollDirection == (int)Axis.X ? "X" : "Y") + (Methods.Solution.SolutionState.Main.ScrollLocked ? " (Locked)" : "");


                    hVScrollBarXYLabel.Content = "Zoom Value: " + Methods.Solution.SolutionState.Main.Zoom.ToString();
                });

                StatusPanelUpdateOpteration = this.Dispatcher.InvokeAsync(action, DispatcherPriority.SystemIdle);
            }
        }
        #endregion



        public void QuickButtonClickEvent(object sender, RoutedEventArgs e)
        {
            if (sender == MoreSettingsButton)
            {
                switch (Methods.Solution.SolutionState.Main.LastQuickButtonState)
                {
                    case 1:
                        ManiacEditor.Methods.Solution.SolutionActions.SetScrollLockDirection();
                        break;
                    case 2:
                        Methods.Solution.SolutionState.Main.ApplyEditEntitiesTransparency ^= true;
                        break;
                    case 3:
                        ManiacEditor.Methods.Solution.SolutionActions.SwapEncoreManiaEntityVisibility();
                        break;
                    default:
                        Methods.Solution.SolutionState.Main.LastQuickButtonState = 1;
                        ManiacEditor.Methods.Solution.SolutionActions.SetScrollLockDirection();
                        break;
                }
            }
            else if (sender == QuickSwapScrollDirection)
            {
                Methods.Solution.SolutionState.Main.LastQuickButtonState = 1;
                ManiacEditor.Methods.Solution.SolutionActions.SetScrollLockDirection();
            }
            else if (sender == QuickSwapEncoreManiaEntitVisibility)
            {
                Methods.Solution.SolutionState.Main.LastQuickButtonState = 3;
                ManiacEditor.Methods.Solution.SolutionActions.SwapEncoreManiaEntityVisibility();
            }
            else if (sender == QuickEditEntitiesTransparentLayers)
            {
                Methods.Solution.SolutionState.Main.LastQuickButtonState = 2;
                Methods.Solution.SolutionState.Main.ApplyEditEntitiesTransparency ^= true;
            }

        }
        private void FilterButtonOpenContextMenuEvent(object sender, RoutedEventArgs e) 
        {
            var btn = (sender as Button);
            if (!btn.ContextMenu.IsOpen)
            {
                //Open the Context menu when Button is clicked
                btn.ContextMenu.IsEnabled = true;
                btn.ContextMenu.PlacementTarget = (sender as Button);
                btn.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                btn.ContextMenu.IsOpen = true;
            }
        }
        private void FilterCheckChangedEvent(object sender, RoutedEventArgs e)
        {
            if (Methods.Solution.CurrentSolution.Entities != null) Classes.Scene.EditorEntities.ObjectRefreshNeeded = true;
        }

        public void SetSceneOnlyButtonsState(bool enabled)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.FilterButton.IsEnabled = enabled;
                this.MoreSettingsButton.IsEnabled = enabled;
                this.StatusBarQuickButtons.IsEnabled = enabled;
            }));
        }


        public void UpdateTooltips()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                positionLabel.ToolTip = "The position relative to your mouse (Pixels Only for Now)";
                selectionSizeLabel.ToolTip = "The Size of the Selection";
                selectedPositionLabel.ToolTip = "The Position of the Selected Tile";
                selectionBoxSizeLabel.ToolTip = "The Size of the Selection Box";
                EnablePixelModeButton.ToolTip = "Change the Positional/Selection Values to Pixel or Tile Based Values";
                nudgeFasterButton.ToolTip = "Move entities/tiles in a larger increment. (Configurable in Options)\r\nShortcut Key: " + "Ctrl + F1";
                scrollLockButton.ToolTip = "Prevent the Mouse Wheel from Scrolling with the vertical scroll bar\r\nShortcut Key: " + "Ctrl + F2";
                QuickSwapScrollDirection.InputGestureText = ("Ctrl + F3");
            }));

        }

        public void UpdateFilterButtonApperance()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                maniaFilterCheck.Foreground = Methods.Internal.Theming.GetObjectFilterColorBrush(2);
                encoreFilterCheck.Foreground = Methods.Internal.Theming.GetObjectFilterColorBrush(4);
                otherFilterCheck.Foreground = Methods.Internal.Theming.GetObjectFilterColorBrush(0);
                bothFilterCheck.Foreground = Methods.Internal.Theming.GetObjectFilterColorBrush(1);
                pinballFilterCheck.Foreground = Methods.Internal.Theming.GetObjectFilterColorBrush(255);

                if (Properties.Settings.MySettings.UseBitOperators)
                {
                    maniaFilterCheck.Header = "Mania (0b0010)";
                    encoreFilterCheck.Header = "Encore (0b0100)";
                    otherFilterCheck.Header = "Other (0b0000)";
                    bothFilterCheck.Header = "Both (0b0001)";
                    pinballFilterCheck.Header = "All (0b11111111)";
                }
                else
                {
                    maniaFilterCheck.Header = "Mania (2)";
                    encoreFilterCheck.Header = "Encore (4)";
                    otherFilterCheck.Header = "Other (0)";
                    bothFilterCheck.Header = "Both (1 & 5)";
                    pinballFilterCheck.Header = "All (255)";
                }
            }));

        }

        private void TileManiacEditTileEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.TileManiacIntergration(); }
        private void TogglePixelModeEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.CountTilesSelectedInPixels ^= true; }
        public void ToggleScrollLockEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.ScrollLocked ^= true; }
        public void ToggleFasterNudgeEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.Main.EnableFasterNudge ^= true; }
    }
}
