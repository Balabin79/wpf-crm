using Dental.Models;
using Dental.Services.Files;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Dental.Infrastructures.Converters
{

    public class InvoiceMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {
                return new InvoiceCommandParameters()
                {
                    FilePath = Values[0].ToString(),
                    Model = Values[1]
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

    public class InvoiceCommandParameters
    {
        public string FilePath { get; set; }
        public object Model { get; set; }
    }
}

