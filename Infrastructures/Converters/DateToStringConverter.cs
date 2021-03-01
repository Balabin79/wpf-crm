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
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
       {
            try
            {
                DateTime result;
                if (string.IsNullOrEmpty(value.ToString())) return new DateTime().ToShortDateString();
                if (DateTime.TryParse((string)value, out result)) 
                    return result.ToShortDateString();
                return new DateTime().ToShortDateString();
            } catch(Exception e)
            {
                new ConvertorLog(e).run();
                return new DateTime().ToShortDateString();
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result;
            var str = value?.ToString();
            return  (str != null)  ? str : "";
        }
    }
}
