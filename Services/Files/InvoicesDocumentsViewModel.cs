using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.IO;
using Dental.Services;
using DevExpress.Xpf.Core;
using Dental.Views.WindowForms;
using DevExpress.XtraRichEdit;
using System.Diagnostics;
using Dental.Services.Files;

namespace Dental.Services.Files
{
    public class InvoicesDocumentsViewModel : AbstractDocumentsManagement
    {
        public InvoicesDocumentsViewModel() : this("") { }
        public InvoicesDocumentsViewModel(string Guid) : base(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FILES, Guid)){}
        public const string FILES = "B6Dental\\Documents\\Invoices";
    }
}