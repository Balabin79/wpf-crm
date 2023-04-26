using B6CRM.Infrastructures.Converters;
using B6CRM.Models;
using B6CRM.Services;
using B6CRM.ViewModels.AdditionalFields;
using B6CRM.Views.AdditionalFields;
using B6CRM.Views.PatientCard;
using B6CRM.Views.WindowForms;
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

        #region Права на выполнение команд
        public bool CanEditable() => Client?.Id > 0;
        public bool CanOpenFormFields() => ((UserSession)Application.Current.Resources["UserSession"]).ClientsAddFieldsEditable;
        public bool CanOpenAdvertisingsWindow() => ((UserSession)Application.Current.Resources["UserSession"]).ClientsAdvertisingEditable;
        public bool CanOpenClientCategoriesWindow() => ((UserSession)Application.Current.Resources["UserSession"]).ClientsCategoryEditable;
        #endregion

        #region Упраление боковыми списками (клиенты, счета, планы)
        [Command]
        public void ToggleList(object p) //переключить список
        {
            //включаем видимость контрола и подчеркиваем кнопку активного контрола
            var controlName = p.ToString();
            switch (controlName)
            {
                case "ClientsList":
                    ContextLeftList = new ClientsListViewModel(Client?.IsInArchive);
                    ActiveLeftPanel = controlName;
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

        public void DeleteClientCard() // перезагрузка карты и контекста, вызываемая по событию удаления карты
        {
            CreateClient();
            if (ContextLeftList is ClientsListViewModel) ToggleList("ClientsList");           
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

        private void SetUserControl(string userControlName = "MainInfoControl", object selectedItem = null)
        {
            //включаем видимость контрола и подчеркиваем кнопку активного контрола
            switch (userControlName)
            {
                case "MainInfoControl":
                    var mainInfoViewMode = new MainInfoViewModel(Client);
                    Context = mainInfoViewMode;
                    UserControlName = userControlName;
                    mainInfoViewMode.EventClientChanged += SetClientChanged;
                    mainInfoViewMode.EventClientDeleted += DeleteClientCard;
                    break;
                case "ClientInvoicesControl":
                    var clientInvoicesViewModel = new ClientInvoicesViewModel(Client);
                    Context = clientInvoicesViewModel;
                    UserControlName = userControlName;
                    clientInvoicesViewModel.SelectedItem = selectedItem;
                    clientInvoicesViewModel.EventInvoicesReload += InvoicesReload;
                    break;
                case "ClientPlansControl":
                    var clientPlansViewModel = new ClientPlansViewModel(Client);
                    Context = clientPlansViewModel;
                    UserControlName = userControlName;
                    clientPlansViewModel.SelectedItem = selectedItem;
                    clientPlansViewModel.EventPlansReload += PlansReload;
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

        //переход из левого списка счетов (все счета) в карту клиента во вкладку счета клиента
        [Command]
        public void LoadClientInvoices(object p)
        {
            if(p is Invoice invoice && invoice.Client != null)
            {
                Client = invoice.Client;
                SetUserControl("ClientInvoicesControl", invoice);
                IsReadOnly = true;
                return;
            }
        }

        //переход из левого списка планов (все планы) в карту клиента во вкладку планы клиента
        [Command]
        public void LoadClientPlans(object p)
        {
            if (p is Plan plan && plan.Client != null)
            {
                Client = plan.Client;
                SetUserControl("ClientPlansControl", plan);
                IsReadOnly = true;
                return;
            }
        }

        //вызывается по событию когда изменяются данные в MainInfoViewModel
        public void SetClientChanged(Client client)
        {
            if (client != null) Client = client;
            if (ContextLeftList is ClientsListViewModel) ToggleList("ClientsList");
        }

        //вызывается по событию когда изменяются данные в ClientInvoicesControl
        public void InvoicesReload()
        {
            if (ContextLeftList is InvoicesListViewModel) ToggleList("ClientsInvoices");
        }

        //вызывается по событию когда изменяются данные в ClientPlansControl
        public void PlansReload()
        {
            if (ContextLeftList is PlansListViewModel) ToggleList("ClientsPlans");
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

        #region Открытие форм справочников (window)
        public AdvertisingViewModel AdvertisingViewModel { get; set; }
        public ClientCategoryViewModel ClientCategoryViewModel { get; set; }
        public AdditionalClientFieldsViewModel AdditionalClientFieldsViewModel { get; set; }

        [Command]
        public void OpenAdvertisingsWindow()
        {
            try
            {
                AdvertisingViewModel = new AdvertisingViewModel();
                if (Context is ClientInvoicesViewModel vm) 
                { 
                    AdvertisingViewModel.EventAdvertisingsChanged += vm.LoadAdvertisings; 
                    AdvertisingViewModel.EventAdvertisingsDeleted += vm.AdvertisingsDeleted; 
                }
                new AdvertisingsWindow() { DataContext = AdvertisingViewModel }?.Show();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При открытии формы \"Рекламные источники\" произошла ошибка!", true);
            }
        }

        [Command]
        public void OpenClientCategoriesWindow()
        {
            try
            {
                ClientCategoryViewModel = new ClientCategoryViewModel();
                if (Context is MainInfoViewModel vm) ClientCategoryViewModel.EventClientCategoriesChanges += vm.ClientCategoriesChanged;
                new ClientCategoriesWindow() { DataContext = ClientCategoryViewModel }?.Show();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При открытии формы \"Категории клиентов\" произошла ошибка!", true);
            }
        }

        [Command]
        public void OpenFormFields()
        {
            try
            {
                AdditionalClientFieldsViewModel = new AdditionalClientFieldsViewModel();
                if(Context is FieldsViewModel vm) AdditionalClientFieldsViewModel.EventFieldChanges += vm.ClientFieldsLoading;
                new ClientFieldsWindow() { DataContext = AdditionalClientFieldsViewModel }?.Show();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При открытии формы \"Дополнительные поля\" произошла ошибка!", true);
            }
        }
        #endregion
    }
}
