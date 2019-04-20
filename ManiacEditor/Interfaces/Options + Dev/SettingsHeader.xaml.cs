using System.Windows;
using System.Windows.Controls;

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
