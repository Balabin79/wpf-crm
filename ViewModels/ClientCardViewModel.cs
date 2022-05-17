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
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Printing;
using Dental.Services.Files;

namespace Dental.ViewModels
{
    class ClientCardViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        private readonly PatientListViewModel VmList;

        public ClientCardViewModel(int clientId, PatientListViewModel vmList)
        {
            try
            {
                db = new ApplicationContext();
                VmList = vmList;
                Model = db.Clients.Where(f => f.Id == clientId).Include(f => f.Advertising).FirstOrDefault() ?? new Client();
                ClientInfoViewModel = new ClientInfoViewModel(Model);

                Files = new UserFilesManagement(Model.Guid).GetFiles()?.ToObservableCollection();
                Ids = ProgramDirectory.GetIds();    

                IsReadOnly = Model.Id != 0;

       
                /*else
                {

                    if (Model != null && ProgramDirectory.HasPatientCardDirectory(Model.Id.ToString()))
                    {
                        Files = ProgramDirectory.GetFilesFromPatientCardDirectory(Model.Id.ToString()).ToObservableCollection<FileInfo>();                        
                    }
                    if (Directory.Exists(GetPathToPatientCard()))
                    {
                        Files = new DirectoryInfo(GetPathToPatientCard()).GetFiles().ToObservableCollection();
                    }
                }*/


                AdvertisingList = db.Advertising.OrderBy(f => f.Name).ToList();
                Appointments = db.Appointments
                    .Include(f => f.Service).Include(f => f.Employee).Include(f => f.Location).Where(f => f.ClientInfoId == Model.Id).OrderBy(f => f.CreatedAt)
                    .ToArray();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с картой пациента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Editable() => IsReadOnly = !IsReadOnly;

        [Command]
        public void Save()
        {
            try
            {
                ClientInfoViewModel.Copy(Model);
                if (Model.Id == 0)
                {                 
                    db.Clients.Add(Model);
                    VmList?.Collection?.Add(Model);
                    db.SaveChanges();
                    new Notification() { Content = "Новый клиент успешно записан в базу данных!" }.run();
                }
                else
                {
                    if (db.SaveChanges() > 0) 
                    {
                        VmList?.SetCollection();
                        new Notification() { Content = "Отредактированные данные клиента сохранены в базу данных!" }.run();
                    }
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }

        }

        [Command]
        public void Delete()
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить карту клиента из базы данных, без возможности восстановления? Также будут удалены сметы, записи в расписании и все файлы прикрепленные к карте клиента!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;

                new UserFilesManagement(Model.Guid).DeleteDirectory();
                var id = Model?.Id;
                //удалить также в расписании
                db.Entry(Model).State = EntityState.Deleted;
                if (db.SaveChanges() > 0) new Notification() { Content = "Карта клиента полностью удалена из базы данных!" }.run();
               
                if (Application.Current.Resources["Router"] is Navigator nav) nav.LeftMenuClick("Dental.Views.PatientCard.PatientsList");
                VmList.ClientCardWin.Close();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При удалении карты клиента произошла ошибка, перейдите в раздел \"Клиенты\"!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

 

        #region команды, связанных с прикреплением к карте клиентов файлов 
        private const string PATIENTS_CARDS_DIRECTORY = "B6\\Files";
        private string PathToPatientsCards { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PATIENTS_CARDS_DIRECTORY);

        [Command]
        public void OpenDirectory()
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
        public void ExecuteFile(object p)
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
        public void AttachmentFile()
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

                FileInfo newFile = new FileInfo(Path.Combine(path, file.Name)) { CreationTime = DateTime.Now };

              //  var names = new string[] { Model.FullName, "добавлен файл", newFile.Name };

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

                    //var names = new string[] { Model.FullName, "удален файл", file.Name };

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
            //if (Model.Guid == null) Model.Guid = KeyGenerator.GetUniqueKey();
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
            //if (Model.FieldsChanges != null) Model.FieldsChanges = Client.CreateFieldsChanges();
            if (!Model.Equals(ModelBeforeChanges)) hasUnsavedChanges = true;
            return hasUnsavedChanges;
        }

        public bool UserSelectedBtnCancel()
        {
            /* string warningMessage = "";     
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

             return response.ToString() == "No";*/
            return true;
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
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

        public ClientInfoViewModel ClientInfoViewModel
        {
            get { return GetProperty(() => ClientInfoViewModel); }
            set { SetProperty(() => ClientInfoViewModel, value); }
        }

        public Client ModelBeforeChanges { get; set; }

        public ICollection<Advertising> AdvertisingList { get; set; }
        public ICollection<Appointments> Appointments { get; set; }

        public ICollection<string> GenderList
        {
            get => _GenderList;
        }

        private readonly ICollection<string> _GenderList = new List<string> { "Мужчина", "Женщина" };

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

        /**********************************************/
    }
}
