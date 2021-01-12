using System.Linq;

namespace Dental.Interfaces
{
    interface IRepositoryCollection : IRepository
    {
       int ChildExists(ITreeViewCollection model);
       int DeleteDir(ITreeViewCollection model);
    }
}
