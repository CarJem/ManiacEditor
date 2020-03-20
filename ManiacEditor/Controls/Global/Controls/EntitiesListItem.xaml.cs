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
    /// Interaction logic for EntitiesListItem.xaml
    /// </summary>
    public partial class EntitiesListItem : UserControl
    {
        public event RoutedEventHandler Click;
        public event RoutedEventHandler ClickUp;
        public event RoutedEventHandler ClickDown;

        public static readonly Brush DefaultForeText = Brushes.White;

        public EntitiesListItem()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ButtonContent =
DependencyProperty.Register("SelfContent", typeof(string), typeof(EntitiesListItem), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty ButtonForeground =
DependencyProperty.Register("SelfForeground", typeof(Brush), typeof(EntitiesListItem), new UIPropertyMetadata(DefaultForeText));
        public string SelfContent
        {
            get 
            { 
                return (string)GetValue(ButtonContent); 
            }
            set 
            { 
                SetValue(ButtonContent, value); 
            }
        }
        public Brush SelfForeground
        {
            get
            {
                return (Brush)GetValue(ButtonForeground);
            }
            set
            {
                SetValue(ButtonForeground, value);
            }
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            this.Click(this, e);
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClickUp(this, e);
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClickDown(this, e);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.UpDownButtonsCol.Width = new GridLength(25);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.UpDownButtonsCol.Width = new GridLength(0);
        }
    }
}
