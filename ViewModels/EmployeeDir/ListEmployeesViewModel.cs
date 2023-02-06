﻿using System;
using System.Linq;
using System.Text.Json;
using Dental.Models;
using System.Data.Entity;
using System.Collections;
using System.Windows.Media.Imaging;
using System.IO;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Input;
using Dental.Services;
using System.Collections.Generic;
using Dental.Infrastructures.Logs;
using Dental.Views.EmployeeDir;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Views.Documents;
using Dental.Services.Files;
using Dental.Views.AdditionalFields;
using Dental.ViewModels.AdditionalFields;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Models.Base;
using Dental.Infrastructures.Extensions;
using Dental.Infrastructures.Converters;
using Dental.Views.Settings;
using License;
using Dental.Views.About;

namespace Dental.ViewModels.EmployeeDir
{
    public class ListEmployeesViewModel : DevExpress.Mvvm.ViewModelBase, IImageDeletable, IImageSave
    {
        private readonly ApplicationContext db;
        public ListEmployeesViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Config = new Config();
                SetCollection();
                foreach (var i in Collection)
                {
                    ImgLoading(i);
                    i.IsVisible = false;
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Список сотрудников\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Editable(object p)
        {
            if (p is Employee employee)
            {
                employee.IsVisible = !employee.IsVisible;
            }
        }

        [Command]
        public void ExpandAll(object p)
        {
            try
            {
                if (p is DevExpress.Xpf.Grid.CardView card)
                {
                    if (card.IsCardExpanded(0)) card.CollapseAllCards();
                    else card.ExpandAllCards();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void ShowArchive()
        {
            try
            {
                IsArchiveList = !IsArchiveList;
                SetCollection(IsArchiveList);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void Add() => Collection.Add(new Employee() { IsVisible = true });

        [Command]
        public void Save(object p)
        {
            try
            {
                if (Status.Licensed && Status.HardwareID != Status.License_HardwareID)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                if (!Status.Licensed && (Status.Evaluation_Time_Current > Status.Evaluation_Time))
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }

                if (p is Employee model)
                {
                    if (model.Id == 0) db.Entry(model).State = EntityState.Added;
                    if (db.SaveChanges() > 0) new Notification() { Content = "Записано в базу данных!" }.run();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При сохранении данных сотрудника возникла ошибка!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Delete(object p)
        {
            try
            {
                if (p is Employee model)
                {
                    if (model.Id == 0)
                    {
                        Collection.Remove(model);
                        return;
                    }

                    var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить анкету сотрудника из базы данных, без возможности восстановления? Также будут удалены связанные с сотрудником записи в расписании!",
                    messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                    if (response.ToString() == "No") return;

                    //удалить также в расписании и в счетах
                    db.Appointments.Where(f => f.EmployeeId == model.Id)?.ForEach(f => db.Entry(f).State = EntityState.Deleted);

                    db.Entry(model).State = EntityState.Deleted;
                    if (db.SaveChanges() > 0)
                    {
                        Collection.Remove(model);
                        new Notification() { Content = "Анкета сотрудника полностью удалена из базы данных!" }.run();
                    }

                    // удаляем фото 
                    if (Directory.Exists(Config.PathToEmployeesDirectory))
                    {
                        var photo = Directory.GetFiles(Config.PathToEmployeesDirectory).FirstOrDefault(f => f.Contains(model?.Guid));
                        if (photo != null && File.Exists(photo)) File.Delete(photo);
                    }
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При удалении анкеты сотрудника произошла ошибка, перейдите в раздел \"Сотрудники\"!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }


        #region Управление фото
        [Command]
        public void ImageSave(object p)
        {
            try
            {
                if (p is ImageEditEx param)
                {
                    if (!Directory.Exists(Config.PathToEmployeesDirectory)) Directory.CreateDirectory(Config.PathToEmployeesDirectory);

                    var oldPhoto = Directory.GetFiles(Config.PathToEmployeesDirectory).FirstOrDefault(f => f.Contains(param?.ImageGuid));

                    if (oldPhoto != null && File.Exists(oldPhoto))
                    {
                        var response = ThemedMessageBox.Show(title: "Вы уверены?", text: "Заменить текущее фото сотрудника?",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                        if (response.ToString() == "No") return;
                        File.SetAttributes(oldPhoto, FileAttributes.Normal);
                        File.Delete(oldPhoto);
                    }

                    FileInfo photo = new FileInfo(Path.Combine(param.ImagePath));
                    string fileFullName = Path.Combine(Config.PathToEmployeesDirectory, param.ImageGuid + photo.Extension);
                    photo.CopyTo(fileFullName, true);
                    File.SetAttributes(fileFullName, FileAttributes.Normal);
                    new Notification() { Content = "Фото сотрудника сохраненo!" }.run();
                }
            }
            catch (Exception e)
            {

            }
        }

        public void ImageDelete(object p)
        {
            try
            {
                if (p is ImageEditEx img)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить фото сотрудника?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                    if (response.ToString() == "No") return;

                    if (Directory.Exists(Config.PathToEmployeesDirectory))
                    {
                        var file = Directory.GetFiles(Config.PathToEmployeesDirectory).FirstOrDefault(f => f.Contains(img?.ImageGuid));

                        if (file != null) 
                        {
                            File.SetAttributes(file, FileAttributes.Normal);
                            File.Delete(file); 
                        }
                    }

                    img?.Clear();
                    new Notification() { Content = "Фото сотрудника удалено!" }.run();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion

        /***********************************/
        public ObservableCollection<Employee> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        public DocumentsWindow DocumentsWindow { get; set; }

        public bool IsArchiveList
        {
            get { return GetProperty(() => IsArchiveList); }
            set { SetProperty(() => IsArchiveList, value); }
        }

        public void SetCollection(bool isArhive = false) => Collection = db.Employes.OrderBy(d => d.LastName).Where(f => f.IsInArchive == isArhive).ToObservableCollection() ?? new ObservableCollection<Employee>();

        private void ImgLoading(Employee model)
        {
            try
            {
                if (Directory.Exists(Config.PathToEmployeesDirectory))
                {
                    var file = Directory.GetFiles(Config.PathToEmployeesDirectory)?.FirstOrDefault(f => f.Contains(model.Guid));
                    if (file == null) return;

                    using (var stream = new FileStream(file, FileMode.Open))
                    {
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.StreamSource = stream;
                        img.EndInit();
                        img.Freeze();
                        model.Image = img;
                    }
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }
    }
}
