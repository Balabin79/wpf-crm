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
using Dental.Infrastructures.Extensions.Notifications;

namespace Dental.ViewModels
{
    class SubscribesViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public SubscribesViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);
            CancelFormCommand = new LambdaCommand(OnCancelFormCommandExecuted, CanCancelFormCommandExecute);
            SendCommand = new LambdaCommand(OnSendCommandExecuted, CanSendCommandExecute);

            SaveSmsCenterCommand = new LambdaCommand(OnSaveSmsCenterExecuted, CanSaveSmsCenterExecute);
            OpenFormSmsCenterCommand = new LambdaCommand(OnOpenFormSmsCenterExecuted, CanOpenFormSmsCenterExecute);
            CancelFormSmsCenterCommand = new LambdaCommand(OnCancelFormSmsCenterExecuted, CanCancelFormSmsCenterExecute);

            try
            {
                db = new ApplicationContext();
                Employees = db.Employes.OrderBy(f => f.LastName).ToArray();
                Clients = db.Clients.OrderBy(f => f.LastName).ToArray();

                RecipientType = true;

                    SetCollection();
                TypeSubscribeParams = db.SubscribeParams.Where(f => f.Id < 11).ToObservableCollection();                
                SmsCenter = GetSmsCenter();
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

        public ICommand SaveSmsCenterCommand { get; }
        public ICommand OpenFormSmsCenterCommand { get; }
        public ICommand CancelFormSmsCenterCommand { get; }


        public ICommand SendCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanOpenFormCommandExecute(object p) => true;
        private bool CanCancelFormCommandExecute(object p) => true;
        private bool CanSendCommandExecute(object p) => true;

        private bool CanSaveSmsCenterExecute(object p) => true;
        private bool CanOpenFormSmsCenterExecute(object p) => true;
        private bool CanCancelFormSmsCenterExecute(object p) => true;

        private void OnCancelFormSmsCenterExecuted(object p) => SmsCenterSettingsWindow.Close();
        private void OnOpenFormSmsCenterExecuted(object p)
        {
            try
            {
                SmsCenter = GetSmsCenter();
                SmsCenterSettingsWindow = new Views.Subscribes.ServiceSettingsWindow();
                SmsCenterSettingsWindow.DataContext = this;
                SmsCenterSettingsWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        private void OnSaveSmsCenterExecuted(object p)
        {
            try
            {
                if (SmsCenterSettingsWindow == null) return;
                if (SmsCenter?.Id == 0) db.Entry(SmsCenter).State = EntityState.Added;
                int cnt = db.SaveChanges();
                if (cnt > 0)
                {
                    ActionsLog.RegisterAction("Настройки SmsCenter", ActionsLog.ActionsRu["edit"], ActionsLog.SectionPage["SmsCenter"]);
                    var notification = new Notification();
                    notification.Content = "Изменения успешно записаны в базу данных!";
                    notification.run();
                }
                SmsCenterSettingsWindow.Close();
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
                        Title = "Новая позиция";
                        break;
                    default:
                        Model = GetModelById(param);
                        Title = "Редактировать позицию";
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
        private void OnSaveCommandExecuted(object p)
        {
            try
            {
                //ищем совпадающий элемент
                var matchingItem = Collection.Where(f => f.Name == Model.Name && Model.Guid != f.Guid).ToList();

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
                if (Model == null || !new ConfirDeleteInCollection().run(0)) return;

                Delete(new ObservableCollection<ClientsSubscribes>() { Model });
                ActionsLog.RegisterAction(Model.Name, ActionsLog.ActionsRu["delete"], ActionsLog.SectionPage["ClientsSubscribes"]);
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
                }
            }
            catch
            {

            }
        }

        public ObservableCollection<ClientsSubscribes> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        public ClientsSubscribes Model { get; set; }
        public string Title { get; set; }


        private ObservableCollection<ClientsSubscribes> _Collection;
        private ClientsSubscribesWindow Window;

        private void SetCollection()
        {
            Collection = db.ClientsSubscribes
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

        public object SelectedTypeSubscribe { get; set; }

        /******************************************************/
        public Views.Subscribes.ServiceSettingsWindow SmsCenterSettingsWindow { get; set; }
        public SmsCenter SmsCenter { get; set; }
        public SmsCenter GetSmsCenter() => db.SmsCenter?.FirstOrDefault() ?? new SmsCenter();

        public ICollection<Employee> Employees { get; set; }
        public ICollection<Client> Clients { get; set; }


        public ICollection<Client> SelectedRecipients { get; set; }


        public bool RecipientType {
            get => recipientType;
            set
            {
                recipientType = value;
                if (recipientType)
                {
                    VisibilityClients = Visibility.Visible;
                    VisibilityEmployees = Visibility.Collapsed;
                }
                else
                {
                    VisibilityClients = Visibility.Collapsed;
                    VisibilityEmployees = Visibility.Visible;
                }
            }
        }
        public bool recipientType;

        public Visibility VisibilityEmployees 
        { 
            get => visibilityEmployees;
            set => Set(ref visibilityEmployees, value);
        }
        public Visibility visibilityEmployees;

        public Visibility VisibilityClients 
        { 
            get => visibilityClients; 
            set => Set(ref visibilityClients, value); 
        }
        public Visibility visibilityClients;

    }
}
