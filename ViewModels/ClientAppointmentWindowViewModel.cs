using Dental.Models;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Scheduling;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dental.ViewModels
{
    public class ClientAppointmentWindowViewModel : AppointmentWindowViewModel
    {
        public static ClientAppointmentWindowViewModel Create(AppointmentItem appointmentItem, SchedulerControl scheduler, ObservableCollection<PatientInfo> clients)
        {
            return ViewModelSource.Create(() => new ClientAppointmentWindowViewModel(appointmentItem, scheduler, clients));
        }
        
        protected ClientAppointmentWindowViewModel(AppointmentItem appointmentItem, SchedulerControl scheduler, ObservableCollection<PatientInfo> clients) : base(appointmentItem, scheduler)
        {
            Patients = clients;
            Patient = clients.FirstOrDefault(x => x.Id.Equals(CustomFields["PatientId"])); ;
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
    }
}
