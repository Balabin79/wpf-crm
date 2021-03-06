using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dental.Enums;
using Dental.Infrastructures.Collection;
using Dental.Infrastructures.Logs;
using Dental.Interfaces;
using Dental.Models;
using DevExpress.Xpf.Grid;

namespace Dental.Repositories
{
    class EmployeeRepository : AbstractTableViewActionRepository
    {
        public async Task<ObservableCollection<Employee>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.Employes.OrderBy(d => d.LastName).LoadAsync();
                return db.Employes.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<Employee>();
            }
        }

        public void Add(TableView table)
        {
            try
            {
                if (!new ConfirmAddNewInCollection().run()) return;

                Employee item = new Employee();

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Employes.Add(item);
                    db.SaveChanges();
                    AddModel?.Invoke((item, table));
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
                Employee model = (Employee)table.FocusedRow;
                using (ApplicationContext db = new ApplicationContext())
                {
                    Employee item = db.Employes.Where(i => i.Id == model.Id).FirstOrDefault();
                    if (model == null || item == null) return;


                    PropertyInfo[] properties = typeof(Employee).GetProperties();

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
                var model = table.FocusedRow as Employee;
                if (!new ConfirDeleteInCollection().run((int)TypeItem.File)) return;

                var db = new ApplicationContext();
                var row = db.Employes.Where(d => d.Id == model.Id).FirstOrDefault();
                if (row != null) db.Entry(row).State = EntityState.Deleted;
                db.SaveChanges();
                DeleteModel?.Invoke(model);
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
                Employee model = (Employee)table.FocusedRow;
                var db = new ApplicationContext();
                Employee item = db.Employes.Where(i => i.Id == model.Id).FirstOrDefault();

                if (!new ConfirCopyInCollection().run())
                {
                    CopyModel?.Invoke((item, table));
                    return;
                }

                Employee newModel = new Employee();
                newModel.Copy(model);
                newModel.LastName += " Копия";
                db.Employes.Add(newModel);
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
