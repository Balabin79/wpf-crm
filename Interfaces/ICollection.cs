using System.Windows.Input;

namespace B6CRM.Interfaces
{
    interface ICollection
    {
        int? IsDir { get; set; }
        int Id { get; set; }
        string Name { get; set; }
    }
}
