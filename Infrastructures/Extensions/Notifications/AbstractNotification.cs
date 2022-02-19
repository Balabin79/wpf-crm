using DevExpress.Mvvm;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace Dental.Infrastructures.Extensions.Notifications
{

    public abstract class AbstractNotification : INotification
    {
        public virtual string Caption { get; set; } = "Уведомление";
        public virtual string Content { get; set; } = "Новая запись";
        public virtual ImageSource Icon { get; set; } = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/save.png"));
    
        public virtual StreamResourceInfo Sound
        {
            get => Application.GetResourceStream(new Uri(@"pack://application:,,,/Dental;component/Resources/Sounds/Notifications/success.wav"));
        }

        public INotificationService NotificationService
        {
            get => ServiceContainer.Default.GetService<INotificationService>("NotificationService");
        }



        protected void playSound()
        {
            using (var s = Sound.Stream)
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(s);
                player.Load();
                player.Play();
            }
        }

        protected void ShowNotification()
        {
            NotificationService.CreateCustomNotification(this).ShowAsync();
        }

        public virtual void run()
        {
            playSound();
            ShowNotification();
        }
    }

}
