﻿using Dental.Infrastructures.Commands.Base;
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

namespace Dental.Services
{
    /*
     * * Alexander Balabin
     * Навигация по программе
     * - поддержка истории просмотренных страниц
     * - поддержка отслеживание изменений на странице
     * - сохранение последней посещенной страницы
     * 
     * 
     * - добавить различные варианты анимации при переходах
     */
    public sealed class Navigator : ViewModelBase
    {
        public Navigator()
        {
            CurrentPage = GetStartPage();
            FrameOpacity = 1.1;
            LeftMenuClick = new LambdaCommand(OnLeftMenuClickCommandExecuted, CanLeftMenuClickCommandExecute);
        }

        #region Общий ф-нал
        
        private void GoToPage(object p)
        {
            if (p is object[] arr) SlowOpacity(CreatePage(arr[0].ToString(), (int)arr[1]));
            else SlowOpacity(CreatePage(p.ToString()));
        }

        private Page CreatePage(string pageName, int param = -1)
        {
            Type type = Type.GetType(pageName);
            return (param == -1 || param == 0) ? (Page)Activator.CreateInstance(type) : (Page)Activator.CreateInstance(type, param);
        }

        private async void SlowOpacity(Page page)
        {
            await Task.Factory.StartNew(() => {
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

        #region Команды
        private void OnLeftMenuClickCommandExecuted(object p)
        {
            try
            {
                if (HasUnsavedChanges != null && UserSelectedBtnCancel != null)
                {
                    if (HasUnsavedChanges.Invoke() && UserSelectedBtnCancel.Invoke()) return;
                    HasUnsavedChanges = null; 
                    UserSelectedBtnCancel = null;
                }
                GoToPage(p);
            }
            catch 
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При переходе на другую страницу возникла ошибка! Данная страница отсутствует.", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
        #endregion


        private Page GetStartPage()
        {
            using (var db = new ApplicationContext())
            {
                try
                {
                    return CreatePage(defaultPage);
                }
                catch
                {
                    return CreatePage(defaultPage);
                }
            }
        }

        #region Объявление команд
        public ICommand LeftMenuClick { get; }
        private bool CanLeftMenuClickCommandExecute(object p) => true;
        #endregion

        #region Свойства
        public Page CurrentPage
        {
            get => currentPage;
            set => Set(ref currentPage, value);
        }
        private Page currentPage;

        // страница по умолчанию (стартовая страница, если не удалось подгрузить страницу из настроек)
        private readonly string defaultPage = "Dental.Views.ServicesPage";

        public Double FrameOpacity
        {
            get => frameOpacity;
            set => Set(ref frameOpacity, value);
        }
        private Double frameOpacity;

        public int? StartWithLastPage { get; set; }
        #endregion

        #region Делегаты
        public static Func<bool> HasUnsavedChanges { get; set; }
        public static Func<bool> UserSelectedBtnCancel { get; set; }
        #endregion
    }

}
