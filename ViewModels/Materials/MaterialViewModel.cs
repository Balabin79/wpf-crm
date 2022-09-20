using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using DevExpress.Mvvm.DataAnnotations;
using Dental.ViewModels.Invoices;
using Dental.Infrastructures.Converters;
using Dental.Views.NomenclatureDir;
using Nomenclature = Dental.Models.Nomenclature;
using Dental.ViewModels.ServicePrice;
using Dental.Views.ServicePrice;
using DevExpress.Xpf.Printing;
using System.Windows.Data;
using GroupInfo = DevExpress.Xpf.Printing.GroupInfo;
using Dental.Services;

namespace Dental.ViewModels.Materials
{
    class MaterialViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public MaterialViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Collection = db.Nomenclature.OrderBy(f => f.IsDir == 0).ThenBy(f => f.Name).Include(f => f.Parent).Include(f => f.Measure).ToObservableCollection();
                SetMeasures();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Материалы\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanSelectItemInServiceField(object p) => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;
        public bool CanExpandTree(object p) => true;
        public bool CanDelete(object p) => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureDeletable;
        public bool CanSave() => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;
        public bool CanOpenForm(object p) => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;
        public bool CanCancelForm() => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;
        public bool CanOpenFormMeasure() => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;
        public bool CanOpenByParentForm(object p) => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;
        public bool CanOpenDirByParentForm(object p) => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;

        public bool CanPrintPrice() => true;
        public bool CanLoadDocForPrint() => true;


        [Command]
        public void SelectItemInServiceField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is Nomenclature item)
                    {
                        //if (service.IsDir == 1) return;
                        parameters.Popup.EditValue = item;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch
            {

            }
        }

        [Command]
        public void ExpandTree(object p)
        {
            try
            {
                if (p is TreeListView tree)
                {
                    foreach (var node in tree.Nodes)
                    {
                        if (node.IsExpanded)
                        {
                            tree.CollapseAllNodes();                           
                            return;
                        }
                    }
                    tree.ExpandAllNodes();
                    return;
                }
            }
            catch {}
        }
        [Command]
        public void Delete(object p)
        {
            try
            {
                if (p == null) return;
                Model = Collection.Where(f => f.Id == (int)p).FirstOrDefault();
                if (Model == null || !new ConfirDeleteInCollection().run(Model.IsDir)) return;

                if (Model.IsDir == 0) 
                { 
                    db.Entry(Model).State = EntityState.Deleted;
                    Collection.Remove(Model);
                }
                else
                {
                    var collection = new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model).GetItemChilds().OfType<Nomenclature>().ToObservableCollection();
                    collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
                    collection.ForEach(f => Collection.Remove(f));
                }

                db.SaveChanges();
            }
            catch 
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке удаления произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Save()
        {
            try
            {
                 //ищем совпадающий элемент
                 var matchingItem = Collection.Where(f => f.IsDir == MaterialVM.IsDir && f.Name == MaterialVM.Name && MaterialVM.Code == f.Code && MaterialVM.Guid != f.Guid).ToList();

                if (matchingItem.Count() > 0 && matchingItem.Any(f => f.ParentId == Model.ParentId))
                {
                    if (!new TryingCreatingDuplicate().run(Model.IsDir)) return;
                    
                }

                Model.IsDir = MaterialVM.IsDir;
                Model.Name = MaterialVM.Name;
                Model.Code = MaterialVM.Code;
                Model.Price = MaterialVM.Price;
                Model.Parent = MaterialVM.Parent?.Guid == "000" ? null : MaterialVM.Parent;
                Model.ParentId = MaterialVM.Parent?.Guid == "000" ? null : MaterialVM.ParentId;
                Model.Measure = MaterialVM.Measure;
                Model.MeasureId = MaterialVM.MeasureId;

                if (Model.Id == 0) 
                {
                    db.Entry(Model).State = EntityState.Added;
                    if (db.SaveChanges() > 0) Collection.Add(Model);
                }  
                else 
                {
                    db.Entry(Model).State = EntityState.Modified;
                    if(db.SaveChanges() > 0)
                    {
                        Collection.Remove(Collection.FirstOrDefault(f => f.Id == Model.Id));
                        Collection.Add(Model);
                    }

                } 
                //db.SaveChanges();

                Window.Close();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке сохранения в бд произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
        
        [Command]
        public void OpenForm(object p)
        {
            try
            {
                if (!int.TryParse(p.ToString(), out int param)) return;                
                Model = (param > 0) ?  db.Nomenclature.Where(f => f.Id == param).Include(f => f.Parent).FirstOrDefault() : new Nomenclature();
                Model.IsDir = (param < 0) ? param == -1 ? 0 : 1 : Model.IsDir;
                MaterialVM = new MaterialVM(db, Model.Id)
                {
                    Name = Model.Name,
                    Price = Model.Price,
                    ParentId = Model.ParentId,
                    Parent = Model.Parent,
                    IsDir = Model.IsDir ?? 0,
                    Code = Model.Code,
                    IsVisibleItemForm = Model.IsDir == 0,
                    Guid = Model.Guid,
                    Measure = Model.Measure,
                    MeasureId = Model.MeasureId
                };

                if ((Model.Id > 0 && MaterialVM.ParentId != null && MaterialVM.Materials.Count > 0) || (Model.Id == 0)) MaterialVM.Materials?.Add(WithoutCategory);
  
                Window = new NomenclatureWindow() { DataContext = this, Height = MaterialVM.IsDir == 0 ? 325 : 234};
                Window.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке открытия формы произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
        
        [Command]
        public void CancelForm() => Window.Close();

        [Command]
        public void OpenFormMeasure()
        {
            try
            {
                MeasureWin = new MeasureWindow() { DataContext = new MeasureViewModel( this, db) };
                MeasureWin.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке открытия формы \"Единицы измерения\" произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenByParentForm(object p)
        {
            try
            {
                if (!int.TryParse(p.ToString(), out int param)) return;

                var model = db.Nomenclature.Where(f => f.Id == param).Include(f => f.Parent).FirstOrDefault();
                if (model?.IsDir == 0)
                {

                    MaterialVM = new MaterialVM(db)
                    {
                        ParentId = model.ParentId,
                        Parent = model.Parent,
                        IsDir = 0,
                        IsVisibleItemForm = true,
                    };
                }
                else
                {
                    MaterialVM = new MaterialVM(db)
                    {
                        ParentId = model.Id,
                        Parent = model,
                        IsDir = 0,
                        IsVisibleItemForm = true,
                    };
                }
                Model = new Nomenclature();
                Window = new NomenclatureWindow() { DataContext = this, Height = MaterialVM.IsDir == 0 ? 325 : 234 };
                Window.Show();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке открытия формы произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenDirByParentForm(object p)
        {
            try
            {
                if (!int.TryParse(p.ToString(), out int param)) return;

                var model = db.Nomenclature.Where(f => f.Id == param).Include(f => f.Parent).FirstOrDefault();
                if (model?.IsDir == 1)
                {

                    MaterialVM = new MaterialVM(db)
                    {
                        ParentId = model.Id,
                        Parent = model,
                        IsDir = 1,
                        IsVisibleItemForm = false,
                    };
                }
                else
                {
                    MaterialVM = new MaterialVM(db)
                    {
                        ParentId = model.ParentId,
                        Parent = model.Parent,
                        IsDir = 1,
                        IsVisibleItemForm = false,
                    };
                }
                Model = new Nomenclature();
                Window = new NomenclatureWindow() { DataContext = this, Height = MaterialVM.IsDir == 0 ? 325 : 234 };
                Window.Show();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке открытия формы произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }


        public Nomenclature WithoutCategory { get; set; } = new Nomenclature() { Id = 0, IsDir = null, ParentId = null, Name = "Без категории", Guid = "000" };

        public ObservableCollection<Nomenclature> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }
        public Nomenclature Model { get; set; }        
        private NomenclatureWindow Window;
        public MaterialVM MaterialVM
        {
            get { return GetProperty(() => MaterialVM); }
            set { SetProperty(() => MaterialVM, value); }
        }
        public ICollection<Measure> Measures
        {
            get { return GetProperty(() => Measures); }
            set { SetProperty(() => Measures, value); }
        }

        private MeasureWindow MeasureWin;

        public void SetMeasures() => Measures = db.Measure.ToObservableCollection();


        #region Печать
        public PrintServiceWindow PrintServiceWindow { get; set; }

        [Command]
        public void PrintPrice()
        {
            PrintServiceWindow = new PrintServiceWindow() { DataContext = this };
            PrintServiceWindow.Show();
        }

        [Command]
        public void LoadDocForPrint()
        {
            // Create a link and assign a data source to it.
            // Assign your data templates to different report areas.
            CollectionViewLink link = new CollectionViewLink();

            var db = new ApplicationContext();
            CollectionViewSource Source = new CollectionViewSource();

            SetSourceCollectttion();
            Source.Source = SourceCollection;

            Source.GroupDescriptions.Add(new PropertyGroupDescription("ParentName"));

            link.CollectionView = Source.View;
            link.GroupInfos.Add(new GroupInfo((DataTemplate)PrintServiceWindow.Resources["CategoryTemplate"]));
            link.DetailTemplate = (DataTemplate)PrintServiceWindow.Resources["ProductTemplate"];

            // Associate the link with the Document Preview control.
            PrintServiceWindow.preview.DocumentSource = link;

            // Generate the report document 
            // and show pages as soon as they are created.
            link.CreateDocument(true);
        }

        public ICollection<PrintService> SourceCollection { get; set; } = new List<PrintService>();

        private void SetSourceCollectttion()
        {
            db.Nomenclature?.GroupBy(f => f.Parent)?.Where(f => f.Key != null).ForEach(f => f.ForEach(
                i => SourceCollection?.Add(new PrintService() { ParentName = f.Key.Name, ServiceName = i.Name, Price = i.Price })));
        }
        #endregion
    }

}
