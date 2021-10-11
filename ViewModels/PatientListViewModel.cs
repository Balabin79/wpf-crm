using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Views.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Services;

namespace Dental.ViewModels
{
    class PatientListViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public PatientListViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Collection = db.PatientInfo.OrderBy(f => f.LastName).ToObservableCollection();

                #region инициализация команд, связанных с общим функционалом карты пациента
                OpenPatientCardCommand = new LambdaCommand(OnOpenPatientCardCommandExecuted, CanOpenPatientCardCommandExecute);
                MoveToArchiveCommand = new LambdaCommand(OnMoveToArchiveCommandExecuted, CanMoveToArchiveCommandExecute);
                #endregion
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Список пациентов\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand OpenPatientCardCommand { get; }
        private bool CanOpenPatientCardCommandExecute(object p) => true;
        private void OnOpenPatientCardCommandExecuted(object p)
        {
            if (p == null) return;
            var nav = Navigation.Instance;
            int.TryParse(p.ToString(), out int param);
            if (param == -1 || param == 0) nav.LeftMenuClick.Execute("Dental.Views.PatientCard.MainInfoPage");  
            else nav.LeftMenuClick.Execute(new object[] { "Dental.Views.PatientCard.MainInfoPage", param });
        }

        public ICommand MoveToArchiveCommand { get; }
        private bool CanMoveToArchiveCommandExecute(object p) => true;
        private void OnMoveToArchiveCommandExecuted(object p)
        {
            int x = 0;
        }

        public ObservableCollection<PatientInfo> Collection { get; set; } 

        public object AppViewModel { get; set; }
    }
}
