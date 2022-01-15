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
                Files = new ObservableCollection<FileInfo>();
                Ids = ProgramDirectory.GetIds();

                #region инициализация команд, связанных с общим функционалом карты клиента
                // Команда включения - отключения редактирования полей
                EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);
                SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
                DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
                #endregion

                #region инициализация команд, связанных с закладкой "Планы лечения"
                OpenFormPlanCommand = new LambdaCommand(OnOpenFormPlanCommandExecuted, CanOpenFormPlanCommandExecute);
                EditPlanItemCommand = new LambdaCommand(OnEditPlanItemCommandExecuted, CanEditPlanItemCommandExecute);
                SavePlanCommand = new LambdaCommand(OnSavePlanCommandExecuted, CanSavePlanCommandExecute);
                DeletePlanCommand = new LambdaCommand(OnDeletePlanCommandExecuted, CanDeletePlanCommandExecute);

                SelectPosInClassificatorCommand = new LambdaCommand(OnSelectPosInClassificatorCommandExecuted, CanSelectPosInClassificatorCommandExecute);
                AddRowInPlanCommand = new LambdaCommand(OnAddRowInPlanCommandExecuted, CanAddRowInPlanCommandExecute);
                SaveRowInPlanCommand = new LambdaCommand(OnSaveRowInPlanCommandExecuted, CanSaveRowInPlanCommandExecute);
                DeleteRowInPlanCommand = new LambdaCommand(OnDeleteRowInPlanCommandExecuted, CanDeleteRowInPlanCommandExecute);
                CancelFormPlanCommand = new LambdaCommand(OnCancelFormPlanCommandExecuted, CanCancelFormPlanCommandExecute);
                CancelFormPlanItemCommand = new LambdaCommand(OnCancelFormPlanItemCommandExecuted, CanCancelFormPlanItemCommandExecute);
                #endregion

                #region Инициализация команд, связанных с закладкой "ИДС, Документы"
                OpenFormDocCommand = new LambdaCommand(OnOpenFormDocCommandExecuted, CanOpenFormDocCommandExecute);
                OpenFormDocEditCommand = new LambdaCommand(OnOpenFormDocEditCommandExecuted, CanOpenFormDocEditCommandExecute);
                DeleteDocCommand = new LambdaCommand(OnDeleteDocCommandExecuted, CanDeleteDocCommandExecute);
                ImportDocCommand = new LambdaCommand(OnImportDocCommandExecuted, CanImportDocCommandExecute);
                OpenDirDocCommand = new LambdaCommand(OnOpenDirDocCommandExecuted, CanOpenDirDocCommandExecute);
                #endregion

                #region инициализация команд, связанных с прикреплением к карте клиента файлов
                DeleteFileCommand = new LambdaCommand(OnDeleteFileCommandExecuted, CanDeleteFileCommandExecute);
                ExecuteFileCommand = new LambdaCommand(OnExecuteFileCommandExecuted, CanExecuteFileCommandExecute);
                OpenDirectoryCommand = new LambdaCommand(OnOpenDirectoryCommandExecuted, CanOpenDirectoryCommandExecute);
                AttachmentFileCommand = new LambdaCommand(OnAttachmentFileCommandExecuted, CanAttachmentFileCommandExecute);
                #endregion               

                // Если patientCardNumber == 0, то это создается новая карта клиента, иначе загружаем данные существующего клиента
                if (patientId == 0)
                {
                    Model = new PatientInfo();
                    NumberPatientCard = (CreateNewNumberPatientCard()).ToString();
                    Model.ClientCardCreatedAt = DateTime.Now.ToShortDateString();

                    // для новой карты пациента все поля по-умолчанию доступны для редактирования
                    IsReadOnly = false;
                    _BtnIconEditableHide = false;
                    _BtnIconEditableVisible = true;
                }
                else
                {
                    Model = db.PatientInfo.Where(f => f.Id == patientId)
                        ?.Include(f => f.ClientCategory)
                        ?.Include(i => i.TreatmentPlans.Select(g => g.TreatmentPlanItems))
                        .FirstOrDefault();
                    //для существующей карты пациента все поля по-умолчанию недоступны для редактирования
                    NumberPatientCard = Model?.Id.ToString();
                    IsReadOnly = true;
                    _BtnIconEditableHide = true;
                    _BtnIconEditableVisible = false;

                    if (Model != null && ProgramDirectory.HasPatientCardDirectory(Model.Id.ToString()))
                    {
                        Files = ProgramDirectory.GetFilesFromPatientCardDirectory(Model.Id.ToString()).ToObservableCollection<FileInfo>();                        
                    }
                    if (Directory.Exists(GetPathToPatientCard()))
                    {
                        Files = new DirectoryInfo(GetPathToPatientCard()).GetFiles().ToObservableCollection();
                    }
                }
                ModelBeforeChanges = (PatientInfo)Model.Clone();
                LoadFieldsCollection();


                ClassificatorCategories = db.Classificator.ToList();
                Employes = db.Employes.ToList();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с картой пациента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }


        #region команды, связанные с общим функционалом карты пациента
       
        public ICommand EditableCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        private bool CanEditableCommandExecute(object p) => true;
        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;

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
        
        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить карту клиента из базы данных, без возможности восстановления? Также будут удалены планы услуг, записи в расписании, в рассылках, обращениях клиента и все файлы прикрепленные к карте клиента!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;

                DeleteClientFiles();
                db.TreatmentPlan.Include(f => f.TreatmentPlanItems).Where(f => f.PatientInfoId == Model.Id).ToArray()?.ForEach(f => db.Entry(f).State = EntityState.Deleted);

               /* db.TreatmentPlanItems.Where(f => f.TreatmentPlanId == null).ToArray()?.ForEach(f => db.Entry(f).State = EntityState.Deleted);*/
                db.ClientsRequests.Where(f => f.ClientInfoId == Model.Id).ToArray()?.ForEach(f => db.Entry(f).State = EntityState.Deleted);
                db.SubscribesLog.Where(f => f.ClientInfoId == Model.Id).ToArray()?.ForEach(f => db.Entry(f).State = EntityState.Deleted);

                //удалить также в расписании
                db.Entry(Model).State = EntityState.Deleted;
                int cnt = db.SaveChanges();
                // подчищаем остатки
                db.TreatmentPlanItems.Where(f => f.TreatmentPlanId == null).ToArray()?.ForEach(f => db.Entry(f).State = EntityState.Deleted);
                db.SaveChanges();

                if (cnt > 0)
                {
                    var notification = new Notification();
                    notification.Content = "Карта клиента полностью удалена из базы данных!";
                    var nav = Navigation.Instance;
                    notification.run();
                    nav.LeftMenuClick.Execute("Dental.Views.PatientCard.PatientsList");
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
              
        private void OnSaveCommandExecuted(object p)
        {
            try
            {
                if (Model == null) return;
                var notification = new Notification();
                if (Model.Id == 0)
                {
                    notification.Content = "Новый клиент успешно записан в базу данных!";
                    db.PatientInfo.Add(Model);
                    BtnAfterSaveEnable = true;
                }
                else
                {
                    notification.Content = "Отредактированные данные клиента сохранены в базу данных!";
                    // Update();
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

        // Поиск связанных с картой клиента данных перед их удалением
        private void DeleteClientFiles()
        {
            try
            {
                string path = Path.Combine(PathToPatientsCards, Model.Guid);
                if (Directory.Exists(path)) Directory.Delete(path, true);
            } 
            catch(Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Неудачная попытка удалить файлы, прикрепленные к карте клиента. Возможно файлы были запущены в другой программе! Попробуйте закрыть запущенные сторонние программы и повторить!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Error);
            }
        }
        #endregion

        #region команды, связанных с прикреплением к карте клиентов файлов 
        private const string PATIENTS_CARDS_DIRECTORY = "Dental\\ClientsCards";
        private string PathToPatientsCards { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PATIENTS_CARDS_DIRECTORY);

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
                if (Directory.Exists(GetPathToPatientCard())) Process.Start(GetPathToPatientCard());
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
                if (p is FileInfo file) Process.Start(file.FullName);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                   text: "Невозможно выполнить загрузку файла!",
                   messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAttachmentFileCommandExecuted(object p)
        {
            try
            {
                var filePath = string.Empty;
                using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
                {
                    dialog.InitialDirectory = "c:\\";
                    dialog.Filter = "All files (*.*)|*.*|All files (*.*)|*.*";
                    dialog.FilterIndex = 2;
                    dialog.RestoreDirectory = true;

                    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    filePath = dialog.FileName;
                    if (string.IsNullOrEmpty(filePath)) return;
                }

                FileInfo file = new FileInfo(filePath);

                // проверяем на наличие существующего файла
                foreach (var i in Files)
                {
                    if (string.Compare(i.Name, file.Name, StringComparison.CurrentCulture) == 0)
                    {
                        var response = ThemedMessageBox.Show(title: "Внимание!", text: "Файл с таким именем уже есть в списке прикрепленных файлов. Заменить текущий файл?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                        if (response.ToString() == "No") return; // не захотел, поэтому дальше ничего не делаем

                        // Решил заменить файл, удаляем файл, добавляем новый и перезагружаем коллекцию
                        i.Delete();
                    }
                }

                string path = GetPathToPatientCard();

                if (!Directory.Exists(path))  Directory.CreateDirectory(path);
                
                File.Copy(file.FullName, Path.Combine(path, file.Name), true);

                FileInfo newFile = new FileInfo(Path.Combine(path, file.Name));
                newFile.CreationTime = DateTime.Now;

                Files = new DirectoryInfo(path).GetFiles().ToObservableCollection();
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
                if (p is FileInfo file)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить файл с компьютера?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                     file.Delete();
                    Files = new DirectoryInfo(GetPathToPatientCard()).GetFiles().ToObservableCollection();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private string GetPathToPatientCard() => Path.Combine(PathToPatientsCards, GetGuid());
        private string GetGuid()
        {
            if (Model.Guid == null) Model.Guid = KeyGenerator.GetUniqueKey();
            return Model.Guid;
        }

        public ObservableCollection<FileInfo> Files
        {
            get => files;
            set => Set(ref files, value);
        }
        private ObservableCollection<FileInfo> files;
        #endregion

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
                if (p == null) return;
                string fileName = p.ToString();
                if (fileName != null && File.Exists(fileName))
                {
                    FileInfo fileInfo = new FileInfo(fileName);

                    IDSWindow = new IDSWindow();
                    IDSWindow.DataContext = this;
                    var richEdit = IDSWindow.RichEdit;
                    richEdit.ReadOnly = true;
                    richEdit.LoadDocument(fileName, GetDocumentFormat(fileName));

                    richEdit.DocumentSaveOptions.CurrentFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileInfo.Name);
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
                if (p == null) return;
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
                if (p == null) return;
                string fileName = p.ToString();
                if (fileName != null && File.Exists(fileName))
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить документ с компьютера?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    File.Delete(fileName);
                    Ids = GetIds();
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
                var filePath = string.Empty;
                var fileName = string.Empty;

                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));

                    openFileDialog.Filter = "Office Files(*.docx; *.doc; *.rtf; *.odt; *.epub; *.txt; *.html; *.htm; *.mht; *.xml) | *.docx; *.doc; *.rtf; *.odt; *.epub; *.txt; *.html; *.htm; *.mht; *.xml";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string idsDirectory = GetPathIdsDirectoty();
   
                        foreach (var ids in Ids)
                        {
                            if (openFileDialog.SafeFileName == ids.Name)
                            {
                                var response = ThemedMessageBox.Show(title: "Внимание!", text: "Документ с таким именем уже существует. Вы хотите его заменить?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                                if (response.ToString() == "No") return;
                            }
                        }
                        fileName = openFileDialog.SafeFileName;
                        filePath = openFileDialog.FileName;
                    }
                    openFileDialog.Dispose();
                    //ProgramDirectory.ImportIds(new FileInfo(openFileDialog.FileName));
                }
                var newPath = Path.Combine(GetPathIdsDirectoty(), fileName);
                File.Copy(filePath, newPath, true);
                Ids = ProgramDirectory.GetIds();
    }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnOpenDirDocCommandExecuted(object p)
        {
            try
            {
                var dir = GetPathIdsDirectoty();
                if (Directory.Exists(dir)) Process.Start(dir);
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
                string ext = new FileInfo(fileName).Extension.ToString().Replace(".", "").ToLower();
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
        private string GetPathIdsDirectoty() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), IDS_DIRECTORY);
        public const string IDS_DIRECTORY = "Dental\\Ids";
        private ObservableCollection<FileInfo> GetIds()
        {
            ObservableCollection<FileInfo> Ids = new ObservableCollection<FileInfo>();
            var path = GetPathIdsDirectoty();

            IEnumerable<string> filesNames = new List<string>();
            string[] formats = new string[] { "*.docx", "*.doc", "*.rtf", "*.odt", "*.epub", "*.txt", "*.html", "*.htm", "*.mht", "*.xml" };
            foreach (var format in formats)
            {
                var collection = Directory.EnumerateFiles(path, format).ToList();
                if (collection.Count > 0) filesNames = filesNames.Union(collection);
            }
            foreach (var filePath in filesNames) Ids.Add(new FileInfo(filePath));
            return Ids;
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

        public ICommand EditPlanItemCommand { get; }
        public ICommand SelectPosInClassificatorCommand { get; }
        public ICommand AddRowInPlanCommand { get; }
        public ICommand SaveRowInPlanCommand { get; }
        public ICommand DeleteRowInPlanCommand { get; }
        public ICommand CancelFormPlanCommand { get; }
        public ICommand CancelFormPlanItemCommand { get; }

        private bool CanSelectPosInClassificatorCommandExecute(object p) => true;
        private bool CanOpenFormPlanCommandExecute(object p) => true;
        private bool CanEditPlanItemCommandExecute(object p) => true;
        private bool CanSavePlanCommandExecute(object p) => true;
        private bool CanDeletePlanCommandExecute(object p) => true;

        private bool CanAddRowInPlanCommandExecute(object p) => true;
        private bool CanSaveRowInPlanCommandExecute(object p) => true;
        private bool CanDeleteRowInPlanCommandExecute(object p) => true;
        private bool CanCancelFormPlanCommandExecute(object p) => true;
        private bool CanCancelFormPlanItemCommandExecute(object p) => true;

        private void OnOpenFormPlanCommandExecuted(object p)
        {
            try
            {
                if (p != null)
                {
                    PlanModel = db.TreatmentPlan.FirstOrDefault(i => i.Id == (int)p);
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
                if (PlanModel.Id == 0) 
                {
                    if (!string.IsNullOrEmpty(PlanModel["Name"])) return;
                    db.TreatmentPlan.Add(PlanModel);
                }                            
                int cnt = db.SaveChanges();
                if (cnt > 0) PlanModel.Update();
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
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить план лечения?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    db.TreatmentPlanItems.Where(f => f.TreatmentPlanId == plan.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);                   
                    db.TreatmentPlan.Remove(plan);                   
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCancelFormPlanCommandExecuted(object p)
        {
            try
            {
                if (db.Entry(PlanModel).State == EntityState.Modified)
                {
                    db.Entry(PlanModel).State = EntityState.Unchanged;
                }
                db.SaveChanges();
                PlanWindow.Close();
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
                if (p is TreatmentPlan plan) 
                {
                    PlanItemModel = new TreatmentPlanItems();
                    PlanItemModel.TreatmentPlanId = plan.Id;
                    PlanItemModel.TreatmentPlan = plan;

                    PlanItemWindow = new PlanItemWindow();
                    PlanItemWindow.DataContext = this;
                    PlanItemWindow.ShowDialog();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnEditPlanItemCommandExecuted(object p)
        {
            try
            {
                if (p is TreatmentPlanItems item) 
                {
                    PlanItemModel = item;
                    PlanItemWindow = new PlanItemWindow();
                    PlanItemWindow.DataContext = this;
                    PlanItemWindow.ShowDialog();
                }
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
                if (!string.IsNullOrEmpty(PlanItemModel["Classificator"])) return;
                if (PlanItemModel.Id == 0) 
                {
                    db.TreatmentPlan.FirstOrDefault(f => f.Id == PlanItemModel.TreatmentPlanId)?.TreatmentPlanItems.Add(PlanItemModel);
                } 

                int cnt = db.SaveChanges();
                if (cnt > 0)
                {
                    if (cnt > 0) PlanItemModel.Update();
                    var notification = new Notification();
                    notification.Content = "Позиция в плане лечения сохранена!";
                    var nav = Navigation.Instance;
                    notification.run();
                }
                PlanItemWindow.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
                PlanItemModel = null;
                PlanItemWindow.Close();
            }
        }

        private void OnDeleteRowInPlanCommandExecuted(object p)
        {
            try
            {
                if (!string.IsNullOrEmpty(PlanItemModel["Classificator"]))
                {
                    int x = 0;
                }
                if (p is TreatmentPlanItems item)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить позицию в плане лечения?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;

                    db.Entry(item).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCancelFormPlanItemCommandExecuted(object p) 
        { 
            try
            {
                if (db.Entry(PlanItemModel).State == EntityState.Modified)
                {
                    db.Entry(PlanItemModel).State = EntityState.Unchanged;
                }                                    
                db.SaveChanges();
                PlanItemWindow.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }

        }


        private PlanWindow PlanWindow;
        private PlanItemWindow PlanItemWindow;

        public TreatmentPlanItems PlanItemModel
        {
            get => _PlanItemModel;
            set => Set(ref _PlanItemModel, value);
        }
        private TreatmentPlanItems _PlanItemModel;

        public TreatmentPlan PlanModel
        {
            get => _PlanModel;
            set => Set(ref _PlanModel, value);
        }
        private TreatmentPlan _PlanModel;

        public Visibility IsVisibleItemPlanForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupPlanForm { get; set; } = Visibility.Hidden;

        private ObservableCollection<TreatmentPlan> _PlanGroup;
        public ObservableCollection<TreatmentPlan> PlanGroup
        {
            get => _PlanGroup;
            set => Set(ref _PlanGroup, value);
        }

        public List<Classificator> ClassificatorCategories { get; set; }
        public List<Employee> Employes { get; set; }

        #endregion

        public bool HasUnsavedChanges()
        {
            bool hasUnsavedChanges = false;
            if (Model.FieldsChanges != null) Model.FieldsChanges = PatientInfo.CreateFieldsChanges();
            if (!Model.Equals(ModelBeforeChanges)) hasUnsavedChanges = true;
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
        public ICollection<ClientsGroup> ClientsGroupList { get; set; }

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
            ClientsGroupList = db.ClientsGroup.OrderBy(f => f.Name).ToList();
        }
        
        protected void Update()
        {
            db.Entry(Model).State = EntityState.Modified;
            db.SaveChanges();
        }

    }
}
