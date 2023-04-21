using B6CRM.Models;
using DevExpress.Xpf.Grid;
using System.Windows.Controls;

namespace B6CRM.Views.PatientCard
{

    public partial class ClientInvoicesControl : UserControl
    {
        public ClientInvoicesControl()
        {
            InitializeComponent();
        }

        private void CustomSummary(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            try
            {
                if (((GridSummaryItem)e.Item).FieldName == "Price" && e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
                {
                    if (e.Row == null) return;
                    var items = ((InvoiceItems)e.Row)?.Invoice?.InvoiceItems;
                    decimal? price = 0;
                    if (items == null) return;
                    foreach (var item in items)
                    {
                        if (decimal.TryParse(item?.Price?.ToString(), out decimal result))
                        {
                            if (item.Count > 0) price += result * item.Count;
                        }
                    }
                    e.TotalValue = price;
                }
                e.TotalValueReady = true;
            }
            catch { }

        }

        private void CustomRowAppearance(object sender, CustomRowAppearanceEventArgs e)
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
    }
}
