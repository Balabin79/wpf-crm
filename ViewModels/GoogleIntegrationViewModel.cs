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

namespace Dental.ViewModels
{
    class GoogleIntegrationViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;

        public GoogleIntegrationViewModel()
        {
            try
            {
                db = new ApplicationContext();
                GoogleContacts = db.GoogleContacts.ToObservableCollection();
                Settings = db.Settings.FirstOrDefault() ?? new Settings();
                IsEnabled = !string.IsNullOrEmpty(Settings.Key) || !string.IsNullOrEmpty(Settings.Value);
                    SetGoogleAccountViewModel();

                GoogleContactViewModel = new GoogleContactViewModel();
            }
            catch(Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Интеграции\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        #region Команды для работы с аккаунтом Google
        [Command]
        public void OpenGoogleAccountForm()
        {
            SetGoogleAccountViewModel();
            GoogleAccountWindow = new GoogleAccountWindow() { DataContext = this };
            GoogleAccountWindow.Show();
        }

        [Command]
        public void CloseGoogleAccountForm() => GoogleAccountWindow?.Close();

        [Command]
        public void GoogleAccountSave()
        {
            try
            {
                if (Settings?.Key != GoogleAccountViewModel.Key || Settings?.Value != GoogleAccountViewModel.Value)
                {
                    Settings.Key = GoogleAccountViewModel.Key;
                    Settings.Value = GoogleAccountViewModel.Value;
                    if (Settings?.Id == 0) db.Settings.Add(Settings);
                }
                if (db.SaveChanges() > 0) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                IsEnabled = true;
                GoogleAccountWindow?.Close();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке сохранить данные в бд возникла ошибка!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void GoogleAccountDelete()
        {
            try
            {
                if(Settings?.Id > 0) db.Settings.Remove(Settings);
                if (db.SaveChanges() > 0) 
                { 
                    new Notification() { Content = "Данные удалены!" }.run();
                    IsEnabled = false;
                    Settings = new Settings();
                    GoogleAccountViewModel = new GoogleAccountViewModel();
                    GoogleAccountWindow?.Close();
                }

            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке удалить данные из бд!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
        #endregion

        public ObservableCollection<GoogleContacts> GoogleContacts
        {
            get { return GetProperty(() => GoogleContacts); }
            set { SetProperty(() => GoogleContacts, value); }
        }

        #region Аккаунт Google 
        public GoogleAccountViewModel GoogleAccountViewModel
        {
            get { return GetProperty(() => GoogleAccountViewModel); }
            set { SetProperty(() => GoogleAccountViewModel, value); }
        }

        public Settings Settings
        {
            get { return GetProperty(() => Settings); }
            set { SetProperty(() => Settings, value); }
        }

        public bool IsEnabled
        {
            get { return GetProperty(() => IsEnabled); }
            set { SetProperty(() => IsEnabled, value); }
        }

        public GoogleAccountWindow GoogleAccountWindow { get; set; }


        public GoogleContactViewModel GoogleContactViewModel
        {
            get { return GetProperty(() => GoogleContactViewModel); }
            set { SetProperty(() => GoogleContactViewModel, value); }
        }


        public void SetGoogleAccountViewModel() => GoogleAccountViewModel = (Settings != null) ? (GoogleAccountViewModel)Settings.Copy(new GoogleAccountViewModel()) : new GoogleAccountViewModel();

        #endregion
    }
}