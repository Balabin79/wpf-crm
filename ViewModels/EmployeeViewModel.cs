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
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm;
using System.Windows.Media.Imaging;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Windows;

namespace Dental.ViewModels
{
    class EmployeeViewModel : ViewModelBase
    {

        public EmployeeViewModel() : this(0){}

        public EmployeeViewModel(int employeeId = 0)
        {
            CancelCommand = new LambdaCommand(OnCancelCommandExecuted, CanCancelCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenCommand = new LambdaCommand(OnOpenCommandExecuted, CanOpenCommandExecute);

            db = new ApplicationContext();
            context = db.Employes;
            VisibleErrors = Visibility.Collapsed;

            try
            {
                Employee = employeeId == 0 ? new Employee() : context.Where(i => i.Id == employeeId).First();
            } 
            catch (Exception e)
            {
                Employee = new Employee();
                (new ViewModelLog(e)).run();
                Errors = new List<string>(){"Ошибка при загрузке данных сотрудника. " +
                    "Возможно данные повреждены или удалены." +
                    "Вернитесь к списку сотрудников и повторите попытку." +
                    "При повторении ошибки, обратитесь за помощью к администратору или в тех.поддержку" };
            }           
        }

        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }

        private bool CanCancelCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanOpenCommandExecute(object p) => true;

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
                    return;
                }

                db.Entry(Employee).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                int k = 0;
                var errors = ex.EntityValidationErrors.First().ValidationErrors.Select(f => f.ErrorMessage);

                if (errors.Count() > 0) { 
                    Errors = errors;
                    VisibleErrors = Visibility.Visible;
                }
                int x = 0;

            }
            catch (Exception e)
            {
                int x = 0;
                /*
                var error = e.EntityValidationErrors.First().ValidationErrors.First();
                this.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);*/
            }
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
 
    
        public Employee Employee { get; set; } = new Employee();

        private readonly ApplicationContext db;
        private readonly DbSet<Employee> context;
        private IEnumerable<string> errors;
        public object visibleErrors;

        public DbSet<Employee> Context => context;

        public object VisibleErrors
        {
            get => visibleErrors;
            set => Set(ref visibleErrors, value);
        }



        public IEnumerable<string> Errors {
            get => errors;
            set => Set(ref errors, value);
        }




        
        
        [ServiceProperty(Key = "NotificationService")]
        protected virtual INotificationService AppNotificationService { get { return null; } }
   
        public void ShowNotification()
        {
            INotification notification = AppNotificationService.CreatePredefinedNotification("DevAV Tips & Tricks",
                "Take user where they want to go with", "DevExpress Map Controls.",
                new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/status11.png", UriKind.Absolute)));
            notification.ShowAsync();
        }

        }
}
