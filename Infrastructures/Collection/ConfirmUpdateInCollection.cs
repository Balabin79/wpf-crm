using Dental.Enums;
using DevExpress.Xpf.Core;
using System;
using System.Windows;

namespace Dental.Infrastructures.Collection
{
    class ConfirUpdateInCollection
    {              
        public bool run()
        {
            try
            {
                 var response = ThemedMessageBox.Show(title: "Подтверждение действия",
                     text: "Сохранить отредактированные данные?",
                     messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);

                 if (response.ToString() == "Yes")
                 {
                     return true;
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
