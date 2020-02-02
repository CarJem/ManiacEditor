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
	internal class TilePixelConverter : IValueConverter
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

			return ((ushort)value) * Classes.Core.Constants.TILE_SIZE;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
