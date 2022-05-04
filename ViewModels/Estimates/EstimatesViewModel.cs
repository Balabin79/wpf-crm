using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
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
using DevExpress.Mvvm.DataAnnotations;
using Dental.ViewModels.Estimates;

namespace Dental.ViewModels.Estimates
{
    class EstimatesViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        private readonly bool fromPatientCard;
        public EstimatesViewModel(Client client, ApplicationContext db, bool fromPatientCard = false)
        {
            try
            {
                this.db = db;
                this.fromPatientCard = fromPatientCard;
                Client = client;
                Estimates = (Client.Id > 0) ? 
                    db.Estimates.Where(f => f.ClientId == Client.Id)
                        .Include(f => f.EstimateServiseItems.Select(x => x.Employee))
                        .Include(f => f.EstimateServiseItems.Select(x => x.Service))
                        .Include(f => f.EstimateMaterialItems.Select(x => x.Measure))
                        .Include(f => f.EstimateMaterialItems.Select(x => x.Nomenclature))
                    .ToObservableCollection() : 
                    new ObservableCollection<Estimate>();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Сметы\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        #region Сметы
        [Command]
        public void OpenFormEstimate(object p)
        {
            try
            {
                Visibility visibility = fromPatientCard == true ? Visibility.Collapsed : Visibility.Visible;
                string title = "Редактирование сметы";
                if (p != null) Estimate = Estimates.FirstOrDefault(f => f.Id == (int)p);
                else
                {
                    title = "Новая смета";
                    Estimate = new Estimate()
                    {
                        Client = Client,
                        ClientId = Client.Id,
                        StartDate = DateTime.Now.ToShortDateString()
                    };
                }
           
                EstimateVM = new EstimateVM(db) 
                { 
                    Name = Estimate.Name, 
                    StartDate = Estimate.StartDate, 
                    Client = Estimate.Client, 
                    ClientFieldVisibility = visibility,
                    Title = title
                };

                EstimateWindow = new EstimateWindow() { DataContext = this };
                EstimateWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void SaveEstimate()
        {
            try
            {
                if (string.IsNullOrEmpty(EstimateVM.Name)) return;
                Estimate.Name = EstimateVM.Name;
                Estimate.StartDate = EstimateVM.StartDate;
                Estimate.Client = EstimateVM.Client;

                if (Estimate.Id == 0) 
                {
                    db.Entry(Estimate).State = EntityState.Added;
                    Estimates.Add(Estimate);
                } 
                db.SaveChanges();
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

        [Command]
        public void DeleteEstimate(object p)
        {
            try
            {
                if (p is Estimate estimate)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить смету?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    db.EstimateServiceItems.Where(f => f.EstimateId == estimate.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                    db.EstimateMaterialItems.Where(f => f.EstimateId == estimate.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                    db.Entry(estimate).State = EntityState.Deleted;
                    Estimates.Remove(estimate);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void CancelFormEstimate(object p)
        {
            try
            {
                if (p is System.ComponentModel.CancelEventArgs arg)
                {
                    arg.Cancel = false;
                    return;
                }
                EstimateWindow?.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion


        #region Услуги в смете
        [Command]
        public void SelectItemInServiceField(object p)
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

        [Command]
        public void OpenFormEstimateService(object p)
        {
            try
            {
                Employees = db.Employes.OrderBy(f => f.LastName).ToArray();
                Services = db.Services.ToArray();
                if (p is Estimate estimate)
                {
                    EstimateServiceItem = new EstimateServiceItem();
                    EstimateServiceItemVM = new EstimateServiceItemVM() { Estimate = estimate, Title = "Добавление новой услуги" };
                    EstimateServiceWindow = new EstimateServiceWindow() { DataContext = this };
                    EstimateServiceWindow.ShowDialog();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void EditEstimateService(object p)
        {
            try
            {
                if (p is EstimateServiceItem item)
                {
                    Employees = db.Employes.OrderBy(f => f.LastName).ToArray();
                    Services = db.Services.ToArray();
                    EstimateServiceItemVM = new EstimateServiceItemVM() 
                    { 
                        Estimate = item.Estimate, 
                        Employee = item.Employee, 
                        Service = item.Service, 
                        Count = item.Count, 
                        Price = item.Price,
                        Title = "Редактирование услуги"
                    };
                    EstimateServiceItem = item;
                    EstimateServiceWindow = new EstimateServiceWindow() { DataContext = this };
                    EstimateServiceWindow.ShowDialog();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void SaveRowInEstimate(object p)
        {
            if (string.IsNullOrEmpty(EstimateServiceItemVM.Service?.Name)) return;
            try
            {
                EstimateServiceItem.Estimate = EstimateServiceItemVM.Estimate;
                EstimateServiceItem.Service = EstimateServiceItemVM.Service;
                EstimateServiceItem.Employee = EstimateServiceItemVM.Employee;
                EstimateServiceItem.Count = EstimateServiceItemVM.Count;

                if (EstimateServiceItem.Id == 0)
                {
                    db.Entry(EstimateServiceItem).State = EntityState.Added;
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
            finally
            {
                EstimateServiceWindow?.Close();
            }
        }

        [Command]
        public void DeleteEstimateService(object p)
        {
            try
            {
                if (p is EstimateServiceItem item)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить услугу в смете?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
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

        [Command]
        public void CancelFormEstimateItem(object p)
        {
            try
            {
                if (p is System.ComponentModel.CancelEventArgs arg)
                {
                    arg.Cancel = false;
                    return;
                }
                EstimateServiceWindow?.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion

        #region Материалы в смете
        [Command]
        public void SelectItemInMaterialField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is Nomenclature material)
                    {
                        if (material.IsDir == 1) return;
                        parameters.Popup.EditValue = material;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void OpenFormEstimateMaterial(object p)
        {
            try
            {
                Materials = db.Nomenclature.ToArray();
                Measuries = db.Measure.OrderBy(f => f.Name).ToArray();
                if (p is Estimate estimate)
                {
                    EstimateMaterialItem = new EstimateMaterialItem();
                    EstimateMaterialItemVM = new EstimateMaterialItemVM() { Estimate = estimate, Title = "Добавление нового материала" };
                    EstimateMaterialWindow = new EstimateMaterialWindow() { DataContext = this };
                    EstimateMaterialWindow.ShowDialog();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void EditEstimateMaterial(object p)
        {
            try
            {
                if (p is EstimateMaterialItem item)
                {
                    Materials = db.Nomenclature.ToArray();
                    Measuries = db.Measure.OrderBy(f => f.Name).ToArray();
                    EstimateMaterialItemVM = new EstimateMaterialItemVM()
                    {
                        Estimate = item.Estimate,
                        Measure = item.Measure,
                        Nomenclature = item.Nomenclature,
                        Count = item.Count,
                        Price = item.Price,
                        Title = "Редактирование материала"
                    };
                    EstimateMaterialItem = item;
                    EstimateMaterialWindow = new EstimateMaterialWindow() { DataContext = this };
                    EstimateMaterialWindow.ShowDialog();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void SaveMaterialRowInEstimate(object p)
        {
            if (string.IsNullOrEmpty(EstimateMaterialItemVM.Nomenclature?.Name)) return;
            try
            {
                EstimateMaterialItem.Estimate = EstimateMaterialItemVM.Estimate;
                EstimateMaterialItem.Measure = EstimateMaterialItemVM.Measure;
                EstimateMaterialItem.Nomenclature = EstimateMaterialItemVM.Nomenclature;
                EstimateMaterialItem.Count = EstimateMaterialItemVM.Count;

                if (EstimateMaterialItem.Id == 0)
                {
                    db.Entry(EstimateMaterialItem).State = EntityState.Added;
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
            finally
            {
                EstimateMaterialWindow?.Close();
            }
        }

        [Command]
        public void DeleteEstimateMaterial(object p)
        {
            try
            {
                if (p is EstimateMaterialItem item)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить материал в смете?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
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

        [Command]
        public void CancelFormEstimateMaterialItem(object p)
        {
            try
            {
                if (p is System.ComponentModel.CancelEventArgs arg)
                {
                    arg.Cancel = false;
                    return;
                }
                EstimateMaterialWindow?.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion

        public ICollection<Estimate> Estimates { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Measure> Measuries { get; set; }
        public ICollection<Service> Services { get; set; }
        public ICollection<Nomenclature> Materials { get; set; }
        public EstimateVM EstimateVM { get; set; }
        public EstimateServiceItemVM EstimateServiceItemVM { get; set; }
        public EstimateMaterialItemVM EstimateMaterialItemVM { get; set; }
        public Estimate Estimate { get; set; }
        public EstimateServiceItem EstimateServiceItem { get; set; }
        public EstimateMaterialItem EstimateMaterialItem { get; set; }
        public Client Client { get; set; }
        private EstimateWindow EstimateWindow;
        public EstimateServiceWindow EstimateServiceWindow;
        public EstimateMaterialWindow EstimateMaterialWindow;
       


        #region Материалы в смете

        #endregion
    }
}

