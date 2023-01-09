using System.Linq;
using System.Windows.Controls;
using Dental.ViewModels.ClientDir;
using DevExpress.Xpf.WindowsUI;

namespace Dental.Views.PatientCard
{
    public partial class PatientsList : UserControl
    {
        public PatientsList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e) => SelectedItem();

        public void SelectedItem()
        {
            try
            {
                if (clientCard.DataContext is ClientsViewModel vm)
                {
                    if (vm?.Model?.Id == 0)
                    {
                        grid.SelectedItem = null;
                        return;
                    }
                    else
                    {
                        var model = vm?.Clients?.FirstOrDefault(f => f.Id == vm?.Model?.Id);
                        grid.SelectedItem = model;
                    }
                }               
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
    }
}
