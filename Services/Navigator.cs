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
            GoToHistoryItem = new LambdaCommand(OnGoToHistoryItemExecuted, CanGoToHistoryItemExecute);
            GoToPreviousItem = new LambdaCommand(OnGoToPreviousItemExecuted, CanGoToPreviousItemExecute);
            GoToNextItem = new LambdaCommand(OnGoToNextItemExecuted, CanGoToNextItemExecute);

            EnableNextBtn = false;
            EnablePreviousBtn = false;
            EnableHistoryBtn = false;
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

        private void SetHistory(Links link, bool isNew)
        {
            EnableHistoryBtn = true;

            if (isNew == true)
            {
                // получаем последний эл-т в истории, если это та же самая страница, то просто меняем у нее дату
                if (BrowsingHistory.Count != 0 && string.Compare(CurrentPage?.ToString(), BrowsingHistory.Last().Path, StringComparison.CurrentCulture) == 0)
                {
                    link.Time = DateTime.Now.ToShortTimeString();
                    return;
                }
                else BrowsingHistory.Add(link);

                EnableNextBtn = false;
                if (BrowsingHistory.Count > 1) EnablePreviousBtn = true;
            }
            else
            {
                if (BrowsingHistory.Count > 1) // элементов в истории больше 1
                {
                    // если позиция больше 0, то отбражаем кнопку назад
                    int idx = BrowsingHistory.IndexOf(link);

                    // если позиция больше 0, то отображаем кнопку назад
                    if (idx > 0) EnablePreviousBtn = true; else EnablePreviousBtn = false;

                    // если позиция равна позиции последнего эл-та, от отключаем кнопку вперед, иначе включаем
                    if (idx == (BrowsingHistory.Count - 1)) EnableNextBtn = false; else EnableNextBtn = true;
                    CurrentLink = link;
                }
            }
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
                var link = new Links() { Path = CurrentPage?.ToString(), Name = CurrentPage?.Title };
                if (CurrentPage is IUser user) link.Id = user.UserId;

                GoToPage(p);
                SetHistory(link, true);
            }
            catch 
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При переходе на другую страницу возникла ошибка! Данная страница отсутствует.", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        // переходим по ссылке в истории
        private void OnGoToHistoryItemExecuted(object p)
        {
            try
            {
                if (p is Links link)
                {
                    if (string.IsNullOrEmpty(link?.Path)) return;
                    CurrentLink = link;
                    object[] param = new object[] {link.Path, link.Id};
                    GoToPage(param);
                    SetHistory(link, false);
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Переход по ссылке в истории невозможен! Данная страница отсутствует или была удалена.", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        private void OnGoToPreviousItemExecuted(object p)
        {
            try
            {
                if (CurrentLink == null) return;
                int idx = BrowsingHistory.IndexOf(CurrentLink);
                if (idx == -1 || idx == 0 || BrowsingHistory.Count == 1) return;

                var goToItem = BrowsingHistory[idx-1];
                CurrentLink = goToItem;
                GoToPage(goToItem.Path);
                SetHistory(goToItem, false);

            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Переход по ссылке в истории невозможен! Данная страница отсутствует или была удалена.", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        private void OnGoToNextItemExecuted(object p)
        {
            try
            {
                if (CurrentLink == null) return;
                int idx = BrowsingHistory.IndexOf(CurrentLink);
                if (idx == -1 || BrowsingHistory.Count == 1 || idx == BrowsingHistory.Count - 1) return;

                var goToItem = BrowsingHistory[idx + 1];
                CurrentLink = goToItem;
                GoToPage(goToItem.Path);
                SetHistory(goToItem, false);
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Переход по ссылке в истории невозможен! Данная страница отсутствует или была удалена.", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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
        public ICommand GoToHistoryItem { get; }
        public ICommand GoToPreviousItem { get; }
        public ICommand GoToNextItem { get; }

        private bool CanLeftMenuClickCommandExecute(object p) => true;
        private bool CanGoToHistoryItemExecute(object p) => true;
        private bool CanGoToPreviousItemExecute(object p) => true;
        private bool CanGoToNextItemExecute(object p) => true;
        #endregion

        #region Свойства
        public Page CurrentPage
        {
            get => currentPage;
            set => Set(ref currentPage, value);
        }
        private Page currentPage;

        // страница по умолчанию (стартовая страница, если не удалось подгрузить страницу из настроек)
        private readonly string defaultPage = "Dental.Views.DesktopPage";

        // коллекция просмотренных страниц
        public ObservableCollection<Links> BrowsingHistory { get; set; } = new ObservableCollection<Links>();

        public Double FrameOpacity
        {
            get => frameOpacity;
            set => Set(ref frameOpacity, value);
        }
        private Double frameOpacity;

        public int? StartWithLastPage { get; set; }

        public bool EnableNextBtn
        {
            get => enableNextBtn;
            set => Set(ref enableNextBtn, value);
        }
        private bool enableNextBtn;

        public bool EnablePreviousBtn
        {
            get => enablePreviousBtn;
            set => Set(ref enablePreviousBtn, value);
        }
        private bool enablePreviousBtn;

        public bool EnableHistoryBtn
        {
            get => enableHistoryBtn;
            set => Set(ref enableHistoryBtn, value);
        }
        private bool enableHistoryBtn;

        public Links CurrentLink
        {
            get => currentLink;
            set => Set(ref currentLink, value);
        }
        private Links currentLink;
        #endregion

        #region Делегаты
        public static Func<bool> HasUnsavedChanges { get; set; }
        public static Func<bool> UserSelectedBtnCancel { get; set; }
        #endregion
    }

    public class Links
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Time { get; set; } = DateTime.Now.ToShortTimeString();
        public string HistoryGuid { get; set; } = KeyGenerator.GetUniqueKey();
        public int Id { get; set; } = 0;
    }

}
