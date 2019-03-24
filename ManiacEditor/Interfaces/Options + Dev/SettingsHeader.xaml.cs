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

namespace ManiacEditor
{
    /// <summary>
    /// Interaction logic for SettingsHeader.xaml
    /// </summary>
    public partial class SettingsHeader : UserControl
    {
        public SettingsHeader()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string HeaderName
        {
            get { return (string)GetValue(TextBlockTextProperty); }
            set { SetValue(TextBlockTextProperty, value); }
        }

        public static readonly DependencyProperty TextBlockTextProperty =
DependencyProperty.Register("Text", typeof(string), typeof(SettingsHeader), new UIPropertyMetadata(""));
    }
}
