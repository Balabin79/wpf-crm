using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Views.PatientCard;
using Dental.Views.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Services;
using Dental.Models;

namespace Dental.ViewModels
{
    public class PatientListViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public PatientListViewModel()
        {
            try
            {
                db = new ApplicationContext();
                SetCollection();

                #region инициализация команд, связанных с общим функционалом карты пациента
                OpenClientCardCommand = new LambdaCommand(OnOpenClientCardCommandExecuted, CanOpenClientCardCommandExecute);
                ShowArchiveCommand = new LambdaCommand(OnShowArchiveCommandExecuted, CanShowArchiveCommandExecute);
                #endregion

                OpenFormAdvertisingCommand = new LambdaCommand(OnOpenFormAdvertisingExecuted, CanOpenFormAdvertisingExecute);

                OpenFormIdsCommand = new LambdaCommand(OnOpenFormIdsExecuted, CanOpenFormIdsExecute);

                BtnIconArchive = false;
                BtnIconList = true;
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Список пациентов\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand OpenClientCardCommand { get; }
        public ICommand ShowArchiveCommand { get; }
        public ICommand OpenFormAdvertisingCommand { get; }
        public ICommand OpenFormIdsCommand { get; }

        private bool CanOpenClientCardCommandExecute(object p) => true;
        private bool CanShowArchiveCommandExecute(object p) => true;
        private bool CanOpenFormAdvertisingExecute(object p) => true;
        private bool CanOpenFormIdsExecute(object p) => true;

        private void OnOpenFormIdsExecuted(object p)
        {
            try
            {
                IdsWin = new IdsWindow();
                IdsWin.ShowDialog();
            }
            catch 
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Документы\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        private void OnOpenFormAdvertisingExecuted(object p)
        {
            try
            {
                AdvertisingWin = new AdvertisingWindow();
                AdvertisingWin.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnOpenClientCardCommandExecuted(object p)
        {
            try
            {
                ClientCardWin = (p != null) ? 
                    new ClientCardWindow((int)p, this) 
                    : 
                    new ClientCardWindow(0, this);
                ClientCardWin.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Карта клиента\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        } 
        
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

        public ObservableCollection<Client> _Collection;
        public ObservableCollection<Client> Collection 
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }

        public AdvertisingWindow AdvertisingWin { get; set; } 
        public IdsWindow IdsWin { get; set; }
        public ClientCardWindow ClientCardWin { get; set; }

        private void SetCollection(bool isArhive=false)
        {
            Collection =
                 db.Clients
                // ?.Include(i => i.TreatmentPlans.Select(g => g.TreatmentPlanItems.Select(c => c.Classificator)))
                // ?.Include(i => i.TreatmentPlans.Select(g => g.TreatmentPlanItems.Select(c => c.Employee)))
                // ?.Include(i => i.TreatmentPlans.Select(g => g.TreatmentPlanItems.Select(c => c.Status)))
                .OrderBy(f => f.LastName).Where(f => f.IsInArchive == isArhive).ToObservableCollection();

        }
    }
}