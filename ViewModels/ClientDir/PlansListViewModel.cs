using B6CRM.Models;
using B6CRM.Services;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.ViewModels.ClientDir
{
    internal class PlansListViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public PlansListViewModel()
        {
            try
            {
                db = new ApplicationContext();
                //Db = db;
                // Config = db.Config;
                //SelectedClient = new Client();
                //LoadClients();
                //LoadPrintConditions();
                LoadEmployees();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка подключения к базе данных при попытке загрузить список планов работ!", true);
            }
        }

        public void LoadEmployees()
        {
            Employees = db.Employes.OrderBy(f => f.LastName).ToObservableCollection() ?? new ObservableCollection<Employee>();
            foreach (var i in Employees) i.IsVisible = false;
        }

        public ObservableCollection<Employee> Employees
        {
            get { return GetProperty(() => Employees); }
            set { SetProperty(() => Employees, value); }
        }
    }
}
