using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;


namespace ManiacEditor.Converters
{
	public class TilePixelConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (null == value)
			{
				return 0;
			}

			if (!(value is ushort))
			{
				return 0;
			}

			return ((ushort)value) * 16;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
