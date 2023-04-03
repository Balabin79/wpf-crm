using B6CRM.Models.Base;
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
using Microsoft.EntityFrameworkCore;
using DevExpress.Mvvm.Native;
using System.Windows.Data;
using B6CRM.Models;
using B6CRM.Infrastructures.Converters;
using B6CRM.Services;

namespace B6CRM.ViewModels
{
    public class ClientAppointmentWindowViewModel : AppointmentWindowViewModel
    {
        private readonly ApplicationContext db;
        public static ClientAppointmentWindowViewModel Create(
            AppointmentItem appointmentItem,
            SchedulerControl scheduler,
            ShedulerViewModel vm
            )
        {
            return ViewModelSource.Create(() => new ClientAppointmentWindowViewModel(appointmentItem, scheduler, vm));
        }

        protected ClientAppointmentWindowViewModel(
            AppointmentItem appointmentItem,
            SchedulerControl scheduler,
            ShedulerViewModel vm
            ) : base(appointmentItem, scheduler)
        {
            Patients = vm.Clients;
            Services = vm.ClassificatorCategories;
            Locations = vm.LocationAppointments;
            db = vm.db;

            Patient = vm.Clients?.FirstOrDefault(x => x.Id.Equals(CustomFields["ClientInfoId"]));
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
                        Appointment.Description = classificator.FullName;
                    }
                    parameters.Popup.ClosePopup();

                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }


        public ObservableCollection<Client> Patients
        {
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
                CustomFields["ClientInfoId"] = newPatient?.Id;
                CustomFields["Client"] = newPatient;
                Subject = newPatient?.ToString();
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
                Description = newService?.FullName;
                CustomFields["ServiceId"] = newService?.Id;
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
                CustomFields["LocationId"] = newLocation?.Id;
                Location = newLocation?.Name;
                CustomFields["LocationId"] = newLocation?.Id;
            }
        }

        /**** File *****/
        protected IOpenFileDialogService OpenFileDialogService { get { return GetService<IOpenFileDialogService>(); } }

    }
}
