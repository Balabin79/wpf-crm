using Dental.Models;
using Dental.Models.Base;
using Dental.Services;
using Dental.ViewModels.Base;
using Dental.ViewModels.ServicePrice;
using DevExpress.Xpf.Core;
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

namespace Dental.Views.ServicePrice
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : UserControl
    {
        public ServicePage()
        {
            InitializeComponent();
            var db = new ConnectToDb().Context;
            DataContext = new ServiceViewModel(db, db?.Services);
        }

        void OnDragRecordOver(object sender, DragRecordOverEventArgs e)
        {
            if (e.TargetRecord is Service model && model.IsDir == 0)
            {
                //e.DropPosition = e.DropPositionRelativeCoefficient > 0.5 ? DropPosition.After : DropPosition.Before;
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                //e.
                return;
            }
        }
    }
}
