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
using Dental.Reports.IDS;
using Dental.Models.PatientCard;

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
                    Organization = new Organization() {
                        Name = "Дентал-плюс",
                        License = "ЛСУ-345-12-23 от 13 марта 2012 г.",
                    },
                    PatientInfo = new PatientInfo()
                    {
                        FirstName = "Иван",
                        LastName = "Иванов",
                        MiddleName = "Иванович",
                        BirthDate = "12.02.1985",
                        SerialNumberPassport = "6303",
                        NumberPassport = "987885",
                        DateIssuedPassport = "12.02.2012",
                        FullAddress = "Саратовская область, Хвалынский район, с.Подлесное, ул. Лесная, 15"
                    }
                };
                
                var report = new IDSInjection();
                ObjectDataSource objectDataSource = new ObjectDataSource();
                objectDataSource.Name = "Report";
                objectDataSource.DataSource = new DataSource(data);
                objectDataSource.BeginUpdate();
                objectDataSource.DataMember = "GetData";

                objectDataSource.EndUpdate();
                objectDataSource.Fill();
                report.DataSource = objectDataSource;
                


                PrintHelper.ShowPrintPreview(new Window(), report);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
    }
}

