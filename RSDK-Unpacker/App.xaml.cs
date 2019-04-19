using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RSDK_Unpacker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public enum Skin { Dark, Light}

    public partial class App : Application
    {
        public static Skin Skin { get; set; } = Skin.Dark;

        public static bool SkinChanged { get; set; } = false;


        public App()
        {
            ChangeSkin(Skin.Dark);
        }

        public static void ChangeSkin(Skin newSkin)
        {
            Skin = newSkin;

            foreach (ResourceDictionary dict in RSDK_Unpacker.App.Current.Resources.MergedDictionaries)
            {

                if (dict is SkinResourceDictionary skinDict)
                    skinDict.UpdateSource();
                else
                    dict.Source = dict.Source;
            }
        }
    }
}
