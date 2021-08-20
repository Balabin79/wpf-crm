using DevExpress.Xpf.WindowsUI.Navigation;
using System;
using System.Windows.Controls;
using Dental.ViewModels;
using System.Windows;

namespace Dental.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class Employee : UserControl //, DevExpress.Xpf.WindowsUI.Navigation.INavigationAware
    {
        public Employee()
        {
            InitializeComponent();
        }
        /*
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
        }*/

        void OpenFlyout(object sender, RoutedEventArgs e)
        {
            flyoutControl.PlacementTarget = sender as FrameworkElement;
            flyoutControl.Content = "Раздел \"Категории клиентов\" позволяет объединять клиентов в группы, которые можно\nиспользовать для примененения выборочной скидки, маркетинговых рассылок и т.д. \n1. Добавьте в справочник \"Категории клиентов\" позиции, которые вам необходимы. \n2. Заполняя карту пациенты, в поле \"Категории клиентов\" выберите соответствующую позицию.\n";
            flyoutControl.IsOpen = true;
        }
    }
}
