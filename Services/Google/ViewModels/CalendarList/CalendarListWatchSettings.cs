namespace Dental.Services.Google.ViewModels.CalendarList
{
    public class CalendarListWatchSettings : BaseViewModel
    {
        public string Id
        {
            get { return GetProperty(() => Id); }
            set { SetProperty(() => Id, value); }
        }

        public string Token
        {
            get { return GetProperty(() => Token); }
            set { SetProperty(() => Token, value); }
        }

        public string Type
        {
            get { return GetProperty(() => Type); }
            set { SetProperty(() => Type, value); }
        }

        public string Address
        {
            get { return GetProperty(() => Address); }
            set { SetProperty(() => Address, value); }
        }

        public object Params
        {
            get { return GetProperty(() => Params); }
            set { SetProperty(() => Params, value); }
        }

        public string Ttl
        {
            get { return GetProperty(() => Ttl); }
            set { SetProperty(() => Ttl, value); }
        }
    }
}
