using B6CRM.Infrastructures.Converters;
using B6CRM.Models;
using B6CRM.ViewModels.AdditionalFields;
using B6CRM.Views.PatientCard;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Bars;
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

namespace B6CRM.ViewModels.ClientDir
{
    internal class ClientCardDispatcher : ViewModelBase
    {
        public ClientCardDispatcher() 
        {
            SetUserControl();
            SetBarButtonItemsBorder();
        }

        [Command]
        public void Init() //вызывается при переходе по кнопке "Клиенты" - первая загрузка
        {
            Client = new Client();

            //без параметров, значит по-умолчанию
            SetUserControl();
            SetBarButtonItemsBorder();
        }

        [Command] 
        public void Load(object p)
        {
            //вызывается при переходе по кнопкам внитри карты клиентов (карта, инвойсы, планы, посещения и доп.поля)
            if (p is ClientCardParameters parameters)
            {
                //Client = parameters.Client;
                SetUserControl(parameters.BarButtonItem.Name);
                SetBarButtonItemsBorder(parameters.BarButtonItem);
            }

            //вызывается при переходе по списку клиентов из бокового меню
            if (p is Client client)
            {
                Client = client;
                SetUserControl();
                SetBarButtonItemsBorder();
            }
        }

        [Command]
        public void SetUserControlActive(DevExpress.Xpf.Bars.BarButtonItem item)
        {
            SetUserControl(item?.Name);
            SetBarButtonItemsBorder(item);
        }

        private void SetBarButtonItemsBorder(DevExpress.Xpf.Bars.BarButtonItem item = null)
        {
            MainInfoBurButtonItemBorderBrush = BurButtonItemNotActiveBorderBrush;
            ClientInvoicesBurButtonItemBorderBrush = BurButtonItemNotActiveBorderBrush;
            ClientPlansBurButtonItemBorderBrush = BurButtonItemNotActiveBorderBrush;
            VisitsBurButtonItemBorderBrush = BurButtonItemNotActiveBorderBrush;
            AddClientFieldsBurButtonItemBorderBrush = BurButtonItemNotActiveBorderBrush;

            MainInfoBurButtonItemBorderThickness = BurButtonItemNotActiveBorderThickness;
            ClientInvoicesBurButtonItemBorderThickness = BurButtonItemNotActiveBorderThickness;
            ClientPlansBurButtonItemBorderThickness = BurButtonItemNotActiveBorderThickness;
            VisitsBurButtonItemBorderThickness = BurButtonItemNotActiveBorderThickness;
            AddClientFieldsBurButtonItemBorderThickness = BurButtonItemNotActiveBorderThickness;

            switch (item?.Name)
            {
                case "mainInfo": 
                    MainInfoBurButtonItemBorderBrush = BurButtonItemActiveBorderBrush;
                    MainInfoBurButtonItemBorderThickness = BurButtonItemActiveBorderThickness;
                    return;
                case "clientInvoices": 
                    ClientInvoicesBurButtonItemBorderBrush = BurButtonItemActiveBorderBrush;
                    ClientInvoicesBurButtonItemBorderThickness = BurButtonItemActiveBorderThickness;
                    return;
                case "clientPlans": 
                    ClientPlansBurButtonItemBorderBrush = BurButtonItemActiveBorderBrush;
                    ClientPlansBurButtonItemBorderThickness = BurButtonItemActiveBorderThickness;
                    return;
                case "visits": 
                    VisitsBurButtonItemBorderBrush = BurButtonItemActiveBorderBrush;
                    VisitsBurButtonItemBorderThickness = BurButtonItemActiveBorderThickness;
                    return;
                case "addClientFields": 
                    AddClientFieldsBurButtonItemBorderBrush = BurButtonItemNotActiveBorderBrush;
                    AddClientFieldsBurButtonItemBorderThickness = BurButtonItemActiveBorderThickness;
                    return;
            }
            MainInfoBurButtonItemBorderBrush = BurButtonItemActiveBorderBrush;
            MainInfoBurButtonItemBorderThickness = BurButtonItemActiveBorderThickness;
        }

        private void SetUserControl(string userControlName = "mainInfo")
        {
            // сбрасываем все по дефолту
            MainInfoClientControlVisibility = Visibility.Collapsed;
            ClientInvoicesClientControlVisibility = Visibility.Collapsed;
            ClientPlansClientControlVisibility = Visibility.Collapsed;
            VisitsClientControlVisibility = Visibility.Collapsed;
            AddClientFieldsClientControlVisibility = Visibility.Collapsed;

            //включаем видимость контрола и подчеркиваем кнопку активного контрола
            switch (userControlName)
            {
                case "mainInfo":
                    MainInfoClientControlVisibility = Visibility.Visible;
                    DataContext = new MainInfoViewModel(Client);
                    break;
                case "clientInvoices":
                    ClientInvoicesClientControlVisibility = Visibility.Visible;
                    DataContext = new ClientInvoicesViewModel(Client);
                    break;
                case "clientPlans":
                    ClientPlansClientControlVisibility = Visibility.Visible;
                    DataContext = new ClientPlansViewModel(Client);
                    break;
                case "visits":
                    VisitsClientControlVisibility = Visibility.Visible;
                    DataContext = new AppointmentsViewModel(Client);
                    break;

                case "addClientFields":
                    AddClientFieldsClientControlVisibility = Visibility.Visible;
                    DataContext = new FieldsViewModel(Client);
                    break;
            }
        }


        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }

        public object DataContext
        {
            get { return GetProperty(() => DataContext); }
            set { SetProperty(() => DataContext, value); }
        }

        #region  BurButtonItem's Border
        public SolidColorBrush BurButtonItemActiveBorderBrush { get; set; } = new SolidColorBrush(Colors.Gray);
        public SolidColorBrush BurButtonItemNotActiveBorderBrush { get; set; } = new SolidColorBrush(Colors.White);

        public Thickness BurButtonItemActiveBorderThickness { get; set; } = new Thickness(0, 0, 0, 1);
        public Thickness BurButtonItemNotActiveBorderThickness { get; set; } = new Thickness(0, 0, 0, 0);


        #endregion

        #region ClientControl's Visibility 
        public Visibility ClientControlVisibility { get; set; } = Visibility.Visible;
        public Visibility ClientControlCollapsed { get; set; } = Visibility.Collapsed;
        #endregion

        #region ClientControl's
        public Visibility MainInfoClientControlVisibility
        {
            get { return GetProperty(() => MainInfoClientControlVisibility); }
            set { SetProperty(() => MainInfoClientControlVisibility, value); }
        }

        public Visibility ClientInvoicesClientControlVisibility
        {
            get { return GetProperty(() => ClientInvoicesClientControlVisibility); }
            set { SetProperty(() => ClientInvoicesClientControlVisibility, value); }
        }

        public Visibility ClientPlansClientControlVisibility
        {
            get { return GetProperty(() => ClientPlansClientControlVisibility); }
            set { SetProperty(() => ClientPlansClientControlVisibility, value); }
        }

        public Visibility VisitsClientControlVisibility
        {
            get { return GetProperty(() => VisitsClientControlVisibility); }
            set { SetProperty(() => VisitsClientControlVisibility, value); }
        }

        public Visibility AddClientFieldsClientControlVisibility
        {
            get { return GetProperty(() => AddClientFieldsClientControlVisibility); }
            set { SetProperty(() => AddClientFieldsClientControlVisibility, value); }
        }
        #endregion

        #region BurButtonItem's Border
        public SolidColorBrush MainInfoBurButtonItemBorderBrush
        {
            get { return GetProperty(() => MainInfoBurButtonItemBorderBrush); }
            set { SetProperty(() => MainInfoBurButtonItemBorderBrush, value);}
        }
        public SolidColorBrush ClientInvoicesBurButtonItemBorderBrush
        {
            get { return GetProperty(() => ClientInvoicesBurButtonItemBorderBrush); }
            set { SetProperty(() => ClientInvoicesBurButtonItemBorderBrush, value); }
        }
        public SolidColorBrush ClientPlansBurButtonItemBorderBrush
        {
            get { return GetProperty(() => ClientPlansBurButtonItemBorderBrush); }
            set { SetProperty(() => ClientPlansBurButtonItemBorderBrush, value); }
        }
        public SolidColorBrush VisitsBurButtonItemBorderBrush
        {
            get { return GetProperty(() => VisitsBurButtonItemBorderBrush); }
            set { SetProperty(() => VisitsBurButtonItemBorderBrush, value); }
        }
        public SolidColorBrush AddClientFieldsBurButtonItemBorderBrush
        {
            get { return GetProperty(() => AddClientFieldsBurButtonItemBorderBrush); }
            set { SetProperty(() => AddClientFieldsBurButtonItemBorderBrush, value); }
        }
        #endregion

        #region BurButtonItem's BorderThinkenss
        public Thickness MainInfoBurButtonItemBorderThickness
        {
            get { return GetProperty(() => MainInfoBurButtonItemBorderThickness); }
            set { SetProperty(() => MainInfoBurButtonItemBorderThickness, value); }
        }
        public Thickness ClientInvoicesBurButtonItemBorderThickness
        {
            get { return GetProperty(() => ClientInvoicesBurButtonItemBorderThickness); }
            set { SetProperty(() => ClientInvoicesBurButtonItemBorderThickness, value); }
        }
        public Thickness ClientPlansBurButtonItemBorderThickness
        {
            get { return GetProperty(() => ClientPlansBurButtonItemBorderThickness); }
            set { SetProperty(() => ClientPlansBurButtonItemBorderThickness, value); }
        }
        public Thickness VisitsBurButtonItemBorderThickness
        {
            get { return GetProperty(() => VisitsBurButtonItemBorderThickness); }
            set { SetProperty(() => VisitsBurButtonItemBorderThickness, value); }
        }
        public Thickness AddClientFieldsBurButtonItemBorderThickness
        {
            get { return GetProperty(() => AddClientFieldsBurButtonItemBorderThickness); }
            set { SetProperty(() => AddClientFieldsBurButtonItemBorderThickness, value); }
        }
        #endregion
    }
}
