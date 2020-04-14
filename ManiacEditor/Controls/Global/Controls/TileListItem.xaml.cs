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
    public partial class TileListItem : UserControl
    {
        public event RoutedEventHandler Click;
        public event RoutedEventHandler ClickUp;
        public event RoutedEventHandler ClickDown;

        public TileListItem()
        {
            InitializeComponent();
        }

        public TileListItem(ImageSource _source, int _size, Transform _transform)
        {
            InitializeComponent();
            Source = _source;
            ImageSize = _size;
            ImageRenderTransform = _transform;
        }


        public void Update()
        {
            ViewContent.Source = Source;
            ViewContent.Width = ImageSize;
            ViewContent.Height = ImageSize;
            ViewContent.RenderTransform = ImageRenderTransform;
        }

        private ImageSource _Source;
        public ImageSource Source
        {
            get
            {
                return _Source;
            }
            set
            {
                _Source = value;
                ViewContent.Source = value;
            }
        }

        private int _ImageSize;
        public int ImageSize
        {
            get
            {
                return _ImageSize;
            }
            set
            {
                ViewContent.Width = value;
                ViewContent.Height = value;
                _ImageSize = value;
            }
        }

        private Transform _ImageRenderTransform;
        public Transform ImageRenderTransform
        {
            get
            {
                return _ImageRenderTransform;
            }
            set
            {
                ViewContent.RenderTransform = value;
                _ImageRenderTransform = value;
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

        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {

        }
    }
}
