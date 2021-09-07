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
using DevExpress.Xpf.Grid;
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
using Dental.Reports.Recommendations;

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
                string param = p as string;
                XtraReport report = null;
                switch (param)
                {
                    case "IDSOnInjection" : report = new IDSInjection(); break;
                    case "IDSCariesTreatment" : report = new IDSCariesTreatment(); break;
                    case "IDSUrgentHelp": report = new IDSUrgentHelp(); break;
                    case "IDSOrthodonticTreatment": report = new IDSOrthodonticTreatment(); break;
                    case "IDSOrthopedicTreatment": report = new IDSOrthopedicTreatment(); break;
                    case "IDSDisclaimerGuarantees": report = new IDSDisclaimerGuarantees(); break;
                    case "IDSParadonticTreatment": report = new IDSParadonticTreatment(); break;
                    case "IDSDiagnosticManipulations": report = new IDSDiagnosticManipulations(); break;
                    case "IDSBoneAugmentation": report = new IDSBoneAugmentation(); break;
                    case "IDSProcessReconstruction": report = new IDSProcessReconstruction(); break;
                    case "IDSSurgery": report = new IDSSurgery(); break;
                    case "IDSTeethWhitening": report = new IDSTeethWhitening(); break;
                    case "IDSImplantPlacement": report = new IDSImplantPlacement(); break;
                    case "IDSTreatmentAndImplantation": report = new IDSTreatmentAndImplantation(); break;
                    case "IDSEndodonticTreatment": report = new IDSEndodonticTreatment(); break;
                        /*case "IDSList": report = new IDSList(); break;*/
                    case "TeethWhiteningRecommendations": report = new TeethWhiteningRecommendations(); break;
                    case "RecommendationsAfterImplantation": report = new RecommendationsAfterImplantation(); break;
                    case "AfterCariesAndPulpitis": report = new AfterCariesAndPulpitis(); break;
                    case "AfterProstheticsWithDentures": report = new AfterProstheticsWithDentures(); break;
                    case "AfterProstheticsWithCrowns": report = new AfterProstheticsWithCrowns(); break;
                    case "AfterProfOralHygiene": report = new AfterProfOralHygiene(); break;
                    case "AfterToothExtraction": report = new AfterToothExtraction(); break;
                    case "ForCareBraces": report = new ForCareBraces(); break;

                    /*case "ReceiptCashOrder": report = new ReceiptCashOrder(); break;
                    case "DentalServicesAgreement": report = new DentalServicesAgreement(); break;
                    case "ContractGuarantor": report = new ContractGuarantor(); break;
                    case "InstallmentAgreement": report = new InstallmentAgreement(); break;*/
                }

                if (report == null) return;

                var data = new SourceReportData() {
                    Organization = new Organization() {
                        Name = "Дентал-люкс",
                        License = "ЛСУ-345-12-23 от 13 марта 2012 г.",
                    },
                    PatientInfo = new PatientInfo()
                    {
                        FirstName = "Иван",
                        LastName = "Иванов",
                        MiddleName = "Иванович",
                        BirthDate = "12.02.1985",
                        SerialNumberPassport = "6303",
                       // NumberPassport = "987885",
                        DateIssuedPassport = "12.02.2012",
                        //FullAddress = "Саратовская область, Хвалынский район, с.Подлесное, ул. Лесная, 15"
                    }
                };
                
             
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

