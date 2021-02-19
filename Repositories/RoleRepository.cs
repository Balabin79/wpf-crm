using Dental.Enums;
using Dental.Infrastructures.Collection;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dental.Repositories
{
    class RoleRepository
    {
        public static Action<(IModel, TableView)> AddModel;
        public static Action<IModel> DeleteModel;
        public static Action<(IModel, TableView)> UpdateModel;
        public static Action<(IModel, TableView)> CopyModel;
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
                if (!new ConfirmAddNewInCollection().run()) return;

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
                                                         
                    PropertyInfo[] properties = typeof(Role).GetProperties();

                    if (model == null || item == null) return;

                    bool needUpdate = false;
                    foreach (PropertyInfo property in properties)
                    {
                        if (!model[property, item]) needUpdate = true;
                    }

                    if (!needUpdate)
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

                if (!new ConfirCopyInCollection().run())
                {
                    CopyModel?.Invoke((item, table));
                    return;
                }
                Role newModel = new Role();
                newModel.Copy(model);
                newModel.Name += " Копия";
                db.Roles.Add(newModel);
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
