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

        public bool ClientsRead
        {
            get { return GetProperty(() => ClientsRead); }
            set { SetProperty(() => ClientsRead, value); }
        }

        public bool EmployeesRead
        {
            get { return GetProperty(() => EmployeesRead); }
            set { SetProperty(() => EmployeesRead, value); }
        }

        public bool SheduleRead
        {
            get { return GetProperty(() => SheduleRead); }
            set { SetProperty(() => SheduleRead, value); }
        }

        public bool PricesRead
        {
            get { return GetProperty(() => PricesRead); }
            set { SetProperty(() => PricesRead, value); }
        }

        public bool TemplatesRead
        {
            get { return GetProperty(() => TemplatesRead); }
            set { SetProperty(() => TemplatesRead, value); }
        }

        public bool SettingsRead
        {
            get { return GetProperty(() => SettingsRead); }
            set { SetProperty(() => SettingsRead, value); }
        }

        public bool OrgRead
        {
            get { return GetProperty(() => OrgRead); }
            set { SetProperty(() => OrgRead, value); }
        }


        public bool ClientEditable { get; set; } = false;
        public bool ClientDeletable { get; set; } = false;
        public bool ClientTemplatesEditable { get; set; } = false;

        public bool EmployeeEditable { get; set; } = false;
        public bool EmployeeDeletable { get; set; } = false;

        public bool SheduleStatusEditable { get; set; } = false;
        public bool SheduleStatusDeletable { get; set; } = false;
        public bool SheduleLocationEditable { get; set; } = false;
        public bool SheduleLocationDeletable { get; set; } = false;

        public bool PriceEditable { get; set; } = false;
        public bool PriceDeletable { get; set; } = false;


        public bool TemplateEditable { get; set; } = false;
        public bool TemplateDeletable { get; set; } = false;

        public bool OrgEditable { get; set; } = false;
        public bool OrgDeletable { get; set; } = false;
    }
}
