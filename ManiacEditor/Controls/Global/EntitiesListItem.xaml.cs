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

namespace ManiacEditor.Controls.Global
{
    /// <summary>
    /// Interaction logic for EntitiesListItem.xaml
    /// </summary>
    public partial class EntitiesListItem : UserControl
    {
        public event RoutedEventHandler Click;
        public event RoutedEventHandler ClickUp;
        public event RoutedEventHandler ClickDown;

        public EntitiesListItem()
        {
            InitializeComponent();
        }

        public new object Content
        {
            get
            {
                return MainButton.Content;
            }
            set
            {
                MainButton.Content = value;
            }
        }
        public new Brush Foreground
        {
            get
            {
                return MainButton.Foreground;
            }
            set
            {
                MainButton.Foreground = value;
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
