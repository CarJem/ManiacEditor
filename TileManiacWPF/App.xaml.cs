using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TileManiac
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public enum Skin { Dark, Light }

	public partial class App : Application
	{
		public static Skin Skin { get; set; } = Skin.Dark;

		public static bool SkinChanged { get; set; } = false;


		public App()
		{
			if (TileManiac.Properties.Settings.Default.NightMode)
			{
				ChangeSkin(Skin.Dark);
			}
			else
			{
				ChangeSkin(Skin.Light);
			}

		}

		public static void ChangeSkin(Skin newSkin)
		{
			Skin = newSkin;

			foreach (ResourceDictionary dict in Current.Resources.MergedDictionaries)
			{

				if (dict is SkinResourceDictonary skinDict)
					skinDict.UpdateSource();
				else
					dict.Source = dict.Source;
			}
		}
	}
}
