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

namespace ManiacEditor.Controls.Dialog
{
    /// <summary>
    /// Interaction logic for AsyncWaitingBox.xaml
    /// </summary>
    public partial class AsyncWaitingBox : Window
    {
        public bool CanClose { get; set; } = false;
        public AsyncWaitingBox()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!CanClose) e.Cancel = true;
        }
    }
}
