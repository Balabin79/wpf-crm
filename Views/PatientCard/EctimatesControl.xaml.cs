using Dental.Models;
using Dental.ViewModels;
using DevExpress.Xpf.Grid;
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

namespace Dental.Views.PatientCard
{
    /// <summary>
    /// Логика взаимодействия для EctimatesCintrol.xaml
    /// </summary>
    public partial class EctimatesControl : UserControl
    {
        public EctimatesControl()
        {
            InitializeComponent();
        }

        private void TreatmentPlanItems_CustomSummary(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            if (((GridSummaryItem)e.Item).FieldName == "Price" && e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            {
                if (e.Row == null) return;
                var items = ((EstimateServiceItem)e.Row)?.Estimate?.EstimateServiseItems;
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
                var items = ((EstimateMaterialItem)e.Row)?.Estimate?.EstimateServiseItems;
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
    }
}
