using Dental.Interfaces;
using DevExpress.Xpf.Core;
using System.Windows;

namespace Dental.Infrastructures.Collection
{
    class DeleteDir
    {
        public int Run(ITreeViewCollection model)
        {
            var response = ThemedMessageBox.Show(title: "Подтверждение действия", 
                text: "Вы уверены что хотите удалить директорию? Всё содержимое этой директории также будет удалено", 
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);

            if (response.ToString() == "Yes")
            {
               // IRepositoryCollection repository = model.ClassRepository;
                if (model.ChildExists(model) < 1) return model.Delete(model);
                return model.DeleteDir(model);
            }
            return 0;
        }
    }
}
