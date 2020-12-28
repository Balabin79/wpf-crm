
using Dental.Models;
using System.Collections.ObjectModel;


namespace Dental.Repositories
{
    static class SpecialityRepository
    {
        public static ObservableCollection<Speciality> GetFakeSpecialities()
        {
            return new ObservableCollection<Speciality>
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
