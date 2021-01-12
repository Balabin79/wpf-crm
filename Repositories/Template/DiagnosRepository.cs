using Dental.Models.Template;
using Dental.Models;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System;
using System.Linq;
using Dental.Interfaces;

namespace Dental.Repositories.Template
{
    class DiagnosRepository : IRepositoryCollection
    {

        public static ObservableCollection<ITreeViewCollection> Diagnoses { get => GetDiagnoses(); }

        public static ObservableCollection<ITreeViewCollection> GetDiagnoses()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                
                    db.Diagnoses.Load();
                    var x = db.Diagnoses.Local;
                return x;
            
            }
            catch (Exception e)
            {
                return new ObservableCollection<ITreeViewCollection>();
            }

        }

        public int Delete(ITreeViewCollection diagnos)
        {
            ApplicationContext db = new ApplicationContext();
            db.Entry(diagnos).State = EntityState.Deleted;
            db.Diagnoses.Remove(diagnos);
            return db.SaveChanges();
        }

        public int DeleteDir(ITreeViewCollection diagnos)
        {
            ApplicationContext db = new ApplicationContext();
            var query = db.Diagnoses.Where(d => d.ParentId == diagnos.Id && d.Id != diagnos.Id);
            query.ToList().ForEach(p => db.Entry(diagnos).State = EntityState.Deleted);
            query.ToList().ForEach(p => db.Diagnoses.Remove(p));

            //db.Diagnoses.Remove(diagnos);
            return db.SaveChanges();
        }

        public  int ChildExists(ITreeViewCollection diagnos)
        {
            ApplicationContext db = new ApplicationContext();
            return db.Diagnoses.Where(d => d.ParentId == diagnos.Id && d.Id != diagnos.Id).Count();
        }
    }
}
