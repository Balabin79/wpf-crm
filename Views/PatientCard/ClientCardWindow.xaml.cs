using Dental.Models;
using Dental.ViewModels.ClientDir;
using Dental.ViewModels;
using DevExpress.Xpf.WindowsUI;
using System;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using Dental.ViewModels.Invoices;
using Dental.ViewModels.AdditionalFields;

namespace Dental.Views.PatientCard
{
    public partial class ClientCardWindow : Window
    {
        public ClientCardWindow(int clientId) : this(clientId, null) { }
        public ClientCardWindow(int clientId, PatientListViewModel vm)
        {
            InitializeComponent();
            ApplicationContext db = new ApplicationContext();
            var client = db.Clients.FirstOrDefault(f => f.Id == clientId);

            ClientCardViewModel clientCardViewModel = new ClientCardViewModel(clientId, vm);
            FieldsViewModel fieldsViewModel = new FieldsViewModel(client, vm);
            InvoicesViewModel invoicesViewModel = new InvoicesViewModel(client, db, true);
            TreatmentStageViewModel treatmentStageViewModel  = new TreatmentStageViewModel(client, db);

            clientCardViewModel.EventChangeReadOnly += invoicesViewModel.StatusReadOnly;
            clientCardViewModel.EventChangeReadOnly += fieldsViewModel.ChangedReadOnly;
            clientCardViewModel.EventChangeReadOnly += treatmentStageViewModel.StatusReadOnly;

            invoicesViewModel.StatusReadOnly(true);
            treatmentStageViewModel.StatusReadOnly(true);
            fieldsViewModel.IsReadOnly = true;

            clientCardViewModel.EventSaveCard += fieldsViewModel.Save;
            clientCardViewModel.EventSaveCard += treatmentStageViewModel.Save;
            clientCardViewModel.EventNewClientSaved += invoicesViewModel.NewClientSaved;
            clientCardViewModel.EventNewClientSaved += treatmentStageViewModel.NewClientSaved;

            //fieldsViewModel.EventChangeVisibleTab += clientCardViewModel.SetTabVisibility;
            clientCardViewModel.SetTabVisibility(fieldsViewModel.AdditionalFieldsVisible);

            this.DataContext = clientCardViewModel;
            this.Invoices.DataContext = invoicesViewModel;
            this.Fields.DataContext = fieldsViewModel;
            this.Medical.DataContext = treatmentStageViewModel;
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
