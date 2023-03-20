using System;
using Dental.Infrastructures.Commands.Base;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Scheduling;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Charts;
using Dental.Services;

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

                if (p is ChartControl chart)
                {
                    chart.Print();
                }

                /*if (p is PivotGridControl pivot)
                {
                    pivot.ShowPrintPreview(pivot);
                }   */             
                
            }

            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
    }
}
