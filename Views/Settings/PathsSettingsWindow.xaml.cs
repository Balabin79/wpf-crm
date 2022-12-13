
using DevExpress.Utils.CommonDialogs;
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

namespace Dental.Views.Settings
{
    /// <summary>
    /// Логика взаимодействия для PathsSettings.xaml
    /// </summary>
    public partial class PathsSettingsWindow : Window
    {
        public PathsSettingsWindow()
        {
            InitializeComponent();
        }

        private void Cancel_Form(object sender, RoutedEventArgs e) => Close();

        private void selectDbPath(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Файл базы данных SQLite3(dental.db) |*.db";
            var result = fileDialog.ShowDialog();
            // Retrieve the specified file name using the FileName property, e.g.:
            // Process save file dialog results
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Save document
                pathToDb.EditValue = fileDialog.FileName;
                pathToDbDefault.IsChecked = false;
            }
        }

        private void selectProgramDataPath(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            var result = folderDialog.ShowDialog();
            // Retrieve the specified file name using the FileName property, e.g.:
            // Process save file dialog results
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                programDataPath.EditValue = folderDialog.SelectedPath;
                pathToProgramFilesDefault.IsChecked = false;
            }
        }
    }
}
