using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Interfaces;
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
                DeleteItemsInCollection del = deleteItems;
                DiagnosRepository.Delete(tree, del);


                //var rem = Collection.Where(d => list.Contains(d.Id)).ToList();


               /* foreach (var item in rem)
                {
                    Collection.Remove(item);
                }*/
                //var rem = Collection.Where(d => d.Id == 2).FirstOrDefault();
                //if (rem != null) Collection.Remove(rem);
            }
            catch (Exception e)
            {
                int x = 0;
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
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

                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
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

                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
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

                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }
        }

        delegate void DeleteItemsInCollection(List<int> list);       

        private void deleteItems(List<int> list)
        {
            var itemsForRemove = Collection.Where(d => list.Contains(d.Id)).ToList();
            foreach (var item in itemsForRemove)
            {
                Collection.Remove(item);
            }
            
        }

        private ObservableCollection<Diagnos> _Collection;

        [NotMapped]
        public  ObservableCollection<Diagnos> Collection { 
            get {
                if (_Collection == null) _Collection = DiagnosRepository.GetAll();
                return _Collection; }
            set => Set(ref _Collection, value);
        }
    }
}
