﻿using System;
using System.IO;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Dental.Views.WindowForms;
using Dental.Models;
using DevExpress.XtraRichEdit;
using System.Linq;
using Dental.Infrastructures.Converters;

namespace Dental.Services.Files
{
    public class AbstractDocumentsManagement : DevExpress.Mvvm.ViewModelBase
    {
        public AbstractDocumentsManagement(string pathToDir, string guid = null)
        {
            PathToDir = pathToDir;
            Guid = guid;
            Files = GetFiles();
        }

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
                    Files = GetFiles().ToObservableCollection();
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
                        foreach (var file in Files)
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
                Files = GetFiles();
            }
            catch
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
                if (p is DocumentCommandParameters param && param.File != null && param.Model != null)
                {
                    DocWindow = new IDSWindow() { DataContext = this };
                    var richEdit = DocWindow.RichEdit;
                    richEdit.ReadOnly = true;
                    richEdit.LoadDocument(param.File.FullName, GetDocumentFormat(param.File.FullName));
                    richEdit.DocumentSaveOptions.CurrentFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), param.File.Name);
                    richEdit.RtfText = new RtfParse(richEdit.RtfText, param.Model).Run();
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

        public ObservableCollection<FileInfo> GetFiles()
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

        }
      
        virtual protected string PathToDir { get; }
        virtual protected string Guid { get; }

        public ObservableCollection<FileInfo> Files
        {
            get { return GetProperty(() => Files); }
            set { SetProperty(() => Files, value); }
        }

        public IDSWindow DocWindow { get; set; }
    }
}