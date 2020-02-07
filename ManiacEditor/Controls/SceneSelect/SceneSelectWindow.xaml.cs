using System.Windows;

namespace ManiacEditor.Controls.SceneSelect
{
    /// <summary>
    /// Interaction logic for SceneSelectWindow.xaml
    /// </summary>
    public partial class SceneSelectWindow : Window
	{
		public Controls.SceneSelect.SceneSelectHost SceneSelect;
		public SceneSelectWindow(RSDKv5.Gameconfig config = null, Controls.Editor.MainEditor instance = null)
		{
			InitializeComponent();
			SceneSelect = new Controls.SceneSelect.SceneSelectHost(config, instance, this);
			FrameHost.Children.Add(SceneSelect);
		}
	}
}
