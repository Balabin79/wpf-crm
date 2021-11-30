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

    public class MultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            var findCommandParameters = new FindCommandParameters();
           
            findCommandParameters.Tree = (TreeListView) (System.Windows.FrameworkElement)Values[0];

            findCommandParameters.Popup = (PopupBaseEdit)(System.Windows.FrameworkElement)Values[1];

            return findCommandParameters;
        }
        
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FindCommandParameters
    {
        public TreeListView Tree { get; set; }
        public PopupBaseEdit Popup { get; set; }
    }
}

