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
using DevExpress.Xpf.Core;
using System.Windows;
using System.IO;
using System.Diagnostics;
using Dental.Services;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Infrastructures.Converters;
using Dental.Views.Estimates;
using Dental.Models.Base;

namespace Dental.ViewModels
{
    class ClientCardViewModel : ViewModelBase
    {
        private  ApplicationContext db;
        private PatientListViewModel VmList;

        public ClientCardViewModel(int clientId, PatientListViewModel vmList)
        {
            try
            {
                db = new ApplicationContext();
                VmList = vmList;
                Model = db.Clients.Where(f => f.Id == clientId).Include(f => f.Advertising).FirstOrDefault() ?? new Client();
                Files = new ObservableCollection<FileInfo>();
                Ids = ProgramDirectory.GetIds();

                #region инициализация команд, связанных с общим функционалом карты клиента
                // Команда включения - отключения редактирования полей
                EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);
                SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
                DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
                #endregion           

                #region инициализация команд, связанных с прикреплением к карте клиента файлов
                DeleteFileCommand = new LambdaCommand(OnDeleteFileCommandExecuted, CanDeleteFileCommandExecute);
                ExecuteFileCommand = new LambdaCommand(OnExecuteFileCommandExecuted, CanExecuteFileCommandExecute);
                OpenDirectoryCommand = new LambdaCommand(OnOpenDirectoryCommandExecuted, CanOpenDirectoryCommandExecute);
                AttachmentFileCommand = new LambdaCommand(OnAttachmentFileCommandExecuted, CanAttachmentFileCommandExecute);
                #endregion

                #region инициализация команд, связанных с закладкой "Сметы"
                OpenFormEstimateCommand = new LambdaCommand(OnOpenFormEstimateCommandExecuted, CanOpenFormEstimateCommandExecute);
                EditEstimateItemCommand = new LambdaCommand(OnEditEstimateItemCommandExecuted, CanEditEstimateItemCommandExecute);
                SaveEstimateCommand = new LambdaCommand(OnSaveEstimateCommandExecuted, CanSaveEstimateCommandExecute);
                DeleteEstimateCommand = new LambdaCommand(OnDeleteEstimateCommandExecuted, CanDeleteEstimateCommandExecute);

                SelectPosInClassificatorCommand = new LambdaCommand(OnSelectPosInClassificatorCommandExecuted, CanSelectPosInClassificatorCommandExecute);
                AddRowInEstimateCommand = new LambdaCommand(OnAddRowInEstimateCommandExecuted, CanAddRowInEstimateCommandExecute);
                SaveRowInEstimateCommand = new LambdaCommand(OnSaveRowInEstimateCommandExecuted, CanSaveRowInEstimateCommandExecute);
                DeleteRowInEstimateCommand = new LambdaCommand(OnDeleteRowInEstimateCommandExecuted, CanDeleteRowInEstimateCommandExecute);
                CancelFormEstimateCommand = new LambdaCommand(OnCancelFormEstimateCommandExecuted, CanCancelFormEstimateCommandExecute);
                CancelFormEstimateItemCommand = new LambdaCommand(OnCancelFormEstimateItemCommandExecuted, CanCancelFormEstimateItemCommandExecute);
                #endregion

                OpenFormDocCommand = new LambdaCommand(OnOpenFormDocCommandExecuted, CanOpenFormDocCommandExecute);

                // Если patientCardNumber == 0, то это создается новая карта клиента, иначе загружаем данные существующего клиента

                if (Model.Id == 0)
                {
                    NumberPatientCard = CreateNewNumberPatientCard().ToString();
                    Model.ClientCardCreatedAt = DateTime.Now.ToShortDateString();

                    // для новой карты пациента все поля по-умолчанию доступны для редактирования
                    IsReadOnly = false;
                    _BtnIconEditableHide = false;
                    _BtnIconEditableVisible = true;
                }
                else
                {
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
                ModelBeforeChanges = (Client)Model.Clone();
                AdvertisingList = db.Advertising.OrderBy(f => f.Name).ToList();

                Appointments = db.Appointments
                    .Include(f => f.Service)
                    .Include(f => f.Employee)
                    .Include(f => f.Location)
                    .Where(f => f.ClientInfoId == Model.Id)
                    .OrderBy(f => f.CreatedAt)
                    .ToArray();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с картой пациента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        #region Команды и ф-нал связанный с Планом лечения

        // открыть форму плана лечения
        public ICommand OpenFormEstimateCommand { get; }
        // сохранить новый или отредактированный план лечения
        public ICommand SaveEstimateCommand { get; }
        //удалить план лечения
        public ICommand DeleteEstimateCommand { get; }

        public ICommand EditEstimateItemCommand { get; }
        public ICommand SelectPosInClassificatorCommand { get; }
        public ICommand AddRowInEstimateCommand { get; }
        public ICommand SaveRowInEstimateCommand { get; }
        public ICommand DeleteRowInEstimateCommand { get; }
        public ICommand CancelFormEstimateCommand { get; }
        public ICommand CancelFormEstimateItemCommand { get; }

        private bool CanSelectPosInClassificatorCommandExecute(object p) => true;
        private bool CanOpenFormEstimateCommandExecute(object p) => true;
        private bool CanEditEstimateItemCommandExecute(object p) => true;
        private bool CanSaveEstimateCommandExecute(object p) => true;
        private bool CanDeleteEstimateCommandExecute(object p) => true;

        private bool CanAddRowInEstimateCommandExecute(object p) => true;
        private bool CanSaveRowInEstimateCommandExecute(object p) => true;
        private bool CanDeleteRowInEstimateCommandExecute(object p) => true;
        private bool CanCancelFormEstimateCommandExecute(object p) => true;
        private bool CanCancelFormEstimateItemCommandExecute(object p) => true;

        private void OnOpenFormEstimateCommandExecuted(object p)
        {
            try
            {
                if (p != null)
                {
                    Estimate = db.Estimates.FirstOrDefault(i => i.Id == (int)p);
                }
                else
                {
                    Estimate = new Estimate();
                    Estimate.ClientId = Model.Id;
                    Estimate.StartDate = Estimate.StartDate != null ? Estimate.StartDate : DateTime.Now.ToShortDateString();
                }
                EstimateWindow = new EstimateWindow();
                EstimateWindow.DataContext = this;
                EstimateWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSaveEstimateCommandExecuted(object p)
        {
            try
            {
                if (Estimate.Id == 0)
                {
                    if (!string.IsNullOrEmpty(Estimate["Name"])) return;
                    db.Estimates.Add(Estimate);
                }
                int cnt = db.SaveChanges();
                if (cnt > 0) Estimate.Update();
                EstimateWindow.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnDeleteEstimateCommandExecuted(object p)
        {
            try
            {
                if (p is Estimate plan)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить план лечения?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    db.EstimateServiceItems.Where(f => f.EstimateId == plan.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                    db.Estimates.Remove(plan);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCancelFormEstimateCommandExecuted(object p)
        {
            try
            {
                if (db.Entry(Estimate).State == EntityState.Modified)
                {
                    db.Entry(Estimate).State = EntityState.Unchanged;
                }
                db.SaveChanges();
                EstimateWindow.Close();
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
                    if (parameters.Tree.FocusedRow is Service service)
                    {
                        if (service.IsDir == 1) return;
                        parameters.Popup.EditValue = service;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAddRowInEstimateCommandExecuted(object p)
        {
            try
            {
                if (p is Estimate estimate)
                {
                    EstimateServiceItem = new EstimateServiceItem();
                    EstimateServiceItem.EstimateId = estimate.Id;
                    EstimateServiceItem.Estimate = estimate;

                    EstimateServiceWindow = new EstimateServiceWindow();
                    EstimateServiceWindow.DataContext = this;
                    EstimateServiceWindow.ShowDialog();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnEditEstimateItemCommandExecuted(object p)
        {
            try
            {
                if (p is EstimateServiceItem item)
                {
                    EstimateServiceItem = item;
                    EstimateServiceWindow = new EstimateServiceWindow();
                    EstimateServiceWindow.DataContext = this;
                    EstimateServiceWindow.ShowDialog();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSaveRowInEstimateCommandExecuted(object p)
        {
            try
            {
                if (!string.IsNullOrEmpty(EstimateServiceItem["Classificator"])) return;
                if (EstimateServiceItem.Id == 0)
                {
                    db.Estimates.FirstOrDefault(f => f.Id == EstimateServiceItem.EstimateId)?.EstimateServiseItems.Add(EstimateServiceItem);
                }

                int cnt = db.SaveChanges();
                if (cnt > 0)
                {
                    if (cnt > 0) EstimateServiceItem.Update();
                    var notification = new Notification();
                    notification.Content = "Позиция в плане лечения сохранена!";
                    notification.run();
                }
                EstimateServiceWindow.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
                EstimateServiceItem = null;
                EstimateServiceWindow.Close();
            }
        }

        private void OnDeleteRowInEstimateCommandExecuted(object p)
        {
            try
            {
                if (!string.IsNullOrEmpty(EstimateServiceItem["Classificator"]))
                {
                    int x = 0;
                }
                if (p is EstimateServiceItem item)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить позицию в смете?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
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

        private void OnCancelFormEstimateItemCommandExecuted(object p)
        {
            try
            {
                if (db.Entry(EstimateServiceItem).State == EntityState.Modified)
                {
                    db.Entry(EstimateServiceItem).State = EntityState.Unchanged;
                }
                db.SaveChanges();
                EstimateWindow.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }

        }


        private EstimateWindow EstimateWindow;
        private EstimateServiceWindow EstimateServiceWindow;

        public EstimateServiceItem EstimateServiceItem
        {
            get => estimateServiceItem;
            set => Set(ref estimateServiceItem, value);
        }
        private EstimateServiceItem estimateServiceItem;

        public Estimate Estimate
        {
            get => estimate;
            set => Set(ref estimate, value);
        }
        private Estimate estimate;

        public Visibility IsVisibleItemPlanForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupPlanForm { get; set; } = Visibility.Hidden;

        private ObservableCollection<Estimate> _PlanGroup;
        public ObservableCollection<Estimate> PlanGroup
        {
            get => _PlanGroup;
            set => Set(ref _PlanGroup, value);
        }

        public List<Service> ClassificatorCategories { get; set; }
        public List<Employee> Employes { get; set; }

        #endregion

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
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить карту клиента из базы данных, без возможности восстановления? Также будут удалены услуги, записи в расписании, в рассылках, обращениях клиента и все файлы прикрепленные к карте клиента!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;

                DeleteClientFiles();
                var id = Model?.Id;
                //удалить также в расписании
                db.Entry(Model).State = EntityState.Deleted;
                ModelBeforeChanges = null;
                int cnt = db.SaveChanges();
                // подчищаем остатки
                //db.TreatmentPlanItems.Where(f => f.ClientId == null).ToArray()?.ForEach(f => db.Entry(f).State = EntityState.Deleted);
                db.SaveChanges();

                if (cnt > 0)
                {
                    ActionsLog.RegisterAction(Model.FullName, ActionsLog.ActionsRu["delete"], ActionsLog.SectionPage["ClientInfo"]);
                    var notification = new Notification();
                    notification.Content = "Карта клиента полностью удалена из базы данных!";
                    notification.run();
                }
                if (Application.Current.Resources["Router"] is Navigator nav) nav.LeftMenuClick.Execute("Dental.Views.PatientCard.PatientsList");
                VmList.ClientCardWin.Close();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При удалении карты клиента произошла ошибка, перейдите в раздел \"Клиенты\"!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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
                    db.Clients.Add(Model);
                    VmList?.Collection?.Add(Model);
                    BtnAfterSaveEnable = true;
                    ActionsLog.RegisterAction(Model.FullName, ActionsLog.ActionsRu["add"], ActionsLog.SectionPage["ClientInfo"]);
                }
                else 
                {
                    //db.Entry(Model).State = EntityState.Modified;
                    ActionsLog.RegisterAction(Model.FullName, ActionsLog.ActionsRu["delete"], ActionsLog.SectionPage["ClientInfo"]);
                    notification.Content = "Отредактированные данные клиента сохранены в базу данных!";
                    // Update();
                }
                Model.UpdateFields();
                db.SaveChanges();
                if (HasUnsavedChanges())
                {
                    notification.run();
                    ModelBeforeChanges = (Client)Model.Clone();
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
                if (Directory.Exists(path))  Directory.Delete(path, true);
            } 
            catch
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

                var names = new string[] { Model.FullName, "добавлен файл", newFile.Name };
                ActionsLog.RegisterAction(names, ActionsLog.ActionsRu["add"], ActionsLog.SectionPage["ClientInfo"]);

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

                    var names = new string[] { Model.FullName, "удален файл", file.Name };
                    ActionsLog.RegisterAction(names, ActionsLog.ActionsRu["delete"], ActionsLog.SectionPage["ClientInfo"]);

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



        public ICommand OpenFormDocCommand { get; }
        private bool CanOpenFormDocCommandExecute(object p) => true;
        private void OnOpenFormDocCommandExecuted(object p)
        {
            try
            {
                if (p == null) return;
                string fileName = p.ToString();
                if (fileName != null && File.Exists(fileName))
                {
                    new IdsViewModel().OpenFormDoc(Model, fileName);
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
       
        public bool HasUnsavedChanges()
        {
            bool hasUnsavedChanges = false;
            if (Model.FieldsChanges != null) Model.FieldsChanges = Client.CreateFieldsChanges();
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

            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения!" + warningMessage + "\nПродолжить без сохранения?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

            return response.ToString() == "No";
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


        private Client _Model;
        public Client Model
        {
            get => _Model;
            set => Set(ref _Model, value);
        }

        public Client ModelBeforeChanges { get; set; }

        public ICollection<Advertising> AdvertisingList { get; set; }
        public ICollection<Appointments> Appointments { get; set; }

        public ICollection<string> GenderList
        {
            get => _GenderList;
        }

        private readonly ICollection<string> _GenderList = new List<string> { "Мужчина", "Женщина" };

        private int CreateNewNumberPatientCard()
        {
            var id = db.Clients?.Max(e => e.Id);
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

        
        protected void Update()
        {
            db.Entry(Model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ObservableCollection<FileInfo> Ids { get; }
    }
}
