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

                ClickInAreaToothCommand = new LambdaCommand(OnClickInAreaToothExecuted, CanClickInAreaToothExecute);
                DiscountGroupList = db.DiscountGroups.OrderBy(f => f.Name).ToObservableCollection();
                AdvertisingList = db.Advertising.OrderBy(f => f.Name).ToObservableCollection();
                ClientsGroupList = db.ClientsGroup.OrderBy(f => f.Name).ToObservableCollection();
                ClientTreatmentPlans = db.ClientTreatmentPlans.OrderBy(f => f.TreatmentPlanNumber).ToObservableCollection();
                Teeth = new Teeth();
                Teeth.Tooth18 = 
                    new Tooth(){
                    Abbr = "KPPO",
                    ToothNumber = 20,
                    ToothImagePath = "pack://application:,,,/Resources/Icons/Template/tooth_yellow.png"               
                    };
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с данным разделом!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }

        }

        public ICommand ClickInAreaToothCommand { get; }
        private bool CanClickInAreaToothExecute(object p) => true;
        private void OnClickInAreaToothExecuted(object p)
        {
            try
            {
                int x = 0;
                /*if (Model?.Id > 0) db.Entry(Model).State = EntityState.Modified;
                else db.Organizations.Add(Model);
                db.SaveChanges();*/
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }



        public string SelectedGender { get; set; }
        public object SelectedDiscountGroups { get; set; }
        public object SelectedAdvertisings { get; set; }
        public string SelectedClientsGroup { get; set; }

        public Teeth Teeth { get; set;}


        public IEnumerable<DiscountGroups> DiscountGroupList { get; set; }
        public ICollection<Advertising> AdvertisingList { get; set; }
        public ICollection<ClientsGroup> ClientsGroupList { get; set; }
        public ObservableCollection<ClientTreatmentPlans> ClientTreatmentPlans { get; set; }

        public ICollection<string> GenderList
        {
            get => _GenderList;
        }
        private ICollection<string> _GenderList = new List<string> { "Мужчина", "Женщина" };


    }
}
