using B6CRM.Models;
using B6CRM.Services.Files;
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

namespace B6CRM.Infrastructures.Converters
{

    public class DocumentMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {
                return new DocumentCommandParameters()
                {
                    File = (FileInfo)Values[0],
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

    public class DocumentCommandParameters
    {
        public FileInfo File { get; set; }
        public object Model { get; set; }
    }
}

