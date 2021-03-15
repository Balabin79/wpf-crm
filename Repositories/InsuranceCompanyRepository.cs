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
    class InsuranceCompanyRepository : AbstractTableViewActionRepository
    {
        public async Task<ObservableCollection<InsuranceCompany>> GetAll()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                await db.InsuranceCompany.OrderBy(d => d.Name).LoadAsync();
                return db.InsuranceCompany.Local;
            }
            catch (Exception e)
            {
                new RepositoryLog(e).run();
                return new ObservableCollection<InsuranceCompany>();
            }
        }
        
        public void Add(TableView table)
        {
            try
            {
                if (!new ConfirmAddNewInCollection().run()) return;

                InsuranceCompany item = new InsuranceCompany();

                using (ApplicationContext db = new ApplicationContext())
                {
                    db.InsuranceCompany.Add(item);
                    db.SaveChanges();
                    AddModel?.Invoke((item, table));
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
                InsuranceCompany model = (InsuranceCompany)table.FocusedRow;
                using (ApplicationContext db = new ApplicationContext())
                {
                    InsuranceCompany item = db.InsuranceCompany.Where(i => i.Id == model.Id).FirstOrDefault();
                    if (model == null || item == null) return;


                    PropertyInfo[] properties = typeof(InsuranceCompany).GetProperties();

                    bool needUpdate = false;
                    foreach (PropertyInfo property in properties)
                    {
                        if (! model[property, item]) needUpdate = true;                                                     
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
                var model = table.FocusedRow as InsuranceCompany;
                if (!new ConfirDeleteInCollection().run((int)TypeItem.File)) return;

                var db = new ApplicationContext();
                var row = db.InsuranceCompany.Where(d => d.Id == model.Id).FirstOrDefault();
                if (row != null) db.Entry(row).State = EntityState.Deleted;
                db.SaveChanges();
                DeleteModel?.Invoke(model);
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
                InsuranceCompany model = (InsuranceCompany)table.FocusedRow;
                var db = new ApplicationContext();
                InsuranceCompany item = db.InsuranceCompany.Where(i => i.Id == model.Id).FirstOrDefault();

                if (!new ConfirCopyInCollection().run())
                {
                    CopyModel?.Invoke((item, table));
                    return;
                }

                InsuranceCompany newModel = new InsuranceCompany();
                newModel.Copy(model);
                newModel.Name += " Копия";
                db.InsuranceCompany.Add(newModel);
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
