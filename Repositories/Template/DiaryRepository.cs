using Dental.Models.Template;
using Dental.Models;
using System.Collections.ObjectModel;
using System.Data.Entity;

namespace Dental.Repositories.Template
{
    class DiaryRepository
    {

        public static ObservableCollection<Diary> Diary { get => GetDiaries(); }

        public static ObservableCollection<Diary> GetDiaries()
        {
            ApplicationContext db = new ApplicationContext();
            db.Diaries.Load();
            var x = db.Diaries.Local;
            return x;
        }
    }
}
