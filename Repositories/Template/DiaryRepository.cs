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
    class DiaryRepository : AbstractTreeViewActionRepository
    {
        public async Task<ObservableCollection<Diary>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.Diaries.OrderBy( d=> d.Name).LoadAsync();
                return db.Diaries.Local;           
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<Diary>();
            }
        }

        public void Add(TreeListView tree)
        {
            try
            {
                Diary model = (Diary)tree.FocusedNode.Content;
                String NameDir = (model.Dir == 1) ? model.Name : ((Diary)tree.FocusedNode.ParentNode.Content).Name;

                if (model == null || !new ConfirmAddNewInCollection().run(NameDir)) return;
                int ParentId = (model.Dir == (int)TypeItem.Directory) ? model.Id : ((Diary)tree.FocusedNode.ParentNode.Content).Id;
                Diary item = new Diary() { Dir = 0, Name = "Новый элемент", IsSys = 0, ParentId = ParentId };

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Diaries.Add(item);
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
                Diary model = (Diary)tree.FocusedNode.Content;                
                using (ApplicationContext db = new ApplicationContext())
                {
                    Diary item = db.Diaries.Where(i => i.Id == model.Id).First();
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
            try {
                var model = tree.FocusedRow as Diary;
                if (model == null || !new ConfirDeleteInCollection().run(model.Dir)) return;
                var listNodesIds = (new NodeChildren(tree.FocusedNode)).run().Select(d => d.Content).OfType<Diary>()
                    .ToList().Select(d => d.Id).ToList();
            
                var db = new ApplicationContext();
                var ListForRemove = db.Diaries.Where(d => listNodesIds.Contains(d.Id)).ToList();
                       
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
                Diary model = (Diary)tree.FocusedNode.Content;
                if (model == null || !new ConfirCopyInCollection().run(model.Dir)) return;
                var db = new ApplicationContext();
                Diary item = db.Diaries.Where(i => i.Id == model.Id).First();
                if (item == null) return;
                Diary newModel = new Diary()
                {
                    Dir = item.Dir,
                    Name = item.Name + " Копия",
                    IsSys = item.IsSys,
                    ParentId = item.ParentId,
                    IsDelete = item.IsDelete
                };
                db.Diaries.Add(newModel);
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
