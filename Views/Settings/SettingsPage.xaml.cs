﻿using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dental.Views.Settings
{
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void CheckEdit_Checked(object sender, RoutedEventArgs e)
        {
            if (e.Source is CheckEdit field && field.IsChecked == true)
            {
                ThemedMessageBox.Show(title: "Внимание", text: "Ролевой доступ включен, чтобы применить изменения необходимо нажать кнопку \"Сохранить\" и перезапустить приложение!",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Information);
            }
        }

        private void CheckEdit_Unchecked(object sender, RoutedEventArgs e)
        {
            if (e.Source is CheckEdit field && field.IsChecked == false)
            {
                ThemedMessageBox.Show(title: "Внимание", text: "Ролевой доступ выключен, чтобы применить изменения необходимо нажать кнопку \"Сохранить\" и перезапустить приложение!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Information);
                this.passEnabled.EditValue = false;
            }
        }

        private void CheckEditPass_Checked(object sender, RoutedEventArgs e)
        {
            if (e.Source is CheckEdit field && field.IsChecked == true)
            {
                ThemedMessageBox.Show(title: "Внимание", text: "Вход по паролю включен, чтобы применить изменения необходимо нажать кнопку \"Сохранить\" и перезапустить приложение!",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Information);
            }
        }

        private void CheckEditPass_Unchecked(object sender, RoutedEventArgs e)
        {
            if (e.Source is CheckEdit field && field.IsChecked == false)
            {
                ThemedMessageBox.Show(title: "Внимание", text: "Вход по паролю выключен, чтобы применить изменения необходимо нажать кнопку \"Сохранить\" и перезапустить приложение!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Information);
            }       
        }
    }
}
