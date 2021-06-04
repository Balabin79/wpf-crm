using Dental.Enums;
using DevExpress.Xpf.Core;
using System;
using System.Windows;

namespace Dental.Infrastructures.Collection
{
    class TryingCreatingDuplicate
    {              
        public bool run(int category)
        {
            try
            {
                if (category == (int)TypeItem.Directory)
                {
                    var response = ThemedMessageBox.Show(title: "Ошибка",
                        text: "Директория с таким названием и на данном уровне уже существует. Необходимо изменить название!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                }
                else
                {
                    var response = ThemedMessageBox.Show(title: "Ошибка", text: "Элемент с таким название в текущей директории уже существует. Необходимо изменить название!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                }
                return false;

            }
            catch (Exception e)
            {
                return false;
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }
        }

       
    }
}
