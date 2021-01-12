using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Dental.Interfaces
{
    interface ICollection
    {
        
        IRepositoryCollection ClassRepository { get; }
        int Dir { get; set; }
        int Id { get; set; }
        string Name { get; set; }
       // ObservableCollection<ITreeViewCollection> Collection { get; set; }
        ICommand DeleteCommand { get; }
    }
}
