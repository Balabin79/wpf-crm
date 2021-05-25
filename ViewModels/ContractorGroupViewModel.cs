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

namespace Dental.ViewModels
{
    class ContractorGroupViewModel : ViewModelBase
    {

        private readonly ApplicationContext db;
        public ContractorGroupViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);
            CancelFormCommand = new LambdaCommand(OnCancelFormCommandExecuted, CanCancelFormCommandExecute);
            db = new ApplicationContext();
            Collection = GetCollection();
        }

        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenFormCommand { get; }
        public ICommand CancelFormCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanOpenFormCommandExecute(object p) => true;
        private bool CanCancelFormCommandExecute(object p) => true;


        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                if (p == null) return;
                Model = GetModelById((int)p);
                if (Model == null || !new ConfirDeleteInCollection().run((int)TypeItem.File)) return;
                Delete();
                db.SaveChanges();
                Collection = GetCollection();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSaveCommandExecuted(object p)
        {
            try
            {
                if (Model.Id == 0) Add(); else Update();
                db.SaveChanges();
                Collection = GetCollection();
                Window.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnOpenFormCommandExecuted(object p)
        {
            try
            {
                CreateNewWindow();
                if (p != null)
                {
                    Model = GetModelById((int)p);
                    Title = "Редактировать группу контрагентов";
                }
                else
                {
                    Model = CreateNewModel();
                    Title = "Создать новую группу контрагентов";
                }
                Window.DataContext = this;
                Window.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCancelFormCommandExecuted(object p) => Window.Close();

        /******************************************************/
        public ObservableCollection<ContractorGroup> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        public ContractorGroup Model { get; set; }
        public string Title { get; set; }
       
        private ObservableCollection<ContractorGroup> _Collection;
        private ContractorGroupWindow Window;       
        private ObservableCollection<ContractorGroup> GetCollection() => db.ContractorsGroup.OrderBy(d => d.Name).ToObservableCollection();      
        private void CreateNewWindow() => Window = new ContractorGroupWindow(); 
        private ContractorGroup CreateNewModel() => new ContractorGroup();
        private ContractorGroup GetModelById(int id) => db.ContractorsGroup.Where(f => f.Id == id).FirstOrDefault();
        private void Add() => db.ContractorsGroup.Add(Model);
        private void Update() => db.Entry(Model).State = EntityState.Modified;
        private void Delete() => db.Entry(Model).State = EntityState.Deleted;
    }
}
