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

namespace ManiacEditor
{
	/// <summary>
	/// Interaction logic for UnloadingSceneWarning.xaml
	/// </summary>
	public partial class UnloadingSceneWarning : Window
	{
		public WindowDialogResult WindowResult = WindowDialogResult.Invalid;
		public enum WindowDialogResult : int
		{
			Cancel = 1,
			Yes = 2,
			No = 3,
			Invalid = 0
		}
		public UnloadingSceneWarning()
		{
			InitializeComponent();
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			WindowResult = WindowDialogResult.Yes;
			Close();
		}
		private void NoSave_Click(object sender, RoutedEventArgs e)
		{
			WindowResult = WindowDialogResult.No;
			Close();
		}
		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			WindowResult = WindowDialogResult.Cancel;
			Close();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			if (WindowResult == WindowDialogResult.Invalid) WindowResult = WindowDialogResult.Cancel;
		}
	}
}
