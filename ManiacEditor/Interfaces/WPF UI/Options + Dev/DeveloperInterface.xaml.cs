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

namespace ManiacEditor.Interfaces
{
	/// <summary>
	/// Interaction logic for DeveloperInterface.xaml
	/// </summary>
	public partial class DeveloperTerminal : Window
	{
		// For Interger Changer; Change to the Value you want to tweak
		public Editor EditorInstance;

		public DeveloperTerminal(Editor instance)
		{
			InitializeComponent();
			EditorInstance = instance;
		}
	}
}
