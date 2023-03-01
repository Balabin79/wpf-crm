using System;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
//using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Grid;
//using DevExpress.Xpf.PivotGrid;
using DevExpress.Xpf.Printing;
using System.IO;
using Dental.Services;
using DevExpress.XtraRichEdit;
using Dental.Models;
using Dental.Infrastructures.Converters;
using Dental.Views.Documents;
using Dental.Services.Files;

namespace Dental.Infrastructures.Commands
{
    class PrintDocCommand : CommandBase
    {
        public string PathToFile { get; set; }
        public object Model { get; set; }

        public override bool CanExecute(object p) =>true;
        public override void Execute(object p)
        {
            try
            {
                if (p is null) return;


                if (p is DocumentCommandParameters param1)
                {
                    PathToFile = param1.File.FullName;
                    Model = param1.Model;
                }


                if (p is DocParams param)
                {
                    PathToFile = param.PathToFile;
                    if (param.Item?.DataContext is GridCellData cell)
                    {
                        Model = cell.RowData.Row;
                    }
                    if (param.Item?.DataContext is GridCellMenuInfo row)
                    {
                        Model = row.Row.Row;
                    }
                }
                if (PathToFile == null || Model == null) return;
                PrintPreview();
            }                         
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void PrintPreview()
        {
            if (File.Exists(PathToFile))
            {
                RichEditDocumentServer docServer = new RichEditDocumentServer();

                //Pass the document content to the server  
                docServer.LoadDocument(PathToFile, GetDocumentFormat(PathToFile));
                docServer.RtfText = new RtfParse(docServer.RtfText, Model).Run();

                //Create a new component link 
                LegacyPrintableComponentLink printableComponent = new LegacyPrintableComponentLink(docServer);

                //Create a document to print 
                printableComponent.CreateDocument(true);
                printableComponent.ShowPrintPreview(new DocumentPrint());
            }
        }

        private DocumentFormat GetDocumentFormat(string fileName)
        {
            try
            {
                string ext = new FileInfo(fileName).Extension.ToString().Replace(".", "").ToLower();
                switch (ext)
                {
                    case "rtf": return DocumentFormat.Rtf;
                    case "doc": return DocumentFormat.Doc;
                    case "docx": return DocumentFormat.Doc;
                    case "html": return DocumentFormat.Html;
                    case "htm": return DocumentFormat.Html;
                    case "mht": return DocumentFormat.Mht;
                    case "epub": return DocumentFormat.ePub;
                    case "txt": return DocumentFormat.PlainText;
                    case "odt": return DocumentFormat.OpenDocument;
                    default: return DocumentFormat.PlainText;
                }
            }
            catch
            {
                return DocumentFormat.PlainText;
            }
        }
    }
}
