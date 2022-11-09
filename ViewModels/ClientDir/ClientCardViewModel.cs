using System;
using System.Collections.Generic;
using System.Linq;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Printing;
using Dental.Services.Files;
using Dental.Views.PatientCard;
using System.Windows.Data;
using Dental.Services;
using Dental.Models.Base;
using Dental.Infrastructures.Logs;
using System.IO;
using System.Windows.Media.Imaging;
using Dental.Infrastructures.Extensions;

namespace Dental.ViewModels.ClientDir
{
    class ClientCardViewModel : DevExpress.Mvvm.ViewModelBase, IImageDeletable, IImageSave
    {
        private readonly ApplicationContext db;
        private readonly PatientListViewModel VmList;

        public delegate void ChangeReadOnly(bool status);
        public event ChangeReadOnly EventChangeReadOnly;

        public delegate void NewClientSaved(Client client);
        public event NewClientSaved EventNewClientSaved;

        // bool чтобы знать есть ли изменения в другой вкладке (для показа уведомления)
        public delegate bool SaveCard(Client client);
        public event SaveCard EventSaveCard;

        public ClientCardViewModel(int clientId, PatientListViewModel vmList)
        {
            try
            {
                db = new ApplicationContext();
                VmList = vmList;
                Model = db.Clients.FirstOrDefault(f => f.Id == clientId) ?? new Client();
                ClientInfoViewModel = new ClientInfoViewModel(Model);              

                UserFiles = new UserFilesManagement(Model.Guid);
                Document = new ClientsDocumentsViewModel();    

                IsReadOnly = Model.Id != 0;
                Appointments = db.Appointments
                    .Include(f => f.Service).Include(f => f.Employee).Include(f => f.Location).Where(f => f.ClientInfoId == Model.Id).OrderBy(f => f.CreatedAt)
                    .ToArray();
                AdditionalFieldsVisible = Visibility.Hidden;
                ImgLoading(ClientInfoViewModel);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с картой клиента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanSave() => ((UserSession)Application.Current.Resources["UserSession"]).ClientEditable;
        public bool CanEditable() => ((UserSession)Application.Current.Resources["UserSession"]).ClientEditable;
        public bool CanDelete() => ((UserSession)Application.Current.Resources["UserSession"]).ClientDeletable;
        public bool CanLoadDocForPrint() => true;
        public bool CanPrint() => true;

        [Command]
        public void Editable() 
        {
            IsReadOnly = !IsReadOnly;
            EventChangeReadOnly?.Invoke(IsReadOnly || Model?.Id == 0);
        }

        [Command]
        public void Save()
        {
            try
            {
                bool notificationShowed = false;
                ClientInfoViewModel.Copy(Model);
                if (Model.Id == 0) // новый элемент
                {
                    db.Clients.Add(Model);
                    // если статус анкеты (в архиве или нет) не отличается от текущего статуса списка, то тогда добавить
                    if (VmList?.IsArchiveList == Model.IsInArchive) VmList?.Collection?.Add(Model);
                    db.SaveChanges();
                    EventChangeReadOnly?.Invoke(false); // разблокировать команды счетов
                    EventNewClientSaved?.Invoke(Model); // разблокировать команды счетов
                    new Notification() { Content = "Новый клиент успешно записан в базу данных!" }.run();
                    notificationShowed = true;
                }
                else
                { // редактирование су-щего эл-та
                    if (db.SaveChanges() > 0)
                    {
                        // если статус анкеты (в архиве или нет) не отличается от текущего статуса списка, то поменять элемент(отображение изменений), иначе просто добавить
                        if (VmList?.IsArchiveList == Model.IsInArchive)
                        {
                            var item = VmList?.Collection.FirstOrDefault(f => f.Id == Model.Id);
                            if (item == null) VmList?.Collection?.Add(Model); // добавляем
                            else // меняем
                            { 
                                VmList?.Collection?.Remove(item);
                                VmList?.Collection?.Add(Model); 
                            }
                        } 
                        else // иначе если статусы отличаются (допустим убрали анкету в архив), то только удалить из отображаемого списка
                        {
                            var item = VmList?.Collection.FirstOrDefault(f => f.Id == Model.Id);
                            if (item != null)
                            {
                                VmList?.Collection?.Remove(item);
                            }
                        }                           
                        new Notification() { Content = "Отредактированные данные клиента сохранены в базу данных!" }.run();
                        notificationShowed = true;
                    }
                }
                if (Model != null) 
                { 
                    if (EventSaveCard?.Invoke(Model) == true && !notificationShowed) new Notification() { Content = "Отредактированные данные клиента сохранены в базу данных!" }.run(); 
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с картой клиента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Delete()
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить карту клиента из базы данных, без возможности восстановления? Также будут удалены счета, записи в расписании и все файлы прикрепленные к карте клиента!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;

                new UserFilesManagement(Model.Guid).DeleteDirectory();
                var id = Model?.Id;
                //удалить также в расписании и в счетах
                db.Appointments.Where(f => f.ClientInfoId == Model.Id)?.ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.Invoices.Include(f => f.InvoiceItems).Where(f => f.ClientId == Model.Id).ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.AdditionalClientValue.Where(f => f.ClientId == Model.Id)?.ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.Entry(Model).State = EntityState.Deleted;
                if (db.SaveChanges() > 0) new Notification() { Content = "Карта клиента полностью удалена из базы данных!" }.run();

                // может не оказаться этого эл-та в списке, например, он в статусе "В архиве"
                var item = VmList?.Collection.FirstOrDefault(f => f.Id == Model.Id);
                if (item != null )VmList?.Collection.Remove(item);

                db.InvoiceItems.Where(f => f.InvoiceId == null).ForEach(f => db.Entry(f).State = EntityState.Deleted);
                db.SaveChanges();
                VmList?.ClientCardWin.Close();
                // удаляем фото 
                // удаляем фото 
                var photo = Directory.GetFiles(PathToClientsDirectory).FirstOrDefault(f => f.Contains(Model?.Guid));
                if (photo != null && File.Exists(photo)) File.Delete(photo);
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При удалении карты клиента произошла ошибка, перейдите в раздел \"Клиенты\"!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
       
        public bool HasUnsavedChanges()
        {
            bool hasUnsavedChanges = false;
            if (ClientInfoViewModel.FieldsChanges != null) ClientInfoViewModel.FieldsChanges = ClientInfoViewModel.CreateFieldsChanges();
            if (!ClientInfoViewModel.Equals(Model)) hasUnsavedChanges = true;
            return hasUnsavedChanges;
        }

        public bool UserSelectedBtnCancel()
        {
            string fieldNames = "";
            var warningMessage = "\n";
            foreach (var prop in ClientInfoViewModel.FieldsChanges)
             {
                 fieldNames += " \"" + prop + "\", ";                               
             }
            if (fieldNames.Length > 3) warningMessage += "Поля:" + fieldNames.Remove(fieldNames.Length - 2) + "\n";
            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения!" + warningMessage + "\nПродолжить без сохранения?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
             return response.ToString() == "No";
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
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

        public UserFilesManagement UserFiles
        {
            get { return GetProperty(() => UserFiles); }
            set { SetProperty(() => UserFiles, value); }
        }

        public ClientsDocumentsViewModel Document
        {
            get { return GetProperty(() => Document); }
            set { SetProperty(() => Document, value); }
        }

        public ICollection<Appointments> Appointments { get; set; }

        public ICollection<string> GenderList{ get => _GenderList; }

        private readonly ICollection<string> _GenderList = new List<string> { "Мужчина", "Женщина" };     

        public Visibility AdditionalFieldsVisible
        {
            get { return GetProperty(() => AdditionalFieldsVisible); }
            set { SetProperty(() => AdditionalFieldsVisible, value); }
        }
        public void SetTabVisibility(Visibility visibility) => AdditionalFieldsVisible = visibility;

        #region Печать
        [Command]
        public void Print()
        {
            PrintClientWindow = new PrintClientWindow() { DataContext = this };
            PrintClientWindow.Show();
        }

        [Command]
        public void LoadDocForPrint()
        {
            // Create a link and assign a data source to it.
            // Assign your data templates to different report areas.
            CollectionViewLink link = new CollectionViewLink();
            CollectionViewSource Source = new CollectionViewSource();

            SetSourceCollectttion();
            Source.Source = SourceCollection;


            link.CollectionView = Source.View;
            link.ReportHeaderTemplate = (DataTemplate)PrintClientWindow.Resources["HeaderTemplate"];
            link.DetailTemplate = (DataTemplate)PrintClientWindow.Resources["DetailTemplate"];

            // Associate the link with the Document Preview control.
            PrintClientWindow.preview.DocumentSource = link;

            // Generate the report document 
            // and show pages as soon as they are created.
            link.CreateDocument(true);
        }

        public ICollection<Client> SourceCollection { get; set; } = new List<Client>();

        private void SetSourceCollectttion()
        {
            SourceCollection.Add(Model);
        }
        public PrintClientWindow PrintClientWindow { get; set; }
        #endregion

        #region Управление фото

        private string PathToClientsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Clients");
        private string PathToClientsPhotoDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Clients", "Photo");

        [Command]
        public void ImageSave(object p)
        {
            try
            {
                if (p is ImageEditEx param)
                {
                    if (!Directory.Exists(PathToClientsPhotoDirectory)) Directory.CreateDirectory(PathToClientsPhotoDirectory);

                    var oldPhoto = Directory.GetFiles(PathToClientsPhotoDirectory).FirstOrDefault(f => f.Contains(param?.ImageGuid));

                    if (oldPhoto != null && File.Exists(oldPhoto))
                    {
                        var response = ThemedMessageBox.Show(title: "Вы уверены?", text: "Заменить текущее фото клиента?",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                        if (response.ToString() == "No") return;
                        File.Delete(oldPhoto);
                    }

                    FileInfo photo = new FileInfo(Path.Combine(param.ImagePath));
                    photo.CopyTo(Path.Combine(PathToClientsPhotoDirectory, param.ImageGuid + photo.Extension), true);
                    new Notification() { Content = "Фото клиента сохраненo!" }.run();
                }
            }
            catch (Exception e)
            {

            }
        }

        public void ImageDelete(object p)
        {
            try
            {
                if (p is ImageEditEx img)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить фото клиента?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                    if (response.ToString() == "No") return;

                    var file = Directory.GetFiles(PathToClientsPhotoDirectory).FirstOrDefault(f => f.Contains(img?.ImageGuid));

                    if (file != null) File.Delete(file);
                    img?.Clear();
                    new Notification() { Content = "Фото клиента удалено!" }.run();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void ImgLoading(ClientInfoViewModel model)
        {
            try
            {
                if (Directory.Exists(PathToClientsPhotoDirectory))
                {
                    var file = Directory.GetFiles(PathToClientsPhotoDirectory)?.FirstOrDefault(f => f.Contains(model.Guid));
                    if (file == null) return;

                    using (var stream = new FileStream(file, FileMode.Open))
                    {
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.StreamSource = stream;
                        img.EndInit();
                        img.Freeze();
                        model.Image = img;
                    }
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion
    }
}
