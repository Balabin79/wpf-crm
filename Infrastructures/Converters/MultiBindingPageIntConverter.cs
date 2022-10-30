using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Dental.Infrastructures.Converters
{

    public class MultiBindingPageIntConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {
                int? p = null;
                if (int.TryParse(Values[1].ToString(), out int result)) p = result;
                return new PageIntCommandParameters()
                {                    
                    Page = (UserControl)(System.Windows.FrameworkElement)Values[0],
                    Param = p
                };
            } 
            catch
            {
                return new object();
            }

        }
        
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PageIntCommandParameters
    {
        public UserControl Page{ get; set; }
        public int? Param { get; set; }
    }
}

