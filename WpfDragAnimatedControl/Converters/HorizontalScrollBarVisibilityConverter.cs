using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfDragAnimatedControl.Converters
{
    public sealed class HorizontalScrollBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FillType fillType)
            {
                return fillType == FillType.Row || fillType == FillType.Column ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
            }

            return ScrollBarVisibility.Disabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
