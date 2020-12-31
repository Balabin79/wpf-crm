using Dental.Models;
using System;
using System.Collections.ObjectModel;

namespace Dental.ViewModels
{
    class SheduleViewModel
    {

        public virtual ObservableCollection<Doctor> Doctors { get; set; }
        public virtual ObservableCollection<MedicalAppointment> Appointments { get; set; }

        protected SheduleViewModel()
        {
            GetDoctors();
            GetMedicalAppointments();
        }

        private void GetDoctors()
        {
            Doctors = new ObservableCollection<Doctor>();
            Doctors.Add(Doctor.Create(Id: "doctor_1", Name: "Stomatologist"));
            Doctors.Add(Doctor.Create(Id: "doctor_2", Name: "Ophthalmologist"));
            Doctors.Add(Doctor.Create(Id: "doctor_3", Name: "Surgeon"));
        }
            
        private void GetMedicalAppointments()           
        {
            Appointments = new ObservableCollection<MedicalAppointment>();
            Appointments.Add(MedicalAppointment.Create(
                startTime: DateTime.Now.Date.AddHours(10), endTime: DateTime.Now.Date.AddHours(11),
                doctorId: 1, notes: "", location: "101", categoryId: 1, patientName: "Dave Muriel",
                insuranceNumber: "396-36-XXXX", firstVisit: true));
        }
        
    }
}
