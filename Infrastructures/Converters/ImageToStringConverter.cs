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
    public class ImageToStringConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == "") return null;

            /* BitmapImage img = (BitmapImage)imageEdit.Source;
             MemoryStream str = (MemoryStream)img.StreamSource;
             FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
             str.WriteTo(fs);
             fs.Close();*/
            //string uri = value?.ToString() ?? "";
            //return new BitmapImage(new Uri(uri));
            if (string.IsNullOrEmpty(value.ToString())) 
                return "";
            return value.ToString();   
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString())) return "";
            return new BitmapImage(new Uri(value.ToString()));    
        }
    }
}
