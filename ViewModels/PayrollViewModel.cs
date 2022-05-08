using Dental.Infrastructures.Logs;
using Dental.Models;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using Dental.Views.WindowForms;
using Dental.Services;
using Dental.Views.Estimates;
using Dental.Infrastructures.Converters;
using Dental.Infrastructures.Extensions.Notifications;

namespace Dental.ViewModels
{
    class PayrollViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public PayrollViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Collection = db.Appointments.Where(f => !string.IsNullOrEmpty(f.StartTime)).Include(f => f.ClientInfo).Include(f => f.Employee).ToObservableCollection();
 

            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Зарплатные ведомости\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenForm(object p)
        {
           /* try
            {
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
            }*/
        }

        [Command]
        public void Save()
        {
           /* try
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
            }*/
        }

        [Command]
        public void Delete(object p)
        {
            /*try
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
            }*/
        }

        [Command]
        public void CancelForm(object p)
        {
            /*try
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
            }*/
        }

        public ICollection<Appointments> Collection { get; set; }
    }
}
