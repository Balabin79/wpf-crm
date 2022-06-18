using System;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
//using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Grid;
//using DevExpress.Xpf.PivotGrid;
using DevExpress.Xpf.Scheduling;
using DevExpress.Xpf.Printing;


namespace Dental.Infrastructures.Commands
{
    class PrintCommand : CommandBase
    {
        public override bool CanExecute(object p) =>true;
        public override void Execute(object p)
        {
            try
            {
                if (p is TreeListView treeView)
                {
                    treeView.ShowPrintPreview(treeView);
                }

                if (p is TableView tableView)
                {
                    tableView.ShowPrintPreview(tableView);
                }

                /*if (p is ChartControl chart)
                {
                    chart.Print();
                }

                if (p is PivotGridControl pivot)
                {
                    pivot.ShowPrintPreview(pivot);
                }   */             
                
                if (p is SchedulerControl scheduler)
                {
                    MyPrintHelper.PrintScheduler(scheduler);
                }
            }

            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
    }
}
