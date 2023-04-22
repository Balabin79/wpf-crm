using B6CRM.Infrastructures.Extensions.Notifications;
using B6CRM.Models;
using B6CRM.Services;
using B6CRM.Services.Files;
using B6CRM.ViewModels.AdditionalFields;
using B6CRM.Views.PatientCard;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI.Navigation;
using License;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace B6CRM.ViewModels.ClientDir
{
    public class MainInfoViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public MainInfoViewModel(Client client)
        {
            db = new ApplicationContext();
            ClientCategoriesLoad();
            Config = db.Config;

            Client = client?.Id > 0 ? db.Clients.FirstOrDefault(f => f.Id == client.Id) : new Client();
            Init(Client);
        }

        #region Права на выполнение команд
        public bool CanOpenDirectory(object p) => Client?.Id != 0;
        public bool CanExecuteFile(object p) => Client?.Id != 0;

        public bool CanCreate(object p) => ((UserSession)Application.Current.Resources["UserSession"]).ClientsEditable;
        public bool CanDelete() => ((UserSession)Application.Current.Resources["UserSession"]).ClientsDelitable;
        public bool CanSave(object p) => ((UserSession)Application.Current.Resources["UserSession"]).ClientsEditable;

        public bool CanAttachmentFile(object p) => ((UserSession)Application.Current.Resources["UserSession"]).ClientsEditable;
        public bool CanDeleteFile(object p) => ((UserSession)Application.Current.Resources["UserSession"]).ClientsEditable;
        #endregion


        private void Init(Client model)
        {
            try
            {
                if (model == null) return;
                ClientInfoViewModel = new ClientInfoViewModel(model);
                IsReadOnly = model?.Id != 0;
              
                PathToUserFiles = Path.Combine(Config.PathToFilesDirectory, model.Guid);
                Files = Directory.Exists(PathToUserFiles) ? new DirectoryInfo(PathToUserFiles).GetFiles().ToObservableCollection() 
                    : 
                    new ObservableCollection<FileInfo>();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void Create(object p)
        {
            try
            {
                Client = new Client();
                ClientInfoViewModel = new ClientInfoViewModel(Client);
                Init(Client);
                
                if(p is ClientCardControl clientCardControl)
                {
                    //clientCardControl.Load("mainInfo");
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }


        [Command]
        public void Save(object p)
        {
            try
            {
                ClientInfoViewModel.Copy(Client);

                /************************/
                if (Status.Licensed && Status.HardwareID != Status.License_HardwareID)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                if (!Status.Licensed && Status.Evaluation_Time_Current > Status.Evaluation_Time)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                /************************/

                if (Client.Id == 0) // новый элемент
                {
                    db.Clients.Add(Client);
                    // если статус анкеты (в архиве или нет) не отличается от текущего статуса списка, то тогда добавить
                    //if (IsArchiveList == (Client.IsInArchive == 1)) Clients?.Insert(0, Client);
                    db.SaveChanges();
                    //EventChangeReadOnly?.Invoke(false); // разблокировать дополнительные поля
                                                        //EventNewClientSaved?.Invoke(Model); // разблокировать команды счетов
                    PathToUserFiles = Path.Combine(Config.PathToFilesDirectory, Client?.Guid);
                    new Notification() { Content = "Новый клиент успешно записан в базу данных!" }.run();
                    if (p is BarButtonItem item)
                    {
                        //SelectedItem();
                    }
                    
                }
                else
                { // редактирование су-щего эл-та
                    //FieldsViewModel?.Save(Model);
                    if (db.SaveChanges() > 0)
                    {
                        //LoadClients(Model.IsInArchive);
                        //IsArchiveList = Model.IsInArchive == 1;
                        //SelectedItem();
                        new Notification() { Content = "Отредактированные данные сохранены в базу данных!" }.run();
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При сохранении данных произошла ошибка!", true);
            }
        }

        [Command]
        public void Delete()
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить карту клиента из базы данных, без возможности восстановления? Также будут удалены счета, планы работ, записи в расписании и все файлы прикрепленные к карте клиента!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;

                new UserFilesManagement(Client.Guid).DeleteDirectory();
                var id = Client?.Id;

                //удалить также в расписании, в планах и в счетах
                db.Appointments.Where(f => f.ClientInfoId == Client.Id)?.ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.Invoices.Include(f => f.InvoiceItems).Where(f => f.ClientId == Client.Id).ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.Plans.Include(f => f.PlanItems).Where(f => f.ClientId == Client.Id).ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.AdditionalClientValue.Where(f => f.ClientId == Client.Id)?.ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.Entry(Client).State = EntityState.Deleted;
                if (db.SaveChanges() > 0) new Notification() { Content = "Карта клиента полностью удалена из базы данных!" }.run();

                // может не оказаться этого эл-та в списке, например, он в статусе "В архиве"
                //var item = Clients.FirstOrDefault(f => f.Guid == Client.Guid);
               // if (item != null) Clients.Remove(item);

                db.InvoiceItems.Where(f => f.InvoiceId == null).ForEach(f => db.Entry(f).State = EntityState.Deleted);
                db.PlanItems.Where(f => f.PlanId == null).ForEach(f => db.Entry(f).State = EntityState.Deleted);
                db.SaveChanges();


                // удаляем файлы 
                if (Directory.Exists(PathToUserFiles)) Directory.Delete(PathToUserFiles);

                //загружаем новую анкету
                Client = new Client();
                Init(Client);
               // SelectedItem();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При удалении карты клиента произошла ошибка!", true);
            }
        }

        [Command]
        public void ClearDate(object p)
        {
            if (p is DateEdit field)
            {
                field.ClearError();
                field.Clear();
                field.ClosePopup();
                field.EditValue = null;
                ClientInfoViewModel.BirthDate = null;
            }
        }



        #region команды, связанных с прикреплением файлов        
        public string PathToUserFiles
        {
            get { return GetProperty(() => PathToUserFiles); }
            set { SetProperty(() => PathToUserFiles, value); }
        }

        public ObservableCollection<FileInfo> Files
        {
            get { return GetProperty(() => Files); }
            set { SetProperty(() => Files, value); }
        }

        [Command]
        public void OpenDirectory(object p)
        {
            try
            {
                if (PathToUserFiles != null && Directory.Exists(PathToUserFiles))
                {
                    var proc = new Process();
                    proc.StartInfo = new ProcessStartInfo(PathToUserFiles)
                    {
                        UseShellExecute = true
                    };
                    proc.Start();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Невозможно открыть содержащую файл директорию!", true);
            }
        }

        [Command]
        public void ExecuteFile(object p)
        {
            try
            {
                if (p is FileInfo file)
                {
                    var proc = new Process();
                    proc.StartInfo = new ProcessStartInfo(file.FullName)
                    {
                        UseShellExecute = true
                    };
                    proc.Start();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Невозможно выполнить загрузку файла!", true);
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

                if (PathToUserFiles != null && !Directory.Exists(PathToUserFiles)) Directory.CreateDirectory(PathToUserFiles);



                File.Copy(file.FullName, Path.Combine(PathToUserFiles, file.Name), true);
                File.SetAttributes(Path.Combine(PathToUserFiles, file.Name), FileAttributes.Normal);

                FileInfo newFile = new FileInfo(Path.Combine(PathToUserFiles, file.Name));
                newFile.CreationTime = DateTime.Now;

                Files = new DirectoryInfo(PathToUserFiles).GetFiles().ToObservableCollection();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
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
                    File.SetAttributes(file.FullName, FileAttributes.Normal);
                    file.Delete();
                    Files = new DirectoryInfo(PathToUserFiles).GetFiles().ToObservableCollection();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
        #endregion


        public void ClientCategoriesLoad() => ClientCategories = db.ClientCategories?.ToArray()?.ToObservableCollection() ?? new ObservableCollection<ClientCategory>();

        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }

        public ClientInfoViewModel ClientInfoViewModel
        {
            get { return GetProperty(() => ClientInfoViewModel); }
            set { SetProperty(() => ClientInfoViewModel, value); }
        }

        public ObservableCollection<ClientCategory> ClientCategories
        {
            get { return GetProperty(() => ClientCategories); }
            set { SetProperty(() => ClientCategories, value); }
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }

    }
}
