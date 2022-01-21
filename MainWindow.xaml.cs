using Dental.Services;
using DevExpress.Xpf.Core;
using System.Windows;

namespace Dental
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ThemedWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var response = ThemedMessageBox.Show(title: "Внимание", text: "Завершить работу с приложением?",
messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

            if (response.ToString() == "No") 
            {
                e.Cancel = true;
                return;
            }
            Navigation.Instance?.LastPageSaving();
            e.Cancel = false;
            Application.Current.Shutdown();
        }
    }
}
