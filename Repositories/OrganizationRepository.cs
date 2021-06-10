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
    class OrganizationRepository 
    {
        public async Task<ObservableCollection<Organization>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.Organizations.OrderBy(d => d.Name).LoadAsync();
                return db.Organizations.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<Organization>();
            }
        }
        
        public void Add(TableView table)
        {
            try
            {
                if (!new ConfirmAddNewInCollection().run()) return;

                Organization item = new Organization();

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Organizations.Add(item);
                    db.SaveChanges();
                    //AddModel?.Invoke((item, table));
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
                Organization model = (Organization)table.FocusedRow;
                using (ApplicationContext db = new ApplicationContext())
                {
                    Organization item = db.Organizations.Where(i => i.Id == model.Id).FirstOrDefault();
                    if (model == null || item == null) return;


                    PropertyInfo[] properties = typeof(Organization).GetProperties();

                    bool needUpdate = false;
                    foreach (PropertyInfo property in properties)
                    {
                        if (! model[property, item]) needUpdate = true;                                                     
                    }    
                    
                    if (!needUpdate || !new ConfirUpdateInCollection().run())
                    {
                        // UpdateModel?.Invoke((item, table));
                         return;
                    }
                    item.Copy(model);
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();

                  //  UpdateModel?.Invoke((item, table));
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
                var model = table.FocusedRow as Organization;
                if (!new ConfirDeleteInCollection().run((int)TypeItem.File)) return;

                var db = new ApplicationContext();
                var row = db.Organizations.Where(d => d.Id == model.Id).FirstOrDefault();
                if (row != null) db.Entry(row).State = EntityState.Deleted;
                db.SaveChanges();
                //DeleteModel?.Invoke(model);
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
                Organization model = (Organization)table.FocusedRow;
                var db = new ApplicationContext();
                Organization item = db.Organizations.Where(i => i.Id == model.Id).FirstOrDefault();

                if (!new ConfirCopyInCollection().run())
                {
                   /// CopyModel?.Invoke((item, table));
                    return;
                }

                Organization newModel = new Organization();
                newModel.Copy(model);
                newModel.Name += " Копия";
                db.Organizations.Add(newModel);
                db.SaveChanges();
                //CopyModel?.Invoke((newModel, table));                               
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }
    }
}
