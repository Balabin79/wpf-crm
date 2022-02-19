using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace Dental.Infrastructures.Extensions.Notifications
{
    public class Notification : AbstractNotification
    {
        public override string Content { get; set; } = "Новый сотрудник успешно записан в базу данных!";
        public override ImageSource Icon { get; set; } = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/save.png"));

        public override StreamResourceInfo Sound
        {
            get => Application.GetResourceStream(new Uri(@"pack://application:,,,/Dental;component/Resources/Sounds/Notifications/success.wav"));
        }

        public override void run()
        {
            base.run();
        }
    }
}
