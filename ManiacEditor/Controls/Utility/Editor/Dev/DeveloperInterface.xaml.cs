using System.Windows;

namespace ManiacEditor.Controls.Utility.Editor.Dev
{
    /// <summary>
    /// Interaction logic for DeveloperInterface.xaml
    /// </summary>
    public partial class DeveloperTerminal : Window
	{
		// For Interger Changer; Change to the Value you want to tweak
		public Controls.Base.MainEditor EditorInstance;

		public DeveloperTerminal(Controls.Base.MainEditor instance)
		{
			InitializeComponent();
			EditorInstance = instance;
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string newLine = "    ";
            string contents = "";
            if (DevInt1.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt1.ToString() + newLine;
            if (DevInt2.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt2.ToString() + newLine;
            if (DevInt3.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt3.ToString() + newLine;
            if (DevInt4.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt4.ToString() + newLine;
            if (DevInt5.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt5.ToString() + newLine;
            if (DevInt6.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt6.ToString() + newLine;
            if (DevInt7.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt7.ToString() + newLine;
            if (DevInt8.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt8.ToString() + newLine;
            if (DevInt9.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt9.ToString() + newLine;
            if (DevInt10.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt10.ToString() + newLine;
            if (DevInt11.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt11.ToString() + newLine;
            if (DevInt12.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt12.ToString() + newLine;
            if (DevInt13.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt13.ToString() + newLine;
            if (DevInt14.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt14.ToString() + newLine;
            if (DevInt15.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt15.ToString() + newLine;
            if (DevInt16.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt16.ToString() + newLine;
            if (DevInt17.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt17.ToString() + newLine;
            if (DevInt18.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt18.ToString() + newLine;
            if (DevInt19.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt19.ToString() + newLine;
            if (DevInt20.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt20.ToString() + newLine;
            if (DevInt21.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt21.ToString() + newLine;
            if (DevInt22.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt22.ToString() + newLine;
            if (DevInt23.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt23.ToString() + newLine;
            if (DevInt24.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt24.ToString() + newLine;
            if (DevInt25.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt25.ToString() + newLine;
            if (DevInt26.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt26.ToString() + newLine;
            if (DevInt27.IsChecked.Value) contents += Core.Settings.MyDevSettings.DevInt27.ToString() + newLine;


            Clipboard.SetText(contents);

        }
    }
}
