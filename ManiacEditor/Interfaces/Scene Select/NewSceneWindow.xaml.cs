using System.Windows;

namespace ManiacEditor.Interfaces
{
    public partial class NewSceneWindow : Window
	{
		public int Scene_Width = 128;
		public int Scene_Height = 128;
		public int BG_Width = 256;
		public int BG_Height = 256;
		public string SceneFolder;
		public NewSceneWindow()
		{
			InitializeComponent();
		}

		private void numericUpDown1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			Scene_Width = (int)layerWidthNud.Value;
		}

		private void numericUpDown2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			Scene_Height = (int)layerHeightNud.Value;
		}

		private void numericUpDown3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			BG_Height = (int)backgroundHeightNud.Value;
		}

		private void numericUpDown4_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			BG_Width = (int)backgroundWidthNud.Value;
		}

		private void button3_Click(object sender, RoutedEventArgs e)
		{
			GenerationsLib.Core.FolderSelectDialog path = new GenerationsLib.Core.FolderSelectDialog();
			path.ShowDialog();
			if (path.FileName != null)
			{
				SceneFolder = path.FileName;
				LocationBox.Text = SceneFolder;
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
