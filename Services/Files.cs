using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Dental.Enums;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Infrastructures.Logs;
using Dental.Interfaces.Template;
using Dental.Models;
using Dental.Models.Base;
using Dental.Services;
using Dental.ViewModels;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;

namespace Dental.Services
{
    class ManagementCustomerFiles : ViewModelBase
    {
       /* private  string SectionName { get; set; } 
        public ManagementCustomerFiles(string sectionName = null)
        {
            if (sectionName != null) SectionName = sectionName;
            #region инициализация команд, связанных с прикреплением к карте пациента файлов
            DeleteFileCommand = new LambdaCommand(OnDeleteFileCommandExecuted, CanDeleteFileCommandExecute);
            ExecuteFileCommand = new LambdaCommand(OnExecuteFileCommandExecuted, CanExecuteFileCommandExecute);
            OpenDirectoryCommand = new LambdaCommand(OnOpenDirectoryCommandExecuted, CanOpenDirectoryCommandExecute);
            AttachmentFileCommand = new LambdaCommand(OnAttachmentFileCommandExecuted, CanAttachmentFileCommandExecute);
            #endregion
        }

        #region команды, связанных с прикреплением к карте пациентов файлов     
        public ICommand DeleteFileCommand { get; }
        public ICommand ExecuteFileCommand { get; }
        public ICommand AttachmentFileCommand { get; }
        public ICommand OpenDirectoryCommand { get; }

        private bool CanDeleteFileCommandExecute(object p) => true;
        private bool CanExecuteFileCommandExecute(object p) => true;
        private bool CanAttachmentFileCommandExecute(object p) => true;
        private bool CanOpenDirectoryCommandExecute(object p) => true;

        private void OnOpenDirectoryCommandExecuted(object p)
        {
            try
            {
                var file = p as ClientFiles;
                var dir = Path.GetDirectoryName(file?.Path);
                Process.Start(dir);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                    text: "Невозможно открыть содержащую файл директорию!",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
            }
        }

        private void OnExecuteFileCommandExecuted(object p)
        {
            try
            {
                var file = p as ClientFiles;
                Process.Start(file?.Path);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                   text: "Невозможно запустить файл!",
                   messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAttachmentFileCommandExecuted(object p)
        {
            try
            {
                var fileContent = string.Empty;
                var filePath = string.Empty;
                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "All files (*.*)|*.*|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        filePath = openFileDialog.FileName;
                        ClientFiles file = new ClientFiles();
                        file.Path = filePath;
                        file.DateCreated = DateTime.Today.ToShortDateString();
                        file.Name = Path.GetFileNameWithoutExtension(filePath);
                        file.FullName = Path.GetFileName(filePath);
                        if (Path.HasExtension(filePath))
                        {
                            file.Extension = Path.GetExtension(filePath);
                        }
                        file.Size = new FileInfo(filePath).Length.ToString();

                        if (FindDoubleFile(file.FullName))
                        {
                            var response = ThemedMessageBox.Show(title: "Внимание!", text: "Файл с таким именем уже есть в списке прикрепленных файлов. Вы хотите его заменить?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                            if (response.ToString() == "No") return;
                            var idx = Files.IndexOf(f => (string.Compare(f.FullName, file.FullName, StringComparison.CurrentCulture) == 0));
                            if (idx != 0) return;
                            Files[idx] = file;
                            return;
                        }
                        Files.Insert(0, file);
                    }
                }

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnDeleteFileCommandExecuted(object p)
        {
            try
            {
                var file = p as ClientFiles;
                var item = Files.Where<ClientFiles>(f => string.Compare(f.FullName, file.FullName, StringComparison.CurrentCulture) == 0).FirstOrDefault();
                if (item == null) return;
                if (string.Compare(file.Status, ClientFiles.STATUS_SAVE_RUS, StringComparison.CurrentCulture) == 0)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Вы собираетесь физически удалить файл с компьютера! Вы уверены в своих действиях?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    ProgramDirectory.RemoveFileFromPatientsCard(Model.Id.ToString(), file);
                }
                Files.Remove(file);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private bool FindDoubleFile(string fileName)
        {
            foreach (var file in Files)
            {
                if (string.Compare(file.FullName, fileName, StringComparison.CurrentCulture) == 0) return true;
            }
            return false;
        }

        private bool SaveFiles()
        {
            try
            {
                //если нет директории программы, то создаем ее
                if (!ProgramDirectory.HasMainProgrammDirectory())
                {
                    var _ = ProgramDirectory.CreateMainProgrammDirectoryForPatientCards();
                }
                if (!ProgramDirectory.HasOrgDirectoty())
                {
                    var _ = ProgramDirectory.CreatePatientCardDirectory(Model.Id.ToString());
                }

                ProgramDirectory.Errors.Clear();
                MoveFilesToPatientCardDirectory();
                return true;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
                return false;
            }
        }

        private void MoveFilesToPatientCardDirectory()
        {
            foreach (ClientFiles file in Files)
            {
                if (string.Compare(file.Status, ClientFiles.STATUS_SAVE_RUS, StringComparison.CurrentCulture) == 0) continue;
                ProgramDirectory.SaveInPatientCardDirectory(Model.Id.ToString(), file);
                file.Status = ClientFiles.STATUS_SAVE_RUS;
            }
        }

        public ObservableCollection<ClientFiles> _Files;
        public ObservableCollection<ClientFiles> Files
        {
            get => _Files;
            set => Set(ref _Files, value);
        }*/
    }
}