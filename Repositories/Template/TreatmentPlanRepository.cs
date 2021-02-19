﻿using Dental.Models.Template;
using Dental.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Linq;
using DevExpress.Xpf.Grid;
using Dental.Infrastructures.Collection;
using Dental.Infrastructures.Collection.Tree;
using System.Threading.Tasks;
using DevExpress.Mvvm.Native;
using Dental.Enums;
using Dental.Infrastructures.Logs;
using Dental.Models.Base;

namespace Dental.Repositories.Template
{
    class TreatmentPlanRepository
    {
        public static Action<(IModel, TreeListView)> AddModel;
        public static Action<List<int>> DeleteModel;

        public static async Task<ObservableCollection<TreatmentPlan>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.TreatmentPlanes.OrderBy(d => d.Name).LoadAsync();
                return db.TreatmentPlanes.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<TreatmentPlan>();
            }
        }

        public static void Add(TreeListView tree)
        {
            try
            {
                TreatmentPlan model = (TreatmentPlan)tree.FocusedNode.Content;
                String NameDir = (model.Dir == 1) ? model.Name : ((TreatmentPlan)tree.FocusedNode.ParentNode.Content).Name;

                if (model == null || !new ConfirmAddNewInCollection().run(NameDir)) return;
                int ParentId = (model.Dir == (int)TypeItem.Directory) ? model.Id : ((TreatmentPlan)tree.FocusedNode.ParentNode.Content).Id;
                TreatmentPlan item = new TreatmentPlan() { Dir = 0, Name = "Новый элемент", IsSys = 0, ParentId = ParentId };

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.TreatmentPlanes.Add(item);
                    db.SaveChanges();
                    AddModel?.Invoke((item, tree));
                }
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

        public static void Update(TreeListView tree)
        {
            try
            {
                TreatmentPlan model = (TreatmentPlan)tree.FocusedNode.Content;
                using (ApplicationContext db = new ApplicationContext())
                {
                    TreatmentPlan item = db.TreatmentPlanes.Where(i => i.Id == model.Id).First();
                    if (model == null || item == null) return;

                    if (item.Name != model.Name || item.Dir != model.Dir)
                    {                       
                        item.Name = model.Name;
                        item.ParentId = model.ParentId;
                        item.Dir = model.Dir;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();                        
                    }

                }
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

        public static void Delete(TreeListView tree)
        {
            try
            {
                var model = tree.FocusedRow as TreatmentPlan;
                if (model == null || !new ConfirDeleteInCollection().run(model.Dir)) return;
                var listNodesIds = (new NodeChildren(tree.FocusedNode)).run().Select(d => d.Content).OfType<TreatmentPlan>()
                    .ToList().Select(d => d.Id).ToList();

                var db = new ApplicationContext();
                var ListForRemove = db.TreatmentPlanes.Where(d => listNodesIds.Contains(d.Id)).ToList();

                foreach (var item in ListForRemove)
                {
                    db.Entry(item).State = EntityState.Deleted;
                }
                db.SaveChanges();
                DeleteModel?.Invoke(listNodesIds);
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

        public static void Copy(TreeListView tree)
        {
            try
            {
                TreatmentPlan model = (TreatmentPlan)tree.FocusedNode.Content;
                if (model == null || !new ConfirCopyInCollection().run(model.Dir)) return;
                var db = new ApplicationContext();
                TreatmentPlan item = db.TreatmentPlanes.Where(i => i.Id == model.Id).First();
                if (item == null) return;
                TreatmentPlan newModel = new TreatmentPlan()
                {
                    Dir = item.Dir,
                    Name = item.Name + " Копия",
                    IsSys = item.IsSys,
                    ParentId = item.ParentId,
                    IsDelete = item.IsDelete
                };
                db.TreatmentPlanes.Add(newModel);
                db.SaveChanges();
                AddModel?.Invoke((newModel, tree));
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
    }
}
