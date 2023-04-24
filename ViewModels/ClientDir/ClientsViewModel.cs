using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using B6CRM.Views.PatientCard;
using B6CRM.Views.WindowForms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using DevExpress.Mvvm.Native;
using B6CRM.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;
using B6CRM.Views.Documents;
using B6CRM.Services.Files;
using B6CRM.Views.AdditionalFields;
using B6CRM.ViewModels.AdditionalFields;
using System.IO;
using B6CRM.Infrastructures.Extensions.Notifications;
using B6CRM.Models.Base;
using DevExpress.Xpf.Editors;
using System.Diagnostics;
using B6CRM.Reports;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.Parameters;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.ConnectionParameters;
using System.Text.Json;
using DevExpress.Xpf.Grid;
using System.Text;
using DevExpress.Mvvm;
using DevExpress.Xpf.WindowsUI.Navigation;
using License;
using System.Windows.Data;
using Microsoft.VisualBasic;
using GroupInfo = DevExpress.Xpf.Printing.GroupInfo;
using System.Linq.Expressions;
using B6CRM.Models;
using B6CRM.Services;
using B6CRM.Infrastructures.Converters;

namespace B6CRM.ViewModels.ClientDir
{
    public class ClientsViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public ClientsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Db = db;
                Config = db.Config;
  
                //LoadEmployees();
                
                Model = new Client();
                
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка подключения к базе данных при попытке загрузить список клиентов!", true);
            }
        }


        #region Права на выполнение команд
        public bool CanOpenFormFields() => ((UserSession)Application.Current.Resources["UserSession"]).ClientsAddFieldsEditable;
        public bool CanPrintClients() => ((UserSession)Application.Current.Resources["UserSession"]).PrintClients;
        #endregion

        //это поле для привязки (используется в команде импорта данных)
        public ApplicationContext Db { get; set; }


        #region Переход из списка инвойсов или списков клиентов (загрузка карты)

        [Command]
        public void Load(object p)
        {
            try
            {
                if (p is Client model)
                {
                    Model = model;
                  /*  if (Application.Current.Resources["Router"] is MainViewModel nav &&
                          nav?.NavigationService is NavigationServiceBase service &&
                          service.Current is PatientsList page
                          ) page.clientCard.tabs.SelectedIndex = 0;*/
                }

                if (p is Invoice invoice && invoice.Client != null)
                {
                    if (invoice.Client == null) return;
                    Model = invoice.Client;

                    if (Application.Current.Resources["Router"] is MainViewModel nav &&
                     nav?.NavigationService is NavigationServiceBase service &&
                     service.Current is PatientsList page
                     )
                    {
                      /*  page.clientCard.tabs.SelectedIndex = 1;
                        if (page.invoicesList.grid.ItemsSource is ObservableCollection<Invoice> invoices)
                        {
                            page.clientCard.Invoices.grid.SelectedItem = invoice;
                        }*/
                    }
                }

                if (Status.Licensed && Status.HardwareID != Status.License_HardwareID)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                if (!Status.Licensed && Status.Evaluation_Time_Current > Status.Evaluation_Time)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
        #endregion


        #region Работа с разделом карты "Дополнительные поля"
        public ObservableCollection<Appointments> Appointments
        {
            get { return GetProperty(() => Appointments); }
            set { SetProperty(() => Appointments, value); }
        }

        [Command]
        public void OpenFormFields()
        {
            try
            {
                var vm = new AdditionalClientFieldsViewModel(db);
                vm.EventFieldChanges += UpdateFields;
                new ClientFieldsWindow() { DataContext = vm }?.Show();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При открытии формы \"Дополнительные поля\" произошла ошибка!", true);
            }
        }

        public void UpdateFields()
        {
            FieldsViewModel.ClientFieldsLoading(Model);
            AdditionalFieldsVisible = FieldsViewModel?.Fields.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion



        #region Карта клиента

 

        public Client Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public Visibility AdditionalFieldsVisible
        {
            get { return GetProperty(() => AdditionalFieldsVisible); }
            set { SetProperty(() => AdditionalFieldsVisible, value); }
        }

        public FieldsViewModel FieldsViewModel
        {
            get { return GetProperty(() => FieldsViewModel); }
            set { SetProperty(() => FieldsViewModel, value); }
        }

        public void SetTabVisibility(Visibility visibility) => AdditionalFieldsVisible = visibility;
        #endregion

        private INavigationService NavigationService { get { return GetService<INavigationService>(); } }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }

    }

}