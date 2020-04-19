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

namespace ManiacEditor.Controls.Toolbox
{
    /// <summary>
    /// Interaction logic for OffsetTilesUtility.xaml
    /// </summary>
    public partial class OffsetTilesTool : Window
    {
        public int IndexOffsetAmount { get; private set; }
        public OffsetTilesTool()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IndexOffsetAmount = IndexOffsetValue.Value.Value;
            DialogResult = true;
        }
    }
}
