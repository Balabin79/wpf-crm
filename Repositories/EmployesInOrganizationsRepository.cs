using Dental.Enums;
using Dental.Infrastructures.Collection;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Models.Base;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dental.Repositories
{
    class EmployesInOrganizationsRepository
    {
        public Action<(IModel, TableView)> AddModel;
        public Action<IModel> DeleteModel;
        public Action<(List<EmployesInOrganizations>, TableView)> UpdateModel;
        public Action<(IModel, TableView)> CopyModel;

        public async Task<ObservableCollection<EmployesInOrganizations>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.EmployesInOrganizations.OrderBy(d => d.Id).Include("Organization").Include("Employee").LoadAsync();
                return db.EmployesInOrganizations.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<EmployesInOrganizations>();
            }
        }
        
        public void Add(TableView table)
        {
            try
            {
                if (!new ConfirmAddNewInCollection().run()) return;

                EmployesInOrganizations item = new EmployesInOrganizations() {};

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.EmployesInOrganizations.Add(item);
                    db.SaveChanges();
                    if (AddModel != null)  AddModel((item, table));
                }
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
      
        public void Update(TableView table, object employee, object organizations)
        {
            try
            {
                if (employee == null || organizations == null) return;
                if (!int.TryParse(employee.ToString(), out int employeeId)) return;
                var list = (List<object>)organizations;
                var orgsIds = new List<int>();

                foreach (var i in list)
                {
                    orgsIds.Add((int)i);
                }
                
                using (ApplicationContext db = new ApplicationContext())
                {
                    var orgsIdsFromDb = db.EmployesInOrganizations.Where(i => i.EmployeeId == employeeId).
                        Select(i => i.OrganizationId).ToList();
                    
                    var noDuplicateValues = orgsIds.Except(orgsIdsFromDb).ToList();

                    if (noDuplicateValues.Count == 0 || !new ConfirUpdateInCollection().run())
                    {
                        return;
                    }

                    List<EmployesInOrganizations> collection = new List<EmployesInOrganizations>();
                    foreach(var i in noDuplicateValues)
                    {
                        var ob = new EmployesInOrganizations() { EmployeeId = employeeId, OrganizationId = i };              
                        db.EmployesInOrganizations.Add(ob);
                        db.SaveChanges();
                        var last = db.EmployesInOrganizations.Where(v => v.Id == ob.Id).Include("Organization").Include("Employee").First();
                        collection.Add(last);
                    }
                    // эл-т, который создается при создании первой позиции.
                    db.EmployesInOrganizations.Where(v => v.EmployeeId == null).ToList().ForEach(v => db.EmployesInOrganizations.Remove(v));

                    UpdateModel?.Invoke((collection, table));
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
                var model = table.FocusedRow as EmployesInOrganizations;
                if (model == null || !new ConfirDeleteInCollection().run((int)TypeItem.File)) return;

                var db = new ApplicationContext();
                var row = db.EmployesInOrganizations.Where(d => d.Id == model.Id).FirstOrDefault();
                    db.Entry(row).State = EntityState.Deleted;
                db.SaveChanges();
                DeleteModel?.Invoke(model);
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
       
        public ObservableCollection<EmployesInOrganizations> GetEmployeeOrgs(object EmployeeId)
        {
            try
            {
                var _EmployeeId = (int)EmployeeId;
                ApplicationContext db = new ApplicationContext();
                db.EmployesInOrganizations.Where(i => i.EmployeeId == _EmployeeId).OrderBy(d => d.Id).Include("Organization").LoadAsync();
                return db.EmployesInOrganizations.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<EmployesInOrganizations>();
            }
        }

    }
}
