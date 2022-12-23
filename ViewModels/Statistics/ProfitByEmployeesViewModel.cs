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
    public class ProfitByEmployeesViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public ProfitByEmployeesViewModel()
        {
            db = new ApplicationContext();

            Data =        
                 new ObservableCollection<object> {
                    new  { Period = "Антон М.П.", Sum = (decimal?)715000.00},
                    new { Period = "Иванов И.А.", Sum = (decimal?)602000.00},
                    new  { Period = "Светлакова А.С.", Sum = (decimal?)631000.00},
                    new  { Period = "Светлицын И.И.", Sum = (decimal?)690000.00},
                    new  { Period = "Семушкина И.А.", Sum = (decimal?)210000.00},
                    new  { Period = "Федотова И.В.", Sum = (decimal?)426000.00},
                    new  { Period = "Фомин П.А.", Sum = (decimal?)47000.00}
                   };
            
        }

        public ObservableCollection<object> Data { get; private set; }

    }
}
