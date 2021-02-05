using Dental.Models;
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
        }

        void OnDeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var _FocucedRow = (e.Source as TableView).FocusedRow;
            string nameClass = _FocucedRow.GetType().Name;

            switch(nameClass) 
            { 
                case "Organization" : ((Organization)AssociatedObject.DataContext).DeleteCommand.Execute(_FocucedRow); break;
                case "Role" : ((Role)AssociatedObject.DataContext).DeleteCommand.Execute(_FocucedRow); break;
                case "Speciality" : ((Speciality)AssociatedObject.DataContext).DeleteCommand.Execute(_FocucedRow); break;
                //case "EmployeeStatus" : ((EmployeeStatus)AssociatedObject.DataContext).DeleteCommand.Execute(_FocucedRow); break;
            }






            //vm.DeleteCommand.Execute(_FocucedRow);
        }


    }
}
