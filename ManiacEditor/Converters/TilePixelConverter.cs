using System;
using System.Globalization;
using System.Windows.Data;

namespace ManiacEditor.Converters
{
    /// <summary>
    /// Converts a number of tiles, in any one dimension, to the equivilent number of pixels.
    /// </summary>
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

            return ((ushort)value) * EditorConstants.TILE_SIZE;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
