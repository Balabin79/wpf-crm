using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
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
using Dental.Views.PatientCard;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using DevExpress.Mvvm.DataAnnotations;

namespace Dental.ViewModels
{
    class ClientCardViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private  ApplicationContext db;
        private PatientListViewModel VmList;

        public ClientCardViewModel(int clientId, PatientListViewModel vmList)
        {
            try
            {
                db = new ApplicationContext();
                VmList = vmList;
                Model = db.Clients.Where(f => f.Id == clientId)
                    .Include(f => f.Estimates.Select(i => i.EstimateServiseItems.Select(x => x.Employee)))
                    .Include(f => f.Estimates.Select(i => i.EstimateServiseItems.Select(x => x.Service)))
                    .Include(f => f.Advertising).FirstOrDefault() ?? new Client();
                Files = new ObservableCollection<FileInfo>();
                Ids = ProgramDirectory.GetIds();    

                // Если patientCardNumber == 0, то это создается новая карта клиента, иначе загружаем данные существующего клиента

                if (Model.Id == 0)
                {
                    NumberPatientCard = CreateNewNumberPatientCard().ToString();
                    Model.ClientCardCreatedAt = DateTime.Now.ToShortDateString();

                    // для новой карты пациента все поля по-умолчанию доступны для редактирования
                    IsReadOnly = false;
                    BtnIconEditableHide = false;
                    BtnIconEditableVisible = true;
                }
                else
                {
                    //для существующей карты пациента все поля по-умолчанию недоступны для редактирования
                    NumberPatientCard = Model?.Id.ToString();
                    IsReadOnly = true;
                    BtnIconEditableHide = true;
                    BtnIconEditableVisible = false;

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
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с картой пациента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
     

        #region команды, связанные с общим функционалом карты пациента

        [Command]
        public void Editable()
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

        [Command]
        public void Delete(object p)
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

        [Command]
        public void Save(object p)
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

        [Command]
        public void OpenDirectory(object p)
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

        [Command]
        public void OExecuteFile(object p)
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

        [Command]
        public void AttachmentFile(object p)
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

        [Command]
        public void DeleteFile(object p)
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
            get { return GetProperty(() => Files); }
            set { SetProperty(() => Files, value); }
        }
        #endregion

        [Command]
        public void OpenFormDoc(object p)
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

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public string NumberPatientCard
        {
            get { return GetProperty(() => NumberPatientCard); }
            set { SetProperty(() => NumberPatientCard, value); }
        }

        public bool BtnAfterSaveEnable
        {
            get { return GetProperty(() => BtnAfterSaveEnable); }
            set { SetProperty(() => BtnAfterSaveEnable, value); }
        }

        public Client Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
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

        public bool BtnIconEditableVisible
        {
            get { return GetProperty(() => BtnIconEditableVisible); }
            set { SetProperty(() => BtnIconEditableVisible, value); }
        }

        public bool BtnIconEditableHide
        {
            get { return GetProperty(() => BtnIconEditableHide); }
            set { SetProperty(() => BtnIconEditableHide, value); }
        }
        
        protected void Update()
        {
            db.Entry(Model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ObservableCollection<FileInfo> Ids { get; }
    }
}
