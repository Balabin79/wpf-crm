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

namespace Dental.ViewModels.Templates
{
    class MainViewModel : ViewModelBase
    {
        public bool CanOpenDiagnoses(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenDiaries(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenAllergies(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenComplaints(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenPlans(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenObjectively(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenDescriptionXRay(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;
        public bool CanOpenAnamneses(object p) => ((UserSession)Application.Current.Resources["UserSession"]).TemplatesRead;

        [Command]
        public void OpenDiagnoses(object p)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    new TemplatesWin()
                    {
                        DataContext = new TreeBaseViewModel<Diagnos>(db, db?.Diagnoses),
                        TitleWin = "Шаблоны \"Диагнозы\""
                    }?.ShowDialog();
                }
            } 
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Диагнозы\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenDiaries(object p)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    new TemplatesWin()
                    {
                        TitleWin = "Шаблоны \"Лечение\"",
                        DataContext = new TreeBaseViewModel<Diary>(db, db?.Diaries)
                    }?.ShowDialog();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Лечение\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenAllergies(object p)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    new TemplatesWin()
                    {
                        TitleWin = "Шаблоны \"Аллергии\"",
                        DataContext = new TreeBaseViewModel<Allergy>(db, db?.Allergies)
                    }?.ShowDialog();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Аллергии\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenComplaints(object p)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    new TemplatesWin()
                    {
                        TitleWin = "Шаблоны \"Жалобы пациента\"",
                        DataContext = new TreeBaseViewModel<Complaint>(db, db?.Complaints)
                    }?.ShowDialog();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Жалобы пациента\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }

        }

        [Command]
        public void OpenPlans(object p)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    new TemplatesWin()
                    {
                        TitleWin = "Шаблоны \"Планы лечения\"",
                        DataContext = new TreeBaseViewModel<TreatmentPlan>(db, db?.TreatmentPlans)
                    }?.ShowDialog();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Планы лечения\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenObjectively(object p)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    new TemplatesWin()
                    {
                        TitleWin = "Шаблоны \"Объективное обследование\"",
                        DataContext = new TreeBaseViewModel<Objectively>(db, db?.Objectively)
                    }?.ShowDialog();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Объективное обследование\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenDescriptionXRay(object p)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    new TemplatesWin()
                    {
                        TitleWin = "Шаблоны \"Описание рентгеновских снимков\"",
                        DataContext = new TreeBaseViewModel<DescriptionXRay>(db, db?.DescriptionXRay)
                    }?.ShowDialog();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Описание рентгеновских снимков\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenAnamneses(object p)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    new TemplatesWin()
                    {
                        DataContext = new TreeBaseViewModel<Anamnes>(db, db?.Anamneses),
                        TitleWin = "Шаблоны \"Анамнезы\""
                    }?.ShowDialog();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Анамнезы\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
    }
}
