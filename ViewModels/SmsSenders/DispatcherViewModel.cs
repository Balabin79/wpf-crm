using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;


namespace B6CRM.ViewModels.SmsSenders
{
    public class DispatcherViewModel : ViewModelBase
    {
        public DispatcherViewModel() => Init();


        [Command]
        public void Init() //первая загрузка
        {
            UserControlName = "ProstoSmsControl";
            IsReadOnly = false;
            //без параметров, значит по-умолчанию
            SetUserControl();
        }

        [Command]
        public void Load(object p)
        {
            //вызывается при переходе по кнопкам 
            SetUserControl(p?.ToString() ?? "ProstoSmsControl");
        }


        private void SetUserControl(string userControlName = "ProstoSmsControl")
        {
            //включаем видимость контрола и подчеркиваем кнопку активного контрола
            UserControlName = userControlName;
            switch (userControlName)
            {
                case "ProstoSmsControl": Context = new ProstoSmsViewModel(); break;
                //case "SmsCenterControl": Context = new SmsCenterViewModel(); break;
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

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }        
        
    }
}
