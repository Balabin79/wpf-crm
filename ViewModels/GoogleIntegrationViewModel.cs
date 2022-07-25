using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Services;
using DevExpress.Mvvm.DataAnnotations;
using Dental.ViewModels.GoogleIntegration;
using Dental.Views.Integration.Google;
using System.Collections.Generic;

namespace Dental.ViewModels
{
    public class GoogleIntegrationViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;

        public GoogleIntegrationViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Settings = db.Settings.FirstOrDefault() ?? new Settings();
                IsReadOnly = Settings.Id != 0;
                FieldsValues = Settings.GetFieldsValues();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Интеграции\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public Settings Settings
        {
            get { return GetProperty(() => Settings); }
            set { SetProperty(() => Settings, value); }
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public string FieldsValues
        {
            get { return GetProperty(() => FieldsValues); }
            set { SetProperty(() => FieldsValues, value); }
        }

        [Command]
        public void Editable() => IsReadOnly = !IsReadOnly;

        [Command]
        public void Save()
        {
            try
            {
                if (Settings?.Id == 0) db.Settings.Add(Settings);
                if (db.SaveChanges() > 0) new Notification() { Content = "Настройки интеграции сохранены!" }.run();
            }
            catch (Exception e)
            {
            }
        }

        [Command]
        public void Delete()
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить настройки интеграции Google!",messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                if (response.ToString() == "No") return;
                var model = db.Settings.FirstOrDefault();
                if (model == null) return;
                db.Settings.Remove(model);
                if (db.SaveChanges() > 0) new Notification() { Content = "Настройки интеграции удалены!" }.run();
                Settings = new Settings();

            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При удалении аккаунта Google произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }


    }
}