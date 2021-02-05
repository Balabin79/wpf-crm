using Dental.Infrastructures.Collection;
using Dental.Infrastructures.Logs;
using Dental.Interfaces;
using Dental.Models;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dental.Repositories
{
    class EmployeeStatusRepository
    {
        public static Action<(EmployeeStatus, TableView)> AddModel;
        public static void RegisterAddModel(Action<(EmployeeStatus, TableView)> action) => AddModel += action;

        public static Action<List<int>> DeleteModel;
        public static void RegisterDeleteModel(Action<List<int>> action) => DeleteModel += action;

        public static async Task<ObservableCollection<EmployeeStatus>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.EmployeeStatuses.OrderBy(d => d.Name).LoadAsync();
                return db.EmployeeStatuses.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<EmployeeStatus>();
            }
        }
        /*
        public static void Add(TableView tree)
        {
            try
            {
                EmployeeStatus model = (EmployeeStatus)tree.FocusedRow;
                String NameDir = (model.Dir == 1) ? model.Name : ((EmployeeStatus)tree.FocusedNode.ParentNode.Content).Name;

                if (model == null || !new ConfirmAddNewInCollection().run(NameDir)) return;
                int ParentId = (model.Dir == (int)TypeItem.Directory) ? model.Id : ((EmployeeStatus)tree.FocusedNode.ParentNode.Content).Id;
                Diary item = new Diary() { Dir = 0, Name = "Новый элемент", IsSys = 0, ParentId = ParentId };

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.EmployeeStatuses.Add(item);
                    db.SaveChanges();
                    if (AddModel != null) AddModel((item, tree));
                }
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

        public static void Update(TableView tree)
        {
            try
            {
                EmployeeStatus model = (EmployeeStatus)tree.FocusedNode.Content;
                using (ApplicationContext db = new ApplicationContext())
                {
                    EmployeeStatus item = db.EmployeeStatuses.Where(i => i.Id == model.Id).First();
                    if (model == null || item == null) return;

                    if (item.Name != model.Name)
                    {
                        if (!new ConfirUpdateInCollection().run()) return;
                    }
                    item.Name = model.Name;
                    item.ParentId = model.ParentId;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

        public static void Delete(TableView tree)
        {
            try
            {
                var model = tree.FocusedRow as EmployeeStatus;
                if (model == null || !new ConfirDeleteInCollection().run(model.Dir)) return;
                var listNodesIds = (new NodeChildren(tree.FocusedNode)).run().Select(d => d.Content).OfType<EmployeeStatus>()
                    .ToList().Select(d => d.Id).ToList();

                var db = new ApplicationContext();
                var ListForRemove = db.EmployeeStatuses.Where(d => listNodesIds.Contains(d.Id)).ToList();

                foreach (var item in ListForRemove)
                {
                    db.Entry(item).State = EntityState.Deleted;
                }
                db.SaveChanges();
                DeleteModel(listNodesIds);
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

        public static void Copy(TableView tree)
        {
            try
            {
                EmployeeStatus model = (EmployeeStatus)tree.FocusedNode.Content;
                if (model == null || !new ConfirCopyInCollection().run(model.Dir)) return;
                var db = new ApplicationContext();
                EmployeeStatus item = db.Diaries.Where(i => i.Id == model.Id).First();
                if (item == null) return;
                EmployeeStatus newModel = new EmployeeStatus()
                {
                    Dir = item.Dir,
                    Name = item.Name + " Копия",
                    IsSys = item.IsSys,
                    ParentId = item.ParentId,
                    IsDelete = item.IsDelete
                };
                db.EmployeeStatuses.Add(newModel);
                db.SaveChanges();
                if (AddModel != null) AddModel((newModel, tree));
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }*/

    }
}
