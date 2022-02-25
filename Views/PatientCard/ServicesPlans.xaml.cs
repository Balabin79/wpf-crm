using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using Dental.Models;

namespace Dental.Views.PatientCard
{
    public partial class ServicesPlans : UserControl
    {
        public ServicesPlans()
        {
            InitializeComponent(); 
         
        }

        private void TreatmentPlanItems_CustomSummary(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            if (((GridSummaryItem)e.Item).FieldName == "Cost" && e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            {
                if (e.Row == null) return;
                var items = ((TreatmentPlanItems)e.Row)?.TreatmentPlan?.TreatmentPlanItems;
                decimal cost = 0;
                foreach (var item in items)
                {
                    if (decimal.TryParse(item.Classificator?.Cost?.ToString(), out decimal result))
                    {
                        if (item.Count > 0) cost += (result * item.Count);
                    }                   
                }
                e.TotalValue = cost;               
            }

            if (((GridSummaryItem)e.Item).FieldName == "Price" && e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            {
                if (e.Row == null) return;
                var items = ((TreatmentPlanItems)e.Row)?.TreatmentPlan?.TreatmentPlanItems;
                decimal? price = 0;
                foreach (var item in items)
                {              
                    if (item.Count > 0 && item.Classificator?.Price != null) 
                            price += (item.Classificator?.Price * item.Count);                 
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
    }
}
