using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
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
        { try
            {
                if (string.IsNullOrEmpty(value.ToString())) return null;
                return new BitmapImage(new Uri(value.ToString()));
            } catch(Exception e)
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
                var img = Image.FromFile(value.ToString());
                using (var stream = new MemoryStream())
                {
                    img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    return stream.ToArray();
                }
            } catch (Exception e)
            {
                return null;
            }

        }
    }
}
