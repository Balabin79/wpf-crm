using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Interfaces.Template;
using Dental.Models;
using Dental.Models.Base;
using Dental.Repositories;
using DevExpress.Xpf.Grid;
using System.Data.Entity;
using System.Collections;
using System.Windows.Media.Imaging;

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
                Context.OrderBy(d => d.LastName).ToList().ForEach(f => f.Image = string.IsNullOrEmpty(f.Photo) ? null : new BitmapImage(new Uri(f.Photo)));
                return Context.Local;
            }
        }


    }
}
