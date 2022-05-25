using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using DevExpress.Mvvm.DataAnnotations;

namespace Dental.ViewModels
{
    class OrgViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;

        public OrgViewModel()
        {
            db = new ApplicationContext();

        }
    }
}
