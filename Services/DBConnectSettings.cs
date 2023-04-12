using B6CRM.Infrastructures.Extensions.Notifications;
using B6CRM.Models;
using B6CRM.Models.Base;
using B6CRM.ViewModels;
using B6CRM.Views.WindowForms;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace B6CRM.Services
{
    public class DBConnectSettings : DevExpress.Mvvm.ViewModelBase
    {
        private ApplicationContext db;

        public DBConnectSettings() 
        {
           // db = new ApplicationContext();
            Config = new Config();
            Timeout = 7;
            if (File.Exists(Config.PathToConfig))
            {
                var json = File.ReadAllText(Config.PathToConfig).Trim();
                if (json.Length > 10 && JsonSerializer.Deserialize(json, new StoreConnectToDb().GetType()) is StoreConnectToDb config)
                {
                    DbType = config.Db;
                    PostgresConnect = config.PostgresConnect;
                }
            }

            if (PostgresConnect == null) PostgresConnect = new PostgresConnect();
        }


        [AsyncCommand]
        public async Task SaveConfig()
        {
            IsWaitIndicatorVisible = true;
                   
            try
            {              
                StoreConnectToDb connect = new StoreConnectToDb();
                // пытаемся применить новые параметры подключения
                if (DbType == null || DbType == 0)
                {
                    Config.DbType = 0;
                    Config.ConnectionString = Config.PathToDbDefault;
                    PostgresConnect = null;
                }
                else
                {
                    var isConnected = await Task<bool>.Run(() => { return TryingConnect(); });
                    if (!isConnected)
                    {
                        IsWaitIndicatorVisible = false;
                        var response = ThemedMessageBox.Show(title: "Внимание!", text: "Не удалось установить подключение к базе данных! Сохранить введенные параметры подключения?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning, defaultButton: MessageBoxResult.No);
                        if (response.ToString() == "No")  return;                     
                    }

                    Config.DbType = 1;
                    Config.ConnectionString = $"Host={PostgresConnect?.Host ?? ""};Port={PostgresConnect?.Port};Database={PostgresConnect?.Database ?? ""};Username={PostgresConnect?.Username ?? ""};Password={PostgresConnect?.Password ?? ""};";
                }

                ConfigHandler();

                if (IsConfigChanged) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();

                IsConfigChanged = false;  
            }           
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Не удалось сохранить настройки подключения к базе данных!", true);
            }
            finally
            {
                IsWaitIndicatorVisible = false;
            }
            
        }

        private bool TryingConnect() => CheckNetworkConnect.IsConnectSuccess(PostgresConnect?.Host, (PostgresConnect?.Port ?? 5432), Timeout);
  
        private void ConfigHandler()
        {
            // сравниваем значения из формы с теми что из файла, если изменились, то изменяем флаг оповещения
            if (File.Exists(Config.PathToConfig))
            {
                var json = File.ReadAllText(Config.PathToConfig).Trim();

                if (json.Length > 10 && JsonSerializer.Deserialize(json, new StoreConnectToDb().GetType()) is StoreConnectToDb file)
                {
                    // значения не поменялись
                    if (
                        DbType == file.Db
                        && Config.ConnectionString == file.ConnectionString
                        && PostgresConnect?.Database == file.PostgresConnect?.Database
                        && PostgresConnect?.Password == file.PostgresConnect?.Password
                        && PostgresConnect?.Username == file.PostgresConnect?.Username
                        && PostgresConnect?.Port == file.PostgresConnect?.Port
                        && PostgresConnect?.Host == file.PostgresConnect?.Host
                        ) return;
                }
            }

            var connect = new StoreConnectToDb();
            if (DbType == 0)
            {
                connect.Db = 0;
                connect.ConnectionString = Config.PathToDbDefault;
                PostgresConnect = null;
            }

            if (DbType == 1)
            {
                connect.Db = 1;
                connect.ConnectionString = $"Host={PostgresConnect?.Host ?? ""};Port={PostgresConnect?.Port};Database={PostgresConnect?.Database ?? ""};Username={PostgresConnect?.Username ?? ""};Password={PostgresConnect?.Password ?? ""};";

                connect.PostgresConnect = PostgresConnect;
            }

            var config = JsonSerializer.Serialize(connect);

            if (File.Exists(Config.PathToConfig)) File.Delete(Config.PathToConfig);
            File.WriteAllText(Config.PathToConfig, config);

            Config.ConnectionString = connect.ConnectionString ?? Config.PathToDbDefault;
            Config.DbType = connect.Db;
            IsConfigChanged = true;
        }


        private bool IsConfigChanged { get; set; } = false;

        public int? DbType
        {
            get { return GetProperty(() => DbType); }
            set { SetProperty(() => DbType, value); }
        }

        public PostgresConnect PostgresConnect
        {
            get { return GetProperty(() => PostgresConnect); }
            set { SetProperty(() => PostgresConnect, value); }
        }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }

        public int Timeout
        {
            get { return GetProperty(() => Timeout); }
            set { SetProperty(() => Timeout, value); }
        }

        public bool IsWaitIndicatorVisible
        {
            get { return GetProperty(() => IsWaitIndicatorVisible); }
            set { SetProperty(() => IsWaitIndicatorVisible, value); }
        }
    }
}
