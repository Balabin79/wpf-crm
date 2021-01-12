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
        }

        void OnDeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var _FocucedRow = (e.Source as TreeListView).FocusedRow;
            string nameClass = _FocucedRow.GetType().Name;

            switch(nameClass) 
            { 
                case "Diagnos" : ((DiagnosRepository)AssociatedObject.DataContext).DeleteCommand.Execute(_FocucedRow); break;
               
            }






            //vm.DeleteCommand.Execute(_FocucedRow);
        }


    }
}
