using Dental.Interfaces;
using Dental.Models;
using Dental.Models.Template;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Dental.Infrastructures.Collection
{
    class DeleteInTree
    {              
        public bool run(int category)
        {
            try
            {
                if (category == 1)
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
            catch (Exception e)
            {
                return false;
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }
        }

       
    }
}
