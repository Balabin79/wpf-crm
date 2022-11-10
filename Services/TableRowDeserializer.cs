using Dental.Models;
using Dental.Models.Base;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.Services
{
    public class TableRowDeserializer<T> where T : AbstractBaseModel
    {
        public void Run(string table, DbSet<T>context)
        {
            try
            {
                var path = Path.Combine(Config.PathToDataTablesDirectory, table);
                if (Directory.Exists(path))
                {                   
                    using (var db = new ApplicationContext())
                    {
                        var dateDir = Directory.GetLastWriteTime(path);
                        var row = db.TablesSync.FirstOrDefault(f => f.Table == table);
                        if (DateTime.TryParse(row?.Date?.ToString(), out DateTime dateRow))
                        {
                            // отбираем все файлы, в которые данные записывались уже после синхронизации
                            var files = new DirectoryInfo(path).GetFileSystemInfos().Where(f => f.LastWriteTime >= dateRow).ToArray();

                                
                                foreach (var i in files)
                                {
                                //отбираем в списке файлов дату последнего сохраненного
                                if (dateRow < i.LastWriteTime) dateRow = i.LastWriteTime;

                                var file = new FileInfo(i.FullName);

                                // получаем объект
                                var ob = JsonSerializer.Deserialize(file.OpenRead(), typeof(T));
                                    
                                // десериализуем файл и записываем его в бд (если Guid су-щий, то перезапсь, иначе добавляем), присваиваеи по кастомным атрибутам

                                }
                            row.Date = dateRow.ToString();
                        }
                            
                        
                        //var timestampDir = (int)DateTime.UtcNow.Subtract(dateDir).TotalSeconds;


                        else
                        {
                            //  добавить дату последнего
                        }
                    }
                }
               
                
                
              /*  table = (typeof(T).CustomAttributes.Where(f => f.AttributeType?.Name == "TableAttribute")?.
                    FirstOrDefault()).ConstructorArguments?.FirstOrDefault().Value.ToString();*/

            }
            catch (Exception e)
            {
              /*  ThemedMessageBox.Show(
                    title: "Ошибка",
                    text: "При сохранении значения в базу данных произошла ошибка!",
                    messageBoxButtons: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error
                );*/
            }
        }
    }
}
