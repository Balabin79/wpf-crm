using Dental.Models;
using System.Collections.Generic;
using System.Windows;

namespace Dental.Views.NomenclatureDir
{
    public partial class NomenclatureWindow : Window
    {
        public NomenclatureWindow()
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

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

    }
}
