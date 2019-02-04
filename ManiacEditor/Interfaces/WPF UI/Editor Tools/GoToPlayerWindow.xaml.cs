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
using RSDKv5;
using System.Diagnostics;

namespace ManiacEditor.Interfaces
{
	/// <summary>
	/// Interaction logic for GoToPlayerWindow.xaml
	/// </summary>
	public partial class GoToPlayerBox : Window
	{
		int playerObjectCount = 0;
		public int selectedPlayer = 0;
		public Editor EditorInstance;
		public GoToPlayerBox(Editor instance)
		{
			InitializeComponent();
			EditorInstance = instance;
			playerObjectCount = EditorInstance.playerObjectPosition.Count;
			for (int i = 0; i < playerObjectCount - 1; i++)
			{
				Position pos = EditorInstance.playerObjectPosition[i].Position;
				String id = EditorInstance.playerObjectPosition[i].SlotID.ToString();
				String posText = "X: " + pos.X.High + " Y: " + pos.Y.High;
				ComboBox1.Items.Add("[" + id + "] " + posText);
			}

		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			EditorInstance.selectPlayerObject_GoTo = ComboBox1.SelectedIndex;
			this.Close();
		}

		public int GetSelectedPlayer()
		{
			//Debug.Print(selectedPlayer.ToString());
			return selectedPlayer;
		}
	}
}
