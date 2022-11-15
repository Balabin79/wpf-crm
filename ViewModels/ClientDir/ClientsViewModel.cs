using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Views.PatientCard;
using Dental.Views.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Services;
using Dental.Models;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Views.Documents;
using Dental.Services.Files;
using Dental.Views.AdditionalFields;
using Dental.ViewModels.AdditionalFields;
using System.IO;
using System.Windows.Media.Imaging;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.ViewModels.Invoices;
using Dental.Models.Base;
using Dental.Infrastructures.Extensions;

namespace Dental.ViewModels.ClientDir
{
    public class ClientsViewModel : DevExpress.Mvvm.ViewModelBase, IImageDeletable, IImageSave
    {
        public delegate void ChangeReadOnly(bool status);
        public event ChangeReadOnly EventChangeReadOnly;

        public delegate void NewClientSaved(Client client);
        public event NewClientSaved EventNewClientSaved;

        // bool чтобы знать есть ли изменения в другой вкладке (для показа уведомления)
        public delegate bool SaveCard(Client client);
        public event SaveCard EventSaveCard;


        public readonly ApplicationContext db;
        public ClientsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                SetCollection();
                Model = new Client();
                Init(Model);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Список клиентов\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanOpenFormDocuments() => ((UserSession)Application.Current.Resources["UserSession"]).ClientTemplatesEditable;

        public bool CanOpenFormFields() => ((UserSession)Application.Current.Resources["UserSession"]).ClientAddFieldsEditable;

        public bool CanOpenClientCard(object p) => ((UserSession)Application.Current.Resources["UserSession"]).OpenClientCard;

        public bool CanShowArchive() => true;

        [Command]
        public void OpenFormDocuments()
        {
            try
            {
                Window wnd = Application.Current.Windows.OfType<Window>().Where(w => w.ToString() == DocumentsWindow?.ToString()).FirstOrDefault();
                if (wnd != null)
                {
                    wnd.Activate();
                    return;
                }

                DocumentsWindow = new DocumentsWindow() { DataContext = new ClientsDocumentsViewModel() };
                DocumentsWindow.Show();
            }
            catch 
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Документы\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenFormFields()
        {
            try
            {
                Window wnd = Application.Current.Windows.OfType<Window>().Where(w => w.ToString() == FieldsWindow?.ToString()).FirstOrDefault();
                if (wnd != null)
                {
                    wnd.Activate();
                    return;
                }

                FieldsWindow = new ClientFieldsWindow() { DataContext = new AdditionalClientFieldsViewModel() };
                FieldsWindow.Show();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Дополнительные поля\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenClientCard(object p)
        {
            try
            {
               // ClientCardWin = (p != null) ? new ClientCardWindow((int)p, this) : new ClientCardWindow(0, this);
                //ClientCardWin?.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Карта клиента\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void ShowArchive()
        {
            try
            {
                IsArchiveList = !IsArchiveList; 
                SetCollection(IsArchiveList);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public bool IsArchiveList
        {
            get { return GetProperty(() => IsArchiveList); }
            set { SetProperty(() => IsArchiveList, value); }
        }

        public ObservableCollection<Client> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        public DocumentsWindow DocumentsWindow { get; set; }
        public ClientFieldsWindow FieldsWindow { get; set; }
        public ClientCardControl ClientCardWin { get; set; }

        public void SetCollection(bool isArhive=false) => Collection = db.Clients
            .Where(f => f.IsInArchive == isArhive)
            //.Select(f => new {f.Id, f.Guid, f.LastName, f.FirstName, f.MiddleName, f.BirthDate, f.Sex, f.Address })
            .OrderBy(f => f.LastName)
            //.OfType<Client>()
            .ToObservableCollection();

        private string PathToClientsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Clients");
        private string PathToClientsPhotoDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Clients", "Photo");

        private void ImgLoading(Client model)
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


        #region Карта клиента
        private void Init(Client model)
        {        
            ClientInfoViewModel = new ClientInfoViewModel(model);
            IsReadOnly = model?.Id != 0;
            Document = new ClientsDocumentsViewModel();
            foreach (var i in Collection) ImgLoading(i);


            FieldsViewModel = new FieldsViewModel(model, this);
            InvoicesViewModel = new InvoicesViewModel(model, db);
            TreatmentStageViewModel = new TreatmentStageViewModel(model, db);

            EventChangeReadOnly += InvoicesViewModel.StatusReadOnly;
            EventChangeReadOnly += FieldsViewModel.ChangedReadOnly;
            EventChangeReadOnly += TreatmentStageViewModel.StatusReadOnly;

            InvoicesViewModel.StatusReadOnly(true);
            TreatmentStageViewModel.StatusReadOnly(true);
            FieldsViewModel.IsReadOnly = true;

            EventSaveCard += FieldsViewModel.Save;
            EventSaveCard += TreatmentStageViewModel.Save;
            EventNewClientSaved += InvoicesViewModel.NewClientSaved;
            EventNewClientSaved += TreatmentStageViewModel.NewClientSaved;

            //fieldsViewModel.EventChangeVisibleTab += clientCardViewModel.SetTabVisibility;
            SetTabVisibility(FieldsViewModel.AdditionalFieldsVisible);
        }

        [Command]
        public void Load(object p)
        {
            try
            {
                if (p is Client model) 
                { 
                    Model = model; 
                    Init(Model); 
                }
            }
            catch (Exception e)
            {

            }
        }

        [Command]
        public void Create()
        {
            try
            {
                Model = new Client();
                Init(Model);
            }
            catch (Exception e)
            {

            }
        }

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
                    if (IsArchiveList == Model.IsInArchive) Collection?.Add(Model);
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
                        if (IsArchiveList == Model.IsInArchive)
                        {
                            var item = Collection.FirstOrDefault(f => f.Id == Model.Id);
                            if (item == null) Collection?.Add(Model); // добавляем
                            else // меняем
                            {
                                Collection?.Remove(item);
                                Collection?.Add(Model);
                            }
                        }
                        else // иначе если статусы отличаются (допустим убрали анкету в архив), то только удалить из отображаемого списка
                        {
                            var item = Collection.FirstOrDefault(f => f.Id == Model.Id);
                            if (item != null)
                            {
                                Collection?.Remove(item);
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
                var item = Collection.FirstOrDefault(f => f.Guid == Model.Guid);
                if (item != null) Collection.Remove(item);

                db.InvoiceItems.Where(f => f.InvoiceId == null).ForEach(f => db.Entry(f).State = EntityState.Deleted);
                db.SaveChanges();
                // удаляем фото 
                var photo = Directory.GetFiles(PathToClientsDirectory).FirstOrDefault(f => f.Contains(Model?.Guid));
                if (photo != null && File.Exists(photo)) File.Delete(photo);
                //загружаем новую анкету
                Model = new Client();
                Init(Model);
               // SetCollection();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При удалении карты клиента произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
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

        public ClientsDocumentsViewModel Document
        {
            get { return GetProperty(() => Document); }
            set { SetProperty(() => Document, value); }
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public Visibility AdditionalFieldsVisible
        {
            get { return GetProperty(() => AdditionalFieldsVisible); }
            set { SetProperty(() => AdditionalFieldsVisible, value); }
        }

        public FieldsViewModel FieldsViewModel
        {
            get { return GetProperty(() => FieldsViewModel); }
            set { SetProperty(() => FieldsViewModel, value); }
        }

        public InvoicesViewModel InvoicesViewModel
        {
            get { return GetProperty(() => InvoicesViewModel); }
            set { SetProperty(() => InvoicesViewModel, value); }
        }

        public TreatmentStageViewModel TreatmentStageViewModel
        {
            get { return GetProperty(() => TreatmentStageViewModel); }
            set { SetProperty(() => TreatmentStageViewModel, value); }
        }

        public void SetTabVisibility(Visibility visibility) => AdditionalFieldsVisible = visibility;

        #endregion


        #region Управление фото

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