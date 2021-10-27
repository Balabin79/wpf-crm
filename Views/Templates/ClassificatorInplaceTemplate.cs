using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dental.Models;
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
                Classificator c = row.Row as Classificator;
                if (c != null)
                    //return person.Address.Contains("London") ? PrimaryRowDetailsTemplate : SecondaryRowDetailsTemplate;
                     return c.IsDir == 0 ? FileRowDetailsTemplate : DirectoryRowDetailsTemplate;
                    //if (c.IsDir == 0) return FileRowDetailsTemplate;
             
            }
            return base.SelectTemplate(item, container);
        }
    }
}
