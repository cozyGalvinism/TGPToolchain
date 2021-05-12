using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace TGPToolchain.Converters
{
    public abstract class GenericValueConverter<TInput, TOutput> : IValueConverter
    {
        public abstract TOutput Convert(TInput value, object parameter);
        public abstract TInput ConvertBack(TOutput value, object parameter);
        
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TInput tValue) return default(TInput);
            return Convert(tValue, parameter);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TOutput tValue) return default(TOutput);
            return ConvertBack(tValue, parameter);
        }
    }
}