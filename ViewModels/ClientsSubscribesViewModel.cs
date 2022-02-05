using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
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
using System.Text.Json;

namespace Dental.ViewModels
{
    class ClientsSubscribesViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public ClientsSubscribesViewModel()
        {           
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);
            CancelFormCommand = new LambdaCommand(OnCancelFormCommandExecuted, CanCancelFormCommandExecute);          
            SendCommand = new LambdaCommand(OnSendCommandExecuted, CanSendCommandExecute);          

            ExpandTreeCommand = new LambdaCommand(OnExpandTreeCommandExecuted, CanExpandTreeCommandExecute);

            try
            {
                db = new ApplicationContext();
                SetCollection();
                ClientsGroups = db.ClientsGroup?.ToArray();
                TypeSubscribeParams = db.SubscribeParams.Where(f => f.Id < 11).ToObservableCollection();
                VoiceParams = db.SubscribeParams.Where(f => f.Id > 10).ToObservableCollection();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Настройки\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenFormCommand { get; }
        public ICommand CancelFormCommand { get; }
 
        public ICommand ExpandTreeCommand { get; }

        public ICommand SendCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanOpenFormCommandExecute(object p) => true;
        private bool CanCancelFormCommandExecute(object p) => true;
        private bool CanExpandTreeCommandExecute(object p) => true;
        private bool CanSendCommandExecute(object p) => true;

        private void OnExpandTreeCommandExecuted(object p)
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
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        private void OnCancelFormCommandExecuted(object p) => Window.Close();
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
                            .GetDirectories().OfType<ClientsSubscribes>().ToObservableCollection();
                        if (Group.Count > 0 && Model.ParentId != null && Model.IsDir == 1) Group.Add(WithoutCategory);
                        SelectedGroup = Collection.Where(f => f.Id == Model?.ParentId && f.Id != Model.Id).FirstOrDefault();
                        SelectedClientGroup = ClientsGroups.Where(f => f.Id == Model?.ClientGroupId).FirstOrDefault();                

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
                Window.ShowDialog();
                SelectedGroup = null;
                SelectedClientGroup = null;
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
                //ищем совпадающий элемент
                var matchingItem = Collection.Where(f => f.IsDir == Model.IsDir && f.Name == Model.Name && Model.Guid != f.Guid).ToList();

                if (SelectedGroup != null)
                {
                    int id = ((ClientsSubscribes)SelectedGroup).Id;
                    if (id == 0) Model.ParentId = null;
                    else Model.ParentId = id;
                }

                if (matchingItem.Count() > 0 && matchingItem.Any(f => f.ParentId == Model.ParentId))
                {
                    new TryingCreatingDuplicate().run(Model.IsDir);
                    return;
                }
                if (SelectedClientGroup != null) Model.ClientGroupId = ((ClientsGroup)SelectedClientGroup)?.Id;
                if (Model.Settings != null)
                {
                    try
                    {
                        Model.JsonSettings = JsonSerializer.Serialize(Model.Settings);
                    } 
                    catch
                    {
                        Model.JsonSettings = JsonSerializer.Serialize(new Services.Smsc.SmsSettings.Settings());
                    }                    
                }
                if (Model.Report != null)
                {
                    try
                    {
                        Model.JsonReport = JsonSerializer.Serialize(Model.Report);
                    }
                    catch (Exception e)
                    {
                        Model.JsonReport = JsonSerializer.Serialize(new Services.Smsc.SmsSettings.Report());
                    }
                }

                if (Model.Id == 0) Add(); else Update();
                db.SaveChanges();

                SelectedGroup = null;
                SelectedClientGroup = null;
                Window.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                if (p == null) return;
                Model = GetModelById((int)p);
                if (Model == null || !new ConfirDeleteInCollection().run(Model.IsDir)) return;

                if (Model.IsDir == 0) Delete(new ObservableCollection<ClientsSubscribes>() { Model });
                else 
                {
                    Delete(new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetItemChilds().OfType<ClientsSubscribes>().ToObservableCollection());
                    ActionsLog.RegisterAction(Model.Name, ActionsLog.ActionsRu["delete"], ActionsLog.SectionPage["ClientsSubscribes"]); 
                } 
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSendCommandExecuted(object p) 
        {
            try
            {
                var s = db.ClientsSubscribes?.FirstOrDefault(f => f.Id == ((int)p));
                if (s != null)
                {
                    if (string.IsNullOrEmpty(s.Content))
                    {
                        var res = ThemedMessageBox.Show(title: "Внимание", text: "Поле содержания сообщения не заполнено! Отправить пустое сообщение?",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                        if (res.ToString() == "No") return;
                    }

                    // если не заполнена категория клиентов
                    if (s.ClientGroupId == null)
                    {
                        var res = ThemedMessageBox.Show(title: "Внимание", text: "Не заполнено поле \"Категория клиентов\"! Сообщение будет отправлено всем клиентам, чья карта не находится в статусе \"В архиве\"!. Отправить всем клиентам?",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                        if (res.ToString() == "No") return;
                    }
                    // получаем коллекцию пользователей, относящаюся к целевой категории 
                    var clients = (s.ClientGroupId == null) ? db.PatientInfo.ToArray() : db.PatientInfo.Where(f => f.ClientCategoryId == s.ClientGroupId).ToArray();

                    //если список клиентов пуст
                    if (clients.Count() == 0)
                    {
                        var mes = "Список клиентов, предназначенных для данной рассылки пуст!";
                        if (s.ClientGroupId != null) mes = "Список клиентов входящих в указанную категорию пуст!";

                        ThemedMessageBox.Show(title: "Внимание", text: mes,
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                        return;
                    }


                    var msg = new MessageParse(s.Content, clients).Run();
                }
            }
            catch (Exception e)
            { }
        }

        /************* Специфика этой ViewModel ******************/
        private ObservableCollection<ClientsSubscribes> _Group;
        public ObservableCollection<ClientsSubscribes> Group
        {
            get => _Group;
            set => Set(ref _Group, value);
        }

        public ClientsSubscribes WithoutCategory { get; set; } = new ClientsSubscribes() { Id = 0, IsDir = null, ParentId = null, Name = "Без категории" };

        private object selectedGroup;
        public object SelectedGroup
        {
            get => selectedGroup;
            set => Set(ref selectedGroup, value);
        }

        private object selectedClientGroup;
        public object SelectedClientGroup
        {
            get => selectedClientGroup;
            set => Set(ref selectedClientGroup, value);
        }


        /******************************************************/
        public ObservableCollection<ClientsSubscribes> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        public ClientsSubscribes Model { get; set; }
        public string Title { get; set; }
        public Visibility IsVisibleItemForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupForm { get; set; } = Visibility.Hidden;


        private void VisibleItemForm()
        {
            IsVisibleItemForm = Visibility.Visible;
            IsVisibleGroupForm = Visibility.Hidden;
            Window.Width = 800;
            Window.Height = 930;
        }
        private void VisibleItemGroup()
        {
            IsVisibleItemForm = Visibility.Hidden;
            IsVisibleGroupForm = Visibility.Visible;
            Window.Width = 800;
            Window.Height = 280;
        }


        private ObservableCollection<ClientsSubscribes> _Collection;
        private ClientsSubscribesWindow Window;

        private void SetCollection() 
        {
            Collection = db.ClientsSubscribes
            .Include(f => f.ClientGroup)
            .Include(f => f.SubscribeParams)
            .OrderBy(d => d.DateSubscribe).ToObservableCollection();

            foreach (var i in Collection)
            {
                if (!string.IsNullOrEmpty(i.JsonSettings))
                {
                    try
                    {
                        i.Settings = JsonSerializer.Deserialize<Services.Smsc.SmsSettings.Settings>(i.JsonSettings);
                    } 
                    catch
                    {
                        i.Settings = new Services.Smsc.SmsSettings.Settings();
                    }
                    
                }
                else i.Settings = new Services.Smsc.SmsSettings.Settings();
                
                if (!string.IsNullOrEmpty(i.JsonReport))
                {
                    try
                    {
                        i.Report = JsonSerializer.Deserialize<Services.Smsc.SmsSettings.Report>(i.JsonReport);
                    }
                    catch
                    {
                        i.Report = new Services.Smsc.SmsSettings.Report();
                    }

                }
                else i.Report = new Services.Smsc.SmsSettings.Report();
            }
        } 
        public ICollection<ClientsGroup> ClientsGroups { get; }


        private void CreateNewWindow() => Window = new ClientsSubscribesWindow();
        private ClientsSubscribes CreateNewModel() => new ClientsSubscribes();

        private ClientsSubscribes GetModelById(int id) => Collection.Where(f => f.Id == id).FirstOrDefault();
        
        private void Add()
        {
            db.Entry(Model).State = EntityState.Added;
            ActionsLog.RegisterAction(Model.Name, ActionsLog.ActionsRu["add"], ActionsLog.SectionPage["ClientsSubscribes"]);
            Collection.Add(Model);
        }
        private void Update()
        {
            if (db.Entry(Model).State == EntityState.Modified)
            {
                ActionsLog.RegisterAction(Model.Name, ActionsLog.ActionsRu["edit"], ActionsLog.SectionPage["ClientsSubscribes"]);
                Model.UpdateFields();
            }              
        }

        private void Delete(ObservableCollection<ClientsSubscribes> collection)
        {
            collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
            collection.ForEach(f => Collection.Remove(f));
        }

        public ObservableCollection<SubscribeParams> TypeSubscribeParams { get; }

        public ObservableCollection<SubscribeParams> VoiceParams { get; }    

        public object SelectedVoice { get; set; }
        public object SelectedTypeSubscribe { get; set; }
    }
}
