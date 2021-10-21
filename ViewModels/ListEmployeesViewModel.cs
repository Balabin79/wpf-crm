using System;
using System.Linq;
using Dental.Models;
using System.Data.Entity;
using System.Collections;
using System.Windows.Media.Imaging;
using System.IO;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Services;

namespace Dental.ViewModels
{
    class ListEmployeesViewModel : ViewModelBase
    {

        private ApplicationContext db;
        public ListEmployeesViewModel()
        {
            try
            {
                db = Db.Instance.Context;
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Список сотрудников\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }


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
