using B6CRM.Infrastructures.Extensions.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace B6CRM.Views.About
{
    /// <summary>
    /// Логика взаимодействия для LicenseWindow.xaml
    /// </summary>
    public partial class LicenseWindow : Window
    {
        public LicenseWindow()
        {
            InitializeComponent();
        }


        private void LoadLicense(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Файл лицензии(dental.license) |*.license";
            var result = fileDialog.ShowDialog();
            // Retrieve the specified file name using the FileName property, e.g.:
            // Process save file dialog results
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Save document
                pathToLicense.EditValue = fileDialog.FileName;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.HardwareID?.Text))
                {
                    if (this.HardwareID.Text == "Недоступно") return;
                    System.Windows.Clipboard.SetData(System.Windows.DataFormats.Text, (Object)this.HardwareID.Text);
                    new Notification() { Content = "Скопировано в буфер обмена!" }.run();
                }            
            }
            catch
            {

            }
        }
    }
}
