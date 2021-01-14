using Dental.Models.Template;
using Dental.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Linq;
using Dental.Interfaces;


namespace Dental.Repositories.Template
{
    class DiagnosRepository : IRepository
    {

        public static ObservableCollection<Diagnos> Collection { get; set; } = GetDiagnoses();

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


        public static int Delete(Diagnos model)
        {

            using (ApplicationContext db = new ApplicationContext())
            {
                var list = Recursion(model, new List<Diagnos>() { model });

                list.ToList().ForEach(d => list.ToList().Remove(d));
                list.ToList().ForEach(d => Collection.Remove(d));
                return db.SaveChanges();
            }
        }

        public static List<Diagnos> Recursion(Diagnos model, List<Diagnos> nodes)
        {
            List<Diagnos> list = Collection.Where(d => d.ParentId == model.Id).ToList();

            if (list.Count > 0)
            {
                foreach (Diagnos item in list)
                {
                    if (item.ParentId != item.Id && item.Dir == 1) Recursion(item, nodes);
                    nodes.Add(item);
                }
            }
            return nodes;
        }
    }
}
