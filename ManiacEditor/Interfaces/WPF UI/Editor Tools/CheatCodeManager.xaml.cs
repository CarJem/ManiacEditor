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
using System.Windows.Shapes;

namespace ManiacEditor
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
                Properties.Settings.Default.RunGamePath = ofd.FileName;
        }

        private void button14_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = "Select ManiaModManager.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Properties.Settings.Default.RunModLoaderPath = ofd.FileName;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Settings.mySettings.Save();
            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
