using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using System.IO;
using System.Windows.Media.Imaging;
using Dental.Views.WindowForms;
using System.Windows.Media;
using DevExpress.Mvvm.DataAnnotations;

namespace Dental.ViewModels
{
    class ShedulerViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public ShedulerViewModel()
        {
            try
            {
                db = new ApplicationContext();
                LocationAppointments = GetLocationCollection();
                StatusAppointments = GetStatusCollection();
                ClassificatorCategories = db.Services.ToObservableCollection();

                Doctors = db.Employes.Where(f => f.IsInSheduler != null && f.IsInSheduler > 0).OrderBy(d => d.LastName).ToObservableCollection();
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
                }

                Clients = db.Clients.OrderBy(f => f.LastName).ToObservableCollection();
                SelectedDoctors = new List<object>();
                Doctors.ForEach(f => SelectedDoctors.Add(f));
                CreateCalendars();

                Appointments = db.Appointments.Include(f => f.Service).Include(f => f.Employee).Include(f => f.ClientInfo).Include(f => f.Location)
                    .Where(f => !string.IsNullOrEmpty(f.StartTime)).OrderBy(f => f.CreatedAt).ToObservableCollection();

                LocationAppointments.ForEach(f => LocationAppointmentsBeforeChanges.Add((LocationAppointment)f.Clone()));
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Расписание\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

     
        [Command]
        public void AppointmentAdded()
        {
            try
            {
                foreach (var i in Appointments)
                {
                    if (db.Entry(i).State == EntityState.Detached)
                    {
                        //var emp = db.Employes.Where(f => f.Id == i.EmployeeId).FirstOrDefault();
                        //var client = db.Clients.Where(f => f.Id == i.ClientInfoId).FirstOrDefault();
                        var serv = db.Services.Where(f => f.Id == i.ServiceId).FirstOrDefault();
                        i.Price = serv?.Price;
                        db.Entry(i).State = EntityState.Added;
                    }
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void AppointmentEdited(object p)
        {
            try
            {
                if (p is DevExpress.Xpf.Scheduling.AppointmentEditedEventArgs appointment)
                {
                    foreach(var i in Appointments)
                    {
                        var emp = db.Employes.Where(f => f.Id == i.EmployeeId).FirstOrDefault();
                        var client = db.Clients.Where(f => f.Id == i.ClientInfoId).FirstOrDefault();
                        var serv = db.Services.Where(f => f.Id == i.ServiceId).FirstOrDefault();
                        i.Price = serv?.Price;
                    }
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void AppointmentRemoved(object p)
        {
            try
            {
                if (p is DevExpress.Xpf.Scheduling.AppointmentRemovedEventArgs arg)
                foreach (var i in arg.Appointments)
                {
                    var item = db.Appointments.Where(f => f.Guid == ((Appointments)i.SourceObject).Guid)?.FirstOrDefault();
                    if (item != null) db.Entry(item).State = EntityState.Deleted;                   
                }
                int cnt = db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        #region Справочник "Места встреч"
        [Command]
        public void OpenWindowLocation()
        {
            try
            {
                LocationWindow = new LocationAppointmentWindow() { DataContext = this };
                LocationWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void CloseWindowLocation() => LocationWindow.Close();

        [Command]
        public void AddLocation(object p)
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

        [Command]
        public void DeleteLocation(object p)
        {
            try
            {
                if (p is LocationAppointment model)
                {
                    int cnt = 0;
                    if (model.Id != 0)
                    {
                        if (!new ConfirDeleteInCollection().run(0)) return;
                        db.Entry(model).State = EntityState.Deleted;
                        cnt = db.SaveChanges();
                    }

                    else db.Entry(model).State = EntityState.Detached;
                    LocationAppointments.Remove(LocationAppointments.Where(f => f.Guid == model.Guid).FirstOrDefault());
                    //LocationAppointments = GetLocationCollection();
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

        [Command]
        public void SaveLocation()
        {
            try
            {
                foreach (var item in LocationAppointments)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    if (item.Id == 0) db.Entry(item).State = EntityState.Added;
                }
                LocationAppointments = GetLocationCollection();
                LocationAppointmentsBeforeChanges.Clear();
                LocationAppointments.ForEach(f => LocationAppointmentsBeforeChanges.Add((LocationAppointment)f.Clone()));
                if (db.SaveChanges() > 0) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
            }
            catch (Exception e)
            {
                new ViewModelLog(e).run();
            }
        }

        public LocationAppointmentWindow LocationWindow { get; set; }

        public ObservableCollection<LocationAppointment> LocationAppointments
        {
            get { return GetProperty(() => LocationAppointments); }
            set { SetProperty(() => LocationAppointments, value); }
        }

        public ObservableCollection<LocationAppointment> LocationAppointmentsBeforeChanges { get; set; } = new ObservableCollection<LocationAppointment>();
        private ObservableCollection<LocationAppointment>  GetLocationCollection() => db.LocationAppointment.OrderBy(f => f.Name).ToObservableCollection();
        #endregion

        #region Справочник "Статусы в шедулере"       
        [Command]
        public void OpenWindowStatus()
        {
            try
            {
                StatusWindow = new StatusAppointmentWindow() { DataContext = this };
                StatusWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void CloseWindowStatus() => StatusWindow.Close();

        [Command]
        public void AddStatus(object p)
        {
            try
            {
                StatusAppointments.Add(new AppointmentStatus());
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void SaveStatus()
        {
            try
            {
                foreach (var item in StatusAppointments)
                {
                    if (string.IsNullOrEmpty(item.Caption)) continue;

                    if (item.Id == 0) db.Entry(item).State = EntityState.Added;
                    // если эл-т новый или модифицированный, то необходимо сериализовать цвет и присвоить соответствующему полю
                    if (db.Entry(item).State == EntityState.Added
                        || StatusAppointmentsBeforeChanges.Where(f => item.Guid == f.Guid && item.Brush != f.Brush).FirstOrDefault() != null)
                    {
                        item.BrushColor = item.Brush?.Color.ToString();
                    }
                }
                StatusAppointmentsBeforeChanges.Clear();
                StatusAppointments.ForEach(f => StatusAppointmentsBeforeChanges.Add((AppointmentStatus)f.Clone()));
                
                if (db.SaveChanges() > 0) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void DeleteStatus(object p)
        {
            try
            {
                if (p is AppointmentStatus model)
                {
                    int cnt = 0;
                    if (model.Id != 0)
                    {
                        if (!new ConfirDeleteInCollection().run(0)) return;
                        db.Entry(model).State = EntityState.Deleted;
                        cnt = db.SaveChanges();
                    }

                    else db.Entry(model).State = EntityState.Detached;
                    StatusAppointments.Remove(StatusAppointments.Where(f => f.Guid == model.Guid).FirstOrDefault());

                    if (cnt > 0)
                    {
                        StatusAppointmentsBeforeChanges.Clear();
                        StatusAppointments.ForEach(f => StatusAppointmentsBeforeChanges.Add((AppointmentStatus)f.Clone()));
                    }
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public StatusAppointmentWindow StatusWindow { get; set; }

        public ObservableCollection<AppointmentStatus> StatusAppointments
        {
            get { return GetProperty(() => StatusAppointments); }
            set { SetProperty(() => StatusAppointments, value); }
        }

        public ObservableCollection<AppointmentStatus> StatusAppointmentsBeforeChanges { get; set; } = new ObservableCollection<AppointmentStatus>();
        private ObservableCollection<AppointmentStatus> GetStatusCollection()
        {
            var collection = db.AppointmentStatus.OrderBy(f => f.Caption).ToObservableCollection();
            foreach (var i in collection)
            {
                try
                {
                    Color colorName = (Color)ColorConverter.ConvertFromString(i.BrushColor);
                    i.Brush = new SolidColorBrush(colorName);
                }
                catch
                {
                    i.Brush = null;
                }
            }

            return collection;
        }
        #endregion

        [Command]
        public void Save(object p)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public ObservableCollection<Service> ClassificatorCategories { get; set; }
        public virtual ObservableCollection<Employee> Doctors { get; set; }

        public virtual ObservableCollection<Client> Clients { get; set; }
        public virtual ObservableCollection<Appointments> Appointments { get; set; }
        public ObservableCollection<ResourceEntity> Calendars { get; set; }

        public virtual List<object> SelectedDoctors { get; set; }
        
        private void CreateCalendars() => Calendars = db.Resources.ToObservableCollection();
    }
}
