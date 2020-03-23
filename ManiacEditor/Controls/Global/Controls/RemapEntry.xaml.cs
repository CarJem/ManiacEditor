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

namespace ManiacEditor.Controls.Global.Controls
{
    /// <summary>
    /// Interaction logic for RemapEntry.xaml
    /// </summary>
    public partial class RemapEntry : UserControl
    {
        public event RoutedEventHandler Click;
        public RemapEntry()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string HeaderName
        {
            get
            {
                return (string)GetValue(TextBlockTextProperty);
            }
            set
            {
                SetValue(TextBlockTextProperty, value);
            }
        }

        public string KeybindTag
        {
            get
            {
                return (string)GetValue(KeybindTagTextProperty);
            }
            set
            {
                SetValue(KeybindTagTextProperty, value);
            }
        }

        public static readonly DependencyProperty KeybindTagTextProperty =
DependencyProperty.Register("KeybindTag", typeof(string), typeof(RemapEntry), new UIPropertyMetadata(""));

        public static readonly DependencyProperty TextBlockTextProperty =
DependencyProperty.Register("HeaderName", typeof(string), typeof(RemapEntry), new UIPropertyMetadata(""));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.Click != null)
            {
                this.BindingButton.Tag = KeybindTag;
                this.Click(sender, e);
            }
        }
    }
}
