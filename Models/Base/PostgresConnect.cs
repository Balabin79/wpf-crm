﻿using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    public class PostgresConnect : ViewModelBase, IDataErrorInfo
    {
        public string Server
        {
            get { return GetProperty(() => Server); }
            set { SetProperty(() => Server, value?.Trim()); }
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

        public string UserName
        {
            get { return GetProperty(() => UserName); }
            set { SetProperty(() => UserName, value?.Trim()); }
        }

        public string Password
        {
            get { return GetProperty(() => Password); }
            set { SetProperty(() => Password, value?.Trim()); }
        }

    public string Error { get => string.Empty; }
    public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
}
}
