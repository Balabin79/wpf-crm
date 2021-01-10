using Dental.Models.Template;
using Dental.Models;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System;

namespace Dental.Repositories.Template
{
    class DiaryRepository
    {
        public static ObservableCollection<Diary> Diary { get => GetDiaries(); }

        public static ObservableCollection<Diary> GetDiaries()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Diaries.Load();
                    return db.Diaries.Local;
                }
            }
            catch (Exception e)
            {
                return new ObservableCollection<Diary>();
            }
        }
    }
}
