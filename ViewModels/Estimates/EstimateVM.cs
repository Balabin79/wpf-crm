using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Dental.ViewModels.Estimates
{
    class EstimateVM : BindableBase, IDataErrorInfo
    {
        private readonly ApplicationContext db;
        public EstimateVM(ApplicationContext db)
        {
            this.db = db;
            Clients = db.Clients.OrderBy(f => f.LastName).ToArray();
        }

        [Required(ErrorMessage = @"Поле ""Название сметы"" обязательно для заполнения")]
        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }

        public string StartDate
        {
            get { return GetProperty(() => StartDate); }
            set { SetProperty(() => StartDate, value); }
        }

        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }

        public string Title
        {
            get { return GetProperty(() => Title); }
            set { SetProperty(() => Title, value); }
        }

        public Visibility ClientFieldVisibility
        {
            get { return GetProperty(() => ClientFieldVisibility); }
            set { SetProperty(() => ClientFieldVisibility, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public ICollection<Client> Clients { get; set; }
    }
}
