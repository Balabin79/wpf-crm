using Dental.Models.Template;
using Dental.Models;
using System.Collections.ObjectModel;
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

namespace Dental.Repositories.Template
{
    class TreatmentPlanRepository : AbstractTreeViewActionRepository
    {
        public async Task<ObservableCollection<TreatmentPlan>> GetAll()
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

        public void Add(TreeListView tree)
        {
            try
            {
                TreatmentPlan item;
                TreatmentPlan model = (TreatmentPlan)tree.FocusedNode?.Content;
                if (model == null)
                {
                    model = new TreatmentPlan() { IsDir = 1, Name = "План лечения", IsSys = 1, ParentId = 0 };
                    if (!new ConfirmAddNewInCollection().run(model.Name)) return;
                    item = model;
                }
                else
                {
                    string NameDir = (model.IsDir == 1) ? model.Name : ((TreatmentPlan)tree.FocusedNode.ParentNode.Content).Name;
                    if (!new ConfirmAddNewInCollection().run(NameDir)) return;
                    int ParentId = (model.IsDir == (int)TypeItem.Directory) ? model.Id : ((TreatmentPlan)tree.FocusedNode.ParentNode.Content).Id;
                    item = new TreatmentPlan() { IsDir = 0, Name = "Новый элемент", IsSys = 0, ParentId = ParentId };
                }

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

        public void Update(TreeListView tree)
        {
            try
            {
                TreatmentPlan model = (TreatmentPlan)tree.FocusedNode.Content;
                using (ApplicationContext db = new ApplicationContext())
                {
                    TreatmentPlan item = db.TreatmentPlanes.Where(i => i.Id == model.Id).First();
                    if (item.Name != model.Name || item.IsDir != model.IsDir)
                    {
                        if (!new ConfirUpdateInCollection().run())
                        {
                            UpdateModel?.Invoke((item, tree));
                            return;
                        }
                        item.Name = model.Name;
                        item.ParentId = model.ParentId;
                        item.IsDir = model.IsDir;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    UpdateModel?.Invoke((item, tree));

                }
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

        public void Delete(TreeListView tree)
        {
            try
            {
                var model = tree.FocusedRow as TreatmentPlan;
                if (model == null || !new ConfirDeleteInCollection().run(model.IsDir)) return;
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

        public void Copy(TreeListView tree)
        {
            try
            {
                TreatmentPlan model = (TreatmentPlan)tree.FocusedNode.Content;
                if (model == null || !new ConfirCopyInCollection().run(model.IsDir)) return;
                var db = new ApplicationContext();
                TreatmentPlan item = db.TreatmentPlanes.Where(i => i.Id == model.Id).First();
                if (item == null) return;
                TreatmentPlan newModel = new TreatmentPlan()
                {
                    IsDir = item.IsDir,
                    Name = item.Name + " Копия",
                    IsSys = item.IsSys,
                    ParentId = item.ParentId,
                    IsDelete = item.IsDelete
                };
                db.TreatmentPlanes.Add(newModel);
                db.SaveChanges();
                CopyModel?.Invoke((newModel, tree));
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
    }
}
