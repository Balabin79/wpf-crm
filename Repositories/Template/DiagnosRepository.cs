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

namespace Dental.Repositories.Template
{
    

    class DiagnosRepository
    {
        public ObservableCollection<Diagnos> Collection { get; set; } = GetAllAsync().Result;


        public static async Task<ObservableCollection<Diagnos>> GetAllAsync()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    await db.Diagnoses.LoadAsync();
                    return db.Diagnoses.Local;
                }
            }
            catch (Exception e)
            {
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
                    //db.Entry(diagnos).State = EntityState.Added;
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
                    item.Name = model.Name;
                    item.ParentId = model.ParentId;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
            } 
            catch(Exception e)
            {

            }

        }


        public static int Delete(TreeListView tree)
        {
            TreeListView Tree = tree;
            var model = tree.FocusedRow as Diagnos;
            if (model == null || !new DeleteInTree().run(model.Dir)) return 0;

            using (ApplicationContext db = new ApplicationContext())
            {/*
                var list = Recursion(model, new List<Diagnos>() { model });

                list.ToList().ForEach(d => list.ToList().Remove(d));
                list.ToList().ForEach(d => Collection.Remove(d));
                return db.SaveChanges();*/

                var list = (new NodeChildren(tree.FocusedNode)).run();

                return 1;
                //return db.SaveChanges();
            }
        }

        public static List<Diagnos> Recursion(Diagnos model, List<Diagnos> nodes)
        {
            /* List<Diagnos> list = Collection.Where(d => d.ParentId == model.Id).ToList();

             if (list.Count > 0)
             {
                 foreach (Diagnos item in list)
                 {
                     if (item.ParentId != item.Id && item.Dir == 1) Recursion(item, nodes);
                     nodes.Add(item);
                 }
             }
             return nodes;*/
            
            //Where(d => d.ParentId == model.Id).ToList();
            return nodes;
        }
    }
}
