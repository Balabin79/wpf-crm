using Dental.Models.Base;
using Dental.Models.Templates;
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

    public class FormTreeMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            try
            {               
                return new FormTreeParameters()
                {
                   // Model = Values[0] is ITreeModel ? Values[0] : Activator.CreateInstance(Type.GetType(Values[0].ToString())),
                    IsDir = int.TryParse(Values[1].ToString(), out int res) == true ? res : 0
                };
            } 
            catch (Exception e)
            {
                return new FormTreeParameters();
            }

        }
        
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FormTreeParameters
    {
        public object Model { get; set; }
        public int IsDir { get; set; } = 0;
    }
}

