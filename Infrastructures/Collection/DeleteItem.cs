using Dental.Interfaces;
using DevExpress.Xpf.Core;
using System.Windows;

namespace Dental.Infrastructures.Collection
{
    class DeleteItem
    {
        public int Run(ITreeViewCollection model)
        {
            var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: "Вы уверены что хотите удалить эту позицию?",
                   messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);

            if (response.ToString() == "Yes")
            {
                //IRepository repository = model.ClassRepository;
                return model.Delete(model);
            }
            return 0;
        }
    }
}
