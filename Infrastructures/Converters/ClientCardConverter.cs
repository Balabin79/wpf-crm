using B6CRM.Models;
using B6CRM.Models.Base;
using B6CRM.Models.Templates;
using DevExpress.Xpf.Bars;
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

    public class ClientCardConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {
                return new ClientCardParameters()
                {
                    BarButtonItem = (BarButtonItem)Values[0], 
                    Client = Values[1] is Client ? (Client)Values[1] : new Client()
                };
            }
            catch
            {
                return new ClientCardParameters();
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ClientCardParameters
    {
        public BarButtonItem BarButtonItem { get; set; }
        public Client Client { get; set; }
    }
}

