using System.Linq;
using System.Windows.Controls;
using B6CRM.Models;
using B6CRM.ViewModels.ClientDir;
using DevExpress.Xpf.WindowsUI;

namespace B6CRM.Views.PatientCard
{
    public partial class PatientsList : UserControl
    {
        public PatientsList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
           // SelectedItem();
        }

        public void SelectedItem()
        {
            try
            {
                if (clientCard.DataContext is ClientsListViewModel vm)
                {
                    if (vm?.SelectedClient?.Id == 0)
                    {
                        grid.SelectedItem = null;
                        return;
                    }
                    else
                    {
                        grid.SelectedItem = vm?.Clients?.FirstOrDefault(f => f.Id == vm?.SelectedClient?.Id);
                        return;
                    }
                }
                grid.SelectedItem = null;
            }
            catch
            {

            }
        }

        public void SelectedInvoiceItem()
        {
            try
            {
                invoicesList.grid.SelectedItem = null;
            }
            catch { }
        }

       /* private void clientCard_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (((System.Windows.FrameworkElement)sender).Resources["MainInfoViewModel"] is MainInfoViewModel vm)
            {
                vm?.Load(e);
            }
        }*/

        private void BarButtonItem_CreateClient(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (clientCard.Resources["MainInfoViewModel"] is MainInfoViewModel vm)
            {
                vm?.Create(clientCard);
            }
        }
    }
}
