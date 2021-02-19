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
    class EmployeeStatusRepository : AbstractTableViewActionRepository
    { 
        public  async Task<ObservableCollection<EmployeeStatus>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.EmployeeStatuses.OrderBy(d => d.Name).LoadAsync();
                return db.EmployeeStatuses.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<EmployeeStatus>();
            }
        }
        
        public void Add(TableView table)
        {
            try
            {
                if (!new ConfirmAddNewInCollection().run()) return;

                EmployeeStatus item = new EmployeeStatus() {Name = "Новый элемент", Description = "" };

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.EmployeeStatuses.Add(item);
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
                EmployeeStatus model = (EmployeeStatus)table.FocusedRow;
                using (ApplicationContext db = new ApplicationContext())
                {
                    EmployeeStatus item = db.EmployeeStatuses.Where(i => i.Id == model.Id).FirstOrDefault();

                    PropertyInfo[] properties = typeof(EmployeeStatus).GetProperties();

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
                var model = table.FocusedRow as EmployeeStatus;
                if (model == null || !new ConfirDeleteInCollection().run((int)TypeItem.File)) return;

                var db = new ApplicationContext();
                var row = db.EmployeeStatuses.Where(d => d.Id == model.Id).FirstOrDefault();
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
                EmployeeStatus model = (EmployeeStatus)table.FocusedRow;
                var db = new ApplicationContext();
                EmployeeStatus item = db.EmployeeStatuses.Where(i => i.Id == model.Id).FirstOrDefault();

                if (!new ConfirCopyInCollection().run())
                {
                    CopyModel?.Invoke((item, table));
                    return;
                }
                EmployeeStatus newModel = new EmployeeStatus();
                newModel.Copy(model);
                newModel.Name += " Копия";
                db.EmployeeStatuses.Add(newModel);
                db.SaveChanges();
                CopyModel?.Invoke((newModel, table));
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

    }
}
