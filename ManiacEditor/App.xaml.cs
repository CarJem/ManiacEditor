using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ManiacEditor.Interfaces.Assets;

namespace ManiacEditor
{
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
            ChangeSkin(Skin.Dark);
            this.InitializeComponent();
        }


		public void Load()
        {
			var UI = new ManiacEditor.Editor();
            UI.Run();
        }

		public static void ChangeSkin(Skin newSkin)
        {
            Skin = newSkin;

            foreach (ResourceDictionary dict in ManiacEditor.App.Current.Resources.MergedDictionaries)
            {

                if (dict is SkinResourceDictionary skinDict)
                    skinDict.UpdateSource();
                else
                    dict.Source = dict.Source;
            }
        }
	}
}
