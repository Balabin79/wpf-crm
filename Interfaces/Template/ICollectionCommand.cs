using System.Windows.Input;

namespace B6CRM.Interfaces.Template
{
    interface ICollectionCommand
    {
        ICommand DeleteCommand { get; }
        ICommand AddCommand { get; }
        ICommand UpdateCommand { get; }
        ICommand CopyCommand { get; }
    }
}
