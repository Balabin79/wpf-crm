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

namespace Dental.ViewModels.AdditionalFields
{
    public class AdditionalFieldsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
     /*   public AdditionalFieldsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Collection = db.AdditionalField.OrderByDescending(f => f.IsSys == 1).ThenBy(f => f.IsDir == 1).ThenBy(f => f.Caption).Include(f => f.Parent).Include(f => f.TypeValue).ToObservableCollection();
                CommonValueViewModel = new CommonValueViewModel(db);
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
                if (p is AdditionalField field)
                {
                    Model = new AdditionalField();
                    if (field.IsSys == 1) // новая
                    {
                        FieldVM = new FieldVM(db, field.Id) { Parent = field, ParentId = field.Id, IsSys = 0, IsDir = 0, Id = 0 };
                    }
                    else // редактирование
                    {
                        FieldVM = new FieldVM(db, Model.Id)
                        {
                            Caption = field.Caption,
                            SysName = field.SysName,
                            ParentId = field.ParentId,
                            Parent = field.Parent,
                            IsDir = field.IsDir,
                            IsSys = 0,
                            TypeValue = field.TypeValue,
                            Guid = field.Guid,
                            Id = field.Id
                        };
                        Model = field;
                    }
                }               
                Window = new FieldWindow() { DataContext = this, Height = 275 };
                Window.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Delete(object p)
        {
            try
            {
                if (p is AdditionalField field)
                {
                    if (field.IsSys == 1) return;
                    if (!new ConfirDeleteInCollection().run(Model.IsDir)) return;
                    db.Entry(field).State = EntityState.Deleted;
                    if (db.SaveChanges() > 0) 
                    {
                        Collection.Remove(field); 
                    }

                }
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
                Model.IsDir = FieldVM.IsDir;
                Model.IsSys = FieldVM.IsSys;
                Model.Caption = FieldVM.Caption;
                Model.SysName = FieldVM.SysName;
                Model.TypeValueId = FieldVM.TypeValue?.Id;
                Model.ParentId = FieldVM.Parent?.Id;

                if (Model.Id == 0)
                {
                    db.Entry(Model).State = EntityState.Added;
                    if (db.SaveChanges() > 0) 
                    { 
                        Collection.Add(Model);
                    }
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
            catch(Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке сохранения в бд произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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

        public CommonValueViewModel CommonValueViewModel { get; set; }
        public FieldWindow Window { get; private set; }
        public FieldVM FieldVM { get; private set; }
        public AdditionalField Model { get; private set; }


        public ObservableCollection<AdditionalField> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }*/





    }
}
