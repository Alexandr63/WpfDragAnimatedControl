using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfDragAnimatedPanel.Example.Converters
{
    public sealed class HorizontalScrollBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (FillType)value == FillType.Horizontal ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
