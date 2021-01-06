using Dental.Models.Template;
using Dental.Models;
using System.Collections.ObjectModel;
using System.Data.Entity;

namespace Dental.Repositories.Template
{
    class DiagnosRepository
    {

        public static ObservableCollection<Diagnos> Diagnoses { get => GetDiagnoses(); }

        public static ObservableCollection<Diagnos> GetDiagnoses()
        {
            ApplicationContext db = new ApplicationContext();
            db.Diagnoses.Load();
            var x = db.Diagnoses.Local;
            return x;
        }
    }
}
