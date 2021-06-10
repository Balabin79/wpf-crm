using System.Windows.Input;

namespace Dental.Interfaces
{
    interface ICollection
    {
        int? IsDir { get; set; }
        int Id { get; set; }
        string Name { get; set; }
    }
}
