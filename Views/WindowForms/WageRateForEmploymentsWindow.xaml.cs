using Dental.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace Dental.Views.WindowForms
{
    public partial class WageRateForEmploymentsWindow : Window
    {
        public WageRateForEmploymentsWindow()
        {
            InitializeComponent();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            
            if (this.view.DataContext is WageRateViewModel vm)
            {
                if (vm.HasUnsavedChanges() && vm.UserSelectedBtnCancel())
                {
                    e.Cancel = true;
                    return;
                }
            }
            e.Cancel = false;
            return;
        }
    }
}
