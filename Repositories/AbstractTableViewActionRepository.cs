using Dental.Interfaces;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using System;


namespace Dental.Repositories
{
    abstract class AbstractTableViewActionRepository : IRepository
    {
        public static Action<(IModel, TableView)> AddModel;
        public static Action<IModel> DeleteModel;
        public static Action<(IModel, TableView)> UpdateModel;
        public static Action<(IModel, TableView)> CopyModel;
    }
}
