using System;
using System.Windows;
using System.Diagnostics;

namespace ManiacEditor.Interfaces.Updater
{
    /// <summary>
    /// Interaction logic for Updater.xaml
    /// </summary>
    public partial class ManiacUpdaterWindow : Window
	{
		public ManiacUpdaterWindow()
		{
			InitializeComponent();
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
	}
}
