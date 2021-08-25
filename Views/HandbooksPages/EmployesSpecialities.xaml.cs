using Dental.Models;
using System.Windows.Controls;

namespace Dental.Views.HandbooksPages
{
    /// <summary>
    /// Логика взаимодействия для Roles.xaml
    /// </summary>
    public partial class EmployesSpecialities : Page
    {
        public EmployesSpecialities()
        {
            InitializeComponent();
        }

        private void ComboBoxEditSettings_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ((DevExpress.Xpf.Editors.ComboBoxEdit)sender).SelectedIndex = ((Dental.Models.EmployesSpecialities)view.FocusedRow).EmployeeId;
            ((DevExpress.Xpf.Editors.ComboBoxEdit)sender).EditValue = ((Dental.Models.EmployesSpecialities)view.FocusedRow).EmployeeId;
        }
    }
}
