using Dental.Infrastructures.Converters;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Models.Base;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.ViewModels.ClientDir
{
    public class TreatmentStageViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public TreatmentStageViewModel(Client client = null, ApplicationContext context = null)
        {
            try
            {
                db = context ?? new ApplicationContext();
                Client = client;
                SetCollection();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с вкладкой \"Врачебная\" в карте клиента!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenForm(object p)
        {
            if (p is TreatmentFormParameters parameters)
            {
                if (parameters.Table.FocusedRow is TreatmentStage model)
                {
                    switch(parameters.Name)
                    {
                        //case "Comlaint" : 
                    }
                }
                //parameters.Popup.ClosePopup();
            }
        }

        [Command]
        public void Clear(object p)
        {
            try
            {
                if (p is TreatmentFormParameters parameters)
                {
                    if (parameters.Table.FocusedRow is TreatmentStage model)
                    {
                        switch (parameters.Name)
                        {
                            //case "Comlaint" : 
                            //case "Anamnes" : 
                            //case "Objectivly" : 
                            //case "DescriptionXRay" : 
                            //case "Diagnos" : 
                            //case "Plan" : 
                            //case "Treatment" : 
                            //case "Allergy" : 
                            //case "Recommendations" : 
                        }
                    }
                    //parameters.Popup.ClosePopup();
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке очистить поле!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        /*******/
        public void StatusReadOnly(bool status)
        {
            IsReadOnly = status;
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        [Command]
        public void SelectItemInServiceField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is ITreeModel model)
                    {
                        if (model.IsDir == 1) return;
                        parameters.Popup.EditValue = model;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch
            {

            }
        }

        [Command]
        public void Add(object p)
        {
            try
            {
                var item = new TreatmentStage() { Client = Client, ClientId = Client?.Id, Date = DateTime.Now.ToShortDateString().ToString() };
                Collection?.Add(item);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке добавить значение в поле!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Save(object p)
        {
            try
            {

            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке сохранить значение в поле!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }


        private void SetCollection() => Collection = db.TreatmentStage.Where(f => f.ClientId == Client.Id).Include(f => f.Client).ToObservableCollection() ?? new ObservableCollection<TreatmentStage>();


        public TreatmentStage Model { get; set; } //этап лечения
        public ObservableCollection<TreatmentStage> Collection { get; set; }
        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }
    }
}
