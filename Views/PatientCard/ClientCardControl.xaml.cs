using B6CRM.Models;
using B6CRM.ViewModels.ClientDir;
using B6CRM.ViewModels;
using DevExpress.Xpf.WindowsUI;
using System;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using B6CRM.ViewModels.AdditionalFields;
using DevExpress.Xpf.Bars;
using System.Windows.Media;

namespace B6CRM.Views.PatientCard
{
    public partial class ClientCardControl : UserControl
    {
        public ClientCardControl() 
        { 
            InitializeComponent(); 
        }

        private void BarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var notSelected = new Thickness(0, 0, 0, 0);
            var selected = new Thickness(0, 0, 0, 1);
            mainInfo.BorderThickness = selected;

            
            ClientInvoicesControl.Visibility = Visibility.Collapsed;
            VisitsControl.Visibility = Visibility.Collapsed;
            ClientPlansControl.Visibility = Visibility.Collapsed;
            AddClientFieldsControl.Visibility = Visibility.Collapsed;
            MainInfoControl.Visibility = Visibility.Collapsed;


            if (e is ItemClickEventArgs item)
            { 
                if(item.Item.Name == "mainInfo")
                {
                    SetSelected(mainInfo);
                    MainInfoControl.Visibility = Visibility.Visible;
                }
                else SetNotSelected(mainInfo);

                if (item.Item.Name == "clientInvoices")
                {
                    SetSelected(clientInvoices);
                    ClientInvoicesControl.Visibility = Visibility.Visible;
                }
                else SetNotSelected(clientInvoices);

                if (item.Item.Name == "clientPlans")
                {
                    SetSelected(clientPlans);
                    ClientPlansControl.Visibility = Visibility.Visible;
                }
                else SetNotSelected(clientPlans);

                if (item.Item.Name == "visits")
                {
                    SetSelected(visits);
                    VisitsControl.Visibility = Visibility.Visible;
                }
                else SetNotSelected(visits);

                if (item.Item.Name == "addClientFields")
                {
                    SetSelected(addClientFields);
                    AddClientFieldsControl.Visibility = Visibility.Visible;
                }
                else SetNotSelected(addClientFields);
            }
        }

        private BarButtonItem SetSelected(BarButtonItem item)
        {
            item.BorderThickness = new Thickness(0, 0, 0, 1);
            item.BorderBrush = new Border().BorderBrush = new SolidColorBrush(Colors.Gray);
            return item;
        }

        private BarButtonItem SetNotSelected(BarButtonItem item)
        {
            item.BorderThickness = new Thickness(0, 0, 0, 0);
            item.BorderBrush = new Border().BorderBrush = new SolidColorBrush(Colors.White);
            return item;
        }
    }
}
