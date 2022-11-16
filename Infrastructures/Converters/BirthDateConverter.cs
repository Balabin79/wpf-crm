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
    public class BirthDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (string.IsNullOrEmpty(value?.ToString())) return null;

                if (DateTime.TryParse(value?.ToString(), out DateTime result))
                    return result.ToString("D");
                return value.ToString();
            }
            catch (Exception e)
            {
                new ConvertorLog(e).run();
                return "Дата не заполнена";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value?.ToString() ?? "";

    }
}
