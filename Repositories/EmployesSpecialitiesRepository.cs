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
    class EmployesSpecialitiesRepository : AbstractTableViewActionRepository
    {
        public async Task<ObservableCollection<EmployesSpecialities>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.EmployesSpecialities.OrderBy(d => d.Id).Include("Speciality").Include("Employee").LoadAsync();
                return db.EmployesSpecialities.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<EmployesSpecialities>();
            }
        }
        
        public void Add(TableView table)
        {
            try
            {
                if (!new ConfirmAddNewInCollection().run()) return;

                EmployesSpecialities item = new EmployesSpecialities() {};

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.EmployesSpecialities.Add(item);
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
                EmployesSpecialities model = (EmployesSpecialities)table.FocusedRow;
                using (ApplicationContext db = new ApplicationContext())
                {
                    EmployesSpecialities item = db.EmployesSpecialities.Where(i => i.Id == model.Id).FirstOrDefault();
                                                         
                    PropertyInfo[] properties = typeof(Advertising).GetProperties();

                    if (model == null || item == null) return;

                    bool needUpdate = false;
                    foreach (PropertyInfo property in properties)
                    {
                        if (!model[property, item]) needUpdate = true;
                    }

                    if (!needUpdate || !new ConfirUpdateInCollection().run())
                    {
                        UpdateModel?.Invoke((item, table));
                        return;
                    }
                    item.Copy(model);
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
                var model = table.FocusedRow as EmployesSpecialities;
                if (model == null || !new ConfirDeleteInCollection().run((int)TypeItem.File)) return;

                var db = new ApplicationContext();
                var row = db.Advertising.Where(d => d.Id == model.Id).FirstOrDefault();
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
                EmployesSpecialities model = (EmployesSpecialities)table.FocusedRow;
                var db = new ApplicationContext();
                EmployesSpecialities item = db.EmployesSpecialities.Where(i => i.Id == model.Id).FirstOrDefault();

                if (!new ConfirCopyInCollection().run())
                {
                    CopyModel?.Invoke((item, table));
                    return;
                }
                EmployesSpecialities newModel = new EmployesSpecialities() {EmployeeId = model.EmployeeId, SpecialityId  = model.SpecialityId };
                db.EmployesSpecialities.Add(newModel);
                db.SaveChanges();
                EmployesSpecialities newItem = db.EmployesSpecialities.Where(i => i.Id == newModel.Id).FirstOrDefault();
                CopyModel?.Invoke((newItem, table));              
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
        
    }
}
