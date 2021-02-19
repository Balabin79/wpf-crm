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
    class DiagnosViewModel : ViewModelBase, ICollectionCommand
    {
        public DiagnosViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            UpdateCommand = new LambdaCommand(OnUpdateCommandExecuted, CanUpdateCommandExecute);
            CopyCommand = new LambdaCommand(OnCopyCommandExecuted, CanCopyCommandExecute);
            DiagnosRepository.AddModel += ((IModel, TreeListView) c) => {
                Collection.Add((Diagnos)c.Item1);
                TreeListNode node;
                if (((Diagnos)c.Item2.FocusedNode.Content).Dir == (int)TypeItem.Directory)
                {
                    node = c.Item2.FocusedNode.Nodes.Where(d => ((Diagnos)d.Content).Id == c.Item1.Id).FirstOrDefault();
                }
                else
                {
                    node = c.Item2.FocusedNode.ParentNode.Nodes.Where(d => ((Diagnos)d.Content).Id == c.Item1.Id).FirstOrDefault();
                }
                if (node != null)
                {
                    c.Item2.FocusedNode = node;
                    c.Item2.ScrollIntoView(node.RowHandle);
                    //c.Item2.ShowEditForm();
                }
            };
            DiagnosRepository.DeleteModel += (List<int> list) => {
                var itemsForRemove = Collection.Where(d => list.Contains(d.Id)).ToList();
                foreach (var item in itemsForRemove) Collection.Remove(item);               
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
                DiagnosRepository.Delete(tree);
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
                DiagnosRepository.Add(tree);
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
                DiagnosRepository.Update(tree);
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
                DiagnosRepository.Copy(tree);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }               

        private ObservableCollection<Diagnos> _Collection;

        [NotMapped]
        public  ObservableCollection<Diagnos> Collection { 
            get {
                if (_Collection == null) _Collection = DiagnosRepository.GetAll().Result;
                return _Collection; }
            set => Set(ref _Collection, value);
        }
    }
}
