using Dental.Services;
using Dental.ViewModels;
using DevExpress.Xpf.Core;
using System.Windows;

namespace Dental.Views.PatientCard
{
    public partial class AdvertisingWindow : Window
    {
        public AdvertisingWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Resources["vm"] is AdvertisingViewModel vm)
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
