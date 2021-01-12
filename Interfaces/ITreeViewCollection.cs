
namespace Dental.Interfaces
{
    interface ITreeViewCollection : ICollection
    {
        int? ParentId { get; set; }
        int ChildExists(ITreeViewCollection model);
        int DeleteDir(ITreeViewCollection model);
    }
}
