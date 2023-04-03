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
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null || (int)value == 0) return "Visible";
            return "Collapsed";
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true) return "Collapsed";
            return "Visible";
        }
    }
}
