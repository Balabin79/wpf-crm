using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Views.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace Dental.ViewModels
{
    class PatientCardViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public PatientCardViewModel()
        {
            try
            {
                db = new ApplicationContext();
                IsReadOnly = true;
                _BtnIconEditableHide = true;
                _BtnIconEditableVisible = false;
                Model = new PatientInfo();
                Model.PatientCardNumber = (CreateNewNumberPatientCard()).ToString();
                Model.PatientCardCreatedAt = DateTime.Now.ToShortDateString();

                ClickToothGreenCommand = new LambdaCommand(OnClickToothGreenCommandExecuted, CanClickToothGreenCommandExecute);
                ClickToothYelPlCommand = new LambdaCommand(OnClickToothYelPlCommandExecuted, CanClickToothYelPlCommandExecute);
                ClickToothYelCorCommand = new LambdaCommand(OnClickToothYelCorCommandExecuted, CanClickToothYelCorCommandExecute);
                ClickToothImpCommand = new LambdaCommand(OnClickToothImpCommandExecuted, CanClickToothImpCommandExecute);
                ClickToothRedRCommand = new LambdaCommand(OnClickToothRedRCommandExecuted, CanClickToothRedRCommandExecute);
                ClickToothRedPtCommand = new LambdaCommand(OnClickToothRedPtCommandExecuted, CanClickToothRedPtCommandExecute);
                ClickToothRedPCommand = new LambdaCommand(OnClickToothRedPCommandExecuted, CanClickToothRedPCommandExecute);
                ClickToothRedCCommand = new LambdaCommand(OnClickToothRedCCommandExecuted, CanClickToothRedCCommandExecute);
                ClickToothGrayCommand = new LambdaCommand(OnClickToothGrayCommandExecuted, CanClickToothGrayCommandExecute);
                DeleteFileCommand = new LambdaCommand(OnDeleteFileCommandExecuted, CanDeleteFileCommandExecute);
                AttachmentFileCommand = new LambdaCommand(OnAttachmentFileCommandExecuted, CanAttachmentFileCommandExecute);

                EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);

                DiscountGroupList = db.DiscountGroups.OrderBy(f => f.Name).ToObservableCollection();
                AdvertisingList = db.Advertising.OrderBy(f => f.Name).ToObservableCollection();
                ClientsGroupList = db.ClientsGroup.OrderBy(f => f.Name).ToObservableCollection();
                ClientTreatmentPlans = db.ClientTreatmentPlans.OrderBy(f => f.TreatmentPlanNumber).ToObservableCollection();


             
                _Teeth = new PatientTeeth();

            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с данным разделом!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }

        }
      
        public ICommand ClickToothGreenCommand { get; }
        public ICommand ClickToothYelPlCommand { get; }
        public ICommand ClickToothYelCorCommand { get; }
        public ICommand ClickToothImpCommand { get; }
        public ICommand ClickToothRedRCommand { get; }
        public ICommand ClickToothRedPtCommand { get; }
        public ICommand ClickToothRedPCommand { get; }
        public ICommand ClickToothRedCCommand { get; }
        public ICommand ClickToothGrayCommand { get; }
        public ICommand EditableCommand { get; }
        public ICommand DeleteFileCommand { get; }
        public ICommand AttachmentFileCommand { get; }

        private bool CanClickToothGreenCommandExecute(object p) => true;
        private bool CanClickToothYelPlCommandExecute(object p) => true;
        private bool CanClickToothYelCorCommandExecute(object p) => true;
        private bool CanClickToothImpCommandExecute(object p) => true;
        private bool CanClickToothRedRCommandExecute(object p) => true;
        private bool CanClickToothRedPtCommandExecute(object p) => true;
        private bool CanClickToothRedPCommandExecute(object p) => true;
        private bool CanClickToothRedCCommandExecute(object p) => true;
        private bool CanClickToothGrayCommandExecute(object p) => true;
        private bool CanEditableCommandExecute(object p) => true;
        private bool CanDeleteFileCommandExecute(object p) => true;
        private bool CanAttachmentFileCommandExecute(object p) => true;

        private void OnClickToothGreenCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothGreenCommandExecuted");

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothYelPlCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothYelPlCommandExecuted");

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothYelCorCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothYelCorCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothImpCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothImpCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothRedRCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothRedRCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothRedPtCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothRedPtCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothRedPCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothRedPCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothRedCCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothRedCCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 
        
        private void OnClickToothGrayCommandExecuted(object p)
        {
            try
            {
                SetToothState(p, "OnClickToothGrayCommandExecuted");
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnEditableCommandExecuted(object p)
        {
            IsReadOnly = !IsReadOnly;
            BtnIconEditableHide = IsReadOnly;
            BtnIconEditableVisible = !IsReadOnly;                            
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
                        //Get the path of specified file
                        filePath = openFileDialog.FileName;
                        ClientFiles file = new ClientFiles();
                        file.Path = filePath;
                        file.DateCreated = DateTime.Today.ToShortDateString();
                        file.Name = "vvv";
                        TempFiles.Add(file);
                       // Process.Start(filePath);

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
                int x = 0;

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public ObservableCollection<ClientFiles> Files { get; set; }
        public ObservableCollection<ClientFiles> TempFiles { get; set; } = new ObservableCollection<ClientFiles>();

        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set => Set(ref _IsReadOnly, value);
        }

        public PatientTeeth _Teeth;
        public PatientTeeth Teeth
        {
            get => _Teeth;
            set => Set(ref _Teeth, value);
        }

        private PatientInfo _Model;
        public PatientInfo Model
        {
            get => _Model;
            set => Set(ref _Model, value);
        }


        public string SelectedGender { get; set; }
        public object SelectedDiscountGroups { get; set; }
        public object SelectedAdvertisings { get; set; }
        public string SelectedClientsGroup { get; set; }




        public IEnumerable<DiscountGroups> DiscountGroupList { get; set; }
        public ICollection<Advertising> AdvertisingList { get; set; }
        public ICollection<ClientsGroup> ClientsGroupList { get; set; }
        public ObservableCollection<ClientTreatmentPlans> ClientTreatmentPlans { get; set; }

        public ICollection<string> GenderList
        {
            get => _GenderList;
        }
        private readonly ICollection<string> _GenderList = new List<string> { "Мужчина", "Женщина" };

        private void SetToothState(object p, string methodName)
        {
            Tooth tooth = p as Tooth;
            if (tooth == null) return;

            switch (methodName) {
                case "OnClickToothGreenCommandExecuted" : tooth.ToothImagePath = PatientTeeth.ImgPathGreen; tooth.Abbr = ""; break;
                case "OnClickToothYelPlCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathYellow; tooth.Abbr = PatientTeeth.Plomba; break;
                case "OnClickToothYelCorCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathYellow; tooth.Abbr = PatientTeeth.Coronka; break;
                case "OnClickToothImpCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathImp; tooth.Abbr = ""; break;
                case "OnClickToothRedRCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathRed; tooth.Abbr = PatientTeeth.Radiks; break;
                case "OnClickToothRedPtCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathRed; tooth.Abbr = PatientTeeth.Periodontit; break;
                case "OnClickToothRedPCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathRed; tooth.Abbr = PatientTeeth.Pulpit; break;
                case "OnClickToothRedCCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathRed; tooth.Abbr = PatientTeeth.Caries; break;
                case "OnClickToothGrayCommandExecuted": tooth.ToothImagePath = PatientTeeth.ImgPathGray; tooth.Abbr = ""; break;
            }
        }

        private int CreateNewNumberPatientCard()
        {
            var id = db.PatientInfo?.OrderBy(f => f.Id).Select(f => f.Id)?.ToList()?.LastOrDefault();
            if (id == null) return 1;
            return (int)id++;
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
    }
}
