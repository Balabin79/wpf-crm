using System;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using System.IO;
using DevExpress.Data.ODataLinq.Helpers;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using Dental.Services;

namespace Dental.ViewModels
{
    class EmployeeViewModel : ViewModelBase
    {
        public EmployeeViewModel() : this(0){}

        public EmployeeViewModel(int? employeeId = 0)
        {
            CancelCommand = new LambdaCommand(OnCancelCommandExecuted, CanCancelCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenCommand = new LambdaCommand(OnOpenCommandExecuted, CanOpenCommandExecute);

            db = Db.Instance.Context; 
            context = db.Employes;
            Collection = GetCollection();
            VisibleErrors = Visibility.Collapsed;

            try
            {
                if (employeeId == 0 || employeeId == null)
                {
                    Employee = new Employee();
                    Title = "Новый сотрудник";
                }
                else
                {
                    Employee = context.Where(i => i.Id == employeeId).First();                   
                    Employee.Image = !string.IsNullOrEmpty(Employee.Photo) && File.Exists(Employee.Photo) ? new BitmapImage(new Uri(Employee.Photo)) : null;
                    Title = Employee.FullName;                 
                }
                Address = new AddressViewModel(Employee);
            } 
            catch (Exception e)
            {
                Title = "Новый сотрудник";
                Employee = new Employee();
                (new ViewModelLog(e)).run();
                Errors = new List<string>(){"Ошибка при загрузке данных сотрудника. " +
                    "Возможно данные повреждены или удалены." +
                    "Вернитесь к списку сотрудников и повторите попытку." +
                    "При повторении ошибки, обратитесь за помощью к администратору или в тех.поддержку" };
                VisibleErrors = true;
            }           
        }

        public Employee Employee { get; set; }
        public AddressViewModel Address { get; set; }
        public DbSet<Employee> Context => context;

        public object VisibleErrors
        {
            get => visibleErrors;
            set => Set(ref visibleErrors, value);
        }

        public IEnumerable<string> Errors
        {
            get => errors;
            set => Set(ref errors, value);
        }

        public string Title
        {
            get => title;
            set => Set(ref title, value);
        }


        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }


        private void OnCancelCommandExecuted(object p)
        {
            try
            {

               // Repository.Delete(table);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSaveCommandExecuted(object p)
        {
            try
            {
                Errors = null;
                VisibleErrors = Visibility.Collapsed;

                if (Employee.Id == 0)
                {
                    context.Add(Employee);
                    db.SaveChanges();

                    new Notification().run();
                    return;
                }
                db.Entry(Employee).State = EntityState.Modified;
                db.SaveChanges();            
            }
            catch (DbEntityValidationException ex)
            {
                var errors = ex.EntityValidationErrors.First().ValidationErrors.Select(f => f.ErrorMessage);

                if (errors.Count() > 0) { 
                    Errors = errors;
                    VisibleErrors = Visibility.Visible;
                }
            }
            catch (Exception e){}
        }

        private void OnOpenCommandExecuted(object p)
        {
            int x = 0;
            try
            {

                //Repository.Update(table);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
 
        private readonly ApplicationContext db;
        private readonly DbSet<Employee> context;
        private IEnumerable<string> errors;
        private string title;
        private object visibleErrors;

        private bool CanCancelCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanOpenCommandExecute(object p) => true;


        private ObservableCollection<Employee> _Collection;
        public ObservableCollection<Employee> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }

        private ObservableCollection<Employee> GetCollection() => db.Employes.OrderBy(d => d.LastName).ToObservableCollection();
    }
}
