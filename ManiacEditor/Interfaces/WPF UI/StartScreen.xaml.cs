using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ManiacEditor.Interfaces
{
	/// <summary>
	/// Interaction logic for StartScreen.xaml
	/// </summary>
	public partial class StartScreen : UserControl
	{
		bool updateAvaliable = false;
		bool sourceBuild = false;
		public Editor EditorInstance;
		public SceneSelect SelectScreen;

		public string SelectedSavedPlace = "";
		public string SelectedModFolder = "";

		public StartScreen(Editor instance)
		{
			InitializeComponent();
			EditorInstance = instance;
			SelectScreen = new SceneSelect(null, instance, true);
			SceneSelectHost.Children.Add(SelectScreen);
			SelectScreen.Refresh();
		}

		public void UpdateStatusLabel(int condition, EditorUpdater updater)
		{
			switch (condition)
			{
				case 0:
					updateAvaliable = false;
					break;
				case 1:
					updateAvaliable = true;
					break;
				case 2:
					updateAvaliable = false;
					sourceBuild = true;
					break;
				default:
					updateAvaliable = false;
					break;
			}
			if (updateAvaliable)
			{
				this.updateInfoLabel.Text = "An Update is Avaliable!" + Environment.NewLine + Environment.NewLine + string.Format("Local Version: {0}", updater.GetVersion()) + Environment.NewLine + string.Format("Current Version: {0}", updater.GetCurrentVersion()) + Environment.NewLine + Environment.NewLine + string.Format("Latest Version Details: {0}", updater.GetBuildMessage());
			}
			else
			{
				if (sourceBuild)
				{
					this.updateInfoLabel.Text = "Your using the source. No need to update!" + Environment.NewLine + Environment.NewLine + string.Format("Local Version: {0}", updater.GetVersion()) + Environment.NewLine + string.Format("Current Version: {0}", updater.GetCurrentVersion()) + Environment.NewLine + Environment.NewLine + string.Format("Latest Version Details: {0}", updater.GetBuildMessage());
				}
				else
				{
					this.updateInfoLabel.Text = "No Updates Found!" + Environment.NewLine + Environment.NewLine + string.Format("Local Version: {0}", updater.GetVersion()) + Environment.NewLine + string.Format("Current Version: {0}", updater.GetCurrentVersion()) + Environment.NewLine + Environment.NewLine + string.Format("Latest Version Details: {0}", updater.GetBuildMessage());
				}
			}
		}

		private void linkLabel1_LinkClicked(object sender, RoutedEventArgs e)
		{
			ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/CarJem/ManiacEditor-GenerationsEdition/releases");
			Process.Start(sInfo);
		}

		private void linkLabel2_LinkClicked(object sender, RoutedEventArgs e)
		{
			ProcessStartInfo sInfo = new ProcessStartInfo("https://ci.appveyor.com/project/CarJem/maniaceditor-generationsedition");
			Process.Start(sInfo);
		}

		private void linkLabel3_LinkClicked(object sender, RoutedEventArgs e)
		{
			EditorInstance.OpenSceneForceFully();
		}

		private void linkLabel4_LinkClicked(object sender, RoutedEventArgs e)
		{
			EditorInstance.AboutToolStripMenuItem_Click(null, null);
		}

		private void linkLabel5_LinkClicked(object sender, RoutedEventArgs e)
		{
			EditorInstance.OptionToolStripMenuItem_Click(null, null);
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

			if (!Properties.Settings.Default.UseForcefulStartup)
			{
				devCheck.Visibility = Visibility.Hidden;
				devLink.Visibility = Visibility.Hidden;
			}

			if (Properties.Settings.Default.ShowFirstTimeSetup)
			{
				FirstTimeOverlay.Visibility = Visibility.Visible;
				SceneSelectHost.Visibility = Visibility.Hidden;
			}
		}

		private void DeveloperNoteAcceptedButton_Click(object sender, RoutedEventArgs e)
		{
			Properties.Settings.Default.NeverShowThisAgain = true;
			DeveloperNoteOverlay.Visibility = Visibility.Hidden;
		}

		private void QuickSettingSetButton_Click(object sender, RoutedEventArgs e)
		{
			if (minimalRadioButton.IsChecked.Value) EditorSettings.ApplyPreset(0);
			else if (basicRadioButton.IsChecked.Value) EditorSettings.ApplyPreset(1);
			else if (superRadioButton.IsChecked.Value) EditorSettings.ApplyPreset(2);
			else if (hyperRadioButton.IsChecked.Value) EditorSettings.ApplyPreset(3);

			Properties.Settings.Default.ShowFirstTimeSetup = false;
			FirstTimeOverlay.Visibility = Visibility.Hidden;
			SceneSelectHost.Visibility = Visibility.Visible;
		}
	}
}
