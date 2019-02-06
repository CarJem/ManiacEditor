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
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections.Specialized;
using System.Windows.Forms.PropertyGridInternal;
using Keys = System.Windows.Forms.Keys;
using KeysConverter = System.Windows.Forms.KeysConverter;

namespace TileManiacWPF
{
	/// <summary>
	/// Interaction logic for OptionsMenu.xaml
	/// </summary>
	public partial class OptionsMenu : Window
	{


		public OptionsMenu()
		{
			InitializeComponent();
			SetupDefaults();
			SetKeybindTextboxes();
		}

		public void SetupDefaults()
		{
			switch (Properties.Settings.Default.ListSetting)
			{
				case 0:
					collisionListRadioButton.IsChecked = true;
					break;
				case 1:
					tileListRadioButton.IsChecked = true;
					break;
			}
			switch (Properties.Settings.Default.ViewAppearanceMode)
			{
				case 0:
					overlayEditorViewRadioButton.IsChecked = true;
					break;
				case 1:
					collisionEditorViewRadioButton.IsChecked = true;
					break;
			}
			switch (Properties.Settings.Default.RenderViewerSetting)
			{
				case 0:
					tileRenderViewRadioButton.IsChecked = true;
					break;
				case 1:
					collisionRenderViewRadioButton.IsChecked = true;
					break;
				case 2:
					overlayRenderViewRadioButton.IsChecked = true;
					break;
			}
			if (Properties.Settings.Default.NightMode)
			{
				DarkModeCheckBox.IsChecked = true;
			}
		}

		private void ListViewModeChanged(object sender, RoutedEventArgs e)
		{
			tileListRadioButton.IsChecked = false;
			collisionListRadioButton.IsChecked = false;

			if (e.Source == tileListRadioButton)
			{
				Properties.Settings.Default.ListSetting = 1;
				tileListRadioButton.IsChecked = true;
			}
			else if (e.Source == collisionListRadioButton)
			{
				Properties.Settings.Default.ListSetting = 0;
				collisionListRadioButton.IsChecked = true;
			}
		}

		private void EditorViewModeChanged(object sender, RoutedEventArgs e)
		{
			collisionEditorViewRadioButton.IsChecked = false;
			overlayEditorViewRadioButton.IsChecked = false;

			if (e.Source == collisionEditorViewRadioButton)
			{
				Properties.Settings.Default.ViewAppearanceMode = 1;
				collisionEditorViewRadioButton.IsChecked = true;
			}
			else if (e.Source == overlayEditorViewRadioButton)
			{
				Properties.Settings.Default.ViewAppearanceMode = 0;
				overlayEditorViewRadioButton.IsChecked = true;
			}
		}

		private void RenderViewModeChanged(object sender, RoutedEventArgs e)
		{
			tileRenderViewRadioButton.IsChecked = false;
			collisionRenderViewRadioButton.IsChecked = false;
			overlayRenderViewRadioButton.IsChecked = false;

			if (e.Source == tileRenderViewRadioButton)
			{
				Properties.Settings.Default.RenderViewerSetting = 0;
				tileRenderViewRadioButton.IsChecked = true;
			}
			else if (e.Source == collisionRenderViewRadioButton)
			{
				Properties.Settings.Default.RenderViewerSetting = 1;
				collisionRenderViewRadioButton.IsChecked = true;
			}
			else if (e.Source == overlayRenderViewRadioButton)
			{
				Properties.Settings.Default.RenderViewerSetting = 2;
				overlayRenderViewRadioButton.IsChecked = true;
			}
		}

		private void ResetSettingsToDefault(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to wipe your settings?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				Properties.Settings.Default.Reset();
			}
			else
			{

			}
		}

		private void button11_Click(object sender, RoutedEventArgs e)
		{
			String title = "Save Settings";
			String details = "Are you sure you want to save your settings, if the editor breaks because of one of these settings, you will have to redownload or manually reset you editor's config file! It's best you use the OK button to 'test' out the features before you save them.";
			if (MessageBox.Show(details, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				Properties.Settings.Default.Save();
			}
			else
			{
				return;
			}
		}

		private void EditKeyCombo(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button)) return;
			Button KeyBind = sender as Button;
			bool state = true;
			if (state)
			{
				string keybindName = KeyBind.Tag.ToString();

				StringCollection keyBindList = Properties.KeyBinds.Default[keybindName] as StringCollection;

				KeyBindConfigurator keybinder = new KeyBindConfigurator(keybindName);
				keybinder.ShowDialog();
				if (keybinder.DialogResult == true)
				{
					KeysConverter kc = new KeysConverter();
					System.Windows.Forms.Keys keyBindtoSet = keybinder.CurrentBindingKey;
					int keyIndex = keybinder.ListIndex;

					var keybindDict = Properties.KeyBinds.Default[keybindName] as StringCollection;
					String KeyString = kc.ConvertToString(keyBindtoSet);
					keybindDict.RemoveAt(keyIndex);
					keybindDict.Add(KeyString);
					Properties.KeyBinds.Default[keybindName] = keybindDict;
				}
			}
			SetKeybindTextboxes();

		}

		private void SetKeybindTextboxes()
		{
			
			foreach (StackPanel stack in Extensions.FindVisualChildren<StackPanel>(NewControls))
			{
				foreach (Button t in Extensions.FindVisualChildren<Button>(stack))
				{
					if (t.Tag != null) t.Content = KeyBindPraser(t.Tag.ToString());
				}
			}
			foreach (StackPanel stack in Extensions.FindVisualChildren<StackPanel>(EditControls))
			{
				foreach (Button t in Extensions.FindVisualChildren<Button>(stack))
				{
					if (t.Tag != null) t.Content = KeyBindPraser(t.Tag.ToString());
				}
			}
			foreach (StackPanel stack in Extensions.FindVisualChildren<StackPanel>(ViewControls))
			{
				foreach (Button t in Extensions.FindVisualChildren<Button>(stack))
				{
					if (t.Tag != null) t.Content = KeyBindPraser(t.Tag.ToString());
				}
			}
			foreach (StackPanel stack in Extensions.FindVisualChildren<StackPanel>(ToolsControls))
			{
				foreach (Button t in Extensions.FindVisualChildren<Button>(stack))
				{
					if (t.Tag != null) t.Content = KeyBindPraser(t.Tag.ToString());
				}
			}
			foreach (StackPanel stack in Extensions.FindVisualChildren<StackPanel>(FolderControls))
			{
				foreach (Button t in Extensions.FindVisualChildren<Button>(stack))
				{
					if (t.Tag != null) t.Content = KeyBindPraser(t.Tag.ToString());
				}
			}
			foreach (StackPanel stack in Extensions.FindVisualChildren<StackPanel>(OtherControls))
			{
				foreach (Button t in Extensions.FindVisualChildren<Button>(stack))
				{
					if (t.Tag != null) t.Content = KeyBindPraser(t.Tag.ToString());
				}
			}
		}

		private string KeyBindPraser(string keyRefrence)
		{
		
			if (keyRefrence == "NULL") return "N/A";

			List<string> keyBindList = new List<string>();
			List<string> keyBindModList = new List<string>();

			if (!Extensions.KeyBindsSettingExists(keyRefrence)) return "N/A";

			var keybindDict = Properties.KeyBinds.Default[keyRefrence] as StringCollection;
			if (keybindDict != null)
			{
				keyBindList = keybindDict.Cast<string>().ToList();
			}
			else
			{
				return "N/A";
			}

			if (keyBindList == null)
			{
				return "N/A";
			}

			if (keyBindList.Count > 1)
			{
				return string.Format("{0} Keybinds", keyBindList.Count);
			}
			else if ((keyBindList.Count == 1))
			{
				return keyBindList[0];
			}
			else
			{
				return "N/A";
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
		private void OptionBox_FormClosing(object sender, CancelEventArgs e)
		{
			/*
            if (checkBox15.Checked && !Properties.Settings.Default.NightMode)
            {
                DialogResult result = MessageBox.Show("To apply this setting correctly, you will have to restart the editor, would you like to that now?", "Restart to Apply", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Properties.Settings.Default.NightMode = true;
                    Properties.Settings.Default.Save();
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
            else if (!checkBox15.Checked && Properties.Settings.Default.NightMode)
            {
                DialogResult result = MessageBox.Show("To apply this setting correctly, you will have to restart the editor, would you like to that now?", "Restart to Apply", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Properties.Settings.Default.NightMode = false;
                    Properties.Settings.Default.Save();
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
            */
			if (DarkModeCheckBox.IsChecked == true && !Properties.Settings.Default.NightMode)
			{
				Properties.Settings.Default.NightMode = true;
				Properties.Settings.Default.Save();
				App.ChangeSkin(Skin.Dark);
				App.SkinChanged = true;

			}
			else if (!DarkModeCheckBox.IsChecked == true && Properties.Settings.Default.NightMode)
			{
				Properties.Settings.Default.NightMode = false;
				Properties.Settings.Default.Save();
				App.ChangeSkin(Skin.Light);
				App.SkinChanged = true;

			}
		}
	}
}
