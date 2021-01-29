using System.Windows.Input;

namespace Dental.Interfaces.Template
{
    interface ICollectionCommand
    {
        ICommand DeleteCommand { get; }
        ICommand AddCommand { get; }
        ICommand UpdateCommand { get; }
        ICommand CopyCommand { get; }
    }
}
