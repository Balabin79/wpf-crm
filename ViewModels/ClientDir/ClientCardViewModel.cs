using B6CRM.Models;
using B6CRM.Services;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.ViewModels.ClientDir
{
    public class ClientCardViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public ClientCardViewModel()
        {
            db = new ApplicationContext();
            ClientCategoriesLoad();
            //Config = db.Config;

        }

        [Command]
        public void Load(object p)
        {
            if (p is Client client)
            {
                Client = db.Clients.FirstOrDefault(f => f.Id == client.Id);
                ClientInfoViewModel = new ClientInfoViewModel(client);
            }
        }

        public void ClientCategoriesLoad() => ClientCategories = db.ClientCategories?.ToArray()?.ToObservableCollection() ?? new ObservableCollection<ClientCategory>();

        private Client Client { get; set; }

        public ClientInfoViewModel ClientInfoViewModel
        {
            get { return GetProperty(() => ClientInfoViewModel); }
            set { SetProperty(() => ClientInfoViewModel, value); }
        }

        public ObservableCollection<ClientCategory> ClientCategories
        {
            get { return GetProperty(() => ClientCategories); }
            set { SetProperty(() => ClientCategories, value); }
        }

    }
}
