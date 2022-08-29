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
        
        [Command]
        public void OpenDiagnoses(object p)
        {
            try
            {
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
                DiariesWin?.Show();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму шаблонов \"Дневники\"!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenInitialInspections(object p)
        {
            try
            {
                InitialInspectionsWin = new InitialInspectionsWin() { DataContext = new InitialInspectionViewModel() };
                InitialInspectionsWin?.Show();
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
                ComplaintsWin?.Show();
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
                TreatmentPlansWin?.Show();
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
                ObjectivelyWin?.Show();
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
                DescriptionXRaysWin?.Show();
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
                AnamnesesWin?.Show();
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
        public InitialInspectionsWin InitialInspectionsWin { get; set; }
        public DescriptionXRaysWin DescriptionXRaysWin { get; set; }
        public ComplaintsWin ComplaintsWin { get; set; }
        public TreatmentPlansWin TreatmentPlansWin { get; set; }
    }
}
