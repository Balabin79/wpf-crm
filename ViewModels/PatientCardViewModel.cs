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

                #region инициализация команд, связанных с общим функционалом карты пациента
                // Команда включения - отключения редактирования полей
                EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);
                SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
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
                AttachmentFileCommand = new LambdaCommand(OnAttachmentFileCommandExecuted, CanAttachmentFileCommandExecute);
                #endregion               

                // Если patientCardNumber == 0, то это создается новая карта пациента, иначе загружаем данные существующего клиента
                if (patientId == 0)
                {
                    Model = new PatientInfo();
                    Model.PatientCardNumber = (CreateNewNumberPatientCard()).ToString();
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
                    IsReadOnly = true;
                    _BtnIconEditableHide = true;
                    _BtnIconEditableVisible = false;
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
        #endregion

        #region команды, связанных с прикреплением к карте пациентов файлов     
        public ICommand DeleteFileCommand { get; }
        public ICommand ExecuteFileCommand { get; }
        public ICommand AttachmentFileCommand { get; }

        private bool CanDeleteFileCommandExecute(object p) => true;
        private bool CanExecuteFileCommandExecute(object p) => true;
        private bool CanAttachmentFileCommandExecute(object p) => true;

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
                        //Get the path of specified file
                        filePath = openFileDialog.FileName;
                        ClientFiles file = new ClientFiles();
                        file.Path = filePath;
                        file.DateCreated = DateTime.Today.ToShortDateString();
                        file.Name = "vvv";
                        TempFiles.Add(file);
                        // Process.Start(filePath);

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
                int x = 0;

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion

        #region команды, связанные с общим функционалом карты пациента
        public ICommand EditableCommand { get; }
        private bool CanEditableCommandExecute(object p) => true;
        private void OnEditableCommandExecuted(object p)
        {
            IsReadOnly = !IsReadOnly;
            BtnIconEditableHide = IsReadOnly;
            BtnIconEditableVisible = !IsReadOnly;
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
                }
                else
                {
                    notification.Content = "Отредактированные данные пациента сохранены в базу данных!";
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
            return !Model.Equals(ModelBeforeChanges);
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

        public ObservableCollection<ClientFiles> Files { get; set; }
        public ObservableCollection<ClientFiles> TempFiles { get; set; } = new ObservableCollection<ClientFiles>();

        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set => Set(ref _IsReadOnly, value);
        }

        public PatientTeeth _Teeth;
        public PatientTeeth Teeth
        {
            get => _Teeth;
            set => Set(ref _Teeth, value);
        }

        private PatientInfo _Model;
        public PatientInfo Model
        {
            get => _Model;
            set => Set(ref _Model, value);
        }


        public PatientInfo ModelBeforeChanges { get; set; }

        public IEnumerable<string> DiscountGroupList { get; set; }
        public ICollection<string> AdvertisingList { get; set; }
        public IEnumerable<string> ClientsGroupList { get; set; }
        public ObservableCollection<ClientTreatmentPlans> ClientTreatmentPlans { get; set; }

        public ICollection<string> GenderList
        {
            get => _GenderList;
        }
        private readonly ICollection<string> _GenderList = new List<string> { "Мужчина", "Женщина" };

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

        private int CreateNewNumberPatientCard()
        {
            var id = db.PatientInfo?.OrderBy(f => f.Id).Select(f => f.Id)?.ToList()?.LastOrDefault();
            if (id == null) return 1;
            return (int)id++;
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
            DiscountGroupList = db.DiscountGroups.OrderBy(f => f.Name).Select(f => f.Name).ToList();
            AdvertisingList = db.Advertising.OrderBy(f => f.Name).Select(f => f.Name).ToList();
            ClientsGroupList = db.ClientsGroup.OrderBy(f => f.Name).Select(f => f.Name).ToList();
            ClientTreatmentPlans = db.ClientTreatmentPlans.OrderBy(f => f.TreatmentPlanNumber).ToObservableCollection();
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

        private void Delete(ObservableCollection<DiscountGroups> collection)
        {
            collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
        }

    }
}
