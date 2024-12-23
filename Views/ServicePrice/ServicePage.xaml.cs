﻿using B6CRM.Models.Base;
using B6CRM.ViewModels.Base;
using B6CRM.ViewModels.ServicePrice;
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
using Microsoft.EntityFrameworkCore;
using B6CRM.Models;
using B6CRM.Services;

namespace B6CRM.Views.ServicePrice
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : UserControl
    {
        public ServicePage()
        {
            InitializeComponent();
            try
            {
                var db = new ApplicationContext();
                DataContext = new ServiceViewModel(db, db?.Services);
            }
            catch(Exception e) 
            {
                Log.ErrorHandler(e);
            }
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
