using Dental.Models;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace Dental.ViewModels
{
    public class InvoiceViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public InvoiceViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Collection = db.Appointments.Where(f => !string.IsNullOrEmpty(f.StartTime)).Include(f => f.ClientInfo).Include(f => f.Employee).ToObservableCollection();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Счета\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
        public ICollection<Appointments> Collection { get; set; }
    }
}
