using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using System;
using Dental.Interfaces;
using Dental.Models.Base;
using Dental.Repositories.Template;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Services;
using DevExpress.Xpf.Grid;

namespace Dental.Models.Template
{
    [Table("Diary")]
    class Diary : TreeModelBase, ITreeViewCollection
    {

        public Diary()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            UpdateCommand = new LambdaCommand(OnUpdateCommandExecuted, CanUpdateCommandExecute);
        }


        public ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanAddCommandExecute(object p) => true;
        private bool CanUpdateCommandExecute(object p) => true;


        private void OnDeleteCommandExecuted(object p)
        {
            try
            {

                var tree = p as TreeListView;
                if (tree == null) return;
                DiaryRepository.Delete(tree);

            }
            catch (Exception e)
            {

                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }
        }

        private void OnAddCommandExecuted(object p)
        {
            try
            {
                var tree = p as TreeListView;
                if (tree == null) return;
                DiaryRepository.Add(tree);
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
                Diary model = (Diary)tree.FocusedRow;
                int x = 0;

            }
            catch (Exception e)
            {

                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }
        }


        [NotMapped]
        public static ObservableCollection<Diary> Collection { get; set; } = DiaryRepository.Collection;

    }
}