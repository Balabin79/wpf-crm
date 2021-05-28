using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Input;
using Dental.Enums;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Interfaces.Template;
using Dental.Models.Base;
using Dental.Models.Template;
using Dental.Repositories.Template;
using DevExpress.Xpf.Grid;

namespace Dental.ViewModels
{
    class TreatmentPlanViewModel : ViewModelBase, ICollectionCommand
    {
        public TreatmentPlanViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            UpdateCommand = new LambdaCommand(OnUpdateCommandExecuted, CanUpdateCommandExecute);
            CopyCommand = new LambdaCommand(OnCopyCommandExecuted, CanCopyCommandExecute);

            Repository = new TreatmentPlanRepository();
            Repository.AddModel += ((IModel, TreeListView) c) => {
                Collection.Add((TreatmentPlan)c.Item1);
                TreeListNode node;
                if (((TreatmentPlan)c.Item2.FocusedNode.Content).IsDir == (int)TypeItem.Directory)
                {
                    node = c.Item2.FocusedNode.Nodes.Where(d => ((TreatmentPlan)d.Content).Id == c.Item1.Id).FirstOrDefault();
                }
                else
                {
                    node = c.Item2.FocusedNode.ParentNode.Nodes.Where(d => ((TreatmentPlan)d.Content).Id == c.Item1.Id).FirstOrDefault();
                }
                if (node != null)
                {
                    c.Item2.FocusedNode = node;
                    c.Item2.ScrollIntoView(node.RowHandle);
                    //c.Item2.ShowEditForm();
                }
            };
            Repository.DeleteModel += (List<int> list) => {
                var itemsForRemove = Collection.Where(d => list.Contains(d.Id)).ToList();
                foreach (var item in itemsForRemove) Collection.Remove(item);
            };
            Repository.CopyModel += ((IModel, TreeListView) c) =>
            {
                var copiedRow = Collection.Where(d => d.Id == ((TreatmentPlan)c.Item2.FocusedRow)?.Id).FirstOrDefault();
                if (copiedRow != null)
                {
                    int index = Collection.IndexOf(copiedRow) + 1;
                    Collection.Insert(index, (TreatmentPlan)c.Item1);
                    var row = Collection.Where(d => d.Id == c.Item1.Id).FirstOrDefault();
                    if (row != null)
                    {
                        c.Item2.FocusedRow = row;
                        c.Item2.ScrollIntoView(c.Item1);
                        c.Item2.FocusedRow = c.Item1;
                        //c.Item2.ShowEditForm();
                    }
                }
            };
            Repository.UpdateModel += ((IModel, TreeListView) c) => {
                var row = Collection.Where(d => d.Id == c.Item1.Id).FirstOrDefault();
                if (row != null)
                {
                    int index = Collection.IndexOf(row);
                    Collection[index] = (TreatmentPlan)c.Item1;
                }
            };
        }

        public ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand CopyCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanAddCommandExecute(object p) => true;
        private bool CanUpdateCommandExecute(object p) => true;
        private bool CanCopyCommandExecute(object p) => true;


        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                var tree = p as TreeListView;
                if (tree == null) return;
                Repository.Delete(tree);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAddCommandExecuted(object p)
        {
            try
            {
                var tree = p as TreeListView;
                if (tree == null) return;
                Repository.Add(tree);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnUpdateCommandExecuted(object p)
        {
            try
            {
                var tree = p as DevExpress.Xpf.Grid.TreeListView;
                if (tree == null) return;
                Repository.Update(tree);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCopyCommandExecuted(object p)
        {
            try
            {
                var tree = p as DevExpress.Xpf.Grid.TreeListView;
                if (tree == null) return;
                Repository.Copy(tree);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        TreatmentPlanRepository Repository { get; set; }

        private ObservableCollection<TreatmentPlan> _Collection;

        [NotMapped]
        public  ObservableCollection<TreatmentPlan> Collection { 
            get {
                if (_Collection == null) _Collection = Repository.GetAll().Result;
                return _Collection; }
            set => Set(ref _Collection, value);
        }
    }
}
