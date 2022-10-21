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

namespace Dental.ViewModels.ClientDir
{
    public class PatientListViewModel : DevExpress.Mvvm.ViewModelBase
    {
        public readonly ApplicationContext db;
        public PatientListViewModel()
        {
            try
            {
                db = new ApplicationContext();
                SetCollection();
                foreach (var i in Collection) ImgLoading(i);
            }
            catch
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
                ClientCardWin = (p != null) ? new ClientCardWindow((int)p, this) : new ClientCardWindow(0, this);
                ClientCardWin?.ShowDialog();
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
        public ClientCardWindow ClientCardWin { get; set; }

        public void SetCollection(bool isArhive=false) => Collection = db.Clients.OrderBy(f => f.LastName).Where(f => f.IsInArchive == isArhive).ToObservableCollection();

        private string PathToClientsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Clients");

        private void ImgLoading(Client model)
        {
            try
            {
                string pathToClientPhoto = Path.Combine(PathToClientsDirectory, model.Guid, "Photo");
                if (Directory.Exists(pathToClientPhoto))
                {
                    var files = Directory.GetFiles(pathToClientPhoto);
                    if (files.Length > 0) model.Photo = files[0];
                }

                if (!string.IsNullOrEmpty(model.Photo) && File.Exists(model.Photo))
                {
                    using (var stream = new FileStream(model.Photo, FileMode.Open))
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
                else model.Photo = null;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

    }
}