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


        [Command]
        public void GenerateInvoice(object p)
        {
            try
            {
                if (Service == null || Patient == null)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Поля услуга и клиент должны быть заполнены!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    return;
                }

                Employee employee = null;
                if (Resource != null) employee = Resource.SourceObject as Employee;


                    var item = new InvoiceItems
                    {
                        Count = 1,
                        Price = Service?.Price,
                        Name = Service?.Name,
                        Code = Service?.Code,
                    };


                    var invoice = new Invoice()
                    {
                        Client = Patient,
                        ClientId = Patient?.Id,
                        Employee = employee,
                        EmployeeId = employee?.Id,
                        DateTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds(),
                        Date = DateTime.Now.ToString(),
                        Number = int.TryParse(db.Invoices?.ToList()?.OrderByDescending(f => f.Id)?.FirstOrDefault()?.Number, out int result) ? string.Format("{0:00000000}", ++result) : "00000001",
                        Name = "Счет"
                    };
                    invoice.InvoiceItems.Add(item);

                    db.Entry(invoice).State = EntityState.Added;
                    if(db.SaveChanges() > 0)
                    {
                        ThemedMessageBox.Show(title: "Внимание", text: "Счет сформирован!",
                            messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Information);
                        return;
                    }
                

            }
            catch (Exception e)
            {
                new ViewModelLog(e).run();
            }
        }


        /**** File *****/
        protected IOpenFileDialogService OpenFileDialogService { get { return this.GetService<IOpenFileDialogService>(); } }

    }
}
