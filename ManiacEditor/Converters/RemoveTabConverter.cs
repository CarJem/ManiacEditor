using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Media;

namespace ManiacEditor.Converters
{
	public class RemoveTabConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var val = value as string ?? string.Empty;
			return val.Replace("\t", string.Empty);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("Method not implemented");
		}
	}
}
