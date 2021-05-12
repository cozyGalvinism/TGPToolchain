using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace TGPToolchain.Converters
{
    public class DoubleDataGridLength : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double dValue) return default(DataGridLength);
            return new DataGridLength(dValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DataGridLength dgValue) return -1d;
            return dgValue.Value;
        }
    }
}