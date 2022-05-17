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
    public class DateToStringShortDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
       {
            try
            {
                if (string.IsNullOrEmpty(value?.ToString())) return null;
                if (DateTime.TryParse(value?.ToString(), out DateTime result)) 
                    return result.ToShortDateString();
                return DateTime.Now.ToShortDateString();
            } catch(Exception e)
            {
                new ConvertorLog(e).run();
                return DateTime.Now.ToShortDateString();
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value?.ToString() ?? "";
    }
}
