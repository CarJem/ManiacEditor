using System.Windows;

namespace ManiacEditor.Controls.SceneSelect
{
    /// <summary>
    /// Interaction logic for SceneSelectWindow.xaml
    /// </summary>
    public partial class SceneSelectWindow : Window
	{
		public Controls.SceneSelect.SceneSelectHost SceneSelect;
		public SceneSelectWindow(RSDKv5.GameConfig config = null, Controls.Editor.MainEditor instance = null, bool selectDirectory = false)
		{
			InitializeComponent();
			SceneSelect = new Controls.SceneSelect.SceneSelectHost(config, instance, this);
			FrameHost.Children.Add(SceneSelect);
			if (selectDirectory)
			{
				SceneSelect.AddANewDataDirectory();
			}
		}
	}
}
