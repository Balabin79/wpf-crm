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
using System.IO;
using System.Windows.Media.Imaging;
using Dental.Views.WindowForms;
using System.Windows.Media;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm;
using DevExpress.Xpf.Scheduling;
using Dental.Views.EmployeeDir;
using Dental.ViewModels.EmployeeDir;
using Dental.Services;
using Dental.Views.PatientCard;
using Dental.ViewModels.ClientDir;
using Dental.Models.Base;
using System.Windows.Data;


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

                Appointments = db.Appointments.Include(f => f.Service).Include(f => f.Employee).Include(f => f.ClientInfo).Include(f => f.Location)
                    .Where(f => !string.IsNullOrEmpty(f.StartTime)).OrderBy(f => f.CreatedAt).ToObservableCollection();

                LoadEmployees(db);
                LoadClients(db);
                SetSelectedEmployees();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Расписание\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanAppointmentAdded(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SheduleRead;
        public bool CanAppointmentEdited(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SheduleRead;
        public bool CanAppointmentRemoved(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SheduleRead;

        public bool CanOpenWindowLocation() => ((UserSession)Application.Current.Resources["UserSession"]).SheduleLocationEditable;
        public bool CanCloseWindowLocation() => ((UserSession)Application.Current.Resources["UserSession"]).SheduleLocationEditable;
        public bool CanAddLocation() => ((UserSession)Application.Current.Resources["UserSession"]).SheduleLocationEditable;
        public bool CanDeleteLocation(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SheduleLocationDeletable;
        public bool CanSaveLocation() => ((UserSession)Application.Current.Resources["UserSession"]).SheduleLocationEditable;

        public bool CanOpenWindowStatus() => ((UserSession)Application.Current.Resources["UserSession"]).SheduleStatusEditable;
        public bool CanCloseWindowStatus() => ((UserSession)Application.Current.Resources["UserSession"]).SheduleStatusEditable;
        public bool CanAddStatus(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SheduleStatusEditable;
        public bool CanSaveStatus() => ((UserSession)Application.Current.Resources["UserSession"]).SheduleStatusEditable;
        public bool CanDeleteStatus(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SheduleStatusDeletable;

        public bool CanOpenFormEmployeeCard(object p) => ((UserSession)Application.Current.Resources["UserSession"]).OpenEmployeeCard;
        public bool CanOpenFormClientCard(object p) => ((UserSession)Application.Current.Resources["UserSession"]).OpenClientCard;

        [Command]
        public void AppointmentAdded(object p)
        {
            try
            {
                var item = Appointments.FirstOrDefault(f => f.Id == 0);
                if (item == null) return;
                db.Entry(item).State = EntityState.Added;
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
        }

        [Command]
        public void AppointmentEdited(object p)
        {
            try
            {
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
                Services.Reestr.Update((int)Tables.Appointments);
                db.SaveChanges();
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
                (new ViewModelLog(e)).run();
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
                        Services.Reestr.Update((int)Tables.LocationAppointment);
                    }

                    else db.Entry(model).State = EntityState.Detached;
                    LocationAppointments.Remove(LocationAppointments.Where(f => f.Guid == model.Guid).FirstOrDefault());                 
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
                if (db.SaveChanges() > 0) 
                { 
                    new Infrastructures.Extensions.Notifications.Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                    Services.Reestr.Update((int)Tables.LocationAppointment);
                }              
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

        private ObservableCollection<LocationAppointment>  GetLocationCollection() => db.LocationAppointment.OrderBy(f => f.Name).ToObservableCollection();
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
                    item.BrushColor = item.Brush?.Color.ToString();
                    
                }

                if (db.SaveChanges() > 0) 
                { 
                    new Infrastructures.Extensions.Notifications.Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                    Services.Reestr.Update((int)Tables.AppointmentsStatuses);
                }
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
                        Services.Reestr.Update((int)Tables.AppointmentsStatuses);
                    }

                    else db.Entry(model).State = EntityState.Detached;
                    StatusAppointments.Remove(StatusAppointments.Where(f => f.Guid == model.Guid).FirstOrDefault());
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

        private ObservableCollection<AppointmentStatus> GetStatusCollection()
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
                catch
                {
                    i.Brush = null;
                }
            }

            return collection;
        }
        #endregion


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
       

        private string PathToEmployeesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Employees");

        [Command]
        public void OpenFormClientCard(object p)
        {
            try
            {
                if (p == null) return;
                var client = db.Clients.FirstOrDefault(f => f.Id == (int)p);
                var win = new ClientCardWindow(client.Id);
                win.ShowDialog();

                if (win.DataContext is ClientCardViewModel vm)
                {
                    if (client.FirstName != vm.Model.FirstName ||
                        client.LastName != vm.Model.LastName ||
                        client.MiddleName != vm.Model.MiddleName)
                    {
                        if (Application.Current.Resources["Router"] is Navigator nav) { nav.LeftMenuClick("Dental.Views.Sheduler"); }
                    }
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Карта сотрудника\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        private void LoadEmployees(ApplicationContext db)
        {
            Doctors = db.Employes.Where(f => f.IsInSheduler != null && f.IsInSheduler > 0).OrderBy(d => d.LastName).ToObservableCollection();
            foreach (var i in Doctors)
            {
                

                if (!string.IsNullOrEmpty(i.Photo) )
                {
                    string pathToEmpPhoto = Path.Combine(PathToEmployeesDirectory, i?.Photo);
                    if (File.Exists(pathToEmpPhoto))
                    {
                        using (var stream = new FileStream(pathToEmpPhoto, FileMode.Open))
                        {
                            var img = new BitmapImage();
                            img.BeginInit();
                            img.CacheOption = BitmapCacheOption.OnLoad;
                            img.StreamSource = stream;
                            img.EndInit();
                            img.Freeze();
                            i.Image = img;
                            stream.Close();
                            stream.Dispose();
                        }
                    }
                    else i.Image = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/avatar.png"));
                }           
            }
        }

        private void LoadClients(ApplicationContext db) => Clients = db.Clients.OrderBy(f => f.LastName).ToObservableCollection();
        
        private void SetSelectedEmployees()
        {
            var userSession = (UserSession)Application.Current.Resources["UserSession"];
            SelectedDoctors = new List<object>();
            try 
            {
                if (userSession?.Employee != null && userSession?.Employee?.IsDoctor == 1)
                {
                    SelectedDoctors.Add(Doctors.FirstOrDefault(f => f.Id == userSession.Employee?.Id));
                }
                else Doctors.ForEach(f => SelectedDoctors.Add(f));
            } catch 
            {
                Doctors.ForEach(f => SelectedDoctors.Add(f));
            }
        }    
    }
}
