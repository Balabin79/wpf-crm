using Dental.Enums;
using Dental.Infrastructures.Collection;
using Dental.Infrastructures.Logs;
using Dental.Interfaces;
using Dental.Models;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dental.Repositories
{
    class EmployeeStatusRepository
    {
        public static Action<(EmployeeStatus, TableView)> AddModel;
        public static Action<EmployeeStatus> DeleteModel;
        public static Action<(EmployeeStatus, TableView)> UpdateModel;
        public static Action<(EmployeeStatus, TableView)> CopyModel;


        public static async Task<ObservableCollection<EmployeeStatus>> GetAll()
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
        
        public static void Add(TableView table)
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
      
        public static void Update(TableView table)
        {
            try
            {
                EmployeeStatus model = (EmployeeStatus)table.FocusedRow;
                using (ApplicationContext db = new ApplicationContext())
                {
                    EmployeeStatus item = db.EmployeeStatuses.Where(i => i.Id == model.Id).FirstOrDefault();
                    if (model == null || item == null) return;

                    if (item.Name != model.Name)
                    {
                        if (!new ConfirUpdateInCollection().run() && UpdateModel != null)
                        {
                            UpdateModel((item, table));
                            return;
                        } 
                        else
                        {
                            item.Name = model.Name;
                            item.Description = model.Description;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    if (UpdateModel != null) UpdateModel((item, table));
                }
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

        public static void Delete(TableView table)
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
       
        public static void Copy(TableView table)
        {
            try
            {
                EmployeeStatus model = (EmployeeStatus)table.FocusedRow;
                var db = new ApplicationContext();
                EmployeeStatus item = db.EmployeeStatuses.Where(i => i.Id == model.Id).FirstOrDefault();

                if (model == null || !new ConfirCopyInCollection().run() || CopyModel == null || item == null)
                {
                    CopyModel((item, table));
                    return;
                }
                else
                {
                    EmployeeStatus newModel = new EmployeeStatus()
                    {
                        Name = item.Name + " Копия",
                        Description = item.Description
                    };
                    db.EmployeeStatuses.Add(newModel);
                    db.SaveChanges();
                    if (CopyModel != null) 
                    {
                        CopyModel((newModel, table));
                    }                   
                }
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

    }
}
