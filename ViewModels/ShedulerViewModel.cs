using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dental.Models;
using Microsoft.EntityFrameworkCore;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using Dental.Views.WindowForms;
using System.Windows.Media;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm;
using DevExpress.Xpf.Scheduling;
using Dental.ViewModels.EmployeeDir;
using Dental.Services;
using Dental.Views.PatientCard;
using Dental.ViewModels.ClientDir;
using Dental.Models.Base;
using System.Windows.Data;
using Dental.Views.Settings;
using Dental.Reports;
using DevExpress.Xpf.Printing;
using Dental.Infrastructures.Extensions.Notifications;
using License;
using Dental.Views.About;

namespace Dental.ViewModels
{
    public class ShedulerViewModel : ViewModelBase
    {
        public readonly ApplicationContext db;
        public ShedulerViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Config = db.Config;
                PathToEmployeesDirectory = Config.PathToEmployeesDirectory;

                SetLocationAppointments();
                SetStatusCollection();
                ClassificatorCategories = db.Services.Where(f => f.IsHidden != true)?.OrderBy(f => f.Sort).ToObservableCollection();

                Appointments = db.Appointments.Include(f => f.Service).Include(f => f.Employee).Include(f => f.ClientInfo).Include(f => f.Location)
                    .Where(f => !string.IsNullOrEmpty(f.StartTime)).OrderBy(f => f.CreatedAt).ToObservableCollection();

                LoadEmployees(db);
                LoadClients(db);
                SetSelectedEmployees();
                SetWorkTime();

                NotificationEvents = db.NotificationEvents?.ToArray();
            }
            catch(Exception e)
            {
                Log.ErrorHandler(e, "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Расписание\"!", true);
            }
        }

        private NotificationEvent[] NotificationEvents { get; set; }

        public void SetLocationAppointments() => LocationAppointments = db.LocationAppointment.OrderBy(f => f.Name).ToObservableCollection();

        public bool CanAppointmentAdded(object p) => ((UserSession)Application.Current.Resources["UserSession"]).AppointmentEditable;
        public bool CanAppointmentEdited(object p) => ((UserSession)Application.Current.Resources["UserSession"]).AppointmentEditable;
        public bool CanAppointmentRemoved(object p) => ((UserSession)Application.Current.Resources["UserSession"]).AppointmentDeletable;

        public bool CanOpenWindowLocation() => ((UserSession)Application.Current.Resources["UserSession"]).ShedulerLocationEditable;
        public bool CanOpenWindowStatus() => ((UserSession)Application.Current.Resources["UserSession"]).ShedulerStatusEditable;
        public bool CanOpenWorkTime() => ((UserSession)Application.Current.Resources["UserSession"]).ShedulerWorkTimeEditable;
        public bool CanPrint(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PrintSheduler;

        [Command]
        public void AppointmentAdded(object p)
        {
            try
            {
                if (Status.Licensed && Status.HardwareID != Status.License_HardwareID)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                if (!Status.Licensed && (Status.Evaluation_Time_Current > Status.Evaluation_Time))
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }

                var item = Appointments.FirstOrDefault(f => f.Id == 0);
                if (item == null) return;

                db.Entry(item).State = EntityState.Added;
                db.SaveChanges();

                if (item?.Employee?.IsNotify == true)
                {
                    var eventName = NotificationEvents?.FirstOrDefault(f => f.EventName == "AppointmentAdd");
                    if (eventName == null || eventName.IsNotify != true) return;
                        AddNotification(
                            $"Вам назначена встреча с {item.PatientName ?? "Неизвестно"}, место встречи: {item.LocationName ?? "Неизвестно"} в {item.Date}. Дополнительная информация: {item.Description ?? "Неизвестно"};", eventName, item.Employee?.Telegram
                            );
                }
                    
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        private void AddNotification(string msg, NotificationEvent notificationEvent, string chatId)
        {
            TelegramNotificationsQueueService.AddToQueue(new TelegramNotification() 
            { 
                Msg = msg, 
                NotificationEvent = notificationEvent,
                NotificationEventId = notificationEvent?.Id,
                ChatId = chatId 
            }, db);
        }
        

        [Command]
        public void AppointmentEdited(object p)
        {
            try
            {
                if (p is AppointmentEditedEventArgs e && e.Appointments.Count > 0)
                {
                    var item = e.Appointments[0].SourceObject as Appointments; 
                    db.SaveChanges();
                    if (item?.Employee?.IsNotify == true)
                    {
                        var eventName = NotificationEvents?.FirstOrDefault(f => f.EventName == "AppointmentEdit");
                        if (eventName == null || eventName.IsNotify != true) return;

                        AddNotification($"Изменения по встрече с {item.PatientName ?? "Неизвестно"}, место встречи: {item.LocationName ?? "Неизвестно"} в {item.Date}. Дополнительная информация: {item.Description ?? "Неизвестно"};", eventName, item.Employee?.Telegram);
                    }                        
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void AppointmentRemoved(object p)
        {
            try
            {
                if (p is AppointmentRemovedEventArgs e && e.Appointments.Count > 0)
                {
                    var item = e.Appointments[0].SourceObject as Appointments;
                    if(item != null) db.Remove(item);
                    db.SaveChanges();
                    if (item?.Employee?.IsNotify == true)
                    {
                        var eventName = NotificationEvents?.FirstOrDefault(f => f.EventName == "AppointmentRemove");
                        if (eventName == null || eventName.IsNotify != true) return;

                        AddNotification($"Отменена встреча с {item.PatientName ?? "Неизвестно"}, место встречи: {item.LocationName ?? "Неизвестно"} в {item.Date}. Дополнительная информация: {item.Description ?? "Неизвестно"};", eventName, item.Employee?.Telegram);
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        #region Справочник "Места встреч"
        [Command]
        public void OpenWindowLocation()
        {
            try
            {
                db.LocationAppointment.ForEach(f => db.Entry(f).State = EntityState.Unchanged);

                Window wnd = Application.Current.Windows.OfType<Window>().Where(w => w.ToString() == LocationWindow?.ToString()).FirstOrDefault();
                if (wnd != null)
                {
                    wnd.Activate();
                    return;
                }
                LocationWindow = new LocationAppointmentWindow() { DataContext = this };
                LocationWindow.Show();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void CloseWindowLocation() => LocationWindow.Close();

        [Command]
        public void AddLocation()
        {
            try
            {
                LocationAppointments.Add(new LocationAppointment());
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void DeleteLocation(object p)
        {
            try
            {
                if (p is LocationAppointment model)
                {
                    if (model.Id != 0)
                    {
                        if (!new ConfirDeleteInCollection().run(0)) return;
                        db.Entry(model).State = EntityState.Deleted;
                    }
                    else db.Entry(model).State = EntityState.Detached;
                    if (db.SaveChanges() > 0)
                    {
                        new Notification() { Content = "Локация удалена из базы данных!" }.run();
                        SetLocationAppointments();
                    }
                    //LocationAppointments.Remove(LocationAppointments.Where(f => f.Guid == model.Guid).FirstOrDefault());
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
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
                if (db.SaveChanges() > 0)
                {
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                    SetLocationAppointments();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        public LocationAppointmentWindow LocationWindow { get; set; }

        public ObservableCollection<LocationAppointment> LocationAppointments
        {
            get { return GetProperty(() => LocationAppointments); }
            set { SetProperty(() => LocationAppointments, value); }
        }

        #endregion

        #region Справочник "Статусы в шедулере"       
        [Command]
        public void OpenWindowStatus()
        {
            try
            {

                db.AppointmentStatus.ForEach(f => db.Entry(f).State = EntityState.Unchanged);


                Window wnd = Application.Current.Windows.OfType<Window>().Where(w => w.ToString() == StatusWindow?.ToString()).FirstOrDefault();
                if (wnd != null)
                {
                    wnd.Activate();
                    return;
                }

                StatusWindow = new StatusAppointmentWindow() { DataContext = this };
                StatusWindow.Show();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void CloseWindowStatus() => StatusWindow?.Close();

        [Command]
        public void AddStatus(object p)
        {
            try
            {
                StatusAppointments.Add(new AppointmentStatus());
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
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
                    item.BrushColor = item.Brush?.Color.ToString();

                }

                if (db.SaveChanges() > 0) 
                { 
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                    SetStatusCollection();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void DeleteStatus(object p)
        {
            try
            {
                if (p is AppointmentStatus model)
                {
                    if (model.Id != 0)
                    {
                        if (!new ConfirDeleteInCollection().run(0)) return;
                        db.Entry(model).State = EntityState.Deleted;
                    }

                    else db.Entry(model).State = EntityState.Detached;
                    
                    if(db.SaveChanges() > 0)
                    {
                        new Notification() { Content = "Статус удален из базы данных!" }.run();
                        SetStatusCollection();
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
        #endregion

        [Command]
        public void Print(object p)
        {
            try
            {
                if (p is SchedulerControl scheduler)
                {
                    XtraSchedulerReport1 report = new XtraSchedulerReport1();
                    DateTimeRange dateTimeRange = scheduler.VisibleIntervals[0];
                    //TimeSpanRange timeSpanRange = scheduler.;
                    scheduler.SchedulerPrintAdapter.DateTimeRange = dateTimeRange;
                    scheduler.SelectedInterval = dateTimeRange;
                    
                    scheduler.SchedulerPrintAdapter.AssignToReport(report);
                    PrintHelper.ShowPrintPreview(scheduler, report);
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void OpenWorkTime() 
        {
            var vm = new WorkTimeVM(db);
            vm.SetWokTimeEvent += SetWorkTime;
            new WorkTimeWindow() { DataContext = vm }.Show(); 
        }

        public void SetWorkTime() => WorkTime = db.Branches.FirstOrDefault()?.WorkTime ?? WorkTimeVM.workTimeDefault;

        public string WorkTime
        {
            get { return GetProperty(() => WorkTime); }
            set { SetProperty(() => WorkTime, value); }
        }

        public StatusAppointmentWindow StatusWindow { get; set; }

        public ObservableCollection<AppointmentStatus> StatusAppointments
        {
            get { return GetProperty(() => StatusAppointments); }
            set { SetProperty(() => StatusAppointments, value); }
        }

        private void SetStatusCollection()
        {
            var collection = db.AppointmentStatus.OrderBy(f => f.Caption).ToObservableCollection();
            foreach (var i in collection)
            {
                try
                {
                    Color colorName;
                    if (i.BrushColor != null)
                    {
                        colorName = (Color)ColorConverter.ConvertFromString(i.BrushColor);
                        i.Brush = new SolidColorBrush(colorName);
                    }
                }
                catch(Exception e)
                {
                    Log.ErrorHandler(e);
                    i.Brush = null;
                }
            }

            StatusAppointments = collection;
        }
        

        public ObservableCollection<Service> ClassificatorCategories
        {
            get { return GetProperty(() => ClassificatorCategories); }
            set { SetProperty(() => ClassificatorCategories, value); }
        }

        public ObservableCollection<Employee> Doctors { get; set; }

        public virtual ObservableCollection<Client> Clients
        {
            get { return GetProperty(() => Clients); }
            set { SetProperty(() => Clients, value); }
        }

        public ObservableCollection<Appointments> Appointments { get; set; }

        public virtual List<object> SelectedDoctors
        {
            get { return GetProperty(() => SelectedDoctors); }
            set { SetProperty(() => SelectedDoctors, value); }
        }

        /*****************************************************************************************/


        private string PathToEmployeesDirectory;

        private void LoadEmployees(ApplicationContext db)
        {
            Doctors = db.Employes.Where(f => f.IsInSheduler != null && f.IsInSheduler > 0 && f.IsInArchive != true).OrderBy(d => d.LastName).ToObservableCollection();
            foreach (var i in Doctors)
            {
                if (Directory.Exists(PathToEmployeesDirectory))
                {
                    var file = Directory.GetFiles(PathToEmployeesDirectory)?.FirstOrDefault(f => f.Contains(i.Guid));
                    if (file == null)
                    {
                        i.Image = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/avatar.png"));
                        continue;
                    }

                    using (var stream = new FileStream(file, FileMode.Open))
                    {
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.StreamSource = stream;
                        img.EndInit();
                        img.Freeze();
                        i.Image = img;
                    }
                }
            }
        }

        private void LoadClients(ApplicationContext db) => Clients = db.Clients.Where(f => f.IsInArchive != true ).OrderBy(f => f.LastName).ToObservableCollection();

        private void SetSelectedEmployees()
        {
            SelectedDoctors = new List<object>();
            try
            {
                if (Application.Current.Resources["UserSession"] is UserSession userSession)
                {
                    if (userSession.Employee == null)
                    {
                        Doctors.ForEach(f => SelectedDoctors.Add(f));
                        return;
                    }
                    SelectedDoctors.Add(Doctors.FirstOrDefault(f => f.Id == userSession.Employee?.Id));
                    return;
                }
                Doctors.ForEach(f => SelectedDoctors.Add(f));
            }
            catch
            {
                Doctors.ForEach(f => SelectedDoctors.Add(f));
            }
        }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }
    }
}
