using Dental.Models;
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

    public class DocumentClientMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {
                return new DocumentClientCommandParameters()
                {
                    File = (FileInfo)Values[0],
                    Client = (Client)Values[1]
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

    public class DocumentClientCommandParameters
    {
        public FileInfo File { get; set; }
        public Client Client { get; set; }
    }
}

