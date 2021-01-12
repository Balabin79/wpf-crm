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
                    return db.Diagnoses.Local;
            
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
            db.Entry(diagnos).State = EntityState.Deleted;
            db.Diagnoses.Remove(diagnos);
            return db.SaveChanges();
        }

        public  int ChildExists(ITreeViewCollection diagnos)
        {
            ApplicationContext db = new ApplicationContext();
            return db.Diagnoses.Where(d => d.ParentId == diagnos.Id).Count();
        }
    }
}
