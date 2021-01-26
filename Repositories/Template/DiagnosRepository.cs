using Dental.Models.Template;
using Dental.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Linq;
using Dental.Interfaces;
using DevExpress.Xpf.Grid;
using Dental.Infrastructures.Collection;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection.Tree;

namespace Dental.Repositories.Template
{
    class DiagnosRepository : IRepository
    {

        public static ObservableCollection<Diagnos> Collection { get; set; } = GetDiagnoses();
        public static TreeListView Tree { get; set; }

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


        public static void Add(TreeListView tree)
        {
            Diagnos model = (Diagnos)tree.FocusedNode.Content;           
            TreeListNode node = new TreeListNode() { Content = new Diagnos() { Dir = 0, Name = "Новый", IsSys = 0 }};
           
            if (model.Dir == 1) // директория
            {  
                tree.FocusedNode.Nodes.Add(node);
            }
               
            else
            {
                tree.FocusedNode.ParentNode.Nodes.Add(node);
            }
           
            tree.FocusedNode = node;
            tree.ShowEditForm();
        }


        public static void Update(Diagnos model)
        {
            int x = 0;

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

                var list = (new DeleteItemsInTree(tree, model)).run();


                return db.SaveChanges();
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
