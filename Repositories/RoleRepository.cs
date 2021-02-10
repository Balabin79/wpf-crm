using Dental.Enums;
using Dental.Infrastructures.Collection;
using Dental.Infrastructures.Logs;
using Dental.Models;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dental.Repositories
{
    class RoleRepository
    {
        public static Action<(Role, TableView)> AddModel;
        public static Action<Role> DeleteModel;
        public static Action<(Role, TableView)> UpdateModel;
        public static Action<(Role, TableView)> CopyModel;


        public static async Task<ObservableCollection<Role>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.Roles.OrderBy(d => d.Name).LoadAsync();
                return db.Roles.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<Role>();
            }
        }
        
        public static void Add(TableView table)
        {
            try
            {
                Role model = (Role)table.FocusedRow;

                if (model == null || !new ConfirmAddNewInCollection().run()) return;

                Role item = new Role() {Name = "Новый элемент", Description = "" };

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Roles.Add(item);
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
                Role model = (Role)table.FocusedRow;
                using (ApplicationContext db = new ApplicationContext())
                {
                    Role item = db.Roles.Where(i => i.Id == model.Id).FirstOrDefault();
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
                var model = table.FocusedRow as Role;
                if (model == null || !new ConfirDeleteInCollection().run((int)TypeItem.File)) return;

                var db = new ApplicationContext();
                var row = db.Roles.Where(d => d.Id == model.Id).FirstOrDefault();
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
                Role model = (Role)table.FocusedRow;
                var db = new ApplicationContext();
                Role item = db.Roles.Where(i => i.Id == model.Id).FirstOrDefault();

                if (model == null || !new ConfirCopyInCollection().run() || CopyModel == null || item == null)
                {
                    CopyModel((item, table));
                    return;
                }
                else
                {
                    Role newModel = new Role()
                    {
                        Name = item.Name + " Копия",
                        Description = item.Description
                    };
                    db.Roles.Add(newModel);
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
