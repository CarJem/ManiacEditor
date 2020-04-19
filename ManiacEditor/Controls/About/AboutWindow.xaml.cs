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

namespace ManiacEditor.Controls.About
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
            GenerateCredits();
		}

		private void GenerateCredits()
		{
            CreditsRoll.Children.Add(GenerateString("Developed By:"));
            CreditsRoll.Children.Add(GenerateCreditLine("CarJem Generations", "https://github.com/CarJem", "", "https://www.youtube.com/channel/UC4-VHCZD7eLdxRr5aUXAQ5w", "https://twitter.com/carter5467_99"));
            CreditsRoll.Children.Add(GenerateString("Originally Created By:"));
            CreditsRoll.Children.Add(GenerateCreditLine("Koolkdev", "https://github.com/koolkdev/ManiacEditor"));
            CreditsRoll.Children.Add(GenerateString("Extended/Enhanced By:"));
            CreditsRoll.Children.Add(GenerateCreditLine("OtherworldBob", "https://github.com/OtherworldBob/ManiacEditor", "For his fork that allowed ours to exist"));
            CreditsRoll.Children.Add(GenerateCreditLine("CampbellSonic", "https://github.com/campbellsonic/ManiacEditor", "Early object rendering work and feature ideas"));
            CreditsRoll.Children.Add(GenerateCreditLine("SuperSonic16", "https://github.com/thesupersonic16", "For the original entity rendering system and providing ManiaPal Intergration"));
            CreditsRoll.Children.Add(GenerateCreditLine("Rubberduckycooly", "https://github.com/Rubberduckycooly/RSDK", "The RSDK-Reverse libs that allow this editor to exist today"));
            CreditsRoll.Children.Add(GenerateCreditLine("Beta Angel", "https://twitter.com/BetaAngel01", "Programming, the original tile maniac source code, an active assistant and my motivation"));
            CreditsRoll.Children.Add(GenerateString("Our Most Honored:"));
            CreditsRoll.Children.Add(GenerateCreditLine("Axanery", "https://github.com/koolkdev/ManiacEditor", "Amazing helper since Maniac Editor's early stages"));
            CreditsRoll.Children.Add(GenerateCreditLine("LJSTAR", "https://twitter.com/LJSTAR_", "Guardian of 'feature requests' and 'usability optimizations' in favor of the users", "https://www.youtube.com/channel/UCipu-balRJyva72YPT7nOEg"));
            CreditsRoll.Children.Add(GenerateCreditLine("Yarcaz", "https://twitter.com/Yarcaz0", "Best friend, personal mentor, and provider of my inital motivation"));
            CreditsRoll.Children.Add(GenerateCreditLine("CodenameGamma", "https://twitter.com/Codenamegamma", "Higher level technical assistance for Maniac!", "https://www.youtube.com/CodenameGamma"));
            CreditsRoll.Children.Add(GenerateString("Special Thanks To:"));
            CreditsRoll.Children.Add(GenerateCreditLine("Stealth", "https://twitter.com/HCStealth"));
            CreditsRoll.Children.Add(GenerateCreditLine("Taxman/Christian Whitehead", "https://twitter.com/cfwhitehead"));
            CreditsRoll.Children.Add(GenerateCreditLine("SEGA", "https://www.sega.com/"));
        }

        private TextBlock GenerateString(string message)
        {
            TextBlock item = new TextBlock();
            var bold = new Bold(new Run(message));
            item.Inlines.Add(bold);
            item.Foreground = Methods.Internal.Theming.NormalText;
            return item;
        }

        private CreditEntry GenerateCreditLine(string title, string url, string message = "", string url2 = "", string url3 = "", string url4 = "")
        {
            CreditEntry item = new CreditEntry();
            item.SetCreditsEntry(title, url, message, url2, url3, url4);
            return item;
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
