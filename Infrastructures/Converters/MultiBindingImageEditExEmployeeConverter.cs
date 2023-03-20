using Dental.Infrastructures.Extensions;
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

    public class MultiBindingImageEditExEmployeeConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {
                return new ImageEditExCommandParameters()
                {
                    ImgEdit =  Values[0] as ImageEditEx,
                    Employee = Values[1] as Employee
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

    public class ImageEditExCommandParameters
    {
        public ImageEditEx ImgEdit{ get; set; }
        public Employee Employee { get; set; }
    }
}

