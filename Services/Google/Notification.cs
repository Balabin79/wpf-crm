using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.Services.Google
{
    public class Notification : INotification
    {
        public void ShowMsgError(string msg) => ThemedMessageBox.Show(
                title: "Ошибка",
                text: msg,
                messageBoxButtons: MessageBoxButton.OK,
                icon: MessageBoxImage.Error
                );
      

        public void ShowSuccessMsg(string msg) => ThemedMessageBox.Show(
            title: "Успешно выполнено", 
            text: msg, messageBoxButtons: 
            MessageBoxButton.OK, icon: 
            MessageBoxImage.Information
            );
    }
}
