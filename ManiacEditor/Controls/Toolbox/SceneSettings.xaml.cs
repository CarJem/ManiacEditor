using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GenerationsLib.Core;
using ManiacEditor.Classes.Prefrences;
using ManiacEditor.Extensions;
using ManiacEditor.Methods.Solution;

namespace ManiacEditor.Controls.Toolbox
{
    /// <summary>
    /// Interaction logic for ModPackEditor.xaml
    /// </summary>
    public partial class SceneSettings : Window
    {

        public SceneSettings(Controls.Editor.MainEditor instance)
        {
            InitializeComponent();
            if (SolutionPaths.CurrentSceneData.Is_IZStage)
            {
                if (CurrentSolution.IZ_Stage == null) CurrentSolution.IZ_Stage = new Structures.IZStage();
                if (CurrentSolution.IZ_Stage.Unlocks == null) CurrentSolution.IZ_Stage.Unlocks = new List<string>();
                if (CurrentSolution.IZ_Stage.Assets == null) CurrentSolution.IZ_Stage.Assets = new List<Structures.IZAsset>();
            }
            else
            {
                InfinityZoneTab.Visibility = Visibility.Collapsed;
                InfinityZoneTab.IsEnabled = false;
            }


            DataContext = SceneCurrentSettings.ManiacINIData;
            RefreshListBoxes();
        }


        private void RefreshListBoxes()
        {
            if (SolutionPaths.CurrentSceneData.Is_IZStage)
            {
                ZoneUnlocksListBox.ItemsSource = null;
                ZoneUnlocksListBox.ItemsSource = CurrentSolution.IZ_Stage.Unlocks;

                SpritePathsListBox.ItemsSource = null;
                SpritePathsListBox.ItemsSource = CurrentSolution.IZ_Stage.Assets;
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SceneCurrentSettings.SaveFile();
        }

        private void LoadSource_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetRedirectSpriteFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderSelectDialog fsd = new FolderSelectDialog();
            if (fsd.ShowDialog())
            {
                SceneCurrentSettings.ManiacINIData.RedirectSpriteDataFolder = fsd.FileName;
                RefreshListBoxes();
            }
        }

        private void SpritePathsEditButton_Click(object sender, RoutedEventArgs e)
        {
            bool inRange = CurrentSolution.IZ_Stage.Assets.Count > SpritePathsListBox.SelectedIndex;
            bool isValid = SpritePathsListBox.SelectedIndex != -1 && CurrentSolution.IZ_Stage.Assets.Count != 0;
            if (inRange && isValid)
            {
                var previousData = CurrentSolution.IZ_Stage.Assets[SpritePathsListBox.SelectedIndex];
                var result = GenerationsLib.WPF.TextPromptDual.ShowDialog("", "Base Path: ", "New Path: ", previousData.BasePath, previousData.NewPath);

                CurrentSolution.IZ_Stage.Assets[SpritePathsListBox.SelectedIndex] = new Structures.IZAsset(result.Item1, result.Item2);
                RefreshListBoxes();
            }
        }

        private void SpritePathsRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            bool inRange = CurrentSolution.IZ_Stage.Assets.Count > SpritePathsListBox.SelectedIndex;
            bool isValid = SpritePathsListBox.SelectedIndex != -1 && CurrentSolution.IZ_Stage.Assets.Count != 0;
            if (inRange && isValid)
            {
                CurrentSolution.IZ_Stage.Assets.RemoveAt(SpritePathsListBox.SelectedIndex);
                RefreshListBoxes();
            }
        }

        private void SpritePathsAddButton_Click(object sender, RoutedEventArgs e)
        {
            var result = GenerationsLib.WPF.TextPromptDual.ShowDialog("", "Base Path: ", "New Path: ", "BASE_PATH", "NEW_PATH");
            var newValue = new Structures.IZAsset(result.Item1, result.Item2);

            CurrentSolution.IZ_Stage.Assets.Add(newValue);
            RefreshListBoxes();
        }

        private void ZoneUnlocksAddButton_Click(object sender, RoutedEventArgs e)
        {
            string newValue = GenerationsLib.WPF.TextPrompt2.ShowDialog("", "Enter your value", "NewPath");
            CurrentSolution.IZ_Stage.Unlocks.Add(newValue);
            RefreshListBoxes();
        }

        private void ZoneUnlocksRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            bool inRange = CurrentSolution.IZ_Stage.Unlocks.Count > ZoneUnlocksListBox.SelectedIndex;
            bool isValid = ZoneUnlocksListBox.SelectedIndex != -1 && CurrentSolution.IZ_Stage.Unlocks.Count != 0;
            if (inRange && isValid)
            {
                CurrentSolution.IZ_Stage.Unlocks.RemoveAt(ZoneUnlocksListBox.SelectedIndex);
                RefreshListBoxes();
            }
        }

        private void ZoneUnlocksEditButton_Click(object sender, RoutedEventArgs e)
        {
            bool inRange = CurrentSolution.IZ_Stage.Unlocks.Count > ZoneUnlocksListBox.SelectedIndex;
            bool isValid = ZoneUnlocksListBox.SelectedIndex != -1 && CurrentSolution.IZ_Stage.Unlocks.Count != 0;
            if (inRange && isValid)
            {
                CurrentSolution.IZ_Stage.Unlocks[ZoneUnlocksListBox.SelectedIndex] = GenerationsLib.WPF.TextPrompt2.ShowDialog("", "Enter your value", CurrentSolution.IZ_Stage.Unlocks[ZoneUnlocksListBox.SelectedIndex]);
                RefreshListBoxes();
            }
        }
    }
}
