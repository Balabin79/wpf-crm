﻿using System;
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
    class NomenclatureViewModel : ViewModelBase
    {

        ApplicationContext db;
        public NomenclatureViewModel()
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
                if (Unit?.SelectedUnit != null) Model.UnitId = ((Unit)Unit.SelectedUnit).Id;
                if (NomenclatureGroup?.SelectedNomenclatureGroup != null) Model.NomenclatureGroupId = ((NomenclatureGroup)NomenclatureGroup.SelectedNomenclatureGroup).Id;
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
                    Title = "Редактировать номенклатуру";
                    NomenclatureGroup = new NomenclatureGroupViewModel(Model?.NomenclatureGroup?.Id);
                    Unit = new UnitViewModel(Model?.Unit?.Id);
                }
                else
                {
                    Model = CreateNewModel();
                    Title = "Создать номенклатуру";
                    NomenclatureGroup = new NomenclatureGroupViewModel();
                    Unit = new UnitViewModel();
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

        /************* Специфика этой ViewModel ******************/

        public NomenclatureGroupViewModel NomenclatureGroup { get; set; }
        public UnitViewModel Unit { get; set; }

        /******************************************************/
        public ObservableCollection<Nomenclature> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        public Nomenclature Model { get; set; }
        public string Title { get; set; }       
        private ObservableCollection<Nomenclature> _Collection;
        private NomenclatureWindow Window;       
        private ObservableCollection<Nomenclature> GetCollection() => db.Nomenclature.OrderBy(d => d.Name).Include(b => b.NomenclatureGroup).ToObservableCollection();      
        private void CreateNewWindow() => Window = new NomenclatureWindow(); 
        private Nomenclature CreateNewModel() => new Nomenclature();
        private Nomenclature GetModelById(int id) => db.Nomenclature.Where(f => f.Id == id).Include(b => b.NomenclatureGroup).Include(b => b.Unit).FirstOrDefault();
              
        private void Add() => db.Nomenclature.Add(Model);
        private void Update() => db.Entry(Model).State = EntityState.Modified;
        private void Delete() => db.Entry(Model).State = EntityState.Deleted;

        
    }
}
