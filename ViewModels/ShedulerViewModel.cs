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
using Dental.Views.WindowForms;

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
                LocationAppointments = GetLocationCollection();

                SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);

                OpenWindowLocationCommand = new LambdaCommand(OnOpenWindowLocationExecuted, CanOpenWindowLocationExecute);
                CloseWindowLocationCommand = new LambdaCommand(OnCloseWindowLocationExecuted, CanCloseWindowLocationExecute);
                AddLocationCommand = new LambdaCommand(OnAddLocationExecuted, CanAddLocationExecute);
                DeleteLocationCommand = new LambdaCommand(OnDeleteLocationExecuted, CanDeleteLocationExecute);
                SaveLocationCommand = new LambdaCommand(OnSaveLocationExecuted, CanSaveLocationExecute);

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
                    else i.Image = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/avatar.png"));
                    //специальности
                    i.Specialities = string.Join(",", i?.EmployesSpecialities?.Select(f => f.Speciality.Name)?.ToArray());
                    if (i.Specialities.Length < 1) i.Specialities = "Специальность не указана";
                }

                Clients = db.PatientInfo.ToObservableCollection();

                CreateDoctors();
                CreateCalendars();
                CreateMedicalAppointments();
                LocationAppointments.ForEach(f => LocationAppointmentsBeforeChanges.Add((LocationAppointment)f.Clone()));
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


        #region Справочник "Места встреч"
        public ICommand OpenWindowLocationCommand { get; }
        public ICommand CloseWindowLocationCommand { get; }

        public ICommand AddLocationCommand { get; }
        public ICommand DeleteLocationCommand { get; }
        public ICommand SaveLocationCommand { get; }
        private bool CanOpenWindowLocationExecute(object p) => true;
        private bool CanCloseWindowLocationExecute(object p) => true;
        private bool CanAddLocationExecute(object p) => true;
        private bool CanDeleteLocationExecute(object p) => true;
        private bool CanSaveLocationExecute(object p) => true;

        private void OnOpenWindowLocationExecuted(object p)
        {
            try
            {
                LocationWindow = new LocationAppointmentWindow();
                LocationWindow.DataContext = this; 
                LocationWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCloseWindowLocationExecuted(object p) => LocationWindow.Close();

        private void OnAddLocationExecuted(object p)
        {
            try
            {
                LocationAppointments.Add(new LocationAppointment());
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnDeleteLocationExecuted(object p)
        {
            try
            {
                if (p is LocationAppointment model)
                {
                    if (model.Id != 0 && !new ConfirDeleteInCollection().run(0)) return;
                    if (model.Id != 0) db.Entry(model).State = EntityState.Deleted;
                    else db.Entry(model).State = EntityState.Detached;
                    int cnt = db.SaveChanges();
                    LocationAppointments = GetLocationCollection();
                    if (cnt > 0)
                    {
                        LocationAppointmentsBeforeChanges.Clear();
                        LocationAppointments.ForEach(f => LocationAppointmentsBeforeChanges.Add((LocationAppointment)f.Clone()));
                    }
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }


        private void OnSaveLocationExecuted(object p)
        {
            try
            {
                foreach (var item in LocationAppointments)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    if (item.Id == 0) db.Entry(item).State = EntityState.Added;
                }
                int cnt = db.SaveChanges();
                LocationAppointments = GetLocationCollection();
                LocationAppointmentsBeforeChanges.Clear();
                LocationAppointments.ForEach(f => LocationAppointmentsBeforeChanges.Add((LocationAppointment)f.Clone()));
                if (cnt > 0)
                {
                    var notification = new Notification();
                    notification.Content = "Изменения сохранены в базу данных!";
                    notification.run();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public LocationAppointmentWindow LocationWindow { get; set; }
        public ObservableCollection<LocationAppointment> LocationAppointments 
        {
            get => locationAppointments;
            set => Set(ref locationAppointments, value); 
        }
        private ObservableCollection<LocationAppointment> locationAppointments;

        public ObservableCollection<LocationAppointment> LocationAppointmentsBeforeChanges { get; set; } = new ObservableCollection<LocationAppointment>();
        private ObservableCollection<LocationAppointment>  GetLocationCollection() => db.LocationAppointment.OrderBy(f => f.Name).ToObservableCollection();
        #endregion


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
        public virtual ObservableCollection<PatientInfo> Clients { get; set; }
        public virtual ObservableCollection<MedicalAppointment> Appointments { get; set; }
        public ObservableCollection<ResourceEntity> Calendars { get; set; }

        private void CreateDoctors() => Doctors = db.Employes.Include("EmployesSpecialities").ToObservableCollection();        
        private void CreateCalendars() => Calendars = db.Resources.ToObservableCollection();
        private void CreateMedicalAppointments() => Appointments = db.Appointments.Include(f => f.ClientInfo).ToObservableCollection();

    }
}
