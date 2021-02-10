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

namespace Dental.Repositories.Template
{
    class DiaryRepository
    {
        public static Action<(Diary, TreeListView)> AddModel;
        public static void RegisterAddModel(Action<(Diary, TreeListView)> action) => AddModel += action;

        public static Action<List<int>> DeleteModel;
        public static void RegisterDeleteModel(Action<List<int>> action) => DeleteModel += action;

        public static async Task<ObservableCollection<Diary>> GetAll()
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

        public static void Add(TreeListView tree)
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
                    if(AddModel != null) AddModel((item, tree));
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
                Diary model = (Diary)tree.FocusedNode.Content;                
                using (ApplicationContext db = new ApplicationContext())
                {
                    Diary item = db.Diaries.Where(i => i.Id == model.Id).First();
                    if (model == null || item == null) return;

                    if (item.Name != model.Name)
                    { 
                        if (!new ConfirUpdateInCollection().run()) return;
                        else
                        {
                            item.Name = model.Name;
                            item.ParentId = model.ParentId;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
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
                DeleteModel(listNodesIds);            
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
                if (AddModel != null) AddModel((newModel, tree));
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
    }
}
