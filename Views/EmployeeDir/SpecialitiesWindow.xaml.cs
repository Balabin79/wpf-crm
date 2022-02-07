using Dental.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Dental.Views.EmployeeDir
{
    public partial class SpecialitiesWindow : Window
    {
        public SpecialitiesWindow()
        {
            InitializeComponent();
        }

        private void TextEdit_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (e.NewValue?.ToString() == "0")
            {
                ((DevExpress.Xpf.Editors.BaseEdit)sender).EditValue = "";
            }
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Resources["vm"] is SpecialityViewModel vm)
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
