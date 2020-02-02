using System.Windows;
using System.Windows.Controls;

namespace ManiacEditor.Controls.SceneSelect
{
    /// <summary>
    /// Interaction logic for EditSceneSelectInfoWindow.xaml
    /// </summary>
    public partial class EditSceneSelectInfoWindow : Window
	{
		public RSDKv5.Gameconfig.SceneInfo Scene;

		public EditSceneSelectInfoWindow()
		{
			Scene = new RSDKv5.Gameconfig.SceneInfo();
			InitializeComponent();
		}

		public EditSceneSelectInfoWindow(RSDKv5.Gameconfig.SceneInfo scene)
		{
			if (scene == null)
				Scene = new RSDKv5.Gameconfig.SceneInfo();
			else
				Scene = scene;
			InitializeComponent();
			TextBox1_TextChanged(null, null);
		}

		private void EditSceneSelectInfoForm_Load(object sender, RoutedEventArgs e)
		{
			textBox1.Text = Scene.Name;
			textBox2.Text = Scene.Zone;
			textBox3.Text = Scene.SceneID;
			positionIDLabel.Content = "Stage ID:" + Scene.LevelID.ToString();
			checkBox1.IsChecked = (Scene.ModeFilter & 0b0001) != 0;
			checkBox2.IsChecked = (Scene.ModeFilter & 0b0010) != 0;
			checkBox3.IsChecked = (Scene.ModeFilter & 0b0100) != 0;
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			Scene.Name = textBox1.Text;
			Scene.Zone = textBox2.Text;
			Scene.SceneID = textBox3.Text;
			Scene.ModeFilter |= (byte)((checkBox1.IsChecked.Value ? 1 : 0) << 0);
			Scene.ModeFilter |= (byte)((checkBox2.IsChecked.Value ? 1 : 0) << 1);
			Scene.ModeFilter |= (byte)((checkBox3.IsChecked.Value ? 1 : 0) << 2);
			DialogResult = true;
			Close();
		}

		private void button2_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
		{
			label4.Content = $"Data\\Stages\\{textBox2.Text}\\Scene{textBox3.Text}.bin";
		}
	}
}
