using Dental.Models;
using DevExpress.Xpf.Grid;
using System.Windows.Controls;

namespace Dental.Views.Invoices
{
    /// <summary>
    /// Логика взаимодействия для InvoicesControl.xaml
    /// </summary>
    public partial class InvoicesControl : UserControl
    {
        public InvoicesControl()
        {
            InitializeComponent();
        }

        private void TreatmentPlanItems_CustomSummary(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            if (((GridSummaryItem)e.Item).FieldName == "Price" && e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            {
                if (e.Row == null) return;
                var items = ((InvoiceServiceItems)e.Row)?.Invoice?.InvoiceServiceItems;
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

        private void attachFileTableView_CustomRowAppearance(object sender, CustomRowAppearanceEventArgs e)
        {
            if (e.Source.DataControl is GridControl dataControl)
            {
                e.Handled = true;
                dataControl.UpdateTotalSummary();
                return;
            }
        }

        private void MaterialItems_CustomSummary(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            if (((GridSummaryItem)e.Item).FieldName == "Price" && e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            {
                if (e.Row == null) return;
                var items = ((InvoiceMaterialItems)e.Row)?.Invoice?.InvoiceMaterialItems;
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
    }
}
