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

namespace ManiacEditor
{
	/// <summary>
	/// Interaction logic for Mainform.xaml
	/// </summary>
	public partial class Mainform : Window
	{
		public Mainform()
		{
			InitializeComponent();
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					Border border = new Border()
					{
						BorderBrush = (SolidColorBrush)FindResource("DisabledText"),
						BorderThickness = new Thickness(1)
					};
					TextBlock CollisionPosition = new TextBlock()
					{
						Text = GetCollisionSection(y),
						Foreground = (SolidColorBrush)FindResource("NormalText"),
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center				
					};
					Grid.SetColumn(CollisionPosition, x);
					Grid.SetRow(CollisionPosition, y);
					Grid.SetColumn(border, x);
					Grid.SetRow(border, y);
					CollisionViewer.Children.Add(border);
					CollisionViewer.Children.Add(CollisionPosition);

				}
			}
		}

		public string GetCollisionSection(int y)
		{
			switch (y)
			{
				case 0:
					return "0";
				case 1:
					return "1";
				case 2:
					return "2";
				case 3:
					return "3";
				case 4:
					return "4";
				case 5:
					return "5";
				case 6:
					return "6";
				case 7:
					return "7";
				case 8:
					return "8";
				case 9:
					return "9";
				case 10:
					return "A";
				case 11:
					return "B";
				case 12:
					return "C";
				case 13:
					return "D";
				case 14:
					return "E";
				case 15:
					return "F";
				default:
					return "NULL";

			}

		}
	}
}
