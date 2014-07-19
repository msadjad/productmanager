using System;
using System.Globalization;
using System.Windows.Data;

namespace IMPOS.Helpers
{
    /// <summary>
    /// dar soorati ke tick ra zad panjare gheyb shan
    /// </summary>
    class HiddenToCheckedConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool) value;
        }
    }
}
