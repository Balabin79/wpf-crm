using Dental.Services;
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
                Window wnd = Application.Current.Windows.OfType<Window>().Where(w => w.ToString() == DiagnosesWin?.ToString()).FirstOrDefault();
                if (wnd != null)
                {
                    wnd.Activate();
                    return;
                }
                DiagnosesWin = new DiagnosesWin() { DataContext = new DiagnosViewModel() };
                DiagnosesWin?.Show();
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
                DiariesWin = new DiariesWin() { DataContext = new DiaryViewModel() };
                DiariesWin?.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Дневники\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenAllergies(object p)
        {
            try
            {
                AllergiesWin = new AllergiesWin() { DataContext = new AllergyViewModel() };
                AllergiesWin?.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Первичный осмотр\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenComplaints(object p)
        {
            try
            {
                ComplaintsWin = new ComplaintsWin() { DataContext = new ComplaintViewModel() };
                ComplaintsWin?.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Жалобы\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }

        }

        [Command]
        public void OpenPlans(object p)
        {
            try
            {
                TreatmentPlansWin = new TreatmentPlansWin() { DataContext = new TreatmentPlanViewModel() };
                TreatmentPlansWin?.ShowDialog();
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
                ObjectivelyWin = new ObjectivelyWin() { DataContext = new ObjectivelyViewModel() };
                ObjectivelyWin?.ShowDialog();
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
                DescriptionXRaysWin = new DescriptionXRaysWin() { DataContext = new DescriptionXRayViewModel() };
                DescriptionXRaysWin?.ShowDialog();
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
                AnamnesesWin = new AnamnesesWin() { DataContext = new AnamnesViewModel() };
                AnamnesesWin?.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Анамнезы\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public DiagnosesWin DiagnosesWin { get; set; }
        public DiariesWin DiariesWin { get; set; }
        public AnamnesesWin AnamnesesWin { get; set; }
        public ObjectivelyWin ObjectivelyWin { get; set; }
        public AllergiesWin AllergiesWin { get; set; }
        public DescriptionXRaysWin DescriptionXRaysWin { get; set; }
        public ComplaintsWin ComplaintsWin { get; set; }
        public TreatmentPlansWin TreatmentPlansWin { get; set; }
    }
}
