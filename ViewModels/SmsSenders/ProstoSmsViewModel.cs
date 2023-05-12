﻿using B6CRM.Infrastructures.Converters;
using B6CRM.Infrastructures.Extensions.Notifications;
using B6CRM.Models;
using B6CRM.Reports;
using B6CRM.Services;
using B6CRM.Services.SmsServices;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using License;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            Employees = db.Employes.OrderBy(f => f.LastName).ToArray();
            SendingStatuses = db.SendingStatuses.ToArray();
            Channels = db.Channels?.Where(f => f.ProstoSms == 1)?.ToArray();
            ServicePass = db.ServicesPasses.FirstOrDefault(f => f.Name == "ProstoSms") ?? new ServicePass() { Name = "ProstoSms" };

            try
            {
                if (!string.IsNullOrEmpty(ServicePass.Pass)) 
                    ServicePass.PassDecr = Encoding.Unicode.GetString(Convert.FromBase64String(ServicePass.Pass));
            }
            catch 
            {
                ServicePass.PassDecr = ServicePass.Pass;
            }

            IsWaitIndicatorVisible = false;
        }

        #region Права на выполнение команд
        public bool CanAdd() => ((UserSession)Application.Current.Resources["UserSession"]).SmsEditable;
        public bool CanDelete(object p) => ((UserSession)Application.Current.Resources["UserSession"]).SmsDelitable;
        public bool CanSave() => ((UserSession)Application.Current.Resources["UserSession"]).SmsEditable;
        public bool CanSavePass() => ((UserSession)Application.Current.Resources["UserSession"]).SmsEditable;
        #endregion

        private void Load()
        {
            Sms = db.Sms
                ?.Include(f => f.ClientCategory)
                ?.Include(f => f.SendingStatus)
                ?.Include(f => f.Channel)
                ?.Include(f => f.SmsRecipients)
                ?.ToObservableCollection();

            // сбрасываем фильтр счетов в вкарте клиента на значение по умолчание
        }

        [Command]
        public void LoadRecipients(object p) 
        {
            try
            {

            }
            catch (Exception e) 
            { 

            }



           /* RecipientsList = clientCategory != null 
                ? 
                db.Clients?.
                Include(f => f.ClientCategory)?.
                Where(f => f.ClientCategoryId == clientCategory)?.
                OrderByDescending(f => f.LastName)?.
                Select(f => new ProstoSmsRecipients() { })?.
                ToObservableCollection()
                :
                db.Clients?.
                Include(f => f.ClientCategory)?.
                OrderByDescending(f => f.LastName)?.
                Select(f => new ProstoSmsRecipients() { })?.
                ToObservableCollection(); */           
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

        [Command]
        public void SavePass()
        {
            try
            {
                if (ServicePass.Id == 0 && (!string.IsNullOrEmpty(ServicePass.Pass) || !string.IsNullOrEmpty(ServicePass.Login)))
                {
                    db.ServicesPasses.Add(ServicePass);
                }

                if (!string.IsNullOrEmpty(ServicePass.PassDecr)) ServicePass.Pass = Convert.ToBase64String(Encoding.Unicode.GetBytes( ServicePass.PassDecr));

                if (db.SaveChanges() > 0) 
                {
                    new Notification() { Content = "Настройки \"ProstoSms\" сохранены в базу данных!" }.run();
                }
            }
            catch(Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке сохранить настройки \"ProstoSms\" в базу данных!", true);
            }
        }
        #endregion

        public ServicePass ServicePass
        {
            get { return GetProperty(() => ServicePass); }
            set { SetProperty(() => ServicePass, value); }
        }

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
    }
}
