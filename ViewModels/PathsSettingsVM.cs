﻿using Dental.Services;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.ViewModels
{
    public class PathsSettingsVM : DevExpress.Mvvm.ViewModelBase
    {
        public PathsSettingsVM()
        {
            Config = new Config();
            // считываем значения для местоположения бд и программных файлов
            if (Config.ConnectionString == null || Config.ConnectionString == Config.PathToDbDefault)
            {
                IsPathToDbDefault = true;
                PathToDb = null;
            }
            else
            {
                IsPathToDbDefault = false;
                PathToDb = Config.ConnectionString;
            }

            if (Config.PathToProgram == null || Config.PathToProgram == Config.defaultPath)
            {
                IsPathToProgramFilesDefault = true;
                PathToProgramFiles = null;
            }
            else
            {
                IsPathToProgramFilesDefault = false;
                PathToProgramFiles = Config.PathToProgram;
            }
        }

        public bool IsPathToDbDefault
        {
            get { return GetProperty(() => IsPathToDbDefault); }
            set { SetProperty(() => IsPathToDbDefault, value); }
        }

        public bool IsPathToProgramFilesDefault
        {
            get { return GetProperty(() => IsPathToProgramFilesDefault); }
            set { SetProperty(() => IsPathToProgramFilesDefault, value); }
        }

        public string PathToDb
        {
            get { return GetProperty(() => PathToDb); }
            set { SetProperty(() => PathToDb, value); }
        }

        public string PathToProgramFiles
        {
            get { return GetProperty(() => PathToProgramFiles); }
            set { SetProperty(() => PathToProgramFiles, value); }
        }

        [Command]
        public void Save(object p)
        {
            try
            {
                // если какая-то из галочек снята
                if (!IsPathToDbDefault || !IsPathToProgramFilesDefault)
                {
                    if (!IsPathToDbDefault)
                    {
                        if (string.IsNullOrEmpty(PathToDb?.Trim()))
                        {
                            ThemedMessageBox.Show(title: "Внимание!", text: "Поле \"Путь к базе данных\" не заполнено! Поставьте галочку \"По умолчанию\" или введите значение в поле",
                            messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Warning);
                            return;
                        }
                    }

                    if (!IsPathToProgramFilesDefault)
                    {
                        if (string.IsNullOrEmpty(PathToProgramFiles?.Trim()))
                        {
                            ThemedMessageBox.Show(title: "Внимание!", text: "Поле \"Путь к программным файлам\" не заполнено! Поставьте галочку \"По умолчанию\" или введите значение в поле",
                            messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Warning);
                            return;
                        }
                    } 
                }

                // если галочка проставлена то обнуляем тектовое поле и обнуляем поле в файле конфигурации
                if (IsPathToDbDefault) PathToDb = null;
                if (IsPathToProgramFilesDefault) PathToProgramFiles = null;

                var config = JsonSerializer.Serialize(new UserConfig()
                {
                    DBName = Config.defaultDBName,
                    ConnectionString = PathToDb,
                    PathToProgram = PathToProgramFiles
                });

                // File.WriteAllText("./dental.conf");
                if (File.Exists(Config.PathToConfig)) File.Delete(Config.PathToConfig);
                File.WriteAllText(Config.PathToConfig, config);

                Config.ConnectionString = PathToDb ?? Config.PathToDbDefault;
                Config.PathToProgram = PathToProgramFiles ?? Config.PathToProgramDirectory;

                if (p is Window win) win?.Close();             
            }
            catch (Exception e)
            {

            }
        }

        [Command]
        public void CloseApp(object p)
        {
            if (p is Window win) win?.Close();

            var response = ThemedMessageBox.Show(title: "Внимание", text: "Завершить работу с приложением?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
            if (response.ToString() == "No") return;

            Environment.Exit(0);
        }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }
    }
}