using Dental.Models.Template;
using Dental.Models;
using System.Collections.ObjectModel;
using System.Data.Entity;

namespace Dental.Repositories.Template
{
    class TreatmentPlanRepository
    {

        public static ObservableCollection<TreatmentPlan> TreatmentPlan { get => GetTreatmentPlanes(); }

        public static ObservableCollection<TreatmentPlan> GetTreatmentPlanes()
        {
            ApplicationContext db = new ApplicationContext();
            db.TreatmentPlanes.Load();
            var x = db.TreatmentPlanes.Local;
            return x;
        }
    }
}
