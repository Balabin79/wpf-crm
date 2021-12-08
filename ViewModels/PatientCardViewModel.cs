using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Views.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using Dental.Services;
using Dental.Infrastructures.Extensions.Notifications;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;
using Dental.Infrastructures.Converters;
using System.Data.Entity.Validation;
using System.Windows.Documents;
using DevExpress.XtraRichEdit;

namespace Dental.ViewModels
{
    class PatientCardViewModel : ViewModelBase
    {
        private  ApplicationContext db;

        public PatientCardViewModel() : this(0) { }

        public PatientCardViewModel(int patientId)
        {
            try
            {
                db = new ApplicationContext();
                Files = new ObservableCollection<ClientFiles>();
                Ids = ProgramDirectory.GetIds();

                #region инициализация команд, связанных с общим функционалом карты пациента
                // Команда включения - отключения редактирования полей
                EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);
                SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
                DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
                #endregion


                #region инициализация команд, связанных с закладкой "Планы лечения"
                OpenFormPlanCommand = new LambdaCommand(OnOpenFormPlanCommandExecuted, CanOpenFormPlanCommandExecute);
                SavePlanCommand = new LambdaCommand(OnSavePlanCommandExecuted, CanSavePlanCommandExecute);
                DeletePlanCommand = new LambdaCommand(OnDeletePlanCommandExecuted, CanDeletePlanCommandExecute);

                SelectPosInClassificatorCommand = new LambdaCommand(OnSelectPosInClassificatorCommandExecuted, CanSelectPosInClassificatorCommandExecute);
                AddRowInPlanCommand = new LambdaCommand(OnAddRowInPlanCommandExecuted, CanAddRowInPlanCommandExecute);
                SaveRowInPlanCommand = new LambdaCommand(OnSaveRowInPlanCommandExecuted, CanSaveRowInPlanCommandExecute);
                DeleteRowInPlanCommand = new LambdaCommand(OnDeleteRowInPlanCommandExecuted, CanDeleteRowInPlanCommandExecute);
                CancelFormPlanCommand = new LambdaCommand(OnCancelFormPlanCommandExecuted, CanCancelFormPlanCommandExecute);
                #endregion

                #region Инициализация команд, связанных с закладкой "ИДС, Документы"
                OpenFormDocCommand = new LambdaCommand(OnOpenFormDocCommandExecuted, CanOpenFormDocCommandExecute);
                OpenFormDocEditCommand = new LambdaCommand(OnOpenFormDocEditCommandExecuted, CanOpenFormDocEditCommandExecute);
                DeleteDocCommand = new LambdaCommand(OnDeleteDocCommandExecuted, CanDeleteDocCommandExecute);
                ImportDocCommand = new LambdaCommand(OnImportDocCommandExecuted, CanImportDocCommandExecute);
                OpenDirDocCommand = new LambdaCommand(OnOpenDirDocCommandExecuted, CanOpenDirDocCommandExecute);
                #endregion

                #region инициализация команд, связанных с картой зубов пациента
                ClickToothGreenCommand = new LambdaCommand(OnClickToothGreenCommandExecuted, CanClickToothGreenCommandExecute);
                ClickToothYelPlCommand = new LambdaCommand(OnClickToothYelPlCommandExecuted, CanClickToothYelPlCommandExecute);
                ClickToothYelCorCommand = new LambdaCommand(OnClickToothYelCorCommandExecuted, CanClickToothYelCorCommandExecute);
                ClickToothImpCommand = new LambdaCommand(OnClickToothImpCommandExecuted, CanClickToothImpCommandExecute);
                ClickToothRedRCommand = new LambdaCommand(OnClickToothRedRCommandExecuted, CanClickToothRedRCommandExecute);
                ClickToothRedPtCommand = new LambdaCommand(OnClickToothRedPtCommandExecuted, CanClickToothRedPtCommandExecute);
                ClickToothRedPCommand = new LambdaCommand(OnClickToothRedPCommandExecuted, CanClickToothRedPCommandExecute);
                ClickToothRedCCommand = new LambdaCommand(OnClickToothRedCCommandExecuted, CanClickToothRedCCommandExecute);
                ClickToothGrayCommand = new LambdaCommand(OnClickToothGrayCommandExecuted, CanClickToothGrayCommandExecute);
                #endregion

                #region инициализация команд, связанных с прикреплением к карте пациента файлов
                DeleteFileCommand = new LambdaCommand(OnDeleteFileCommandExecuted, CanDeleteFileCommandExecute);
                ExecuteFileCommand = new LambdaCommand(OnExecuteFileCommandExecuted, CanExecuteFileCommandExecute);
                OpenDirectoryCommand = new LambdaCommand(OnOpenDirectoryCommandExecuted, CanOpenDirectoryCommandExecute);
                AttachmentFileCommand = new LambdaCommand(OnAttachmentFileCommandExecuted, CanAttachmentFileCommandExecute);
                #endregion               

                // Если patientCardNumber == 0, то это создается новая карта пациента, иначе загружаем данные существующего клиента
                if (patientId == 0)
                {
                    Model = new PatientInfo();
                    NumberPatientCard = (CreateNewNumberPatientCard()).ToString();
                    Model.PatientCardCreatedAt = DateTime.Now.ToShortDateString();

                    // для новой карты пациента все поля по-умолчанию доступны для редактирования
                    IsReadOnly = false;
                    _BtnIconEditableHide = false;
                    _BtnIconEditableVisible = true;
                }
                else
                {
                    Model = db.PatientInfo.Where(f => f.Id == patientId)?
                        .Include(i => i.TreatmentPlans.Select(g => g.TreatmentPlanItems))
                        .FirstOrDefault();
                    //для существующей карты пациента все поля по-умолчанию недоступны для редактирования
                    NumberPatientCard = Model?.Id.ToString();
                    IsReadOnly = true;
                    _BtnIconEditableHide = true;
                    _BtnIconEditableVisible = false;

                    if (Model != null && ProgramDirectory.HasPatientCardDirectory(Model.Id.ToString()))
                    {
                        Files = ProgramDirectory.GetFilesFromPatientCardDirectory(Model.Id.ToString()).ToObservableCollection<ClientFiles>();                        
                    }
                }
                ModelBeforeChanges = (PatientInfo)Model.Clone();
                LoadFieldsCollection();
                LoadTeeth();


                ClassificatorCategories = db.Classificator.ToList();
                Employes = db.Employes.ToList();
                /*
                TreatmentPlans = (Model.Id == 0) ? new ObservableCollection<TreatmentPlan>() : 
                    db.TreatmentPlan.Where(f => f.PatientInfoId == Model.Id)
                    .Include(f => f.TreatmentPlanItems.Select(i => i.Classificator))
                    .Include(f => f.TreatmentPlanItems.Select(i => i.Employee))
                    .OrderBy(f => f.Id).ToObservableCollection();*/
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с картой пациента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        #region Команды и связанный ф-нал с ИДС и документами
        // открыть форму и загрузить документ
        public ICommand OpenFormDocCommand { get; }
        public ICommand OpenFormDocEditCommand { get; }
        public ICommand DeleteDocCommand { get; }
        public ICommand ImportDocCommand { get; }
        public ICommand OpenDirDocCommand { get; }

        private bool CanOpenFormDocCommandExecute(object p) => true;
        private bool CanOpenFormDocEditCommandExecute(object p) => true;
        private bool CanDeleteDocCommandExecute(object p) => true;
        private bool CanImportDocCommandExecute(object p) => true;
        private bool CanOpenDirDocCommandExecute(object p) => true;

        private void OnOpenFormDocCommandExecuted(object p)
        {
            try
            {
                string fileName = p.ToString();
                if (fileName != null && File.Exists(fileName))
                {
                    FileInfo fileInfo = new FileInfo(fileName);

                    IDSWindow = new IDSWindow();
                    IDSWindow.DataContext = this;
                    var richEdit = IDSWindow.RichEdit;
                    richEdit.ReadOnly = true;
                    richEdit.LoadDocument(fileName, GetDocumentFormat(fileName));

                    richEdit.DocumentSaveOptions.CurrentFileName = Path.Combine(ProgramDirectory.GetPathMyDocuments(), fileInfo.Name);

                    richEdit.RtfText = new RtfParse(richEdit.RtfText, Model).Run();
                    IDSWindow.Show();                
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnOpenFormDocEditCommandExecuted(object p)
        {
            try
            {
                string fileName = p.ToString();
                if (fileName != null && File.Exists(fileName))
                {
                    IDSWindow = new IDSWindow();
                    IDSWindow.DataContext = this;
                    var richEdit = IDSWindow.RichEdit;
                    richEdit.LoadDocument(fileName, GetDocumentFormat(fileName));                   
                    IDSWindow.Show();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnDeleteDocCommandExecuted(object p)
        {
            try
            {
                string fileName = p.ToString();
                if (fileName != null && File.Exists(fileName))
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить документ с компьютера?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    FileInfo fileInfo = new FileInfo(fileName);
                    ProgramDirectory.RemoveIDSFile(fileInfo);
                    Ids.Remove(Ids.Where(f => f.FullName == fileInfo.FullName).FirstOrDefault());
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnImportDocCommandExecuted(object p)
        {
            try
            {

            } 
            catch(Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnOpenDirDocCommandExecuted(object p)
        {
            try
            {
                var dir = ProgramDirectory.GetPathIdsDirectoty();
                Process.Start(dir);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                    text: "Невозможно открыть содержащую документы директорию!",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
            }
        }

        private DocumentFormat GetDocumentFormat(string fileName)
        {
            try
            {
                string ext = new FileInfo(fileName).Extension.ToString().Replace(".", "");
                switch(ext)
                {
                    case "rtf": return DocumentFormat.Rtf;
                    case "doc": return DocumentFormat.Doc;
                    case "docx": return DocumentFormat.Doc;
                    case "html": return DocumentFormat.Html;
                    case "htm": return DocumentFormat.Html;
                    case "mht": return DocumentFormat.Mht;
                    case "epub": return DocumentFormat.ePub;
                    case "txt": return DocumentFormat.PlainText;
                    case "odt": return DocumentFormat.OpenDocument;
                    default: return DocumentFormat.PlainText;
                }
            }
            catch(Exception e)
            {
                return DocumentFormat.PlainText;
            }
        }

        public IDSWindow IDSWindow { get; set; }
      
        public ObservableCollection<FileInfo> Ids 
        { 
            get => ids; 
            set => Set(ref ids, value); 
        }
        private ObservableCollection<FileInfo> ids;
        #endregion


        #region Команды и ф-нал связанный с Планом лечения

        // открыть форму плана лечения
        public ICommand OpenFormPlanCommand { get; }
        // сохранить новый или отредактированный план лечения
        public ICommand SavePlanCommand { get; }
        //удалить план лечения
        public ICommand DeletePlanCommand { get; }


        public ICommand SelectPosInClassificatorCommand { get; }
        public ICommand AddRowInPlanCommand { get; }
        public ICommand SaveRowInPlanCommand { get; }
        public ICommand DeleteRowInPlanCommand { get; }
        public ICommand CancelFormPlanCommand { get; }

        private bool CanSelectPosInClassificatorCommandExecute(object p) => true;
        private bool CanOpenFormPlanCommandExecute(object p) => true;
        private bool CanSavePlanCommandExecute(object p) => true;
        private bool CanDeletePlanCommandExecute(object p) => true;
        private bool CanAddRowInPlanCommandExecute(object p) => true;
        private bool CanSaveRowInPlanCommandExecute(object p) => true;
        private bool CanDeleteRowInPlanCommandExecute(object p) => true;
        private bool CanCancelFormPlanCommandExecute(object p) => true;

        private void OnOpenFormPlanCommandExecuted(object p)
        {
            try
            {
                if (p != null)
                {
                    PlanModel = db.TreatmentPlan.Where(i => i.Id == (int)p).FirstOrDefault();
                }
                else
                {
                    PlanModel = new TreatmentPlan();
                    PlanModel.PatientInfoId = Model.Id;
                    PlanModel.DateTime = PlanModel.DateTime != null ? PlanModel.DateTime : DateTime.Now.ToShortDateString();
                }
                PlanWindow = new PlanWindow();
                PlanWindow.DataContext = this;
                PlanWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSavePlanCommandExecuted(object p)
        {
            try
            {
                if (PlanModel.Id == 0) Model.TreatmentPlans.Add(PlanModel);
                db.SaveChanges();
                PlanWindow.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnDeletePlanCommandExecuted(object p)
        {
            try
            {
                if (p is TreatmentPlan plan)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Вы уверены в своих действиях?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    
                    db.TreatmentPlan.Remove(plan);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSelectPosInClassificatorCommandExecuted(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is Classificator classificator)
                    {
                        if (classificator.IsDir == 1) return;
                        parameters.Popup.EditValue = classificator;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAddRowInPlanCommandExecuted(object p)
        {
            try
            {
                if (p is TreatmentPlan plan) plan.TreatmentPlanItems.Add(new TreatmentPlanItems());
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSaveRowInPlanCommandExecuted(object p)
        {
            try
            {
                if (db.GetValidationErrors().Count() > 0) return;

                int cnt = db.SaveChanges();
                if (cnt > 0)
                {
                    var notification = new Notification();
                    notification.Content = "Позиции в плане лечения сохранены!";
                    var nav = Navigation.Instance;
                    notification.run();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnDeleteRowInPlanCommandExecuted(object p)
        {
            try
            {
                if (p is TreatmentPlanItems item)
                {
                    db.Entry(item).State = EntityState.Deleted;
                    db.SaveChanges();
                    //var planitem = TreatmentPlans.Select(f => f.TreatmentPlanItems.Where(i => i.Id == item.Id));
                }

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCancelFormPlanCommandExecuted(object p) => PlanWindow.Close();

        private PlanWindow PlanWindow;
       
        
        public TreatmentPlan PlanModel
        {
            get => _PlanModel;
            set => Set(ref _PlanModel, value);
        }
        public TreatmentPlan _PlanModel;

        public Visibility IsVisibleItemPlanForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupPlanForm { get; set; } = Visibility.Hidden;

        private ObservableCollection<TreatmentPlan> _PlanGroup;
        public ObservableCollection<TreatmentPlan> PlanGroup
        {
            get => _PlanGroup;
            set => Set(ref _PlanGroup, value);
        }
       /* public ObservableCollection<TreatmentPlan> TreatmentPlans
        {
            get => _TreatmentPlans;
            set => Set(ref _TreatmentPlans, value);
        }
        public ObservableCollection<TreatmentPlan> _TreatmentPlans;*/

        private void VisibleItemPlanForm()
        {
            IsVisibleItemPlanForm = Visibility.Visible;
            IsVisibleGroupPlanForm = Visibility.Hidden;
            PlanWindow.Width = 800;
            PlanWindow.Height = 328;
        }
        private void VisibleItemPlanGroup()
        {
            IsVisibleItemPlanForm = Visibility.Hidden;
            IsVisibleGroupPlanForm = Visibility.Visible;
            PlanWindow.Width = 800;
            PlanWindow.Height = 280;
        }

        public List<Classificator> ClassificatorCategories { get; set; }
        public List<Employee> Employes { get; set; }


        #endregion

        #region команды, связанные с общим функционалом карты пациента
        public ICommand EditableCommand { get; }
        private bool CanEditableCommandExecute(object p) => true;
        private void OnEditableCommandExecuted(object p)
        {
            try
            {
                IsReadOnly = !IsReadOnly;
                BtnIconEditableHide = IsReadOnly;
                BtnIconEditableVisible = !IsReadOnly;
                if (Model != null && Model.Id != 0) BtnAfterSaveEnable = !IsReadOnly;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public ICommand DeleteCommand { get; }
        private bool CanDeleteCommandExecute(object p) => true;
        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить карту пациента из базы данных, без возможности восстановления?",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;
                if (!CheckingRelatedData())
                {
                    //показываем сообщение, что какие связанные с картой пациента данные необходимо удалить, чтобы стало возможным удалить карту пациента, либо программно удалить, но предупредить и показать какие связанные данные будут удалены с бд в том числе
                }

                Delete();
                var notification = new Notification();
                notification.Content = "Карта пациента полностью удалена из базы данных!";
                var nav = Navigation.Instance;
                notification.run();
                nav.LeftMenuClick.Execute("Dental.Views.PatientCard.PatientsList");

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public ICommand SaveCommand { get; }
        private bool CanSaveCommandExecute(object p) => true;
        private void OnSaveCommandExecuted(object p)
        {
            try
            {
                var notification = new Notification();
                if (Model.Id == 0)
                {
                    notification.Content = "Новый пациент успешно записан в базу данных!";
                    db.PatientInfo.Add(Model);
                    SaveTeeth();
                    BtnAfterSaveEnable = true;
                }
                else
                {
                    notification.Content = "Отредактированные данные пациента сохранены в базу данных!";
                    SaveFiles();
                   // Update();
                    SaveTeeth();
                }
                db.SaveChanges();
                if (HasUnsavedChanges())
                {
                    notification.run();
                    ModelBeforeChanges = (PatientInfo)Model.Clone();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }

        }

        // Поиск связанных с картой пациента данных перед их удалением
        private bool CheckingRelatedData()
        {

            return true;
        }
        #endregion


        #region команды, связанных с формулой зубов
        public ICommand ClickToothGreenCommand { get; }
        public ICommand ClickToothYelPlCommand { get; }
        public ICommand ClickToothYelCorCommand { get; }
        public ICommand ClickToothImpCommand { get; }
        public ICommand ClickToothRedRCommand { get; }
        public ICommand ClickToothRedPtCommand { get; }
        public ICommand ClickToothRedPCommand { get; }
        public ICommand ClickToothRedCCommand { get; }
        public ICommand ClickToothGrayCommand { get; }

        private bool CanClickToothGreenCommandExecute(object p) => true;
        private bool CanClickToothYelPlCommandExecute(object p) => true;
        private bool CanClickToothYelCorCommandExecute(object p) => true;
        private bool CanClickToothImpCommandExecute(object p) => true;
        private bool CanClickToothRedRCommandExecute(object p) => true;
        private bool CanClickToothRedPtCommandExecute(object p) => true;
        private bool CanClickToothRedPCommandExecute(object p) => true;
        private bool CanClickToothRedCCommandExecute(object p) => true;
        private bool CanClickToothGrayCommandExecute(object p) => true;

        private void OnClickToothGreenCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothGreenCommandExecuted");

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnClickToothYelPlCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothYelPlCommandExecuted");

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnClickToothYelCorCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothYelCorCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnClickToothImpCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothImpCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnClickToothRedRCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothRedRCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnClickToothRedPtCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothRedPtCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnClickToothRedPCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothRedPCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnClickToothRedCCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothRedCCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnClickToothGrayCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothGrayCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public PatientTeeth _Teeth;
        public PatientTeeth Teeth
        {
            get => _Teeth;
            set => Set(ref _Teeth, value);
        }

        private void SetToothState(object p, string methodName)
        {
            Tooth tooth = p as Tooth;
            if (tooth == null) return;

            switch (methodName)
            {
                case "OnClickToothGreenCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathGreen; tooth.Abbr = ""; break;
                case "OnClickToothYelPlCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathYellow; tooth.Abbr = PatientTeeth.Plomba; break;
                case "OnClickToothYelCorCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathYellow; tooth.Abbr = PatientTeeth.Coronka; break;
                case "OnClickToothImpCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathImp; tooth.Abbr = ""; break;
                case "OnClickToothRedRCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathRed; tooth.Abbr = PatientTeeth.Radiks; break;
                case "OnClickToothRedPtCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathRed; tooth.Abbr = PatientTeeth.Periodontit; break;
                case "OnClickToothRedPCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathRed; tooth.Abbr = PatientTeeth.Pulpit; break;
                case "OnClickToothRedCCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathRed; tooth.Abbr = PatientTeeth.Caries; break;
                case "OnClickToothGrayCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathGray; tooth.Abbr = ""; break;
            }
        }

        private void SaveTeeth()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var jsonTeeth = JsonSerializer.Serialize<PatientTeeth>(Teeth, options);

                var teeth = db.Teeth.Where(i => i.PatientInfoId == Model.Id).FirstOrDefault();
                if (teeth == null)
                {

                    db.Teeth.Add(new Teeth()
                    {
                        Guid = KeyGenerator.GetUniqueKey(),
                        PatientInfoId = Model.Id,
                        PatientTeeth = jsonTeeth
                    });
                }
                else teeth.PatientTeeth = jsonTeeth;
                db.SaveChanges();
            }
            catch(Exception e)
            {
                (new ViewModelLog(e)).run();
            }           
        }

        private void LoadTeeth()
        {
            try
            {
                var teeth = db.Teeth.Where(i => i.PatientInfoId == Model.Id).FirstOrDefault();
                if (teeth == null)
                {
                    Teeth = new PatientTeeth();
                    return;
                }
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var patientTeeth = JsonSerializer.Deserialize<PatientTeeth>(teeth.PatientTeeth, options);
                Teeth = new PatientTeeth(patientTeeth);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion

        #region команды, связанных с прикреплением к карте пациентов файлов     
        public ICommand DeleteFileCommand { get; }
        public ICommand ExecuteFileCommand { get; }
        public ICommand AttachmentFileCommand { get; }
        public ICommand OpenDirectoryCommand { get; }

        private bool CanDeleteFileCommandExecute(object p) => true;
        private bool CanExecuteFileCommandExecute(object p) => true;
        private bool CanAttachmentFileCommandExecute(object p) => true;
        private bool CanOpenDirectoryCommandExecute(object p) => true;

        private void OnOpenDirectoryCommandExecuted(object p)
        {
            try
            {
                var file = p as ClientFiles;
                var dir = Path.GetDirectoryName(file?.Path);
                Process.Start(dir);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                    text: "Невозможно открыть содержащую файл директорию!",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
            }
        }

        private void OnExecuteFileCommandExecuted(object p)
        {
            try
            {
                var file = p as ClientFiles;
                Process.Start(file?.Path);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                   text: "Невозможно запустить файл!",
                   messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAttachmentFileCommandExecuted(object p)
        {
            try
            {
                var fileContent = string.Empty;
                var filePath = string.Empty;
                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "All files (*.*)|*.*|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        filePath = openFileDialog.FileName;
                        ClientFiles file = new ClientFiles();
                        file.Path = filePath;
                        file.DateCreated = DateTime.Today.ToShortDateString();
                        file.Name = Path.GetFileNameWithoutExtension(filePath);
                        file.FullName = Path.GetFileName(filePath);
                        if (Path.HasExtension(filePath))
                        {
                            file.Extension = Path.GetExtension(filePath);
                        }
                        file.Size = new FileInfo(filePath).Length.ToString();

                        if (FindDoubleFile(file.FullName))
                        {
                            var response = ThemedMessageBox.Show(title: "Внимание!", text: "Файл с таким именем уже есть в списке прикрепленных файлов. Вы хотите его заменить?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                            if (response.ToString() == "No") return;
                            var idx = Files.IndexOf(f => (string.Compare(f.FullName, file.FullName, StringComparison.CurrentCulture) == 0));
                            if (idx == -1) return;
                            file.Status = ClientFiles.STATUS_NEW_RUS;
                            Files[idx] = file;
                            return;
                        }
                        Files.Insert(0,file);
                    }
                }

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnDeleteFileCommandExecuted(object p)
        {
            try
            {
                var file = p as ClientFiles;
                var item = Files.Where<ClientFiles>(f => string.Compare(f.FullName, file.FullName, StringComparison.CurrentCulture) == 0).FirstOrDefault();
                if (item == null) return;               
                if (string.Compare(file.Status, ClientFiles.STATUS_SAVE_RUS, StringComparison.CurrentCulture) == 0)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Вы собираетесь физически удалить файл с компьютера! Вы уверены в своих действиях?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;                     
                    ProgramDirectory.RemoveFileFromPatientsCard(Model.Id.ToString(), file);
                }
                Files.Remove(file);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private bool FindDoubleFile(string fileName)
        {
            foreach(var file in Files)
            {
                if (string.Compare(file.FullName, fileName, StringComparison.CurrentCulture) == 0) return true;
            }
            return false;
        } 

        private bool SaveFiles()
        {
            try
            {
                //если нет директории программы, то создаем ее
                if (!ProgramDirectory.HasMainProgrammDirectory())
                {
                    var _ = ProgramDirectory.CreateMainProgrammDirectoryForPatientCards();
                }
                if (!ProgramDirectory.HasPatientCardDirectory(Model.Id.ToString()))
                {
                    var _ = ProgramDirectory.CreatePatientCardDirectory(Model.Id.ToString());
                }

                ProgramDirectory.Errors.Clear();
                MoveFilesToPatientCardDirectory();
                return true;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
                return false;
            }
        }

        private void MoveFilesToPatientCardDirectory()
        {
            foreach (ClientFiles file in Files)
            {
                if (string.Compare(file.Status, ClientFiles.STATUS_SAVE_RUS, StringComparison.CurrentCulture) == 0) continue;
                ProgramDirectory.SaveInPatientCardDirectory(Model.Id.ToString(), file);
                file.Status = ClientFiles.STATUS_SAVE_RUS;
            }
        }

        public bool HasUnsavedFiles()
        {
            if (Files.Count > 0)
            {
                foreach (var i in Files)
                {
                    if (i.Status == ClientFiles.STATUS_NEW_RUS)
                    {
                        Model.FieldsChanges["Административная"].Add("Прикрепляемые файлы");
                        return true;
                    }
                }
            }
            return false;
        }

        public ObservableCollection<ClientFiles> _Files;
        public ObservableCollection<ClientFiles> Files
        {
            get => _Files;
            set => Set(ref _Files, value);
        }
        #endregion

       

        public bool HasUnsavedChanges()
        {
            bool hasUnsavedChanges = false;
            if (Model.FieldsChanges != null) Model.FieldsChanges = PatientInfo.CreateFieldsChanges();
            if (!Model.Equals(ModelBeforeChanges)) hasUnsavedChanges = true;
            if (HasUnsavedFiles()) hasUnsavedChanges = true;
            return hasUnsavedChanges;
        }

        public bool UserSelectedBtnCancel()
        {
            string warningMessage = "";     
            foreach (var tab in Model.FieldsChanges)
            {
                if (tab.Value.Count == 0) continue;
                string fieldNames = " ";
                foreach (var field in tab.Value)
                {
                    fieldNames += " \"" + field + "\",";
                }
                 warningMessage = "\nВо вкладке \"" + tab.Key + "\", поля:" + fieldNames.Remove(fieldNames.Length - 1) + "\n";
            }

             var response = ThemedMessageBox.Show(title: "Внимание", text: "В карте пациента содержатся несохраненные изменения! Если вы не хотите их потерять, то нажмите кнопку \"Отмена\", а затем кнопку сохранить (иконка с дискетой).\nИзменения:" + warningMessage,
                messageBoxButtons: MessageBoxButton.OKCancel, icon: MessageBoxImage.Warning);

            return response.ToString() == "Cancel";
        }


        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set => Set(ref _IsReadOnly, value);
        }

        public string _NumberPatientCard;
        public string NumberPatientCard 
        { 
            get => _NumberPatientCard;
            set => Set(ref _NumberPatientCard, value);
        }

        public bool _BtnAfterSaveEnable = false;
        public bool BtnAfterSaveEnable
        {
            get => _BtnAfterSaveEnable;
            set => Set(ref _BtnAfterSaveEnable, value);
        }


        private PatientInfo _Model;
        public PatientInfo Model
        {
            get => _Model;
            set => Set(ref _Model, value);
        }

        public PatientInfo ModelBeforeChanges { get; set; }

        public ICollection<string> AdvertisingList { get; set; }
        public IEnumerable<string> ClientsGroupList { get; set; }
        //public ObservableCollection<ClientTreatmentPlans> ClientTreatmentPlans { get; set; }

        public ICollection<string> GenderList
        {
            get => _GenderList;
        }

        private readonly ICollection<string> _GenderList = new List<string> { "Мужчина", "Женщина" };

        private int CreateNewNumberPatientCard()
        {
            var id = db.PatientInfo?.Max(e => e.Id);
            if (id == null) return 1;
            return (int)++id;
        }
        private bool _BtnIconEditableVisible;
        public bool BtnIconEditableVisible
        {
            get => _BtnIconEditableVisible;
            set => Set(ref _BtnIconEditableVisible, value);
        }

        private bool _BtnIconEditableHide;
        public bool BtnIconEditableHide
        {
            get => _BtnIconEditableHide;
            set => Set(ref _BtnIconEditableHide, value);
        }

        private void LoadFieldsCollection()
        {
            AdvertisingList = db.Advertising.OrderBy(f => f.Name).Select(f => f.Name).ToList();
            ClientsGroupList = db.ClientsGroup.OrderBy(f => f.Name).Select(f => f.Name).ToList();
        }



        private void Add()
        {
            db.Entry(Model).State = EntityState.Added;
            db.SaveChanges();
        }
        
        protected void Update()
        {
            db.Entry(Model).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void Delete()
        {
            db.Entry(Model).State = EntityState.Deleted;
            db.SaveChanges();
        }

    }
}
