using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
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
using Dental.Views.AdditionalFields;
using Dental.Infrastructures.Converters;

namespace Dental.ViewModels.AdditionalFields
{
    public class AdditionalFieldsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public AdditionalFieldsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Collection = db.AdditionalField.OrderByDescending(f => f.IsSys == 1).ThenBy(f => f.IsDir == 1).ThenBy(f => f.Caption).Include(f => f.Parent).ToObservableCollection();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Дополнительные поля\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenForm(object p)
        {
            try
            {
                if (!int.TryParse(p.ToString(), out int param)) return;
                Model = (param > 0) ? db.AdditionalField.Where(f => f.Id == param).Include(f => f.Parent).FirstOrDefault() : new AdditionalField();
                Model.IsDir = (param < 0) ? param == -1 ? 0 : 1 : Model.IsDir;
                FieldVM = new FieldVM(db, Model.Id)
                {
                    Caption = Model.Caption,
                    SysName = Model.SysName,
                    ParentId = Model.ParentId,
                    Parent = Model.Parent,
                    IsDir = Model.IsDir ?? 0,
                    TypeValue = Model.TypeValue,
                    IsVisibleItemForm = Model.IsDir == 0,
                    Guid = Model.Guid
                };

                if ((Model.Id > 0 && FieldVM.ParentId != null && FieldVM.Fields.Count > 0) || (Model.Id == 0)) FieldVM.Fields?.Add(WithoutCategory);

                Window = new FieldWindow() { DataContext = this, Height = FieldVM.IsDir == 0 ? 325 : 230 };
                Window.ShowDialog();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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
                    var collection = new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model).GetItemChilds().OfType<AdditionalField>().ToObservableCollection();
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
                var matchingItem = Collection.Where(f => f.IsDir == FieldVM.IsDir && f.Caption == FieldVM.Caption && FieldVM.SysName == f.SysName && FieldVM.TypeValue == f.TypeValue && FieldVM.Guid != f.Guid).ToList();

                if (matchingItem.Count() > 0 && matchingItem.Any(f => f.ParentId == Model.ParentId))
                {
                    if (!new TryingCreatingDuplicate().run(Model.IsDir)) return;

                }

                Model.IsDir = FieldVM.IsDir;
                Model.Caption = FieldVM.Caption;
                Model.SysName = FieldVM.SysName;
                Model.TypeValue = FieldVM.TypeValue;
                Model.Parent = FieldVM.Parent?.Guid == "000" ? null : FieldVM.Parent;
                Model.ParentId = FieldVM.Parent?.Guid == "000" ? null : FieldVM.ParentId;

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
        public void SelectItemInServiceField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is AdditionalField item)
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
        public void CancelForm() => Window.Close();

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


        public FieldWindow Window { get; private set; }
        public FieldVM FieldVM { get; private set; }
        public AdditionalField Model { get; private set; }
        public AdditionalField WithoutCategory { get; set; } = new AdditionalField() { Id = 0, IsDir = null, ParentId = null, Caption = "Без категории", Guid = "000" };
        public ObservableCollection<AdditionalField> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }





    }
}
