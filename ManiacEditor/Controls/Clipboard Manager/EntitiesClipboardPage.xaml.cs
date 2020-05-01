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
    /// Interaction logic for EntitiesClipboardPage.xaml
    /// </summary>
    public partial class EntitiesClipboardPage : UserControl
    {
        public EntitiesClipboardPage()
        {
            InitializeComponent();
            this.DataContext = Methods.Solution.SolutionClipboard.ClipboardViewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecentEntitiesList.SelectedIndex == -1) return;
            SolutionClipboard.ClipboardViewModel.RemoveFromObjectsHistory(SolutionClipboard.ClipboardViewModel.ObjectsClipboardHistory[RecentEntitiesList.SelectedIndex]);
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecentEntitiesList.SelectedIndex == -1) return;
            SolutionClipboard.ClipboardViewModel.SetObjectClipboard(SolutionClipboard.ClipboardViewModel.ObjectsClipboardHistory[RecentEntitiesList.SelectedIndex]);
        }


    }
}
