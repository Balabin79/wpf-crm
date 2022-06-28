using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Dental.Services.Files
{
    public abstract class AbstractFilesManagement : DevExpress.Mvvm.ViewModelBase
    {
        public AbstractFilesManagement(string path) 
        { 
            PathTo = path;
            Files = GetFiles().ToObservableCollection();
        }

        virtual public IEnumerable<FileInfo> GetFiles()
        {
            try
            {
                return new DirectoryInfo(PathTo).GetFiles();
            }
            catch
            {
                return new FileInfo[]{};
            }          
        }

        virtual public DirectoryInfo CreateDirectory()
        {
            try
            {
                return Directory.Exists(PathTo) ? Directory.CreateDirectory(PathTo) : new DirectoryInfo(PathTo);
            }
            catch
            {
                return new DirectoryInfo(PathTo);
            }
        }

        [Command]
        public void OpenDirectory()
        {
            try
            {
                if (Directory.Exists(PathTo)) Process.Start(PathTo);
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при открытии директории!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void ExecuteFile(object p)
        {
            try
            {
                if (p is FileInfo file) Process.Start(file.FullName);
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Невозможно выполнить загрузку файла!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void AttachmentFile()
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

                if (!Directory.Exists(PathTo)) Directory.CreateDirectory(PathTo);

                File.Copy(file.FullName, Path.Combine(PathTo, file.Name), true);

                FileInfo newFile = new FileInfo(Path.Combine(PathTo, file.Name)) { CreationTime = DateTime.Now };

                Files = new DirectoryInfo(PathTo).GetFiles().ToObservableCollection();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке прикрепить файл!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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
                    file.Delete();
                    Files = new DirectoryInfo(PathTo).GetFiles().ToObservableCollection();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке удалить файл!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }


        virtual public void DeleteDirectory() 
        { 
            if (Directory.Exists(PathTo)) Directory.Delete(PathTo, true); 
        }

        virtual public void RemoveFile(FileInfo file) => file.Delete();

        virtual public void SaveFile(FileInfo file) => File.Copy(file.FullName, Path.Combine(PathTo, file.FullName), true);

        virtual protected string PathTo { get; }

        public ObservableCollection<FileInfo> Files
        {
            get { return GetProperty(() => Files); }
            set { SetProperty(() => Files, value); }
        }
    }
}
