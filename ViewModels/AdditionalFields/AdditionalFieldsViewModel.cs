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
using Dental.Views.WindowForms;
using Dental.Services;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Views.AdditionalFields;
using Dental.ViewModels.Invoices;

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
                Collection = GetCollection();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Дополнительные поля\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }


        public CategoryWindow CategoryWindow { get; private set; }
        public FieldWindow FieldWindow { get; private set; }
        public CategoryVM CategoryVM { get; private set; }

        [Command]
        public void OpenCategoryForm(object p)
        {
            try
            {

                if (p == null)
                {
                    CategoryVM = new CategoryVM() { Title = "Новая категория"};
                    CategoryWindow = new CategoryWindow() { DataContext = CategoryVM};
                    CategoryWindow.Show();
                }


                /*

                CreateNewWindow();
                if (p == null) return;
                int.TryParse(p.ToString(), out int param);
                if (param == -3) return;

                switch (param)
                {
                    case -1:
                        Model = CreateNewModel();
                        Model.IsDir = 0;
                        Title = "Новая позиция";
                        Group = Collection.Where(f => f.IsDir == 1 && f.Guid != Model?.Guid).OrderBy(f => f.Name).ToObservableCollection();
                        VisibleItemForm();
                        break;
                    case -2:
                        Model = CreateNewModel();
                        Title = "Создать группу";
                        Model.IsDir = 1;
                        Group = Collection.Where(f => f.IsDir == 1 && f.Guid != Model?.Guid).OrderBy(f => f.Name).ToObservableCollection();
                        if (Group.Count != 0) Group.Add(WithoutCategory);
                        VisibleItemGroup();
                        break;
                    default:
                        Model = GetModelById(param);
                        Group = new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetDirectories().OfType<Service>().ToObservableCollection();
                        if (Group.Count > 0 && Model.ParentId != null && Model.IsDir == 1) Group.Add(WithoutCategory);
                        SelectedGroup = Collection.Where(f => f.Id == Model?.ParentId && f.Id != Model.Id).FirstOrDefault();

                        if (Model.IsDir == 0)
                        {
                            Title = "Редактировать позицию";
                            VisibleItemForm();
                        }
                        else
                        {
                            Title = "Редактировать группу";
                            VisibleItemGroup();
                        }
                        break;
                }

                Window.DataContext = this;
                Window.ShowDialog();*/
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void OpenFieldForm(object p)
        { }


            /// <summary>
            /// /////////////////////////
            /// </summary>
            /// <param name="p"></param>

            [Command]
        public void Delete(object p)
        {
            try
            {
                if (p == null) return;
                Model = GetModelById((int)p);
                if (Model == null || !new ConfirDeleteInCollection().run(Model.IsDir)) return;

                if (Model.IsDir == 0) Delete(new ObservableCollection<Service>() { Model });
                else
                {
                    Delete(new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model).GetItemChilds().OfType<Service>().ToObservableCollection());
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        [Command]
        public void Save()
        {
            try
            {
                //ищем совпадающий элемент
                var matchingItem = Collection.Where(f => f.IsDir == Model.IsDir && f.Name == Model.Name && Model.Guid != f.Guid).ToList();
                /*
                if (SelectedGroup != null)
                {
                    int id = ((Service)SelectedGroup).Id;
                    if (id == 0) Model.ParentId = null;
                    else Model.ParentId = id;
                }

                if (matchingItem.Count() > 0 && matchingItem.Any(f => f.ParentId == Model.ParentId))
                {
                    new TryingCreatingDuplicate().run(Model.IsDir);
                    return;
                }

                if (Model.Id == 0) Add(); else Update();
                db.SaveChanges();

                SelectedGroup = null;
                Window.Close();*/
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
      
        [Command]
        public void CancelForm() => Window.Close();


        /******************************************************/
        public ObservableCollection<Service> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }


        public Service Model { get; set; }
        public string Title { get; set; }
        public Visibility IsVisibleItemForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupForm { get; set; } = Visibility.Hidden;

        private ClassificatorWindow Window;

        private ObservableCollection<Service> GetCollection() => db.Services.OrderBy(d => d.Name).ToObservableCollection();

        private void CreateNewWindow() => Window = new ClassificatorWindow();
        private Service CreateNewModel() => new Service();

        private Service GetModelById(int id) => Collection.Where(f => f.Id == id).FirstOrDefault();

        private void Add()
        {
            db.Entry(Model).State = EntityState.Added;
            if (db.SaveChanges() > 0) Collection.Add(Model);
        }
        private void Update()
        {
            db.Entry(Model).State = EntityState.Modified;
            if (db.SaveChanges() > 0) Model.UpdateFields();

        }

        private void Delete(ObservableCollection<Service> collection)
        {
            collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
            collection.ForEach(f => Collection.Remove(f));
        }
    }
}
