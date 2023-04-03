using B6CRM.Models;
using B6CRM.ViewModels;
using B6CRM.ViewModels.ClientDir;
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

namespace B6CRM.Views.PatientCard
{
    /// <summary>
    /// Логика взаимодействия для PlanControl.xaml
    /// </summary>
    public partial class PlanControl : UserControl
    {
        public PlanControl()
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
                    var items = ((PlanItem)e.Row)?.Plan?.PlanItems;
                    decimal price = 0;
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
