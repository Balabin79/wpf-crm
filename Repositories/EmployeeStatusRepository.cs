using System.Collections.ObjectModel;
using Dental.Models;


namespace Dental.Repositories
{
    static class EmployeeStatusRepository
    {
        public static ObservableCollection<EmployeeStatus> GetFakeEmployeeStatuses()
        {
            return new ObservableCollection<EmployeeStatus>
                {
                    new EmployeeStatus() {Name="Работает", Description="", IsActive=true},
                    new EmployeeStatus() {Name="Уволен", Description="", IsActive=true},
                    new EmployeeStatus() {Name="В отпуске", Description="", IsActive=true}
                };
        }
    }
}
