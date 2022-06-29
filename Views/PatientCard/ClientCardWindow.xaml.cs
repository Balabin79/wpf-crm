using Dental.Models;
using Dental.ViewModels;
using DevExpress.Xpf.WindowsUI;
using System;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using Dental.ViewModels.Invoices;

namespace Dental.Views.PatientCard
{
    public partial class ClientCardWindow : Window
    {
        public ClientCardWindow(int clientId, PatientListViewModel vm)
        {
            InitializeComponent();
            ApplicationContext db = new ApplicationContext();
            var client = db.Clients.FirstOrDefault(f => f.Id == clientId);
            ClientCardViewModel clientCardViewModel = new ClientCardViewModel(clientId, vm);
            InvoicesViewModel invoicesViewModel = new InvoicesViewModel(client, db, true);
            clientCardViewModel.EventChangeReadOnly += invoicesViewModel.StatusReadOnly;
            invoicesViewModel.StatusReadOnly(true);

            this.DataContext = clientCardViewModel;
            this.Invoices.DataContext = invoicesViewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DataContext is ClientCardViewModel vm)
            {
                if (vm.HasUnsavedChanges() && vm.UserSelectedBtnCancel())
                {
                    e.Cancel = true;
                    return;                   
                }
                e.Cancel = false;
            }
            e.Cancel = false;
        }
    }
}
