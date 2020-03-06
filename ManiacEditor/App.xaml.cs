using GenerationsLib.WPF.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ManiacEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>


    public partial class App : Application
    {
        public static Skin Skin
        {
            get
            {
                return GenerationsLib.WPF.Themes.SkinResourceDictionary.CurrentTheme;
            } 
            set
            {
                GenerationsLib.WPF.Themes.SkinResourceDictionary.CurrentTheme = Skin;
            }
        }


        public App()
        {
            SkinResourceDictionary.ChangeSkin(Skin.Dark, ManiacEditor.App.Current.Resources.MergedDictionaries);
            this.InitializeComponent();
        }


		public void Load()
        {
			var UI = new ManiacEditor.Controls.Editor.MainEditor();
            UI.Editor_Run();
        }
	}
}
