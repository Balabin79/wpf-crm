using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;

namespace Dental.ViewModels.Invoices
{
    public class InvoiceVM : ViewModelBase, IDataErrorInfo
    {
        public delegate void SaveCommand(object m);
        public event SaveCommand EventSave;

        public InvoiceVM(ICollection<Employee> employees) => Employees = employees;            

        [Required(ErrorMessage = @"Поле ""Название счета"" обязательно для заполнения")]
        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }

        public string Number
        {
            get { return GetProperty(() => Number); }
            set { SetProperty(() => Number, value); }
        }

        [Required(ErrorMessage = @"Поле ""Клиент"" обязательно для заполнения")]
        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }

        public Employee Employee
        {
            get { return GetProperty(() => Employee); }
            set { SetProperty(() => Employee, value); }
        }    
        
        public string Date
        {
            get { return GetProperty(() => Date); }
            set { SetProperty(() => Date, value); }
        }        
        
        public long? DateTimestamp
        {
            get { return GetProperty(() => DateTimestamp); }
            set { SetProperty(() => DateTimestamp, value); }
        }
        
        public int? Paid
        {
            get { return GetProperty(() => Paid); }
            set { SetProperty(() => Paid, value); }
        }

        public Invoice Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
        }

        public ICollection<Employee> Employees { get; set; }


        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public ICollection<Client> Clients { get; set; }

        [Command]
        public void Save(object p)
        {
            try
            {
                Model.Name = Name;
                Model.Employee = Employee;
                Model.EmployeeId = Employee?.Id;
                Model.Date = Date;
                Model.DateTimestamp = DateTimestamp;
                Model.Paid = Paid;

                EventSave?.Invoke(Model);
                if (p is Window win) win?.Close();
            }
            catch
            {
                if (p is Window win) win?.Close();
            }
        }
    }
}
