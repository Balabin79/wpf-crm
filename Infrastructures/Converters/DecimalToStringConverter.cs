using Dental.Infrastructures.Logs;
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
    public class DecimalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
       {
            try
            {
                if (string.IsNullOrEmpty(value?.ToString())) return null;
                if (decimal.TryParse((string)value, out decimal result)) 
                    return result;
                return 0.00;
            } catch(Exception e)
            {
                new ConvertorLog(e).run();
                return 0.00;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value?.ToString();
            return  (str != null)  ? str : "0.00";
        }
    }
}
