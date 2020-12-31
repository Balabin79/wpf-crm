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
    }
}
