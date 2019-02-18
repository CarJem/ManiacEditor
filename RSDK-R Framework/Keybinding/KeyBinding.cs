using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows.Data;
using KeysConverter = System.Windows.Forms.KeysConverter;

namespace RSDK_R_Framework
{
	public class KeyBinding
	{
		public static List<StackPanel> ControlPanels;
		public static SettingsPropertyCollection Keybindings;
		public static SettingsPropertyCollection DefaultKeybindings;
		public static SolidColorBrush NormalTextColor;
		public KeyBinding(SettingsPropertyCollection CurrentKeybinds, SettingsPropertyCollection DefaultCollection, List<StackPanel> SettingsPages, bool DarkMode = false)
		{
			Keybindings = CurrentKeybinds;
			DefaultKeybindings = DefaultCollection;
			ControlPanels = SettingsPages;

			if (DarkMode) NormalTextColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
			else NormalTextColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
		}

		/*private void EditKeyCombo(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button)) return;
			Button KeyBind = sender as Button;
			bool state = true;
			if (state)
			{
				string keybindName = KeyBind.Tag.ToString();

				StringCollection keyBindList = Keybindings[keybindName].DefaultValue as StringCollection;

				KeyBindConfigurator keybinder = new KeyBindConfigurator(keybindName);
				keybinder.ShowDialog();
				if (keybinder.DialogResult == true)
				{
					KeysConverter kc = new KeysConverter();
					System.Windows.Forms.Keys keyBindtoSet = keybinder.CurrentBindingKey;
					int keyIndex = keybinder.ListIndex;

					var keybindDict = Keybindings[keybindName].DefaultValue as StringCollection;
					String KeyString = kc.ConvertToString(keyBindtoSet);
					keybindDict.RemoveAt(keyIndex);
					keybindDict.Add(KeyString);
					Keybindings[keybindName].DefaultValue = keybindDict;
				}
			}
			SetKeybindTextboxes(ControlPanels);

		}*/

		List<string> AllKeyBinds = new List<string>();
		List<string> KnownKeybinds = new List<string>();

		private void RefreshKeybindList()
		{
			AllKeyBinds.Clear();
			AllKeyBinds = new List<string>();
			KnownKeybinds.Clear();
			KnownKeybinds = new List<string>();
			foreach (SettingsProperty currentProperty in Keybindings)
			{
				KnownKeybinds.Add(currentProperty.Name);
			}
			foreach (string keybind in KnownKeybinds)
			{
				if (!Extensions.KeyBindsSettingExists(keybind)) continue;
				var keybindDict = Keybindings[keybind].DefaultValue as StringCollection;
				if (keybindDict != null && keybindDict.Count != 0)
				{
					foreach (string item in keybindDict)
					{
						AllKeyBinds.Add(item);
					}
				}

			}
		}
		private void SetKeybindTextboxes(List<StackPanel> stackPanels)
		{
			RefreshKeybindList();
			foreach (StackPanel stackPanel in stackPanels)
			{
				foreach (StackPanel stack in Extensions.FindVisualChildren<StackPanel>(stackPanel))
				{
					foreach (Button t in Extensions.FindVisualChildren<Button>(stack))
					{
						ProcessKeybindingButtons(t);
					}
				}
			}

		}

		private void ProcessKeybindingButtons(Button t)
		{
			t.Foreground = NormalTextColor;
			if (t.Tag != null)
			{
				System.Tuple<string, string> tuple = KeyBindPraser(t.Tag.ToString());
				t.Content = tuple.Item1;
				if (tuple.Item2 != null)
				{
					if (HasSingleMultipleOccurances(t.Tag.ToString())) t.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
					else t.Foreground = NormalTextColor;
					ToolTipService.SetShowOnDisabled(t, true);
					t.ToolTip = tuple.Item2;
				}
				else
				{
					if (HasMultipleOccurances(tuple.Item1)) t.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
					else t.Foreground = NormalTextColor;
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

			var keybindDict = Keybindings[keyRefrence].DefaultValue as StringCollection;
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

		private Tuple<string, string> KeyBindPraser(string keyRefrence)
		{
			if (keyRefrence == "NULL") return new Tuple<string, string>("N/A", null);

			List<string> keyBindList = new List<string>();
			List<string> keyBindModList = new List<string>();

			if (!Extensions.KeyBindsSettingExists(keyRefrence)) return new Tuple<string, string>("N/A", null);

			var keybindDict = Keybindings[keyRefrence].DefaultValue as StringCollection;
			if (keybindDict != null)
			{
				keyBindList = keybindDict.Cast<string>().ToList();
			}
			else
			{
				return new Tuple<string, string>("N/A", null);
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
	}
}
