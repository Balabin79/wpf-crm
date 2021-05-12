using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Input;
using Dental.Enums;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Interfaces.Template;
using Dental.Models;
using Dental.Models.Base;
using Dental.Repositories;
using DevExpress.Xpf.Grid;
using Dental.Views.PatientCard.IDS;
using System.Windows.Documents;
using Dental.Reports;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Printing;
using System.Windows;

namespace Dental.ViewModels
{
    class IdsViewModel : ViewModelBase
    {
        public IdsViewModel()
        {
            OpenCommand = new LambdaCommand(OnOpenCommandExecuted, CanOpenCommandExecute);      
        }

        public ICommand OpenCommand { get; }

        private bool CanOpenCommandExecute(object p) => true;


        private void OnOpenCommandExecuted(object p)
        {
            try
            {
                var report = new XtraReport2();
                PrintHelper.ShowPrintPreview(new Window(), report);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public string DocumentSource { get; set; }
        public string OrganizationName { get; set; }
        public string PatientFullNameName { get; set; }
        public string EmployeeFullNameName { get; set; }

        public string employeeFullNameName;






    }
}
