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
    class DiscountGroupsRepository : AbstractTableViewActionRepository
    {
        public async Task<ObservableCollection<DiscountGroups>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.DiscountGroups.OrderBy(d => d.Name).LoadAsync();
                return db.DiscountGroups.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<DiscountGroups>();
            }
        }
        
        public void Add(TableView table)
        {
            try
            {
                if (!new ConfirmAddNewInCollection().run()) return;

                DiscountGroups item = new DiscountGroups() {Name = "Новый элемент", Description = "", AmountDiscount = 0 };

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.DiscountGroups.Add(item);
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
                DiscountGroups model = (DiscountGroups)table.FocusedRow;
                using (ApplicationContext db = new ApplicationContext())
                {
                    DiscountGroups item = db.DiscountGroups.Where(i => i.Id == model.Id).FirstOrDefault();
                                                         
                    PropertyInfo[] properties = typeof(DiscountGroups).GetProperties();

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
                var model = table.FocusedRow as DiscountGroups;
                if (model == null || !new ConfirDeleteInCollection().run((int)TypeItem.File)) return;

                var db = new ApplicationContext();
                var row = db.DiscountGroups.Where(d => d.Id == model.Id).FirstOrDefault();
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
                DiscountGroups model = (DiscountGroups)table.FocusedRow;
                var db = new ApplicationContext();
                DiscountGroups item = db.DiscountGroups.Where(i => i.Id == model.Id).FirstOrDefault();

                if (!new ConfirCopyInCollection().run())
                {
                    CopyModel?.Invoke((item, table));
                    return;
                }
                DiscountGroups newModel = new DiscountGroups() {Name = model.Name + " Копия", Description  = model.Description, 
                    AmountDiscount = model.AmountDiscount, DiscountGroupType = model.DiscountGroupType };
                db.DiscountGroups.Add(newModel);
                db.SaveChanges();
                DiscountGroups newItem = db.DiscountGroups.Where(i => i.Id == newModel.Id).FirstOrDefault();
                CopyModel?.Invoke((newItem, table));              
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

    }
}
