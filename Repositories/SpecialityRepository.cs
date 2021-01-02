
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
                    new Speciality() {Name="Терапевт", Description="", ShowInShedule=true},
                    new Speciality() {Name="Ортодонт", Description="", ShowInShedule=true},
                    new Speciality() {Name="Хирург", Description="", ShowInShedule=true},
                    new Speciality() {Name="Протезист", Description="", ShowInShedule=true},
                    new Speciality() {Name="Рентгенолог", Description="", ShowInShedule=true},
                    new Speciality() {Name="Анестезиолог", Description="", ShowInShedule=false }
                };
        }
    }
}
