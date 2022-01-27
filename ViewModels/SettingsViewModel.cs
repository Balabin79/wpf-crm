using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Models.Base;
using Dental.Services;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;

namespace Dental.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public SettingsViewModel()
        {
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);

            IsReadOnly = true;
            try
            {
                db = new ApplicationContext();
                Model = GetModel();
                if (Model != null)
                {
                    IsReadOnly = true;
                    _BtnIconEditableHide = true;
                    _BtnIconEditableVisible = false;
                }
                else
                {
                    IsReadOnly = false;
                    _BtnIconEditableHide = false;
                    _BtnIconEditableVisible = true;
                }
                ModelBeforeChanges = (Settings)Model.Clone();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Настройки\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }

            Navigator.HasUnsavedChanges = HasUnsavedChanges;
            Navigator.UserSelectedBtnCancel = UserSelectedBtnCancel;
            ProgramLaunchOptions = new string[]
            {
                "Расписание сотрудников(по умолчанию)",
                "Начинать с последней посещенной страницы",
                "Начинать с выбранной страницы"
            };
            PageList = Helper.Pages;
        }

        #region Блокировка полей
        public ICommand EditableCommand { get; }
        private bool CanEditableCommandExecute(object p) => true;
        private void OnEditableCommandExecuted(object p)
        {
            try
            {
                IsReadOnly = !IsReadOnly;
                BtnIconEditableHide = IsReadOnly;
                BtnIconEditableVisible = !IsReadOnly;
                if (Model != null && Model.Id != 0) BtnAfterSaveEnable = !IsReadOnly;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private bool _BtnIconEditableVisible;
        public bool BtnIconEditableVisible
        {
            get => _BtnIconEditableVisible;
            set => Set(ref _BtnIconEditableVisible, value);
        }

        private bool _BtnIconEditableHide;
        public bool BtnIconEditableHide
        {
            get => _BtnIconEditableHide;
            set => Set(ref _BtnIconEditableHide, value);
        }

        public bool _BtnAfterSaveEnable = false;
        public bool BtnAfterSaveEnable
        {
            get => _BtnAfterSaveEnable;
            set => Set(ref _BtnAfterSaveEnable, value);
        }

        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set => Set(ref _IsReadOnly, value);
        }
        #endregion

        #region Управление моделью
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        private bool CanSaveCommandExecute(object p) => true;
        private bool CanDeleteCommandExecute(object p) => true;

        private void OnSaveCommandExecuted(object p)
        {
            try
            {
                if (Model == null) return;
                var notification = new Notification();
                if (Model.Id == 0) db.Settings.Add(Model);

                db.SaveChanges();
                notification.Content = "Изменения сохранены в базу данных!";

                if (HasUnsavedChanges())
                {
                    notification.run();
                    ModelBeforeChanges = (Settings)Model.Clone();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public Settings ModelBeforeChanges { get; set; }

        public bool HasUnsavedChanges()
        {
            bool hasUnsavedChanges = false;
            if (Model?.FieldsChanges != null) Model.FieldsChanges = new List<string>();
            if (!Model.Equals(ModelBeforeChanges)) hasUnsavedChanges = true;
            return hasUnsavedChanges;
        }

        public bool UserSelectedBtnCancel()
        {
            string warningMessage = "\nПоля: ";
            foreach (var fieldName in Model.FieldsChanges)
            {
                warningMessage += " " + "\"" + fieldName + "\"" + ",";
            }
            warningMessage = warningMessage.Remove(warningMessage.Length - 1);

            var response = ThemedMessageBox.Show(title: "Внимание", text: "В форме \"Настройки\" имеются несохраненные изменения! Если вы не хотите их потерять, то нажмите кнопку \"Отмена\", а затем кнопку сохранить (иконка с дискетой).\nИзменения:" + warningMessage,
               messageBoxButtons: MessageBoxButton.OKCancel, icon: MessageBoxImage.Warning);

            return response.ToString() == "Cancel";
        }

        public Settings model;
        public Settings Model
        {
            get => model;
            set => Set(ref model, value);
        }

        public ICollection<string> ProgramLaunchOptions { get; }
        public ICollection<string> PageList { get; }

        public int? SelectedStartPageIdx { get; set; }
        public int? SelectedProgramLaunchOptionsIdx 
        {
            get => selectedProgramLaunchOptionsIdx; 
            set
            {
                if (selectedProgramLaunchOptionsIdx == 2 && IsReadOnly) EnabledPageList = true; 
                else EnabledPageList = false;
                Set(ref selectedProgramLaunchOptionsIdx, value);
            } 
        }
        public int? selectedProgramLaunchOptionsIdx;

        public bool EnabledPageList { get; set; } = false;

        private Settings GetModel() => db.Settings?.FirstOrDefault() ?? new Settings();
        #endregion

    }
}
