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

    public class ExportDataConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {
                return new ExportDataCommandParameters()
                {
                    Type = Values[0] as Type,
                    Context = Values[1] as ApplicationContext
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

    public class ExportDataCommandParameters
    {
        public Type Type { get; set; }
        public ApplicationContext Context { get; set; }
    }
}

