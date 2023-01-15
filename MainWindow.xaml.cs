using Dental.Models;
using Dental.Services;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Navigation;
using System;
using System.IO;
using System.Windows;
using Dental.Views.About;
using Dental.ViewModels;
using License;

namespace Dental
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            try
            {              
                InitializeComponent();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message );
               
            }

        }

        private void ThemedWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string mes = "Завершить работу с приложением?";
          /*  if (Navigator.HasUnsavedChanges != null && Navigator.UserSelectedBtnCancel != null && Navigator.HasUnsavedChanges.Invoke()) mes = "Имеются несохраненные изменения. " + mes; */

            var response = ThemedMessageBox.Show(title: "Внимание", text: mes, messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

            if (response.ToString() == "No") 
            {
                e.Cancel = true;
                return;
            }
            e.Cancel = false;

           Application.Current.Shutdown();         
        }

        private void stat_Click(object sender, EventArgs e) => stat.IsSelected = true;

        private void temp_Click(object sender, EventArgs e) => templ.IsSelected = true;
        
    }
}
