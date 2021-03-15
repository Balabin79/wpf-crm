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
    class EmployesSpecialitiesRepository
    {
        public Action<(IModel, TableView)> AddModel;
        public Action<IModel> DeleteModel;
        public Action<(List<EmployesSpecialities>, TableView)> UpdateModel;
        public Action<(IModel, TableView)> CopyModel;

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
                    if (AddModel != null) 
                        AddModel((item, table));
                }
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
      
        public void Update(TableView table, object employee, object specialities)
        {
            try
            {
                if (employee == null || specialities == null) return;
                if (!int.TryParse(employee.ToString(), out int employeeId)) return;
                var list = (List<object>)specialities;
                var specialitiesIds = new List<int>();

                foreach (var i in list)
                {
                    specialitiesIds.Add((int)i);
                }
                
                using (ApplicationContext db = new ApplicationContext())
                {
                    var specialitiesIdsFromDb = db.EmployesSpecialities.Where(i => i.EmployeeId == employeeId).
                        Select(i => i.SpecialityId).ToList();
                    
                    var noDuplicateValues = specialitiesIds.Except(specialitiesIdsFromDb).ToList();

                    if (noDuplicateValues.Count == 0 || !new ConfirUpdateInCollection().run())
                    {
                        return;
                    }

                    List<EmployesSpecialities> collection = new List<EmployesSpecialities>();
                    foreach(var i in noDuplicateValues)
                    {
                        var ob = new EmployesSpecialities() { EmployeeId = employeeId, SpecialityId = i };              
                        db.EmployesSpecialities.Add(ob);
                        db.SaveChanges();
                        var last = db.EmployesSpecialities.Where(v => v.Id == ob.Id).Include("Speciality").Include("Employee").First();
                        collection.Add(last);
                    }
                    // эл-т, который создается при создании первой позиции.
                    db.EmployesSpecialities.Where(v => v.EmployeeId == null).ToList().ForEach(v => db.EmployesSpecialities.Remove(v));

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
                var model = table.FocusedRow as EmployesSpecialities;
                if (model == null || !new ConfirDeleteInCollection().run((int)TypeItem.File)) return;

                var db = new ApplicationContext();
                var row = db.EmployesSpecialities.Where(d => d.Id == model.Id).FirstOrDefault();
                    db.Entry(row).State = EntityState.Deleted;
                db.SaveChanges();
                DeleteModel?.Invoke(model);
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
       
        public ObservableCollection<EmployesSpecialities> GetEmployeeSpecialities(object EmployeeId)
        {
            try
            {
                var _EmployeeId = (int)EmployeeId;
                ApplicationContext db = new ApplicationContext();
                db.EmployesSpecialities.Where(i => i.EmployeeId == _EmployeeId).OrderBy(d => d.Id).Include("Speciality").LoadAsync();
                return db.EmployesSpecialities.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<EmployesSpecialities>();
            }
        }

    }
}
