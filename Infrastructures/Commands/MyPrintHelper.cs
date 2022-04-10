using DevExpress.Mvvm;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Scheduling;
using DevExpress.XtraPrinting.Native;
using System.Windows;
using System.Windows.Controls;

namespace Dental.Infrastructures.Commands
{
    public static class MyPrintHelper
    {
        public static Window mainWindow { get; set; }
        //public static Page page { get; set; }

        public static void PrintScheduler(SchedulerControl scheduler)
        {            
            XtraSchedulerReport1 report = new XtraSchedulerReport1();
            DateTimeRange dateTimeRange = scheduler.VisibleIntervals[0];
            scheduler.SchedulerPrintAdapter.DateTimeRange = dateTimeRange;
            scheduler.SchedulerPrintAdapter.AssignToReport(report);
            PrintHelper.ShowPrintPreview(mainWindow, report);
        }
    }
}
