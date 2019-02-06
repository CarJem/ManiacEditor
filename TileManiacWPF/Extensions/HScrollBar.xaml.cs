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
using System.Windows.Controls.Primitives;

namespace TileManiacWPF
{
    /// <summary>
    /// Interaction logic for HScrollBar.xaml
    /// </summary>
    public partial class HScrollBar : UserControl
    {
        public HScrollBar()
        {
            InitializeComponent();
        }
	}
	public static class ScrollBarExtensions
	{
		static void SetViewportSize(ScrollBar bar, double size)
		{
			var max = (bar.Maximum - bar.Minimum);
			bar.ViewportSize = size / (max - size) * max;
			bar.IsEnabled = (bar.ViewportSize >= 0 &&
				bar.ViewportSize != double.PositiveInfinity);
			InvalidateScrollBar(bar);
		}

		static void InvalidateScrollBar(ScrollBar bar)
		{
			var v = bar.Value;
			bar.Value = (bar.Value == bar.Maximum) ? bar.Minimum : bar.Maximum;
			bar.Value = v;
		}
	}
}
