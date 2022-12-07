using Dental.Models;
using Dental.ViewModels.ClientDir;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dental.Views.PatientCard
{
    public partial class MedicalTab : UserControl
    {
        public MedicalTab()
        {
            InitializeComponent();
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            childTeethPage.Visibility = Visibility.Collapsed;
            teethPage.Visibility = Visibility.Visible;
            if (childTeethPage.DataContext is ClientsViewModel vm && vm.Model != null) vm.Model.IsChild = 0;
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {          
            childTeethPage.Visibility = Visibility.Visible;
            teethPage.Visibility = Visibility.Collapsed;
            if (childTeethPage.DataContext is ClientsViewModel vm && vm.Model != null) vm.Model.IsChild = 1; 
        }

        private void teethPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (childTeethPage.DataContext is ClientsViewModel vm && vm.Model?.IsChild == 1)
            {
                childTeethPage.Visibility = Visibility.Visible;
                teethPage.Visibility = Visibility.Collapsed;
            }
            else
            {
                childTeethPage.Visibility = Visibility.Collapsed;
                teethPage.Visibility = Visibility.Visible;
            }
        }

        private void childTeethPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (childTeethPage.DataContext is ClientsViewModel vm && vm.Model?.IsChild == 1)
            {
                childTeethPage.Visibility = Visibility.Visible;
                teethPage.Visibility = Visibility.Collapsed;
            }
            else
            {
                childTeethPage.Visibility = Visibility.Collapsed;
                teethPage.Visibility = Visibility.Visible;
            }
        }
    }
}
