﻿using B6CRM.Infrastructures.Converters;
using B6CRM.Infrastructures.Extensions.Notifications;
using B6CRM.Models;
using B6CRM.Reports;
using B6CRM.Services;
using B6CRM.Services.SmsServices;
using B6CRM.Services.SmsServices.ProstoSmsService.Response.BalanceMethod;
using B6CRM.Services.SmsServices.ProstoSmsService.Response.PushMsgMethod;
using B6CRM.Views.WindowForms;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Sql.MasterDetail;
using DevExpress.DataAccess.Native.Web;
using DevExpress.DataAccess.Sql;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DocumentFormat.OpenXml.Office2010.Excel;
using License;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace B6CRM.ViewModels.SmsSenders
{
    public class SmsViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public SmsViewModel(string serviceName)
        {
            db = new ApplicationContext();
            SetService(serviceName);

            Load();

            ClientCategories = db.ClientCategories.ToArray();
            Employees = db.Employes.OrderBy(f => f.LastName).ToArray();
            SendingStatuses = db.SendingStatuses.ToArray();

            ServicePassVM = new ServicePassViewModel(serviceName);

            IsReadOnly = true;

            IsWaitIndicatorVisible = false;

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
        #endregion

        private void Load(int? sendingStatus = null)
        {
            try
            {
                Sms = sendingStatus == null
                    ?
                    db.Sms
                        ?.Where(f => f.ServiceId == ServiceId)
                        ?.Include(f => f.ClientCategory)
                        ?.Include(f => f.SendingStatus)
                        ?.Include(f => f.Channel)
                        ?.Include(f => f.SmsRecipients)?.ThenInclude(f => f.Client)?.ThenInclude(f => f.ClientCategory)
                        ?.ToObservableCollection()
                    :
                    db.Sms
                        ?.Where(f => f.ServiceId == ServiceId && f.SendingStatus.Id == ((int)sendingStatus))
                        ?.Include(f => f.ClientCategory)
                        ?.Include(f => f.SendingStatus)
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
                    if (sms.Id == 0 || sms.SendingStatus?.Id != 2)
                    {
                        var status = db.SendingStatuses?.FirstOrDefault(f => f.Id == 2);

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
                var date = DateTime.Now;
                var model = new Sms
                {
                    Date = date.ToString(),
                    ServiceId = ServiceId
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
                        sms.SendingStatus = null;
                        sms.SmsRecipients = null;

                        db.SmsRecipients.Where(f => f.SmsId == sms.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                        db.Entry(sms).State = EntityState.Deleted;

                    }
                    else
                    {
                        db.Entry(sms).State = EntityState.Detached;
                    }
                    if (db.SaveChanges() > 0)
                    {
                        db.SmsRecipients.Where(f => f.SmsId == null).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
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

                    HttpResponseMessage result = await send.SendMsg(contacts: contacts, text: sms.Msg);

                    string json = await result.Content.ReadAsStringAsync();


                    var response = JsonConvert.DeserializeObject<PushMsg>(json);

                    if (response?.response?.msg?.err_code != 0) ShowError(response?.response?.msg?.text);
                    else
                    {
                        var msg = $"Всего отправлено: {response?.response?.data?.n_raw_sms ?? 0} шт.\n Израсходовано: {string.Format(response?.response?.data?.credits?.ToString(), "C2")}";
                        ShowSuccess(msg);

                        var smsSending = new SmsSendingDate
                        {
                            IDSms = response?.response?.data?.id,
                            Date = DateTime.Now.ToString(),
                            Sms = sms
                        };
                        db.SmsSendingDate.Add(smsSending);
                        db.SaveChanges();
                    }
                    //GetBalance();
                }
            }
            catch (Exception e)
            {

            }
        }

        [AsyncCommand]
        public async Task GetBalance()
        {
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
                text: message,
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
            if (string.IsNullOrEmpty(ServicePassVM?.ServicePass?.Login) || string.IsNullOrEmpty(ServicePassVM?.ServicePass?.Pass))
            {
                ThemedMessageBox.Show(title: "Внимание!", text: "Не заполнены логин или пароль к сервису \"" + ServiceName + "\"!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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

            return true;
        }

        private void SetService(string channelName)
        {
            ServiceName = channelName;
            try
            {
                switch (channelName)
                {
                    case "ProstoSms":
                        Channels = db.Channels?.Where(f => f.ProstoSms == 1)?.ToArray();
                        ServiceId = (int)SmsServices.ProstoSms;
                        CascadeRoutingList = db.CascadeRouting.Where(f => f.ProviderId == 1).ToList() ?? new List<CascadeRouting>();
                        break;

                    case "SmsCenter":
                        Channels = db.Channels?.Where(f => f.SmsCenter == 1)?.ToArray();
                        ServiceId = (int)SmsServices.SmsCenter;
                        break;
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке установки значений сервиса!", true);
            }
        }

        #endregion

        [Command]
        public void SetFilter(object p)
        {
            if (int.TryParse(p?.ToString(), out int result))
            {
                Load(result);
                IsShowSmsSenders = result;
            }
            else
            {
                Load();
                IsShowSmsSenders = null;
            }
        }

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

        public string GetClientContact(string channel, object p)
        {
            if (p is Client client) return channel == "Email" ? client.Email : client.Phone;
            return "";
        }

        public int MsgLength(string msg) => msg?.Length ?? 0;

        public ICollection<Employee> Employees
        {
            get { return GetProperty(() => Employees); }
            set { SetProperty(() => Employees, value); }
        }

        public ICollection<ClientCategory> ClientCategories
        {
            get { return GetProperty(() => ClientCategories); }
            set { SetProperty(() => ClientCategories, value); }
        }

        public ICollection<SendingStatus> SendingStatuses
        {
            get { return GetProperty(() => SendingStatuses); }
            set { SetProperty(() => SendingStatuses, value); }
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

        public bool IsWaitIndicatorVisible
        {
            get { return GetProperty(() => IsWaitIndicatorVisible); }
            set { SetProperty(() => IsWaitIndicatorVisible, value); }
        }

        public int? IsShowSmsSenders
        {
            get { return GetProperty(() => IsShowSmsSenders); }
            set { SetProperty(() => IsShowSmsSenders, value); }
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public string ServiceName
        {
            get { return GetProperty(() => ServiceName); }
            set { SetProperty(() => ServiceName, value); }
        }

        private int ServiceId;

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

    public enum SmsServices { ProstoSms = 1, SmsCenter = 2 };


}
