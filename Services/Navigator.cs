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
using DevExpress.Xpf.Core;
using Dental.Views.PatientCard;
using DevExpress.Mvvm.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Data.SQLite;
using Dental.Views;
using Dental.ViewModels.Org;
using Dental.ViewModels.AdditionalFields;
using Dental.Views.Settings;

namespace Dental.Services
{

    public sealed class Navigator : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public Navigator()
        {
            new CheckDBConnection().Run();
            db = new ApplicationContext();
          //  CurrentPage = CreatePage(defaultPage);
            //FrameOpacity = 1.1;
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
                new CheckDBConnection().Run();
                List<string> forms = new List<string>();
                switch (CurrentPage.ToString())
                {
                    case "Dental.Views.Organization":
                        if (CurrentPage.Resources["vm"] is OrganizationViewModel org && CurrentPage.Resources["af"] is CommonValueViewModel comFields)
                        {
                            if (org.OrganizationVM.HasChanges())
                            {
                                forms.Add("форме \"Организации\"");
                            }

                            if (comFields.HasChanges())
                            {
                                forms.Add("форме \"Дополнительные значения\"");
                            }
                            if (forms.Count > 0)
                            {
                                var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения в " + String.Join(" и в ", forms) + ". Закрыть без сохранения?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                                if (response.ToString() == "No") return;
                            }
                        }
                        break;
                }
                await GoToPage(p);
            }
            catch (Exception e)
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
        public readonly string defaultPage = "Dental.Views.PatientCard.PatientsList";

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
                var path = Path.Combine(new Config().PathToProgramDirectory, "B6Dental.chm");
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
