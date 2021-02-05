using Dental.Enums;
using DevExpress.Xpf.Core;
using System;
using System.Windows;

namespace Dental.Infrastructures.Collection
{
    class ConfirCopyInCollection
    {              
        public bool run(int category)
        {
            try
            {
                if (category == (int)TypeItem.Directory)
                {
                    var response = ThemedMessageBox.Show(title: "Подтверждение действия",
                        text: "Скопировать директорию?",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);

                    if (response.ToString() == "Yes")
                    {
                        return true;
                    }
                }
                else
                {
                    var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: "Скопировать эту позицию?",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);

                    if (response.ToString() == "Yes")
                    {
                        return true;
                    }
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
