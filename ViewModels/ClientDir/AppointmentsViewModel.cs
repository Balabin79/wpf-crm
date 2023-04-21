using B6CRM.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.ViewModels.ClientDir
{
    public class AppointmentsViewModel : ViewModelBase
    {

        [Command]
        public void Load(object p)
        {
            if (p is Client model) {
            using (var db = new ApplicationContext())
            {
                Appointments = db.Appointments.Where(f => f.ClientInfoId == model.Id)
                    .Include(f => f.Employee)
                    .Include(f => f.Service)
                    .OrderByDescending(f => f.CreatedAt).ToObservableCollection() 
                    ?? 
                    new ObservableCollection<Appointments>();
                }
            }
        }

        public ObservableCollection<Appointments> Appointments
        {
            get { return GetProperty(() => Appointments); }
            set { SetProperty(() => Appointments, value); }
        } 
    }
}
