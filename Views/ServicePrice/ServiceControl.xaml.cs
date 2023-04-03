using B6CRM.Models;
using B6CRM.Models.Base;
using B6CRM.Services;
using B6CRM.ViewModels.ServicePrice;
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
using Microsoft.EntityFrameworkCore;

namespace B6CRM.Views.ServicePrice
{
    /// <summary>
    /// Логика взаимодействия для ServiceControl.xaml
    /// </summary>
    public partial class ServiceControl : UserControl
    {
        public ServiceControl()
        {
            InitializeComponent();
            var db = new ApplicationContext();
            DataContext = new ServiceViewModel(db, db?.Services);
        }
    }
}
