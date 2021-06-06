using System;
using System.Windows;
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
using Dental.Interfaces;
using Dental.Models.Base;
using DevExpress.Xpf.Core;

namespace Dental.ViewModels
{
    class NomenclatureViewModel : ViewModelBase
    {

        readonly ApplicationContext db;
        public NomenclatureViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);
            CancelFormCommand = new LambdaCommand(OnCancelFormCommandExecuted, CanCancelFormCommandExecute);

            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();
            } catch (Exception)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с данным разделом!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
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
                if (Model == null || !new ConfirDeleteInCollection().run(Model.IsDir)) return;

                if (Model.IsDir == 0) Delete(new ObservableCollection<Nomenclature>(){Model});
                else Delete (new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetItemChilds().OfType<Nomenclature>().ToObservableCollection());
                db.SaveChanges();
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

                //ищем совпадающий элемент
                var matchingItem = Collection.Where(f => f.IsDir == Model.IsDir && f.Name == Model.Name && Model.Id != f.Id).ToList();

                if (SelectedNomenclatureGroup != null) Model.ParentId = ((Nomenclature)SelectedNomenclatureGroup).Id;

                if (matchingItem.Count()>0 &&  matchingItem.Any(f => f.ParentId == Model.ParentId))
                {
                    new TryingCreatingDuplicate().run(Model.IsDir);
                    return;
                }               

                if (Model.Id == 0) Add(); else Update();
                db.SaveChanges();

                SelectedNomenclatureGroup = null;
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
                if (p == null) return;
                int.TryParse(p.ToString(), out int param);
                if (param == -3) return;

                switch (param)
                {
                    case -1:
                        Model = CreateNewModel();
                        Unit = new UnitViewModel();
                        Model.IsDir = 0;
                        Title = "Создать номенклатуру";
                        NomenclatureGroup = Collection.Where(f => f.IsDir == 1 && f.Id != Model?.Id).OrderBy(f => f.Name).ToObservableCollection();
                        VisibleItemForm();
                        break;
                    case -2:
                        Model = CreateNewModel();
                        Unit = new UnitViewModel();
                        Title = "Создать номенклатурную группу";
                        Model.IsDir = 1;
                        NomenclatureGroup = Collection.Where(f => f.IsDir == 1 && f.Id != Model?.Id).OrderBy(f => f.Name).ToObservableCollection();
                        VisibleItemGroup();
                        break;
                    default:
                        Model = GetModelById(param);
                        NomenclatureGroup = new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetDirectories().OfType<Nomenclature>().ToObservableCollection();
                        
                        SelectedNomenclatureGroup = Collection.Where(f => f.Id == Model?.ParentId && f.Id != Model.Id).FirstOrDefault();
                        Unit = new UnitViewModel(Model?.UnitId);
                        if (Model.IsDir == 0)
                        {
                            Title = "Редактировать номенклатуру";
                            VisibleItemForm();
                        }
                        else
                        {
                            Title = "Редактировать номенклатурную группу";
                            VisibleItemGroup();
                        }
                        break;
                }

                Window.DataContext = this;
                Window.ShowDialog();
                SelectedNomenclatureGroup = null;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCancelFormCommandExecuted(object p) => Window.Close();

        /************* Специфика этой ViewModel ******************/
        public UnitViewModel Unit { get; set; }

        public ICollection<Nomenclature> NomenclatureGroup { get; set; }

        private object _SelectedNomenclatureGroup;
        public object SelectedNomenclatureGroup
        {
            get => _SelectedNomenclatureGroup;
            set => Set(ref _SelectedNomenclatureGroup, value); 
        }


        /******************************************************/
        public ObservableCollection<Nomenclature> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        public Nomenclature Model { get; set; }
        public string Title { get; set; }
        public Visibility IsVisibleItemForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupForm { get; set; } = Visibility.Hidden;


        private void VisibleItemForm() 
        {
            IsVisibleItemForm = Visibility.Visible;
            IsVisibleGroupForm = Visibility.Hidden;
            Window.Height = 330;
        }
        private void VisibleItemGroup() 
        {
            IsVisibleItemForm = Visibility.Hidden;
            IsVisibleGroupForm = Visibility.Visible;
            Window.Height = 280;
        }

        private ObservableCollection<Nomenclature> _Collection;
        private NomenclatureWindow Window;       
        private ObservableCollection<Nomenclature> GetCollection() => db.Nomenclature.OrderBy(d => d.Name).ToObservableCollection();      
        private void CreateNewWindow() => Window = new NomenclatureWindow(); 
        private Nomenclature CreateNewModel() => new Nomenclature();

        private Nomenclature GetModelById(int id) 
        {
            return Collection.Where(f => f.Id == id).FirstOrDefault();
        }

        private void Add() 
        { 
            db.Entry(Model).State = EntityState.Added;
            db.SaveChanges();
            Collection.Add(Model);
        }
        private void Update() 
        { 
            db.Entry(Model).State = EntityState.Modified;
            db.SaveChanges();
            var index = Collection.IndexOf(Model);
            if (index != -1) Collection[index] = Model;
        }

        private void Delete(ObservableCollection<Nomenclature> collection)
        {
            collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
            collection.ForEach(f => Collection.Remove(f));
        }    
    }
}
