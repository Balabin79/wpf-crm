using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DevExpress.Xpf.Grid;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Dental.Infrastructures.TreeList
{
    public class NodeImageSelector : TreeListNodeImageSelector
    {
        public ImageSource Open { get; set; }
        public ImageSource Closed { get; set; } 
        public ImageSource File { get; set; } 

        public override System.Windows.Media.ImageSource Select(DevExpress.Xpf.Grid.TreeList.TreeListRowData rowData)
        {

            Open = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/folder_open_gnone_32.png"));
            Closed = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/folder_closed_gnone_32.png"));
            File = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/document_gnome_32.png"));


            var template = rowData.Row as Dental.Models.Template.ITemplate;

            if (rowData == null || template.Dir == 0) return File;


            return rowData.Node.IsExpanded ? Open : Closed;

        }
    }
}
