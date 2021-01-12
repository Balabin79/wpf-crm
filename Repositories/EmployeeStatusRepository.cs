using Dental.Interfaces;
using Dental.Models;
using System.Collections.ObjectModel;


namespace Dental.Repositories
{
    class EmployeeStatusRepository
    {
        public static ObservableCollection<EmployeeStatus> GetFakeEmployeeStatuses()
        {
            return new ObservableCollection<EmployeeStatus>
                {
                    new EmployeeStatus() {Name="Работает", Description=""},
                    new EmployeeStatus() {Name="Уволен", Description=""},
                    new EmployeeStatus() {Name="В отпуске", Description=""}
                };
        }
        
    }
}
