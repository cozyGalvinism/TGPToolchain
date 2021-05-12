using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using TGPToolchain.Helpers;

namespace TGPToolchain.Converters
{
    public class StringSplit : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not List<string> stringValues ? "" : string.Join(", ", stringValues);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not string strValue ? new List<string>() : strValue.Split(',').Select(s => s.Trim()).ToList();
        }
    }
}