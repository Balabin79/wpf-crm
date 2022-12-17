using Dental.ViewModels;
using Dental.Views.Settings;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.Services
{
    public class CheckDBConnection
    {
        public void Run()
        {
            bool successConn = false;
            while (!successConn) successConn = CheckFileDbExists() && CheckConnectionToDb();
        }

        private bool CheckConnectionToDb()
        {
            bool openDbSuccess = false;
            do
            {
                try
                {
                    using (var conn = new SQLiteConnection()
                    {
                        ConnectionString = new SQLiteConnectionStringBuilder()
                        {
                            DataSource = Config.ConnectionString,
                            Version = 3
                        }.ConnectionString
                    })
                    {

                        conn.Open();
                        openDbSuccess = true;
                    }
                }
                catch (Exception e)
                {
                    ThemedMessageBox.Show(
                        title: "Ошибка",
                        text: "Ошибка при попытке подключения к базе данных. Если используется локальная сеть, то, возможно, в данный момент недоступно сетевое расположение файла. Проверьте настройки подключения к базе данных.",
                        messageBoxButtons: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error, windowStartupLocation: WindowStartupLocation.CenterOwner
                        );

                    new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
                    continue;
                }
            }
            while (!openDbSuccess);
            return true;
        }

        private bool CheckFileDbExists()
        {
            while (!File.Exists(Config.ConnectionString))
            {
                ThemedMessageBox.Show(
                    title: "Ошибка",
                    text: "Не найден файл базы данных. Если используется локальная сеть, то, возможно, в данный момент недоступно сетевое расположение файла. Проверьте настройки подключения к базе данных.",
                    messageBoxButtons: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error, windowStartupLocation: WindowStartupLocation.CenterOwner
                    );

                new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
            }
            return true;
        }
    }
}
