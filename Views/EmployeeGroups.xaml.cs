using DevExpress.Xpf.Editors.Flyout;
using DevExpress.Xpf.Grid;
using System.Windows;
using System.Windows.Controls;

namespace Dental.Views
{
    public partial class EmployeeGroups : Page
    {
        public EmployeeGroups()
        {
            InitializeComponent();
        }

        private void TableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            ((TableView)sender).PostEditor();
        }
    }
}
