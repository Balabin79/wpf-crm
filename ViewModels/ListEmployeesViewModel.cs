﻿using System;
using System.Linq;
using Dental.Models;
using System.Data.Entity;
using System.Collections;
using System.Windows.Media.Imaging;
using System.IO;

namespace Dental.ViewModels
{
    class ListEmployeesViewModel : ViewModelBase
    {


        public ListEmployeesViewModel()
        {
            db = new ApplicationContext();
        }

        private ApplicationContext db;

        protected DbSet<Employee> Context { get => db.Employes; }

        public IEnumerable Employees
        {
            get 
            {
                Context.OrderBy(d => d.LastName).ToList()
                    .ForEach(f => f.Image = !string.IsNullOrEmpty(f.Photo) && File.Exists(f.Photo) ? new BitmapImage(new Uri(f.Photo)) : null);
                return Context.Local;
            }
        }
    }
}
