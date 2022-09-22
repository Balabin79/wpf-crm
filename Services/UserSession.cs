using Dental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.Services
{
    public class UserSession
    {

        public Employee Employee { get; set; }

        public bool OpenClientCard { get; set; } = false;
        public bool ClientsListRead { get; set; } = false;
        public bool ClientEditable { get; set; } = false;
        public bool ClientDeletable { get; set; } = false;
        public bool ClientTemplatesEditable { get; set; } = false;
        public bool ClientAddFieldsEditable { get; set; } = false;

        public bool OpenEmployeeCard { get; set; } = false;
        public bool EmployeesListRead { get; set; } = false;
        public bool EmployeeEditable { get; set; } = false;
        public bool EmployeeDeletable { get; set; } = false;
        public bool EmployeeTemplatesEditable { get; set; } = false;
        public bool EmployeeAddFieldsEditable { get; set; } = false;

        public bool AddFieldsRead { get; set; } = false;
        public bool AddFieldsEditable { get; set; } = false;
        public bool AddFieldsDeletable { get; set; } = false;

        public bool SheduleRead { get; set; } = false;
        public bool SheduleStatusEditable { get; set; } = false;
        public bool SheduleStatusDeletable { get; set; } = false;
        public bool SheduleLocationEditable { get; set; } = false;
        public bool SheduleLocationDeletable { get; set; } = false;

        public bool InvoicesRead { get; set; } = false;
        public bool InvoiceEditable { get; set; } = false;
        public bool InvoiceDeletable { get; set; } = false;

        public bool NomenclaturesRead { get; set; } = false;
        public bool NomenclatureEditable { get; set; } = false;
        public bool NomenclatureDeletable { get; set; } = false;

        public bool ServicesRead { get; set; } = false;
        public bool ServiceEditable { get; set; } = false;
        public bool ServiceDeletable { get; set; } = false;

        public bool TemplatesRead { get; set; } = false;
        public bool TemplateEditable { get; set; } = false;
        public bool TemplateDeletable { get; set; } = false;

        public bool SettingsRead { get; set; } = false;

        public bool SyncRun { get; set; } = false;


        


    }
}
