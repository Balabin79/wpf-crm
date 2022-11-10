using Dental.Models.Base;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.Services
{
    public class TableRowSerializer<T> where T : AbstractBaseModel
    {
        public void Run(object p)
        {
            try
            {
                var table = (typeof(T).CustomAttributes.Where(f => f.AttributeType?.Name == "TableAttribute")?.
                    FirstOrDefault()).ConstructorArguments?.FirstOrDefault().Value.ToString();

                var path = Path.Combine(Config.PathToDataTablesDirectory, table);
                if (Directory.Exists(path) && p is AbstractBaseModel model)
                {
                    string pathToFile = Path.Combine(path, model.Guid);
                    if (p is T m) 
                    {
                        // сущ-щий файл необходимо удалять, а не перезаписывать в него, чтобы обновить метаданные директории
                        //if (File.Exists(pathToFile)) File.Delete(pathToFile); 
                        File.WriteAllText(pathToFile, JsonSerializer.Serialize(m));
                    }
                        
                                           
                }
            }
            catch (Exception e)
            {
                /*
                ThemedMessageBox.Show(
                    title: "Ошибка",
                    text: "При сохранении значения в базу данных произошла ошибка!",
                    messageBoxButtons: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error
                );*/
            }
        }
    }
}
