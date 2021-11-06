using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Dental.Views.Templates
{
    class ClassificatorInplaceTemplate : DataTemplateSelector
    {
        public DataTemplate FileRowDetailsTemplate { get; set; }
        public DataTemplate DirectoryRowDetailsTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            RowData row = item as RowData;
            if (row != null)
            {
                if (row.Row is Dental.Models.Classificator c) {
                    return c.IsDir == 0 ? FileRowDetailsTemplate : DirectoryRowDetailsTemplate;
                }
       
             
            }
            return base.SelectTemplate(item, container);
        }
    }
}
