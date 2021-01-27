using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;

namespace Dental.Infrastructures.Collection.Tree
{
    class NodeChildren
    {
        private TreeListNode Model { get; set; }

        public NodeChildren(TreeListNode model)
        { 
            if (model == null) throw new Exception("Empty model");
            Model = model;
        }

        public List<TreeListNode> run()
        {
            return FindAllNodeChildren(new List <TreeListNode>(), Model);

        }

        public List<TreeListNode> FindAllNodeChildren(List<TreeListNode>nodes, TreeListNode node)
        {
            if (node.HasChildren == true)
            {
                foreach (var item in node.Nodes)
                {
                    if (item.HasChildren == true) FindAllNodeChildren(nodes, item);
                    nodes.Add(item);
                }
            }
            return nodes;
        }
    }
}
