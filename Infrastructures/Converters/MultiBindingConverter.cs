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

namespace B6CRM.Infrastructures.Converters
{

    public class MultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {
                return new FindCommandParameters()
                {
                    Tree = (TreeListControl)(System.Windows.FrameworkElement)Values[0],
                    Popup = (PopupBaseEdit)(System.Windows.FrameworkElement)Values[1]
                };
            }
            catch
            {
                return new object();
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FindCommandParameters
    {
        public TreeListControl Tree { get; set; }
        public PopupBaseEdit Popup { get; set; }
    }
}

