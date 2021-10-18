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
using System.IO;
using Dental.Services;

namespace Dental.ViewModels
{
    class IdsViewModel : ViewModelBase
    {
        public IdsViewModel()
        {
            OpenCommand = new LambdaCommand(OnOpenCommandExecuted, CanOpenCommandExecute);
            Ids = ProgramDirectory.GetIdsFilesNames();
        }

        #region функционал связанный с ИДС
        public ObservableCollection<FileInfo> _Ids;
        public ObservableCollection<FileInfo> Ids { get; set; } = new ObservableCollection<FileInfo>();
        #endregion

        public ICommand OpenCommand { get; }

        private bool CanOpenCommandExecute(object p) => true;


        private void OnOpenCommandExecuted(object p)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
    }
}

