using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Application_Bac3_WPF
{
    public class DoneToTextStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isDone = value is bool b && b;
            if (parameter?.ToString() == "Color")
                return isDone ? Brushes.Gray : Brushes.Black;
            if (parameter?.ToString() == "TextDecorations")
                return isDone ? TextDecorations.Strikethrough : null;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}