using System.Linq;
using System.Windows.Controls;
using Dental.ViewModels.ClientDir;
using DevExpress.Xpf.WindowsUI;

namespace Dental.Views.PatientCard
{
    public partial class PatientsList : Page
    {
        public PatientsList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e) => SelectedItem();

        public void SelectedItem()
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
                    var model = vm?.Collection?.FirstOrDefault(f => f.Id == vm?.Model?.Id);
                    grid.SelectedItem = model;
                }
            }
        }
    }
}
