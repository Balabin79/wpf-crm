using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using DevExpress.Xpf.Core;

namespace B6CRM.Views.About
{
    /// <summary>
    /// Логика взаимодействия для InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            //e.Handled = true;
            try
            {
                if (sender.GetType() != typeof(Hyperlink)) return;
                string link = ((Hyperlink)sender).NavigateUri.ToString();
                Process.Start(link);
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Не удалось запустить браузер по умолчанию!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Exclamation);
            }
        }
    }
}
