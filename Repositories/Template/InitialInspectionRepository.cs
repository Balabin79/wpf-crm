using Dental.Models.Template;
using Dental.Models;
using System.Collections.ObjectModel;
using System.Data.Entity;

namespace Dental.Repositories.Template
{
    class InitialInspectionRepository
    {

        public static ObservableCollection<InitialInspection> InitialInspection { get => GetInitialInspectiones(); }

        public static ObservableCollection<InitialInspection> GetInitialInspectiones()
        {
            ApplicationContext db = new ApplicationContext();
            db.InitialInspectiones.Load();
            var x = db.InitialInspectiones.Local;
            return x;
        }
    }
}
