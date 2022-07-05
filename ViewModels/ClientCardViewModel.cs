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

        public delegate void ChangeReadOnly(bool status);
        public event ChangeReadOnly EventChangeReadOnly;


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

                
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с картой клиента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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
                ClientInfoViewModel.Copy(Model);
                if (Model.Id == 0) // новый элемент
                {
                    db.Clients.Add(Model);
                    // если статус анкеты (в архиве или нет) не отличается от текущего статуса списка, то тогда добавить
                    if (VmList?.IsArchiveList == Model.IsInArchive) VmList?.Collection?.Add(Model);
                    db.SaveChanges();
                    EventChangeReadOnly?.Invoke(false); // разблокировать команды счетов
                    new Notification() { Content = "Новый клиент успешно записан в базу данных!" }.run();
                }
                else
                { // редактирование су-щего эл-та
                    if (db.SaveChanges() > 0)
                    {
                        // если статус анкеты (в архиве или нет) не отличается от текущего статуса списка, то поменять элемент(отображение изменений), иначе просто добавить
                        if (VmList?.IsArchiveList == Model.IsInArchive)
                        {
                            var item = VmList.Collection.FirstOrDefault(f => f.Id == Model.Id);
                            if (item == null) VmList?.Collection?.Add(Model); // добавляем
                            else // меняем
                            { 
                                VmList?.Collection?.Remove(item);
                                VmList?.Collection?.Add(Model); 
                            }
                        } 
                        else // иначе если статусы отличаются (допустим убрали анкету в архив), то только удалить из отображаемого списка
                        {
                            var item = VmList.Collection.FirstOrDefault(f => f.Id == Model.Id);
                            if (item != null)
                            {
                                VmList?.Collection?.Remove(item);
                            }
                        }                           
                        new Notification() { Content = "Отредактированные данные клиента сохранены в базу данных!" }.run();
                    }
                }
            }
            catch
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

                db.Invoices.Include(f => f.InvoiceServiceItems).Include(f => f.InvoiceMaterialItems).Where(f => f.ClientId == Model.Id).ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.Entry(Model).State = EntityState.Deleted;
                if (db.SaveChanges() > 0) new Notification() { Content = "Карта клиента полностью удалена из базы данных!" }.run();

                // может не оказаться этого эл-та в списке, например, он в статусе "В архиве"
                var item = VmList.Collection.FirstOrDefault(f => f.Id == Model.Id);
                if (item != null )VmList.Collection.Remove(item);

                db.InvoiceMaterialItems.Where(f => f.InvoiceId == null).ForEach(f => db.Entry(f).State = EntityState.Deleted);
                db.InvoiceServiceItems.Where(f => f.InvoiceId == null).ForEach(f => db.Entry(f).State = EntityState.Deleted);
                db.SaveChanges();
                VmList.ClientCardWin.Close();
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

        public ICollection<AdditionalField> AdditionalFields { get; set; }
        public ICollection<AdditionalClientValue> AdditionalClientValues { get; set; }

        public ICollection<string> GenderList{ get => _GenderList; }

        private readonly ICollection<string> _GenderList = new List<string> { "Мужчина", "Женщина" };
        
        protected void Update()
        {
            db.Entry(Model).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void AdditionalFieldsLoading()
        {
            var parent = db.AdditionalField.FirstOrDefault(f => f.IsSys == 1 && f.SysName == "Client");
            if (parent == null) return;
            // получаем все поля 
            AdditionalFields = db.AdditionalField.Where(f => f.ParentId == parent.Id).ToArray();
            
            // если поля для данного раздела вообще есть, то далее ищем поля, в которых значения есть для данного клиента
            if (AdditionalFields.Count() == 0) return;
            AdditionalClientValues = db.AdditionalClientValue.Where(f => f.ClientId == Model.Id).ToObservableCollection() ?? new ObservableCollection<AdditionalClientValue>();

            //
        }
    }
}
