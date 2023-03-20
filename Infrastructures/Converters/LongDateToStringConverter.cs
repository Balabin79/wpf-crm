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
    public class LongDateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
       {
            try
            {
                if (string.IsNullOrEmpty(value?.ToString())) return null;
                if (DateTime.TryParse(value?.ToString(), out DateTime result)) 
                    return result;
                return DateTime.Now.ToString();
            } catch
            {
                return DateTime.Now.ToString();
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value?.ToString() ?? "";
        
    }
}
