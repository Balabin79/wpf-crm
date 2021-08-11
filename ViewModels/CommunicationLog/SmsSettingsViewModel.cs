using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Input;
using Dental.Enums;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Views.Nomenclatures.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using System.Windows.Media.Imaging;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Models.Base;
using Dental.Interfaces;
using Dental.Models.Template;
using Dental.Views.Nomenclatures;

namespace Dental.ViewModels.CommunicationLog
{
    class SmsSettingsViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public SmsSettingsViewModel()
        {
        }     

    }
}
