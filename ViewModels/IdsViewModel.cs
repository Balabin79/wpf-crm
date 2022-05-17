using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.IO;
using Dental.Services;
using DevExpress.Xpf.Core;
using Dental.Views.WindowForms;
using DevExpress.XtraRichEdit;
using System.Diagnostics;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;

namespace Dental.ViewModels
{
    class IdsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        public IdsViewModel()
        {
            try
            {
                Ids = ProgramDirectory.GetIds();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может некорректно работать с документами!",  messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

       [Command]
        public void OpenFormDocEdit(object p)
        {
            try
            {
                if (p == null) return;
                string fileName = p.ToString();
                if (fileName != null && File.Exists(fileName))
                {
                    IDSWindow = new IDSWindow() { DataContext = this };
                    var richEdit = IDSWindow.RichEdit;
                    richEdit.LoadDocument(fileName, GetDocumentFormat(fileName));
                    IDSWindow.Show();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void OnDeleteDocCommandExecuted(object p)
        {
            try
            {
                if (p == null) return;
                string fileName = p.ToString();
                if (fileName != null && File.Exists(fileName))
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить документ с компьютера?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    File.Delete(fileName);
                    Ids = GetIds();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void OnImportDocCommandExecuted()
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
                        string idsDirectory = GetPathIdsDirectoty();

                        foreach (var ids in Ids)
                        {
                            if (openFileDialog.SafeFileName == ids.Name)
                            {
                                var response = ThemedMessageBox.Show(title: "Внимание!", text: "Документ с таким именем уже существует. Вы хотите его заменить?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                                if (response.ToString() == "No") return;
                            }
                        }
                        fileName = openFileDialog.SafeFileName;
                        filePath = openFileDialog.FileName;
                    }
                    openFileDialog.Dispose();
                    //ProgramDirectory.ImportIds(new FileInfo(openFileDialog.FileName));
                }
                var newPath = Path.Combine(GetPathIdsDirectoty(), fileName);
                File.Copy(filePath, newPath, true);
                Ids = ProgramDirectory.GetIds();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void OnOpenDirDocCommandExecuted()
        {
            try
            {
                var dir = GetPathIdsDirectoty();
                if (Directory.Exists(dir)) Process.Start(dir);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                    text: "Невозможно открыть содержащую документы директорию!",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
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
       
        private string GetPathIdsDirectoty() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), IDS_DIRECTORY);
        
        public const string IDS_DIRECTORY = "Dental\\Ids";
        
        private ObservableCollection<FileInfo> GetIds()
        {
            ObservableCollection<FileInfo> Ids = new ObservableCollection<FileInfo>();
            var path = GetPathIdsDirectoty();

            IEnumerable<string> filesNames = new List<string>();
            string[] formats = new string[] { "*.docx", "*.doc", "*.rtf", "*.odt", "*.epub", "*.txt", "*.html", "*.htm", "*.mht", "*.xml" };
            foreach (var format in formats)
            {
                var collection = Directory.EnumerateFiles(path, format).ToList();
                if (collection.Count > 0) filesNames = filesNames.Union(collection);
            }
            foreach (var filePath in filesNames) Ids.Add(new FileInfo(filePath));
            return Ids;
        }

        public IDSWindow IDSWindow { get; set; }

        public ObservableCollection<FileInfo> Ids
        {
            get { return GetProperty(() => Ids); }
            set { SetProperty(() => Ids, value); }
        }

        public void OpenFormDoc(Client Model, string fileName)
        {
            try
            {
                if (File.Exists(fileName) && Model != null)
                {
                    FileInfo fileInfo = new FileInfo(fileName);

                    IDSWindow = new IDSWindow() { DataContext = this };
                    var richEdit = IDSWindow.RichEdit;
                    richEdit.ReadOnly = true;
                    richEdit.LoadDocument(fileName, GetDocumentFormat(fileName));

                    richEdit.DocumentSaveOptions.CurrentFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileInfo.Name);
                    richEdit.RtfText = new RtfParse(richEdit.RtfText, Model).Run();
                    IDSWindow.Show();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
    }
}