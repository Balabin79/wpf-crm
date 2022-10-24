using Dental.Infrastructures.Converters;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Models.Base;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduling;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using System.Windows.Data;

namespace Dental.ViewModels
{
    public class ClientAppointmentWindowViewModel : AppointmentWindowViewModel
    {
        public static ClientAppointmentWindowViewModel Create(AppointmentItem appointmentItem, SchedulerControl scheduler, ObservableCollection<Client> clients, ObservableCollection<Service> services, ObservableCollection<LocationAppointment> locations
            )
        {
            return ViewModelSource.Create(() => new ClientAppointmentWindowViewModel(appointmentItem, scheduler, clients, services, locations));
        }
        
        protected ClientAppointmentWindowViewModel(AppointmentItem appointmentItem, SchedulerControl scheduler,  ObservableCollection<Client> clients, 
            ObservableCollection<Service> services,ObservableCollection<LocationAppointment> locations) : base(appointmentItem, scheduler)
        {
            Patients = clients;
            Services = services;
            Locations = locations;
            AppointmentItem = appointmentItem;
            //AttachmentFile = appointmentItem.CustomFields["AttachmentFile"]?.ToString();
            try
            {
                if (((int)AppointmentItem?.Id) > 0)
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        var appointment = db.Appointments.FirstOrDefault(f => f.Id == (int)AppointmentItem.Id);
                        if (appointment != null)
                        {
                            var path = Path.Combine(PathToAppointmentsDirectory, appointment?.Guid);
                            if (Directory.Exists(path))
                            {
                                var files = Directory.GetFiles(path);
                                if (files.Length > 0)
                                {
                                    var name = new FileInfo(files[0]).Name;
                                    AttachmentFile = files[0];
                                    AttachmentFileName = name;
                                    CustomFields["AttachmentFile"] = files[0];
                                    CustomFields["AttachmentFileName"] = name;
                                }
                            }
                        }

                    }
                }
            }
            catch
            {
                AttachmentFile = null;
                AttachmentFileName = null;
            }

            Patient = clients?.FirstOrDefault(x => x.Id.Equals(CustomFields["ClientInfoId"])); 
            if (CustomFields["Client"] is Client client)
            {
                Patient = client;
            }

            Service = services?.FirstOrDefault(x => x.Id.Equals(CustomFields["ServiceId"]));
            LocationAppointment = locations?.FirstOrDefault(x => x.Id.Equals(CustomFields["LocationId"]));

            SelectPosInClassificatorCommand = new DelegateCommand<object>(OnSelectPosInClassificatorCommandExecuted, CanSelectPosInClassificatorCommandExecute);
        }

        public DelegateCommand<object> SelectPosInClassificatorCommand { get; private set; }
        private bool CanSelectPosInClassificatorCommandExecute(object p) => true;
        private void OnSelectPosInClassificatorCommandExecuted(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.CurrentItem is Service classificator)
                    {
                        if (classificator.IsDir == 1) return;
                        parameters.Popup.EditValue = classificator;
                        this.Appointment.Description = classificator.FullName;
                    }
                    parameters.Popup.ClosePopup();

                }
            }
            catch (Exception e)
            {
                new ViewModelLog(e).run();
            }
        }

        public bool CanUploadFile() => true;
        public bool CanClearFile() => true;
       
        public ObservableCollection<Client> Patients {
            get => patients;
            set => patients = value;
        }
        private ObservableCollection<Client> patients;

        Client patient;
        [BindableProperty]
        public virtual Client Patient
        {
            get { return patient; }
            set
            {
                Client newPatient = value;
                if (patient == newPatient) return;
                patient = newPatient;
                CustomFields["ClientInfoId"] = newPatient.Id;
                CustomFields["Client"] = newPatient;
                Subject = newPatient.ToString();
            }
        }

        public ObservableCollection<Service> Services
        {
            get => services;
            set => services = value;
        }
        private ObservableCollection<Service> services;

        Service service;
        [BindableProperty]
        public virtual Service Service
        {
            get { return service; }
            set
            {
                Service newService = value;
                if (service == newService) return;
                service = newService;
                Description = newService.FullName;
                CustomFields["ServiceId"] = newService.Id;
            }
        }

        public ObservableCollection<LocationAppointment> Locations
        {
            get => locations;
            set => locations = value;
        }
        private ObservableCollection<LocationAppointment> locations;

        LocationAppointment location;
        [BindableProperty]
        public virtual LocationAppointment LocationAppointment
        {
            get { return location; }
            set
            {
                LocationAppointment newLocation = value;
                if (location == newLocation) return;
                location = newLocation;
                CustomFields["LocationId"] = newLocation.Id;
                Location = newLocation.Name;
                CustomFields["LocationId"] = newLocation.Id;
            }
        }

        /**** File *****/
        protected IOpenFileDialogService OpenFileDialogService { get { return this.GetService<IOpenFileDialogService>(); } }

        private AppointmentItem AppointmentItem { get; set; }

        public string AttachmentFile
        {
            get { return GetProperty(() => AttachmentFile); }
            set { 
                SetProperty(() => AttachmentFile, value);
                CustomFields["AttachmentFile"] = value;
            }
        }

        private string PathToAppointmentsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Appointments");

        public string AttachmentFileName
        {
            get { return GetProperty(() => AttachmentFileName); }
            set { 
                SetProperty(() => AttachmentFileName, value);
                CustomFields["AttachmentFileName"] = value;
            }
        }

        [Command]
        public void UploadFile()
        {
            if (OpenFileDialogService.ShowDialog())
            {
                AttachmentFile = OpenFileDialogService.File.GetFullName();
                AttachmentFileName = OpenFileDialogService.File?.Name;
            }
        }

        [Command]
        public void OpenFile(object p)
        {
            try
            {
                var fileName = p.ToString();
                if (fileName?.Length < 2 || !File.Exists(fileName)) return; 
                Process.Start(fileName);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                   text: "Невозможно выполнить загрузку файла!",
                   messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void ClearFile() 
        { 
            AttachmentFile = null; 
            AttachmentFileName = null; 
        }
        

    }
}
