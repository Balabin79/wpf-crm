using Dental.Infrastructures.Commands;
using Dental.Interfaces.Template;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System.Windows.Input;

namespace Dental.Behaviors
{
    public class DeleteRowDataGridBehaviors : Behavior<TableView>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.CommandBindings.Add(new CommandBinding(GridCommands.DeleteFocusedRow, OnDeleteExecuted));
            AssociatedObject.CommandBindings.Add(new CommandBinding(GridCommands.AddNewRow, OnAddNewExecuted));
            //AssociatedObject.CommandBindings.Add(new CommandBinding(GridCommands.EditFocusedRow, OnEditExecuted));              
            AssociatedObject.CommandBindings.Add(new CommandBinding(TreeCommands.CopyRowCommand, OnCopyExecuted));
        }

        void OnDeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((ICollectionCommand)AssociatedObject.DataContext).DeleteCommand.Execute(e.Source);
        }

        void OnAddNewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((ICollectionCommand)AssociatedObject.DataContext).AddCommand.Execute(e.Source);
        }

        void OnEditExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((ICollectionCommand)AssociatedObject.DataContext).UpdateCommand.Execute(e.Source);
        }

        void OnCopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((ICollectionCommand)AssociatedObject.DataContext).CopyCommand.Execute(e.Source);
        }
    }
}
