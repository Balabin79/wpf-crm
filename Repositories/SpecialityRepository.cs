using Dental.Models.Template;
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

namespace Dental.Repositories
{
    class SpecialityRepository : AbstractTreeViewActionRepository
    {
        public async Task<ObservableCollection<Speciality>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.Specialities.OrderBy(d => d.Name).LoadAsync();
                return db.Specialities.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<Speciality>();
            }
        }

        public void Add(TreeListView tree)
        {
            try
            {
                Speciality model = (Speciality)tree.FocusedNode.Content;

                String NameDir = (model.Dir == 1) ? model.Name :
                    model.ParentId != 0 ? ((Speciality)tree.FocusedNode?.ParentNode.Content).Name : "Без категории";

                if (model == null || !new ConfirmAddNewInCollection().run(NameDir)) return;
                int ParentId = (model.Dir == (int)TypeItem.Directory) ? model.Id : ((Speciality)tree.FocusedNode.ParentNode.Content).Id;
                Speciality speciality = new Speciality() { Dir = 0, Name = "Новый элемент", IsSys = 0, ParentId = ParentId, ShowInShedule = 0};

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Specialities.Add(speciality);
                    db.SaveChanges();
                    if (AddModel != null) AddModel((speciality, tree));
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
                Speciality model = (Speciality)tree.FocusedNode.Content;
                using (ApplicationContext db = new ApplicationContext())
                {
                    Speciality item = db.Specialities.Where(i => i.Id == model.Id).First();
                    if (item.Name != model.Name || item.Dir != model.Dir)
                    {
                        if (!new ConfirUpdateInCollection().run())
                        {
                            UpdateModel?.Invoke((item, tree));
                            return;
                        }
                        item.Name = model.Name;
                        item.ParentId = model.ParentId;
                        item.Dir = model.Dir;
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
                var model = tree.FocusedRow as Speciality;
                if (model == null || !new ConfirDeleteInCollection().run(model.Dir)) return;

                var listNodesIds = (new NodeChildren(tree.FocusedNode)).run().Select(d => d.Content).OfType<Speciality>()
                    .ToList().Select(d => d.Id).ToList();

                var db = new ApplicationContext();
                var ListForRemove = db.Specialities.Where(d => listNodesIds.Contains(d.Id)).ToList();

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
                Speciality model = (Speciality)tree.FocusedNode.Content;
                if (model == null || !new ConfirCopyInCollection().run(model.Dir)) return;
                var db = new ApplicationContext();
                Speciality item = db.Specialities.Where(i => i.Id == model.Id).First();
                if (item == null) return;
                Speciality newModel = new Speciality()
                {
                    Dir = item.Dir,
                    Name = item.Name + " Копия",
                    IsSys = item.IsSys,
                    ParentId = item.ParentId,
                    IsDelete = item.IsDelete
                };
                db.Specialities.Add(newModel);
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
