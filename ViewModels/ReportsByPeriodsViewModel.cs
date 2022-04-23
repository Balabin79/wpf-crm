using System;
using System.Collections.Generic;
using System.Linq;
using Dental.Models;
using System.Windows;
using DevExpress.Xpf.Core;

using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using Dental.Services;
using Dental.Infrastructures.Extensions.Notifications;
using System.IO;
using System.Windows.Media.Imaging;
using Dental.Views.WindowForms;

namespace Dental.ViewModels
{
    class ReportsByPeriodsViewModel : ViewModelBase
    {

        public ReportsByPeriodsViewModel()
        {
            try
            {
                db = new ApplicationContext();

            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Действия пользователя\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

     

        ApplicationContext db;

        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public int PeriodType { get; set; }
    }


}