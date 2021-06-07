using Dental.Enums;
using Dental.Infrastructures.Collection;
using Dental.Infrastructures.Logs;
using Dental.Models;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dental.Repositories
{
    class SpecialityRepository : AbstractTableViewActionRepository
    {
        public async Task<ObservableCollection<Speciality>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.Specialities.OrderBy(d => d.Name).LoadAsync();
                return db.Specialities.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<Speciality>();
            }
        }
        
        public void Add(TableView table)
        {
            try
            {
                if (!new ConfirmAddNewInCollection().run()) return;

                Speciality item = new Speciality() {Name = "Новый элемент", ShowInShedule = 0 };

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Specialities.Add(item);
                    db.SaveChanges();
                    if (AddModel != null) AddModel((item, table));
                }
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
      
        public void Update(TableView table)
        {
            try
            {
                Speciality model = (Speciality)table.FocusedRow;
                using (ApplicationContext db = new ApplicationContext())
                {
                    Speciality item = db.Specialities.Where(i => i.Id == model.Id).FirstOrDefault();
                                                         
                    PropertyInfo[] properties = typeof(Speciality).GetProperties();

                    if (model == null || item == null) return;

                    bool needUpdate = false;


                    if (!needUpdate || !new ConfirUpdateInCollection().run())
                    {
                        UpdateModel?.Invoke((item, table));
                        return;
                    }
              
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();

                    UpdateModel?.Invoke((item, table));
                }
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

        public void Delete(TableView table)
        {
            try
            {
                var model = table.FocusedRow as Speciality;
                if (model == null || !new ConfirDeleteInCollection().run((int)TypeItem.File)) return;

                var db = new ApplicationContext();
                var row = db.Specialities.Where(d => d.Id == model.Id).FirstOrDefault();
                if (row != null) db.Entry(row).State = EntityState.Deleted;
                db.SaveChanges();
                DeleteModel(model);
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
       
        public void Copy(TableView table)
        {
            try
            {
                Speciality model = (Speciality)table.FocusedRow;
                var db = new ApplicationContext();
                Speciality item = db.Specialities.Where(i => i.Id == model.Id).FirstOrDefault();

                if (!new ConfirCopyInCollection().run())
                {
                    CopyModel?.Invoke((item, table));
                    return;
                }
                Speciality newModel = new Speciality() {Name = model.Name + " Копия", ShowInShedule = model.ShowInShedule };
                db.Specialities.Add(newModel);
                db.SaveChanges();
                Speciality newItem = db.Specialities.Where(i => i.Id == newModel.Id).FirstOrDefault();
                CopyModel?.Invoke((newItem, table));              
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
    }
}
