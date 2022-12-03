using Dental.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Data.Entity;
using Dental.Models;
using System.Collections.ObjectModel;
using Dental.Views;
using DevExpress.Xpf.Core;
using Dental.Views.PatientCard;
using DevExpress.Mvvm.DataAnnotations;
using System.Diagnostics;
using System.IO;

namespace Dental.Services
{

    public sealed class Navigator : DevExpress.Mvvm.ViewModelBase
    {
        public Navigator()
        {
            CurrentPage = CreatePage(defaultPage);
            FrameOpacity = 1.1;

            try
            {
                using(var db = new ApplicationContext())
                {
                    RoleEnabled = db.Settings.FirstOrDefault()?.RolesEnabled == 1;
                }
            }
            catch (Exception e)
            {
                RoleEnabled = false;
                ThemedMessageBox.Show(title: "Ошибка", text: e.Message,
        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }         
        }

        #region Общий ф-нал
        private async Task GoToPage(object p)
        {
            if (p is object[] arr) CurrentPage = CreatePage(arr[0].ToString(), (int)arr[1]);
            else CurrentPage = CreatePage(p.ToString());                           
        }

        private Page CreatePage(string pageName, int param = -1)
        {
            IsSelected = pageName;
            Type type = Type.GetType(pageName);
            return (param > 0) ? (Page)Activator.CreateInstance(type, param) : (Page)Activator.CreateInstance(type);
        }

        private async Task SlowOpacity(Page page)
        {
            await Task.Factory.StartNew(() =>
            {
                for (double i = 1.0; i > 0.0; i -= 0.1)
                {
                    FrameOpacity = i;
                    Thread.Sleep(20);
                }

                CurrentPage = page;
                for (double i = 0.0; i < 1.1; i += 0.1)
                {
                    FrameOpacity = i;
                    Thread.Sleep(20);
                }
            });
        }
        #endregion

        [Command]
        public async Task LeftMenuClick(object p)
        {
            try
            {
                if (HasUnsavedChanges != null && UserSelectedBtnCancel != null)
                {
                    if (HasUnsavedChanges.Invoke() && UserSelectedBtnCancel.Invoke()) return;
                    HasUnsavedChanges = null;
                    UserSelectedBtnCancel = null;
                }
                await GoToPage(p);
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При переходе на другую страницу возникла ошибка! Данная страница отсутствует.", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        #region Свойства
        public bool? RoleEnabled
        {
            get { return GetProperty(() => RoleEnabled); }
            set { SetProperty(() => RoleEnabled, value); }
        }

        public Page CurrentPage
        {
            get { return GetProperty(() => CurrentPage); }
            set { SetProperty(() => CurrentPage, value); }
        }

        public Double FrameOpacity
        {
            get { return GetProperty(() => FrameOpacity); }
            set { SetProperty(() => FrameOpacity, value); }
        }

        public string IsSelected
        {
            get { return GetProperty(() => IsSelected); }
            set { SetProperty(() => IsSelected, value); }
        }

        // страница по умолчанию (стартовая страница, если не удалось подгрузить страницу из настроек)
        private readonly string defaultPage = "Dental.Views.PatientCard.PatientsList";

        public int? StartWithLastPage { get; set; }

        #endregion

        #region Делегаты
        public static Func<bool> HasUnsavedChanges { get; set; }
        public static Func<bool> UserSelectedBtnCancel { get; set; }
        #endregion


        [Command]
        public void Help()
        {
            try
            {
                var path = Path.Combine(Config.PathToProgramDirectory, "B6Dental.chm");
                if (!File.Exists(path)) 
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Не найден файл справки!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    return; 
                }
                Process.Start(path);
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть файл справки!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
    }
}
