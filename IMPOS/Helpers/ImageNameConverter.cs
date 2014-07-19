using System;
using System.Globalization;
using System.Windows.Data;

namespace IMPOS.Helpers
{
    /// <summary>
    /// addrese kamel ro migire va esme file ro barmigardoone
    /// </summary>
    class ImageNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value.ToString();
            return str.Substring(str.LastIndexOf('\\')+1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool) value;
        }
    }
}
