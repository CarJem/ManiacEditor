using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Configuration;
using System.Windows.Data;
using System.Runtime.InteropServices;
using GenerationsLib.Core;

namespace ManiacEditor.Extensions
{
    public static class SpecialExtensions
    {
		public static bool KeyBindsSettingExists(string name)
		{
			Classes.Options.InputPreferences.Init();
			bool found = false;
			foreach (var currentKeybind in Properties.Settings.MyKeyBinds.GetInputs())
			{
				if (name == currentKeybind)
				{
					found = true;
				}

			}
			return found;
		}
	}

}
