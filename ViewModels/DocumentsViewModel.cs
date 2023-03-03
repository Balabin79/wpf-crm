using Dental.Models;
using Dental.Services;
using Dental.Services.Files;
using Dental.Views.Documents;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using DevExpress.XtraRichEdit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm.Native;
using System.Diagnostics;
using Dental.Views.WindowForms;
using Dental.Infrastructures.Converters;
using DevExpress.Xpf.Printing;

namespace Dental.ViewModels
{
    public class DocumentsViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public DocumentsViewModel()
        {
            db = new ApplicationContext();
            Config = new Config();
            PathToDir = Config.PathToClientsDocumentsDirectory;
            IsReadOnly = true;

            Clients = db.Clients.Where(f => f.IsInArchive == false).OrderBy(f => f.LastName).ToObservableCollection() ?? new ObservableCollection<Client>();

            Employees = db.Employes.Where(f => f.IsInArchive == false).OrderBy(f => f.LastName).ToObservableCollection() ?? new ObservableCollection<Employee>();

            //foreach (var i in Employees) i.IsVisible = false;
            LoadDocuments();            
        }

        [Command]
        public void Editable() => IsReadOnly = !IsReadOnly;

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }
        #region Документы

        [Command]
        public void OpenFormDocuments()
        {
            /*try
            {
                Window wnd = Application.Current.Windows.OfType<Window>().Where(w => w.ToString() == DocumentsWindow?.ToString()).FirstOrDefault();
                if (wnd != null)
                {
                    wnd.Activate();
                    return;
                }

                var vm = new DocumentsViewModel();
                vm.EventUpdateDocuments += LoadDocuments;
                DocumentsWindow = new DocumentsWindow() { DataContext = vm };
                DocumentsWindow.Show();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Документы\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }*/
        }

        public void LoadDocuments()
        {
            try
            {
                Documents = new ObservableCollection<FileInfo>();
                if (Directory.Exists(Config.PathToClientsDocumentsDirectory))
                {
                    IEnumerable<string> filesNames = new List<string>();
                    string[] formats = new string[] { "*.docx", "*.doc", "*.rtf", "*.odt", "*.epub", "*.txt", "*.html", "*.htm", "*.mht", "*.xml" };
                    foreach (var format in formats)
                    {
                        var collection = Directory.EnumerateFiles(Config.PathToClientsDocumentsDirectory, format).ToList();
                        if (collection.Count > 0) filesNames = filesNames.Union(collection);
                    }
                    foreach (var filePath in filesNames) Documents.Add(new FileInfo(filePath));
                }
            }
            catch (Exception e)
            {

            }

        }

        public ObservableCollection<FileInfo> Documents
        {
            get { return GetProperty(() => Documents); }
            set { SetProperty(() => Documents, value); }
        }

        //public DocumentsWindow DocumentsWindow { get; set; }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }
        #endregion


        [Command]
        public void OpenFormDocEdit(object p)
        {
            try
            {
                string fileName = p?.ToString();
                if (fileName != null && File.Exists(fileName))
                {
                    DocWindow = new IDSWindow() { DataContext = this };
                    DocWindow.RichEdit.LoadDocument(fileName, GetDocumentFormat(fileName));
                    DocWindow.Show();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при открытии формы документа!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void DeleteDoc(object p)
        {
            try
            {
                string fileName = p?.ToString();
                if (fileName != null && File.Exists(fileName))
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить документ?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    File.Delete(fileName);
                    LoadDocuments();
                    //Files = GetFiles().ToObservableCollection();
                   // PrintMenuUpdating?.Invoke();
                    //EventUpdateDocuments?.Invoke();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке удаления документа!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void ImportDoc()
        {
            try
            {
                var filePath = string.Empty;
                var fileName = string.Empty;

                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));

                    openFileDialog.Filter = "Office Files(*.docx; *.doc; *.rtf; *.odt; *.epub; *.txt; *.html; *.htm; *.mht; *.xml) | *.docx; *.doc; *.rtf; *.odt; *.epub; *.txt; *.html; *.htm; *.mht; *.xml";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        foreach (var file in Documents)
                        {
                            if (openFileDialog.SafeFileName == file.Name)
                            {
                                var response = ThemedMessageBox.Show(title: "Внимание!", text: "Документ с таким именем уже существует. Вы хотите его заменить?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                                if (response.ToString() == "No") return;
                            }
                        }
                        fileName = openFileDialog.SafeFileName;
                        filePath = openFileDialog.FileName;
                    }
                    openFileDialog.Dispose();
                }
                if (string.IsNullOrEmpty(fileName)) return;
                var newPath = Path.Combine(PathToDir, fileName);
                File.Copy(filePath, newPath, true);
                LoadDocuments();
                //PrintMenuUpdating?.Invoke();
                //EventUpdateDocuments?.Invoke();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке импорта документа!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenDirDoc()
        {
            if (Directory.Exists(PathToDir)) Process.Start(PathToDir);
        }

        [Command]
        public void OpenFormDoc(object p)
        {
            try
            {
                if (p is FileInfo file && file != null )
                {
                    DocWindow = new IDSWindow() { DataContext = this };
                    var richEdit = DocWindow.RichEdit;
                    richEdit.ReadOnly = true;
                    richEdit.LoadDocument(file.FullName, GetDocumentFormat(file.FullName));
                    richEdit.DocumentSaveOptions.CurrentFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), file.Name);
                    richEdit.RtfText = new RtfParse(richEdit.RtfText, ClientSearch, EmployeeSearch).Run();
                    DocWindow.Show();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму документа!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        private DocumentFormat GetDocumentFormat(string fileName)
        {
            try
            {
                string ext = new FileInfo(fileName).Extension.ToString().Replace(".", "").ToLower();
                switch (ext)
                {
                    case "rtf": return DocumentFormat.Rtf;
                    case "doc": return DocumentFormat.Doc;
                    case "docx": return DocumentFormat.Doc;
                    case "html": return DocumentFormat.Html;
                    case "htm": return DocumentFormat.Html;
                    case "mht": return DocumentFormat.Mht;
                    case "epub": return DocumentFormat.ePub;
                    case "txt": return DocumentFormat.PlainText;
                    case "odt": return DocumentFormat.OpenDocument;
                    default: return DocumentFormat.PlainText;
                }
            }
            catch
            {
                return DocumentFormat.PlainText;
            }
        }

        #region Печать
        [Command]
        public void Print(object p)
        {
            var pathToFile = p?.ToString();

            if (string.IsNullOrEmpty(pathToFile)) return;

            if (File.Exists(pathToFile))
            {
                RichEditDocumentServer docServer = new RichEditDocumentServer();

                //Pass the document content to the server  
                docServer.LoadDocument(pathToFile, GetDocumentFormat(pathToFile));
                docServer.HtmlText = new RtfParse(docServer.HtmlText, ClientSearch, EmployeeSearch).Run();

                //Create a new component link 
                LegacyPrintableComponentLink printableComponent = new LegacyPrintableComponentLink(docServer);

                //Create a document to print 
                printableComponent.CreateDocument(true);
                printableComponent.ShowPrintPreview(new DocumentPrint());
            }
        }


        public Employee EmployeeSearch { get; set; }
        public Client ClientSearch { get; set; }
        #endregion






        /*  public ObservableCollection<FileInfo> GetFiles()
          {
              try
              {
                  Files = new ObservableCollection<FileInfo>();
                  if (!Directory.Exists(PathToDir))
                  {
                      Directory.CreateDirectory(PathToDir);

                  }
                  Files = new ObservableCollection<FileInfo>();
                  IEnumerable<string> filesNames = new List<string>();
                  string[] formats = new string[] { "*.docx", "*.doc", "*.rtf", "*.odt", "*.epub", "*.txt", "*.html", "*.htm", "*.mht", "*.xml" };
                  foreach (var format in formats)
                  {
                      var collection = Directory.EnumerateFiles(PathToDir, format).ToList();
                      if (collection.Count > 0) filesNames = filesNames.Union(collection);
                  }
                  foreach (var filePath in filesNames) Files.Add(new FileInfo(filePath));
                  return Files;
              }
              catch
              {
                  return Files;
              }

          }*/

        public ObservableCollection<Client> Clients
        {
            get { return GetProperty(() => Clients); }
            set { SetProperty(() => Clients, value); }
        }

        public ObservableCollection<Employee> Employees
        {
            get { return GetProperty(() => Employees); }
            set { SetProperty(() => Employees, value); }
        }


        virtual protected string PathToDir { get; }
        virtual protected string Guid { get; }


        public IDSWindow DocWindow { get; set; }



    }
}
