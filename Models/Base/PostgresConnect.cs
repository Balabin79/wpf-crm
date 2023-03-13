using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    [Serializable]
    public class PostgresConnect : ViewModelBase
    {
        public string Host
        {
            get { return GetProperty(() => Host); }
            set { SetProperty(() => Host, value?.Trim()); }
        }

        public int Port
        {
            get { return GetProperty(() => Port); }
            set { SetProperty(() => Port, value); }
        }

        public string Database
        {
            get { return GetProperty(() => Database); }
            set { SetProperty(() => Database, value?.Trim()); }
        }

        public string Username
        {
            get { return GetProperty(() => Username); }
            set { SetProperty(() => Username, value?.Trim()); }
        }

        public string Password
        {
            get { return GetProperty(() => Password); }
            set { SetProperty(() => Password, value?.Trim()); }
        }
    }
}
