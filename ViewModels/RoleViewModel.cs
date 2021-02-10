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
    class RoleViewModel : ViewModelBase, ICollectionCommand
    {
        public RoleViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            UpdateCommand = new LambdaCommand(OnUpdateCommandExecuted, CanUpdateCommandExecute);
            CopyCommand = new LambdaCommand(OnCopyCommandExecuted, CanCopyCommandExecute);

            RoleRepository.CopyModel += ((Role, TableView) c) => {
                var copiedRow = Collection.Where(d => d.Id == ((Role)c.Item2.FocusedRow)?.Id).FirstOrDefault();
                if (copiedRow != null)
                {
                    int index = Collection.IndexOf(copiedRow) + 1;
                    Collection.Insert(index, c.Item1);
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
            RoleRepository.UpdateModel += ((Role, TableView) c) => {
                var row = Collection.Where(d => d.Id == c.Item1.Id).FirstOrDefault();
                if (row != null)
                {
                    int index = Collection.IndexOf(row);
                    Collection.Remove(row);
                    Collection.Insert(index, c.Item1);
                    c.Item2.FocusedRow = row;
                    c.Item2.ScrollIntoView(c.Item1);
                    c.Item2.FocusedRow = c.Item1;
                    //c.Item2.ShowEditForm();
                }
            };
            RoleRepository.AddModel += ((Role, TableView) c) => {
                Collection.Add(c.Item1);
                var row = Collection.Where(d => d.Id == c.Item1.Id).FirstOrDefault();

                if (row != null)
                {
                    c.Item2.FocusedRow = row;
                    c.Item2.ScrollIntoView(row);
                    //c.Item2.ShowEditForm();
                }
            };
            RoleRepository.DeleteModel += (Role model) => {
                var item = Collection.Where(d => d.Id == model.Id).FirstOrDefault();
                if (item != null) Collection.Remove(item);
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
                var table = p as TableView;
                if (table == null) return;
                RoleRepository.Delete(table);
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
                RoleRepository.Add(table);
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
                RoleRepository.Update(table);
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
                RoleRepository.Copy(table);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private ObservableCollection<Role> _Collection;

        [NotMapped]
        public ObservableCollection<Role> Collection
        {
            get
            {
                if (_Collection == null) _Collection = RoleRepository.GetAll().Result;
                return _Collection;
            }
            set => Set(ref _Collection, value);
        }
    }
}
