using Dental.Services;
using Dental.ViewModels;
using Dental.ViewModels.AdditionalFields;
using DevExpress.Xpf.Core;
using System.Windows;

namespace Dental.Views.AdditionalFields
{
    public partial class CommonValuesWindow : Window
    {
        public CommonValuesWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Resources["vm"] is CommonValueViewModel vm)
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
