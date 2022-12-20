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
using System.Timers;

namespace Dental.Services
{
    public class CheckDBConnection
    {
        public void Run()
        {
            bool successConn = false;
            while (!successConn) successConn = CheckConnectionToDb();
        }

        private bool CheckConnectionToDb()
        {
            bool openDbSuccess = false;
            do
            {
                try
                {
                    Timer timer = new Timer() { Interval = 10000};
                    timer.Start();
                    using (var conn = new SQLiteConnection()
                    {
                        ConnectionString = new SQLiteConnectionStringBuilder()
                        {
                            DataSource = new Config().ConnectionString,
                            Version = 3,
                            BusyTimeout = 10000,
                            FailIfMissing = true,
                         
                        }.ConnectionString
                    })
                    {
                        try
                        {
                            var t = Task.Run(() => conn.Open());
                            if (!t.Wait(10000))
                            {
                                conn.Close();
                                conn.Dispose();
                                throw new Exception("исключение по таймауту");
                            }
                        }
                        catch (ObjectDisposedException e)
                        {

                        }

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
    }
}
