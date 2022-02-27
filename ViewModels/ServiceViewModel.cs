using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;

namespace Dental.ViewModels
{
    class ServiceViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public ServiceViewModel()
        {
            ExpandAllCommand = new LambdaCommand(OnExpandAllCommandExecuted, CanExpandAllCommandExecute);

            try
            {
                db = new ApplicationContext();
                Collection = db.Appointments
                    .Include(f => f.Service)
                    .Include(f => f.Employee)
                    .Include(f => f.ClientInfo)
                    .Include(f => f.Location)
                    .Where(f => !string.IsNullOrEmpty(f.StartTime))
                    .OrderBy(f => f.CreatedAt)
                    .ToArray();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Услуги\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand ExpandAllCommand { get; }
        private bool CanExpandAllCommandExecute(object p) => true;
        private void OnExpandAllCommandExecuted(object p)
        {
            try
            {
                if (p is DevExpress.Xpf.Grid.CardView card)
                {
                    if (card.IsCardExpanded(0)) card.CollapseAllCards();
                    else card.ExpandAllCards();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public ICollection<Appointments> Collection { get; set; }
    }
}
