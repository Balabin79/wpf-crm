using B6CRM.Infrastructures.Extensions.Notifications;
using B6CRM.Models;
using B6CRM.Services;
using B6CRM.Services.SmsServices;
using B6CRM.Services.SmsServices.ProstoSmsService.Response.BalanceMethod;
using B6CRM.Services.SmsServices.ProstoSmsService.Response.PushMsgMethod;
using B6CRM.Views.WindowForms;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using License;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace B6CRM.ViewModels.SmsSenders
{
    public class ProstoSmsViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public ProstoSmsViewModel()
        {
            db = new ApplicationContext();

            Load();

            ClientCategories = db.ClientCategories.ToArray();
            CascadeRoutingList = db.CascadeRouting.Where(f => f.ProviderId == 1).ToList() ?? new List<CascadeRouting>();

            ServicePassVM = new ServicePassViewModel("ProstoSms");

            IsReadOnly = true;

            GetBalance();
        }

        #region Права на выполнение команд
        public bool CanAdd() => ((UserSession)Application.Current.Resources["UserSession"]).SmsEditable;
        public bool CanDelete(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SmsDelitable;
        public bool CanSave() => ((UserSession)Application.Current.Resources["UserSession"]).SmsEditable;
        public bool CanSavePass() => ((UserSession)Application.Current.Resources["UserSession"]).SmsEditable;

        public bool CanLoadRecipients(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SmsEditable;
        public bool CanEditable() => ((UserSession)Application.Current.Resources["UserSession"]).SmsEditable;
        public bool CanSending(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SmsSending;
        public bool CanResending(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SmsSending;
        public bool CanDeleteClientFromRecipientsList(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SmsEditable;

        public bool CanOpenCascadeRoutingForm() => ((UserSession)Application.Current.Resources["UserSession"]).SmsEditable;
        public bool CanInfo(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SmsSending;
        #endregion

        private void Load()
        {
            try
            {
                Sms = db.Sms
                        ?.Where(f => f.ServiceId == (int)SmsServices.ProstoSms)
                        ?.Include(f => f.ClientCategory)
                        ?.Include(f => f.Channel)
                        ?.Include(f => f.SmsRecipients)?.ThenInclude(f => f.Client)?.ThenInclude(f => f.ClientCategory)
                        ?.ToObservableCollection();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Загрузка списка получателей sms!", true);
            }
            // сбрасываем фильтр счетов в вкарте клиента на значение по умолчание
        }

        [Command]
        public void LoadRecipients(object p)
        {
            try
            {
                if (p is Sms sms)
                {
                    sms.SmsRecipients = sms.ClientCategory?.Id != null
                        ?
                        db.Clients?.Where(f => f.ClientCategoryId == sms.ClientCategory.Id)
                        ?.Select(f => new SmsRecipient() { Client = f, Sms = sms })
                        ?.ToObservableCollection()
                        :
                        db.Clients
                        ?.Select(f => new SmsRecipient() { Client = f, Sms = sms })
                        ?.ToObservableCollection();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при загрузке списка получателей sms!", true);
            }
        }

        #region Рассылки
        [Command]
        public void Add()
        {
            try
            {
                var model = new Sms
                {
                    Name = "Новая рассылка",
                    ServiceId = (int)SmsServices.ProstoSms
                };

                db.Sms.Add(model);
                if (db.SaveChanges() > 0)
                {
                    Load();
                    if (db.SaveChanges() > 0) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void Save()
        {
            try
            {
                #region Lic
                if (Status.Licensed && Status.HardwareID != Status.License_HardwareID)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                if (!Status.Licensed && Status.Evaluation_Time_Current > Status.Evaluation_Time)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                #endregion

                if (db.SaveChanges() > 0)
                {
                    db.SmsRecipients.Where(f => f.SmsId == null).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                    db.SmsSendingDate.Where(f => f.SmsId == null).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                    db.SaveChanges();
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке сохранить рассылку в базу данных!", true);
            }
        }

        [Command]
        public void Delete(object p)
        {
            try
            {
                if (p is Sms sms)
                {
                    if (sms.Id > 0)
                    {
                        var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить рассылку?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                        if (response.ToString() == "No") return;

                        sms.Channel = null;
                        sms.ClientCategory = null;
                        sms.SmsRecipients = null;
                        sms.SmsSendingDate = null;

                        db.SmsRecipients.Where(f => f.SmsId == sms.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);

                        db.SmsSendingDate.Where(f => f.SmsId == sms.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);

                        db.Entry(sms).State = EntityState.Deleted;
                    }
                    else
                    {
                        db.Entry(sms).State = EntityState.Detached;
                    }
                    if (db.SaveChanges() > 0)
                    {
                        db.SmsRecipients.Where(f => f.SmsId == null).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                        db.SmsSendingDate.Where(f => f.SmsId == null).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                        db.SaveChanges();
                        new Notification() { Content = "Рассылка удалена из базы данных!" }.run();
                    }
                    Load();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке удалить рассылку из базы данных!", true);
            }
        }

        [AsyncCommand]
        public async Task Sending(object p)
        {
            try
            {
                if (p is Sms sms)
                {
                    var contacts = sms.Channel?.Name == "Email" ? GetEmails(sms) : GetPhones(sms);

                    if (!BeforeSendChecking(sms, contacts)) return;

                    var send = new ProstoSms(servicePassVm: ServicePassVM);
                    HttpResponseMessage result = await send.SendMsg(contacts: contacts, sms: sms);
                    string json = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<PushMsg>(json);

                    if (response?.response?.msg?.err_code != 0) ShowError(response?.response?.msg?.text);
                    else
                    {
                        var msg = $"Всего отправлено: {response?.response?.data?.n_raw_sms ?? 0} шт.\n" +
                            string.Format("Израсходовано: {0:C2}", response?.response?.data?.credits);
                        ShowSuccess(msg);

                        var smsSending = new SmsSendingDate
                        {
                            IDSms = response?.response?.data?.id,
                            Date = sms?.Date ?? DateTime.Now.ToString(),
                            Sms = sms
                        };
                        db.SmsSendingDate.Add(smsSending);
                        db.SaveChanges();
                    }
                    GetBalance();
                }
            }
            catch (Exception e)
            {

            }
        }

        [AsyncCommand]
        public async Task GetBalance()
        {
            if (!PingService.IsNetworkAvailable())
            {
                Balance = 0;
                CreditUsed = 0;
                return;
            }
            var send = new ProstoSms(servicePassVm: ServicePassVM);

            HttpResponseMessage result = await send.GetAccountBalance();
            string json = await result.Content.ReadAsStringAsync();

            var balance = JsonConvert.DeserializeObject<Balance>(json);

            Balance = balance?.response?.data?.credits ?? 0;
            CreditUsed = balance?.response?.data?.credits_used ?? 0;

            if (balance?.response?.msg?.err_code != 0) ShowError(balance?.response?.msg?.text);
        }

        private void ShowError(string message)
        {
            ThemedMessageBox.Show(
                title: "Ошибка",
                text: message ?? "Ошибка при выполнении запроса к сервису!",
                messageBoxButtons: MessageBoxButton.OK,
                icon: MessageBoxImage.Error
                );
        }

        private void ShowSuccess(string message)
        {
            ThemedMessageBox.Show(
                title: "Успешно отправлено",
                text: message,
                messageBoxButtons: MessageBoxButton.OK,
                icon: MessageBoxImage.Information
                );
        }

        private string GetPhones(Sms sms)
        {
            var list = new List<string>();

            foreach (var i in sms.SmsRecipients?.ToArray())
            {
                if (!string.IsNullOrEmpty(i.Client?.Phone))
                    list.Add(i.Client?.Phone);
            }
            if (list.Count() > 0) return string.Join(",", list);
            return "";
        }

        private string GetEmails(Sms sms)
        {
            var list = new List<string>();

            foreach (var i in sms.SmsRecipients?.ToArray())
            {
                if (!string.IsNullOrEmpty(i.Client?.Email))
                    list.Add(i.Client?.Email);
            }
            if (list.Count() > 0) return string.Join(",", list);
            return "";
        }

        private bool BeforeSendChecking(Sms sms, string contacts)
        {
            if (!PingService.IsNetworkAvailable())
            {
                ThemedMessageBox.Show(title: "Внимание!", text: "Отсутствует подключение к интернету!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(ServicePassVM?.ServicePass?.Login) || string.IsNullOrEmpty(ServicePassVM?.ServicePass?.Pass))
            {
                ThemedMessageBox.Show(title: "Внимание!", text: "Не заполнены логин или пароль к сервису \"ProstoSms\"!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                return false;
            }

            if (sms.SmsRecipients?.Count() < 1)
            {
                ThemedMessageBox.Show(title: "Внимание!", text: "Cписок получателей для отправки пуст!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(sms.Msg))
            {
                ThemedMessageBox.Show(title: "Внимание!", text: "Попытка отправить пустое сообщение!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(contacts))
            {
                ThemedMessageBox.Show(title: "Внимание!", text: "Список контактов получателей сообщения пуст!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(sms?.Date) && DateTime.TryParse(sms?.Date, out DateTime date2) && date2 < DateTime.Now)
            {
                ThemedMessageBox.Show(title: "Внимание!", text: "Дата отложенной доставки не может быть меньше текущей даты!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        #endregion

        [Command]
        public void DeleteClientFromRecipientsList(object p)
        {
            if (p is SmsRecipient smsRecipient)
            {
                try
                {
                    var list = smsRecipient.Sms?.SmsRecipients?.ToObservableCollection();
                    list?.Remove(smsRecipient);
                    smsRecipient.Sms.SmsRecipients = list;
                }
                catch
                {

                }
            }
        }

        [Command]
        public void Editable() => IsReadOnly = !IsReadOnly;

        [Command]
        public void OpenCascadeRoutingForm() => new CascadeRoutingWindow() { DataContext = this }.Show();


        [Command]
        public void SaveCascadeRouting()
        {
            try
            {
                if (db.SaveChanges() > 0) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
            }
            catch
            {

            }
        }

        [Command]
        public void Info(object p)
        {
            try
            {

            }
            catch
            {

            }
        }

        public string GetClientContact(string channel, object p)
        {
            if (p is Client client) return channel == "Email" ? client.Email : client.Phone;
            return "";
        }

        public int MsgLength(string msg) => msg?.Length ?? 0;

        public ICollection<ClientCategory> ClientCategories
        {
            get { return GetProperty(() => ClientCategories); }
            set { SetProperty(() => ClientCategories, value); }
        }

        public ICollection<Channel> Channels
        {
            get { return GetProperty(() => Channels); }
            set { SetProperty(() => Channels, value); }
        }

        public ObservableCollection<ProstoSmsRecipients> RecipientsList
        {
            get { return GetProperty(() => RecipientsList); }
            set { SetProperty(() => RecipientsList, value); }
        }

        // актуально для ProstoSms
        public ICollection<CascadeRouting> CascadeRoutingList
        {
            get { return GetProperty(() => CascadeRoutingList); }
            set { SetProperty(() => CascadeRoutingList, value); }
        }

        public ObservableCollection<Sms> Sms
        {
            get { return GetProperty(() => Sms); }
            set { SetProperty(() => Sms, value); }
        }

        public object SelectedItem
        {
            get { return GetProperty(() => SelectedItem); }
            set { SetProperty(() => SelectedItem, value); }
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public ServicePassViewModel ServicePassVM
        {
            get { return GetProperty(() => ServicePassVM); }
            set { SetProperty(() => ServicePassVM, value); }
        }

        public Decimal Balance
        {
            get { return GetProperty(() => Balance); }
            set { SetProperty(() => Balance, value); }
        }

        public Decimal CreditUsed
        {
            get { return GetProperty(() => CreditUsed); }
            set { SetProperty(() => CreditUsed, value); }
        }
    }
}
