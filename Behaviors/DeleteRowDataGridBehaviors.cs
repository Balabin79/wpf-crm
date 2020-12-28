using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Dental.Models;
using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;

namespace Dental.Behaviors
{
    public class DeleteRowDataGridBehaviors : Behavior<TableView>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.CommandBindings.Add(new CommandBinding(GridCommands.DeleteFocusedRow, OnDeleteExecuted));
        }

        void OnDeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var _FocucedRow = (e.Source as TableView).FocusedRow;
            var vm = AssociatedObject.DataContext as Organization;
            vm.DeleteCommand.Execute(_FocucedRow);
        }
    }
}
