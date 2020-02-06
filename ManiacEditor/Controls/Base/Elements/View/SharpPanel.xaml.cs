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

namespace ManiacEditor.Controls.Base.Elements.View
{
    /// <summary>
    /// Interaction logic for SharpPanel.xaml
    /// </summary>
    public partial class SharpPanel : UserControl
    {
        public SharpPanel()
        {
            InitializeComponent();

            this.ViewPanelContextMenu.Foreground = (SolidColorBrush)FindResource("NormalText");
            this.ViewPanelContextMenu.Background = (SolidColorBrush)FindResource("NormalBackground");
        }

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
    }
}
