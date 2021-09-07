using System.Windows;
using System.Windows.Controls;

namespace Dental.Views
{
    public partial class DiscountGroups : Page
    {
        public DiscountGroups()
        {
            InitializeComponent();
        }

        void OpenFlyout(object sender, RoutedEventArgs e)
        {
            flyoutControl.PlacementTarget = sender as FrameworkElement;
            flyoutControl.Content = "Раздел \"Реклама\" помогает отслеживать источники, благодаря которым клиенты узнают о клинике.\nЭто позволяет систематизировать и анализировать информацию об эффективности рекламы в различных источниках. \n1. Добавьте в справочник \"Рекламные источники\" позиции, которые вам необходимы. \n2. Заполняя карту пациенты, в поле \"Рекламные источники\" выберите источник, благодаря которому клиент узнал о клинике. \n3. Сводные данные по источникам рекламы и клиентам доступны в разделе \"Отчеты\". ";
            flyoutControl.IsOpen = true;
        }
    }
}
