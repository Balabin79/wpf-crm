namespace B6CRM.Interfaces
{
    interface ITreeViewCollection : ICollection
    {
        int? ParentId { get; }
    }
}
