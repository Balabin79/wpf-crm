using Dental.Models.Template;
using Dental.Models;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System;

namespace Dental.Repositories.Template
{
    class DiagnosRepository
    {

        public static ObservableCollection<Diagnos> Diagnoses { get => GetDiagnoses(); }

        public static ObservableCollection<Diagnos> GetDiagnoses()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Diagnoses.Load();
                    return db.Diagnoses.Local;
                }
            }
            catch (Exception e)
            {
                return new ObservableCollection<Diagnos>();
            }

        }
    }
}
