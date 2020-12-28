using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Dental.Views.HandbooksPages
{
    /// <summary>
    /// Логика взаимодействия для Organizations.xaml
    /// </summary>
    public partial class Organizations : Page
    {
        public Organizations()
        {
            InitializeComponent();
       //     this.deleteRowItem.ItemClick += deleteRowItemX;
        }


        void addNewRow(object sender, RoutedEventArgs e)
        {
            view.AddNewRow();
            int newRowHandle = DataControlBase.NewItemRowHandle;
            GridOrganisation.SetCellValue(newRowHandle, "Name", "New Product");
            GridOrganisation.SetCellValue(newRowHandle, "GenDirector", "New Company");
            GridOrganisation.SetCellValue(newRowHandle, "Phone","6546565656");
            GridOrganisation.SetCellValue(newRowHandle, "Email", "fwewerfwerfwef");
        }

        private void deleteRowItemX(object sender, ItemClickEventArgs e)
        {
            GridCellMenuInfo menuInfo = view.GridMenu.MenuInfo as GridCellMenuInfo;
            if (menuInfo != null && menuInfo.Row != null)
                view.DeleteRow(menuInfo.Row.RowHandle.Value);
        }

    }
}
