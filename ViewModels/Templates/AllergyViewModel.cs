using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using Dental.Services;
using System.Data.Entity;
using Dental.Models.Base;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using Dental.Views.Templates;
using Dental.Infrastructures.Converters;
using DevExpress.Xpf.Grid;
using Dental.Infrastructures.Collection;
using Dental.Models.Templates;
using System;

namespace Dental.ViewModels.Templates
{
    public class AllergyViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public AllergyViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Collection = db?.Allergies.Include(f => f.Parent).OrderByDescending(f => f.IsDir).OrderBy(f => f.Name).ToObservableCollection() ?? new ObservableCollection<Allergy>();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Аллергии\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ObservableCollection<Allergy> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        [Command]
        public void OpenForm(object p)
        {
            try
            {
                if (!int.TryParse(p.ToString(), out int param)) return;
                Model = (param > 0) ? db.Allergies.Where(f => f.Id == param).Include(f => f.Parent).FirstOrDefault() : new Allergy();
                Model.IsDir = (param < 0) ? param == -1 ? 0 : 1 : Model.IsDir;
                VM = new AllergyMediatorVM(db, Model.Id)
                {
                    Name = Model.Name,
                    ParentId = Model.ParentId,
                    Parent = Model.Parent,
                    IsDir = Model.IsDir ?? 0,
                    IsVisibleItemForm = Model.IsDir == 0,
                    Guid = Model.Guid
                };

                if ((Model.Id > 0 && VM.ParentId != null && VM.Collection?.Count > 0) || (Model.Id == 0)) VM.Collection?.Add(WithoutCategory);

                Window = new TemplateWin() { DataContext = this, Height = VM.IsDir == 0 ? 280 : 235 };
                Window.Show();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке открытия формы произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            } 
        }

        [Command]
        public void CloseForm(object p) => Window?.Close();       

        [Command]
        public void SelectItemInField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is Allergy item)
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
            catch { }
        }


        [Command]
        public void Save(object p)
        {
            try
            {
                //ищем совпадающий элемент
                var matchingItem = Collection.Where(f => f.IsDir == VM.IsDir && f.Name == VM.Name && VM.Guid != f.Guid).ToList();

                if (matchingItem.Count() > 0 && matchingItem.Any(f => f.ParentId == Model.ParentId))
                {
                    if (!new TryingCreatingDuplicate().run(Model.IsDir)) return;
                }

                Model.IsDir = VM.IsDir;
                Model.Name = VM.Name;
                Model.Parent = VM.Parent?.Guid == "000" ? null : VM.Parent;
                Model.ParentId = VM.Parent?.Guid == "000" ? null : VM.ParentId;

                if (Model.Id == 0)
                {
                    db.Entry(Model).State = EntityState.Added;
                    if (db.SaveChanges() > 0) Collection.Add(Model);
                }
                else
                {
                    db.Entry(Model).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        Collection.Remove(Collection.FirstOrDefault(f => f.Id == Model.Id));
                        Collection.Add(Model);
                    }
                }
                Window.Close();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке сохранения в бд произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
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
                    var collection = new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model).GetItemChilds().OfType<Allergy>().ToObservableCollection();
                     collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
                     collection.ForEach(f => Collection.Remove(f));
                }

                db.SaveChanges();
            }
            catch(Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке удаления произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
        
        public AllergyMediatorVM VM { get; set; }
        public Allergy WithoutCategory { get; set; } = new Allergy() { Id = 0, IsDir = 1, ParentId = 0, Name = "Без категории", Guid = "000" };
        public Allergy Model { get; set; }
        private TemplateWin Window;
    }
}
