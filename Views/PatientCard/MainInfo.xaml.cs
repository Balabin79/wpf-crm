using System.Windows.Controls;
using DevExpress.Xpf.WindowsUI;

namespace B6CRM.Views.PatientCard
{
    public partial class MainInfo : UserControl
    {
        public MainInfo()
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
