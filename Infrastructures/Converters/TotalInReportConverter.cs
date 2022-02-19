using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Dental.Infrastructures.Converters
{
    public class TotalInReportConverter : IValueConverter
    {

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var argVal = value;
                return PerformLookupByArgument(argVal);
            }

            private object PerformLookupByArgument(object argVal)
            {
            string name = ((DevExpress.Xpf.Charts.StackedBarTotalLabelItem)argVal).Argument.ToString();
            var collection = ((ViewModels.ReportsByEmpViewModel)((DevExpress.Xpf.Charts.StackedBarTotalLabelItem)argVal).Label.DataContext).Employes;

            foreach (var i in collection)
            {
                if (string.Compare(i.Name, name, StringComparison.CurrentCulture) == 0)
                {
                    return ((DevExpress.Xpf.Charts.StackedBarTotalLabelItem)argVal).Text = 
                        "Чистая выручка: " + (i.Prices - i.Costs).ToString();
                }
            }
                return "";
                //TODO - find the label content by the argument  
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return null;
            }
        
    }
}
