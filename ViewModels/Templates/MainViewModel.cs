using Dental.Models;
using Dental.Models.Templates;
using Dental.Services;
using Dental.ViewModels.Base;
using Dental.Views.Templates;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm.Native;
using System.Data.Entity;
using System.Collections.ObjectModel;
using Dental.Views.Settings;

namespace Dental.ViewModels.Templates
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public MainViewModel() => db = new ApplicationContext();


        #region Права на выполнение команд
        public bool CanOpenDiagnoses(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenDiaries(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenAllergies(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenComplaints(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenPlans(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenObjectively(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenDescriptionXRay(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenAnamneses(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        #endregion 

        [Command]
        public void OpenDiagnoses(object p)
        {
            try
            {
                new TemplatesWin() { DataContext = new TreeBaseViewModel<Diagnos>(db, db?.Diagnoses), TitleWin = "Шаблоны \"Диагнозы\"" }?.ShowDialog();

            }
            catch
            {
                var response = ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Диагнозы\"! Проверьте настройки подключения к базе данных.",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                if (response.ToString() == "OK")
                    new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
            }
        }

        [Command]
        public void OpenDiaries(object p)
        {
            try
            {
                new TemplatesWin()
                {
                    TitleWin = "Шаблоны \"Лечение\"",
                    DataContext = new TreeBaseViewModel<Diary>(db, db?.Diaries)
                }?.ShowDialog();
            }
            catch
            {
                var response = ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Лечение\"! Проверьте настройки подключения к базе данных.",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                if (response.ToString() == "OK")
                    new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
            }
        }

        [Command]
        public void OpenAllergies(object p)
        {
            try
            {
                new TemplatesWin()
                {
                    TitleWin = "Шаблоны \"Аллергии\"",
                    DataContext = new TreeBaseViewModel<Allergy>(db, db?.Allergies)
                }?.ShowDialog();
            }
            catch
            {
                var response = ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Аллергии\"! Проверьте настройки подключения к базе данных.",
                 messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                if (response.ToString() == "OK")
                    new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
            }
        }

        [Command]
        public void OpenComplaints(object p)
        {
            try
            {
                new TemplatesWin()
                {
                    TitleWin = "Шаблоны \"Жалобы пациента\"",
                    DataContext = new TreeBaseViewModel<Complaint>(db, db?.Complaints)
                }?.ShowDialog();
            }
            catch
            {
                var response = ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Жалобы пациента\"! Проверьте настройки подключения к базе данных.",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                if (response.ToString() == "OK")
                    new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
            }

        }

        [Command]
        public void OpenPlans(object p)
        {
            try
            {
                new TemplatesWin()
                {
                    TitleWin = "Шаблоны \"Планы лечения\"",
                    DataContext = new TreeBaseViewModel<TreatmentPlan>(db, db?.TreatmentPlans)
                }?.ShowDialog();

            }
            catch
            {
                var response = ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Планы лечения\"! Проверьте настройки подключения к базе данных.",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                if (response.ToString() == "OK")
                    new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
            }
        }

        [Command]
        public void OpenObjectively(object p)
        {
            try
            {
                new TemplatesWin()
                {
                    TitleWin = "Шаблоны \"Объективное обследование\"",
                    DataContext = new TreeBaseViewModel<Objectively>(db, db?.Objectively)
                }?.ShowDialog();
            }
            catch
            {
                var response = ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Объективное обследование\"! Проверьте настройки подключения к базе данных.",
                     messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                if (response.ToString() == "OK")
                    new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
            }
        }

        [Command]
        public void OpenDescriptionXRay(object p)
        {
            try
            {
                new TemplatesWin()
                {
                    TitleWin = "Шаблоны \"Описание рентгеновских снимков\"",
                    DataContext = new TreeBaseViewModel<DescriptionXRay>(db, db?.DescriptionXRay)
                }?.ShowDialog();

            }
            catch
            {
                var response = ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Описание рентгеновских снимков\"! Проверьте настройки подключения к базе данных.",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                if (response.ToString() == "OK")
                    new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
            }
        }

        [Command]
        public void OpenAnamneses(object p)
        {
            try
            {
                new TemplatesWin()
                {
                    DataContext = new TreeBaseViewModel<Anamnes>(db, db?.Anamneses),
                    TitleWin = "Шаблоны \"Анамнезы\""
                }?.ShowDialog();

            }
            catch
            {
                var response = ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Анамнезы\"! Проверьте настройки подключения к базе данных.",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                if (response.ToString() == "OK")
                    new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
            }
        }
    }
}
