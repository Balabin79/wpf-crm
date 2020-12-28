using System.Collections.Generic;

namespace Dental.Models
{
    public class Speciality
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }


        public List<Speciality> FakeListSpecialistes
        {
            get
            {
                return new List<Speciality>
                {
                    new Speciality() {Name="Терапевт", Description="", IsActive=true},
                    new Speciality() {Name="Ортодонт", Description="", IsActive=true},
                    new Speciality() {Name="Хирург", Description="", IsActive=true},
                    new Speciality() {Name="Протезист", Description="", IsActive=true},
                    new Speciality() {Name="Рентгенолог", Description="", IsActive=true},
                    new Speciality() {Name="Анестезиолог", Description="", IsActive=true}
                };
            }

        }
    }
}
