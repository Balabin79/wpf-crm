using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm.POCO;

namespace Dental.Models
{
    public class Doctor
    {
        public static Doctor Create()
        {
            return ViewModelSource.Create(() => new Doctor());
        }
        public static Doctor Create(int Id, string Name)
        {
            Doctor doctor = Doctor.Create();
            doctor.Id = Id;
            doctor.Name = Name;
            return doctor;
        }

        protected Doctor() { }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}
