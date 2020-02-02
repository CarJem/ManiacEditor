using System;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ManiacEditor.Controls.Utility.Editor.Options
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
	{
		public AboutWindow()
		{
			InitializeComponent();
			Title = String.Format("About {0}", Methods.ProgramBase.AssemblyAttributeAccessors.AssemblyTitle);
			labelProductName.Text = Methods.ProgramBase.AssemblyAttributeAccessors.AssemblyProduct;
			labelVersion.Text = String.Format("Version {0}", Methods.ProgramBase.GetCasualVersion());
			buildDateLabel.Text = String.Format("Build Date: {0}", Methods.ProgramBase.AssemblyAttributeAccessors.GetBuildTime) + Environment.NewLine + String.Format("Architecture: {0}", Methods.ProgramBase.AssemblyAttributeAccessors.GetProgramType);
			labelCopyright.Text = Methods.ProgramBase.AssemblyAttributeAccessors.AssemblyCopyright;

		}

		private void linkLabel3_LinkClicked(object sender, RoutedEventArgs e)
		{
			var updater = new Controls.Updater.ManiacUpdaterWindow();
			updater.Owner = this;
			updater.ShowDialog();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}
