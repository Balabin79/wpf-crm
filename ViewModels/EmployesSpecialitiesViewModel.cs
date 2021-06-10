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
    class EmployesSpecialitiesViewModel : ViewModelBase, ICollectionCommand
    {

        public EmployesSpecialitiesViewModel()
        {
            Repository = new EmployesSpecialitiesRepository();
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
              
            UpdateCommand = new LambdaCommand(OnUpdateCommandExecuted, CanUpdateCommandExecute);      

              Repository.UpdateModel += ((List<EmployesSpecialities>, TableView table) c) => {
                   
                  foreach (var i in c.Item1)
                  {
                      Collection.Add(i);
                  }
                  Collection?.Where(v => v.EmployeeId == null).ToList().ForEach(v => Collection.Remove(v));
                  c.Item2.CloseEditForm();
              };
              Repository.AddModel += ((IModel, TableView) c) => {
                  Collection.Add((EmployesSpecialities)c.Item1);
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
                Repository.Update(table, SetEmployee, SetSpecialityList);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        EmployesSpecialitiesRepository Repository { get; set; }

        private ObservableCollection<EmployesSpecialities> _Collection;

        [NotMapped]
        public ObservableCollection<EmployesSpecialities> Collection
        {
            get
            {
                if (_Collection == null) _Collection = Repository.GetAll().Result;
                return _Collection;
            }
            set => Set(ref _Collection, value);
        }

        private object _SetSpecialityList;
        [NotMapped]
        public object SetSpecialityList {
            get => _SetSpecialityList;
            set => Set(ref _SetSpecialityList, value);
        }

        private object _SetEmployee;
        [NotMapped]
        public object SetEmployee {
            get => _SetEmployee;
            set
            {
                Set(ref _SetEmployee, value);
                if (_SetEmployee != null) setFields(Repository.GetEmployeeSpecialities(_SetEmployee));
            }
        }

        private ObservableCollection<Speciality> _GetSpecialityListEmployee;
        [NotMapped]
        public ObservableCollection<Speciality> GetSpecialityListEmployee { 
            //get => new SpecialityRepository().GetAll().Result;
            set => Set(ref _GetSpecialityListEmployee, value);
        }
/*
        [NotMapped]
        public ObservableCollection<Employee> GetEmployeeList
        {
            get => new EmployeeRepository().GetAll().Result;
        }
*/
        private void setFields(object result)
        {
            try
            {
                ObservableCollection<EmployesSpecialities> Specialities = (ObservableCollection<EmployesSpecialities>)result;
                var query = Specialities.Where(i => i.EmployeeId == (int)SetEmployee);             
                SetSpecialityList = query.Select(i => i.SpecialityId).ToList();
               // query.Select(i => i.Speciality).ToList().ForEach(i => GetSpecialityListEmployee.Add(i));
          
            } catch (Exception e)
            {
                int y = 0; 
            }
            
        }
    }
}
