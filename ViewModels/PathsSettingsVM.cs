using Dental.Services;
using DevExpress.Mvvm.DataAnnotations;
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
            // считываем значения для местоположения бд и программных файлов
            if (Config.ConnectionString == Path.Combine(Config.PathToDbDefault))
            {
                IsPathToDbDefault = true;
                PathToDb = null;
            }
            else
            {
                IsPathToDbDefault = false;
                PathToDb = Config.ConnectionString;
            }

            if (Config.ConnectionString == Path.Combine(Config.PathToDbDefault))
            {
                IsPathToProgramFilesDefault = true;
                PathToProgramFiles = null;
            }
            else
            {
                IsPathToProgramFilesDefault = false;
                PathToProgramFiles = Config.PathToProgramDirectory;
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

                if (IsPathToProgramFilesDefault == false)
                {
                    if (string.IsNullOrEmpty(PathToProgramFiles?.Trim()))
                    {
                        ThemedMessageBox.Show(title: "Внимание!", text: "Поле \"Путь к программным файлам\" не заполнено! Поставьте галочку \"По умолчанию\" или введите значение в поле",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Warning);
                        return;
                    }
                }

                //если пути изменены по сравнению со старой версией, то 
    
                var dBName = Config.defaultDBName;
                var connectionString = PathToDb ?? Config.PathToDbDefault;
                var pathToProgram = PathToProgramFiles ?? Config.PathToProgramDirectory;

                var config = JsonSerializer.Serialize(new UserConfig()
                {
                    DBName = dBName,
                    ConnectionString = connectionString,
                    PathToProgram = pathToProgram
                });
                File.WriteAllText("./dental.conf", config);

                Config.ConnectionString = connectionString;
                Config.PathToProgram = pathToProgram;

                // var json = File.ReadAllText("./dental.conf").Trim();
            }
        }

    }
}
