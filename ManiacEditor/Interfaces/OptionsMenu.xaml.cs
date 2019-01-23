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
using SharpDX;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections.Specialized;
using System.Windows.Forms.PropertyGridInternal;
using Cyotek.Windows.Forms;
using ManiacEditor.Interfaces;
using Keys = System.Windows.Forms.Keys;
using KeysConverter = System.Windows.Forms.KeysConverter;

namespace ManiacEditor.Interfaces
{
	/// <summary>
	/// Interaction logic for OptionsMenu.xaml
	/// </summary>
	public partial class OptionsMenu : Window
	{
		bool preRenderRadioGroupCheckChangeAllowed = true;
		bool collisionColorsRadioGroupCheckChangeAllowed = true;
		public Editor EditorInstance;
		System.Windows.Forms.Timer CheckGraphicalSettingTimer;
		public OptionsMenu(Editor instance)
		{
			InitializeComponent();
			EditorInstance = instance;

			CheckGraphicalSettingTimer = new System.Windows.Forms.Timer();
			CheckGraphicalSettingTimer.Interval = 10;
			CheckGraphicalSettingTimer.Tick += CheckGraphicalPresetModeState;

			preRenderRadioGroupsUpdate(Properties.Settings.Default.preRenderSceneOption);
			collisionColorsRadioGroupUpdate(Properties.Settings.Default.CollisionColorsDefault);
			preRenderRadioGroupCheckChangeAllowed = true;
			collisionColorsRadioGroupCheckChangeAllowed = true;
			if (Properties.Settings.Default.x16Default) uncheckOtherGridDefaults(1);
			if (Properties.Settings.Default.x128Default) uncheckOtherGridDefaults(2);
			if (Properties.Settings.Default.x256Default) uncheckOtherGridDefaults(3);
			if (Properties.Settings.Default.CustomGridDefault) uncheckOtherGridDefaults(4);

			foreach (RadioButton rdo in Extensions.FindVisualChildren<RadioButton>(MenuLangGroup))
			{
				if (rdo.Tag.ToString() == Settings.mySettings.LangDefault)
				{
					rdo.IsChecked = true;
				}
			}

			foreach (RadioButton rdo in Extensions.FindVisualChildren<RadioButton>(MenuLangGroup2))
			{
				if (rdo.Tag.ToString() == Settings.mySettings.LangDefault)
				{
					rdo.IsChecked = true;
				}
			}

			foreach (RadioButton rdo in Extensions.FindVisualChildren<RadioButton>(ButtonLayoutGroup))
			{
				if (rdo.Tag.ToString() == Settings.mySettings.ButtonLayoutDefault)
				{
					rdo.IsChecked = true;
				}
			}



			if (Properties.Settings.Default.NightMode)
			{
				DarkModeCheckBox.IsChecked = true;
			}

			CheckGraphicalPresetModeState(null, null);

			SetKeybindTextboxes();
			UpdateCustomColors();

		}

		private void UpdateCustomColors()
		{
			CSAC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Properties.Settings.Default.CollisionSAColour.A, Properties.Settings.Default.CollisionSAColour.R, Properties.Settings.Default.CollisionSAColour.G, Properties.Settings.Default.CollisionSAColour.B));
			SSTOC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Properties.Settings.Default.CollisionTOColour.A, Properties.Settings.Default.CollisionTOColour.R, Properties.Settings.Default.CollisionTOColour.G, Properties.Settings.Default.CollisionTOColour.B));
			CSLRDC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Properties.Settings.Default.CollisionLRDColour.A, Properties.Settings.Default.CollisionLRDColour.R, Properties.Settings.Default.CollisionLRDColour.G, Properties.Settings.Default.CollisionLRDColour.B));
			WLC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Properties.Settings.Default.WaterColorDefault.A, Properties.Settings.Default.WaterColorDefault.R, Properties.Settings.Default.WaterColorDefault.G, Properties.Settings.Default.WaterColorDefault.B));
			GDC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Properties.Settings.Default.GridColorDefault.A, Properties.Settings.Default.GridColorDefault.R, Properties.Settings.Default.GridColorDefault.G, Properties.Settings.Default.GridColorDefault.B));
		}

		private void CheckGraphicalPresetModeState(object sender, EventArgs e)
		{
			minimalRadioButton.IsChecked = false;
			minimalRadioButton2.IsChecked = false;
			basicRadioButton.IsChecked = false;
			basicRadioButton2.IsChecked = false;
			superRadioButton.IsChecked = false;
			superRadioButton2.IsChecked = false;
			hyperRadioButton.IsChecked = false;
			hyperRadioButton2.IsChecked = false;
			customRadioButton.IsChecked = false;
			customRadioButton2.IsChecked = false;


			if (EditorSettings.isMinimalPreset())
			{
				minimalRadioButton.IsChecked = true;
				minimalRadioButton2.IsChecked = true;
			}
			else if (EditorSettings.isBasicPreset())
			{
				basicRadioButton.IsChecked = true;
				basicRadioButton2.IsChecked = true;
			}
			else if (EditorSettings.isSuperPreset())
			{
				superRadioButton.IsChecked = true;
				superRadioButton2.IsChecked = true;
			}
			else if (EditorSettings.isHyperPreset())
			{
				hyperRadioButton.IsChecked = true;
				hyperRadioButton2.IsChecked = true;
			}
			else
			{
				customRadioButton.IsChecked = true;
				customRadioButton2.IsChecked = true;
			}
		}

		private void radioButton1_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (SceneSelectRadio2 != null)
			{
				if (SceneSelectRadio2.IsChecked == true)
				{
					Properties.Settings.Default.IsFilesViewDefault = true;
					Properties.Settings.Default.SceneSelectRadioButton1On = false;
					Properties.Settings.Default.SceneSelectRadioButton2On = true;
				}
				else
				{
					Properties.Settings.Default.IsFilesViewDefault = false;
					Properties.Settings.Default.SceneSelectRadioButton1On = true;
					Properties.Settings.Default.SceneSelectRadioButton2On = false;

				}
			}
		}

		private void radioButton2_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (SceneSelectRadio2 != null)
			{
				if (SceneSelectRadio2.IsChecked == true)
				{
					Properties.Settings.Default.IsFilesViewDefault = true;
					Properties.Settings.Default.SceneSelectRadioButton1On = false;
					Properties.Settings.Default.SceneSelectRadioButton2On = true;
				}
				else
				{
					Properties.Settings.Default.IsFilesViewDefault = false;
					Properties.Settings.Default.SceneSelectRadioButton1On = true;
					Properties.Settings.Default.SceneSelectRadioButton2On = false;

				}
			}

		}

		private void radioButtonY_CheckedChanged_1(object sender, RoutedEventArgs e)
		{
			if (radioButtonY != null)
			{
				if (radioButtonY.IsChecked == true)
				{
					Properties.Settings.Default.ScrollLockDirection = true;
					Properties.Settings.Default.ScrollLockX = false;
					Properties.Settings.Default.ScrollLockY = true;
				}
				else
				{
					Properties.Settings.Default.ScrollLockDirection = false;
					Properties.Settings.Default.ScrollLockX = true;
					Properties.Settings.Default.ScrollLockY = false;

				}
			}
		}

		private void radioButtonX_CheckedChanged_1(object sender, RoutedEventArgs e)
		{
			if (radioButtonY != null)
			{
				if (radioButtonY.IsChecked == true)
				{
					Properties.Settings.Default.ScrollLockDirection = true;
					Properties.Settings.Default.ScrollLockX = false;
					Properties.Settings.Default.ScrollLockY = true;
				}
				else
				{
					Properties.Settings.Default.ScrollLockDirection = false;
					Properties.Settings.Default.ScrollLockX = true;
					Properties.Settings.Default.ScrollLockY = false;

				}
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

		private void ResetControlsToDefault(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to reset your control configuration?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				Properties.KeyBinds.Default.Reset();
			}
			else
			{

			}
		}

		private void RPCCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (Properties.Settings.Default.ShowDiscordRPC == false)
			{
				RPCCheckBox.IsChecked = true;
				Properties.Settings.Default.ShowDiscordRPC = true;
				EditorInstance.UpdateDiscord(EditorInstance.ScenePath);
			}
			else
			{
				RPCCheckBox.IsChecked = false;
				Properties.Settings.Default.ShowDiscordRPC = false;
				EditorInstance.UpdateDiscord();
			}
		}

		private void button5_Click(object sender, RoutedEventArgs e)
		{
			EditorSettings.exportSettings();
		}

		private void importOptionsButton_Click(object sender, RoutedEventArgs e)
		{
			EditorSettings.importSettings();
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

		private void button13_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.Title = "Select SonicMania.exe";
			ofd.Filter = "Windows PE Executable|*.exe";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				Properties.Settings.Default.RunGamePath = ofd.FileName;
		}

		private void button14_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.Title = "Select ManiaModManager.exe";
			ofd.Filter = "Windows PE Executable|*.exe";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				Properties.Settings.Default.RunModLoaderPath = ofd.FileName;

		}

		private void button15_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.Title = "Select RSDK Animation Editor.exe";
			ofd.Filter = "Windows PE Executable|*.exe";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				Properties.Settings.Default.RunAniEdPath = ofd.FileName;
		}

		private void button16_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.Title = "Select TileManiac.exe";
			ofd.Filter = "Windows PE Executable|*.exe";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				Properties.Settings.Default.RunTileManiacPath = ofd.FileName;
		}

		private void button17_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.Title = "Select Color Palette Program (.exe)";
			ofd.Filter = "Windows PE Executable|*.exe";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				Properties.Settings.Default.RunPalleteEditorPath = ofd.FileName;
		}

		private void uncheckOtherGridDefaults(int i)
		{
			switch (i)
			{
				case 1:
					Properties.Settings.Default.x16Default = true;
					Properties.Settings.Default.x128Default = false;
					Properties.Settings.Default.x256Default = false;
					Properties.Settings.Default.CustomGridDefault = false;
					x16checkbox.IsChecked = true;
					x128checkbox.IsChecked = false;
					x256checkbox.IsChecked = false;
					customGridCheckbox.IsChecked = false;
					break;
				case 2:
					Properties.Settings.Default.x16Default = false;
					Properties.Settings.Default.x128Default = true;
					Properties.Settings.Default.x256Default = false;
					Properties.Settings.Default.CustomGridDefault = false;
					x16checkbox.IsChecked = false;
					x128checkbox.IsChecked = true;
					x256checkbox.IsChecked = false;
					customGridCheckbox.IsChecked = false;
					break;
				case 3:
					Properties.Settings.Default.x16Default = false;
					Properties.Settings.Default.x128Default = false;
					Properties.Settings.Default.x256Default = true;
					Properties.Settings.Default.CustomGridDefault = false;
					x16checkbox.IsChecked = false;
					x128checkbox.IsChecked = false;
					x256checkbox.IsChecked = true;
					customGridCheckbox.IsChecked = false;
					break;
				case 4:
					Properties.Settings.Default.x16Default = false;
					Properties.Settings.Default.x128Default = false;
					Properties.Settings.Default.x256Default = false;
					Properties.Settings.Default.CustomGridDefault = true;
					x16checkbox.IsChecked = false;
					x128checkbox.IsChecked = false;
					x256checkbox.IsChecked = false;
					customGridCheckbox.IsChecked = true;
					break;
					/*default:
						//x16checkbox.Checked = true; //Default
						x128checkbox.Checked = false;
						x256checkbox.Checked = false;
						customGridCheckbox.Checked = false;
						break;*/
			}


		}

		private void x16checkbox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (x16checkbox.IsChecked == true)
			{
				uncheckOtherGridDefaults(1);
			}
		}

		private void X128checkbox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (x128checkbox.IsChecked == true)
			{
				uncheckOtherGridDefaults(2);
			}
		}

		private void customGridCheckbox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (customGridCheckbox.IsChecked == true)
			{
				uncheckOtherGridDefaults(4);
			}
		}

		private void x256checkbox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (x256checkbox.IsChecked == true)
			{
				uncheckOtherGridDefaults(3);
			}
		}

		private void preRenderAlways_CheckedChanged(object sender, RoutedEventArgs e)
		{
			preRenderRadioGroupsUpdate(3);
			Properties.Settings.Default.preRenderSceneOption = 3;
			preRenderRadioGroupCheckChangeAllowed = true;
		}

		private void promptForPreRender_CheckedChanged(object sender, RoutedEventArgs e)
		{
			preRenderRadioGroupsUpdate(2);
			Properties.Settings.Default.preRenderSceneOption = 2;
			preRenderRadioGroupCheckChangeAllowed = true;
		}

		private void caseBasedPreRender_CheckedChanged(object sender, RoutedEventArgs e)
		{
			preRenderRadioGroupsUpdate(1);
			Properties.Settings.Default.preRenderSceneOption = 1;
			preRenderRadioGroupCheckChangeAllowed = true;
		}

		private void manualPreRender_CheckedChanged(object sender, RoutedEventArgs e)
		{
			preRenderRadioGroupsUpdate(0);
			Properties.Settings.Default.preRenderSceneOption = 0;
			preRenderRadioGroupCheckChangeAllowed = true;
		}

		private void preRenderRadioGroupsUpdate(int type)
		{
			bool[] groups = new[] { false, false, false, false };
			for (int i = 0; i < 4; i++) if (type == i) groups[i] = true;
			if (preRenderRadioGroupCheckChangeAllowed == true)
			{
				preRenderRadioGroupCheckChangeAllowed = false;
				manualPreRender.IsChecked = false || groups[0];
				caseBasedPreRender.IsChecked = false || groups[1];
				promptForPreRender.IsChecked = false || groups[2];
				preRenderAlways.IsChecked = false || groups[3];
			}

		}


		private void radioButton4_CheckedChanged(object sender, RoutedEventArgs e)
		{
			preRenderRadioGroupsUpdate(1);
			Properties.Settings.Default.CollisionColorsDefault = 0;
			collisionColorsRadioGroupCheckChangeAllowed = true;
		}

		private void radioButton3_CheckedChanged(object sender, RoutedEventArgs e)
		{
			preRenderRadioGroupsUpdate(2);
			Properties.Settings.Default.CollisionColorsDefault = 1;
			collisionColorsRadioGroupCheckChangeAllowed = true;
		}

		private void radioButton1_CheckedChanged_1(object sender, RoutedEventArgs e)
		{
			preRenderRadioGroupsUpdate(3);
			Properties.Settings.Default.CollisionColorsDefault = 2;
			collisionColorsRadioGroupCheckChangeAllowed = true;
		}

		private void collisionColorsRadioGroupUpdate(int type)
		{
			bool[] groups = new[] { false, false, false };
			for (int i = 0; i < 3; i++) if (type == i) groups[i] = true;
			if (collisionColorsRadioGroupCheckChangeAllowed == true)
			{
				collisionColorsRadioGroupCheckChangeAllowed = false;
				radioButton4.IsChecked = false || groups[0];
				radioButton3.IsChecked = false || groups[1];
				radioButton1.IsChecked = false || groups[2];
			}

		}

		private void comboBox8_DropDown(object sender, RoutedEventArgs e)
		{
			//Grid Default Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Properties.Settings.Default.GridColorDefault = colorSelect.Color;
			}
		}

		private void comboBox7_DropDown(object sender, RoutedEventArgs e)
		{
			//Water Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Properties.Settings.Default.WaterColorDefault = colorSelect.Color;
				EditorInstance.waterColor = colorSelect.Color;
			}
		}

		private void comboBox6_DropDown(object sender, RoutedEventArgs e)
		{
			//Collision Solid(Top Only) Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Properties.Settings.Default.CollisionTOColour = colorSelect.Color;
			}
		}

		private void comboBox5_DropDown(object sender, RoutedEventArgs e)
		{
			//Collision Solid(LRD) Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Properties.Settings.Default.CollisionLRDColour = colorSelect.Color;
			}
		}

		private void comboBox4_DropDown(object sender, RoutedEventArgs e)
		{
			//Collision Solid(All) Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Properties.Settings.Default.CollisionSAColour = colorSelect.Color;
			}
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



		public object this[string propertyName]
		{
			get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
			set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
		}

		private void radioButton12_Click(object sender, RoutedEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			if (sender != null) Settings.mySettings.LangDefault = button.Tag.ToString();
		}

		private void SetButtonLayoutDefault(object sender, RoutedEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			if (sender != null) Settings.mySettings.ButtonLayoutDefault = button.Tag.ToString();
		}


		private void SetGraphicalPresetSetting(object sender, RoutedEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			if (sender != null) EditorSettings.ApplyPreset(button.Tag.ToString());
			CheckGraphicalPresetModeState(null, null);
		}

		private void EditKeyCombo(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button)) return;
			Button KeyBind = sender as Button;
			bool state = true;
			if (state)
			{
				string keybindName = KeyBind.Tag.ToString();

				StringCollection keyBindList = Settings.myKeyBinds[keybindName] as StringCollection;

				KeyBindConfigurator keybinder = new KeyBindConfigurator(keybindName);
				keybinder.ShowDialog();
				if (keybinder.DialogResult == true)
				{
					KeysConverter kc = new KeysConverter();
					System.Windows.Forms.Keys keyBindtoSet = keybinder.CurrentBindingKey;
					int keyIndex = keybinder.ListIndex;

					var keybindDict = Settings.myKeyBinds[keybindName] as StringCollection;
					String KeyString = kc.ConvertToString(keyBindtoSet);
					keybindDict.RemoveAt(keyIndex);
					keybindDict.Add(KeyString);
					Settings.myKeyBinds[keybindName] = keybindDict;
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
			foreach (StackPanel stack in Extensions.FindVisualChildren<StackPanel>(MenuItemControls))
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

			var keybindDict = Settings.myKeyBinds[keyRefrence] as StringCollection;
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
	}
}
