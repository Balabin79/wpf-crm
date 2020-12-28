
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

    public class FullActionDataGridBehavior : DeleteRowDataGridBehaviors
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.CommandBindings.Add(new CommandBinding(GridCommands.EndEditFocusedRow, OnEditExecuted));
            AssociatedObject.CommandBindings.Add(new CommandBinding(GridCommands.AddNewRow, OnAddNewExecuted));
        }

        void OnEditExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("OnEditExecuted");

            var _FocucedRow = (e.Source as TableView).FocusedRow;
            //var vm = AssociatedObject.DataContext as ViewModel1;
            //vm.EditCommand.Execute(_FocucedRow);
        }

        void OnAddNewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("OnAddNewExecuted");

            // what do we need to do else ? I want to show Inline Form.  

            (e.Source as TableView).ShowEditor();
        }
    }
}
