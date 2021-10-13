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
                db = Db.Instance.Context;
                SetCollection();

                #region инициализация команд, связанных с общим функционалом карты пациента
                OpenPatientCardCommand = new LambdaCommand(OnOpenPatientCardCommandExecuted, CanOpenPatientCardCommandExecute);
                ShowArchiveCommand = new LambdaCommand(OnShowArchiveCommandExecuted, CanShowArchiveCommandExecute);
                #endregion

                BtnIconArchive = false;
                BtnIconList = true;
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

        public ICommand ShowArchiveCommand { get; }
        private bool CanShowArchiveCommandExecute(object p) => true;
        private void OnShowArchiveCommandExecuted(object p)
        {
            try
            {
                BtnIconArchive = !BtnIconArchive;
                BtnIconList = !BtnIconList;
                if (BtnIconList) SetCollection();
                else SetCollection(true);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private bool _BtnIconArchive;
        public bool BtnIconArchive
        {
            get => _BtnIconArchive;
            set => Set(ref _BtnIconArchive, value);
        }

        private bool _BtnIconList;
        public bool BtnIconList
        {
            get => _BtnIconList;
            set => Set(ref _BtnIconList, value);
        }

        public ObservableCollection<PatientInfo> _Collection;
        public ObservableCollection<PatientInfo> Collection 
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }

        private void SetCollection(bool isArhive=false)
        {
           Collection = db.PatientInfo.OrderBy(f => f.LastName).Where(f => f.IsInArchive == isArhive).ToObservableCollection();
        }

    }
}
