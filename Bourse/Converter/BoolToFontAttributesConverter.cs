using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace Bourse.Converter
{
    public class BoolToFontAttributesConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? FontAttributes.Bold : FontAttributes.None;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is FontAttributes fa && fa == FontAttributes.Bold;
        }
    }
}
