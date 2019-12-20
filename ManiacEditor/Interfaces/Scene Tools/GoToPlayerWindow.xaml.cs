using System;
using System.Windows;
using RSDKv5;

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
			EditorInstance.UIModes.selectPlayerObject_GoTo = ComboBox1.SelectedIndex;
			this.Close();
		}

		public int GetSelectedPlayer()
		{
			//Debug.Print(selectedPlayer.ToString());
			return selectedPlayer;
		}
	}
}
