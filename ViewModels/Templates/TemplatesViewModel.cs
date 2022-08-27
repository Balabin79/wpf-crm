using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Converters;
using Dental.Infrastructures.Extensions.Notifications;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Views.Invoices;
using System.Collections;
using DevExpress.Xpf.Bars;
using DevExpress.Utils.Svg;
//using DevExpress.XtraPrinting.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using Dental.Infrastructures.Commands;
using Dental.Views.Documents;
using Dental.Services.Files;
using System.IO;
using DevExpress.Xpf.Grid;
using Dental.Models.Base;
using Dental.Views.PatientCard.Medical;
using Dental.ViewModels.Templates;
using Dental.Views.Templates;

namespace Dental.ViewModels.Templates
{
    public class TemplatesViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;

        public TemplatesViewModel()
        {
            try
            {
                db = new ApplicationContext();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Шаблоны\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ObservableCollection<Diagnos> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        [Command]
        public void OpenDiagnosesForm(object p)
        {
            DiagnosViewModel = new DiagnosViewModel();

            DiagnosesWin = new DiagnosesWin() { DataContext = DiagnosViewModel };
            DiagnosesWin?.Show();
        }

        [Command]
        public void CloseDiagnosesForm() => DiagnosesWin?.Close();

        public DiagnosesWin DiagnosesWin { get; set; }
        public DiagnosViewModel DiagnosViewModel { get; set; }







        /// <summary>
        /// /////////////////
        /// </summary>
        /// 
        /// 
        /// 
        /// 
        /// <param name="status"></param>
        public void StatusReadOnly(bool status)
        {
            IsReadOnly = status;
        }
        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

       




        public ICollection<Employee> Employees { get; set; }
       
        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }

        private InvoiceWindow InvoiceWindow;
        public InvoiceServiceWindow InvoiceServiceWindow;
        public InvoiceMaterialWindow InvoiceMaterialWindow;

       
    }
}

