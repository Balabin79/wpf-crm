using Dental.Models;
using Dental.ViewModels;
using DevExpress.Xpf.WindowsUI;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dental.Views.PatientCard
{
    public partial class ClientCardWindow : Window
    {
        public ClientCardWindow(PatientInfo client, PatientListViewModel vm)
        {
            InitializeComponent();
            this.DataContext = new PatientCardViewModel(client, vm); 
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DataContext is PatientCardViewModel vm)
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
