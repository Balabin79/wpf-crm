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
            AssociatedObject.CommandBindings.Add(new CommandBinding(GridCommands.EditFocusedRow, OnEditExecuted));
          //  AssociatedObject.CommandBindings.Add(new CommandBinding(((TreeListView)AssociatedObject)., OnEditExecuted));
       




        }

        void OnDeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var _FocucedRow = (e.Source as TreeListView).FocusedRow;
            ((ITreeViewCollection)AssociatedObject.DataContext).DeleteCommand.Execute(_FocucedRow);
        }

        void OnAddNewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((ITreeViewCollection)AssociatedObject.DataContext).AddCommand.Execute(e.Source);
        }

        void OnEditExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((ITreeViewCollection)AssociatedObject.DataContext).UpdateCommand.Execute((e.Source as TreeListView).FocusedRow);
        }


    }
}
