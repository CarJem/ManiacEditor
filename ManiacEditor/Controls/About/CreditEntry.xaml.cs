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
using System.Diagnostics;

namespace ManiacEditor.Controls.About
{
    /// <summary>
    /// Interaction logic for CreditEntry.xaml
    /// </summary>
    public partial class CreditEntry : UserControl
    {
        public CreditEntry()
        {
            InitializeComponent();
        }

        public void SetCreditsEntry(string title, string url, string message = "", string url2 = "", string url3 = "", string url4 = "")
        {
            MainLinkText.Text = title;
            MainHyperLink.NavigateUri = new Uri(url);
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Visibility = Visibility.Visible;
                MessageBox.Text = " - (" + message + ")";
            }
            else MessageBox.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(url2))
            {
                HyperLinkHost2.Visibility = Visibility.Visible;
                HyperLink2.NavigateUri = new Uri(url2);
            }
            if (!string.IsNullOrEmpty(url3))
            {
                HyperLinkHost3.Visibility = Visibility.Visible;
                HyperLink3.NavigateUri = new Uri(url3);
            }
            if (!string.IsNullOrEmpty(url4))
            {
                HyperLinkHost4.Visibility = Visibility.Visible;
                HyperLink4.NavigateUri = new Uri(url4);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
