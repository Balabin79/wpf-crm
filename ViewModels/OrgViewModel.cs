using System;
using System.Collections.Generic;
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
using DevExpress.Mvvm.DataAnnotations;
using Dental.Services.Files;
using Dental.Services;

namespace Dental.ViewModels
{
    class OrgViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;

        public OrgViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Model = db.Org.FirstOrDefault() ?? new Org();
                OrgFormViewModel = new OrgFormViewModel(Model);
                UserFiles = new UserFilesManagement(Model.Guid);
                Document = new OrgDocumentsViewModel();

                IsReadOnly = Model.Id != 0;
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Организация\"!",
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
                OrgFormViewModel.Copy(Model);
                if (Model.Id == 0)
                {
                    db.Org.Add(Model);
                    db.SaveChanges();
                    new Notification() { Content = "Новый данные успешно записаны!" }.run();
                }
                else
                {
                    if (db.SaveChanges() > 0) new Notification() { Content = "Отредактированные данные организации сохранены!" }.run();
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
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить всю информацию о организации? Также будут удалены все прикрепленные к форме файлы!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;

                new UserFilesManagement(Model.Guid).DeleteDirectory();
                var id = Model?.Id;
                //удалить также в расписании
                db.Entry(Model).State = EntityState.Deleted;
                if (db.SaveChanges() > 0) new Notification() { Content = "Все данные организации удалены!" }.run();
                if (Application.Current.Resources["Router"] is Navigator nav) nav.LeftMenuClick("Dental.Views.OrgPage");
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При удалении данных организации произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }


        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public Org Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
        }

        public UserFilesManagement UserFiles
        {
            get { return GetProperty(() => UserFiles); }
            set { SetProperty(() => UserFiles, value); }
        }

        public OrgDocumentsViewModel Document
        {
            get { return GetProperty(() => Document); }
            set { SetProperty(() => Document, value); }
        }

        public OrgFormViewModel OrgFormViewModel
        {
            get { return GetProperty(() => OrgFormViewModel); }
            set { SetProperty(() => OrgFormViewModel, value); }
        }
    }
}
