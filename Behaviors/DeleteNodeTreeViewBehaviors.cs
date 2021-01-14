using Dental.Infrastructures.Collection;
using Dental.Interfaces;
using Dental.Models;
using Dental.Models.Template;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System.Windows.Input;

namespace Dental.Behaviors
{
    public class DeleteNodeTreeViewBehaviors : Behavior<TreeListView>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.CommandBindings.Add(new CommandBinding(GridCommands.DeleteFocusedRow, OnDeleteExecuted));
            AssociatedObject.CommandBindings.Add(new CommandBinding(GridCommands.AddNewRow, OnAddNewExecuted));
        }

        void OnDeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var _FocucedRow = (e.Source as TreeListView).FocusedRow;
            //new DeleteInTree().run((Diagnos)_FocucedRow);
            ((ITreeViewCollection)AssociatedObject.DataContext).DeleteCommand.Execute(_FocucedRow);
            //((ITreeViewCollection)AssociatedObject.DataContext).DeleteCommand.Execute(_FocucedRow);
        }

        void OnAddNewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((ITreeViewCollection)AssociatedObject.DataContext).AddCommand.Execute((e.Source as TreeListView).FocusedRow);
        }


    }
}
