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
    class DiaryRepository : IRepository
    {

        public static ObservableCollection<Diary> Collection { get; set; } = GetCollection();
        public static TreeListView Tree { get; set; }

        public static ObservableCollection<Diary> GetCollection()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                db.Diaries.Load();
                return db.Diaries.Local;
            }
            catch (Exception e)
            {
                return new ObservableCollection<Diary>();
            }
        }


        public static void Add(TreeListView tree)
        {
            Diary model = (Diary)tree.FocusedNode.Content;
            TreeListNode node = new TreeListNode() { Content = new Diary() { Dir = 0, Name = "Новый", IsSys = 0 } };

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


        public static void Update(Diary model)
        {
            int x = 0;

        }


        public static int Delete(TreeListView tree)
        {
            TreeListView Tree = tree;
            var model = tree.FocusedRow as Diary;
            if (model == null || !new DeleteInTree().run(model.Dir)) return 0;

            using (ApplicationContext db = new ApplicationContext())
            {/*
                var list = Recursion(model, new List<Diagnos>() { model });

                list.ToList().ForEach(d => list.ToList().Remove(d));
                list.ToList().ForEach(d => Collection.Remove(d));
                return db.SaveChanges();*/

               // var list = (new DeleteItemsInTree(tree, model)).run();


                return db.SaveChanges();
            }
        }

        public static List<Diary> Recursion(Diary model, List<Diary> nodes)
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
