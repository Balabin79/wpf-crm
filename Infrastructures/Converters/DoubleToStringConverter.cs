using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Dental.Infrastructures.Converters
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
       {
            try
            {
                if (string.IsNullOrEmpty(value?.ToString())) return null;
                if (double.TryParse((string)value, out double result)) 
                    return result;
                return 0;
            } catch
            {
                return 0;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value?.ToString();
            return  (str != null)  ? str : "";
        }
    }
}
