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
        public IList<string> SavedPositionsTextList = new List<string>();
        private Editor Instance;
		public GoToPositionBox(Editor instance)
		{
			InitializeComponent();
            Instance = instance;
            GetSavedPositions();
		}

        public void GetSavedPositions()
        {
            var SavedPositions = Instance.ManiacINI.ReturnPositionMarkers();
            if (SavedPositions.Count != 0) SavedPositionsList.IsEnabled = true;
            else SavedPositionsList.IsEnabled = false;
            foreach (var positions in SavedPositions)
            {
                Label item = new Label();
                item.Content = positions.Item1 + string.Format(" - ({0})", positions.Item2);
                SavedPositionsTextList.Add(positions.Item2);
                SavedPositionsList.Items.Add(item);
            }
        }

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			if (TileModeCheckbox.IsChecked.Value)
			{
				tilesMode = true;
			}
			goTo_X = (int)X.Value;
			goTo_Y = (int)Y.Value;

            if (SavePositionCheckbox.IsChecked.Value)
            {
                Instance.ManiacINI.AddSavedCoordinates(SavedPositionTextBox.Text, goTo_X, goTo_Y, tilesMode);
            }

			this.DialogResult = true;
		}

        private void SavedPositionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SavedPositionsList.SelectedIndex == -1) return;
            if (SavedPositionsTextList[SavedPositionsList.SelectedIndex] == null) return;
            string Coords = SavedPositionsTextList[SavedPositionsList.SelectedIndex];
            string[] SplitCoords = Coords.Split(',');
            Int32.TryParse(SplitCoords[0], out int x);
            Int32.TryParse(SplitCoords[1], out int y);

            X.Value = x;
            Y.Value = y;
        }
    }
}
