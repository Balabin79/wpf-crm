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

namespace Dental.ViewModels
{
    class EmployeeStatusViewModel : ViewModelBase
    {

        ApplicationContext db;
        public EmployeeStatusViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            CopyCommand = new LambdaCommand(OnCopyCommandExecuted, CanCopyCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);
            CancelFormCommand = new LambdaCommand(OnCancelFormCommandExecuted, CanCancelFormCommandExecute);
            db = new ApplicationContext();
            Collection = db.EmployeeStatuses.OrderBy(d => d.Name).ToObservableCollection();

            /*
                        Repository.CopyModel += ((IModel, TableView) c) => {
                            var copiedRow = Collection.Where(d => d.Id == ((EmployeeStatus)c.Item2.FocusedRow)?.Id).FirstOrDefault(); 
                            if (copiedRow != null)
                            {
                                int index = Collection.IndexOf(copiedRow) + 1;
                                Collection.Insert(index, (EmployeeStatus)c.Item1);
                                var row = Collection.Where(d => d.Id == c.Item1.Id).FirstOrDefault();
                                if (row != null)
                                {
                                    c.Item2.FocusedRow = row;
                                    c.Item2.ScrollIntoView(c.Item1);
                                    c.Item2.FocusedRow = c.Item1;
                                    //c.Item2.ShowEditForm();
                                }
                            }
                        };
                        Repository.UpdateModel += ((IModel, TableView) c) => {               
                            var row = Collection.Where(d => d.Id == c.Item1.Id).FirstOrDefault();
                            if (row != null)
                            {
                                int index = Collection.IndexOf(row);
                                Collection[index] = (EmployeeStatus)c.Item1;
                            }
                        };
                        Repository.AddModel += ((IModel, TableView) c) => {
                            Collection.Add((EmployeeStatus)c.Item1);
                            var row = Collection.Where(d => d.Id == c.Item1.Id).FirstOrDefault();

                            if (row != null)
                            {
                                c.Item2.FocusedRow = row;
                                c.Item2.ScrollIntoView(row);
                                //c.Item2.ShowEditForm();
                            }
                        };
                        Repository.DeleteModel += (IModel model) => {
                            var item = Collection.Where(d => d.Id == model.Id).FirstOrDefault();
                            if (item != null) Collection.Remove(item);               
                        };*/

        }

        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand OpenFormCommand { get; }
        public ICommand CancelFormCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanCopyCommandExecute(object p) => true;
        private bool CanOpenFormCommandExecute(object p) => true;
        private bool CanCancelFormCommandExecute(object p) => true;


        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                var table = p as TableView;
                if (table == null) return;
               // Repository.Delete(table);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCopyCommandExecuted(object p)
        {
            try
            {
                var table = p as DevExpress.Xpf.Grid.TableView;
                if (table == null) return;
                //Repository.Copy(table);
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
                if (EmployeeStatus.Id == 0)
                {
                    db.EmployeeStatuses.Add(EmployeeStatus);
                    db.SaveChanges();
                    Collection.Add(EmployeeStatus);
                    EmployeeStatusWindow.Close();
                } 
                else
                {
                    db.Entry(EmployeeStatus).State = EntityState.Modified;
                    db.SaveChanges();
                    Collection = db.EmployeeStatuses.OrderBy(d => d.Name).ToObservableCollection();
                    EmployeeStatusWindow.Close();
                }
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
                EmployeeStatusWindow = new EmployeeStatusWindow();
                EmployeeStatusWindow.Title = "Cтатус coтрудника";
                EmployeeStatusWindow.Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/status11.png"));

                if (p != null)
                {
                    EmployeeStatus = db.EmployeeStatuses.Where(f => f.Id == (int)p).FirstOrDefault();
                    WindowTitle = "Редактировать статус";
                }
                else
                {
                    EmployeeStatus = new EmployeeStatus();
                    WindowTitle = "Создать новый статус";
                }
                EmployeeStatusWindow.DataContext = this;
                EmployeeStatusWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }


        private void OnCancelFormCommandExecuted(object p) => EmployeeStatusWindow.Close();

        public ObservableCollection<EmployeeStatus> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }

        public EmployeeStatus EmployeeStatus { get; set; }
        public string WindowTitle { get; set; }
        private ObservableCollection<EmployeeStatus> _Collection;
        private EmployeeStatusWindow EmployeeStatusWindow;
    }
}
