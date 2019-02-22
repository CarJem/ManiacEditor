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
using MessageBox = RSDKrU.MessageBox;

namespace ManiacEditor.Interfaces
{
	/// <summary>
	/// Interaction logic for GoToPositionWindow.xaml
	/// </summary>
	public partial class GoToPositionBox : Window
	{
		public bool tilesMode = false;
		public int goTo_X = 0;
		public int goTo_Y = 0;
		public GoToPositionBox()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			if (TileModeCheckbox.IsChecked.Value)
			{
				tilesMode = true;
			}
			goTo_X = (int)X.Value;
			goTo_Y = (int)Y.Value;
			this.DialogResult = true;
		}
	}
}
