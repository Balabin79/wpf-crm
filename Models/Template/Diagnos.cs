﻿using Dental.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Dental.Repositories.Template;
using Dental.Infrastructures.Commands.Base;
using DevExpress.Xpf.Core;
using System;
using Dental.Interfaces;
using Dental.Infrastructures.Collection;
using System.Linq;
using System.Data.Entity;

namespace Dental.Models.Template
{
    [Table("Diagnoses")]
    class Diagnos : ViewModelBase, ITreeViewCollection
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ParentId")]
        public int? ParentId { get; set; }

        [Required]
        [Column("Name")]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Column("Dir")]
        [Display(Name = "Директория")]
        public int Dir { get; set; }

       /* public IRepositoryCollection ClassRepository 
        {
            get => new DiagnosRepository();
        }*/

        public Diagnos()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
        }

        
        public ICommand DeleteCommand { get; }
        private bool CanDeleteCommandExecute(object p) => true;
        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                (new DeleteStrategy((ITreeViewCollection)p)).run();
            }
            catch (Exception e)
            {

                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }
        }

        [NotMapped]
        public ObservableCollection<Diagnos> Collection {
            get {
                if (_diagnoses == null) _diagnoses = GetDiagnoses();
                return _diagnoses;
            }
            set {
                Set(ref _diagnoses, value); 
            }
        }
        private ObservableCollection<Diagnos>  _diagnoses;


        public int Delete(ITreeViewCollection diagnos)
        {
            Diagnos model = (Diagnos)diagnos;
            ApplicationContext db = new ApplicationContext();
            db.Entry(diagnos).State = EntityState.Deleted;
            db.Diagnoses.Remove(model);
            return db.SaveChanges();
        }

        public int DeleteDir(ITreeViewCollection diagnos)
        {
            Diagnos model = (Diagnos)diagnos;
            ApplicationContext db = new ApplicationContext();
            var query = db.Diagnoses.Where(d => d.ParentId == diagnos.Id && d.Id != diagnos.Id);
            query.ToList().ForEach(p => db.Entry(diagnos).State = EntityState.Deleted);
            query.ToList().ForEach(p => db.Diagnoses.Remove(p));

            //db.Diagnoses.Remove(diagnos);
            return db.SaveChanges();
        }

        public int ChildExists(ITreeViewCollection diagnos)
        {
            ApplicationContext db = new ApplicationContext();
            return db.Diagnoses.Where(d => d.ParentId == diagnos.Id && d.Id != diagnos.Id).Count();
        }



        public static ObservableCollection<Diagnos> GetDiagnoses()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();

                db.Diagnoses.Load();
                return db.Diagnoses.Local;


            }
            catch (Exception e)
            {
                return new ObservableCollection<Diagnos>();
            }

        }

    }
}
