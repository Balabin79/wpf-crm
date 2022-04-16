using Dental.Services;
using Dental.ViewModels;
using DevExpress.Xpf.Core;
using System.Windows;

namespace Dental.Views.Documents
{
    public partial class PurchaseInvoiceWindow : Window
    {
        public PurchaseInvoiceWindow()
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
