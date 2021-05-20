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
using Dental.Repositories;
using DevExpress.Xpf.Grid;
using Dental.Models;
using System.Windows;
using Dental.Views.Nomenclatures.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using DevExpress.CodeParser;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Dental.Infrastructures.Collection;

namespace Dental.ViewModels
{
    class EmployeeStatusViewModel : ViewModelBase
    {

        ApplicationContext db;
        public EmployeeStatusViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);
            //CancelFormCommand = new LambdaCommand(OnCancelFormCommandExecuted, CanCancelFormCommandExecute);
            db = new ApplicationContext();
         //   Collection = GetCollection();         
        }

        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenFormCommand { get; }
        public ICommand CancelFormCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanOpenFormCommandExecute(object p) => true;
        private bool CanCancelFormCommandExecute(object p) => true;


        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                if (p == null) return;
             //   var model = db.EmployeeStatuses?.Where(f => f.Id == (int)p).FirstOrDefault();
              //  if (model == null || !new ConfirDeleteInCollection().run((int)TypeItem.File)) return;

              //  db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();
              //  Collection = GetCollection();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSaveCommandExecuted(object p)
        {
            try
            {
               // if (EmployeeStatus.Id == 0) db.EmployeeStatuses.Add(EmployeeStatus);
                //else db.Entry(EmployeeStatus).State = EntityState.Modified;
   
                db.SaveChanges();
              //  Collection = GetCollection();
               // EmployeeStatusWindow.Close();             
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnOpenFormCommandExecuted(object p)
        {
            try
            {
               // EmployeeStatusWindow = new EmployeeStatusWindow();
               // EmployeeStatusWindow.Title = "Cтатус coтрудника";
              //  EmployeeStatusWindow.Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/status11.png"));

                if (p != null)
                {
                  //  EmployeeStatus = db.EmployeeStatuses.Where(f => f.Id == (int)p).FirstOrDefault();
                    WindowTitle = "Редактировать статус";
                }
                else
                {
                    EmployeeStatus = new EmployeeStatus();
                    WindowTitle = "Создать новый статус";
                }
               // EmployeeStatusWindow.DataContext = this;
               // EmployeeStatusWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        //private void OnCancelFormCommandExecuted(object p) => EmployeeStatusWindow.Close();

        public ObservableCollection<EmployeeStatus> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }

        public EmployeeStatus EmployeeStatus { get; set; }
        public string WindowTitle { get; set; }
        private ObservableCollection<EmployeeStatus> _Collection;
        //private EmployeeStatusWindow EmployeeStatusWindow;
       // private ObservableCollection<EmployeeStatus> GetCollection() => db.EmployeeStatuses.OrderBy(d => d.Name).ToObservableCollection();
    }
}
