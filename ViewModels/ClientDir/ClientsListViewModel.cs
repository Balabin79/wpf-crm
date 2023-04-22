using B6CRM.Models;
using B6CRM.Reports;
using B6CRM.Services;
using B6CRM.Views.PatientCard;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Printing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace B6CRM.ViewModels.ClientDir
{
    public class ClientsListViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public ClientsListViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Db = db;
                Config = db.Config;
                SelectedClient = new Client();
                LoadClients();
                LoadPrintConditions();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка подключения к базе данных при попытке загрузить список клиентов!", true);
            }
        }

        [Command]
        public void SetSelectedClient(Client client)
        {
            SelectedClient = client;
        }

        [Command]
        public void Load(object p) => LoadClients();
        

        public void LoadClients(int? isArhive = 0) =>
            Clients = db.Clients.Where(f => f.IsInArchive == isArhive).OrderBy(f => f.LastName).ToObservableCollection() ?? new ObservableCollection<Client>();
        

        #region Работа с фильтрами и поиском в списке клиентов
        [Command]
        public void ShowArchive()
        {
            try
            {
                IsArchiveList = !IsArchiveList;
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        public bool IsArchiveList
        {
            get { return GetProperty(() => IsArchiveList); }
            set { SetProperty(() => IsArchiveList, value); }
        }

        public object LastNameSearch { get; set; }

        [Command]
        public void ClientsSearch()
        {
            try
            {
                Clients = db.Clients.Where(f => f.IsInArchive == (IsArchiveList ? 1 : 0)).OrderBy(f => f.LastName).ToObservableCollection();

                if (!string.IsNullOrEmpty(LastNameSearch?.ToString()))
                {
                    Clients = Clients.Where(f => f.LastName.ToLower().Contains(LastNameSearch.ToString().ToLower())).OrderBy(f => f.LastName).ToObservableCollection();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        #endregion


        #region Печать
        public ObservableCollection<PrintCondition> PrintConditions
        {
            get { return GetProperty(() => PrintConditions); }
            set { SetProperty(() => PrintConditions, value); }
        }

        public object PrintConditionsSelected { get; set; }

        private void LoadPrintConditions()
        {
            PrintConditions = new ObservableCollection<PrintCondition>()
            {
                new PrintCondition(){Name = "Не в архиве", Id = -3, Type = true.GetType()},
                new PrintCondition(){Name = "В архиве", Id = -2, Type = true.GetType()}
            };
            db.ClientCategories?.ToArray()?.ForEach(f => PrintConditions.Add(
                new PrintCondition() { Name = f.Name, Id = f.Id, Type = f.GetType() }
                ));
        }

        [Command]
        public void PrintClients()
        {
            PrintClientsWindow = new PrintClientsWindow() { DataContext = this };
            PrintClientsWindow.Show();
        }

        [Command]
        public void LoadDocForPrint()
        {
            try
            {
                // Create a link and assign a data source to it.
                // Assign your data templates to different report areas.
                CollectionViewLink link = new CollectionViewLink();
                CollectionViewSource Source = new CollectionViewSource();

                SetSourceCollectttion();

                Source.Source = SourceCollection;

                Source.GroupDescriptions.Add(new PropertyGroupDescription("ClientCategory.Name"));

                link.CollectionView = Source.View;
                link.GroupInfos.Add(new GroupInfo((DataTemplate)PrintClientsWindow.Resources["CategoryTemplate"]));
                link.DetailTemplate = (DataTemplate)PrintClientsWindow.Resources["ProductTemplate"];

                // Associate the link with the Document Preview control.
                PrintClientsWindow.preview.DocumentSource = link;

                // Generate the report document 
                // and show pages as soon as they are created.
                link.CreateDocument(true);
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }

        }

        public ICollection<Client> SourceCollection { get; set; } = new List<Client>();

        private void SetSourceCollectttion()
        {
            try
            {
                SourceCollection = new List<Client>();
                var ctx = db.Clients;
                var where = "";
                var or = "";

                if (PrintConditionsSelected is List<object> collection)
                {
                    var marked = collection.OfType<PrintCondition>().ToArray();
                    if (marked.Length > 0) where = " WHERE ";
                    if (marked.Length > 1) or = " OR ";

                    if (marked.FirstOrDefault(f => f.Id == -2) != null) where += " IsInArchive = 1";
                    if (marked.FirstOrDefault(f => f.Id == -3) != null)
                        where += where.Length > 10 ? or + "IsInArchive = 0" : "IsInArchive = 0";

                    var cat = marked.Where(f => f.Type == new ClientCategory().GetType())?.Select(f => f.Id)?.OfType<int?>().ToArray();

                    if (cat.Length > 0)
                    {
                        where += !string.IsNullOrEmpty(where) ? " AND" : " WHERE";
                        where += $" ClientCategoryId IN ({string.Join(",", cat)}) ";
                    }
                }

                if (!string.IsNullOrEmpty(where))
                {
                    SourceCollection = db.Clients.FromSqlRaw("SELECT * FROM ClientInfo" + where).
                       Include(f => f.ClientCategory).
                       OrderBy(f => f.ClientCategoryId).
                       ThenBy(f => f.LastName).
                       ToArray();
                    return;
                }
                SourceCollection = db.Clients.
                   Include(f => f.ClientCategory).
                   OrderBy(f => f.ClientCategoryId).
                   ThenBy(f => f.LastName).
                   ToArray();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
        public PrintClientsWindow PrintClientsWindow { get; set; }

        #endregion

        //это поле для привязки (используется в команде импорта данных)
        public ApplicationContext Db { get; set; }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }

        public ObservableCollection<Client> Clients
        {
            get { return GetProperty(() => Clients); }
            set { SetProperty(() => Clients, value); }
        }

        public Client SelectedClient
        {
            get { return GetProperty(() => SelectedClient); }
            set { SetProperty(() => SelectedClient, value); }
        }
    }
}
