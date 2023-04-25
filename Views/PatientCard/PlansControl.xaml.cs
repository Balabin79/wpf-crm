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

namespace B6CRM.Views.PatientCard
{
    /// <summary>
    /// Логика взаимодействия для InvoicesControl.xaml
    /// </summary>
    public partial class PlansControl : UserControl
    {
        public PlansControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) => grid.SelectedItem = null;
    }
}
