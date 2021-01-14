
using System.Collections;
using System.Collections.ObjectModel;

namespace Dental.Interfaces
{
    interface ITreeViewCollection : ICollection
    {
        int? ParentId { get; }
    }
}
