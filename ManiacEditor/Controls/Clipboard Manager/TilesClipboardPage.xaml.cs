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
using ManiacEditor.Methods.Solution;

namespace ManiacEditor.Controls.Clipboard_Manager
{
    /// <summary>
    /// Interaction logic for TilesClipboardPage.xaml
    /// </summary>
    public partial class TilesClipboardPage : UserControl
    {


        public TilesClipboardPage()
        {
            InitializeComponent();
            this.DataContext = Methods.Solution.SolutionClipboard.ClipboardViewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecentTilesList.SelectedIndex == -1) return;
            SolutionClipboard.ClipboardViewModel.RemoveFromTilesHistory(SolutionClipboard.ClipboardViewModel.TilesClipboardHistory[RecentTilesList.SelectedIndex]);
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecentTilesList.SelectedIndex == -1) return;
            SolutionClipboard.ClipboardViewModel.SetTileClipboard(SolutionClipboard.ClipboardViewModel.TilesClipboardHistory[RecentTilesList.SelectedIndex]);
        }
    }
}
