using DevExpress.Xpf.Editors.Flyout;
using DevExpress.Xpf.Grid;
using System.Windows;
using System.Windows.Controls;

namespace Dental.Views
{
    public partial class Organization : Page
    {
        public Organization()
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
