using Dental.Enums;
using DevExpress.Xpf.Core;
using System;
using System.Windows;

namespace Dental.Infrastructures.Collection
{
    class ConfirDeleteInCollection
    {              
        public bool run(int? category)
        {
            try
            {
                if (category == null)
                {

                    var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: "Вы уверены что хотите удалить этот элемент?",
                           messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);

                    if (response.ToString() == "Yes") return true;
                    return false;
                }
                if (category == (int)TypeItem.Directory)
                {
                    var response = ThemedMessageBox.Show(title: "Подтверждение действия",
                        text: "Вы уверены что хотите удалить директорию? Всё содержимое этой директории также будет удалено",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);

                    if (response.ToString() == "Yes")
                    {
                        return true;
                    }
                }
                else
                {
                    var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: "Вы уверены что хотите удалить эту позицию?",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);

                    if (response.ToString() == "Yes")
                    {
                        return true;
                    }
                }
                return false;

            }
            catch 
            {
                return false;
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }
        }

       
    }
}
