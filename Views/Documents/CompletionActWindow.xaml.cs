using Dental.Services;
using Dental.ViewModels;
using DevExpress.Xpf.Core;
using System.Windows;

namespace Dental.Views.Documents
{
    public partial class CompletionActWindow : Window
    {
        public CompletionActWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Resources["vm"] is DocumentsViewModel vm)
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
