﻿using Dental.Models.Base;
using Dental.ViewModels;
using DevExpress.Xpf.WindowsUI.Navigation;
using System;
using System.Windows.Controls;

namespace Dental.Views
{
    /// <summary>
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class Employee : Page, IUser
    {
        public Employee() : this(0){}

        public Employee(int id)
        {
            InitializeComponent();
            this.DataContext = new EmployeeViewModel(id);
            UserId = id;
        }

        public int UserId { get; set; }
    }
}
