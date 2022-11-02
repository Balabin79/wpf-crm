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
    public class RowTotalConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {
                if (Values == null || Values.Length == 0) return string.Format(CultureInfo.CurrentCulture, "{0:C2}", 0);
                int cnt = 0;
                if (int.TryParse(Values[0]?.ToString(), out int result)) cnt = result;

                decimal? price = 0;
                if (decimal.TryParse(Values[1]?.ToString(), out decimal param)) price = param;

                return cnt == 0 || price == 0
                    ?
                    string.Format(CultureInfo.CurrentCulture, "{0:C2}", price)
                    :
                    string.Format(CultureInfo.CurrentCulture, "{0:C2}", cnt * price);
            }
            catch
            {
                return string.Format(CultureInfo.CurrentCulture, "{0:C2}", 0);
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
