using Dental.Models;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.ViewModels.Statistics
{
    public class EmployeesWorkloadViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public EmployeesWorkloadViewModel()
        {
            db = new ApplicationContext();

            Data =        
                 new ObservableCollection<object> {
                    new  { Period = "Asia", Sum = (decimal?)5.28},
                    new { Period = "Australia", Sum = (decimal?)2.27},
                    new  { Period = "Europe", Sum = (decimal?)3.72},
                    new  { Period = "North America", Sum = (decimal?)4.18},
                    new  { Period = "South America", Sum = (decimal?)2.11}
                   };
            
        }

        public ObservableCollection<object> Data { get; private set; }

    }
}
