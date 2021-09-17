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


namespace Dental.ViewModels
{
    class PatientCardViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public PatientCardViewModel()
        {
            try
            {
                db = new ApplicationContext();
                              
                ClickToothGreenCommand = new LambdaCommand(OnClickToothGreenCommandExecuted, CanClickToothGreenCommandExecute);
                ClickToothYelPlCommand = new LambdaCommand(OnClickToothYelPlCommandExecuted, CanClickToothYelPlCommandExecute);
                ClickToothYelCorCommand = new LambdaCommand(OnClickToothYelCorCommandExecuted, CanClickToothYelCorCommandExecute);
                ClickToothImpCommand = new LambdaCommand(OnClickToothImpCommandExecuted, CanClickToothImpCommandExecute);
                ClickToothRedRCommand = new LambdaCommand(OnClickToothRedRCommandExecuted, CanClickToothRedRCommandExecute);
                ClickToothRedPtCommand = new LambdaCommand(OnClickToothRedPtCommandExecuted, CanClickToothRedPtCommandExecute);
                ClickToothRedPCommand = new LambdaCommand(OnClickToothRedPCommandExecuted, CanClickToothRedPCommandExecute);
                ClickToothRedCCommand = new LambdaCommand(OnClickToothRedCCommandExecuted, CanClickToothRedCCommandExecute);
                ClickToothGrayCommand = new LambdaCommand(OnClickToothGrayCommandExecuted, CanClickToothGrayCommandExecute);


                DiscountGroupList = db.DiscountGroups.OrderBy(f => f.Name).ToObservableCollection();
                AdvertisingList = db.Advertising.OrderBy(f => f.Name).ToObservableCollection();
                ClientsGroupList = db.ClientsGroup.OrderBy(f => f.Name).ToObservableCollection();
                ClientTreatmentPlans = db.ClientTreatmentPlans.OrderBy(f => f.TreatmentPlanNumber).ToObservableCollection();
             
                _Teeth = new Teeth();

            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с данным разделом!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }

        }


        public ICommand ClickToothGreenCommand { get; }
        public ICommand ClickToothYelPlCommand { get; }
        public ICommand ClickToothYelCorCommand { get; }
        public ICommand ClickToothImpCommand { get; }
        public ICommand ClickToothRedRCommand { get; }
        public ICommand ClickToothRedPtCommand { get; }
        public ICommand ClickToothRedPCommand { get; }
        public ICommand ClickToothRedCCommand { get; }
        public ICommand ClickToothGrayCommand { get; }


        private bool CanClickToothGreenCommandExecute(object p) => true;
        private bool CanClickToothYelPlCommandExecute(object p) => true;
        private bool CanClickToothYelCorCommandExecute(object p) => true;
        private bool CanClickToothImpCommandExecute(object p) => true;
        private bool CanClickToothRedRCommandExecute(object p) => true;
        private bool CanClickToothRedPtCommandExecute(object p) => true;
        private bool CanClickToothRedPCommandExecute(object p) => true;
        private bool CanClickToothRedCCommandExecute(object p) => true;
        private bool CanClickToothGrayCommandExecute(object p) => true;

        private void OnClickToothGreenCommandExecuted(object p)
        {
            try
            {
                int x = 0;

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothYelPlCommandExecuted(object p)
        {
            try
            {
                int x = 0;

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothYelCorCommandExecuted(object p)
        {
            try
            {
                int x = 0;

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothImpCommandExecuted(object p)
        {
            try
            {
                int x = 0;

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothRedRCommandExecuted(object p)
        {
            try
            {
                int x = 0;

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothRedPtCommandExecuted(object p)
        {
            try
            {
                int x = 0;

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothRedPCommandExecuted(object p)
        {
            try
            {
                int x = 0;

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothRedCCommandExecuted(object p)
        {
            try
            {
                int x = 0;

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothGrayCommandExecuted(object p)
        {
            try
            {
                int x = 0;

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public string SelectedGender { get; set; }
        public object SelectedDiscountGroups { get; set; }
        public object SelectedAdvertisings { get; set; }
        public string SelectedClientsGroup { get; set; }

        public Teeth _Teeth;
        public Teeth Teeth { 
            get => _Teeth; 
            set =>Set (ref _Teeth, value); 
        }


        public IEnumerable<DiscountGroups> DiscountGroupList { get; set; }
        public ICollection<Advertising> AdvertisingList { get; set; }
        public ICollection<ClientsGroup> ClientsGroupList { get; set; }
        public ObservableCollection<ClientTreatmentPlans> ClientTreatmentPlans { get; set; }

        public ICollection<string> GenderList
        {
            get => _GenderList;
        }
        private ICollection<string> _GenderList = new List<string> { "Мужчина", "Женщина" };
    }
}
