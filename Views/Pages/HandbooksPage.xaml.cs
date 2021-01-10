using System.Windows;
using System.Windows.Controls;

namespace Dental.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для HandbooksPage.xaml
    /// </summary>
    public partial class HandbooksPage : Page
    {
        public HandbooksPage()
        {
            InitializeComponent();
            this.DataContext = Application.Current.Windows[0].Resources["viewModel"];

        }

        private void BarButtonItem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            int x = 0;
        }
    }
}
