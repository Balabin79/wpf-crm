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
            UserControlName = "MainInfoControl";
            SetUserControl();
        }

        [Command]
        public void Init() //вызывается при переходе по кнопке "Клиенты" - первая загрузка
        {
            Client = new Client();
            UserControlName = "MainInfoControl";

            //без параметров, значит по-умолчанию
            SetUserControl();
        }

        [Command] 
        public void Load(object p)
        {
            //вызывается при переходе по списку клиентов из бокового меню
            if (p is Client client)
            {
                Client = client;
                SetUserControl();
                return;
            }

            //вызывается при переходе по кнопкам внитри карты клиентов (карта, инвойсы, планы, посещения и доп.поля)
            SetUserControl(p.ToString());
        }

      
        private void SetUserControl(string userControlName = "MainInfoControl")
        {
            //включаем видимость контрола и подчеркиваем кнопку активного контрола
            switch (userControlName)
            {
                case "MainInfoControl": 
                    Context = new MainInfoViewModel(Client);
                    UserControlName = userControlName;
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
                    Context = new FieldsViewModel(Client);
                    UserControlName = userControlName;
                    break;
            }
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
    }
}
