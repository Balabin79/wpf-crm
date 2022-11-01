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

    public class MultiBindingInvoiceItemConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {
                int p = -1;
                Invoice invoice = null;

                if (int.TryParse(Values[0].ToString(), out int result)) p = result;
                if (Values[1] is Invoice) invoice = (Invoice)Values[1];

                return new InvoiceItemCommandParameters()
                {
                    Invoice = invoice,
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

    public class InvoiceItemCommandParameters
    {
        public Invoice Invoice { get; set; }
        public int Param { get; set; }
    }
}

