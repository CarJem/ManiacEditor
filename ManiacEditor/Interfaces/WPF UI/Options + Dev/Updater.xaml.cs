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
using System.Diagnostics;

namespace ManiacEditor
{
	/// <summary>
	/// Interaction logic for Updater.xaml
	/// </summary>
	public partial class Updater : Window
	{
		bool updateAvaliable = false;
		bool sourceBuild = false;
		public Updater(int condition, EditorUpdater updater)
		{
			InitializeComponent();
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
					this.updateInfoLabel.Text = "You're using the source. No need to update!" + Environment.NewLine + Environment.NewLine + string.Format("Local Version: {0}", updater.GetVersion()) + Environment.NewLine + string.Format("Current Version: {0}", updater.GetCurrentVersion()) + Environment.NewLine + Environment.NewLine + string.Format("Latest Version Details: {0}", updater.GetBuildMessage());
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
	}
}
