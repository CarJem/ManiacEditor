﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Specialized;
using Keys = System.Windows.Forms.Keys;
using KeysConverter = System.Windows.Forms.KeysConverter;
using ManiacEditor.Extensions;

namespace ManiacEditor.Controls.Options
{
    /// <summary>
    /// Interaction logic for KeyBindConfigurator.xaml
    /// </summary>
    public partial class KeyBindConfigurator : Window
	{
		IList<string> KeyBindsList = new List<string>();
		public System.Windows.Forms.Keys CurrentBindingKey = System.Windows.Forms.Keys.None;
		public bool ctrlChecked = false;
		public bool altChecked = false;
		public bool tabChecked = false;
		public bool shiftChecked = false;
		public int ListIndex = 0;
		public int KeyCount = 1;

		string KeyRefrence = "";

		public const string ctrl = "CTRL";
		public const string shift = "SHIFT";
		public const string alt = "ALT";
		public const string plus = " + ";

		public KeyBindConfigurator(string _keyRefrence)
		{
			KeyRefrence = _keyRefrence;
			InitializeComponent();
			UpdateResultLabel();
			SetupExistingKeybinds(_keyRefrence);
			SetupListBox();
		}

		private void SetupListBox()
		{
			this.KeybindList.Items.Clear();
			for (int i = 0; i < KeyCount; i++)
			{
				Button content = new Button()
				{
					Content = KeyBindsList[i],
					Tag = i,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Stretch
				};
				ListViewItem item = new ListViewItem()
				{
					Content = content

				};
				content.Click += ListItemClicked;
				this.KeybindList.Items.Add(item);
			}
		}

		private void SetupExistingKeybinds(string keyRefrence)
		{
			var keybindDict = Properties.Settings.MyKeyBinds.GetInput(keyRefrence) as List<string>;
			if (keybindDict != null) KeyBindsList = keybindDict.Cast<string>().ToArray();
			KeyCount = KeyBindsList.Count();

			UpdateResultLabel();
		}

		private void UpdateSelection()
		{
			UpdateResultLabel();
		}


		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			Keys keyData = KeyEventExts.ToWinforms(e).KeyData;
			CurrentBindingKey = keyData;
			UpdateResultLabel();
		}

		private void textBox1_TextChanged(object sender, RoutedEventArgs e)
		{
			KeysConverter kc = new KeysConverter();
			InputBox.Content = kc.ConvertToString(CurrentBindingKey);
		}

		private void UpdateResultLabel()
		{
			KeysConverter kc = new KeysConverter();
			if (KeyBindsList.Count == KeyCount)
			{
				PreviousBoxLabel.Content = KeyBindsList[ListIndex].ToString();
				InputBoxLabel.Content = kc.ConvertToString(CurrentBindingKey);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void ListItemClicked(object sender, RoutedEventArgs e)
		{
			Button item = sender as Button;
			ListIndex = (int)item.Tag;
			UpdateSelection();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			CurrentBindingKey = Keys.Alt;
			UpdateResultLabel();
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			CurrentBindingKey = Keys.ControlKey;
			UpdateResultLabel();
		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			CurrentBindingKey = Keys.ShiftKey;
			UpdateResultLabel();
		}

		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Not Implemented Yet", "Sorry!", MessageBoxButton.OK, MessageBoxImage.Question);
		}

		private void RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Not Implemented Yet", "Sorry!", MessageBoxButton.OK, MessageBoxImage.Question);
		}

		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("You can not reverse this, do you want to continue?", "WARNING!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			if (result == MessageBoxResult.Yes)
			{
                Properties.Settings.MyKeyBinds.SetInput(KeyRefrence, Properties.Settings.MyDefaultKeyBinds.GetInput(KeyRefrence));

                Classes.Options.InputPreferences.Save();
				Classes.Options.InputPreferences.Reload();
				KeysConverter kc = new KeysConverter();
				CurrentBindingKey = (Keys)kc.ConvertFromString(KeyBindsList[ListIndex].ToString());

				UpdateResultLabel();
				SetupExistingKeybinds(KeyRefrence);
				SetupListBox();
			}
		}
	}
}
