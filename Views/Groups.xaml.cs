using DevExpress.Xpf.Editors.Flyout;
using DevExpress.Xpf.Grid;
using System.Windows;
using System.Windows.Controls;

namespace Dental.Views
{
    public partial class Groups : Page
    {
        public Groups()
        {
            InitializeComponent();
        }

        void OpenFlyout(object sender, RoutedEventArgs e)
        {
            flyoutControl.PlacementTarget = sender as FrameworkElement;
            flyoutControl.Content = "Раздел \"Категории клиентов\" позволяет объединять клиентов в группы, которые можно\nиспользовать для примененения выборочной скидки, маркетинговых рассылок и т.д. \n1. Добавьте в справочник \"Категории клиентов\" позиции, которые вам необходимы. \n2. Заполняя карту пациенты, в поле \"Категории клиентов\" выберите соответствующую позицию.\n";
            flyoutControl.IsOpen = true;
        }

    }
}
