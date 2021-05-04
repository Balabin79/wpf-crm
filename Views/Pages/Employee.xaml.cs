using DevExpress.Xpf.WindowsUI.Navigation;
using System;
using System.Windows.Controls;
using Dental.ViewModels;

namespace Dental.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class Employee : UserControl, DevExpress.Xpf.WindowsUI.Navigation.INavigationAware
    {
        public Employee()
        {
            InitializeComponent();
        }

        public void NavigatedFrom(NavigationEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public void NavigatedTo(NavigationEventArgs e)
        {
            try
            {
                this.MainGrid.DataContext = e.Parameter == null ? new EmployeeViewModel() : this.MainGrid.DataContext = new EmployeeViewModel((int)e.Parameter);
            } 
            catch(Exception ex)
            {
                this.MainGrid.DataContext = new EmployeeViewModel();
            }
        }

        public void NavigatingFrom(NavigatingEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
