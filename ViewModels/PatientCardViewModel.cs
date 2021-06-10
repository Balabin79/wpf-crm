using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Interfaces.Template;
using Dental.Models;
using Dental.Models.Base;
using Dental.Models.PatientCard;
using Dental.Repositories;
using DevExpress.Xpf.Grid;

namespace Dental.ViewModels
{
    class PatientCardViewModel : ViewModelBase//, ICollectionCommand
    {/*
        public PatientCardViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            UpdateCommand = new LambdaCommand(OnUpdateCommandExecuted, CanUpdateCommandExecute);
            CopyCommand = new LambdaCommand(OnCopyCommandExecuted, CanCopyCommandExecute);

            Repository = new PatientCardRepository();
            questionaries = Repository.GetQuestionaries();

            Repository.CopyModel += ((IModel, TableView) c) => {
                var copiedRow = Collection.Where(d => d.Id == ((Employee)c.Item2.FocusedRow)?.Id).FirstOrDefault();
                if (copiedRow != null)
                {
                    int index = Collection.IndexOf(copiedRow) + 1;
                    Collection.Insert(index, (PatientInfo)c.Item1);
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
            Repository.UpdateModel += ((IModel, TableView) c) => {
                var row = Collection.Where(d => d.Id == c.Item1.Id).FirstOrDefault();
                if (row != null)
                {
                    int index = Collection.IndexOf(row);
                    Collection[index] = (PatientInfo)c.Item1;
                }
            };
            Repository.AddModel += ((IModel, TableView) c) => {
                Collection.Add((PatientInfo)c.Item1);
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
        private bool CanCopyCommandExecute(object p) => true;


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
                Repository.Update(table);
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
                Repository.Copy(table);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        PatientCardRepository Repository { get; set; }

        private ObservableCollection<PatientInfo> _Collection;

        public ObservableCollection<PatientInfo> Collection
        {
            get
            {
                if (_Collection == null) _Collection = Repository.GetAll().Result;
                return _Collection;
            }
            set => Set(ref _Collection, value);
        }

        readonly ICollection<Questionary> questionaries;
        public ICollection<Questionary> Questionaries
        {
            get => questionaries;
        }

        public ICollection<Questionary> GenderList
        {
            get => questionaries.Where(i => i.ParentId == 1).ToList();
        }


        // Ортодонтическая карта
        public ICollection<Questionary> AreaList
        {
            get => questionaries.Where(i => i.ParentId == 4).ToList();
        }

        public ICollection<Questionary> MaritalStatusList
        {
            get => questionaries.Where(i => i.ParentId == 7).ToList();
        }

        public ICollection<Questionary> EducationList
        {
            get => questionaries.Where(i => i.ParentId == 12).ToList();
        }       
        
        public ICollection<Questionary> EmploymentList
        {
            get => questionaries.Where(i => i.ParentId == 23).ToList();
        }        
        
        public ICollection<Questionary> PaymentTypeList
        {
            get => questionaries.Where(i => i.ParentId == 31).ToList();
        }        
        
        public ICollection<Questionary> ExemptionCategoryList
        {
            get => questionaries.Where(i => i.ParentId == 37).ToList();
        }        
        
        public ICollection<Questionary> СomplaintsList
        {
            get => questionaries.Where(i => i.ParentId == 51).ToList();
        }        
        
        public ICollection<Questionary> MaternalHealthList // нарушение здоровья матери во время беременности
        {
            get => questionaries.Where(i => i.ParentId == 67).ToList();
        }        
        
        public ICollection<Questionary> TypeFeedingList // вид вскармливания
        {
            get => questionaries.Where(i => i.ParentId == 75).ToList();
        }   
        
        public ICollection<Questionary> BornList // рожден
        {
            get => questionaries.Where(i => i.ParentId == 72).ToList();
        }       
        
        public ICollection<Questionary> BadHabitsList // наличие вредных привычек
        {
            get => questionaries.Where(i => i.ParentId == 82).ToList();
        }*/



    }
}
