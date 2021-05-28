
namespace Dental.Interfaces
{
    interface ITreeViewCollection : ICollection
    {
        int? ParentId { get; }
    }
}
