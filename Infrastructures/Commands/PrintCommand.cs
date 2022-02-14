using System;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Grid;

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
            }

            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
    }
}
