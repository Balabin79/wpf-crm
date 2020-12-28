using Dental.Models.Share;
using DevExpress.Mvvm.POCO;
using System;
using System.Linq;

namespace Dental.Models
{
    class Doctor
    {
        public static Doctor Create()
        {
            return ViewModelSource.Create(() => new Doctor());
        }

        public static Doctor Create(string Id, string Name)
        {
            Doctor doctor = Doctor.Create();
            doctor.Id = Id;
            doctor.Name = Name;
            return doctor;
        }

        protected Doctor() { }

        public virtual string Id { get; set; }
        public virtual string Type { get; set; }
        public Employee Employee { get; set; }



        public virtual string Name { get; set; }
    }
}
