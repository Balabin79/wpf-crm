using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DevExpress.Xpf.Grid;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using SharpVectors;
using DevExpress.Utils.Svg;
using DevExpress.Xpf.Core.Native;
using B6CRM.Models.Base;

namespace B6CRM.Infrastructures.TreeList
{
    public class NodeImageSelector : TreeListNodeImageSelector
    {
        public ImageSource Open { get; set; }
        public ImageSource Closed { get; set; }
        public ImageSource File { get; set; }


        public override ImageSource Select(DevExpress.Xpf.Grid.TreeList.TreeListRowData rowData)
        {

            // Open = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/folder_open_gnone_32.png"));
            // Closed = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/folder_closed_gnone_32.png"));
            //File = new BitmapImage(new Uri("pack://application:,,,/DevExpress.Images.v20.1;component/Images/RichEdit/PenColor_32x32.png"));
            Open = WpfSvgRenderer.CreateImageSource(new Uri("pack://application:,,,/Resources/Icons/svg/folder-open.svg"));
            Closed = WpfSvgRenderer.CreateImageSource(new Uri("pack://application:,,,/Resources/Icons/svg/folder.svg"));
            File = WpfSvgRenderer.CreateImageSource(new Uri("pack://application:,,,/Resources/Icons/svg/file.svg"));



            var template = rowData.Row as ITree;

            if (rowData == null || rowData.Node == null || template?.IsDir == 0) return File;

            return rowData.Node.IsExpanded ? Open : Closed;

        }
    }
}
