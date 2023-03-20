using Dental.Models;

namespace Dental.Services
{
    public class UserSession : DevExpress.Mvvm.ViewModelBase
    {
        public Employee Employee
        {
            get { return GetProperty(() => Employee); }
            set { SetProperty(() => Employee, value); }
        }

        public bool ShowSheduler
        {
            get { return GetProperty(() => ShowSheduler); }
            set { SetProperty(() => ShowSheduler, value); }
        }

        public bool ShowClients
        {
            get { return GetProperty(() => ShowClients); }
            set { SetProperty(() => ShowClients, value); }
        }

        public bool ShowEmployees
        {
            get { return GetProperty(() => ShowEmployees); }
            set { SetProperty(() => ShowEmployees, value); }
        }

        public bool ShowPrices
        {
            get { return GetProperty(() => ShowPrices); }
            set { SetProperty(() => ShowPrices, value); }
        }

        public bool ShowDocuments
        {
            get { return GetProperty(() => ShowDocuments); }
            set { SetProperty(() => ShowDocuments, value); }
        }

        public bool ShowStatistics
        {
            get { return GetProperty(() => ShowStatistics); }
            set { SetProperty(() => ShowStatistics, value); }
        }

        public bool ShowSettings
        {
            get { return GetProperty(() => ShowSettings); }
            set { SetProperty(() => ShowSettings, value); }
        }

        public bool AppointmentEditable { get; set; } = false;
        public bool AppointmentDeletable { get; set; } = false;
        public bool PrintSheduler { get; set; } = false;
        public bool ShedulerStatusEditable { get; set; } = false;
        public bool ShedulerLocationEditable { get; set; } = false;
        public bool ShedulerWorkTimeEditable { get; set; } = false;

        public bool ClientsEditable { get; set; } = false;
        public bool ClientsDelitable { get; set; } = false;
        public bool InvoiceEditable { get; set; } = false;
        public bool InvoiceDelitable { get; set; } = false;
        public bool PrintInvoice { get; set; } = false;
        public bool PlanEditable { get; set; } = false;
        public bool PlanDelitable { get; set; } = false;
        public bool PrintPlan { get; set; } = false;
        public bool ClientsImport { get; set; } = false;
        public bool PrintClients { get; set; } = false;
        public bool ClientsAddFieldsEditable { get; set; } = false;
        public bool ClientsCategoryEditable { get; set; } = false;
        public bool ClientsAdvertisingEditable { get; set; } = false;

        public bool EmployeeEditable { get; set; } = false;
        public bool EmployeeDelitable { get; set; } = false;
        public bool PrintEmployees { get; set; } = false;
        public bool EmployeeImport { get; set; } = false;

        public bool PriceEditable { get; set; } = false;
        public bool PriceDelitable { get; set; } = false;
        public bool PrintPrices { get; set; } = false;

        public bool DocumentEditable { get; set; } = false;
        public bool DocumentDelitable { get; set; } = false;
        public bool DocumentImport { get; set; } = false;
        public bool PrintDocument { get; set; } = false;
    }
}
