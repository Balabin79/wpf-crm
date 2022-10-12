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
using System.Text.Json;
using Dental.Infrastructures.Collection;

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
                SetTeeth();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с вкладкой \"Врачебная\" в карте клиента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanOpenTreatmentForm(object p) => true;
        public bool CanDelete(object p) => true;
        public bool CanSaveTreatment(object p) => true;
        public bool CanOpenForm(object p) => true;
        public bool CanCancelForm() => true;
        public bool CanAddChecked(object p) => true;
        public bool CanClear(object p) => true;
        public bool CanToothMarked(object p) => true;

        [Command]
        public void OpenTreatmentForm(object p)
        {
            try
            {
                Window wnd = Application.Current.Windows.OfType<Window>().Where(w => w.ToString() == TreatmentStageWindow?.ToString()).FirstOrDefault();
                if (wnd != null)
                {
                    wnd.Activate();
                    return;
                }

                if (p is TreatmentStage model) VM = new TreatmentStageVM() { Date = model?.Date, Name = model?.Name, Model = model };
                else VM = new TreatmentStageVM();

                TreatmentStageWindow = new TreatmentStageWindow() { DataContext = this, Height = 220 };
                TreatmentStageWindow?.Show();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Delete(object p)
        {
            try
            {
                if (p is TreatmentStage model)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить область?",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                    if (response.ToString() == "No") return;

                    db.TreatmentStage.Remove(model);
                    Collection.Remove(model);
                    Services.Reestr.Update((int)Tables.TreatmentStage);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке удаления области!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }

        }

        [Command]
        public void SaveTreatment(object p)
        {
            try
            {
                if(VM.Model != null)
                {
                    var model = db.TreatmentStage.FirstOrDefault(f => f.Guid == VM.Model.Guid);
                    model.Name = VM?.Name;
                    model.Date = VM?.Date;
                }
                else
                {
                    var item = new TreatmentStage()
                    {
                        Client = Client,
                        ClientId = Client?.Id,
                        Date = VM?.Date ?? DateTime.Now.ToShortDateString(),
                        Name = VM.Name ?? "Без названия"
                    };
                    Collection?.Add(item);
                    db?.TreatmentStage.Add(item);
                }
                Services.Reestr.Update((int)Tables.TreatmentStage);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке добавить значение в поле!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenForm(object p)
        {
            if (p is TreatmentParameters parameters)
            {
                if (parameters.Model is TreatmentStage model)
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
                    TemplateWin.ShowDialog();
                }
            }
        }

        [Command]
        public void CancelForm() => TreatmentStageWindow?.Close();

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
                        var response = ThemedMessageBox.Show(title: "Внимание", text: "Очистить поле?",
                            messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                        if (response.ToString() == "No") return;

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
                            case "Recomendation": model.Recommendations = null; break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке очистить поле!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool Save(Client client)
        {
            try
            {
                Client.Teeth = JsonSerializer.Serialize(Teeth);
                Client.ChildTeeth = JsonSerializer.Serialize(ChildTeeth);
                Services.Reestr.Update((int)Tables.TreatmentStage);
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                Client.Teeth = null;
                Client.ChildTeeth = null;
                return false;
            }

        }

        [Command]
        public void ToothMarked(object p)
        {
            try
            {
                if (p is ToothCommandParameters param)
                {
                    switch (param.Diagnos)
                    {
                        case "Healthy": param.Tooth.ToothImagePath = TeethImages.ImgPathGreen; param.Tooth.Abbr = "З"; break;
                        case "Missing": param.Tooth.ToothImagePath = TeethImages.ImgPathGray; param.Tooth.Abbr = "О"; break;
                        case "Impacted": param.Tooth.ToothImagePath = TeethImages.ImgPathGray; param.Tooth.Abbr = "НП"; break;
                        case "Radiks": param.Tooth.ToothImagePath = TeethImages.ImgPathGray; param.Tooth.Abbr = "КН"; break;
                        case "Caries": param.Tooth.ToothImagePath = TeethImages.ImgPathRed; param.Tooth.Abbr = "К"; break;
                        case "Pulpit": param.Tooth.ToothImagePath = TeethImages.ImgPathRed; param.Tooth.Abbr = "П"; break;
                        case "Gangrene": param.Tooth.ToothImagePath = TeethImages.ImgPathRed; param.Tooth.Abbr = "Г"; break;
                        case "Granuloma": param.Tooth.ToothImagePath = TeethImages.ImgPathRed; param.Tooth.Abbr = "Гр"; break;
                        case "Deletable": param.Tooth.ToothImagePath = TeethImages.ImgPathRed; param.Tooth.Abbr = "Э"; break;
                        case "MetalCrown": param.Tooth.ToothImagePath = TeethImages.ImgPathYellow; param.Tooth.Abbr = "КМ"; break;
                        case "Bridge": param.Tooth.ToothImagePath = TeethImages.ImgPathYellow; param.Tooth.Abbr = "М"; break;
                        case "Rp": param.Tooth.ToothImagePath = TeethImages.ImgPathYellow; param.Tooth.Abbr = "ПР"; break;
                        case "Seal": param.Tooth.ToothImagePath = TeethImages.ImgPathYellow; param.Tooth.Abbr = "ПЛ"; break;
                        case "Imp": param.Tooth.ToothImagePath = TeethImages.ImgPathImp; param.Tooth.Abbr = "Имп"; break;
                    }
                }
            }
            catch(Exception e)
            {

            }
        }

        public void StatusReadOnly(bool status)
        {
            IsReadOnly = status;
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        private void SetCollection() => Collection = Client == null ? new ObservableCollection<TreatmentStage>() : db.TreatmentStage.Where(f => f.ClientId == Client.Id).Include(f => f.Client).ToObservableCollection();


        public TreatmentStage Model { get; set; } //этап лечения
        public ObservableCollection<TreatmentStage> Collection { get; set; }
        public ICollection<TreeTemplate> Templates { get; set; }
        public SelectValueInTemplateWin TemplateWin { get; set; }
        public TreatmentStageWindow TreatmentStageWindow { get; set; }
        public TreatmentStageVM VM { get; set; }

        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }

        public PatientTeeth Teeth
        {
            get { return GetProperty(() => Teeth); }
            set { SetProperty(() => Teeth, value); }
        }       
        
        public ChildTeeth ChildTeeth
        {
            get { return GetProperty(() => ChildTeeth); }
            set { SetProperty(() => ChildTeeth, value); }
        }            

        public string TemplateName { get; set; }

        public void NewClientSaved(Client client)
        {
            Client = db.Clients.FirstOrDefault(f => f.Id == client.Id) ?? new Client();
        }

        public void SetTeeth()
        {
            try
            {
                if (Client != null && Client.Teeth?.Length > 100) Teeth = JsonSerializer.Deserialize<PatientTeeth>(Client.Teeth);
                else Teeth = new PatientTeeth();

                if (Client != null && Client.ChildTeeth?.Length > 100) ChildTeeth = JsonSerializer.Deserialize<ChildTeeth>(Client.ChildTeeth);
                else ChildTeeth = new ChildTeeth();
            }
            catch
            {
                Teeth = new PatientTeeth();
                ChildTeeth = new ChildTeeth();
            }
        }
    }

    public class TreeTemplate : ITree, IModel
    {
        public int Id { get; set; }
        public bool IsChecked { get; set; } = false;
        public int? IsDir { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }

        public int? CreatedAt { get; set; }
        public int? UpdatedAt { get; set; }
        public string Guid { get; set; }
    }
}
