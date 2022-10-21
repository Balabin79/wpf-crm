using System;
using System.Linq;
using Dental.Models;
using System.Data.Entity;
using System.Collections;
using System.Windows.Media.Imaging;
using System.IO;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Input;
using Dental.Services;
using System.Collections.Generic;
using Dental.Infrastructures.Logs;
using Dental.Views.EmployeeDir;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Views.Documents;
using Dental.Services.Files;
using Dental.Views.AdditionalFields;
using Dental.ViewModels.AdditionalFields;

namespace Dental.ViewModels.EmployeeDir
{
    public class ListEmployeesViewModel : DevExpress.Mvvm.ViewModelBase
    {
        public readonly ApplicationContext db;
        public ListEmployeesViewModel()
        {
            try
            {
                db = new ApplicationContext();
                SetCollection();
                foreach (var i in Collection) ImgLoading(i);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Список сотрудников\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanOpenFormEmployeeCard(object p) => ((UserSession)Application.Current.Resources["UserSession"]).OpenEmployeeCard;
        public bool CanOpenFormFields() => ((UserSession)Application.Current.Resources["UserSession"]).EmployeeAddFieldsEditable;
        public bool CanExpandAll(object p) => true;
        public bool CanNavigateTo(object p) => ((UserSession)Application.Current.Resources["UserSession"]).OpenEmployeeCard;
        public bool CanOpenFormDocuments() => ((UserSession)Application.Current.Resources["UserSession"]).EmployeeTemplatesEditable;
        public bool CanShowArchive() => true;

        [Command]
        public void OpenFormEmployeeCard(object p)
        {
            try
            {
                EmployeeWin = (p != null)
                    ? new EmployeeCardWindow(Collection.Where(f => f.Id == (int)p).FirstOrDefault(), this)
                    : new EmployeeCardWindow(new Employee(), this);
                EmployeeWin.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Карта сотрудника\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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

                FieldsWindow = new EmployeeFieldsWindow() { DataContext = new AdditionalEmployeeFieldsViewModel() };
                FieldsWindow.Show();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Дополнительные поля\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void ExpandAll(object p)
        {
            try
            {
                if (p is DevExpress.Xpf.Grid.CardView card)
                {
                    if (card.IsCardExpanded(0)) card.CollapseAllCards();
                    else card.ExpandAllCards();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void NavigateTo(object p)
        {
            try
            {
                if (string.IsNullOrEmpty(p.ToString())) return;

                if (!int.TryParse(p.ToString(), out int param)) param = 0;
                if (Application.Current.Resources["Router"] is Navigator nav)
                {
                    if (param == -1 || param == 0) nav.LeftMenuClick("Dental.Views.EmployeeDir.Employee");
                    else nav.LeftMenuClick(new object[] { "Dental.Views.EmployeeDir.Employee", param });
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

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

                DocumentsWindow = new DocumentsWindow() { DataContext = new EmployeesDocumentsViewModel() };
                DocumentsWindow.Show();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Документы\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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

        public ObservableCollection<Employee> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        public EmployeeCardWindow EmployeeWin { get; set; }
        public DocumentsWindow DocumentsWindow { get; set; }

        public EmployeeFieldsWindow FieldsWindow { get; set; }

        public bool IsArchiveList
        {
            get { return GetProperty(() => IsArchiveList); }
            set { SetProperty(() => IsArchiveList, value); }
        }

        public void SetCollection(bool isArhive = false) => Collection = db.Employes.OrderBy(d => d.LastName).Where(f => f.IsInArchive == isArhive).ToObservableCollection();

        private string PathToEmployeesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Employees");

        private void ImgLoading(Employee model)
        {
            try
            {
                string pathToEmpPhoto = Path.Combine(PathToEmployeesDirectory, model.Guid, "Photo");
                if (Directory.Exists(pathToEmpPhoto))
                {
                    var files = Directory.GetFiles(pathToEmpPhoto);
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
