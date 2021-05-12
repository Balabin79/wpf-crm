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
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Expressions;
using DevExpress.DataAccess.ObjectBinding;
using System.Drawing;

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
                var data = new SourceReportData() { 
                    OrganizationName = "Елки - палки",
                    PatientFullNameName = "Иванов Иван Иванович",
                    EmployeeFullNameName = "Петров Петр Петрович"
                };

                var report = new XtraReport1();
                ObjectDataSource objectDataSource = new ObjectDataSource();
                objectDataSource.Name = "Report";
                objectDataSource.DataSource = new DataSource(data);
                objectDataSource.BeginUpdate();
                objectDataSource.DataMember = "GetData";

                objectDataSource.EndUpdate();
                objectDataSource.Fill();
                report.DataSource = objectDataSource;








                // var s = new Dental.Reports.SourceReportData();
               
               /* DevExpress.DataAccess.ObjectBinding.ObjectDataSource objds = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource();
                objds.Name = "ObjectDataSource1";
                objds.DataSource = typeof(SourceReportData);
                objds.Fill();*/


                PrintHelper.ShowPrintPreview(new Window(), report);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
    }
}

