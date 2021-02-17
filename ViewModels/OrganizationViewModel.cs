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
    class OrganizationViewModel: ViewModelBase, ICollectionCommand
    {
        public OrganizationViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            UpdateCommand = new LambdaCommand(OnUpdateCommandExecuted, CanUpdateCommandExecute);
            CopyCommand = new LambdaCommand(OnCopyCommandExecuted, CanCopyCommandExecute);

            OrganizationRepository.CopyModel += ((Organization, TableView) c) => {
                var copiedRow = Collection.Where(d => d.Id == ((Organization)c.Item2.FocusedRow)?.Id).FirstOrDefault();
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
            OrganizationRepository.UpdateModel += ((Organization, TableView) c) => {
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
            OrganizationRepository.AddModel += ((Organization, TableView) c) => {
                Collection.Add(c.Item1);
                var row = Collection.Where(d => d.Id == c.Item1.Id).FirstOrDefault();

                if (row != null)
                {
                    c.Item2.FocusedRow = row;
                    c.Item2.ScrollIntoView(row);
                    //c.Item2.ShowEditForm();
                }
            };
            OrganizationRepository.DeleteModel += (Organization model) => {
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
                OrganizationRepository.Delete(table);
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
                OrganizationRepository.Add(table);
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
                OrganizationRepository.Update(table);
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
                OrganizationRepository.Copy(table);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private ObservableCollection<Organization> _Collection;

        public ObservableCollection<Organization> Collection
        {
            get
            {
                if (_Collection == null) _Collection = OrganizationRepository.GetAll().Result;
                return _Collection;
            }
            set => Set(ref _Collection, value);
        }
    }
}
