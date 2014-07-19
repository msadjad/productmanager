


using System;
using System.Globalization;
using System.Windows.Data;

namespace IMPOS.Helpers
{
    /// <summary>
    /// motaghayer true shod size 30 dar gheyre in size 0
    /// handle kardane visible height be jaye mvvm az height estefade shode
    /// </summary>
    public class HeightEquilZeroConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool) value) ? "30" : "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}