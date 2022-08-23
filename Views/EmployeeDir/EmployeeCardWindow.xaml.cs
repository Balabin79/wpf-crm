using Dental.Models;
using Dental.ViewModels.EmployeeDir;
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
        public EmployeeCardWindow(Employee emp) : this(emp, null){}

        public EmployeeCardWindow(Employee emp, ListEmployeesViewModel vm)
        {
            InitializeComponent();
            EmployeeViewModel empCard = new EmployeeViewModel(emp, vm);
            FieldsViewModel fieldsViewModel = new FieldsViewModel(emp, vm);
            //PriceViewModel price = new PriceViewModel(emp);

            empCard.EventChangeReadOnly += fieldsViewModel.ChangedReadOnly;
            fieldsViewModel.IsReadOnly = true;

            empCard.EventSaveCard += fieldsViewModel.Save;

            empCard.SetTabVisibility(fieldsViewModel.AdditionalFieldsVisible);
            //empCard.SetTabIndividualPriceVisibility();

            this.DataContext = empCard;
            this.Fields.DataContext = fieldsViewModel;        
            //this.IndividualPrice.DataContext = fieldsViewModel;        
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
