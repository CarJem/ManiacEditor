using System.Windows;

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
            ChangeSkin(Skin.Light);
        }


		public void Load(string DataDir, string ScenePath, string ModPath, int LevelID, bool launchAsShortcut, int shortcutMode, bool isEncoreMode, int X, int Y)
        {
			var UI = new ManiacEditor.Editor(DataDir, ScenePath, ModPath, LevelID, launchAsShortcut, shortcutMode, isEncoreMode, X, Y);
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
