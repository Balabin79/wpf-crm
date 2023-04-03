using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DevExpress.Xpf.Grid;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core.Native;
using System.Drawing;
using B6CRM.Models.Base;

namespace B6CRM.Infrastructures.TreeList
{
    public class NodeImageSelectorForSettings : TreeListNodeImageSelector
    {
        public ImageSource Page { get; set; }
        public ImageSource PageCommand { get; set; }

        public override ImageSource Select(DevExpress.Xpf.Grid.TreeList.TreeListRowData rowData)
        {
            Page = WpfSvgRenderer.CreateImageSource(new Uri("pack://application:,,,/Resources/Icons/svg/browser.svg"));
            PageCommand = WpfSvgRenderer.CreateImageSource(new Uri("pack://application:,,,/Resources/Icons/svg/preview.svg"));

            if (rowData.Row is ICategoryTree model)
            {
                if (rowData?.Node == null || model?.IsCategory == 0) return PageCommand;
                return Page;
            }
            return PageCommand;
        }
    }
}
