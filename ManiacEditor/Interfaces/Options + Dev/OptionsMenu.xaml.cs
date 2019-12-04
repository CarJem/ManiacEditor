using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Collections.Specialized;
using Cyotek.Windows.Forms;
using KeysConverter = System.Windows.Forms.KeysConverter;

namespace ManiacEditor.Interfaces
{
    /// <summary>
    /// Interaction logic for OptionsMenu.xaml
    /// </summary> 
    public partial class OptionsMenu : Window
	{
		bool collisionColorsRadioGroupCheckChangeAllowed = true;
		System.Windows.Forms.Timer CheckGraphicalSettingTimer;


        public OptionsMenu()
		{
			InitializeComponent();

            SetScrollerToggleTypeRadioButtonState(Settings.MySettings.ScrollerPressReleaseMode);
            CheckGraphicalSettingTimer = new System.Windows.Forms.Timer();
			CheckGraphicalSettingTimer.Interval = 10;
			CheckGraphicalSettingTimer.Tick += CheckGraphicalPresetModeState;

			if (Settings.MyDefaults.ScrollLockDirectionDefault == false) radioButtonX.IsChecked = true;
			else radioButtonY.IsChecked = true;

            if (Properties.Internal.Default.PortableMode) PortableCheckbox.IsChecked = true;
            else NonPortableCheckbox.IsChecked = true;

            if (Settings.MyDefaults.SceneSelectFilesViewDefault) SceneSelectRadio2.IsChecked = true;
            else SceneSelectRadio1.IsChecked = true;

            collisionColorsRadioGroupUpdate(Settings.MyDefaults.DefaultCollisionColors);
			collisionColorsRadioGroupCheckChangeAllowed = true;
			if (Settings.MyDefaults.DefaultGridSizeOption == 0) uncheckOtherGridDefaults(1);
			if (Settings.MyDefaults.DefaultGridSizeOption == 1) uncheckOtherGridDefaults(2);
			if (Settings.MyDefaults.DefaultGridSizeOption == 2) uncheckOtherGridDefaults(3);
			if (Settings.MyDefaults.DefaultGridSizeOption == 3) uncheckOtherGridDefaults(4);

            if (Properties.Internal.Default.PortableMode) PortableCheckbox.IsChecked = true;
            else NonPortableCheckbox.IsChecked = true;

			foreach (RadioButton rdo in Extensions.FindVisualChildren<RadioButton>(MenuLangGroup))
			{
				if (rdo.Tag.ToString() == Settings.MyDefaults.MenuLanguageDefault)
				{
					rdo.IsChecked = true;
				}
			}

			foreach (RadioButton rdo in Extensions.FindVisualChildren<RadioButton>(ButtonLayoutGroup))
			{
				if (rdo.Tag.ToString() == Settings.MyDefaults.MenuButtonLayoutDefault)
				{
					rdo.IsChecked = true;
				}
			}



			if (Settings.MySettings.NightMode)
			{
				DarkModeCheckBox.IsChecked = true;
			}

			CheckGraphicalPresetModeState(null, null);

			SetAllKeybindTextboxes();
			UpdateCustomColors();

            switch (Properties.Defaults.Default.TileManiacListSetting)
            {
                case 0:
                    collisionListRadioButton.IsChecked = true;
                    break;
                case 1:
                    tileListRadioButton.IsChecked = true;
                    break;
            }
            switch (Properties.Defaults.Default.TileManiacViewAppearanceMode)
            {
                case 0:
                    overlayEditorViewRadioButton.IsChecked = true;
                    break;
                case 1:
                    collisionEditorViewRadioButton.IsChecked = true;
                    break;
            }
            switch (Properties.Defaults.Default.TileManiacRenderViewerSetting)
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

        }

		private void UpdateCustomColors()
		{
			CSAC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Settings.MyDefaults.CollisionSAColour.A, Settings.MyDefaults.CollisionSAColour.R, Settings.MyDefaults.CollisionSAColour.G, Settings.MyDefaults.CollisionSAColour.B));
			SSTOC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Settings.MyDefaults.CollisionTOColour.A, Settings.MyDefaults.CollisionTOColour.R, Settings.MyDefaults.CollisionTOColour.G, Settings.MyDefaults.CollisionTOColour.B));
			CSLRDC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Settings.MyDefaults.CollisionLRDColour.A, Settings.MyDefaults.CollisionLRDColour.R, Settings.MyDefaults.CollisionLRDColour.G, Settings.MyDefaults.CollisionLRDColour.B));
			WLC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Settings.MyDefaults.WaterEntityColorDefault.A, Settings.MyDefaults.WaterEntityColorDefault.R, Settings.MyDefaults.WaterEntityColorDefault.G, Settings.MyDefaults.WaterEntityColorDefault.B));
			GDC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Settings.MyDefaults.DefaultGridColor.A, Settings.MyDefaults.DefaultGridColor.R, Settings.MyDefaults.DefaultGridColor.G, Settings.MyDefaults.DefaultGridColor.B));
		}

		private void CheckGraphicalPresetModeState(object sender, EventArgs e)
		{
			minimalRadioButton2.IsChecked = false;
			basicRadioButton2.IsChecked = false;
			superRadioButton2.IsChecked = false;
			hyperRadioButton2.IsChecked = false;
			customRadioButton2.IsChecked = false;


			if (EditorSettings.isMinimalPreset())
			{
				minimalRadioButton2.IsChecked = true;
			}
			else if (EditorSettings.isBasicPreset())
			{
				basicRadioButton2.IsChecked = true;
			}
			else if (EditorSettings.isSuperPreset())
			{
				superRadioButton2.IsChecked = true;
			}
			else if (EditorSettings.isHyperPreset())
			{
				hyperRadioButton2.IsChecked = true;
			}
			else
			{
				customRadioButton2.IsChecked = true;
			}
		}

		private void radioButton1_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (SceneSelectRadio2 != null)
			{
				if (SceneSelectRadio2.IsChecked == true)
				{
					Settings.MyDefaults.SceneSelectFilesViewDefault = true;
				}
				else
				{
					Settings.MyDefaults.SceneSelectFilesViewDefault = false;

				}
			}
		}

		private void radioButton2_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (SceneSelectRadio2 != null)
			{
				if (SceneSelectRadio2.IsChecked == true)
				{
					Settings.MyDefaults.SceneSelectFilesViewDefault = true;
				}
				else
				{
					Settings.MyDefaults.SceneSelectFilesViewDefault = false;

				}
			}

		}

		private void radioButtonY_CheckedChanged_1(object sender, RoutedEventArgs e)
		{
			if (radioButtonY != null)
			{
				if (radioButtonY.IsChecked == true)
				{
					Settings.MyDefaults.ScrollLockDirectionDefault = true;
				}
				else
				{
					Settings.MyDefaults.ScrollLockDirectionDefault = false;

				}
			}
		}

		private void radioButtonX_CheckedChanged_1(object sender, RoutedEventArgs e)
		{
			if (radioButtonX != null)
			{
				if (radioButtonX.IsChecked == true)
				{
					Settings.MyDefaults.ScrollLockDirectionDefault = false;
				}
				else
				{
					Settings.MyDefaults.ScrollLockDirectionDefault = true;

				}
			}

		}

        private void ResetToDefault(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to wipe your settings? (This is includes all of your Keybinds, Data Directories, Defaults and so on...)", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                EditorConstants.ResetAllSettings();
            }
        }

		private void RPCCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (Settings.MySettings.ShowDiscordRPC == false)
			{
				RPCCheckBox.IsChecked = true;
				Settings.MySettings.ShowDiscordRPC = true;
				Editor.Instance.Discord.UpdateDiscord(Editor.Instance.Discord.ScenePath);
			}
			else
			{
				RPCCheckBox.IsChecked = false;
				Settings.MySettings.ShowDiscordRPC = false;
				Editor.Instance.Discord.UpdateDiscord();
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
            EditorConstants.SaveAllSettings();
            this.DialogResult = true;
        }

        private void ModLoader_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = "Select Mania Mod Manager.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Settings.MyDefaults.ModLoaderPath = ofd.FileName;
        }

        private void SonicMania_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = "Select Sonic Mania.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Settings.MyDefaults.SonicManiaPath = ofd.FileName;
        }

        private void button15_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.Title = "Select RSDK Animation Editor.exe";
			ofd.Filter = "Windows PE Executable|*.exe";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				Settings.MyDefaults.AnimationEditorPath = ofd.FileName;
		}

		private void uncheckOtherGridDefaults(int i)
		{
			switch (i)
			{
				case 1:
                    Settings.MyDefaults.DefaultGridSizeOption = 0;
					x16checkbox.IsChecked = true;
					x128checkbox.IsChecked = false;
					x256checkbox.IsChecked = false;
					customGridCheckbox.IsChecked = false;
					break;
				case 2:
                    Settings.MyDefaults.DefaultGridSizeOption = 1;
                    x16checkbox.IsChecked = false;
					x128checkbox.IsChecked = true;
					x256checkbox.IsChecked = false;
					customGridCheckbox.IsChecked = false;
					break;
				case 3:
                    Settings.MyDefaults.DefaultGridSizeOption = 2;
                    x16checkbox.IsChecked = false;
					x128checkbox.IsChecked = false;
					x256checkbox.IsChecked = true;
					customGridCheckbox.IsChecked = false;
					break;
				case 4:
                    Settings.MyDefaults.DefaultGridSizeOption = 3;
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
				Settings.MyDefaults.DefaultGridColor = colorSelect.Color;
                UpdateCustomColors();
            }
        }

		private void comboBox7_DropDown(object sender, RoutedEventArgs e)
		{
			//Water Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Settings.MyDefaults.WaterEntityColorDefault = colorSelect.Color;
				Editor.Instance.UIModes.waterColor = colorSelect.Color;
			}
		}

		private void comboBox6_DropDown(object sender, RoutedEventArgs e)
		{
			//Collision Solid(Top Only) Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Settings.MyDefaults.CollisionTOColour = colorSelect.Color;
                UpdateCustomColors();
            }
        }

		private void comboBox5_DropDown(object sender, RoutedEventArgs e)
		{
			//Collision Solid(LRD) Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Settings.MyDefaults.CollisionLRDColour = colorSelect.Color;
                UpdateCustomColors();
            }
        }

		private void comboBox4_DropDown(object sender, RoutedEventArgs e)
		{
			//Collision Solid(All) Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Settings.MyDefaults.CollisionSAColour = colorSelect.Color;
                UpdateCustomColors();
            }
        }

		private void OptionBox_FormClosing(object sender, CancelEventArgs e)
		{
            /*
            if (checkBox15.Checked && !Settings.mySettings.NightMode)
            {
                DialogResult result = MessageBox.Show("To apply this setting correctly, you will have to restart the editor, would you like to that now?", "Restart to Apply", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Settings.mySettings.NightMode = true;
                    Settings.mySettings.Save();
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
            else if (!checkBox15.Checked && Settings.mySettings.NightMode)
            {
                DialogResult result = MessageBox.Show("To apply this setting correctly, you will have to restart the editor, would you like to that now?", "Restart to Apply", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Settings.mySettings.NightMode = false;
                    Settings.mySettings.Save();
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
            */
            EditorConstants.ReloadAllSettings();

            if (DarkModeCheckBox.IsChecked == true && !Settings.MySettings.NightMode)
			{
				Settings.MySettings.NightMode = true;
				Settings.MySettings.Save();
				App.ChangeSkin(Skin.Dark);
				App.SkinChanged = true;
                Editor.Instance.Theming.RefreshTheme();

			}
			else if (!DarkModeCheckBox.IsChecked == true && Settings.MySettings.NightMode)
			{
				Settings.MySettings.NightMode = false;
				Settings.MySettings.Save();
				App.ChangeSkin(Skin.Light);
				App.SkinChanged = true;
                Editor.Instance.Theming.RefreshTheme();

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
			if (sender != null) Settings.MyDefaults.MenuLanguageDefault = button.Tag.ToString();
		}

		private void SetButtonLayoutDefault(object sender, RoutedEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			if (sender != null) Settings.MyDefaults.MenuButtonLayoutDefault = button.Tag.ToString();
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

				StringCollection keyBindList = Settings.MyKeyBinds[keybindName] as StringCollection;

				KeyBindConfigurator keybinder = new KeyBindConfigurator(keybindName);
				keybinder.ShowDialog();
				if (keybinder.DialogResult == true)
				{
					KeysConverter kc = new KeysConverter();
					System.Windows.Forms.Keys keyBindtoSet = keybinder.CurrentBindingKey;
					int keyIndex = keybinder.ListIndex;

					var keybindDict = Settings.MyKeyBinds[keybindName] as StringCollection;
					String KeyString = kc.ConvertToString(keyBindtoSet);
					keybindDict.RemoveAt(keyIndex);
					keybindDict.Add(KeyString);
					Settings.MyKeyBinds[keybindName] = keybindDict;
				}
			}
			SetAllKeybindTextboxes();

		}
		List<string> AllKeyBinds = new List<string>();
		List<string> KnownKeybinds = new List<string>();

		private void RefreshKeybindList()
		{
			AllKeyBinds.Clear();
			AllKeyBinds = new List<string>();
			KnownKeybinds.Clear();
			KnownKeybinds = new List<string>();
			foreach (SettingsProperty currentProperty in Settings.MyKeyBinds.Properties)
			{
				KnownKeybinds.Add(currentProperty.Name);
			}
			foreach (string keybind in KnownKeybinds)
			{
				if (!Extensions.KeyBindsSettingExists(keybind)) continue;
				var keybindDict = Settings.MyKeyBinds[keybind] as StringCollection;
				if (keybindDict != null && keybindDict.Count != 0)
				{
					foreach (string item in keybindDict)
					{
						AllKeyBinds.Add(item);
					}
				}

			}
		}
        private void SetAllKeybindTextboxes()
        {
            RefreshKeybindList();
            SetKeybindTextboxes(ControlsPage);
            SetKeybindTextboxes(TileManiacOptions1);
            SetKeybindTextboxes(TileManiacOptions2);
            SetKeybindTextboxes(TileManiacOptions3);
            SetKeybindTextboxes(TileManiacOptions4);
            SetKeybindTextboxes(TileManiacOptions5);
            SetKeybindTextboxes(TileManiacOptions6);

        }

        private void SetKeybindTextboxes(StackPanel panel)
        {
            foreach (StackPanel stack in Extensions.FindVisualChildren<StackPanel>(panel))
            {
                foreach (Button t in Extensions.FindVisualChildren<Button>(stack))
                {
                    ProcessKeybindingButtons(t);
                }
            }
        }

		private void ProcessKeybindingButtons(Button t)
		{
            if (t.Tag != null && t.Tag.ToString() == "LOCK") return;
			t.Foreground = (SolidColorBrush)FindResource("NormalText");
			if (t.Tag != null)
			{
				System.Tuple<string,string> tuple = KeyBindPraser(t.Tag.ToString());
				t.Content = tuple.Item1;
				if (tuple.Item2 != null)
				{
					if (HasSingleMultipleOccurances(t.Tag.ToString())) t.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
					else t.Foreground = (SolidColorBrush)FindResource("NormalText");
					ToolTipService.SetShowOnDisabled(t, true);
					t.ToolTip = tuple.Item2;
				}
				else
				{
					if (HasMultipleOccurances(tuple.Item1)) t.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
					else t.Foreground = (SolidColorBrush)FindResource("NormalText");
				}
			}
		}

		public bool HasMultipleOccurances(String targetBinding)
		{
			if (targetBinding == "None" || targetBinding == "N/A") return false;
			int occurances = 0;
			foreach (string currentBinding in AllKeyBinds)
			{
				if (targetBinding == currentBinding) occurances++;
			}
			if (occurances < 2) return false;
			else return true;
		}

		public bool HasSingleMultipleOccurances(String keyRefrence)
		{		
			List<string> keyBindList = new List<string>();
			List<string> keyBindDuplicatesList = new List<string>();

			var keybindDict = Settings.MyKeyBinds[keyRefrence] as StringCollection;
			if (keybindDict != null) keyBindList = keybindDict.Cast<string>().ToList();
			if (keyBindList != null)
			{
				foreach (string keybind in keyBindList)
				{
					if (HasMultipleOccurances(keybind)) keyBindDuplicatesList.Add(keybind);
				}
			}
			if (keyBindDuplicatesList.Count < 2) return false;
			else return true;
			

		}

		private Tuple<string,string> KeyBindPraser(string keyRefrence)
		{
			if (keyRefrence == "NULL") return new Tuple<string, string>("N/A", null);

			List<string> keyBindList = new List<string>();
			List<string> keyBindModList = new List<string>();

			if (!Extensions.KeyBindsSettingExists(keyRefrence)) return new Tuple<string, string>("N/A", null);

			var keybindDict = Settings.MyKeyBinds[keyRefrence] as StringCollection;
			if (keybindDict != null)
			{
				keyBindList = keybindDict.Cast<string>().ToList();
			}
			else
			{
				return new Tuple<string, string>("N/A",null);
			}

			if (keyBindList == null)
			{
				return new Tuple<string, string>("N/A", null);
			}

			if (keyBindList.Count > 1)
			{
				string tooltip = "Possible Combos for this Keybind:";
				foreach (string keyBind in keyBindList)
				{
					tooltip += Environment.NewLine + keyBind;
				}
				return new Tuple<string, string>(string.Format("{0} Keybinds", keyBindList.Count), tooltip);
			}
			else if ((keyBindList.Count == 1))
			{
				return new Tuple<string, string>(keyBindList[0], null);
			}
			else
			{
				return new Tuple<string, string>("N/A", null);
			}

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
            EditorConstants.ReloadAllSettings();
			this.DialogResult = true;
		}



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CheatCodeManager cheatCodeManager = new CheatCodeManager();
            cheatCodeManager.Owner = this;
            cheatCodeManager.ShowDialog();
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        bool settingsTypeChangeLock = false;
        private void DefaultSaveLocationChanged(object sender, RoutedEventArgs e)
        {
            if (settingsTypeChangeLock == false)
            {
                if (PortableCheckbox.IsChecked == true)
                {
                    settingsTypeChangeLock = true;
                    if (MessageBox.Show("To apply this setting, the application must close. Would you like to continue?", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        Properties.Internal.Default.PortableMode = true;
                        Properties.Internal.Default.Save();
                        Environment.Exit(0);
                    }
                    else
                    {
                        PortableCheckbox.IsChecked = false;
                        NonPortableCheckbox.IsChecked = true;
                    }

                    settingsTypeChangeLock = false;
                }
                else
                {
                    settingsTypeChangeLock = true;
                    if (MessageBox.Show("To apply this setting, the application must close. Would you like to continue?", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        Properties.Internal.Default.PortableMode = false;
                        Properties.Internal.Default.Save();
                        Environment.Exit(0);
                    }
                    else
                    {
                        PortableCheckbox.IsChecked = true;
                        NonPortableCheckbox.IsChecked = false;
                    }

                    settingsTypeChangeLock = false;
                }
            }

        }

        private void DataDirectoriesExport_Click(object sender, RoutedEventArgs e)
        {

            if (Settings.MySettings.DataDirectories != null && Settings.MySettings.DataDirectories.Count >= 1)
            {
                string[] output = new string[Settings.MySettings.DataDirectories.Count];
                Settings.MySettings.DataDirectories.CopyTo(output, 0);
                FolderSelectDialog fsd = new FolderSelectDialog();
                fsd.InitialDirectory = EditorConstants.SettingsPortableDirectory;
                fsd.Title = "Select a Place to Save the Output";
                if (fsd.ShowDialog() == true)
                {
                    string result = System.IO.Path.Combine(fsd.FileName, "DataFolderExport.txt");
                    if (!System.IO.File.Exists(result)) System.IO.File.Create(result).Close();
                    System.IO.File.WriteAllLines(result, output);
                }

            }

        }

        private void SavedPlacesExport_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.MySettings.SavedPlaces != null && Settings.MySettings.SavedPlaces.Count >= 1)
            {
                string[] output = new string[Settings.MySettings.SavedPlaces.Count];
                Settings.MySettings.SavedPlaces.CopyTo(output, 0);
                FolderSelectDialog fsd = new FolderSelectDialog();
                fsd.InitialDirectory = EditorConstants.SettingsPortableDirectory;
                fsd.Title = "Select a Place to Save the Output";
                if (fsd.ShowDialog() == true)
                {
                    string result = System.IO.Path.Combine(fsd.FileName, "SavedPlacesExport.txt");
                    if (!System.IO.File.Exists(result)) System.IO.File.Create(result).Close();
                    System.IO.File.WriteAllLines(result, output);
                }

            }
        }

        private void DataDirectoriesImport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "DataFolderExport"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                List<string> input = System.IO.File.ReadAllLines(filename).ToList();
                foreach (var entry in input)
                {
                    if (!Settings.MySettings.DataDirectories.Contains(entry)) Settings.MySettings.DataDirectories.Add(entry);
                }

            }
        }

        private void SavedPlacesImport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "SavedPlacesExport"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                List<string> input = System.IO.File.ReadAllLines(filename).ToList();
                foreach (var entry in input)
                {
                    if (!Settings.MySettings.SavedPlaces.Contains(entry)) Settings.MySettings.SavedPlaces.Add(entry);
                }

            }
        }

        private void ScrollerToggleTypeClickEvent(object sender, RoutedEventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button == ScrollerToggleModeClickButton)
            {
                SetScrollerToggleTypeRadioButtonState(false);
                Settings.MySettings.ScrollerPressReleaseMode = false;
            }
            else
            {
                SetScrollerToggleTypeRadioButtonState(true);
                Settings.MySettings.ScrollerPressReleaseMode = true;
            }
        }

        private void SetScrollerToggleTypeRadioButtonState(bool enabled)
        {
            ScrollerToggleModeClickButton.IsChecked = !enabled;
            ScrollerToggleModePressReleaseButton.IsChecked = enabled;
        }

        #region Tile Maniac Settings

        private void ListViewModeChanged(object sender, RoutedEventArgs e)
        {
            tileListRadioButton.IsChecked = false;
            collisionListRadioButton.IsChecked = false;

            if (e.Source == tileListRadioButton)
            {
                Properties.Defaults.Default.TileManiacListSetting = 1;
                tileListRadioButton.IsChecked = true;
            }
            else if (e.Source == collisionListRadioButton)
            {
                Properties.Defaults.Default.TileManiacListSetting = 0;
                collisionListRadioButton.IsChecked = true;
            }
        }

        private void EditorViewModeChanged(object sender, RoutedEventArgs e)
        {
            collisionEditorViewRadioButton.IsChecked = false;
            overlayEditorViewRadioButton.IsChecked = false;

            if (e.Source == collisionEditorViewRadioButton)
            {
                Properties.Defaults.Default.TileManiacViewAppearanceMode = 1;
                collisionEditorViewRadioButton.IsChecked = true;
            }
            else if (e.Source == overlayEditorViewRadioButton)
            {
                Properties.Defaults.Default.TileManiacViewAppearanceMode = 0;
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
                Properties.Defaults.Default.TileManiacRenderViewerSetting = 0;
                tileRenderViewRadioButton.IsChecked = true;
            }
            else if (e.Source == collisionRenderViewRadioButton)
            {
                Properties.Defaults.Default.TileManiacRenderViewerSetting = 1;
                collisionRenderViewRadioButton.IsChecked = true;
            }
            else if (e.Source == overlayRenderViewRadioButton)
            {
                Properties.Defaults.Default.TileManiacRenderViewerSetting = 2;
                overlayRenderViewRadioButton.IsChecked = true;
            }
        }

        #endregion
    }
}
