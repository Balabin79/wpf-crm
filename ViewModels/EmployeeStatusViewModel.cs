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
using Dental.Models;
using Dental.Repositories;
using DevExpress.Xpf.Grid;

namespace Dental.ViewModels
{
    class EmployeeStatusViewModel : ViewModelBase, ICollectionCommand
    {
        public EmployeeStatusViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            UpdateCommand = new LambdaCommand(OnUpdateCommandExecuted, CanUpdateCommandExecute);
            CopyCommand = new LambdaCommand(OnCopyCommandExecuted, CanCopyCommandExecute);
            EmployeeStatusRepository.AddModel += addItem;
            EmployeeStatusRepository.DeleteModel += deleteItems;
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
                var table = p as TableView;
                if (table == null) return;
                //EmployeeStatusRepository.Delete(table);
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
                var table = p as TableView;
                if (table == null) return;
                //EmployeeStatusRepository.Add(table);
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
                var table = p as DevExpress.Xpf.Grid.TableView;
                if (table == null) return;
                //EmployeeStatusRepository.Update(table);
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
                var table = p as DevExpress.Xpf.Grid.TableView;
                if (table == null) return;
                //EmployeeStatusRepository.Copy(table);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void addItem((EmployeeStatus, TableView) c)
        {
            Collection.Add(c.Item1);
            /*TreeListNode node;
            if (((Diary)c.Item2.FocusedNode.Content).Dir == (int)TypeItem.Directory)
            {
                node = c.Item2.FocusedNode.Nodes.Where(d => ((Diary)d.Content).Id == c.Item1.Id).FirstOrDefault();
            }
            else
            {
                node = c.Item2.FocusedNode.ParentNode.Nodes.Where(d => ((Diary)d.Content).Id == c.Item1.Id).FirstOrDefault();
            }
            if (node != null)
            {
                c.Item2.FocusedNode = node;
                c.Item2.ScrollIntoView(node.RowHandle);
                //c.Item2.ShowEditForm();
            }*/
        }

        private void deleteItems(List<int> list)
        {
            var itemsForRemove = Collection.Where(d => list.Contains(d.Id)).ToList();
            foreach (var item in itemsForRemove)
            {
                Collection.Remove(item);
            }
        }

        private ObservableCollection<EmployeeStatus> _Collection;

        [NotMapped]
        public ObservableCollection<EmployeeStatus> Collection
        {
            get
            {
                if (_Collection == null) _Collection = EmployeeStatusRepository.GetAll().Result;
                return _Collection;
            }
            set => Set(ref _Collection, value);
        }
    }
}
