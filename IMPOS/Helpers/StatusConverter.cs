using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IMPOS.Helpers
{
    /// <summary>
    /// gheyre mortabet va mortabet va royat va inha, baraye datagrid checkbox ha ke faghat yeki faal bashe
    /// gm -> gheyre mortabete
    /// ro -> royat
    /// e -> edit
    /// </summary>
    public class StatusConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "gm")
            {
                return value.ToString() == "0";
            }
            if (parameter.ToString() == "ro")
            {
                return value.ToString() == "1";
            }
            if (parameter.ToString() == "e")
            {
                return value.ToString() == "2";
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "gm")
            {
                return ((bool)value)?0:-1;
            }
            if (parameter.ToString() == "ro")
            {
                return ((bool)value) ? 1 : -1;
            }
            if (parameter.ToString() == "e")
            {
                return ((bool)value) ? 2 : -1;
            }
            return 0;
        }
    }
}
