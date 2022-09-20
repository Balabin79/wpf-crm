using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DevExpress.Xpf.Grid;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Dental.Models.Base;

namespace Dental.Infrastructures.TreeList
{
    public class NodeImageSelectorForSettings : TreeListNodeImageSelector
    {
        public ImageSource Page { get; set; }
        public ImageSource PageCommand { get; set; } 

        public override System.Windows.Media.ImageSource Select(DevExpress.Xpf.Grid.TreeList.TreeListRowData rowData)
        {
            Page = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Formheading.png"));
            PageCommand = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Lightning.png"));

            if (rowData.Row is ICategoryTree model)
            {
                if (rowData?.Node == null || model?.IsCategory == 0) return PageCommand;
                return Page;
            }
            return PageCommand;
        }
    }
}
