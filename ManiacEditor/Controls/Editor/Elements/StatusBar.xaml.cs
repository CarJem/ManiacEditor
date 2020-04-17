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

namespace ManiacEditor.Controls.Editor.Elements
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
                    if (!Methods.Solution.SolutionState.CountTilesSelectedInPixels) text = "X: " + (int)(e.X / Methods.Solution.SolutionState.Zoom) + " Y: " + (int)(e.Y / Methods.Solution.SolutionState.Zoom);
                    else text = "X: " + (int)((e.X / Methods.Solution.SolutionState.Zoom) / 16) + " Y: " + (int)((e.Y / Methods.Solution.SolutionState.Zoom) / 16);
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

                    if (Methods.Solution.SolutionState.CountTilesSelectedInPixels == false)
                    {
                        selectedPositionLabel.Content = "Selected Tile Position: X: " + (int)Methods.Solution.SolutionState.SelectedTileX + ", Y: " + (int)Methods.Solution.SolutionState.SelectedTileY;
                        selectedPositionLabel.ToolTip = "The Position of the Selected Tile";
                    }
                    else
                    {
                        selectedPositionLabel.Content = "Selected Tile Pixel Position: " + "X: " + (int)Methods.Solution.SolutionState.SelectedTileX * 16 + ", Y: " + (int)Methods.Solution.SolutionState.SelectedTileY * 16;
                        selectedPositionLabel.ToolTip = "The Pixel Position of the Selected Tile";
                    }
                    if (Methods.Solution.SolutionState.CountTilesSelectedInPixels == false)
                    {
                        selectionSizeLabel.Content = "Amount of Tiles in Selection: " + (Methods.Solution.SolutionState.SelectedTilesCount);
                        selectionSizeLabel.ToolTip = "The Size of the Selection";
                    }
                    else
                    {
                        selectionSizeLabel.Content = "Length of Pixels in Selection: " + Methods.Solution.SolutionState.SelectedTilesCount * 16;
                        selectionSizeLabel.ToolTip = "The Length of all the Tiles (by Pixels) in the Selection";
                    }

                    selectionBoxSizeLabel.Content = "Selection Box Size: X: " + (Methods.Solution.SolutionState.TempSelectX2 - Methods.Solution.SolutionState.TempSelectX1) + ", Y: " + (Methods.Solution.SolutionState.TempSelectY2 - Methods.Solution.SolutionState.TempSelectY1);

                    scrollLockDirLabel.Content = "Scroll Direction: " + (Methods.Solution.SolutionState.ScrollDirection == (int)Axis.X ? "X" : "Y") + (Methods.Solution.SolutionState.ScrollLocked ? " (Locked)" : "");


                    hVScrollBarXYLabel.Content = "Zoom Value: " + Methods.Solution.SolutionState.Zoom.ToString();
                });

                StatusPanelUpdateOpteration = this.Dispatcher.InvokeAsync(action, DispatcherPriority.SystemIdle);
            }
        }
        #endregion



        public void QuickButtonClickEvent(object sender, RoutedEventArgs e)
        {
            if (sender == MoreSettingsButton)
            {
                switch (Methods.Solution.SolutionState.LastQuickButtonState)
                {
                    case 1:
                        ManiacEditor.Methods.Solution.SolutionActions.SetScrollLockDirection();
                        break;
                    case 2:
                        Methods.Solution.SolutionState.ApplyEditEntitiesTransparency ^= true;
                        break;
                    case 3:
                        ManiacEditor.Methods.Solution.SolutionActions.SwapEncoreManiaEntityVisibility();
                        break;
                    default:
                        Methods.Solution.SolutionState.LastQuickButtonState = 1;
                        ManiacEditor.Methods.Solution.SolutionActions.SetScrollLockDirection();
                        break;
                }
            }
            else if (sender == QuickSwapScrollDirection)
            {
                Methods.Solution.SolutionState.LastQuickButtonState = 1;
                ManiacEditor.Methods.Solution.SolutionActions.SetScrollLockDirection();
            }
            else if (sender == QuickSwapEncoreManiaEntitVisibility)
            {
                Methods.Solution.SolutionState.LastQuickButtonState = 3;
                ManiacEditor.Methods.Solution.SolutionActions.SwapEncoreManiaEntityVisibility();
            }
            else if (sender == QuickEditEntitiesTransparentLayers)
            {
                Methods.Solution.SolutionState.LastQuickButtonState = 2;
                Methods.Solution.SolutionState.ApplyEditEntitiesTransparency ^= true;
            }

        }
        private void FilterButtonOpenContextMenuEvent(object sender, RoutedEventArgs e) { FilterButton.ContextMenu.IsOpen = true; }
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
                nudgeFasterButton.ToolTip = "Move entities/tiles in a larger increment. (Configurable in Options)\r\nShortcut Key: " + KeyBindPraser("NudgeFaster");
                scrollLockButton.ToolTip = "Prevent the Mouse Wheel from Scrolling with the vertical scroll bar\r\nShortcut Key: " + KeyBindPraser("ScrollLock");
                QuickSwapScrollDirection.InputGestureText = KeyBindPraser("ScrollLockTypeSwitch", false, true);
            }));

        }

        public string KeyBindPraser(string keyRefrence, bool tooltip = false, bool nonRequiredBinding = false)
        {
            string nullString = (nonRequiredBinding ? "" : "N/A");
            if (nonRequiredBinding && tooltip) nullString = "None";
            List<string> keyBindList = new List<string>();
            List<string> keyBindModList = new List<string>();

            if (!Extensions.Extensions.KeyBindsSettingExists(keyRefrence)) return nullString;
           
            if (Properties.Settings.MyKeyBinds == null) return nullString;

            var keybindDict = Properties.Settings.MyKeyBinds.GetInput(keyRefrence) as List<string>;
            if (keybindDict != null)
            {
                keyBindList = keybindDict.Cast<string>().ToList();
            }
            else
            {
                return nullString;
            }

            if (keyBindList == null)
            {
                return nullString;
            }

            if (keyBindList.Count > 1)
            {
                string keyBindLister = "";
                foreach (string key in keyBindList)
                {
                    keyBindLister += String.Format("({0}) ", key);
                }
                if (tooltip) return String.Format(" ({0})", keyBindLister);
                else return keyBindLister;
            }
            else if ((keyBindList.Count == 1) && keyBindList[0] != "None")
            {
                if (tooltip) return String.Format(" ({0})", keyBindList[0]);
                else return keyBindList[0];
            }
            else
            {
                return nullString;
            }


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
        private void TogglePixelModeEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.CountTilesSelectedInPixels ^= true; }
        public void ToggleScrollLockEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.ScrollLocked ^= true; }
        public void ToggleFasterNudgeEvent(object sender, RoutedEventArgs e) { Methods.Solution.SolutionState.EnableFasterNudge ^= true; }
    }
}
