using Dental.Interfaces;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using System;


namespace Dental.Repositories
{
    abstract class AbstractTableViewActionRepository : IRepository
    {
        public Action<(IModel, TableView)> AddModel;
        public Action<IModel> DeleteModel;
        public Action<(IModel, TableView)> UpdateModel;
        public Action<(IModel, TableView)> CopyModel;
    }
}
