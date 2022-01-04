﻿using Dental.Infrastructures.Converters;
using Dental.Infrastructures.Logs;
using Dental.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Scheduling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Dental.ViewModels
{
    public class ClientAppointmentWindowViewModel : AppointmentWindowViewModel
    {
        public static ClientAppointmentWindowViewModel Create(
            AppointmentItem appointmentItem, 
            SchedulerControl scheduler, 
            ObservableCollection<PatientInfo> clients, 
            ObservableCollection<Classificator> services,
            ObservableCollection<LocationAppointment> locations
            )
        {
            return ViewModelSource.Create(() => new ClientAppointmentWindowViewModel(
                appointmentItem, 
                scheduler, 
                clients, 
                services,
                locations
                ));
        }
        
        protected ClientAppointmentWindowViewModel(
            AppointmentItem appointmentItem, 
            SchedulerControl scheduler, 
            ObservableCollection<PatientInfo> clients, 
            ObservableCollection<Classificator> services,
            ObservableCollection<LocationAppointment> locations
            ) : base(appointmentItem, scheduler)
        {
            Patients = clients;
            Services = services;
            Locations = locations;

            Patient = clients?.FirstOrDefault(x => x.Id.Equals(CustomFields["ClientInfoId"])); 
                //?? clients?.FirstOrDefault(x => x.Id.Equals(appointmentItem?.ResourceId));

            Service = services?.FirstOrDefault(x => x.Id.Equals(CustomFields["ServiceId"]));
            LocationAppointment = locations?.FirstOrDefault(x => x.Id.Equals(CustomFields["LocationId"]));

            SelectPosInClassificatorCommand = new DelegateCommand<object>(
                OnSelectPosInClassificatorCommandExecuted, 
                CanSelectPosInClassificatorCommandExecute
                );
        }

        public DelegateCommand<object> SelectPosInClassificatorCommand { get; private set; }
        private bool CanSelectPosInClassificatorCommandExecute(object p) => true;
        private void OnSelectPosInClassificatorCommandExecuted(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is Classificator classificator)
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
                (new ViewModelLog(e)).run();
            }
        }



        public ObservableCollection<PatientInfo> Patients {
            get => patients;
            set => patients = value;
        }
        private ObservableCollection<PatientInfo> patients;

        PatientInfo patient;
        [BindableProperty]
        public virtual PatientInfo Patient
        {
            get { return patient; }
            set
            {
                PatientInfo newPatient = value;
                if (patient == newPatient)
                    return;
                patient = newPatient;
                CustomFields["PatientId"] = newPatient.Id;
                Subject = newPatient.FullName;
            }
        }

        public ObservableCollection<Classificator> Services
        {
            get => services;
            set => services = value;
        }
        private ObservableCollection<Classificator> services;

        Classificator service;
        [BindableProperty]
        public virtual Classificator Service
        {
            get { return service; }
            set
            {
                Classificator newService = value;
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
                if (location == newLocation)
                    return;
                location = newLocation;
                CustomFields["LocationId"] = newLocation.Id;
                Location = newLocation.Name;
                CustomFields["LocationId"] = newLocation.Id;
            }
        }

    }
}
