using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using Dental.Services;
using Dental.Infrastructures.Extensions.Notifications;
using System.IO;
using System.Windows.Media.Imaging;

namespace Dental.ViewModels
{
    class ShedulerViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public ShedulerViewModel()
        {
            try
            {
                db = new ApplicationContext();

                SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
                AppointmentAddedCommand = new LambdaCommand(OnAppointmentAddedCommandExecuted, CanAppointmentAddedCommandExecute);

                Doctors = db.Employes.OrderBy(d => d.LastName).Include(f => f.EmployesSpecialities.Select(i => i.Speciality)).ToObservableCollection();
                foreach (var i in Doctors)
                {
                    if (!string.IsNullOrEmpty(i.Photo) && File.Exists(i.Photo))
                    {
                        using (var stream = new FileStream(i.Photo, FileMode.Open))
                        {
                            var img = new BitmapImage();
                            img.BeginInit();
                            img.CacheOption = BitmapCacheOption.OnLoad;
                            img.StreamSource = stream;
                            img.EndInit();
                            img.Freeze();
                            i.Image = img;
                            stream.Close(); stream.Dispose();
                        }
                    }
                    else i.Image = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/user_s.png"));
                    //специальности
                    i.Specialities = string.Join(",", i?.EmployesSpecialities?.Select(f => f.Speciality.Name)?.ToArray());
                    if (i.Specialities.Length < 1) i.Specialities = "Специальность не указана";
                }

                Patients = db.PatientInfo.ToObservableCollection();

                CreateDoctors();
                CreateCalendars();
                CreateMedicalAppointments();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Расписание\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand AppointmentAddedCommand { get; }
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanAppointmentAddedCommandExecute(object p) => true;
        private void OnAppointmentAddedCommandExecuted(object p)
        {
            try
            {
                foreach(var i in Appointments)
                {
                    if (db.Entry(i).State == EntityState.Detached) db.Entry(i).State = EntityState.Added;
                }

                int cnt = db.SaveChanges();
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
                
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }


        public virtual ObservableCollection<Employee> Doctors { get; set; }
        public virtual ObservableCollection<PatientInfo> Patients { get; set; }
        public virtual ObservableCollection<MedicalAppointment> Appointments { get; set; }
        public ObservableCollection<ResourceEntity> Calendars { get; set; }

        private void CreateDoctors() => Doctors = db.Employes.Include("EmployesSpecialities").ToObservableCollection();        
        private void CreateCalendars() => Calendars = db.Resources.ToObservableCollection();
        private void CreateMedicalAppointments() => Appointments = db.Appointments.ToObservableCollection();

    }
}
