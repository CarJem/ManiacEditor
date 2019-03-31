using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = RSDKrU.MessageBox;

namespace ManiacEditor.Interfaces
{
	/// <summary>
	/// Interaction logic for SceneSelectWindow.xaml
	/// </summary>
	public partial class SceneSelectWindow : Window
	{
		public SceneSelect SceneSelect;
		public SceneSelectWindow(RSDKv5.GameConfig config = null, Editor instance = null)
		{
			InitializeComponent();
			SceneSelect = new SceneSelect(config, instance, this);
			FrameHost.Children.Add(SceneSelect);
		}
	}
}
