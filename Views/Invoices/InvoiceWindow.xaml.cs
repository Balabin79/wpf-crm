using Dental.ViewModels.Invoices;
using System;
using System.Windows;

namespace Dental.Views.Invoices
{
    public partial class InvoiceWindow : Window
    {
        public InvoiceWindow()
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

        private void Cancel_Form(object sender, RoutedEventArgs e) => Close();

        private void DateEdit_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (DataContext is InvoiceVM vm)
            {
                var dateEdit = DateInvoice.EditValue?.ToString();
                if (dateEdit != null && DateTime.TryParse(dateEdit, out DateTime date)) vm.DateTimestamp = new DateTimeOffset(date).ToUnixTimeSeconds();
            }          
        }
    }
}
