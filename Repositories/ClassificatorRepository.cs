using Dental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Repositories
{
    public static class ClassificatorRepository
    {

        public static List<Classificator> ClassificatorCategories { get => GetClassificatorCategories();  }

        public static List<Classificator> GetClassificatorCategories()
        {

                 return new ApplicationContext().Classificator.Where(i => i.IsDir == 1).ToList();
        }

        public static List<Classificator> GetClassificatorItemsCategory(int category)
        {
            using (var db = new ApplicationContext())
            {
                return db.Classificator.Where(i => i.IsDir == 0 && i.ParentId == category).ToList();
            }
        }


    }
}
