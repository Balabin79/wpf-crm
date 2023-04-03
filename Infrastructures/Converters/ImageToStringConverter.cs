using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace B6CRM.Infrastructures.Converters
{
    public class ImageToStringConverter : IValueConverter
    {

        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (string.IsNullOrEmpty(value.ToString())) return "";
                return value.ToString();
            }
            catch
            {
                return null;
            }

        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (string.IsNullOrEmpty(value.ToString())) return null;
                return new ImageSourceConverter().ConvertFromString(value.ToString()) as ImageSource;
            }
            catch
            {
                return null;
            }

        }
    }
}
