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
    class LoyaltyProgramsRepository : AbstractTableViewActionRepository
    {
        public async Task<ObservableCollection<LoyaltyPrograms>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.LoyaltyPrograms.OrderBy(d => d.Name).LoadAsync();
                return db.LoyaltyPrograms.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<LoyaltyPrograms>();
            }
        }
        
        public void Add(TableView table)
        {
            try
            {
                if (!new ConfirmAddNewInCollection().run()) return;

                LoyaltyPrograms item = new LoyaltyPrograms() {Name = "Новый элемент", Description = "", 
                    PeriodTo = DateTime.Now.ToShortDateString().ToString() };

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.LoyaltyPrograms.Add(item);
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
                LoyaltyPrograms model = (LoyaltyPrograms)table.FocusedRow;
                using (ApplicationContext db = new ApplicationContext())
                {
                    LoyaltyPrograms item = db.LoyaltyPrograms.Where(i => i.Id == model.Id).FirstOrDefault();
                                                         
                    PropertyInfo[] properties = typeof(LoyaltyPrograms).GetProperties();

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
                var model = table.FocusedRow as LoyaltyPrograms;
                if (model == null || !new ConfirDeleteInCollection().run((int)TypeItem.File)) return;

                var db = new ApplicationContext();
                var row = db.LoyaltyPrograms.Where(d => d.Id == model.Id).FirstOrDefault();
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
                LoyaltyPrograms model = (LoyaltyPrograms)table.FocusedRow;
                var db = new ApplicationContext();
                LoyaltyPrograms item = db.LoyaltyPrograms.Where(i => i.Id == model.Id).FirstOrDefault();

                if (!new ConfirCopyInCollection().run())
                {
                    CopyModel?.Invoke((item, table));
                    return;
                }
                LoyaltyPrograms newModel = new LoyaltyPrograms() {Name = model.Name + " Копия", 
                    Description  = model.Description, PeriodTo = model.PeriodTo };
                db.LoyaltyPrograms.Add(newModel);
                db.SaveChanges();
                LoyaltyPrograms newItem = db.LoyaltyPrograms.Where(i => i.Id == newModel.Id).FirstOrDefault();
                CopyModel?.Invoke((newItem, table));              
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
            }
        }

    }
}
