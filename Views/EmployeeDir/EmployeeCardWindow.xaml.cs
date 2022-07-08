using Dental.Models;
using Dental.ViewModels;
using Dental.ViewModels.AdditionalFields;
using System.Windows;
using System.Windows.Controls;

namespace Dental.Views.EmployeeDir
{
    /// <summary>
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class EmployeeCardWindow : Window
    {
        public EmployeeCardWindow(Employee emp, ListEmployeesViewModel vm)
        {
            InitializeComponent();
            EmployeeViewModel empCard = new EmployeeViewModel(emp, vm);
            FieldsViewModel fieldsViewModel = new FieldsViewModel(emp, vm);

            this.DataContext = empCard;
            this.Fields.DataContext = fieldsViewModel;

            empCard.SetTabVisibility(fieldsViewModel.AdditionalFieldsVisible);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DataContext is EmployeeViewModel vm)
            {
                if (vm.HasUnsavedChanges() && vm.UserSelectedBtnCancel())
                {
                    e.Cancel = true;
                    return;
                }
                e.Cancel = false;
            }
        }
    }
}
