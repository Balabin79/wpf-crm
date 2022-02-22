using Dental.Models;
using Dental.ViewModels;
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
            this.DataContext = new EmployeeViewModel(emp, vm);
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
