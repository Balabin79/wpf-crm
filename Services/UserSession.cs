using Dental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.Services
{
    public class UserSession : DevExpress.Mvvm.ViewModelBase
    {
        public Employee Employee
        {
            get { return GetProperty(() => Employee); }
            set { SetProperty(() => Employee, value); }
        }

        public bool ClientsListRead
        {
            get { return GetProperty(() => ClientsListRead); }
            set { SetProperty(() => ClientsListRead, value); }
        }

        public bool EmployeesListRead
        {
            get { return GetProperty(() => EmployeesListRead); }
            set { SetProperty(() => EmployeesListRead, value); }
        }

        public bool AddFieldsRead
        {
            get { return GetProperty(() => AddFieldsRead); }
            set { SetProperty(() => AddFieldsRead, value); }
        }

        public bool SheduleRead
        {
            get { return GetProperty(() => SheduleRead); }
            set { SetProperty(() => SheduleRead, value); }
        }

        public bool InvoicesRead
        {
            get { return GetProperty(() => InvoicesRead); }
            set { SetProperty(() => InvoicesRead, value); }
        }

        public bool NomenclaturesRead
        {
            get { return GetProperty(() => NomenclaturesRead); }
            set { SetProperty(() => NomenclaturesRead, value); }
        }

        public bool ServicesRead
        {
            get { return GetProperty(() => ServicesRead); }
            set { SetProperty(() => ServicesRead, value); }
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

        public bool SyncRun
        {
            get { return GetProperty(() => SyncRun); }
            set { SetProperty(() => SyncRun, value); }
        }


        public bool OpenClientCard { get; set; } = false;
        public bool ClientEditable { get; set; } = false;
        public bool ClientDeletable { get; set; } = false;
        public bool ClientTemplatesEditable { get; set; } = false;
        public bool ClientAddFieldsEditable { get; set; } = false;

        public bool OpenEmployeeCard { get; set; } = false;
        public bool EmployeeEditable { get; set; } = false;
        public bool EmployeeDeletable { get; set; } = false;
        public bool EmployeeTemplatesEditable { get; set; } = false;
        public bool EmployeeAddFieldsEditable { get; set; } = false;

        public bool AddFieldsEditable { get; set; } = false;
        public bool AddFieldsDeletable { get; set; } = false;

        public bool SheduleStatusEditable { get; set; } = false;
        public bool SheduleStatusDeletable { get; set; } = false;
        public bool SheduleLocationEditable { get; set; } = false;
        public bool SheduleLocationDeletable { get; set; } = false;

        public bool InvoiceEditable { get; set; } = false;
        public bool InvoiceDeletable { get; set; } = false;

        public bool NomenclatureEditable { get; set; } = false;
        public bool NomenclatureDeletable { get; set; } = false;

        public bool ServiceEditable { get; set; } = false;
        public bool ServiceDeletable { get; set; } = false;

        public bool TemplateEditable { get; set; } = false;
        public bool TemplateDeletable { get; set; } = false;


        public bool OrgEditable { get; set; } = false;
        public bool OrgDeletable { get; set; } = false;
    }
}
