using Dental.Enums;
using DevExpress.Xpf.Core;
using System;
using System.Windows;

namespace Dental.Infrastructures.Collection
{
    class ConfirmAddNewInCollection
    {              
        public bool run(string name)
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Подтверждение действия",
                    text: "Создать новый элемент в директории \"" + name + "\"?",
                    messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);

                if (response.ToString() == "Yes")
                {
                    return true;
                }
                return false;

            }
            catch
            {
                return false;
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }
        }

        public bool run()
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Подтверждение действия",
                    text: "Создать новый элемент?",
                    messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);

                if (response.ToString() == "Yes")
                {
                    return true;
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
