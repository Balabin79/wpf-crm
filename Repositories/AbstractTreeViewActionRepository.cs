using Dental.Interfaces;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;

namespace Dental.Repositories
{
    abstract class AbstractTreeViewActionRepository : IRepository
    {
        public Action<(IModel, TreeListView)> AddModel;
        public Action<List<int>> DeleteModel;
        public Action<(IModel, TreeListView)> UpdateModel;
        public Action<(IModel, TreeListView)> CopyModel;    
    }
}
