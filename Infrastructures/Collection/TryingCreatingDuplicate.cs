using Dental.Enums;
using DevExpress.Xpf.Core;
using System;
using System.Windows;

namespace Dental.Infrastructures.Collection
{
    class TryingCreatingDuplicate
    {              
        public bool run(int? category)
        {
            try
            {
                var answer = "Yes";
                if (category == null || category == (int)TypeItem.Directory)
                {
                    var response = ThemedMessageBox.Show(title: "Ошибка",
                        text: "Директория с таким названием и на данном уровне уже существует. Продолжить?",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    answer = response.ToString();
                }
                else
                {
                    var response = ThemedMessageBox.Show(title: "Ошибка", text: "Элемент с таким название в текущей директории уже существует. Продолжить?",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    answer = response.ToString();
                }
                return answer == "Yes";
            }
            catch 
            {
                return false;
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }
        }

       
    }
}
