using System.Windows;
using System.Windows.Controls;

namespace ManiacEditor.Controls.Misc.Configuration
{
    /// <summary>
    /// Interaction logic for CheatCodeManager.xaml
    /// </summary>
    public partial class CheatCodeManager : Window
    {
        public CheatCodeManager()
        {
            InitializeComponent();
        }

        private void button13_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = "Select SonicMania.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Properties.Settings.MyDefaults.SonicManiaPath = ofd.FileName;
        }

        private void button14_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = "Select ManiaModManager.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Properties.Settings.MyDefaults.ModLoaderPath = ofd.FileName;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Classes.Options.GeneralSettings.Save();
            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void VersionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VersionSelector.SelectedIndex != -1)
            {
                Methods.Runtime.GameHandler.SelectedGameVersion = Methods.Runtime.GameHandler.GameVersion[VersionSelector.SelectedIndex];
            }
        }
    }
}
