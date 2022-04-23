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
using Dental.Infrastructures.Extensions.Notifications;
using System.IO;
using System.Windows.Media.Imaging;
using Dental.Views.WindowForms;
using System.Windows.Media;

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
                StatusAppointments = GetStatusCollection();
                ClassificatorCategories = db.Services.ToObservableCollection();

                SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);

                #region Команды формы Appointment
                AppointmentAddedCommand = new LambdaCommand(OnAppointmentAddedCommandExecuted, CanAppointmentAddedCommandExecute);
                AppointmentEditedCommand = new LambdaCommand(OnAppointmentEditedCommandExecuted, CanAppointmentEditedCommandExecute);
                AppointmentRemovedCommand = new LambdaCommand(OnAppointmentRemovedCommandExecuted, CanAppointmentRemovedCommandExecute);
                #endregion 

                #region Команды локации встреч
                OpenWindowLocationCommand = new LambdaCommand(OnOpenWindowLocationExecuted, CanOpenWindowLocationExecute);
                CloseWindowLocationCommand = new LambdaCommand(OnCloseWindowLocationExecuted, CanCloseWindowLocationExecute);
                AddLocationCommand = new LambdaCommand(OnAddLocationExecuted, CanAddLocationExecute);
                DeleteLocationCommand = new LambdaCommand(OnDeleteLocationExecuted, CanDeleteLocationExecute);
                SaveLocationCommand = new LambdaCommand(OnSaveLocationExecuted, CanSaveLocationExecute);
                #endregion

                #region Команды статусы
                OpenWindowStatusCommand = new LambdaCommand(OnOpenWindowStatusExecuted, CanOpenWindowStatusExecute);
                CloseWindowStatusCommand = new LambdaCommand(OnCloseWindowStatusExecuted, CanCloseWindowStatusExecute);
                AddStatusCommand = new LambdaCommand(OnAddStatusExecuted, CanAddStatusExecute);
                DeleteStatusCommand = new LambdaCommand(OnDeleteStatusExecuted, CanDeleteStatusExecute);
                SaveStatusCommand = new LambdaCommand(OnSaveStatusExecuted, CanSaveStatusExecute);
                #endregion

                AppointmentAddedCommand = new LambdaCommand(OnAppointmentAddedCommandExecuted, CanAppointmentAddedCommandExecute);

                Doctors = db.Employes.OrderBy(d => d.LastName).ToObservableCollection();
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

                Clients = db.Clients.ToObservableCollection();
                SelectedDoctors = new List<object>();
                Doctors.ForEach(f => SelectedDoctors.Add(f));
                CreateCalendars();

                Appointments = db.Appointments.Include(f => f.Service).Include(f => f.Employee).Include(f => f.ClientInfo).Include(f => f.Location)
                    .Where(f => !string.IsNullOrEmpty(f.StartTime)).OrderBy(f => f.CreatedAt).ToObservableCollection();

                LocationAppointments.ForEach(f => LocationAppointmentsBeforeChanges.Add((LocationAppointment)f.Clone()));
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Расписание\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand SaveCommand { get; }
        
        private bool CanSaveCommandExecute(object p) => true;
        
        #region "Форма Appointment"
        public ICommand AppointmentAddedCommand { get; }
        public ICommand AppointmentEditedCommand { get; }
        public ICommand AppointmentRemovedCommand { get; }
        private bool CanAppointmentAddedCommandExecute(object p) => true;
        private bool CanAppointmentEditedCommandExecute(object p) => true;
        private bool CanAppointmentRemovedCommandExecute(object p) => true;

        private void OnAppointmentAddedCommandExecuted(object p)
        {
            try
            {
                foreach (var i in Appointments)
                {
                    if (db.Entry(i).State == EntityState.Detached)
                    {
                        var emp = db.Employes.Where(f => f.Id == i.EmployeeId).FirstOrDefault();
                        var client = db.Clients.Where(f => f.Id == i.ClientInfoId).FirstOrDefault();
                        var serv = db.Services.Where(f => f.Id == i.ServiceId).FirstOrDefault();

                        db.Entry(i).State = EntityState.Added;
                    }
                }
                int cnt = db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAppointmentEditedCommandExecuted(object p)
        {
            try
            {
                /*if (p is DevExpress.Xpf.Scheduling.AppointmentEditedEventArgs appointment)
                {
                    foreach(var i in appointment.Appointments)
                    {
                        int x = 0;
                    }
                }*/
                int cnt = db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAppointmentRemovedCommandExecuted(object p)
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
        #endregion

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

        #region Справочник "Статусы в шедулере"
        public ICommand OpenWindowStatusCommand { get; }
        public ICommand CloseWindowStatusCommand { get; }

        public ICommand AddStatusCommand { get; }
        public ICommand DeleteStatusCommand { get; }
        public ICommand SaveStatusCommand { get; }
        private bool CanOpenWindowStatusExecute(object p) => true;
        private bool CanCloseWindowStatusExecute(object p) => true;
        private bool CanAddStatusExecute(object p) => true;
        private bool CanDeleteStatusExecute(object p) => true;
        private bool CanSaveStatusExecute(object p) => true;

        private void OnOpenWindowStatusExecuted(object p)
        {
            try
            {
                StatusWindow = new StatusAppointmentWindow();
                StatusWindow.DataContext = this;
                StatusWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCloseWindowStatusExecuted(object p) => StatusWindow.Close();

        private void OnAddStatusExecuted(object p)
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

        private void OnSaveStatusExecuted(object p)
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
                int cnt = db.SaveChanges();
                StatusAppointmentsBeforeChanges.Clear();
                StatusAppointments.ForEach(f => StatusAppointmentsBeforeChanges.Add((AppointmentStatus)f.Clone()));
                
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

        private void OnDeleteStatusExecuted(object p)
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
            get => statusAppointments;
            set => Set(ref statusAppointments, value);
        }
        private ObservableCollection<AppointmentStatus> statusAppointments;

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

        public ObservableCollection<Service> ClassificatorCategories { get; set; }
        public virtual ObservableCollection<Employee> Doctors { get; set; }

        public virtual ObservableCollection<Client> Clients { get; set; }
        public virtual ObservableCollection<Appointments> Appointments { get; set; }
        public ObservableCollection<ResourceEntity> Calendars { get; set; }

        public virtual List<object> SelectedDoctors { get; set; }
        
        private void CreateCalendars() => Calendars = db.Resources.ToObservableCollection();
    }
}
