using Dental.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Infrastructures.Collection
{
    class AddInCollection
    {
        public int run(ITreeViewCollection model)
        {
           /* if (model.Id == model.ParentId && model.Dir == 0)
            {
                // это корневой файл
                // показываем форму с выбором корневой и обычной папки и файла
                int x = 0;
                return 0;
            }
            if (model.Id == model.ParentId && model.Dir == 1)
            {
                // это корневая папка 
                // показываем форму с выбором корневой и обычной папки и файла
                int x = 0;
                return 0;
            }

            else if (model.Dir == 1 && model.ParentId != model.Id)
            {
                // это вложенная папка, 
                // показываем форму с выбором файла и обычной папки, присваиваем записи ParentId = текущему Id
                int x = 0;
                return 0;
            }


            if (model.Dir == 0 && model.ParentId != 0)
            {
                // это файл в папке
                // показываем форму с выбором файла и обычной папки, присваиваем записи ParentId = текущему ParentId
                int x = 0;
                return 0;
            }*/


            return 0;
        }
    }
}
