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
    class EmployeeRepository
    {
        public async Task<ObservableCollection<Employee>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.Employes.OrderBy(d => d.LastName).Include(b => b.EmployesInOrganizations)
                    .Include(b => b.EmployesSpecialities)
                    .Include("EmployesSpecialities.Speciality")
                    .Include("EmployesInOrganizations.Organization")
                    .LoadAsync();
                return db.Employes.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<Employee>();
            }
        }

        public void Open(TableView table)
        {
            try
            {
                if (!new ConfirmAddNewInCollection().run()) return;

                Employee item = new Employee();

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Employes.Add(item);
                    db.SaveChanges();

                }
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

        public void Save(Employee employee, bool isNew)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (isNew) 
                    {
                        db.Employes.Add(employee);
                        db.SaveChanges();

                        // показываем флеш
                        return;
                    }

                    Employee item = db.Employes.Where(i => i.Id == employee.Id).FirstOrDefault();
                    if (item == null) return;

                    PropertyInfo[] properties = typeof(Employee).GetProperties();

                    bool needUpdate = false;
                    foreach (PropertyInfo property in properties)
                    {
                        if (!employee[property, item]) needUpdate = true;
                    }

                    if (!needUpdate || !new ConfirUpdateInCollection().run())  return;

                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();

                    // показываем флеш
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
      
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
    }
}
