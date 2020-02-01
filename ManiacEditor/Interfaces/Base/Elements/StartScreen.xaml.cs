using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ManiacEditor.Interfaces.Base.Elements
{
    /// <summary>
    /// Interaction logic for StartScreen.xaml
    /// </summary>
    public partial class StartScreen : UserControl
	{
		public Interfaces.Base.MainEditor EditorInstance;
		public SceneSelect SelectScreen;

		public string SelectedSavedPlace = "";
		public string SelectedModFolder = "";

		public StartScreen(Interfaces.Base.MainEditor instance)
		{
			InitializeComponent();
			EditorInstance = instance;
			SelectScreen = new SceneSelect(null, instance);
			SceneSelectHost.Children.Add(SelectScreen);
			SelectScreen.Refresh();
		}

		private void linkLabel3_LinkClicked(object sender, RoutedEventArgs e)
		{
			EditorInstance.OpenSceneForceFully();
		}

		private void linkLabel4_LinkClicked(object sender, RoutedEventArgs e)
		{
			EditorInstance.AboutScreenEvent(null, null);
		}

		private void linkLabel5_LinkClicked(object sender, RoutedEventArgs e)
		{
			EditorInstance.OptionsMenuEvent(null, null);
		}

		private void linkLabel6_LinkClicked(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://docs.google.com/document/d/1NBvcqzvOzqeTVzgAYBR0ttAc5vLoFaQ4yh_cdf-7ceQ/edit?usp=sharing");
		}

		private void linkLabel7_LinkClicked(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://github.com/CarJem/ManiacEditor-GenerationsEdition");
		}

		private void linkLabel8_LinkClicked(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://ci.appveyor.com/project/CarJem/maniaceditor-generationsedition");
		}

        private void linkLabel9_LinkClicked(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=RNZMRCMNX3SJY&currency_code=USD&source=url");
        }

        private void linkLabel10_LinkClicked(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://maniaceditor-generationsedition.readthedocs.io/en/latest/");
        }

        private void BasicRadioButton_GotFocus(object sender, RoutedEventArgs e)
		{
			modeLabel.Content = "Basic - A step up from minimal.";
		}

		private void MinimalRadioButton_GotFocus(object sender, RoutedEventArgs e)
		{
			modeLabel.Content = "Minimal - The very lowest you can go if you are don't have enough power.";
		}

		private void SuperRadioButton_GotFocus(object sender, RoutedEventArgs e)
		{
			modeLabel.Content = "Super - The best feature set without killing it!";
		}

		private void HyperRadioButton_GotFocus(object sender, RoutedEventArgs e)
		{
			modeLabel.Content = "Hyper - Kicking things into OVERDRIVE!";
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{

			if (!Properties.Settings.Default.NeverShowThisAgain)
			{
				DeveloperNoteOverlay.Visibility = Visibility.Visible;
			}

			if (!Properties.DevSettings.Default.DevAutoStart)
			{
				devCheck.Visibility = Visibility.Hidden;
				devLink.Visibility = Visibility.Hidden;
			}

			//if (Properties.Settings.Default.ShowFirstTimeSetup)
			//{
			//	FirstTimeOverlay.Visibility = Visibility.Visible;
			//	SceneSelectHost.Visibility = Visibility.Hidden;
			//}
		}

		private void DeveloperNoteAcceptedButton_Click(object sender, RoutedEventArgs e)
		{
			Properties.Settings.Default.NeverShowThisAgain = true;
            Properties.Settings.Default.Save();

            DeveloperNoteOverlay.Visibility = Visibility.Hidden;
		}

		private void QuickSettingSetButton_Click(object sender, RoutedEventArgs e)
		{
			if (minimalRadioButton.IsChecked.Value) EditorSettings.ApplyPreset(0);
			else if (basicRadioButton.IsChecked.Value) EditorSettings.ApplyPreset(1);
			else if (superRadioButton.IsChecked.Value) EditorSettings.ApplyPreset(2);
			else if (hyperRadioButton.IsChecked.Value) EditorSettings.ApplyPreset(3);

			Properties.Settings.Default.ShowFirstTimeSetup = false;
            Properties.Settings.Default.Save();
            FirstTimeOverlay.Visibility = Visibility.Hidden;
			SceneSelectHost.Visibility = Visibility.Visible;
		}

        private void YoutubeButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/channel/UC4-VHCZD7eLdxRr5aUXAQ5w");
        }

        private void PatreonButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Comming Soon!", "Sorry!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TwitterButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://twitter.com/carter5467_99");
        }

        private void DiscordButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("You can reach me via discord by: CarJem Generations#7078", "Discord Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
