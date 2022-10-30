using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dental.Models;
using Dental.Reports;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Expressions;

namespace Dental.Views.Invoices
{
    /// <summary>
    /// Логика взаимодействия для InvoicesPage.xaml
    /// </summary>
    public partial class InvoicesPage : Page
    {
        public InvoicesPage()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.Invoices.InvoicesViewModel();
        }

        private void ServiceItems_CustomSummary(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            try
            {
                if (((GridSummaryItem)e.Item).FieldName == "Price" && e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
                {
                    if (e.Row == null) return;
                    var items = ((InvoiceServiceItems)e.Row)?.Invoice?.InvoiceServiceItems;
                    if (items == null) return;
                    decimal price = 0;
                    foreach (var item in items)
                    {
                        if (decimal.TryParse(item.Service?.Price?.ToString(), out decimal result))
                        {
                            if (item.Count > 0) price += (result * item.Count);
                        }
                    }
                    e.TotalValue = price;
                }
                e.TotalValueReady = true;
            }
            catch { }
        }

        private void attachFileTableView_CustomRowAppearance(object sender, CustomRowAppearanceEventArgs e)
        {
            try
            {
                if (e.Source.DataControl is GridControl dataControl)
                {
                    e.Handled = true;
                    dataControl.UpdateTotalSummary();
                    return;
                }
            }
            catch { }
        }

        private void MaterialItems_CustomSummary(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            try
            {
                if (((GridSummaryItem)e.Item).FieldName == "Price" && e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
                {
                    if (e.Row == null) return;
                    var items = ((InvoiceMaterialItems)e.Row)?.Invoice?.InvoiceMaterialItems;
                    if (items == null) return;
                    decimal price = 0;
                    foreach (var item in items)
                    {
                        if (decimal.TryParse(item.Nomenclature?.Price?.ToString(), out decimal result))
                        {
                            if (item.Count > 0) price += (result * item.Count);
                        }
                    }
                    e.TotalValue = price;
                }
                e.TotalValueReady = true;
            }
            catch { }
        }

        private void BarButtonItem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            /*ServicesInvoiceReport report = new ServicesInvoiceReport();
            var parameter = new Parameter()
            {
                Name = "Id",
                Description = "Id:",
                Type = typeof(int),
                Value = 4,
                Visible = false
            };
            report.RequestParameters = false;
            report.Parameters.Add(parameter);
            report.FilterString = "[Id] = [Parameters.Id]";*/

           
            
                

                // Invoke the Print dialog.
                //PrintHelper.ShowPrintPreview(this, report);
        }
    }
}
