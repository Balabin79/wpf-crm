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


    class DiagnosRepository
    {
        public static ObservableCollection<Diagnos> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                db.Diagnoses.OrderBy( d=> d.Name).Load();
                return db.Diagnoses.Local;           
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<Diagnos>();
            }
        }


        public static void Add(TreeListView tree)
        {
            try
            {
                Diagnos model = (Diagnos)tree.FocusedNode.Content;

                int ParentId = (model.Dir == (int)TypeItem.Directory) ? model.Id : ((Diagnos)tree.FocusedNode.ParentNode.Content).Id;
                Diagnos diagnos = new Diagnos() { Dir = 0, Name = "Новый элемент", IsSys = 0, ParentId = ParentId };

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Diagnoses.Add(diagnos);
                    db.SaveChanges();
                }

                TreeListNode node = new TreeListNode() { Content = diagnos };

                if (model.Dir == (int)TypeItem.Directory) // директория
                {
                    ((Diagnos)node.Content).ParentId = ParentId;
                    tree.FocusedNode.Nodes.Insert(0, node);
                }
                else
                {
                    ((Diagnos)node.Content).ParentId = ParentId;
                    tree.FocusedNode.ParentNode.Nodes.Insert(0, node);
                }
                tree.FocusedRowHandle = node.RowHandle;
                //   tree.ShowEditForm();
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
                Diagnos model = (Diagnos)tree.FocusedNode.Content;
                using (ApplicationContext db = new ApplicationContext())
                {
                    Diagnos item = db.Diagnoses.Where(i => i.Id == model.Id).First();
                    if (item == null) return;
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


        public static void Delete(TreeListView tree, Delegate deleteItemsInCollection)
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
                    db.Diagnoses.Remove(item);
                }
          
                db.SaveChanges();
                deleteItemsInCollection.DynamicInvoke(listNodesIds);            
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
                Diagnos model = (Diagnos)tree.FocusedNode.Content;
                var db = new ApplicationContext();
                Diagnos item = db.Diagnoses.Where(i => i.Id == model.Id).First();
                if (item == null) return;
                Diagnos newModel = new Diagnos()
                {
                    Dir = item.Dir,
                    Name = item.Name,
                    IsSys = item.IsSys,
                    ParentId = item.ParentId,
                    IsDelete = item.IsDelete
                };
                
                db.Diagnoses.Add(newModel);
                db.SaveChanges();
                
                TreeListNode node = new TreeListNode() { Content = newModel };               
                tree.FocusedNode.ParentNode.Nodes.Insert(0, node);
                tree.FocusedRowHandle = node.RowHandle;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
    }
}
