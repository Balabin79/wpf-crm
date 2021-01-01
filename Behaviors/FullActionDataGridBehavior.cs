using Dental.Models;
using Dental.ViewModels;
using Dental.Views.HandbooksPages;
using DevExpress.Xpf.Grid;
using DevExpress.XtraPrinting;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Dental.Behaviors
{

    public class FullActionDataGridBehavior : DeleteRowDataGridBehaviors
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.CommandBindings.Add(new CommandBinding(GridCommands.EditFocusedRow, OnUpdateRowExecuted));
            AssociatedObject.CommandBindings.Add(new CommandBinding(GridCommands.AddNewRow, OnAddNewExecuted));
        }

        void OnUpdateRowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var _FocucedRow = (e.Source as TableView).FocusedRow;
            string nameClass = _FocucedRow.GetType().Name;

            switch (nameClass)
            {
                case "Organization": ((Organization)AssociatedObject.DataContext).EditCommand.Execute(_FocucedRow); break;
            }
        }

        void OnAddNewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            int x = 0;
             ((Organization)AssociatedObject.DataContext).AddCommand.Execute(sender);
        }
    }
}
