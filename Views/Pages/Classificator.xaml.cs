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

namespace Dental.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Classificator.xaml
    /// </summary>
    public partial class Classificator : Page
    {
        public Classificator()
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
