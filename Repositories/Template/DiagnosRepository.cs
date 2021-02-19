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

namespace Dental.Repositories.Template
{
    class DiagnosRepository : AbstractTreeViewActionRepository
    {
        public async Task<ObservableCollection<Diagnos>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.Diagnoses.OrderBy( d=> d.Name).LoadAsync();
                return db.Diagnoses.Local;           
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<Diagnos>();
            }
        }

        public void Add(TreeListView tree)
        {
            try
            {
                Diagnos model = (Diagnos)tree.FocusedNode.Content;
                String NameDir = (model.Dir == 1) ? model.Name : ((Diagnos)tree.FocusedNode.ParentNode.Content).Name;

                if (model == null || !new ConfirmAddNewInCollection().run(NameDir)) return;
                int ParentId = (model.Dir == (int)TypeItem.Directory) ? model.Id : ((Diagnos)tree.FocusedNode.ParentNode.Content).Id;
                Diagnos diagnos = new Diagnos() { Dir = 0, Name = "Новый элемент", IsSys = 0, ParentId = ParentId };

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Diagnoses.Add(diagnos);
                    db.SaveChanges();
                    if(AddModel != null) AddModel((diagnos:diagnos, tree:tree));
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
                Diagnos model = (Diagnos)tree.FocusedNode.Content;
                using (ApplicationContext db = new ApplicationContext())
                {
                    Diagnos item = db.Diagnoses.Where(i => i.Id == model.Id).First();
                    if (item.Name != model.Name || item.Dir != model.Dir)
                    {
                        if (!new ConfirUpdateInCollection().run()) {
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
                var model = tree.FocusedRow as Diagnos;
                if (model == null || !new ConfirDeleteInCollection().run(model.Dir)) return;

                var listNodesIds = (new NodeChildren(tree.FocusedNode)).run().Select(d => d.Content).OfType<Diagnos>()
                    .ToList().Select(d => d.Id).ToList();
            
                var db = new ApplicationContext();
                var ListForRemove = db.Diagnoses.Where(d => listNodesIds.Contains(d.Id)).ToList();
                       
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
                Diagnos model = (Diagnos)tree.FocusedNode.Content;
                if (model == null || !new ConfirCopyInCollection().run(model.Dir)) return;
                var db = new ApplicationContext();
                Diagnos item = db.Diagnoses.Where(i => i.Id == model.Id).First();
                if (item == null) return;
                Diagnos newModel = new Diagnos()
                {
                    Dir = item.Dir,
                    Name = item.Name + " Копия",
                    IsSys = item.IsSys,
                    ParentId = item.ParentId,
                    IsDelete = item.IsDelete
                };               
                db.Diagnoses.Add(newModel);
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
