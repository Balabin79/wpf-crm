using Dental.Services;
using Dental.ViewModels;
using DevExpress.Xpf.Core;
using System.Windows;

namespace Dental.Views.NomenclatureDir
{
    public partial class WarehouseWindow : Window
    {
        public WarehouseWindow()
        {
            InitializeComponent();
        }

        private void Window_Warehouse_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Resources["vm"] is WarehouseViewModel vm)
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
