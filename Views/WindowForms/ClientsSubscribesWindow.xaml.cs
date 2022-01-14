using System.Windows;

namespace Dental.Views.WindowForms
{
    public partial class ClientsSubscribesWindow : Window
    {
        public ClientsSubscribesWindow()
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

    }
}
