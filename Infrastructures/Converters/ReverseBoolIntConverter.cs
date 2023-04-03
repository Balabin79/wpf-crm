using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace B6CRM.Infrastructures.Converters
{
    public class ReverseBoolIntConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null || (int)value == 0)
                return true;
            return false;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
                return 0;
            return 1;
        }
    }
}
