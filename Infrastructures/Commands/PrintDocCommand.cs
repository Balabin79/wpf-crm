using System;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
//using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Grid;
//using DevExpress.Xpf.PivotGrid;
using DevExpress.Xpf.Scheduling;
using DevExpress.Xpf.Printing;
using System.IO;
using DevExpress.Xpf.RichEdit;
using Dental.Services;
using DevExpress.XtraRichEdit;
using Dental.Models;
using Dental.Infrastructures.Converters;
using DevExpress.XtraPrinting;
using System.Windows.Controls;
using System.Drawing.Printing;
using Dental.Views.Documents;

namespace Dental.Infrastructures.Commands
{
    class PrintDocCommand : CommandBase
    {
        public override bool CanExecute(object p) =>true;
        public override void Execute(object p)
        {
            try
            {
                if (p is null) return;


                if (p is DocParams param)
                {
                    var invoice = ((GridCellData)(param.Item).DataContext).Row;

                    string pathToFile = p.ToString();
                    if (File.Exists(param.PathToFile))
                    {
                       /* RichEditDocumentServer documentServer = new RichEditDocumentServer();
                        documentServer.LoadDocument(param.PathToFile, GetDocumentFormat(param.PathToFile));
                        documentServer.RtfText = new RtfParse(documentServer.RtfText, invoice).Run();*/





                        //Initialize a new server and printer 
                        PrintDialog printDialog = new PrintDialog();
                        RichEditDocumentServer docServer = new RichEditDocumentServer();

                        //Pass the document content to the server  
                        docServer.LoadDocument(param.PathToFile, GetDocumentFormat(param.PathToFile));
                        docServer.RtfText = new RtfParse(docServer.RtfText, invoice).Run();

                        //Change the document layout
                        // docServer.Document.Sections[0].Page.Landscape = true;
                        //docServer.Document.Sections[0].Page.PaperKind = PaperKind.A4;

                        //Create a new component link 
                        LegacyPrintableComponentLink printableComponent = new LegacyPrintableComponentLink(docServer);

                        //Create a document to print 
                        printableComponent.CreateDocument(true);
                        printableComponent.ShowPrintPreview(new DocumentPrint());


                    }



                    /*

                    using (DevExpress.XtraPrinting.PrintingSystem print = new DevExpress.XtraPrinting.PrintingSystem())
                    {
                        print.ExecCommand
                        print.PrintDlg();

                        using (PrintableComponentLink link = new PrintableComponentLink(print))
                        {



                            link.Component = documentServer;
                            link.CreateDocument();
                            print.Document.Name = "PdfDocument";
                            //printingSystem.ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
                            //printingSystem.ExportOptions.Pdf.PageRange = "1-3";
                            //printingSystem.ExportOptions.Pdf.DocumentOptions  
                            link.ShowPreviewDialog();
                        }
                    }



                        /*var richEdit = new RichEditControl();
                        richEdit.LoadDocument(param.PathToFile, GetDocumentFormat(param.PathToFile));
                        richEdit.RtfText = new RtfParse(richEdit.RtfText, invoice).Run();
                        if (richEdit.IsPrintingAvailable)
                        {
                            richEdit.ShowPrintPreview();*/
                }
            }                         
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
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
