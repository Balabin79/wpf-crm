using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Views.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Xpf.Grid;
using Dental.Services;
using Dental.Infrastructures.Collection;

namespace Dental.ViewModels
{
    class ClientGroupViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public ClientGroupViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);
            CancelFormCommand = new LambdaCommand(OnCancelFormCommandExecuted, CanCancelFormCommandExecute);

            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Группы клиентов\"!",
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
                if (Model == null || !new ConfirDeleteInCollection().run(null)) return;
                Model = GetModelById((int)p);
                Delete(new ObservableCollection<ClientsGroup>() { Model });
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
                if (Model.Id == 0) Add(); else Update();
                db.SaveChanges();
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
                        Title = "Новая группа клиентов";
                        break;                  
                    default:
                        Model = GetModelById(param);
                        Title = "Редактировать группу клиентов";
                        break;
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
        public ObservableCollection<ClientsGroup> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        public ClientsGroup Model { get; set; }
        public string Title { get; set; }

        private ObservableCollection<ClientsGroup> _Collection;
        private GroupsWindow Window;
        private ObservableCollection<ClientsGroup> GetCollection() => db.ClientsGroup.OrderBy(d => d.Name).ToObservableCollection();
        private void CreateNewWindow() => Window = new GroupsWindow();
        private ClientsGroup CreateNewModel() => new ClientsGroup();

        private ClientsGroup GetModelById(int id)
        {
            return Collection.Where(f => f.Id == id).FirstOrDefault();
        }

        private void Add()
        {
            Model.Guid = KeyGenerator.GetUniqueKey();
            db.Entry(Model).State = EntityState.Added;
            db.SaveChanges();
            Collection.Add(Model);
        }
        private void Update()
        {
            if (string.IsNullOrEmpty(Model.Guid)) KeyGenerator.GetUniqueKey();
            db.Entry(Model).State = EntityState.Modified;
            db.SaveChanges();
            var index = Collection.IndexOf(Model);
            if (index != -1) Collection[index] = Model;
        }

        private void Delete(ObservableCollection<ClientsGroup> collection)
        {

            collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
            collection.ForEach(f => Collection.Remove(f));
        }
    }
}
