using Dental.Infrastructures.Converters;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Models.Base;
using Dental.Views.PatientCard;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.ViewModels.ClientDir
{
    public class TreatmentStageViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public TreatmentStageViewModel(Client client = null, ApplicationContext context = null)
        {
            try
            {
                db = context ?? new ApplicationContext();
                Client = client;
                SetCollection();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с вкладкой \"Врачебная\" в карте клиента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenForm(object p)
        {
            if (p is TreatmentFormParameters parameters)
            {
                if (parameters.Table.FocusedRow is TreatmentStage model)
                {
                    Templates = new List<TreeTemplate>();
                    TemplateName = parameters.Name;
                    switch (parameters.Name)
                    {
                        case "Complaint":
                            Templates = db.Complaints.Select(f => new TreeTemplate()
                            {
                                Id = f.Id,
                                IsDir = f.IsDir,
                                ParentId = f.ParentId,
                                Name = f.Name
                            }).ToArray(); break;
                        case "Anamnes" :
                            Templates = db.Anamneses.Select(f => new TreeTemplate()
                                {
                                    Id = f.Id,
                                    IsDir = f.IsDir,
                                    ParentId = f.ParentId,
                                    Name = f.Name
                                }).ToArray(); break;
                            case "Objectivly" :
                            Templates = db.Objectively.Select(f => new TreeTemplate()
                                {
                                    Id = f.Id,
                                    IsDir = f.IsDir,
                                    ParentId = f.ParentId,
                                    Name = f.Name
                                }).ToArray(); break;
                            case "DescriptionXRay" :
                            Templates = db.DescriptionXRay.Select(f => new TreeTemplate()
                                {
                                    Id = f.Id,
                                    IsDir = f.IsDir,
                                    ParentId = f.ParentId,
                                    Name = f.Name
                                }).ToArray(); break;
                            case "Diagnos" :
                            Templates = db.Diagnoses.Select(f => new TreeTemplate()
                                {
                                    Id = f.Id,
                                    IsDir = f.IsDir,
                                    ParentId = f.ParentId,
                                    Name = f.Name
                                }).ToArray(); break;
                            case "Plan" :
                            Templates = db.TreatmentPlans.Select(f => new TreeTemplate()
                                {
                                    Id = f.Id,
                                    IsDir = f.IsDir,
                                    ParentId = f.ParentId,
                                    Name = f.Name
                                }).ToArray(); break;
                            case "Treatment" :
                            Templates = db.Diaries.Select(f => new TreeTemplate() // override on Treatment
                                {
                                    Id = f.Id,
                                    IsDir = f.IsDir,
                                    ParentId = f.ParentId,
                                    Name = f.Name
                                }).ToArray(); break;
                            case "Allergy" :
                            Templates = db.Allergies.Select(f => new TreeTemplate()
                            {
                                Id = f.Id,
                                IsDir = f.IsDir,
                                ParentId = f.ParentId,
                                Name = f.Name
                            }).ToArray(); break;
                    }
                    Model = model;
                    TemplateWin = new SelectValueInTemplateWin() { DataContext = this };
                    TemplateWin.Show();
                }
                //parameters.Popup.ClosePopup();
            }
        }

        [Command]
        public void Add(object p)
        {
            try
            {
                var item = new TreatmentStage() { Client = Client, ClientId = Client?.Id, Date = DateTime.Now.ToLongTimeString() };
                Collection?.Add(item);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке добавить значение в поле!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void AddChecked(object p)
        {
            try
            {
                if (p is TreeListView tree)
                {                  
                    var values = ((TreatmentStageViewModel)tree.DataContext).Templates.Where(f => f.IsChecked == true).ToArray();
                    var str = new StringBuilder();
                    values.ForEach(f => str.Append(f.Name + "\n"));

                    int idx = Collection.IndexOf(f => f.Guid == Model?.Guid);
                    if (idx < 0) return;

                    switch(TemplateName)
                    {
                        case "Complaint" : Collection[idx].Complaints = str.ToString(); break;
                        case "Anamnes" : Collection[idx].Anamneses = str.ToString(); break;
                        case "Objectivly" : Collection[idx].Objectively = str.ToString(); break;
                        case "DescriptionXRay" : Collection[idx].DescriptionXRay = str.ToString(); break;
                        case "Diagnos" : Collection[idx].Diagnoses = str.ToString(); break;
                        case "Plan" : Collection[idx].Plans = str.ToString(); break;
                        case "Treatment" : Collection[idx].Treatments = str.ToString(); break;
                        case "Allergy" : Collection[idx].Allergies = str.ToString(); break;
                    }
                    TemplateWin?.Close();
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке добавить значение в поле!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Clear(object p)
        {
            try
            {

                if (p is TreatmentParameters parameters)
                {
                    if (parameters.Model is TreatmentStage model)
                    {
                        switch (parameters.Name)
                        {
                            case "Complaint": model.Complaints = null; break;
                            case "Anamnes": model.Anamneses = null; break;
                            case "Objectivly": model.Objectively = null; break;
                            case "DescriptionXRay": model.DescriptionXRay = null; break;
                            case "Diagnos": model.Diagnoses = null; break;
                            case "Plan": model.Plans = null; break;
                            case "Treatment": model.Treatments = null; break;
                            case "Allergy": model.Allergies = null; break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке очистить поле!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        /**********************************************************/
        public void StatusReadOnly(bool status)
        {
            IsReadOnly = status;
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        [Command]
        public void SelectItemInServiceField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is ITreeModel model)
                    {
                        if (model.IsDir == 1) return;
                        parameters.Popup.EditValue = model;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch
            {

            }
        }


        [Command]
        public void Save(object p)
        {
            try
            {

            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке сохранить значение в поле!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }


        private void SetCollection() => Collection = db.TreatmentStage.Where(f => f.ClientId == Client.Id).Include(f => f.Client).ToObservableCollection() ?? new ObservableCollection<TreatmentStage>();


        public TreatmentStage Model { get; set; } //этап лечения
        public ObservableCollection<TreatmentStage> Collection { get; set; }
        public ICollection<TreeTemplate> Templates { get; set; }
        public SelectValueInTemplateWin TemplateWin { get; set; }
        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }

        public string TemplateName { get; set; }
    }


    public class TreeTemplate : ITreeModel
    {
        public int Id { get; set; }
        public bool IsChecked { get; set; } = false;
        public int? IsDir { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }

    }
}
