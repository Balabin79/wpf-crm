using B6CRM.Infrastructures.Converters;
using B6CRM.Models;
using B6CRM.ViewModels.AdditionalFields;
using B6CRM.Views.PatientCard;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static B6CRM.ViewModels.ClientDir.ClientCardDispatcher;

namespace B6CRM.ViewModels.ClientDir
{
    internal class ClientCardDispatcher : ViewModelBase
    {
        public delegate void ReadOnlyChanged(bool status);
        public event ReadOnlyChanged EventReadOnlyChanged;

        public bool CanEditable() => Client?.Id > 0;

        #region Упраление боковыми списками (клиенты, счета, планы)
        [Command]
        public void ToggleList(object p) //переключить список
        {
            //включаем видимость контрола и подчеркиваем кнопку активного контрола
            var controlName = p.ToString();
            switch (controlName)
            {
                case "ClientsList":
                    ContextLeftList = new ClientsListViewModel();
                    ActiveLeftPanel = controlName;
                    //CreateClient();
                    break;
                case "ClientsInvoices":
                    ContextLeftList = new InvoicesListViewModel();
                    ActiveLeftPanel = controlName;
                    break;
                case "ClientsPlans":
                    ContextLeftList = new PlansListViewModel();
                    ActiveLeftPanel = controlName;
                    break;
            }
        }

        [Command]
        public void SetSelectedClient(Client client)
        {
            Client = client;
        }

        public string ActiveLeftPanel
        {
            get { return GetProperty(() => ActiveLeftPanel); }
            set { SetProperty(() => ActiveLeftPanel, value); }
        }

        public object ContextLeftList
        {
            get { return GetProperty(() => ContextLeftList); }
            set { SetProperty(() => ContextLeftList, value); }
        }


        public Invoice SelectedInvoiceToInvoicesList
        {
            get { return GetProperty(() => SelectedInvoiceToInvoicesList); }
            set { SetProperty(() => SelectedInvoiceToInvoicesList, value); }
        }
        #endregion

        #region Упраление картой клиента
        [Command]
        public void Init(object p) //вызывается при переходе по кнопке "Клиенты" - первая загрузка
        {
            Client = new Client();
            UserControlName = "MainInfoControl";
            ActiveLeftPanel = "ClientsList";
            IsReadOnly = false;
            //без параметров, значит по-умолчанию
            SetUserControl();
            // загружаем список клиентов в боковую панель
            ToggleList("ClientsList");
            // снимаем выделение в списке клиентов
            /*if (p is GridControl grid)
            {
                grid.SelectedItem = null;
            }*/
        }
        
        [Command]
        public void Load(object p)
        {
            //вызывается при переходе по списку клиентов из бокового меню
            if (p is Client client)
            {
                Client = client;
                SetUserControl();
                IsReadOnly = true;
                return;
            }
            //вызывается при переходе по кнопкам внитри карты клиентов (карта, инвойсы, планы, посещения и доп.поля)
            SetUserControl(p.ToString());
        }


        [Command]
        public void CreateClient()
        {
            Client = new Client();
            SetUserControl("MainInfoControl");
            IsReadOnly = false;           
        }

        private void SetUserControl(string userControlName = "MainInfoControl")
        {
            //включаем видимость контрола и подчеркиваем кнопку активного контрола
            switch (userControlName)
            {
                case "MainInfoControl":
                    var mainInfoViewMode = new MainInfoViewModel(Client);
                    Context = mainInfoViewMode;
                    UserControlName = userControlName;
                    mainInfoViewMode.EventClientChanged += SetClientChanged;
                    break;
                case "ClientInvoicesControl":
                    Context = new ClientInvoicesViewModel(Client);
                    UserControlName = userControlName;
                    break;
                case "ClientPlansControl":
                    Context = new ClientPlansViewModel(Client);
                    UserControlName = userControlName;
                    break;
                case "VisitsControl":
                    Context = new AppointmentsViewModel(Client);
                    UserControlName = userControlName;
                    break;
                case "AddClientFieldsControl":
                    var fieldsViewModel = new FieldsViewModel(Client);
                    Context = fieldsViewModel;
                    UserControlName = userControlName;
                    EventReadOnlyChanged += fieldsViewModel.ChangedReadOnly;
                    break;
            }
        }

        [Command]
        public void Editable()
        {
                IsReadOnly = !IsReadOnly;
                EventReadOnlyChanged?.Invoke(IsReadOnly);
        }

        //вызывается по событию когда изменяются данные в MainInfoViewModel
        public void SetClientChanged(Client client)
        {
            if (client != null) Client = client;
        }

        public string UserControlName
        {
            get { return GetProperty(() => UserControlName); }
            set { SetProperty(() => UserControlName, value); }
        }

        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }

        public object Context
        {
            get { return GetProperty(() => Context); }
            set { SetProperty(() => Context, value); }
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public bool IsSaveEnabled
        {
            get { return GetProperty(() => IsSaveEnabled); }
            set { SetProperty(() => IsSaveEnabled, value); }
        }
        #endregion
    }
}
