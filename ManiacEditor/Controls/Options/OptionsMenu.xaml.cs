﻿using System;
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
using ManiacEditor.Extensions;

using ManiacEditor.Controls.Global.Controls;
using ManiacEditor.Controls.Misc;
using ManiacEditor.Controls.Misc.Configuration;
using ManiacEditor.Controls.Misc.Dev;

using GenerationsLib.WPF.Themes;

namespace ManiacEditor.Controls.Options
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

            SetScrollerToggleTypeRadioButtonState(Properties.Settings.MySettings.ScrollerPressReleaseMode);
			SetStartScreenStyle(Properties.Settings.MySettings.UseClassicStartScreen);

			if (Properties.Settings.MyDefaults.ScrollLockDirectionDefault == false) radioButtonX.IsChecked = true;
			else radioButtonY.IsChecked = true;

            if (Properties.Settings.MyInternalSettings.PortableMode) PortableCheckbox.IsChecked = true;
            else NonPortableCheckbox.IsChecked = true;

            if (Properties.Settings.MyDefaults.SceneSelectFilesViewDefault) SceneSelectRadio2.IsChecked = true;
            else SceneSelectRadio1.IsChecked = true;

            collisionColorsRadioGroupUpdate(Properties.Settings.MyDefaults.DefaultCollisionColors);
			collisionColorsRadioGroupCheckChangeAllowed = true;
			if (Properties.Settings.MyDefaults.DefaultGridSizeOption == 0) uncheckOtherGridDefaults(1);
			if (Properties.Settings.MyDefaults.DefaultGridSizeOption == 1) uncheckOtherGridDefaults(2);
			if (Properties.Settings.MyDefaults.DefaultGridSizeOption == 2) uncheckOtherGridDefaults(3);
			if (Properties.Settings.MyDefaults.DefaultGridSizeOption == 3) uncheckOtherGridDefaults(4);

            if (Properties.Settings.MyInternalSettings.PortableMode) PortableCheckbox.IsChecked = true;
            else NonPortableCheckbox.IsChecked = true;

			UserThemeComboBox.SelectedIndex = (int)Properties.Settings.MySettings.UserTheme;

			SetAllKeybindTextboxes();
			UpdateCustomColors();
        }

		private void UpdateCustomColors()
		{
			CSAC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Properties.Settings.MyDefaults.CollisionSAColour.A, Properties.Settings.MyDefaults.CollisionSAColour.R, Properties.Settings.MyDefaults.CollisionSAColour.G, Properties.Settings.MyDefaults.CollisionSAColour.B));
			SSTOC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Properties.Settings.MyDefaults.CollisionTOColour.A, Properties.Settings.MyDefaults.CollisionTOColour.R, Properties.Settings.MyDefaults.CollisionTOColour.G, Properties.Settings.MyDefaults.CollisionTOColour.B));
			CSLRDC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Properties.Settings.MyDefaults.CollisionLRDColour.A, Properties.Settings.MyDefaults.CollisionLRDColour.R, Properties.Settings.MyDefaults.CollisionLRDColour.G, Properties.Settings.MyDefaults.CollisionLRDColour.B));
			WLC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Properties.Settings.MyDefaults.WaterEntityColorDefault.A, Properties.Settings.MyDefaults.WaterEntityColorDefault.R, Properties.Settings.MyDefaults.WaterEntityColorDefault.G, Properties.Settings.MyDefaults.WaterEntityColorDefault.B));
			GDC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Properties.Settings.MyDefaults.DefaultGridColor.A, Properties.Settings.MyDefaults.DefaultGridColor.R, Properties.Settings.MyDefaults.DefaultGridColor.G, Properties.Settings.MyDefaults.DefaultGridColor.B));
		}

		private void radioButton1_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (SceneSelectRadio2 != null)
			{
				if (SceneSelectRadio2.IsChecked == true)
				{
					Properties.Settings.MyDefaults.SceneSelectFilesViewDefault = true;
				}
				else
				{
					Properties.Settings.MyDefaults.SceneSelectFilesViewDefault = false;

				}
			}
		}

		private void radioButton2_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (SceneSelectRadio2 != null)
			{
				if (SceneSelectRadio2.IsChecked == true)
				{
					Properties.Settings.MyDefaults.SceneSelectFilesViewDefault = true;
				}
				else
				{
					Properties.Settings.MyDefaults.SceneSelectFilesViewDefault = false;

				}
			}

		}

		private void radioButtonY_CheckedChanged_1(object sender, RoutedEventArgs e)
		{
			if (radioButtonY != null)
			{
				if (radioButtonY.IsChecked == true)
				{
					Properties.Settings.MyDefaults.ScrollLockDirectionDefault = true;
				}
				else
				{
					Properties.Settings.MyDefaults.ScrollLockDirectionDefault = false;

				}
			}
		}

		private void radioButtonX_CheckedChanged_1(object sender, RoutedEventArgs e)
		{
			if (radioButtonX != null)
			{
				if (radioButtonX.IsChecked == true)
				{
					Properties.Settings.MyDefaults.ScrollLockDirectionDefault = false;
				}
				else
				{
					Properties.Settings.MyDefaults.ScrollLockDirectionDefault = true;

				}
			}

		}

        private void ResetToDefault(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to wipe your settings? (This is includes all of your Keybinds, Data Directories, Defaults and so on...)", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Properties.Settings.ResetAllSettings();
            }
        }

		private void RPCCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (Properties.Settings.MySettings.ShowDiscordRPC == false)
			{
				RPCCheckBox.IsChecked = true;
				Properties.Settings.MySettings.ShowDiscordRPC = true;
				DiscordRP.UpdateDiscord(ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.ToString());
			}
			else
			{
				RPCCheckBox.IsChecked = false;
				Properties.Settings.MySettings.ShowDiscordRPC = false;
				DiscordRP.UpdateDiscord();
			}
		}

		private void button5_Click(object sender, RoutedEventArgs e)
		{
			Methods.Internal.Settings.exportSettings();
		}

		private void importOptionsButton_Click(object sender, RoutedEventArgs e)
		{
			Methods.Internal.Settings.importSettings();
		}

		private void button11_Click(object sender, RoutedEventArgs e)
		{
            Properties.Settings.SaveAllSettings();
            this.DialogResult = true;
        }

		private void uncheckOtherGridDefaults(int i)
		{
			switch (i)
			{
				case 1:
                    Properties.Settings.MyDefaults.DefaultGridSizeOption = 0;
					x16checkbox.IsChecked = true;
					x128checkbox.IsChecked = false;
					x256checkbox.IsChecked = false;
					customGridCheckbox.IsChecked = false;
					break;
				case 2:
                    Properties.Settings.MyDefaults.DefaultGridSizeOption = 1;
                    x16checkbox.IsChecked = false;
					x128checkbox.IsChecked = true;
					x256checkbox.IsChecked = false;
					customGridCheckbox.IsChecked = false;
					break;
				case 3:
                    Properties.Settings.MyDefaults.DefaultGridSizeOption = 2;
                    x16checkbox.IsChecked = false;
					x128checkbox.IsChecked = false;
					x256checkbox.IsChecked = true;
					customGridCheckbox.IsChecked = false;
					break;
				case 4:
                    Properties.Settings.MyDefaults.DefaultGridSizeOption = 3;
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
				Properties.Settings.MyDefaults.DefaultGridColor = colorSelect.Color;
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
				Properties.Settings.MyDefaults.WaterEntityColorDefault = colorSelect.Color;
				Methods.Solution.SolutionState.Main.waterColor = colorSelect.Color;
			}
		}

		private void comboBox6_DropDown(object sender, RoutedEventArgs e)
		{
			//Collision Solid(Top Only) Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Properties.Settings.MyDefaults.CollisionTOColour = colorSelect.Color;
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
				Properties.Settings.MyDefaults.CollisionLRDColour = colorSelect.Color;
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
				Properties.Settings.MyDefaults.CollisionSAColour = colorSelect.Color;
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
            Properties.Settings.ReloadAllSettings();
			var selected = ApplySkinFromSelectedIndex();

			if (Properties.Settings.MySettings.UserTheme != selected)
			{

				Properties.Settings.MySettings.UserTheme = selected;
				Classes.Options.GeneralSettings.Save();
				GenerationsLib.WPF.Themes.SkinResourceDictionary.ChangeSkin(selected, ManiacEditor.App.Current.Resources.MergedDictionaries);
				Methods.Internal.Theming.RefreshTheme();
			}



        }

		private Skin ApplySkinFromSelectedIndex()
		{
			switch (UserThemeComboBox.SelectedIndex)
			{
				case 0:
					return Skin.Light;
				case 1:
					return Skin.Dark;
				case 2:
					return Skin.Beta;
				case 3:
					return Skin.Shard;
				case 4:
					return Skin.CarJem;
				case 5:
					return Skin.Gamma;
				case 6:
					return Skin.Sparks;
				default:
					return Skin.Light;
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
			if (sender != null) Properties.Settings.MyDefaults.MenuLanguageDefault = button.Tag.ToString();
		}

		private void SetButtonLayoutDefault(object sender, RoutedEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			if (sender != null) Properties.Settings.MyDefaults.MenuButtonLayoutDefault = button.Tag.ToString();
		}

        #region Keybinding

        private void EditKeyCombo(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button)) return;
			Button KeyBind = sender as Button;
			bool state = true;
			if (state)
			{
				string keybindName = KeyBind.Tag.ToString();

				List<string> keyBindList = Properties.Settings.MyKeyBinds.GetInput(keybindName) as List<string>;

				KeyBindConfigurator keybinder = new KeyBindConfigurator(keybindName);
				keybinder.ShowDialog();
				if (keybinder.DialogResult == true)
				{
					KeysConverter kc = new KeysConverter();
					System.Windows.Forms.Keys keyBindtoSet = keybinder.CurrentBindingKey;
					int keyIndex = keybinder.ListIndex;

					var keybindDict = Properties.Settings.MyKeyBinds.GetInput(keybindName) as List<string>;
					String KeyString = kc.ConvertToString(keyBindtoSet);
					keybindDict.RemoveAt(keyIndex);
					keybindDict.Add(KeyString);
					Properties.Settings.MyKeyBinds.SetInput(keybindName , keybindDict);
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
			foreach (var currentProperty in Properties.Settings.MyKeyBinds.GetType().GetProperties())
			{
				KnownKeybinds.Add(currentProperty.Name);
			}
			foreach (string keybind in KnownKeybinds)
			{
				if (!Extensions.SpecialExtensions.KeyBindsSettingExists(keybind)) continue;
				var keybindDict = Properties.Settings.MyKeyBinds.GetInput(keybind) as List<string>;
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
      }



        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
		{
            Properties.Settings.ReloadAllSettings();
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
				if (sender == PortableCheckbox && !Properties.Settings.MyInternalSettings.PortableMode)
				{
					if (Classes.Options.InternalSwitches.TestPortableModeEligibilty() == true)
					{
						bool result = MessageBox.Show("To apply this setting, the application must close. Would you like to continue?", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes;
						if (result == true) SetPortableModeState(true);
						else
						{
							ResetToCurrent();
						}
					}
				}
				else if (sender == NonPortableCheckbox && Properties.Settings.MyInternalSettings.PortableMode)
				{
					bool result = MessageBox.Show("To apply this setting, the application must close. Would you like to continue?", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes;
					if (result == true) SetStaticModeState(true);
					else
					{
						ResetToCurrent();
					}
				}			
			}

			void ResetToCurrent()
			{
				if (Properties.Settings.MyInternalSettings.PortableMode == true)
				{
					SetPortableModeState();
				}
				else
				{
					SetStaticModeState();
				}
			}
        }

		private void SetPortableModeState(bool saveToFile = false)
		{
			settingsTypeChangeLock = true;
			PortableCheckbox.IsChecked = true;
			NonPortableCheckbox.IsChecked = false;
			settingsTypeChangeLock = false;
			
			if (saveToFile)
			{
				Properties.Settings.MyInternalSettings.PortableMode = true;
				Classes.Options.InternalSwitches.Save();
				Environment.Exit(0);
			}
		}
		private void SetStaticModeState(bool saveToFile = false)
		{
			settingsTypeChangeLock = true;
			PortableCheckbox.IsChecked = false;
			NonPortableCheckbox.IsChecked = true;
			settingsTypeChangeLock = false;

			if (saveToFile)
			{
				Properties.Settings.MyInternalSettings.PortableMode = false;
				Classes.Options.InternalSwitches.Save();
				Environment.Exit(0);
			}
		}

        private void DataDirectoriesExport_Click(object sender, RoutedEventArgs e)
        {

            if (Classes.Prefrences.CommonPathsStorage.Collection.SavedDataDirectories != null && Classes.Prefrences.CommonPathsStorage.Collection.SavedDataDirectories.Count >= 1)
            {
                string[] output = new string[Classes.Prefrences.CommonPathsStorage.Collection.SavedDataDirectories.Count];
                Classes.Prefrences.CommonPathsStorage.Collection.SavedDataDirectories.CopyTo(output, 0);
				GenerationsLib.Core.FolderSelectDialog fsd = new GenerationsLib.Core.FolderSelectDialog();
                fsd.InitialDirectory = Methods.ProgramPaths.SettingsPortableDirectory;
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
            if (Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces != null && Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces.Count >= 1)
            {
                string[] output = new string[Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces.Count];
                Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces.CopyTo(output, 0);
				GenerationsLib.Core.FolderSelectDialog fsd = new GenerationsLib.Core.FolderSelectDialog();
                fsd.InitialDirectory = Methods.ProgramPaths.SettingsPortableDirectory;
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
                    if (!Classes.Prefrences.CommonPathsStorage.Collection.SavedDataDirectories.Contains(entry)) Classes.Prefrences.CommonPathsStorage.Collection.SavedDataDirectories.Add(entry);
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
                    if (!Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces.Contains(entry)) Classes.Prefrences.CommonPathsStorage.Collection.SavedPlaces.Add(entry);
                }

            }
        }

        private void ScrollerToggleTypeClickEvent(object sender, RoutedEventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button == ScrollerToggleModeClickButton)
            {
                SetScrollerToggleTypeRadioButtonState(false);
                Properties.Settings.MySettings.ScrollerPressReleaseMode = false;
            }
            else
            {
                SetScrollerToggleTypeRadioButtonState(true);
                Properties.Settings.MySettings.ScrollerPressReleaseMode = true;
            }
        }


		private void SetScrollerToggleTypeRadioButtonState(bool enabled)
        {
            ScrollerToggleModeClickButton.IsChecked = !enabled;
            ScrollerToggleModePressReleaseButton.IsChecked = enabled;

		}


		#region Start Screen Style
		private void ModernStartScreenRadioButton_Click(object sender, RoutedEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			if (button == ModernStartScreenRadioButton)
			{
				SetStartScreenStyle(false);
				Properties.Settings.MySettings.UseClassicStartScreen = false;
			}
			else
			{
				SetStartScreenStyle(true);
				Properties.Settings.MySettings.UseClassicStartScreen = true;
			}
		}

		private void SetStartScreenStyle(bool enabled)
		{
			ClassicStartScreenRadioButton.IsChecked = enabled;
			ModernStartScreenRadioButton.IsChecked = !enabled;
		}

		#endregion

		#region Program Paths
		private void ModLoader_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Methods.ProgramLauncher.UpdateModManagerPath();
		}

		private void SonicMania_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Methods.ProgramLauncher.UpdateSonicManiaPath();
		}

		private void button15_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Methods.ProgramLauncher.UpdateRSDKAnimationEditorPath();
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Methods.ProgramLauncher.UpdateImageEditorPath();
		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Methods.ProgramLauncher.UpdateImageEditorArguments();
		}

		private void Button_Click_4(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Methods.ProgramLauncher.UpdateCheatEnginePath();
		}

        #endregion


    }
}
