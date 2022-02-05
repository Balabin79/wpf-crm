using DevExpress.Xpf.Grid;
using System.Windows;

namespace Dental.Views.PatientCard
{
    public partial class GroupsWindow : Window
    {
        public GroupsWindow()
        {
            InitializeComponent();
        }

        private void TableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            ((TableView)sender).PostEditor();
        }

        private void TextEdit_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if(e.NewValue?.ToString() == "0" )
            {
                ((DevExpress.Xpf.Editors.BaseEdit)sender).EditValue = "";
            }
            e.Handled = true;
        }
    }
}
