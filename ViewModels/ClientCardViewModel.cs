﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Views.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System.Windows;
using System.IO;
using System.Diagnostics;
using Dental.Services;
using Dental.Infrastructures.Extensions.Notifications;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Printing;
using Dental.Services.Files;

namespace Dental.ViewModels
{
    class ClientCardViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        private readonly PatientListViewModel VmList;

        public ClientCardViewModel(int clientId, PatientListViewModel vmList)
        {
            try
            {
                db = new ApplicationContext();
                VmList = vmList;
                Model = db.Clients.Where(f => f.Id == clientId).Include(f => f.Advertising).FirstOrDefault() ?? new Client();
                ClientInfoViewModel = new ClientInfoViewModel(Model);

                UserFiles = new UserFilesManagement(Model.Guid);
                Document = new ClientsDocumentsViewModel();    

                IsReadOnly = Model.Id != 0;

                AdvertisingList = db.Advertising.OrderBy(f => f.Name).ToList();
                Appointments = db.Appointments
                    .Include(f => f.Service).Include(f => f.Employee).Include(f => f.Location).Where(f => f.ClientInfoId == Model.Id).OrderBy(f => f.CreatedAt)
                    .ToArray();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с картой пациента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Editable() => IsReadOnly = !IsReadOnly;

        [Command]
        public void Save()
        {
            try
            {
                ClientInfoViewModel.Copy(Model);
                if (Model.Id == 0)
                {
                    db.Clients.Add(Model);
                    VmList?.Collection?.Add(Model);
                    db.SaveChanges();
                    new Notification() { Content = "Новый клиент успешно записан в базу данных!" }.run();
                }
                else
                {
                    if (db.SaveChanges() > 0)
                    {
                        VmList?.SetCollection();
                        new Notification() { Content = "Отредактированные данные клиента сохранены в базу данных!" }.run();
                    }
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void Delete()
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить карту клиента из базы данных, без возможности восстановления? Также будут удалены сметы, записи в расписании и все файлы прикрепленные к карте клиента!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;

                new UserFilesManagement(Model.Guid).DeleteDirectory();
                var id = Model?.Id;
                //удалить также в расписании
                db.Entry(Model).State = EntityState.Deleted;
                if (db.SaveChanges() > 0) new Notification() { Content = "Карта клиента полностью удалена из базы данных!" }.run();

                if (Application.Current.Resources["Router"] is Navigator nav) nav.LeftMenuClick("Dental.Views.PatientCard.PatientsList");
                VmList.ClientCardWin.Close();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При удалении карты клиента произошла ошибка, перейдите в раздел \"Клиенты\"!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
       
        public bool HasUnsavedChanges()
        {
            bool hasUnsavedChanges = false;
            //if (Model.FieldsChanges != null) Model.FieldsChanges = Client.CreateFieldsChanges();
           // if (!Model.Equals(ModelBeforeChanges)) hasUnsavedChanges = true;
            return hasUnsavedChanges;
        }

        public bool UserSelectedBtnCancel()
        {
            /* string warningMessage = "";     
             foreach (var tab in Model.FieldsChanges)
             {
                 if (tab.Value.Count == 0) continue;
                 string fieldNames = " ";
                 foreach (var field in tab.Value)
                 {
                     fieldNames += " \"" + field + "\",";
                 }
                  warningMessage = "\nВо вкладке \"" + tab.Key + "\", поля:" + fieldNames.Remove(fieldNames.Length - 1) + "\n";
             }

             var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения!" + warningMessage + "\nПродолжить без сохранения?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

             return response.ToString() == "No";*/
            return true;
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public Client Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
        }

        public ClientInfoViewModel ClientInfoViewModel
        {
            get { return GetProperty(() => ClientInfoViewModel); }
            set { SetProperty(() => ClientInfoViewModel, value); }
        }

        public UserFilesManagement UserFiles
        {
            get { return GetProperty(() => UserFiles); }
            set { SetProperty(() => UserFiles, value); }
        }

        public ClientsDocumentsViewModel Document
        {
            get { return GetProperty(() => Document); }
            set { SetProperty(() => Document, value); }
        }

        public ICollection<Advertising> AdvertisingList { get; set; }
        public ICollection<Appointments> Appointments { get; set; }

        public ICollection<string> GenderList{ get => _GenderList; }

        private readonly ICollection<string> _GenderList = new List<string> { "Мужчина", "Женщина" };
        
        protected void Update()
        {
            db.Entry(Model).State = EntityState.Modified;
            db.SaveChanges();
        }

        /**********************************************/
    }
}
