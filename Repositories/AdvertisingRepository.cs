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
    class AdvertisingRepository : AbstractTableViewActionRepository
    {
        public async Task<ObservableCollection<Advertising>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.Advertising.OrderBy(d => d.Name).LoadAsync();
                return db.Advertising.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<Advertising>();
            }
        }
        
        public void Add(TableView table)
        {
            try
            {
                if (!new ConfirmAddNewInCollection().run()) return;

                Advertising item = new Advertising() {Name = "Новый элемент", Description = "" };

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Advertising.Add(item);
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
                Advertising model = (Advertising)table.FocusedRow;
                using (ApplicationContext db = new ApplicationContext())
                {
                    Advertising item = db.Advertising.Where(i => i.Id == model.Id).FirstOrDefault();
                                                         
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
                var model = table.FocusedRow as Advertising;
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
                Advertising model = (Advertising)table.FocusedRow;
                var db = new ApplicationContext();
                Advertising item = db.Advertising.Where(i => i.Id == model.Id).FirstOrDefault();

                if (!new ConfirCopyInCollection().run())
                {
                    CopyModel?.Invoke((item, table));
                    return;
                }
                Advertising newModel = new Advertising() {Name = model.Name + " Копия", Description  = model.Description };
                db.Advertising.Add(newModel);
                db.SaveChanges();
                Advertising newItem = db.Advertising.Where(i => i.Id == newModel.Id).FirstOrDefault();
                CopyModel?.Invoke((newItem, table));              
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

    }
}
