using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using Dental.Views.WindowForms;
using Dental.Services;
using Dental.Views.Estimates;
using Dental.Infrastructures.Converters;
using Dental.Infrastructures.Extensions.Notifications;

namespace Dental.ViewModels
{
    class EstimateViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public EstimateViewModel(Client client = null)
        {
            OpenFormEstimateCommand = new LambdaCommand(OnOpenFormEstimateCommandExecuted, CanOpenFormEstimateCommandExecute);
            EditEstimateItemCommand = new LambdaCommand(OnEditEstimateItemCommandExecuted, CanEditEstimateItemCommandExecute);
            SaveEstimateCommand = new LambdaCommand(OnSaveEstimateCommandExecuted, CanSaveEstimateCommandExecute);
            DeleteEstimateCommand = new LambdaCommand(OnDeleteEstimateCommandExecuted, CanDeleteEstimateCommandExecute);

            SelectPosInClassificatorCommand = new LambdaCommand(OnSelectPosInClassificatorCommandExecuted, CanSelectPosInClassificatorCommandExecute);
            AddRowInEstimateCommand = new LambdaCommand(OnAddRowInEstimateCommandExecuted, CanAddRowInEstimateCommandExecute);
            SaveRowInEstimateCommand = new LambdaCommand(OnSaveRowInEstimateCommandExecuted, CanSaveRowInEstimateCommandExecute);
            DeleteRowInEstimateCommand = new LambdaCommand(OnDeleteRowInEstimateCommandExecuted, CanDeleteRowInEstimateCommandExecute);
            CancelFormEstimateCommand = new LambdaCommand(OnCancelFormEstimateCommandExecuted, CanCancelFormEstimateCommandExecute);
            CancelFormEstimateItemCommand = new LambdaCommand(OnCancelFormEstimateItemCommandExecuted, CanCancelFormEstimateItemCommandExecute);

            try
            {
                db = new ApplicationContext();
                Client = client ?? new Client();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Сметы\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }


        // открыть форму плана лечения
        public ICommand OpenFormEstimateCommand { get; }
        // сохранить новый или отредактированный план лечения
        public ICommand SaveEstimateCommand { get; }
        //удалить план лечения
        public ICommand DeleteEstimateCommand { get; }

        public ICommand EditEstimateItemCommand { get; }
        public ICommand SelectPosInClassificatorCommand { get; }
        public ICommand AddRowInEstimateCommand { get; }
        public ICommand SaveRowInEstimateCommand { get; }
        public ICommand DeleteRowInEstimateCommand { get; }
        public ICommand CancelFormEstimateCommand { get; }
        public ICommand CancelFormEstimateItemCommand { get; }

        private bool CanSelectPosInClassificatorCommandExecute(object p) => true;
        private bool CanOpenFormEstimateCommandExecute(object p) => true;
        private bool CanEditEstimateItemCommandExecute(object p) => true;
        private bool CanSaveEstimateCommandExecute(object p) => true;
        private bool CanDeleteEstimateCommandExecute(object p) => true;

        private bool CanAddRowInEstimateCommandExecute(object p) => true;
        private bool CanSaveRowInEstimateCommandExecute(object p) => true;
        private bool CanDeleteRowInEstimateCommandExecute(object p) => true;
        private bool CanCancelFormEstimateCommandExecute(object p) => true;
        private bool CanCancelFormEstimateItemCommandExecute(object p) => true;


        #region Сметы
        private Client Client { get; set; }

        private EstimateWindow EstimateWindow;

        public Estimate Estimate
        {
            get => estimate;
            set => Set(ref estimate, value);
        }
        private Estimate estimate;

        public ObservableCollection<Estimate> Estimates { get; set; } = new ObservableCollection<Estimate>();

        public Estimate EstimateClone { get; set; }

        private void OnOpenFormEstimateCommandExecuted(object p)
        {
            try
            {
                if (p != null) Estimate = db.Estimates.Include(f => f.Client).Include(f => f.EstimateServiseItems).FirstOrDefault(i => i.Id == (int)p);
                else
                {
                    Estimate = new Estimate()
                    {
                        Client = this.Client,
                        ClientId = this.Client.Id,
                        StartDate = DateTime.Now.ToShortDateString()
                    };
                }
                EstimateClone = (Estimate)Estimate.Clone();
                EstimateWindow = new EstimateWindow() { DataContext = this };
                EstimateWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSaveEstimateCommandExecuted(object p)
        {
            try
            {
                if (string.IsNullOrEmpty(Estimate.Name)) return;
                if (Estimate.Id == 0)
                {
                    db.Entry(Estimate).State = EntityState.Added;
                    //Estimates.Add(Estimate);

                }
                int cnt = db.SaveChanges();
                if (cnt > 0) db.Entry<Estimate>(Estimate).Reload();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
            finally
            {
                EstimateWindow?.Close();
            }
        }

        private void OnDeleteEstimateCommandExecuted(object p)
        {
            try
            {
                if (p is Estimate estimate)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить смету?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    db.EstimateServiceItems.Where(f => f.EstimateId == estimate.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                    db.Estimates.Local.Remove(estimate);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCancelFormEstimateCommandExecuted(object p)
        {
            Estimate = EstimateClone;
            EstimateWindow?.Close();
        }
        #endregion


        #region Состав сметы
        private void OnSelectPosInClassificatorCommandExecuted(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is Service service)
                    {
                        if (service.IsDir == 1) return;
                        parameters.Popup.EditValue = service;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAddRowInEstimateCommandExecuted(object p)
        {
            try
            {
                if (p is Estimate estimate)
                {
                    EstimateServiceItem = new EstimateServiceItem();
                    EstimateServiceItem.EstimateId = estimate.Id;
                    EstimateServiceItem.Estimate = estimate;

                    /* EstimateServiceWindow = new EstimateServiceWindow();
                     EstimateServiceWindow.DataContext = this;
                     EstimateServiceWindow.ShowDialog();*/
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnEditEstimateItemCommandExecuted(object p)
        {
            try
            {
                if (p is EstimateServiceItem item)
                {
                    EstimateServiceItem = item;
                    /*  EstimateServiceWindow = new EstimateServiceWindow();
                      EstimateServiceWindow.DataContext = this;
                      EstimateServiceWindow.ShowDialog();*/
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSaveRowInEstimateCommandExecuted(object p)
        {
            try
            {
                if (!string.IsNullOrEmpty(EstimateServiceItem["Classificator"])) return;
                if (EstimateServiceItem.Id == 0)
                {
                    db.Estimates.FirstOrDefault(f => f.Id == EstimateServiceItem.EstimateId)?.EstimateServiseItems.Add(EstimateServiceItem);
                }

                int cnt = db.SaveChanges();
                if (cnt > 0)
                {
                    //if (cnt > 0) EstimateServiceItem.Update();
                    var notification = new Notification();
                    notification.Content = "Позиция в плане лечения сохранена!";
                    notification.run();
                }
                //EstimateServiceWindow.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
                EstimateServiceItem = null;
                // EstimateServiceWindow.Close();
            }
        }

        private void OnDeleteRowInEstimateCommandExecuted(object p)
        {
            try
            {
                if (!string.IsNullOrEmpty(EstimateServiceItem["Classificator"]))
                {
                    int x = 0;
                }
                if (p is EstimateServiceItem item)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить позицию в смете?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;

                    db.Entry(item).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCancelFormEstimateItemCommandExecuted(object p)
        {
            try
            {
                if (db.Entry(EstimateServiceItem).State == EntityState.Modified)
                {
                    db.Entry(EstimateServiceItem).State = EntityState.Unchanged;
                }
                db.SaveChanges();
                EstimateWindow.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }

        }


        public EstimateServiceItem EstimateServiceItem
        {
            get => estimateServiceItem;
            set => Set(ref estimateServiceItem, value);
        }
        private EstimateServiceItem estimateServiceItem;
        //private EstimateServiceWindow EstimateServiceWindow;


        public Visibility IsVisibleItemPlanForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupPlanForm { get; set; } = Visibility.Hidden;

        public List<Service> ClassificatorCategories { get; set; }
        public List<Employee> Employes { get; set; }

        #endregion

    }
}

