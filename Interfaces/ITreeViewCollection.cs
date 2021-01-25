
namespace Dental.Interfaces
{
    interface ITreeViewCollection : ICollection
    {
        int ParentId { get; }
        int IsSys { get; set; }
        int IsDelete { get; set; }
    }
}
