using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;

namespace Dental.Infrastructures.Collection.Tree
{
    class NodeChildren : IDisposable
    {
        private TreeListNode Model { get; set; }

        public NodeChildren(TreeListNode model)
        { 
            if (model == null) throw new Exception("Empty model");
            Model = model;
        }

        public List<TreeListNode> run()
        {
            var list = FindAllNodeChildren(new List <TreeListNode>(), Model);
            list.Add(Model);
            return list;
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

        public void Dispose()
        {
            Dispose(true);
        }

        private bool _Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposing || _Disposed) return;
            _Disposed = true;
        }
    }
}
