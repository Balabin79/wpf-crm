﻿using DevExpress.Mvvm.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class Employee : UserControl
    {
        public Employee()
        {
            InitializeComponent();
        }

        private void ImageEdit_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            int x = 0;
        }

        private void ImageEdit_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            int x = 0;
        }
    }
}
