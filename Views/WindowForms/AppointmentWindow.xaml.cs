﻿using DevExpress.Xpf.Core;
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
using System.Windows.Shapes;

namespace Dental.Views.WindowForms
{
    /// <summary>
    /// Логика взаимодействия для AppointmentWindow.xaml
    /// </summary>
    public partial class AppointmentWindow : ThemedWindow
    {
        public AppointmentWindow()
        {
            InitializeComponent();
        }

        private void GridColumn_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}