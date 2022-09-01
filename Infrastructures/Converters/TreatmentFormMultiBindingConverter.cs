using Dental.Models;
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

    public class TreatmentFormMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {
                return new TreatmentFormParameters()
                {
                    Table = (TableView)(System.Windows.FrameworkElement)Values[0],
                    Name = Values[1].ToString()
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

    public class TreatmentFormParameters
    {
        public TableView Table { get; set; }
        public string Name { get; set; }
    }
}

