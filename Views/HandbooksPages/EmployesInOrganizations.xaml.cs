using System.Windows.Controls;

namespace Dental.Views.HandbooksPages
{
    public partial class EmployesInOrganizations : Page
    {
        public EmployesInOrganizations()
        {
            InitializeComponent();
        }

        private void ComboBoxEditSettings_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ((DevExpress.Xpf.Editors.ComboBoxEdit)sender).SelectedIndex = ((Dental.Models.EmployesInOrganizations)view.FocusedRow).EmployeeId;
            ((DevExpress.Xpf.Editors.ComboBoxEdit)sender).EditValue = ((Dental.Models.EmployesInOrganizations)view.FocusedRow).EmployeeId;
        }
    }
}
