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
        #region Update Position Label

        private DispatcherOperation CurrentPositionUpdateOperation { get; set; }
        public void UpdatePositionLabel(System.Windows.Forms.MouseEventArgs m = null)
        {
            var action = new Action(() =>
            {
                System.Drawing.Point e;
                if (m != null) e = m.Location;
                else e = new System.Drawing.Point(0, 0);

                string text;
                if (!Methods.Editor.SolutionState.CountTilesSelectedInPixels) text = "X: " + (int)(e.X / Methods.Editor.SolutionState.Zoom) + " Y: " + (int)(e.Y / Methods.Editor.SolutionState.Zoom);
                else text = "X: " + (int)((e.X / Methods.Editor.SolutionState.Zoom) / 16) + " Y: " + (int)((e.Y / Methods.Editor.SolutionState.Zoom) / 16);
                positionLabel.Content = text;
            });

            if (CurrentPositionUpdateOperation != null && CurrentPositionUpdateOperation.Status != DispatcherOperationStatus.Completed)
            {
                CurrentPositionUpdateOperation.Abort();
            }

            CurrentPositionUpdateOperation = positionLabel.Dispatcher.InvokeAsync(action, DispatcherPriority.SystemIdle);


        }
        #endregion

        public void UpdateStatusPanel()
        {
            //
            // Tooltip Bar Info 
            //

            _levelIDLabel.Content = "Level ID: " + Methods.Editor.SolutionState.LevelID.ToString();
            seperator1.Visibility = Visibility.Visible;
            seperator2.Visibility = Visibility.Visible;
            seperator3.Visibility = Visibility.Visible;
            seperator4.Visibility = Visibility.Visible;
            seperator5.Visibility = Visibility.Visible;
            seperator6.Visibility = Visibility.Visible;
            seperator7.Visibility = Visibility.Visible;
            //seperator8.Visibility = Visibility.Visible;
            //seperator9.Visibility = Visibility.Visible;

            if (Methods.Editor.SolutionState.CountTilesSelectedInPixels == false)
            {
                selectedPositionLabel.Content = "Selected Tile Position: X: " + (int)Methods.Editor.SolutionState.SelectedTileX + ", Y: " + (int)Methods.Editor.SolutionState.SelectedTileY;
                selectedPositionLabel.ToolTip = "The Position of the Selected Tile";
            }
            else
            {
                selectedPositionLabel.Content = "Selected Tile Pixel Position: " + "X: " + (int)Methods.Editor.SolutionState.SelectedTileX * 16 + ", Y: " + (int)Methods.Editor.SolutionState.SelectedTileY * 16;
                selectedPositionLabel.ToolTip = "The Pixel Position of the Selected Tile";
            }
            if (Methods.Editor.SolutionState.CountTilesSelectedInPixels == false)
            {
                selectionSizeLabel.Content = "Amount of Tiles in Selection: " + (Methods.Editor.SolutionState.SelectedTilesCount - Methods.Editor.SolutionState.DeselectTilesCount);
                selectionSizeLabel.ToolTip = "The Size of the Selection";
            }
            else
            {
                selectionSizeLabel.Content = "Length of Pixels in Selection: " + (Methods.Editor.SolutionState.SelectedTilesCount - Methods.Editor.SolutionState.DeselectTilesCount) * 16;
                selectionSizeLabel.ToolTip = "The Length of all the Tiles (by Pixels) in the Selection";
            }

            selectionBoxSizeLabel.Content = "Selection Box Size: X: " + (Methods.Editor.SolutionState.TempSelectX2 - Methods.Editor.SolutionState.TempSelectX1) + ", Y: " + (Methods.Editor.SolutionState.TempSelectY2 - Methods.Editor.SolutionState.TempSelectY1);

            scrollLockDirLabel.Content = "Scroll Direction: " + (Methods.Editor.SolutionState.ScrollDirection == (int)Axis.X ? "X" : "Y") + (Methods.Editor.SolutionState.ScrollLocked ? " (Locked)" : "");


            hVScrollBarXYLabel.Content = "Zoom Value: " + Methods.Editor.SolutionState.Zoom.ToString();

            //
            // End of Tooltip Bar Info Section
            //
        }

        public void QuickButtonClickEvent(object sender, RoutedEventArgs e)
        {
            if (sender == MoreSettingsButton)
            {
                switch (Methods.Editor.SolutionState.LastQuickButtonState)
                {
                    case 1:
                        ManiacEditor.Methods.Editor.EditorActions.SetScrollLockDirection();
                        break;
                    case 2:
                        Methods.Editor.SolutionState.ApplyEditEntitiesTransparency ^= true;
                        break;
                    case 3:
                        ManiacEditor.Methods.Editor.EditorActions.SwapEncoreManiaEntityVisibility();
                        break;
                    default:
                        Methods.Editor.SolutionState.LastQuickButtonState = 1;
                        ManiacEditor.Methods.Editor.EditorActions.SetScrollLockDirection();
                        break;
                }
            }
            else if (sender == QuickSwapScrollDirection)
            {
                Methods.Editor.SolutionState.LastQuickButtonState = 1;
                ManiacEditor.Methods.Editor.EditorActions.SetScrollLockDirection();
            }
            else if (sender == QuickSwapEncoreManiaEntitVisibility)
            {
                Methods.Editor.SolutionState.LastQuickButtonState = 3;
                ManiacEditor.Methods.Editor.EditorActions.SwapEncoreManiaEntityVisibility();
            }
            else if (sender == QuickEditEntitiesTransparentLayers)
            {
                Methods.Editor.SolutionState.LastQuickButtonState = 2;
                Methods.Editor.SolutionState.ApplyEditEntitiesTransparency ^= true;
            }

        }
        private void FilterButtonOpenContextMenuEvent(object sender, RoutedEventArgs e) { FilterButton.ContextMenu.IsOpen = true; }
        private void FilterCheckChangedEvent(object sender, RoutedEventArgs e)
        {
            if (Methods.Editor.Solution.Entities != null) Methods.Editor.Solution.Entities.FilterRefreshNeeded = true;
        }

        public void UpdateTooltips()
        {
            positionLabel.ToolTip = "The position relative to your mouse (Pixels Only for Now)";
            selectionSizeLabel.ToolTip = "The Size of the Selection";
            selectedPositionLabel.ToolTip = "The Position of the Selected Tile";
            selectionBoxSizeLabel.ToolTip = "The Size of the Selection Box";
            EnablePixelModeButton.ToolTip = "Change the Positional/Selection Values to Pixel or Tile Based Values";
            nudgeFasterButton.ToolTip = "Move entities/tiles in a larger increment. (Configurable in Options)\r\nShortcut Key: " + KeyBindPraser("NudgeFaster");
            scrollLockButton.ToolTip = "Prevent the Mouse Wheel from Scrolling with the vertical scroll bar\r\nShortcut Key: " + KeyBindPraser("ScrollLock");
            QuickSwapScrollDirection.InputGestureText = KeyBindPraser("ScrollLockTypeSwitch", false, true);
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

        public void UpdateFilterButtonApperance(bool startup)
        {
            if (startup)
            {
                maniaFilterCheck.Foreground = Methods.Internal.Theming.GetObjectFilterColorBrush(2);
                encoreFilterCheck.Foreground = Methods.Internal.Theming.GetObjectFilterColorBrush(4);
                otherFilterCheck.Foreground = Methods.Internal.Theming.GetObjectFilterColorBrush(0);
                bothFilterCheck.Foreground = Methods.Internal.Theming.GetObjectFilterColorBrush(1);
                pinballFilterCheck.Foreground = Methods.Internal.Theming.GetObjectFilterColorBrush(255);
            }
            if (Properties.Settings.MySettings.UseBitOperators)
            {
                maniaFilterCheck.Content = "Mania (0b0010)";
                encoreFilterCheck.Content = "Encore (0b0100)";
                otherFilterCheck.Content = "Other (0b0000)";
                bothFilterCheck.Content = "Both (0b0001)";
                pinballFilterCheck.Content = "All (0b11111111)";
            }
            else
            {
                maniaFilterCheck.Content = "Mania (2)";
                encoreFilterCheck.Content = "Encore (4)";
                otherFilterCheck.Content = "Other (0)";
                bothFilterCheck.Content = "Both (1 & 5)";
                pinballFilterCheck.Content = "All (255)";
            }
        }

        private void TileManiacEditTileEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.TileManiacIntergration(); }
        private void TogglePixelModeEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.CountTilesSelectedInPixels ^= true; }
        public void ToggleScrollLockEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.ScrollLocked ^= true; }
        public void ToggleFasterNudgeEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.EnableFasterNudge ^= true; }
    }
}
