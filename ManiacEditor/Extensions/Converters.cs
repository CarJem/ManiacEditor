using System;
using System.Windows.Data;

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
