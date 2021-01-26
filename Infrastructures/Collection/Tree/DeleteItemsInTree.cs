using Dental.Interfaces;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Infrastructures.Collection.Tree
{
    class DeleteItemsInTree
    {
        private TreeListView Tree { get; set; }
        private ITreeViewCollection Model { get; set; }

        public DeleteItemsInTree(TreeListView tree, ITreeViewCollection model)
        {
            if (tree == null) throw new Exception("Empty tree");
            if (model == null) throw new Exception("Empty model");
            Tree = tree;
            Model = model;
        }

        public List<ITreeViewCollection> run()
        {
            List<ITreeViewCollection> list = Recursion(Model, new List<ITreeViewCollection>() { Model });
            
            return new List<ITreeViewCollection>();
        }



        public List<ITreeViewCollection> Recursion(ITreeViewCollection model, List<ITreeViewCollection> nodes)
        {
            if ( Tree.Nodes.Count > 0)
            {
                foreach (var node in Tree.Nodes)
                {
                    var row = (ITreeViewCollection)node.Content;
                    if (row.Id != model.Id && node.HasChildren == true)
                    {
                        //Recursion(row, node.Nodes);
                    }
                }

            }

            List<ITreeViewCollection> list = Tree.Nodes.Select(d => d.Content).
                Cast<ITreeViewCollection>().Where(d => d.ParentId == model.Id).ToList();

             if (list.Count > 0)
             {
                 foreach (ITreeViewCollection item in list)
                 {
                     if (item.ParentId != item.Id && item.Dir == 1) Recursion(item, nodes);
                     nodes.Add(item);
                 }
             }
             return nodes;
        }
    }
}
