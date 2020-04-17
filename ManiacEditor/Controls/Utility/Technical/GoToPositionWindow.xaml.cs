using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using RSDKv5;

namespace ManiacEditor.Controls.Utility
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
        public IList<Position> PlayerSpawnPositionsList = new List<Position>();

        int playerObjectCount = 0;
        public int selectedPlayer = 0;

        private Controls.Editor.MainEditor Instance;
		public GoToPositionBox(Controls.Editor.MainEditor instance)
		{
			InitializeComponent();
            Instance = instance;
            GetSavedPositions();
            GetPlayerPositions();

        }

        private void GetPlayerPositions()
        {
            PlayerSpawnPositionsList = new List<Position>();
            foreach (var player in GetPlayers())
            {
                Position pos = player.Position;
                String id = player.SlotID.ToString();
                String posText = "X: " + pos.X.High + " Y: " + pos.Y.High;
                ComboBox1.Items.Add("[" + id + "] " + posText);
                PlayerSpawnPositionsList.Add(pos);
            }
        }

        private List<Classes.Scene.EditorEntity> GetPlayers()
        {
            List<Classes.Scene.EditorEntity> players = new List<Classes.Scene.EditorEntity>();
            foreach (var _entity in Methods.Solution.CurrentSolution.Entities.Entities)
            {
                if (_entity.Object.Name.Name == "Player")
                {
                    players.Add(_entity);
                }
            }
            return players;

        }

        public void GetSavedPositions()
        {
            if (Classes.Prefrences.SceneCurrentSettings.ManiacINIData.Positions != null)
            {
                var SavedPositions = Classes.Prefrences.SceneCurrentSettings.ManiacINIData.Positions;
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
        }

        private void GoToPlayer_Click(object sender, RoutedEventArgs e)
        {
            int i = ComboBox1.SelectedIndex;
            Methods.Solution.SolutionActions.GoToPosition(PlayerSpawnPositionsList[i].X.High, PlayerSpawnPositionsList[i].Y.High, true);
            this.Close();
        }

        private void GoToCoords_Click(object sender, RoutedEventArgs e)
		{
			if (TileModeCheckbox.IsChecked.Value)
			{
				tilesMode = true;
			}
			goTo_X = (int)X.Value;
			goTo_Y = (int)Y.Value;

            if (SavePositionCheckbox.IsChecked.Value)
            {
                Classes.Prefrences.SceneCurrentSettings.AddSavedCoordinates(SavedPositionTextBox.Text, goTo_X, goTo_Y, tilesMode);
            }

            if (tilesMode)
            {
                goTo_X *= 16;
                goTo_Y *= 16;
            }
            Methods.Solution.SolutionActions.GoToPosition(goTo_X, goTo_Y, true);
            this.Close();
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

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox1.SelectedItem == null)
            {
                PlayerWarpButton.IsEnabled = false;
            }
            else 
            {
                PlayerWarpButton.IsEnabled = true;
            }
        }

        private void SavePositionCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (SavePositionCheckbox.IsChecked.Value)
            {
                SavedPositionTextBox.IsEnabled = true;
                SavedPositionTextBox_TextChanged(null, null);
            }
            else
            {
                SavedPositionTextBox.IsEnabled = false;
                SavedPositionTextBox_TextChanged(null, null);
            }
        }

        private void SavedPositionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool hasNoText = (SavedPositionTextBox.Text == null || SavedPositionTextBox.Text == "");
            if (SavePositionCheckbox.IsChecked.Value)
            {
                if (hasNoText) gotocoordsbutton.IsEnabled = false;
                else gotocoordsbutton.IsEnabled = true;

            }
            else
            {
                gotocoordsbutton.IsEnabled = true;
            }
        }
    }
}
