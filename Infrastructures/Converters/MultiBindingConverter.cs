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

    class MultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            var findCommandParameters = new FindCommandParameters();
           
            findCommandParameters.Property2 = ((Models.Base.AbstractBaseModel)((DevExpress.Xpf.Grid.GridCellData)((System.Windows.FrameworkElement)Values[1]).DataContext).Row).Id;

            findCommandParameters.Property1 = Values[0];
            return findCommandParameters;
        }
        
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FindCommandParameters
    {
        public object Property1 { get; set; }
        public int Property2 { get; set; }
    }
}

