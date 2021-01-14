using System.Windows.Input;

namespace Dental.Interfaces
{
    interface ICollection
    {
        int Dir { get; set; }
        int Id { get; set; }
        string Name { get; set; }
        ICommand DeleteCommand { get; }
        ICommand AddCommand { get; }
    }
}
