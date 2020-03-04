using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ManiacEditor.Controls.Assets;

namespace ManiacEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>


    public partial class App : Application
    {
        public static Enums.Skin Skin { get; set; } = Enums.Skin.Dark;

        public static bool SkinChanged { get; set; } = false;


        public App()
        {
            ChangeSkin(Enums.Skin.Dark);
            this.InitializeComponent();
        }


		public void Load()
        {
			var UI = new ManiacEditor.Controls.Editor.MainEditor();
            UI.Run();
        }

		public static void ChangeSkin(Enums.Skin newSkin)
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
