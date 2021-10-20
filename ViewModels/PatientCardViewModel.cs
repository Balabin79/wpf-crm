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

namespace Dental.ViewModels
{
    class PatientCardViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public PatientCardViewModel() : this(0) { }

        public PatientCardViewModel(int patientId)
        {
            try
            {
                db = Db.Instance.Context;
                Files = new ObservableCollection<ClientFiles>();

               #region инициализация команд, связанных с общим функционалом карты пациента
               // Команда включения - отключения редактирования полей
                EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);
                SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
                DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
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
                    Model = db.PatientInfo.Where(f => f.Id == patientId).FirstOrDefault();
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
                _Teeth = new PatientTeeth();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с картой пациента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

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
                            if (idx != 0) return; 
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
                if (Model != null && Model.Id !=0) BtnAfterSaveEnable = !IsReadOnly;
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
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Вы уверены, что хотите полностью удалить карту пациента из базы данных? Если необходимо ее просто убрать из списка, воспользуйтесь кнопкой \"Переместить в архив \". Удалить карту пациента с базы данных, без возможности восстановления?",
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
                    Add();
                    BtnAfterSaveEnable = true;
                }
                else
                {
                    notification.Content = "Отредактированные данные пациента сохранены в базу данных!";
                    SaveFiles();
                    Update();
                }
                if (HasUnsavedChanges())
                {
                    notification.run();
                    ModelBeforeChanges = (PatientInfo)Model.Clone();
                }
            } 
            catch(Exception e)
            {
                (new ViewModelLog(e)).run();
            }

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

        public bool IsContinueAfterWarningMessage()
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

            return response.ToString() != "Cancel";
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
        public ObservableCollection<ClientTreatmentPlans> ClientTreatmentPlans { get; set; }

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
            ClientTreatmentPlans = db.ClientTreatmentPlans.OrderBy(f => f.TreatmentPlanNumber).ToObservableCollection();
        }

        private bool CheckingRelatedData()
        {
            return true;
        }

        private void Add()
        {
            Model.Guid = KeyGenerator.GetUniqueKey();
            db.Entry(Model).State = EntityState.Added;
            db.SaveChanges();
        }
        
        private void Update()
        {
            if (string.IsNullOrEmpty(Model.Guid)) KeyGenerator.GetUniqueKey();
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
