using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Input;
using Dental.Enums;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Interfaces.Template;
using Dental.Models;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using System.Windows.Documents;
using Dental.Reports;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Printing;
using System.Windows;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Expressions;
using DevExpress.DataAccess.ObjectBinding;
using System.Drawing;
using Dental.Reports.IDS;
using Dental.Reports.Recommendations;
using System.IO;
using Dental.Services;
using DevExpress.Xpf.Core;
using Dental.Views.WindowForms;
using DevExpress.XtraRichEdit;
using System.Diagnostics;

namespace Dental.ViewModels
{
    class IdsViewModel : ViewModelBase
    {
        public IdsViewModel()
        {
            try
            {
                OpenFormDocEditCommand = new LambdaCommand(OnOpenFormDocEditCommandExecuted, CanOpenFormDocEditCommandExecute);
                DeleteDocCommand = new LambdaCommand(OnDeleteDocCommandExecuted, CanDeleteDocCommandExecute);
                ImportDocCommand = new LambdaCommand(OnImportDocCommandExecuted, CanImportDocCommandExecute);
                OpenDirDocCommand = new LambdaCommand(OnOpenDirDocCommandExecuted, CanOpenDirDocCommandExecute);

                Ids = ProgramDirectory.GetIds();
            }
            catch(Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может некорректно работать с документами!",  messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        // открыть форму и загрузить документ
        public ICommand OpenFormDocEditCommand { get; }
        public ICommand DeleteDocCommand { get; }
        public ICommand ImportDocCommand { get; }
        public ICommand OpenDirDocCommand { get; }

        private bool CanOpenFormDocEditCommandExecute(object p) => true;
        private bool CanDeleteDocCommandExecute(object p) => true;
        private bool CanImportDocCommandExecute(object p) => true;
        private bool CanOpenDirDocCommandExecute(object p) => true;
       

        private void OnOpenFormDocEditCommandExecuted(object p)
        {
            try
            {
                if (p == null) return;
                string fileName = p.ToString();
                if (fileName != null && File.Exists(fileName))
                {
                    IDSWindow = new IDSWindow();
                    IDSWindow.DataContext = this;
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

        private void OnDeleteDocCommandExecuted(object p)
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

        private void OnImportDocCommandExecuted(object p)
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

        private void OnOpenDirDocCommandExecuted(object p)
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
            get => ids;
            set => Set(ref ids, value);
        }
        private ObservableCollection<FileInfo> ids;

        public void OpenFormDoc(Client Model, string fileName)
        {
            try
            {
                if (File.Exists(fileName) && Model != null)
                {
                    FileInfo fileInfo = new FileInfo(fileName);

                    IDSWindow = new IDSWindow();
                    IDSWindow.DataContext = this;
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