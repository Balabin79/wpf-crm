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

    public class MultiBindingToObjectConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {
                return new CommandParameters()
                {
                    Row = Values[0] is Type type ? Activator.CreateInstance(type) : Values[0],
                    Type = int.TryParse(Values[1].ToString(), out int result) ? result : 0
                };
            } 
            catch
            {
                return null;
            }

        }
        
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CommandParameters
    {
        public object Row { get; set; }
        public int Type { get; set; }
    }
}

