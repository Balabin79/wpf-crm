using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;


namespace B6CRM.ViewModels.Statistics
{
    public class StatisticViewModel : ViewModelBase
    {
        public StatisticViewModel() 
        {
            Context = new LineViewModel();
            UserControlName = "ProfitControl";
        }      

        [Command]     // Общий доход
        public void Profit() => SetUserControl("ProfitControl");

        [Command]     // Доход по сотрудникам
        public void ProfitByEmployees() => SetUserControl("ProfitByEmployeesControl");

        [Command]     // По рекламным источникам
        public void Advertisings() => SetUserControl("AdvertisingsControl");

        private void SetUserControl(string userControlName = "ProfitControl")
        {
            //включаем видимость контрола и подчеркиваем кнопку активного контрола
            switch (userControlName)
            {
                case "ProfitControl":
                    Context = new LineViewModel();
                    UserControlName = userControlName;
                    break;
                case "ProfitByEmployeesControl":
                    Context = new ProfitByEmployeesViewModel();
                    UserControlName = userControlName;
                    break;
                case "AdvertisingsControl":
                    Context = new AdvertisingSourcesViewModel();
                    UserControlName = userControlName;
                    break;
            }
        }

        public string UserControlName
        {
            get { return GetProperty(() => UserControlName); }
            set { SetProperty(() => UserControlName, value); }
        }

        public object Context
        {
            get { return GetProperty(() => Context); }
            set { SetProperty(() => Context, value); }
        }
    }
}
