using System.Windows;

namespace ManiacEditor.Interfaces.SceneSelect
{
    /// <summary>
    /// Interaction logic for SceneSelectWindow.xaml
    /// </summary>
    public partial class SceneSelectWindow : Window
	{
		public Interfaces.SceneSelect.SceneSelectHost SceneSelect;
		public SceneSelectWindow(RSDKv5.Gameconfig config = null, Interfaces.Base.MainEditor instance = null)
		{
			InitializeComponent();
			SceneSelect = new Interfaces.SceneSelect.SceneSelectHost(config, instance, this);
			FrameHost.Children.Add(SceneSelect);
		}
	}
}
