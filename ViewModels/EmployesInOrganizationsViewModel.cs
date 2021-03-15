using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Dental.Enums;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Interfaces.Template;
using Dental.Models;
using Dental.Models.Base;
using Dental.Repositories;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

namespace Dental.ViewModels
{
    class EmployesInOrganizationsViewModel : ViewModelBase, ICollectionCommand
    {

        public EmployesInOrganizationsViewModel()
        {
            Repository = new EmployesInOrganizationsRepository();
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
              
            UpdateCommand = new LambdaCommand(OnUpdateCommandExecuted, CanUpdateCommandExecute);      

              Repository.UpdateModel += ((List<EmployesInOrganizations>, TableView table) c) => {
                   
                  foreach (var i in c.Item1)
                  {
                      Collection.Add(i);
                  }
                  Collection?.Where(v => v.EmployeeId == null).ToList().ForEach(v => Collection.Remove(v));
                  c.Item2.CloseEditForm();
              };
              Repository.AddModel += ((IModel, TableView) c) => {
                  Collection.Add((EmployesInOrganizations)c.Item1);
                  var row = Collection.Where(d => d.Id == c.Item1.Id).FirstOrDefault();

                  if (row != null)
                  {
                      c.Item2.FocusedRow = row;
                      c.Item2.ScrollIntoView(row);
                      //c.Item2.ShowEditForm();
                  }
              };
              Repository.DeleteModel += (IModel model) => {
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


        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                var table = p as TableView;
                if (table == null) return;
                Repository.Delete(table);
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
                Repository.Add(table);
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
                Repository.Update(table, SetEmployee, SetOrgList);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        EmployesInOrganizationsRepository Repository { get; set; }

        private ObservableCollection<EmployesInOrganizations> _Collection;

        [NotMapped]
        public ObservableCollection<EmployesInOrganizations> Collection
        {
            get
            {
                if (_Collection == null) _Collection = Repository.GetAll().Result;
                return _Collection;
            }
            set => Set(ref _Collection, value);
        }

        private object _SetOrgList;
        [NotMapped]
        public object SetOrgList {
            get => _SetOrgList;
            set => Set(ref _SetOrgList, value);
        }

        private object _SetEmployee;
        [NotMapped]
        public object SetEmployee {
            get => _SetEmployee;
            set
            {
                Set(ref _SetEmployee, value);
                if (_SetEmployee != null) setFields(Repository.GetEmployeeOrgs(_SetEmployee));
            }
        }

        private ObservableCollection<Organization> _GetOrgListEmployee;
        [NotMapped]
        public ObservableCollection<Organization> GetOrgListEmployee { 
            get => new OrganizationRepository().GetAll().Result;
            set => Set(ref _GetOrgListEmployee, value);
        }

        [NotMapped]
        public ObservableCollection<Employee> GetEmployeeList
        {
            get => new EmployeeRepository().GetAll().Result;
        }

        private void setFields(object result)
        {
            try
            {
                ObservableCollection<EmployesInOrganizations> Orgs = (ObservableCollection<EmployesInOrganizations>)result;
                var query = Orgs.Where(i => i.EmployeeId == (int)SetEmployee);             
                SetOrgList = query.Select(i => i.Id).ToList();
                query.Select(i => i.Organization).ToList().ForEach(i => GetOrgListEmployee.Add(i));
          
            } catch (Exception e)
            {
               
            }
            
        }
    }
}
