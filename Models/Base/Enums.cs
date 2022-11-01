using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    public enum Tables
    {
        AdditionalClientFields = 1,
        AdditionalClientValues = 2,
        AdditionalEmployeeFields = 3,
        AdditionalEmployeeValues = 4,
        Appointments = 5,
        AppointmentsStatuses = 6,
        ClientInfo = 7,
        CommonValues = 8,
        Employees = 9,
        Invoices = 10,
        InvoiceMaterialItems = 11,
        InvoiceServiceItems = 12,
        LocationAppointment = 13,
        Measure = 14,
        Nomenclature = 15,
        Reestr = 16,
        Service = 17,
        ShedulerStatuses = 18,
        TemplateType = 19,
        TreatmentStage = 20,
        Allergies = 21,
        Anamnes = 22,
        Complaint = 23,
        DescriptionXRay = 24,
        Diagnoses = 25,
        Treatments = 26,
        Objectively = 27,
        TreatmentPlans = 28,
        Organizations = 29
    }

    public enum InvoiceType
    { 
        Service,
        Nomenclature
    }

}
